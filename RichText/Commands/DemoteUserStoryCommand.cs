using RichText.Entities;

namespace RichText.Commands
{
    public class DemoteUserStoryCommand
    {
        public string? UserStoryId { get; set; }
        public string? ProjectId { get; set; }
        public UserStory? UserStory { get; set; }
    }
}
