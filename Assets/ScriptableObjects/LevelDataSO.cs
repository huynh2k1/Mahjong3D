using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "SO/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public int xSize;
    public int ySize;
    public int zSize;
    public int[] gridData; // Flatten 3D → 1D

    public int Get(int x, int y, int z)
        => gridData[x + y * xSize + z * xSize * ySize];
}
