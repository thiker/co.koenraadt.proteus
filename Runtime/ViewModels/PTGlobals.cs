
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTGlobals : ObservableObject
    {
        private List<string> _selectedNodes;
        private string[] _selectedViewers;
        private bool _xrayViewEnabled = true;
        private bool _explodedViewEnabled = true;

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
        

        public PTGlobals()
        {
        }
    }
}

