using System;
using System.Collections.Generic;
using System.Collections;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

 public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner networkRunnerPrefab;
    [SerializeField]
    private NetworkPrefabRef playerAvatarPrefab;

    // 接続状況
    private bool isMatch = false;

    public static GameLauncher Instance { get; private set; }

    public bool GetMatchState { get { return isMatch; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public async void Match()
    {
        var networkRunner = Instantiate(networkRunnerPrefab);
        // GameLauncherを、NetworkRunnerのコールバック対象に追加する
        networkRunner.AddCallbacks(this);

        await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared
        });
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    // プレイヤーがセッションへ参加した時に呼ばれるコールバック
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        Debug.Log("接続");

        FadeInOut fadeScr; // フェード機能
        fadeScr = FadeInOut.CreateInstance();
        // ローカルプレイヤー以外なら「相手が入ってきた」
        if (player != runner.LocalPlayer)
        {
            // 表示用コルーチン
            StartCoroutine(NowMatch(fadeScr));
            // 自分自身がマスタークライアントかどうかを判定する
            if (networkRunnerPrefab.IsSharedModeMasterClient)
            {
                Debug.Log("自分自身がマスタークライアントです");
            }
            else
            {
                Debug.Log("クライアントです");
            }

            isMatch = true;
        }  
    }
    private IEnumerator NowMatch(FadeInOut fadeInOut)
    {
        yield return new WaitForSeconds(0.8f);
        // シーン遷移
        fadeInOut.LoadScene("GameOnline");
        yield break; // ← この行でコルーチン終了
    }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
}