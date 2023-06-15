using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Script for Echoing Shout's shader. TOUCH THIS AND YOU DIE!!!!!!!!!!
///  Also I'm not serializing this because it shouldn't be serialized :)
/// </summary>
public class EchoManager : MonoBehaviour
{
    // smack this bad boy on the gameobject you instantiate when casting echoing shout as a child of the player

    [Tooltip("Length of echo shader animation")]
    // length of echo in seconds
    [SerializeField] private float _echoTime = 3.5f; // should be constant and roughly equal to the lifetime of echoing shout so that the ripple moves with the bullets (21 tiles / 6 tiles/s = 3.5s)
    
    // reference to coroutine that controls the echo effect
    private Coroutine _echoCoroutine;

    // material on game object
    private Material _material;

    // shader property conversion (unnecessary comment tbh)
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter"); 

    /// <summary>
    /// triggers the echo
    /// </summary>
    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
        CallEcho();
    }

    /// <summary>
    /// triggers echo coroutine
    /// </summary>
    public void CallEcho()
    {
        _echoCoroutine = StartCoroutine(EchoAction(-0.1f, 1f));
    }

    /// <summary>
    /// echo effect controlling subroutine
    /// </summary>
    /// <param name="startPos"> start val (ALWAYS -0.1f)</param>
    /// <param name="endPos"> end val (ALWAYS 1f)</param>
    /// <returns>cool effect idk</returns>    
    private IEnumerator EchoAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _echoTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _echoTime));
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount); // use lerp amount to set progress of ripple effect

            yield return null;
        }
    }
}
