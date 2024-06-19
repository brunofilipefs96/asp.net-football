using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Turma_5413_TP_BrunoSilva.Models;

namespace Turma_5413_TP_BrunoSilva.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Articles.Any())
                {
                    Console.WriteLine("DB has been seeded");
                    return; 
                }

                Console.WriteLine("Seeding database with articles...");
                var articles = GetArticlesFromJson(Path.Combine("wwwroot", "json", "articles.json")); 

                context.Articles.AddRange(articles);
                await context.SaveChangesAsync();
                Console.WriteLine("Database seeded successfully.");
            }
        }

        private static List<Article> GetArticlesFromJson(string filePath)
        {
            Console.WriteLine($"Reading articles from {filePath}");
            var json = File.ReadAllText(filePath);
            var articlesRoot = JsonConvert.DeserializeObject<ArticlesRoot>(json);
            return articlesRoot?.Articles ?? new List<Article>();
        }
    }

    public class ArticlesRoot
    {
        public List<Article> Articles { get; set; }
    }
}
