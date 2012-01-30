namespace StealFocus.TfsExtensions.Web.UI.ReleaseNotes
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using StealFocus.TfsExtensions.Dto;
    using StealFocus.TfsExtensions.Web.UI.Controllers;

    public partial class Show : Page
    {
        public static WorkItemDtoCollection GetSelected()
        {
            Guid teamProjectCollectionId = new Guid(HttpContext.Current.Request.QueryString[QueryStringKey.TeamProjectCollectionId]);

            // Gets the Work Item IDs from the query string where each one is semi-colon separated with its Team Build definition name
            // e.g. 1;TeamBuildA,3;TeamBuildB,6;TeamBuildA
            string selectedWorkItemIdCommaSeparatedList = HttpContext.Current.Request.QueryString[QueryStringKey.SelectedWorkItemIds];
            string[] selectedWorkItemIds = selectedWorkItemIdCommaSeparatedList.Split(',');
            int[] workItemIds = new int[selectedWorkItemIds.Length];
            string[] buildNumbers = new string[selectedWorkItemIds.Length];
            for (int i = 0; i < selectedWorkItemIds.Length; i++)
            {
                string[] valueSplit = selectedWorkItemIds[i].Split(';');
                workItemIds[i] = int.Parse(valueSplit[0], CultureInfo.CurrentCulture);
                buildNumbers[i] = valueSplit[1];
            }

            WorkItemDtoCollection workItems = ReleaseNotesController.GetTfsConfigurationServer().GetWorkItemsFromTeamBuilds(teamProjectCollectionId, workItemIds, buildNumbers);
            return workItems;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}