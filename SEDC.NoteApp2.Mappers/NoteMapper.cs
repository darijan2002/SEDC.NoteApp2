using SEDC.NoteApp2.Domain.Models;
using SEDC.NoteApp2.Dto.Models;

namespace SEDC.NoteApp2.Mappers
{
    public static class NoteMapper
    {
        public static Note ToNote(this NoteDto noteDto)
        {
            Note note = new Note()
            {
                Id = noteDto.Id,
                Text = noteDto.Text,
                Color = noteDto.Color,
                Tag = noteDto.Tag,
                UserId = noteDto.UserId
            };

            return note;
        }

        public static NoteDto ToNoteDto(this Note note)
        {
            NoteDto noteDto = new NoteDto()
            {
                Id = note.Id,
                Text = note.Text,
                Color = note.Color,
                Tag = note.Tag,
                UserId = note.UserId,
                UserFullName = string.Empty
            };

            if (note.User != null)
            {
                noteDto.UserFullName = $"{note.User.FirstName} {note.User.LastName}";
            }

            return noteDto;
        }
    }
}
