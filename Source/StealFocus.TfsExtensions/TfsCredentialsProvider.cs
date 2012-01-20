namespace StealFocus.TfsExtensions
{
    using System;
    using System.Net;

    using Microsoft.TeamFoundation.Client;

    public class TfsCredentialsProvider : ICredentialsProvider
    {
        private readonly NetworkCredential credentials;

        public TfsCredentialsProvider(string user, string domain, string password)
        {
            this.credentials = new NetworkCredential(user, password, domain);
        }

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials)
        {
            return this.credentials;
        }

        public void NotifyCredentialsAuthenticated(Uri uri)
        {
            // Do nothing.
        }
    }
}
