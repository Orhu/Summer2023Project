using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the players damage linearly.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Damage Boon")]
    public class DamageBoon : Boon
    {
        [Tooltip("The amount to multiply the player's current damage multiplier by.")] [Min(1)]
        [SerializeField] private float damageIncrease = 1.1f;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Player.Get().GetComponent<PlayerController>().damageMultiplier *= damageIncrease;
        }
    }
}