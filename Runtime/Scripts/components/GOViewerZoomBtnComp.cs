using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

public class GOViewerZoomBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    public float ZoomScalar = 0.001f;
    private bool _isPressed;
    private string _attachedViewerId;
    protected PTViewer _linkedViewerData;

    void Start()
    {
        _linkedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }


    public void Init(string linkedViewerId) {
        _attachedViewerId = linkedViewerId;
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0)) {
            _isPressed = false;
        } 
        if (_isPressed) {
            Repository.Instance.Viewers.ZoomViewer(_linkedViewerData.Id, ZoomScalar);
        }
    }

    public void OnPointerDown(RaycastHit hit)
    {
        _isPressed = true;
    }

    public void OnPointerUp(RaycastHit hit) {
        _isPressed = false;
    }

}
