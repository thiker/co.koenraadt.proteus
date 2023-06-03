using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTState : ObservableObject
    {
        private string _id;
        private Dictionary<string, object> _values;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public Dictionary<string, object> Values
        {
            get => _values;
            set => SetProperty(ref _values, value);
        }

        public PTState()
        {
        }
    }
}
