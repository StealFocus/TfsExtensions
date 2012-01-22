namespace StealFocus.TfsExtensions.Dto
{
    using System;

    public class WorkItemDto
    {
        public string TypeName { get; set; }

        public int Id { get; set; }

        public string ChangedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; }

        public string Reason { get; set; }

        public string State { get; set; }

        public string Title { get; set; }

        public Uri Uri { get; set; }

        public string AssociatedBuildNumber { get; set; }
    }
}
