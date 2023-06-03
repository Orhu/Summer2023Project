using UnityEngine;


    public abstract class FSMAction : ScriptableObject
    {
        public abstract void OnStateUpdate(BaseStateMachine stateMachine);
        public abstract void OnStateEnter(BaseStateMachine stateMachine);
        public abstract void OnStateExit(BaseStateMachine stateMachine);
    }
