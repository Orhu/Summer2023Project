using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A list of modifier types that will be filtered out.
    /// </summary>
    [CreateAssetMenu(menuName = "Cards/Modifier Filter")]
    public class ModifierFilter : ScriptableObject
    {
        [Tooltip("A lits of the class names of modifiers to not apply to this action.")]
        [SerializeField] private string[] forbiddenModifiers = new string[0];

        // The forbidden modifiers converted to System.Type
        private Type[] _forbiddenModifierTypes;
        private Type[] forbiddenModifierTypes
        {
            get
            {
                if (_forbiddenModifierTypes == null)
                {
                    _forbiddenModifierTypes = new Type[forbiddenModifiers.Length];

                    for (int i = 0; i < _forbiddenModifierTypes.Length; i++)
                    {
                        _forbiddenModifierTypes[i] = Type.GetType($"Cardificer.{forbiddenModifiers[i]}");
                        if (_forbiddenModifierTypes[i] == null)
                        {
                            throw new Exception($"Error: Can't find modifier with name: {forbiddenModifiers[i]} which is listed in {name}.");
                        }
                    }
                }

                return _forbiddenModifierTypes;
            }
        }

        /// <summary>
        /// Filters a list of modifiers based off of this
        /// </summary>
        /// <param name="modifiers"> The modifier list to filter. </param>
        /// <returns> The list of modifiers with all modifiers who's types are forbidden removed. </returns>
        public List<AttackModifier> FilterModifierList(List<AttackModifier> modifiers)
        {
            return modifiers.Where(IsModifierAllowed).ToList();
        }

        /// <summary>
        /// Gets whether a modifier is allowed by this filter.
        /// </summary>
        /// <param name="modifier"> The modifier to test. </param>
        /// <returns> True if the modifier is allowed. </returns>
        public bool IsModifierAllowed(AttackModifier modifier)
        {
            foreach (Type forbiddenModifierType in forbiddenModifierTypes)
            {
                if (forbiddenModifierType.IsInstanceOfType(modifier)) { return false; }
            }
            return true;
        }
    }
}