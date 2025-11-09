using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    [SerializeField] private UIManager ui;
    public GameState CurState { get; private set; }
    private void Awake()
    {
        I = this;
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
    }

    private void OnDestroy()
    {
        HomeUI.ClickPlayAction -= PlayGame; 

        GameUI.ClickHomeAction -= HomeScene;
        GameUI.ClickReplayAction -= ReplayGame;
    }

    void HomeScene()
    {
        ui.Show(UIType.Home);
        ui.Hide(UIType.Game);
    }

    void PlayGame()
    {
        ui.Hide(UIType.Home);
        ui.Show(UIType.Game);
    }

    void ReplayGame()
    {

    }

    void WinGame()
    {
        ui.Show(UIType.Win);
    }

    void SwapBlock()
    {

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
