
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "FSM/State")]
    public sealed class State : BaseState
    {
        public List<FSMAction> actions = new List<FSMAction>();
        public List<FSMTransition> transitions = new List<FSMTransition>();

        public override void Execute(BaseStateMachine machine)
        {
            foreach (var action in actions)
            {
                action.Execute(machine);
            }

            foreach (var transition in transitions)
            {
                transition.Execute(machine);
            }
        }
    }