using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Turma_5413_TP_BrunoSilva.Models
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string>();

        public IList<string> AvailableRoles { get; set; } = new List<string>();
    }
}
