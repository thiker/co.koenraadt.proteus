using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

/// <summary>
/// Button component for the viewer that can enable and disable the gizmo of the viewer.
/// </summary>
public class GOViewerGizmoBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    private string _attachedViewerId;
    protected PTViewer _linkedViewerData;

    /// <summary>
    /// Starts and initializes the gizmo button component.
    /// </summary>
    void Start()
    {
        _linkedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Initializes the gizmo button component and links the viewer its related to.
    /// </summary>
    /// <param name="linkedViewerId">The id of the linked viewer.</param>
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
