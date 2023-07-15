using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

/// <summary>
/// Button component that closes a viewer.
/// </summary>
public class GOViewerCloseBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    private string _attachedViewerId;
    protected PTViewer _linkedViewerData;

    /// <summary>
    /// Starts and initializes the close button component.
    /// </summary>
    void Start()
    {
        _linkedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Initializes the component and saves the reference to its related viewer.
    /// </summary>
    /// <param name="linkedViewerId">The id of the linked viewer.</param>
    public void Init(string linkedViewerId) {
        _attachedViewerId = linkedViewerId;
    }

    /// <summary>
    /// Action that closes the viewer that the component is linked to.
    /// </summary>
    public void CloseViewer() {
        Repository.Instance.Viewers.DeleteViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Whenever the user clicks the viewr that the component is linked to closes.
    /// </summary>
    /// <param name="hit"></param>
    public void OnPointerDown(RaycastHit hit)
    {
        CloseViewer();
    }
}