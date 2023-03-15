using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class GOViewer : MonoBehaviour
{
    public string Id { get; internal set; }
    public GameObject nodePrefab;
    public GameObject edgePrefab;
    private ObservableCollection<PTNode> _nodesData;
    private Dictionary<string, GameObject> _nodesPrefabGo;

    private float _debugLocationOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionaries
        _nodesData = new();
        _nodesPrefabGo = new();

        InitViewer();
    }

    /// <summary>
    /// Inializes a Viewer Instance
    /// </summary>
    void InitViewer()
    {
        _nodesData = Repository.Instance.GetNodes();
        _nodesData.CollectionChanged += OnNodesDataChanged;
        SpawnNodes(_nodesData.Cast<PTNode>().ToList());
    }

    /// <summary>
    /// Update when the nodes data collection has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnNodesDataChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Spawn new Items
        if (e.NewItems is not null)
        {
            SpawnNodes(e.NewItems.Cast<PTNode>().ToList());
        }

        // Remove old items
        if (e.OldItems is not null)
        {
            foreach(PTNode nodeData in e.OldItems)
            {
                DestroyNode(nodeData.Id);
            }
        }
    }

    /// <summary>
    /// Spawn nodes in the Viewer.
    /// </summary>
    /// <param name="node"></param>
    void SpawnNodes(List<PTNode> nodesData)
    {
        foreach(PTNode nodeData in nodesData)
        {
            SpawnNode(nodeData);
        }
    }

    /// <summary>
    /// Spawn a node prefab in the scene.
    /// </summary>
    /// <param name="nodeData">Data of the node.</param>
    private void SpawnNode(PTNode nodeData)
    {
        // Destroy node if already existing
        DestroyNode(nodeData.Id);    

        // Create new node
        GameObject nodePrefabGo = Instantiate(nodePrefab, new Vector3(0,0, _debugLocationOffset), Quaternion.identity);
        _debugLocationOffset += 10;
        _nodesPrefabGo[nodeData.Id] = nodePrefabGo;

        // Setup node with Node Data
        GONode nodeGo = nodePrefabGo.GetComponent<GONode>();
        nodeGo.Init(nodeData);
    }

    /// <summary>
    /// Destroy a Node in the viewer.
    /// </summary>
    /// <param name="node"></param>
    void DestroyNode(string nodeId)
    {
        GameObject nodePrefabGo;
        if (_nodesPrefabGo.TryGetValue(nodeId, out nodePrefabGo))
        {
            Destroy(nodePrefabGo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalMatrix("_WorldToBox", transform.worldToLocalMatrix);
    }

    private void OnDestroy()
    {
        //TODO: Destoy all nodes and edges e.t.c.
    }
}
