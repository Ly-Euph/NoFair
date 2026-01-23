using System;

/// <summary>
/// Botの学習結果を保存するためのデータクラス
/// ※ ロジックは一切書かない
/// </summary>
[Serializable]
public class BotAISaveData
{
    // データ構造変更用
    public int version = 1;

    // 行動ごとの学習倍率
    // [0] 弱魔法
    // [1] 強魔法
    // [2] チャージ
    // [3] ブロック
    public float[] learnedBias = new float[4];

    // 学習回数（確認用）
    public int battleCount;
}