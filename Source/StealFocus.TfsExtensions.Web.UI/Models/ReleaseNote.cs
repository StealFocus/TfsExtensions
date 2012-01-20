namespace StealFocus.TfsExtensions.Web.UI.Models
{
    public class ReleaseNote
    {
        public string BuildNumber { get; set; }

        public int WorkItemId { get; set; }

        public string WorkItemTitle { get; set; }

        public string TypeName { get; set; }
    }
}