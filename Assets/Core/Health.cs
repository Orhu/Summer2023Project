using UnityEngine;


/// <summary>
/// Add this component to make an object damageable and affectable by status effects.
/// </summary>
public class Health : MonoBehaviour
{
    [Tooltip("The Max health of this object")]
    public int MaxHealth = 5;
    // The current health of this object
    public int currentHealth { get; private set; }

    delegate void AttackNotification(Attack attack);
    AttackNotification onAttacked;

    /// <summary>
    /// Initializes current health.
    /// </summary>
    void Start()
    {
        currentHealth = MaxHealth;
    }

    /// <summary>
    /// Receive an attack, apply status effects and kill the owner if out of health.
    /// </summary>
    /// <param name="attack"> The attack being received</param>
    public void ReceiveAttack(Attack attack)
    {
        //TODO: Status effects
        currentHealth -= attack.damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        onAttacked(attack);
    }
}
