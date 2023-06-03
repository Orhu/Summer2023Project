using UnityEngine;

public class BaseState : ScriptableObject
{
    public virtual void OnStateUpdate(BaseStateMachine machine)
    {
    }

    public virtual void OnStateEnter(BaseStateMachine machine)
    {
    }

    public virtual void OnStateExit(BaseStateMachine machine)
    {
    }
}