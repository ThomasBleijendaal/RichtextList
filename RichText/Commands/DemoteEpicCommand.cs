using RichText.Entities;

namespace RichText.Commands
{
    public class DemoteEpicCommand
    {
        public string? EpicId { get; set; }
        public string? ProjectId { get; set; }
        public Epic? Epic { get; set; }
    }
}
