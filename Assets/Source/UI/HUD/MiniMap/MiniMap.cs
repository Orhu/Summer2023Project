using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Gives functionality to HUD minimap 
    /// mainly for drawing and resetting the map itself
    /// </summary>
    public class MiniMap : MonoBehaviour
    {
        [Tooltip("Grid layout for organizing room visuals")]
        [SerializeField] private GameObject roomImageContainer;

        [Tooltip("Room visual prefab, change it's sprite to change the background color of the room visual.")]
        [SerializeField] private GameObject cellVisualPrefab;

        [Tooltip("Sprite representing a blank room space")]
        [SerializeField] private Sprite blankRoomSprite;

        [Tooltip("Sprite representing an non visited room")]
        [SerializeField] private Sprite nonVisitedRoomSprite;

        [Tooltip("Sprite representing a visited room")]
        [SerializeField] private Sprite visitedRoomSprite;

        // A local reference to the current room, observes current room changes
        private Room localCurrentRoom;

        [Tooltip("Float representing the padding between rooms")]
        [SerializeField] private float roomPadding = 10f;

        [Tooltip("How much we scale drawing each room from each other")]
        [SerializeField] private float drawScale = 100f;

        /// <summary>
        /// Observe if the current room changes,
        /// if it does, reset and update the map
        /// </summary>
        private void Update()
        {
            if (localCurrentRoom != FloorGenerator.currentRoom)
            {
                ResetMap();
                UpdateMap();
            }
        }

        /// <summary>
        /// Completely remove all room visuals from the map:
        /// wipe the map clean
        /// </summary>
        private void ResetMap()
        {
            foreach(Transform child in roomImageContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// instantiate room visual prefab (images)
        /// depending on where a room is relative to the current
        /// room the player is in
        /// </summary>
        public void UpdateMap()
        {
            // Reassign the current room
            localCurrentRoom = FloorGenerator.currentRoom;
            
            // Update the current room by drawing all it's cells
            foreach (MapCell cell in localCurrentRoom.GetRoomCells(FloorGenerator.map.map))
            {
                Vector2 drawLocation = cell.location - localCurrentRoom.roomLocation;
                GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);
                cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * drawScale), cellVisual.transform.localPosition.y + (drawLocation.y * drawScale));
                cellVisual.GetComponentInChildren<TextMeshProUGUI>().text = localCurrentRoom.roomType.displayName;
            }

            // Loop through neighboring cells
            foreach (MapCell cell in localCurrentRoom.GetNeighboringCells(FloorGenerator.map.map))
            {

                // Apply a padding between rooms based on direction
                Vector2 paddingVec = new Vector2();
                if (cell.location.x < localCurrentRoom.roomLocation.x)
                {
                    paddingVec.x = -roomPadding;
                }
                else if(cell.location.x == localCurrentRoom.roomLocation.x)
                {
                    paddingVec.x = 0f;
                }
                else
                {
                    paddingVec.x = roomPadding;
                }

                if (cell.location.y < localCurrentRoom.roomLocation.y)
                {
                    paddingVec.y = -roomPadding;
                }
                else if (cell.location.y == localCurrentRoom.roomLocation.y)
                {
                    paddingVec.y = 0f;
                }
                else
                {
                    paddingVec.y = roomPadding;
                }

                // Check if neighboring cell's room is visited
                if (cell.room.generated)
                {
                    // Loop through that room's inner cells
                    foreach(MapCell innerCell in cell.room.GetRoomCells(FloorGenerator.map.map))
                    {
                        // Instantiate the room visual
                        GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);

                        // Decide location of where to draw cells
                        Vector2 drawLocation = (innerCell.location - localCurrentRoom.roomLocation);
                        cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * drawScale) + paddingVec.x, cellVisual.transform.localPosition.y + (drawLocation.y * drawScale) + paddingVec.y);
                        
                        // Modify the room visual
                        cellVisual.GetComponentInChildren<TextMeshProUGUI>().text = innerCell.room.roomType.displayName;
                        cellVisual.GetComponent<Image>().color = Color.gray;
                    }
                }
                // If the cell's room has not been visited
                else
                {
                    // Instantiate the room visual
                    GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);

                    Vector2 drawLocation = cell.location - localCurrentRoom.roomLocation;
                    cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * drawScale) + paddingVec.x, cellVisual.transform.localPosition.y + (drawLocation.y * drawScale) + paddingVec.y);
                    
                    cellVisual.GetComponent<Image>().sprite = nonVisitedRoomSprite;
                }
            }

        }
    }
}

