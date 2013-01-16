namespace StealFocus.TfsExtensions.Console
{
    using System;
    using System.Globalization;

    using StealFocus.TfsExtensions.Build.Client;

    /// <remarks>
    /// <![CDATA[StealFocus.TfsExtensions.Console.exe /tfsUrl:http://server /updateRetentionPolicies /teamProjectName:myTeamProject /numberOfStoppedBuildsToKeep:1 /numberOfFailedBuildsToKeep:10 /numberOfPartiallySucceededBuildsToKeep:10 /numberOfSucceededBuildsToKeep:15 /deleteOptions:All /force:true]]>
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
                    UpdateRetentionPolicies(tfsUrl, args);
                }
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
            System.Console.WriteLine(" /tfsUrl:http://tfsUrl");
            System.Console.WriteLine(" /updateRetentionPolicies");
        }

        private static void UpdateRetentionPolicies(Uri tfsUrl, string[] args)
        {
            string teamProjectName = string.Empty;
            int numberOfStoppedBuildsToKeep = 10;
            int numberOfFailedBuildsToKeep = 10;
            int numberOfPartiallySucceededBuildsToKeep = 10;
            int numberOfSucceededBuildsToKeep = 10;
            string deleteOptions = string.Empty;
            bool force = false;
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
                else if (arg.StartsWith("/force", StringComparison.OrdinalIgnoreCase))
                {
                    force = bool.Parse(arg.Replace("/force:", string.Empty));
                }
            }

            System.Console.WriteLine("Updating retention policies across all Build Definitions for Team Project '{0}'.", teamProjectName);
            System.Console.WriteLine("Number of stopped builds to keep is '{0}'.", numberOfStoppedBuildsToKeep);
            System.Console.WriteLine("Number of failed builds to keep is '{0}'.", numberOfFailedBuildsToKeep);
            System.Console.WriteLine("Number of partially succeeded builds to keep is '{0}'.", numberOfPartiallySucceededBuildsToKeep);
            System.Console.WriteLine("Number of succeeded builds to keep is '{0}'.", numberOfSucceededBuildsToKeep);
            System.Console.WriteLine("Delete options are '{0}'.", deleteOptions);
            System.Console.WriteLine();
            bool execute = true;
            if (!force)
            {
                System.Console.WriteLine("Are these options correct? (y/n)");
                ConsoleKeyInfo consoleKeyInfo = System.Console.ReadKey();
                if (consoleKeyInfo.KeyChar != 'y')
                {
                    execute = false;
                }

                System.Console.WriteLine();
                System.Console.WriteLine();
            }

            if (execute)
            {
                BuildServerFacade.UpdateRetentionPolicies(tfsUrl, teamProjectName, numberOfStoppedBuildsToKeep, numberOfFailedBuildsToKeep, numberOfPartiallySucceededBuildsToKeep, numberOfSucceededBuildsToKeep, deleteOptions);
                System.Console.WriteLine("Done.");
            }
        }
    }
}
