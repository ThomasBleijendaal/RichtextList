using System.Collections.Generic;
using System.Threading.Tasks;

namespace RichText.Abstractions
{
    public interface IEntityService
    {
        Task<IEnumerable<IEntity>> GetListAsync();

        Task<IEntity> UpsertEntityAsync(IEntity entity, IEnumerable<IEntity> parentEntities);

        IEntity CreateNewEntity(IEntity? parentEntity);

        Task<IEntity?> ConvertEntityUpAsync(IEntity lowerEntity, IEnumerable<IEntity> parentEntities);

        Task<IEntity?> ConvertEntityDownAsync(IEntity higherEntity, IEntity parentEntity);
    }
}
