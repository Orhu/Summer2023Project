using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    public abstract class BaseAction : ScriptableObject
    {
        public abstract void Execute(BaseStateMachine stateMachine);
    }
}
