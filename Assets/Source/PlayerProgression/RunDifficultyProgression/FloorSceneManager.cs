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
    private static List<string> floors
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<FloorSceneManager>("FloorOrder");
            }

            return instance._floors;
        }
    }



    // Delegate called when a floor is loaded
    public static System.Action onFloorLoaded;

    // The current floor
    private static int? _currentFloor = null;
    public static int currentFloor 
    { 
        private set
        {
            _currentFloor = value;
        }
        get
        {
            if (_currentFloor == null)
            {
                _currentFloor = floors.IndexOf(SceneManager.GetActiveScene().name);
            }

            return _currentFloor.Value;
        }
    }

    // The instance of the floor order.
    public static bool hasFloorBeenLoaded { private set; get; } = false;


    // The instance of the floor order.
    private static FloorSceneManager instance;

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
        hasFloorBeenLoaded = true;
        return true;
    }

    /// <summary>
    /// Loads the next floor in the list
    /// </summary>
    /// <returns> True if a floor was loaded, false otherwise. Can be false if we are at the end of the list or if load floor failed. </returns>
    static public bool LoadNextFloor()
    {
        return LoadFloor(currentFloor + 1);
    }
}
