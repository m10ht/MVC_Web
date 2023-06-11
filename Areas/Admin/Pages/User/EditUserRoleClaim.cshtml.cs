using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly AppDbContext _myBlogContext;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(AppDbContext myBlogContext, UserManager<AppUser> userManager) {
            _myBlogContext = myBlogContext;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage {get; set;}
        public AppUser user {get; set;}
        public IdentityUserClaim<string> userClaim {get; set;}

        [BindProperty]
        public InputModel Input {get; set;}
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


        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");

        public async Task<IActionResult> OnGetAddClaimAsync(string userid) {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                return NotFound("Không tìm thấy userrr");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid) {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null)
                return NotFound("Không tìm thấy trang");
            if (!ModelState.IsValid)
                return NotFound("Không tìm thấy trang");
            
            var claims = _myBlogContext.UserClaims.Where(c => c.UserId == user.Id);
            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue)) {
                ModelState.AddModelError(string.Empty, "Đặc tính đã có rồi");
                return Page();
            }
            var result = await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }
            StatusMessage = "Đã thêm đặc tính cho User";
            return RedirectToPage("./AddRole", new {Id = user.Id});
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid) {
            if (claimid == null)
                return NotFound("Không tìm thấy Claim");
            userClaim = _myBlogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null)
                return NotFound("Không tìm thấy userrr");
            Input = new InputModel {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid) {
            if (claimid == null)
                return NotFound("Không tìm thấy Claim");
            userClaim = _myBlogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null)
                return NotFound("Không tìm thấy userrr");
            if (!ModelState.IsValid)
                return NotFound("Không tìm thấy trang");

            if (_myBlogContext.UserClaims.Any(uc => uc.Id == userClaim.Id &&
                                            uc.ClaimType == Input.ClaimType &&
                                            uc.ClaimValue == Input.ClaimValue &&
                                            uc.UserId == user.Id))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính đã tồn tại");
                return Page();
            }
            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;

            await _myBlogContext.SaveChangesAsync();

            StatusMessage = "Bạn đã cập nhật đặc tính thành công";
            return RedirectToPage("./AddRole", new {Id = user.Id});
            
        }

        public async Task<IActionResult> OnPostDeleteClaimAsync(int? claimid) {
            if (claimid == null)
                return NotFound("Không tìm thấy Claim");
            userClaim = _myBlogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null)
                return NotFound("Không tìm thấy userrr");

            await _userManager.RemoveClaimAsync(user, new Claim (userClaim.ClaimType, userClaim.ClaimValue));
            StatusMessage = "Bạn đã xóa đặc tính";
            return RedirectToPage("./AddRole", new {Id = user.Id});
            
        }

    }
}
