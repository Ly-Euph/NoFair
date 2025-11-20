using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

/// <summary>
/// ゲームマッチングの起動と管理を行うクラス
/// NetworkRunner を生成してマッチング処理、シーン遷移、セッション退出までを管理
/// </summary>
public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs")]
    [SerializeField] private NetworkRunner networkRunnerPrefab; // Runnerプレハブ
    [SerializeField] private NetworkPrefabRef playerAvatarPrefab; // プレイヤーアバタープレハブ

    // 実際に生成された NetworkRunner のインスタンス
    private NetworkRunner activeRunner;

    // ホストか判定
    private bool isHost = false;
    // マッチング状態
    private bool isMatch = false;
    
    // 他スクリプト制御
    public bool GetMatchState => isMatch;
    public bool GetisHost => isHost;

    // シングルトン化
    public static GameLauncher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // シーン切り替えでも保持
    }

    /// <summary>
    /// マッチング開始
    /// </summary>
    public async void Match()
    {
        // RunnerをPrefabから生成
        activeRunner = Instantiate(networkRunnerPrefab);
        // コールバック対象に自身を登録
        activeRunner.AddCallbacks(this);

        // マッチング開始（Sharedモード）
        await activeRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            PlayerCount = 2
        }) ;
    }

    /// <summary>
    /// プレイヤーがセッションに参加した際に呼ばれる
    /// </summary>
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player.PlayerId}");

        // ローカルプレイヤー以外なら相手が入ってきた
        if (player != runner.LocalPlayer)
        {
            StartCoroutine(NowMatch());

            // True:Host
            // False:Client
            isHost = activeRunner.IsServer;
            isMatch = true;
        }
    }

    /// <summary>
    /// マッチング後に少し待ってゲームシーンへ遷移
    /// </summary>
    private IEnumerator NowMatch()
    {
        yield return new WaitForSeconds(0.8f);

        // フェードを使ってシーン遷移（FadePoolManager でプール管理）
        FadePoolManager.Instance.GetFade().LoadScene("GameOnline");

        yield break;
    }

    /// <summary>
    /// セッションを安全に退出
    /// </summary>
    public void LeaveSession()
    {
        if (activeRunner != null && activeRunner.IsRunning)
        {
            isMatch = false;
            activeRunner.Shutdown(); // ルームを離脱 + Runner停止
            activeRunner = null;
        }
    }

    // Clientが抜けた場合のHost
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        LeaveSession();
        // タイトルに戻す
        FadePoolManager.Instance.GetFade().LoadScene("Title");
    }
    // Hostが抜けた場合にClientはこれが呼び出される
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        // LeaveSession関数だと既にランナーが起動してない状態なので強制的に切る
        isMatch = false;
        activeRunner.Shutdown(); // ルームを離脱 + Runner停止
        activeRunner = null;
        // タイトルに戻す
        FadePoolManager.Instance.GetFade().LoadScene("Title");
    }

    // -----------------------------------
    // INetworkRunnerCallbacks の未使用コールバック
    // 必須実装のため空メソッドを定義
    // -----------------------------------
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
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
