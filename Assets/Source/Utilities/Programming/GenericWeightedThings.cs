using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A generic class that allows for picking a random thing, with a weight and a maximum number of times it can be chosen
    /// </summary>
    /// <typeparam name="T"> The type of thing </typeparam>
    [System.Serializable]
    public class GenericWeightedThing<T>
    {
        [Tooltip("The weighted thing")]
        public T thing;

        [Tooltip("The weight that the thing has")]
        public float weight = 1;

        [Tooltip("The maximum number of times this thing can be chosen. Make this less than 0 for no max")]
        public int maxChosen = -1;

        // The number of times this thing has been chosen
        [System.NonSerialized] public int timesChosen = 0;

        /// <summary>
        /// Constructor that initializes the generic weighted thing
        /// </summary>
        /// <param name="weightedThing"> The weighted thing </param>
        /// <param name="weightOfThing"> The weight of the weighted thing </param>
        /// <param name="maxChosenOfThing"> The maximum number of times this thing can be chosen. Make this less than 0 for no max </param>
        public GenericWeightedThing(T weightedThing, float weightOfThing, int maxChosenOfThing = -1)
        {
            thing = weightedThing;
            weight = weightOfThing;
            maxChosen = maxChosenOfThing;
            timesChosen = 0;
        }
    }

    /// <summary>
    /// A generic class that allows for picking a random thing from a list of things
    /// with a weight and a maximum number of times they can be chosen
    /// </summary>
    /// <typeparam name="T"> The type of the things </typeparam>
    [System.Serializable]
    public class GenericWeightedThings<T>
    {
        [Tooltip("The initial list of things that can be chosen")]
        public List<GenericWeightedThing<T>> things;

        // The list of things that can be chosen, with the things that have reached their maximum allotted amount removed
        [System.NonSerialized] public List<GenericWeightedThing<T>> choosableThings;

        // The total weight of all the things
        [System.NonSerialized] private float totalWeight = 0;

        /// <summary>
        /// Adds the things in the things list to the choosable things list
        /// </summary>
        private void AddThingsToChoosableThings()
        {
            if (things == null)
            {
                return;
            }

            foreach (GenericWeightedThing<T> thing in things)
            {
                GenericWeightedThing<T> newThing = new GenericWeightedThing<T>(thing.thing, thing.weight, thing.maxChosen);
                Add(newThing, false);
            }
        }

        /// <summary>
        /// Recaulcates the total weight of all the things in the list
        /// </summary>
        public void calculateTotalWeight()
        {
            totalWeight = 0;

            foreach (GenericWeightedThing<T> thing in choosableThings)
            {
                totalWeight += thing.weight;
            }
        }

        /// <summary>
        /// Adds another thing to the list of things that can be chosen
        /// </summary>
        /// <param name="newThing"> The new thing </param>
        /// <param name="weight"> The weight of this new thing </param>
        /// <param name="maxChosen"> The maximum number of times this new thing can be chosen (Set it to less than 0 for no max) </param>
        /// <param name="addToPermanentListOfThings"> Whether or not to add to the permanent list of things </param>
        public void Add(T newThing, float weight, int maxChosen = -1, bool addToPermanentListOfThings = true)
        {
            Add(new GenericWeightedThing<T>(newThing, weight, maxChosen), addToPermanentListOfThings);
        }

        /// <summary>
        /// Adds another thing to the list of things that can be chosen
        /// </summary>
        /// <param name="newThing"> The new thing </param>
        /// <param name="addToPermanentListOfThings"> Whether or not to add to the permanent list of things </param>
        public void Add(GenericWeightedThing<T> newThing, bool addToPermanentListOfThings = true)
        {
            if (choosableThings == null)
            {
                choosableThings = new List<GenericWeightedThing<T>>();
            }
            choosableThings.Add(newThing);
            if (addToPermanentListOfThings)
            {
                if (things == null)
                {
                    things = new List<GenericWeightedThing<T>>();
                }
                things.Add(newThing);
            }
            totalWeight += newThing.weight;
        }

        /// <summary>
        /// Removes a thing from the choosable things
        /// </summary>
        /// <param name="removedThing"> The removed thing </param>
        public void Remove(GenericWeightedThing<T> removedThing)
        {
            if (choosableThings.Remove(removedThing))
            {
                totalWeight -= removedThing.weight;
            }
        }

        /// <summary>
        /// Returns a random thing using the weights
        /// </summary>
        /// <param name="random"> The random to use (if null, Unity's random will be used) </param>
        /// <returns> A random thing </returns>
        public T GetRandomThing(System.Random random = null)
        {
            if (choosableThings == null || choosableThings.Count == 0)
            {
                Reset();
                if (choosableThings == null || choosableThings.Count == 0)
                {
                    throw new System.Exception("Attempted to get a random thing when there are no choosable things");
                }
            }

            float randomPercent;
            if (random == null)
            {
                randomPercent = Random.value;
            }
            else
            {
                randomPercent = (float)random.NextDouble();
            }

            float percentCounter = 0.0f;

            foreach (GenericWeightedThing<T> thing in choosableThings)
            {
                percentCounter += (thing.weight / totalWeight);

                if (percentCounter >= randomPercent)
                {
                    thing.timesChosen++;
                    if ((thing.maxChosen == thing.timesChosen) && (thing.maxChosen >= 0))
                    {
                        Remove(thing);
                    }
                    return thing.thing;
                }
            }

            GenericWeightedThing<T> lastThing = choosableThings[choosableThings.Count - 1];
            lastThing.timesChosen++;
            if (lastThing.maxChosen == lastThing.timesChosen)
            {
                Remove(lastThing);
            }

            return lastThing.thing;
        }

        /// <summary>
        /// Gets a random thing, using a unique seed created by the given position
        /// </summary>
        /// <param name="position"> The position to use to get the seed </param>
        /// <returns> The random thing </returns>
        public T GetRandomThing(Vector3 position)
        {
            System.Random random = new System.Random(PositionToSeed(position));
            return GetRandomThing(random);
        }

        /// <summary>
        /// Gets multiple random things
        /// </summary>
        /// <param name="num"> The number of random things to get </param>
        /// <param name="random"> The random to use (if null, will use Unity's default random) </param>
        /// <returns> The random things </returns>
        public List<T> GetRandomThings(int num, System.Random random = null)
        {
            List<T> randomThings = new List<T>();
            for (int i = 0; i < num; i++)
            {
                randomThings.Add(GetRandomThing(random));
            }
            return randomThings;
        }

        /// <summary>
        /// Gets multiple random things, using a unique seed created by the given position
        /// </summary>
        /// <param name="num"> The number of random things to get </param>
        /// <param name="position"> The position to use to get the seed </param>
        /// <returns> The random thigns </returns>
        public List<T> GetRandomThings(int num, Vector3 position)
        {
            System.Random random = new System.Random(PositionToSeed(position));
            return GetRandomThings(num, random);
        }

        /// <summary>
        /// Resets the choosable things list, initializing it with the list of permanent things
        /// </summary>
        public void Reset()
        {
            if (choosableThings != null)
            {
                choosableThings.Clear();
            }
            totalWeight = 0;
            AddThingsToChoosableThings();
        }

        /// <summary>
        /// Maps a position to a unique seed, using the floor generator seed.
        /// </summary>
        /// <param name="position"> The position to convert. </param>
        /// <returns> The seed corresponding to the position. </returns>
        private int PositionToSeed(Vector3 position)
        {
            // Szudzik's function:
            int x = (int)position.x >= 0 ? 2 * (int)position.x : -2 * (int)position.x - 1;
            int y = (int)position.y >= 0 ? 2 * (int)position.y : -2 * (int)position.y - 1;
            int result = x >= y ? x * x + x + y : x + y * y;

            return result + FloorGenerator.seed;
        }
    }
}