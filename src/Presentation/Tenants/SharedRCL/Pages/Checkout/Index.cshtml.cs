using Mapster;
using Microsoft.Extensions.Options;
using Travaloud.Application.Basket;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Shared.Authorization;

namespace Travaloud.Tenants.SharedRCL.Pages.Checkout;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ICloudbedsService _cloudbedsService;
    private readonly IPaymentConfirmationService _paymentConfirmationService;
    private readonly IBasketService _basketService;
    private readonly StripeSettings _stripeSettings;
    public string StripePublicKey { get; set; }

    public IndexModel(ICloudbedsService cloudbedsService,
        IOptions<StripeSettings> stripeOptions,
        IPaymentConfirmationService paymentConfirmationService, 
        IBasketService basketService)
    {
        _cloudbedsService = cloudbedsService;
        _paymentConfirmationService = paymentConfirmationService;
        _basketService = basketService;
        _stripeSettings = stripeOptions.Value;
        StripePublicKey = _stripeSettings.ApiPublishKey;
    }

    public BasketModel? Basket { get; set; }

    [BindProperty] public CheckoutFormComponent CheckoutFormComponentModel { get; set; } = new();

    public ApplicationUser? AuthenticatedUser { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? bookingId)
    {
        await OnGetDataAsync();

        if (bookingId.HasValue)
            await BookingService.DeleteAsync(bookingId.Value);
        
        Basket = await BasketService.GetBasket();

        var url = Request.GetEncodedUrl();

        LoginModal.ReturnUrl = url;
        RegisterModal.ReturnUrl = url;

        var estimatedArrivalTimeRequired = Basket.Items.Any(x => x.PropertyId.HasValue);
        CheckoutFormComponentModel.EstimatedArrivalTimeRequired = estimatedArrivalTimeRequired;
        
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
            CheckoutFormComponentModel.EstimatedArrivalTime,
            CheckoutFormComponentModel.Password,
            CheckoutFormComponentModel.ConfirmPassword
        );

        var errorMessages = new List<string>();

        // We need to check if availability has changed since the session was created 
        basket = await errorMessages.CheckCloudbedsReservations(basket, TenantWebsiteService, _cloudbedsService, BasketService);

        // No concurrency errors, continue to payment
        if (errorMessages.Count == 0)
        {
            var guestId = UserId;

            if (!guestId.HasValue)
            {
                var existingUser = await UserManager.FindByEmailAsync(basket.Email);

                if (existingUser == null)
                {
                    var user = new ApplicationUser
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
                else
                {
                    existingUser.Nationality = CheckoutFormComponentModel.Nationality;
                    existingUser.Gender = CheckoutFormComponentModel.Gender;
                    existingUser.PhoneNumber = CheckoutFormComponentModel.PhoneNumber;
                    existingUser.DateOfBirth = CheckoutFormComponentModel.DateOfBirth;
                    existingUser.FirstName = CheckoutFormComponentModel.FirstName;
                    existingUser.LastName = CheckoutFormComponentModel.Surname;

                    await UserManager.UpdateAsync(existingUser);
                    
                    guestId = Guid.Parse(existingUser.Id);
                    await SignInManager.SignInAsync(existingUser, isPersistent: false);
                }
            }
            else
            {
                var existingUser = await UserManager.FindByIdAsync(guestId.Value.ToString());

                if (existingUser != null)
                {
                    existingUser.Nationality = CheckoutFormComponentModel.Nationality;
                    existingUser.Gender = CheckoutFormComponentModel.Gender;
                    existingUser.PhoneNumber = CheckoutFormComponentModel.PhoneNumber;
                    existingUser.DateOfBirth = CheckoutFormComponentModel.DateOfBirth;
                    existingUser.FirstName = CheckoutFormComponentModel.FirstName;
                    existingUser.LastName = CheckoutFormComponentModel.Surname;

                    await UserManager.UpdateAsync(existingUser);
                }
            }

            if (guestId.HasValue)
            {
                HttpContextAccessor.HttpContext?.Session.SetString("GuestId", guestId.Value.ToString());

                Guid? bookingId = null;
                var createBookingRequest = await _paymentConfirmationService.CreateBookingRequest(guestId.Value, basket); 
                
                if (basket.BookingId.HasValue)
                {
                    bookingId = basket.BookingId;

                    var booking = await BookingService.GetAsync(bookingId.Value);
                    var updateBookingRequest = createBookingRequest.Adapt<UpdateBookingRequest>();
                    updateBookingRequest.Id = bookingId.Value;
                    updateBookingRequest.StaffMemberRequired = false;
                    updateBookingRequest.ConcurrencyVersion = booking.ConcurrencyVersion;

                    await BookingService.UpdateAsync(bookingId.Value, updateBookingRequest);
                }
                else
                {
                    bookingId = await BookingService.CreateAsync(createBookingRequest);
                }

                if (!bookingId.HasValue)
                {
                    ModelState.AddModelError("bookingId", "There was an error creating your booking. Please try again or Contact Us.");
                    return Page();
                }
                
                await _basketService.SetBookingId(bookingId.Value);
                
                HttpContextAccessor.HttpContext?.Session.SetString("BookingId", bookingId.Value.ToString());
                
                Logger.Information("Booking {BookingId} created for Guest {GuestId} ", bookingId, guestId);

                //TODO: Add back in when we have a cloudbeds fix
                // var booking = await BookingService.GetAsync(bookingId.Value);
                // var propertyBookingsProcessed = await _paymentConfirmationService.ProcessPropertyBookings(
                //     Basket,
                //     booking,
                //     cardToken,
                //     paymentAuthorizationCode,
                //     TenantWebsiteService,
                //     _cloudbedsService,
                //     BookingService
                // );
                //
                // if (!propertyBookingsProcessed)
                //     return await RefundAndFail(bookingId.Value, paymentAuthorizationCode,
                //         stripeStatus.PaymentIntentId, Basket.Total);
                
                return LocalRedirect($"/payment");
            }
        }

        foreach (var errorMessage in errorMessages)
        {
            ModelState.AddModelError(Guid.NewGuid().ToString(), errorMessage);
        }

        return Page();
    }
}