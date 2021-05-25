using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RichText.Abstractions;
using RichText.Entities;
using RichText.Queries;

namespace RichText.Handlers.QueryHandlers
{
    public class GetUserStoriesQueryHandler : BaseHandler, IQueryHandler<GetUserStoriesQuery, IReadOnlyList<UserStory>>
    {
        private readonly IResultsResolver<UserStory> _userStoryResultsResolver;

        public GetUserStoriesQueryHandler(
            IAppState appState,
            IResultsResolver<UserStory> userStoryResultsResolver,
            IHttpClientFactory httpClientFactory) : base(appState, httpClientFactory)
        {
            _userStoryResultsResolver = userStoryResultsResolver;
        }

        public async Task<IReadOnlyList<UserStory>> QueryAsync(GetUserStoriesQuery query)
        {
            var results = await GetRequestAsync($"/rest/agile/1.0/epic/{query.EpicId}/issue");
            return _userStoryResultsResolver.Resolve(results);
        }
    }
}
