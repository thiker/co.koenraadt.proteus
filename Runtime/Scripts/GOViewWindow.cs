using System.Collections;
using System.Collections.Generic;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using UnityEngine;

public class GOViewWindow : MonoBehaviour, IProteusInteraction
{
    private bool _isDragging;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            _isDragging = false;
        }
    }

    public void OnPointerDown(RaycastHit hit)
    {  
        _isDragging = true;
    }

    public void OnPointerUp(RaycastHit hit) 
    {
        _isDragging = false;
    }

    public void OnPointerMove( RaycastHit hit) {
        if (_isDragging) {
            Debug.Log("dragging");
        }
        Debug.Log($"hovering ");
    }
}
