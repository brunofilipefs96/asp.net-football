using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Turma_5413_TP_BrunoSilva.Data;
using Turma_5413_TP_BrunoSilva.Models;
using System.Collections.Generic;

namespace Turma_5413_TP_BrunoSilva.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ArticlesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Articles
        public async Task<IActionResult> Index(string searchString)
        {
            var articles = from a in _context.Articles.Include(a => a.Ratings)
                           select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                if (DateTime.TryParse(searchString, out var date))
                {
                    articles = articles.Where(a => a.PublishedDate.Year == date.Year);
                    if (date.Month > 0)
                    {
                        articles = articles.Where(a => a.PublishedDate.Month == date.Month);
                    }
                }
                else if (int.TryParse(searchString, out var year))
                {
                    articles = articles.Where(a => a.PublishedDate.Year == year);
                }
                else
                {
                    articles = articles.Where(a => a.Title.Contains(searchString) || a.Author.Contains(searchString));
                }
            }

            ViewBag.CurrentFilter = searchString;

            var articlesList = await articles.ToListAsync();
            foreach (var article in articlesList)
            {
                article.Rating = article.Ratings.Any() ? article.Ratings.Average(r => r.Rating) : 0;
            }

            // Passar a estrutura de anos e meses para a view
            ViewBag.ArticleDates = await GetArticleDatesAsync();

            return View(articlesList);
        }


        // Método para obter as datas dos artigos
        private async Task<Dictionary<int, Dictionary<int, int>>> GetArticleDatesAsync()
        {
            var articleDates = await _context.Articles
                .GroupBy(a => new { a.PublishedDate.Year, a.PublishedDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var result = new Dictionary<int, Dictionary<int, int>>();
            foreach (var date in articleDates)
            {
                if (!result.ContainsKey(date.Year))
                {
                    result[date.Year] = new Dictionary<int, int>();
                }
                result[date.Year][date.Month] = date.Count;
            }

            return result;
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            bool hasVoted = false;
            int votesCount = 0;

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Cliente"))
            {
                var userId = _userManager.GetUserId(User);
                hasVoted = await _context.ArticleRatings
                    .AnyAsync(r => r.ArticleId == id && r.UserId == userId);
            }

            votesCount = await _context.ArticleRatings
                .CountAsync(r => r.ArticleId == id);

            ViewBag.HasVoted = hasVoted;
            ViewBag.VotesCount = votesCount;
            ViewBag.Comments = await _context.Comments
                .Where(c => c.ArticleId == id)
                .ToListAsync();

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,PublishedDate,IsPublic,ImageUrl")] Article article)
        {
            if (ModelState.IsValid)
            {
                var author = _userManager.GetUserName(User);
                if (author == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to determine the author of the article.");
                    return View(article);
                }

                article.Author = author;
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PublishedDate,IsPublic,ImageUrl")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Articles/RateArticle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateArticle(int ArticleId, int rating)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var article = await _context.Articles.FindAsync(ArticleId);
            if (article == null)
            {
                return NotFound();
            }

            var existingRating = await _context.ArticleRatings
                .FirstOrDefaultAsync(r => r.ArticleId == ArticleId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
            }
            else
            {
                _context.ArticleRatings.Add(new ArticleRating
                {
                    ArticleId = ArticleId,
                    UserId = userId,
                    Rating = rating
                });
            }

            await _context.SaveChangesAsync();

            // Update article rating
            var ratings = await _context.ArticleRatings
                .Where(r => r.ArticleId == ArticleId)
                .ToListAsync();
            article.Rating = ratings.Average(r => r.Rating);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = ArticleId });
        }

        // POST: Articles/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AddComment(int articleId, string content)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                ArticleId = articleId,
                UserId = userId,
                UserName = _userManager.GetUserName(User),
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = articleId });
        }

        // POST: Articles/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente,Administrador")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != _userManager.GetUserId(User) && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = comment.ArticleId });
        }

        // POST: Articles/DeleteAllComments
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteAllComments(int articleId)
        {
            var comments = await _context.Comments
                .Where(c => c.ArticleId == articleId)
                .ToListAsync();

            _context.Comments.RemoveRange(comments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = articleId });
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
