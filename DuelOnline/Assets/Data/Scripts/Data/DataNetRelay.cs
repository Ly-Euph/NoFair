using Fusion;
using UnityEngine;

public class DataNetRelay : NetworkBehaviour
{
    public static DataNetRelay Instance { get; protected set; }

    [Networked] public int Player1HP { get; set; }
    [Networked] public int Player2HP { get; set; }

    public override void Spawned()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // HP‰Šú‰»”O‚Ì‚½‚ß
        Player1HP = 4;
        Player2HP = 4;
        Instance = this;
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
