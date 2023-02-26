using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTViewer : ObservableObject
    {
        private string _id;
        private string _selectedPTNodeId;

        /// <value>
        /// The identifier of the viewer.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <value>
        /// The Id of the currently selected PTNode.
        /// </value>
        public string SelectedPTNodeId
        {
            get => _selectedPTNodeId;
            set => SetProperty(ref _selectedPTNodeId, value);
        }

        public PTViewer(string id)
        {
            _id = id;
        }
    }
}
