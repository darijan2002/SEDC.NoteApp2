using SEDC.NoteApp2.Shared.Enums;

namespace SEDC.NoteApp2.Domain.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public TagType Tag { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
