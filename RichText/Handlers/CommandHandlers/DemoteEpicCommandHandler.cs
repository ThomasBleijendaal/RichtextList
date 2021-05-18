using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;
using RichText.Entities;

namespace RichText.Handlers.CommandHandlers
{
    // TODO: put item in correct order
    public class DemoteEpicCommandHandler : BaseHandler, ICommandHandlerWithResponse<DemoteEpicCommand, IEntity>
    {
        private readonly ICommandHandler<DeleteEpicCommand> _deleteEpicCommandHandler;
        private readonly ICommandHandler<UpsertUserStoryCommand> _upsertUserStoryCommandHandler;

        public DemoteEpicCommandHandler(
            ICommandHandler<DeleteEpicCommand> deleteEpicCommandHandler,
            ICommandHandler<UpsertUserStoryCommand> upsertUserStoryCommandHandler,
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _deleteEpicCommandHandler = deleteEpicCommandHandler;
            _upsertUserStoryCommandHandler = upsertUserStoryCommandHandler;
        }

        public async Task<IEntity?> HandleAsync(DemoteEpicCommand command)
        {
            if (command.Epic == null)
            {
                return default;
            }

            if (command.Epic.Id.StartsWith("new"))
            {
                return new UserStory(command.Epic.Id, command.Epic.Key, command.Epic.Name);
            }

            await _deleteEpicCommandHandler.HandleAsync(new DeleteEpicCommand { Id = command.Epic.Id });

            var userStory = new UserStory
            {
                Name = command.Epic.Name
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
