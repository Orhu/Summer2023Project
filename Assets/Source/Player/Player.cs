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

        // Stores a ref to the player's health component.
        private static Health _health;
        public static Health health
        {
            get
            {
                if (_health != null) { return _health; }
                _health = Get().GetComponent<Health>();
                return _health;
            }
        }

        // Stores a ref to the player's feet collider.
        private static Collider2D _feet;
        public static Collider2D feet
        {
            get
            {
                if (_feet != null) { return _feet; }

                Collider2D[] playerColliders = Get().GetComponentsInChildren<Collider2D>();
                foreach (var playerCollider in playerColliders)
                {
                    if (!playerCollider.isTrigger)
                    {
                        _feet = playerCollider;
                        return _feet;
                    }
                }
                
                Debug.LogError("No feet collider found! Make sure you have a non-trigger collider attached to the enemy.");
                return null;
            }
        }

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

        /// <summary>
        /// Returns the feet collider's center, correctly offset from base transform
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetFeetPosition()
        {
            var offset = feet.offset;
            var position = feet.transform.position;
            return new Vector2(position.x + offset.x, position.y + offset.y);
        }
    }
}