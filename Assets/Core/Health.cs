using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Add this component to make an object damageable and affectable by status effects.
/// </summary>
public class Health : MonoBehaviour
{
    [Tooltip("The Max health of this object")]
    public int MaxHealth = 5;
    // The current health of this object
    public int currentHealth { get; private set; }

    // The amount of time this health will be invincible for
    private float invincibilityTime = 0f;
    public float InvincibilityTime
    { set { invincibilityTime = Mathf.Max(value, 0); } get { return invincibilityTime; } }


    public UnityEvent<float> UpdateHealthBar, SetInitialHealthBarValue;

    delegate void AttackNotification(Attack attack);
    AttackNotification onAttacked;

    /// <summary>
    /// Initializes current health.
    /// </summary>
    void Start()
    {
        currentHealth = MaxHealth;
        
        // set max health bar value, then update health bar to contain its current value
        SetInitialHealthBarValue?.Invoke(MaxHealth);
        UpdateHealthBar?.Invoke(currentHealth);
    }

    /// <summary>
    /// Update invincibility timer.
    /// </summary>
    void Update()
    {
        invincibilityTime -= Time.deltaTime;
    }

    /// <summary>
    /// Receive an attack, apply status effects and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received</param>
    public void ReceiveAttack(Attack attack)
    {
        if (invincibilityTime == 0)
        {
            //TODO: Status effects
            currentHealth -= attack.damage;

            // take damage event is triggered
            UpdateHealthBar?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }

            if (onAttacked != null)
            {
                onAttacked(attack);
            }
        }
    }
}
