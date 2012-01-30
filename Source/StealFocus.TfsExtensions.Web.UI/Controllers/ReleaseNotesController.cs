namespace StealFocus.TfsExtensions.Web.UI.Controllers
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.TeamFoundation.Client;

    using StealFocus.TfsExtensions.Dto;
    using StealFocus.TfsExtensions.Web.UI.Configuration;

    public class ReleaseNotesController : Controller
    {
        public static TfsConfigurationServer GetTfsConfigurationServer()
        {
            string tfsUrlAppSetting = ConfigurationManager.AppSettings[AppSettingsKey.TfsUrl];
            Uri tfsUri;
            if (string.IsNullOrEmpty(tfsUrlAppSetting))
            {
                tfsUri = new Uri("http://" + System.Web.HttpContext.Current.Request.Url.Host);
            }
            else
            {
                tfsUri = new Uri(tfsUrlAppSetting);
            }

            string username = ConfigurationManager.AppSettings[AppSettingsKey.TfsUserName];
            if (string.IsNullOrEmpty(username))
            {
                return TfsConfigurationServerFactoryExtensions.GetConfigurationServerAndAuthenticate(tfsUri);
            }

            string domain = ConfigurationManager.AppSettings[AppSettingsKey.TfsUserDomain];
            string password = ConfigurationManager.AppSettings[AppSettingsKey.TfsUserPassword];
            return TfsConfigurationServerFactoryExtensions.GetConfigurationServerAndAuthenticate(tfsUri, domain, username, password);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TeamProject()
        {
            TeamProjectDtoCollection teamProjects = GetTfsConfigurationServer().GetAllTeamProjectsInAllTeamProjectCollections();
            return View(teamProjects);
        }

        [HttpGet]
        public ActionResult TeamBuildDefinitions(Guid teamProjectCollectionId, string teamProjectName)
        {
            BuildDefinitionDtoCollection buildDefinitionDtoCollection = GetTfsConfigurationServer().GetBuildDefinitions(teamProjectCollectionId, teamProjectName);
            ViewData[ViewDataKey.TeamProjectCollectionId] = teamProjectCollectionId;
            ViewData[ViewDataKey.TeamProjectName] = teamProjectName;
            return View(buildDefinitionDtoCollection);
        }

        [HttpPost]
        public ActionResult TeamBuildDefinitions(FormCollection formCollection)
        {
            if (formCollection == null)
            {
                throw new ArgumentNullException("formCollection");
            }

            Guid teamProjectCollectionId = Guid.Parse(formCollection[FormCollectionItemName.TeamProjectCollectionId]);
            string teamProjectName = formCollection[FormCollectionItemName.TeamProjectName];
            string selectedTeamBuildDefinitions = formCollection[FormCollectionItemName.SelectedTeamBuildDefinitions];
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add(RouteValueDictionaryKey.TeamProjectCollectionId, teamProjectCollectionId);
            routeValueDictionary.Add(RouteValueDictionaryKey.TeamProjectName, teamProjectName);
            routeValueDictionary.Add(RouteValueDictionaryKey.SelectedTeamBuildDefinitions, selectedTeamBuildDefinitions);
            return RedirectToAction(ActionName.TeamBuilds, routeValueDictionary);
        }

        [HttpGet]
        public ActionResult TeamBuilds(Guid teamProjectCollectionId, string teamProjectName, string selectedTeamBuildDefinitions)
        {
            if (string.IsNullOrEmpty(selectedTeamBuildDefinitions))
            {
                throw new ArgumentNullException("selectedTeamBuildDefinitions");
            }

            ViewData[ViewDataKey.TeamProjectCollectionId] = teamProjectCollectionId;
            ViewData[ViewDataKey.TeamProjectName] = teamProjectName;
            string[] selectedTeamBuildDefinitionsList = selectedTeamBuildDefinitions.Split(',');
            TeamBuildDtoCollection teamBuildDtoCollection = GetTfsConfigurationServer().GetTeamBuilds(teamProjectCollectionId, teamProjectName, selectedTeamBuildDefinitionsList);
            return View(teamBuildDtoCollection);
        }

        [HttpPost]
        public ActionResult TeamBuilds(FormCollection formCollection)
        {
            if (formCollection == null)
            {
                throw new ArgumentNullException("formCollection");
            }

            Guid teamProjectCollectionId = Guid.Parse(formCollection[FormCollectionItemName.TeamProjectCollectionId]);
            string selectedTeamBuildUris = formCollection[FormCollectionItemName.SelectedTeamBuildUris];
            RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
            routeValueDictionary.Add(RouteValueDictionaryKey.TeamProjectCollectionId, teamProjectCollectionId);
            routeValueDictionary.Add(RouteValueDictionaryKey.SelectedTeamBuildUris, selectedTeamBuildUris);
            return RedirectToAction(ActionName.WorkItems, routeValueDictionary);
        }

        [HttpGet]
        public ActionResult WorkItems(Guid teamProjectCollectionId, string selectedTeamBuildUris)
        {
            if (string.IsNullOrEmpty(selectedTeamBuildUris))
            {
                throw new ArgumentNullException("selectedTeamBuildUris");
            }

            ViewData[ViewDataKey.TeamProjectCollectionId] = teamProjectCollectionId;
            string[] selectedTeamBuildUrisList = selectedTeamBuildUris.Split(',');
            ArrayList arrayList = new ArrayList(selectedTeamBuildUrisList.Length);
            foreach (string selectedTeamBuildUri in selectedTeamBuildUrisList)
            {
                arrayList.Add(new Uri(selectedTeamBuildUri));
            }
            
            Uri[] uris = (Uri[])arrayList.ToArray(typeof(Uri));
            WorkItemDtoCollection workItemsFromTeamBuilds = GetTfsConfigurationServer().GetWorkItemsFromTeamBuilds(teamProjectCollectionId, uris);
            return View(workItemsFromTeamBuilds);
        }

        [HttpPost]
        public ActionResult WorkItems(FormCollection formCollection)
        {
            if (formCollection == null)
            {
                throw new ArgumentNullException("formCollection");
            }

            Guid teamProjectCollectionId = Guid.Parse(formCollection[FormCollectionItemName.TeamProjectCollectionId]);

            // Gets the Work Item IDs from the form collection where each one is semi-colon separated with its Team Build definition name
            // e.g. 1;TeamBuildA,3;TeamBuildB,6;TeamBuildA
            string selectedWorkItemIds = formCollection[FormCollectionItemName.SelectedWorkItemIds];
            string redirectUrl = GetReportUrl(teamProjectCollectionId, selectedWorkItemIds);
            return Redirect(redirectUrl);
        }

        private static string GetReportUrl(Guid teamProjectCollectionId, string selectedWorkItemIds)
        {
            return string
                .Format(
                    CultureInfo.CurrentCulture,
                    "~/ReleaseNotes/Show.aspx?{0}={1}&{2}={3}",
                    QueryStringKey.TeamProjectCollectionId,
                    teamProjectCollectionId,
                    QueryStringKey.SelectedWorkItemIds,
                    selectedWorkItemIds);
        }
    }
}
