using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component supports applying simple on-touch damage when the attached collider is touched
/// </summary>
public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Does this enemy deal damage to the player when it is touched?")]
    [SerializeField] private bool canDealDamageOnTouch;

    [Tooltip("How often can this enemy deal damage on touch?")] 
    [SerializeField] private int delayBetweenTouchDamage;

    [Tooltip("How much damage does this enemy deal when touched?")] 
    [SerializeField] private int damageOnTouch;

    [Tooltip("What type of damage is dealt on touch?")] 
    [SerializeField] private DamageData.DamageType damageTypeOnTouch;

    [Tooltip("What status effects does this enemy deal when touched?")] 
    [SerializeField] private List<StatusEffect> statusEffectsOnTouch;

    /// <summary>
    /// Applies on touch damage to the collided player
    /// </summary>
    /// <param name="other"> The other collider </param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canDealDamageOnTouch)
        {
            var coroutine = AttemptOnTouchDamage(collision);
            StartCoroutine(coroutine);
        }
    }

    /// <summary>
    /// Attempts to deal on touch damage, if possible
    /// </summary>
    /// <param name="collision"> The other collider </param>
    /// <returns></returns>
    IEnumerator AttemptOnTouchDamage(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            yield break;
        }

        canDealDamageOnTouch = false;
        DamageData attackData = new DamageData(damageOnTouch, damageTypeOnTouch, statusEffectsOnTouch, this);
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.ReceiveAttack(attackData);
            yield return new WaitForSeconds(delayBetweenTouchDamage);
        }

        canDealDamageOnTouch = true;
    }
}