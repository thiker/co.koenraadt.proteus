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

public class GONode : MonoBehaviour
{
    private PTNode _nodeData;
    private TextMeshPro _displayNameTMP;
    private GameObject _nodeGameObject;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _displayNameObj = transform.Find("DisplayNameText").gameObject;
        _nodeGameObject = transform.Find("Node").gameObject;
        _displayNameTMP = _displayNameObj.GetComponent<TextMeshPro>();

        // Set presentation
        UpdateNodePresentation(_nodeData);
    }

    public void Init(PTNode nodeData)
    {
        _nodeData = nodeData;
        _nodeData.PropertyChanged += OnNodeDataChanged;
    }

    private void OnNodeDataChanged(object obj, PropertyChangedEventArgs e)  
    {
        UpdateNodePresentation((PTNode)obj);
    }

    private void UpdateNodePresentation(PTNode nodeData)
    {
        if (nodeData?.DisplayName != null && _displayNameTMP != null)
        {
            _displayNameTMP.SetText(nodeData.DisplayName);
        }

        if (nodeData?.ImageTexture != null && _nodeGameObject != null)
        {
            float ratio = nodeData.ImageTexture.width / nodeData.ImageTexture.height;

            _nodeGameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", nodeData.ImageTexture);
            if (ratio >= 1)
            {
                _nodeGameObject.transform.localScale = new Vector3(5 * ratio, 5, 1);
            } else
            {
                _nodeGameObject.transform.localScale = new Vector3(5,5 * ratio, 1);
            }

        } 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        _nodeData.PropertyChanged -= OnNodeDataChanged;
    }
}
