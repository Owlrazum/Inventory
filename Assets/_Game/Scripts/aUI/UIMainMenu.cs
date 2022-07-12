using UnityEngine;

public class UIMainMenu : UIBaseFadingCanvas
{
    private UIButton _settingsButton;

    private enum State
    { 
        Hided,
        Entered
    }

    private State _state;

    protected override void Awake()
    {
        base.Awake();
        
        UIEventsContainer.EscapePressed += OnEscapePressed;
        UIEventsContainer.EventSettingsExit += OnSettingsExit;

        if (!transform.GetChild(0).TryGetComponent(out _settingsButton))
        {
            UIEventsContainer.EventBuildLog("The first child should be goback button");
        }

        _state = State.Hided;

        _settingsButton.EventOnTouch += OnSettingsButtonPress;
    }

    private void OnDestroy()
    { 
        UIEventsContainer.EscapePressed -= OnEscapePressed;
        UIEventsContainer.EventSettingsExit -= OnSettingsExit;
        
        _settingsButton.EventOnTouch -= OnSettingsButtonPress;
    }

    private void OnEscapePressed()
    { 
        if (_state == State.Hided)
        {
            _state = State.Entered;
            ShowItself();
            Time.timeScale = 0;
        }
        else
        {
            _state = State.Hided;
            HideItself();
            Time.timeScale = 1;
        }
    }

    private void OnSettingsButtonPress()
    {
        UIEventsContainer.EventSettingsEnter?.Invoke();
        UIEventsContainer.EscapePressed -= OnEscapePressed;
        _state = State.Hided;
        HideItself();
    }

    private void OnSettingsExit()
    {
        UIEventsContainer.EscapePressed += OnEscapePressed;
        _state = State.Entered;
        ShowItself();
    }
}