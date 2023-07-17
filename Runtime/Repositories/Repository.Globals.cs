using co.koenraadt.proteus.Runtime.ViewModels;
using co.koenraadt.proteus.Runtime.Controllers;
using System.Linq;
using co.koenraadt.proteus.Runtime.Other;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace co.koenraadt.proteus.Runtime.Repositories
{
    /// <summary>
    /// Part of the repository that holds all general Proteus related data.
    /// </summary>
    public class ProteusRepository
    {
        private static ProteusRepository _instance = null;
        private static PTGlobals _globalsInstance = null;

        /// <summary>
        /// Singleton instance of the Proteus part of the repository.
        /// </summary>
        public static ProteusRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProteusRepository();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets the global state of proteus.
        /// </summary>
        public PTGlobals GetGlobals()
        {
            if (_globalsInstance == null)
            {
                _globalsInstance = new();
            }
            return _globalsInstance;
        }

        /// <summary>
        /// Updates the global state of Proteus.
        /// </summary>
        /// <param name="update"></param>
        public void UpdateGlobals(PTGlobals update)
        {
            Helpers.CombineValues(_globalsInstance, update);
        }

        /// <summary>
        /// Sets the selected node. Clears the selection when the nodeId is null.
        /// </summary>
        /// <param name="nodeId">Id of the node to select.</param>
        public void SelectNode(string nodeId)
        {
            CommsController.Instance.SendMessage("proteus/actions/extern/selection", nodeId);

            if (nodeId == "" || nodeId == null)
            {
                ClearNodeSelection();
                return;
            }
            GetGlobals().SelectedNodes = new() { nodeId };
        }

        /// <summary>
        /// Selects a node based on its name.
        /// </summary>
        /// <param name="nodeName">Name of the node to select.</param>
        public void SelectNodeByName(string nodeName)
        {
            PTNode node = Repository.Instance.Models.GetNodeByName(nodeName);

            if (node != null)
            {
                SelectNode(node.Id);
            }
        }


        /// <summary>
        /// Sets the node selection to the provided list.
        /// </summary>
        /// <param name="nodeIds">List of node ids to select.</param>
        public void SelectNodes(List<string> nodeIds)
        {
            GetGlobals().SelectedNodes = nodeIds; 
        }

        /// <summary>
        /// Select nodes based on names instead of ids.
        /// </summary>
        /// <param name="names">Names of the nodes to select.</param>
        public void SelectNodesByNames(string[] names)
        {
            List<string> nodeIds = new();
            foreach (string name in names) {
                PTNode node = Repository.Instance.Models.GetNodeByName(name);
                if (node != null)
                {
                    nodeIds.Add(node.Id);
                }
            }
            SelectNodes(nodeIds);
        }

        /// <summary>
        /// Clears the selection of nodes.
        /// </summary>
        public void ClearNodeSelection()
        {
            GetGlobals().SelectedNodes = new();
        }


        /// <summary>
        /// Get the display names of the selected nodes.
        /// </summary>
        public List<string> GetNodeSelectionDisplayNames()
        {
            List<string> names = new();
            foreach (string id in GetGlobals().SelectedNodes)
            {
                PTNode node = Repository.Instance.Models.GetNodeById(id);
                if (node != null)
                {
                    names.Add(node.Name);
                }
            }

            return names;
        }

        /// <summary>
        /// Gets the currently selected viewer.
        /// </summary>
        /// <returns>selected viewer</returns>
        public PTViewer GetSelectedViewer()
        {
            string selectedViewerId = GetGlobals()?.SelectedViewers?.FirstOrDefault();
            PTViewer selectedViewerData = Repository.Instance.Viewers.GetViewerById(selectedViewerId);
            return selectedViewerData;
        }

        /// <summary>
        /// Checks whether a viewer is selected.
        /// </summary>
        /// <param name="viewerId">The id of the viewer to check.</param>
        /// <returns></returns>
        public bool IsViewerSelected(string viewerId)
        {
            if (viewerId == null || viewerId == "")
            {
                return false;
            }

            return GetSelectedViewer()?.Id == viewerId;
        }

        /// <summary>
        /// Selects a viewer. Clears the selection when viewerId is null.
        /// </summary>
        /// <param name="viewerId">The id of the viewer to select.</param>
        public void SelectViewer(string viewerId)
        {
            if (viewerId == "" || viewerId == null)
            {
                ClearViewerSelection();
                return;
            }
            GetGlobals().SelectedViewers = new[] { viewerId };
        }

        /// <summary>
        /// Clears the viewer selection.
        /// </summary>
        public void ClearViewerSelection()
        {
            GetGlobals().SelectedViewers = new string[] { };
        }
    }
}