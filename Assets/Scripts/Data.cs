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

    public static bool Sound
    {
        get => PlayerPrefs.GetInt("Sound", 0) == 0 ? true : false;
        set => PlayerPrefs.SetInt("Sound", value ? 0 : 1);
    }

    public static bool Music
    {
        get => PlayerPrefs.GetInt("Music", 0) == 0 ? true : false;
        set => PlayerPrefs.SetInt("Music", value ? 0 : 1);
    }
}
