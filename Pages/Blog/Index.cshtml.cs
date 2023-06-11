using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace CS58_ASP_Razor_09.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly App.Models.AppDbContext _context;

        public IndexModel(App.Models.AppDbContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;
        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int current_page {get;  set;}
        public int count_pages {get; set;}
        public readonly static int countSTT = 0;

        public async Task OnGetAsync(string searchString) {
            int totalArticle = await _context.articles.CountAsync();
            count_pages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);
            if (current_page < 1)
                current_page = 1;
            if (current_page > count_pages)
                current_page = count_pages;

            if (_context.articles != null) {
                var result = (from article in _context.articles
                                orderby article.Created descending
                                select article)
                                .Skip((current_page - 1) * ITEMS_PER_PAGE)
                                .Take(ITEMS_PER_PAGE);
                if (!string.IsNullOrEmpty(searchString)) {
                    Article = result.Where(a => a.Title.Contains(searchString)).ToList();
                } else {
                    Article = await result.ToListAsync();
                }
            }
        }
    }
}
