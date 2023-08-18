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

        // The modifiers that are applied to the action.
        protected List<AttackModifier> modifiers = new List<AttackModifier>();

        // The objects ignored by this.
        private List<GameObject> ignoredObjects;

        // The number of active attacks spawned by this.
        private int numActiveAttacks = 0;

        // Whether or not this allows currently the destruction of the projectile this is attached to.
        public override bool allowDestruction { get => numActiveAttacks == 0; }

        // The projectile this is attached to.
        private Projectile projectile;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile attachedProjectile)
        {
            if (!applyToZeroDamage && attachedProjectile.attackData.damage == 0) { return; }

            if (inheritModifiers)
            {
                if (filter == null)
                {
                    modifiers = new List<AttackModifier>(attachedProjectile.modifiers);
                }
                else
                {
                    modifiers = filter.FilterModifierList(attachedProjectile.modifiers);
                }
            }

            projectile = attachedProjectile;
            int playCount = this.playCount;

            switch (playTime)
            {
                case PlayTime.Repeatedly:
                case PlayTime.OnSpawned:
                    ignoredObjects = new List<GameObject>(attachedProjectile.ignoredObjects);
                    attachedProjectile.StartCoroutine(DelayedPlayAction(playCount));
                    break;

                case PlayTime.OnHit:
                    attachedProjectile.onHit += collision =>
                    {
                        ignoredObjects = new List<GameObject>(attachedProjectile.ignoredObjects);
                        ignoredObjects.Add(collision.gameObject);
                        if (--playCount < 0) { return; }
                        attachedProjectile.StartCoroutine(DelayedPlayAction());
                    };
                    break;

                case PlayTime.OnOverlap:
                    attachedProjectile.onOverlap += hitCollider =>
                    {
                        ignoredObjects = new List<GameObject>(attachedProjectile.ignoredObjects);
                        ignoredObjects.Add(hitCollider.gameObject);
                        if (--playCount < 0) { return; }
                        attachedProjectile.StartCoroutine(DelayedPlayAction());
                    };
                    break;

                case PlayTime.OnDestroyed:
                    attachedProjectile.onDestroyed +=
                        // Creates a new game object to act as the source of the played action
                        () =>
                        {
                            if (attachedProjectile.forceDestroy) { return; }
                            ignoredObjects = new List<GameObject>(attachedProjectile.ignoredObjects);
                            attachedProjectile.StartCoroutine(DelayedPlayAction());
                        };
                    break;
            }

            /// <summary>
            /// Plays the action.
            /// </summary>
            /// <returns> The time to wait to play it. </returns>
            IEnumerator DelayedPlayAction(int playCount = 1)
            {
                do
                {
                    if (--playCount < 0) { yield break; }
                    if (delay > 0)
                    {
                        yield return new WaitForSeconds(delay);
                    }

                    if (playTime == PlayTime.Repeatedly && projectile.isDestroyed) { yield break; }
                    if (action is Attack attack)
                    {
                        numActiveAttacks++;
                        attack.Play(this, modifiers, attachedProjectile.causer, ignoredObjects: ignoredObjects,
                            attackFinished: () =>
                            {
                                numActiveAttacks--;
                            });
                    }
                    else
                    {
                        action.Play(useThisAsActor ? this : attachedProjectile.actor, ignoredObjects);
                    }

                    yield return null;
                } while (playTime == PlayTime.Repeatedly);
            }
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
            return projectile.transform;
        }


        /// <summary>
        /// Get the position that the action should be aimed at.
        /// </summary>
        /// <returns> The mouse position in world space. </returns>
        public Vector3 GetActionAimPosition()
        {
            return projectile.actor.GetActionAimPosition();
        }


        /// <summary>
        /// Gets the collider of this actor.
        /// </summary>
        /// <returns> The collider. </returns>
        public Collider2D GetCollider()
        {
            return projectile.GetComponent<Collider2D>();
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
            return projectile.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Gets the damage multiplier of this actor
        /// </summary>
        /// <returns> The damage multiplier. </returns>
        public float GetDamageMultiplier()
        {
            return projectile.actor.GetDamageMultiplier();
        }

        #endregion
    }
}