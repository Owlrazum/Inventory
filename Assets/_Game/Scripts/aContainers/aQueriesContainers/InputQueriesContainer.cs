using System;

public static class InputQueriesContainer
{
    #region MovementCommands
    public static Func<HorizontalMoveCommand> FuncHorizontalMoveCommand;
    public static HorizontalMoveCommand QueryHorizontalMoveCommand()
    { 
#if UNITY_EDITOR
        if (FuncHorizontalMoveCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncHorizontalMoveCommand.Invoke();
    }

    public static Func<VerticalMoveCommand> FuncVerticalMoveCommand;
    public static VerticalMoveCommand QueryVerticalMoveCommand()
    { 
#if UNITY_EDITOR
        if (FuncVerticalMoveCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncVerticalMoveCommand.Invoke();
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

    public static Func<DashCommand> FuncDashCommand;
    public static DashCommand QueryDashCommand()
    { 
#if UNITY_EDITOR
        if (FuncDashCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncDashCommand.Invoke();
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

    public static Func<WallRunCommand> FuncWallRunCommand;
    public static WallRunCommand QueryWallRunCommand()
    { 
#if UNITY_EDITOR
        if (FuncWallRunCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncWallRunCommand.Invoke();
    }
    #endregion//MovementCommands

    #region CameraAndPortalCommands
    public static Func<ShootPortalCommand> FuncShootPortalCommand;
    public static ShootPortalCommand QueryShootPortalCommand()
    { 
#if UNITY_EDITOR
        if (FuncShootPortalCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncShootPortalCommand.Invoke();
    }

        public static Func<LookUpCommand> FuncLookUpCommand;
    public static LookUpCommand QueryLookUpCommand()
    { 
#if UNITY_EDITOR
        if (FuncLookUpCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncLookUpCommand.Invoke();
    }

    public static Func<LookDownCommand> FuncLookDownCommand;
    public static LookDownCommand QueryLookDownCommand()
    { 
#if UNITY_EDITOR
        if (FuncLookDownCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncLookDownCommand.Invoke();
    }
    #endregion//CameraAndPortalCommands

    public static Func<PickUpItemCommand> FuncPickUpMovableCommand;
    public static PickUpItemCommand QueryPickUpItemCommand()
    { 
#if UNITY_EDITOR
        if (FuncPickUpMovableCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncPickUpMovableCommand.Invoke();
    }

    public static Func<RotateShellCommand> FuncRotateShellCommand;
    public static RotateShellCommand QueryRotateShellCommand()
    { 
#if UNITY_EDITOR
        if (FuncRotateShellCommand.GetInvocationList().Length != 1)
        {
            throw new NotSupportedException("There should be only one subscription");
        }
#endif

        return FuncRotateShellCommand.Invoke();
    }
}