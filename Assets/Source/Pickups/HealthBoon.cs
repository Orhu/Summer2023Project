using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the player's max health when picked up.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Health Boon")]
    public class HealthBoon : Boon
    {
        [Tooltip("The number of quarter hearts to increase max health by")] [Min(1)]
        public int increaseAmount = 4;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Player.health.maxHealth += increaseAmount;
            Player.health.Heal(increaseAmount);
        }
    }
}