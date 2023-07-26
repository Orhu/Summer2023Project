using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using System.Linq;

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

        [Tooltip("Prefab visual for vertical doors")]
        [SerializeField] private GameObject doorVerticalPrefab;

        [Tooltip("Prefab visual for horizontal doors")]
        [SerializeField] private GameObject doorHorizontalPrefab;

        [Tooltip("Prefab for the room type visual")]
        [SerializeField] private GameObject roomTypeImagePrefab;

        // A local reference to the current room, observes current room changes
        private Room localCurrentRoom;

        [Tooltip("Float representing the padding between rooms")]
        [SerializeField] private float roomPadding = 100f;

        [Tooltip("How much we scale drawing each room from each other")]
        [SerializeField] private Vector2 cellSize = new Vector2(400f, 400f);

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
            currentScale = 1;
            roomImageContainer.transform.localScale = new Vector2(currentScale, currentScale);
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
            MenuManager.Toggle<CardMenu>();
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

            // Keep track of a list of seen cells when drawing all cells each update
            List<MapCell> seenCells = new List<MapCell>();

            // Loop through the floorgenerators map
            for (int i = 0; i < FloorGenerator.map.map.GetLength(0); i++)
            {
                for(int j = 0; j < FloorGenerator.map.map.GetLength(1); j++)
                {
                    // grabbing the cell at i, j
                    MapCell cell = FloorGenerator.map.map[i, j];

                    // If a cell does not have a room, or it has been seen and drawn before, or the room is not visited, skip it
                    if (!cell.room || seenCells.Contains(cell) || !cell.room.generated)
                        continue;
                    
                    // Used as a position reference for the roomtype sprite
                    GameObject parentCell = null;

                    // Draw all the cells of a given cells room
                    foreach (MapCell innerCell in cell.room.GetRoomCells(FloorGenerator.map.map))
                    {
                        // Add the current inner cell to the seen cells
                        seenCells.Add(innerCell);

                        // Instantiate the room visual
                        GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);

                        // Set the parent cell variable
                        if (parentCell == null)
                        {
                            parentCell = cellVisual;
                        }

                        Vector2 paddingVec = new Vector2();

                        // ----------- Determine padding and sizing of the cell visual based on certain factors

                        // stretch the visual
                        Vector2 intermediarySize = (cellSize - cellVisual.GetComponent<RectTransform>().rect.size) / 2;

                        if (innerCell.location.x != innerCell.room.roomLocation.x)
                        {
                            // There is a cell to the left of this cell that's part of the room 
                            cellVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(cellVisual.GetComponent<RectTransform>().sizeDelta.x + intermediarySize.x, cellVisual.GetComponent<RectTransform>().sizeDelta.y);
                            paddingVec.x -= intermediarySize.x / 2;
                        }
                        if (innerCell.location.x != innerCell.room.roomLocation.x + innerCell.room.roomType.sizeMultiplier.x - 1)
                        {
                            // There is a cell to the right of this cell that's part of the room 
                            cellVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(cellVisual.GetComponent<RectTransform>().sizeDelta.x + intermediarySize.x, cellVisual.GetComponent<RectTransform>().sizeDelta.y);
                            paddingVec.x += intermediarySize.x / 2;
                        }


                        if (innerCell.location.y != innerCell.room.roomLocation.y)
                        {
                            // There is a cell to the left of this cell that's part of the room 
                            cellVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(cellVisual.GetComponent<RectTransform>().sizeDelta.x, cellVisual.GetComponent<RectTransform>().sizeDelta.y + intermediarySize.y);
                            paddingVec.y -= intermediarySize.y / 2;
                        }
                        if (innerCell.location.y != innerCell.room.roomLocation.y + innerCell.room.roomType.sizeMultiplier.y - 1)
                        {
                            // There is a cell to the right of this cell that's part of the room 
                            cellVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(cellVisual.GetComponent<RectTransform>().sizeDelta.x, cellVisual.GetComponent<RectTransform>().sizeDelta.y + intermediarySize.y);
                            paddingVec.y += intermediarySize.y / 2;
                        }

                        // Place the cell in the correct spot
                        Vector2 drawLocation = (innerCell.location - localCurrentRoom.roomLocation);
                        cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * cellSize.x), cellVisual.transform.localPosition.y + (drawLocation.y * cellSize.y)) + paddingVec;


                        // ----------------- Place doors
                        if (innerCell.direction.HasFlag(Direction.Up))
                        {
                            // place a door up
                            GameObject doorVisual = Instantiate(doorHorizontalPrefab, cellVisual.transform);
                            doorVisual.transform.localPosition = new Vector2(doorVisual.transform.localPosition.x - paddingVec.x, doorVisual.transform.localPosition.y + (cellVisual.GetComponent<RectTransform>().sizeDelta.y / 2));
                        }

                        if (innerCell.direction.HasFlag(Direction.Down))
                        {
                            // place a door up
                            GameObject doorVisual = Instantiate(doorHorizontalPrefab, cellVisual.transform);
                            doorVisual.transform.localPosition = new Vector2(doorVisual.transform.localPosition.x - paddingVec.x, doorVisual.transform.localPosition.y - (cellVisual.GetComponent<RectTransform>().sizeDelta.y / 2));
                        }

                        if (innerCell.direction.HasFlag(Direction.Left))
                        {
                            // place a door up
                            GameObject doorVisual = Instantiate(doorVerticalPrefab, cellVisual.transform);
                            doorVisual.transform.localPosition = new Vector2(doorVisual.transform.localPosition.x - (cellVisual.GetComponent<RectTransform>().sizeDelta.x / 2), doorVisual.transform.localPosition.y - paddingVec.y);
                        }

                        if (innerCell.direction.HasFlag(Direction.Right))
                        {
                            // place a door up
                            GameObject doorVisual = Instantiate(doorVerticalPrefab, cellVisual.transform);
                            doorVisual.transform.localPosition = new Vector2(doorVisual.transform.localPosition.x + (cellVisual.GetComponent<RectTransform>().sizeDelta.x / 2), doorVisual.transform.localPosition.y - paddingVec.y);
                        }
                    }


                    // --------- Draw Neighboring (unvisited) cells ---------
                    List<MapCell> neighboringCellsOfCellRoom = cell.room.GetNeighboringCells(FloorGenerator.map.map).Except(seenCells).ToList();

                    foreach (MapCell neighborCell in neighboringCellsOfCellRoom)
                    {
                        if (!neighborCell.room.generated)
                        {
                            seenCells.Add(neighborCell);
                            // Instantiate the room visual
                            GameObject cellVisual = Instantiate(cellVisualPrefab, roomImageContainer.transform);

                            Vector2 drawLocation = neighborCell.location - localCurrentRoom.roomLocation;
                            cellVisual.transform.localPosition = new Vector2(cellVisual.transform.localPosition.x + (drawLocation.x * cellSize.x), cellVisual.transform.localPosition.y + (drawLocation.y * cellSize.y));
                            cellVisual.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.85f);

                            if (neighborCell.room.roomType.roomTypeSprite && neighborCell.room.roomType.displayName != "Exit")
                            {
                                GameObject roomTypeVisual = Instantiate(roomTypeImagePrefab, cellVisual.transform);
                                roomTypeVisual.GetComponent<Image>().sprite = neighborCell.room.roomType.roomTypeSprite;

                                int roomScale = Mathf.Min(neighborCell.room.roomType.sizeMultiplier.x, neighborCell.room.roomType.sizeMultiplier.y);

                                roomTypeVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(roomTypeVisual.GetComponent<RectTransform>().sizeDelta.x * roomScale, roomTypeVisual.GetComponent<RectTransform>().sizeDelta.y * roomScale);

                                roomTypeVisual.transform.localPosition = new Vector2(roomTypeVisual.transform.localPosition.x + ((cellVisualPrefab.GetComponent<RectTransform>().sizeDelta.x * neighborCell.room.roomType.sizeMultiplier.x) / 2), roomTypeVisual.transform.localPosition.y + ((cellVisualPrefab.GetComponent<RectTransform>().sizeDelta.y * neighborCell.room.roomType.sizeMultiplier.y) / 2));


                                roomTypeVisual.transform.SetParent(roomImageContainer.transform, true);
                            }
                        }
                    }

                    // ---------------------- Draw current cell's room sprite
                    if (cell.room.roomType.roomTypeSprite)
                    {
                        GameObject roomTypeVisual = Instantiate(roomTypeImagePrefab, parentCell.transform);
                        roomTypeVisual.GetComponent<Image>().sprite = cell.room.roomType.roomTypeSprite;

                        int roomScale = Mathf.Min(cell.room.roomType.sizeMultiplier.x, cell.room.roomType.sizeMultiplier.y);

                        roomTypeVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(roomTypeVisual.GetComponent<RectTransform>().sizeDelta.x * roomScale, roomTypeVisual.GetComponent<RectTransform>().sizeDelta.y * roomScale);

                        roomTypeVisual.transform.localPosition = new Vector2(roomTypeVisual.transform.localPosition.x + ((cellVisualPrefab.GetComponent<RectTransform>().sizeDelta.x * cell.room.roomType.sizeMultiplier.x) / 2), roomTypeVisual.transform.localPosition.y + ((cellVisualPrefab.GetComponent<RectTransform>().sizeDelta.y * cell.room.roomType.sizeMultiplier.y) / 2));


                        roomTypeVisual.transform.SetParent(roomImageContainer.transform, true);
                    }
                    
                    
                    
                }
            }
        }
    }
}