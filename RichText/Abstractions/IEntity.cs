﻿using System.Collections.Generic;

namespace RichText.Abstractions
{
    public interface IEntity
    {
        string Id { get; set; }
        string? Name { get; set; }

        IEnumerable<IEntity>? SubEntities { get; set; }

        bool IsSaveable { get; }
    }
}
