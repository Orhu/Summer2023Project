using System.Linq;
using UnityEditor;
using UnityEngine;

public class SelectAllPlayActions
{
    [MenuItem("Tools/SelectAllOfType")]
    static void Select()
    {
        foreach (string path in AssetDatabase.GetAllAssetPaths())
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(path) is Cardificer.PlayActionInheritDamage action)
            {
                Selection.objects = Selection.objects.Append(action).ToArray();
            }
        }
    }
}