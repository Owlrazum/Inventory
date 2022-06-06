using UnityEngine;

/// <summary>
/// Specific methods should be implemented and called depending on the classes itself;
/// Turns out that a complete generic too complex.
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// So the object pool itself will manage positioning and parenting
    /// </summary>
    public Transform GetTransform();
}
