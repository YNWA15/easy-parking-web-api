
namespace PublicParkingsSofiaWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public double? ParkCredits { get; set; } = null;
        public ICollection<Vehicle>? Vehicles { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
