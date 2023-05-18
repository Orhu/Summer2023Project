using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [SerializeField]
    [Tooltip("The Duration this status effect will be applied for")]
    [Min(0.0166666667f)]
    private float duration;

    [SerializeField]
    protected GameObject particleEffect;


    public float Duration 
    { 
        get { return duration; } 
        set {
            duration = Mathf.Max(value, 0);
            if (duration == 0)
            {
                Destroy(this);
            }
        }
    }

    public virtual int Stacks { get; protected set; } = 1;

    protected GameObject gameObject;

    internal virtual StatusEffect Instantiate(GameObject gameObject)
    {
        StatusEffect instance = (StatusEffect)CreateInstance(GetType());

        instance.Duration = Duration;
        instance.gameObject = gameObject;

        if (particleEffect != null)
        {
            instance.particleEffect = Instantiate<GameObject>(particleEffect);
            instance.particleEffect.transform.parent = gameObject.transform;
            instance.particleEffect.transform.localPosition = Vector3.zero;
        }

        return instance;
    }

    internal virtual bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.Stacks += Stacks;
        return true;
    }

    internal virtual void Update() 
    {
        Duration -= Time.deltaTime;
    }

    protected void OnDestroy()
    {
        Destroy(particleEffect);
    }
}
