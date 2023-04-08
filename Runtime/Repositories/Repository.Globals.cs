using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Other;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public class ProteusRepository
    {
        private static ProteusRepository _instance = null;
        private static PTGlobals _globalsInstance = null;

        public static ProteusRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProteusRepository();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Get the global state of proteus.
        /// </summary>
        public PTGlobals GetGlobals() {
            if (_globalsInstance == null) {
                _globalsInstance = new();
            }   
            return _globalsInstance;
        }

        public void UpdateGlobals(PTGlobals update) {
            Helpers.CombineValues(_globalsInstance, update);
        }

        public void SelectNode(string nodeId) 
        {
            if (nodeId == "") {
                GetGlobals().SelectedNodes = new string[]{};
                return;
            }
            GetGlobals().SelectedNodes = new[]{nodeId};
        }
    }
}