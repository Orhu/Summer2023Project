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
    }

    /// <summary>
    /// If no target is provided, just use the player as the target
    /// </summary>
    private void Start()
    {
        if (currentTarget == null)
        {
            currentTarget = GameObject.FindGameObjectWithTag("Player");
        }
    }

    /// <summary>
    /// Execute the current state
    /// </summary>
    private void Update()
    { 
        currentState.Execute(this);
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
}