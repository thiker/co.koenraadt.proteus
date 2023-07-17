using System.Collections.Generic;
using co.koenraadt.proteus.Runtime.Repositories;
using co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;
using System.ComponentModel;
using MQTTnet;

namespace co.koenraadt.proteus.Runtime.Controllers
{
    /// <summary>
    /// Controls the connection between Proteus and the Digital Twin components. Furthermore, it handles high-level behavior of the exploded and xray views.
    /// </summary>
    public class DigiTwinController
    {
        private static DigiTwinController _instance = null;
        private static PTGlobals _globalsData;
        private List<GODigiTwinComponent> _digiTwinComponents;

        /// <summary>
        /// The singleton instance of the Digital Twin controller
        /// </summary>
        public static DigiTwinController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                    _instance.Init();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes the Digital Twin controller.
        /// </summary>
        public void Init()
        {
            _instance._digiTwinComponents = new();

            _globalsData = Repository.Instance.Proteus.GetGlobals();
            _globalsData.PropertyChanged += OnGlobalsDataChanged;
        }

        /// <summary>
        /// Callback function for the globalsData object to receive updates whenever properties of the global data changes.
        /// </summary>
        /// <param name="obj">the globals data object</param>
        /// <param name="e">object containing the arguments of the PropertChanged event</param>
        private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                // When the nodes selection changes, update the Xray and Exploded views.
                case "SelectedNodes":
                    {
                        UpdateXrayView();
                        UpdateExplodedView();
                        break;
                    }
            }
        }

        /// <summary>
        /// Links a Digital Twin component to the DigiTwinController so it has a reference to it. These references are used to update the xray and exploded views.
        /// </summary>
        /// <param name="obj">The GODigiTwinComponent to link.</param>
        public void LinkDigiTwinComponent(GODigiTwinComponent obj)
        {
            _digiTwinComponents.Add(obj);
        }


        /// <summary>
        /// Unlinks a Digital Twin component from the DigiTwinController so it no longer has a reference to it.
        /// </summary>
        /// <param name="obj">The GODigiTwinComponent to unlink.</param>
        public void UnlinkDigiTwinComponent(GODigiTwinComponent obj)
        {
            _digiTwinComponents.Remove(obj);
        }

        /// <summary>
        /// Calculates and updates the Xray view of Proteus.
        /// </summary>
        public void UpdateXrayView()
        {
            _digiTwinComponents.ForEach((comp) =>
            {
                comp.UpdateXrayView();
            });
        }

        /// <summary>
        /// Calculates and updates the Exploded view of Proteus.
        /// </summary>
        public void UpdateExplodedView()
        {

            GODigiTwinComponent activeComp = null;

            // Get the active component that is currently selected and has exploded view enabled.
            foreach (GODigiTwinComponent comp in _digiTwinComponents)
            {
                if (comp.HasLinkedNodeInSelection() && comp.DoExplodedView)
                {
                    activeComp = comp;
                    break;
                }
            };

            // Go over all Digital Twin components and call the exploded view function with the calculated explode origin.
            foreach (GODigiTwinComponent comp in _digiTwinComponents)
            {
                bool isExploded = comp != activeComp;

                if (activeComp != null)
                {
                    Vector3 explodeOrigin = activeComp.gameObject.transform.position;
                    comp.UpdateExplodedView(explodeOrigin, isExploded);
                } else {
                    comp.UpdateExplodedView(new Vector3(0,0,0), false);
                }
            }

        }

        /// <summary>
        /// Update function which should be called by the digital twin controller's gameobject on Unity Update,to ensure the messages are processed in the main thread.
        /// The DigiTwinController does not inherit from monobehavior and therefore has no Update override itself.
        /// </summary>
        public void Update()
        {
        }
    }
}
