using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GODebugger : MonoBehaviour
{
    bool debugMode = true;
    KeyCode debugKey = KeyCode.F3;
    bool isHoldingDebugKey = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!debugMode)
        {
            return;
        }

        // Object Clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"<color=lightblue>PROTEUS</color> Hit: {hit.colliderInstanceID} of Type: {hit.collider.GetType()}");
            }
        }

        if (Input.GetKeyDown(debugKey))
        {
            isHoldingDebugKey = true;
        }

        if (Input.GetKeyUp(debugKey))
        {
            isHoldingDebugKey = false;
        }

        if (isHoldingDebugKey)
        {
            // Test Command
            if (Input.GetKeyDown(KeyCode.S))
            {
                Repository.Instance.Models.UpdateNode(new PTNode() { Id = "debug-node-test", Name = "DebugNodeTest" });
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Repository.Instance.Models.DeleteNodeById("debug-node-test");
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                CommsController.Instance.Init();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer" });
                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer2", Position = new Vector3(20, 0, 0), Rotation = new Quaternion(0,0,0,0)});
            }
        }
    }
}
