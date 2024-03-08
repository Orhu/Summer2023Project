using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace Cardificer
{
    /// <summary>
    /// Add this component to make an object damageable.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Tooltip("The Max health of this object")] [Min(1)]
        [SerializeField] private int _maxHealth = 5;
        public int maxHealth
        {
            set
            {
                if (_maxHealth == value) { return; }

                _maxHealth = value;
            }
            get => _maxHealth;
        }
        
        // The current health of this object
        private int _currentHealth = 0;
        public int currentHealth
        {
            get => _currentHealth;
            set
            {
                if (_currentHealth == value) { return; }

                _currentHealth = value;
                if (hasBossHealthbar)
                {
                    BossHealthbarManager.instance.UpdateHealth(value);
                }
            }
        }

        [Tooltip("How long of a duration does this unit get invincibility when hit?")]
        public float invincibilityDuration = 0.25f;

        [Tooltip("All status effects this is immune to.")]
        public List<StatusEffect> immuneStatusEffects = new List<StatusEffect>();

        [Tooltip("The prefab to spawn to indicate damage numbers.")]
        public DamageNumber damageNumberPrefab;

        [Tooltip("Called when this dies")]
        public UnityEvent onDeath;
        
        [Tooltip("Indicates whether this enemy has an associated boss health bar")]
        [SerializeField] private bool hasBossHealthbar = false;

        //for "damaged version" of bosses
        [Tooltip("Indicates whether we should trigger a hurt animation on half health")]
        [SerializeField] private bool animateWhenHurt = false;
        [SerializeField] private string hurtAnimatorObjName;
        private Animator hurtAnimator;

        // Called when invincibility changes and passes the new invincibility
        public Action<bool> onInvincibilityChanged;

        // Called before this processes an attack and passes the incoming attack so can be modified.   
        public RequestIncomingAttackModification onRequestIncomingAttackModification;
        public delegate void RequestIncomingAttackModification(ref DamageData attack);
        
        // Called when damage is taken.
        public System.Action onDamageTaken;

        // is this unit currently invincible?
        private bool _invincible = false;
        private bool invincible
        {
            set
            {
                CancelInvoke(nameof(TurnOffInvincibility));
                if (value)
                {
                    Invoke(nameof(TurnOffInvincibility), invincibilityDuration);
                }

                _invincible = value;
                onInvincibilityChanged?.Invoke(value);
            }
            get { return _invincible; }
        }

        // All status effects currently affecting this
        private List<StatusEffect> statusEffects = new List<StatusEffect>();

        [Tooltip("Sound played on death")]
        public SoundContainer deathSounds;
        [Tooltip("Sound played when damaged")]
        public SoundContainer hitSounds;
        [Tooltip("Sound played when cleansed")]
        public BasicSound cleanseSound;

        private

        /// <summary>
        /// Initializes current health and health sounds.
        /// </summary>
        void Start()
        {
            if (hasBossHealthbar)
            {
                BossHealthbarManager.instance.StartHealthbar(maxHealth);
            }

            //set default settings for Health Sounds for ease of implementation. If these need to change this can happen in the future!
            deathSounds.containerType = SoundContainerType.RandomRandom;
            hitSounds.containerType = SoundContainerType.RandomRandom;

            if (!deathSounds.IsValid())
            {
                deathSounds.clipsInContainer = new AudioClip[] { SoundGetter.Instance.defaultDeathAudioClip };
                deathSounds.soundSettings.volume = 1.2f;
                deathSounds.soundSettings.loop = false;
                if(AudioManager.instance.printDebugMessages) Debug.Log("No death sound set on " + gameObject.name + ". Setting deathsound to Default Death Sound.", gameObject);
            }

            if (!hitSounds.IsValid())
            {
                hitSounds.clipsInContainer = new AudioClip[] { SoundGetter.Instance.defaultHitAudioClip };
                hitSounds.soundSettings.volume = 1.2f;
                hitSounds.soundSettings.loop = false;
                if (AudioManager.instance.printDebugMessages) Debug.Log("No hit sounds set on " + gameObject.name + ". Setting deathsound to hit Death Sound.", gameObject);
            }

            hitSounds.useDefaultSettings = false;
            hitSounds.soundSettings.randomizePitch = true;
            hitSounds.soundSettings.bufferTime = 0.1f;

            if (gameObject.tag != "Player")
            {
                deathSounds.outputAudioMixerGroup = SoundGetter.Instance.enemyAudioMixerGroup;
                hitSounds.outputAudioMixerGroup = SoundGetter.Instance.enemyAudioMixerGroup;
            }

            if (currentHealth != 0) { return; }
            currentHealth = maxHealth;

        }

        /// <summary>
        /// Update status effects and prunes null values
        /// </summary>
        void Update()
        {
            for (int i = 0; i < statusEffects.Count; i++)
            {
                statusEffects[i]?.Update();
                if (statusEffects[i] == null)
                {
                    statusEffects.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }

        /// <summary>
        /// Receive an attack and kill the owner if out of health.
        /// </summary>
        /// <param name="attack"> The attack being received. </param>
        /// <param name="ignoreInvincibility"> Whether or not invincibility frames should effect this attack. </param>
        public void ReceiveAttack(DamageData attack, bool ignoreInvincibility = false)
        {
            if (currentHealth <= 0 || (invincible && !ignoreInvincibility)) { return; }

            // Damage
            onRequestIncomingAttackModification?.Invoke(ref attack);
            var prevHealth = currentHealth;
            currentHealth = Mathf.Clamp(currentHealth - attack.damage, 0, maxHealth);

            if (attack.damage > 0)
            {
                onDamageTaken?.Invoke();

                if (animateWhenHurt && currentHealth <= (maxHealth/2))
                {
                    //TEMP: this is expensive
                    if (hurtAnimator == null) { hurtAnimator = GameObject.Find(hurtAnimatorObjName).GetComponent<Animator>(); }
                    hurtAnimator.SetTrigger("hurt");
                }

                AudioManager.instance.PlaySoundBaseOnTarget(hitSounds, transform, true);
            }


            if (!attack.dontShowDamageNumber && ((attack.damage != 0 || attack.statusEffects.Count > 0) && damageNumberPrefab != null))
            {
                GameObject damageNumber = Instantiate(damageNumberPrefab.gameObject);
                damageNumber.GetComponent<DamageNumber>().damageData = attack;
                damageNumber.transform.position = transform.position;
            }

            if (currentHealth <= 0 && prevHealth > 0)
            {
                if (hasBossHealthbar)
                {
                    BossHealthbarManager.instance.DisableHealthbar();
                }

                AudioManager.instance.PlaySoundBaseAtPos(deathSounds, transform.position, gameObject.name);
                //print($"{gameObject.name} calling DeathSounds to play at position.");
                onDeath?.Invoke();
                return;
            }
            else if (invincibilityDuration != 0 && (attack.damage > 0 != attack.invertInvincibility))
            {
                invincible = true;
            }

            // Status effects
            if (CompareTag("Inanimate")) { return; }
            foreach (StatusEffect statusEffect in attack.statusEffects)
            {
                if (!immuneStatusEffects.Contains(statusEffect))
                {
                    StatusEffect matchingEffect = statusEffects.Find(statusEffect.Stack);
                    if (matchingEffect == null)
                    {
                        statusEffects.Add(statusEffect.CreateCopy(gameObject));
                        AudioManager.instance.PlaySoundBaseOnTarget(SoundGetter.Instance.GetStatusEffectSound(statusEffect.StatusType()), transform, true);
                    }
                }
            }
        }

        /// <summary>
        /// Increases the current health by the given amount, maxed out at the max health
        /// </summary>
        /// <param name="healAmount"> The amount to heal by. </param>
        public void Heal(int healAmount)
        {
            currentHealth = Mathf.Min(Math.Max(healAmount, 0) + currentHealth, maxHealth);
        }

        /// <summary>
        /// Whether or not this has a given status effect.
        /// </summary>
        /// <param name="statusEffect"> The effect to check for. </param>
        /// <returns> True if this has the given effect. </returns>
        public bool HasStatusEffect(StatusEffect statusEffect)
        {
            return statusEffects.FirstOrDefault((StatusEffect eachEffect) => { return eachEffect.GetType() == statusEffect.GetType(); }) != null;
        }


        /// <summary>
        /// Removes all status effects
        /// </summary>
        public void Cleanse()
        {
            while(statusEffects.Count > 0)
            {
                Destroy(statusEffects[statusEffects.Count - 1]);
                statusEffects.RemoveAt(statusEffects.Count - 1);
            }

            AudioManager.instance.PlayOneshotOnTarget(cleanseSound, transform);

        }

        /// <summary>
        /// Disable invincibility
        /// </summary>
        private void TurnOffInvincibility()
        {
            invincible = false;
        }
    }
}
