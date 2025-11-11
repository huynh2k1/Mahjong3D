using System;
using UnityEngine;
using UnityEngine.UI;

public class LosePU : PU
{
    public override UIType Type => UIType.Lose;

    [SerializeField] Button replay, home;
    [SerializeField] Text textScore;
    [SerializeField] Text textHighScore;

    public static Action ClickReplayAction;
    public static Action ClickHomeAction;



    protected override void Awake()
    {
        base.Awake();
        replay.onClick.AddListener(HandleReplayAction);
        home.onClick.AddListener(HandleHomeAction);
    }

    public override void Show()
    {
        base.Show();
        UpdateTextScore();
        UpdateTextHighScore();
    }

    void HandleReplayAction()
    {
        Hide();
        ClickReplayAction?.Invoke();
    }

    void HandleHomeAction()
    {
        Hide();
        ClickHomeAction?.Invoke();
    }

    void UpdateTextScore()
    {
        textScore.text = $"Score: {Data.Score}";
    }

    void UpdateTextHighScore()
    {
        textHighScore.text = $"Best Score: {Data.Score}";
    }
}
