using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    [Tooltip("The number of quarter hearts to heal")] [Min(1)]
    public int healAmount = 1;

    /// <summary>
    /// Pickup health.
    /// </summary>
    /// <param name="collision"> The thing to test if it is the player. </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Health>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
