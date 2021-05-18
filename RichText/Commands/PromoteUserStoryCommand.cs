using RichText.Entities;

namespace RichText.Commands
{
    public class PromoteUserStoryCommand
    {
        public string? ProjectId { get; set; }
        public UserStory? UserStory { get; set; }
    }
}
