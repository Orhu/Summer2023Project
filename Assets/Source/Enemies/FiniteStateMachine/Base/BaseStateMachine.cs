using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementType = Cardificer.RoomInterface.MovementType;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the state machine that manages and switches between states. Essentially serves as the "brain" and logic of an enemy.
    /// </summary>
    [RequireComponent(typeof(Health), typeof(SimpleMovement))]
    public class BaseStateMachine : MonoBehaviour, IActor
    {
        // the state this machine starts in
        [SerializeField] private State initialState;

        // Delay after this enemy is spawned before it begins performing logic
        [SerializeField] private float delayBeforeLogic;

        // The pathfinding target
        [HideInInspector] public Vector2 currentPathfindingTarget;

        // The attack target
        [HideInInspector] public Vector2 currentAttackTarget;

        // the current state this machine is in
        [HideInInspector] public State currentState;

        // Tracks whether we are currently exhausted
        [HideInInspector] public bool exhausted;

        /// <summary>
        /// Struct used to store path data with this state machine instance,
        /// so we can remember pathfinding data on our ScriptableObjects where we cannot store them in-object
        /// </summary>
        public struct PathData
        {
            // Path to target 
            public Path path;

            // Index of where we are in the path
            public int targetIndex;

            // Do we ignore incoming path requests?
            public bool ignorePathRequests;

            // Store the path following coroutine so it can be cancelled as needed
            public IEnumerator prevFollowCoroutine;
            
            // Should we keep following the path? Checked every time an enemy tries to move
            public bool keepFollowingPath;
            
            // Tracks whether our destination has been reached or not
            [HideInInspector] public bool destinationReached;
        }

        // Stores our current path data
        [HideInInspector] public PathData pathData;

        // Struct to store cooldown data
        public struct CooldownData
        {
            // is action cooldown available?
            public Dictionary<BaseAction, bool> cooldownReady;
        }

        // Stores our current attack data
        [HideInInspector] public CooldownData cooldownData;

        // Maintained list of components which are cached for performance
        private Dictionary<Type, Component> cachedComponents;

        // Tracks the time this was initialized
        private float timeStarted;
        
        // Cached feet collider
        private Collider2D _feetCollider;
        private Collider2D feetCollider
        {
            get
            {
                if (_feetCollider != null)
                {
                    return _feetCollider;
                }

                Collider2D enemyFeetCollider = null;
                var enemyColliders = GetComponentsInChildren<Collider2D>();
                foreach (var enemyCollider in enemyColliders)
                {
                    if (!enemyCollider.isTrigger)
                    {
                        enemyFeetCollider = enemyCollider;
                        break;
                    }
                }

                if (enemyFeetCollider != null)
                {
                    _feetCollider = enemyFeetCollider;
                    return _feetCollider;
                }
                else
                {
                    Debug.LogError("No feet collider found! Make sure you have a non-trigger collider attached to the enemy.");
                    return null;
                }
            }
        }

        [Tooltip("Movement type this enemy begins in")]
        [SerializeField] private MovementType startingMovementType;

        // Current movement type of this enemy
        private MovementType _currentMovementType;
        public MovementType currentMovementType
        {
            set
            {
                switch (value)
                {
                    case MovementType.Burrow:
                        feetCollider.gameObject.layer = LayerMask.NameToLayer("Burrowing");
                        break;
                    case MovementType.Fly:
                        feetCollider.gameObject.layer = LayerMask.NameToLayer("Flying");
                        break;
                    case MovementType.Walk:
                        feetCollider.gameObject.layer = LayerMask.NameToLayer("Walking");
                        break;
                    default:
                        Debug.LogError("Tried to set " + name + " to an invalid movement type");
                        return;
                }
                _currentMovementType = value;
            }
            get => _currentMovementType;
        }

        /// <summary>
        /// Chase Data struct used to store chase data as it is passed to the pathfinding singleton from pathing scriptable objects.
        /// Never need to actually cache the data in the state machine, just declaring the struct here so it is only declared once.
        /// </summary>
        public struct ChaseData
        {
            // represents the state machine that requested the path
            private BaseStateMachine stateMachine;
            // represents the current update path coroutine
            private Coroutine prevUpdateCoroutine;
            // represents the current follow path coroutine
            private Coroutine prevFollowCoroutine;
        }

        [Tooltip("Draw debug gizmos? Pathfinding target is magenta, attack target is yellow, current waypoint is cyan")]
        [SerializeField] private bool drawGizmos;
        
        // The current waypoint we are pathing to (updated by pathing scriptable objects). only used to draw debug gizmos
        [HideInInspector] public Vector2 currentWaypoint = Vector2.zero;
        
        /// Allows the state machine to track other variables that may not be shared between enemy types (ie floor bosses)
        public Dictionary<string, object> trackedVariables = new Dictionary<string, object>();
        
        // Tracks whether this is the first time this object has been started (needed to make sure we call OnStateEnter AFTER the initial logic delay)
        private bool firstTimeStarted = true;

        /// <summary>
        /// Initialize variables
        /// </summary>
        private void Awake()
        {
            currentState = initialState;
            cooldownData.cooldownReady = new Dictionary<BaseAction, bool>();
            cachedComponents = new Dictionary<Type, Component>();
            currentMovementType = startingMovementType;
        }

        /// <summary>
        /// Gets the position of the feet collider
        /// </summary>
        /// <returns> The position of the feet collider. </returns>
        public Vector2 GetFeetPos()
        {
            Vector2 offset = feetCollider.offset;
            Vector2 position = feetCollider.transform.position;
            return new Vector2(position.x + offset.x, 
                position.y + offset.y);
        }
        
        
        /// <summary>
        /// Grab the player gameobject and sets it to the default target
        /// </summary>
        private void Start()
        {
            SetStats();
            timeStarted = Time.time;
            FloorGenerator.floorGeneratorInstance.currentRoom.livingEnemies.Add(gameObject);
        }

        /// <summary>
        /// Sets base stats of this enemy according to the stats manager
        /// </summary>
        void SetStats()
        {
            // assign health
            var startingHealth = Mathf.RoundToInt(GetComponent<Health>().maxHealth * DifficultyProgressionManager.healthMultiplier);
            GetComponent<Health>().maxHealth = startingHealth;
            GetComponent<Health>().currentHealth = startingHealth;
            
            // assign speed
            GetComponent<SimpleMovement>().maxSpeed *= DifficultyProgressionManager.moveSpeedMultiplier;
            
            // assign damage on touch
            feetCollider.GetComponent<DamageOnInteract>().damageData.damage *= Mathf.RoundToInt(DifficultyProgressionManager.onTouchDamageMultiplier);
        }

        /// <summary>
        /// Execute the current state
        /// </summary>
        private void Update()
        {
            if (exhausted || Time.time - timeStarted <= delayBeforeLogic)
            {
                GetComponent<Movement>().movementInput = Vector2.zero;
                return;
            }

            if (firstTimeStarted)
            {
                firstTimeStarted = false;
                currentState.OnStateEnter(this);
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
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(currentPathfindingTarget, Vector3.one);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(currentAttackTarget, Vector3.one);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(currentWaypoint, Vector3.one);
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
        
        /// <summary>
        /// Get the audio source component from the object. 
        /// </summary>
        /// <returns>Returns the relevant audio source</returns>
        public AudioSource GetAudioSource()
        {
            return GetComponent<AudioSource>(); 
        }

        /// <summary>
        /// Gets the damage multiplier of this actor
        /// </summary>
        /// <returns> The damage multiplier. </returns>
        public float GetDamageMultiplier()
        {
            return DifficultyProgressionManager.projectileDamageMultiplier;
        }

        #endregion
    }
}