using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTViewer : ObservableObject
    {
        private string _id;
        private Vector3 _position;
        private Quaternion _rotation;

        /// <value>
        /// The identifier of the viewer.
        /// </value>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <value>
        /// The position of the viewer.
        /// <value>
        public Vector3 Position 
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        /// <value>
        /// The rotation of the viewer.
        /// <value>
        public Quaternion Rotation 
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }
    }
}
