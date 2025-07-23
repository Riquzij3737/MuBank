using System.Text.Json;

namespace Mubank.Models.DTOS
{
    public class ConfigsValues
    {
        public static LoggingConfig GetConfigValues()
        {
            var logging = JsonSerializer.Deserialize<LoggingConfig>(
                File.ReadAllText("C:\\Visual Studio Projects\\_NetProjects\\C#\\Mubank\\appsettings.json"));


            return logging;

        }
    }

    public class LoggingConfig
    {
        public LogLevelConfig LogLevel { get; set; }
        public ConnectionStringConfig ConnectionString { get; set; }
        public JwtSecretsConfig JwtSecrets { get; set; }
        public ApiKeysConfig ApiKeys { get; set; }
    }

    public class LogLevelConfig
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    public class ConnectionStringConfig
    {
        public string DefaultConnection { get; set; }
    }

    public class JwtSecretsConfig
    {
        public string SecretKey { get; set; }
        public string Audience { get; set; }
    }

    public class ApiKeysConfig
    {
        public string ApiGeminiKey { get; set; }
        public string ApiEmailSmtp { get; set; }
    }

}
