using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Handles mirroring based on state of animations.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {
        [Tooltip("The states to the parameter name to use to mirror")]
        [SerializeField] private List<ClipToParameter> _animactionClipsToMirrorParameters;
        private Dictionary<AnimationClip, string> animactionClipsToMirrorParameters = new Dictionary<AnimationClip, string>();

        // The mirror parameters to their names.
        private Dictionary<string, bool> mirrorParametersToValues = new Dictionary<string, bool>();

        // The animator to control
        private Animator animator;

        /// <summary>
        /// Initialize reference.
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();

            foreach (ClipToParameter animactionClipToMirrorParameter in _animactionClipsToMirrorParameters)
            {
                animactionClipsToMirrorParameters.Add(animactionClipToMirrorParameter.clip, animactionClipToMirrorParameter.parameterName);

                if (mirrorParametersToValues.ContainsKey(animactionClipToMirrorParameter.parameterName)) { continue; }
                mirrorParametersToValues.Add(animactionClipToMirrorParameter.parameterName, false);
            }
        }

        /// <summary>
        /// Updates the transform to mirror appropriately
        /// </summary>
        private void Update()
        {
            if (!animator.hasBoundPlayables) { return; }

            AnimationClip currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            if (currentClip == null || !animactionClipsToMirrorParameters.ContainsKey(currentClip)) { return; }
            transform.localScale = new Vector3(mirrorParametersToValues[animactionClipsToMirrorParameters[currentClip]] ? -1 : 1, 1, 1);
        }

        /// <summary>
        /// Sets the value of the given boolean parameter.
        /// </summary>
        /// <param name="name"> The parameter name. </param>
        /// <param name="value"> The new parameter value. </param>
        public void SetMirror(string name, bool value)
        {
            if (!animator.hasBoundPlayables) { return; }

            mirrorParametersToValues[name] = value;
        }

        /// <summary>
        /// Sets the value of the given boolean parameter.
        /// </summary>
        /// <param name="name"> The parameter name. </param>
        /// <param name="value"> The new parameter value. </param>
        public void SetBool(string name, bool value)
        {
            if (!animator.hasBoundPlayables) { return; }

            animator.SetFloat(name, value ? 1f : 0f);
        }

        /// <summary>
        /// Sets the value of the given trigger parameter.
        /// </summary>
        /// <param name="name"> The parameter name. </param>
        public void SetTrigger(string name)
        {
            if (!animator.hasBoundPlayables) { return; }

            animator.SetTrigger(name);
        }

        /// <summary>
        /// A animation clip and a parameter name.
        /// </summary>
        [System.Serializable]
        private class ClipToParameter
        {
            [Tooltip("The clip to mirror")]
            public AnimationClip clip;

            [Tooltip("The name of the property to determinate if the clip should be mirrored")]
            public string parameterName;
        }

    }
}