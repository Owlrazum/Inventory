using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    protected void Awake()
    {
        PlayerQueriesContainer.FuncTransform += GetTransform;
        PlayerQueriesContainer.QueryTransform();
        PlayerQueriesContainer.FuncPlayerCharacterInstance += GetPlayerCharacterInstance;
    }

    private void OnDestroy()
    {
        PlayerQueriesContainer.FuncTransform -= GetTransform;
        PlayerQueriesContainer.FuncPlayerCharacterInstance -= GetPlayerCharacterInstance;
    }

    #region EventsHandling
    #endregion

    #region Getters
    private Transform GetTransform()
    {
        return transform;
    }

    private PlayerCharacter GetPlayerCharacterInstance()
    {
        return this;
    }
    #endregion
}
