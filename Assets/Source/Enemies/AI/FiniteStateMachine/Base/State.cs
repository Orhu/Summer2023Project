
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "FSM/State")]
    public sealed class State : BaseState
    {
        public List<FSMAction> actions = new List<FSMAction>();
        public List<FSMTransition> transitions = new List<FSMTransition>();

        public override void OnStateUpdate(BaseStateMachine machine)
        {
            foreach (var action in actions)
            {
                action.OnStateUpdate(machine);
            }

            foreach (var transition in transitions)
            {
                transition.Execute(machine);
            }
        }
        
        public override void OnStateEnter(BaseStateMachine stateMachine)
        {
            foreach (var action in actions)
            {
                action.OnStateEnter(stateMachine);
            }
        }
        
        public override void OnStateExit(BaseStateMachine stateMachine)
        {
            foreach (var action in actions)
            {
                action.OnStateExit(stateMachine);
            }
        }
    }