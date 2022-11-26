using System;

namespace golf1052.Mastodon.Models.Accounts
{
    public class MastodonAccountField
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime? VerifiedAt { get; set; }
    }
}
