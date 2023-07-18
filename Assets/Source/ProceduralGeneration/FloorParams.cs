using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public class RoomTypeParams
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

        /// <summary>
        /// Parses all the params and stores them in the variables
        /// </summary>
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
        public void ParseTemplateParams()
        {
            templateParams = new TemplateParams();
            templateParams.templatesPool = new RoomTypesToDifficultiesToTemplates();
            foreach (RoomTypeParams roomTypeParams in roomTypesToParams)
            {
                templateParams.templatesPool.Add(roomTypeParams.roomType, roomTypeParams.templateParams);
            }
        }

        /// <summary>
        /// Updates the use difficulty on the difficulties to templates
        /// </summary>
        private void OnValidate()
        {
            if (roomTypesToParams == null) { return; }

            foreach (RoomTypeParams roomTypeParams in roomTypesToParams)
            {
                if (roomTypeParams.roomType == null)
                {
                    roomTypeParams.templateParams.useDifficulty = false;
                    continue;
                }
                roomTypeParams.templateParams.useDifficulty = roomTypeParams.roomType.useDifficulty;
            }
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Class for making the floor params easier to read in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(FloorParams.RoomTypeParams), true)]
    public class FloorParamsDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the floor params property
        /// </summary>
        /// <param name="position"> The position the property begins at </param>
        /// <param name="property"> The property being drawn </param>
        /// <param name="label"> The label of this property </param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty roomType = property.FindPropertyRelative("roomType");
            SerializedProperty exteriorParams = property.FindPropertyRelative("exteriorParams");
            SerializedProperty layoutParams = property.FindPropertyRelative("layoutParams");
            SerializedProperty templateParams = property.FindPropertyRelative("templateParams");

            label.text = "Room Type: ";
            position.height = EditorGUI.GetPropertyHeight(roomType);

            roomType.isExpanded = EditorGUI.Foldout(position, roomType.isExpanded, label);
            Rect roomTypeRect = position;
            roomTypeRect.x += 100;
            roomTypeRect.width -= 100;
            EditorGUI.PropertyField(roomTypeRect, roomType, GUIContent.none, true);

            if (roomType.isExpanded)
            {
                position.y += EditorGUI.GetPropertyHeight(roomType) + 2;

                EditorGUI.indentLevel++;

                position.height = EditorGUI.GetPropertyHeight(exteriorParams);
                EditorGUI.PropertyField(position, exteriorParams, new GUIContent("Exterior Params"), true);
                position.y += EditorGUI.GetPropertyHeight(exteriorParams) + 2;

                position.height = EditorGUI.GetPropertyHeight(layoutParams);
                EditorGUI.PropertyField(position, layoutParams, new GUIContent("Layout Params"), true);
                position.y += EditorGUI.GetPropertyHeight(layoutParams) + 2;

                position.height = EditorGUI.GetPropertyHeight(templateParams);
                EditorGUI.PropertyField(position, templateParams, new GUIContent("Template Params"), true);
                position.y += EditorGUI.GetPropertyHeight(templateParams) + 2;

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Gets the height this property should have
        /// </summary>
        /// <param name="property"> The property </param>
        /// <param name="label"> The label of this property </param>
        /// <returns> The height of this property </returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty roomType = property.FindPropertyRelative("roomType");
            SerializedProperty exteriorParams = property.FindPropertyRelative("exteriorParams");
            SerializedProperty layoutParams = property.FindPropertyRelative("layoutParams");
            SerializedProperty templateParams = property.FindPropertyRelative("templateParams");

            float height = 0;
            height += EditorGUI.GetPropertyHeight(roomType) + 2;

            if (roomType.isExpanded)
            {
                height += EditorGUI.GetPropertyHeight(exteriorParams) + 2;
                height += EditorGUI.GetPropertyHeight(layoutParams) + 2;
                height += EditorGUI.GetPropertyHeight(templateParams) + 2;
            }

            return height;
        }
    }

#endif
}