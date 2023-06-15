using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals damage when objects overlap triggers on this object.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageOnInteract : MonoBehaviour
{
    [Tooltip("The damage that will be dealt.")]
    [SerializeField] private DamageData damageData;

    [Tooltip("The frequency at which damage will be dealt.")]
    [SerializeField] private float damageInterval = 0.5f;

    [Tooltip("Whether or not this should wait for the damage interval to deal damage when first overlapping.")]
    [SerializeField] private bool immediatelyDamage = true;

    [Tooltip("If true, the attached trigger can not deal damage.")]
    [SerializeField] private bool noTriggerDamage;

    [Tooltip("If true, the attached collider can not deal damage.")]
    [SerializeField] private bool noCollisionDamage;

    // The collider used for overlap detection.
    private new Collider2D collider;

    /// <summary>
    /// Get references
    /// </summary>
    private void Awake()
    {
        Player.Get();
        collider = GetComponent<Collider2D>();
    }
    
    /// <summary>
    /// Start damaging.
    /// </summary>
    /// <param name="other"> The overlapped thing. </param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (noTriggerDamage || other.CompareTag(tag)) { return; }
        
        Health health = other.GetComponent<Health>();
        if (health == null) { return; }

        StartCoroutine(DealDamage(other, health));
    }
    
    /// <summary>
    /// Start damaging.
    /// </summary>
    /// <param name="other"> The collided thing. </param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (noCollisionDamage || other.collider.CompareTag(tag)) { return; }
        
        Health health = other.collider.GetComponentInParent<Health>();
        if (health == null) { return; }

        StartCoroutine(DealDamage(other.collider, health));
    }

    /// <summary>
    /// Deal damage on an interval.
    /// </summary>
    /// <param name="other"> The collider of the thing to damage. </param>
    /// <param name="health"> The health to damage. </param>
    /// <returns></returns>
    private IEnumerator DealDamage(Collider2D other, Health health)
    {
        if (immediatelyDamage)
        {
            health.ReceiveAttack(damageData);
        }

        yield return new WaitForSeconds(damageInterval);
        while (collider.IsTouching(other))
        {
            health.ReceiveAttack(damageData);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
