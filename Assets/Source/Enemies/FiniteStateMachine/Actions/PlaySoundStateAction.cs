using System;
using System.Collections;
using UnityEditorInternal;
using System.Threading.Tasks;
using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action to update our target to be the player
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Actions/PlaySoundStateAction")]
    public class PlaySoundStateAction : BaseAction
    {
        [SerializeField] private BasicSound actionSound;
        [SerializeField] private int delaySoundByMiliseconds;

        public override void Execute(BaseStateMachine stateMachine)
        {
            PlaySound(stateMachine);
        }

        private async void PlaySound(BaseStateMachine stateMachine)
        {
            await Task.Delay(delaySoundByMiliseconds);
            AudioManager.instance.PlaySoundBaseOnTarget(actionSound, stateMachine.transform, true);

        }
    }
}