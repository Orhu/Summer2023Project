using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Health))]
    public class DamageFlash : MonoBehaviour
    {
        [Tooltip("The color to make the sprite when damaged.")]
        [SerializeField] private Color invincibilityFlashColor = Color.red;

        [Tooltip("The time to make this a different color for.")]
        [SerializeField] private float flashDuration = 0.25f;

        private List<SavedSpriteRenderer> spriteRenderers = new List<SavedSpriteRenderer>();

        /// <summary>
        /// Initializes references
        /// </summary>
        private void Awake()
        {
            GetComponent<Health>().onDamageTaken += 
                () =>
                {
                    StartCoroutine(Flash());
                };

            GetAllSpriteRenderers();
        }

        /// <summary>
        /// saves all sprite renderers on the object and it's children, grandchildren, etc.
        /// </summary>
        private void GetAllSpriteRenderers()
        {
            //use to store ALL children & this object
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(transform);

            while (queue.Count > 0)
            {
                var newParent = queue.Dequeue();

                //save if it has a sprite renderer & it is not a shadow
                if (newParent.gameObject.tag != "Shadow" && newParent.gameObject.GetComponent<SpriteRenderer>())
                {
                    spriteRenderers.Add(new SavedSpriteRenderer(newParent.gameObject.GetComponent<SpriteRenderer>()));
                }
               
                foreach (Transform child in newParent) queue.Enqueue(child);
            }
        }

        /// <summary>
        /// Enables or disables tinting of the sprite.
        /// </summary>
        /// <returns> The time to stay tinted. </returns>
        private IEnumerator Flash()
        {
            foreach (var renderer in spriteRenderers) 
            {
                renderer.ShowColor(invincibilityFlashColor);
            }

            yield return new WaitForSeconds(flashDuration);

       
            foreach (var renderer in spriteRenderers)
            {
                renderer.RestoreSpriteColor();
            }
        }
    }

    //this is jank sorry (last minute)!
    //saves color before sprite starts flashing red so it can be restored
    public class SavedSpriteRenderer
    {
        private SpriteRenderer renderer;
        private Color originalSpriteColor;

        public SavedSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            renderer = spriteRenderer;
            originalSpriteColor = renderer.color;
        }

        //restore the sprite renderer's original color
        public void RestoreSpriteColor()
        {
            renderer.color = originalSpriteColor;
        }

        public void ShowColor(Color color)
        {
            renderer.color = color;
        }
    }
}
