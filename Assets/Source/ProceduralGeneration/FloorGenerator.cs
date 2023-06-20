using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cardificer
{
    /// <summary>
    /// Handles generation for an entire floor: The layout and the external rooms. 
    /// The rooms handle their own internal generation, but they will use the parameters on this generator (the templates).
    /// </summary>
    public class FloorGenerator : MonoBehaviour
    {
        [Tooltip("The generation parameters for the layout of this floor")]
        public LayoutGenerationParameters layoutGenerationParameters;

        [Tooltip("The template generation parameters for this floor")]
        public TemplateGenerationParameters templateGenerationParameters;

        [Tooltip("The size of a room on this floor")]
        public Vector2Int roomSize;

        [Tooltip("A dictionary that holds room types and their associated exterior generation parameters for this floor")]
        public RoomTypesToRoomExteriorGenerationParameters roomTypesToExteriorGenerationParameters;

        [Tooltip("The seed to use for generation")]
        public int seed = 0;

        [Tooltip("Whether or not to randomize the seed on start")]
        public bool randomizeSeed;

        // Event called when the room is changed
        [HideInInspector] public UnityEvent onRoomChange;

        // The random instance
        [HideInInspector] static public System.Random random;

        // A reference to the generated map
        [HideInInspector] public Map map;

        // The room the player is currently in
        private Room _currentRoom;
        [HideInInspector]
        public Room currentRoom
        {
            get { return _currentRoom; }
            set
            {
                _currentRoom = value;
                onRoomChange?.Invoke();
            }
        }

        // The instance
        public static FloorGenerator floorGeneratorInstance { get; private set; }

        /// <summary>
        /// Sets up the singleton
        /// </summary>
        void Awake()
        {
            if (floorGeneratorInstance != null && floorGeneratorInstance != this)
            {
                Destroy(gameObject);
                return;
            }
            floorGeneratorInstance = this;
        }


        /// <summary>
        /// Generates the layout and exteriors of the room
        /// </summary>
        private void Start()
        {
            if (SaveManager.autosaveExists)
            {
                seed = SaveManager.savedFloorSeed;
            }
            else if (randomizeSeed)
            {
                seed = Random.Range(0, System.Int32.MaxValue);
            }

            random = new System.Random(seed);

            GetSpecialRoomsFromDeck();
            GetTilesFromDeck();
            Deck.playerDeck.onCardAdded += OnCardAdded;
            Deck.playerDeck.onCardRemoved += OnCardRemoved;
            map = GetComponent<LayoutGenerator>().Generate(layoutGenerationParameters);
            GetComponent<RoomExteriorGenerator>().Generate(roomTypesToExteriorGenerationParameters, map, roomSize);

            // Autosave loading
            if (!SaveManager.autosaveExists) 
            {
                Room startRoom = map.startCell.room.GetComponent<Room>();
                startRoom.Enter(Direction.None);
                return; 
            }

            List<Vector3Int> vistedRooms = SaveManager.savedVisitedRooms;
            Room lastRoom = map.startCell.room.GetComponent<Room>();
            int nextCardIndex = vistedRooms[0].z;
            foreach (Vector3Int vistedRoom in vistedRooms)
            {
                lastRoom.Exit();
                lastRoom = map.map[vistedRoom.x, vistedRoom.y].room.GetComponent<Room>();
                lastRoom.Generate(false);
                while (nextCardIndex < vistedRoom.z)
                {
                    OnCardAdded(Deck.playerDeck.cards[nextCardIndex]);
                    nextCardIndex++;
                }
            }
            lastRoom.Enter();
        }

        /// <summary>
        /// Initializes the floor generator with the special rooms found in the deck
        /// </summary>
        private void GetSpecialRoomsFromDeck()
        {
            int max = SaveManager.autosaveExists ? SaveManager.savedVisitedRooms[0].z : Deck.playerDeck.cards.Count;
            for (int i = 0; i < max; i++)
            {
                Card card = Deck.playerDeck.cards[i];
                if (card.effects == null)
                {
                    return;
                }

                foreach (DungeonEffect effect in card.effects)
                {
                    if (effect == null)
                    {
                        continue;
                    }
                    if (effect.specialRooms == null)
                    {
                        return;
                    }
                    foreach (Template specialRoom in effect.specialRooms)
                    {
                        layoutGenerationParameters.numSpecialRooms++;
                        templateGenerationParameters.templatesPool.At(RoomType.Special).At(Difficulty.NotApplicable).Add(specialRoom);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the floor generator with tiles found in the deck
        /// </summary>
        private void GetTilesFromDeck()
        {
            foreach (TileType tileType in System.Enum.GetValues(typeof(TileType)))
            {
                TileTypeToPossibleTiles tileTypeToPossibleTiles;
                tileTypeToPossibleTiles.tileType = tileType;
                tileTypeToPossibleTiles.possibleTiles = new List<Tile>();
                templateGenerationParameters.tileTypesToPossibleTiles.tileTypesToPossibleTiles.Add(tileTypeToPossibleTiles);
            }

            int max = SaveManager.autosaveExists ? SaveManager.savedVisitedRooms[0].z : Deck.playerDeck.cards.Count;
            for (int i = 0; i < max; i++)
            {
                OnCardAdded(Deck.playerDeck.cards[i]);
            }
        }

        /// <summary>
        /// Adds the added tiles from the card
        /// </summary>
        /// <param name="card"> The card </param>
        private void OnCardAdded(Card card)
        {
            if (card == null || card.effects == null)
            {
                return;
            }

            foreach (DungeonEffect effect in card.effects)
            {
                if (effect == null)
                {
                    continue;
                }
                if (effect.tiles == null)
                {
                    return;
                }
                foreach (Tile tile in effect.tiles)
                {
                    templateGenerationParameters.tileTypesToPossibleTiles.At(tile.type).Add(tile);
                }
            }
        }

        /// <summary>
        /// Removes the added tiles from the card
        /// </summary>
        /// <param name="card"> The card </param>
        private void OnCardRemoved(Card card)
        {
            foreach (DungeonEffect effect in card.effects)
            {
                foreach (Tile tile in effect.tiles)
                {
                    templateGenerationParameters.tileTypesToPossibleTiles.At(tile.type).Remove(tile);
                }
            }
        }

        /// Transforms a map location to a world location
        /// </summary>
        /// <param name="mapLocation"> The location to transform </param>
        /// <param name="startLocation"> The start location (aka midpoint of the map) </param>
        /// <param name="roomSize"> The room size </param>
        /// <returns> The world location </returns>
        static public Vector2 TransformMapToWorld(Vector2Int mapLocation, Vector2Int startLocation, Vector2Int roomSize)
        {
            return new Vector2((mapLocation.x - startLocation.x) * roomSize.x, (mapLocation.y - startLocation.y) * roomSize.y);
        }
    }
}