using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [SerializeField]
    [Tooltip("The Duration this status effect will be applied for")]
    [Min(0.0166666667f)]
    private float duration;
    public float Duration { get { return duration; } set { duration = Mathf.Max(value, 0); } }
    [field: SerializeField]
    public virtual int Stacks { get; private set; }

    protected GameObject gameObject;

    public abstract StatusEffect Instantiate(GameObject gameObject);

    public virtual bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.Stacks += Stacks;
        return true;
    }

    public virtual void Update() { }
}
