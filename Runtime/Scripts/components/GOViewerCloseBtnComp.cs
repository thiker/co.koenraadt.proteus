using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

public class GOViewerCloseBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    private string _attachedViewerId;
    protected PTViewer _linkedViewerData;

    void Start()
    {
        _linkedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }


    public void Init(string linkedViewerId) {
        _attachedViewerId = linkedViewerId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseViewer() {
        Repository.Instance.Viewers.DeleteViewerById(_attachedViewerId);
    }

    public void OnPointerDown(RaycastHit hit)
    {
        CloseViewer();
    }
}