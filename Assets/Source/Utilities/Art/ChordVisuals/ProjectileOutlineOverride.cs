using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Draws an outline around a projectile.
    /// </summary>
    public class ProjectileOutlineOverride : MonoBehaviour
    {
        [Tooltip("The scale applied to each outline")]
        [SerializeField] private Color newColor;


        /// <summary>
        /// Starts override all outlines
        /// </summary>
        public void OverrideOutlines()
        {
            foreach (ProjectileOutline outline in transform.parent.GetComponentsInChildren<ProjectileOutline>())
            {
                outline.GetComponent<SpriteRenderer>().color = newColor;
            }
        }
    }
}