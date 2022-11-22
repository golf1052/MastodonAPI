namespace golf1052.Mastodon.Models.Apps
{
    public class MastodonApplication
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? RedirectUri { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? AccessToken { get; set; }
        public string VapidKey { get; set; } = string.Empty;
    }
}
