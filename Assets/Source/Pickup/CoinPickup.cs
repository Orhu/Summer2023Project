using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Tooltip("The number of coins to give")] [Min(1)]
    public int coins = 1;

    /// <summary>
    /// Pickup coins.
    /// </summary>
    /// <param name="collision"> If player give coins. </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // TODO: Make work.
            Destroy(gameObject);
        }
    }
}
