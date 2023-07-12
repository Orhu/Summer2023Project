using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Utility for adding labels to a bunch of things.
/// </summary>
public class AddLabels : ScriptableWizard
{
    // The labels to add.
    [SerializeField] private string[] labelsToAdd = { "" };

    /// <summary>
    /// Asks the user for labels to add.
    /// </summary>
    [MenuItem("Tools/Add Label To Selected Objects...")]
    static void AddTagToSelectedObjects()
    {
        DisplayWizard(
            "Add Label To Selected Objects",
            typeof(AddLabels),
            "Confirm");
    }

    /// <summary>
    /// Adds the labels.
    /// </summary>
    void OnWizardCreate()
    {
        foreach (Object selectedObject in Selection.objects)
        {
            string[] newLabels = AssetDatabase.GetLabels(selectedObject).Union(labelsToAdd).ToArray();
            AssetDatabase.SetLabels(selectedObject, newLabels);
        }
    }
}