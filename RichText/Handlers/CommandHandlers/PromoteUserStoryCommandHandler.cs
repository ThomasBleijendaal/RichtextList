using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;

namespace RichText.Handlers.CommandHandlers
{
    // TODO: put item in correct order
    public class PromoteUserStoryCommandHandler : BaseHandler, ICommandHandlerWithResponse<PromoteUserStoryCommand, IEntity>
    {
        private readonly ICommandHandler<DeleteUserStoryCommand> _deleteUserStoryCommandHandler;
        private readonly ICommandHandler<UpsertEpicCommand> _upsertEpicCommandHandler;

        public PromoteUserStoryCommandHandler(
            ICommandHandler<DeleteUserStoryCommand> deleteUserStoryCommandHandler,
            ICommandHandler<UpsertEpicCommand> upsertEpicCommandHandler,
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _deleteUserStoryCommandHandler = deleteUserStoryCommandHandler;
            _upsertEpicCommandHandler = upsertEpicCommandHandler;
        }

        public async Task<IEntity?> HandleAsync(PromoteUserStoryCommand command)
        {
            if (command.UserStory == null)
            {
                return default;
            }

            if (command.UserStory.Id.StartsWith("new"))
            {
                return new Epic(command.UserStory.Id, command.UserStory.Key, command.UserStory.Name);
            }

            await _deleteUserStoryCommandHandler.HandleAsync(new DeleteUserStoryCommand { Id = command.UserStory.Id });

            var epic = new Epic
            {
                Name = command.UserStory.Name
            };

            await _upsertEpicCommandHandler.HandleAsync(new UpsertEpicCommand
            {
                ProjectId = command.ProjectId,
                Epic = epic
            });

            return epic;
        }
    }
}
