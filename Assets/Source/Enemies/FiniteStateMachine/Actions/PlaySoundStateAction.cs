using System;
using System.Collections;
using UnityEditorInternal;
using System.Threading.Tasks;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Plays a BasicSound as part of a StateMachine
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/PlaySoundStateAction")]
    public class PlaySoundStateAction : BaseAction
    {
        [Header("The BasicSound to play on this State Action")]
        [SerializeField] private BasicSound actionSound;

        [Header("Delay the sound by x miliseconds")]
        [SerializeField] private int delaySoundByMiliseconds;

        /// <summary>
        /// Play the BasicSound
        /// </summary>
        /// <param name="stateMachine">The BaseStateMacine that is calling this method. </param>
        public override void Execute(BaseStateMachine stateMachine)
        {
            PlaySound(stateMachine);
        }

        /// <summary>
        /// Wait for some time the play the BasicSound
        /// </summary>
        /// <param name="stateMachine">The BaseStateMacine that is calling this method. </param>
        private async void PlaySound(BaseStateMachine stateMachine)
        {
            await Task.Delay(delaySoundByMiliseconds);
            AudioManager.instance.PlaySoundBaseOnTarget(actionSound, stateMachine.transform, true);

        }
    }
}