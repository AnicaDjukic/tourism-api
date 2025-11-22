using Microsoft.Extensions.Options;

namespace tourism_api.Domain;

public class TourReservation
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int TouristId { get; set; }
    public int Guests { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsValid()
    {
        return Guests > 0;
    }
}