using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    
    private void SetTintEnable(bool tintEnabled)
    {
        spriteRenderer.color = tintEnabled ? invincibilityFlashColor : Color.white;
    }
}
