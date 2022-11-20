namespace Ngestor.Daemon;
public class Environment : IEnvironment
{
    public string MachineName => System.Environment.MachineName;
}
