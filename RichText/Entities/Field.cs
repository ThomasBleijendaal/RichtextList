using System;
using System.Collections.Generic;
using RichText.Abstractions;

namespace RichText.Entities
{
    public class Field : IEntity
    {
        public Field()
        {
            Id = $"new{Guid.NewGuid()}";
        }

        public string Id { get; set; }
        public string? Key { get; set; }
        public string? Name { get; set; }

        public IEnumerable<IEntity>? SubEntities { get; set; }

        public bool IsSaveable => false;

        public bool IsNew => false;
    }
}
