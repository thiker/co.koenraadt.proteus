using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using Codice.Client.Common.TreeGrouper;
using static UnityEngine.UI.Image;
using System;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Microsoft.Msagl.Layout.LargeGraphLayout;

public class GONode : MonoBehaviour, IProteusInteraction
{
    private string _nodeId;
    private string _attachedViewerId;
    private PTNode _nodeData;
    private PTViewer _attachedViewerData;
    private PTGlobals _globalsData;
    private TextMeshPro _displayNameTMP;
    private GameObject _nodeGameObject;
    private MaterialPropertyBlock _matPropBlock;

    // Initialize the node
    public void Init(string nodeId, string attachedViewerId)
    {
        _nodeId = nodeId;
        _attachedViewerId = attachedViewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables
        _matPropBlock = new MaterialPropertyBlock();

        // Get data
        _nodeData = Repository.Instance.Models.GetNodeById(_nodeId);
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        // TODO: Reimplement correctly
        // Get the text object
        GameObject _displayNameObj = transform.Find("DisplayNameText").gameObject;
        _displayNameTMP = _displayNameObj.GetComponent<TextMeshPro>();

        // Get the node object
        _nodeGameObject = transform.Find("Node").gameObject;

        // initialize the event listeners
        linkEventListeners();

        // Set presentation
        UpdateNodePresentation();
    }

    // Update is called once per frame
    void Update()
    {
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = _nodeGameObject.GetComponentInChildren<Renderer>();
        //set the matrix
        if (_attachedViewerData?.ViewWindowWorldToLocal is not null)
            _matPropBlock.SetMatrix("_WorldToBox", (Matrix4x4)_attachedViewerData.ViewWindowWorldToLocal);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(_matPropBlock);
    }



    void OnDestroy()
    {
        if (_nodeData != null)
        {
            _nodeData.PropertyChanged -= OnNodeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged -= OnViewerDataChanged;
        }

        if (_globalsData != null)
        {
            _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        }
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Debug.Log($"PROTEUS: Clicked on node with identifier, {_nodeData.Id} ");
        if (Repository.Instance.Proteus.IsViewerSelected(_attachedViewerData.Id))
        {
            Repository.Instance.Proteus.SelectNode(_nodeData.Id);
        }
    }

    private void linkEventListeners()
    {
        if (_nodeData != null)
        {
            _nodeData.PropertyChanged += OnNodeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged += OnViewerDataChanged;
        }

        if (_globalsData != null)
        {
            _globalsData.PropertyChanged += OnGlobalsDataChanged;
        }
    }

    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedNodes")
        {
            // Change text color based whether node is selected
            if (_globalsData.SelectedNodes.Contains(_nodeData.Id) && _displayNameTMP != null)
            {
                _displayNameTMP.color = Color.blue;
            }
            else
            {
                _displayNameTMP.color = Color.black;
            }
        }
    }

    private void OnNodeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateNodePresentation();
    }

    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "LayoutNodes" || e.PropertyName == "ZoomScale" || e.PropertyName == "Scale")
        {
            UpdateNodePresentation();
        }
    }

    // Updates the node's visual representation.
    private void UpdateNodePresentation()
    {
        // Update the position
        Vector3 nodeViewerPosition = new Vector3(0, 0, 0);
        var layoutNodes = _attachedViewerData?.LayoutNodes;

        if (layoutNodes != null)
        {
            if ((bool)(layoutNodes?.TryGetValue(_nodeData.Id, out nodeViewerPosition)))
            {
                //Debug.Log($"Updating local position {_nodeData.Id} {nodeViewerPosition.x} {nodeViewerPosition.y}");
                transform.SetLocalPositionAndRotation(nodeViewerPosition, transform.localRotation);
            }
        }

        // Set Display name
        if (_nodeData?.DisplayName != null && _displayNameTMP != null)
        {
            _displayNameTMP.SetText(_nodeData.DisplayName);
        }

        // Set the image texture
        if (_nodeGameObject != null)
        {
            Vector3 viewerScale = new Vector3(0, 0, 0);
            Vector3 zoomScale = new Vector3(0, 0, 0);
            Transform displayNameTransform;
            float triggerPercentageOfNodeInView = 0.3f;

            if (_attachedViewerData?.Scale != null && _attachedViewerData?.ZoomScale != null)
            {
                viewerScale = (Vector3)_attachedViewerData.Scale;
                zoomScale = (Vector3)_attachedViewerData.ZoomScale;
            }

            if (zoomScale.x <= (viewerScale.x / _nodeData.UnitWidth) * triggerPercentageOfNodeInView)
            {
                // Remove diagram texture
                _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", null);

                // Place label in center
                _displayNameTMP.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
                _displayNameTMP.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, 0, _nodeData.UnitDepth), _displayNameTMP.gameObject.transform.localRotation);
            }
            else
            {
                // Place label on top
                _displayNameTMP.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, _nodeData.UnitHeight, _nodeData.UnitDepth), _displayNameTMP.gameObject.transform.localRotation);
                _displayNameTMP.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom;

                if (_nodeData?.ImageTexture != null)
                {
                    //TODO: Fix that texture maintains aspect ratio ratio
                    _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", _nodeData.ImageTexture);
                }
            }

            // Update the local scale
            _nodeGameObject.transform.localScale = new Vector3(_nodeData.UnitWidth, _nodeData.UnitHeight, 1);
        }

    }
}
