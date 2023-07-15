using UnityEngine;

public class GOViewCubeWidget : MonoBehaviour
{
    /// <summary>
    /// Updates the GOViewCubeWidgets rotation.
    /// </summary>
    void Update()
    {
        transform.rotation = Quaternion.Inverse(Camera.main.transform.rotation);
    }
}
