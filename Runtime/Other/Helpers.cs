using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (source == null || target == null)
            {
                Debug.LogWarning("PROTEUS: Trying to combine values but source or target is null");
                return;
            }

            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);

                if (value != null && !value.Equals(targetValue) && value != targetValue)
                {
                    prop.SetValue(target, value, null);
                }
            }
        }


        //TODO: Refactor the combine values method and isempty since they can be slow!.
        public static bool IsEmpty<T>(T value)
        {
            Type[] interfaces = value?.GetType()?.GetInterfaces();

            if (interfaces?.Length > 0)
            {
                foreach (Type interfaceType in interfaces)
                {
                    var property = interfaceType.GetProperty("Count");

                    if (property != null) {
                        int count = (int)property.GetValue(value, null);

                        if (count <= 0)
                        {
                            return true;
                        }
                    }

                }
            }


            return false;
        }

        public static bool IsBehavioralMetaClass(string x)
        {
            return x == "Statechart" || x == "ActivityDiagram" || x == "SequenceDiagram";
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
            if (source == null)
            {
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
