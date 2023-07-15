
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    /// <summary>
    /// Class that holds the global data / defaults of Proteus.
    /// </summary>
    public class PTGlobals : ObservableObject
    {
        private Vector3 _defaultViewerPosition = new Vector3(-4,7,3);
        private Vector3 _defaultViewerScale = new Vector3(2,2,2);
        private List<string> _selectedNodes;
        private string[] _selectedViewers;
        private bool _xrayViewEnabled = true;
        private bool _explodedViewEnabled = true;
        private float _defaultNodeUnitWidth = 10.0f;
        private float _defaultNodeUnitHeight = 10.0f;
        private float _defaultNodeInViewTriggerPercentage = .5f;

        /// <value>
        /// List containing the identifiers of the nodes that are selected.
        /// </value>
        public List<string> SelectedNodes
        {
            get => _selectedNodes;
            set => SetProperty(ref _selectedNodes, value);
        }

        /// <summary>
        /// Array containing the ids of the viewers that are selected.
        /// </summary>
        public string[] SelectedViewers
        {
            get => _selectedViewers;
            set => SetProperty(ref _selectedViewers, value);
        }

        /// <summary>
        /// Whether xray view is enabled globally.
        /// </summary>
        public bool XrayViewEnabled 
        {
            get => _xrayViewEnabled;
            set => SetProperty(ref _xrayViewEnabled, value);
        }

        /// <summary>
        /// Whether exploded view is enabled globally.
        /// </summary>
        public bool ExplodedViewEnabled 
        {
            get => _explodedViewEnabled;
            set => SetProperty(ref _explodedViewEnabled, value);
        }

        /// <summary>
        /// The default positition that a viewer is spawned at.
        /// </summary>
        public Vector3 DefaultViewerPosition 
        {
            get => _defaultViewerPosition;
            set => SetProperty(ref _defaultViewerPosition, value);
        }

        /// <summary>
        /// The default scale that a viewer should have.
        /// </summary>
        public Vector3 DefaultViewerScale 
        {
            get => _defaultViewerScale;
            set => SetProperty(ref _defaultViewerScale, value);
        }

        /// <summary>
        /// The default height that a node should have.
        /// </summary>
        public float DefaultNodeUnitHeight 
        {
            get => _defaultNodeUnitHeight;
            set => SetProperty(ref _defaultNodeUnitHeight, value);
        }

        /// <summary>
        /// The default width that a node should have.
        /// </summary>
        public float DefaultNodeUnitWidth
        {
            get => _defaultNodeUnitWidth;
            set => SetProperty(ref _defaultNodeUnitWidth, value);
        }

        /// <summary>
        /// The default percentage that should trigger a node to change from LOD in semantic zooming.
        /// </summary>
        public float DefaultNodeInViewTriggerPercentage 
        {
            get => _defaultNodeInViewTriggerPercentage;
            set => SetProperty(ref _defaultNodeInViewTriggerPercentage, value);
        }


        public PTGlobals()
        {
        }
    }
}

