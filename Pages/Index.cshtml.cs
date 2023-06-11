using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace CS58_ASP_Razor_09.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _myBlogContext;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext myBlogContext)
    {
        _logger = logger;
        _myBlogContext = myBlogContext;
    }

    public void OnGet()
    {
        var posts = (from a in _myBlogContext.articles
                    orderby a.Created descending
                    select a).ToList();

        if (posts != null) {
            foreach (var post in posts)
                Console.WriteLine($"*** {post.Title} ***");
        } else
            Console.WriteLine("Post is Null");
        ViewData["post"] = posts;
    }
}
