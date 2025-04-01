namespace ScriptBee.Analysis.Instance.Docker;

public interface IFreePortProvider
{
    int GetFreeTcpPort();
}
