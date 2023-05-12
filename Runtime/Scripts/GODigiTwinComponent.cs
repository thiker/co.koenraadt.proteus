using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEditor;
using UnityEngine;

public class GODigiTwinComponent : MonoBehaviour
{
    public List<string> LinkedNodes;
    public float ExplodeFactor = 1.5f;
    public bool DoXrayView = true;
    public bool DoExplodedView = true;
    private PTGlobals _globalsData;
    private Renderer _renderer;
    private Material _xrayMaterial;
    private Material _originalMaterial;
    private Vector3 _explodedViewOffset;

    void awake()
    {
        gameObject.layer = LayerMask.NameToLayer("ProteusViz");
        _explodedViewOffset = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        _globalsData = Repository.Instance.Proteus.GetGlobals();

        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
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
        bool isInSelection = LinkedNodes.Intersect(_globalsData.SelectedNodes).Count() > 0;
        return isInSelection;
    }

    public void UpdateXrayView()
    {
        bool isInSelection = HasLinkedNodeInSelection();

        // If no active selection disable xray
        if (!_globalsData.XrayViewEnabled || isInSelection || _globalsData.SelectedNodes.Length == 0)
        {
            _renderer.material = _originalMaterial;
            return;
        }

        // Save original material
        if (_renderer.material != _xrayMaterial)
        {
            _originalMaterial = _renderer.material;
        }

        // Make transparent if not linked to a node in the selection
        if (isInSelection)
        {
            _renderer.material = _originalMaterial;
        }
        else if (_globalsData.XrayViewEnabled)
        {
            _renderer.material = _xrayMaterial;
        }
    }

    public void UpdateExplodedView(Vector3 origin, bool isExploded)
    {
        if (DoExplodedView && isExploded && _globalsData.ExplodedViewEnabled)
        {
            Vector3 direction = (transform.position - origin).normalized;
            _explodedViewOffset += direction * ExplodeFactor;
            transform.position += _explodedViewOffset;
        } else {
            transform.position -= _explodedViewOffset;
            _explodedViewOffset = new Vector3(0,0,0);
        }
    }
}
