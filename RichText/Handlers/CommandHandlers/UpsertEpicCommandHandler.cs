using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class UpsertEpicCommandHandler : BaseHandler, ICommandHandler<UpsertEpicCommand>
    {
        private readonly IAppState _appState;

        public UpsertEpicCommandHandler(
            IAppState appState,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _appState = appState;
        }

        public async Task HandleAsync(UpsertEpicCommand command)
        {
            if (command.Epic == null)
            {
                return;
            }

            if (command.Epic.Id.StartsWith("new"))
            {
                var response = await PostRequestAsync("/rest/api/2/issue", new
                {
                    update = new { },
                    fields = new Dictionary<string, object?>
                    {
                        ["summary"] = command.Epic.Name,
                        [_appState.EpicNamePropertyName] = command.Epic.Name,
                        ["project"] = new
                        {
                            id = command.ProjectId
                        },
                        ["issuetype"] = new
                        {
                            id = _appState.EpicIssueType
                        }
                    }
                });

                var result = JObject.Parse(response);
                if (result == null)
                {
                    throw new InvalidOperationException();
                }

                command.Epic.Id = result.Value<string>("id") ?? throw new InvalidOperationException();
                command.Epic.Key = result.Value<string>("key");
            }
            else
            {
                await PutRequestAsync($"/rest/api/2/issue/{command.Epic.Id}", new
                {
                    update = new { },
                    fields = new Dictionary<string, string?>
                    {
                        ["summary"] = command.Epic.Name,
                        [_appState.EpicNamePropertyName] = command.Epic.Name
                    }
                });
            }
        }
    }
}
