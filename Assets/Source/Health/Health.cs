using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Add this component to make an object damageable and affectable by status effects.
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


    public UnityEvent<float> UpdateHealthBar, SetInitialHealthBarValue;

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
        SetInitialHealthBarValue?.Invoke(maxHealth);
        UpdateHealthBar?.Invoke(currentHealth);
    }

    /// <summary>
    /// Update invincibility timer.
    /// </summary>
    void Update()
    {
        InvincibilityTime -= Time.deltaTime;
    }

    /// <summary>
    /// Receive an attack, apply status effects and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received</param>
    public void ReceiveAttack(Attack attack)
    {
        if (InvincibilityTime == 0)
        {
            //TODO: Status effects
            currentHealth -= attack.damage;

            // take damage event is triggered
            UpdateHealthBar?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                onDeath?.Invoke();
            }

            if (onAttacked != null)
            {
                onAttacked(attack);
            }
        }
    }

    /// <summary>
    /// Increases the current health by the given amount, maxxed out at the max health
    /// </summary>
    /// <param name="healAmount"> The amount to heal by</param>
    public void Heal(int healAmount)
    {
        if (currentHealth + healAmount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healAmount;
        }

        UpdateHealthBar?.Invoke(currentHealth);
    }
}
