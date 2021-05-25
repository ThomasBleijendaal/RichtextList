using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class Meta : IEntity
    {
        public Meta()
        {
            Id = $"new{Guid.NewGuid()}";
        }

        public string Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }

        public string? EpicIssueType { get; set; }
        public string? UserStoryIssueType { get; set; }
        public string? SubTaskIssueType { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => false;

        public bool IsNew => false;
    }
}
