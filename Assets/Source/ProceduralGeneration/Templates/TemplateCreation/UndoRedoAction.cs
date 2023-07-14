using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that tracks an action performed and the action needed to undo it
/// </summary>
public class UndoRedoAction
{
    // The action that would need to happen to redo what just happened
    public System.Action redoAction;

    // The action that would need to happen to undo what just happened
    public System.Action undoAction;

    // Any created or deleted objects that these actions have to do with
    public List<GameObject> relevantObjects;

    /// <summary>
    /// Initializes the list of relevant objects
    /// </summary>
    public UndoRedoAction()
    {
        relevantObjects = new List<GameObject>();
    }
}
