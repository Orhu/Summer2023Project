using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cardificer
{
    /// <summary>
    /// Base class for all menus that handle controller navigation
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        [Tooltip("The game object that will be selected when this menu is navigated to.")]
        [SerializeField] protected GameObject initialSelection;

        /// <summary>
        /// Sets the selection to the initial selection.
        /// </summary>
        public void InitializeSelection()
        {
            StartCoroutine(WaitForIntialization());
            IEnumerator WaitForIntialization()
            {
                for (int i = 0; i < 30 && initialSelection == null; i++)
                {
                    yield return null;
                }
                EventSystem.current.SetSelectedGameObject(initialSelection);
            }
        }
    }
}