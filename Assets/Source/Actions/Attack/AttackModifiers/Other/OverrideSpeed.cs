using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Overrides projectile speed.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOverrideSpeed", menuName = "Cards/AttackModifers/OverrideSpeed")]
    public class OverrideSpeed : AttackModifier
    {
        [Tooltip("The new initial speed to add in tiles/s.")]
        [SerializeField] private float initialSpeed = -999f;

        [Tooltip("The new acceleration to add in tiles/s^2.")]
        [SerializeField] private float acceleration = -999f;

        [Tooltip("The new max speed to add in tiles/s.")]
        [SerializeField] private float maxSpeed = -999f;

        [Tooltip("The new min speed to add in tiles/s.")]
        [SerializeField] private float minSpeed = -999f;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                if (value.speed != -999f)
                {
                    value.speed = initialSpeed;
                }
                if (value.acceleration != -999f)
                {
                    value.acceleration = acceleration;
                }
                if (value.maxSpeed != -999f)
                {
                    value.maxSpeed = maxSpeed;
                }
                if (value.minSpeed != -999f)
                {
                    value.minSpeed = minSpeed;
                }
            }
        }
    }
}