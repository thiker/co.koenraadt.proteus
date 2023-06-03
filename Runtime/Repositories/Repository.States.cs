using Packages.co.koenraadt.proteus.Runtime.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Packages.co.koenraadt.proteus.Runtime.Other;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Packages.co.koenraadt.proteus.Runtime.Repositories
{
    public class StatesRepository
    {
        private static StatesRepository _instance = null;

        private ObservableCollection<PTState> _ptStates;

        public static StatesRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatesRepository();
                    _instance.Init();
                }
                return _instance;
            }
        }

        private void Init()
        {
            // Initialise collections
            _ptStates = new();

            // TODO: Initialize the repo and load everything
        }

        /// <summary>
        /// Adds a PTState to the StatesRepository or updates it.
        /// </summary>
        /// <param name="newState">The PTState to add.</param>
        public void UpdateState(PTState newState)
        {
            PTState oldState = GetStateById(newState.Id);

            // If not already existing add the node
            if (oldState == null)
            {
                _ptStates.Add(newState);
            }
            else
            {
                Helpers.CombineValues(oldState, newState);
            }
        }

        public void UpdateStateValue(string id, string key, object value)
        {
            PTState ptState = GetStateById(id);

            // If state does not yet exist create it
            if (ptState == null)
            {
                ptState = new PTState() {  Id = id, Values=new() };
                UpdateState(ptState);
            }

            if (ptState.Values == null) { ptState.Values = new(); }

            // Clone so property changed event is fired
            Dictionary<string,object> newDict = new(ptState.Values);
            newDict[key] = value;
            ptState.Values = newDict; 
        }

        /// <summary>
        /// Get the collection of states.
        /// </summary>
        /// <returns>Collection of PTStates</returns>
        public ObservableCollection<PTState> GetStates()
        {
            return _ptStates;
        }

        /// <summary>
        /// Get a PTState by its Id.
        /// </summary>
        /// <param name="id">the state's identifier.</param>
        /// <returns>The PTState with its respective Id</returns>
        public PTState GetStateById(string id)
        {
            if (string.IsNullOrEmpty(id)) { return null; }
            PTState foundState = _ptStates.FirstOrDefault(x => x.Id == id);
            return foundState;
        }

        /// <summary>
        /// Removes a state by its id
        /// </summary>
        /// <param name="id">the state's identifier</param>
        public void DeleteStateById(string id)
        {
            PTState stateToDelete = GetStateById(id);
            if (stateToDelete != null)
            {
                int ix = _ptStates.IndexOf(stateToDelete);
                _ptStates.RemoveAt(ix);
            }
        }

    }
}