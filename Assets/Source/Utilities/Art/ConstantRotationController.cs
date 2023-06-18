using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Utility script to apply a constant rotation on the GameObject of a projectile over its lifetime.
    ///
    /// If you only want the sprite to rotate, you will need to place the sprite renderer and this script on a child GameObject by themselves.
    /// </summary>
    public class ConstantRotationController : MonoBehaviour
    {
        [Tooltip("Rate of rotation in degrees per second")]
        [SerializeField] private float rotationRate;

        [Tooltip("Sets whether to rotate clockwise (TRUE) or counter-clockwise (FALSE)")]
        [SerializeField] private bool rotateClockwise = true;

        /// <summary>
        /// Constant rotation is applied to the GameObject each frame.
        /// </summary>
        void Update()
        {
            float rotationDirectionVal = rotateClockwise ? 1f : -1f;
            this.transform.Rotate(0f, 0f, this.transform.rotation.z - rotationDirectionVal * rotationRate * Time.deltaTime);
        }
    }
}