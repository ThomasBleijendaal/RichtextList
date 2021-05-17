using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Services
{
    // TODO: support dynamic configuration of down and up casting
    public class EntityService : IEntityService
    {
        public IEntity? ConvertEntityDown(IEntity higherEntity)
        {
            return higherEntity switch
            {
                Epic => new UserStory(higherEntity.Id, higherEntity.Name),
                UserStory => new SubTask(higherEntity.Id, higherEntity.Name),

                _ => default
            };
        }

        public IEntity? ConvertEntityUp(IEntity lowerEntity)
        {
            return lowerEntity switch
            {
                UserStory => new Epic(lowerEntity.Id, lowerEntity.Name),
                SubTask => new UserStory(lowerEntity.Id, lowerEntity.Name),

                _ => default
            };
        }

        public IEntity CreateNewEntity(IEntity? parentEntity)
        {
            return parentEntity switch
            {
                Epic => new UserStory(),
                UserStory => new SubTask(),

                _ => new Epic()
            };
        }

        public async Task<IEnumerable<IEntity>> GetListAsync()
        {
            await Task.Delay(1);

            return new[]
            {
                new Epic
                {
                    Id = "1",
                    Name = "1",
                    SubEntities = new []
                    {
                        new UserStory
                        {
                            Id = "11",
                            Name = "11"
                        }
                    }
                },
                new Epic
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
