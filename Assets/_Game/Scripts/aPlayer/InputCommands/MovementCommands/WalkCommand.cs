using UnityEngine;

public class WalkCommand : InputCommand
{ 
    public float Vertical { get; set; }
    public float Horizontal { get; set; }

    public KeyCode UpKeyCode;
    public KeyCode RightKeyCode;
    public KeyCode DownKeyCode;
    public KeyCode LeftKeyCode;
}