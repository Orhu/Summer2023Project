using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer{
    public class MiniMap : MonoBehaviour
    {
        [Tooltip("Grid layout for organizing room visuals")]
        [SerializeField] private GridLayoutGroup roomImageContainer;

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


        private void Awake()
        {
            if (roomImageContainer == null)
            {
                roomImageContainer = GetComponentInChildren<GridLayoutGroup>();
            }
        }

        private void Update()
        {
            if (localCurrentRoom != FloorGenerator.currentRoom)
            {
                ResetMap();
                UpdateMap();
            }
        }

        private void ResetMap()
        {
            foreach(Transform child in roomImageContainer.transform)
            {
                child.gameObject.GetComponent<Image>().sprite = blankRoomSprite;
            }
        }

        public void UpdateMap()
        {
            print("Updating map");
            localCurrentRoom = FloorGenerator.currentRoom;

            List<MapCell> roomEdges = FloorGenerator.currentRoom.GetEdgeCells(FloorGenerator.map.map);
            foreach(MapCell cell in roomEdges)
            {
                print(cell.room.generated);
            }

            roomImageContainer.gameObject.transform.GetChild(4).GetComponent<Image>().sprite = visitedRoomSprite;

            for (int i = 0; i < roomEdges.Count; i++)
            {
                if (roomEdges[i].direction.HasFlag(Direction.Up)) // Up
                {
                    if (roomEdges[i].room.generated)
                    {
                        roomImageContainer.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = visitedRoomSprite;
                    }
                    else
                    {
                        roomImageContainer.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = nonVisitedRoomSprite;
                    }
                }
                if (roomEdges[i].direction.HasFlag(Direction.Left)) // Left
                {
                    if (roomEdges[i].room.generated)
                    {
                        roomImageContainer.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = visitedRoomSprite;
                    }
                    else
                    {
                        roomImageContainer.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = nonVisitedRoomSprite;
                    }
                }
                if (roomEdges[i].direction.HasFlag(Direction.Right)) // Right
                {
                    if (roomEdges[i].room.generated)
                    {
                        roomImageContainer.gameObject.transform.GetChild(5).GetComponent<Image>().sprite = visitedRoomSprite;
                    }
                    else
                    {
                        roomImageContainer.gameObject.transform.GetChild(5).GetComponent<Image>().sprite = nonVisitedRoomSprite;
                    }
                }
                if (roomEdges[i].direction.HasFlag(Direction.Down)) // Down
                {
                    if (roomEdges[i].room.generated)
                    {
                        roomImageContainer.gameObject.transform.GetChild(7).GetComponent<Image>().sprite = visitedRoomSprite;
                    }
                    else
                    {
                        roomImageContainer.gameObject.transform.GetChild(7).GetComponent<Image>().sprite = nonVisitedRoomSprite;
                    }
                }
            }
        }
    }
}

