namespace PANDA.Common.Configuration;

public class DatabaseSettings
{
    public enum DatabaseType
    {
        Mst = 1,
        Postgres = 2,
    }
    
    public DatabaseType Type { get; set; }
    public string Server { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int Timeout { get; set; } = 30;
    public string? SearchPath { get; set; } = null;
}