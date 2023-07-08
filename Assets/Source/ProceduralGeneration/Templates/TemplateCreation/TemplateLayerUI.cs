using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TemplateLayerUI : MonoBehaviour
{
    [Tooltip("A reference to the input field for the layer")]
    [SerializeField] private TMP_InputField inputField;

    [Tooltip("A reference to the visible image")]
    [SerializeField] private Image visibleImage;

    [Tooltip("A reference to the hidden image")]
    [SerializeField] private Image hiddenImage;

    [Tooltip("A reference to the remove button")]
    [SerializeField] private GameObject removeButton;

    [Tooltip("A reference to the active star")]
    [SerializeField] private TMP_Text activeText;

    // Whether or not this layer is removable
    [HideInInspector] public bool removable = true;

    // Delegate called when the layer is named
    public System.Action<string> onLayerNamed;

    // Delegate called when the hide button is pressed
    public System.Action onLayerHiddenToggled;

    // Delegate called when the activate button is pressed
    public System.Action onLayerActivated;

    // Delegate called when the removed button is pressed
    public System.Action onLayerRemoved;

    // Whether or not this layer is currently hidden
    private bool hidden = false;

    /// <summary>
    /// Removes the remove button if not removable
    /// </summary>
    private void Start()
    {
        if (!removable)
        {
            Destroy(removeButton);
        }
    }

    /// <summary>
    /// Called on input field finished editing
    /// </summary>
    public void OnNamed()
    {
        onLayerNamed?.Invoke(inputField.text);
    }

    /// <summary>
    /// Updates the input field text
    /// </summary>
    /// <param name="name"> The new name of the layer </param>
    public void Name(string name)
    {
        inputField.text = name;
    }

    /// <summary>
    /// Called when hide button is pressed
    /// </summary>
    public void OnHiddenToggled()
    {
        onLayerHiddenToggled?.Invoke();
        hidden = !hidden;
        hiddenImage.gameObject.SetActive(hidden);
        visibleImage.gameObject.SetActive(!hidden);
    }

    /// <summary>
    /// Called when the activate button was pressed
    /// </summary>
    public void OnActivated()
    {
        onLayerActivated?.Invoke();
        activeText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the active star
    /// </summary>
    public void Deactivate()
    {
        activeText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the remove button is pressed
    /// </summary>
    public void OnRemoved()
    {
        onLayerRemoved?.Invoke();
    }
}
