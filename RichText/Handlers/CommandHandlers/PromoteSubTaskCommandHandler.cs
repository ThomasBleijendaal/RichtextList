using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;

namespace RichText.Handlers.CommandHandlers
{
    // TODO: can this be done by just removing parent id?
    // TODO: put item in correct order
    public class PromoteSubTaskCommandHandler : BaseHandler, ICommandHandlerWithResponse<PromoteSubTaskCommand, IEntity>
    {
        private readonly ICommandHandler<DeleteSubTaskCommand> _deleteSubTaskCommandHandler;
        private readonly ICommandHandler<UpsertUserStoryCommand> _upsertUserStoryCommandHandler;

        public PromoteSubTaskCommandHandler(
            IAppState appState,
            ICommandHandler<DeleteSubTaskCommand> deleteSubTaskCommandHandler,
            ICommandHandler<UpsertUserStoryCommand> upsertUserStoryCommandHandler,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _deleteSubTaskCommandHandler = deleteSubTaskCommandHandler;
            _upsertUserStoryCommandHandler = upsertUserStoryCommandHandler;
        }

        public async Task<IEntity?> HandleAsync(PromoteSubTaskCommand command)
        {
            if (command.SubTask == null)
            {
                return default;
            }

            if (command.SubTask.Id.StartsWith("new"))
            {
                return new UserStory(command.SubTask.Id, command.SubTask.Key, command.SubTask.Name);
            }

            await _deleteSubTaskCommandHandler.HandleAsync(new DeleteSubTaskCommand { Id = command.SubTask.Id });

            var userStory = new UserStory
            {
                Name = command.SubTask.Name
            };

            await _upsertUserStoryCommandHandler.HandleAsync(new UpsertUserStoryCommand
            {
                EpicId = command.EpicId,
                ProjectId = command.ProjectId,
                UserStory = userStory
            });

            return userStory;
        }
    }
}
