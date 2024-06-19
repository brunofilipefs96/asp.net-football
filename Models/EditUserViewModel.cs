using System.ComponentModel.DataAnnotations;

namespace Turma_5413_TP_BrunoSilva.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
