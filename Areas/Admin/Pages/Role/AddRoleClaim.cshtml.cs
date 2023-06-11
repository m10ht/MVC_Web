using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    public class AddRoleClaimModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }
        [TempData]
        public string StatusMessage {get; set;}
        [BindProperty]
        public InputModel Input {get; set;}

        public IdentityRole role {get; set;}

        public class InputModel {
            [Required(ErrorMessage = "Phải nhập tên Claim")]
            [Display(Name = "Tên Claim")]
            [StringLength(100, ErrorMessage = "Phải nhập tên Claim từ {2} đến {1} ký tự", MinimumLength = 3)]
            public string ClaimType {get; set;}

            [Required(ErrorMessage = "Phải nhập tên giá trị Claim")]
            [Display(Name = "Giá trị")]
            [StringLength(100, ErrorMessage = "Phải nhập giá trị Claim từ {2} đến {1} ký tự", MinimumLength = 3)]
            public string ClaimValue {get; set;}
        }
        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if (roleid == null)
                return NotFound("Không tìm thầy role");

            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return NotFound("Không tìm thầy role");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid) {
            if (!ModelState.IsValid)
                return Page();
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
                return Page();
            if ((await _roleManager.GetClaimsAsync(role)).Any(c=> c.Type == Input.ClaimType && c.Value == Input.ClaimValue)) {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            Claim claim = new Claim(Input.ClaimType, Input.ClaimValue);

            var result = await _roleManager.AddClaimAsync(role, claim);

            if (!result.Succeeded) {
                 result.Errors.ToList().ForEach( error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                 });
                 return Page();
            }
            StatusMessage = $"Bạn đã tạo đặc tính cho role thành công";
            return RedirectToPage("./Edit", new {roleid=role.Id});
        }
    }
}
