using System.ComponentModel.DataAnnotations.Schema;

namespace PublicParkingsSofiaWebAPI.Models
{
    public class Reservation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime StartReservationPeriod { get; set; }
        public DateTime EndReservationPeriod { get; set; }
        public DateTime CreatedResrvationTime { get; set; }
        public double Price { get; set; }
        public double? UpdatedPeriodPrice { get; set; } = null; //notused
        public bool IsPaid { get; set; } = false;
        public bool IsStarted { get; set; } = false;
        public bool IsEnded { get; set; } = false; //notused
        public bool CanStartEarly { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public bool IsFailed { get; set; } = false;
        public bool isUserCreditsUpdated { get; set; } = false; //notUsed
        public bool Is15MinOver { get; set; } = false;
        public double MinOver { get; set; } = 0;
        public int ParkingId { get; set; }
        public Parking? Parking { get; set; }
        public int SpotId { get; set; }
        public ParkingSpot? Spot { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string CarRegNumber { get; set; }
        public bool is1hrAdded { get; set; } = false; ///notused
    }
}
