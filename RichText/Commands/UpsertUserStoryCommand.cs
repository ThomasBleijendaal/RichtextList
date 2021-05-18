using RichText.Entities;

namespace RichText.Commands
{
    public class UpsertUserStoryCommand
    {
        public string? ProjectId { get; set; }
        public string? EpicId { get; set; }
        public UserStory? UserStory { get; set; }
    }
}
