using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Map/TileConfig")]
public class TileConfig : ScriptableObject
{
    public string tileName; // 表示用やデバッグ用
    public GameObject prefab;

    [Header("接続方向（上下左右）")]
    public bool connectNorth; // Z+
    public bool connectSouth; // Z-
    public bool connectEast;  // X+
    public bool connectWest;  // X-

    public bool ConnectsTo(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return connectNorth;
        if (dir == Vector2Int.down) return connectSouth;
        if (dir == Vector2Int.right) return connectEast;
        if (dir == Vector2Int.left) return connectWest;
        return false;
    }

    public List<Vector2Int> GetConnectedDirections()
    {
        List<Vector2Int> dirs = new();
        if (connectNorth) dirs.Add(Vector2Int.up);
        if (connectSouth) dirs.Add(Vector2Int.down);
        if (connectEast) dirs.Add(Vector2Int.right);
        if (connectWest) dirs.Add(Vector2Int.left);
        return dirs;
    }
}
