using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cardificer
{
    /// <summary>
    /// A list of modifier types that will be filtered out.
    /// </summary>
    [CreateAssetMenu(menuName = "Cards/Modifier Filter")]
    public class ModifierFilter : ScriptableObject
    {
        [Tooltip("A lits of the class names of modifiers to not apply to this action. Null is a valid replacement modifier")]
        [SerializeField] private ForbiddenToReplacementModifers[] forbiddenModifiers = new ForbiddenToReplacementModifers[0];

        /// <summary>
        /// Filters a list of modifiers based off of this
        /// </summary>
        /// <param name="modifiers"> The modifier list to filter. </param>
        /// <returns> The list of modifiers with all modifiers who's types are forbidden removed. </returns>
        public List<AttackModifier> FilterModifierList(List<AttackModifier> modifiers)
        {
            return modifiers.Select(ReplaceModifiers).Where(item => item != null).ToList();
        }

        /// <summary>
        /// Gets whether a modifier is allowed by this filter.
        /// </summary>
        /// <param name="modifier"> The modifier to test. </param>
        /// <returns> True if the modifier is allowed. </returns>
        public AttackModifier ReplaceModifiers(AttackModifier modifier)
        {
            foreach (ForbiddenToReplacementModifers forbiddenModifier in forbiddenModifiers)
            {
                if (forbiddenModifier.forbiddenModifierType.IsInstanceOfType(modifier))
                { 
                    return forbiddenModifier.replaceWith; 
                }
            }
            return modifier;
        }



        /// <summary>
        /// Class for mapping modifiers to their replacement.
        /// </summary>
        [System.Serializable]
        private class ForbiddenToReplacementModifers
        {
            [Tooltip("The name of the modifier to forbid.")]
            [SerializeField]private string forbiddenModifierName;

            [Tooltip("The modifier to replace the forbidden one with. Leave null to remove instead of replacing the forbidden modifier.")]
            public AttackModifier replaceWith;


            // The type of modifier to forbid.
            public Type forbiddenModifierType => Type.GetType($"Cardificer.{forbiddenModifierName}");
        }

#if UNITY_EDITOR
        /// <summary>
        /// Class for making the DecisionPair easier to read in the inspector.
        /// </summary>
        [CustomPropertyDrawer(typeof(ForbiddenToReplacementModifers))]
        public class ForbiddenToReplacementModifersPropertyDrawer : PropertyDrawer
        {
            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Calculate rects
                var replaceTextRect = new Rect(position.x - 60, position.y, 60, position.height);
                var nameRect = new Rect(position.x, position.y, position.width / 2f - 20, position.height);
                var withTextRect = new Rect(position.x + position.width / 2f - 15, position.y, 40, position.height);
                var replacementRect = new Rect(position.x + position.width / 2f + 20, position.y, position.width / 2f - 20, position.height);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.LabelField(replaceTextRect, "Replace");
                EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("forbiddenModifierName"), GUIContent.none);
                EditorGUI.LabelField(withTextRect, "With");
                EditorGUI.PropertyField(replacementRect, property.FindPropertyRelative("replaceWith"), GUIContent.none);

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }
#endif
    }
}