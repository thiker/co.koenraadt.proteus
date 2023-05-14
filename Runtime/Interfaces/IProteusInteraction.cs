using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.Interfaces
{
    public interface IProteusInteraction
    {
        public void OnPointerDown(RaycastHit hit) { }
        public void OnPointerAltDown(RaycastHit hit) { }
        public void OnPointerTertiaryDown(RaycastHit hit) { }
        public void OnPointerUp(RaycastHit hit) { }
        public void OnPointerAltUp(RaycastHit hit) { }
        public void OnPointerTertiaryUp(RaycastHit hit) { }
        public void OnPointerMove(RaycastHit hit) { }

    }
}