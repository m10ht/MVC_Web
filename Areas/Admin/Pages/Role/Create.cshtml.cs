using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public CreateModel(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }
        [TempData]
        public string StatusMessage {get; set;}
        [BindProperty]
        public CreateInput Input {get; set;}

        public class CreateInput {
            [Required(ErrorMessage = "Phải nhập tên Role")]
            [Display(Name = "Tên Role")]
            [StringLength(100, ErrorMessage = "Phải nhập tên Role từ {2} đến {1} ký tự", MinimumLength = 3)]
            public string roleName {get; set;}
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid)
                return Page();

            var newRole = new IdentityRole(Input.roleName);
            var newRoleResult = await _roleManager.CreateAsync(newRole);
            if (newRoleResult.Succeeded) {
                StatusMessage = $"Bạn đã tạo role mới: {Input.roleName}";
                return RedirectToPage("./Index");
            } else {
                 newRoleResult.Errors.ToList().ForEach( error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                 });
            }

            // await _roleManager.CreateAsync(newRole);

            return Page();
        }
    }
}
