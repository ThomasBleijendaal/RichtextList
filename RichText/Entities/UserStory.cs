using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class UserStory : IEntity
    {
        public UserStory(string id, string? name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name;
        }

        public UserStory()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
    }
}
