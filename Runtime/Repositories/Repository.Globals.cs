using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Other;
using UnityEngine.Networking.Types;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public class ProteusRepository
    {
        private static ProteusRepository _instance = null;
        private static PTGlobals _globalsInstance = null;

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
        /// Get the global state of proteus.
        /// </summary>
        public PTGlobals GetGlobals()
        {
            if (_globalsInstance == null)
            {
                _globalsInstance = new();
            }
            return _globalsInstance;
        }

        public void UpdateGlobals(PTGlobals update)
        {
            Helpers.CombineValues(_globalsInstance, update);
        }

        /// <summary>
        /// Sets the selected node. Clears the selection when the nodeId is null
        /// </summary>
        /// <param name="nodeId">Id of the node to select</param>
        public void SelectNode(string nodeId)
        {
            if (nodeId == "" || nodeId == null)
            {
                ClearNodeSelection();
                return;
            }
            GetGlobals().SelectedNodes = new[] { nodeId };
        }

        /// <summary>
        /// Deselects a node. 
        /// </summary>
        /// <param name="nodeId">The Id of the node to deselect</param>
        public void ClearNodeSelection()
        {
            GetGlobals().SelectedNodes = new string[] { };
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
        /// Updates the selected viewer. Clears the selection when viewerId is null.
        /// </summary>
        /// <param name="viewerId"></param>
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