using System.Xml.Linq;

namespace tourism_api.Domain
{
    public class RestaurantReservation
    {
        public int Id { get; set; }
        public string ReservationDate { get; set; }
        public string MealType { get; set; }
        public int GuestsNumber { get; set; }
        public int TouristId { get; set; }
        public int RestaurantId { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(MealType) || GuestsNumber <= 0)
            {
                return false;
            } 
            if(!DateTime.TryParseExact(ReservationDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate)){
                return false;
            }
            return parsedDate >= DateTime.Now.Date;
        }
    }
}
