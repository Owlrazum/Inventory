using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InputCommandsSystem : MonoBehaviour
{
    private WalkCommand _walkCommand;
    private InventoryCommand _inventoryCommand;

    private GlideCommand _glideCommand;

    private JumpCommand _jumpCommand;

    private PickUpItemCommand _pickUpItemCommand;

    /// <summary>
    /// Initialize Input Commands through PlayerPrefs or other method
    /// </summary>
    private void Awake()
    {
        _walkCommand = new WalkCommand();
        _walkCommand.UpKeyCode    = KeyCode.W;
        _walkCommand.RightKeyCode = KeyCode.D;
        _walkCommand.DownKeyCode  = KeyCode.S;
        _walkCommand.LeftKeyCode  = KeyCode.A;

        _inventoryCommand = new InventoryCommand();
        _inventoryCommand.TriggeringKeyCode = KeyCode.E;

        _glideCommand = new GlideCommand();
        _glideCommand.TriggeringKeyCode = KeyCode.LeftShift;

        _jumpCommand = new JumpCommand();
        _jumpCommand.TriggeringKeyCode = KeyCode.Space;

        _pickUpItemCommand = new PickUpItemCommand();
        _pickUpItemCommand.TriggeringKeyCode = KeyCode.E;

        InputDelegatesContainer.FuncWalkCommand  += GetWalkCommand;

        InputDelegatesContainer.FuncGlideCommand += GetGlideCommand;
        InputDelegatesContainer.FuncJumpCommand  += GetJumpCommand;

        InputDelegatesContainer.FuncPickUpItemCommand += GetPickUpItemCommand;
    }

    private void OnDestroy()
    {
        InputDelegatesContainer.FuncWalkCommand  -= GetWalkCommand;

        InputDelegatesContainer.FuncGlideCommand -= GetGlideCommand;
        InputDelegatesContainer.FuncJumpCommand  -= GetJumpCommand;

        InputDelegatesContainer.FuncPickUpItemCommand -= GetPickUpItemCommand;
    }

    #region MovementCommandsGetterMethods
    private WalkCommand GetWalkCommand()
    {
        return _walkCommand;
    }

    private InventoryCommand GetInventoryCommand()
    {
        return _inventoryCommand;
    }

    private GlideCommand GetGlideCommand()
    {
        return _glideCommand;
    }

    private JumpCommand GetJumpCommand()
    {
        return _jumpCommand;
    }
    #endregion//MovementCommandsGetterMethods

    private PickUpItemCommand GetPickUpItemCommand()
    {
        return _pickUpItemCommand;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        ResetCommands();

        UpdateWalkCommand();

        if (Input.GetKeyDown(_inventoryCommand.TriggeringKeyCode))
        {
            InputDelegatesContainer.EventInventoryCommandTriggered?.Invoke();
        }

        if (Input.GetKey(_glideCommand.TriggeringKeyCode)) // Similar to working
        {
            _glideCommand.IsTriggered = true;
        }

        if (Input.GetKeyDown(_jumpCommand.TriggeringKeyCode))
        {
            _jumpCommand.IsTriggered = true;
        }

        if (Input.GetKeyDown(_pickUpItemCommand.TriggeringKeyCode))
        {
            _pickUpItemCommand.IsTriggered = true;
        }

    }

    private void UpdateWalkCommand()
    {
        if (Input.GetKey(_walkCommand.UpKeyCode))
        {
            _walkCommand.Vertical = 1;
        }

        if (Input.GetKey(_walkCommand.RightKeyCode))
        {
            _walkCommand.Horizontal = 1;
        }

        if (Input.GetKey(_walkCommand.DownKeyCode))
        {
            _walkCommand.Vertical = -1;
        }

        if (Input.GetKey(_walkCommand.LeftKeyCode))
        {
            _walkCommand.Horizontal = -1;
        }
    }

    private void ResetCommands()
    {
        _walkCommand.Vertical = 0;
        _walkCommand.Horizontal = 0;

        _glideCommand.IsTriggered = false;
        _jumpCommand.IsTriggered = false;
        _pickUpItemCommand.IsTriggered = false;
    }
}