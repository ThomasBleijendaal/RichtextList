using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class UpsertEpicCommandHandler : BaseHandler, ICommandHandler<UpsertEpicCommand>
    {
        public UpsertEpicCommandHandler(
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
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
                    fields = new
                    {
                        summary = command.Epic.Name,
                        customfield_10006 = command.Epic.Name, // MAGIC
                        project = new
                        {
                            id = command.ProjectId
                        },
                        issuetype = new
                        {
                            id = "10000" // MAGIC
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
                    fields = new
                    {
                        summary = command.Epic.Name,
                        customfield_10006 = command.Epic.Name // MAGIC
                    }
                });
            }
        }
    }
}
