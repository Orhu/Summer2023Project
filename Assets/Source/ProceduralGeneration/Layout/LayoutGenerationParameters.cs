using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The parameters that affect the layout generation
    /// </summary>
    [System.Serializable]
    public class LayoutGenerationParameters
    {
        [Tooltip("The (approximate) number of normal rooms to generate")]
        [SerializeField] public int numNormalRooms;

        [Tooltip("The variance of randomness for the number of normal rooms to generate")]
        [SerializeField] public int numNormalRoomsVariance;

        [Tooltip("The number of special rooms that will appear")]
        [SerializeField] public int numSpecialRooms;

        [Tooltip("The number of doors that is preferred")]
        [Range(1, 4)]
        [SerializeField] public int preferredNumDoors;

        [Tooltip("How strictly the generation adheres to the preferred number of doors (100 = very strict, 0 = not strict at all)")] [Range(0, 100)]
        [SerializeField] public float strictnessNumDoors;
    }

    /// <summary>
    /// Stores a list of room types and their associated layout parameters
    /// </summary>
    [System.Serializable]
    public class RoomTypesToLayoutParameters
    {
        /// <summary>
        /// A list of room types to layout parameters
        /// </summary>
        public List<RoomTypeToLayoutParameters> roomTypesToLayoutParameters;

        /// <summary>
        /// Gets the layout parameters associated with a given room type
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <returns> The layout parameters </returns>
        public RoomTypeToLayoutParameters At(RoomType roomType)
        {
            foreach (RoomTypeToLayoutParameters roomTypeToLayoutParameters in roomTypesToLayoutParameters)
            {
                if (roomTypeToLayoutParameters.roomType == roomType)
                {
                    return roomTypeToLayoutParameters;
                }
            }

            throw new System.Exception("No room type of " + roomType.displayName + " in room types to layout parameters");
        }
    }

    /// <summary>
    /// Stores a room type and its associated layout parameters
    /// </summary>
    [System.Serializable]
    public class RoomTypeToLayoutParameters
    {
        [Tooltip("The room type")]
        public RoomType roomType;
        
        [Tooltip("The number of this type of room to appear in the layout")]
        public int numRooms;

        [Tooltip("The variance allowed with the number of rooms")]
        public int numRooomsVariance;
    }
}