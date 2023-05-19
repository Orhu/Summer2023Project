using UnityEngine;

/// <summary>
/// The base any modification that can be made to action actions
/// </summary>
[ExecuteInEditMode]
public abstract class ActionModifier : ScriptableObject 
{
    public void Awake()
    {
        UnityEditor.EditorGUIUtility.SetIconForObject(this, UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Content/Developer Utilities/Icons/ActionModifierIcon.png"));
    }
}