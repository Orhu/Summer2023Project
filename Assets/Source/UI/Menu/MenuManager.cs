using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Singleton for the menu manager
    [HideInInspector]public static MenuManager instance;
    // Booster pack menu reference, assigned in inspector
    public BoosterPackMenu boosterPackMenu;

    /// <summary>
    /// Assign singleton variable
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if(boosterPackMenu == null)
        {
            boosterPackMenu = GetComponentInChildren<BoosterPackMenu>();
        }
    }
}
