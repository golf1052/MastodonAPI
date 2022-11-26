using System;

namespace golf1052.Mastodon.Models.Accounts
{
    public class MastodonCredentialAccountRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Position { get; set; }
        public int Permissions { get; set; }
        public bool Highlighted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
