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
    
    [JsonProperty("roomRateDetailed")] public IEnumerable<RoomRateDetailedDto> DetailedRoomRates { get; set; }

    [JsonProperty("derivedType")] public string DerivedType { get; set; } = default!;

    [JsonProperty("derivedValue")] public decimal DerivedValue { get; set; }

    [JsonIgnore] public bool IsSharedRoom => ChildrenExtraCharge == null;

    [JsonIgnore] public string Availability => RoomsAvailable < 5 ? $"Only {RoomsAvailable} left" : $"{RoomsAvailable} {(IsSharedRoom ? "bed" : "room")}{(RoomsAvailable > 1 ? "s" : "")} available";

    [JsonIgnore] public int MaximumOccupancy => this.GetMaximumOccupancy();
    
    [JsonIgnore] public string CurrencySymbol { get; set; }
    
    [JsonIgnore] public DefaultIdType PropertyId { get; set; }
    
    [JsonIgnore] public string PropertyName { get; set; }
    
    [JsonIgnore] public int? RoomQuantity { get; set; }
    
    [JsonIgnore] public int? AdultQuantity { get; set; }
    
    [JsonIgnore] public int? ChildrenQuantity { get; set; }
}