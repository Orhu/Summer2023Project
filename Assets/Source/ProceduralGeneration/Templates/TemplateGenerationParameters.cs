using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The parameters to use when generating a template
    /// </summary>
    [System.Serializable]
    public class TemplateGenerationParameters
    {
        [Tooltip("The pool of templates to draw from and their associated room types")]
        [SerializeField] public RoomTypesToDifficultiesToTemplates templatesPool;

        [Tooltip("How much the percentage chance of generating a hard room increases when generating a new room")]
        [Range(0, 100)]
        [SerializeField] public float hardRoomPercentageIncrease;

        [Tooltip("The tile types and the possible tiles they can spawn. Use this to specify the generics of this floor")]
        [SerializeField] public TileTypesToPossibleTiles tileTypesToPossibleTiles;

        [Tooltip("The tile types and their associated generic tiles")]
        public GenericTiles genericTiles;

        // The templates that have been used
        [HideInInspector] private RoomTypesToDifficultiesToTemplates usedTemplates;

        // The current percent chance that a hard room will generate
        [HideInInspector] private float hardRoomPercentage;

        /// <summary>
        /// Constructor that makes sure the used templates variable is initialized
        /// </summary>
        public TemplateGenerationParameters()
        {
            usedTemplates = new RoomTypesToDifficultiesToTemplates();

            foreach (RoomType roomType in System.Enum.GetValues(typeof(RoomType)))
            {
                RoomTypeToDifficultiesToTemplates roomTypeToDifficultiesToTemplates = new RoomTypeToDifficultiesToTemplates();
                roomTypeToDifficultiesToTemplates.roomType = roomType;
                DifficultiesToTemplates difficultiesToTemplates = new DifficultiesToTemplates();
                foreach (Difficulty difficulty in System.Enum.GetValues(typeof(Difficulty)))
                {
                    DifficultyToTemplates difficultyToTemplates = new DifficultyToTemplates();
                    difficultyToTemplates.difficulty = difficulty;
                    difficultyToTemplates.templates = new List<Template>();
                    difficultiesToTemplates.difficultiesToTemplates.Add(difficultyToTemplates);
                }
                roomTypeToDifficultiesToTemplates.difficultiesToTemplates = difficultiesToTemplates;
                usedTemplates.roomTypesToDifficultiesToTemplates.Add(roomTypeToDifficultiesToTemplates);
            }
        }

        /// <summary>
        /// Sees if it's possible to spawn the preferred tile. If not, then gets a random tile of the same type. If there are
        /// no possible tiles of that type, spawns the generic version of that type.
        /// </summary>
        /// <param name="preferredTile"> The preferred tile to spawn </param>
        /// <returns> The random tile </returns>
        public Tile GetRandomTile(TemplateTile preferredTile)
        {
            List<Tile> possibleTiles = tileTypesToPossibleTiles.At(preferredTile.tileType);

            if (possibleTiles.Contains(preferredTile.preferredTile))
            {
                return preferredTile.preferredTile.ShallowCopy();
            }

            if (possibleTiles.Count != 0)
            {
                return possibleTiles[FloorGenerator.random.Next(0, possibleTiles.Count)].ShallowCopy();
            }

            return genericTiles.At(preferredTile.tileType).ShallowCopy();
        }

        /// <summary>
        /// Gets a random template that can be used with the room type
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <returns> A random template </returns>
        public Template GetRandomTemplate(RoomType roomType)
        {
            Difficulty difficulty;
            List<Template> possibleTemplates = GetPossibleTemplates(roomType, out difficulty);
            Template randomTemplate = possibleTemplates[FloorGenerator.random.Next(0, possibleTemplates.Count)];
            templatesPool.Remove(roomType, difficulty, randomTemplate);
            usedTemplates.At(roomType).At(difficulty).Add(randomTemplate);

            if (templatesPool.At(roomType).At(difficulty).Count == 0)
            {
                List<Template> templates = usedTemplates.At(roomType).At(difficulty);
                for (int i = 0; i < templates.Count; i++)
                {
                    templatesPool.At(roomType).At(difficulty).Add(templates[i]);
                }
                usedTemplates.At(roomType).At(difficulty).Clear();
            }

            return randomTemplate;
        }

        /// <summary>
        /// Gets the possible templates from the room type
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="difficulty"> The difficulty that was chosen </param>
        /// <returns> The list of possible templates </returns>
        private List<Template> GetPossibleTemplates(RoomType roomType, out Difficulty difficulty)
        {
            DifficultiesToTemplates difficultiesToTemplates = templatesPool.At(roomType);

            // Normal is the only type of room that difficulty matters for
            if (roomType == RoomType.Normal)
            {
                if (hardRoomPercentage / 100 > FloorGenerator.random.NextDouble())
                {
                    difficulty = Difficulty.Hard;
                    hardRoomPercentage = 0;
                }
                else
                {
                    difficulty = Difficulty.Easy;
                    hardRoomPercentage += hardRoomPercentageIncrease;
                }

                return difficultiesToTemplates.At(difficulty);
            }

            difficulty = Difficulty.NotApplicable;
            return difficultiesToTemplates.At(difficulty);
        }
    }

    /// <summary>
    /// The difficulty of a template
    /// </summary>
    [System.Serializable]
    public enum Difficulty
    {
        NotApplicable,
        Easy,
        Hard
    }

    /// <summary>
    /// A dictionary that tracks room types and their associated difficulties and their associated templates
    /// </summary>
    [System.Serializable]
    public class RoomTypesToDifficultiesToTemplates
    {
        [Tooltip("The room types and their associated difficulties and their associated templates")]
        public List<RoomTypeToDifficultiesToTemplates> roomTypesToDifficultiesToTemplates = new List<RoomTypeToDifficultiesToTemplates>();

        /// <summary>
        /// Gets the difficulties to templates associated with the given room type
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <returns> The difficulties to templates </returns>
        public DifficultiesToTemplates At(RoomType roomType)
        {
            for (int i = 0; i < roomTypesToDifficultiesToTemplates.Count; i++)
            {
                if (roomTypesToDifficultiesToTemplates[i].roomType == roomType)
                {
                    return roomTypesToDifficultiesToTemplates[i].difficultiesToTemplates;
                }
            }

            throw new System.Exception("No templates associated with room type  " + roomType.ToString());
        }

        /// <summary>
        /// Removes the specified template from the specified difficulty from the specified room type
        /// </summary>
        /// <param name="roomType"> The room type to remove from </param>
        /// <param name="difficulty"> The difficulty to remove from </param>
        /// <param name="template"> The template to remove </param>
        public void Remove(RoomType roomType, Difficulty difficulty, Template template)
        {
            List<Template> templates = At(roomType).At(difficulty);
            templates.Remove(template);
        }
    }

    /// <summary>
    /// A struct that holds a room type and its associated difficulties and their associated templates
    /// </summary>
    [System.Serializable]
    public struct RoomTypeToDifficultiesToTemplates
    {
        [Tooltip("The room type")]
        public RoomType roomType;

        [Tooltip("The room type's associated difficulties and templates")]
        public DifficultiesToTemplates difficultiesToTemplates;
    }

    /// <summary>
    /// A dictionary that tracks difficulties with their templates
    /// </summary>
    [System.Serializable]
    public class DifficultiesToTemplates
    {
        [Tooltip("The difficulties and their associated templates")]
        public List<DifficultyToTemplates> difficultiesToTemplates = new List<DifficultyToTemplates>();

        /// <summary>
        /// Finds the associated list of templates of the given difficulty
        /// </summary>
        /// <param name="difficulty"> The difficulty </param>
        /// <returns> The list of templates </returns>
        public List<Template> At(Difficulty difficulty)
        {
            for (int i = 0; i < difficultiesToTemplates.Count; i++)
            {
                if (difficultiesToTemplates[i].difficulty == difficulty)
                {
                    return difficultiesToTemplates[i].templates;
                }
            }

            throw new System.Exception("No templates associated with difficulty " + difficulty.ToString());
        }
    }

    /// <summary>
    /// A struct that holds a difficulty and its associated templates 
    /// </summary>
    [System.Serializable]
    public struct DifficultyToTemplates
    {
        [Tooltip("The difficulty")]
        public Difficulty difficulty;

        [Tooltip("The associated templates of that difficulty")]
        public List<Template> templates;
    }

    /// <summary>
    /// Holds tile types and their associated possible tiles that could be spawned
    /// </summary>
    [System.Serializable]
    public class TileTypesToPossibleTiles
    {
        [Tooltip("Tile types and their associated possible tiles")]
        public List<TileTypeToPossibleTiles> tileTypesToPossibleTiles = new List<TileTypeToPossibleTiles>();

        /// <summary>
        /// Gets the associated possible tiles with the given tile type
        /// </summary>
        /// <param name="tileType"> The tile type </param>
        /// <returns> The possible tiles </returns>
        public List<Tile> At(TileType tileType)
        {
            for (int i = 0; i < tileTypesToPossibleTiles.Count; i++)
            {
                if (tileTypesToPossibleTiles[i].tileType == tileType)
                {
                    return tileTypesToPossibleTiles[i].possibleTiles;
                }
            }

            throw new System.Exception("No tiles associated with tile type " + tileType.ToString());
        }
    }

    /// <summary>
    /// Holds a tile type and its associated possible spawnable tiles
    /// </summary>
    [System.Serializable]
    public struct TileTypeToPossibleTiles
    {
        [Tooltip("The tile type")]
        public TileType tileType;

        [Tooltip("The possible spawnable tiles (from the player deck)")]
        public List<Tile> possibleTiles;
    }
}