using System.IO;
using UnityEngine;
using System.Diagnostics;

/// <summary>
/// BotAIのセーブ／ロード専用クラス
/// </summary>
public static class BotAIStorage
{
    /// <summary>
    /// 実行ファイル(.exe)が置かれているフォルダを取得
    /// </summary>
    public static string GetExeDirectory()
    {
        return Path.GetDirectoryName(
            Process.GetCurrentProcess().MainModule.FileName
        );
    }

    /// <summary>
    /// AIデータをロードする
    /// </summary>
    public static BotAISaveData Load(string fileName)
    {
        string path = Path.Combine(GetExeDirectory(), fileName);

        // 初回起動時
        if (!File.Exists(path))
        {
            BotAISaveData data = new BotAISaveData();
            for (int i = 0; i < 4; i++)
                data.learnedBias[i] = 1.0f;

            return data;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<BotAISaveData>(json);
    }

    /// <summary>
    /// AIデータをJSONとして保存
    /// </summary>
    public static void Save(string fileName, BotAISaveData data)
    {
        string path = Path.Combine(GetExeDirectory(), fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        UnityEngine.Debug.Log("Saving AI to: " + path);
    }
}
