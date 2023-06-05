using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    // the state this machine starts in
    [SerializeField] private BaseState initialState;

    // delay after this enemy is spawned before it begins performing logic
    [SerializeField] private float delayBeforeLogic;
    
    // the target
    [HideInInspector] public GameObject currentTarget;
    
    // the player
    [HideInInspector] public GameObject player;

    // our feet position
    [HideInInspector] public Collider2D feetCollider;

    // the current state this machine is in
    [HideInInspector] public BaseState currentState;

    public struct PathData
    {
        // path to target 
        public Path path;
        
        // index of where we are in the path
        public int targetIndex;
        
        // do we ignore incoming path requests?
        public bool ignorePathRequests;
        
        // store the path following coroutine so it can be cancelled as needed
        public Coroutine prevFollowCoroutine;
    }
    
    // stores our current path data
    [HideInInspector] public PathData pathData;

    // maintained list of components which are cached for performance
    private Dictionary<Type, Component> cachedComponents;
    
    // tracks the time this was initialized
    private float timeStarted;

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
        timeStarted = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
        currentTarget = player;
        FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies.Add(gameObject);
    }

    /// <summary>
    /// Execute the current state
    /// </summary>
    private void Update()
    {
        if (Time.time - timeStarted <= delayBeforeLogic) return;
        
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
        FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies.Remove(gameObject);
    }
}