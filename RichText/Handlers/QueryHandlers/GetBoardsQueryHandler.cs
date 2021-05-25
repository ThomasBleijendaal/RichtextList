using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Queries;

namespace RichText.Handlers.QueryHandlers
{
    public class GetBoardsQueryHandler : BaseHandler, IQueryHandler<GetBoardsQuery, IReadOnlyList<Board>>
    {
        private readonly IResultsResolver<Board> _boardResultsResolver;

        public GetBoardsQueryHandler(
            IAppState appState,
            IResultsResolver<Board> boardResultsResolver,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _boardResultsResolver = boardResultsResolver;
        }

        public async Task<IReadOnlyList<Board>> QueryAsync(GetBoardsQuery query)
        {
            var results = await GetRequestAsync($"/rest/agile/1.0/board");
            var boards = _boardResultsResolver.Resolve(results);

            return boards;
        }
    }
}
