namespace Travaloud.Infrastructure.Mailing;

public class MailSettings
{
    public string? From { get; set; }

    public string? Host { get; set; }

    public int Port { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? DisplayName { get; set; }
    
    public string? ToAddress { get; set; }
    
    public string? PrimaryBackgroundColor { get; set; }
    
    public string? SecondaryBackgroundColor { get; set; }
    
    public string? HeaderBackgroundColor { get; set; }
    
    public string? TextColor { get; set; }
    
    public string? LogoImageUrl { get; set; }
    
    public string[]? BccAddress { get; set; }
}