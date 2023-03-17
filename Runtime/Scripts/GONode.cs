using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class GONode : MonoBehaviour
{
    private PTNode _nodeData;
    private TextMeshPro _displayNameTMP;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _displayNameObj = transform.Find("DisplayNameText").gameObject;
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
        if (nodeData == null)
        {
            return;
        }

        _displayNameTMP.text = nodeData?.DisplayName;
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
