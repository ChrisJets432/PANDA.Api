using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using PANDA.Common;
using PANDA.Common.Configuration;

namespace PANDA.Infrastructure.Core;

public class DatabaseProvider
{
    private DatabaseSettings? _settings;
    private string? _connectionString = null;
    
    public IDbConnection Database;

    public DatabaseProvider()
    {
        DetermineDatabaseSettings();
        GetConnection();
        
        ArgumentNullException.ThrowIfNull(Database);
    }
    
    public string ConnectionString
    {
        get
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                return _connectionString;
            }
            
            ArgumentNullException.ThrowIfNull(_settings);
            return _connectionString =
                $"Server={_settings.Server};Database={_settings.Database};User={_settings.User};Password={_settings.Password};Connection Timeout={_settings.Timeout};";
        }
    }

    public void DetermineDatabaseSettings()
    {
        _settings = new DatabaseSettings {
            Server = ConfigurationManager.GetValue<string>("Database:Server") 
                     ?? throw new NullReferenceException("Database:Server"),
            Database = ConfigurationManager.GetValue<string>("Database:Database") 
                       ?? throw new NullReferenceException("Database:Database"),
            User = ConfigurationManager.GetValue<string>("Database:User") 
                   ?? throw new NullReferenceException("Database:User"),
            Password = ConfigurationManager.GetValue<string>("Database:Password") 
                       ?? throw new NullReferenceException("Database:Password")
        };

        var databaseType = ConfigurationManager.GetValue<string>("Database:Type") 
                           ?? throw new NullReferenceException("Database:Type");
        if (int.TryParse(databaseType, out int result))
        {
            _settings.Type = (DatabaseSettings.DatabaseType)result;
        }
        else
        {
            _settings.Type = databaseType.ToUpper() switch
            {
                "MST" or "MICROSOFT" => DatabaseSettings.DatabaseType.Mst,
                "POST" or "POSTGRES" => DatabaseSettings.DatabaseType.Postgres,
                _ => throw new NotSupportedException("Database:Type -> " + databaseType),
            };
        }

        if (int.TryParse(ConfigurationManager.GetValue<string>("Database:Timeout"), out var timeout))
        {
            _settings.Timeout = timeout;
        }

        if ((ConfigurationManager.GetValue<string>("Database:SearchPath") ?? string.Empty) is string searchPath &&
            !string.IsNullOrWhiteSpace(searchPath))
        {
            _settings.SearchPath = searchPath;
        }
    }

    public void GetConnection()
    {
        ArgumentNullException.ThrowIfNull(_settings);

        Database = _settings.Type switch
        {
            DatabaseSettings.DatabaseType.Mst => new SqlConnection(ConnectionString),
            DatabaseSettings.DatabaseType.Postgres => new NpgsqlConnection(ConnectionString),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}