using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The camera script for the template creator
/// </summary>
public class TemplateCreatorCamera : MonoBehaviour
{
    [Tooltip("How much the scroll wheel affects the zoom")]
    float cameraZoomSpeed = 1;

    /// <summary>
    /// Zooms the camera
    /// </summary>
    void Update()
    {
        GetComponent<Camera>().orthographicSize += -Input.mouseScrollDelta.y * cameraZoomSpeed;
    }
}
