namespace StealFocus.TfsExtensions.Web.UI.ReleaseNotes
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using StealFocus.TfsExtensions.Dto;
    using StealFocus.TfsExtensions.Web.UI.Controllers;

    public partial class Show : Page
    {
        public static IEnumerable<WorkItemDto> GetSelected()
        {
            Guid teamProjectCollectionId = new Guid(HttpContext.Current.Request.QueryString[QueryStringKey.TeamProjectCollectionId]);

            // Gets the Work Item IDs from the query string where each one is semi-colon separated with its Team Build definition name
            // e.g. 1;TeamBuildA,3;TeamBuildB,6;TeamBuildA
            string selectedWorkItemIdCommaSeparatedList = HttpContext.Current.Request.QueryString[QueryStringKey.SelectedWorkItemIds];
            string[] selectedWorkItemIds = selectedWorkItemIdCommaSeparatedList.Split(',');
            WorkItemSummaryDto[] workItemSummaries = new WorkItemSummaryDto[selectedWorkItemIds.Length];
            for (int i = 0; i < selectedWorkItemIds.Length; i++)
            {
                // Each item from "selectedWorkItemIds" is of the form "WorkItemId;TeamBuildDefinitionName" 
                // e.g. "6;TeamBuildA"
                string[] valueSplit = selectedWorkItemIds[i].Split(';');
                WorkItemSummaryDto workItemSummaryDto = new WorkItemSummaryDto();
                workItemSummaryDto.Id = int.Parse(valueSplit[0], CultureInfo.CurrentCulture);
                workItemSummaryDto.AssociatedBuildNumber = valueSplit[1];
                workItemSummaries[i] = workItemSummaryDto;
            }

            IEnumerable<WorkItemDto> workItems = ReleaseNotesController.GetTfsConfigurationServer().GetWorkItemsFromTeamBuilds(teamProjectCollectionId, workItemSummaries);
            return workItems;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}