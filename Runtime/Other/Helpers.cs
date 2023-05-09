using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packages.co.koenraadt.proteus.Runtime.Interfaces;
using UnityEngine;
namespace Packages.co.koenraadt.proteus.Runtime.Other
{
    public class Helpers
    {
        /// <summary>
        /// Merges values of source into target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">object to merge into </param>
        /// <param name="source">object to take values from</param>
        public static void CombineValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);

                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        /// <summary>
        /// Raycasts for proteusviz layer
        /// </summary>
        /// <returns></returns>
        public static RaycastHit[] RayCastProteusViz()
        {
            LayerMask layerMask = LayerMask.GetMask("ProteusViz");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, layerMask);

            return hits;
        }

        /// <summary>
        /// Find a proteus interactable component in parents of the source game object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The first interactable component found</returns>
        public static IProteusInteraction FindInteractableComponentInParent(GameObject source)
        {
            if (source == null) {
                return null;
            }

            GameObject obj = source;

            do
            {
                Component[] results = obj.GetComponents<Component>();
                foreach (Component comp in results)
                {
                    if (comp is IProteusInteraction interactComp)
                    {
                        return interactComp;
                    }
                }
                obj = obj?.transform?.parent?.gameObject;
            }
            while (obj != null);

            return null;
        }
    }
}
