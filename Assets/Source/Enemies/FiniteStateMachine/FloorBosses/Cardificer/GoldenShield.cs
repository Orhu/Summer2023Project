using System.Collections;
using System.Collections.Generic;
using Cardificer;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class GoldenShield : MonoBehaviour
{
    [Tooltip("Golden shield sprite")]
    [SerializeField] private Sprite goldenShieldSprite;

    [Tooltip("How much health can golden shield block?")]
    [SerializeField] private int goldenShieldMaxHealth;

    [Tooltip("How many seconds since the last damage instance until golden shield refreshes its health?")]
    [SerializeField] private float goldenShieldRefreshTime;
    
    [Tooltip("How many instances of damage will cause the golden shield to refresh its health?")]
    [SerializeField] private int goldenShieldRefreshAmount;

    [HideInInspector] public bool goldenShieldActive;

    private int damageInstanceCount;
    
    private float lastDamageTime;

    private float timeSinceLastDamage => Time.time - lastDamageTime;

    private GameObject goldenShieldGameObject;

    private int goldenShieldCurrentHealth;

    void Start()
    {
        lastDamageTime = Time.time;
        GetComponent<Health>().onRequestIncomingAttackModification += OnRequestIncomingAttackModification;
        goldenShieldCurrentHealth = goldenShieldMaxHealth;
        goldenShieldGameObject = new GameObject
        {
            name = "Golden Shield",
            transform =
            {
                parent = gameObject.transform
            }
        };
        goldenShieldGameObject.AddComponent<SpriteRenderer>().sprite = goldenShieldSprite;
        goldenShieldGameObject.SetActive(false);
    }

    /// <summary>
    /// Enables the golden shield if it is active and is not at 0 health, also handles refreshing the golden shield after some duration
    /// </summary>
    void Update()
    {
        if (!goldenShieldActive) return;
        
        goldenShieldGameObject.SetActive(goldenShieldCurrentHealth > 0);

        if (timeSinceLastDamage >= goldenShieldRefreshTime)
        {
            goldenShieldCurrentHealth = goldenShieldMaxHealth;
        }

        // TODO it may be a good idea to switch between different sprites to indicate how damaged the shield is, just do that with a simple if currentHealth < someThreshold
    }

    /// <summary>
    /// Enables golden shield
    /// </summary>
    public void ActivateGoldenShield()
    {
        goldenShieldActive = true;
        goldenShieldGameObject.SetActive(true);
        goldenShieldGameObject.transform.localPosition = Vector2.zero;
        // TODO here is where you'd want to set the animation trigger for golden shield, if there is one
    }

    public void OnRequestIncomingAttackModification(ref DamageData damage)
    {
        damageInstanceCount += 1;
        if (damageInstanceCount > goldenShieldRefreshAmount)
        {
            // if we have taken too many unique instances of damage, refresh golden shield hp
            goldenShieldCurrentHealth = goldenShieldMaxHealth;
            damageInstanceCount = 0;
        }
        
        lastDamageTime = Time.time;

        if (!goldenShieldActive || goldenShieldCurrentHealth <= 0) return;

        if (damage.damage > goldenShieldCurrentHealth)
        {
            // golden shield can absorb part of this attack
            damage.damage -= goldenShieldCurrentHealth;
            TakeDamage(goldenShieldCurrentHealth);
        }
        else
        {
            // golden shield can absorb this entire attack
            TakeDamage(damage.damage);
            damage.damage = 0;
        }
    }

    private void TakeDamage(int damageAmount)
    {
        goldenShieldCurrentHealth -= damageAmount;
        if (goldenShieldCurrentHealth < 0)
        {
            goldenShieldCurrentHealth = 0;
        }
    }
}