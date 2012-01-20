namespace StealFocus.TfsExtensions.Dto
{
    using System;

    public class TeamBuildDto
    {
        public string BuildNumber { get; set; }

        public Uri Uri { get; set; }

        public string DefinitionName { get; set; }
    }
}
