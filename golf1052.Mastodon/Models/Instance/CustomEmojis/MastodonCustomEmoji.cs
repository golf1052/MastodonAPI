namespace golf1052.Mastodon.Models.Instance.CustomEmojis
{
    public class MastodonCustomEmoji
    {
        public string Shortcode { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string StaticUrl { get; set; } = string.Empty;
        public bool VisibleInPicker { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
