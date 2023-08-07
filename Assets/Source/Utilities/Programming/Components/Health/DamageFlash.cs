using System.Collections;
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



        private SpriteRenderer spriteRenderer;

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
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }


        /// <summary>
        /// Enables or disables tinting of the sprite.
        /// </summary>
        /// <returns> The time to stay tinted. </returns>
        private IEnumerator Flash()
        {
            spriteRenderer.color = invincibilityFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
    }
}
