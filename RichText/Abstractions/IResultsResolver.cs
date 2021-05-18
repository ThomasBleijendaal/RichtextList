using System.Collections.Generic;

namespace RichText.Abstractions
{
    public interface IResultsResolver<TEntity>
        where TEntity : IEntity
    {
        IReadOnlyList<TEntity> Resolve(string json);
    }
}
