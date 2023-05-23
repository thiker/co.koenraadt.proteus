using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Other;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public class ModelsRepository
    {
        private static ModelsRepository _instance = null;

        private ObservableCollection<PTNode> _ptNodes;
        private ObservableCollection<PTEdge> _ptEdges;

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
            PTNode foundNode = _ptNodes.FirstOrDefault(x => x.Id == id);
            return foundNode;
        }

        /// <summary>
        /// Get a PTNode by its name
        /// </summary>
        /// <param name="id">the node's name.</param>
        /// <returns>The PTNode with the respective Name</returns>
        public PTNode GetNodeByName(string name)
        {
            PTNode foundNode = _ptNodes.FirstOrDefault(x => x.Name == name);
            return foundNode;
        }

        /// <summary>
        /// Get a PTEdge by its Id.
        /// </summary>
        /// <param name="id">the edge's identifier.</param>
        /// <returns>The PTEdge with the respective Id</returns>
        public PTEdge GetEdgeById(string id)
        {
            PTEdge foundEdge = _ptEdges.FirstOrDefault(x => x.Id == id);
            return foundEdge;
        }

        /// <summary>
        /// Removes a node by its id
        /// </summary>
        /// <param name="id">the node's identifier</param>
        public void DeleteNodeById(string id)
        {
            PTNode nodeToDelete = GetNodeById(id);
            if (nodeToDelete is not null)
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
            if (edgeToDelete is not null)
            {
                int ix = _ptEdges.IndexOf(edgeToDelete);
                _ptEdges.RemoveAt(ix);
            }
        }
    }
}