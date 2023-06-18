using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Generic SteeringBehavior to be used by different steering behaviors
    /// </summary>
    public abstract class SteeringBehavior : MonoBehaviour
    {
        /// <summary>
        /// Update danger and interest arrays according to steering type
        /// </summary>
        /// <param name="danger">Current danger values array</param>
        /// <param name="interest">Current interest values array</param>
        /// <param name="aiData">AI data object containing targets and obstacles</param>
        /// <returns></returns>
        public abstract (float[] danger, float[] interest)
            GetSteering(float[] danger, float[] interest, AIData aiData);
    }
}