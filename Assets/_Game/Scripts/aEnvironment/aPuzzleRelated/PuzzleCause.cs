using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCause : MonoBehaviour
{ 
    [SerializeField]
    private List<PuzzleCauseReactionable> _reactables;

    protected enum PuzzleCauseType
    {
        OneWayOneTime,
        TwoWayMultipleTime
    }

    [SerializeField]
    protected PuzzleCauseType _type;

    private Func<bool> _finishedReactionDelegate;
    private int _finishedReactionsCount;

    protected virtual void Awake()
    {
        _finishedReactionDelegate += OnFinishedReaction;
    }

    protected void NotifyReactables()
    { 
        _finishedReactionsCount = 0;

        switch (_type)
        {
            case PuzzleCauseType.OneWayOneTime:
                foreach (var reactable in _reactables)
                {
                    reactable.ReactToOneWayOneTimePuzzleCause(_finishedReactionDelegate);
                }
                break;
            case PuzzleCauseType.TwoWayMultipleTime:
                foreach (var reactable in _reactables)
                {
                    reactable.ReactToTwoWayMultipleTimesPuzzleCause(_finishedReactionDelegate);
                }
                break;
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>Is exceeded</returns>
    protected virtual bool OnFinishedReaction()
    {
        _finishedReactionsCount++;
        if (_finishedReactionsCount >= _reactables.Count)
        { 
            _finishedReactionsCount = 0;
            return true;
        }
        return false;
    }
}