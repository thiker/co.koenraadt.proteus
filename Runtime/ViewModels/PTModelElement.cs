using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTModelElement : ObservableObject
    {
        private string _id;
        private string _metaClass;
        private string _name;
        private string _description;
        private string _displayName;
        private string[] _relatedNodes;

        /// <value>
        /// The identifier of the edge.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <value>
        /// The name of the model element
        /// </value>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <value>
        /// The display name of the model element
        /// </value>
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        /// <value>
        /// The description name of the model element
        /// </value>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }


        /// <summary>
        /// The MetaClass of model element.
        /// </summary>
        public string MetaClass
        {
            get => _metaClass;
            set => SetProperty(ref _metaClass, value);
        }

        /// <summary>
        /// The related diagrams of the model element.
        /// </summary>
        public string[] RelatedNodes
        {
            get => _relatedNodes;
            set => SetProperty(ref _relatedNodes, value);
        }


        public PTModelElement ()
        {
        }
    }
}
