using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using UnityEngine;

public class GOVRDemoSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer", RootNodeIds = new string[]{ "GUID 2c1bb3a0-688c-4ed3-a317-9ee1dc8e2879" }, Scale = new Vector3(3, 3, 3), Position = new Vector3(-4, 7, 3), Rotation = new Quaternion(0, 0, 0, 0) });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
