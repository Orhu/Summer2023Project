using System;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    [RequireComponent(typeof(Button))]
    public class CloseAllMenusOnClick : MonoBehaviour
    {
        /// <summary>
        /// Bind on click.
        /// </summary>
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { MenuManager.CloseAllMenus(); });
        }
    }
}