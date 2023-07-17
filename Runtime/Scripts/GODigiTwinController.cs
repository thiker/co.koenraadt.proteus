using System.Collections;
using System.Collections.Generic;
using co.koenraadt.proteus.Runtime.Controllers;
using UnityEngine;

/// <summary>
/// GameObject for the digital twin controller which ensures the digital twin controller's update function is called on the same loop as Unity's update function.
/// Furthermore, on destroy it will also destroy the digital twin controller.
/// </summary>
public class GODigiTwinController : MonoBehaviour
{
    DigiTwinController _controller;
    // Start is called before the first frame update
    void Start()
    {
        _controller = DigiTwinController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        _controller?.Update();
    }
}
