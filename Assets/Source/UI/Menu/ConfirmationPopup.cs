using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Class for creating a popup menu for an action.
    /// </summary>
    public class ConfirmationPopup : Menu
    {
        [Tooltip("The text box that describes what is being confirmed/denied.")]
        [SerializeField] private TMP_Text descriptionTextBox;

        [Tooltip("The button for confirming the action.")]
        [SerializeField] private Button confirmButton;

        [Tooltip("The text box for the confirm button.")]
        [SerializeField] private TMP_Text confirmTextBox;

        [Tooltip("The button for confirming the action.")]
        [SerializeField] private Button denyButton;

        [Tooltip("The text box for the deny button.")]
        [SerializeField] private TMP_Text denyTextBox;


        // The text that describes what is being confirmed/denied.
        public string text
        {
            set => descriptionTextBox.text = value;
        }

        // The text that goes on the confirm button.
        public string confirmButtonText
        {
            set => confirmTextBox.text = value;
        }

        // The text that goes on the deny button.
        public string denyButtonText
        {
            set => denyTextBox.text = value;
        }


        // Called when the action is confirmed.
        public event System.Action onConfirmed;

        // Called when the action is denied.
        public event System.Action onDenied;


        /// <summary>
        /// Sets text & bindings.
        /// </summary>
        private void Awake()
        {
            confirmButton.onClick.AddListener(
                () =>
                {
                    onConfirmed?.Invoke();
                    onConfirmed = null;
                    onDenied = null;
                    MenuManager.Close<ConfirmationPopup>();
                });
            denyButton.onClick.AddListener(
                () =>
                {
                    onDenied?.Invoke();
                    onConfirmed = null;
                    onDenied = null;
                    MenuManager.Close<ConfirmationPopup>();
                });
        }

        /// <summary>
        /// Calls denied if this was closed without picking an option.
        /// </summary>
        private void OnDisable()
        {
            onDenied?.Invoke();
            onConfirmed = null;
            onDenied = null;
        }
    }
}