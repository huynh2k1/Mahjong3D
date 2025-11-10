using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public static LevelControl I;
    [SerializeField] Level[] listLevel;
    Level _curLevel;

    private void Awake()
    {
        I = this;
    }

    public void InitLevel()
    {
        DestroyCurLevel();
        _curLevel = Instantiate(listLevel[Data.CurLevel], transform);
        _curLevel.InitLevel();
    }

    public void CheckIncreaseLevel()
    {
        if (Data.CurLevel < listLevel.Length - 1)
        {
            Data.CurLevel++;
        }
        else
        {
            Data.CurLevel = 0;
        }
    }

    public void DestroyCurLevel()
    {
        if(_curLevel != null)
        {
            _curLevel.Destroy();
        }
    }

    public void SelectBlock(Block block)
    {
        _curLevel.SelectBlock(block);
    }
}
