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

namespace Packages.co.koenraadt.proteus.Runtime.Controllers
{
    public class DigiTwinController
    {
        private static DigiTwinController _instance = null;
    
        public static DigiTwinController Instance {
            get {
                if (_instance == null) {
                    _instance = new();
                }
                return _instance;
            }
        }
    }
}
