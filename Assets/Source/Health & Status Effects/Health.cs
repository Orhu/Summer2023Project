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
    // The amount of time this health will be invincible for
    private float invincibilityTime = 0f;
    public float InvincibilityTime
    { set { invincibilityTime = Mathf.Max(value, 0); } get { return invincibilityTime; } }


    private List<StatusEffect> statusEffects = new List<StatusEffect>();


    public UnityEvent<float> onHealthChanged, onMaxHealthChanged;

    public UnityEvent onDeath;

    delegate void AttackNotification(Attack attack);
    AttackNotification onAttacked;

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
    /// Update invincibility timer.
    /// </summary>
    void Update()
    {
        InvincibilityTime -= Time.deltaTime;
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i] == null)
            {
                statusEffects.RemoveAt(i);
                i--;
                continue;
            }
            statusEffects[i].Update();
        }
    }

    /// <summary>
    /// Receive an attack  and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received. </param>
    public void ReceiveAttack(Attack attack)
    {
        if (InvincibilityTime == 0)
        {
            currentHealth -= attack.damage;

            // take damage event is triggered
            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                onDeath?.Invoke();
            }

            if (onAttacked != null)
            {
                onAttacked(attack);
            }
        }

        foreach (StatusEffect statusEffect in attack.statusEffects)
        {
            StatusEffect matchingEffect = statusEffects.Find(statusEffect.Stack);
            if (matchingEffect == null)
            {
                statusEffects.Add(statusEffect.Instantiate(gameObject));
            }
        }
    }

    /// <summary>
    /// Increases the current health by the given amount, maxed out at the max health
    /// </summary>
    /// <param name="healAmount"> The amount to heal by</param>
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(healAmount + currentHealth, maxHealth);

        onHealthChanged?.Invoke(currentHealth);
    }
}
