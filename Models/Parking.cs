
namespace PublicParkingsSofiaWebAPI.Models
{
    public class Parking
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<ParkingSpot>? ParkingSpots { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool HasFreeSpot { get; set; } = true;
        public double? Rating { get; set; } = 3;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public double CostPerHour { get; set; } = 6;
        public double CostPer4h { get; set; } = 5;
        public double CostPer8h { get; set; } = 4;
        public double CostPerDay { get; set; } = 3;
        public double CostPerWeek { get; set; } = 2;
    }
    public class ParkingInfo
    {
        public int FreeSpots { get; set; }
        public int FreeSpotsWithoutFutureReservationsYet { get; set; }
        public int AvaliableSpotForNext2Hours { get; set; }
        public int AvaliableSpotForNext4Hours { get; set; }
        public int AvaliableSpotForNext8Hours { get; set; }
        public int AvaliableSpotForNext12Hours { get; set; }
        public int AvaliableSpotForNext24Hours { get; set; }
        public int AvaliableSpotForNextWeek { get; set; }
        public DateTime? AvaliableMaxEndTime { get; set; }
        public int? AvaliableSpotsMaxEndTimeCount { get; set; }
        public DateTime? AvaliableSecondMaxEndTime { get; set; }
        public int? AvaliableSpotsSecondMaxEndTimeCount { get; set; }
        public DateTime? SpotWillBeFreeForReservationSoonestTime { get; set; }
        public double? SoonestFreeSpotAvaliableForHours { get; set; }
        //public int AvaliableSpotForNextWeek { get; set; }

    }
    public class CreateParking
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int spotsCount { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double CostPerHour { get; set; } = 6;
        public double CostPer4h { get; set; } = 5;
        public double CostPer8h { get; set; } = 4;
        public double CostPerDay { get; set; } = 3;
        public double CostPerWeek { get; set; } = 2;
    }
}
