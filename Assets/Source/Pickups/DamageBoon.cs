using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the players damage linearly.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Damage Boon")]
    public class DamageBoon : Boon
    {
        [Tooltip("The amount to add to the player's damage multiplier. (0.1 adds a flat 10% damage)")] [Min(0)]
        [SerializeField] private float damageIncrease = 0.1f;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Player.Get().GetComponent<PlayerController>().damageMultiplier += damageIncrease;
        }
    }
}