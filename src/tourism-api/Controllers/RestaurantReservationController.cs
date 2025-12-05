using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tourism_api.Domain;
using tourism_api.Repositories;

namespace tourism_api.Controllers
{
    [Route("api/restaurants/{restaurantId}/reservations")]
    [ApiController]
    public class RestaurantReservationController : ControllerBase
    {
        private readonly RestaurantRepository _restaurantRepo;
        private readonly RestaurantReservationRepository _reservationRepo;
        private readonly UserRepository _userRepo;
        public RestaurantReservationController(IConfiguration configuration)
        {
            _restaurantRepo = new RestaurantRepository(configuration);
            _reservationRepo = new RestaurantReservationRepository(configuration);
            _userRepo = new UserRepository(configuration);
        }

        [HttpPost]
        public ActionResult<RestaurantReservation> Create(int restaurantId, [FromBody] RestaurantReservation newReservation)
        {
            if (!newReservation.IsValid())
            {
                return BadRequest("Invalid reservation data.");
            }

            try
            {
                Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
                if (restaurant == null)
                {
                    return NotFound($"Restaurant with ID {restaurantId} not found.");
                }

                User user = _userRepo.GetById(newReservation.TouristId);
                if (user == null)
                {
                    return NotFound($"User with ID {newReservation.TouristId} not found.");
                }
                if (user.Role == "vlasnik" || user.Role == "vodic")
                {
                    return BadRequest("Only tourists can make reservations. Please sign in as a tourist.");
                }
                newReservation.RestaurantId = restaurantId;
                RestaurantReservation createdReservation = _reservationRepo.Create(newReservation);
                return Ok(createdReservation);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<RestaurantReservation>> GetById(int restaurantId)
        {
            try
            {
                Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
                if (restaurant == null)
                {
                    return NotFound($"Restaurant with ID {restaurantId} not found.");
                }
                List<RestaurantReservation> reservations = _reservationRepo.GetReservationsByRestaurantId(restaurantId);

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantReservation> GetReservationById(int id)
        {
            try
            {
                RestaurantReservation reservation = _reservationRepo.GetReservationById(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int restaurantId, int id)
        {
            try
            {
                Restaurant restaurant = _restaurantRepo.GetById(restaurantId);
                if (restaurant == null)
                {
                    return NotFound($"Restaurant with ID {restaurantId} not found.");
                }

                RestaurantReservation reservation = _reservationRepo.GetReservationById(id);
                if (reservation == null)
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }

                DateTime cancelDate = DateTime.Now;
                DateTime reservationDate = DateTime.Parse(reservation.ReservationDate);
                if (reservation.MealType == "Breakfast 08:00h" && (reservationDate.AddHours(8) - cancelDate).TotalHours < 12)
                {
                    return BadRequest("Breakfast can be canceled minimum 12 hours before reservation time.");
                }
                if (reservation.MealType == "Lunch 13:00h" && (reservationDate.AddHours(13) - cancelDate).TotalHours < 4)
                {
                    return BadRequest("Lunch can be canceled minimum 4 hours before reservation time.");
                }
                if (reservation.MealType == "Dinner 18:00h" && (reservationDate.AddHours(18) - cancelDate).TotalHours < 4)
                {
                    return BadRequest("Dinner can be canceled minimum 4 hours before reservation time.");
                }

                bool isDeleted = _reservationRepo.Delete(id);
                if (isDeleted)
                {
                    return NoContent();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while deleting the reservation.");
            }
        }
    }
}
