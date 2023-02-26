using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class DTComponent : ObservableObject
    {
        private string _id;
        private string _linkedViewerId;

        /// <value>
        /// The identifier of the digital twin component.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <value>
        /// The identifier of the viewer that the component is linked to.
        /// </value>
        public string LinkedViewerId
        {
            get => _linkedViewerId;
            set => SetProperty(ref _linkedViewerId, value);
        }

        public DTComponent (string id, string linkedViewerId)
        {
            _id = id;
            _linkedViewerId = linkedViewerId;
        }
    }
}
