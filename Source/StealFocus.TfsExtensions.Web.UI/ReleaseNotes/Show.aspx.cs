namespace StealFocus.TfsExtensions.Web.UI.ReleaseNotes
{
    using System;
    using System.Web;
    using System.Web.UI;
    using StealFocus.TfsExtensions.Dto;
    using StealFocus.TfsExtensions.Web.UI.Controllers;
    using StealFocus.TfsExtensions.Web.UI.Models;

    public partial class Show : Page
    {
        public static ReleaseNoteCollection GetSelected()
        {
            Guid teamProjectCollectionId = new Guid(HttpContext.Current.Request.QueryString[QueryStringKey.TeamProjectCollectionId]);
            string teamProjectName = HttpContext.Current.Request.QueryString[QueryStringKey.TeamProjectName];
            string selectedTeamBuildUriCommaSeparatedList = HttpContext.Current.Request.QueryString[QueryStringKey.SelectedTeamBuildUris];
            string[] selectedTeamBuildUris = selectedTeamBuildUriCommaSeparatedList.Split(',');
            Uri[] selectedTeamBuildUrisList = new Uri[selectedTeamBuildUris.Length];
            for (int i = 0; i < selectedTeamBuildUris.Length; i++)
            {
                selectedTeamBuildUrisList[i] = new Uri(selectedTeamBuildUris[i]);
            }

            WorkItemDtoCollection workItems = ReleaseNotesController.GetTfsConfigurationServer().GetWorkItems(teamProjectCollectionId, selectedTeamBuildUrisList);
            ReleaseNoteCollection releaseNotes = new ReleaseNoteCollection();
            foreach (WorkItemDto workItem in workItems)
            {
                ReleaseNote releaseNote = new ReleaseNote();
                releaseNote.BuildNumber = teamProjectCollectionId.ToString();
                releaseNote.TypeName = teamProjectName;
                releaseNote.WorkItemId = workItem.Id;
                releaseNote.WorkItemTitle = selectedTeamBuildUriCommaSeparatedList;
                releaseNotes.Add(releaseNote);
            }

            return releaseNotes;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}