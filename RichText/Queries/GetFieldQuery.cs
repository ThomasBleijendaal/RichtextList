using System;

namespace RichText.Queries
{
    public class GetFieldQuery
    {
        public GetFieldQuery(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}
