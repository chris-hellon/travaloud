using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;


public class CheckoutGuestComponent
{
    public Guid? Id { get; set; }
    public Guid ItemId { get; set; }
        
    [Required]
    [Display(Name = "First Name")]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [Display(Name = "Surname")]
    [MaxLength(100)]
    public string? Surname { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    [MaxLength(256)]
    public string? Email { get; set; }

    [Required]
    [Display(Name = "Date Of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    [MaxLength(50)]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Nationality")]
    [MaxLength(100)]
    public string? Nationality { get; set; }

    [Required]
    [Display(Name = "Gender")]
    [MaxLength(50)]
    public string? Gender { get; set; }

    public CheckoutGuestComponent()
    {
            
    }

    public CheckoutGuestComponent(Guid? id, Guid itemId, string? firstName, string? surname, string? email, DateTime? dateOfBirth, string? phoneNumber, string? nationality, string? gender)
    {
        Id = id;
        ItemId = itemId;
        FirstName = firstName;
        Surname = surname;
        Email = email;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Nationality = nationality;
        Gender = gender;
    }
}