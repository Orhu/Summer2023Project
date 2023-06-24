using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The tiles that make up the exterior of the rooms
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewRoomExteriorGenerationParameters", menuName = "Generation/RoomExteriorGenerationParameters")]

    public class RoomExteriorGenerationParameters : ScriptableObject
    {
        [Tooltip("The wall tile")]
        public Tile wallTile;

        [Tooltip("The door tile")]
        public Tile doorTile;

        [Tooltip("The possible right wall sprites to use (will pick randomly from these)")]
        public GenericWeightedThings<Sprite> rightWallSprites;

        [Tooltip("The possible top wall sprites to use")]
        public GenericWeightedThings<Sprite> topWallSprites;

        [Tooltip("The possible left wall sprites to use")]
        public GenericWeightedThings<Sprite> leftWallSprites;

        [Tooltip("The possible bottom wall sprites to use")]
        public GenericWeightedThings<Sprite> bottomWallSprites;

        [Tooltip("The possible top right wall corner sprites to use")]
        public GenericWeightedThings<Sprite> topRightWallCornerSprites;

        [Tooltip("The possible top left wall corner sprites to use")]
        public GenericWeightedThings<Sprite> topLeftWallCornerSprites;

        [Tooltip("The possible bottom left wall corner sprites to use")]
        public GenericWeightedThings<Sprite> bottomLeftWallCornerSprites;

        [Tooltip("The possible bottom right wall corner sprites to use")]
        public GenericWeightedThings<Sprite> bottomRightWallCornerSprites;

        [Tooltip("The possible right door sprites to use")]
        public GenericWeightedThings<DoorSprites> rightDoorSprites;

        [Tooltip("The possible top door sprites to use")]
        public GenericWeightedThings<DoorSprites> topDoorSprites;

        [Tooltip("The possible left door sprites to use")]
        public GenericWeightedThings<DoorSprites> leftDoorSprites;

        [Tooltip("The possible bottom door sprites to use")]
        public GenericWeightedThings<DoorSprites> bottomDoorSprites;

        [Tooltip("The possible above right door sprites to use")]
        public GenericWeightedThings<Sprite> aboveRightDoorSprites;

        [Tooltip("The possible below right door sprites to use")]
        public GenericWeightedThings<Sprite> belowRightDoorSprites;

        [Tooltip("The possible beside top door right sprites to use")]
        public GenericWeightedThings<Sprite> besideTopDoorRightSprites;

        [Tooltip("The possible beside top door left sprites to use")]
        public GenericWeightedThings<Sprite> besideTopDoorLeftSprites;

        [Tooltip("The possible above left door sprites to use")]
        public GenericWeightedThings<Sprite> aboveLeftDoorSprites;

        [Tooltip("The possible below left door sprites to use")]
        public GenericWeightedThings<Sprite> belowLeftDoorSprites;

        [Tooltip("The possible beside bottom door right sprites to use")]
        public GenericWeightedThings<Sprite> besideBottomDoorRightSprites;

        [Tooltip("The possible beside bottom door left sprites to use")]
        public GenericWeightedThings<Sprite> besideBottomDoorLeftSprites;

        [Tooltip("The possible floor sprites to use")]
        public GenericWeightedThings<Sprite> floorSprites;

        /// <summary>
        /// Resets all the GenericWeightedThings lists
        /// </summary>
        public void Reset()
        {
            rightWallSprites.Reset();
            topWallSprites.Reset();
            leftWallSprites.Reset();
            bottomWallSprites.Reset();
            topRightWallCornerSprites.Reset();
            topLeftWallCornerSprites.Reset();
            bottomLeftWallCornerSprites.Reset();
            bottomRightWallCornerSprites.Reset();
            rightDoorSprites.Reset();
            topDoorSprites.Reset();
            leftDoorSprites.Reset();
            bottomDoorSprites.Reset();
            aboveRightDoorSprites.Reset();
            belowRightDoorSprites.Reset();
            besideTopDoorRightSprites.Reset();
            besideTopDoorLeftSprites.Reset();
            aboveLeftDoorSprites.Reset();
            belowLeftDoorSprites.Reset();
            besideBottomDoorRightSprites.Reset();
            besideBottomDoorLeftSprites.Reset();
            floorSprites.Reset();
        }
    }

    /// <summary>
    /// Holds two sprites for a door, an opened one and a closed one
    /// </summary>
    [System.Serializable]
    public class DoorSprites
    {
        [Tooltip("The door opened tile to use")]
        [SerializeField] public Sprite doorOpened;

        [Tooltip("The door closed tile")]
        [SerializeField] public Sprite doorClosed;
    }
}