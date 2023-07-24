using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// An action that spawns an thing at the aim location.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpawnThing", menuName = "Cards/Actions/Spawn Thing")]
    public class SpawnThing : Action
    {
        [Tooltip("The thing to spawn")]
        [SerializeField] private GameObject thing;

        [Tooltip("The delay before it is spawned")] [Min(0f)]
        [SerializeField] private float delay = 0f;

        /// <summary>
        /// Plays this action and causes all its effects.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<GameObject> ignoredObjects)
        {
            if (delay <= 0)
            {
                Instantiate(thing).transform.position = actor.GetActionSourceTransform().position;
            }
            else
            {
                actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(DelayedSpawn(actor));
            }
        }

        /// <summary>
        /// Spawns the thing.
        /// </summary>
        private IEnumerator DelayedSpawn(IActor actor)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(thing).transform.position = actor.GetActionSourceTransform().position;
        }
    }
}