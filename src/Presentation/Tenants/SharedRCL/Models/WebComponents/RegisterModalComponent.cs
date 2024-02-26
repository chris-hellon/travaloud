using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class RegisterModalComponent
{
    [Required]
    [Display(Name = "First Name")]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Surname")]
    [MaxLength(100)]
    public string Surname { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    [MaxLength(256)]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    [MaxLength(50)]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Nationality")]
    [MaxLength(100)]
    public string Nationality { get; set; }

    [Required]
    [Display(Name = "Gender")]
    [MaxLength(50)]
    public string Gender { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [BindProperty]
    [DataType(DataType.EmailAddress)]
    public string HoneyPot { get; set; } = string.Empty;

    public SelectList Nationalities { get; set; }

    public string? ReturnUrl { get; set; }

    public RegisterModalComponent()
    {
        Nationalities = new SelectList(Helpers.GetNationalities(), "Key", "Value", "");
    }
}