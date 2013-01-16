namespace StealFocus.TfsExtensions.Tests.Build.Client
{
    using System;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StealFocus.TfsExtensions.Build.Client;
    using StealFocus.TfsExtensions.Dto;

    [TestClass]
    public class BuildServerFacadeTests
    {
        [TestMethod]
        [Ignore]
        public void IntegrationTestUpdateRetentionPolicies()
        {
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri("myTfsUrl"));
            tfsConfigurationServer.Authenticate();
            TeamProjectDtoCollection allTeamProjectCollections = tfsConfigurationServer.GetAllTeamProjectsInAllTeamProjectCollections();
            foreach (TeamProjectDto teamProjectDto in allTeamProjectCollections)
            {
                TfsTeamProjectCollection teamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectDto.CollectionId);
                IBuildServer buildServer = teamProjectCollection.GetService<IBuildServer>();
                BuildServerFacade buildServerFacade = new BuildServerFacade(buildServer);
                buildServerFacade.UpdateRetentionPolicies(teamProjectDto.DisplayName, 1, 5, 5, 10, "All");
            }
        }
    }
}
