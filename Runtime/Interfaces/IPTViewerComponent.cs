using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.Interfaces
{
    /// <summary>
    /// Interface used for components of the Viewer.
    /// </summary>
    public interface IPTViewerComponent
    {

    /// <summary>
    /// Called on initialization by the viewer with a reference to the linked viewer's id.
    /// </summary>
    /// <param name="linkedViewerId">the id of the linked viewer</param>
    public void Init(string linkedViewerId)  {
    }
    }
}