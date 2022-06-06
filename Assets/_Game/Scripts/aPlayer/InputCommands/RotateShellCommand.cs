public class RotateShellCommand : InputCommand
{
    public enum Mode
    { 
        Pitch90,
        //Yaw180
    }

    public Mode RotationMode { get; set; }
    
    // Input Update use only 
    public float _lastTriggerTime;
}