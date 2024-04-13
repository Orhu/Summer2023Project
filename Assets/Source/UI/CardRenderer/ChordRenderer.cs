using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Represents the ChordRenderer UI element
    /// </summary>
    public class ChordRenderer : MonoBehaviour
    {
        [Tooltip("Image representing the chorded runes base image")]
        public Image chordRootImage;

        [Tooltip("Image representing the chorded runes background image")]
        public Image chordBackgroundImage;

        [Tooltip("The text representing the name of the root chord card.")]
        public TextMeshProUGUI chordRootText;

        [Tooltip("The text representing the effect of the second chorded card.")]
        public TextMeshProUGUI chordEffectText;

        [Tooltip("Animator for the chordRenderer")]
        public Animator chordAnimator;

        // The card attached to the chord root image
        private Card chordRootCard;

        /// <summary>
        /// Displays the first level of chording, just displaying the root image
        /// </summary>
        /// <param name="currentCardInHand">The card that is being previewed (being chorded)</param>
        public void DisplayChordLevelOne(Card currentCardInHand)
        {
            if (chordRootCard != currentCardInHand)
            {
                chordRootCard = currentCardInHand;
                chordAnimator.Play("A_RuneRenderer_Spin");
            }
            chordRootImage.enabled = true;
            chordRootImage.sprite = currentCardInHand.chordImage;
            chordBackgroundImage.color = currentCardInHand.chordColor;
            chordEffectText.text = "";
            chordEffectText.enabled = false;
            chordRootText.enabled = true;
            chordRootText.text = currentCardInHand.displayName;
        }

        /// <summary>
        /// Displays the second level of chording, changing the background color of the chord
        /// </summary>
        /// <param name="currentCardInHand">The card that is being previewed (being chorded)</param>
        public void DisplayChordLevelTwo(Card currentCardInHand)
        {
            chordBackgroundImage.color = currentCardInHand.chordColor;
            chordEffectText.enabled = true;
            chordEffectText.text = "+" + currentCardInHand.chordEffectText;
            chordEffectText.color = currentCardInHand.chordColor;
        }

        /// <summary>
        /// Resets the UI of the chord to be empty
        /// </summary>
        public void ResetChord()
        {
            chordRootCard = null;
            chordRootImage.enabled = false;
            chordBackgroundImage.color = Color.white;
            chordRootText.text = "";
            chordRootText.enabled = false;
            chordEffectText.text = "";
            chordEffectText.enabled = false;
        }
    }
}