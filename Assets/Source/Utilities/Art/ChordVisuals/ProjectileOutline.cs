using UnityEngine;

namespace Cardificer
{
    public class ProjectileOutline : MonoBehaviour
    {
        [Tooltip("The scale applied to each outline")]
        [SerializeField] private float scale = 0.1f;

        GameObject visualObject;
        SpriteRenderer sprite;
        SpriteRenderer targetRenderer;
        // Start is called before the first frame update
        void Start()
        {
            visualObject = transform.parent.gameObject;

            int outlineIndex = transform.parent.parent.GetComponentsInChildren<ProjectileOutline>().Length;
            scale *= outlineIndex;
            scale += 1;
            transform.localScale = new Vector3(scale, scale, scale);

            sprite = GetComponent<SpriteRenderer>();
            sprite.sortingOrder = -outlineIndex;
            targetRenderer = visualObject.GetComponent<SpriteRenderer>();
            if (targetRenderer == null)
            {
                targetRenderer = visualObject.GetComponentInChildren<SpriteRenderer>();
            }

            sprite.sprite = targetRenderer.sprite;
            sprite.enabled = true;

            transform.SetParent(targetRenderer.transform, false);
        }

        private void FixedUpdate()
        {
            sprite.sprite = targetRenderer.sprite;
        }
    }
}