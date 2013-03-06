StealFocus TFS Extensions
=========================
Extensions for Microsoft Team Foundation Server. These include:
- A class library containing helper type functions (available via NuGet).
- A console application providing admin type functionality (available for direct download).

Downloading
-----------
You can download the library from NuGet: [http://nuget.org/packages/StealFocus.TfsExtensions](http://nuget.org/packages/StealFocus.TfsExtensions)

You can download the console application from the build output location: [http://build01.stealfocus.co.uk/Output/StealFocus.TfsExtensions/](http://build01.stealfocus.co.uk/Output/StealFocus.TfsExtensions/)

Console Application
-------------------
To update retention polices for all build definitions in a Team Project:

    StealFocus.TfsExtensions.Console.exe /tfsUrl:http://server /updateRetentionPolicies /teamProjectName:myTeamProject /numberOfStoppedBuildsToKeep:1 /numberOfFailedBuildsToKeep:5 /numberOfPartiallySucceededBuildsToKeep:5 /numberOfSucceededBuildsToKeep:10 /deleteOptions:All /force:false

To list all Team Build definitions for all Team Projects:

    StealFocus.TfsExtensions.Console.exe /tfsUrl:http://server /listAllTeamBuilds
	
Help
----
Contact the mailing list:
- <StealFocus-TfsExtensions@yahoogroups.co.uk>
- [http://uk.groups.yahoo.com/group/StealFocus-TfsExtensions](http://uk.groups.yahoo.com/group/StealFocus-TfsExtensions)

You can see build status here: [http://build01.stealfocus.co.uk/ccnet/ViewFarmReport.aspx](http://build01.stealfocus.co.uk/ccnet/ViewFarmReport.aspx)
