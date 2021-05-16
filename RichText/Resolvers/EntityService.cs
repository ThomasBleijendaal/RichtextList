using System.Collections.Generic;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Resolvers
{
    public class EntityService : IEntityService
    {
        public async Task<IEnumerable<IEntity>> GetListAsync()
        {
            await Task.Delay(1);

            return new[]
            { 
                new Ticket 
                {
                    Id = "1",
                    Name = "1",
                    SubEntities = new []
                    {
                        new Ticket
                        {
                            Id = "11",
                            Name = "11"
                        }
                    }
                },
                new Ticket
                {
                    Id = "2",
                    Name = "2"
                }
            };
        }

        public async Task<IEntity> UpsertEntityAsync(IEntity entity)
        {
            await Task.Delay(1000);

            return entity;
        }
    }
}
