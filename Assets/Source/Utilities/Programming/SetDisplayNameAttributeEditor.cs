using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SetDisplayNameAttribute))]
public class SetDisplayNameAttributeEditor : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PropertyField(position, property, new GUIContent((attribute as SetDisplayNameAttribute).NewName));
	}
}