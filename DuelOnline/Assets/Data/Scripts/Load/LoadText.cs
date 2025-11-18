using System.Collections;
using UnityEngine;
using TMPro;

namespace Mygame.TextLoad
{
    public class LoadText
    {
        // ネットワークの接続状況を確認する
        public IEnumerator ILoadNow(TextMeshProUGUI tmp)
        {
            string text = tmp.text;
            char dot='.';
            while (true)
            {
                // まずクリア
                tmp.text = text;

                for (int i = 0; i < 3; i++)
                {
                    tmp.text += dot;
                    yield return new WaitForSeconds(0.8f); // 表示速度
                    yield return tmp;
                }
            }
        }
    }
}
