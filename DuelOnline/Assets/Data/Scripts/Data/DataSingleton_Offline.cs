using UnityEngine;

public class DataSingleton_Offline : DataBase
{
    // HP‚Ì‚â‚èæ‚è
    protected override void Awake()
    {
        base.Awake();
    }

    override public int PlHP
    {
        get { return plhp; }
        set { plhp = value; }
    }
    override public int PlMP
    {
        get { return plmp; }
        set { 
            plmp = value;
            Debug.Log("MPƒ`ƒƒ[ƒW");
        }
    }
    override public int EmHP
    {
        get { return emhp; }
        set { emhp = value; }
    }
}
