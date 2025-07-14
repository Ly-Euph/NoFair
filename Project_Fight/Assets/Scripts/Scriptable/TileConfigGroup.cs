using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ������TileConfig�i�����E�ʘH�j���܂Ƃ߂�ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Map/TileConfigGroup")]
public class TileConfigGroup : ScriptableObject
{
    public List<TileConfig> roomConfigs;
    public List<TileConfig> pathConfigs;
}
