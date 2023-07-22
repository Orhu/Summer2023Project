using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Utility for copying and adjusting references.
/// </summary>
public class DeepCopy : ScriptableWizard
{
    // The labels to add.
    [SerializeField] private string copyFilepath = "Assets/Content/Test";

    /// <summary>
    /// Gives a find and replace prompt.
    /// </summary>
    [MenuItem("Assets/Deep Copy", priority = -1)]
    static void Copy()
    {
        DisplayWizard<DeepCopy>("Deep Copy", "Confirm");
    }

    /// <summary>
    /// Checks to make sure a deep copy is possible right now.
    /// </summary>
    /// <returns> True if possible. </returns>
    [MenuItem("Assets/Deep Copy", true)]
    private static bool isValidSelection()
    {
        return Selection.objects.Length == 1;
    }

    /// <summary>
    /// Creates copies of the selected asset and all its dependencies in the new file location.
    /// </summary>
    void OnWizardCreate()
    {
        string selectionPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
        string[] dependencies = AssetDatabase.GetDependencies(selectionPath);
        string parentFolder = GetParentFolder(selectionPath);
        
        List<(string, string)> newGUIDs = new List<(string, string)>();
        List<string> newFiles = new List<string>(dependencies.Length);



        Debug.Log($"Copying {selectionPath} and its dependencies to {copyFilepath}...");

        // Create new files
        foreach (string path in dependencies)
        {
            if (path.StartsWith(parentFolder))
            {
                string newPath = path.Replace(parentFolder, copyFilepath);
                if (AssetDatabase.CopyAsset(path, newPath))
                {
                    newFiles.Add(newPath);
                    newGUIDs.Add((AssetDatabase.AssetPathToGUID(path), AssetDatabase.AssetPathToGUID(newPath)));
                }
                else
                {
                    Debug.LogError($"Failed to copy to {newPath}. Make sure that {copyFilepath} contains all the same sub folders as the original folder.");
                    DeleteCreatedFiles();
                }
            }
        }

        // Adjust references
        foreach (string newFile in newFiles)
        {
            if (newFile.EndsWith(".png") || newFile.EndsWith(".jpeg") || newFile.EndsWith(".wav")) { continue; }
            
            string absolutePath = Application.dataPath + newFile.Substring(6);


            StreamReader reader = File.OpenText(absolutePath);
            string contents = reader.ReadToEnd();
            
            reader.Close();

            foreach ((string, string) oldToNewGUID in newGUIDs)
            {
                contents = contents.Replace(oldToNewGUID.Item1, oldToNewGUID.Item2);
            }


            StreamWriter writer = File.CreateText(absolutePath);
            writer.Write(contents);
            writer.Close();
        }

        Debug.Log($"Finished copying");

        // Gets the parent folder of the selected object
        static string GetParentFolder(string selectionPath)
        {
            int i = 0;
            string[] splitPath = selectionPath.Split('/');
            string parentFolder = splitPath.Aggregate(
                (string acumulate, string value) =>
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            return value;
                        case int n when n < splitPath.Length - 1:
                            return acumulate + "/" + value;
                        default:
                            return acumulate;
                    }
                });
            return parentFolder;
        }
        // Delete files if a copy failed.
        void DeleteCreatedFiles()
        {
            foreach (string newFile in newFiles)
            {
                AssetDatabase.DeleteAsset(newFile);
            }
        }
    }
}
