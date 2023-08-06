using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    [RequireComponent(typeof(Button))]
    public class OpenMenuOnClick : MonoBehaviour
    {
        [Tooltip("The name of the menu to open, must match the class name of the menu. Ex: for the pause menu enter PauseMenu")]
        [SerializeField] private string menuName;

        /// <summary>
        /// Bind on click.
        /// </summary>
        void Awake()
        {
            Type menuType = Type.GetType($"Cardificer.{menuName}");
            GetComponent<Button>().onClick.AddListener(() => { MenuManager.Open(menuType); });
        }
    }
}