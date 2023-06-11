using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }

        public class RoleModel : IdentityRole {
            public string[] Claims {get; set;}
        }
        public List<RoleModel> _roles {get; set;}

        [TempData]
        public string StatusMessage {get; set;}
        public async Task OnGet()
        {
            var roles = _roleManager.Roles.OrderBy(name => name.Name).ToList();
            _roles = new List<RoleModel>();
            foreach (var r in roles ) {
                var claims = await _roleManager.GetClaimsAsync(r);
                var claimString = claims.Select(c => c.Type + "=" + c.Value);
                RoleModel role = new RoleModel() {
                    Name = r.Name,
                    Id = r.Id,
                    Claims = claimString.ToArray()
                };
                _roles.Add(role);
            }
        }
        public void OnPost() => RedirectToPage();
    }
}
