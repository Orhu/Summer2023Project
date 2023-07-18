using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [System.Serializable] [CreateAssetMenu(fileName = "NewLayoutParams", menuName = "Generation/FloorParams")]
    public class FloorParams : ScriptableObject
    {
        [Tooltip("The default room exterior parameters")]
        [SerializeField] private RoomExteriorParams defaultExteriorParams;

        [Tooltip("The tile to spawn for the walls")]
        [SerializeField] private Tile WallTile;

        [Tooltip("The tile to spawn for the doors")]
        [SerializeField] private Tile doorTile;

        [Tooltip("The tile to spawn for the floors")]
        [SerializeField] private GameObject floorTile;

        [Tooltip("The layout params that apply to the entire map")]
        [SerializeField] private MapLayoutParams mapLayoutParams;

        [Tooltip("The room types and their individual params")]
        [SerializeField] private List<RoomTypeParams> roomTypesToParams;


        // The (parsed) layout parameters
        public LayoutParams layoutParams { private set; get; }

        // The (parsed) exterior parameters
        public RoomTypesToRoomExteriorParams exteriorParams { private set; get; }

        // The (parsed) template params
        public TemplateParams templateParams { private set; get; }

        /// <summary>
        /// Stores a room type and its associated parameters
        /// </summary>
        [System.Serializable]
        private class RoomTypeParams
        {
            [Tooltip("The room type these parameters apply to")]
            public RoomType roomType;

            [Tooltip("The exterior parameters of this room type (leave blank for default)")]
            public RoomExteriorParams exteriorParams;

            [Tooltip("The layout parameters of this room type")]
            public RoomLayoutParams layoutParams;

            [Tooltip("The templates that can appear in this room type")]
            public DifficultiesToTemplates templateParams;

        }

        public void ParseParams()
        {
            ParseLayoutParams();
            ParseExteriorParams();
            ParseTemplateParams();
        }

        /// <summary>
        /// Parses the floor generator params to get only the layout parameters
        /// </summary>
        public void ParseLayoutParams()
        {
            layoutParams = new LayoutParams();
            layoutParams.mapLayoutParams = mapLayoutParams;
            layoutParams.roomTypesToLayoutParams = new RoomTypesToLayoutParams();
            foreach (RoomTypeParams roomTypeParams in roomTypesToParams)
            {
                layoutParams.roomTypesToLayoutParams.Add(roomTypeParams.roomType, roomTypeParams.layoutParams);
            }
        }

        /// <summary>
        /// Parses the floor generator params to get only the exterior parameters
        /// </summary>
        /// <returns> The exterior parameters </returns>
        public void ParseExteriorParams()
        {
            exteriorParams = new RoomTypesToRoomExteriorParams();
            exteriorParams.wallTile = WallTile;
            exteriorParams.doorTile = doorTile;
            exteriorParams.floorTile = floorTile;
            exteriorParams.defaultRoomExteriorParams = defaultExteriorParams;
            exteriorParams.roomTypesToRoomExteriorParams = new List<RoomTypeToRoomExteriorParams>();
            foreach (RoomTypeParams roomTypeParams in roomTypesToParams)
            {
                exteriorParams.Add(roomTypeParams.roomType, roomTypeParams.exteriorParams);
            }
        }

        /// <summary>
        /// Parses the floor generator params to get only the template param
        /// </summary>
        /// <returns></returns>
        public void ParseTemplateParams()
        {
            templateParams = new TemplateParams();
            templateParams.templatesPool = new RoomTypesToDifficultiesToTemplates();
            foreach (RoomTypeParams roomTypeParams in roomTypesToParams)
            {
                templateParams.templatesPool.Add(roomTypeParams.roomType, roomTypeParams.templateParams);
            }
        }
    }
}