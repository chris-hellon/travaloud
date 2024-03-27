namespace Travaloud.Application.Basket.Dto;

public class BasketItemGuestModel
{
    public DefaultIdType ItemId { get; set; }
    public DefaultIdType? Id { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }

    public BasketItemGuestModel Update(string? firstName, string? surname, string? email, DateTime? dateOfBirth, string? phoneNumber, string? nationality, string? gender)
    {
        FirstName = firstName;
        Surname = surname;
        Email = email;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Nationality = nationality;
        Gender = gender;
        
        return this;
    }
}