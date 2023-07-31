using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Menu for choosing what boon to pickup
    /// </summary>
    public class PickABoonMenu : MonoBehaviour
    {
        [Tooltip("The text to display the boon name in.")]
        [SerializeField] private TMP_Text boonNameTextbox;

        [Tooltip("The text to display the boon description in.")]
        [SerializeField] private TMP_Text boonDescriptionTextbox;

        [Tooltip("The text to display the boon pick count text in.")]
        [SerializeField] private TMP_Text boonPickCountTextbox;

        [Tooltip("The boons in this menu.")]
        [SerializeField] private List<BoonInfo> boons;

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
            public GameObject boonPrefab;
        }
    }
}