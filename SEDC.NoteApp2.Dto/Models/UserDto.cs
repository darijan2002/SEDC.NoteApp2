using System.Collections.Generic;

namespace SEDC.NoteApp2.Dto.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public List<NoteDto> Notes { get; set; }
    }
}
