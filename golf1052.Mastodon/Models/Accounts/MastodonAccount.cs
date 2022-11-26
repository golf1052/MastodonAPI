using System;
using System.Collections.Generic;
using golf1052.Mastodon.Models.Instance.CustomEmojis;

namespace golf1052.Mastodon.Models.Accounts
{
    public class MastodonAccount
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string AvatarStatic { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public string HeaderStatic { get; set; } = string.Empty;
        public bool Locked { get; set; }
        public List<MastodonAccountField> Fields { get; set; } = new List<MastodonAccountField>();
        public List<MastodonCustomEmoji> Emojis { get; set; } = new List<MastodonCustomEmoji>();
        public bool Bot { get; set; }
        public bool Group { get; set; }
        public bool? Discoverable { get; set; }
        public MastodonAccount? Moved { get; set; }
        public bool? Suspended { get; set; }
        public bool? Limited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastStatusAt { get; set; }
        public int StatusesCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public MastodonCredentialAccountSource? Source { get; set; }
        public MastodonCredentialAccountRole? Role { get; set; }
    }
}
