using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Makes an attack orbit further.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddOrbitRadius", menuName = "Cards/AttackModifers/Add[Stat]/AddOrbitRadius [Duplicate Only]", order = 1)]
    public class AddOrbitRadius : DuplicateAttackSequence
    {
        [Tooltip("The additional radius")]
        [SerializeField] private float radius;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            if (value is OrbitProjectile orbitProjectile)
            {
                orbitProjectile.radius += radius;
            }
            else
            {
                Debug.LogWarning("Tried to modify radius on " + value.name + ", which does not use a CircleProjectileShape");
            }
        }
    }
}
