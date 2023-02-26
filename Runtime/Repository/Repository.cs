using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.Repository
{
    public partial class Repository
    {
        private static Repository _instance = null;
        private readonly Dictionary<string, PTNode> _ptNodes = new();
        private readonly Dictionary<string, PTEdge> _ptEdges = new();

        public static Repository Instance
        {
            get
            {
                if (_instance == null)
                {
                    Init();
                    _instance = new Repository();
                }
                return _instance;
            }
        }

        public static void Init()
        {
            // TODO: Initialize the repo and load everything
        }

        /// <summary>
        /// Adds a PTNode to the repository.
        /// </summary>
        /// <param name="node">The PTNode to add.</param>
        public void AddPTNode(PTNode node)
        {
            _ptNodes[node.Id] = node;
        }

        /// <summary>
        /// Adds a PTEdge to the repository.
        /// </summary>
        /// <param name="edge">The PTEdge to add.</param>
        public void AddPTEdge(PTEdge edge)
        {
            _ptEdges[edge.Id] = edge;

        }

        /// <summary>
        /// Get a PTNode by its Id.
        /// </summary>
        /// <param name="id">the node's identifier.</param>
        /// <returns>The PTNode with the respective Id</returns>
        public PTNode GetNodeById(string id) { return _ptNodes[id]; }

        /// <summary>
        /// Get a PTEdgde by its Id.
        /// </summary>
        /// <param name="id">the edge's identifier.</param>
        /// <returns>The PTEdge with the respective Id</returns>
        public PTEdge GetPTEdgeById(string id) { return _ptEdges[id]; }
    }
}
