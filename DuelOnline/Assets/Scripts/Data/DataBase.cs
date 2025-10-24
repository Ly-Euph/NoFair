using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBase : MonoBehaviour
{
    // HP
    protected int plhp=4; // ©•ª
    protected int plmp=1; // ©•ª
    protected int emhp=4; // ‘Šè

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
            Debug.Log("MPƒ`ƒƒ[ƒW");
        }
    }
    virtual public int EmHP
    {
        get { return emhp; }
        set { emhp = value; }
    }
}
