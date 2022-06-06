using UnityEngine;

public class UIInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIEventsContainer.EscapePressed?.Invoke();
        }
    }
}