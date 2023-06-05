using System;
using System.Collections;
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
    
    [Tooltip("How long of a duration does this unit get invincibility when hit?")]
    public float invincibilityDuration = 0.25f;
    
    [Tooltip("All status effects this is immune to.")]
    public List<StatusEffect> immuneStatusEffects = new List<StatusEffect>();

    [Tooltip("All status effects currently affecting this.")]
    private List<StatusEffect> statusEffects = new List<StatusEffect>();

    [Tooltip("Called when health values are changed and passes the new health.")]
    public UnityEvent<float> onHealthChanged, onMaxHealthChanged;
    
    [Tooltip("Called when this is attacked and passes the attack.")]
    public UnityEvent<DamageData> onAttacked;
    
    [Tooltip("Called when this dies")]
    public UnityEvent onDeath;

    // Called when invincibility changes and passes the new invincibility
    public Action<bool> onInvincibilityChanged;

    // Called before this processes an attack and passes the incoming attack so can be modified.   
    public RequestIncomingAttackModification onRequestIncomingAttackModification;
    public delegate void RequestIncomingAttackModification(ref DamageData attack);



    // is this unit currently invincible?
    private bool _invincible = false;
    private bool invincible
    {
        set
        {
            CancelInvoke(nameof(TurnOffInvincibility));
            if (value)
            {
                Invoke(nameof(TurnOffInvincibility), invincibilityDuration);
            }

            _invincible = value;
            onInvincibilityChanged?.Invoke(value);
        }
        get { return _invincible;  }
    }

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
    /// Receive an attack and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received. </param>
    public void ReceiveAttack(DamageData attack)
    {
        ReceiveAttack(attack, Vector2.zero);
    }
    public void ReceiveAttack(DamageData attack, Vector2 knockbackDirection)
    {
        if (invincible) return;
        
        // Damage
        onRequestIncomingAttackModification?.Invoke(ref attack);
        var prevHealth = currentHealth;
        currentHealth -= attack.damage;

        onHealthChanged?.Invoke(currentHealth);
        onAttacked?.Invoke(attack);
        if (currentHealth <= 0 && prevHealth > 0)
        {
            onDeath?.Invoke();
            return;
        }
        else if (invincibilityDuration != 0 && (attack.damage > 0 != attack.invertInvincibility))
        {
            invincible = true;
        }
        
        // Status effects
        foreach (StatusEffect statusEffect in attack.statusEffects)
        {
            if (!immuneStatusEffects.Contains(statusEffect))
            {
                StatusEffect matchingEffect = statusEffects.Find(statusEffect.Stack);
                if (matchingEffect == null)
                {
                    statusEffects.Add(statusEffect.CreateCopy(gameObject));
                }
            }
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

    /// <summary>
    /// Disable invincibility
    /// </summary>
    private void TurnOffInvincibility()
    {
        invincible = false;
    }
}
