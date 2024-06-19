using System.ComponentModel.DataAnnotations;

namespace Turma_5413_TP_BrunoSilva.Models
{
    public class ArticleRating
    {
        public int Id { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; 

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } 

        public Article Article { get; set; } = null!;
    }
}
