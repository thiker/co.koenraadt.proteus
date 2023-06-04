using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

public class GOViewerScaleBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    public float ScaleStep = 0.005f;
    private string _attachedViewerId;
    private bool _isPressed = false;
    protected PTViewer _attachedViewerData;

    void Start()
    {
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }


    public void Init(string attachedViewerId) {
        _attachedViewerId = attachedViewerId;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0)) {
            _isPressed = false;
        } 
        if (_isPressed) {
            Repository.Instance.Viewers.ScaleViewer(_attachedViewerData.Id, new Vector3(ScaleStep, ScaleStep, ScaleStep));
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
