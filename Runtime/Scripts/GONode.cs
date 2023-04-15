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
    }


    private void OnNodeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateNodePresentation();
    }

    // Updates the node's visual representation.
    private void UpdateNodePresentation()
    {
        if (_nodeData?.DisplayName != null && _displayNameTMP != null)
        {
            _displayNameTMP.SetText(_nodeData.DisplayName);
        }

        if (_nodeData?.ImageTexture != null && _nodeGameObject != null)
        {
            float ratio = _nodeData.ImageTexture.width / _nodeData.ImageTexture.height;

            _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", _nodeData.ImageTexture);
            if (ratio >= 1)
            {
                _nodeGameObject.transform.localScale = new Vector3(5 * ratio, 5, 1);
            }
            else
            {
                _nodeGameObject.transform.localScale = new Vector3(5, 5 * ratio, 1);
            }

        }
    }
}
