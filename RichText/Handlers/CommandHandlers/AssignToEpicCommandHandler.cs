using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class AssignToEpicCommandHandler : BaseHandler, ICommandHandler<AssignToEpicCommand>
    {
        public AssignToEpicCommandHandler(
            IAppState appState,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
        }

        public async Task HandleAsync(AssignToEpicCommand command)
        {
            await PostRequestAsync($"/rest/agile/1.0/epic/{command.EpicId}/issue", new
            {
                issues = new[] { command.Id }
            });
        }
    }
}
