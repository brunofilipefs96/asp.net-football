using System;
using System.ComponentModel.DataAnnotations;

namespace Turma_5413_TP_BrunoSilva.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Article Article { get; set; } = null!;
    }
}
