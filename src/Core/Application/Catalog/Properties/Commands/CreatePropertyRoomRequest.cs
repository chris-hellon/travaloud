namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyRoomRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public FileUploadRequest? Image { get; set; }
}