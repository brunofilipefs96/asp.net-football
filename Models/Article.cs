using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Turma_5413_TP_BrunoSilva.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        public string? ImageUrl { get; set; }

        public string? Author { get; set; }

        public double Rating { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<ArticleRating> Ratings { get; set; } = new List<ArticleRating>();
    }
}
