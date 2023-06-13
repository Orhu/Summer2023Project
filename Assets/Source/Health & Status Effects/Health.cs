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
    [SerializeField] private int _maxHealth = 5;
    public int maxHealth
    {
        get => _maxHealth;
        set
        {
            if(_maxHealth == value) { return; }

            _maxHealth = value;
            onMaxHealthChanged?.Invoke(value);
        }
    }


    // The current health of this object
    private int _currentHealth = 0;
    public int currentHealth 
    {
        get => _currentHealth;
        set
        {
            if (_maxHealth == value) { return; }

            _currentHealth = value;
            onHealthChanged?.Invoke(value);
        }
    }
    
    [Tooltip("How long of a duration does this unit get invincibility when hit?")]
    public float invincibilityDuration = 0.25f;
    
    [Tooltip("All status effects this is immune to.")]
    public List<StatusEffect> immuneStatusEffects = new List<StatusEffect>();

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
    
    // All status effects currently affecting this
    private List<StatusEffect> statusEffects = new List<StatusEffect>();

    /// <summary>
    /// Initializes current health.
    /// </summary>
    void Start()
    {
        onMaxHealthChanged?.Invoke(maxHealth);
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth != 0) { return; }
        currentHealth = maxHealth;
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
    /// <param name="ignoreInvincibility"> Whether or not invincibility frames should effect this attack. </param>
    public void ReceiveAttack(DamageData attack, bool ignoreInvincibility = false)
    {
        if (invincible && !ignoreInvincibility) return;
        
        // Damage
        onRequestIncomingAttackModification?.Invoke(ref attack);
        var prevHealth = currentHealth;
        currentHealth -= attack.damage;

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
    }

    /// <summary>
    /// Disable invincibility
    /// </summary>
    private void TurnOffInvincibility()
    {
        invincible = false;
    }
}
