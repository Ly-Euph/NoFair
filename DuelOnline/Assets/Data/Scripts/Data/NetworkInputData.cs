using Fusion;
/// <summary>
/// Fusion で送信する入力内容
/// ※ ジェスチャーで決まった技の種類だけ送れば OK
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public int ActionID;
}