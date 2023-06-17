using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Base class for all components that move game objects
    /// </summary>
    [RequireComponent(typeof(AnimatorController))]
    public abstract class Movement : MonoBehaviour
    {
        [Tooltip("The amount knockback will be multiplied by.")]
        [Min(0f)]
        [SerializeField] protected float knockbackMultiplier = 1f;

        // The desired movement direction.
        private Vector2 _movementInput;
        public virtual Vector2 movementInput
        {
            get => _movementInput;
            set
            {
                _movementInput = value;
                animatorComponent.SetBool("moving", value.sqrMagnitude > 0);
                if (value.x == 0) { return; }
                animatorComponent.SetMirror("runLeft", value.x < 0);
            }
        }

        // A delegate that is queried before moving to adjust the speed based on outside factors.
        public delegate void ModifySpeed(ref float speed);
        public ModifySpeed requestSpeedModifications;

        // animator component to make the pretty animations do their thing
        private AnimatorController animatorComponent;

        /// <summary>
        /// Initialize Reference
        /// </summary>
        protected void Awake()
        {
            animatorComponent = GetComponent<AnimatorController>();
        }

        /// <summary>
        /// Causes knockback to this moment component.
        /// </summary>
        /// <param name="direction"> The direction to apply the knockback in. Should be normalized. </param>
        /// <param name="info"> The knockback info. </param>
        public abstract void Knockback(Vector2 direction, KnockbackInfo info);
    }

    /// <summary>
    /// The info needed to apply knockback to something.
    /// </summary>
    [System.Serializable]
    public class KnockbackInfo
    {
        [Tooltip("The distance this will move in tiles.")]
        public float amount = 1f;

        [Tooltip("The time it will take to reach the target destination.")]
        public float duration = 0.2f;

        [Tooltip("Whether or not this should lose all its momentum when hit")]
        public bool resetMomentum = true;
    }
}