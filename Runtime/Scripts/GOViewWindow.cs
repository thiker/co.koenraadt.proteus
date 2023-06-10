using System.Collections;
using System.Collections.Generic;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;

public class GOViewWindow : MonoBehaviour, IProteusInteraction
{
    private bool _isDragging;
    private string _attachedViewerId;
    private Vector3 _lastLocalHitPoint; // the point where the user started the pointer event
    private PTViewer _attachedViewerData;

    public void Init(string attachedViewerId) {
        _attachedViewerId = attachedViewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get viewer data of node
        _attachedViewerData = Repository.Instance.Viewers.GetViewerById(_attachedViewerId);
    }

    // Update is called once per frame
    void Update()
    {
        // Release on global mosue up aswell
        if (Input.GetMouseButtonUp(2) || (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl)) || (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.LeftControl)))
        {
            _isDragging = false;
        }
    }

    public void OnPointerDown(RaycastHit hit)
    {
        Repository.Instance.Proteus.SelectViewer(_attachedViewerId); // select the viewer
    }

    public void OnPointerCtrlClickDown(RaycastHit hit)
    {
        _isDragging = true;
        _lastLocalHitPoint = transform.InverseTransformPoint(hit.point);
    }

    public void OnPointerCtrlClickUp(RaycastHit hit)
    {
        _isDragging = false;
    }

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
