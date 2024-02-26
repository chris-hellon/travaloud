using Microsoft.AspNetCore.Mvc;

namespace FuseHostelsAndTravel.Pages.OurStory;

public class IndexModel : TravaloudBasePageModel
{
    public override string MetaKeywords()
    {
        return "our story, hostel brand, backpacking, travel community";
    }

    public override string MetaDescription()
    {
        return "Learn about the story behind Fuse Hostels and Travels. We're more than just a hostel brand, we're a community of like-minded travelers who share a passion for backpacking and exploring the world.";
    }

    [BindProperty]
    public HeaderBannerComponent? HeaderBanner { get; private set; }

    [BindProperty]
    public ContainerHalfImageRoundedTextComponent? IntroductionBanner { get; private set; }

    [BindProperty]
    public ContainerHalfImageRoundedTextComponent? AboutBanner { get; private set; }

    [BindProperty]
    public List<NavPill>? NavPills { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["Title"] = "Our Story";
        
        await base.OnGetDataAsync();

        NavPills =
        [
            new NavPill("WHO WE ARE", 1400),
            new NavPill("WHAT WE DO", 1600),
            new NavPill("OUR CORE VALUES", 1800)
        ];

        HeaderBanner = new HeaderBannerComponent("FUSE HOSTELS & TRAVEL?", "WHO IS", null, "https://travaloudcdn.azureedge.net/fuse/assets/images/our-story.jpg?w=2000", new List<OvalContainerComponent>()
        {
            new("aboutPageHeaderBannerOvals1", 15, null, -30, null)
        });

        IntroductionBanner = new ContainerHalfImageRoundedTextComponent(new List<string>() { "WHO WE ARE" }, null, "<p>What's up world explorers! FUSE is not just another hostel, we're your home away from home in Vietnam. Our aim is to offer the most epic locations with some kick ass social spaces and cool activities, right on your doorstep.</p><p>We might be super fresh to the hostel scene as a company, but this isn’t our first rodeo. Our team is made up of local legends and well-travelled wanderers who are experts at creating awesome experiences that are all created with the aim to chill, meet some new mates and dip your toes into the local adventure that awaits!</p>",
                "https://travaloudcdn.azureedge.net/fuse/assets/images/fuse-purple-logo.webp?w=800", null, new List<OvalContainerComponent>()
                {
                    new("aboutPageIntroductionOvals1", 15, null, null, -28),
                    new("aboutPageIntroductionOvals2", null, 15, null, 18)
                })
            { AnimationStart = "onLoad"};

        AboutBanner = new ContainerHalfImageRoundedTextComponent(new List<string>() { "WHAT WE DO" }, null, "<p>At FUSE Hostels and Travel, we offer so much more than just a bed to lay your head. Our hostels are stunning spaces to meet people from all over the world, to experience the vibrant local culture of Vietnam, plus we can guarantee an outrageously good time! Our goal is to make your stay nothing short of epic, from start to finish.</p>",
            "https://travaloudcdn.azureedge.net/fuse/assets/images/what-we-do.jpg?w=800", null, new List<OvalContainerComponent>()
            {
                new("aboutPageAboutOvals1", -40, null, -20, null),
                new("aboutPageAboutOvals2", null, -40, null, -20)
            }, true);

        return Page();
    }
}