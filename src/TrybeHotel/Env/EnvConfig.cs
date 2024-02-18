namespace TrybeHotel.Env;

// Código desenvolvido baseado no artigo:
// https://medium.com/xp-inc/dica-r%C3%A1pida-net-env-76b0c66dac4
public static class EnvConfig
{
    public static void Load(string path)
    {
        try
        {
            VerifyEnvironmentVariables();
            return;
        }
        catch (Exception) { }

        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"The \"{path}\" environment variables file not found!"
            );

        foreach (var line in File.ReadAllLines(path))
        {
            var part = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (part.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(part[0], part[1]);
        }

        VerifyEnvironmentVariables();
    }

    private static void VerifyEnvironmentVariables()
    {
        string[] variableNames = EnvironmentVariables.GetVariableNames();
        foreach (string name in variableNames)
        {
            var variableValue = Environment.GetEnvironmentVariable(name);
            if (variableValue is null || variableValue.Length == 0)
                throw new Exception($"Environment variable \"{name}\" not found or is empty!");
        }
    }
}