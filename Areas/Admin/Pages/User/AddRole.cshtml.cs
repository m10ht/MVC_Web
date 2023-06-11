// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using App.Models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AddRoleModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        [BindProperty]
        [DisplayName("Các role gán cho User")]
        public string[] _roleNames {get; set;}
        public AppUser user {get; set;}
        public SelectList allRoles {get; set;}

        public List<IdentityRoleClaim<string>> claimsInRoleClaim {set; get;}
        public List<IdentityUserClaim<string>> claimsInUserClaim {set; get;}
        [TempData]
        public string StatusMessage {get; set;}
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound("Không có User");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Không có User");

            _roleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleNames = _roleManager.Roles.Select(r => r.Name).ToList();
            allRoles = new SelectList(roleNames);

            GetClaims(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
                return Page();
            
            user = await _userManager.FindByIdAsync(id);
            if (user == null) {
                return NotFound("Không tìm thấy User");
            }
            GetClaims(id);

            var presentRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var deleteRoles = presentRoleNames.Where(r => !_roleNames.Contains(r));
            var addRoles = _roleNames.Where(r => !presentRoleNames.Contains(r));

            List<string> roleNames = _roleManager.Roles.Select(r => r.Name).ToList();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
                if (!resultDelete.Succeeded) {
                    foreach (var error in resultDelete.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                        return Page();
                }
            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
                if (!resultAdd.Succeeded) {
                    foreach (var error in resultDelete.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            StatusMessage = "Bạn đã cập nhật role thành công";

            return RedirectToPage("./Index");
        }

        void GetClaims(string id) {
            var listRoles = from ur in _context.UserRoles
                            join c in _context.Roles on ur.RoleId equals c.Id
                            where ur.UserId == id
                            select c;

            var _claimInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;
            claimsInRoleClaim = _claimInRole.ToList();

            claimsInUserClaim = (from uc in _context.UserClaims
                                where uc.UserId == id
                                select uc).ToList();
        }
    }
}
