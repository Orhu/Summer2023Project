using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class used by the player animation on the game
/// over menu ui. Helps activate things during the animation
/// </summary>
public class GameOverMenuAnimationHelper : MonoBehaviour
{
    [Tooltip("Text representing the title of the game over menu")]
    [SerializeField] private TextMeshProUGUI gameOverTitle;

    [Tooltip("Text used for the restart button")]
    [SerializeField] private TextMeshProUGUI restartButtonText;

    [Tooltip("Text used for the quit button")]
    [SerializeField] private TextMeshProUGUI quitButtonText;

    /// <summary>
    /// Called at the end of the player death UI animation
    /// </summary>
    public void ActivateTitleAndButtons()
    {
        StartCoroutine(FadeInText(gameOverTitle, 0.5f));
        StartCoroutine(FadeInText(restartButtonText, 0.5f));
        StartCoroutine(FadeInText(quitButtonText, 0.5f));
        // reenable the button component for the restart and quit buttons
        restartButtonText.transform.GetComponentInParent<Button>().enabled = true;
        quitButtonText.transform.GetComponentInParent<Button>().enabled = true;
    }

    /// <summary>
    /// Used to fade a TMPro text component in using it's alpha value.
    /// Ensure the TMPro's vertext color already has an alpha of 0.
    /// </summary>
    /// <param name="textMesh">The TMPro text component being altered</param>
    /// <param name="fadeDuration">The duration of the fade in</param>
    /// <returns></returns>
    private IEnumerator FadeInText(TextMeshProUGUI textMesh, float fadeDuration)
    {
        Color startColor = textMesh.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float alpha = Mathf.Lerp(0f, 1f, t);

            textMesh.color = new Color(endColor.r, endColor.g, endColor.b, alpha);

            yield return null;
        }

        textMesh.color = endColor; // Ensure the final color is set correctly
    }
}
