using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Scriptable object that holds data about a room type
    /// </summary>
    public class RoomType : ScriptableObject
    {
        [Tooltip("The name of this room type (for display purposes)")]
        public string displayName { get; private set; }

        [Tooltip("Whether or not this room type can only spawn as a dead end")]
        public bool deadEnd { get; private set; }

        [Tooltip("Whether or not this room counts as a normal room")]
        public bool normalRoom { get; private set; }

        [Tooltip("The size multiplier of this room: What size is this room compared to the size of a normal room?")]
        public Vector2Int sizeMultiplier { get; private set; }

        [Tooltip("The explicit room size of this room type")]
        public Vector2Int explicitRoomSize { get; private set; }

        [Tooltip("The attached rooms and where they must be attached")]
        public List<AttachedRoomToAttachmentLocation> attachedRooms;
    }

    /// <summary>
    /// An enum that tracks attachment locations for attached rooms
    /// </summary>
    public enum AttachmentLocation
    {
        NA, // Not applicable: The attached room doesn't care where it's attached
        Opposite, // The attached room must be opposite to a door
        Adjacent, // The attached room must be on one of the two sides adjacent to a door
    }

    /// <summary>
    /// Holds a RoomType and the attachment location of that room
    /// </summary>
    public class AttachedRoomToAttachmentLocation
    {
        /// <summary>
        /// The attached room
        /// </summary>
        public RoomType attachedRoom;

        /// <summary>
        /// The attachment location of the attached room
        /// </summary>
        public AttachmentLocation attachmentLocation;
    }
}