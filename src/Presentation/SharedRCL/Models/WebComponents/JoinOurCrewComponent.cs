using System.ComponentModel.DataAnnotations;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class JoinOurCrewComponent
{
    [Required]
    [Display(Name = "First Name")]
    [BindProperty]
    public string FirstName { get; set; } = default!;
        
    [Required]
    [Display(Name = "Last Name")]
    [BindProperty]
    public string LastName { get; set; } = default!;
        
    [Required]
    [BindProperty]
    public string Email { get; set; } = default!;
        
    [Required]
    [Display(Name = "How can we collaborate?")]
    [BindProperty]
    public string HowCanWeCollaborate { get; set; } = default!;
        
    [Required]
    [Display(Name = "What are your estimated dates here?")]
    [BindProperty]
    public string EstimatedDates { get; set; } = default!;
        
    [Required]
    [Display(Name = "Which destinations have you previously visited?")]
    [BindProperty]
    public string DestinationsVisited { get; set; } = default!;
        
    [Display(Name = "Instagram handle")]
    [BindProperty]
    public string? Instagram { get; set; }
        
    [Display(Name = "TikTok handle")]
    [BindProperty]
    public string? TikTok { get; set; }
        
    [Display(Name = "YouTube handle")]
    [BindProperty]
    public string? YouTube { get; set; }
        
    [Display(Name = "Portfolio Url")]
    [BindProperty]
    public string? Portfolio { get; set; }
        
    [Display(Name = "What equipment do you have?")]
    [BindProperty]
    public string? Equipment { get; set; }
        
    [Required]
    [BindProperty]
    public Guid JobVacancyId { get; set; } = default!;
    
    [Required]
    [BindProperty]
    public string? JobVacancy { get; set; } = default!;
    
    [Required]
    [BindProperty]
    public string? Location { get; set; } = default!;
    
    [BindProperty]
    [DataType(DataType.EmailAddress)]
    public string HoneyPot { get; set; } = string.Empty;
}