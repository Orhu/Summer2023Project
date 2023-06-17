using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Makes an attack's projectile bigger.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAddShapeSize", menuName = "Cards/AttackModifers/Add[Stat]/AddShapeSize [Duplicate Only]", order = 1)]
    public class AddShapeSize : AttackModifier
    {
        [Tooltip("The additional radius in tiles.")]
        [SerializeField] private Vector2 size;

        [Tooltip("The amount to scale up the visuals by.")]
        [SerializeField] private float visualsScale;

        // The projectile this modifies
        public override Projectile modifiedProjectile
        {
            set
            {
                if (value.shape is BoxProjectileShape box)
                {
                    box.size += size;
                    value.visualObject.transform.localScale = value.visualObject.transform.localScale + new Vector3(visualsScale, visualsScale, visualsScale);
                }
                else
                {
                    Debug.LogWarning("Tried to modify radius on " + value.name + ", which does not use a BoxProjectileShape");
                }
            }
        }
    }
}
