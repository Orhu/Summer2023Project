using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Cardificer
{
    public class SoundGetter : MonoBehaviour
    {

        public static SoundGetter Instance;

        public StatusSound[] statusSounds;
        private Dictionary<StatusEffectType, BasicSound> statusSoundsDict = new Dictionary<StatusEffectType, BasicSound>();

        public AudioMixerGroup enemyAudioMixerGroup, playerActionsAudioMixerGroup;

        public BasicSound healthPickupSound;


        /// <summary>
        /// Implementing the singleton pattern. 
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

        public BasicSound GetStatusEffectSound(StatusEffectType type)
        {
            try
            {
                //print("GetSound Succeeded! " + statusSoundsDict[type].name + " got.");
                return statusSoundsDict[type];

            } catch
            {
                print($"GetStatusEffectSound of type {type} failed!");
                return new BasicSound();
            }

        }

    }

    [System.Serializable]
    public struct StatusSound
    {
        public string effectName => effectType.ToString();
        public StatusEffectType effectType;
        public BasicSound effectApplySound;
    }

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
