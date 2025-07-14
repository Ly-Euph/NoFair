using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("マップ設定")]
    public int width = 5;
    public int height = 5;
    public float tileSize = 5f;

    [Header("タイル定義まとめ")]
    public TileConfigGroup configGroup;

    private TileConfig[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new TileConfig[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                List<TileConfig> candidates = GetValidTileConfigs(x, y);
                if (candidates.Count > 0)
                {
                    var selected = candidates[Random.Range(0, candidates.Count)];
                    map[x, y] = selected;

                    Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                    Instantiate(selected.prefab, pos, Quaternion.identity, this.transform);
                }
            }
        }
    }

    List<TileConfig> GetValidTileConfigs(int x, int y)
    {
        List<TileConfig> sourceList;

        // 30%で部屋を配置（例）
        bool isRoom = Random.value < 0.4f;
        sourceList = isRoom ? configGroup.roomConfigs : configGroup.pathConfigs;

        List<TileConfig> candidates = new();

        foreach (var config in sourceList)
        {
            bool valid = false;

            foreach (var dir in config.GetConnectedDirections())
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;

                var neighbor = map[nx, ny];
                if (neighbor != null)
                {
                    // 逆方向が接続可能なら成立（部屋→通路 or 通路→部屋）
                    if (neighbor.ConnectsTo(-dir) && config.ConnectsTo(dir))
                    {
                        valid = true;
                        break;
                    }
                }
            }

            // 通路の場合：孤立しててもOK（or制限つける）
            if (!valid && !isRoom)
            {
                valid = true;
            }

            if (valid)
                candidates.Add(config);
        }

        return candidates;
    }
}
