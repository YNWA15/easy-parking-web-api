
namespace PublicParkingsSofiaWebAPI.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public int? NumberInParking { get; set; }
        public int ParkingId { get; set; }
        public Parking? Parking { get; set; }
        public bool IsFree { get; set; } = true;
        public bool IsReadyToGetNextReservation { get; set; } = false;
        public bool ClientLateForReleaseTheSpot { get; set; } = false;
        public DateTime? IsPaidTill { get; set; }
        public bool? IsReserved { get; set; } = false;
        public DateTime? ReservedFrom { get; set; } = null;
        public DateTime? ReservedTo { get; set; } = null;
        public virtual Reservation? CurrentReservation { get; set; } = null; //notUsed
        public virtual int? CurrentReservationId { get; set; } = null; ///notused
        public ICollection<Reservation>? Reservations { get; set; }

    }
}
