using System;
using System.Collections;
using System.Collections.Generic;
using Cardificer;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the state machine that manages and switches between states. Essentially serves as the "brain" and logic of an enemy.
    /// </summary>
    public class BaseStateMachine : MonoBehaviour, IActor
    {
        // the state this machine starts in
        [SerializeField] private BaseState initialState;

        // delay after this enemy is spawned before it begins performing logic
        [SerializeField] private float delayBeforeLogic;

        // the pathfinding target
        [HideInInspector] public Vector2 currentPathfindingTarget;

        // the attack target
        [HideInInspector] public Vector2 currentAttackTarget;

        // cached feet collider
        [HideInInspector] public Collider2D feetCollider;

        // the current state this machine is in
        [HideInInspector] public BaseState currentState;

        // tracks whether our destination has been reached or not
        [HideInInspector] public bool destinationReached;

        // tracks whether we are currently exhausted
        [HideInInspector] public bool exhausted;

        // struct used to store path data with this state machine instance, so we can remember pathfinding data on scriptableobjects where we cannot store them in-object
        public struct PathData
        {
            // path to target 
            public Cardificer.Path path;

            // index of where we are in the path
            public int targetIndex;

            // do we ignore incoming path requests?
            public bool ignorePathRequests;

            // store the path following coroutine so it can be cancelled as needed
            public IEnumerator prevFollowCoroutine;
        }

        // stores our current path data
        [HideInInspector] public PathData pathData;

        // struct to store cooldown data
        public struct CooldownData
        {
            // is action cooldown available?
            public Dictionary<FSMAction, bool> cooldownReady;
        }

        // stores our current attack data
        [HideInInspector] public CooldownData cooldownData;

        // maintained list of components which are cached for performance
        private Dictionary<Type, Component> cachedComponents;

        // tracks the time this was initialized
        private float timeStarted;

        [Tooltip("Movement type this enemy begins in")]
        [SerializeField] private MovementType startingMovementType;

        // current movement type of this enemy
        [HideInInspector] public MovementType currentMovementType;

        // chase data struct used to store chase data as it is passed to the pathfinding singleton from pathing scriptable objects.
        // never need to actually cache the data in the state machine, just declaring the struct here so it is only declared once
        public struct ChaseData
        {
            private BaseStateMachine stateMachine;
            private Coroutine prevUpdateCoroutine;
            private Coroutine prevFollowCoroutine;
        }

        // draw debug gizmos?
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
            feetCollider = FindMyFeet();
            currentMovementType = startingMovementType;
        }

        /// <summary>
        /// Retrieves feet collider
        /// </summary>
        Collider2D FindMyFeet()
        {
            if (feetCollider != null)
            {
                return feetCollider;
            }

            var thisCollider = GetComponentInChildren<Collider2D>();

            if (thisCollider != null && !thisCollider.isTrigger)
            {
                feetCollider = thisCollider;
                return thisCollider;
            }
            else
            {
                Debug.LogError(
                    "No feet collider found! Make sure the enemy has a non-trigger collider component attached to one of its children.");
                return null;
            }
        }

        /// <summary>
        /// Grab the player gameobject and sets it to the default target
        /// </summary>
        private void Start()
        {
            timeStarted = Time.time;
            FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies.Add(gameObject);
            currentState.OnStateEnter(this);
        }

        /// <summary>
        /// Execute the current state
        /// </summary>
        private void Update()
        {
            if (Time.time - timeStarted <= delayBeforeLogic) return;

            if (exhausted)
            {
                GetComponent<Movement>().movementInput = Vector2.zero;
                return;
            }

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
            if (!gameObject.scene.isLoaded)
            {
                return;
            }

            FloorGenerator.floorGeneratorInstance.currentRoom.RemoveEnemy(gameObject);
        }

        /// <summary>
        /// Draw debug gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(debugWaypoint, Vector3.one);
        }

        #region IActor Implementation

        // Gets whether or not this actor can act.
        IActor.CanActRequest _canAct;

        public bool canAct
        {
            get
            {
                bool shouldAct = true;
                _canAct?.Invoke(ref shouldAct);
                return shouldAct;
            }
        }

        /// <summary>
        /// Get the transform that the action should be played from.
        /// </summary>
        /// <returns> The actor transform. </returns>
        public Transform GetActionSourceTransform()
        {
            return transform;
        }


        /// <summary>
        /// Get the position that the action should be aimed at.
        /// </summary>
        /// <returns> The mouse position in world space. </returns>
        public Vector3 GetActionAimPosition()
        {
            return currentAttackTarget;
        }


        /// <summary>
        /// Gets the collider of this actor.
        /// </summary>
        /// <returns> The collider. </returns>
        public Collider2D GetCollider()
        {
            return GetComponent<Collider2D>();
        }


        /// <summary>
        /// Gets the delegate that will fetch whether this actor can act.
        /// </summary>
        /// <returns> A delegate with a out parameter, that allows any subscribed objects to determine whether or not this actor can act. </returns>
        public ref IActor.CanActRequest GetOnRequestCanAct()
        {
            return ref _canAct;
        }

        #endregion
    }
}