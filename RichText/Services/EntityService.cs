﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;
using RichText.Enums;
using RichText.Queries;

namespace RichText.Services
{
    public class EntityService : IEntityService
    {
        private readonly IAppState _appState;
        private readonly IQueryHandler<GetBoardsQuery, IReadOnlyList<Board>> _getBoardsQueryHandler;
        private readonly IQueryHandler<GetEpicsQuery, IReadOnlyList<Epic>> _getEpicsQueryHandler;
        private readonly ICommandHandler<UpsertEpicCommand> _upsertEpicCommandHandler;
        private readonly ICommandHandler<UpsertUserStoryCommand> _upsertUserStoryCommandHandler;
        private readonly ICommandHandler<UpsertSubTaskCommand> _upsertSubTaskCommandHandler;
        private readonly ICommandHandlerWithResponse<DemoteEpicCommand, IEntity> _demoteEpicCommandHandler;
        private readonly ICommandHandlerWithResponse<DemoteUserStoryCommand, IEntity> _demoteUserStoryCommandHandler;
        private readonly ICommandHandlerWithResponse<PromoteUserStoryCommand, IEntity> _promoteUserStoryCommandHandler;
        private readonly ICommandHandlerWithResponse<PromoteSubTaskCommand, IEntity> _promoteSubTaskCommandHandler;

        public EntityService(
            IAppState appState,
            IQueryHandler<GetBoardsQuery, IReadOnlyList<Board>> getBoardsQueryHandler,
            IQueryHandler<GetEpicsQuery, IReadOnlyList<Epic>> getEpicsQueryHandler,
            ICommandHandler<UpsertEpicCommand> upsertEpicCommandHandler,
            ICommandHandler<UpsertUserStoryCommand> upsertUserStoryCommandHandler,
            ICommandHandler<UpsertSubTaskCommand> upsertSubTaskCommandHandler,
            ICommandHandlerWithResponse<DemoteEpicCommand, IEntity> demoteEpicCommandHandler,
            ICommandHandlerWithResponse<DemoteUserStoryCommand, IEntity> demoteUserStoryCommandHandler,
            ICommandHandlerWithResponse<PromoteUserStoryCommand, IEntity> promoteUserStoryCommandHandler,
            ICommandHandlerWithResponse<PromoteSubTaskCommand, IEntity> promoteSubTaskCommandHandler)
        {
            _appState = appState;
            _getBoardsQueryHandler = getBoardsQueryHandler;
            _getEpicsQueryHandler = getEpicsQueryHandler;
            _upsertEpicCommandHandler = upsertEpicCommandHandler;
            _upsertUserStoryCommandHandler = upsertUserStoryCommandHandler;
            _upsertSubTaskCommandHandler = upsertSubTaskCommandHandler;
            _demoteEpicCommandHandler = demoteEpicCommandHandler;
            _demoteUserStoryCommandHandler = demoteUserStoryCommandHandler;
            _promoteUserStoryCommandHandler = promoteUserStoryCommandHandler;
            _promoteSubTaskCommandHandler = promoteSubTaskCommandHandler;
        }

        public async Task<IEntity?> ConvertEntityDownAsync(IEntity higherEntity, IEntity parentEntity)
        {
            if (higherEntity is Epic epic && parentEntity is Epic parentEpic)
            {
                var command = new DemoteEpicCommand
                {
                    Epic = epic,
                    ProjectId = _appState.ProjectId,
                    EpicId = parentEpic.Id
                };

                var response = await _demoteEpicCommandHandler.HandleAsync(command);

                return response;
            }
            else if (higherEntity is UserStory userStory && parentEntity is UserStory parentUserStory)
            {
                var command = new DemoteUserStoryCommand
                {
                    UserStory = userStory,
                    ProjectId = _appState.ProjectId,
                    UserStoryId = parentUserStory.Id
                };

                var response = await _demoteUserStoryCommandHandler.HandleAsync(command);

                return response;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IEntity?> ConvertEntityUpAsync(IEntity lowerEntity, IEnumerable<IEntity> parentEntities)
        {
            if (lowerEntity is UserStory userStory)
            {
                var command = new PromoteUserStoryCommand
                {
                    ProjectId = "16100", // MAGIC
                    UserStory = userStory
                };

                var response = await _promoteUserStoryCommandHandler.HandleAsync(command);

                return response;
            }
            else if (lowerEntity is SubTask subTask && parentEntities.OfType<Epic>().FirstOrDefault() is Epic parentEpic)
            {
                var command = new PromoteSubTaskCommand
                {
                    EpicId = parentEpic.Id,
                    ProjectId = "16100",
                    SubTask = subTask
                };

                var response = await _promoteSubTaskCommandHandler.HandleAsync(command);

                return response;
            }
            else
            {
                throw new InvalidOperationException();
            }
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
            if (_appState.State == ViewState.NoBoardConfig)
            {
                return await _getBoardsQueryHandler.QueryAsync(new GetBoardsQuery());
            }
            else if (_appState.State == ViewState.BoardConfig)
            {
                return await _getEpicsQueryHandler.QueryAsync(new GetEpicsQuery { BoardId = _appState.BoardId });
            }
            else
            {
                return Enumerable.Empty<IEntity>();
            }
        }

        public async Task<IEntity> UpsertEntityAsync(IEntity entity, IEnumerable<IEntity> parentEntities)
        {
            if (entity is Epic epic)
            {
                await _upsertEpicCommandHandler.HandleAsync(new UpsertEpicCommand
                {
                    Epic = epic,
                    ProjectId = _appState.ProjectId
                });
                return epic;
            }
            else if (entity is UserStory userStory)
            {
                var parentEpic = parentEntities.OfType<Epic>().First();
                await _upsertUserStoryCommandHandler.HandleAsync(new UpsertUserStoryCommand
                {
                    EpicId = parentEpic.Id,
                    ProjectId = _appState.ProjectId,
                    UserStory = userStory
                });
                return userStory;
            }
            else if (entity is SubTask subTask)
            {
                var parentUserStory = parentEntities.OfType<UserStory>().First();
                await _upsertSubTaskCommandHandler.HandleAsync(new UpsertSubTaskCommand
                {
                    ParentId = parentUserStory.Id,
                    ProjectId = _appState.ProjectId,
                    SubTask = subTask
                });
                return subTask;
            }
            else
            {
                return entity;
            }
        }
    }
}
