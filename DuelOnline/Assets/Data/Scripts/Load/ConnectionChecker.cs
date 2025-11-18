using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Mygame.Checker
{
    public class ConnectionChecker
    {
        // ネットワークの接続状況を確認する
        public IEnumerator ICheck(System.Action<bool> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");
            request.timeout = 3;
            yield return request.SendWebRequest();

            bool isOnline = request.result == UnityWebRequest.Result.Success;
            Debug.Log(isOnline ? "インターネット接続あり" : "インターネット接続不可");

            callback?.Invoke(isOnline);
            yield break;
        }
    }
}
