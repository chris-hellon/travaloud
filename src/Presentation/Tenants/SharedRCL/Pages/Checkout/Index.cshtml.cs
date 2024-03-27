using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Checkout;
using Travaloud.Application.Checkout.Commands;
using Travaloud.Shared.Authorization;

namespace Travaloud.Tenants.SharedRCL.Pages.Checkout;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ICheckoutService _checkoutService;

    public IndexModel(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    public BasketModel? Basket { get; set; }
    
    [BindProperty]
    public CheckoutFormComponent CheckoutFormComponentModel { get; set; } = new();
    
    public ApplicationUser? AuthenticatedUser { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        await OnGetDataAsync();

        Basket = await BasketService.GetBasket();

        if (!CurrentUser.IsAuthenticated() || !UserId.HasValue) return Page();
        
        AuthenticatedUser = await UserManager.FindByIdAsync(UserId.ToString()!);

        if (AuthenticatedUser != null)
        {
            CheckoutFormComponentModel.Email = AuthenticatedUser.Email;
            CheckoutFormComponentModel.FirstName = AuthenticatedUser.FirstName;
            CheckoutFormComponentModel.Surname = AuthenticatedUser.LastName;
            CheckoutFormComponentModel.Nationality = AuthenticatedUser.Nationality;
            CheckoutFormComponentModel.DateOfBirth = AuthenticatedUser.DateOfBirth;
            CheckoutFormComponentModel.Gender = AuthenticatedUser.Gender;
            CheckoutFormComponentModel.PhoneNumber = AuthenticatedUser.PhoneNumber;
        }
        else
        {
            CheckoutFormComponentModel.Email = Basket.Email;
            CheckoutFormComponentModel.FirstName = Basket.FirstName;
            CheckoutFormComponentModel.Surname = Basket.Surname;
            CheckoutFormComponentModel.Nationality = Basket.Nationality;
            CheckoutFormComponentModel.DateOfBirth = Basket.DateOfBirth;
            CheckoutFormComponentModel.Gender = Basket.Gender;
            CheckoutFormComponentModel.PhoneNumber = Basket.PhoneNumber;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostProceedToPayment()
    {
        var basket = await BasketService.SetPrimaryContactInformation(
            CheckoutFormComponentModel.FirstName,
            CheckoutFormComponentModel.Surname,
            CheckoutFormComponentModel.Email,
            CheckoutFormComponentModel.DateOfBirth,
            CheckoutFormComponentModel.PhoneNumber,
            CheckoutFormComponentModel.Nationality,
            CheckoutFormComponentModel.Gender,
            CheckoutFormComponentModel.Password,
            CheckoutFormComponentModel.ConfirmPassword
        );
        
         var guestId = UserId;

        if (!guestId.HasValue)
        {
            var existingUser = await UserManager.FindByEmailAsync(basket.Email);

            if (existingUser == null)
            {
                var user = new ApplicationUser()
                {
                    FirstName = basket.FirstName,
                    LastName = basket.Surname,
                    PhoneNumber = basket.PhoneNumber,
                    Gender = basket.Gender,
                    Nationality = basket.Nationality,
                    DateOfBirth = basket.DateOfBirth,
                    UserName = basket.Email,
                    Email = basket.Email,
                    SignUpDate = DateTime.Now,
                    IsActive = true,
                    EmailConfirmed = true,
                    RefreshTokenExpiryTime = DateTime.Now
                };

                var result = await UserManager.CreateAsync(user, basket.Password ?? "P@55w0rd");

                if (result.Succeeded)
                {
                    guestId = Guid.Parse(user.Id);
                    var userResult = await UserManager.AddToRoleAsync(user, TravaloudRoles.Guest);

                    if (userResult.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(basket.Password))
                            await SignInManager.SignInAsync(user, isPersistent: false);
                    }
                }
            }
            else guestId = Guid.Parse(existingUser.Id);
        }

        var paymentLinkUrl = await _checkoutService.CreatePaymentLink(new CreatePaymentLinkRequest()
        {
            Basket = basket,
            GuestId = guestId.Value,
            
        });
        
        return Redirect(paymentLinkUrl);
    }
}