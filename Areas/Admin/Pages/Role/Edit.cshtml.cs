using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public EditModel(RoleManager<IdentityRole> roleManager, AppDbContext context) {
            _roleManager = roleManager;
            _context = context;
        }
        [TempData]
        public string StatusMessage {get; set;}
        [BindProperty]
        public EditInput Input {get; set;}

        public List<IdentityRoleClaim<string>> Claims {get; set;}

        public class EditInput {
            [Required(ErrorMessage = "Phải nhập tên Role")]
            [Display(Name = "Tên Role")]
            [StringLength(100, ErrorMessage = "Phải nhập tên Role từ {2} đến {1} ký tự", MinimumLength = 3)]
            public string roleName {get; set;}
        }

        public IdentityRole role {get; set;}
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thấy Role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role != null) {
                    Input = new EditInput {
                    roleName = role.Name
                };
                Claims = _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToList();
                return Page();
            } else
                return NotFound("Không tìm thấy Role");
        }
        public async Task<IActionResult> OnPostAsync(string roleid) {
            if (!ModelState.IsValid)
                return Page();
            if (roleid == null)
                return NotFound("Không tìm thấy Role");

            role = await _roleManager.FindByIdAsync(roleid);

            Claims = _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToList();
            if (role == null)
                return NotFound("Không tìm thấy Role");

            role.Name = Input.roleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded) {
                StatusMessage = $"Bạn đã chỉnh sửa role: {Input.roleName}";
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
