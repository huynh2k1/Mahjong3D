using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UI
{
    public override UIType Type => UIType.Home;

    [SerializeField] Button play;
    [SerializeField] Button htp;

    public static Action ClickPlayAction;
    public static Action ClickHtpAction;

    private void Awake()
    {
        play.onClick.AddListener(HandleClickPlayAction);
        htp.onClick.AddListener(HandleClickHtpAction);
    }

    void HandleClickPlayAction()
    {
        ClickPlayAction?.Invoke();
    }

    void HandleClickHtpAction()
    {
        ClickHtpAction?.Invoke();   
    }
}
