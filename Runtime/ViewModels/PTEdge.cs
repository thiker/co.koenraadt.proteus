using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    /// <summary>
    /// Class that holds the data for an edge of the 3DML formatted model.
    /// </summary>
    public class PTEdge : ObservableObject
    {
        private string _id;
        private string _source;
        private string _target;

        /// <summary>
        /// The identifier of the edge.
        /// </summary>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// The id of the source node of the edge.
        /// </summary>
        public string Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        /// <summary>
        /// The id of the target node of the edge.
        /// </summary>
        public string Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }

        public PTEdge ()
        {
        }
    }
}
