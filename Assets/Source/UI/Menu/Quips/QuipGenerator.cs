using UnityEngine;
using TMPro;

/// <summary>
/// Generates quips for the main menu
/// 
/// NOTE: Deleting or removing this functionality
/// will break this game. It'll be, like, super boring.
/// </summary>
public class QuipGenerator : MonoBehaviour
{
    [Tooltip("Text to display the quip")]
    [SerializeField] private TextMeshProUGUI quipText;

    [Tooltip("Text file to write all the quips in")]
    [SerializeField] private TextAsset textFileOfQuips;

    // List of sentences we parse from the textFile
    private string[] sentenctList;

    /// <summary>
    /// Split the text file and create the sentence list
    /// </summary>
    private void Awake()
    {
        sentenctList = textFileOfQuips.text.Split('\n');
    }

    /// <summary>
    /// Randomly choose a quip sentence
    /// </summary>
    void Start()
    {
        quipText.text = sentenctList[Random.Range(0, sentenctList.Length)];
    }
}
