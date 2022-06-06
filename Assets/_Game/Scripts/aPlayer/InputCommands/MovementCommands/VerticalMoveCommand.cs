using UnityEngine;

/// <summary>
/// Does not use TriggeringKeyCode;
/// </summary>
public class VerticalMoveCommand : InputCommand
{
    public KeyCode TriggeringKeyCodeToUp  { get; set; }
    public KeyCode TriggeringKeyCodeToDown { get; set; }

    public bool CommandingToUp  { get; set; }
    public bool CommandingToDown { get; set; }
}