using System.Text.RegularExpressions;
using Travaloud.Application.Cloudbeds.Dto;


namespace Travaloud.Application.Common.Extensions;

public static class CloudbedsExtensions
{
    public static int GetMaximumOccupancy(this PropertyRoomDto propertyRoom)
    {
        if (!propertyRoom.IsSharedRoom)
            return int.Parse(propertyRoom.MaxGuests);

        const string pattern = @"\d+";
        var match = Regex.Match(propertyRoom.RoomTypeName, pattern);

        return match.Success ?
            int.Parse(match.Value) :
            0;
    }
}