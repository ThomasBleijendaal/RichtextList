using RichText.Entities;

namespace RichText.Commands
{
    public class UpsertSubTaskCommand
    {
        public string? ProjectId { get; set; }
        public string? ParentId { get; set; }
        public SubTask? SubTask { get; set; }
    }
}
