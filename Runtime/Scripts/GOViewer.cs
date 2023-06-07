using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class GOViewer : MonoBehaviour, IProteusInteraction
{
    public bool Detached = false;
    public string[] RootNodeIds;
    public string Id { get; internal set; }
    public string ViewerId = null;
    private string _linkedViewerId;

    public GameObject NodePrefab;
    public GameObject EdgePrefab;
    private GameObject _modelAnchor;
    private GameObject _viewWindow;
    private List<GameObject> _viewWindowBorders;
    PTViewer _viewerData;
    private PTGlobals _globalsData;
    private ObservableCollection<PTNode> _nodesData;
    private ObservableCollection<PTEdge> _edgesData;
    private Dictionary<string, GameObject> _nodePrefabGOs;
    private Dictionary<string, GameObject> _edgePrefabGOs;



    /// <summary>
    /// Inializes a Viewer Instance
    /// </summary>
    public void Init(string viewerId)
    {
        _linkedViewerId = viewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Detached) {
            _linkedViewerId = ViewerId;
             Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = _linkedViewerId, Detached=Detached, RootNodeIds = RootNodeIds , Scale = transform.localScale, Position = transform.position, Rotation = transform.rotation});
        }

        _viewerData = Repository.Instance.Viewers.GetViewerById(_linkedViewerId);
        // Initialize dictionaries
        _nodesData = new();
        _edgesData = new();
        _nodePrefabGOs = new();
        _edgePrefabGOs = new();
        _viewWindowBorders = new();

        // Get the game objects
        _modelAnchor = transform.Find("ModelAnchor").gameObject;
        _viewWindow = transform.Find("ViewWindow").gameObject;
        _viewWindow.GetComponent<GOViewWindow>().Init(_viewerData.Id);

        _viewWindowBorders.Add(transform.Find("ViewWindowBorderBottom").gameObject);
        _viewWindowBorders.Add(transform.Find("ViewWindowBorderTop").gameObject);

        // Link viewer components
        List<IPTViewerComponent> allComponents = transform.GetComponentsInChildren<IPTViewerComponent>().ToList();
        foreach(var comp in allComponents) {
            comp.Init(_viewerData.Id);
        }

        // Get the nodes
        //TODO: Refactor so that nodes data in viewer is based on nodesLayout instead of global.
        _nodesData = Repository.Instance.Models.GetNodes();
        _edgesData = Repository.Instance.Models.GetEdges();
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        // Link event listeners
        LinkEventListeners();

        // Spawn objects
        SpawnNodes();
        SpawnEdges();

        UpdateViewerPresentation();
        UpdateModelAnchorOffsetPresentation();

    }

    void Update()
    {
        if (_viewWindow != null)
        {
            //  Update the window window world to local matrix
            Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = _viewerData.Id, ViewWindowWorldToLocal = _viewWindow.transform.worldToLocalMatrix });
        }

        if ((bool)_viewerData.IsBillboarding)
        {
            // Calculates the billboarding rotation
            if (Camera.main != null && transform != null)
            {
                Vector3 relativePos = Camera.main.transform.position - transform.position; // the relative position
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                Repository.Instance.Viewers.SetViewerRotation(_viewerData.Id, rotation);
            }

        }
    }

    void OnDestroy()
    {
        _viewerData.PropertyChanged -= OnViewerDataChanged;
        _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        _nodesData.CollectionChanged -= OnNodesDataChanged;
        _edgesData.CollectionChanged -= OnEdgesDataChanged;
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectViewer(_viewerData.Id);
    }

    private void LinkEventListeners()
    {
        _viewerData.PropertyChanged += OnViewerDataChanged;
        _globalsData.PropertyChanged += OnGlobalsDataChanged;
        _nodesData.CollectionChanged += OnNodesDataChanged;
        _edgesData.CollectionChanged += OnEdgesDataChanged;
    }

    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ModelAnchorOffset" || e.PropertyName == "Zoom" || e.PropertyName == "MaxZoom" || e.PropertyName == "MinZoom")
        {
            UpdateModelAnchorOffsetPresentation();
        }

        if (e.PropertyName == "LayoutEdges")
        {
            SpawnEdges();
        }

        if (e.PropertyName == "LayoutNodes")
        {
            SpawnNodes();
        }

        UpdateViewerPresentation();
    }

    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedViewers")
        {
            UpdateViewerPresentation();
        }
    }

    /// <summary>
    /// When the layout of the edges has changed update the presentation.
    /// </summary>
    private void SpawnEdges()
    {
        List<string> edgeIds = new();
        List<string> edgesToRemove;

        List<PTEdge> relatedEdges = Repository.Instance.Viewers.GetRelatedEdgesOfViewer(_viewerData.Id);

        Debug.Log($"Found {relatedEdges.Count} related edges");

        foreach (PTEdge edge in relatedEdges)
        {
            edgeIds.Add(edge.Id);

            if (!_edgePrefabGOs.ContainsKey(edge.Id))
            {
                SpawnEdge(edge);
            }
        }

        edgesToRemove = _edgePrefabGOs.Keys.Except(edgeIds).ToList();

        // Remove dangling edges.
        foreach (string id in edgesToRemove)
        {
            DestroyEdge(id);
        }
    }


    //TODO: Only regenerate view layouts on repo level.
    private void OnEdgesDataChanged(object obj, NotifyCollectionChangedEventArgs e)
    {
        // Regenerate the viewer's layout
        Repository.Instance.Viewers.RegenerateViewerLayout(_viewerData.Id);
    }


    /// <summary>
    /// Update when the nodes data collection has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNodesDataChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Regenerate the viewer's layout
        Repository.Instance.Viewers.RegenerateViewerLayout(_viewerData.Id);
    }

    /// <summary>
    /// Spawn nodes in the Viewer.
    /// </summary>
    /// <param name="node"></param>
    private void SpawnNodes()
    {
        List<string> nodeIds = new();
        List<string> nodesToRemove;

        List<PTNode> relatedNodes = Repository.Instance.Viewers.GetRelatedNodesOfViewer(_viewerData.Id);

        Debug.Log($"Found {relatedNodes.Count} related edges");

        foreach (PTNode node in relatedNodes)
        {
            nodeIds.Add(node.Id);

            if (!_nodePrefabGOs.ContainsKey(node.Id))
            {
                SpawnNode(node);
            }
        }

        nodesToRemove = _nodePrefabGOs.Keys.Except(nodeIds).ToList();

        // Remove dangling edges.
        foreach (string id in nodesToRemove)
        {
            DestroyNode(id);
        }
    }

    /// <summary>
    /// Spawn a node prefab in the scene.
    /// </summary>
    /// <param name="nodeData">Data of the node.</param>
    private void SpawnNode(PTNode node)
    {
        // Destroy node if already existing
        DestroyNode(node.Id);

        // Create new node
        GameObject nodePrefabGo = Instantiate(NodePrefab, _modelAnchor.transform, false);
        _nodePrefabGOs[node.Id] = nodePrefabGo;

        // Setup node with Node Data
        GONode nodeGo = nodePrefabGo.GetComponent<GONode>();
        nodeGo.Init(node.Id, _viewerData.Id);
    }

    /// <summary>
    /// Spawn a edge prefab in the scene.
    /// </summary>
    /// <param name="edgeData">Data of the edge.</param>
    private void SpawnEdge(PTEdge edge)
    {
        // Destroy edge if already existing
        DestroyEdge(edge.Id);

        // Create new edge
        GameObject edgePrefabGo = Instantiate(EdgePrefab, _modelAnchor.transform, false);
        _edgePrefabGOs[edge.Id] = edgePrefabGo;

        // Setup edge with edge Data
        GOEdge edgeGo = edgePrefabGo.GetComponent<GOEdge>();
        edgeGo.Init(edge.Id, _viewerData.Id);
    }

    /// <summary>
    /// Destroy a Node in the viewer.
    /// </summary>
    /// <param name="id">id of the node</param>
    private void DestroyNode(string id)
    {
        GameObject nodePrefabGo;
        if (_nodePrefabGOs.TryGetValue(id, out nodePrefabGo))
        {
            Destroy(nodePrefabGo);
        }
    }

    /// <summary>
    /// Destroy a Edge in the viewer.
    /// </summary>
    /// <param name="id">id of the edge</param>
    private void DestroyEdge(string id)
    {
        GameObject edgePrefabGo;
        if (_edgePrefabGOs.TryGetValue(id, out edgePrefabGo))
        {
            Destroy(edgePrefabGo);
        }
    }

    private void UpdateModelAnchorOffsetPresentation()
    {
        // Update the view windows offset
        if (_viewerData.ModelAnchorOffset != null)
        {
            _modelAnchor.transform.SetLocalPositionAndRotation((Vector3)_viewerData.ModelAnchorOffset, _modelAnchor.transform.localRotation);
        }

    }

    private void UpdateViewerPresentation()
    {
        // Update the local scale
        if (_viewerData.Scale != null)
        {
            transform.localScale = (Vector3)_viewerData.Scale;
        }

        // Update the zoom level
        if (_viewerData.ZoomScale != null && _viewerData.Scale != null)
        {
            Vector3 zoom = (Vector3)_viewerData.ZoomScale;
            Vector3 viewerScale = (Vector3)_viewerData.Scale;

            float zoomX = (1.0f / viewerScale.x) * zoom.x;
            float zoomY = (1.0f / viewerScale.y) * zoom.y;
            float zoomZ = (1.0f / viewerScale.z) * zoom.z;

            _modelAnchor.transform.localScale = new Vector3(zoomX, zoomY, zoomZ);
        }



        // Update rotation and position
        if (_viewerData.Position != null && _viewerData.Rotation != null)
        {
            //transform.SetPositionAndRotation((Vector3)_viewerData.Position, (Quaternion)_viewerData.Rotation);
        }

        //TODO: Refactor to only run on selection change
        bool isSelectedViewer = Repository.Instance.Proteus.IsViewerSelected(_viewerData.Id);

        if (isSelectedViewer)
        {
            foreach(GameObject border in _viewWindowBorders)
            {
                border.GetComponent<Renderer>().material.color = Color.blue;
            }
        } else
        {
            //TODO: On Deselection
            foreach (GameObject border in _viewWindowBorders)
            {
                border.GetComponent<Renderer>().material.color = Color.white;
            }
        }

    }
}
