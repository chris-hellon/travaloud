using Microsoft.Extensions.Options;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Checkout;
using Travaloud.Application.Checkout.Commands;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Cloudbeds.Dto;
using Travaloud.Application.Cloudbeds.Queries;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Shared.Authorization;

namespace Travaloud.Tenants.SharedRCL.Pages.Checkout;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ICheckoutService _checkoutService;
    private readonly ICloudbedsService _cloudbedsService;
    private readonly StripeSettings _stripeSettings;
    public string StripePublicKey { get; set; }

    public IndexModel(ICheckoutService checkoutService, ICloudbedsService cloudbedsService, IOptions<StripeSettings> stripeOptions)
    {
        _checkoutService = checkoutService;
        _cloudbedsService = cloudbedsService;
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
        if (basket.Items.Any(x => x.PropertyId.HasValue))
        {
            var properties = await TenantWebsiteService.GetProperties(new CancellationToken());
            if (properties != null)
            {
                var propertyBasketItems = basket.Items.Where(x => x.PropertyId.HasValue);
                foreach (var basketItem in propertyBasketItems)
                {
                    if (basketItem.Rooms != null)
                    {
                        var property = properties.FirstOrDefault(x =>
                            basketItem.PropertyId != null && x.Id == basketItem.PropertyId.Value);

                        var cloudbedsAvailabilityResponse = await _cloudbedsService.GetPropertyAvailability(
                            new GetPropertyAvailabilityRequest()
                            {
                                StartDate = basketItem.CheckInDateParsed.Value.ToString("yyyy-MM-dd"),
                                EndDate = basketItem.CheckOutDateParsed.Value.ToString("yyyy-MM-dd"),
                                PropertyId = property.CloudbedsPropertyId,
                                PropertyApiKey = property.CloudbedsApiKey
                            });

                        if (cloudbedsAvailabilityResponse is {Success: true, Data: not null} && cloudbedsAvailabilityResponse.Data.Any())
                        {
                            var propertyData = cloudbedsAvailabilityResponse.Data.First();

                            var propertyRoomTypeIds = basketItem.Rooms.Select(x => x.RoomTypeId);
                            var existingRooms = propertyData.PropertyRooms.Where(x => propertyRoomTypeIds.Contains(x.RoomTypeId));
                            var propertyRoomDtos = existingRooms as PropertyRoomDto[] ?? existingRooms.ToArray();

                            if (propertyRoomDtos.Length != 0)
                            {
                                foreach (var basketRoomItem in basketItem.Rooms)
                                {
                                    var existingRoom = propertyRoomDtos.FirstOrDefault(x => x.RoomTypeId == basketRoomItem.RoomTypeId);
                                    
                                    if (existingRoom == null)
                                    {
                                        errorMessages.Add($"{basketItem.PropertyName} - {basketRoomItem.RoomTypeName} currently has no availability, please select an alternative room.");
                                        basket = await BasketService.RemoveRoom(basketRoomItem.Id, basketItem.Id);
                                        continue;
                                    }
                                    
                                    if (existingRoom.RoomsAvailable >= basketRoomItem.RoomQuantity) continue;
                                    
                                    errorMessages.Add($"{basketItem.PropertyName} - {basketRoomItem.RoomTypeName} currently only has {existingRoom.RoomsAvailable} room(s) available, amend your selection.");
                                    basket = await BasketService.RemoveRoom(basketRoomItem.Id, basketItem.Id);
                                }
                            }
                            else
                            {
                                errorMessages.Add($"{basketItem.PropertyName} currently has no availability, please select alternative dates.");
                                basket = await BasketService.RemoveItem(basketItem.Id);
                            }
                        }
                        else
                        {
                            errorMessages.Add($"{basketItem.PropertyName} currently has no availability, please select alternative dates.");
                            basket = await BasketService.RemoveItem(basketItem.Id);
                        }
                    }
                    else errorMessages.Add($"{basketItem.PropertyName} currently has no rooms selected.");
                }
            }
        }

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
                    guestId = Guid.Parse(existingUser.Id);
                    await SignInManager.SignInAsync(existingUser, isPersistent: false);
                }
            }

            var paymentLinkUrl = await _checkoutService.CreatePaymentLink(new CreatePaymentLinkRequest()
            {
                Basket = basket,
                GuestId = guestId.Value,
            });

            return Redirect(paymentLinkUrl);
        }

        foreach (var errorMessage in errorMessages)
        {
            ModelState.AddModelError(Guid.NewGuid().ToString(), errorMessage);
        }

        return Page();
    }
}