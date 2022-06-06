using UnityEngine;

/// <summary>
/// Does not use TriggeringKeyCode;
/// </summary>
public class HorizontalMoveCommand : InputCommand
{
    public KeyCode TriggeringKeyCodeToLeft  { get; set; }
    public KeyCode TriggeringKeyCodeToRight { get; set; }

    public bool CommandingToLeft  { get; set; }
    public bool CommandingToRight { get; set; }
}