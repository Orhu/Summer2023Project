using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// An action modifier that changes the attack of an action modifier.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPlayAction", menuName = "Cards/AttackModifers/PlayAction")]
    public class PlayAction : AttackModifier, IActor
    {
        [Tooltip("The action to play.")]
        [SerializeField] private Action action;

        [Tooltip("The modifier types to not apply to this.")]
        [SerializeField] private ModifierFilter filter;

        [Tooltip("The delay before the action is taken")] [Min(0f)]
        [SerializeField] private float delay = 0f;

        [Tooltip("When the action is played")]
        [SerializeField] private PlayTime playTime;
        private enum PlayTime
        {
            OnSpawned,
            OnHit,
            OnOverlap,
            OnDestroyed,
            Repeatedly,
        }

        [Tooltip("The number of times this can play an action.")] [Min(1)]
        [SerializeField] private int playCount = 1000;

        [Tooltip("Causes actions played by this to have the same modifiers as the projectile this modifies. Will exclude any play action modifiers")]
        [SerializeField] private bool inheritModifiers = false;

        [Tooltip("Whether or not this will play when on 0 damage projectiles")]
        [SerializeField] private bool applyToZeroDamage = false;

        [Tooltip("Whether or not this will be the actor used as the source for playing actions (Does not affect attacks)")]
        [SerializeField] private bool useThisAsActor = false;

        // The damage multiplier of this action
        private float damageMultiplier = 1f;

        // The objects ignored by this.
        private List<GameObject> ignoredObjects;

        // The source of the projectile.
        private Transform sourceTransform;

        // Parent of the projectile to spawn.
        private IActor parentActor;

        // The causer of the damage.
        private GameObject causer;

        // The objects ignored by this.
        protected List<AttackModifier> modifiers = new List<AttackModifier>();

        // The root of all projectiles
        private static GameObject playActionRoot;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile value)
        {
            if (!applyToZeroDamage && value.attackData.damage == 0) { return; }
            if (playActionRoot == null)
            {
                playActionRoot = new GameObject("Play Action Roots");
            }

            causer = value.causer;
            parentActor = value.actor;
            sourceTransform = value.transform;

            if (inheritModifiers)
            {
                if (filter == null)
                {
                    modifiers = new List<AttackModifier>(value.modifiers);
                }
                else
                {
                    modifiers = filter.FilterModifierList(value.modifiers);
                }
            }

            int playCount = this.playCount;

            switch (playTime)
            {
                case PlayTime.Repeatedly:
                case PlayTime.OnSpawned:
                    ignoredObjects = value.ignoredObjects;
                    value.StartCoroutine(DelayedPlayAction(playCount));
                    break;

                case PlayTime.OnHit:
                    value.onHit += collision =>
                    {
                        ignoredObjects = new List<GameObject>(value.ignoredObjects);
                        ignoredObjects.Add(collision.gameObject);
                        if (--playCount < 0) { return; }
                        value.StartCoroutine(DelayedPlayAction());
                    };
                    break;

                case PlayTime.OnOverlap:
                    value.onOverlap += hitCollider =>
                    {
                        ignoredObjects = new List<GameObject>(value.ignoredObjects);
                        ignoredObjects.Add(hitCollider.gameObject);
                        if (--playCount < 0) { return; }
                        value.StartCoroutine(DelayedPlayAction());
                    };
                    break;

                case PlayTime.OnDestroyed:
                    value.onDestroyed +=
                        // Creates a new game object to act as the source of the played action
                        () =>
                        {
                            if (value.forceDestroy) { return; }

                            // Create runner object since the projectile will be null.
                            GameObject coroutineRunner = new GameObject(value.name + " Play " + action.name + " Source");
                            coroutineRunner.transform.position = sourceTransform.position;
                            coroutineRunner.transform.rotation = sourceTransform.rotation;
                            coroutineRunner.transform.parent = playActionRoot.transform;
                            sourceTransform = coroutineRunner.transform;
                            MonoBehaviour mono = sourceTransform.gameObject.AddComponent<Empty>();
                            FloorGenerator.onRoomChange += () => { Destroy(coroutineRunner); };

                            ignoredObjects = new List<GameObject>(value.ignoredObjects);
                            mono.GetComponent<MonoBehaviour>().StartCoroutine(DelayedPlayAction());
                        };
                    break;
            }
        }

        /// <summary>
        /// Plays the action.
        /// </summary>
        /// <returns> The time to wait to play it. </returns>
        private IEnumerator DelayedPlayAction(int playCount = 1)
        {
            do
            {
                if (--playCount < 0) { yield break; }
                if (delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }

                if (action is Attack attack)
                {
                    attack.Play(this, modifiers, causer, ignoredObjects: ignoredObjects);
                }
                else
                {
                    action.Play(useThisAsActor ? this :parentActor, ignoredObjects);
                }

                yield return null;
            } while (playTime == PlayTime.Repeatedly);
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
                return shouldAct;
            }
        }

        /// <summary>
        /// Get the transform that the action should be played from.
        /// </summary>
        /// <returns> The actor transform. </returns>
        public Transform GetActionSourceTransform()
        {
            return sourceTransform;
        }


        /// <summary>
        /// Get the position that the action should be aimed at.
        /// </summary>
        /// <returns> The mouse position in world space. </returns>
        public Vector3 GetActionAimPosition()
        {
            return parentActor.GetActionAimPosition();
        }


        /// <summary>
        /// Gets the collider of this actor.
        /// </summary>
        /// <returns> The collider. </returns>
        public Collider2D GetCollider()
        {
            return sourceTransform.GetComponent<Collider2D>();
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
        /// Get the attached audiosource component. 
        /// </summary>
        /// <returns> The audiosource. </returns>
        public AudioSource GetAudioSource()
        {
            return sourceTransform.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Gets the damage multiplier of this actor
        /// </summary>
        /// <returns> The damage multiplier. </returns>
        public float GetDamageMultiplier()
        {
            return damageMultiplier;
        }

        #endregion

        /// <summary>
        /// Empty MonoComponent class since MonoComponent can't be instantiated.
        /// </summary>
        private class Empty : MonoBehaviour { }
    }
}