using UnityEngine;

using TMPro;

public class UIBuildLogCanvas : UIBaseFadingCanvas
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private int _maxLogCount = 4;

    private int _logCounter;

    protected override void Awake()
    {
        base.Awake();
        UIEventsContainer.EventBuildLog += OnEventBuildLog;
    }

    private void OnDestroy()
    { 
        UIEventsContainer.EventBuildLog -= OnEventBuildLog;
    }

    private void OnEventBuildLog(string msg)
    {
        if (_logCounter > _maxLogCount)
        {
            _text.text = "";
            _logCounter = 0;
        }
        
        _text.text += " " + msg + "\n";
        _logCounter++;
    }
}