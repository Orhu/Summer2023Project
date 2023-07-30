using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class TutorialManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MenuManager.Open<InstructionMenu>(lockOpen: true);
            SaveManager.tutorialCompleted = true;
        }
    }
}