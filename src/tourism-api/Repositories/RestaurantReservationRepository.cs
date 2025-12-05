using Microsoft.Data.Sqlite;
using tourism_api.Domain;

namespace tourism_api.Repositories
{
    public class RestaurantReservationRepository
    {
        private readonly string _connectionString;
        public RestaurantReservationRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString:SQLiteConnection"];
        }
        public RestaurantReservation Create(RestaurantReservation reservation)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    INSERT INTO Reservations (ReservationDate, MealType, GuestsNumber, TouristId, RestaurantId) 
                    VALUES (@ReservationDate, @MealType, @GuestsNumber, @TouristId, @RestaurantId);
                    SELECT LAST_INSERT_ROWID();";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@ReservationDate", reservation.ReservationDate);
                command.Parameters.AddWithValue("@MealType", reservation.MealType);
                command.Parameters.AddWithValue("@GuestsNumber", reservation.GuestsNumber);
                command.Parameters.AddWithValue("@TouristId", reservation.TouristId);
                command.Parameters.AddWithValue("@RestaurantId", reservation.RestaurantId);

                reservation.Id = Convert.ToInt32(command.ExecuteScalar());

                return reservation;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public List<RestaurantReservation> GetReservationsByRestaurantId(int restaurantId)
        {
            try
            {
                List<RestaurantReservation> reservations = new List<RestaurantReservation>();
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT * FROM Reservations WHERE RestaurantId=@restaurantId";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@restaurantId", restaurantId);
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    RestaurantReservation reservation = new RestaurantReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        ReservationDate = reader["ReservationDate"].ToString(),
                        MealType = reader["MealType"].ToString(),
                        GuestsNumber = Convert.ToInt32(reader["GuestsNumber"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        RestaurantId = Convert.ToInt32(reader["RestaurantId"])
                    };
                    reservations.Add(reservation);
                }
                return reservations;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public RestaurantReservation GetReservationById(int Id)
        {
            try
            {
                RestaurantReservation reservation = null;
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = @"
                    SELECT * FROM Reservations WHERE Id=@Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", Id);
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    reservation = new RestaurantReservation
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        ReservationDate = reader["ReservationDate"].ToString(),
                        MealType = reader["MealType"].ToString(),
                        GuestsNumber = Convert.ToInt32(reader["GuestsNumber"]),
                        TouristId = Convert.ToInt32(reader["TouristId"]),
                        RestaurantId = Convert.ToInt32(reader["RestaurantId"])
                    };
                }
                return reservation;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u konverziji podataka iz baze: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(_connectionString);
                connection.Open();

                string query = "DELETE FROM Reservations WHERE Id = @Id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri konekciji ili izvršavanju neispravnih SQL upita: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Konekcija nije otvorena ili je otvorena više puta: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
    }
}