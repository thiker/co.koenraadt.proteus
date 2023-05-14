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

public class GONode : MonoBehaviour, IProteusInteraction
{
    private string _nodeId;
    private string _attachedViewerId;

    private PTNode _nodeData;
    private PTViewer _attachedViewerData;

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

        // Get node data
        _nodeData = Repository.Instance.Models.GetNodeById(_nodeId);

        // Get viewer data of node
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);

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
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectNode(_nodeData.Id);
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
    }


    private void OnNodeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateNodePresentation();
    }

    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "LayoutNodes")
        {
            UpdateNodePresentation();
        }
    }

    // Updates the node's visual representation.
    private void UpdateNodePresentation()
    {
        // Update the position
        Vector3 nodeViewerPosition = new Vector3(0,0,0);
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
        if (_nodeData?.ImageTexture != null && _nodeGameObject != null)
        {
            
            //TODO: FIx ratio
            float ratio = _nodeData.ImageTexture.width / _nodeData.ImageTexture.height; // Ratio between heigth and width of the image
            ratio = 1;
            _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", _nodeData.ImageTexture);

            //TODO: Check the magic constant 5?
            if (ratio >= 1)
            {
                _nodeGameObject.transform.localScale = new Vector3(_nodeData.UnitWidth * ratio, _nodeData.UnitHeight, 1);
            }
            else
            {
                _nodeGameObject.transform.localScale = new Vector3(_nodeData.UnitWidth, _nodeData.UnitHeight * ratio, 1);
            }

        }
    }
}
