using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script for Echoing Shout's shader. It just animates the ripple on the shader when you cast Echoing Shout.
/// </summary>
public class EchoShaderController : MonoBehaviour
{
    // smack this bad boy on the gameobject you instantiate when casting echoing shout as a child of the player

    [Tooltip("Length of echo shader animation (in seconds)")]
    // length of echo in seconds
    [SerializeField] private float echoTime = 3.5f; // should be constant and roughly equal to the lifetime of echoing shout so that the ripple moves with the bullets (21 tiles / 6 tiles/s = 3.5s)
    
    // reference to coroutine that controls the echo effect
    private Coroutine echoCoroutine;

    // material on game object
    private Material material;

    // shader property conversion
    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter"); 

    /// <summary>
    /// triggers the echo
    /// </summary>
    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        CallEcho();
    }

    /// <summary>
    /// triggers echo coroutine
    /// </summary>
    public void CallEcho()
    {
        echoCoroutine = StartCoroutine(EchoAction(-0.1f, 1f));
    }

    /// <summary>
    /// echo effect controlling subroutine
    /// </summary>
    /// <param name="startPos"> start position in WaveDistanceFromCenter slider (ECHOING SHOUT IS ALWAYS -0.1f)</param>
    /// <param name="endPos"> end position in WaveDistanceFromCenter slider (ECHOING SHOUT IS ALWAYS 1f)</param>
    /// <returns></returns>    
    private IEnumerator EchoAction(float startPos, float endPos)
    {
        material.SetFloat(waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < echoTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / echoTime));
            material.SetFloat(waveDistanceFromCenter, lerpedAmount); // use lerp amount to set progress of ripple effect

            yield return null;
        }
    }
}
