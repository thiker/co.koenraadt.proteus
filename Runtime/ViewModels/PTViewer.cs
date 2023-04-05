using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTViewer : ObservableObject
    {
        private string _id;
        private Vector3? _position;
        private Quaternion? _rotation;
        private Matrix4x4? _viewWindowWorldToLocal;

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
        public Vector3? Position 
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        /// <value>
        /// The rotation of the viewer.
        /// <value>
        public Quaternion? Rotation 
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        /// <value>
        /// The world to local matrix of the viewer's view window.
        /// </value>
        public Matrix4x4? ViewWindowWorldToLocal
        {
            get => _viewWindowWorldToLocal;
            set => SetProperty(ref _viewWindowWorldToLocal, value);
        }
    }
}
