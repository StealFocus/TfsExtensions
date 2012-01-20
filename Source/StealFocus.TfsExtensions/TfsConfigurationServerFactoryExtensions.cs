namespace StealFocus.TfsExtensions
{
    using System;
    using Microsoft.TeamFoundation.Client;

    public static class TfsConfigurationServerFactoryExtensions
    {
        public static TfsConfigurationServer GetConfigurationServerAndAuthenticate(Uri tfsUrl, string domain, string userName, string password)
        {
            TfsCredentialsProvider tfsCredentialsProvider = new TfsCredentialsProvider(userName, domain, password);
            return GetConfigurationServerAndAuthenticate(tfsUrl, tfsCredentialsProvider);
        }

        public static TfsConfigurationServer GetConfigurationServerAndAuthenticate(Uri tfsUrl, ICredentialsProvider tfsCredentialsProvider)
        {
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUrl, tfsCredentialsProvider);
            tfsConfigurationServer.Authenticate();
            return tfsConfigurationServer;
        }

        public static TfsConfigurationServer GetConfigurationServerAndAuthenticate(Uri tfsUrl)
        {
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUrl);
            tfsConfigurationServer.Authenticate();
            return tfsConfigurationServer;
        }
    }
}
