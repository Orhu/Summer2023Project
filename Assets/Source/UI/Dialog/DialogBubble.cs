using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Class used for managing the text of a dialog bubble
    /// </summary>
    public class DialogBubble : MonoBehaviour
    {
        [Tooltip("Text that will be displayed in the dialog bubble")]
        [TextArea]
        public string text;

        private TextMeshProUGUI textMesh;

        [Tooltip("How much of a delay between each character of the text being drawn")]
        [SerializeField] private float textDelay;

        [Tooltip("What bubble sprite to be used")]
        [SerializeField] private Sprite bubbleType;

        private Image bubbleImage;

        [Tooltip("The local offset of where to put the dialogue bubble.")]
        [SerializeField] private Vector2 localOffset;

        [Tooltip("The local scale of the bubble, where (1, 1) is about the size of the player.")]
        [SerializeField] private Vector2 localScale;

        // A reference to the original scale.
        private Vector2 baseScale;

        /// <summary>
        /// Find the text mesh component of the dialog box
        /// </summary>
        private void Awake()
        {
            if (textMesh == null)
            {
                textMesh = GetComponentInChildren<TextMeshProUGUI>();
            }
            if (bubbleImage == null)
            {
                bubbleImage = GetComponentInChildren<Image>();
            }

            if (bubbleType != null)
            {
                bubbleImage.sprite = bubbleType;
            }
            baseScale = transform.localScale;

            // Set initial position and scale
            SetPosition(localOffset);
            SetScale(localScale);
        }

        /// <summary>
        /// When the text box is reenabled, have it typewrite the text.
        /// </summary>
        private void OnEnable()
        {
            ResetText();
            SetText(text);
        }

        /// <summary>
        /// For resetting the text mesh text back to a blank
        /// </summary>
        private void ResetText()
        {
            textMesh.text = "";
        }

        /// <summary>
        /// Calls the coroutine to typewrite the text using the local text variable,
        /// the text mesh, and a text delay
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text) 
        {
            StartCoroutine(TypeWriterEffect(text, textMesh, textDelay));
        }

        /// <summary>
        /// For changing the position of the textbox dynamically
        /// </summary>
        /// <param name="offset">Offset from a (0,0) local position</param>
        public void SetPosition(Vector2 offset)
        {
            transform.localPosition = Vector2.zero + offset;
        }

        /// <summary>
        /// For changing the scale of the textbox dynamically
        /// </summary>
        /// <param name="scale">from a (1,1) local scale</param>
        public void SetScale(Vector2 scale)
        {
            transform.localScale = baseScale * scale;
        }

        /// <summary>
        /// Creates a typewriter effect that displays one text character at a time
        /// </summary>
        /// <param name="text">the text to be displayed</param>
        /// <param name="tmp">the text mesh to display the text</param>
        /// <param name="delay">the delay for each character</param>
        /// <returns></returns>
        IEnumerator TypeWriterEffect(string text, TextMeshProUGUI tmp, float delay)
        {
            foreach (char c in text)
            {
                tmp.text += c;
                yield return new WaitForSeconds(delay);
            }
            tmp.text = text;
        }
    }
}

