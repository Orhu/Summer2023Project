using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A generic class that allows for picking a random thing, with a weight and a maximum number of times it can be chosen
/// </summary>
/// <typeparam name="T"> The thing </typeparam>
[System.Serializable]
public class GenericWeightedThing<T>
{
    [Tooltip("The weighted thing")]
    public T thing;

    [Tooltip("The weight that the thing has")]
    public float weight = 0;

    [Tooltip("The maximum number of times this thing can be chosen. Make this less than 0 for no max")]
    public int maxChosen = -1;

    // The number of times this thing has been chosen
    [HideInInspector] public int timesChosen;

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
    [HideInInspector] public List<GenericWeightedThing<T>> choosableThings;

    // The total weight of all the things
    private float totalWeight;

    /// <summary>
    /// Initializes the choosable things list with the things list
    /// </summary>
    public GenericWeightedThings()
    {
        if (things == null)
        {
            return;
        }
        foreach (GenericWeightedThing<T> thing in things)
        {
            Add(thing);
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
    public void Add(T newThing, float weight, int maxChosen = -1)
    {
        Add(new GenericWeightedThing<T>(newThing, weight, maxChosen));
    }

    /// <summary>
    /// Adds another thing to the list of things that can be chosen
    /// </summary>
    /// <param name="newThing"> The new thing </param>
    public void Add(GenericWeightedThing<T> newThing)
    {
        choosableThings.Add(newThing);
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
        float randomPercent;
        if (random == null)
        {
            randomPercent = Random.value;
        }
        else
        {
            randomPercent = (float) random.NextDouble();
        }

        float percentCounter = 0.0f;
        
        foreach (GenericWeightedThing<T> thing in choosableThings)
        {
            percentCounter += (thing.weight / totalWeight);

            if (percentCounter >= randomPercent)
            {
                thing.timesChosen++;
                if (thing.maxChosen == thing.timesChosen)
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
}
