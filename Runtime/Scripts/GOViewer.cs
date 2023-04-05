using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class GOViewer : MonoBehaviour
{
    public string Id { get; internal set; }
    public GameObject NodePrefab;
    public GameObject EdgePrefab;
    private GameObject _modelAnchor;
    private GameObject _viewWindow;
    private PTViewer _viewerData;
    private ObservableCollection<PTNode> _nodesData;
    private Dictionary<string, GameObject> _nodePrefabGOs;

    private float _debugLocationOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"PROTEUS: Starting viewer {_viewerData.Id}");

        // Initialize dictionaries
        _nodesData = new();
        _nodePrefabGOs = new();

        // Get the game objects
        _modelAnchor = transform.Find("ModelAnchor").gameObject;
        _viewWindow = transform.Find("ViewWindow").gameObject;

        // Get the nodes
        _nodesData = Repository.Instance.Models.GetNodes();

        // Link event listeners
        linkEventListeners();

        // Spawn the nodes in the viewer
        SpawnNodes(_nodesData.Cast<PTNode>().ToList());
    }

    /// <summary>
    /// Inializes a Viewer Instance
    /// </summary>
    public void Init(string viewerId)
    {
        _viewerData = Repository.Instance.Viewers.GetViewerById(viewerId);
    }

    private void linkEventListeners()
    {
        _viewerData.PropertyChanged += OnViewerDataChanged;
        _nodesData.CollectionChanged += OnNodesDataChanged;
    }


    private void OnViewerDataChanged(object obj, PropertyChangedEventArgs e)
    {
        UpdateViewerPresentation();
    }


    /// <summary>
    /// Update when the nodes data collection has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNodesDataChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Spawn new Items
        if (e.NewItems is not null)
        {
            SpawnNodes(e.NewItems.Cast<PTNode>().ToList());
        }

        // Remove old items
        if (e.OldItems is not null)
        {
            foreach (PTNode nodeData in e.OldItems)
            {
                DestroyNode(nodeData.Id);
            }
        }
    }

    /// <summary>
    /// Spawn nodes in the Viewer.
    /// </summary>
    /// <param name="node"></param>
    private void SpawnNodes(List<PTNode> nodesData)
    {
        foreach (PTNode nodeData in nodesData)
        {
            SpawnNode(nodeData.Id);
        }
    }

    /// <summary>
    /// Spawn a node prefab in the scene.
    /// </summary>
    /// <param name="nodeData">Data of the node.</param>
    private void SpawnNode(string nodeId)
    {
        // Destroy node if already existing
        DestroyNode(nodeId);

        // Create new node
        GameObject nodePrefabGo = Instantiate(NodePrefab, _modelAnchor.transform, false);
        nodePrefabGo.transform.localPosition = new Vector3(_debugLocationOffset, 0, 0);
        _debugLocationOffset += 5.5f;
        _nodePrefabGOs[nodeId] = nodePrefabGo;

        // Setup node with Node Data
        GONode nodeGo = nodePrefabGo.GetComponent<GONode>();
        nodeGo.Init(nodeId, _viewerData.Id);
    }

    /// <summary>
    /// Destroy a Node in the viewer.
    /// </summary>
    /// <param name="node"></param>
    private void DestroyNode(string nodeId)
    {
        GameObject nodePrefabGo;
        if (_nodePrefabGOs.TryGetValue(nodeId, out nodePrefabGo))
        {
            Destroy(nodePrefabGo);
        }
    }

    private void UpdateViewerPresentation()
    {
        if (_viewerData.Position is not null && _viewerData.Rotation is not null)
        {
            transform.SetPositionAndRotation((Vector3)_viewerData.Position, (Quaternion)_viewerData.Rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_viewWindow != null)
        {
            Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = _viewerData.Id, ViewWindowWorldToLocal = _viewWindow.transform.worldToLocalMatrix });
        }
    }

    private void OnDestroy()
    {
        _viewerData.PropertyChanged -= OnViewerDataChanged;
        _nodesData.CollectionChanged -= OnNodesDataChanged;
    }
}
