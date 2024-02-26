using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class ChangePasswordModel : TravaloudBasePageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        var hasPassword = await UserManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToPage("./SetPassword");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        await SignInManager.RefreshSignInAsync(user);
        StatusMessage = "Your password has been changed.";

        return RedirectToPage();
    }
}