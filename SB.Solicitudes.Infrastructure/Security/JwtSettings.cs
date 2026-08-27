namespace SB.Solicitudes.Infrastructure.Security
{
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpirationMinutes { get; set; } = 60;
    }
}
