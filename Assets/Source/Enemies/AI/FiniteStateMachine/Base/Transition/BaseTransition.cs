using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the base class of a finite state machine transition
    /// </summary>
    public abstract class BaseTransition : ScriptableObject
    {
        /// <summary>
        /// Execute this transition
        /// </summary>
        /// <param name="machine"> The stateMachine to use </param>
        public abstract void Execute(BaseStateMachine machine);
    }
}