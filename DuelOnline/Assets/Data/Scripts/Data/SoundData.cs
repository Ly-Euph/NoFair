using UnityEngine;

/// <summary>
/// ゲーム全体のサウンド情報をまとめるデータコンテナ
/// </summary>
[CreateAssetMenu(fileName = "SoundData", menuName = "GameData/SoundData", order = 0)]
public class SoundData : ScriptableObject
{
    [Header("🎵 BGMクリップ一覧")]
    public AudioClip[] bgmClips;

    [Header("🔊 SEクリップ一覧")]
    public AudioClip[] seClips;
}
