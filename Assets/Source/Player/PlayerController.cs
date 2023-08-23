using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Cardificer
{
    /// <summary>
    /// Agent serves as the brain of any agent. Has the ability to input basic tasks, delegating them to various parts of the agent as needed.
    /// </summary>
    [RequireComponent(typeof(Movement), typeof(AnimatorController), typeof(DashAbility))]
    public class PlayerController : MonoBehaviour, IActor
    {
        [Tooltip("The amount that the aim direction angle has to change before it counts as an input")]
        [SerializeField] private float aimAngleChangeThreshold = 1;

        [Tooltip("The magnitude of the aim direction that the gamepad must input before it counts as aiming")]
        [SerializeField] private float aimMagnitudeThreshold = 0.5f;

        // Damage multiplier of this actor
        [HideInInspector] public float damageMultiplier = 1f;

        // Tracks whether moving and playing cards is enabled
        private bool _movingEnabled = true;
        public bool movingEnabled
        {
            get => _movingEnabled;
            set
            {
                _movingEnabled = value;
                if (!_movingEnabled)
                {
                    movementComponent.movementInput = new Vector2(0, 0);
                }
            }
        }

        // Tracks whether the player should be paused
        [HideInInspector] public bool paused => Time.timeScale == 0;

        // Tracks the controller aim direction
        [System.NonSerialized] public Vector2 aimDirection;

        // Tracks whether the last input was a gamepad input
        [System.NonSerialized] public bool lastInputWasGamepad = false;

        // Movement component to allow the agent to move
        private Movement movementComponent;

        // The attempted movement input
        private Vector2 attemptedMovementInput;

        // Animator component to make the pretty animations do their thing.
        private AnimatorController animatorComponent;

        // The component responsible for the dashing ability
        private DashAbility dashAbility;

        // The amount of time spent in the current run.
        public float playtime { private set; get; } = 0f;

        /// <summary>
        /// Initialize components.
        /// </summary>
        private void Awake()
        {
            movementComponent = GetComponent<Movement>();
            animatorComponent = GetComponent<AnimatorController>();
            dashAbility = GetComponent<DashAbility>();
            dashAbility.onDashBegin += OnDashBegin;
            dashAbility.onDashEnd += OnDashEnd;

            GetComponent<PlayerInput>().uiInputModule = MenuManager.uiInputModule;
        }

        /// <summary>
        /// Load autosave.
        /// </summary>
        private void Start()
        {
            if (!SaveManager.autosaveExists) 
            {
                MenuManager.Open<DraftMenu>(lockOpen: true);
                return; 
            }

            if (!Player.SetMoney(SaveManager.savedPlayerMoney))
            {
                SaveManager.AutosaveCorrupted("Invalid player money");
                return;
            }

            playtime = (float)SaveManager.savedPlaytime.TotalSeconds;

            transform.position = SaveManager.savedPlayerPosition;
            // TODO: There is a small probability that the player position is invalid and is not caught by the default save file corruption detection.

            damageMultiplier = SaveManager.savedPlayerDamage;
            (movementComponent as SimpleMovement).maxSpeed = SaveManager.savedPlayerSpeed;

            Health health = GetComponent<Health>();
            health.maxHealth = SaveManager.savedPlayerMaxHealth;
            health.currentHealth = SaveManager.savedPlayerHealth;
            if (health.currentHealth > health.maxHealth || health.currentHealth <= 0 || health.maxHealth <= 0)
            {
                SaveManager.AutosaveCorrupted("Invalid player health of " + health.currentHealth);
                return;
            }           
        }

        /// <summary>
        /// Retrieve inputs where necessary and perform actions as needed
        /// </summary>
        private void Update()
        {
            playtime += Time.deltaTime;

            if (canAct && !paused)
            {
                animatorComponent.SetMirror("castLeft", GetActionAimPosition().x - transform.position.x < 0);
            }

            if (movingEnabled && !paused)
            {
                movementComponent.movementInput = attemptedMovementInput;
                return;
            }
        }

        /// <summary>
        /// Moves the player 
        /// </summary>
        /// <param name="moveInput"> The move input </param>
        private void OnMove(InputValue moveInput)
        {
            lastInputWasGamepad = false;
            attemptedMovementInput = moveInput.Get<Vector2>().normalized;
        }

        /// <summary>
        /// Moves the player, but from a gamepad
        /// </summary>
        /// <param name="moveInput"> The move input </param>
        private void OnMoveGamepad(InputValue moveInput)
        {
            lastInputWasGamepad = true;
            attemptedMovementInput = moveInput.Get<Vector2>().normalized;
        }

        /// <summary>
        /// Handles the aim input from the controller
        /// </summary>
        /// <param name="aimInput"> The aim input </param>
        private void OnAimGamepad(InputValue aimInput)
        {
            Vector2 newAimInput = aimInput.Get<Vector2>();

            if (newAimInput.sqrMagnitude < aimMagnitudeThreshold * aimMagnitudeThreshold) { return; }

            newAimInput.Normalize();
            if (aimDirection != Vector2.zero && Vector2.Angle(newAimInput, aimDirection) < aimAngleChangeThreshold) { return; }

            lastInputWasGamepad = true;
            aimDirection = aimInput.Get<Vector2>().normalized;
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard1()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(0);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard1Gamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(0);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard2()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(1);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard2Gamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(1);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard3()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(2);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard3Gamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(2);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard4()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(3);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard4Gamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(3);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard5()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(4);
            }
        }

        /// <summary>
        /// Previews a card
        /// </summary>
        private void OnPreviewCard5Gamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                Deck.playerDeck.SelectCard(4);
            }
        }

        /// <summary>
        /// Cast the selected cards
        /// </summary>
        private void OnCast()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && canAct && !paused)
            {
                if (Deck.playerDeck.PlayChord())
                {
                    animatorComponent.SetTrigger("cast");
                    animatorComponent.SetMirror("idleLeft", GetActionAimPosition().x - transform.position.x < 0);
                    animatorComponent.SetMirror("runLeft", GetActionAimPosition().x - transform.position.x < 0);
                }
            }
        }

        /// <summary>
        /// Cast the selected cards, but using a gamepad input
        /// </summary>
        private void OnCastGamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                if (Deck.playerDeck.PlayChord())
                {
                    animatorComponent.SetTrigger("cast");
                    animatorComponent.SetMirror("idleLeft", GetActionAimPosition().x - transform.position.x < 0);
                    animatorComponent.SetMirror("runLeft", GetActionAimPosition().x - transform.position.x < 0);
                }
            }
        }

        /// <summary>
        /// Dashing
        /// </summary>
        private void OnDash()
        {
            lastInputWasGamepad = false;
            if (movingEnabled && !paused)
            {
                dashAbility.StartDash(attemptedMovementInput, Deck.playerDeck);
            }
        }

        /// <summary>
        /// Dashing
        /// </summary>
        private void OnDashGamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && !paused)
            {
                dashAbility.StartDash(attemptedMovementInput, Deck.playerDeck);
            }
        }

        /// <summary>
        /// Handles beginning the dash
        /// </summary>
        private void OnDashBegin()
        {
            movingEnabled = false;
        }

        /// <summary>
        /// Handles ending the dash
        /// </summary>
        private void OnDashEnd()
        {
            movingEnabled = true;
        }

        /// <summary>
        /// Pauses the game
        /// </summary>
        private void OnPause()
        {
            lastInputWasGamepad = false;
            MenuManager.Toggle<PauseMenu>();
        }

        /// <summary>
        /// Pauses the game
        /// </summary>
        private void OnPauseGamepad()
        {
            lastInputWasGamepad = true;
            MenuManager.Toggle<PauseMenu>();
        }

        /// <summary>
        /// Opens the map
        /// </summary>
        private void OnOpenMap()
        {
            lastInputWasGamepad = false;
            if (MenuManager.IsMenuOpen(typeof(PauseMenu))) { return; }
            MenuManager.Toggle<MapMenu>();
        }

        /// <summary>
        /// Opens the map
        /// </summary>
        private void OnOpenMapGamepad()
        {
            lastInputWasGamepad = true;
            if (MenuManager.IsMenuOpen(typeof(PauseMenu))) { return; }
            MenuManager.Toggle<MapMenu>();
        }

        /// <summary>
        /// Opens the card menu
        /// </summary>
        private void OnOpenCardMenu()
        {
            lastInputWasGamepad = false;
            if (MenuManager.IsMenuOpen(typeof(PauseMenu))) { return; }
            MenuManager.Toggle<CardMenu>();
        }

        /// <summary>
        /// Opens the card menu
        /// </summary>
        private void OnOpenCardMenuGamepad()
        {
            lastInputWasGamepad = true;
            if (MenuManager.IsMenuOpen(typeof(PauseMenu))) { return; }
            MenuManager.Toggle<CardMenu>();
        }

        /// <summary>
        /// Unselects all spells
        /// </summary>
        private void OnUnselectSpellsGamepad()
        {
            lastInputWasGamepad = true;
            if (movingEnabled && canAct && !paused)
            {
                List<int> selectedCards = Deck.playerDeck.previewedCardIndices;
                foreach (int selectedCard in selectedCards)
                {
                    Deck.playerDeck.SelectCard(4);
                }
            }
        }

        /// <summary>
        /// Shows the floor layout
        /// </summary>
        private void OnShowLayout()
        {
            lastInputWasGamepad = false;
            // Make sure this can only happen when testing in the editor
            #if UNITY_EDITOR
            FloorGenerator.ShowLayout();
            #endif
        }

        /// <summary>
        /// Prints the name of the template of the current room
        /// </summary>
        private void OnPrintCurrentRoomTemplate()
        {
            // Make sure this only happens when testing in the editor
            #if UNITY_EDITOR
            Debug.Log("Current room type: " + FloorGenerator.currentRoom.roomType.displayName);
            Debug.Log("Current room template: " + FloorGenerator.currentRoom.template);
            #endif
        }

        /// <summary>
        /// Handles the player being destroyed
        /// </summary>
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
            if (lastInputWasGamepad)
            {
                return (Vector3) aimDirection + transform.position;
            }
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