using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.Role
{
    public class EditRoleClaimModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext context) {
            _roleManager = roleManager;
            _context = context;
        }
        [TempData]
        public string StatusMessage {get; set;}
        [BindProperty]
        public InputModel Input {get; set;}

        public IdentityRole role {get; set;}

        public IdentityRoleClaim<string> claim {set; get;}

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
        public async Task<IActionResult> OnGetAsync(int? claimid)
        {
            if (claimid == null)
                return NotFound("Không tìm thầy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null)
                return NotFound("Không tìm thầy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) 
                return NotFound("Không tìm thầy role");

            Input = new InputModel() {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? claimid) {
            if (claimid == null)
                return NotFound("Không tìm thầy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null)
                return NotFound("Không tìm thầy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) 
                return NotFound("Không tìm thầy role");
            if (!ModelState.IsValid)
                return Page();

            if ((_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))) {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            // var result = await _roleManager.AddClaimAsync(role, claim);

            StatusMessage = $"Bạn cập nhật thành công";
            return RedirectToPage("./Edit", new {roleid=role.Id});
        }


        public async Task<IActionResult> OnPostDeleteAsync(int? claimid) {
            if (claimid == null)
                return NotFound("Không tìm thầy role");

            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null)
                return NotFound("Không tìm thầy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) 
                return NotFound("Không tìm thầy role");

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            // var result = await _roleManager.AddClaimAsync(role, claim);

            StatusMessage = $"Bạn đã xóa thành công";
            return RedirectToPage("./Edit", new {roleid=role.Id});
        }

        
    }
}
