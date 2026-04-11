namespace api.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public required string Audience { get; init; }
        public required string Issuer { get; init; }
        public required string Key { get; init; }
        public int ExpiresMinutes { get; init; } = 60;
    }
}
