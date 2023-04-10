using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packages.co.koenraadt.proteus.Runtime.Repositories;
using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Unity.Plastic.Newtonsoft.Json;
using System.Linq;
using System.ComponentModel;

namespace Packages.co.koenraadt.proteus.Runtime.Controllers
{
    public class DigiTwinController
    {
        private static DigiTwinController _instance = null;
        private static PTGlobals _globalsData;
        private List<GODigiTwinComponent> _digiTwinComponents;
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

        public void Init()
        {
            _instance._digiTwinComponents = new();

            _globalsData = Repository.Instance.Proteus.GetGlobals();
            _globalsData.PropertyChanged += OnGlobalsDataChanged;
        }

        public void Update()
        {

        }

        private void OnGlobalsDataChanged(object obj, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedNodes":
                    {
                        UpdateXrayView();
                        UpdateExplodedView();
                        break;
                    }
            }
        }


        public void LinkDigiTwinComponent(GODigiTwinComponent obj)
        {
            _digiTwinComponents.Add(obj);
        }

        public void UnlinkDigiTwinComponent(GODigiTwinComponent obj)
        {
            _digiTwinComponents.Remove(obj);
        }

        public void UpdateXrayView()
        {
            _digiTwinComponents.ForEach((comp) =>
            {
                comp.UpdateXrayView();
            });
        }

        public void UpdateExplodedView()
        {

            GODigiTwinComponent activeComp = null;

            // Get active comp
            foreach (GODigiTwinComponent comp in _digiTwinComponents)
            {
                if (comp.hasLinkedNodeInSelection())
                {
                    activeComp = comp;
                    break;
                }
            };


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
    }
}
