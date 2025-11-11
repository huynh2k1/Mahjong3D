using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeCtrl : MonoBehaviour
{
    [SerializeField] Text _textTime;
    const float TimeCount = 301;
    Coroutine countDownCoroutine;

    public void StartCountDown()
    {
        StopCountDown();
        countDownCoroutine = StartCoroutine(CountDownCoroutine());
    }

    public void StopCountDown()
    {
        if(countDownCoroutine != null)
        {
            StopCoroutine(countDownCoroutine);
        }
    }


    IEnumerator CountDownCoroutine()
    {
        float time = TimeCount;
        UpdateTextTime(time);
        while (time > 0)
        {
            time -= Time.deltaTime;
            UpdateTextTime(time);
            yield return null;
        }
        time = 0;
        UpdateTextTime(time);
        GameManager.I.LoseGame();
    }

    void UpdateTextTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);
        _textTime.text = $"{minutes:00}:{seconds:00}";
    }
}
