using UnityEngine;

public abstract class UI : MonoBehaviour
{
    public abstract UIType Type { get; }
    public virtual void Show() => gameObject.SetActive(true);
    public virtual void Hide() => gameObject.SetActive(false);
}

public enum UIType
{
    Home,
    Game,
    Win,
    HowToPlay,
    Lose,
}
