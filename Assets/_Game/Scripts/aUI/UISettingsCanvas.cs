public class UISettingsCanvas : UIBaseFadingCanvas
{
    private UIButton _goBackButton;

    private bool _isEnteredSettings;

    protected override void Awake()
    {
        base.Awake();

        UIEventsContainer.EventSettingsEnter += OnSettingsEnter;
        UIEventsContainer.EventSettingsExit += OnSettingsExit;

        if (!transform.GetChild(0).TryGetComponent(out _goBackButton))
        {
            UIEventsContainer.EventBuildLog("The first child should be goback button");
        }

        _goBackButton.EventOnClick += OnGoBackPress;
    }

    private void OnDestroy()
    {
        UIEventsContainer.EventSettingsEnter -= OnSettingsEnter;
        UIEventsContainer.EventSettingsExit -= OnSettingsExit;

        _goBackButton.EventOnClick -= OnGoBackPress;

        if (_isEnteredSettings)
        {
            UIEventsContainer.EscapePressed -= OnGoBackPress;
        }
    }

    private void OnSettingsEnter()
    {
        _isEnteredSettings = true;
        ShowItself();
        UIEventsContainer.EscapePressed += OnGoBackPress;
    }

    private void OnGoBackPress()
    {
        UIEventsContainer.EventSettingsExit?.Invoke();
    }

    private void OnSettingsExit()
    {
        _isEnteredSettings = false;
        HideItself();
        UIEventsContainer.EscapePressed -= OnGoBackPress;
    }
}