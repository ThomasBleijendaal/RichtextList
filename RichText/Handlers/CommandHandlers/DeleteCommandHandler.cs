using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class DeleteCommandHandler : BaseHandler,
        ICommandHandler<DeleteEpicCommand>,
        ICommandHandler<DeleteUserStoryCommand>,
        ICommandHandler<DeleteSubTaskCommand>
    {
        public DeleteCommandHandler(
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task HandleAsync(DeleteEpicCommand command) => await DeleteRequestAsync($"/rest/api/2/issue/{command.Id}");
        public async Task HandleAsync(DeleteUserStoryCommand command) => await DeleteRequestAsync($"/rest/api/2/issue/{command.Id}");
        public async Task HandleAsync(DeleteSubTaskCommand command) => await DeleteRequestAsync($"/rest/api/2/issue/{command.Id}");
    }
}
