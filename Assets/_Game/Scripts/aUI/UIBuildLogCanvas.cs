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
        UIDelegatesContainer.BuildLog += BuildLog;
    }

    private void OnDestroy()
    { 
        UIDelegatesContainer.BuildLog -= BuildLog;
    }

    private void BuildLog(string msg)
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