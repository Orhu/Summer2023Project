using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The Params to use when generating a template
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewTemplateParams", menuName = "Generation/TemplateParams")]
    public class TemplateParams : ScriptableObject
    {
        [Tooltip("The pool of templates to draw from and their associated room types")]
        public RoomTypesToDifficultiesToTemplates templatesPool;

        // The templates that have been used
        [HideInInspector] private RoomTypesToDifficultiesToTemplates usedTemplates;

        // The current percent chance that a hard room will generate
        [HideInInspector] private float hardRoomPercentage;

        /// <summary>
        /// Constructor that makes sure the used templates variable is initialized
        /// </summary>
        public TemplateParams()
        {
            usedTemplates = new RoomTypesToDifficultiesToTemplates();
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
            if (!usedTemplates.Contains(roomType))
            {
                RoomTypeToDifficultiesToTemplates roomTypeToDifficultiesToTemplates = new RoomTypeToDifficultiesToTemplates();
                roomTypeToDifficultiesToTemplates.roomType = roomType;
                DifficultyToTemplates difficultyToTemplates = new DifficultyToTemplates();
                difficultyToTemplates.difficulty = difficulty;
                difficultyToTemplates.templates = new List<Template>();
                difficultyToTemplates.templates.Add(randomTemplate);
                roomTypeToDifficultiesToTemplates.difficultiesToTemplates = new DifficultiesToTemplates();
                roomTypeToDifficultiesToTemplates.difficultiesToTemplates.Add(difficultyToTemplates);
                usedTemplates.Add(roomTypeToDifficultiesToTemplates);
            }
            else if (!usedTemplates.At(roomType).Contains(difficulty))
            {
                DifficultyToTemplates difficultyToTemplates = new DifficultyToTemplates();
                difficultyToTemplates.difficulty = difficulty;
                difficultyToTemplates.templates = new List<Template>();
                difficultyToTemplates.templates.Add(randomTemplate);
                usedTemplates.At(roomType).Add(difficultyToTemplates);
            }
            else
            {
                usedTemplates.At(roomType).At(difficulty).Add(randomTemplate);
            }

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

            if (roomType.useDifficulty)
            {
                if (hardRoomPercentage / 100 > FloorGenerator.random.NextDouble())
                {
                    difficulty = Difficulty.Hard;
                    hardRoomPercentage = 0;
                }
                else
                {
                    difficulty = Difficulty.Easy;
                    hardRoomPercentage += DifficultyProgressionManager.hardRoomPercentageIncrease; 
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
        /// Checks whether the templates pool contains the room type
        /// </summary>
        /// <param name="roomType"> The room type to check </param>
        /// <returns> Whether or not the templates pool contains the room type </returns>
        public bool Contains(RoomType roomType)
        {
            foreach (RoomTypeToDifficultiesToTemplates roomTypeToDifficultiesToTemplates in roomTypesToDifficultiesToTemplates)
            {
                if (roomTypeToDifficultiesToTemplates.roomType == roomType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a room type to difficulties to templates to the pool
        /// </summary>
        /// <param name="roomTypeToDifficultiesToTemplates"> The room type to difficulties to templates to add </param>
        public void Add(RoomTypeToDifficultiesToTemplates roomTypeToDifficultiesToTemplates)
        {
            roomTypesToDifficultiesToTemplates.Add(roomTypeToDifficultiesToTemplates);
        }

        /// <summary>
        /// Adds a room type to difficulties to templates to the pool
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="difficultiesToTemplates"> The difficulties to templates </param>
        public void Add(RoomType roomType, DifficultiesToTemplates difficultiesToTemplates)
        {
            roomTypesToDifficultiesToTemplates.Add(new RoomTypeToDifficultiesToTemplates(roomType, difficultiesToTemplates));
        }

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

        /// <summary>
        /// Constructor that takes a room type and difficulties to templates
        /// </summary>
        /// <param name="roomType"> The room type </param>
        /// <param name="difficultiesToTemplates"> The room type's associated difficulties and templates </param>
        public RoomTypeToDifficultiesToTemplates(RoomType roomType, DifficultiesToTemplates difficultiesToTemplates)
        {
            this.roomType = roomType;
            this.difficultiesToTemplates = difficultiesToTemplates;
        }
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

        /// <summary>
        /// Adds a difficulty to templates to the list
        /// </summary>
        /// <param name="difficultyToTemplates"> The difficulty to templates to add </param>
        public void Add(DifficultyToTemplates difficultyToTemplates)
        {
            difficultiesToTemplates.Add(difficultyToTemplates);
        }

        /// <summary>
        /// Checks if the list contains the given difficulty
        /// </summary>
        /// <param name="difficulty"> The difficulty to check </param>
        /// <returns> Whether or not the list contains the given difficulty </returns>
        public bool Contains(Difficulty difficulty)
        {
            for (int i = 0; i < difficultiesToTemplates.Count; i++)
            {
                if (difficultiesToTemplates[i].difficulty == difficulty)
                {
                    return true;
                }
            }

            return false;
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
}