using TrybeHotel.Env;

namespace TrybeHotel.Services
{
    public class TokenOptions
    {
        public const string Token = "Token";
        public string Secret { get; set; } = null!;
        public int ExpiresDay { get; set; } = 7;

        public TokenOptions()
        {
            var envSecretKey = Environment
                .GetEnvironmentVariable(EnvironmentVariables.AUTH_TOKEN_SECRET_KEY);
            var envExpiresDay = Environment
                .GetEnvironmentVariable(EnvironmentVariables.AUTH_TOKEN_EXPIRE_DAYS);
            var expiresDayIsInt = int.TryParse(envExpiresDay, out int expiresDay);
            Secret = envSecretKey is not null ? envSecretKey : string.Empty;
            if (!expiresDayIsInt) return;
            ExpiresDay = expiresDay;
        }
    }
}