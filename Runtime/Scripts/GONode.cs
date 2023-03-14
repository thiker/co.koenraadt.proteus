using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GONode : MonoBehaviour
{
    private PTNode _nodeData;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(PTNode nodeData)
    {
        _nodeData = nodeData;
        Debug.Log($"Node {_nodeData.Id} init itself.");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
    }
}
