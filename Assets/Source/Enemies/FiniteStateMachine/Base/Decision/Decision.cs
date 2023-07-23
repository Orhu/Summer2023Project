using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Skaillz.EditInline;
using System.Linq;

namespace Cardificer.FiniteStateMachine
{

    /// <summary>
    /// Represents a condition to evaluate whether a transition should be true or false
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        /// <summary>
        /// Given a state machine, returns whether the transition should be true or false
        /// </summary>
        /// <param name="state"> The state machine to use. </param>
        /// <returns> True if the expression evaluates to true, false otherwise </returns>
        public abstract bool Decide(BaseStateMachine state);

        /// <summary>
        /// A version of the decision that provides extra data so that any enumerable collection of Decision.Combinable can have .Decide called on it.
        /// </summary>
        [System.Serializable]
        public class Combinable
        {
            [Tooltip("Whether or not to invert this decision's result.")]
            public bool invert = false;

            [Tooltip("The decision to evaluate.")]
            [EditInline]
            public Decision decision;

            [Tooltip("The operator to use to combine this with the next decision")]
            public LogicalOperator nextOperator;

            /// <summary>
            /// Represents a logical operator that can be used to combine 2 decisions
            /// </summary>
            public enum LogicalOperator
            {
                [InspectorName("")]
                None,
                [InspectorName("OR")]
                Or,
                [InspectorName("AND")]
                And,
            }

#if UNITY_EDITOR
            /// <summary>
            /// Class for making the DecisionPair easier to read in the inspector.
            /// </summary>
            [CustomPropertyDrawer(typeof(Combinable))]
            public class DecisionPairPropertyDrawer : PropertyDrawer
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
                    var invertLabelRect = new Rect(position.x - 40, position.y, position.width, position.height);
                    var invertRect = new Rect(position.x, position.y, position.width, position.height);
                    var decisionRect = new Rect(position.x + 30, position.y + 1, position.width - 95, position.height);
                    var operatorRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);

                    // Draw fields - pass GUIContent.none to each so they are drawn without labels
                    EditorGUI.LabelField(invertLabelRect, "Invert:");
                    EditorGUI.PropertyField(invertRect, property.FindPropertyRelative("invert"), GUIContent.none);
                    EditorGUI.PropertyField(decisionRect, property.FindPropertyRelative("decision"), GUIContent.none);
                    EditorGUI.PropertyField(operatorRect, property.FindPropertyRelative("nextOperator"), GUIContent.none);

                    // Set indent back to what it was
                    EditorGUI.indentLevel = indent;

                    EditorGUI.EndProperty();
                }
            }
#endif
        }
    }
    public static class DecisionListDecider
    {
        public static bool Decide(this IEnumerable<Decision.Combinable> decisions, BaseStateMachine stateMachine)
        {
            if (decisions.Count() == 0)
            {
                return true;
            }

            //Calculate AND results
            List<bool> andResults = new List<bool>();

            for (int i = 0; i < decisions.Count(); i++)
            {
                andResults.Add(decisions.ElementAt(i).invert != decisions.ElementAt(i).decision.Decide(stateMachine));

                for (;
                    i + 1 < decisions.Count() && decisions.ElementAt(i).nextOperator is Decision.Combinable.LogicalOperator.And;
                    i++
                    )
                {
                    int andIndex = andResults.Count() - 1;
                    bool newValue = andResults[andIndex] && (decisions.ElementAt(i + 1).invert != decisions.ElementAt(i + 1).decision.Decide(stateMachine));
                    andResults[andIndex] = newValue;
                }
            }

            // Calculate OR results
            bool result = andResults[0];
            for (int i = 1; i < andResults.Count; i++)
            {
                result = result || andResults[i];
            }

            return result;
        }
    }
}
