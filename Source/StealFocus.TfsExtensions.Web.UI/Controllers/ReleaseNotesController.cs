namespace StealFocus.TfsExtensions.Web.UI.Controllers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Web.Mvc;

    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;

    public class ReleaseNotesController : Controller
    {
        public ActionResult Index()
        {
            Uri tfsUrl = new Uri("http://tfspoc.acme.com");
            TfsCredentialsProvider tfsCredentialsProvider = new TfsCredentialsProvider("___", "___", "___");
            TfsConfigurationServer tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUrl, tfsCredentialsProvider);
            tfsConfigurationServer.Authenticate();
            ReadOnlyCollection<CatalogNode> collectionNodes = tfsConfigurationServer.CatalogNode.QueryChildren(new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection teamProjectCollection = tfsConfigurationServer.GetTeamProjectCollection(collectionId);
                System.Diagnostics.Debug.WriteLine("tpc name - " + teamProjectCollection.Name);
            }

            return View();
        }

        public ActionResult SingleBuild()
        {
            return View();
        }

        public ActionResult ListOfBuilds()
        {
            return View();
        }
    }
}
