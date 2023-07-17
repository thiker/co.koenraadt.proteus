using co.koenraadt.proteus.Runtime.Controllers;
using UnityEngine;

/// <summary>
/// GameObject for the communication controller which ensures the communication controller's update function is called on the same loop as Unity's update function.
/// Furthermore, on destroy it will also destroy the communication controller.
/// </summary>
public class GOCommsController : MonoBehaviour
{
    /// <summary>
    /// Updates the communication controller every unity update.
    /// </summary>
    void Update()
    {
        CommsController.Instance.Update();
    }

    /// <summary>
    /// When destroyed also destroy the communication controller.
    /// </summary>
    private void OnDestroy()
    {
        CommsController.Instance.Destroy();
    }
}
