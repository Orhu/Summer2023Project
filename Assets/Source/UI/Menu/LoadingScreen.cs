using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cardificer
{
    public class LoadingScreen : MonoBehaviour
    {
        /// <summary>
        /// Opens a loading screen that is guaranteed to be shown.
        /// </summary>
        /// <param name="loadingAction"> The code that should execute while loading. </param>
        /// <param name="finishLoading"> Invoke this to tell the loading screen that everything is finished loading. </param>
        /// <param name="minLoadingTime"> The minimum time that the loading screen will be shown for. </param>
        public static void Open(System.Action loadingAction, out System.Action finishLoading, float minLoadingTime = 1f)
        {
            bool readyToClose = false;
            MenuManager.Open<LoadingScreen>(false, true);
            MenuManager.StartCoroutine(DelayedAction());
            finishLoading = () => MenuManager.StartCoroutine(DelayedClose());
            
            // Runs the loading action after half the min loading time.
            IEnumerator DelayedAction()
            {
                yield return new WaitForSeconds(minLoadingTime / 2);
                loadingAction();
                DelayedClose();
            }

            // Closes the loading screen after half the min loading time if it is ready to be closed.
            IEnumerator DelayedClose()
            {
                if (readyToClose)
                {
                    yield return new WaitForSeconds(minLoadingTime / 2);
                    MenuManager.Close<LoadingScreen>(true);
                }
                else
                {
                    readyToClose = true;
                }
            }
        }
        public static void Open(System.Action loadingAction, float loadingTime = 1f)
        {
            MenuManager.Open<LoadingScreen>(false, true);
            MenuManager.StartCoroutine(DelayedAction());

            IEnumerator DelayedAction()
            {
                yield return new WaitForSeconds(loadingTime / 2);
                loadingAction();
                yield return new WaitForSeconds(loadingTime / 2);
                MenuManager.Close<LoadingScreen>(true);
            }
        }

    }
}