using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Opens a menu with a confirmation window to open it
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class OpenConfirmationMenuOnClick : MonoBehaviour
    {
        [Tooltip("The text that describes what is being confirmed/denied.")] [Multiline]
        [SerializeField] private string text = "Are you sure?";

        [Tooltip("The text that goes on the confirm button.")]
        [SerializeField] private string confirmButtonText = "Yes";

        [Tooltip("The text that goes on the deny button.")]
        [SerializeField] private string denyButtonText = "No";

        [Tooltip("Called when the confirmation box was confirmed.")]
        public UnityEvent onConfirmed;

        [Tooltip("Called when the confirmation box was denied.")]
        public UnityEvent onDenied;

        // Set this to true to not open the confirmation menu on clicked.
        public bool skipConfirmation { set; get; } = false;

        /// <summary>
        /// Bind on click.
        /// </summary>
        void OnEnable()
        {
            GetComponent<Button>().onClick.RemoveListener(Confirm);
            GetComponent<Button>().onClick.RemoveListener(OpenConfirmationBox);
            if (skipConfirmation)
            {
                GetComponent<Button>().onClick.AddListener(Confirm);
            }
            else
            {
                GetComponent<Button>().onClick.AddListener(OpenConfirmationBox);
            }

            void Confirm()
            {
                onConfirmed?.Invoke();
            }

            void Deny()
            {
                onDenied?.Invoke();
            }

            void OpenConfirmationBox()
            {
                ConfirmationPopup popup = MenuManager.Open<ConfirmationPopup>(closeOtherMenus: false);
                popup.text = text;
                popup.confirmButtonText = confirmButtonText;
                popup.denyButtonText = denyButtonText;

                popup.onConfirmed += Confirm;
                popup.onDenied += Deny;
            }
        }
    }
}