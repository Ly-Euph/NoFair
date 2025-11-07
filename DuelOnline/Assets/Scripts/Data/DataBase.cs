using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBase : MonoBehaviour
{
    // HP
    protected int plhp=4; // 自分
    protected int plmp=1; // 自分
    protected int emhp=4; // 相手
    protected bool isReady = false; // 待機
    public static DataBase Instance { get; protected set; }


    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    virtual public int PlHP
    {
        get { return plhp; }
        set { plhp = value; }
    }
    virtual public int PlMP
    {
        get { return plmp; }
        set { 
            plmp = value;
            Debug.Log("MPチャージ");
        }
    }
    virtual public int EmHP
    {
        get { return emhp; }
        set { emhp = value; }
    }

    virtual public bool IsReady
    {
        get { return isReady; }
        set { isReady = value; }
    }
}
