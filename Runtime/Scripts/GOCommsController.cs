using Packages.co.koenraadt.proteus.Runtime.Controllers;
using UnityEngine;

public class GOCommsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CommsController.Instance.Update();
    }

    private void OnDestroy()
    {
        CommsController.Instance.Destroy();
    }
}
