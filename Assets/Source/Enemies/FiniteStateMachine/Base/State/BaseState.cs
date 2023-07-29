using System.Collections;
using System.Collections.Generic;
using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a State type in a Finite State Machine
    /// </summary>
    public abstract class BaseState : ScriptableObject
    {
        /// <summary>
        /// Get the state the BaseState represents
        /// </summary>
        /// <returns> The state the BaseState represents </returns>
        public abstract State GetState();
    }
}