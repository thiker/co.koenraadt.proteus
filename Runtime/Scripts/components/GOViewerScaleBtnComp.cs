using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;

/// <summary>
/// Button component for viewers that can control a viewer's scale.
/// </summary>
public class GOViewerScaleBtnComp : MonoBehaviour, IProteusInteraction, IPTViewerComponent
{
    /// <summary>
    /// The step that the viewer will scale when the button is clicked.
    /// </summary>
    public float ScaleStep = 0.005f;

    private string _attachedViewerId;
    private bool _isPressed = false;
    protected PTViewer _attachedViewerData;

    /// <summary>
    /// Initializes and starts the button component that scales the viewer.
    /// </summary>
    void Start()
    {
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Initializes the scale button component and links the viewer its related to.
    /// </summary>
    /// <param name="linkedViewerId">The id of the linked viewer.</param>
    public void Init(string linkedViewerId) {
        _attachedViewerId = linkedViewerId;
    }

    /// <summary>
    /// While the user holds down / presses the mouse the viewer will increment in scale with the scale step.
    /// </summary>
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
