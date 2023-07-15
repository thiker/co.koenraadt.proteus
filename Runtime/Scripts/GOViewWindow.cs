using System.Collections;
using System.Collections.Generic;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;

/// <summary>
/// The viewer's view window that the nodes and edges in the visualization are constraint to.
/// </summary>
public class GOViewWindow : MonoBehaviour, IProteusInteraction
{
    private bool _isDragging;
    private string _attachedViewerId;
    private Vector3 _lastLocalHitPoint; // the point where the user started the pointer event
    private PTViewer _attachedViewerData;

    /// <summary>
    /// Initializes the view window and links the attached viewer.
    /// </summary>
    /// <param name="attachedViewerId"></param>
    public void Init(string attachedViewerId) {
        _attachedViewerId = attachedViewerId;
    }

    /// <summary>
    /// Starts and initializes the view window.
    /// </summary>
    void Start()
    {
        // Get viewer data of node
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    /// <summary>
    /// Stop dragging when the user release the mouse.
    /// </summary>
    void Update()
    {
        // Release on global mosue up aswell
        if (Input.GetMouseButtonUp(2) || (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl)) || (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.LeftControl)))
        {
            _isDragging = false;
        }
    }

    /// <summary>
    /// Select the viewer when the view window is clicked.
    /// </summary>
    /// <param name="hit">The raycast result from the interaction</param>
    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectViewer(_attachedViewerId); // select the viewer
    }

    /// <summary>
    /// When the user control clicks the viewwindow, the panning / dragging is enabled.
    /// </summary>
    /// <param name="hit">The raycast result from the interaction</param>
    public void OnPointerCtrlClickDown(RaycastHit hit)
    {
        _isDragging = true;
        _lastLocalHitPoint = transform.InverseTransformPoint(hit.point);
    }

    /// <summary>
    /// Stop dragging / panning when the user releases the control click.
    /// </summary>
    /// <param name="hit"></param>
    public void OnPointerCtrlClickUp(RaycastHit hit)
    {
        _isDragging = false;
    }

    /// <summary>
    /// When the user is dragging and moving the mouse, the offset position for the model anchor is updated so the viewer is panned.
    /// </summary>
    /// <param name="hit"></param>
    public void OnPointerMove(RaycastHit hit)
    {
        if (_isDragging && Repository.Instance.Proteus.IsViewerSelected(_attachedViewerData?.Id))
        {
            Vector3 localHitPosition = transform.InverseTransformPoint(hit.point);
            Vector3 offset = (localHitPosition - _lastLocalHitPoint); 
            offset.z = 0;

            Repository.Instance.Viewers.AddModelAnchorOffset(_attachedViewerId, offset);

            _lastLocalHitPoint = localHitPosition;
        }

    }
}
