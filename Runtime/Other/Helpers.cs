using System;
using System.Linq;
using co.koenraadt.proteus.Runtime.Interfaces;
using UnityEngine;
namespace co.koenraadt.proteus.Runtime.Other
{
    /// <summary>
    /// Collection of Helper function used by Proteus.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Merges values of a source into a target object.
        /// </summary>
        /// <typeparam name="T">the type of the objects to merge.</typeparam>
        /// <param name="target">Object to merge into.</param>
        /// <param name="source">Object to take values from.</param>
        public static void CombineValues<T>(T target, T source)
        {
            if (source == null || target == null)
            {
                Debug.LogWarning("PROTEUS: Trying to combine values but source or target is null");
                return;
            }

            Type t = typeof(T);
            Debug.Log($"PROTEUS: Combining values for type {t.Name}");
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                var targetValue = prop.GetValue(target, null);

                if (value != null && !value.Equals(targetValue) && value != targetValue)
                {
                    Debug.Log($"Changing Property: {prop.Name} to value {value}");
                    prop.SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// Generates a new Guid.
        /// </summary>
        /// <returns>A string containing the generated Guid.</returns>
        public static string GenerateUniqueId() {
            return  $"pt-id-{Guid.NewGuid()}";
        }

        /// <summary>
        /// Checks for collections if they are empty.
        /// </summary>
        /// <typeparam name="T">the type of the object to check.</typeparam>
        /// <param name="value">the object to check</param>
        /// <returns>whether the object / collection is empty.</returns>
        [Obsolete("IsEmpty is slow and was only used during testing since CombineValues was always merging lists even if they had not changed, causing in unnecessary propertyChanged events.")]
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

        /// <summary>
        /// Checks whether the given class is a behavioral class.
        /// </summary>
        /// <param name="x">The class to check.</param>
        /// <returns>Whether the given class is marked as behavioral.</returns>
        public static bool IsBehavioralMetaClass(string x)
        {
            return x == "Statechart" || x == "ActivityDiagram" || x == "SequenceDiagram";
        }

        /// <summary>
        /// Performs a raycast on the proteusviz layer originating from the user's mouse position.
        /// </summary>
        /// <returns>The raycast hit result.</returns>
        public static RaycastHit[] RayCastProteusViz()
        {
            LayerMask layerMask = LayerMask.GetMask("ProteusViz");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, layerMask);

            return hits;
        }

        /// <summary>
        /// Find a proteus interactable component in parents of the given source game object.
        /// </summary>
        /// <param name="source">The source object to start the search from</param>
        /// <returns>The first interactable component found</returns>
        public static IProteusInteraction FindInteractableComponentInParent(GameObject source)
        {
            GameObject obj = source;

            if (obj == null)
            {
                return null;
            }

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

                if (obj != null)
                {
                    if (obj.transform != null) {
                        if (obj.transform.parent != null)
                        {
                            obj = obj.transform.parent.gameObject;
                        }
                    }
                }
            }
            while (obj != null);

            return null;
        }
    }
}
