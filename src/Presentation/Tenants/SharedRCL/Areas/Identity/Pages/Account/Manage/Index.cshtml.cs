using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Areas.Identity.Pages.Account.Manage;

public class IndexModel : TravaloudBasePageModel
{
    [BindProperty]
    public InputModel? Input { get; set; }
    
    public class InputModel : RegisterModalComponent
    {
        public string? Username { get; set; }

        [Display(Name = "Passport Number")]
        public string? PassportNumber { get; set; }

        [Display(Name = "Passport Issue Date")]
        public DateTime? PassportIssueDate { get; set; }

        [Display(Name = "Passport Expiry Date")]
        public DateTime? PassportExpiryDate { get; set; }

        [Display(Name = "Passport Issuing Country")]
        public string? PassportIssuingCountry { get; set; }

        [Display(Name = "Visa Issue Date")]
        public DateTime? VisaIssueDate { get; set; }

        [Display(Name = "Visa Expiry Date")]
        public DateTime? VisaExpiryDate { get; set; }
    }

    private async Task LoadAsync(ApplicationUser user)
    {
        await base.OnGetDataAsync();
            
        Input = new InputModel
        {
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            FirstName = user.FirstName,
            Surname = user.LastName,
            Gender = user.Gender,
            PassportNumber = user.PassportNumber,
            Nationality = user.Nationality,
            PassportExpiryDate = user.PassportExpiryDate,
            PassportIssueDate = user.PassportIssueDate,
            PassportIssuingCountry = user.PassportIssuingCountry,
            VisaIssueDate = user.VisaIssueDate,
            VisaExpiryDate = user.VisaExpiryDate
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(InputModel model)
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
        }

        user.DateOfBirth = model.DateOfBirth;
        user.FirstName = model.FirstName;
        user.LastName = model.Surname;
        user.FullName = $"{model.FirstName} {model.Surname}";
        user.Gender = model.Gender;
        user.Nationality = model.Nationality;
        user.PassportExpiryDate = model.PassportExpiryDate;
        user.PassportIssueDate = model.PassportIssueDate;
        user.PassportIssuingCountry = model.PassportIssuingCountry;
        user.PassportNumber = model.PassportNumber;
        user.VisaExpiryDate = model.VisaExpiryDate;
        user.VisaIssueDate = model.VisaIssueDate;
        user.PhoneNumber = model.PhoneNumber;

        await UserManager.UpdateAsync(user);

        await SignInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
}