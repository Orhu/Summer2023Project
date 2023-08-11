using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Draws an outline around a projectile.
    /// </summary>
    public class ProjectileOutline : MonoBehaviour
    {
        [Tooltip("The scale applied to each outline")]
        [SerializeField] private float scale = 0.1f;

        // The visual object this is outlining.
        private GameObject visualObject;

        // The sprite of visual object this is outlining.
        SpriteRenderer targetRenderer;

        // The outline sprite.
        SpriteRenderer sprite;

        /// <summary>
        /// Starts drawing the outline of this projectile. 
        /// </summary>
        public void DrawOutline()
        {
            visualObject = transform.parent.gameObject;

            int outlineIndex = transform.parent.GetComponentsInChildren<ProjectileOutline>().Length;
            scale *= outlineIndex;
            scale += 1;
            transform.localScale = new Vector3(scale, scale, scale);

            sprite = GetComponent<SpriteRenderer>();
            sprite.sortingOrder = -outlineIndex;
            targetRenderer = visualObject.GetComponent<SpriteRenderer>();
            if (targetRenderer == null)
            {
                targetRenderer = visualObject.GetComponentInChildren<SpriteRenderer>();
            }

            sprite.sprite = targetRenderer.sprite;
            sprite.enabled = true;

            transform.SetParent(targetRenderer.transform, false);
        }

        /// <summary>
        /// Keeps sprite up to date.
        /// </summary>
        private void FixedUpdate()
        {
            if (targetRenderer == null) { return; }
            sprite.sprite = targetRenderer.sprite;
        }
    }
}