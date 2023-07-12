using System.Collections;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Plays any action on a regular interval.
    /// </summary>
    public class Turret : MonoBehaviour, IActor
    {
        [Tooltip("The action to play.")]
        public Action action;

        [Tooltip("The time between playing the action.")]
        public float playRate = 1f;

        [Tooltip("Whether or not to keep acting after all enemies have been killed.")]
        public bool playAfterClear = false;

        [Tooltip("Whether or not to keep acting after all enemies have been killed.")]
        public bool fireImmediately = false;

        [Tooltip("When to play the action")]
        [SerializeField] private PlayTime playTime;
        private enum PlayTime
        {
            AlwaysShooting,
            WhenOverlapping
        }
        
        // The coroutine responsible for playing actions.
        private Coroutine coroutine;


        /// <summary>
        /// Initializes timer.
        /// </summary>
        private void Start()
        {
            if (playTime == PlayTime.AlwaysShooting)
            {
                coroutine = StartCoroutine(PlayAction());
            }

            if (!playAfterClear)
            {
                FloorGenerator.currentRoom.onCleared += StopAllCoroutines;
            }
        }

        /// <summary>
        /// Plays the action repeatedly.
        /// </summary>
        /// <returns> The time to wait for the next action. </returns>
        private IEnumerator PlayAction()
        {
            if (!fireImmediately)
            {
                yield return new WaitForSeconds(playRate);
            }

            while (true)
            {
                if (CanAct)
                {
                    action.Play(this);
                }
                yield return new WaitForSeconds(playRate);
            }
        }

        /// <summary>
        /// Starts acting if set to act when overlapping.
        /// </summary>
        /// <param name="collision"> The thing that was overlapped. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playTime == PlayTime.WhenOverlapping && collision.CompareTag("Player") && !collision.isTrigger)
            {
                coroutine = StartCoroutine(PlayAction());
            }
        }

        /// <summary>
        /// Starts acting if set to act when overlapping.
        /// </summary>
        /// <param name="collision"> The thing that was overlapped. </param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (playTime == PlayTime.WhenOverlapping && collision.CompareTag("Player") && !collision.isTrigger)
            {
                StopAllCoroutines();
                coroutine = null;
            }
        }

        #region IActor Implementation
        /// <summary>
        /// Get the transform that the action should be played from.
        /// </summary>
        /// <returns> The transform. </returns>
        public Transform GetActionSourceTransform()
        {
            return transform;
        }

        /// <summary>
        /// Get the position that the action should be aimed at.
        /// </summary>
        /// <returns> The position in world space. </returns>
        public Vector3 GetActionAimPosition()
        {
            return transform.position + transform.right;
        }

        /// <summary>
        /// Gets the collider of this actor.
        /// </summary>
        /// <returns> The collider. </returns>
        public Collider2D GetCollider()
        {
            return GetComponent<Collider2D>();
        }

        // Whether or not this can act.
        bool CanAct
        {
            get
            {
                bool shouldAct = true;
                canAct?.Invoke(ref shouldAct);
                return shouldAct;
            }
        }
        IActor.CanActRequest canAct;

        /// <summary>
        /// Gets the delegate that will fetch whether this actor can act.
        /// </summary>
        /// <returns> A delegate with a out parameter, that allows any subscribed objects to determine whether or not this actor can act. </returns>
        public ref IActor.CanActRequest GetOnRequestCanAct() { return ref canAct; }

        /// <summary>
        /// Gets the AudioSource on the object. 
        /// </summary>
        /// <returns></returns>
        public AudioSource GetAudioSource()
        {
            return GetComponent<AudioSource>(); 
        }

        /// <summary>
        /// Gets the damage multiplier of this actor.
        /// </summary>
        /// <returns> The damage multiplier. </returns>
        public float GetDamageMultiplier()
        {
            return DifficultyProgressionManager.turretDamageMultiplier;
        }

        #endregion
    }
}