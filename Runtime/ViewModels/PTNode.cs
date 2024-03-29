﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace co.koenraadt.proteus.Runtime.ViewModels
{
    /// <summary>
    /// Class that holds the data for a node of the 3DML formatted model.
    /// </summary>
    public class PTNode : ObservableObject
    {

        private string _id;
        private string _name;
        private string _displayName;
        private string _description;
        private string _metaClass;

        private Texture2D _imageTexture;

        private string[] _modelElements;
        private string[] _edges;

        private float _unitWidth = 10f;
        private float _unitHeight = 10f;
        private float _unitDepth = 1.0f;

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
        /// The MetaClass of the node.
        /// </summary>
        public string MetaClass
        {
            get => _metaClass;
            set => SetProperty(ref _metaClass, value);
        }

        /// <summary>
        /// Texture of the node's diagram image.
        /// </summary>
        public Texture2D ImageTexture
        {
            get => _imageTexture;
            set => SetProperty(ref _imageTexture, value);
        }

        /// <summary>
        /// The width of the node in the viewer.
        /// </summary>
        public float UnitWidth
        {
            get => _unitWidth;
            set => SetProperty(ref _unitWidth, value);
        }

        /// <summary>
        /// The height of the node in the viewer.
        /// </summary>
        public float UnitHeight
        {
            get => _unitHeight;
            set => SetProperty(ref _unitHeight, value);
        }

        /// <summary>
        /// The depth of the node in the viewer.
        /// </summary>
        public float UnitDepth
        {
            get => _unitDepth;
            set => SetProperty(ref _unitDepth, value);
        }

        /// <summary>
        /// The ids of the edges that the node is connected to
        /// </summary>
        public string[] Edges
        {
            get => _edges;
            set => SetProperty(ref _edges, value);
        }

        /// <summary>
        /// The ids of the model elements that are used by the node.
        /// </summary>
        public string[] ModelElements
        {
            get => _modelElements;
            set => SetProperty(ref _modelElements, value);
        }

        public PTNode()
        {
        }
    }
}
