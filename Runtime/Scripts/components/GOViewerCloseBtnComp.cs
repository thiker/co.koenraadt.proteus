using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

public class GOViewerCloseBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    private string _attachedViewerId;
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
        
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Viewers.DeleteViewerById(_attachedViewerId);
    }
}