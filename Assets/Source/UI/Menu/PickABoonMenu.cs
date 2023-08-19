using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Menu for choosing what boon to pickup
    /// </summary>
    public class PickABoonMenu : Menu
    {
        [Tooltip("Called when any boon has been selected.")]
        public UnityEvent onSelected;

        [Tooltip("Called when any boon has been deselected.")]
        public UnityEvent onDeselected;


        [Tooltip("The text to display the boon name in.")]
        [SerializeField] private TMP_Text boonNameTextbox;

        [Tooltip("The text to display the boon description in.")]
        [SerializeField] private TMP_Text boonDescriptionTextbox;

        [Tooltip("The text to display the boon pick count text in.")]
        [SerializeField] private TMP_Text boonPickCountTextbox;

        [Tooltip("The boons in this menu.")]
        [SerializeField] private List<BoonInfo> boons;


        // Called when a boon was picked by the player.
        public event System.Action onPicked;

        // The currently selected boon.
        public Boon selectedBoon { private set; get; }

        /// <summary>
        /// Used to organize the boons this menu can give.
        /// </summary>
        [System.Serializable]
        private class BoonInfo
        {
            [Tooltip("The name of the boon")]
            public string nameText = "Unnamed Boon";

            [Tooltip("Description of the boon")] [Multiline]
            public string descriptionText = "This boon does not have a description written for it but it needs one.";

            [Tooltip("The text describing how many times this boon has been picked. [Pick Count] will be replaced by the actual count")]
            public string pickCountText = "This boon has been picked [Pick Count] times";

            [Tooltip("The button used to select this boon.")]
            public Button button;

            [Tooltip("The boon to actually give the player.")]
            public Boon boon;

            [Tooltip("Called when this boon has been selected.")]
            [SerializeField] public UnityEvent onSelected;
        }

        /// <summary>
        /// Initializes all bindings.
        /// </summary>
        private void Start()
        {
            onDeselected.AddListener(
                () =>
                {
                    selectedBoon = null;
                });

            foreach (BoonInfo boonInfo in boons)
            {
                boonInfo.button.onClick.AddListener(
                    // Set text appropriately
                    () =>
                    {
                        boonNameTextbox.text = boonInfo.nameText;
                        boonDescriptionTextbox.text = boonInfo.descriptionText;
                        boonPickCountTextbox.text = boonInfo.pickCountText.Replace("[Pick Count]", boonInfo.boon.pickCount.ToString());

                        selectedBoon = boonInfo.boon;
                        onSelected?.Invoke();
                        boonInfo.onSelected?.Invoke();
                    });
            }
        }

        /// <summary>
        /// Reset on reopen
        /// </summary>
        private void OnEnable()
        {
            onDeselected?.Invoke();
        }

        /// <summary>
        /// Applies the selected boon and closes this.
        /// </summary>
        public void PickSelectedBoon()
        {
            selectedBoon.Apply();
            MenuManager.Close<PickABoonMenu>(true);
        }
    }
}