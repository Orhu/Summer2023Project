using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoManager : MonoBehaviour
{
    // smack this bad boy on the gameobject you instantiate when casting echoing shout as a child of the player

    [Tooltip("Length of echo shader animation")]
    [SerializeField] private float _echoTime = 0.5f; // Length of echo

    private Coroutine _echoCoroutine; // reference to coroutine that controls the echo effect

    private Material _material; // material on game object

    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter"); // shader property (unnecessary comment tbh)

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
        CallEcho();
    }

    // triggers the echo effect
    public void CallEcho()
    {
        _echoCoroutine = StartCoroutine(EchoAction(-0.1f, 1f));
    }

    // echo effect controlling subroutine
    private IEnumerator EchoAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _echoTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _echoTime));
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
    }
}
