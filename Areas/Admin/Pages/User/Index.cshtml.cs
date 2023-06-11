using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.User
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class UserAndRole : AppUser
        {
            public string? RoleNames { get; set; }
        }
        public List<UserAndRole> _users { get; set; }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int current_page { get; set; }
        public int count_pages { get; set; }
        public readonly static int countSTT = 0;

        public int totalUser { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
        public async void OnGet()
        {
            try
            {
                var qr = _userManager.Users.OrderBy(name => name.UserName);
                totalUser = qr.Count();
                count_pages = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE);
                if (current_page < 1)
                    current_page = 1;
                if (current_page > count_pages)
                    current_page = count_pages;

                _users = new List<UserAndRole>();
                
                var qr1 = qr.Skip((current_page - 1) * ITEMS_PER_PAGE)
                            .Take(ITEMS_PER_PAGE)
                            .Select(u => new UserAndRole()
                            {
                                Id = u.Id,
                                UserName = u.UserName
                            });
                _users = qr1.ToList();

                foreach (var user in _users)
                {
                    try
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        user.RoleNames = string.Join(", ", roles);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}, UserId: {user.Id}");
                    }

                }
            }

            catch (Exception ex)
            {
                // Ghi log lỗi ở đây
                Console.WriteLine($"Error: {ex.Message}");
                StatusMessage = $"Error: {ex.Message}";
            }

        }
        public void OnPost() => RedirectToPage();
    }
}
