using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    [SerializeField] private UIManager ui;
    [SerializeField] private LevelControl levelCtrl;
    [SerializeField] private GameUI gameUI;
    [SerializeField] private TimeCtrl timeCtrl;
    [SerializeField] private ScoreCtrl scoreCtrl;
    public GameState CurState { get; private set; }
    private void Awake()
    {
        I = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        HomeScene();
    }

    private void OnEnable()
    {
        HomeUI.ClickPlayAction += PlayGame;

        GameUI.ClickHomeAction += HomeScene;
        GameUI.ClickReplayAction += ReplayGame; 

        WinPU.ClickHomeAction += HomeScene;
        WinPU.ClickReplayAction += ReplayGame;
        WinPU.ClickNextAction += NextLevel;

        LosePU.ClickHomeAction += HomeScene;    
        LosePU.ClickReplayAction += ReplayGame;
    }

    private void OnDestroy()
    {
        HomeUI.ClickPlayAction -= PlayGame; 

        GameUI.ClickHomeAction -= HomeScene;
        GameUI.ClickReplayAction -= ReplayGame;

        WinPU.ClickHomeAction -= HomeScene;
        WinPU.ClickReplayAction -= ReplayGame;
        WinPU.ClickNextAction -= NextLevel; 

        LosePU.ClickHomeAction -= HomeScene;
        LosePU.ClickReplayAction -= ReplayGame;
    }

    void HomeScene()
    {
        ChangeState(GameState.None);
        ui.Show(UIType.Home);
        ui.Hide(UIType.Game);
    }

    void PlayGame()
    {
        scoreCtrl.InitScore();
        ChangeState(GameState.Play);
        levelCtrl.InitLevel();
        ui.Hide(UIType.Home);
        ui.Show(UIType.Game);
        gameUI.UpdateTextLevel();
        CameraCtrl.I.ResetCamera();
        timeCtrl.StartCountDown();
    }

    void ReplayGame()
    {
        ui.Show(UIType.Game);
        scoreCtrl.InitScore();
        timeCtrl.StartCountDown();
        gameUI.UpdateTextLevel();
        CameraCtrl.I.ResetCamera();
        ChangeState(GameState.Play);
        levelCtrl.InitLevel();
    }

    public void WinGame()
    {
        timeCtrl.StopCountDown();
        ChangeState(GameState.None);
        ui.Hide(UIType.Game);
        DOVirtual.DelayedCall(1f, () =>
        {
            SoundCtrl.I.PlaySound(TypeSound.WIN);
            ui.Show(UIType.Win);
        });
    }

    public void LoseGame()
    {
        timeCtrl.StopCountDown();
        ChangeState(GameState.None);
        ui.Show(UIType.Lose);
    }

    public void NextLevel()
    {
        levelCtrl.CheckIncreaseLevel();
        PlayGame();
    }

    public void ChangeState(GameState newState)
    {
        CurState = newState;
    }

    public void AddScore(int score)
    {
        scoreCtrl.AddScore(score);
    }
}

public enum GameState
{
    None,
    Play
}
