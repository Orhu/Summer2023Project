using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Handles tutorial progression.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        /// <summary>
        /// Opens manual.
        /// </summary>
        void Start()
        {
            MenuManager.Open<InstructionMenu>(lockOpen: true);
            SaveManager.tutorialCompleted = true;
        }
    }
}