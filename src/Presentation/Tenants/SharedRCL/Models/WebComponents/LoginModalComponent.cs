using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class LoginModalComponent
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public string? BookingUrl { get; set; }

    public LoginModalComponent()
    {

    }
}

