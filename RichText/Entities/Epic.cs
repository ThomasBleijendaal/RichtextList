using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class Epic : IEntity
    {
        public Epic(string id, string? key, string? name)
        {
            Id = id;
            Name = name;
        }

        public Epic()
        {
            Id = $"new{Guid.NewGuid()}";
        }

        public string Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => !string.IsNullOrWhiteSpace(Name);
        public bool IsNew => Id?.Contains("new") ?? true;
    }
}
