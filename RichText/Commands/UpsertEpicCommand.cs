using RichText.Entities;

namespace RichText.Commands
{
    public class UpsertEpicCommand
    {
        public string? ProjectId { get; set; }
        public Epic? Epic { get; set; }
    }
}
