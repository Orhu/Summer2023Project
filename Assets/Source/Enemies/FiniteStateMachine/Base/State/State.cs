using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;

 namespace Cardificer.FiniteStateMachine
 {
     /// <summary>
     /// Represents a state in a finite state machine. Performs actions and transitions as needed in this state.
     /// </summary>
     [CreateAssetMenu(menuName = "FSM/State")]
     public sealed class State : ScriptableObject
     {
         [Tooltip("Actions run when the state is entered.")] [EditInline]
         public List<BaseAction> enterActions = new List<BaseAction>();

         [Tooltip("Actions run when the state is exited.")] [EditInline]
         public List<BaseAction> exitActions = new List<BaseAction>();

         [Tooltip("Actions run every frame when in this state.")] [EditInline]
         public List<BaseAction> updateActions = new List<BaseAction>();

         [Tooltip("This state's transitions. Evaluated every frame.")] [EditInline]
         public List<Transition> transitions = new List<Transition>();

         /// <summary>
         /// Run through all of this state's actions and transitions, executing them.
         /// </summary>
         /// <param name="machine"> The state machine to be used. </param>
         public void OnStateUpdate(BaseStateMachine machine)
         {
             foreach (var action in updateActions)
             {
                 action.Execute(machine);
             }

             foreach (var transition in transitions)
             {
                 transition.Evaluate(machine);
             }
         }

         /// <summary>
         /// Runs through all of this state's actions, executing their OnStateEnter methods.
         /// </summary>
         /// <param name="stateMachine"> The state machine to be used. </param>
         public void OnStateEnter(BaseStateMachine stateMachine)
         {
             foreach (var action in enterActions)
             {
                 stateMachine.cooldownData.cooldownReady.TryAdd(action, true);
                 action.Execute(stateMachine);
             }
         }

         /// <summary>
         /// Runs through all of this state's actions, executing their OnStateExit methods.
         /// </summary>
         /// <param name="stateMachine"> The state machine to be used. </param>
         public void OnStateExit(BaseStateMachine stateMachine)
        {
            foreach (var action in exitActions)
             {
                 action.Execute(stateMachine);
             }
         }
     }
 }