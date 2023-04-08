
using CommunityToolkit.Mvvm.ComponentModel;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTGlobals : ObservableObject
    {
        private string[] _selectedNodes;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public string[] SelectedNodes
        {
            get => _selectedNodes;
            set => SetProperty(ref _selectedNodes, value);
        }

        public PTGlobals()
        {
        }
    }
}

