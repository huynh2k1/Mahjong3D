using System.Collections.Generic;
using UnityEngine;

public class ReverseLevelGenerator : MonoBehaviour
{
    [Header("Kích thước level")]
    public int xSize = 3;
    public int ySize = 2;
    public int zSize = 3;

    [Header("Block prefab (12 mẫu khác nhau)")]
    public GameObject[] blockPrefabs; // ví dụ: 12 prefab khác nhau
    public Transform parent;

    private int[,,] grid;

    void Start()
    {
        GenerateSolvableLevel();
    }

    void GenerateSolvableLevel()
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
                GenerateSolvableLevel();
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
        // Xóa block cũ nếu có
        foreach (Transform c in parent)
            Destroy(c.gameObject);

        for (int y = 0; y < ySize; y++)
        {
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    int sym = grid[x, y, z];
                    if (sym == -1) continue;

                    // Tạo block tương ứng
                    Vector3 pos = new Vector3(x, y, z);
                    Instantiate(blockPrefabs[sym], pos, Quaternion.identity, parent);
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
}
