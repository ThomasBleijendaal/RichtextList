using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Queries;

namespace RichText.Handlers.QueryHandlers
{
    public class GetEpicsQueryHandler : BaseHandler, IQueryHandler<GetEpicsQuery, IReadOnlyList<Epic>>
    {
        private readonly IResultsResolver<Epic> _epicResultsResolver;
        private readonly IQueryHandler<GetUserStoriesQuery, IReadOnlyList<UserStory>> _userStoriesQueryHandler;

        public GetEpicsQueryHandler(
            IAppState appState,
            IResultsResolver<Epic> epicResultsResolver,
            IQueryHandler<GetUserStoriesQuery, IReadOnlyList<UserStory>> userStoriesQueryHandler,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _epicResultsResolver = epicResultsResolver;
            _userStoriesQueryHandler = userStoriesQueryHandler;
        }

        public async Task<IReadOnlyList<Epic>> QueryAsync(GetEpicsQuery query)
        {
            var results = await GetRequestAsync($"/rest/agile/1.0/board/{query.BoardId}/epic");
            var epics = _epicResultsResolver.Resolve(results);

            foreach (var epic in epics)
            {
                var userStoryResults = await _userStoriesQueryHandler.QueryAsync(new GetUserStoriesQuery { EpicId = epic.Id });

                epic.SubEntities = userStoryResults;
            }

            return epics;
        }
    }
}
