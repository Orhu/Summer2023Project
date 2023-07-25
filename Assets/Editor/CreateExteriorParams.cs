using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Cardificer
{
    /// <summary>
    /// Utility for creating exterior params
    /// </summary>
    public class CreateExteriorParams : ScriptableWizard
    {
        // The prefix for the exterior params
        [SerializeField] private string prefix = "";

        /// <summary>
        /// Gives a find and replace prompt.
        /// </summary>
        [MenuItem("Tools/Create Exterior Params")]
        static void Create()
        {
            DisplayWizard<CreateExteriorParams>("CreateExteriorParams", "Confirm");
        }

        /// <summary>
        /// Loads sprites using the given name
        /// </summary>
        /// <param name="name"> The name to use </param>
        /// <returns> The sprites that were loaded </returns>
        private List<Sprite> LoadSprites(string name)
        {
            List<Sprite> loadedSprites = new List<Sprite>();
            Sprite nonNumberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(prefix + "_" + name, typeof(Sprite));
            if (nonNumberedSprite != null)
            {
                loadedSprites.Add(nonNumberedSprite);
            }

            int number = 1;
            while (true)
            {
                Sprite numberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(prefix + "_" + name + number, typeof(Sprite));
                if (numberedSprite == null)
                {
                    break;
                }
                else
                {
                    loadedSprites.Add(numberedSprite);
                }
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
            RoomExteriorParams newExteriorParams = CreateInstance<RoomExteriorParams>();

            ExteriorParamNames names = (ExteriorParamNames) AssetDatabase.LoadAssetAtPath("ExteriorParamNames", typeof(ExteriorParamNames));

            // Walls
            List<Sprite> rightWallSprites = LoadSprites(names.rightWallName);
            if (rightWallSprites == null)
            {
                Debug.LogWarning("Couldn't find any right wall sprites, looking for name: " + names.rightWallName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.rightWallSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in rightWallSprites)
                {
                    newExteriorParams.rightWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topWallSprites = LoadSprites(names.rightWallName);
            if (topWallSprites == null)
            {
                Debug.LogWarning("Couldn't find any top wall sprites, looking for name: " + names.topWallName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topWallSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in topWallSprites)
                {
                    newExteriorParams.topWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftWallSprites = LoadSprites(names.rightWallName);
            if (leftWallSprites == null)
            {
                Debug.LogWarning("Couldn't find any left wall sprites, looking for name: " + names.leftWallName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.leftWallSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in leftWallSprites)
                {
                    newExteriorParams.leftWallSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomWallSprites = LoadSprites(names.rightWallName);
            if (bottomWallSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom wall sprites, looking for name: " + names.bottomWallName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomWallSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in bottomWallSprites)
                {
                    newExteriorParams.bottomWallSprites.Add(sprite, 1, -1, true);
                }
            }

            // Corners
            List<Sprite> topRightCornerSprites = LoadSprites(names.topRightCornerName);
            if (topRightCornerSprites == null)
            {
                Debug.LogWarning("Couldn't find any top right corner sprites, looking for name: " + names.topRightCornerName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topRightCornerSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in topRightCornerSprites)
                {
                    newExteriorParams.topRightCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topLeftCornerSprites = LoadSprites(names.topLeftCornerName);
            if (topLeftCornerSprites == null)
            {
                Debug.LogWarning("Couldn't find any top left corner sprites, looking for name: " + names.topLeftCornerName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topLeftCornerSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in topLeftCornerSprites)
                {
                    newExteriorParams.topLeftCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomLeftCornerSprites = LoadSprites(names.bottomLeftCornerName);
            if (bottomLeftCornerSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom left corner sprites, looking for name: " + names.bottomLeftCornerName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomLeftCornerSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in bottomLeftCornerSprites)
                {
                    newExteriorParams.bottomLeftCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomRightCornerSprites = LoadSprites(names.bottomRightCornerName);
            if (bottomRightCornerSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom right corner sprites, looking for name: " + names.bottomRightCornerName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomRightCornerSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in bottomRightCornerSprites)
                {
                    newExteriorParams.bottomRightCornerSprites.Add(sprite, 1, -1, true);
                }
            }

            // Doors
            List<DoorSprites> rightDoorSprites = LoadDoorSprites(names.rightDoorName);
            if (rightDoorSprites == null)
            {
                Debug.LogWarning("Couldn't find any right door sprites, looking for name: " + names.rightDoorName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.rightDoorSprites = new GenericWeightedThings<DoorSprites>();
                foreach (DoorSprites sprite in rightDoorSprites)
                {
                    newExteriorParams.rightDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> topDoorSprites = LoadDoorSprites(names.topDoorName);
            if (topDoorSprites == null)
            {
                Debug.LogWarning("Couldn't find any top door sprites, looking for name: " + names.topDoorName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topDoorSprites = new GenericWeightedThings<DoorSprites>();
                foreach (DoorSprites sprite in topDoorSprites)
                {
                    newExteriorParams.topDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> leftDoorSprites = LoadDoorSprites(names.leftDoorName);
            if (leftDoorSprites == null)
            {
                Debug.LogWarning("Couldn't find any left door sprites, looking for name: " + names.leftDoorName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.leftDoorSprites = new GenericWeightedThings<DoorSprites>();
                foreach (DoorSprites sprite in leftDoorSprites)
                {
                    newExteriorParams.leftDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            List<DoorSprites> bottomDoorSprites = LoadDoorSprites(names.bottomDoorName);
            if (bottomDoorSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom door sprites, looking for name: " + names.bottomDoorName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomDoorSprites = new GenericWeightedThings<DoorSprites>();
                foreach (DoorSprites sprite in bottomDoorSprites)
                {
                    newExteriorParams.bottomDoorSprites.Add(sprite, 1, -1, true);
                }
            }

            // Door frames
            List<Sprite> rightDoorTopDoorFrameSprites = LoadSprites(names.rightDoorTopDoorFrameName);
            if (rightDoorTopDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any right door top door frame sprites, looking for name: " + names.rightDoorTopDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.rightDoorTopDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in rightDoorTopDoorFrameSprites)
                {
                    newExteriorParams.rightDoorTopDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> rightDoorBottomDoorFrameSprites = LoadSprites(names.rightDoorBottomDoorFrameName);
            if (rightDoorBottomDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any right door bottom door frame sprites, looking for name: " + names.rightDoorBottomDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.rightDoorBottomDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in rightDoorBottomDoorFrameSprites)
                {
                    newExteriorParams.rightDoorBottomDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topDoorRightDoorFrameSprites = LoadSprites(names.topDoorRightDoorFrameName);
            if (topDoorRightDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any top door right door frame sprites, looking for name: " + names.topDoorRightDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topDoorRightDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in topDoorRightDoorFrameSprites)
                {
                    newExteriorParams.topDoorRightDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> topDoorLeftDoorFrameSprites = LoadSprites(names.topDoorLeftDoorFrameName);
            if (topDoorLeftDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any top door left door frame sprites, looking for name: " + names.topDoorLeftDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.topDoorLeftDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in topDoorLeftDoorFrameSprites)
                {
                    newExteriorParams.topDoorLeftDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftDoorTopDoorFrameSprites = LoadSprites(names.leftDoorTopDoorFrameName);
            if (leftDoorTopDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any left door top door frame sprites, looking for name: " + names.leftDoorTopDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.leftDoorTopDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in leftDoorTopDoorFrameSprites)
                {
                    newExteriorParams.leftDoorTopDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> leftDoorBottomDoorFrameSprites = LoadSprites(names.leftDoorBottomDoorFrameName);
            if (leftDoorBottomDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any left door bottom door frame sprites, looking for name: " + names.leftDoorBottomDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.leftDoorBottomDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in leftDoorBottomDoorFrameSprites)
                {
                    newExteriorParams.leftDoorBottomDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomDoorRightDoorFrameSprites = LoadSprites(names.bottomDoorRightDoorFrameName);
            if (bottomDoorRightDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom door right door frame sprites, looking for name: " + names.bottomDoorRightDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomDoorRightDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in bottomDoorRightDoorFrameSprites)
                {
                    newExteriorParams.bottomDoorRightDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            List<Sprite> bottomDoorLeftDoorFrameSprites = LoadSprites(names.bottomDoorLeftDoorFrameName);
            if (bottomDoorLeftDoorFrameSprites == null)
            {
                Debug.LogWarning("Couldn't find any bottom door left door frame sprites, looking for name: " + names.bottomDoorLeftDoorFrameName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.bottomDoorLeftDoorFrameSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in bottomDoorLeftDoorFrameSprites)
                {
                    newExteriorParams.bottomDoorLeftDoorFrameSprites.Add(sprite, 1, -1, true);
                }
            }

            // Floors
            List<Sprite> floorSprites = LoadSprites(names.floorName);
            if (floorSprites == null)
            {
                Debug.LogWarning("Couldn't find any floor sprites, looking for name: " + names.floorName + " with prefix: " + prefix);
            }
            else
            {
                newExteriorParams.floorSprites = new GenericWeightedThings<Sprite>();
                foreach (Sprite sprite in floorSprites)
                {
                    newExteriorParams.floorSprites.Add(sprite, 1, -1, true);
                }
            }

            // Create the exterior params asset
            AssetDatabase.CreateAsset(newExteriorParams, prefix + "ExteriorParams");
        }
    }
}