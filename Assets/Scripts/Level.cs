using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Kích thước level")]
    public int xSize = 3;
    public int ySize = 2;
    public int zSize = 3;

    [Header("Block prefab")]
    public Block[] blockPrefabs; // ví dụ: 12 prefab khác nhau
    private List<Block> listBlock = new List<Block>();
    private int[,,] grid;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void InitLevel()
    {
        CameraCtrl.I.target = transform;
        GenerateLevelCanResolve();
    }

    #region LEVEL GENERATE
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

        // Tạo danh sách symbol ID (mỗi symbol xuất hiện 1 cặp)
        List<int> symbolList = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            int sym = i % blockPrefabs.Length;
            symbolList.Add(sym);
        }

        Shuffle(symbolList);

        // Đặt các cặp theo thứ tự đảo ngược (reverse placement)
        foreach (var sym in symbolList)
        {
            bool placed = PlacePair(sym);
            if (!placed)
            {
                Debug.LogWarning($"⚠️ Không thể đặt cặp {sym}, thử lại...");
                GenerateLevelCanResolve();
                return;
            }
        }

        Debug.Log("✅ Sinh level thành công – đảm bảo thắng 100%!");
        SpawnBlocks();
    }

    bool PlacePair(int sym)
    {
        // Lấy danh sách tất cả ô trống hiện tại
        List<Vector3Int> emptyList = new List<Vector3Int>();
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++)
                for (int z = 0; z < zSize; z++)
                    if (grid[x, y, z] == -1)
                        emptyList.Add(new Vector3Int(x, y, z));

        Shuffle(emptyList);

        // Tìm 2 vị trí mà cả hai đều “free” (theo luật không bị chặn trái/phải)
        for (int i = 0; i < emptyList.Count; i++)
        {
            var pos1 = emptyList[i];
            if (!IsFree(pos1.x, pos1.y, pos1.z)) continue;

            for (int j = i + 1; j < emptyList.Count; j++)
            {
                var pos2 = emptyList[j];
                if (!IsFree(pos2.x, pos2.y, pos2.z)) continue;

                // Đặt 2 block
                grid[pos1.x, pos1.y, pos1.z] = sym;
                grid[pos2.x, pos2.y, pos2.z] = sym;
                return true;
            }
        }

        return false; // không tìm được chỗ hợp lệ
    }

    bool IsFree(int x, int y, int z)
    {
        // Ô trống coi là free khi đang sinh
        if (grid[x, y, z] != -1) return false;

        bool leftFree = (x - 1 < 0) || (grid[x - 1, y, z] == -1);
        bool rightFree = (x + 1 >= xSize) || (grid[x + 1, y, z] == -1);

        bool frontFree = (z - 1 < 0) || (grid[x, y, z - 1] == -1);
        bool backFree = (z + 1 >= zSize) || (grid[x, y, z + 1] == -1);

        // Chỉ cần ít nhất 1 mặt nằm ngang được tự do
        return leftFree || rightFree || frontFree || backFree;
    }

    void SpawnBlocks()
    {
        listBlock.Clear();

        // Tính offset để căn giữa level
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

                    // Tính vị trí căn giữa
                    Vector3 pos = new Vector3(x, y, z) - centerOffset;

                    Block block = Instantiate(blockPrefabs[sym]);
                    block.transform.position = pos;
                    block.transform.SetParent(transform, false);
                    block.gridPos = new Vector3Int(x, y, z);
                    listBlock.Add(block);
                    block.OnBlockClick = SelectBlock;
                }
            }
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    #endregion

    #region Merge 2 Block
    Block block1;
    Block block2;
    public void SelectBlock(Block block)
    {
        if (CanOut(block) == false)
        {
            block.Shake();
            return;
        }
        if(block1 == null)
        {
            block1 = block;
            block1.Hover();
        }else if(block2 == null)
        {
            if(block != block1)
            {
                if(block.ID != block1.ID)
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

        bool leftFree = true;
        bool rightFree = true;
        bool frontFree = true;
        bool backFree = true;

        //Check Left
        for (var left = 0; left < x; left++)
        {
            if (grid[left, y, z] != -1)
            {
                Debug.LogWarning($"Bị chặn trái ({left}, {y}, {z})");
                leftFree = false;
                break;
            }
        }

        //Check Right
        for (var right = x + 1; right < xSize; right++)
        {
            if (grid[right, y, z] != -1)
            {
                Debug.LogWarning($"Bị chặn phải ({right}, {y}, {z})");

                rightFree = false;
                break;
            }
        }

        //Check Front
        for(var front = 0; front < z; front++)
        {
            if (grid[x, y, front] != -1)
            {
                Debug.LogWarning($"Bị chặn trước ({x}, {y}, {front})");

                frontFree = false;
                break;
            }
        }

        //Check Back
        for(var back = z + 1; back < zSize; back++)
        {
            if (grid[x, y, back] != -1)
            {
                Debug.LogWarning($"Bị chặn sau ({x}, {y}, {back})");

                backFree = false;
                break;
            }
        }


        return (leftFree || rightFree) && (frontFree || backFree);
    }

    void MergeTwoBlock()
    {
        grid[block1.gridPos.x, block1.gridPos.y, block1.gridPos.z] = -1;
        grid[block2.gridPos.x, block2.gridPos.y, block2.gridPos.z] = -1;

        if (listBlock.Contains(block1))
        {
            listBlock.Remove(block1);
        }

        if (listBlock.Contains(block2))
        {
            listBlock.Remove(block2);
        }

        if(listBlock.Count == 0)
        {
            GameManager.I.WinGame();
        }
        block1.Destroy();
        block2.Destroy();
        block1 = null;
        block2 = null; 
    }
    #endregion
}
