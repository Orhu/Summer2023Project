using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Toggles the text to show whether you are in placing mode or not
    /// </summary>
    public class TogglePlacingText : MonoBehaviour
    {
        // Whether or not tiles are being placed
        private bool inPlacingMode = true;

        // A reference to the text component
        private TextMeshProUGUI text;

        /// <summary>
        /// Toggles the placing mode text
        /// </summary>
        public void TogglePlacingModeText()
        {
            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
            }

            inPlacingMode = !inPlacingMode;

            if (inPlacingMode)
            {
                text.text = "Placing: True";
            }
            else
            {
                text.text = "Placing: False";
            }
        }
    }
}