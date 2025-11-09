using System;
using UnityEngine;
using UnityEngine.UI;

public class WinPU : PU
{
    public override UIType Type => UIType.Win;

    [SerializeField] Button replay, home, next;

    public static Action ClickReplayAction;
    public static Action ClickHomeAction;
    public static Action ClickNextAction;

    protected override void Awake()
    {
        base.Awake();
        replay.onClick.AddListener(HandleReplayAction);
        home.onClick.AddListener(HandleHomeAction);
        next.onClick.AddListener(HandleNextAction);
    }


    void HandleReplayAction()
    {
        ClickReplayAction?.Invoke();
    }

    void HandleHomeAction()
    {
        ClickHomeAction?.Invoke();
    }

    void HandleNextAction()
    {
        ClickNextAction?.Invoke();
    }
}
