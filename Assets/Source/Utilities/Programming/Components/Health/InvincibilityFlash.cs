using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Health))]
    public class InvincibilityFlash : MonoBehaviour
    {
        [Tooltip("The color to make the sprite when invincible.")]
        [SerializeField] private Color invincibilityFlashColor;

        [Tooltip("Sprite Renderer to change tint of.")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Initializes references
        /// </summary>
        private void Awake()
        {
            GetComponent<Health>().onInvincibilityChanged += SetTintEnable;
            if (spriteRenderer == null) { // temporary while i rewire all the enemies
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }


        /// <summary>
        /// Enables or disables tinting of the sprite.
        /// </summary>
        /// <param name="tintEnabled"> Whether or not the tint should be shown. </param>
        private void SetTintEnable(bool tintEnabled)
        {
            spriteRenderer.color = tintEnabled ? invincibilityFlashColor : Color.white;
        }
    }
}
