using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

/// <summary>
/// Component that implements and handle the behvior of a Proteus viewer.
/// </summary>
public class GOViewer : MonoBehaviour, IProteusInteraction
{
    /// <summary>
    /// Whether the viewer should be detached from the visualization controller. Used in the VR experiment. Detached viewers are not automatically placed.
    /// </summary>
    public bool Detached = false;

    /// <summary>
    /// The list of ids for the root nodes of the viewer. The viewer will visualize the hierarchy of all nodes that are descendants of this root node.
    /// </summary>
    public string[] RootNodeIds;

    /// <summary>
    /// The Id of the viewer that can be explicitely set when the viewer is detached.
    /// </summary>
    public string Id { get; internal set; }
    public string ViewerId = null;

    /// <summary>
    /// Reference to the NodePrefab that the viewer instantiates for each node.
    /// </summary>
    public GameObject NodePrefab;

    /// <summary>
    /// Reference to the EdgePrefab that the viewer instantiates for each edge.
    /// </summary>
    public GameObject EdgePrefab;

    private PTViewer _viewerData;
    private string _linkedViewerId;
    private GameObject _viewerContainer;
    private GameObject _modelAnchor;
    private GameObject _viewWindow;
    private List<GameObject> _viewWindowBorders;
    private PTGlobals _globalsData;
    private ObservableCollection<PTNode> _nodesData;
    private ObservableCollection<PTEdge> _edgesData;
    private Dictionary<string, GameObject> _nodePrefabGOs;
    private Dictionary<string, GameObject> _edgePrefabGOs;



    /// <summary>
    /// Initializes the viewer and sets its related viewer id.
    /// </summary>
    /// <param name="viewerId">The id of the viewer.</param>
    public void Init(string viewerId)
    {
        _linkedViewerId = viewerId;
    }

    /// <summary>
    /// Initializes and starts the viewer. This will obtain references to the gameobjects used by the viewer and spawns the nodes and edges used to visualize the viewer's layout.
    /// </summary>
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
        _viewerContainer = transform.Find("ViewerContainer").gameObject;
        _modelAnchor = _viewerContainer.transform.Find("ModelAnchor").gameObject;
        _viewWindow = _viewerContainer.transform.Find("ViewWindow").gameObject;
        _viewWindow.GetComponent<GOViewWindow>().Init(_viewerData.Id);

        _viewWindowBorders.Add(_viewerContainer.transform.Find("ViewWindowBorderBottom").gameObject);
        _viewWindowBorders.Add(_viewerContainer.transform.Find("ViewWindowBorderTop").gameObject);

        // Link viewer components
        List<IPTViewerComponent> allComponents = transform.GetComponentsInChildren<IPTViewerComponent>().ToList();
        foreach(var comp in allComponents) {
            comp.Init(_viewerData.Id);
        }

        // Get the nodes
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

    /// <summary>
    /// Links the event listeners so that the viewer is notified whenever data that the viewer relies on changes.
    /// </summary>
    private void LinkEventListeners()
    {
        _viewerData.PropertyChanged += OnViewerDataChanged;
        _globalsData.PropertyChanged += OnGlobalsDataChanged;
        _nodesData.CollectionChanged += OnNodesDataChanged;
        _edgesData.CollectionChanged += OnEdgesDataChanged;
    }

    /// <summary>
    /// On every update save the viewwindow's WorldToLocalMatrix that is used to crop the contents of the viewer inside this viewwindow. The viewer is also rotated towards the user every update if billboarding is enabled.
    /// </summary>
    void Update()
    {
        if (_viewWindow != null)
        {
            //  Update the window window world to local matrix
            Repository.Instance.Viewers.SetViewWindowWorldToLocal(_viewerData.Id, _viewWindow.transform.worldToLocalMatrix);
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

    /// <summary>
    /// Whenever the viewer is destroyed, clean up all data change listeners.
    /// </summary>
    void OnDestroy()
    {
        _viewerData.PropertyChanged -= OnViewerDataChanged;
        _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        _nodesData.CollectionChanged -= OnNodesDataChanged;
        _edgesData.CollectionChanged -= OnEdgesDataChanged;
    }

    /// <summary>
    /// Select the viewer whenever its clicked.
    /// </summary>
    /// <param name="hit">The raycast result from the interaction.</param>
    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectViewer(_viewerData.Id);
    }

    /// <summary>
    /// Whenever the viewer's data changes, the viewer is updated so the visualization reflects the current state. For example, whenever the layout changes, edges and nodes are updated and spawned / removed accordingly.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Whenever the viewer selection changes, update the viewer's presentation accordingly to reflect if it is selected.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SelectedViewers")
        {
            UpdateViewerPresentation();
        }
    }

    /// <summary>
    /// Spawns the edge prefabs that are used to create a visual representation of the edges in the model.
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


    //TODO: Maybe only regenerate view layouts on repo level.
    /// <summary>
    /// Regenerate the viewer's layout when the edges data collection has changed.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnEdgesDataChanged(object obj, NotifyCollectionChangedEventArgs e)
    {
        // Regenerate the viewer's layout
        Repository.Instance.Viewers.RegenerateViewerLayout(_viewerData.Id);
    }

    //TODO: Maybe only regenerate view layouts on repo level.
    /// <summary>
    /// Regenerate the viewer's layout when the nodes data collection has changed.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnNodesDataChanged(object obj, NotifyCollectionChangedEventArgs e)
    {
        // Regenerate the viewer's layout
        Repository.Instance.Viewers.RegenerateViewerLayout(_viewerData.Id);
    }

    /// <summary>
    /// Spawns the node prefabs that are used to create a visual representation of the edges in the model.
    /// </summary>
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
    /// Spawns a node prefab in the scene and links it to the viewer.
    /// </summary>
    /// <param name="nodeData">Data of the node to add.</param>
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
    /// Spawn an edge prefab in the scene and links it to the viewer.
    /// </summary>
    /// <param name="edgeData">Data of the edge to add.</param>
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
    /// <param name="id">Id of the node to destroy.</param>
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
    /// <param name="id">id of the edge to destroy.</param>
    private void DestroyEdge(string id)
    {
        GameObject edgePrefabGo;
        if (_edgePrefabGOs.TryGetValue(id, out edgePrefabGo))
        {
            Destroy(edgePrefabGo);
        }
    }

    /// <summary>
    /// Updates the position of the viewer's model anchor to set its position and rotation to the offset that is controlled by the user when the user pans the viewer.
    /// </summary>
    private void UpdateModelAnchorOffsetPresentation()
    {
        // Update the view windows offset
        if (_viewerData.ModelAnchorOffset != null)
        {
            _modelAnchor.transform.SetLocalPositionAndRotation((Vector3)_viewerData.ModelAnchorOffset, _modelAnchor.transform.localRotation);
        }

    }

    /// <summary>
    /// Updates the viewer's presentation, such as scale and zoom level.
    /// </summary>
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
        if (_viewerData.Position != null && _viewerData.Rotation != null && !Detached)
        {
            // Set the main position
            transform.position = (Vector3)_viewerData.Position;
            
            // Set the rotation of the container
            _viewerContainer.transform.rotation = (Quaternion)_viewerData.Rotation;

        }

        //TODO: Maybe Refactor to only run on selection change
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
