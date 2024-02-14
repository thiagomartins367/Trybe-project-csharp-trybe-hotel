using System.Reflection;

namespace TrybeHotel.Env;

public static class EnvironmentVariables
{
    public const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
    public const string PORT = "PORT";
    public const string DB_SERVER = "DB_SERVER";
    public const string DB_PORT = "DB_PORT";
    public const string DB_DIALECT = "DB_DIALECT";
    public const string DB_NAME = "DB_NAME";
    public const string DB_USER = "DB_USER";
    public const string DB_PASSWORD = "DB_PASSWORD";
    public const string AUTH_TOKEN_SECRET_KEY = "AUTH_TOKEN_SECRET_KEY";
    public const string AUTH_TOKEN_EXPIRE_DAYS = "AUTH_TOKEN_EXPIRE_DAYS";

    public static string[] GetVariableNames()
    {
        Type myType = typeof(EnvironmentVariables);
        FieldInfo[] fields = myType.GetFields();
        IEnumerable<string> variableNames = fields
            .Select(fieldInfo => fieldInfo.GetValue(myType)!.ToString()!);
        return variableNames.ToArray();
    }
}