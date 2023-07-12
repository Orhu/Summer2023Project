using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Utility for adding labels to a bunch of things.
/// </summary>
public class BatchRename : ScriptableWizard
{
    // The labels to add.
    [SerializeField] private string find = "";

    // What to replace the found thing with
    [SerializeField] private string replaceWith = "";

    /// <summary>
    /// Asks the user for labels to add.
    /// </summary>
    [MenuItem("Tools/Batch Rename")]
    static void AddTagToSelectedObjects()
    {
        DisplayWizard<BatchRename>(
            title: "BatchRename", 
            createButtonName: "Confirm");
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
