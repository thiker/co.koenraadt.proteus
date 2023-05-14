using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Other;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class GOVizController : MonoBehaviour
{
    public GameObject ViewerPrefab;
    private PTGlobals _globalsData;
    private ObservableCollection<PTViewer> _viewersData;
    private Dictionary<string, GameObject> _viewerPrefabGOs;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize 
        _viewersData = new();
        _viewerPrefabGOs = new();

        // Get the data
        _viewersData = Repository.Instance.Viewers.GetViewers();
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        linkEventListeners();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool pointerDown = Input.GetMouseButtonDown(0);
        bool pointerAltDown = Input.GetMouseButtonDown(1);
        bool pointerTertiaryDown = Input.GetMouseButtonDown(2);
        bool pointerUp = Input.GetMouseButtonUp(0);
        bool pointerAltUp = Input.GetMouseButtonUp(1);
        bool pointerTertiaryUp = Input.GetMouseButtonUp(2);
        bool pointerMove = Mathf.Abs(mouseX) > 0 || Mathf.Abs(mouseY) > 0;

        if (pointerDown || pointerAltDown || pointerTertiaryDown || pointerUp || pointerAltUp || pointerTertiaryUp || pointerMove )
        {
            hits = Helpers.RayCastProteusViz();

            // When no hits are registered on a pointer down event clear the selections.
            if (hits.Length <= 0 && pointerDown)
            {
                Repository.Instance.Proteus.ClearViewerSelection();
                Repository.Instance.Proteus.ClearNodeSelection();
            }

            foreach (RaycastHit hit in hits)
            {
                IProteusInteraction interactionComp = Helpers.FindInteractableComponentInParent(hit.collider.gameObject);

                if (pointerDown)
                {
                    interactionComp?.OnPointerDown(hit);
                }

                if (pointerAltDown)
                {
                    interactionComp?.OnPointerAltDown(hit);
                }

                if (pointerTertiaryDown)
                {
                    interactionComp?.OnPointerTertiaryDown(hit);
                }

                if (pointerUp)
                {
                    interactionComp?.OnPointerUp(hit);
                }

                if (pointerAltUp)
                {
                    interactionComp?.OnPointerAltUp(hit);
                }

                if (pointerTertiaryUp)
                {
                    interactionComp?.OnPointerTertiaryUp(hit);
                }

                if (pointerMove)
                {
                    interactionComp?.OnPointerMove(hit);
                }
            }
        }
    }



    void OnDestroy()
    {
        _viewersData.CollectionChanged -= OnViewersDataChanged;
        _globalsData.PropertyChanged -= OnGlobalsDataChanged;
    }

    private void linkEventListeners()
    {
        _viewersData.CollectionChanged += OnViewersDataChanged;
        _globalsData.PropertyChanged += OnGlobalsDataChanged;
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
    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "SelectedNodes":
                {
                    // Update digital twin viz
                    break;
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
        GameObject viewerPrefabGO;
        if (viewerData.Position is not null)
        {
            viewerPrefabGO = Instantiate(ViewerPrefab, (Vector3)viewerData.Position, Quaternion.identity, transform);
        }
        else
        {
            throw new System.Exception("PROTEUS: Error, tried instantiating viewer but position is null");
        }

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
}
