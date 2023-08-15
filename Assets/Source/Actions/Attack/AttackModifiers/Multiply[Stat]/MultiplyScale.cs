using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Multiplies the scale of a projectile.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMultiplyScale", menuName = "Cards/AttackModifers/Multiply[Stat]/MultiplyScale")]
    public class MultiplyScale : AttackModifier
    {
        [Tooltip("The amount scale will be multiplied by.")]
        [SerializeField] private float scaleFactor = 1f;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            float newScale = value.transform.localScale.x + scaleFactor - 1;
            value.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}