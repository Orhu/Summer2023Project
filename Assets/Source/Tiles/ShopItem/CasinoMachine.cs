using System.Collections;
using Cardificer;
using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Handles logic related to a casino machine
    /// </summary>
    public class CasinoMachine : MonoBehaviour
    {
        [Header("Basic")]
        [Tooltip("How much does it cost to roll at this casino machine?")]
        [SerializeField] private int rollCost = 5;

        [Tooltip("Maximum uses of this machine (-1 is infinite)")] [Min(-1)]
        [SerializeField] private int machineUses = -1;

        [Header("Roll Stats")]
        [Tooltip("Chance to win a reward? Configure individual reward chances via the loot pool.")] [Range(0, 1)]
        [SerializeField] private float rewardChance = 0.05f;

        [Tooltip("Increase in chances per roll. Resets when a reward is won.")] [Range(0, 1)]
        [SerializeField] private float rewardIncrease = 0.05f;

        [Tooltip("Maximum achievable chance to win a reward (caps reward increase, set to 1 for no cap)")] [Range(0, 1)]
        [SerializeField] private float maxRewardChance = 0.75f;

        [Tooltip(
            "Maximum times rolled without reward (-1 never gives pity, pity always gives 100% win chance and overrides max reward chance)")]
        [Min(-1)]
        [SerializeField] private int numOfRollsForPity = -1;

        [Header("Animations and Cooldowns")]
        [Tooltip("Rolling duration in seconds")] [Min(0.1f)]
        [SerializeField] private float rollingDuration = 2f;

        [Tooltip("Cooldown after rolling in seconds")] [Min(0.1f)]
        [SerializeField] private float cooldownDuration = 1f;

        // TODO when animations are added proper we will use that instead of text
        [Tooltip("Reference to text field (can be removed once animations are added)")]
        [SerializeField] private TextMeshProUGUI text;

        [Header("Loot")]
        [Tooltip("Loot pool (any pickup can be placed in here)")]
        [SerializeField] private GameObjectLootTable lootPool;

        // Stores the Random that is seeded based on the world pos of the casino slot
        private System.Random random;

        // Tracks whether we can sell another card
        private bool canRollSlots => machineUses is -1 or > 0;

        // Tracks whether the machine is currently rolling and cannot be used
        private bool currentlyRolling = false;

        // Tracks the number of rolls since the last reward
        private int timesRolledSinceLastReward = 0;

        /// <summary>
        /// Seeds the random based on the world position of the casino slot
        /// </summary>
        void Start()
        {
            random = new System.Random(GenericWeightedThings<int>.PositionToSeed(transform.position));
            Player.SetMoney(150);
            text.text = "Ready (" + GetWinChance() * 100 + "% to win)";
        }

        /// <summary>
        /// Roll the slots, if the player has enough money
        /// </summary>
        /// <param name="other"> The collider of the thing that collided with the tile. </param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!currentlyRolling && canRollSlots && other.collider.CompareTag("Player") && Player.AddMoney(-rollCost))
            {
                StartCoroutine(RollSlots());
            }
        }

        /// <summary>
        /// Rolls the slots and gives a reward if the player wins
        /// </summary>
        /// <returns> Waits for animations to play </returns>
        private IEnumerator RollSlots()
        {
            if (machineUses != -1)
            {
                machineUses--;
            }

            currentlyRolling = true;

            text.text = "Rolling...";
            yield return new WaitForSeconds(rollingDuration); // rolling animation

            // roll the slots
            float currentRewardChance = GetWinChance();
            float rollResult = (float)random.NextDouble();

            bool playerWins = rollResult <= currentRewardChance;
            if (playerWins)
            {
                PlayerWins();
            }
            else
            {
                PlayerLoses();
            }

            yield return new WaitForSeconds(cooldownDuration);
            if (machineUses == -1 || canRollSlots)
            {
                text.text = "Ready (" + GetWinChance() * 100 + "% to win)";
            }
            else
            {
                text.text = "Closed (out of uses)";
            }

            currentlyRolling = false;
            yield break;
        }

        /// <summary>
        /// Handles player winning a roll logic
        /// </summary>
        private void PlayerWins()
        {
            text.text = "You win!";
            timesRolledSinceLastReward = 0;
            var reward = lootPool.weightedLoot.GetRandomThing(random);
            Instantiate(reward, (Vector2)transform.position + Vector2.down, Quaternion.identity);
        }

        /// <summary>
        /// Handles player losing a roll logic
        /// </summary>
        private void PlayerLoses()
        {
            text.text = "You lose!";
            timesRolledSinceLastReward++;
        }

        /// <summary>
        /// Returns the current chances of winning on this casino machine
        /// </summary>
        /// <returns> The current chances of winning on this casino machine. Always returns 1 when pity is hit. </returns>
        private float GetWinChance()
        {
            if (numOfRollsForPity != -1 && timesRolledSinceLastReward >= numOfRollsForPity)
            {
                // we hit pity, guaranteed reward
                return 1;
            }
            else
            {
                float chanceToWin = rewardChance + timesRolledSinceLastReward * rewardIncrease;
                return chanceToWin > maxRewardChance ? maxRewardChance : chanceToWin;
            }
        }
    }
}