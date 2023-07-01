using TMPro;
using UnityEngine;

public class TogglePlacingText : MonoBehaviour
{
    // Whether or not tiles are being placed
    private bool inPlacingMode = true;

    // A reference to the text component
    private TextMeshPro text;

    /// <summary>
    /// Toggles the placing mode text
    /// </summary>
    public void TogglePlacingModeText()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshPro>();
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
