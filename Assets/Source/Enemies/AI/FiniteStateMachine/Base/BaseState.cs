using UnityEngine;

public class BaseState : ScriptableObject
{
    public virtual void Execute(BaseStateMachine stateMachine)
    {
    }
}