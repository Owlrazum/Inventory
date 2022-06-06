using UnityEngine;

public class AnimationLeaf : AnimationNode
{
    private Animator[] _animatorReferences;
    private int[] _stateHashes;
    private const float FADE_DURATION = 0.1f;
    public AnimationLeaf(Animator[] animatorReferencesArg, int[] stateHashesArg)
    {
        _animatorReferences = animatorReferencesArg;
        _stateHashes = stateHashesArg;
    }

    public override AnimationNode Resolve()
    {
        for (int i = 0; i < _animatorReferences.Length; i++)
        {
            //_animatorReferences[i].CrossFade();
        }

        return null;
    }
}