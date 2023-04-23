using Packages.co.koenraadt.proteus.Runtime.Other;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            PTViewer oldViewer = GetViewerById(newViewer.Id);

            // If not already existing add the node
            if (oldViewer is null)
            {
                _ptViewers.Add(newViewer);
            }
            else
            {
                Helpers.CombineValues(oldViewer, newViewer);
            }
        }

        /// <summary>
        /// Get a PTViewer by its Id.
        /// </summary>
        /// <param name="id">the viewer's identifier.</param>
        /// <returns>The PTViewer with the respective Id</returns>
        public PTViewer GetViewerById(string id)
        {
            PTViewer foundViewer = _ptViewers.FirstOrDefault(x => x.Id == id);
            return foundViewer;
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
   
            if (viewer != null)
            {
                viewer.ModelAnchorOffset += offset;
            }


        }



    }

}
