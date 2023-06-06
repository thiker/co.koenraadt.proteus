using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Other;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public class ModelsRepository
    {
        private static ModelsRepository _instance = null;

        private ObservableCollection<PTNode> _ptNodes;
        private ObservableCollection<PTEdge> _ptEdges;
        private ObservableCollection<PTModelElement> _ptModelElements;

        public static ModelsRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModelsRepository();
                    _instance.Init();
                }
                return _instance;
            }
        }

        private void Init()
        {
            // Initialise collections
            _ptNodes = new();
            _ptEdges = new();
            _ptModelElements = new();

            // TODO: Initialize the repo and load everything
        }

        /// <summary>
        /// Adds a PTNode to the ModelsRepository or updates it.
        /// </summary>
        /// <param name="node">The PTNode to add.</param>
        public void UpdateNode(PTNode newNode)
        {         
            PTNode oldNode = GetNodeById(newNode.Id);

            // If not already existing add the node
            if (oldNode is null)
            {
                _ptNodes.Add(newNode);
            } else
            {
                Helpers.CombineValues(oldNode, newNode);
            }
        }

        /// <summary>
        /// Adds a PTEdge to the ModelsRepository.
        /// </summary>
        /// <param name="edge">The PTEdge to add.</param>
        public void UpdateEdge(PTEdge newEdge)
        {
            PTEdge oldEdge = GetEdgeById(newEdge.Id);

            // If not already existing add the edge
            if (oldEdge is null)
            {
                _ptEdges.Add(newEdge);
            }
            else
            {
                Helpers.CombineValues(oldEdge, newEdge);
            }
        }

        /// <summary>
        /// Adds a PTModelElement to the ModelsRepository.
        /// </summary>
        /// <param name="modelElement">The PTModelElement to add.</param>
        public void UpdateModelElement(PTModelElement newModelElement)
        {
            PTModelElement oldModelElement = GetModelElementById(newModelElement.Id);

            // If not already existing add the edge
            if (oldModelElement is null)
            {
                _ptModelElements.Add(newModelElement);
            }
            else
            {
                Helpers.CombineValues(oldModelElement, newModelElement);
            }
        }

        /// <summary>
        /// Get the collection of nodes.
        /// </summary>
        /// <returns>Collection of PTNodes</returns>
        public ObservableCollection<PTNode> GetNodes()
        {
            return _ptNodes;
        }

        /// <summary>
        /// Get the collection of edges.
        /// </summary>
        /// <returns>Collection of PTEdges</returns>
        public ObservableCollection<PTEdge> GetEdges()
        {
            return _ptEdges;
        }

        /// <summary>
        /// Get a PTNode by its Id.
        /// </summary>
        /// <param name="id">the node's identifier.</param>
        /// <returns>The PTNode with the respective Id</returns>
        public PTNode GetNodeById(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }
            PTNode foundNode = _ptNodes.FirstOrDefault(x => x.Id == id);
            return foundNode;
        }

        /// <summary>
        /// Get a PTEdge by its Id.
        /// </summary>
        /// <param name="id">the edge's identifier.</param>
        /// <returns>The PTEdge with the respective Id</returns>
        public PTEdge GetEdgeById(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }
            PTEdge foundEdge = _ptEdges.FirstOrDefault(x => x.Id == id);
            return foundEdge;
        }

        /// <summary>
        /// Get a PTModelElement by its Id.
        /// </summary>
        /// <param name="id">the element's identifier.</param>
        /// <returns>The PTModelElement with the respective Id</returns>
        public PTModelElement GetModelElementById(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }

            PTModelElement foundModelElement = _ptModelElements.FirstOrDefault(x => x.Id == id);
            return foundModelElement;
        }



        /// <summary>
        /// Get a PTNode by its name
        /// </summary>
        /// <param name="id">the node's name.</param>
        /// <returns>The PTNode with the respective Name</returns>
        public PTNode GetNodeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) { return null; }
            PTNode foundNode = _ptNodes.FirstOrDefault(x => x.Name == name);
            return foundNode;
        }

        public List<PTNode> GetRelatedBehavioralNodesById(string id)
        {
            List<PTNode> relatedBehavioralNodes = new();
            List<PTNode> relatedNodes = new();

            PTNode rootNode = GetNodeById(id);

            if (rootNode != null)
            {
                foreach (string el in rootNode.ModelElements) {
                    PTModelElement element = GetModelElementById(el);

                    if (element != null && element?.RelatedNodes != null)
                    {
                        foreach (string relatedNodeId in element.RelatedNodes)
                        {
                            PTNode relatedNode = GetNodeById(relatedNodeId);

                            if (relatedNode != null)
                            {
                                relatedNodes.Add(relatedNode);
                            }
                        }
                    }

                }
            }

            // Filter to only include behavioral nodes
            relatedBehavioralNodes = relatedNodes.Where(x => Helpers.IsBehavioralMetaClass(x.MetaClass)).ToList();

            return relatedBehavioralNodes;
        }

        /// <summary>
        /// Gets the related nodes and edges of a root node.
        /// </summary>
        /// <param name="rootNodeId">the id of the root node</param>
        /// <returns>a tuple containing the nodes and edges related to the root node.</returns>
        public Tuple<List<string>, List<string>> FindRelatedNodesAndEdgesOfRootNode(string rootNodeId)
        {
            HashSet<string> seenEdges = new();
            List<string> relatedNodeIds = new();
            List<string> relatedEdgeIds = new();

            PTNode rootNode = GetNodeById(rootNodeId);

            Debug.Log($"Getting related for root node with name{rootNode.Name} and id {rootNode.Id}");

            if (rootNode == null) { Debug.LogWarning($"Tried to find related nodes and edges of root node but root node with {rootNodeId} is null."); return Tuple.Create(relatedNodeIds, relatedEdgeIds); };
            if (rootNode.Edges == null) {  Debug.LogWarning($"Tried to find edges of root node {rootNode.Id} {rootNode.Name} but rootNode.Edges is null"); return Tuple.Create(relatedNodeIds, relatedEdgeIds); };
            
            foreach(string edgeId in rootNode.Edges)
            {
                PTEdge edge = GetEdgeById(edgeId);
                string targetNodeId = edge.Target;

                if (seenEdges.Contains(edge.Id) || targetNodeId == rootNodeId)
                {
                    break;
                }

                seenEdges.Add(edge.Id);

                relatedNodeIds.Add(targetNodeId);
                relatedEdgeIds.Add(edge.Id);

                var result = FindRelatedNodesAndEdgesOfRootNode(targetNodeId);
                relatedNodeIds = relatedNodeIds.Concat(result.Item1).ToList();
                relatedEdgeIds = relatedEdgeIds.Concat(result.Item2).ToList();
            }

            return Tuple.Create(relatedNodeIds, relatedEdgeIds);
        }

        /// <summary>
        /// Removes a node by its id
        /// </summary>
        /// <param name="id">the node's identifier</param>
        public void DeleteNodeById(string id)
        {
            PTNode nodeToDelete = GetNodeById(id);
            if (nodeToDelete != null)
            {
                int ix = _ptNodes.IndexOf(nodeToDelete);
                _ptNodes.RemoveAt(ix);
            }
        }

        /// <summary>
        /// Removes an edge by its id
        /// </summary>
        /// <param name="id">the edge's identifier</param>
        public void DeleteEdgeById(string id)
        {
            PTEdge edgeToDelete = GetEdgeById(id);
            if (edgeToDelete != null)
            {
                int ix = _ptEdges.IndexOf(edgeToDelete);
                _ptEdges.RemoveAt(ix);
            }
        }

        /// <summary>
        /// Removes a model element by its id
        /// </summary>
        /// <param name="id">the model element's identifier</param>
        public void DeleteModelElementById(string id)
        {
            PTModelElement modelElementToDelete = GetModelElementById(id);
            
            if (modelElementToDelete != null)
            {
                int ix = _ptModelElements.IndexOf(modelElementToDelete);
                _ptModelElements.RemoveAt(ix);
            }
        }
    }
}