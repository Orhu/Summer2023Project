using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;
using UnityEditor;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents the base class of a finite state machine transition. If false, the state machine remains in the same state
    /// </summary>
    [CreateAssetMenu(fileName = "NewTransition", menuName = "FSM/Transition")]
    public class Transition : ScriptableObject
    {
        [Tooltip("What state to enter on true")] [EditInline]
        [SerializeField] protected State trueState;

        [Tooltip("Conditions to evaluate")]
        [SerializeField] private List<DecisionPair> decisions;

        /// <summary>
        /// A decision and the next logical operator in the sequence.
        /// </summary>
        [System.Serializable]
        private class DecisionPair
        {
            [Tooltip("The decision to evaluate.")] [EditInline]
            public BaseDecision decision;

            [Tooltip("The operator to use to combine this with the next decision")]
            public LogicalOperator nextOperator;
        }

        /// <summary>
        /// Class for making the DecisionPair easier to read in the inspector.
        /// </summary>
        [CustomPropertyDrawer(typeof(DecisionPair))]
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
                var decisionRect = new Rect(position.x, position.y, position.width - 65, position.height);
                var operatorRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(decisionRect, property.FindPropertyRelative("decision"), GUIContent.none);
                EditorGUI.PropertyField(operatorRect, property.FindPropertyRelative("nextOperator"), GUIContent.none);

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }

        /// <summary>
        /// Represents a logical operator that can be used to combine 2 decisions
        /// </summary>
        private enum LogicalOperator
        {
            [InspectorName("")]
            None,
            [InspectorName("OR")]
            Or,
            [InspectorName("AND")]
            And,
            [InspectorName("NOR")]
            Nor,
            [InspectorName("NAND")]
            Nand,
        }

        /// <summary>
        /// Evaluate this transition
        /// </summary>
        /// <param name="stateMachine"> The stateMachine to use </param>
        public void Evaluate(BaseStateMachine stateMachine)
        {
            if (Execute(stateMachine))
            {
                // true and not inverted, change to true state
                StateTransition(stateMachine);
            }
        }

        /// <summary>
        /// Transitions to the true state
        /// </summary>
        /// <param name="stateMachine"> The state machine to use </param>
        private void StateTransition(BaseStateMachine stateMachine)
        {
            stateMachine.currentState.OnStateExit(stateMachine);
            stateMachine.currentState = trueState;
            stateMachine.timeSinceTransition = 0f;
            stateMachine.currentState.OnStateEnter(stateMachine);
        }

        /// <summary>
        /// Execute this transition
        /// </summary>
        /// <param name="machine"> The stateMachine to use </param>
        protected bool Execute(BaseStateMachine machine)
        {
            if (decisions.Count == 0) 
            {
                Debug.LogWarning("No decisions added to " + name);
                return false; 
            }
            if (decisions.Count == 1) 
            { 
                return decisions[0].decision.Decide(machine); 
            }

            //Calculate && results
            List<(bool, LogicalOperator)> andResults = new List<(bool, LogicalOperator)>();


            for (int i = 0; i < decisions.Count; i++)
            {
                andResults.Add((decisions[i].decision.Decide(machine), decisions[i].nextOperator));

                for (
                    int andIndex = i;
                    i + 1 < decisions.Count && decisions[i].nextOperator is LogicalOperator.And or LogicalOperator.Nand;
                    i++
                    )
                {
                    bool newValue = EvalueateOperator(decisions[i + 1].decision.Decide(machine), andResults[andIndex].Item1, andResults[andIndex].Item2);
                    andResults[andIndex] = (newValue, decisions[i + 1].nextOperator);
                }
            }

            // Calculate or results
            bool result = andResults[0].Item1;
            for (int i = 1; i < andResults.Count; i++)
            {
                result = EvalueateOperator(result, andResults[i].Item1, andResults[i - 1].Item2);
            }

            return result;
        }

        /// <summary>
        /// Evaluates a given boolean operator.
        /// </summary>
        /// <param name="a"> The first boolean input. </param>
        /// <param name="b"> The second boolean input. </param>
        /// <param name="logicalOperator"> The operator used to combine a & b. </param>
        /// <returns> The logical combination of a b. </returns>
        private bool EvalueateOperator(bool a, bool b, LogicalOperator logicalOperator)
        {
            switch (logicalOperator)
            {
                case LogicalOperator.Or:
                    return a || b;
                case LogicalOperator.And:
                    return a && b;
                case LogicalOperator.Nor:
                    return !a && !b;
                case LogicalOperator.Nand:
                    return !a || !b;
                default:
                    throw new System.Exception("Invalid operator during transition evaluation.");
            }
        }
    }
}