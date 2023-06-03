
    using UnityEngine;
    using UnityEngine.Serialization;

    [CreateAssetMenu(menuName = "FSM/Transition")]
    public class FSMTransition : ScriptableObject
    {
        // the condition to evaluate
        public FSMDecision decision;
        
        // what state to enter on true
        public BaseState trueState;
        
        // what state to enter on false
        public BaseState falseState;

        public void Execute(BaseStateMachine machine)
        {
            var result = decision.Decide(machine);
            
            if (result && trueState is not RemainInState)
            {
                machine.currentState = trueState;
            } else if (!result && falseState is not RemainInState)
            {
                machine.currentState = falseState;
            }
        }
    }
