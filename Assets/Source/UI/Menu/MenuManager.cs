using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Singleton manager class for UI menus
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        // Singleton for the menu manager
        private static MenuManager instance;

        /// <summary>
        /// Assign singleton variable
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        // Stores the prefabs used when instantiating menus.
        private Dictionary<Type, GameObject> menuTypesToPrefabs = new Dictionary<Type, GameObject>();

        // The menus that are currently locked open.
        private HashSet<GameObject> lockedOpenMenus = new HashSet<GameObject>();

        /// <summary>
        /// Opens a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to open. </typeparam>
        /// <param name="closeOtherMenus"> Whether or not this will close other currently open menus. </param>
        /// <param name="lockOpen"> Whether or not this menu will be allowed to be closed by the menu manager. </param>
        /// <returns> The actual menu object that was opened. </returns>
        public static MenuType Open<MenuType>(bool lockOpen = false, bool closeOtherMenus = true) where MenuType : Component
        {
            if (closeOtherMenus)
            {
                CloseAllMenus();
            }

            MenuType menu = instance.GetComponentInChildren<MenuType>();
            if (menu == null)
            {
                GameObject newMenu = Instantiate(GetMenuPrefab<MenuType>());
                newMenu.transform.SetParent(instance.transform);
                menu = newMenu.GetComponent<MenuType>();
            }

            if (lockOpen)
            {
                instance.lockedOpenMenus.Add(menu.gameObject);
            }
            else
            {
                instance.lockedOpenMenus.Remove(menu.gameObject);
            }

            menu.gameObject.SetActive(true);
            return menu;
        }

        /// <summary>
        /// Closes a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to close. </typeparam>
        /// <param name="closeLockedMenus"> Whether or not this will close locked menus. </param>
        /// <returns> The actual menu object that was closed. </returns>
        public static MenuType Close<MenuType>(bool closeLockedMenus = false) where MenuType : Component
        {
            MenuType menu = instance.GetComponentInChildren<MenuType>();

            if (menu == null || instance.lockedOpenMenus.Contains(menu.gameObject) && !closeLockedMenus) { return null; }

            menu.gameObject.SetActive(false);
            return menu;
        }

        /// <summary>
        /// Closes all currently open menus.
        /// </summary>
        /// <param name="closeLockedMenus"> Whether or not this will close locked menus. </param>
        public static void CloseAllMenus(bool closeLockedMenus = false)
        {
            for (int i = 0; i < instance.transform.childCount; i++)
            {
                GameObject child = instance.transform.GetChild(i).gameObject;
                if (closeLockedMenus || !instance.lockedOpenMenus.Contains(child))
                {
                    child.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Toggles a given menu open/closed.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to toggle. </typeparam>
        /// <param name="newIsOpened"> Whether or not the menu is now open or closed. </param>
        /// <param name="closeOtherMenus"> Whether or not this will close other currently open menus. </param>
        /// <returns> The actual menu object that was toggled. </returns>
        public static MenuType Toggle<MenuType>(out bool newIsOpened, bool closeOtherMenus = true) where MenuType : Component
        {
            newIsOpened = !IsMenuOpen<MenuType>();
            if (newIsOpened)
            {
                return Open<MenuType>(closeOtherMenus: closeOtherMenus);
            }
            else
            {
                return Close<MenuType>();
            }
        }
        public static MenuType Toggle<MenuType>(bool closeOtherMenus = true) where MenuType : Component
        {
            return Toggle<MenuType>(out bool _, closeOtherMenus);
        }

        /// <summary>
        /// Gets if a given menu is open.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to see if its open. </typeparam>
        /// <returns> True if the given menu is open. </returns>
        public static bool IsMenuOpen<MenuType>() where MenuType : Component
        {
            return (instance.GetComponentInChildren<MenuType>()?.gameObject.activeSelf).GetValueOrDefault();
        }


        /// <summary>
        /// Gets the prefab to instantiate for a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu get the prefab for. </typeparam>
        /// <returns> The prefab of the given menu. </returns>
        private static GameObject GetMenuPrefab<MenuType>() where MenuType : Component
        {
            if (instance.menuTypesToPrefabs.TryGetValue(typeof(MenuType), out GameObject value))
            {
                return value;
            }


#if UNITY_EDITOR
            foreach (string guid in AssetDatabase.FindAssets("*", new[] { "Assets/Source/UI/Menu/SubMenus" }))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (prefab?.GetComponent<MenuType>() != null)
                {
                    instance.menuTypesToPrefabs.Add(typeof(MenuType), prefab);
                    return prefab;
                }
            }

            throw new System.Exception($"No prefab for {typeof(MenuType)} that is Assets/Source/UI/Menu/SubMenus IMPORTAINT: Be sure to add the asset to the menus asset bundle in addition to adding it to the folder.");
#else
            AssetBundle menusAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "menus"));
            foreach (GameObject prefab in menusAssetBundle.LoadAllAssets<GameObject>())
            {
                if (prefab.GetComponent<MenuType>() != null)
                {
                    instance.menuTypesToPrefabs.Add(typeof(MenuType), prefab);
                    menusAssetBundle.Unload(false);
                    return prefab;
                }
            }

            menusAssetBundle.Unload(false);
            throw new System.Exception($"No prefab for {typeof(MenuType)} that is in the menus asset bundle.");
#endif

        }
    }
}