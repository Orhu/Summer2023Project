using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    // the state this machine starts in
    [SerializeField] private BaseState initialState;

    // the target
    [HideInInspector] public GameObject currentTarget;
    
    // the player
    [HideInInspector] public GameObject player;

    // our feet position
    [HideInInspector] public Collider2D feetCollider;

    // the current state this machine is in
    [HideInInspector] public BaseState currentState;

    // maintained list of components which are cached for performance
    private Dictionary<Type, Component> cachedComponents;

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void Awake()
    {
        currentState = initialState;
        cachedComponents = new Dictionary<Type, Component>();
        feetCollider = GetComponentInChildren<Collider2D>();
    }

    /// <summary>
    /// Grab the player gameobject and sets it to the default target
    /// </summary>
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentTarget = player;
        FloorGenerator.floorGeneratorInstance.currentRoom.AddEnemy(gameObject);
    }

    /// <summary>
    /// Execute the current state
    /// </summary>
    private void Update()
    {
        currentState.OnStateUpdate(this);
    }

    /// <summary>
    /// Get a component from this object, caching it if it is not already cached
    /// </summary>
    /// <typeparam name="T"> Component to get </typeparam>
    /// <returns> Requested component, if it exists. If not, returns null </returns>
    public new T GetComponent<T>() where T : Component
    {
        if (cachedComponents.ContainsKey(typeof(T)))
            return cachedComponents[typeof(T)] as T;

        var component = base.GetComponent<T>();
        if (component != null)
        {
            cachedComponents.Add(typeof(T), component);
        }

        return component;
    }

    /// <summary>
    /// Remove me from the floor generator's list of living enemies
    /// </summary>
    private void OnDestroy()
    {
        FloorGenerator.floorGeneratorInstance.currentRoom.RemoveEnemy(gameObject);
    }
}