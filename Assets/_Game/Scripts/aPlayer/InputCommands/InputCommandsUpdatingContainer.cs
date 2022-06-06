using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InputCommandsUpdatingContainer : MonoBehaviour
{
    #region MovementCommandsDeclaration
    private HorizontalMoveCommand _horizontalMoveCommand;
    private VerticalMoveCommand _verticalMoveCommand;

    private GlideCommand _glideCommand;
    private WallRunCommand _wallRunCommand;

    private JumpCommand _jumpCommand;
    private DashCommand _dashCommand;
    #endregion//MovementCommandsDeclaration

    #region CameraAndPortalCommandsDeclaration
    private ShootPortalCommand _shootPortalCommand;

    private LookUpCommand   _lookUpCommand;
    private LookDownCommand _lookDownCommand;
    #endregion//CameraAndPortalCommandsDeclaration

    private PickUpItemCommand _pickUpMovableCommand;
    private RotateShellCommand _rotateShellCommand;

    /// <summary>
    /// Initialize Input Commands through PlayerPrefs or other method
    /// </summary>
    private void Awake()
    {
        #region MovementCommandsInitialization
        _horizontalMoveCommand = new HorizontalMoveCommand();
        _horizontalMoveCommand.TriggeringKeyCodeToLeft = KeyCode.A;
        _horizontalMoveCommand.TriggeringKeyCodeToRight = KeyCode.D;

        _verticalMoveCommand = new VerticalMoveCommand();
        _verticalMoveCommand.TriggeringKeyCodeToDown = KeyCode.S;
        _verticalMoveCommand.TriggeringKeyCodeToUp = KeyCode.W;

        _glideCommand = new GlideCommand();
        _glideCommand.TriggeringKeyCode = KeyCode.LeftShift;

        _wallRunCommand = new WallRunCommand();
        _wallRunCommand.TriggeringKeyCode = KeyCode.LeftShift;

        _jumpCommand = new JumpCommand();
        _jumpCommand.TriggeringKeyCode = KeyCode.Space;

        _dashCommand = new DashCommand();
        _dashCommand.TriggeringKeyCode = KeyCode.Q;
        #endregion//MovementCommandsInitialization

        #region CameraAndPortalCommandsInitialization
        _shootPortalCommand = new ShootPortalCommand();
        _shootPortalCommand.IsMouseBased = true;
        _shootPortalCommand.IsBluePortalLeftMouseButton = true;

        _lookUpCommand = new LookUpCommand();
        _lookUpCommand.TriggeringKeyCode = KeyCode.W;

        _lookDownCommand = new LookDownCommand();
        _lookDownCommand.TriggeringKeyCode = KeyCode.S;
        #endregion//CameraAndPortalCommandsInitialization

        _pickUpMovableCommand = new PickUpItemCommand();
        _pickUpMovableCommand.TriggeringKeyCode = KeyCode.E;

        _rotateShellCommand = new RotateShellCommand();
        _rotateShellCommand.TriggeringKeyCode = KeyCode.R;
        _rotateShellCommand._lastTriggerTime = -1;

        #region MovementCommandsSubsciption
        InputQueriesContainer.FuncHorizontalMoveCommand  += GetHorizontalMoveCommand;
        InputQueriesContainer.FuncVerticalMoveCommand  += GetVerticalMoveCommand;

        InputQueriesContainer.FuncGlideCommand += GetGlideCommand;
        InputQueriesContainer.FuncWallRunCommand += GetWallRunCommand;

        InputQueriesContainer.FuncJumpCommand  += GetJumpCommand;
        InputQueriesContainer.FuncDashCommand  += GetDashCommand;
        #endregion//MovementCommandsSubsciption

        InputQueriesContainer.FuncShootPortalCommand += GetShootPortalCommand;

        InputQueriesContainer.FuncLookUpCommand += GetLookUpCommand;
        InputQueriesContainer.FuncLookDownCommand += GetLookDownCommand;

        InputQueriesContainer.FuncPickUpMovableCommand += GetPickUpMovableCommand;
        InputQueriesContainer.FuncRotateShellCommand   += GetRotateShellCommand;
    }

    private void OnDestroy()
    {
        #region MovementCommandsUnsubsciption
        InputQueriesContainer.FuncHorizontalMoveCommand  -= GetHorizontalMoveCommand;
        InputQueriesContainer.FuncVerticalMoveCommand    -= GetVerticalMoveCommand;

        InputQueriesContainer.FuncGlideCommand -= GetGlideCommand;
        InputQueriesContainer.FuncWallRunCommand -= GetWallRunCommand;

        InputQueriesContainer.FuncJumpCommand  -= GetJumpCommand;
        InputQueriesContainer.FuncDashCommand  -= GetDashCommand;
        #endregion//MovementCommandsUnsubsciption

        InputQueriesContainer.FuncShootPortalCommand -= GetShootPortalCommand;

        InputQueriesContainer.FuncLookUpCommand -= GetLookUpCommand;
        InputQueriesContainer.FuncLookDownCommand -= GetLookDownCommand;

        InputQueriesContainer.FuncPickUpMovableCommand -= GetPickUpMovableCommand;
        InputQueriesContainer.FuncRotateShellCommand   -= GetRotateShellCommand;
    }

    #region MovementCommandsGetterMethods
    private HorizontalMoveCommand GetHorizontalMoveCommand()
    {
        return _horizontalMoveCommand;
    }

    private VerticalMoveCommand GetVerticalMoveCommand()
    {
        return _verticalMoveCommand;
    }

    private GlideCommand GetGlideCommand()
    {
        return _glideCommand;
    }

    private WallRunCommand GetWallRunCommand()
    {
        return _wallRunCommand;
    }

    private JumpCommand GetJumpCommand()
    {
        return _jumpCommand;
    }

    private DashCommand GetDashCommand()
    {
        return _dashCommand;
    }
    #endregion//MovementCommandsGetterMethods

    #region CameraAndPortalCommandsGetterMethods
    private ShootPortalCommand GetShootPortalCommand()
    {
        return _shootPortalCommand;
    }

    private LookUpCommand GetLookUpCommand()
    {
        return _lookUpCommand;
    }

    private LookDownCommand GetLookDownCommand()
    {
        return _lookDownCommand;
    }
    #endregion//CameraAndPortalCommandsGetterMethods

    private PickUpItemCommand GetPickUpMovableCommand()
    {
        return _pickUpMovableCommand;
    }

    private RotateShellCommand GetRotateShellCommand()
    {
        return _rotateShellCommand;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        
        ResetCommands();

        #region MovementCommandsUpdateChecking
        if (Input.GetKey(_horizontalMoveCommand.TriggeringKeyCodeToLeft))// || true)
        { 
            _horizontalMoveCommand.IsTriggered = true;
            _horizontalMoveCommand.CommandingToLeft  = true;
        }
        else if (Input.GetKey(_horizontalMoveCommand.TriggeringKeyCodeToRight))
        {
            _horizontalMoveCommand.IsTriggered = true;
            _horizontalMoveCommand.CommandingToRight = true;
        }

        if (Input.GetKey(_verticalMoveCommand.TriggeringKeyCodeToDown))
        {
            _verticalMoveCommand.IsTriggered = true;
            _verticalMoveCommand.CommandingToDown = true;
        }
        else if (Input.GetKey(_verticalMoveCommand.TriggeringKeyCodeToUp))
        {
            _verticalMoveCommand.IsTriggered = true;
            _verticalMoveCommand.CommandingToUp = true;
        }

        if (Input.GetKey(_glideCommand.TriggeringKeyCode)) // Similar to working
        {
            _glideCommand.IsTriggered = true;
        }

        if (Input.GetKey(_wallRunCommand.TriggeringKeyCode))
        {
            if (Input.GetKey(_horizontalMoveCommand.TriggeringKeyCodeToLeft))
            {
                _wallRunCommand.IsTriggered = true;
                _wallRunCommand.CommandingToLeft = true;
            }
            else if (Input.GetKey(_horizontalMoveCommand.TriggeringKeyCodeToRight))
            {
                _wallRunCommand.IsTriggered = true;
                _wallRunCommand.CommandingToRight = true;
            }
        }

        if (Input.GetKeyDown(_jumpCommand.TriggeringKeyCode))
        {
            _jumpCommand.IsTriggered = true;
        }

        if (Input.GetKeyDown(_dashCommand.TriggeringKeyCode))
        {
            _dashCommand.IsTriggered = true;
        }
        #endregion//MovementCommandsUpdateChecking
    
        #region CameraAndPortalCommandsUpdateChecking
        if (Input.GetKey(_lookUpCommand.TriggeringKeyCode))
        {
            _lookUpCommand.IsTriggered = true;
        }

        if (Input.GetKey(_lookDownCommand.TriggeringKeyCode))
        {
            _lookDownCommand.IsTriggered = true;
        }

        if (_shootPortalCommand.IsMouseBased)
        {
            if (_shootPortalCommand.IsBluePortalLeftMouseButton && 
                Input.GetMouseButtonDown(0) 
                ||
                !_shootPortalCommand.IsBluePortalLeftMouseButton && 
                Input.GetMouseButtonDown(1))
            {
                _shootPortalCommand.IsTriggered = true;
                _shootPortalCommand.IsBluePortalCommanded = true;
                _shootPortalCommand.mousePosition = Input.mousePosition;
            }
            else if (_shootPortalCommand.IsBluePortalLeftMouseButton && 
                Input.GetMouseButtonDown(1) 
                ||
                !_shootPortalCommand.IsBluePortalLeftMouseButton && 
                Input.GetMouseButtonDown(0))
            {
                _shootPortalCommand.IsTriggered = true;
                _shootPortalCommand.IsRedPortalCommanded = true;
                _shootPortalCommand.mousePosition = Input.mousePosition;
            }
        }
        #endregion//CameraAndPortalCommandsUpdateChecking
    
        if (Input.GetKeyDown(_pickUpMovableCommand.TriggeringKeyCode))
        {
            _pickUpMovableCommand.IsTriggered = true;
        }

        if (Input.GetKeyDown(_rotateShellCommand.TriggeringKeyCode))
        {
            _rotateShellCommand.IsTriggered = true;
            _rotateShellCommand.RotationMode = RotateShellCommand.Mode.Pitch90;

            // if (_rotateShellCommand._lastTriggerTime >= 0)
            // {
            //     _rotateShellCommand.IsTriggered = true;
            //     _rotateShellCommand.RotationMode = RotateShellCommand.Mode.Yaw180;
            //     _rotateShellCommand._lastTriggerTime = -1;
            // }
            // else
            // {
            //     _rotateShellCommand._lastTriggerTime = 0;
            // }
        }

        // if (_rotateShellCommand._lastTriggerTime >= 0)
        // {
        //     _rotateShellCommand._lastTriggerTime += Time.deltaTime;
        //     if (_rotateShellCommand._lastTriggerTime >= _doublePressPauseTime)
        //     {
        //         _rotateShellCommand.IsTriggered = true;
        //         _rotateShellCommand.RotationMode = RotateShellCommand.Mode.Pitch90;
        //         _rotateShellCommand._lastTriggerTime = -1;
        //     }
        // }
    }

    private void ResetCommands()
    { 
        #region MovementCommandsResetting
        _horizontalMoveCommand.IsTriggered = false;
        _horizontalMoveCommand.CommandingToLeft = false;
        _horizontalMoveCommand.CommandingToRight = false;

        _verticalMoveCommand.IsTriggered = false;
        _verticalMoveCommand.CommandingToDown = false;
        _verticalMoveCommand.CommandingToUp = false;

        _glideCommand.IsTriggered = false;

        _wallRunCommand.IsTriggered = false;
        _wallRunCommand.CommandingToLeft = false;
        _wallRunCommand.CommandingToRight = false;

        _jumpCommand.IsTriggered = false;
        _jumpCommand.IsConsumed = false;

        _dashCommand.IsTriggered = false;
        #endregion//MovementCommandsResetting
        
        _shootPortalCommand.IsTriggered = false;
        _shootPortalCommand.IsBluePortalCommanded = false;
        _shootPortalCommand.IsRedPortalCommanded = false;

        _lookUpCommand.IsTriggered = false;
        _lookDownCommand.IsTriggered = false;

        _pickUpMovableCommand.IsTriggered = false;
        _rotateShellCommand.IsTriggered = false;
    }
}