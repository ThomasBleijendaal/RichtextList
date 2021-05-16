using System.Collections.Generic;
using System.Threading.Tasks;

namespace RichText.Abstractions
{
    public interface IEntityService
    {
        Task<IEnumerable<IEntity>> GetListAsync();

        Task<IEntity> UpsertEntityAsync(IEntity entity);
    }
}
