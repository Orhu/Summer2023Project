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
        void Update()
        {
            if (GetComponent<Camera>().orthographicSize + -Input.mouseScrollDelta.y * cameraZoomSpeed < minZoom)
            {
                GetComponent<Camera>().orthographicSize = minZoom;
            }
            else
            {
                GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * cameraZoomSpeed;
            }
            GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * cameraZoomSpeed;
        }
    }
}