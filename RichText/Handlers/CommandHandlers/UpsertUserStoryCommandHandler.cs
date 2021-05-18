using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class UpsertUserStoryCommandHandler : BaseHandler, ICommandHandler<UpsertUserStoryCommand>
    {
        private readonly ICommandHandler<AssignToEpicCommand> _assignToEpicCommandHandler;

        public UpsertUserStoryCommandHandler(
            ICommandHandler<AssignToEpicCommand> assignToEpicCommandHandler,
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _assignToEpicCommandHandler = assignToEpicCommandHandler;
        }

        public async Task HandleAsync(UpsertUserStoryCommand command)
        {
            if (command.UserStory == null)
            {
                return;
            }

            if (command.UserStory.Id.StartsWith("new"))
            {
                var response = await PostRequestAsync("/rest/api/2/issue", new
                {
                    update = new { },
                    fields = new
                    {
                        summary = command.UserStory.Name,
                        project = new
                        {
                            id = command.ProjectId
                        },
                        issuetype = new
                        {
                            id = "10001" // MAGIC
                        },
                        description = "TODO" // TODO
                    }
                });

                var result = JObject.Parse(response);
                if (result == null)
                {
                    throw new InvalidOperationException();
                }

                command.UserStory.Id = result.Value<string>("id") ?? throw new InvalidOperationException();
                command.UserStory.Key = result.Value<string>("key");

                await _assignToEpicCommandHandler.HandleAsync(new AssignToEpicCommand
                {
                    EpicId = command.EpicId,
                    Id = command.UserStory.Id
                });
            }
            else
            {
                await PutRequestAsync($"/rest/api/2/issue/{command.UserStory.Id}", new
                {
                    update = new { },
                    fields = new
                    {
                        summary = command.UserStory.Name,
                        description = "TODO"
                    }
                });
            }
        }
    }
}
