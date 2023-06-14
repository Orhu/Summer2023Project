using UnityEngine;

    public abstract class BaseAction : ScriptableObject
    {
        public abstract void Execute(BaseStateMachine stateMachine);
    }
