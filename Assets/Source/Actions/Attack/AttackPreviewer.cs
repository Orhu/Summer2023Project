using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// Base class for attack previewers
    /// </summary>
    public class AttackPreviewer : MonoBehaviour
    {
        // The owner of the preview.
        internal IActor actor;
        // The attack being previewed.
        internal Attack attack;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
