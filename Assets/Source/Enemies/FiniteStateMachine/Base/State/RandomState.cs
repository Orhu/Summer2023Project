using System.Collections;
using System.Collections.Generic;
using Cardificer;
using Cardificer.FiniteStateMachine;
using Skaillz.EditInline;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// A state that can be used to transition from one state into a randomly picked state from a weighted list of possible states
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Random State")]
    public class RandomState : BaseState
    {
        [Tooltip("Possible states to pick from")]
        [SerializeField] private GenericWeightedThings<BaseState> states;

        /// <summary>
        /// Grabs a random state and returns it
        /// </summary>
        /// <returns> A random state from the weighted list of states </returns>
        public override State GetState()
        {
            return states.GetRandomThing().GetState();
        }
    }
}