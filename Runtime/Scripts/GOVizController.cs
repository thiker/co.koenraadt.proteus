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

/// <summary>
/// Gameobject used to implement / control the visualization controller for Proteus. 
/// </summary>
public class GOVizController : MonoBehaviour
{
    /// <summary>
    /// The prefab the visualization controller should use to represent a viewer.
    /// </summary>
    public GameObject ViewerPrefab;


    private PTGlobals _globalsData;
    private ObservableCollection<PTViewer> _viewersData;
    private Dictionary<string, GameObject> _viewerPrefabGOs;


    /// <summary>
    /// Starts and initializes the visualiation controller.
    /// </summary>
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

    /// <summary>
    /// Links the event listeners so that the visualization is updated when the data it uses changes.
    /// </summary>
    private void linkEventListeners()
    {
        _viewersData.CollectionChanged += OnViewersDataChanged;
        _globalsData.PropertyChanged += OnGlobalsDataChanged;
    }

    /// <summary>
    /// Handles the interaction for the visualization of Proteus and calls the components that are interacted with.
    /// </summary>
    void Update()
    {
        RaycastHit[] hits;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool pointerDown = Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftControl) && ! Input.GetKey(KeyCode.LeftAlt);
        bool pointerAltDown = Input.GetMouseButtonDown(1);
        bool pointerTertiaryDown = Input.GetMouseButtonDown(2); // alternative for mobile
        bool pointerCtrlClickDown = Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl);
        bool pointerAltClickDown = Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt);
        bool pointerUp = Input.GetMouseButtonUp(0);
        bool pointerAltUp = Input.GetMouseButtonUp(1);
        bool pointerTertiaryUp = Input.GetMouseButtonUp(2);
        bool pointerCtrlClickUp = (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl)) || (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.LeftControl));
        bool pointerAltClickUp = (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftAlt)) || (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.LeftAlt)); 
        bool pointerMove = Mathf.Abs(mouseX) > 0 || Mathf.Abs(mouseY) > 0;

        if (pointerDown || pointerAltDown || pointerTertiaryDown || pointerCtrlClickDown || pointerAltClickDown || pointerUp || pointerAltUp || pointerTertiaryUp || pointerCtrlClickUp || pointerAltClickUp || pointerMove )
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

                if (pointerCtrlClickDown) {
                    interactionComp?.OnPointerCtrlClickDown(hit);
                }

                if (pointerAltClickDown) {
                    interactionComp?.OnPointerAltClickDown(hit);
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

                if (pointerCtrlClickUp) {
                    interactionComp?.OnPointerCtrlClickUp(hit);
                }

                if (pointerAltClickUp) {
                    interactionComp?.OnPointerAltClickUp(hit);
                }

                if (pointerMove)
                {
                    interactionComp?.OnPointerMove(hit);
                }
            }
        }
    }

    /// <summary>
    /// Detroy the visualization controller and unlink the event listeners for data the visualization controller uses.
    /// </summary>
    void OnDestroy()
    {
        if (_viewersData != null)
        {
            _viewersData.CollectionChanged -= OnViewersDataChanged;
        }

        if (_globalsData != null)
        {
            _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        }
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
    /// Spawns a viewer in the scene.
    /// </summary>
    /// <param name="viewerData">The data of the viewer to add.</param>
    /// <exception cref="System.Exception">Thrown when a viewer is instantiated but the position is null.</exception>
    private void SpawnViewer(PTViewer viewerData)
    {  
        Debug.Log($"PROTEUS: Spawning viewer {viewerData.Id}...");
        // Destroy node if already existing
        DestroyViewer(viewerData.Id);

        if (viewerData.Detached) {
            return;
        }     
     
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
    /// Destroys a viewer.
    /// </summary>
    /// <param name="viewerId">The id of the viewer to destroy.</param>
    private void DestroyViewer(string viewerId)
    {
        GameObject viewerPrefabGO;
        if (_viewerPrefabGOs.TryGetValue(viewerId, out viewerPrefabGO))
        {
            Destroy(viewerPrefabGO);
        }
    }
}
