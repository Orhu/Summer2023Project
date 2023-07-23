using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

namespace Cardificer
{
    /// <summary>
    /// Map manager script for handling rendering and logic
    /// for the map menu UI object
    /// </summary>
    public class MapMenu : MonoBehaviour
    {
        [Tooltip("Grid layout for organizing room visuals")]
        [SerializeField] private GameObject roomImageContainer;

        [Tooltip("The text representing what floor you're on.")]
        [SerializeField] private TextMeshProUGUI floorText;

        [Tooltip("Room visual prefab, change it's sprite to change the background color of the room visual.")]
        [SerializeField] private GameObject cellVisualPrefab;

        [Tooltip("Sprite representing a blank room space")]
        [SerializeField] private Sprite blankRoomSprite;

        [Tooltip("Sprite representing an non visited room")]
        [SerializeField] private Sprite nonVisitedRoomSprite;

        // A local reference to the current room, observes current room changes
        private Room localCurrentRoom;

        [Tooltip("Float representing the padding between rooms")]
        [SerializeField] private float roomPadding = 100f;

        [Tooltip("How much we scale drawing each room from each other")]
        [SerializeField] private float drawScale = 500;

        // current Scale of the map
        private float currentScale = 1;

        // minimum scale of the map
        private float minScale = 0.1f;

        // max scale of the map
        private float maxScale = 2f;

        [Tooltip("The speed of zooming in and out the map")]
        [SerializeField] private float scaleSpeed = 0.5f;

        /// <summary>
        /// At the start of each floor, assign the floor name
        /// </summary>
        private void Start()
        {
            floorText.text = "Floor " + 1;
        }

        /// <summary>
        /// Zooms the map in and out based on input
        /// </summary>
        /// <param name="input">Input value used for zoom</param>
        public void OnZoom(InputValue input)
        {

            currentScale += input.Get<float>() * Time.unscaledDeltaTime * scaleSpeed;
            if (currentScale >= maxScale)
            {
                currentScale = maxScale;
            }
            else if (currentScale <= minScale)
            {
                currentScale = minScale;
            }
            roomImageContainer.transform.localScale = new Vector2(currentScale, currentScale);
        }

        /// <summary>
        /// Reset the map on button click
        /// </summary>
        /// <param name="input">Button used to reset the map</param>
        public void OnReset(InputValue input)
        {
            currentScale = 1;
            roomImageContainer.transform.localScale = new Vector2(currentScale, currentScale);
            roomImageContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Reset the map when the map is reenabled
        /// </summary>
        private void OnEnable()
        {
            GetComponentInChildren<ScrollRect>().normalizedPosition = Vector3.zero;
            if (localCurrentRoom != FloorGenerator.currentRoom)
            {
                ResetMap();
                UpdateMap();
            }

        }

        /// <summary>
        /// Button to swap to the deck viewer
        /// </summary>
        public void DeckViewerButtonEvent()
        {
            MenuManager.ToggleCardMenu();
        }

        /// <summary>
        /// Completely remove all room visuals from the map:
        /// wipe the map clean
        /// </summary>
        private void ResetMap()
        {
            foreach (Transform child in roomImageContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// instantiate room visual prefab (images)
        /// depending on where a room is relative to the current
        /// room the player is in
        /// </summary>
        private void UpdateMap()
        {
            // Reassign the current room
            localCurrentRoom = FloorGenerator.currentRoom;


            for(int i = 0; i < FloorGenerator.map.map.GetLength(0); i++)
            {
                for(int j = 0; j < FloorGenerator.map.map.GetLength(1); j++)
                {
                    MapCell cell = FloorGenerator.map.map[i, j];
                    if (!cell.room)
                        continue;

                    // Apply a padding between rooms based on direction
                    Vector2 paddingVec = new Vector2();
                    
                    if (cell.location.x < localCurrentRoom.roomLocation.x)
                    {
                        paddingVec.x = -roomPadding;
                    }
                    else if (cell.location.x == localCurrentRoom.roomLocation.x)
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

                   /* Vector2 paddingScale;
                    if (cell.room.generated)
                    {
                        paddingScale = new Vector2(cell.room.roomLocation.x - localCurrentRoom.roomLocation.x, cell.room.roomLocation.y - localCurrentRoom.roomLocation.y);
                    }
                    else
                    {
                        paddingScale = new Vector2(cell.location.x - localCurrentRoom.roomLocation.x, cell.location.y - localCurrentRoom.roomLocation.y);
                    }

                    paddingVec = new Vector2(paddingVec.x * Mathf.Abs(paddingScale.x), paddingVec.y * Mathf.Abs(paddingScale.y));*/
                    

                    // Check if any given cell is visited
                    if (cell.room.generated)
                    {

                        // Instantiate the room visual
                        GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);
                        

                        // Decide location of where to draw cells
                        Vector2 drawLocation = (cell.location - localCurrentRoom.roomLocation);
                        cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * drawScale) + paddingVec.x, cellVisual.transform.localPosition.y + (drawLocation.y * drawScale) + paddingVec.y);

                        // Modify the room visual
                        cellVisual.GetComponentInChildren<TextMeshProUGUI>().text = cell.room.roomType.displayName;
                        cellVisual.GetComponent<Image>().color = Color.gray;
                    }
                    // If the cell's room has not been visited
                    else
                    {
                        // Check to see if the cell has been seen before. If it has draw it
                        if (cell.seenByMap)
                        {
                            // Instantiate the room visual
                            GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);

                            Vector2 drawLocation = cell.location - localCurrentRoom.roomLocation;
                            cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * drawScale) + paddingVec.x, cellVisual.transform.localPosition.y + (drawLocation.y * drawScale) + paddingVec.y);

                            cellVisual.GetComponent<Image>().sprite = nonVisitedRoomSprite;
                        }
                        // Lastly, if the cell hasn't been visited or seen,
                        // see if it is a neighbor of the current cell. If it is,
                        // draw it
                        else 
                        {
                            List<MapCell> listOfNeighboringCells = localCurrentRoom.GetNeighboringCells(FloorGenerator.map.map);
                            if (listOfNeighboringCells.Contains(cell))
                            {
                                cell.seenByMap = true;
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
        }
    }
}