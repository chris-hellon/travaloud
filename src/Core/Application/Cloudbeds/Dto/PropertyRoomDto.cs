namespace Travaloud.Application.Cloudbeds.Dto;

public class PropertyRoomDto
{
    [JsonProperty("roomTypeID")] public int RoomTypeId { get; set; }

    [JsonProperty("roomTypeName")] public string RoomTypeName { get; set; } = default!;

    [JsonProperty("roomTypeNameShort")] public string RoomTypeNameShort { get; set; } = default!;

    [JsonProperty("roomTypeDescription")] public string RoomTypeDescription { get; set; } = default!;

    [JsonProperty("maxGuests")] public string MaxGuests { get; set; } = default!;

    [JsonProperty("adultsIncluded")] public string AdultsIncluded { get; set; } = default!;

    [JsonProperty("childrenIncluded")] public int ChildrenIncluded { get; set; }

    [JsonProperty("roomTypePhotos")] public List<RoomTypePhotoDto> RoomTypePhotos { get; set; } = default!;

    [JsonProperty("roomTypeFeatures")] public List<string> RoomTypeFeatures { get; set; } = default!;

    [JsonProperty("roomRateID")] public int RoomRateId { get; set; }

    [JsonProperty("roomRate")] public decimal RoomRate { get; set; }

    [JsonProperty("roomsAvailable")] public int RoomsAvailable { get; set; }

    [JsonProperty("adultsExtraCharge")] public Dictionary<int, decimal>? AdultsExtraCharge { get; set; }

    [JsonProperty("childrenExtraCharge")] public Dictionary<int, decimal>? ChildrenExtraCharge { get; set; }

    [JsonProperty("derivedType")] public string DerivedType { get; set; } = default!;

    [JsonProperty("derivedValue")] public decimal DerivedValue { get; set; }
}