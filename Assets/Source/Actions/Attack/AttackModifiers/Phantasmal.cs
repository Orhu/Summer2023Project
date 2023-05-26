using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An action modifier that changes the attack of an action modifier.
/// </summary>

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewPhantasmal", menuName = "Cards/AttackModifers/Phantasmal")]
    internal class Phantasmal : AttackModifier
    {
        // The projectile this modifies
        public override Projectile ModifiedProjectile
        {
            set
            {
                value.gameObject.layer = LayerMask.NameToLayer("PhantasmalProjectile");
            }
            protected get { return null; }
        }
    }
}
