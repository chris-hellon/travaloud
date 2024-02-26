using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class ContactComponent
{
	[BindProperty]
	[Required(ErrorMessage = "Please enter a Name.")]
	public string Name { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Please enter an Email Address.")]
	[DataType(DataType.EmailAddress)]
	public string Email { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Please enter a Contact Number")]
	[DataType(DataType.PhoneNumber)]
	[Display(Name = "Contact Number")]
	public string ContactNumber { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Please enter a Message.")]
	public string Message { get; set; } = string.Empty;

	[BindProperty]
	[DataType(DataType.EmailAddress)]
	public string HoneyPot { get; set; } = string.Empty;

	public ContactComponent()
	{

	}
}