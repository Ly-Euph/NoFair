using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 複数のTileConfig（部屋・通路）をまとめるScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Map/TileConfigGroup")]
public class TileConfigGroup : ScriptableObject
{
    public List<TileConfig> roomConfigs;
    public List<TileConfig> pathConfigs;
}
