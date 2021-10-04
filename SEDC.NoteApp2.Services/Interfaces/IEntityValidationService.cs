using SEDC.NoteApp2.Dto.Models;
using SEDC.NoteApp2.Dto.ValidationModels;

namespace SEDC.NoteApp2.Services.Interfaces
{
    public interface IEntityValidationService
    {
        ValidationResponse ValidateNote(NoteDto noteDto);
        ValidationResponse ValidateRegisterUser(RegisterUserDto userDto);
    }
}
