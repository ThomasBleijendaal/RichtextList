using RichText.Entities;

namespace RichText.Commands
{
    public class PromoteSubTaskCommand
    {
        public string? EpicId { get; set; }
        public string? ProjectId { get; set; }
        public SubTask? SubTask { get; set; }
    }
}
