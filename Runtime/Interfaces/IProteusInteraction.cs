using UnityEngine;

namespace Packages.co.koenraadt.proteus.Runtime.Interfaces
{
    public interface IProteusInteraction
    {
        public void OnTriggerDown(RaycastHit hit) { }


        public void OnTriggerUp(RaycastHit hit) { }

        public void OnTriggerMove(RaycastHit hit) { }

    }
}