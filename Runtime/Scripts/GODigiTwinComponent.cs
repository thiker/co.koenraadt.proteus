using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using co.koenraadt.proteus.Runtime.Controllers;
using co.koenraadt.proteus.Runtime.Interfaces;
using co.koenraadt.proteus.Runtime.Repositories;
using co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Digital Twin component that used to to connect existing parts of the digital twin to Proteus. This component can be inherited from to implement custom behavior for example when the linked states changes.
/// </summary>
public class GODigiTwinComponent : MonoBehaviour, IProteusInteraction
{
    /// <summary>
    /// The name of the main diagrma that the digital twin component is linked to.
    /// </summary>
    public string MainDiagramName;

    /// <summary>
    /// The list of nodes that the digital twin component is linked to.
    /// </summary>
    public List<string> LinkedNodes;

    /// <summary>
    /// The list of states that the digital twin component is linked to.
    /// </summary>
    public List<string> LinkedStates;

    /// <summary>
    /// The opacity factor that the component will change its material when xrayed.
    /// </summary>
    public float XrayOpacityFactor = .1f;

    /// <summary>
    /// The factor that the component should move away from the explode origin when exploded.
    /// </summary>
    public float ExplodeFactor = 1.5f;

    /// <summary>
    /// Wether the component should trigger xray view.
    /// </summary>
    public bool DoXrayView = true;

    /// <summary>
    /// Whether the component should trigger exploded view. 
    /// </summary>
    public bool DoExplodedView = true;

    /// <summary>
    /// Whether the component shoud react to xray view.
    /// </summary>
    public bool ReactsToXray = true;

    /// <summary>
    /// Whether the component should react to exploded view.
    /// </summary>
    public bool ReactsToExplodedView = true;

    private string _xrayMatAddress = "Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat";
    private bool _originalRendererEnabled;
    private ObservableCollection<PTState> _statesCollection;
    private PTGlobals _globalsData;
    private Renderer _renderer;
    private Material _xrayMaterial;
    private Material _originalMaterial;
    private Vector3 _explodedViewOffset;
    private AsyncOperationHandle<Material> handle;

    /// <summary>
    /// Sets the layer of the object to proteus viz so it can react to Proteus interaction events.
    /// </summary>
    virtual protected void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("ProteusViz");
        _explodedViewOffset = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Initializes and starts the digital twin component.
    /// </summary>
    virtual protected void Start()
    {
        Debug.Log($"PROTEUS: Starting Digi Twin component{transform.name}... ");
        _globalsData = Repository.Instance.Proteus.GetGlobals();
        _statesCollection = Repository.Instance.States.GetStates();

        _statesCollection.CollectionChanged += OnStatesCollectionChanged;

        if (TryGetComponent(out _renderer))
        {
            _originalRendererEnabled = _renderer.enabled;
            _originalMaterial = new Material(_renderer.material.shader);
            _originalMaterial.CopyPropertiesFromMaterial(_renderer.material);
        } else
        {
            Debug.LogWarning($"PROTEUS: Tried to get renderer of {transform.name} DigiTwin Component but has none");
        }


        handle = Addressables.LoadAssetAsync<Material>(_xrayMatAddress);
        handle.Completed += Handle_Completed;

        // _xrayMaterial = (Material)Resources.Load("Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat", typeof(Material));
        // if (_xrayMaterial == null ) {
        //     Debug.LogError($"PROTEUS: Loaded Xray material is null");
        // }
        // Debug.Log($"Got xray material loaded resource: {_xrayMaterial}");
        DigiTwinController.Instance.LinkDigiTwinComponent(this);
    }

    /// <summary>
    /// Whenever the xray material asset is loaded, store the result as a reference for the digital twin component to use.
    /// </summary>
    /// <param name="operation"></param>
    private void Handle_Completed(AsyncOperationHandle<Material> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            _xrayMaterial = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {_xrayMatAddress} failed to load.");
        }
    }


    /// <summary>
    /// Called on every unity update.
    /// </summary>
    virtual protected void Update()
    {
    }

    /// <summary>
    /// Destroys and cleans up the digital twin component.
    /// </summary>
    virtual protected void OnDestroy()
    {
        if (_statesCollection != null)
        {
            _statesCollection.CollectionChanged -= OnStatesCollectionChanged;
        }
        DigiTwinController.Instance.UnlinkDigiTwinComponent(this);
    }

    /// <summary>
    /// Checks whether the digital twin component has a linked node that is currently selected.
    /// </summary>
    /// <returns>Returns true when one of the linked nodes is currently selected.</returns>
    public bool HasLinkedNodeInSelection()
    {
        bool isInSelection = LinkedNodes.Intersect(Repository.Instance.Proteus.GetNodeSelectionDisplayNames()).Count() > 0;
        return isInSelection;
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectNodeByName(MainDiagramName);
    }

    /// <summary>
    /// Updates the component to react to xray view. Changes the transparency of the object accordingly.
    /// </summary>
    public void UpdateXrayView()
    {
        bool isInSelection = HasLinkedNodeInSelection();

        // If no active selection disable xray
        if (!_globalsData.XrayViewEnabled || isInSelection || _globalsData.SelectedNodes.Count == 0)
        {
            if (_renderer != null)
            {
                _renderer.enabled = _originalRendererEnabled;
                _renderer.material = _originalMaterial;
            }

            return;
        }

        // Make transparent if not linked to a node in the selection
        if (isInSelection && _renderer != null)
        {

            _renderer.enabled = _originalRendererEnabled;
            _renderer.material = _originalMaterial;
        }
        else if (_globalsData.XrayViewEnabled && ReactsToXray && _renderer != null)
        {
            _renderer.material = _xrayMaterial;
            _renderer.material.color = new Color(1f, 1f, 1f, XrayOpacityFactor);

            if (XrayOpacityFactor == 0)
            {
                _renderer.enabled = false;
            }
        }
    }

    /// <summary>
    /// Updates the component to react to exploded view. Calculates its new location and offset accordingly.
    /// </summary>
    /// <param name="origin">The origin of the explosion.</param>
    /// <param name="isExploded">Whether the component should explode.</param>
    public void UpdateExplodedView(Vector3 origin, bool isExploded)
    {
        transform.position -= _explodedViewOffset;
        _explodedViewOffset = new Vector3(0, 0, 0);

        if (ReactsToExplodedView && isExploded && _globalsData.ExplodedViewEnabled)
        {
            Vector3 direction = (transform.position - origin).normalized;
            _explodedViewOffset += direction * ExplodeFactor;
            transform.position += _explodedViewOffset;
        }
    }

    /// <summary>
    /// Ensure that the digital twin component maintans a reference to the states it is linked to.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private void OnStatesCollectionChanged(object obj, NotifyCollectionChangedEventArgs e)
    {
        // Unlink removed
        if (e?.OldItems != null)
        {
            foreach (PTState oldState in e.OldItems)
            {
                if (LinkedStates.Contains(oldState.Id))
                {
                    oldState.PropertyChanged -= OnStateDataChanged;
                }
            }
        }

        // Link new
        if (e?.NewItems != null)
        {
            foreach (PTState newState in e.NewItems)
            {
                if (LinkedStates.Contains(newState.Id))
                {
                    newState.PropertyChanged -= OnStateDataChanged;
                    newState.PropertyChanged += OnStateDataChanged;
                }
            }
        }
    }

    protected void OnStateDataChanged(object obj, PropertyChangedEventArgs e)
    {
        OnStateDataChanged((PTState)obj, e);
    }

    /// <summary>
    /// Method called whenever the state changes for an object that the digital twin component is linked to. Can be overridenn to define custom behavior on state changes.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    virtual protected void OnStateDataChanged(PTState obj ,PropertyChangedEventArgs e)
    {

    }

}
