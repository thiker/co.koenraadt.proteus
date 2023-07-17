using co.koenraadt.proteus.Runtime.Controllers;
using co.koenraadt.proteus.Runtime.Repositories;
using co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions and helpers used during the development of Proteus for debugging.
/// </summary>
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
                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer", RootNodeIds = new string[]{ "GUID 2c1bb3a0-688c-4ed3-a317-9ee1dc8e2879" }, Scale = new Vector3(3, 3, 3), Position = new Vector3(-4, 7, 3), Rotation = new Quaternion(0, 0, 0, 0) });


                // Test generate behavorial diagram veiwer
                List<PTNode> behavioralNodes = Repository.Instance.Models.GetRelatedBehavioralNodesById("GUID 0e97bf50-b195-452e-aad4-7bf3e337f190");

                List<string> rootIds = new();
                foreach (PTNode node in behavioralNodes)
                {
                    rootIds.Add(node.Id);
                }

                Repository.Instance.Viewers.UpdateViewer(new PTViewer() { Id = "test-viewer-behavorial", RootNodeIds = rootIds.ToArray(), Scale = new Vector3(3, 3, 3), Position = new Vector3(-8, 7, 3), Rotation = new Quaternion(0, 0, 0, 0) });





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
