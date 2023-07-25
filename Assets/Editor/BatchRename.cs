using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Utility for renaming a bunch of things.
/// </summary>
public class BatchRename : ScriptableWizard
{
    // The labels to add.
    [SerializeField] private string find = "";

    // What to replace the found thing
    // 
    [SerializeField] private string replaceWith = "";

    /// <summary>
    /// Gives a find and replace prompt.
    /// </summary>
    [MenuItem("Tools/Batch Rename")]
    static void Rename()
    {
        DisplayWizard<BatchRename>("BatchRename", "Confirm");
    }

    /// <summary>
    /// Adds the labels.
    /// </summary>
    void OnWizardCreate()
    {
        foreach (Object selectedObject in Selection.objects)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedObject), selectedObject.name.Replace(find, replaceWith));
        }
    }
}
