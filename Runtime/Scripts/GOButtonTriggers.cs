using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using UnityEngine;

public class GOButtonTriggers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            SpawnViewer();
        }
    }

    public void SpawnViewer() {
        Debug.Log("PROTEUS: Spawning viewer after button trigger interaction.");
        Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer", RootNodeIds = new string[]{ "GUID 2c1bb3a0-688c-4ed3-a317-9ee1dc8e2879" }, Scale = new Vector3(1.2f, 1.2f, 1.2f), Position = new Vector3(-1.4f,1.7f, 0), Rotation = new Quaternion(0, 0, 0, 0) });
    }
}
