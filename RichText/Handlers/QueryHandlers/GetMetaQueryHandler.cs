using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Queries;

namespace RichText.Handlers.QueryHandlers
{
    public class GetMetaQueryHandler : BaseHandler, IQueryHandler<GetMetaQuery, Meta>
    {
        private readonly IResultResolver<Meta> _metaResultResolver;

        public GetMetaQueryHandler(
            IAppState appState,
            IResultResolver<Meta> metaResultResolver,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _metaResultResolver = metaResultResolver;
        }

        public async Task<Meta> QueryAsync(GetMetaQuery query)
        {
            var result = await GetRequestAsync($"/rest/api/2/issue/createmeta");
            var meta = _metaResultResolver.Resolve(result);

            return meta;
        }
    }
}
