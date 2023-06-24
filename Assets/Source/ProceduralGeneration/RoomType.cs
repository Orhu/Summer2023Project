using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Scriptable object that holds data about a room type
    /// </summary>
    public class RoomType : ScriptableObject
    {
        [Tooltip("The name of this room type (for display/dev purposes)")]
        public string displayName { private set; get; }

        [Tooltip("Whether or not this room type will be generated as a start room")]
        public bool startRoom { private set; get; }

        [Tooltip("The size multiplier of this room: What size is this room compared to the size of a normal room?")]
        public Vector2Int sizeMultiplier { private set; get; }

        [Tooltip("Whether or not this room type can only spawn as a dead end")]
        public bool deadEnd { private set; get; }

        [Tooltip("The attached room (leave blank for no attached room). This can only be specified on dead ends, and the attached room must also be a dead end.")]
        public RoomType attachedRoom { private set; get; }

        [Tooltip("The location where the attached room must be attached")]
        public AttachmentLocation attachmentLocation { private set; get; }

        [Tooltip("Whether or not it should not use a random offset (as opposed to an explicit one)")]
        public bool useRandomOffset { private set; get; } 

        [Tooltip("The explicit offset to use if use random offset is false")]
        public Vector2Int offset { private set; get; }

        [Tooltip("Whether or not this room type uses the difficulty meter")]
        public bool useDifficulty { private set; get; }

        /// <summary>
        /// An enum that tracks attachment locations for attached rooms
        /// </summary>
        [System.Serializable]
        public enum AttachmentLocation
        {
            NA, // Not applicable: The attached room doesn't care where it's attached
            SoftOpposite, // The attached room must be on the opposite side to a door, but not necessarily directly opposite
            HardOpposite, // The attached room must be directly opposite to a door
            SoftAdjacent, // The attached room must be on the same side as a door, but not necessarily on one of the two walls closest to the door
            HardAdjacent, // The attached room must be on the same side as a door, on one of the two walls closest to the door
            Perpendicular, // The attached room must be on one of the two sides rotated ninety degrees from the side the door is on
        }
    }
}