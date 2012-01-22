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

        public static WorkItemDtoCollection GetWorkItems(this TfsConfigurationServer tfsConfigurationServer, Guid teamProjectCollectionId, Uri[] teamBuildUris)
        {
            if (tfsConfigurationServer == null)
            {
                throw new ArgumentNullException("tfsConfigurationServer");
            }

            TfsTeamProjectCollection tfsTeamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(teamProjectCollectionId);
            IBuildServer buildServer = tfsTeamProjectCollection.GetService<IBuildServer>();
            WorkItemStore workItemStore = tfsTeamProjectCollection.GetService<WorkItemStore>();
            IBuildDetail[] buildDetails = buildServer.QueryBuildsByUri(teamBuildUris, new[] { InformationTypes.AssociatedWorkItem }, QueryOptions.Definitions);
            WorkItemDtoCollection workItemDtoCollection = new WorkItemDtoCollection();
            foreach (IBuildDetail buildDetail in buildDetails)
            {
                List<IWorkItemSummary> workItemSummaries = InformationNodeConverters.GetAssociatedWorkItems(buildDetail);
                foreach (IWorkItemSummary workItemSummary in workItemSummaries)
                {
                    WorkItem workItem = workItemStore.GetWorkItem(workItemSummary.WorkItemId);
                    WorkItemDto workItemDto = new WorkItemDto();
                    workItemDto.Id = workItem.Id;
                    workItemDto.ChangedBy = workItem.ChangedBy;
                    workItemDto.ChangedDate = workItem.ChangedDate;
                    workItemDto.CreatedBy = workItem.CreatedBy;
                    workItemDto.CreatedDate = workItem.CreatedDate;
                    workItemDto.Description = workItem.Description;
                    workItemDto.Reason = workItem.Reason;
                    workItemDto.State = workItem.State;
                    workItemDto.Title = workItem.Title;
                    workItemDto.Uri = workItem.Uri;
                    workItemDto.TypeName = workItem.Type.Name;
                    workItemDto.AssociatedBuildNumber = buildDetail.BuildNumber;
                    workItemDtoCollection.Add(workItemDto);
                }
            }

            return workItemDtoCollection;
        }
    }
}
