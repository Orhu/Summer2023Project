using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile bigger.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddScale", menuName = "Cards/AttackModifers/Add[Stat]/AddScale", order = 1)]
    public class AddScale : AttackModifier
    {
        [Tooltip("The amount to add to the scale.")]
        [SerializeField] private float scale;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.transform.localScale = value.transform.localScale + new Vector3(scale, scale, scale);
        }
    }
}
