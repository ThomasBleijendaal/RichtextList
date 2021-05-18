using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class UserStory : IEntity
    {
        public UserStory(string id, string? key, string? name)
        {
            Id = id;
            Key = key;
            Name = name;
        }

        public UserStory()
        {
            Id = $"new{Guid.NewGuid()}";
        }

        public string Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
        public bool IsNew => Id?.Contains("new") ?? true;
    }
}
