namespace RealWorld.Settings;

public class JwtSettingsOptions
{
    public string SecretKey { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int ExpiryMinutes { get; init; }
}