using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTEdge : ObservableObject
    {
        private string _id;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public PTEdge (string id)
        {
            _id = id;
        }
    }
}
