using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace co.koenraadt.proteus.Runtime.ViewModels
{
    /// <summary>
    /// Class used to hold the data of the states in the 3DML formatted model.
    /// </summary>
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

        /// <summary>
        /// Dictionary containing the values of the state object. Each value is identified by its key and holds a value.
        /// </summary>
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
