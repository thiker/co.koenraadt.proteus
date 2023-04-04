using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class GOProteus : MonoBehaviour
{
    public GameObject ViewerPrefab;
    private ObservableCollection<PTViewer> _viewersData;
    private Dictionary<string, GameObject> _viewerPrefabGOs;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        _viewersData = new();
        _viewerPrefabGOs = new();

        // Get the viewers data
        _viewersData = Repository.Instance.Viewers.GetViewers();

        linkEventListeners();
    }

    private void linkEventListeners()
    {
        _viewersData.CollectionChanged += OnViewersDataChanged;
    }



    /// <summary>
    /// Update when the nodes data collection has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnViewersDataChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Spawn new Items
        if (e.NewItems is not null)
        {
            SpawnViewers(e.NewItems.Cast<PTViewer>().ToList());
        }

        // Remove old items
        if (e.OldItems is not null)
        {
            foreach (PTViewer viewerData in e.OldItems)
            {
                DestroyViewer(viewerData.Id);
            }
        }
    }

    /// <summary>
    /// Spawn nodes in the Viewer.
    /// </summary>
    /// <param name="node"></param>
    private void SpawnViewers(List<PTViewer> viewersData)
    {
        foreach (PTViewer viewerData in viewersData)
        {
            SpawnViewer(viewerData);
        }
    }

    /// <summary>
    /// Spawn a node prefab in the scene.
    /// </summary>
    /// <param name="nodeData">Data of the node.</param>
    private void SpawnViewer(PTViewer viewerData)
    {
        // Destroy node if already existing
        DestroyViewer(viewerData.Id);

        // Create new node
        GameObject viewerPrefabGO = Instantiate(ViewerPrefab, viewerData.Position , Quaternion.identity, transform);
        _viewerPrefabGOs[viewerData.Id] = viewerPrefabGO;

        // Setup node with Node Data
        GOViewer viewerGO = viewerPrefabGO.GetComponent<GOViewer>();
        viewerGO.Init(viewerData.Id);
    }

    /// <summary>
    /// Destroy a Node in the viewer.
    /// </summary>
    /// <param name="node"></param>
    private void DestroyViewer(string viewerId)
    {
        GameObject viewerPrefabGO;
        if (_viewerPrefabGOs.TryGetValue(viewerId, out viewerPrefabGO))
        {
            Destroy(viewerPrefabGO);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
