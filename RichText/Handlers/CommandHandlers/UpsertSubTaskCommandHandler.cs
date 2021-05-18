using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RichText.Abstractions;
using RichText.Commands;

namespace RichText.Handlers.CommandHandlers
{
    public class UpsertSubTaskCommandHandler : BaseHandler, ICommandHandler<UpsertSubTaskCommand>
    {
        public UpsertSubTaskCommandHandler(
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task HandleAsync(UpsertSubTaskCommand command)
        {
            if (command.SubTask == null)
            {
                return;
            }

            if (command.SubTask.Id.StartsWith("new"))
            {
                var response = await PostRequestAsync("/rest/api/2/issue", new
                {
                    update = new { },
                    fields = new
                    {
                        summary = command.SubTask.Name,
                        project = new
                        {
                            id = command.ProjectId
                        },
                        issuetype = new
                        {
                            id = "10101" // MAGIC
                        },
                        parent = new
                        {
                            id = command.ParentId
                        },
                        description = "TODO" // TODO
                    }
                });

                var result = JObject.Parse(response);
                if (result == null)
                {
                    throw new InvalidOperationException();
                }

                command.SubTask.Id = result.Value<string>("id") ?? throw new InvalidOperationException();
                command.SubTask.Key = result.Value<string>("key");
            }
            else
            {
                await PutRequestAsync($"/rest/api/2/issue/{command.SubTask.Id}", new
                {
                    update = new { },
                    fields = new
                    {
                        summary = command.SubTask.Name,
                        description = "TODO"
                    }
                });
            }
        }
    }
}
