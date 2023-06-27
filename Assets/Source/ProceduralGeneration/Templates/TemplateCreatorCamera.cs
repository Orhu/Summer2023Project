using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The camera script for the template creator
    /// </summary>
    public class TemplateCreatorCamera : MonoBehaviour
    {
        [Tooltip("How much the scroll wheel affects the zoom")]
        float cameraZoomSpeed = 1;

        [Tooltip("The minimum zoom the camera can have")]
        float minZoom = 0.01f;


        /// <summary>
        /// Zooms the camera
        /// </summary>
        /// <param name="scrollDelta"></param>
        public void Zoom(Vector2 scrollDelta)
        {
            if (GetComponent<Camera>().orthographicSize + -scrollDelta.y * cameraZoomSpeed < minZoom)
            {
                GetComponent<Camera>().orthographicSize = minZoom;
            }
            else
            {
                GetComponent<Camera>().orthographicSize += -scrollDelta.y * cameraZoomSpeed;
            }
            GetComponent<Camera>().orthographicSize += -scrollDelta.y * cameraZoomSpeed;
        }

        /// <summary>
        /// Pans the camera
        /// </summary>
        /// <param name="delta"> The amount to move the camera by </param>
        public void Pan(Vector2 delta)
        {
            transform.position += new Vector3(delta.x, delta.y);
        }
    }
}