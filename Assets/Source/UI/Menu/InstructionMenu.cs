using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Instruction manager script for handling rendering and logic
    /// for the Instruction menu UI object
    /// </summary>
    public class InstructionMenu : MonoBehaviour
    {
        [Tooltip("List of pages in the manual in order")]
        [SerializeField] private Sprite[] manualPages;

        [Tooltip("The image displaying the current manual page")]
        [SerializeField] private Image manualImage;

        [Tooltip("Button to close the menu")]
        [SerializeField] private Button exitButton;

        [Tooltip("Button to go back one page")]
        [SerializeField] private Button previousButton;

        [Tooltip("Button to go forward one page")]
        [SerializeField] private Button nextButton;

        // The current index of the pages we are on and rendering
        private int pageIndex = 0;

        /// <summary>
        /// Change which buttons are active depending on the page index
        /// </summary>
        private void SetButtonsActive()
        {
            if (pageIndex == manualPages.Length - 1)
            {
                exitButton.gameObject.SetActive(true);
                previousButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(false);
            }
            else if (pageIndex == 0)
            {
                previousButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                previousButton.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Reset the instruction manual on update
        /// </summary>
        private void OnEnable()
        {
            pageIndex = 0;
            SetButtonsActive();
            if (PlayerPrefs.GetInt("seenManual") == 0)
            {
                PlayerPrefs.SetInt("seenManual", 1);
                exitButton.gameObject.SetActive(false);
            }
            else
            {
                exitButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// For use by nextButton.
        /// Moves the page count forward one
        /// </summary>
        public void NextPage()
        {
            if (pageIndex < manualPages.Length - 1)
            {
                pageIndex++;
            }
            SetButtonsActive();
            manualImage.sprite = manualPages[pageIndex];
        }

        /// <summary>
        /// For use by previousButton.
        /// Moves the page count backwards one
        /// </summary>
        public void PreviousPage()
        {
            if (pageIndex > 0)
            {
                pageIndex--;
            }
            SetButtonsActive();
            manualImage.sprite = manualPages[pageIndex];
        }

        /// <summary>
        /// For use by exitButton
        /// Closes out of the Instruction menu
        /// </summary>
        public void ExitInstructions()
        {
            MenuManager.Close<InstructionMenu>(closeLockedMenus: true);
        }
    }
}
