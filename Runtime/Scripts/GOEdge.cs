using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.ComponentModel;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using UnityEngine.Splines;
using System.Collections.Generic;

/// <summary>
/// Component that handles the behavior of the Edges that are used in the viewer to visually represent the edges in the 3DML formatted model.
/// </summary>
public class GOEdge : MonoBehaviour
{
    private string _edgeId;
    private string _attachedViewerId;
    private PTEdge _edgeData;
    private PTViewer _attachedViewerData;
    private GameObject _splineGameObject;
    private SplineContainer _splineContainerComponent;
    private MaterialPropertyBlock _matPropBlock;

    /// <summary>
    /// Called to initialize the edge and obtain a reference to the viewer its attached to.
    /// </summary>
    /// <param name="edgeId">The id of the edge that the component is linked to.</param>
    /// <param name="attachedViewerId">The id of the viewer that the edge component is attached to.</param>
    public void Init(string edgeId, string attachedViewerId)
    {
        _edgeId = edgeId;
        _attachedViewerId = attachedViewerId;
    }

    /// <summary>
    /// Starts and initializes the edge. Obtains reference to the gameobjects in the edge prefab that are used to visualize the edge.
    /// </summary>
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
        LinkEventListeners();

        // Set presentation
        UpdateEdgePresentation();
    }

    /// <summary>
    /// Updatest the shader of the edge with the attached viewer's world to local matrix so the edge is croppped to the viewwindow.
    /// </summary>
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

    /// <summary>
    /// Destroys the edge and clears listeners and reference to the repository that it created to obtain edge data.
    /// </summary>
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

    /// <summary>
    /// Updates the edges presentation whenever the viewer's data changes.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "LayoutNodes")
        {
            UpdateEdgePresentation();
        }
    }

    /// <summary>
    /// Links the event listeners that the edge component uses to listen to data changed events from the repository.
    /// </summary>
    private void LinkEventListeners()
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

    /// <summary>
    /// Called whenever the edges data changes and then calls the function to update the edge's presentation.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnEdgeDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateEdgePresentation();
    }

    /// <summary>
    /// Updates the edge's visual representation.
    /// </summary>
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
