using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Increases the players damage linearly.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Damage Boon")]
    public class DamageBoon : Boon
    {
        [Tooltip("The amount to add to the player's current damage multiplier.")] [Range(0, 1)]
        [SerializeField] private float damageIncrease = 0.9f;

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