using UnityEngine;

public class DataSingleton_Online : DataBase
{
    // HP‚Ì‚â‚èŽæ‚è
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
        set { plmp = value; }
    }
    override public int EmHP
    {
        get { return emhp; }
        set { emhp = value; }
    }
}
