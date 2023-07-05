using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

public class GOViewerGizmoBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
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

    void Update()
    {
    }

    public void OnPointerDown(RaycastHit hit)
    {
        bool newVisible = !(bool)_linkedViewerData.GizmoVisible;
        Repository.Instance.Viewers.SetGizmoVisible(_linkedViewerData.Id, newVisible);
    }
}
