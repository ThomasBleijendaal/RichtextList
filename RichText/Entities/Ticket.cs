using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class Epic : IEntity
    {
        public Epic(string id, string? name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name;
        }

        public Epic()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
    }

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

    public class UserStoryTask : IEntity
    {
        public UserStoryTask(string id, string? name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name;
        }

        public UserStoryTask()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
    }

    public class SubTask : IEntity
    {
        public SubTask(string id, string? name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name;
        }

        public SubTask()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
    }
}
