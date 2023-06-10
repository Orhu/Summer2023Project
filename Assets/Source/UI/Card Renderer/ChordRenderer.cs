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

    public void DisplayChordLevelTwo(Card currentCardInHand)
    {
        chordBackgroundImage.color = currentCardInHand.chordColor;
        chordStarImage.enabled = false;
    }

    public void DisplayChordLevelThree(Card currentCardInHand)
    {
        chordStarImage.enabled = true;
        chordStarImage.color = currentCardInHand.chordColor;
    }

    public void ResetChord()
    {
        chordRootImage.enabled = false;
        chordStarImage.enabled = false;
        chordBackgroundImage.color = Color.white;
    }
}
