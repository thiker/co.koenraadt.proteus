﻿using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Routing;
using co.koenraadt.proteus.Runtime.Other;
using co.koenraadt.proteus.Runtime.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Splines;
using static Microsoft.Msagl.Layout.OverlapRemovalFixedSegments.OverlapRemovalFixedSegmentsMst;

namespace co.koenraadt.proteus.Runtime.Repositories
{

    /// <summary>
    /// Part of the repository that holds all viewer related data.
    /// </summary>
    public class ViewersRepository
    {
        private static ViewersRepository _instance = null;
        private ObservableCollection<PTViewer> _ptViewers;

        /// <summary>
        /// The singleton instance of the ViewersRepository.
        /// </summary>
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

        /// <summary>
        /// Initialize the ViewersRepository.
        /// </summary>
        private void Init()
        {
            _ptViewers = new();
        }

        /// <summary>
        /// Creates a new viewer.
        /// </summary>
        /// <param name="viewerData">The data of the viewer to created.</param>
        /// <param name="autoPlace">Whether the viewer should be automatically placed in the scene.</param>
        public void CreateViewer(PTViewer viewerData, bool autoPlace = true)
        {
            PTViewer viewerUpdate = viewerData;

            if (autoPlace && GetViewers().Count > 0)
            {
                PTViewer mostOuterViewer = null;
                foreach (PTViewer viewer in GetViewers())
                {
                    if (mostOuterViewer == null)
                    {
                        mostOuterViewer = viewer;
                        continue;
                    }
                    Vector3 mostRightPos = (Vector3)mostOuterViewer.Position;
                    Vector3 viewerPos = (Vector3)viewer.Position;

                    if (viewerPos.x < mostRightPos.x)
                    {
                        mostOuterViewer = viewer;
                    }
                }
                Vector3 newPos = (Vector3)mostOuterViewer.Position;
                float offsetFactor = 2.2f;
                newPos.x = ((Vector3)mostOuterViewer.Position).x - (offsetFactor * (Vector3)mostOuterViewer.Scale).x;

                viewerUpdate.Position = newPos;
                viewerUpdate.Scale = mostOuterViewer.Scale;
            } else if (autoPlace) {
                viewerUpdate.Position = Repository.Instance.Proteus.GetGlobals().DefaultViewerPosition;
                viewerUpdate.Scale = Repository.Instance.Proteus.GetGlobals().DefaultViewerScale;
            }

            UpdateViewer(viewerUpdate);
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
            if (newViewer?.RootNodeIds != null)
            {
                changedRoot = !(newViewer?.RootNodeIds).Equals(oldViewer?.RootNodeIds);
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
        /// <returns>The PTViewer with the respective Id.</returns>
        public PTViewer GetViewerById(string id)
        {
            if (id != null && id != "")
            {
                PTViewer foundViewer = _ptViewers.FirstOrDefault(x => x.Id == id);
                return foundViewer;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Get the collection of viewers.
        /// </summary>
        /// <returns>Collection of PTViewers.</returns>
        public ObservableCollection<PTViewer> GetViewers()
        {
            return _ptViewers;

        }

        /// <summary> 
        /// Removes a viewer by its id.
        /// </summary>
        /// <param name="id">the viewer's identifier.</param>
        public void DeleteViewerById(string id)
        {
            PTViewer viewerToDelete = GetViewerById(id);
            if (viewerToDelete is not null)
            {
                int ix = _ptViewers.IndexOf(viewerToDelete);
                _ptViewers.RemoveAt(ix);
                Repository.Instance.Proteus.ClearViewerSelection();
            }
        }


        /// <summary>
        /// Updates the position of the viewer.
        /// </summary>
        /// <param name="id">the id of the viewer to update.</param>
        /// <param name="position">the position of the viewer to update.</param>
        public void SetViewerPosition(string id, Vector3 position)
        {
            PTViewer viewer = GetViewerById(id);

            if (viewer != null && position != null)
            {
                viewer.Position = position;
            }
        }


        /// <summary>
        /// Updates the ViewWindow's WorldToLocal matrix for the viewer.
        /// </summary>
        /// <param name="id">the id of the viewer to update. </param>
        /// <param name="viewWindowWorldToLocal">the ViewWindowWorldToLocalMatrix of the viewer to update. </param>
        public void SetViewWindowWorldToLocal(string id, Matrix4x4 viewWindowWorldToLocal)
        {
            PTViewer viewer = GetViewerById(id);

            if (viewer != null && viewWindowWorldToLocal != null)
            {
                viewer.ViewWindowWorldToLocal = viewWindowWorldToLocal;
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
        /// Set the visibility of the gizmo of a viewer.
        /// </summary>
        /// <param name="id">the id of the viewer to update.</param>
        /// <param name="visible">the visibility of the gizmo for the viewer.</param>
        public void SetGizmoVisible(string id, bool visible) {
            PTViewer viewer = GetViewerById(id);

            if (viewer != null) {
                viewer.GizmoVisible = visible;
            }
        }

        /// <summary>
        /// Updates the local position of the view window.
        /// </summary>
        /// <param name="id">the id of the viewer to update. </param>
        /// <param name="position">the local position of the view window.</param>
        public void AddModelAnchorOffset(string id, Vector3 offset)
        {
            PTViewer viewer = GetViewerById(id);
            Vector3 newOffset;
            if (viewer?.ModelAnchorOffset != null)
            {
                newOffset = (Vector3)viewer.ModelAnchorOffset + offset;
            }
            else
            {
                newOffset = new Vector3(0, 0, 0);
            }

            PTViewer updatedViewer = new PTViewer() { Id = viewer.Id, ModelAnchorOffset = newOffset };
            UpdateViewer(updatedViewer);
        }

        /// <summary>
        /// Regenerate the layouts of all viewers.
        /// </summary>
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
            List<PTNode> nodesData = new();
            List<PTEdge> edgesData = new();
            Dictionary<string, Vector3> nodeLayout = new();
            Dictionary<string, List<Spline>> edgeLayout = new();
            float zoomLevel;

            Debug.Log("PROTEUS: Started Generating Viewer Layout...");

            if (viewer != null)
            {

                // Get the nodes and edges related to the viewer.
                List<string> relatedNodeIds = new();
                List<string> relatedEdgeIds = new();

                foreach (string rootNodeId in viewer.RootNodeIds)
                {
                    relatedNodeIds.Add(rootNodeId);

                    var related = Repository.Instance.Models.FindRelatedNodesAndEdgesOfRootNode(rootNodeId);
                    List<string> nodeIds = related.Item1;
                    List<string> edgeIds = related.Item2;

                    relatedNodeIds = relatedNodeIds.Concat(nodeIds).ToList();
                    relatedEdgeIds = relatedEdgeIds.Concat(edgeIds).ToList();
                }

                // Remove duplicates
                relatedNodeIds = relatedNodeIds.Distinct().ToList();
                relatedEdgeIds = relatedEdgeIds.Distinct().ToList();

                Debug.Log($"Found {relatedNodeIds.Count} related nodes and {relatedEdgeIds.Count} edges.");

                // Get the data of the related nodes and edges
                foreach (string nodeId in relatedNodeIds)
                {
                    nodesData.Add(Repository.Instance.Models.GetNodeById(nodeId));
                }

                foreach (string edgeId in relatedEdgeIds)
                {
                    edgesData.Add(Repository.Instance.Models.GetEdgeById(edgeId));
                }


                GeometryGraph msaglGraph = new() { Margins = 0 };

                // Generate nodes for layout
                foreach (PTNode nodeData in nodesData)
                {
                    try
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
                    catch (Exception e) { }
                }

                // Generate edges for layout
                foreach (PTEdge edgeData in edgesData)
                {
                    PTNode sourceNode = Repository.Instance.Models.GetNodeById(edgeData.Source);
                    PTNode targetNode = Repository.Instance.Models.GetNodeById(edgeData.Target);

                    if (sourceNode != null && targetNode != null)
                    {
                        msaglGraph.FindNodeByUserData(targetNode.Id);
                        Debug.Log($"Adding edge source node {sourceNode.Name} {targetNode?.Name}");
                        Debug.Log($"source {msaglGraph.FindNodeByUserData(sourceNode.Id).UserData} target {msaglGraph.FindNodeByUserData(targetNode.Id).UserData}");


                        msaglGraph.Edges.Add(new Edge(
                            msaglGraph.FindNodeByUserData(sourceNode.Id),
                            msaglGraph.FindNodeByUserData(targetNode.Id))
                        {
                            Weight = 1,
                            UserData = edgeData.Id
                        }
                        );
                    }
                    else
                    {
                        Debug.LogWarning("PROTEUS: Tried generating viewer layout but edge has invalid source or target node.");
                        return;
                    }

                }

                // Calculate the new layout
                LayoutHelpers.CalculateLayout(msaglGraph, new SugiyamaLayoutSettings() { EdgeRoutingSettings = new EdgeRoutingSettings { EdgeRoutingMode = EdgeRoutingMode.StraightLine }, LayerSeparation = 0, NodeSeparation = 0 }, null);

                msaglGraph.UpdateBoundingBox();
                msaglGraph.Translate(new Point(-msaglGraph.Left, -msaglGraph.Bottom));

                // Set zoom level
                if (msaglGraph.Width == 0 || msaglGraph.Height == 0)
                {
                    zoomLevel = 0.0f;
                }
                else
                {
                    zoomLevel = (float)Math.Max(1.0 / msaglGraph.Width, 1.0 / msaglGraph.Height);
                }


                viewer.ZoomScale = new Vector3(zoomLevel, zoomLevel, 0.1f);
                viewer.MaxZoomScale = Vector3.positiveInfinity;
                viewer.MinZoomScale = new Vector3(0, 0, 0);

                // Set layout positions for the nodes
                foreach (Node node in msaglGraph.Nodes)
                {
                    nodeLayout[(string)node.UserData] = new Vector3((float)node.BoundingBox.Center.X, (float)node.BoundingBox.Center.Y, 0.0f);
                }

                // Set information for the edges
                foreach (Edge edge in msaglGraph.Edges)
                {
                    List<Spline> splines = new();
                    Spline spline = new();
                    BezierKnot startKnot = new();
                    BezierKnot endKnot = new();

                    splines.Add(spline);

                    startKnot.Position = new((float)edge.Curve.Start.X, (float)edge.Curve.Start.Y, 0);
                    endKnot.Position = new((float)edge.Curve.End.X, (float)edge.Curve.End.Y, 0);

                    startKnot.Rotation = new(0, 0, 90, (float)Space.Self);

                    spline.Add(startKnot);
                    spline.Add(endKnot);

                    edgeLayout[(string)edge.UserData] = splines;
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
        /// <param name="viewerId">The id of the viewer to get the related nodes of.</param>
        /// <returns>List of nodes that are related to a viewer</returns>
        public List<PTNode> GetRelatedNodesOfViewer(string viewerId)
        {
            List<PTNode> nodes = new();
            PTViewer viewer = GetViewerById(viewerId);

            if (viewer == null)
            {
                Debug.LogWarning("PROTEUS: Tried to get related nodes of viewer but viewer is null.");
                return nodes;
            }

            // Get the related edges based on the layout.
            if (viewer.LayoutNodes != null)
            {
                foreach (string nodeId in viewer.LayoutNodes.Keys)
                {
                    PTNode node = Repository.Instance.Models.GetNodeById(nodeId);

                    if (node != null)
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
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
        /// Sets a new scale of a viewer.
        /// </summary>
        /// <param name="viewerId">The id of the viewer to update.</param>
        /// <param name="scaleDelta">The scale delta that is added to the viewer's current scale.</param>
        public void ScaleViewer(string viewerId, Vector3 scaleDelta)
        {
            PTViewer viewer = GetViewerById(viewerId);
            if (viewer != null && scaleDelta != null)
            {
                viewer.Scale = viewer.Scale + scaleDelta;
            }
        }

        /// <summary>
        /// Adds the zoom delta to the specified viewer.
        /// </summary>
        /// <param name="viewerId">Id of the viewer to zoom.</param>
        /// <param name="delta">The zoom delta that will be added to the viewer's scale.</param>
        public void ZoomViewer(string viewerId, float delta = 0.0f)
        {
            PTViewer viewer = GetViewerById(viewerId);

            if (viewer != null && viewer?.ZoomScale != null)
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

                viewer.ZoomScale = new Vector3(x, y, z);
            }


        }

    }

}
