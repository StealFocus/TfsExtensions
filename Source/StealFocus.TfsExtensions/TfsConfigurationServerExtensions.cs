namespace StealFocus.TfsExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Common;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using StealFocus.TfsExtensions.Dto;

    public static class TfsConfigurationServerExtensions
    {
        public static TeamProjectDtoCollection GetAllTeamProjectsInAllTeamProjectCollections(this TfsConfigurationServer tfsConfigurationServer)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            TeamProjectDtoCollection teamProjectDtoCollection = new TeamProjectDtoCollection();
            ReadOnlyCollection<CatalogNode> teamProjectCollectionNodes = tfsConfigurationServer.CatalogNode.QueryChildren(new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);
            foreach (CatalogNode teamProjectCollectionNode in teamProjectCollectionNodes)
            {
                Guid collectionId = new Guid(teamProjectCollectionNode.Resource.Properties[TeamProjectCollectionResourcePropertyName.InstanceId]);
                TfsTeamProjectCollection teamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(collectionId);
                ReadOnlyCollection<CatalogNode> projectNodes = teamProjectCollectionNode.QueryChildren(new[] { CatalogResourceTypes.TeamProject }, false, CatalogQueryOptions.None);
                foreach (CatalogNode teamProjectNode in projectNodes)
                {
                    TeamProjectDto teamProjectDto = new TeamProjectDto();
                    teamProjectDto.CollectionId = teamProjectCollection.InstanceId;
                    teamProjectDto.CollectionName = teamProjectCollection.Name;
                    teamProjectDto.DisplayName = teamProjectNode.Resource.DisplayName;
                    teamProjectDto.Uri = new Uri(teamProjectNode.Resource.Properties[TeamProjectResourcePropertyName.ProjectUri]);
                    teamProjectDto.Id = Guid.Parse(teamProjectNode.Resource.Properties[TeamProjectResourcePropertyName.ProjectId]);
                    teamProjectDtoCollection.Add(teamProjectDto);
                }
            }

            return teamProjectDtoCollection;
        }

        public static BuildDefinitionDtoCollection GetBuildDefinitions(this TfsConfigurationServer tfsConfigurationServer, Guid teamProjectCollectionId, string teamProjectName)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            TfsTeamProjectCollection tfsTeamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectCollectionId);
            IBuildServer buildServer = tfsTeamProjectCollection.GetService<IBuildServer>();
            IBuildDefinition[] buildDefinitions = buildServer.QueryBuildDefinitions(teamProjectName);
            BuildDefinitionDtoCollection buildDefinitionDtoCollection = new BuildDefinitionDtoCollection();
            foreach (IBuildDefinition buildDefinition in buildDefinitions)
            {
                BuildDefinitionDto buildDefinitionDto = new BuildDefinitionDto();
                buildDefinitionDto.TeamProjectName = buildDefinition.TeamProject;
                buildDefinitionDto.Name = buildDefinition.Name;
                buildDefinitionDto.Id = buildDefinition.Id;
                buildDefinitionDto.Uri = buildDefinition.Uri;
                buildDefinitionDtoCollection.Add(buildDefinitionDto);
            }

            return buildDefinitionDtoCollection;
        }

        public static TeamBuildDtoCollection GetTeamBuilds(this TfsConfigurationServer tfsConfigurationServer, Guid teamProjectCollectionId, string teamProjectName, IEnumerable<string> teamBuildDefinitionNames)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            if (teamBuildDefinitionNames == null)
            {
                throw new ArgumentNullException("teamBuildDefinitionNames");
            }

            TfsTeamProjectCollection tfsTeamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectCollectionId);
            IBuildServer buildServer = tfsTeamProjectCollection.GetService<IBuildServer>();
            TeamBuildDtoCollection teamBuildDtoCollection = new TeamBuildDtoCollection();
            foreach (string teamBuildDefinitionName in teamBuildDefinitionNames)
            {
                IBuildDetailSpec buildDetailSpec = buildServer.CreateBuildDetailSpec(teamProjectName, teamBuildDefinitionName);
                buildDetailSpec.Reason = BuildReason.All;
                buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
                buildDetailSpec.QueryDeletedOption = QueryDeletedOption.ExcludeDeleted;
                IBuildQueryResult buildQueryResult = buildServer.QueryBuilds(buildDetailSpec);
                foreach (IBuildDetail buildDetail in buildQueryResult.Builds)
                {
                    TeamBuildDto teamBuildDto = new TeamBuildDto();
                    teamBuildDto.BuildNumber = buildDetail.BuildNumber;
                    teamBuildDto.Uri = buildDetail.Uri;
                    teamBuildDto.DefinitionName = teamBuildDefinitionName;
                    teamBuildDtoCollection.Add(teamBuildDto);
                }
            }

            return teamBuildDtoCollection;
        }

        public static WorkItemDtoCollection GetWorkItemsFromTeamBuilds(this TfsConfigurationServer tfsConfigurationServer, Guid teamProjectCollectionId, Uri[] teamBuildUris)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            TfsTeamProjectCollection tfsTeamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectCollectionId);
            IBuildServer buildServer = tfsTeamProjectCollection.GetService<IBuildServer>();
            WorkItemStore workItemStore = tfsTeamProjectCollection.GetService<WorkItemStore>();
            IBuildDetail[] buildDetails = buildServer.QueryBuildsByUri(teamBuildUris, new[] { InformationTypes.AssociatedWorkItem }, QueryOptions.Definitions);
            Dictionary<int, WorkItemDto> workItems = new Dictionary<int, WorkItemDto>();
            foreach (IBuildDetail buildDetail in buildDetails)
            {
                List<IWorkItemSummary> workItemSummaries = InformationNodeConverters.GetAssociatedWorkItems(buildDetail);
                foreach (IWorkItemSummary workItemSummary in workItemSummaries)
                {
                    if (!workItems.ContainsKey(workItemSummary.WorkItemId))
                    {
                        WorkItem workItem = workItemStore.GetWorkItem(workItemSummary.WorkItemId);
                        WorkItemDto workItemDto = WorkItemDto.CreateFromWorkItem(workItem, buildDetail.BuildNumber);
                        workItems.Add(workItem.Id, workItemDto);
                    }
                }
            }

            WorkItemDtoCollection workItemDtoCollection = new WorkItemDtoCollection();
            foreach (KeyValuePair<int, WorkItemDto> keyValuePair in workItems)
            {
                workItemDtoCollection.Add(keyValuePair.Value);
            }

            return workItemDtoCollection;
        }

        public static IEnumerable<WorkItemDto> GetWorkItemsFromTeamBuilds(this TfsConfigurationServer tfsConfigurationServer, Guid teamProjectCollectionId, IEnumerable<WorkItemSummaryDto> workItems)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            if (workItems == null)
            {
                throw new ArgumentNullException("workItems");
            }

            TfsTeamProjectCollection tfsTeamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectCollectionId);
            WorkItemStore workItemStore = tfsTeamProjectCollection.GetService<WorkItemStore>();
            WorkItemDtoCollection workItemDtoCollection = new WorkItemDtoCollection();
            foreach (WorkItemSummaryDto workItemSummaryDto in workItems)
            {
                WorkItem workItem = workItemStore.GetWorkItem(workItemSummaryDto.Id);
                WorkItemDto workItemDto = WorkItemDto.CreateFromWorkItem(workItem, workItemSummaryDto.AssociatedBuildNumber);
                workItemDtoCollection.Add(workItemDto);
            }

            return workItemDtoCollection;
        }
    }
}
