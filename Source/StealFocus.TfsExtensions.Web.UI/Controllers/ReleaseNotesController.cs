namespace StealFocus.TfsExtensions.Web.UI.Controllers
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.TeamFoundation.Client;

    using StealFocus.TfsExtensions.Dto;

    public class ReleaseNotesController : Controller
    {
        public static TfsConfigurationServer GetTfsConfigurationServer()
        {
            string tfsUrlAppSetting = ConfigurationManager.AppSettings["TfsUrl"];
            Uri tfsUri;
            if (string.IsNullOrEmpty(tfsUrlAppSetting))
            {
                tfsUri = new Uri("http://" + System.Web.HttpContext.Current.Request.Url.Host);
            }
            else
            {
                tfsUri = new Uri(tfsUrlAppSetting);
            }

            string username = ConfigurationManager.AppSettings["TfsUsername"];
            if (string.IsNullOrEmpty(username))
            {
                return TfsConfigurationServerFactoryExtensions.GetConfigurationServerAndAuthenticate(tfsUri);
            }

            string domain = ConfigurationManager.AppSettings["TfsUserDomain"];
            string password = ConfigurationManager.AppSettings["TfsUserPassword"];
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
            ViewData["TeamProjectCollectionId"] = teamProjectCollectionId;
            ViewData["TeamProjectName"] = teamProjectName;
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
            routeValueDictionary.Add("teamProjectCollectionId", teamProjectCollectionId);
            routeValueDictionary.Add("teamProjectName", teamProjectName);
            routeValueDictionary.Add("selectedTeamBuildDefinitions", selectedTeamBuildDefinitions);
            return RedirectToAction("TeamBuilds", routeValueDictionary);
        }

        [HttpGet]
        public ActionResult TeamBuilds(Guid teamProjectCollectionId, string teamProjectName, string selectedTeamBuildDefinitions)
        {
            if (string.IsNullOrEmpty(selectedTeamBuildDefinitions))
            {
                throw new ArgumentNullException("selectedTeamBuildDefinitions");
            }

            ViewData["TeamProjectCollectionId"] = teamProjectCollectionId;
            ViewData["TeamProjectName"] = teamProjectName;
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
            string teamProjectName = formCollection[FormCollectionItemName.TeamProjectName];
            string selectedTeamBuildUris = formCollection[FormCollectionItemName.SelectedTeamBuildUris];
            string redirectUrl = GetReportUrl(teamProjectCollectionId, teamProjectName, selectedTeamBuildUris);
            return Redirect(redirectUrl);
        }

        private static string GetReportUrl(Guid teamProjectCollectionId, string teamProjectName, string selectedTeamBuildUris)
        {
            return string
                .Format(
                    CultureInfo.CurrentCulture,
                    "~/ReleaseNotes/Show.aspx?{0}={1}&{2}={3}&{4}={5}",
                    QueryStringKey.TeamProjectCollectionId,
                    teamProjectCollectionId,
                    QueryStringKey.TeamProjectName,
                    teamProjectName,
                    QueryStringKey.SelectedTeamBuildUris,
                    selectedTeamBuildUris);
        }
    }
}
