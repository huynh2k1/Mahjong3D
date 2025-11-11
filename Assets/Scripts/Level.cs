using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour
{
    [Header("Kích thước level")]
    public int xSize = 3;
    public int ySize = 2;
    public int zSize = 3;

    public LevelDataSO loadedData;

    [Header("Block prefab")]
    public Block[] blockPrefabs;
    private List<Block> listBlock = new List<Block>();
    private int[,,] grid;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void InitLevel()
    {
        CameraCtrl.I.target = transform;
        LoadLevel();
    }
    public void LoadLevel()
    {
        if (loadedData == null)
        {
            Debug.LogError("❌ No LevelData assigned!");
            return;
        }

        xSize = loadedData.xSize;
        ySize = loadedData.ySize;
        zSize = loadedData.zSize;

        grid = new int[xSize, ySize, zSize];

        int index = 0;
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    grid[x, y, z] = loadedData.gridData[index++];

        SpawnBlocks();
    }

    #region LEVEL GENERATE


    [Button("Generate Level")]
    public void GenerateAndSaveLevel()
    {
#if UNITY_EDITOR
        GenerateLevelCanResolve(); // đã có grid

        LevelDataSO data = ScriptableObject.CreateInstance<LevelDataSO>();
        data.xSize = xSize;
        data.ySize = ySize;
        data.zSize = zSize;
        data.gridData = new int[xSize * ySize * zSize];

        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    data.gridData[x + y * xSize + z * xSize * ySize] = grid[x, y, z];

        AssetDatabase.CreateAsset(data, $"Assets/LevelData_{xSize}_{ySize}_{zSize}.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Level saved as ScriptableObject!");
#endif
    }

    void GenerateLevelCanResolve()
    {
        int totalTiles = xSize * ySize * zSize;
        if (totalTiles % 2 != 0)
        {
            Debug.LogError("⚠️ Tổng số ô phải là số chẵn!");
            return;
        }

        grid = new int[xSize, ySize, zSize];
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    grid[x, y, z] = -1;

        int pairCount = totalTiles / 2;

        List<int> symbolPool = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            int s = i % blockPrefabs.Length;
            symbolPool.Add(s);
        }

        Shuffle(symbolPool); // random ID list

        List<Vector3Int> freeCells = new List<Vector3Int>();
        for (int y = 0; y < ySize; y++)
            for (int z = 0; z < zSize; z++)
                for (int x = 0; x < xSize; x++)
                    freeCells.Add(new Vector3Int(x, y, z));

        Shuffle(freeCells); // random spawn order

        int index = 0;

        foreach (int sym in symbolPool)
        {
            // Lấy 2 vị trí gần nhau nhất trong danh sách còn trống
            Vector3Int p1 = freeCells[index++];
            Vector3Int p2 = freeCells[index++];

            grid[p1.x, p1.y, p1.z] = sym;
            grid[p2.x, p2.y, p2.z] = sym;
        }

        //SpawnBlocks();
        Debug.Log("🚀 Level generated fast!");
    }

    bool TryGenerateLevelOptimized()
    {
        grid = new int[xSize, ySize, zSize];
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    grid[x, y, z] = -1;

        int totalTiles = xSize * ySize * zSize;
        int pairCount = totalTiles / 2;

        List<int> symbols = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            int s = i % blockPrefabs.Length;
            symbols.Add(s);
            symbols.Add(s);
        }

        Shuffle(symbols);
        return PlaceBlocksBacktrack(symbols, 0);
    }

    bool PlaceBlocksBacktrack(List<int> symbols, int index)
    {
        if (index >= symbols.Count)
            return true;

        int currentSymbol = symbols[index];
        List<Vector3Int> freeCells = GetOrderedFreePositions();

        foreach (var pos in freeCells)
        {
            if (!CanPlaceHere(pos.x, pos.y, pos.z)) continue;

            grid[pos.x, pos.y, pos.z] = currentSymbol;

            if (StillSolvable() && PlaceBlocksBacktrack(symbols, index + 1))
                return true;

            grid[pos.x, pos.y, pos.z] = -1;
        }

        return false;
    }

    bool CanPlaceHere(int x, int y, int z)
    {
        if (grid[x, y, z] != -1) return false;
        return CountFreeSides(x, y, z) >= 1;
    }

    int CountFreeSides(int x, int y, int z)
    {
        int free = 0;
        if (x - 1 < 0 || grid[x - 1, y, z] == -1) free++;
        if (x + 1 >= xSize || grid[x + 1, y, z] == -1) free++;
        if (z - 1 < 0 || grid[x, y, z - 1] == -1) free++;
        if (z + 1 >= zSize || grid[x, y, z + 1] == -1) free++;
        return free;
    }

    List<Vector3Int> GetOrderedFreePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    if (grid[x, y, z] == -1)
                        list.Add(new Vector3Int(x, y, z));

        // Ưu tiên block ở biên, càng thoáng càng tốt (ít gây dead-end)
        list.Sort((a, b) => CountFreeSides(b.x, b.y, b.z).CompareTo(CountFreeSides(a.x, a.y, a.z)));
        return list;
    }

    bool StillSolvable()
    {
        Dictionary<int, int> count = new Dictionary<int, int>();

        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                {
                    int id = grid[x, y, z];
                    if (id == -1) continue;
                    if (!count.ContainsKey(id))
                        count[id] = 0;
                    count[id]++;
                }

        foreach (var kvp in count)
        {
            if (kvp.Value == 1) // còn 1 block lẻ → phải đảm bảo có thể lấy được
            {
                for (int x = 0; x < xSize; x++)
                    for (int y = 0; y < ySize; y++)
                        for (int z = 0; z < zSize; z++)
                            if (grid[x, y, z] == kvp.Key && CountFreeSides(x, y, z) == 0)
                                return false; // DEAD → backtrack ngay
            }
        }

        return true;
    }

    void SpawnBlocks()
    {
        listBlock.Clear();

        Vector3 centerOffset = new Vector3(
            (xSize - 1) / 2f,
            (ySize - 1) / 2f,
            (zSize - 1) / 2f
        );

        for (int y = 0; y < ySize; y++)
        {
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    int sym = grid[x, y, z];
                    if (sym == -1) continue;

                    Vector3 pos = new Vector3(x, y, z) - centerOffset;

                    Block block = Instantiate(blockPrefabs[sym]);
                    block.transform.position = pos;
                    block.transform.SetParent(transform, false);
                    block.gridPos = new Vector3Int(x, y, z);
                    listBlock.Add(block);
                    //block.OnBlockClick = SelectBlock;
                }
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    #endregion

    #region Merge 2 Block - OPTIMIZED
    Block block1;
    Block block2;

    public void SelectBlock(Block block)
    {
        if (!CanOut(block))
        {
            block.Shake();
            return;
        }

        if (block1 == null)
        {
            block1 = block;
            block1.Hover();
        }
        else if (block2 == null)
        {
            if (block != block1)
            {
                if (block.ID != block1.ID)
                {
                    block1.Normal();
                    block1 = block;
                    block1.Hover();
                    return;
                }
                block2 = block;
                block2.Hover();
                MergeTwoBlock();
            }
        }
    }

    bool CanOut(Block block)
    {
        int x = block.gridPos.x;
        int y = block.gridPos.y;
        int z = block.gridPos.z;

        // Tối ưu: chỉ kiểm tra khi cần thiết
        bool hasLeftBlock = false;
        bool hasRightBlock = false;
        bool hasFrontBlock = false;
        bool hasBackBlock = false;

        // Kiểm tra trái
        for (int left = x - 1; left >= 0; left--)
        {
            if (grid[left, y, z] != -1)
            {
                hasLeftBlock = true;
                break;
            }
        }

        // Kiểm tra phải
        for (int right = x + 1; right < xSize; right++)
        {
            if (grid[right, y, z] != -1)
            {
                hasRightBlock = true;
                break;
            }
        }

        // Kiểm tra trước
        for (int front = z - 1; front >= 0; front--)
        {
            if (grid[x, y, front] != -1)
            {
                hasFrontBlock = true;
                break;
            }
        }

        // Kiểm tra sau
        for (int back = z + 1; back < zSize; back++)
        {
            if (grid[x, y, back] != -1)
            {
                hasBackBlock = true;
                break;
            }
        }

        // Có thể ra nếu: (không bị chặn trái HOẶC không bị chặn phải) VÀ (không bị chặn trước HOẶC không bị chặn sau)
        return (!hasLeftBlock || !hasRightBlock) && (!hasFrontBlock || !hasBackBlock);
    }

    void MergeTwoBlock()
    {
        // Cập nhật grid
        grid[block1.gridPos.x, block1.gridPos.y, block1.gridPos.z] = -1;
        grid[block2.gridPos.x, block2.gridPos.y, block2.gridPos.z] = -1;

        // Xóa khỏi list
        listBlock.Remove(block1);
        listBlock.Remove(block2);

        // Kiểm tra win
        if (listBlock.Count == 0)
        {
            GameManager.I.WinGame();
        }

        // Hủy block
        block1.Destroy();
        block2.Destroy();
        block1 = null;
        block2 = null;
    }
    #endregion
}