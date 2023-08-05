using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A loading screen for hiding async function calls.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        // The minimum time that this will load for.
        [HideInInspector] public float minLoadingTime = 0.5f;

        // All of the operations that this will wait for.
        private List<AsyncOperation> operations = new List<AsyncOperation>();

        /// <summary>
        /// Adds another async operation that this will wait for the completion of before 
        /// </summary>
        /// <param name="operation"></param>
        public void AddAsyncOperation(AsyncOperation operation)
        {
            operations.Add(operation);
        }

        /// <summary>
        /// Causes this to close itself after the min loading time and all operations have completed.
        /// </summary>
        private void OnEnable()
        {
            StartCoroutine(WaitForLoading());

            IEnumerator WaitForLoading()
            {
                // Give a frame for vars to be set.
                yield return null;

                yield return new WaitForSecondsRealtime(minLoadingTime / 2);
                yield return operations;
                yield return new WaitForSecondsRealtime(minLoadingTime / 2);

                operations.Clear();
                MenuManager.Close<LoadingScreen>(true);
            }
        }
    }
}