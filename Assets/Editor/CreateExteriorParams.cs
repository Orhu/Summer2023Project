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

        private List<Sprite> LoadSprites(string name)
        {
            List<Sprite> loadedSprites = new List<Sprite>();
            Sprite nonNumberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(prefix + name, typeof(Sprite));
            if (nonNumberedSprite != null)
            {
                loadedSprites.Add(nonNumberedSprite);
            }

            int number = 1;
            while (true)
            {
                Sprite numberedSprite = (Sprite) AssetDatabase.LoadAssetAtPath(prefix + name + number, typeof(Sprite));
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
        /// Adds the labels.
        /// </summary>
        void OnWizardCreate()
        {
            RoomExteriorParams newExteriorParams = CreateInstance<RoomExteriorParams>();

            ExteriorParamNames names = (ExteriorParamNames) AssetDatabase.LoadAssetAtPath("ExteriorParamNames", typeof(ExteriorParamNames));

            List<Sprite> rightWallSprites = LoadSprites(names.rightWallName);
            newExteriorParams.rightWallSprites = new GenericWeightedThings<Sprite>();
            foreach (Sprite sprite in rightWallSprites)
            {
                newExteriorParams.rightWallSprites.Add(sprite, 1, -1, true);
            }

            List<Sprite> topWallSprites = LoadSprites(names.rightWallName);
            newExteriorParams.topWallSprites = new GenericWeightedThings<Sprite>();
            foreach (Sprite sprite in topWallSprites)
            {
                newExteriorParams.topWallSprites.Add(sprite, 1, -1, true);
            }

            List<Sprite> leftWallSprites = LoadSprites(names.rightWallName);
            newExteriorParams.leftWallSprites = new GenericWeightedThings<Sprite>();
            foreach (Sprite sprite in leftWallSprites)
            {
                newExteriorParams.leftWallSprites.Add(sprite, 1, -1, true);
            }

            List<Sprite> bottomWallSprites = LoadSprites(names.rightWallName);
            newExteriorParams.bottomWallSprites = new GenericWeightedThings<Sprite>();
            foreach (Sprite sprite in bottomWallSprites)
            {
                newExteriorParams.bottomWallSprites.Add(sprite, 1, -1, true);
            }
        }
    }
}