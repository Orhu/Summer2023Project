using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Add this component to make an object damageable.
/// </summary>
public class Health : MonoBehaviour
{
    [Tooltip("The Max health of this object")]
    public int maxHealth = 5;
    // The current health of this object
    public int currentHealth { get; private set; }
    // All status effects this is immune to.
    public List<StatusEffect> immuneStatusEffects = new List<StatusEffect>();

    // All status effects currently affecting this.
    private List<StatusEffect> statusEffects = new List<StatusEffect>();

    // Called when health values are changed and passes the new health.
    public UnityEvent<float> onHealthChanged, onMaxHealthChanged;
    // Called when this is attacked and passes the attack.
    public UnityEvent<AttackData> onAttacked;
    // Called when this dies
    public UnityEvent onDeath;
    // Called before this processes an attack and passes the incoming attack so can be modified.   
    public RequestIncomingAttackModification onRequestIncomingAttackModification;
    public delegate void RequestIncomingAttackModification(ref AttackData attack);


    /// <summary>
    /// Initializes current health.
    /// </summary>
    void Start()
    {
        currentHealth = maxHealth;

        // set max health bar value, then update health bar to contain its current value
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Update status effects and prunes null values
    /// </summary>
    void Update()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            statusEffects[i]?.Update();
            if (statusEffects[i] == null)
            {
                statusEffects.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    /// <summary>
    /// Receive an attack  and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received. </param>
    public void ReceiveAttack(AttackData attack)
    {
        ReceiveAttack(attack, Vector2.zero);
    }
    public void ReceiveAttack(AttackData attack, Vector2 knockbackDirection)
    {
        // Damage
        onRequestIncomingAttackModification?.Invoke(ref attack);
        currentHealth -= attack.damage;

        onHealthChanged?.Invoke(currentHealth);
        onAttacked?.Invoke(attack);
        if (currentHealth <= 0)
        {
            onDeath?.Invoke();
            return;
        }
        
        // Status effects
        foreach (StatusEffect statusEffect in attack.statusEffects)
        {
            if (!immuneStatusEffects.Contains(statusEffect))
            {
                StatusEffect matchingEffect = statusEffects.Find(statusEffect.Stack);
                if (matchingEffect == null)
                {
                    statusEffects.Add(statusEffect.Instantiate(gameObject));
                }
            }
        }

        // Knockback
        if(GetComponent<Rigidbody2D>())
        {
            GetComponent<Rigidbody2D>().MovePosition(transform.position + (Vector3)knockbackDirection * attack.knockback);
        }
        else
        {
            transform.position += (Vector3)knockbackDirection * attack.knockback;
        }
    }

    /// <summary>
    /// Increases the current health by the given amount, maxed out at the max health
    /// </summary>
    /// <param name="healAmount"> The amount to heal by</param>
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(Math.Max(healAmount, 0) + currentHealth, maxHealth);

        onHealthChanged?.Invoke(currentHealth);
    }
}
