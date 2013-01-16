namespace StealFocus.TfsExtensions.Console
{
    using System;
    using System.Globalization;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;

    using StealFocus.TfsExtensions.Build.Client;
    using StealFocus.TfsExtensions.Dto;

    /// <remarks>
    /// <![CDATA[StealFocus.TfsExtensions.Console.exe /tfsUrl:http://server /updateRetentionPolicies /teamProjectName:myTeamProject /numberOfStoppedBuildsToKeep:1 /numberOfFailedBuildsToKeep:10 /numberOfPartiallySucceededBuildsToKeep:10 /numberOfSucceededBuildsToKeep:15 /deleteOptions:All]]>
    /// </remarks>
    internal class Program
    {
        internal static void Main(string[] args)
        {
            string line1 = string.Format(CultureInfo.CurrentCulture, "StealFocus TFS Extensions Console. Version {0}", typeof(Program).Assembly.GetName().Version);
            const string Line2 = "Copyright (c) StealFocus. All rights reserved.";
            System.Console.WriteLine();
            System.Console.WriteLine(line1);
            System.Console.WriteLine(Line2);
            System.Console.WriteLine();
            if (args != null && args.Length >= 2)
            {
                string tfsUrlValue = args[0].Replace("/tfsUrl:", string.Empty);
                Uri tfsUrl = new Uri(tfsUrlValue);
                if (args[1] == "/updateRetentionPolicies")
                {
                    string teamProjectName = string.Empty;
                    int numberOfStoppedBuildsToKeep = 10;
                    int numberOfFailedBuildsToKeep = 10;
                    int numberOfPartiallySucceededBuildsToKeep = 10;
                    int numberOfSucceededBuildsToKeep = 10;
                    string deleteOptions = string.Empty;
                    foreach (string arg in args)
                    {
                        if (arg.StartsWith("/teamProjectName", StringComparison.OrdinalIgnoreCase))
                        {
                            teamProjectName = arg.Replace("/teamProjectName:", string.Empty);
                        }
                        else if (arg.StartsWith("/numberOfStoppedBuildsToKeep", StringComparison.OrdinalIgnoreCase))
                        {
                            numberOfStoppedBuildsToKeep = int.Parse(arg.Replace("/numberOfStoppedBuildsToKeep:", string.Empty), CultureInfo.CurrentCulture);
                        }
                        else if (arg.StartsWith("/numberOfFailedBuildsToKeep", StringComparison.OrdinalIgnoreCase))
                        {
                            numberOfFailedBuildsToKeep = int.Parse(arg.Replace("/numberOfFailedBuildsToKeep:", string.Empty), CultureInfo.CurrentCulture);
                        }
                        else if (arg.StartsWith("/numberOfPartiallySucceededBuildsToKeep", StringComparison.OrdinalIgnoreCase))
                        {
                            numberOfPartiallySucceededBuildsToKeep = int.Parse(arg.Replace("/numberOfPartiallySucceededBuildsToKeep:", string.Empty), CultureInfo.CurrentCulture);
                        }
                        else if (arg.StartsWith("/numberOfSucceededBuildsToKeep", StringComparison.OrdinalIgnoreCase))
                        {
                            numberOfSucceededBuildsToKeep = int.Parse(arg.Replace("/numberOfSucceededBuildsToKeep:", string.Empty), CultureInfo.CurrentCulture);
                        }
                        else if (arg.StartsWith("/deleteOptions", StringComparison.OrdinalIgnoreCase))
                        {
                            deleteOptions = arg.Replace("/deleteOptions:", string.Empty);
                        }
                    }

                    System.Console.WriteLine("Updating retention policies across all Build Definitions for Team Project '{0}'.", teamProjectName);
                    System.Console.WriteLine("Number of stopped builds to keep is '{0}'.", numberOfStoppedBuildsToKeep);
                    System.Console.WriteLine("Number of failed builds to keep is '{0}'.", numberOfFailedBuildsToKeep);
                    System.Console.WriteLine("Number of partially succeeded builds to keep is '{0}'.", numberOfPartiallySucceededBuildsToKeep);
                    System.Console.WriteLine("Number of succeeded builds to keep is '{0}'.", numberOfSucceededBuildsToKeep);
                    System.Console.WriteLine("Delete options are '{0}'.", deleteOptions);
                    UpdateRetentionPolicies(tfsUrl, teamProjectName, numberOfStoppedBuildsToKeep, numberOfFailedBuildsToKeep, numberOfPartiallySucceededBuildsToKeep, numberOfSucceededBuildsToKeep, deleteOptions);
                }

                System.Console.WriteLine();
                System.Console.WriteLine("Done.");
            }
            else
            {
                ShowOptions();
            }
        }

        private static void ShowOptions()
        {
            System.Console.WriteLine("Supported arguments:");
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine(" /tfsUrl:http://tfs");
            System.Console.WriteLine(" /updateRetentionPolicies");
        }

        private static void UpdateRetentionPolicies(Uri tfsUrl, string teamProjectName, int numberOfStoppedBuildsToKeep, int numberOfFailedBuildsToKeep, int numberOfPartiallySucceededBuildsToKeep, int numberOfSucceededBuildsToKeep, string deleteOptions)
        {
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUrl);
            tfsConfigurationServer.Authenticate();
            TeamProjectDtoCollection allTeamProjectCollections = tfsConfigurationServer.GetAllTeamProjectsInAllTeamProjectCollections();
            foreach (TeamProjectDto teamProjectDto in allTeamProjectCollections)
            {
                if (teamProjectDto.DisplayName == teamProjectName)
                {
                    TfsTeamProjectCollection teamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectDto.CollectionId);
                    IBuildServer buildServer = teamProjectCollection.GetService<IBuildServer>();
                    BuildServerFacade buildServerFacade = new BuildServerFacade(buildServer);
                    buildServerFacade.UpdateRetentionPolicies(teamProjectDto.DisplayName, numberOfStoppedBuildsToKeep, numberOfFailedBuildsToKeep, numberOfPartiallySucceededBuildsToKeep, numberOfSucceededBuildsToKeep, deleteOptions);
                }
            }
        }
    }
}
