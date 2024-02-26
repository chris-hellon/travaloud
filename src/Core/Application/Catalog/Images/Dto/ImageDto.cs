namespace Travaloud.Application.Catalog.Images.Dto;

public class ImageDto
{
    public DefaultIdType Id { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string? Title { get; set; } 
    public string? SubTitle1 { get; set; }
    public string? SubTitle2 { get; set; }
    public string? Href { get; set; }
    public string? Html { get; set; }
    public string? VideoPath { get; set; }
    public string? ThumbnailVideoPath { get; set; }
}