using UnityEngine;

/// <summary>
/// The base for any status effect. Has a duration and VFX and can stack with other status effects.
/// </summary>
public abstract class StatusEffect : ScriptableObject
{
    [Tooltip("The Duration this status effect will be applied for")] [Min(0.0166666667f)]
    [SerializeField]  private float _duration = 2f;
    public float Duration
    {
        get { return _duration; }
        set
        {
            _duration = Mathf.Max(value, 0);
            if (_duration == 0)
            {
                Destroy(this);
            }
        }
    }

    [SerializeField]
    [Tooltip("The game object spawned on the effected game object as a visual indicator")]
    protected GameObject visualEffect;


    // The number of times this status effect has been applied.
    public virtual int Stacks { get; protected set; } = 1;
    // The game object this is applied to.
    protected GameObject gameObject;

    /// <summary>
    /// Creates a new status effect that is a copy of the caller.
    /// </summary>
    /// <param name="gameObject"> The object to apply the status effect.</param>
    /// <returns> The status effect that was created. </returns>
    internal virtual StatusEffect Instantiate(GameObject gameObject)
    {
        StatusEffect instance = (StatusEffect)CreateInstance(GetType());

        instance.Duration = Duration;
        instance.gameObject = gameObject;

        if (visualEffect != null)
        {
            instance.visualEffect = Instantiate<GameObject>(visualEffect);
            instance.visualEffect.transform.parent = gameObject.transform;
            instance.visualEffect.transform.localPosition = Vector3.zero;
        }

        return instance;
    }

    /// <summary>
    /// Stacks this effect onto another status effect.
    /// </summary>
    /// <param name="other"> The other particle effect to stack this onto. </param>
    /// <returns> Whether or not this status effect was consumed by the stacking. </returns>
    internal virtual bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.Stacks += Stacks;
        return true;
    }

    /// <summary>
    /// Called every tick and updates the duration.
    /// </summary>
    internal virtual void Update() 
    {
        Duration -= Time.deltaTime;
    }

    /// <summary>
    /// Cleans up the VFX.
    /// </summary>
    protected void OnDestroy()
    {
        Destroy(visualEffect);
    }
}
