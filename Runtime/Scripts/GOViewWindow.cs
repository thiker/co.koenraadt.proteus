using System.Collections;
using System.Collections.Generic;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;

public class GOViewWindow : MonoBehaviour, IProteusInteraction
{
    private bool _isDragging;
    private string _linkedViewerId;
    private Vector3 _lastLocalHitPoint; // the point where the user started the pointer event

    public void Init(string linkedViewerId) {
        _linkedViewerId = linkedViewerId;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }

    public void OnPointerDown(RaycastHit hit)
    {
        _isDragging = true;
        _lastLocalHitPoint = transform.InverseTransformPoint(hit.point);
    }

    public void OnPointerUp(RaycastHit hit)
    {
        _isDragging = false;
    }

    public void OnPointerMove(RaycastHit hit)
    {

        if (_isDragging)
        {
            Vector3 localHitPosition = transform.InverseTransformPoint(hit.point);
            Vector3 offset = (localHitPosition - _lastLocalHitPoint); 
            offset.z = 0;

            Repository.Instance.Viewers.AddModelAnchorOffset(_linkedViewerId, offset);

            _lastLocalHitPoint = localHitPosition;
        }

    }
}
