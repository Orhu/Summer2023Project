using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Cardificer
{
    /// <summary>
    /// Utility for creating exterior params
    /// </summary>
    public class FillExteriorParams : ScriptableWizard
    {
        // The prefix for the exterior params
        [SerializeField] private string prefix = "";

        // The current asset path
        private string path;

        /// <summary>
        /// Gives a find and replace prompt.
        /// </summary>
        [MenuItem("Assets/Fill Exterior Params", priority = -2)]
        static void Create()
        {
            DisplayWizard<FillExteriorParams>("FillExteriorParams", "Confirm");
        }

        /// <summary>
        /// Checks to make sure a deep copy is possible right now.
        /// </summary>
        /// <returns> True if possible. </returns>
        [MenuItem("Assets/Fill Exterior Params", true)]
        private static bool isValidSelection()
        {
            return Selection.objects.Length == 1 && Selection.objects[0] is RoomExteriorParams;
        }

        /// <summary>
        /// Loads sprites using the given name
        /// </summary>
        /// <param name="name"> The name to use </param>
        /// <returns> The sprites that were loaded </returns>
        private List<Sprite> LoadSprites(string name)
        {
            List<Sprite> loadedSprites = new List<Sprite>();
            Sprite nonNumberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(path + prefix + "_" + name + ".png", typeof(Sprite));
            if (nonNumberedSprite != null)
            {
                loadedSprites.Add(nonNumberedSprite);
            }

            int number = 1;
            while (true)
            {
                Sprite numberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(path + prefix + "_" + name + number + ".png", typeof(Sprite));
                if (numberedSprite == null)
                {
                    break;
                }
                else
                {
                    loadedSprites.Add(numberedSprite);
                }
                number++;
            }

            return loadedSprites;
        }

        /// <summary>
        /// Loads door sprites using the given name
        /// </summary>
        /// <param name="name"> The name to use </param>
        /// <returns> The door sprites </returns>
        private List<DoorSprites> LoadDoorSprites(string name)
        {
            List<DoorSprites> doorSprites = new List<DoorSprites>();

            List<Sprite> openedSprites = LoadSprites(name + "Opened");
            List<Sprite> closedSprites = LoadSprites(name + "Closed");

            if (openedSprites.Count != closedSprites.Count)
            {
                Debug.LogWarning("There isn't the same number of opened and closed sprites for door " + name);
                return null;
            }

            for (int i = 0; i < openedSprites.Count; i++)
            {
                DoorSprites newDoorSprites = new DoorSprites();
                newDoorSprites.doorOpened = openedSprites[i];
                newDoorSprites.doorClosed = closedSprites[i];
                doorSprites.Add(newDoorSprites);
            }

            return doorSprites;
        }

        /// <summary>
        /// Creates the exterior params 
        /// </summary>
        void OnWizardCreate()
        {
            RoomExteriorParams exteriorParams = (RoomExteriorParams) Selection.objects[0];
            path = AssetDatabase.GetAssetPath(exteriorParams);

            // Remove the asset name
            string[] folders = path.Split("/");
            path = "";
            for (int i = 0; i < folders.Length - 1; i++)
            {
                path += folders[i];
                path += "/";
            }

            ExteriorParamNames names = (ExteriorParamNames) AssetDatabase.LoadAssetAtPath("Assets/Content/Tiles/RoomExteriorParams/ExteriorParamNames.asset", typeof(ExteriorParamNames));

            // Walls
            List<Sprite> rightWallSprites = LoadSprites(names.rightWallName);
            exteriorParams.rightWallSprites = new GenericWeightedThings<Sprite>();
            if (rightWallSprites == null || rightWallSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any right wall sprites, looking for name [" + names.rightWallName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in rightWallSprites)
                {
                    exteriorParams.rightWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topWallSprites = LoadSprites(names.topWallName);
            exteriorParams.topWallSprites = new GenericWeightedThings<Sprite>();
            if (topWallSprites == null || topWallSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top wall sprites, looking for name [" + names.topWallName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in topWallSprites)
                {
                    exteriorParams.topWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftWallSprites = LoadSprites(names.leftWallName);
            exteriorParams.leftWallSprites = new GenericWeightedThings<Sprite>();
            if (leftWallSprites == null || leftWallSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any left wall sprites, looking for name [" + names.leftWallName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in leftWallSprites)
                {
                    exteriorParams.leftWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomWallSprites = LoadSprites(names.bottomWallName);
            exteriorParams.bottomWallSprites = new GenericWeightedThings<Sprite>();
            if (bottomWallSprites == null || bottomWallSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any bottom wall sprites, looking for name [" + names.bottomWallName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in bottomWallSprites)
                {
                    exteriorParams.bottomWallSprites.Add(sprite, 1, -1, true);
                }
            }

            // Corners
            List<Sprite> topRightCornerSprites = LoadSprites(names.topRightCornerName);
            exteriorParams.topRightCornerSprites = new GenericWeightedThings<Sprite>();
            if (topRightCornerSprites == null || topRightCornerSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top right corner sprites, looking for name [" + names.topRightCornerName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in topRightCornerSprites)
                {
                    exteriorParams.topRightCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topLeftCornerSprites = LoadSprites(names.topLeftCornerName);
            exteriorParams.topLeftCornerSprites = new GenericWeightedThings<Sprite>();
            if (topLeftCornerSprites == null || topLeftCornerSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top left corner sprites, looking for name [" + names.topLeftCornerName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in topLeftCornerSprites)
                {
                    exteriorParams.topLeftCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomLeftCornerSprites = LoadSprites(names.bottomLeftCornerName);
            exteriorParams.bottomLeftCornerSprites = new GenericWeightedThings<Sprite>();
            if (bottomLeftCornerSprites == null || bottomLeftCornerSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any bottom left corner sprites, looking for name [" + names.bottomLeftCornerName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in bottomLeftCornerSprites)
                {
                    exteriorParams.bottomLeftCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomRightCornerSprites = LoadSprites(names.bottomRightCornerName);
            exteriorParams.bottomRightCornerSprites = new GenericWeightedThings<Sprite>();
            if (bottomRightCornerSprites == null || bottomRightCornerSprites.Count == 0 )
            {
                Debug.LogWarning("Couldn't find any bottom right corner sprites, looking for name [" + names.bottomRightCornerName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in bottomRightCornerSprites)
                {
                    exteriorParams.bottomRightCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            // Doors
            List<DoorSprites> rightDoorSprites = LoadDoorSprites(names.rightDoorName);
            exteriorParams.rightDoorSprites = new GenericWeightedThings<DoorSprites>();
            if (rightDoorSprites == null || rightDoorSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any right door sprites, looking for name [" + names.rightDoorName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (DoorSprites sprite in rightDoorSprites)
                {
                    exteriorParams.rightDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> topDoorSprites = LoadDoorSprites(names.topDoorName);
            exteriorParams.topDoorSprites = new GenericWeightedThings<DoorSprites>();
            if (topDoorSprites == null || topDoorSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top door sprites, looking for name [" + names.topDoorName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (DoorSprites sprite in topDoorSprites)
                {
                    exteriorParams.topDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> leftDoorSprites = LoadDoorSprites(names.leftDoorName);
            exteriorParams.leftDoorSprites = new GenericWeightedThings<DoorSprites>();
            if (leftDoorSprites == null)
            {
                Debug.LogWarning("Couldn't find any left door sprites, looking for name [" + names.leftDoorName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (DoorSprites sprite in leftDoorSprites)
                {
                    exteriorParams.leftDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> bottomDoorSprites = LoadDoorSprites(names.bottomDoorName);
            exteriorParams.bottomDoorSprites = new GenericWeightedThings<DoorSprites>();
            if (bottomDoorSprites == null || bottomDoorSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any bottom door sprites, looking for name [" + names.bottomDoorName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (DoorSprites sprite in bottomDoorSprites)
                {
                    exteriorParams.bottomDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            // Door frames
            List<Sprite> rightDoorTopDoorFrameSprites = LoadSprites(names.rightDoorTopDoorFrameName);
            exteriorParams.rightDoorTopDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (rightDoorTopDoorFrameSprites == null || rightDoorTopDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any right door top door frame sprites, looking for name [" + names.rightDoorTopDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in rightDoorTopDoorFrameSprites)
                {
                    exteriorParams.rightDoorTopDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> rightDoorBottomDoorFrameSprites = LoadSprites(names.rightDoorBottomDoorFrameName);
            exteriorParams.rightDoorBottomDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (rightDoorBottomDoorFrameSprites == null || rightDoorBottomDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any right door bottom door frame sprites, looking for name [" + names.rightDoorBottomDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in rightDoorBottomDoorFrameSprites)
                {
                    exteriorParams.rightDoorBottomDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topDoorRightDoorFrameSprites = LoadSprites(names.topDoorRightDoorFrameName);
            exteriorParams.topDoorRightDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (topDoorRightDoorFrameSprites == null || topDoorRightDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top door right door frame sprites, looking for name [" + names.topDoorRightDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in topDoorRightDoorFrameSprites)
                {
                    exteriorParams.topDoorRightDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topDoorLeftDoorFrameSprites = LoadSprites(names.topDoorLeftDoorFrameName);
            exteriorParams.topDoorLeftDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (topDoorLeftDoorFrameSprites == null || topDoorLeftDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any top door left door frame sprites, looking for name [" + names.topDoorLeftDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in topDoorLeftDoorFrameSprites)
                {
                    exteriorParams.topDoorLeftDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftDoorTopDoorFrameSprites = LoadSprites(names.leftDoorTopDoorFrameName);
            exteriorParams.leftDoorTopDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (leftDoorTopDoorFrameSprites == null || leftDoorTopDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any left door top door frame sprites, looking for name [" + names.leftDoorTopDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in leftDoorTopDoorFrameSprites)
                {
                    exteriorParams.leftDoorTopDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftDoorBottomDoorFrameSprites = LoadSprites(names.leftDoorBottomDoorFrameName);
            exteriorParams.leftDoorBottomDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (leftDoorBottomDoorFrameSprites == null || leftDoorBottomDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any left door bottom door frame sprites, looking for name [" + names.leftDoorBottomDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in leftDoorBottomDoorFrameSprites)
                {
                    exteriorParams.leftDoorBottomDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomDoorRightDoorFrameSprites = LoadSprites(names.bottomDoorRightDoorFrameName);
            exteriorParams.bottomDoorRightDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (bottomDoorRightDoorFrameSprites == null || bottomDoorRightDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any bottom door right door frame sprites, looking for name [" + names.bottomDoorRightDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in bottomDoorRightDoorFrameSprites)
                {
                    exteriorParams.bottomDoorRightDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomDoorLeftDoorFrameSprites = LoadSprites(names.bottomDoorLeftDoorFrameName);
            exteriorParams.bottomDoorLeftDoorFrameSprites = new GenericWeightedThings<Sprite>();
            if (bottomDoorLeftDoorFrameSprites == null || bottomDoorLeftDoorFrameSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any bottom door left door frame sprites, looking for name [" + names.bottomDoorLeftDoorFrameName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in bottomDoorLeftDoorFrameSprites)
                {
                    exteriorParams.bottomDoorLeftDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            // Floors
            List<Sprite> floorSprites = LoadSprites(names.floorName);
            exteriorParams.floorSprites = new GenericWeightedThings<Sprite>();
            if (floorSprites == null || floorSprites.Count == 0)
            {
                Debug.LogWarning("Couldn't find any floor sprites, looking for name [" + names.floorName + "] with prefix [" + prefix + "]");
            }
            else
            {
                foreach (Sprite sprite in floorSprites)
                {
                    exteriorParams.floorSprites.Add(sprite, 1, -1, true);
                }
            }
        }
    }
}