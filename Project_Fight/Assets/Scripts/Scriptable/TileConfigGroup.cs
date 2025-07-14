using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// •¡”‚ÌTileConfigi•”‰®E’Ê˜Hj‚ğ‚Ü‚Æ‚ß‚éScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Map/TileConfigGroup")]
public class TileConfigGroup : ScriptableObject
{
    public List<TileConfig> roomConfigs;
    public List<TileConfig> pathConfigs;
}
