using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class CheckoutFormComponent : RegisterModalComponent
{
    [Required]
    [Display(Name = "Estimated Arrival Time")]
    public TimeSpan? EstimatedArrivalTime { get; set; }
        
    public int PropertiesGuestCount { get; set; }
    public int ToursGuestCount { get; set; }
}