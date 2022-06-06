using System;

using UnityEngine;

public class PuzzleCauseReactionable : MonoBehaviour
{
    /// <summary>
    /// Callback should be called when the reaction is finished.
    /// </summary>
    public virtual void ReactToOneWayOneTimePuzzleCause(Func<bool> reactCompleteCallback)
    { 

    }

    /// <summary>
    /// Callback should be called when the reaction is finished.
    /// </summary>
    public virtual void ReactToTwoWayMultipleTimesPuzzleCause(Func<bool> reactCompleteCallback)
    { 

    }
}