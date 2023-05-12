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

public class GOEdge : MonoBehaviour
{
    private string _edgeId;
    private string _attachedViewerId;

    private PTEdge _edgeData;
    private PTViewer _attachedViewerData;

    // Initialize the node
    public void Init(string edgeId, string attachedViewerId)
    {
        _edgeId = edgeId;
        _attachedViewerId = attachedViewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get Edge data
        _edgeData = Repository.Instance.Models.GetEdgeById(_edgeId);

        // Get viewer data of node
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);

        // initialize the event listeners
        linkEventListeners();

        // Set presentation
        UpdateEdgePresentation();
    }

    // Update is called once per frame
    void Update()
    {
    }



    void OnDestroy()
    {
        if (_edgeData != null)
        {
            _edgeData.PropertyChanged -= OnEdgeDataChanged;
        }
    }

    private void linkEventListeners()
    {
        if (_edgeData != null)
        {
            _edgeData.PropertyChanged += OnEdgeDataChanged;
        }
    }


    private void OnEdgeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateEdgePresentation();
    }

    // Updates the node's visual representation.
    private void UpdateEdgePresentation()
    {
    }
}
