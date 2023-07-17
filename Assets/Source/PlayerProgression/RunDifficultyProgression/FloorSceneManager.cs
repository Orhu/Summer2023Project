using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles loading the next floor, and tracks the current floor. 
/// </summary>
[CreateAssetMenu(menuName ="Test")] [ExecuteAlways]
public class FloorSceneManager : ScriptableObject
{
    // Is a string because unity doesn't natively allow you to set scenes in the inspector as far as I'm aware 
    [Tooltip("The list of floors, in the order they will appear in")]
    [SerializeField] private List<string> _floors;
    private static List<string> floors;

    // Delegate called when a floor is loaded
    public static System.Action onFloorLoaded;

    // The current floor
    public static int currentFloor { private set; get; }

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    private void OnEnable()
    {
        if (floors == null)
        {
            floors = _floors;
        }
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
        return floors != null;
    }
}
