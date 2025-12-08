using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームマッチングの起動と管理を行うクラス
/// NetworkRunner を生成してマッチング処理、シーン遷移、セッション退出までを管理
/// </summary>
public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs")]
    [SerializeField] private NetworkRunner networkRunnerPrefab; // Runnerプレハブ
    [SerializeField] private DataNetRelay dataRelayPrefab;
    [SerializeField] private GameObject pl1Avatar;
    [SerializeField] private GameObject pl2Avatar;

    // 実際に生成された NetworkRunner のインスタンス
    private NetworkRunner activeRunner;

    // ホストか判定
    private bool isHost = false;
    // マッチング状態
    private bool isMatch = false;
    
    // 他スクリプト制御
    public bool GetMatchState => isMatch;
    public bool GetisHost => isHost;

    PlayerRef plRef;
    
    // 生成したプレイヤー保管
    NetworkObject pl1;
    NetworkObject pl2;

    private int INPUT = 0;

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

        // ProvideInput を使うならここで設定しておく
        activeRunner.ProvideInput = true;

        // コールバック対象に自身を登録
        activeRunner.AddCallbacks(this);

        // ※ SceneManager を StartGameArgs に渡すのが正しい方法
        //    既に同じ GameObject に追加されていたら重複追加しない
        NetworkSceneManagerDefault sceneMgr = activeRunner.gameObject.GetComponent<NetworkSceneManagerDefault>();
        if (sceneMgr == null)
        {
            sceneMgr = activeRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        // StartGame 呼び出し時に SceneManager を渡す
        var args = new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            PlayerCount = 2,
            // 必要なら最初のシーンインデックスや参照を指定
            // Scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex,
            SceneManager = sceneMgr
        };

        await activeRunner.StartGame(args);
    }
    public void ChangeSceneOnline(string sceneName)
    {
        // ★ Runnerがシーンを同期ロードする
        activeRunner.LoadScene(sceneName);
    }

    public void InsNetRelay()
    {
        if (activeRunner.IsServer)
        {
            activeRunner.Spawn(dataRelayPrefab);
        }
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

            plRef = player;

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
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
        string scene = SceneManager.GetActiveScene().name;

        // ゲームシーン以外ならスキップ
        if (scene != "GameOnline")
        return;

        if (isHost)
        {
            // プレイヤー位置取得
            Transform p1Pos = GameObject.Find("P1Pos").transform;
            Transform p2Pos = GameObject.Find("P2Pos").transform;

            // Host は PlayerRef を使って Spawn して紐付ける
            int index = 0;
            foreach (var player in runner.ActivePlayers)
            {
                Vector3 pos = (index == 0) ? p1Pos.position : p2Pos.position;
                Quaternion rot = (index == 0) ? p1Pos.rotation : p2Pos.rotation;

                var prefab = (index == 0) ? pl1Avatar : pl2Avatar;

                // Spawn と PlayerRef 紐付け
                NetworkObject character = runner.Spawn(prefab, pos, rot, player);
                runner.SetPlayerObject(player, character);
                index++;
            }
        }

        //if (runner.IsServer)
        //{
        //    PlayerPosSet(pl1, p1Pos);
        //    PlayerPosSet(pl2, p2Pos);
        //}
        Debug.Log("キャラクター Spawn & PlayerRef 紐付け 完了");
    }

    // 座標値セット
    public void PlayerPosSet(NetworkObject plPre, Transform plSpawnPos)
    {
        plPre.transform.position = plSpawnPos.transform.position;
        plPre.transform.rotation = plSpawnPos.transform.rotation;
    }


    // ------------------------------------------------------------
    // OnInput：Fusion が呼ぶ入力セット関数
    // ジェスチャー→SAttack が呼ばれた「結果」だけ反映すればOK
    // ------------------------------------------------------------
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("OnInput");
        var data = new NetworkInputData();

        // CharacterBase のジェスチャー解析によって animNum が更新される。
        // animNum によってどの技を出したか判定し、フラグを送る。
        data.ActionID = INPUT; // 数字そのまま送る
        input.Set(data);
    }

    public void SetInputNum(int num)
    {
        INPUT = num;
    }

    // -----------------------------------
    // INetworkRunnerCallbacks の未使用コールバック
    // 必須実装のため空メソッドを定義
    // -----------------------------------
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
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
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
}
