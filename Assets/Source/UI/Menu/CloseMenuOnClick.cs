using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// Closes a given menu on click of a button attached to the same game object
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class CloseMenuOnClick : MonoBehaviour
    {
        [Tooltip("The name of the menu to close, must match the class name of the menu. Ex: for the pause menu enter PauseMenu")]
        [SerializeField] private string menuName;

        /// <summary>
        /// Bind on click.
        /// </summary>
        void Awake()
        {
            Type menuType = Type.GetType($"Cardificer.{menuName}");
            GetComponent<Button>().onClick.AddListener(() => { MenuManager.Close(menuType); });
        }
    }
}