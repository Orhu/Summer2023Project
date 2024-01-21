using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Handles mirroring based on state of animations.
    /// </summary>
    public class AnimatorController : MonoBehaviour
    {
        [Tooltip("The name of the animator trigger parameter to set if this sprite should be mirrored.")]
        [SerializeField] private string flipParamName = "Flip";

        [Tooltip("The default mirror parameter, if empty no parameter will be set by default.")]
        [SerializeField] private string defaultMirrorParam;

        [Tooltip("The states to the parameter name to use to mirror")]
        [SerializeField] private List<ClipToParameter> _animactionClipsToMirrorParameters;
        private Dictionary<AnimationClip, string> animactionClipsToMirrorParameters = new Dictionary<AnimationClip, string>();

        // The mirror parameters to their names.
        private Dictionary<string, bool> mirrorParametersToValues = new Dictionary<string, bool>();

        // The animator to control
        private Animator animator;

        // Whether or not this is mirrored.
        private bool mirrored = false;

        /// <summary>
        /// Initialize reference.
        /// </summary>
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();

            if (animator == null) 
            {
                Debug.LogError($"Animator is not set on Animation Controller component. Source: {this.gameObject.name}");
            }

            foreach (ClipToParameter animactionClipToMirrorParameter in _animactionClipsToMirrorParameters)
            {
                animactionClipsToMirrorParameters.Add(animactionClipToMirrorParameter.clip, animactionClipToMirrorParameter.parameterName);
                mirrorParametersToValues.TryAdd(animactionClipToMirrorParameter.parameterName, false);
            }

            if (defaultMirrorParam.Length > 0)
            {
                mirrorParametersToValues.TryAdd(defaultMirrorParam, false);
            }
        }

        /// <summary>
        /// Updates the transform to mirror appropriately
        /// </summary>
        private void Update()
        {
            if (!animator.hasBoundPlayables) { return; }

            AnimationClip currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            if (currentClip == null) { return; }
            if (!animactionClipsToMirrorParameters.ContainsKey(currentClip)) 
            { 
                if (defaultMirrorParam.Length > 0)
                {
                    SetMirrored(mirrorParametersToValues[defaultMirrorParam]);
                }
                return; 
            }
            SetMirrored(mirrorParametersToValues[animactionClipsToMirrorParameters[currentClip]]);


            void SetMirrored(bool newMirrored)
            {
                if (mirrored != newMirrored)
                {
                    mirrored = newMirrored;
                    if (animator.parameters.Any(p => p.name == flipParamName))
                    {
                        animator.SetTrigger(flipParamName);
                    }
                    else
                    {
                        transform.localScale = new Vector3(newMirrored ? -1 : 1, 1, 1);
                    }
                }
            }
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