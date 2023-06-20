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
                _feet = Get().GetComponentInChildren<Collider2D>();
                return _feet;
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
    }
}