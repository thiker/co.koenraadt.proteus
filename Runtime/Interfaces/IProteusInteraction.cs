using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.Interfaces
{
    /// <summary>
    /// Interface for the interaction of Proteus.
    /// </summary>
    public interface IProteusInteraction
    {
        /// <summary>
        /// Called when the user presses the pointer button down.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerDown(RaycastHit hit) { }

        /// <summary>
        /// Called when the user presses the alternative pointer button down.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerAltDown(RaycastHit hit) { }

        /// <summary>
        /// Called when the user presses the tertiary pointer button down.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerTertiaryDown(RaycastHit hit) { }


        /// <summary>
        /// Called when the user clicked while holding ctrl.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerCtrlClickDown(RaycastHit hit) { }

        /// <summary>
        /// Called when the user presses the pointer down while holding alt.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerAltClickDown(RaycastHit hit) { }

        /// <summary>
        /// Called when the user releases the pointer.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerUp(RaycastHit hit) { }

        /// <summary>
        /// Called when the user release the alternative pointer button.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerAltUp(RaycastHit hit) { }

        /// <summary>
        /// Called when the user release the  tertiary pointer button.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerTertiaryUp(RaycastHit hit) { }

        /// <summary>
        /// Called when the user releases the button when the alt key was held.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerAltClickUp(RaycastHit hit) { }

        /// <summary>
        /// Called when the user releases the pointer and had ctrl clicked.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerCtrlClickUp(RaycastHit hit) { }

        /// <summary>
        /// Called when the user moves the pointer.
        /// </summary>
        /// <param name="hit">The raycasthit that triggered the event.</param>
        public void OnPointerMove(RaycastHit hit) { }
    }
}