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
using UnityEngine.Splines;
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
            PTViewer viewer = GetViewerById(id);
            Dictionary<string, Vector3> nodeLayout = new();
            Dictionary<string, List<Spline>> edgeLayout = new();
            float zoomLevel;

            Debug.Log("PROTEUS: Started Generating Viewer Layout...");

            if (viewer != null)
            {
                GeometryGraph msaglGraph = new() { Margins = 0 };

                // Generate nodes for layout
                foreach (PTNode nodeData in Repository.Instance.Models.GetNodes())
                {
                    msaglGraph.Nodes.Add(new Node(
                        CurveFactory.CreateRectangle(
                            nodeData.UnitWidth,
                            nodeData.UnitHeight,
                            new Point()
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
                        Debug.LogWarning("PROTEUS: Tried generating viewer layout but edge has invalid source or target node.");
                        return;
                    }

                }

                // Calculate the new layout
                LayoutHelpers.CalculateLayout(msaglGraph, new SugiyamaLayoutSettings() { EdgeRoutingSettings = new EdgeRoutingSettings { EdgeRoutingMode = EdgeRoutingMode.StraightLine}, LayerSeparation = 0, NodeSeparation = 0 }, null);

                msaglGraph.UpdateBoundingBox();
                msaglGraph.Translate(new Point(-msaglGraph.Left, -msaglGraph.Bottom));

                // Set zoom level
                if (msaglGraph.Width == 0 || msaglGraph.Height == 0)
                {
                    zoomLevel = 0.0f;
                } else
                {
                    zoomLevel = (float)Math.Max(1.0 / msaglGraph.Width, 1.0 / msaglGraph.Height);
                }


                viewer.ZoomScale = new Vector3(zoomLevel, zoomLevel, 0.1f);
                viewer.MaxZoomScale = Vector3.positiveInfinity;
                viewer.MinZoomScale = new Vector3(0,0,0);

                // Set layout positions for the nodes
                foreach (Node node in msaglGraph.Nodes)
                {
                    nodeLayout[(string)node.UserData] = new Vector3((float)node.BoundingBox.Center.X, (float)node.BoundingBox.Center.Y, 0.0f);
                }

                // Set information for the edges
                foreach(Edge edge in msaglGraph.Edges)
                {
                    List<Spline> splines = new(); 
                    Spline spline = new();
                    BezierKnot startKnot = new();
                    BezierKnot endKnot = new();

                    splines.Add(spline);

                    startKnot.Position = new((float)edge.Curve.Start.X, (float)edge.Curve.Start.Y, 0);
                    endKnot.Position = new((float)edge.Curve.End.X, (float)edge.Curve.End.Y,0);

                    startKnot.Rotation = new(0, 0, 90, (float)Space.Self);

                    spline.Add(startKnot);
                    spline.Add(endKnot);

                    edgeLayout[(string)edge.UserData] = splines;

                    continue;

                    //TODO: Add support for different types of curves
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
                        //Debug.Log($"Edge curve segment {segment.Start.X} {segment.Start.Y}");
                    }
                }

                // Update layout
                viewer.LayoutNodes = nodeLayout;
                viewer.LayoutEdges = edgeLayout;


                Debug.Log("PROTEUS: Viewer Layout Finished Generating");
            }
        }

        /// <summary>
        ///  Get the edges that are related to the viewer.
        /// </summary>
        /// <returns></returns>
        public List<PTEdge> GetRelatedEdgesOfViewer(string viewerId)
        {
            List<PTEdge> edges = new();
            PTViewer viewer = GetViewerById(viewerId);

            if (viewer == null)
            {
                Debug.LogWarning("PROTEUS: Tried to get related edges of viewer but viewer is null.");
                return edges;
            }

            // Get the related edges based on the layout.
            if (viewer.LayoutEdges != null)
            {
                foreach (string edgeId in viewer.LayoutEdges.Keys)
                {
                    PTEdge edge = Repository.Instance.Models.GetEdgeById(edgeId);

                    if (edge != null)
                    {
                        edges.Add(edge);
                    }
                }
            }
            return edges;
        }

        /// <summary>
        /// Adds the zoom delta to the specified viewer
        /// </summary>
        /// <param name="viewerId">Id of the viewer to zoom</param>
        /// <param name="delta">zoom delta</param>
        public void ZoomViewer(string viewerId, float delta = 0.0f)
        {
            PTViewer viewer = GetViewerById(viewerId);

            if (viewer != null && viewer?.ZoomScale != null )
            {
                Vector3 oldZoomScale = (Vector3)viewer.ZoomScale;
                Vector3 maxZoomScale = (Vector3)viewer?.MaxZoomScale;
                Vector3 minZoomScale = (Vector3)viewer?.MinZoomScale;


                float x = oldZoomScale.x + delta;
                float y = oldZoomScale.y + delta;
                float z = oldZoomScale.z + delta;

                // Limit to upper bound
                if (viewer?.MaxZoomScale != null)
                {
                    x = Math.Min(x, maxZoomScale.x);
                    y = Math.Min(y, maxZoomScale.y);
                    z = Math.Min(z, maxZoomScale.z);
                }

                // Limit to lower bound
                if (viewer?.MinZoomScale != null)
                {
                    x = Math.Max(x, minZoomScale.x);
                    y = Math.Max(y, minZoomScale.y);
                    z = Math.Max(z, minZoomScale.z);
                }

                viewer.ZoomScale = new Vector3 (x, y, z);
            }


        }

    }

}
