using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Resolvers
{
    public class BoardResolver : IResultsResolver<Board>
    {
        public IReadOnlyList<Board> Resolve(string json)
        {
            var objects = JsonConvert.DeserializeAnonymousType(json, new
            {
                values = new[]
                {
                    new
                    {
                        id = 0,
                        name = "",
                        type = "scrum"
                    }
                }
            });

            return objects?.values.Select(x => new Board
            {
                Id = x.id.ToString(),
                Type = x.type,
                Name = x.name
            }).ToList() ?? new List<Board>();
        }
    }
}
