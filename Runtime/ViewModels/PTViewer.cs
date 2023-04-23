﻿using CommunityToolkit.Mvvm.ComponentModel;
using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTViewer : ObservableObject
    {
        private string _id;
        private Vector3? _position;
        private Vector3? _modelAnchorOffset = new Vector3(0,0,0);
        private Quaternion? _rotation;
        private Matrix4x4? _viewWindowWorldToLocal;

        private bool? _isBillboarding = true;

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
        /// The local position of the view window
        /// <value>
        public Vector3? ModelAnchorOffset 
        {
            get => _modelAnchorOffset;
            set => SetProperty(ref _modelAnchorOffset, value);
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


        /// <value>
        /// Wether billboarding should be enabled for the viewer.
        /// </value>
        public bool? IsBillboarding 
        {
            get => _isBillboarding;
            set => SetProperty(ref _isBillboarding, value);
        }
    }
}
