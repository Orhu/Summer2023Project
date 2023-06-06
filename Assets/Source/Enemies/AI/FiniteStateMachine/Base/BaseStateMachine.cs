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
    [HideInInspector] public Vector2 currentTarget;
    
    // the player
    [HideInInspector] public GameObject player;

    // our feet position
    [HideInInspector] public Collider2D feetCollider;

    // the current state this machine is in
    [HideInInspector] public BaseState currentState;
    
    // tracks whether our destination has been reached or not
    [HideInInspector] public bool destinationReached;

    public struct PathData
    {
        // path to target 
        public Path path;
        
        // index of where we are in the path
        public int targetIndex;
        
        // do we ignore incoming path requests?
        public bool ignorePathRequests;
        
        // store the path following coroutine so it can be cancelled as needed
        public IEnumerator prevFollowCoroutine;
    }
    
    // stores our current path data
    [HideInInspector] public PathData pathData;

    public struct CooldownData
    {
        // is attack cooldown available?
        public Dictionary<FSMAction, bool> cooldownReady;
    }
    // stores our current attack data
    [HideInInspector] public CooldownData cooldownData;

    // maintained list of components which are cached for performance
    private Dictionary<Type, Component> cachedComponents;
    
    // tracks the time this was initialized
    private float timeStarted;

    [SerializeField] private bool drawGizmos;
    // debug waypoint used for drawing gizmos
    [HideInInspector] public Vector2 debugWaypoint;

    /// <summary>
    /// Initialize variables
    /// </summary>
    private void Awake()
    {
        currentState = initialState;
        cooldownData.cooldownReady = new Dictionary<FSMAction, bool>();
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
        FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies.Add(gameObject);
        currentState.OnStateEnter(this);
    }

    /// <summary>
    /// Execute the current state
    /// </summary>
    private void Update()
    {
        if (Time.time - timeStarted <= delayBeforeLogic) return;
        
        print(currentState);
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

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(debugWaypoint, Vector3.one);
    }
}