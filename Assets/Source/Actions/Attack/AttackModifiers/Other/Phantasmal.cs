using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPhantasmal", menuName = "Cards/AttackModifers/Phantasmal")]
    public class Phantasmal : AttackModifier
    {
        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            value.gameObject.layer = LayerMask.NameToLayer("PhantasmalProjectile");
        }
    }
}
