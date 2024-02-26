namespace Travaloud.Application.Cloudbeds.Dto;

public class RoomTypePhotoDto
{
    [JsonProperty("thumb")] public string Thumb { get; set; } = default!;

    [JsonProperty("image")] public string Image { get; set; } = default!;
}