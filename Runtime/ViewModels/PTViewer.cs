using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Msagl.Core.Geometry.Curves;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

#nullable enable
namespace Packages.co.koenraadt.proteus.Runtime.ViewModels
{
    public class PTViewer : ObservableObject
    {
        private string _id;
        private string _rootNodeId;
        private Vector3? _position;
        private Vector3? _scale;
        private Vector3? _modelAnchorOffset = null;
        private Vector3? _zoomScale;
        private Vector3? _maxZoomScale;
        private Vector3? _minZoomScale;
        private Quaternion? _rotation;
        private Matrix4x4? _viewWindowWorldToLocal;
        private Dictionary<string, Vector3>? _layoutNodes;
        private Dictionary<string, List<Spline>>? _layoutEdges;


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
        /// The Id of the node that is the root for the viewer.
        /// </value>
        public string RootNodeId
        {
            get => _rootNodeId;
            set => SetProperty(ref _rootNodeId, value);
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
        /// The position of the viewer.
        /// <value>
        public Vector3? Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }


        /// <value>
        /// The local position of the view window
        /// <value>
        public Vector3? ModelAnchorOffset 
        {
            get => _modelAnchorOffset;
            set => SetProperty(ref _modelAnchorOffset, value);
        }

        /// <summary>
        /// The zoom level of the viewer.
        /// </summary>
        public Vector3? ZoomScale
        {
            get => _zoomScale;
            set => SetProperty(ref _zoomScale, value);
        }

        /// <summary>
        /// The maximum zoom level of the viewer.
        /// </summary>
        public Vector3? MaxZoomScale
        {
            get => _maxZoomScale;
            set => SetProperty(ref _maxZoomScale, value);
        }

        /// <summary>
        /// The minimum zoom level of the viewer.
        /// </summary>
        public Vector3? MinZoomScale
        {
            get => _minZoomScale;
            set => SetProperty(ref _minZoomScale, value);
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

        /// <value>
        /// Layout containing the positions of the nodes in the viewer.
        /// </value>
        public Dictionary<string, Vector3>? LayoutNodes
        {
            get => _layoutNodes;
            set => SetProperty(ref _layoutNodes, value);
        }

        /// <summary>
        /// Layout containing the curves of the edges in the viewer.
        /// </summary>
        public Dictionary<string, List<Spline>>? LayoutEdges
        {
            get => _layoutEdges;
            set => SetProperty(ref _layoutEdges, value);
        }


    }
}
#nullable disable