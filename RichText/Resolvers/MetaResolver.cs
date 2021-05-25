using System;
using System.Linq;
using Newtonsoft.Json;
using RichText.Abstractions;
using RichText.Entities;

namespace RichText.Resolvers
{
    public class MetaResolver : IResultResolver<Meta>
    {
        public Meta Resolve(string json)
        {
            var obj = JsonConvert.DeserializeAnonymousType(json, new
            {
                projects = new[]
                {
                    new
                    {
                        id = "",
                        key = "",
                        name = "",
                        issuetypes = new []
                        {
                            new
                            {
                                id = "",
                                name = ""
                            }
                        }
                    }
                }
            });

            var proj = obj?.projects.FirstOrDefault() ?? throw new InvalidOperationException();

            return new Meta
            {
                Id = proj.id,
                Key = proj.key,
                Name = proj.name,
                EpicIssueType = proj.issuetypes.First(x => x.name == "Epic").id,
                SubTaskIssueType = proj.issuetypes.First(x => x.name == "Sub-task").id,
                UserStoryIssueType = proj.issuetypes.First(x => x.name == "Story").id,
            };
        }
    }
}
