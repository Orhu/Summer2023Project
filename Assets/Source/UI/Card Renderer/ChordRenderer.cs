using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the ChordRenderer UI element
/// </summary>
public class ChordRenderer : MonoBehaviour
{
    [Tooltip("Image representing the chorded runes base image")]
    public Image chordRootImage;

    [Tooltip("Image representing the chorded runes background image")]
    public Image chordBackgroundImage;

    [Tooltip("Star representing the third card that's chorded")]
    public Image chordStarImage;

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
        chordStarImage.enabled = false;
    }

    /// <summary>
    /// Displays the second level of chording, changing the background color of the chord
    /// </summary>
    /// <param name="currentCardInHand">The card that is being previewed (being chorded)</param>
    public void DisplayChordLevelTwo(Card currentCardInHand)
    {
        chordBackgroundImage.color = currentCardInHand.chordColor;
        chordStarImage.enabled = false;
    }

    /// <summary>
    /// Displays the third level of chording, displays the rotating star 
    /// with the color of the currentCardInHand
    /// </summary>
    /// <param name="currentCardInHand">The card that is being previewed (being chorded)</param>
    public void DisplayChordLevelThree(Card currentCardInHand)
    {
        chordStarImage.enabled = true;
        chordStarImage.color = currentCardInHand.chordColor;
    }

    /// <summary>
    /// Resets the UI of the chord to be empty
    /// </summary>
    public void ResetChord()
    {
        chordRootImage.enabled = false;
        chordStarImage.enabled = false;
        chordBackgroundImage.color = Color.white;
    }
}
