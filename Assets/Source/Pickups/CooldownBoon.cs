using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Decreases the players cooldowns in an exponential decay fashion.
    /// </summary>
    [CreateAssetMenu(menuName = "Loot/Cooldown Boon")]
    public class CooldownBoon : Boon
    {
        [Tooltip("The amount to add to the player's cooldown reduction multiplier. (0.1 adds a flat 10% cooldown reduction)")] [Min(0)]
        [SerializeField] private float cooldownMultiplier = 0.1f;

        /// <summary>
        /// Applied the effects of this boon to the player.
        /// </summary>
        public override void Apply()
        {
            pickCount++;
            Deck.playerDeck.cooldownReduction += cooldownMultiplier;
        }
    }
}