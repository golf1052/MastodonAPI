namespace golf1052.Mastodon.Models.Apps.OAuth
{
    public class MastodonToken
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
        public long? CreatedAt { get; set; }
    }
}
