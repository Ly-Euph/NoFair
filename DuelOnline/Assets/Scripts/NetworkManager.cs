using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private SceneRef sceneRef; // ← Inspector でアサインする

    private NetworkRunner _runner;

    public async void OnClick()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "RandomRoom_" + Random.Range(0, 9999),
            Scene = sceneRef, // Inspector で指定した SceneRef を渡す
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await _runner.StartGame(startArgs);

        if (result.Ok)
            Debug.Log("✅ ランダムマッチ開始成功");
        else
            Debug.LogError($"❌ マッチ開始失敗: {result.ShutdownReason}");
    }
}
