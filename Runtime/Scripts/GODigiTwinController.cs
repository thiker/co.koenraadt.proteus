using System.Collections;
using System.Collections.Generic;
using Packages.co.koenraadt.proteus.Runtime.Controllers;
using UnityEngine;

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
