using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// A component for rendering Player's heart count to screen
    /// </summary>
    public class PlayerHeartContainer : HeartContainer
    {

        /// <summary>
        /// Observer pattern, check local health with player's current health
        /// if there is a change, update health in UI
        /// </summary>
        private void Update()
        {
            SetHearts(Player.health.currentHealth);
        }
    }
}