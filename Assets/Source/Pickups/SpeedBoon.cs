using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the player's max speed when picked up.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Speed Boon")] 
    public class SpeedBoon : Boon
    {
        [Tooltip("The number of tiles/s to increase max speed by.")] [Min(0)]
        public float maxSpeedIncrease = 0.33333333333333333333f;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Player.Get().GetComponent<SimpleMovement>().maxSpeed += maxSpeedIncrease;
        }
    }
}