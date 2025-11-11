using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    [SerializeField] private UIManager ui;
    [SerializeField] private LevelControl levelCtrl;
    [SerializeField] private GameUI gameUI;
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
    }

    private void OnDestroy()
    {
        HomeUI.ClickPlayAction -= PlayGame; 

        GameUI.ClickHomeAction -= HomeScene;
        GameUI.ClickReplayAction -= ReplayGame;

        WinPU.ClickHomeAction -= HomeScene;
        WinPU.ClickReplayAction -= ReplayGame;
        WinPU.ClickNextAction -= NextLevel; 
    }

    void HomeScene()
    {
        ChangeState(GameState.None);
        ui.Show(UIType.Home);
        ui.Hide(UIType.Game);
    }

    void PlayGame()
    {
        ChangeState(GameState.Play);
        levelCtrl.InitLevel();
        ui.Hide(UIType.Home);
        ui.Show(UIType.Game);
        gameUI.UpdateTextLevel();
        CameraCtrl.I.ResetCamera();
    }

    void ReplayGame()
    {
        gameUI.UpdateTextLevel();
        CameraCtrl.I.ResetCamera();
        ChangeState(GameState.Play);
        levelCtrl.InitLevel();
    }

    public void WinGame()
    {
        SoundCtrl.I.PlaySound(TypeSound.WIN);
        ChangeState(GameState.None);
        ui.Show(UIType.Win);
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
}

public enum GameState
{
    None,
    Play
}
