using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    public class DeleteModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public DeleteModel(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }
        [TempData]
        public string StatusMessage {get; set;}
        public IdentityRole role {get; set;}
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thấy Role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Không tìm thấy Role");
            return Page();
                
        }
        public async Task<IActionResult> OnPostAsync(string roleid) {
            if (!ModelState.IsValid)
                return Page();
            if (roleid == null)
                return NotFound("Không tìm thấy Role");

            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Không tìm thấy Role");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded) {
                StatusMessage = $"Bạn đã xóa role: {role.Name}";
                return RedirectToPage("./Index");
            } else {
                 result.Errors.ToList().ForEach( error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                 });
            }

            // await _roleManager.CreateAsync(newRole);

            return Page();
        }
    }
}
