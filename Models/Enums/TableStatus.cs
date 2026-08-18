using System.Text.Json.Serialization;

namespace RestaurantTableReservationAPI.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TableStatus
{
    Available = 0,
    Reserved = 1,
    Occupied = 2
}
