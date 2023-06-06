using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFSMTransition : ScriptableObject
{
    public virtual void Execute(BaseStateMachine machine)
    {
        
    }
}
