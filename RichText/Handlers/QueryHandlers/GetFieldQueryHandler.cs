using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Queries;

namespace RichText.Handlers.QueryHandlers
{
    public class GetFieldQueryHandler : BaseHandler, IQueryHandler<GetFieldQuery, Field>
    {
        public GetFieldQueryHandler(
            IAppState appState,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
        }

        public async Task<Field> QueryAsync(GetFieldQuery query)
        {
            var result = await GetRequestAsync($"/rest/api/2/field");

            var obj = JsonConvert.DeserializeAnonymousType(result, new[]
            {
               new
               {
                   id = "",
                   name = "",
               }
            });

            var field = obj?.Where(x => x.name == query.Name).FirstOrDefault() ?? throw new InvalidOperationException();

            return new Field
            {
                Id = field.id,
                Name = field.name
            };
        }
    }
}
