using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 5x5単位の部屋(Room_)や通路(Path_)プレファブを、隣接ルールに従って自動配置するスクリプト
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("マップ設定")]
    public int width = 5;         // マップ横セル数
    public int height = 5;        // マップ縦セル数
    public float tileSize = 5f;   // 1マスのサイズ（単位）

    private enum TileType { None, Room, Path }
    private TileType[,] map; // タイプを格納する2次元配列

    private List<GameObject> roomPrefabs = new();
    private List<GameObject> pathPrefabs = new();

    void Start()
    {
        LoadPrefabs();
        GenerateTileMap();
        InstantiateTiles();
    }

    /// <summary>
    /// ResourcesからRoom_とPath_で分類して読み込む
    /// </summary>
    void LoadPrefabs()
    {
        var allPrefabs = Resources.LoadAll<GameObject>("");

        foreach (var prefab in allPrefabs)
        {
            if (prefab.name.StartsWith("Room_"))
                roomPrefabs.Add(prefab);
            else if (prefab.name.StartsWith("Path_"))
                pathPrefabs.Add(prefab);
        }

        if (roomPrefabs.Count == 0 || pathPrefabs.Count == 0)
            Debug.LogWarning("Room_またはPath_のプレファブがResources内に存在しません");
    }

    /// <summary>
    /// 隣接ルールに基づいたマップの論理構築（TileType配列）
    /// </summary>
    void GenerateTileMap()
    {
        map = new TileType[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int roomCount = CountAdjacent(x, y, TileType.Room);
                int pathCount = CountAdjacent(x, y, TileType.Path);

                if (roomCount >= 1 && pathCount == 0)
                {
                    map[x, y] = TileType.Path; // 部屋の隣は通路に
                }
                else if (pathCount >= 1 && Random.value < 0.5f)
                {
                    map[x, y] = TileType.Room; // 通路の隣に部屋が来やすく
                }
                else
                {
                    map[x, y] = Random.value < 0.4f ? TileType.Room : TileType.Path;
                }
            }
        }

        // 中心に最低1つの部屋を強制設置
        map[width / 2, height / 2] = TileType.Room;
    }

    /// <summary>
    /// TileTypeに応じたプレファブを生成する
    /// </summary>
    void InstantiateTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject prefab = null;

                switch (map[x, y])
                {
                    case TileType.Room:
                        prefab = GetRandomPrefab(roomPrefabs);
                        break;
                    case TileType.Path:
                        prefab = GetRandomPrefab(pathPrefabs);
                        break;
                }

                if (prefab != null)
                {
                    Quaternion rot = GetRotationFromName(prefab.name); // 任意で方向対応
                    Instantiate(prefab, pos, rot, this.transform);
                }
            }
        }
    }

    /// <summary>
    /// 指定タイプの周囲タイル数をカウント
    /// </summary>
    int CountAdjacent(int x, int y, TileType type)
    {
        int count = 0;
        Vector2Int[] dirs = {
            new Vector2Int(0, 1),   // 上
            new Vector2Int(0, -1),  // 下
            new Vector2Int(1, 0),   // 右
            new Vector2Int(-1, 0),  // 左
        };

        foreach (var dir in dirs)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                if (map[nx, ny] == type)
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// プレファブリストからランダムに1つ取得
    /// </summary>
    GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// プレファブ名に含まれる方向文字列で回転を自動設定（オプション）
    /// </summary>
    Quaternion GetRotationFromName(string name)
    {
        if (name.Contains("_NS")) return Quaternion.identity;
        if (name.Contains("_EW")) return Quaternion.Euler(0, 90, 0);
        if (name.Contains("_NE")) return Quaternion.Euler(0, 0, 0);
        if (name.Contains("_SE")) return Quaternion.Euler(0, 90, 0);
        if (name.Contains("_SW")) return Quaternion.Euler(0, 180, 0);
        if (name.Contains("_NW")) return Quaternion.Euler(0, 270, 0);
        return Quaternion.identity;
    }
}
