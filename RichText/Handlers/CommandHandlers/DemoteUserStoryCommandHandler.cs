using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;

namespace RichText.Handlers.CommandHandlers
{
    // TODO: can this be done by just asigning a parent?
    // TODO: put item in correct order
    public class DemoteUserStoryCommandHandler : BaseHandler, ICommandHandlerWithResponse<DemoteUserStoryCommand, IEntity>
    {
        private readonly ICommandHandler<DeleteUserStoryCommand> _deleteUserStoryCommandHandler;
        private readonly ICommandHandler<UpsertSubTaskCommand> _upsertSubTaskCommandHandler;

        public DemoteUserStoryCommandHandler(
            ICommandHandler<DeleteUserStoryCommand> deleteUserStoryCommandHandler,
            ICommandHandler<UpsertSubTaskCommand> upsertSubTaskCommandHandler,
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _deleteUserStoryCommandHandler = deleteUserStoryCommandHandler;
            _upsertSubTaskCommandHandler = upsertSubTaskCommandHandler;
        }

        public async Task<IEntity?> HandleAsync(DemoteUserStoryCommand command)
        {
            if (command.UserStory == null)
            {
                return default;
            }

            if (command.UserStory.Id.StartsWith("new"))
            {
                return new SubTask(command.UserStory.Id, command.UserStory.Key, command.UserStory.Name);
            }

            await _deleteUserStoryCommandHandler.HandleAsync(new DeleteUserStoryCommand { Id = command.UserStory.Id });

            var subTask = new SubTask
            {
                Name = command.UserStory.Name
            };

            await _upsertSubTaskCommandHandler.HandleAsync(new UpsertSubTaskCommand
            {
                ParentId = command.UserStoryId,
                ProjectId = command.ProjectId,
                SubTask = subTask
            });

            return subTask;
        }
    }
}
