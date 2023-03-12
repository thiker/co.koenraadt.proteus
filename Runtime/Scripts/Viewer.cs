using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Viewer : MonoBehaviour
{
    public string Id { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalMatrix("_WorldToBox", transform.worldToLocalMatrix);
    }
}
