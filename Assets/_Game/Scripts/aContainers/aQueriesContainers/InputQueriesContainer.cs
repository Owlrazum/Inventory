using System;

public static class InputQueriesContainer
{
    #region MovementCommands
    public static Func<WalkCommand> FuncWalkCommand;
    public static WalkCommand QueryWalkCommand()
    { 
#if UNITY_EDITOR
        if (FuncWalkCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncWalkCommand.Invoke();
    }

    public static Func<InventoryCommand> FuncInventoryCommand;
    public static InventoryCommand QueryInventoryCommand()
    { 
#if UNITY_EDITOR
        if (FuncInventoryCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncInventoryCommand.Invoke();
    }

    public static Func<JumpCommand> FuncJumpCommand;
    public static JumpCommand QueryJumpCommand()
    { 
#if UNITY_EDITOR
        if (FuncJumpCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncJumpCommand.Invoke();
    }

    public static Func<GlideCommand> FuncGlideCommand;
    public static GlideCommand QueryGlideCommand()
    { 
#if UNITY_EDITOR
        if (FuncGlideCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncGlideCommand.Invoke();
    }

    #endregion//MovementCommands

    public static Func<PickUpItemCommand> FuncPickUpItemCommand;
    public static PickUpItemCommand QueryPickUpItemCommand()
    { 
#if UNITY_EDITOR
        if (FuncPickUpItemCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncPickUpItemCommand.Invoke();
    }
}