using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UI[] _arrUI;
    protected Dictionary<UIType, UI> _uis = new Dictionary<UIType, UI>();

    protected virtual void Awake()
    {
        foreach (var ui in _arrUI)
        {
            _uis[ui.Type] = ui;
        }
    }

    private void OnEnable()
    {
        HomeUI.ClickHtpAction += ShowHowToPlay;
    }

    private void OnDestroy()
    {
        HomeUI.ClickHtpAction -= ShowHowToPlay; 
    }

    public void Show(UIType type)
    {
        if (!_uis.ContainsKey(type))
        {
            return;
        }
        _uis[type].Show();
    }

    public void Hide(UIType type)
    {
        if (!_uis.ContainsKey(type))
        {
            return;
        }
        _uis[type].Hide();
    }

    public void ShowHowToPlay()
    {
        Show(UIType.HowToPlay);
    }
}
