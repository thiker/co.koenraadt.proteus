
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTGlobals : ObservableObject
    {
        private Vector3 _defaultViewerPosition = new Vector3(-4,7,3);
        private Vector3 _defaultViewerScale = new Vector3(3,3,3);
        private List<string> _selectedNodes;
        private string[] _selectedViewers;
        private bool _xrayViewEnabled = true;
        private bool _explodedViewEnabled = true;
        private float _defaultNodeUnitWidth = 10.0f;
        private float _defaultNodeUnitHeight = 10.0f;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public List<string> SelectedNodes
        {
            get => _selectedNodes;
            set => SetProperty(ref _selectedNodes, value);
        }

        public string[] SelectedViewers
        {
            get => _selectedViewers;
            set => SetProperty(ref _selectedViewers, value);
        }

        public bool XrayViewEnabled 
        {
            get => _xrayViewEnabled;
            set => SetProperty(ref _xrayViewEnabled, value);
        }

        public bool ExplodedViewEnabled 
        {
            get => _explodedViewEnabled;
            set => SetProperty(ref _explodedViewEnabled, value);
        }
        
        public Vector3 DefaultViewerPosition 
        {
            get => _defaultViewerPosition;
            set => SetProperty(ref _defaultViewerPosition, value);
        }
        
        public Vector3 DefaultViewerScale 
        {
            get => _defaultViewerScale;
            set => SetProperty(ref _defaultViewerScale, value);
        }

        public float DefaultNodeUnitHeight 
        {
            get => _defaultNodeUnitHeight;
            set => SetProperty(ref _defaultNodeUnitHeight, value);
        }

        public float DefaultNodeUnitWidth
        {
            get => _defaultNodeUnitWidth;
            set => SetProperty(ref _defaultNodeUnitWidth, value);
        }


        public PTGlobals()
        {
        }
    }
}

