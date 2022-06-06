using System;

public interface IPuzzleHealth
{
    public void OnHealthLost();
    public void AssignSpawnCallBack(Action callBack);
}