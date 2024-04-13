using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Causes attacks to turn hit objects into some other game object.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPolymorph", menuName = "Cards/AttackModifers/Polymorph")]
    public class Polymorph : AttackModifier
    {
        [Tooltip("What hit objects will be morphed into")]
        [Min(0f)]
        [SerializeField] private GameObject morphInto;

        // The projectile doing the polymorphing.
        private Projectile polymorphProjectile;

        public override void Initialize(Projectile value)
        {
            value.onOverlap += MorphObject;
            polymorphProjectile = value;
        }

        /// <summary>
        /// Morphs a game object into a different game object.
        /// </summary>
        /// <param name="component"> A component on the game object to morph. </param>
        private void MorphObject(Component component)
        {
            if (component.CompareTag("Boss") || component.CompareTag("Inanimate")) { return; }
            if (component.GetComponent<Health>() == null) { return; }
            //if (component.gameObject.name.Split("(")[0] == morphInto.name) { return; } 

            GameObject createdObject = Instantiate(morphInto);
            createdObject.transform.position = component.transform.position;
            createdObject.transform.rotation = component.transform.rotation;

            polymorphProjectile.ignoredObjects.Add(createdObject);

            if (component.gameObject.name.Split("(")[0] != morphInto.name)
            {
                Destroy(component.gameObject);
            }
        }
    }
}
