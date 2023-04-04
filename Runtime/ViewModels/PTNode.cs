using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTNode : ObservableObject
    {
        private string _id;
        private string _name;
        private string _displayName;
        private string _description;
        private Texture2D _imageTexture;

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
        
        /// <summary>
        /// The label of the node.
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        /// <summary>
        /// The description of the node.
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// Texture of the node's diagram image.
        /// </summary>
        public Texture2D ImageTexture
        {
            get => _imageTexture;
            set => SetProperty(ref _imageTexture, value);
        }

        public PTNode ()
        {
        }
    }
}
