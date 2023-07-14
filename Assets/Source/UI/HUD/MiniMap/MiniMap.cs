using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cardificer{
    public class MiniMap : MonoBehaviour
    {
        [Tooltip("Grid layout for organizing room visuals")]
        [SerializeField] private GameObject roomImageContainer;

        [Tooltip("Room visual prefab, change it's sprite to change the background color of the room visual.")]
        [SerializeField] private GameObject roomVisualPrefab;

        [Tooltip("Sprite representing a blank room space")]
        [SerializeField] private Sprite blankRoomSprite;

        [Tooltip("Sprite representing an non visited room")]
        [SerializeField] private Sprite nonVisitedRoomSprite;

        [Tooltip("Sprite representing a visited room")]
        [SerializeField] private Sprite visitedRoomSprite;

        // A local reference to the current room, observes current room changes
        private Room localCurrentRoom;

        // For debugging: different rooms have different colors
        Color[] listOfColors;

        /// <summary>
        /// Assign variables
        /// </summary>
        private void Start()
        {
            listOfColors = new Color[] { Color.red, Color.green, Color.blue };
        }

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
            
            // Get a list of neighbor rooms
            List<Room> neighborRooms = FloorGenerator.currentRoom.GetNeighboringRooms(FloorGenerator.map.map);

            // Update the current room by drawing all it's cells
            foreach (MapCell cell in localCurrentRoom.GetRoomCells(FloorGenerator.map.map))
            {
                Vector2 drawLocation = cell.location - localCurrentRoom.roomLocation;
                var roomVisual = Instantiate(roomVisualPrefab, roomImageContainer.transform);
                roomVisual.transform.localPosition = new Vector2(roomVisual.transform.localPosition.x + (drawLocation.x * 100), roomVisual.transform.localPosition.y + (drawLocation.y * 100));
                roomVisual.GetComponentInChildren<TextMeshProUGUI>().text = localCurrentRoom.roomType.displayName;
            }

            // Draw all cells of neighboring rooms
            foreach (Room room in neighborRooms)
            {
                int index = Random.Range(0, listOfColors.Length);
                Color randColor = listOfColors[index];
                foreach(MapCell cell in room.GetRoomCells(FloorGenerator.map.map))
                {
                    Vector2 drawLocation = cell.location - localCurrentRoom.roomLocation;
                    var roomVisual = Instantiate(roomVisualPrefab, roomImageContainer.transform);
                    roomVisual.transform.localPosition = new Vector2(roomVisual.transform.localPosition.x + (drawLocation.x * 100), roomVisual.transform.localPosition.y + (drawLocation.y * 100));
                    roomVisual.GetComponent<Image>().color = randColor;
                    if (!room.generated)
                    {
                        roomVisual.GetComponentInChildren<TextMeshProUGUI>().text = room.roomType.displayName;
                    }
                    else
                    {
                        roomVisual.GetComponent<Image>().sprite = nonVisitedRoomSprite;
                    }
                }
            }

        }
    }
}

