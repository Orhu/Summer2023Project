using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cardificer
{
    /// <summary>
    /// Singleton manager class for UI menus
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        // Singleton for the menu manager
        private static MenuManager instance;

        // Stores the prefabs used when instantiating menus.
        private Dictionary<Type, GameObject> menuTypesToPrefabs = new Dictionary<Type, GameObject>();

        // The menus that are currently locked open.
        private HashSet<GameObject> lockedOpenMenus = new HashSet<GameObject>();

        // The menus that are currently pausing the game.
        private HashSet<GameObject> pausingMenus = new HashSet<GameObject>();

        // Whether or not the player is using the controller to navigate menu.
        private static bool usingController = false;


        /// <summary>
        /// Assign singleton variable
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Handle selection initialization.
        /// </summary>
        private void OnNavigate()
        {
            if (EventSystem.current.currentSelectedGameObject != null) { return; }
            GetComponentInChildren<Menu>().InitializeSelection();
            usingController = true;
        }

        /// <summary>
        /// Opens a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to open. </typeparam>
        /// <param name="pauseGame"> Whether or not this will pause the game until this menu is closed. </param>
        /// <param name="closeOtherMenus"> Whether or not this will close other currently open menus. </param>
        /// <param name="lockOpen"> Whether or not this menu will be allowed to be closed by the menu manager. </param>
        /// <returns> The actual menu object that was opened. </returns>
        public static MenuType Open<MenuType>(bool pauseGame = true,  bool lockOpen = false, bool closeOtherMenus = true) where MenuType : Menu
        {
            return (MenuType)Open(typeof(MenuType), pauseGame, lockOpen, closeOtherMenus);
        }
        public static Component Open(Type menuType, bool pauseGame = true, bool lockOpen = false, bool closeOtherMenus = true)
        {
            if (closeOtherMenus)
            {
                CloseAllMenus();
            }

            Component menu = instance.GetComponentInChildren(menuType, true);
            if (menu == null || menu.gameObject.activeSelf)
            {
                GameObject newMenu = Instantiate(GetMenuPrefab(menuType));
                newMenu.transform.SetParent(instance.transform, false);
                menu = newMenu.GetComponent(menuType);
            }

            menu.transform.SetAsLastSibling();

            if (lockOpen)
            {
                instance.lockedOpenMenus.Add(menu.gameObject);
            }
            else
            {
                instance.lockedOpenMenus.Remove(menu.gameObject);
            }

            if (pauseGame)
            {
                Time.timeScale = 0;
                instance.pausingMenus.Add(menu.gameObject);
            }

            menu.gameObject.SetActive(true);
            if (usingController)
            {
                (menu as Menu).InitializeSelection();
            }
            return menu;
        }

        /// <summary>
        /// Closes a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to close. </typeparam>
        /// <param name="closeLockedMenus"> Whether or not this will close locked menus. </param>
        /// <returns> The actual menu object that was closed. </returns>
        public static MenuType Close<MenuType>(bool closeLockedMenus = false) where MenuType : Menu
        {
            return (MenuType)Close(typeof(MenuType), closeLockedMenus);
        }
        public static Component Close(Type menuType, bool closeLockedMenus = false)
        {
            Component menu = instance.GetComponentInChildren(menuType);

            if (menu == null || instance.lockedOpenMenus.Contains(menu.gameObject) && !closeLockedMenus) { return null; }

            menu.gameObject.SetActive(false);
            instance.pausingMenus.Remove(menu.gameObject);

            if (instance.pausingMenus.Count == 0)
            {
                Time.timeScale = 1;
            }

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
            instance.pausingMenus.Clear();
            Time.timeScale = 1;
        }

        /// <summary>
        /// Toggles a given menu open/closed.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to toggle. </typeparam>
        /// <param name="newIsOpened"> Whether or not the menu is now open or closed. </param>
        /// <param name="closeOtherMenus"> Whether or not this will close other currently open menus. </param>
        /// <returns> The actual menu object that was toggled. </returns>
        public static MenuType Toggle<MenuType>(out bool newIsOpened, bool closeOtherMenus = true) where MenuType : Menu
        {
            return (MenuType)Toggle(typeof(MenuType), out newIsOpened, closeOtherMenus);
        }
        public static Component Toggle(Type menuType, out bool newIsOpened, bool closeOtherMenus = true)
        {
            newIsOpened = !IsMenuOpen(menuType);
            if (newIsOpened)
            {
                return Open(menuType, closeOtherMenus: closeOtherMenus);
            }
            else
            {
                return Close(menuType);
            }
        }
        public static MenuType Toggle<MenuType>(bool closeOtherMenus = true) where MenuType : Menu
        {
            return Toggle<MenuType>(out bool _, closeOtherMenus);
        }
        public static Component Toggle(Type menuType, bool closeOtherMenus = true)
        {
            return Toggle(menuType, out bool _, closeOtherMenus);
        }

        /// <summary>
        /// Gets if a given menu is open.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu to see if its open. </typeparam>
        /// <returns> True if the given menu is open. </returns>
        public static bool IsMenuOpen<MenuType>() where MenuType : Menu
        {
            return IsMenuOpen(typeof(MenuType));
        }
        public static bool IsMenuOpen(Type menuType)
        {
            return (instance.GetComponentInChildren(menuType)?.gameObject.activeSelf).GetValueOrDefault();
        }


        /// <summary>
        /// Gets the prefab to instantiate for a given menu.
        /// </summary>
        /// <typeparam name="MenuType"> The type of menu get the prefab for. </typeparam>
        /// <returns> The prefab of the given menu. </returns>
        private static GameObject GetMenuPrefab(Type menuType)
        {
            if (instance.menuTypesToPrefabs.TryGetValue(menuType, out GameObject value))
            {
                return value;
            }


#if UNITY_EDITOR
            foreach (string guid in AssetDatabase.FindAssets("*", new[] { "Assets/Source/UI/Menu/SubMenus" }))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (prefab?.GetComponent(menuType) != null)
                {
                    instance.menuTypesToPrefabs.Add(menuType, prefab);
                    return prefab;
                }
            }

            throw new System.Exception($"No prefab for {menuType} that is Assets/Source/UI/Menu/SubMenus IMPORTAINT: Be sure to add the asset to the menus asset bundle in addition to adding it to the folder.");
#else
            AssetBundle menusAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "menus"));
            foreach (GameObject prefab in menusAssetBundle.LoadAllAssets<GameObject>())
            {
                if (prefab.GetComponent(menuType) != null)
                {
                    instance.menuTypesToPrefabs.Add(menuType, prefab);
                    menusAssetBundle.Unload(false);
                    return prefab;
                }
            }

            menusAssetBundle.Unload(false);
            throw new System.Exception($"No prefab for {menuType} that is in the menus asset bundle.");
#endif

        }
    }
}