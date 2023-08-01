using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Controls the aim indicator
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAimIndicatorController : MonoBehaviour
    {
        [Tooltip("The game object to rotate")]
        [SerializeField] private GameObject aimIndicator;

        // A reference to the player controller
        private PlayerController playerController;

        /// <summary>
        /// Sets the reference to the player controller
        /// </summary>
        void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        /// <summary>
        /// Updates the rotation of the aim indicator
        /// </summary>
        void Update()
        {
            if (aimIndicator == null) { return; }

            if (!playerController.lastInputWasGamepad || playerController.aimDirection == Vector2.zero)
            {
                aimIndicator.SetActive(false);
                return;
            }

            aimIndicator.SetActive(true);

            if (playerController.aimDirection == Vector2.zero) { return; }

            float pointingLeftOffset = playerController.aimDirection.x < 0 ? 180 : 0;
            float mirroredOffset = aimIndicator.transform.lossyScale.x < 0 ? 180 : 0;

            aimIndicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan(playerController.aimDirection.y / playerController.aimDirection.x) * Mathf.Rad2Deg + pointingLeftOffset + mirroredOffset);
        }
    }
}