using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Resolvers
{
    public class UserStoryResolver : IResultsResolver<UserStory>
    {
        public IReadOnlyList<UserStory> Resolve(string json)
        {
            var objects = JsonConvert.DeserializeAnonymousType(json, new
            {
                issues = new[]
                {
                    new
                    {
                        id = "",
                        key = "",
                        fields = new
                        {
                            summary = "",
                            subtasks = new []
                            {
                                new
                                {
                                    id = "",
                                    key = "",
                                    fields = new
                                    {
                                        summary = ""
                                    }
                                }
                            }
                        }
                    }
                }
            });

            return objects?.issues.Select(x => new UserStory
            {
                Id = x.id,
                Key = x.key,
                Name = x.fields.summary,
                SubEntities = x.fields.subtasks.Select(y => new SubTask
                {
                    Id = y.id,
                    Key = y.key,
                    Name = y.fields.summary
                })
            }).ToList() ?? new List<UserStory>();
        }
    }
}
