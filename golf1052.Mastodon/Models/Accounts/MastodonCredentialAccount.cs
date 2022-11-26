using System.Collections.Generic;

namespace golf1052.Mastodon.Models.Accounts
{
    public class MastodonCredentialAccountSource
    {
        public string Note { get; set; } = string.Empty;
        public List<MastodonAccountField> Fields { get; set; } = new List<MastodonAccountField>();
        public string Privacy { get; set; } = string.Empty;
        public bool Sensitive { get; set; }
        public string Language { get; set; } = string.Empty;
        public int FollowRequestsCount { get; set; }
    }
}
