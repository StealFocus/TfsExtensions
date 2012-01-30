namespace StealFocus.TfsExtensions.Dto
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

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

        public static WorkItemDto CreateFromWorkItem(WorkItem workItem, string associatedBuildNumber)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException("workItem");
            }

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
            workItemDto.AssociatedBuildNumber = associatedBuildNumber;
            return workItemDto;
        }
    }
}
