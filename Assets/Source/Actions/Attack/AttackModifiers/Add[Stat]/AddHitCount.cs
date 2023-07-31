using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds to the maximum number of times an attack can hit enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddHitCount", menuName = "Cards/AttackModifers/Add[Stat]/AddHitCount")]
    public class AddHitCount : AttackModifier
    {
        [Tooltip("The number additional hits to add")]
        [SerializeField] private int hitCountToAdd;

        // The projectile this modifies
        public override void Initialize(Projectile value)
        {
            value.remainingHits += hitCountToAdd;
        }
    }
}

