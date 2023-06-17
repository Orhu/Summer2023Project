using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Static utility for getting a reference to the player game object
    /// </summary>
    public static class Player
    {
        // Stores a ref to the player.
        private static GameObject player;

        // Stores a ref to the player's feet collider.
        private static Collider2D playerFeet;

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <returns> The root game object of the player. </returns>
        public static GameObject Get()
        {
            if (player != null) { return player; }

            player = GameObject.FindGameObjectWithTag("Player");

            return player;
        }

        public static Collider2D GetFeet()
        {
            if (playerFeet != null)
            {
                return playerFeet;
            }

            var playerCollider = Get().GetComponentInChildren<Collider2D>();
            if (playerCollider != null)
            {
                playerFeet = playerCollider;
                return playerFeet;
            }
            else
            {
                Debug.LogError(
                    "No feet collider found! Make sure you have a non-trigger collider attached to the player.");
                return null;
            }
        }

    }
}