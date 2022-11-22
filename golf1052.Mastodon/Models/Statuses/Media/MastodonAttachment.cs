namespace golf1052.Mastodon.Models.Statuses.Media
{
    public class MastodonAttachment
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string? Url { get; set; }
        public string PreviewUrl { get; set; }
        public string Description { get; set; }
        public string BlurHash { get; set; }
    }
}
