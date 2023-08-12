using UnityEngine;

/// <summary>
/// ViewCube widget that helps a user to orient themselves in 3D space.
/// </summary>
public class GOViewCubeWidgetComp : MonoBehaviour
{
    /// <summary>
    /// Updates the GOViewCubeWidgets rotation.
    /// </summary>
    void Update()
    {
        transform.rotation = Quaternion.Inverse(Camera.main.transform.rotation);
    }
}
