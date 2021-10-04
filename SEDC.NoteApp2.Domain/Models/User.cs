using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SEDC.NoteApp2.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public List<Note> Notes { get; set; }
    }
}
