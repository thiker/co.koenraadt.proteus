using Packages.co.koenraadt.proteus.Runtime.Repository;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GODebugger : MonoBehaviour
{
    bool debugMode = true;
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
                Debug.Log($"Hit: {hit.colliderInstanceID} of Type: {hit.collider.GetType()}");
            }
        }

        // Test Command
        if (Input.GetKeyDown(KeyCode.S)) {
            Repository.Instance.AddNode(new PTNode("debug-node", "DebugNode"));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Repository.Instance.DeleteNodeById("debug-node");
        }
    }
}
