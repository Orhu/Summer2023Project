using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Cardificer
{
    /// <summary>
    /// Holds references to Audio things that are referenced by other classes. This class is for easy implementation and may be in constant flux as to what lives here.
    /// </summary>
    public class SoundGetter : MonoBehaviour
    {
        //Singleton pattern
        public static SoundGetter Instance;

        [Tooltip("The list of sounds for each status effect.")]
        public StatusSound[] statusSounds;
        private Dictionary<StatusEffectType, BasicSound> statusSoundsDict = new Dictionary<StatusEffectType, BasicSound>();

        [Tooltip("The AudioMixer Group associated with enemy audio.")]
        public AudioMixerGroup enemyAudioMixerGroup;
        [Tooltip("The AudioMixer Group associated with Player Actions.")]
        public AudioMixerGroup playerActionsAudioMixerGroup;

        [Tooltip("The Sound played when health is picked up.")]
        public BasicSound healthPickupSound;

        /// <summary>
        /// Implementing the singleton pattern and initializing the dictionary. 
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There's more than one SoundGetter! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (StatusSound s in statusSounds)
            {
                statusSoundsDict.Add(s.effectType, s.effectApplySound);
            }

        }

        /// <summary>
        /// Returns a BasicSound associated with a specified StatusEffectType. 
        /// </summary>
        /// <param name="type">The StatusEffectType to query the dictionary of StatusEffectSounds. </param>
        /// <returns> Returns a BasicSound associated with the given StatusEffectType. </returns>
        public BasicSound GetStatusEffectSound(StatusEffectType type)
        {
            try
            {
                //print("GetSound Succeeded! " + statusSoundsDict[type].name + " got.");
                return statusSoundsDict[type];

            } catch
            {
                if (AudioManager.instance.printDebugMessages) print($"GetStatusEffectSound of type {type} failed!");
                return new BasicSound();
            }

        }

    }

    /// <summary>
    /// Used for inspector serialization. Values added to a dictionary at runtime.
    /// </summary>
    [System.Serializable]
    public struct StatusSound
    {
        public string effectName => effectType.ToString();
        public StatusEffectType effectType;
        public BasicSound effectApplySound;
    }

    /// <summary>
    /// Describes a kind of status effect.
    /// </summary>
    public enum StatusEffectType
    {
        Burning,//
        Cursed, //
        DamageReduction, //
        Exhausted, //
        Flowing, //
        Focused, //
        Intangible, //
        Invulnerable, //
        KnockbackReduction, //
        Panicked, //
        Poisoned, //
        Purified, //
        Rooted, //
        Silenced, //
        Slowed_DurationsAddOnStack, //
        Slowed_DurationsResetOnStack, //
        Slowed_SlowIncreasesOnStack, //
        Soaked, //
        Stunned, //
        Thorns, //
        Toxic, //

    }

}
