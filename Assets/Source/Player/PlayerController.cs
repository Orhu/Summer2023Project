using UnityEngine;
using UnityEngine.InputSystem;

namespace Cardificer
{
    /// <summary>
    /// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
    /// </summary>
    [RequireComponent(typeof(Movement), typeof(AnimatorController), typeof(ChannelAbility))]
    public class PlayerController : MonoBehaviour, IActor
    {
        // Damage multiplier of this actor
        [HideInInspector] public float damageMultiplier = 1f;

        // Tracks whether moving and playing cards is enabled
        private bool _movingEnabled = true;
        public bool movingEnabled
        {
            get => _movingEnabled;
            set
            {
                _movingEnabled = false;
                if (!_movingEnabled)
                {
                    movementComponent.movementInput = new Vector2(0, 0);
                }
            }
        }

        // Delegate called when the map is opened (@ALEX TODO: Delete this delegate when you make your map screen)
        public System.Action mapOpened;

        // Delegate called when the map is closed (@ALEX TODO: Delete this delegate when you make your map screen)
        public System.Action mapClosed;

        // Boolean tracking whether the map is open (@ALEX TODO: Delete this boolean when you make your map screen)
        private bool mapOpen = false;
        
        // Movement component to allow the agent to move
        private Movement movementComponent;

        // The attempted movement input
        private Vector2 attemptedMovementInput;

        // Animator component to make the pretty animations do their thing.
        private AnimatorController animatorComponent;

        // The component responsible for the channeling ability
        private ChannelAbility channelAbility;

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void Awake()
        {
            movementComponent = GetComponent<Movement>();
            animatorComponent = GetComponent<AnimatorController>();
            channelAbility = GetComponent<ChannelAbility>();

            Random.state = SaveManager.savedRandomState;
        }

        /// <summary>
        /// Load autosave.
        /// </summary>
        private void Start()
        {
            if (!SaveManager.autosaveExists) { return; }

            if (!Player.SetMoney(SaveManager.savedPlayerMoney))
            {
                SaveManager.AutosaveCorrupted("Invalid player money");
                return;
            }

            transform.position = SaveManager.savedPlayerPosition;
            // TODO: There is a small probability that the player position is invalid and is not caught by the default save file corruption detection.

            damageMultiplier = SaveManager.savedPlayerDamage;
            (movementComponent as SimpleMovement).maxSpeed = SaveManager.savedPlayerSpeed;

            Health health = GetComponent<Health>();
            health.maxHealth = SaveManager.savedPlayerMaxHealth;
            health.currentHealth = SaveManager.savedPlayerHealth;
            if (health.currentHealth > health.maxHealth || health.currentHealth <= 0 || health.maxHealth <= 0)
            {
                SaveManager.AutosaveCorrupted("Invalid player health");
                return;
            }           
        }

        /// <summary>
        /// Retrieve inputs where necessary and perform actions as needed
        /// </summary>
        private void Update()
        {
            if (canAct)
            {
                animatorComponent.SetMirror("castLeft", GetActionAimPosition().x - transform.position.x < 0);
            }

            if (movingEnabled)
            {
                movementComponent.movementInput = attemptedMovementInput;
                return;
            }
        }

        /// <summary>
        /// Moves the player 
        /// </summary>
        /// <param name="moveInput"> The move input </param>
        public void OnMove(InputValue moveInput)
        {
            attemptedMovementInput = moveInput.Get<Vector2>().normalized;
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        public void OnPreviewCard1()
        {
            if (movingEnabled && canAct)
            {
                Deck.playerDeck.SelectCard(0);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        public void OnPreviewCard2()
        {
            if (movingEnabled && canAct)
            {
                Deck.playerDeck.SelectCard(1);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        public void OnPreviewCard3()
        {
            if (movingEnabled && canAct)
            {
                Deck.playerDeck.SelectCard(2);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        public void OnPreviewCard4()
        {
            if (movingEnabled && canAct)
            {
                Deck.playerDeck.SelectCard(3);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        public void OnPreviewCard5()
        {
            if (movingEnabled && canAct)
            {
                Deck.playerDeck.SelectCard(4);
            }
        }

        /// <summary>
        /// Cast the selected cards
        /// </summary>
        public void OnCast()
        {
            if (movingEnabled && canAct)
            {
                if (Deck.playerDeck.PlayChord())
                {
                    animatorComponent.SetTrigger("cast");
                    animatorComponent.SetMirror("idleLeft", GetActionAimPosition().x - transform.position.x < 0);
                    animatorComponent.SetMirror("runLeft", GetActionAimPosition().x - transform.position.x < 0);
                    channelAbility.StopChanneling();
                }
            }
        }

        /// <summary>
        /// Channeling
        /// </summary>
        /// <param name="input"> The input </param>
        public void OnChannel(InputValue input)
        {
            if (movingEnabled && canAct)
            {
                if (input.isPressed)
                {
                    channelAbility.StartChanneling();
                }
                else
                {
                    channelAbility.StopChanneling();
                }
            }
        }

        /// <summary>
        /// Pauses the game
        /// </summary>
        public void OnPause()
        {
            MenuManager.TogglePause();
        }

        /// <summary>
        /// Opens the map
        /// </summary>
        public void OnOpenMap()
        {
            // @ALEX TODO: Delete the boolean and delegates (see above for which ones to delete) when you make your map screen. Also, in LockCameraToRoom, delete the TODO: Delete.
            mapOpen = !mapOpen;
            if (mapOpen)
            {
                mapOpened?.Invoke();
            }
            else
            {
                mapClosed?.Invoke();
            }
        }

        /// <summary>
        /// Opens the card menu
        /// </summary>
        public void OnOpenCardMenu()
        {
            MenuManager.ToggleCardMenu();
        }

        /// <summary>
        /// Shows the floor layout
        /// </summary>
        public void OnShowLayout()
        {
            // Make sure this can only happen when testing in the editor
            #if UNITY_EDITOR
            FloorGenerator.ShowLayout();
            #endif
        }

        private void OnDestroy()
        {
            Player.SetMoney(0);
        }
        #region IActor Implementation

        // Gets whether or not this actor can act.
        IActor.CanActRequest _canAct;
        bool canAct
        {
            get
            {
                bool shouldAct = true;
                _canAct?.Invoke(ref shouldAct);
                return shouldAct && !Deck.playerDeck.isActing;
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
            return Vector3.Scale(Camera.main.ScreenToWorldPoint(Mouse.current.position.value), new Vector3(1, 1, 0));
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
        /// Returns AudioSource component of this actor
        /// </summary>
        /// <returns> The AudioSource. </returns>
        public AudioSource GetAudioSource()
        {
            return GetComponent<AudioSource>(); 
        }

        /// <summary>
        /// Gets damage multiplier of this actor
        /// </summary>
        /// <returns> The damage multiplier. </returns>
        public float GetDamageMultiplier()
        {
            return damageMultiplier;
        }

        #endregion
    }
}