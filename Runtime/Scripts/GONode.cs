using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        _nodeData.PropertyChanged += (object obj, PropertyChangedEventArgs e) =>
        {
            Debug.Log($"Changed event {e.PropertyName}");
        };
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
    }
}
