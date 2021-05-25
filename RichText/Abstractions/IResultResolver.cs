using System.Collections.Generic;

namespace RichText.Abstractions
{
    public interface IResultResolver<TEntity>
        where TEntity : IEntity
    {
        TEntity Resolve(string json);
    }
}
