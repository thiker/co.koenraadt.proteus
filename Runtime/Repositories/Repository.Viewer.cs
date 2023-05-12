using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Routing;
using Packages.co.koenraadt.proteus.Runtime.Other;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Microsoft.Msagl.Layout.OverlapRemovalFixedSegments.OverlapRemovalFixedSegmentsMst;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{

    public class ViewersRepository
    {
        private static ViewersRepository _instance = null;

        private ObservableCollection<PTViewer> _ptViewers;


        public static ViewersRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewersRepository();
                    _instance.Init();
                }
                return _instance;
            }
        }

        private void Init()
        {
            _ptViewers = new();
        }


        /// <summary>
        /// Adds a PTViewer to the ViewersRepository.
        /// </summary>
        /// <param name="newViewer">The viewer data to add.</param>
        public void UpdateViewer(PTViewer newViewer)
        {
            bool changedRoot = false;
            PTViewer oldViewer = GetViewerById(newViewer.Id);

            // Check if root has changed
            if (newViewer?.RootNodeId != null)
            {
                changedRoot = newViewer?.RootNodeId != oldViewer?.RootNodeId;
            }

            // If not already existing add the node
            if (oldViewer is null)
            {
                _ptViewers.Add(newViewer);
            }
            else
            {
                
                Helpers.CombineValues(oldViewer, newViewer);
            }

            if (changedRoot)
            {
                RegenerateViewerLayout(newViewer.Id);
            }
        }

        /// <summary>
        /// Get a PTViewer by its Id.
        /// </summary>
        /// <param name="id">the viewer's identifier.</param>
        /// <returns>The PTViewer with the respective Id</returns>
        public PTViewer GetViewerById(string id)
        {
            if (id != null && id != "")
            {
                PTViewer foundViewer = _ptViewers.FirstOrDefault(x => x.Id == id);
                return foundViewer;
            } else
            {
                return null;
            }

        }

        /// <summary>
        /// Get the collection of viewers.
        /// </summary>
        /// <returns>Collection of PTViewers</returns>
        public ObservableCollection<PTViewer> GetViewers()
        {
            return _ptViewers;

        }

        /// <summary> 
        /// Removes a viewer by its id
        /// </summary>
        /// <param name="id">the viewer's identifier</param>
        public void DeleteViewerById(string id)
        {
            PTViewer viewerToDelete = GetViewerById(id);
            if (viewerToDelete is not null)
            {
                int ix = _ptViewers.IndexOf(viewerToDelete);
                _ptViewers.RemoveAt(ix);
            }
        }

        /// <summary>
        /// Updates the rotation of the viewer.
        /// </summary>
        /// <param name="id">the id of the viewer to update. </param>
        /// <param name="rotation">the rotation of the viewer to update. </param>
        public void SetViewerRotation(string id, Quaternion rotation)
        {
            PTViewer viewer = GetViewerById(id);

            if (viewer != null)
            {
                viewer.Rotation = rotation;
            }
        }

        /// <summary>
        /// Updates the local position of the view window.
        /// </summary>
        /// <param name="id">the id of the viewer to update. </param>
        /// <param name="position">the local position of the view window </param>
        public void AddModelAnchorOffset(string id, Vector3 offset)
        {
            PTViewer viewer = GetViewerById(id);
            Vector3 newOffset;
            if (viewer?.ModelAnchorOffset != null)
            {
                newOffset = (Vector3)viewer.ModelAnchorOffset + offset;
            } else
            {
                newOffset = new Vector3(0, 0, 0);
            }

            PTViewer updatedViewer = new PTViewer() { Id = viewer.Id, ModelAnchorOffset = newOffset };
            UpdateViewer(updatedViewer);
        }

        public void RegenerateViewerLayouts()
        {
            foreach (PTViewer viewer in GetViewers())
            {
                RegenerateViewerLayout(viewer.Id);
            }
        }


        /// <summary>
        /// Regenerates the layout of a viewer.
        /// </summary>
        /// <param name="id">The id of the viewer to regenerate the layout for.</param>
        public void RegenerateViewerLayout(string id)
        {
            Debug.Log("PROTEUS: Started Generating Viewer Layout...");
            PTViewer viewer = GetViewerById(id);

            if (viewer != null)
            {
                GeometryGraph msaglGraph = new() { Margins = 0 };

                // Generate nodes for layout
                foreach (PTNode nodeData in Repository.Instance.Models.GetNodes())
                {
                    msaglGraph.Nodes.Add(new Node(
                        CurveFactory.CreateRectangle(
                            1D,
                            1D,
                            new Microsoft.Msagl.Core.Geometry.Point()
                            ),
                        nodeData.Id
                        )
                    );
                }

                // Generate edges for layout
                foreach (PTEdge edgeData in Repository.Instance.Models.GetEdges())
                {
                    PTNode sourceNode = Repository.Instance.Models.GetNodeById(edgeData.Source);
                    PTNode targetNode = Repository.Instance.Models.GetNodeById(edgeData.Target);


                    if (sourceNode != null && targetNode != null)
                    {
                        //Debug.Log("Generating edges...");
                        msaglGraph.Edges.Add(new Edge(
                            msaglGraph.FindNodeByUserData(sourceNode.Id),
                            msaglGraph.FindNodeByUserData(targetNode.Id))
                           {
                                Weight = 1,
                                UserData = edgeData.Id
                            }
                        );
                    } else
                    {
                        //Debug.LogWarning("PROTEUS: Tried generating viewer layout but edge has invalid source or target node.");
                        return;
                    }

                }

                //Debug.Log("Generating with Edges:");
                foreach(Edge edge in msaglGraph.Edges)
                {
                    Debug.Log($"Edge {edge.Source.UserData} {edge.Target.UserData}");
                }

               // Debug.Log("Generating with Nodes:");
                foreach (Node node in msaglGraph.Nodes)
                {
                    Debug.Log($"Node {node.UserData}");
                }

                // EdgeRoutingSettings = new EdgeRoutingSettings{EdgeRoutingMode=EdgeRoutingMode.StraightLine <-- For linear lines
                LayoutHelpers.CalculateLayout(msaglGraph, new SugiyamaLayoutSettings() {LayerSeparation = 0, NodeSeparation = 0 }, null);

                msaglGraph.UpdateBoundingBox();
                msaglGraph.Translate(new Microsoft.Msagl.Core.Geometry.Point(-msaglGraph.Left, -msaglGraph.Bottom));

                // Set layout positions for the nodes
                foreach (Node node in msaglGraph.Nodes)
                {
                    // If no layout exists initialize it
                    if (viewer.LayoutPositions == null)
                    {
                        viewer.LayoutPositions = new();
                    }

                    viewer.LayoutPositions[(string)node.UserData] = new Vector3((float)node.BoundingBox.Center.X, (float)node.BoundingBox.Center.Y, 0.0f);
                }

                // Set information for the edges
                foreach(Edge edge in msaglGraph.Edges)
                {
                    Debug.Log($"Generating data for edge {edge.UserData} {edge.Curve.GetType()}");
                    if (edge.Curve is Curve)
                    {
                        Point? pt = null;
                        foreach (var segment in (edge.Curve as Curve).Segments)
                        {
                            // When curve contains a line segment.
                            if (segment is LineSegment)
                            {

                            }

                            // When curve contains a cubic Bezier segment.
                            else if (segment is CubicBezierSegment)
                            {
 
                            }

                            // When curve contains an arc.
                            else if (segment is Ellipse)
                            {

                            }
                            else
                            {
                            }
                        }
                    } else if (edge.Curve is LineSegment)
                    {
                        LineSegment segment = (LineSegment)edge.Curve;
                        Debug.Log($"Edge curve segment {segment.Start.X} {segment.Start.Y}");
                    }
                }


                Debug.Log("PROTEUS: Viewer Layout Finished Generating");
            }
        }



    }

}
