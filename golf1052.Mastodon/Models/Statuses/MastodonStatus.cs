using System;
using System.Collections.Generic;
using golf1052.Mastodon.Models.Statuses.Media;

namespace golf1052.Mastodon.Models.Statuses
{
    public class MastodonStatus
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public List<MastodonAttachment> MediaAttachments { get; set; }
    }
}
