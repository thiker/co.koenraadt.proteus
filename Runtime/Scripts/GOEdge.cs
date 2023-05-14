using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.ComponentModel;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using UnityEngine.Splines;
using System.Collections.Generic;

public class GOEdge : MonoBehaviour
{
    private string _edgeId;
    private string _attachedViewerId;
    private PTEdge _edgeData;
    private PTViewer _attachedViewerData;
    private GameObject _splineGameObject;
    private SplineContainer _splineContainerComponent;
    private MaterialPropertyBlock _matPropBlock;

    // Initialize the node
    public void Init(string edgeId, string attachedViewerId)
    {
        _edgeId = edgeId;
        _attachedViewerId = attachedViewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
        _matPropBlock = new MaterialPropertyBlock();

        // Get Edge data
        _edgeData = Repository.Instance.Models.GetEdgeById(_edgeId);

        // Get viewer data of node
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);

        // Get the spline game object
        _splineGameObject = transform.Find("Spline").gameObject;
        _splineContainerComponent = _splineGameObject.GetComponent<SplineContainer>();

        // Decouple the mesh so its unique for the instance
        MeshFilter meshFilter = _splineGameObject.GetComponent<MeshFilter>();
        Mesh duplicatedMesh = meshFilter.mesh;

        // initialize the event listeners
        linkEventListeners();

        // Set presentation
        UpdateEdgePresentation();
    }

    // Update is called once per frame
    void Update()
    {
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = _splineGameObject.GetComponentInChildren<Renderer>();
        //set the matrix
        if (_attachedViewerData?.ViewWindowWorldToLocal is not null)
            _matPropBlock.SetMatrix("_WorldToBox", (Matrix4x4)_attachedViewerData.ViewWindowWorldToLocal);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(_matPropBlock);
    }



    void OnDestroy()
    {
        if (_edgeData != null)
        {
            _edgeData.PropertyChanged -= OnEdgeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged -= OnViewerDataChanged;
        }
    }
    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "LayoutNodes")
        {
            UpdateEdgePresentation();
        }
    }

    private void linkEventListeners()
    {
        if (_edgeData != null)
        {
            _edgeData.PropertyChanged += OnEdgeDataChanged;
        }

        if (_attachedViewerData != null)
        {
            _attachedViewerData.PropertyChanged -= OnViewerDataChanged;
        }
    }


    private void OnEdgeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateEdgePresentation();
    }

    // Updates the edge's visual representation.
    private void UpdateEdgePresentation()
    {
        List<Spline> splines = _attachedViewerData.LayoutEdges[_edgeData.Id];

        Debug.Log($"Updating edge presentation of {_edgeData.Id}");
        foreach (Spline spline in splines)
        {
            _splineContainerComponent.AddSpline(spline);
        }
    }
}
