using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using co.koenraadt.proteus.Runtime.ViewModels;
using co.koenraadt.proteus.Runtime.Interfaces;
using co.koenraadt.proteus.Runtime.Repositories;

/// <summary>
/// Button component for viewer's that can control the level of zoom of a viewer.
/// </summary>
public class GOViewerZoomBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    /// <summary>
    /// The step that the viewer should be scaled every update that the user presses the zoom button.
    /// </summary>
    public float ZoomScalar = 0.001f;

    private bool _isPressed;
    private string _attachedViewerId;
    protected PTViewer _linkedViewerData;

    /// <summary>
    /// Starts and initializes the zoom button component.
    /// </summary>
    void Start()
    {
        _linkedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Initialies the zoom button component and stores a refernece to the viewer its linked to.
    /// </summary>
    /// <param name="linkedViewerId">The id of the viewer that the component is linked to.</param>
    public void Init(string linkedViewerId) {
        _attachedViewerId = linkedViewerId;
    }

    /// <summary>
    /// While the user presses down the zoom button component, increment or decrement the level of zoom with the zoom step.
    /// </summary>
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
