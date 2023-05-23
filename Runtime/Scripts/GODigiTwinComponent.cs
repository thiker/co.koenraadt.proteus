using System.Collections;
using System.Collections.Generic;
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

public class GODigiTwinComponent : MonoBehaviour, IProteusInteraction
{
    public string MainDiagramName;
    public List<string> LinkedNodes;
    public float XrayOpacityFactor = .1f;
    public float ExplodeFactor = 1.5f;
    public bool DoXrayView = true;
    public bool DoExplodedView = true;
    public bool ReactsToXray = true;
    public bool ReactsToExplodedView = true;
    private PTGlobals _globalsData;
    private Renderer _renderer;
    private Material _xrayMaterial;
    private Material _originalMaterial;
    private Vector3 _explodedViewOffset;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("ProteusViz");
        _explodedViewOffset = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        _renderer = GetComponent<Renderer>();
        _originalMaterial = new Material(_renderer.material.shader);
        _originalMaterial.CopyPropertiesFromMaterial(_renderer.material);

        _xrayMaterial = (Material)AssetDatabase.LoadAssetAtPath("Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat", typeof(Material));

        DigiTwinController.Instance.LinkDigiTwinComponent(this);
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
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
            _renderer.material = _originalMaterial;
            return;
        }

        // Make transparent if not linked to a node in the selection
        if (isInSelection)
        {
            _renderer.material = _originalMaterial;
        }
        else if (_globalsData.XrayViewEnabled && ReactsToXray)
        {
            _renderer.material = _xrayMaterial;
            _renderer.material.color = new Color(1f, 1f, 1f, XrayOpacityFactor);
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
}
