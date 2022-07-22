using UnityEngine;

[CreateAssetMenu(fileName = "GameDesciption", menuName = "Crafting/GameDesciption", order = 1)]
public class GameDesciptionSO : ScriptableObject
{
    public LevelDescriptionSO[] Levels;
}