using UnityEngine;

public static class Data 
{
    public static int CurLevel
    {
        get => PlayerPrefs.GetInt("CurLevel", 0);
        set => PlayerPrefs.SetInt("CurLevel", value);
    }

    public static int Score
    {
        get => PlayerPrefs.GetInt("Score", 0);
        set => PlayerPrefs.SetInt("Score", value);
    }
}
