using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Tooltip("Does this enemy deal damage to the player when it is touched?")] [SerializeField]
    private bool dealsDamageOnTouch;

    [Tooltip("How often can this enemy deal damage on touch?")] [SerializeField]
    private int delayBetweenTouchDamage;

    [Tooltip("How much damage does this enemy deal when touched?")] [SerializeField]
    private int damageOnTouch;

    [Tooltip("What type of damage is dealt on touch?")] [SerializeField]
    private DamageData.DamageType damageTypeOnTouch;
    
    [Tooltip("What status effects does this enemy deal when touched?")] [SerializeField]
    private List<StatusEffect> statusEffectsOnTouch;
    
    // whether this enemy's cooldown for damage on touch is available
    private bool canDealDamageOnTouch = true;

    /// <summary>
    /// Applies on touch damage to the collided player
    /// </summary>
    /// <param name="other"> The other collider </param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        var coroutine = AttemptOnTouchDamage(collision);
        StartCoroutine(coroutine);
    }

    IEnumerator AttemptOnTouchDamage(Collision2D collision)
    {
        if (!canDealDamageOnTouch || !dealsDamageOnTouch || !collision.gameObject.CompareTag("Player"))
        {
            yield break;
        }

        canDealDamageOnTouch = false;
        DamageData attackData = new DamageData(damageOnTouch, damageTypeOnTouch, statusEffectsOnTouch, this);
        Health hitHealth = collision.gameObject.GetComponent<Health>();
        if (hitHealth != null)
        {
            hitHealth.ReceiveAttack(attackData, transform.right);
            yield return new WaitForSeconds(delayBetweenTouchDamage);
        }

        canDealDamageOnTouch = true;
    }
}
