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
    public string[] LinkedNodes;
    private PTGlobals _globalsData;
    private Renderer _renderer;
    private Material _xrayMaterial;
    private Material _originalMaterial;

    void awake () {
        DigiTwinController.Instance.LinkDigiTwinComponent(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _globalsData = Repository.Instance.Proteus.GetGlobals();
        _globalsData.PropertyChanged += OnGlobalsDataChanged;

        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.material;
        _xrayMaterial = (Material)AssetDatabase.LoadAssetAtPath("Packages/co.koenraadt.proteus/Runtime/Materials/Mat_Xray.mat", typeof(Material));
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        _globalsData.PropertyChanged -= OnGlobalsDataChanged;
        DigiTwinController.Instance.UnlinkDigiTwinComponent(this);
    }

    private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "SelectedNodes":
                {
                    UpdateXrayView();
                    break;
                }
        }
    }

    private void UpdateXrayView()
    {
        // If no active selection disable xray
        if (_globalsData.SelectedNodes.Length == 0) {
            _renderer.material = _originalMaterial;
            return;
        }

        bool isInSelection = LinkedNodes.Intersect(_globalsData.SelectedNodes).Count() == 0;

        // Save original material
        if (_renderer.material != _xrayMaterial) {
            _originalMaterial = _renderer.material;
        }

        // Make transparent of not linked to a node in the selection
        if (isInSelection) {
            _renderer.material = _xrayMaterial;
        } else {
            _renderer.material = _originalMaterial;
        }
    }
}
