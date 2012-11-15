namespace StealFocus.TfsExtensions
{
    using System;
    using System.Net;

    using Microsoft.TeamFoundation.Client;

    public static class TfsConfigurationServerFactoryExtensions
    {
        public static TfsConfigurationServer GetConfigurationServerAndAuthenticate(Uri tfsUrl, string domain, string userName, string password)
        {
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUrl);
            tfsConfigurationServer.Credentials = new NetworkCredential(userName, password, domain);
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
