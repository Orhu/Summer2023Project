using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Decreases the players cooldowns in an exponential decay fashion.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Cooldown Boon")]
    public class CooldownBoon : Boon
    {
        [Tooltip("The amount to multiply the current cooldown reduction by.")] [Min(1)]
        [SerializeField] private float cooldownMultiplier = 1.1f;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Deck.playerDeck.cooldownReduction *= cooldownMultiplier;
        }
    }
}