using System;

namespace golf1052.Mastodon
{
    public class MastodonException : Exception
    {
        public MastodonException(string message) : base(message)
        {
        }
    }
}
