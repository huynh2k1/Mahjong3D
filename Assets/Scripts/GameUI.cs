using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : UI
{
    public override UIType Type => UIType.Game;

    [SerializeField] Text textLevel;
    [SerializeField] Button home;
    [SerializeField] Button replay;

    public static Action ClickHomeAction;
    public static Action ClickReplayAction;


    private void Awake()
    {
        home.onClick.AddListener(HandleClickHome);
        replay.onClick.AddListener(HandleClickReplay);
    }

    void HandleClickHome()
    {
        ClickHomeAction?.Invoke();  
    }

    void HandleClickReplay()
    {
        ClickReplayAction?.Invoke();
    }

    public void UpdateTextLevel()
    {
        textLevel.text = $"Level {Data.CurLevel + 1}";
    }
}
