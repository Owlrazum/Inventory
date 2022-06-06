using UnityEngine;

public class ShootPortalCommand : InputCommand
{
    public bool IsMouseBased { get; set; }

    public bool IsBluePortalLeftMouseButton { get; set; }

    public bool IsBluePortalCommanded { get; set; }
    public bool IsRedPortalCommanded { get; set; }

    public Vector3 mousePosition { get; set; }
}