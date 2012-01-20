namespace StealFocus.TfsExtensions.Dto
{
    using System;

    public class TeamProjectDto
    {
        public Guid CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string DisplayName { get; set; }

        public Uri Uri { get; set; }

        public Guid Id { get; set; }
    }
}
