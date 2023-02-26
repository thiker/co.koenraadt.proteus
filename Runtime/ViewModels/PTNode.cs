using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTNode : ObservableObject
    {
        private string _id;
        private string _name;

        /// <value>
        /// The identifier of the node.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <value>
        /// The name of the node.
        /// </value>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public PTNode (string id, string name)
        {
           _id = id;
            _name = name;
        }
    }
}
