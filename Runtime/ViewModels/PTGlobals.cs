
using CommunityToolkit.Mvvm.ComponentModel;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTGlobals : ObservableObject
    {
        private string[] _selectedNodes;
        private string _selectedViewer;
        private bool _xrayViewEnabled = true;
        private bool _explodedViewEnabled = true;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public string[] SelectedNodes
        {
            get => _selectedNodes;
            set => SetProperty(ref _selectedNodes, value);
        }

        public string SelectedViewer 
        {
            get => _selectedViewer;
            set => SetProperty(ref _selectedViewer, value);
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

