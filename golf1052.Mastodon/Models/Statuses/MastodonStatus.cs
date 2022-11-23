using System;
using System.Collections.Generic;
using golf1052.Mastodon.Models.Statuses.Media;

namespace golf1052.Mastodon.Models.Statuses
{
    public class MastodonStatus
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Text { get; set; }
        public List<MastodonAttachment> MediaAttachments { get; set; } = new List<MastodonAttachment>();
        public string Uri { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
