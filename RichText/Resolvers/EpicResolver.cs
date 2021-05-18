using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Resolvers
{
    public class EpicResolver : IResultsResolver<Epic>
    {
        public IReadOnlyList<Epic> Resolve(string json)
        {
            var objects = JsonConvert.DeserializeAnonymousType(json, new
            {
                values = new[]
                {
                    new
                    {
                        id = 0,
                        key = "",
                        name = "",
                        color = new
                        {
                            key = ""
                        }
                    }
                }
            });

            return objects?.values.Select(x => new Epic
            {
                Color = x.color.key,
                Id = x.id.ToString(),
                Key = x.key,
                Name = x.name
            }).ToList() ?? new List<Epic>();
        }
    }
}
