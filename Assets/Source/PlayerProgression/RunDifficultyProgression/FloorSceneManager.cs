using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles loading the next floor, and tracks the current floor. 
/// </summary>
public class FloorSceneManager : MonoBehaviour
{
    // Is a string because unity doesn't natively allow you to set scenes in the inspector as far as I'm aware 
    [Tooltip("The list of floors, in the order they will appear in")]
    [SerializeField] private List<string> _floors;
    static private List<string> floors
    {
        set => instance._floors = value;
        get => instance._floors;
    }

    // Delegate called when a floor is loaded
    private System.Action _onFloorLoaded;
    static public System.Action onFloorLoaded
    {
        set => instance._onFloorLoaded = value;
        get => instance._onFloorLoaded;
    }

    // The current floor
    private int _currentFloor;
    static public int currentFloor
    {
        private set => instance._currentFloor = value;
        get => instance._currentFloor;
    }

    // The instance
    private static FloorSceneManager instance;

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Make it so the manager doesn't disappear when it loads the next floor
        DontDestroyOnLoad(this);

        // Player will always start on floor 0
        currentFloor = 0;
    }

    /// <summary>
    /// Loads a particular floor
    /// </summary>
    /// <param name="floorNumber"> The floor to load (indexing starts at 0) </param>
    /// <returns> Whether or not the floor was succesfully loaded </returns>
    static public bool LoadFloor(int floorNumber)
    {
        if (floorNumber >= floors.Count)
        {
            Debug.LogWarning("Attempted to load floor " + floorNumber + ", which does not exist!");
            return false;
        }
        currentFloor = floorNumber;
        onFloorLoaded?.Invoke();
        SceneManager.LoadScene(floors[floorNumber]);
        return true;
    }

    /// <summary>
    /// Loads the next floor in the list
    /// </summary>
    /// <returns> True if a floor was loaded, false otherwise. Can be false if we are at the end of the list or if load floor failed. </returns>
    static public bool LoadNextFloor()
    {
        if (currentFloor + 1 >= floors.Count) { return false; }
        return LoadFloor(currentFloor + 1);
    }

    /// <summary>
    /// Checks if the floor scene manager is valid or not
    /// </summary>
    /// <returns> True if the instance exists, false otherwise </returns>
    static public bool IsValid()
    {
        return instance != null;
    }
}
