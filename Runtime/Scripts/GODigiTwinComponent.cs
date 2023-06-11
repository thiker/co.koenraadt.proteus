using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GODigiTwinComponent : MonoBehaviour, IProteusInteraction
{
    private string _xrayMatAddress = "Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat";
    public string MainDiagramName;
    public List<string> LinkedNodes;
    public List<string> LinkedStates;
    public float XrayOpacityFactor = .1f;
    public float ExplodeFactor = 1.5f;
    public bool DoXrayView = true;
    public bool DoExplodedView = true;
    public bool ReactsToXray = true;
    public bool ReactsToExplodedView = true;
    private bool _originalRendererEnabled;
    private ObservableCollection<PTState> _statesCollection;
    private PTGlobals _globalsData;
    private Renderer _renderer;
    private Material _xrayMaterial;
    private Material _originalMaterial;
    private Vector3 _explodedViewOffset;
    private AsyncOperationHandle<Material> handle;

    virtual protected void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("ProteusViz");
        _explodedViewOffset = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        Debug.Log("Starting digi twin component..");
        _globalsData = Repository.Instance.Proteus.GetGlobals();
        _statesCollection = Repository.Instance.States.GetStates();

        _statesCollection.CollectionChanged += OnStatesCollectionChanged;

        _renderer = GetComponent<Renderer>();
        _originalRendererEnabled = _renderer.enabled;
        _originalMaterial = new Material(_renderer.material.shader);
        _originalMaterial.CopyPropertiesFromMaterial(_renderer.material);

        handle = Addressables.LoadAssetAsync<Material>(_xrayMatAddress);
        handle.Completed += Handle_Completed;

        // _xrayMaterial = (Material)Resources.Load("Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat", typeof(Material));
        // if (_xrayMaterial == null ) {
        //     Debug.LogError($"PROTEUS: Loaded Xray material is null");
        // }
        // Debug.Log($"Got xray material loaded resource: {_xrayMaterial}");
        DigiTwinController.Instance.LinkDigiTwinComponent(this);
    }

    // Instantiate the loaded prefab on complete
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


    // Update is called once per frame
    virtual protected void Update()
    {
    }

    virtual protected void OnDestroy()
    {
        if (_statesCollection != null)
        {
            _statesCollection.CollectionChanged -= OnStatesCollectionChanged;
        }
        DigiTwinController.Instance.UnlinkDigiTwinComponent(this);
    }

    public bool HasLinkedNodeInSelection()
    {
        bool isInSelection = LinkedNodes.Intersect(Repository.Instance.Proteus.GetNodeSelectionDisplayNames()).Count() > 0;
        return isInSelection;
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectNodeByName(MainDiagramName);
    }

    public void UpdateXrayView()
    {
        bool isInSelection = HasLinkedNodeInSelection();

        // If no active selection disable xray
        if (!_globalsData.XrayViewEnabled || isInSelection || _globalsData.SelectedNodes.Count == 0)
        {
            _renderer.enabled = _originalRendererEnabled;
            _renderer.material = _originalMaterial;
            return;
        }

        // Make transparent if not linked to a node in the selection
        if (isInSelection)
        {
            _renderer.enabled = _originalRendererEnabled;
            _renderer.material = _originalMaterial;
        }
        else if (_globalsData.XrayViewEnabled && ReactsToXray)
        {
            _renderer.material = _xrayMaterial;
            _renderer.material.color = new Color(1f, 1f, 1f, XrayOpacityFactor);

            if (XrayOpacityFactor == 0)
            {
                _renderer.enabled = false;
            }
        }
    }

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

    virtual protected void OnStateDataChanged(PTState obj ,PropertyChangedEventArgs e)
    {

    }

}
