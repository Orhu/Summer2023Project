using UnityEngine;

/// <summary>
/// Loads the next floor when the player enters it
/// </summary>
public class Staircase : MonoBehaviour
{
    /// <summary>
    /// Handles entering the staircase, loading the next floor
    /// </summary>
    /// <param name="collision"> The collision that entered the staircase </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }

        if (!FloorSceneManager.LoadNextFloor())
        {
            Debug.Log("YOU WIN!!! WOAH");
        }
    }
}
