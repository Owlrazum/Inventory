using System;

public class AnimationBranch : AnimationNode
{
    private Func<AnimationNode> _resolver;

    public AnimationBranch(Func<AnimationNode> resolverArg)
    {
        _resolver = resolverArg;
    }

    public override AnimationNode Resolve()
    {
        return _resolver.Invoke();
    }
}