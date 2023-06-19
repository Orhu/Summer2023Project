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

        public static Vector2 GetFeetPosition()
        {
            if (playerFeet != null)
            {
                var position = playerFeet.transform.position;
                var offset = playerFeet.offset;
                return new Vector2(position.x + offset.x,
                    position.y + offset.y);
            }

            Collider2D playerFeetCollider = null;
            var playerColliders = Get().GetComponentsInChildren<Collider2D>();
            foreach (var playerCollider in playerColliders)
            {
                if (!playerCollider.isTrigger)
                {
                    playerFeetCollider = playerCollider;
                    break;
                }
            }

            if (playerFeetCollider != null)
            {
                playerFeet = playerFeetCollider;
                var position = playerFeet.transform.position;
                var offset = playerFeet.offset;
                return new Vector2(position.x + offset.x,
                    position.y + offset.y);
            } else {
                Debug.LogError("No feet collider found! Make sure you have a non-trigger collider attached to the player.");
                return Vector2.zero;
            } 
        }

    }
}