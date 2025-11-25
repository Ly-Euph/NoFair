using Fusion;
using UnityEngine;

public class DataNetRelay : NetworkBehaviour
{
    public static DataNetRelay Instance { get; protected set; }

    [Networked] public int Player1HP { get; set; }
    [Networked] public int Player2HP { get; set; }
    [Networked] public int Player1AnimNum { get; set; }
    [Networked] public int Player2AnimNum { get; set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetAnim(string plNum, int anim)
    {
        if (plNum!="Player1"&&plNum!="Player2") return;

        if (plNum=="Player1")
            Player1AnimNum = anim;
        else
            Player2AnimNum = anim;
    }

    // CharaCon->RPC_SetHP->UI”½‰f
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetHP(string plNum,int hp)
    {
        if (plNum != "Player1" && plNum != "Player2") return;
        if (plNum == "Player1")
        {
            Player1HP = hp;
        }
        else
        {
            Player2HP = hp;
        }
    }
}
