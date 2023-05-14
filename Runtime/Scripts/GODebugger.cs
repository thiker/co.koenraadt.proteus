using Packages.co.koenraadt.proteus.Runtime.Controllers;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
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
            if (Input.GetKeyDown(KeyCode.T))
            {
                Repository.Instance.Models.UpdateNode(new PTNode() { Id = "debug-node-test", Name = "DebugNodeTest" });
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Repository.Instance.Models.DeleteNodeById("debug-node-test");
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                CommsController.Instance.Init();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer", RootNodeId = "GUID 67a20f8a-a0ca-4c00-9a1a-18819f96d56e", Scale = new Vector3(1, 1, 1), Position = new Vector3(0, 2, 0), Rotation = new Quaternion(0, 0, 0, 0) });
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Repository.Instance.Viewers.ZoomViewer("test-viewer", 0.01f);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Repository.Instance.Viewers.ZoomViewer("test-viewer", -0.01f);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer", Position = new Vector3(0, 0, Random.Range(0, 10)), Rotation = new Quaternion(Random.Range(0, 2 * Mathf.PI), Random.Range(0, 2 * Mathf.PI), Random.Range(0, 2 * Mathf.PI), 0) });
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("selecting node");
                Repository.Instance.Proteus.SelectNode("GUID aed81bee-acb1-4133-80d2-ce7b9c699f98");
            }
        }
    }
}
