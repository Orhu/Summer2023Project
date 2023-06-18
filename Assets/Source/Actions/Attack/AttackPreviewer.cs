using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Base class for attack previewers
    /// </summary>
    public class AttackPreviewer : MonoBehaviour
    {
        // The owner of the preview.
        public IActor actor;
        // The attack being previewed.
        public Attack attack;
    }
}
