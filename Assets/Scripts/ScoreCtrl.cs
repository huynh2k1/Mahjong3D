using UnityEngine;
using UnityEngine.UI;

public class ScoreCtrl : MonoBehaviour
{
    [SerializeField] Text textScore;

    public void InitScore()
    {
        Data.Score = 0;
        UpdateTextScore(Data.Score);
    }

    public void AddScore(int score)
    {
        Data.Score += score;
        UpdateTextScore(Data.Score);

        if(Data.Score > Data.GetHighScoreAtLevel(Data.CurLevel))
        {
            Data.SetHighScoreAtLevel(Data.CurLevel, Data.Score);
        }
    }

    public void UpdateTextScore(int score)
    {
        textScore.text = $"Score: {score}";
    }
}
