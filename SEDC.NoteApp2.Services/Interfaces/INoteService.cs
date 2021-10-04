using SEDC.NoteApp2.Dto.Models;
using System.Collections.Generic;

namespace SEDC.NoteApp2.Services.Interfaces
{
    public interface INoteService
    {
        List<NoteDto> GetAllNotes();
        List<NoteDto> GetAllNotesByUserId(int userId);
        NoteDto GetNoteById(int id);
        void AddNote(NoteDto noteDto);
        void UpdateNote(NoteDto noteDto);
        void DeleteNote(int id);
    }
}
