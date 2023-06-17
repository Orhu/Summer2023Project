using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(Health), typeof(SpriteRenderer))]
    public class InvincibilityFlash : MonoBehaviour
    {
        [Tooltip("The color to make the sprite when invincible.")]
        [SerializeField] private Color invincibilityFlashColor;



        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// Initializes references
        /// </summary>
        private void Awake()
        {
            GetComponent<Health>().onInvincibilityChanged += SetTintEnable;
            spriteRenderer = GetComponent<SpriteRenderer>();
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
