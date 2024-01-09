using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PublicParkingsSofiaWebAPI.Models;

namespace PublicParkingsSofiaWebAPI.Services
{
    public class ParkingServices
    {
        public ParkingsContext context;
        private readonly UserManager<IdentityUser> userManager;
        public ParkingServices(ParkingsContext _context, UserManager<IdentityUser> _userManager)
        {

            context = _context;
            userManager = _userManager;
        }
        #region PARKING
        public async Task<ICollection<Parking>> GetParkings()
        {
            IQueryable<Parking> parkings = context.Parkings.AsNoTracking().Include(q => q.ParkingSpots).ThenInclude(x => x.Reservations).Include(q => q.Reservations);

            return await parkings.ToListAsync();
        }
        public async Task<Parking> GetParking(int id)
        {
            IQueryable<Parking> parking = context.Parkings
                             .Where(q => q.Id == id);

            return await parking.FirstOrDefaultAsync();
        }
        public async Task<ParkingInfo> GetParkingInfo(int parkingId)
        {
            var _parking = context.Parkings
                             .Where(q => q.Id == parkingId)
                             .Include(x => x.ParkingSpots).ThenInclude(x => x.Reservations)
                             .Include(x => x.Reservations);
            var parking = await _parking.FirstOrDefaultAsync();
            var freeSpotCount = parking.ParkingSpots.Where(x => x.IsFree == true).ToList().Count();
            var freeSpotForNext2HoursCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(2))).ToList().Count();
            var freeSpotForNext4HoursCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(4))).ToList().Count();
            var freeSpotForNext8HoursCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(8))).ToList().Count();
            var freeSpotForNext12HoursCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(12))).ToList().Count();
            var freeSpotForNext24HoursCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(24))).ToList().Count();
            var freeSpotForNextWeekCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.ToList().OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1) >= DateTime.Now.AddHours(168))).ToList().Count();
            var freeSpotWithoutReservationsYetCount = parking.ParkingSpots.Where(x => (x.Reservations == null) || (x.Reservations.Count == 0) || (x.Reservations.Where(x => x.EndReservationPeriod > DateTime.Now.AddHours(1))).ToList().Count == 0).Count();

            DateTime? avaliableMaxEndTime = null;
            int? avaliableSpotsMaxEndTimeCount = null;
            DateTime? avaliableSecondMaxEndTime = null;
            int? avaliableSpotsSecondMaxEndTimeCount = null;
            DateTime? spotWillBeFreeForReservationSoonestTime = null;
            int? soonestFreeSpotsCount = null;
            double? soonestFreeSpotAvaliableForHours = null;

            var parkingInfo = new ParkingInfo();
            parkingInfo.FreeSpots = freeSpotCount;
            parkingInfo.AvaliableSpotForNext2Hours = freeSpotForNext2HoursCount;
            parkingInfo.AvaliableSpotForNext4Hours = freeSpotForNext4HoursCount;
            parkingInfo.AvaliableSpotForNext8Hours = freeSpotForNext8HoursCount;
            parkingInfo.AvaliableSpotForNext12Hours = freeSpotForNext12HoursCount;
            parkingInfo.AvaliableSpotForNext24Hours = freeSpotForNext24HoursCount;
            parkingInfo.AvaliableSpotForNextWeek = freeSpotForNextWeekCount;
            parkingInfo.FreeSpotsWithoutFutureReservationsYet = freeSpotWithoutReservationsYetCount;

            if (parkingInfo.FreeSpotsWithoutFutureReservationsYet == null || parkingInfo.FreeSpotsWithoutFutureReservationsYet == 0)
            {
                if (parkingInfo.FreeSpots != null && parkingInfo.FreeSpots != 0)
                {
                    var freespots = parking.ParkingSpots.Where(x => x.IsFree == true).ToList();
                    foreach (var spot in freespots)
                    {
                        spot.Reservations = spot.Reservations.Where(x => x.StartReservationPeriod > DateTime.Now).ToList().OrderBy(x => x.StartReservationPeriod).ToList();
                    }
                    freespots = freespots.OrderByDescending(x => x.Reservations.ToList()[0].StartReservationPeriod).ToList();
                    avaliableMaxEndTime = freespots[0].Reservations.ToList()[0].StartReservationPeriod;

                    var first = freespots[0].Reservations.ToList()[0].StartReservationPeriod;
                    var longestAv = freespots.Where(x => x.Reservations.ToList()[0].StartReservationPeriod == first).ToList();
                    avaliableSpotsMaxEndTimeCount = longestAv.Count;

                    avaliableSecondMaxEndTime = freespots[0].Reservations.ToList()[0].StartReservationPeriod;
                    var second = freespots[0].Reservations.ToList()[0].StartReservationPeriod;
                    var secondLongestAv = freespots.Where(x => x.Reservations.ToList()[0].StartReservationPeriod == second).ToList();
                    avaliableSpotsSecondMaxEndTimeCount = secondLongestAv.Count;
                }
            }
            else if (parkingInfo.FreeSpots == null || parkingInfo.FreeSpots == 0)
            {
                var busyspots = parking.ParkingSpots.Where(x => x.IsFree == false)?.ToList();
                var listOfTimesWhenSpotsBecomesFree = new List<helperObject>();

                var firstChance = busyspots?.Where(x => (x.Reservations.ToList()[1] != null) && x.Reservations.ToList()[1].StartReservationPeriod >= x.Reservations.ToList()[0].EndReservationPeriod.AddHours(2)).ToList();
                if (firstChance != null && firstChance.Count > 0)
                {
                    foreach (var spot in firstChance)
                    {
                        var b = new helperObject();
                        b.timeWhenSpotBecomeFree = spot.Reservations.ToList()[0].EndReservationPeriod.AddHours(1);
                        TimeSpan timeSpan = spot.Reservations.ToList()[1].StartReservationPeriod - spot.Reservations.ToList()[0].EndReservationPeriod.AddHours(1);
                        b.spotsFreeHours = timeSpan.TotalHours;
                    }
                }
                var secondChance = busyspots.Where(x => (x.Reservations.ToList()[2] != null) && x.Reservations.ToList()[2].StartReservationPeriod >= x.Reservations.ToList()[1].EndReservationPeriod.AddHours(2)).ToList();

                if (secondChance.Count > 0)
                {
                    foreach (var spot in secondChance)
                    {
                        var b = new helperObject();
                        b.timeWhenSpotBecomeFree = spot.Reservations.ToList()[1].EndReservationPeriod.AddHours(1);
                        TimeSpan timeSpan = spot.Reservations.ToList()[2].StartReservationPeriod - spot.Reservations.ToList()[1].EndReservationPeriod.AddHours(1);
                        b.spotsFreeHours = timeSpan.TotalHours;

                    }
                }
                var thirdChance = busyspots.Where(x => (x.Reservations.ToList()[3] != null) && x.Reservations.ToList()[3].StartReservationPeriod >= x.Reservations.ToList()[2].EndReservationPeriod.AddHours(2)).ToList();
                if (secondChance.Count > 0)
                {
                    foreach (var spot in secondChance)
                    {
                        var b = new helperObject();
                        b.timeWhenSpotBecomeFree = spot.Reservations.ToList()[2].EndReservationPeriod.AddHours(1);
                        TimeSpan timeSpan = spot.Reservations.ToList()[3].StartReservationPeriod - spot.Reservations.ToList()[2].EndReservationPeriod.AddHours(1);
                        b.spotsFreeHours = timeSpan.TotalHours;

                    }
                }
                if (listOfTimesWhenSpotsBecomesFree.Count > 0)
                {
                    var ordered = listOfTimesWhenSpotsBecomesFree.OrderBy(x => x.timeWhenSpotBecomeFree).ToList();
                    spotWillBeFreeForReservationSoonestTime = ordered[0].timeWhenSpotBecomeFree;
                    var soonest = ordered.Where(x => x.timeWhenSpotBecomeFree == spotWillBeFreeForReservationSoonestTime).ToList();
                    soonest = soonest.OrderByDescending(x => x.spotsFreeHours).ToList();
                    soonestFreeSpotAvaliableForHours = soonest[0].spotsFreeHours;
                    soonestFreeSpotsCount = soonest.Where(x => x.spotsFreeHours == soonestFreeSpotAvaliableForHours).ToList().Count;
                }
            }

            return parkingInfo;
        }
        public class helperObject
        {
            public DateTime timeWhenSpotBecomeFree { get; set; }
            public double? spotsFreeHours { get; set; }
        }




        #endregion
        #region USER
        public async Task<ICollection<User>> GetUsers()
        {
            IQueryable<User> users = context.Users;


            return await users.ToListAsync();
        }
        public async Task<User> GetUser(int id)
        {
            IQueryable<User> user = context.Users
                             .Where(q => q.Id == id);


            return await user.FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            IQueryable<User> user = context.Users
                             .Where(q => q.Email == email).Include(x => x.Reservations).Include(x => x.Vehicles); //.Include(x=>x.Vehicles)


            return await user.FirstOrDefaultAsync();
        }

        public async Task<User> Registration(User user)
        {
            var email = user.Email;
            var c = await context.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();
            if (c != null)
            {
                throw new ArgumentException("There is user registered with this email!");
            }
            user.ParkCredits = 0;
            context.Users.Add(user);
            await Save();
            return user;
        }
        public async Task UpdateUser(int id, User user)  //not used in app??
        {
            var _user = context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (_user == null)
            {
                throw new ArgumentException("There is not user with this Id!");
            }
            _user.Password = user.Password;
            _user.PhoneNumber = user.PhoneNumber;
            _user.Name = user.Name;
            _user.Vehicles = user.Vehicles;

            context.Users.Attach(_user);
            context.Entry(_user).State = EntityState.Modified;
            await Save();

        }
        public async Task AddCredits(int id, double amount)
        {
            var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("There is not user with this Id!");
            }
            if (user.ParkCredits == null)
            {
                user.ParkCredits = amount;
            }
            else
            {
                user.ParkCredits = user.ParkCredits + amount;
            }

            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;
            await Save();

        }
        public async Task RemoveCredits(int id, double amount)
        {
            var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("There is not user with this Id!");
            }
            user.ParkCredits = user.ParkCredits - amount;
            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;
            await Save();

        }
        public async Task RemoveAllCredits(int id)
        {
            var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("There is not user with this Id!");
            }
            user.ParkCredits = 0;
            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;
            await Save();

        }
        public async Task GiveUserCash(int id)
        {
            var user = context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("There is not user with this Id!");
            }
            user.ParkCredits = 0;
            context.Users.Attach(user);
            context.Entry(user).State = EntityState.Modified;
            await Save();

        }


        #endregion
        #region PARKINGSPOTS
        public async Task<ICollection<ParkingSpot>> GetSpots()
        {
            IQueryable<ParkingSpot> spots = context.ParkingSpots.Include(q => q.Parking).Include(x => x.Reservations);


            return await spots.ToListAsync();
        }
        public async Task<ParkingSpot> GetSpot(int id)
        {
            IQueryable<ParkingSpot> spot = context.ParkingSpots
                             .Where(q => q.Id == id).Include(x => x.Reservations);


            return await spot.FirstOrDefaultAsync();
        }

        public async Task<ICollection<ParkingSpot>> GetBusySpotsOnParking(int id)
        {
            IQueryable<ParkingSpot> spots = context.ParkingSpots
                             .Where(q => q.ParkingId == id && q.IsFree == false);//.Include(x => x.Reservations);


            return await spots.ToListAsync();
        }

        public async Task<Reservation> GetSpotCurrentReservation(int id)
        {
            ParkingSpot spot = await context.ParkingSpots
                             .Where(q => q.Id == id).Include(x => x.Reservations).ThenInclude(x => x.User).FirstOrDefaultAsync();//.Include(x => x.Reservations);

            if (spot.IsFree == true)
            {
                return null;
            }
            var currReservation = spot.Reservations.Where(x => x.IsStarted == true && x.IsEnded == false).FirstOrDefault();
            if (currReservation != null)
            {
                return currReservation;
            }
            return null;
        }


        public async Task UpdateSpot(int id, ParkingSpot spot)
        {
            var _spot = context.ParkingSpots.Where(x => x.Id == id).FirstOrDefault();
            if (_spot == null)
            {
                throw new ArgumentException("There is not spot with this Id!");
            }
            _spot.IsFree = spot.IsFree;
            _spot.ReservedTo = spot.ReservedTo;
            _spot.IsReserved = spot.IsReserved;

            context.ParkingSpots.Attach(_spot);
            context.Entry(_spot).State = EntityState.Modified;
            await Save();
        }

        public async Task<ParkingSpot> GetFreeSpotForClientToUseIt(int parkingId, DateTime parkTill)
        {
            var _parking = context.Parkings.Where(x => x.Id == parkingId).Include(x => x.ParkingSpots);
            var parking = await _parking.FirstOrDefaultAsync();
            if (parking == null)
            {
                throw new ArgumentException("There is not parking with this Id!");
            }
            var spots = parking.ParkingSpots;
            var freeSpots = spots.ToList().Where(x => x.IsFree == true);

            var spotsAvaliableForThisPeriod = freeSpots.Where(x => (DateTime.Compare((x.Reservations.OrderBy(x => x.StartReservationPeriod).ToList()[0].StartReservationPeriod.AddHours(-1)), parkTill) >= 0) || (x.Reservations == null || x.Reservations.Count == 0));
            var avaliableSpot = spotsAvaliableForThisPeriod.FirstOrDefault();
            return avaliableSpot;

        }

        public async Task<ParkingSpot> CheckIfPossibleAddingDefinedReservationToThisSpotsReservations(int parkingSpotId, DateTime reservationStartPeriodTime, DateTime reservationEndPeriodTime)
        {
            var spot = await context.ParkingSpots.Where(x => x.Id == parkingSpotId).Include(x => x.Reservations).FirstOrDefaultAsync();
            var spotFutureReservations = spot.Reservations.ToList().Where(x => x.StartReservationPeriod > DateTime.Now).OrderBy(x => x.StartReservationPeriod).ToList();
            if (spotFutureReservations != null && spotFutureReservations.Count > 0)
            {
                var possibly = true;
                foreach (var reservation in spotFutureReservations)
                {
                    if (reservation.StartReservationPeriod.AddHours(-1) <= reservationStartPeriodTime && reservation.EndReservationPeriod.AddHours(1) > reservationStartPeriodTime)
                    {
                        possibly = false;
                    }
                    if (reservation.StartReservationPeriod.AddHours(-1) >= reservationStartPeriodTime && reservation.StartReservationPeriod.AddHours(-1) < reservationEndPeriodTime)
                    {
                        possibly = false;
                    }

                    if (reservation.StartReservationPeriod.AddHours(-1) < reservationEndPeriodTime && reservation.EndReservationPeriod.AddHours(1) >= reservationEndPeriodTime)
                    {
                        possibly = false;
                    }
                }
                if (possibly)
                {
                    return spot;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return spot;
            }
        }
        public async Task<ParkingSpot> CheckIfPossibleEditReservationPeriodWithoutChangeSpot(int reservationId, int parkingSpotId, DateTime reservationStartPeriodTime, DateTime reservationEndPeriodTime)
        {
            var _reservation = await context.Reservations.Where(x => x.Id == reservationId).FirstOrDefaultAsync();
            var spot = await context.ParkingSpots.Where(x => x.Id == parkingSpotId).Include(x => x.Reservations).FirstOrDefaultAsync();
            var spotFutureReservations = spot.Reservations.ToList().Where(x => x.IsEnded == false && x.Id != reservationId).OrderBy(x => x.StartReservationPeriod).ToList();
            var currReservation = spotFutureReservations.Where(x => x.IsStarted = true).FirstOrDefault();
            var a = spotFutureReservations.Where(x => x.IsEnded == false).OrderBy(x => x.StartReservationPeriod).ToList();

            if (spotFutureReservations != null && spotFutureReservations.Count > 0)
            {
                var possibly = true;
                foreach (var reservation in spotFutureReservations)
                {
                    if (reservation.StartReservationPeriod.AddHours(-1) <= reservationStartPeriodTime && reservation.EndReservationPeriod.AddHours(1) > reservationStartPeriodTime)
                    {
                        possibly = false;
                        return null;
                    }
                    if (reservation.StartReservationPeriod.AddHours(-1) >= reservationStartPeriodTime && reservation.StartReservationPeriod.AddHours(-1) < reservationEndPeriodTime)
                    {
                        possibly = false;
                        return null;
                    }

                    if (reservation.StartReservationPeriod.AddHours(-1) < reservationEndPeriodTime && reservation.EndReservationPeriod.AddHours(1) >= reservationEndPeriodTime)
                    {
                        possibly = false;
                        return null;
                    }
                }
                if (possibly)
                {
                    return spot;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return spot;
            }
        }

        #endregion
        #region RESERVATION
        public async Task<ICollection<Reservation>> GetReservationsWithoutObjects()
        {
            IQueryable<Reservation> reservations = context.Reservations;


            return await reservations.ToListAsync();
        }
        public async Task<ICollection<Reservation>> GetReservations()
        {
            IQueryable<Reservation> reservations = context.Reservations.Include(q => q.Spot)
                .Include(x => x.Parking);


            return await reservations.ToListAsync();
        }

        public async Task<ICollection<Reservation>> GetUserNotEndedReservations(int userId)
        {
            IQueryable<Reservation> reservations = context.Reservations.Where(x => x.UserId == userId && x.IsEnded == false).Include(x => x.Parking).ThenInclude(x => x.Reservations).OrderBy(x => x.StartReservationPeriod);

            return await reservations.ToListAsync();
        }
        public async Task<Reservation> GetReservation(int id)
        {
            IQueryable<Reservation> reservations = context.Reservations
                             .Where(q => q.Id == id).Include(x => x.Parking);

            return await reservations.FirstOrDefaultAsync();
        }

        public async Task<ICollection<Reservation>> GetFutureReservationsForParking(int parkingId)
        {
            IQueryable<Reservation> reservations = context.Reservations!
                             .Where(q => q.ParkingId == parkingId && q.IsStarted == false && q.IsEnded == false).Include(x => x.User).OrderBy(x => x.StartReservationPeriod);

            return await reservations.ToListAsync();
        }

        public async Task<ICollection<Reservation>> GetReservationsByUserEmail(string email)
        {
            IQueryable<Reservation> reservations = context.Reservations
                             .Where(q => q.User.Email == email);


            return await reservations.ToListAsync();
        }
        public async Task<ICollection<Reservation>> GetReservationsBySpotId(int id)
        {
            IQueryable<Reservation> reservations = context.Reservations
                             .Where(q => q.Spot.Id == id);

            return await reservations.ToListAsync();
        }

        public async Task<ICollection<ParkingSpot>> CheckIfPeriodFromNowIsAvaliableInAParking(int parkingId, DateTime endPeriodTime)
        {
            var parking = await context.Parkings.Where(x => x.Id == parkingId).Include(x => x.ParkingSpots).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            var freeSpotsInParking = parking.ParkingSpots.Where(x => x.IsFree == true);
            List<ParkingSpot> avSpots = new List<ParkingSpot>();
            foreach (var spot in freeSpotsInParking)
            {
                if (spot.Reservations != null && spot.Reservations.Count > 0)
                {
                    var orderedSpotReservations = spot.Reservations.Where(x => x.IsEnded == false).OrderBy(x => x.StartReservationPeriod).ToList();
                    if (orderedSpotReservations != null && orderedSpotReservations.Count > 0)
                    {
                        if (orderedSpotReservations[0].StartReservationPeriod >= endPeriodTime.AddHours(1))
                        {
                            avSpots.Add(spot);
                        }
                    }
                    else
                    {
                        avSpots.Add(spot);
                    }
                }
                else
                {
                    avSpots.Add(spot);
                }
            }
            return avSpots;
        }
        public async Task<ICollection<ParkingSpot>> CheckIfPeriodIsAvaliableInAParking(int parkingId, DateTime startPeriodTime, DateTime endPeriodTime)
        {
            IQueryable<Reservation> t = context.Reservations
                             .Where(q => q.Spot.ParkingId == parkingId)
                             .Include(x => x.Spot);
            var reservationsAtParking = await t.ToListAsync();
            IQueryable<ParkingSpot> p = context.ParkingSpots
                             .Where(q => q.ParkingId == parkingId)
                             .Where(q => (q.Reservations == null || q.Reservations.Count == 0));
            var parkingSpotsAtCurrParkingWithoutReservations = await p.ToListAsync();

            DateTime? posiblyAvaliableStartPeriod = null;
            DateTime? posiblyAvaliableEndPeriod = null;
            ParkingSpot? posiblyAvaliableSpot = null;
            List<DateTime> posiblyAvaliableStartPeriods = new List<DateTime>();
            List<DateTime> posiblyAvaliableEndPeriods = new List<DateTime>();
            List<ParkingSpot> posiblyAvaliableSpots = new List<ParkingSpot>();

            for (int i = 0; i < reservationsAtParking.Count(); i++)
            {
                var currIndexReservation = reservationsAtParking.ElementAt(i);
                var comparedStartPeriodWithEndReservedPeriodValue = DateTime.Compare(startPeriodTime, currIndexReservation.EndReservationPeriod);
                var comparedEndPeriodWithStartReservedPeriodValue = DateTime.Compare(endPeriodTime, currIndexReservation.StartReservationPeriod);
                if (comparedEndPeriodWithStartReservedPeriodValue < 0 || comparedStartPeriodWithEndReservedPeriodValue > 0)
                {
                    var u = await GetSpot(currIndexReservation.SpotId);
                    int startPeriodTimeWithCurrSpotBusyTillTime = 0;
                    if (!u.IsFree)
                    {
                        var activeReservation = reservationsAtParking.Where(x => x.IsStarted == true && x.IsEnded == false).FirstOrDefault();
                        if (activeReservation != null)
                        {
                            if (!activeReservation.Is15MinOver)
                            {
                                var g = activeReservation.EndReservationPeriod.AddHours(1);
                                startPeriodTimeWithCurrSpotBusyTillTime = DateTime.Compare(g, startPeriodTime);
                            }
                            else
                            {
                                var g = DateTime.Now.AddHours(1);
                                startPeriodTimeWithCurrSpotBusyTillTime = DateTime.Compare(g, startPeriodTime);
                            }
                        }
                    }
                    if (u.IsFree || startPeriodTimeWithCurrSpotBusyTillTime <= 0)
                    {
                        if (posiblyAvaliableStartPeriod == null)
                        {
                            posiblyAvaliableStartPeriod = startPeriodTime;
                            posiblyAvaliableEndPeriod = endPeriodTime;
                            posiblyAvaliableSpot = currIndexReservation.Spot;

                            posiblyAvaliableStartPeriods.Add(startPeriodTime);
                            posiblyAvaliableEndPeriods.Add(endPeriodTime);
                            posiblyAvaliableSpots.Add(currIndexReservation.Spot);
                        }
                        else
                        {
                            var a = posiblyAvaliableSpots.Find(x => x.Id == currIndexReservation.Spot.Id);

                            if (a != null)
                            {
                                var index = posiblyAvaliableSpots.IndexOf(a);
                                var comparedPosiblyStartPeriodWithEndReservedPeriodValue = DateTime.Compare((DateTime)posiblyAvaliableStartPeriods.ElementAt(index), currIndexReservation.EndReservationPeriod);
                                var comparedPosiblyEndPeriodWithStartReservedPeriodValue = DateTime.Compare((DateTime)posiblyAvaliableEndPeriods.ElementAt(index), currIndexReservation.StartReservationPeriod);

                                if (!(comparedPosiblyEndPeriodWithStartReservedPeriodValue < 0 || comparedStartPeriodWithEndReservedPeriodValue > 0))
                                {
                                    posiblyAvaliableSpots.RemoveAt(index);
                                    posiblyAvaliableStartPeriods.RemoveAt(index);
                                    posiblyAvaliableEndPeriods.RemoveAt(index);
                                }
                            }
                            else
                            {
                                var comparedPosiblyStartPeriodWithEndReservedPeriodValue = DateTime.Compare(startPeriodTime, currIndexReservation.EndReservationPeriod);
                                var comparedPosiblyEndPeriodWithStartReservedPeriodValue = DateTime.Compare(endPeriodTime, currIndexReservation.StartReservationPeriod);

                                if (comparedPosiblyEndPeriodWithStartReservedPeriodValue < 0 || comparedStartPeriodWithEndReservedPeriodValue > 0)
                                {
                                    posiblyAvaliableStartPeriods.Add(startPeriodTime);
                                    posiblyAvaliableEndPeriods.Add(endPeriodTime);
                                    posiblyAvaliableSpots.Add(currIndexReservation.Spot);
                                }
                            }
                        }
                    }
                }
            }
            if (parkingSpotsAtCurrParkingWithoutReservations != null && parkingSpotsAtCurrParkingWithoutReservations.Count > 0)
            {
                var paidSpots = parkingSpotsAtCurrParkingWithoutReservations.Where(x => !x.IsFree).ToList();
                if (paidSpots != null && paidSpots.Count > 0)
                {
                    foreach (var spot in paidSpots)
                    {
                        if (spot.CurrentReservation != null)
                        {
                            var comparedStartPeriodWithEndPaidPeriodValue = DateTime.Compare(startPeriodTime, spot.CurrentReservation.EndReservationPeriod);
                            if (comparedStartPeriodWithEndPaidPeriodValue > 0)
                            {
                                posiblyAvaliableSpots.Add(spot);
                                posiblyAvaliableStartPeriods.Add(startPeriodTime);
                                posiblyAvaliableEndPeriods.Add(endPeriodTime);
                            }
                        }
                    }
                }
                var freeSpots = parkingSpotsAtCurrParkingWithoutReservations.Where(x => x.IsFree).ToList();
                if (freeSpots != null && freeSpots.Count > 0)
                {
                    foreach (var spot in freeSpots)
                    {
                        posiblyAvaliableSpots.Add(spot);
                        posiblyAvaliableStartPeriods.Add(startPeriodTime);
                        posiblyAvaliableEndPeriods.Add(endPeriodTime);
                    }
                }
            }
            if (posiblyAvaliableStartPeriods == null)
            {
                return null;
            }
            else
            {
                return posiblyAvaliableSpots;
            }
        }

        public async Task<Reservation> ChangeReservationSpot(int reservationId, int newSpotId)
        {
            var reservation = await context.Reservations.Where(x => x.Id == reservationId).Include(x => x.Spot).FirstOrDefaultAsync();
            var oldSpot = reservation.Spot;
            var newSpot = await context.ParkingSpots.Where(x => x.Id == newSpotId).Include(x => x.Reservations).FirstOrDefaultAsync();
            if (oldSpot != null && newSpot != null && reservation != null && oldSpot.Id != newSpotId)
            {
                oldSpot.Reservations.Remove(reservation);
                reservation.Spot = newSpot;
                reservation.SpotId = newSpotId;
                newSpot.Reservations.Add(reservation);

                context.ParkingSpots.Attach(oldSpot);
                context.Reservations.Attach(reservation);
                context.ParkingSpots.Attach(newSpot);

                context.Entry(oldSpot).State = EntityState.Modified;
                context.Entry(reservation).State = EntityState.Modified;
                context.Entry(newSpot).State = EntityState.Modified;
                await Save();
                return reservation;
            }
            return null;
        }
        public async Task<Reservation> ReservationCanStartEarly(int reservationId)
        {
            var reservation = await context.Reservations.Where(x => x.Id == reservationId).FirstOrDefaultAsync();
            reservation.CanStartEarly = true;

            context.Reservations.Attach(reservation);

            context.Entry(reservation).State = EntityState.Modified;
            await Save();
            return reservation;
        }

        public async Task<Reservation> FailReservation(int reservationId)
        {
            var reservation = await context.Reservations.Where(x => x.Id == reservationId).FirstOrDefaultAsync();
            reservation.IsFailed = true;
            reservation.IsEnded = true;

            var amaunt = reservation.Price + 10;
            AddCredits(reservation.UserId, amaunt);

            context.Reservations.Attach(reservation);
            context.Entry(reservation).State = EntityState.Modified;
            await Save();
            return reservation;
        }

        public double? CalculatePrice(ParkingSpot spot, int minutes)
        {
            int fullHours = minutes / 60;
            var fullMins = minutes % 60;
            double price = 0;
            if (fullHours == null)
            {
                return null;
            }
            if (fullHours < 0)
            {
                return null;
            }

            else if (fullMins == 0)
            {
                if (fullHours < 4)
                {
                    price = fullHours * spot.Parking.CostPerHour;
                }
                else if (fullHours < 8)
                {
                    price = fullHours * spot.Parking.CostPer4h;
                }
                else if (fullHours < 24)
                {
                    price = fullHours * spot.Parking.CostPer8h;
                }
                else if (fullHours < 168)
                {
                    price = fullHours * spot.Parking.CostPerDay;
                }
                else
                {
                    price = fullHours * spot.Parking.CostPerWeek;
                }
            }
            if (fullMins > 0)
            {
                if (fullHours + 1 < 4)
                {
                    price = (fullHours + 1) * spot.Parking.CostPerHour;
                }
                else if ((fullHours + 1) < 8)
                {
                    price = (fullHours + 1) * spot.Parking.CostPer4h;
                }
                else if ((fullHours + 1) < 24)
                {
                    price = (fullHours + 1) * spot.Parking.CostPer8h;
                }
                else if ((fullHours + 1) < 168)
                {
                    price = (fullHours + 1) * spot.Parking.CostPerDay;
                }
                else
                {
                    price = (fullHours + 1) * spot.Parking.CostPerWeek;
                }
            }
            return price;
        }
        public async Task<Reservation> AddOneHourToReservation(int id, int? newSpotId = null)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Parking).ThenInclude(x => x.Reservations).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (newSpotId == null)
            {
                reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(1);
                reservation.Price = reservation.Price + reservation.Parking.CostPerHour;
                reservation.is1hrAdded = true;

                context.Reservations.Attach(reservation);
                context.Entry(reservation).State = EntityState.Modified;
                await Save();
                return reservation;
            }
            else
            {
                var newSpot = await context.ParkingSpots.Where(x => x.Id == newSpotId).FirstOrDefaultAsync();
                reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(1);
                reservation.SpotId = newSpot.Id;
                reservation.Spot = newSpot;
                reservation.Price = reservation.Price + reservation.Parking.CostPerHour;
                reservation.is1hrAdded = true;
                newSpot.Reservations.Add(reservation);

                context.Reservations.Attach(reservation);
                context.ParkingSpots.Attach(newSpot);
                context.Entry(reservation).State = EntityState.Modified;
                context.Entry(newSpot).State = EntityState.Modified;
                await Save();
                return reservation;
            }
        }
        public async Task<int?> CheckIfPossibleAddOneHourToReservation(int id)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Parking).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }

            var spotReservations = reservation.Spot.Reservations.OrderBy(x => x.StartReservationPeriod).ToList();
            var reservationIndex = spotReservations.IndexOf(reservation);
            if (spotReservations.Count() == reservationIndex + 1)
            {
                return reservation.SpotId;
            }
            else if (spotReservations[reservationIndex + 1].StartReservationPeriod >= reservation.EndReservationPeriod.AddHours(2))
            {
                return reservation.SpotId;
            }
            else
            {
                foreach (var spot in reservation.Parking.ParkingSpots.Where(x => x.Id != reservation.SpotId).ToList())
                {
                    var tryCurrSpot = await CheckIfPossibleAddingDefinedReservationToThisSpotsReservations(spot.Id, spotReservations[reservationIndex + 1].StartReservationPeriod, spotReservations[reservationIndex + 1].EndReservationPeriod);
                    if (tryCurrSpot != null)
                    {
                        return tryCurrSpot.Id;
                    }
                }
            }
            return null;
        }

        public async Task<Reservation> TryEditReservationPeriod(int id, DateTime newStartTime, DateTime newEndTime)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Parking).ThenInclude(x => x.ParkingSpots).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            var avaliableOnSameSpot = await CheckIfPossibleEditReservationPeriodWithoutChangeSpot(id, reservation.SpotId, newStartTime, newEndTime);
            if (avaliableOnSameSpot == null)
            {
                foreach (var spot in reservation.Parking.ParkingSpots.Where(x => x.Id != reservation.SpotId).ToList())
                {
                    var avaliabeAnotherSpot = await CheckIfPossibleAddingDefinedReservationToThisSpotsReservations(spot.Id, newStartTime, newEndTime);
                    if (avaliabeAnotherSpot != null)
                    {

                        if (newStartTime >= DateTime.Now.AddHours(1) || avaliableOnSameSpot.IsFree)
                        {
                            var oldPrice = reservation.Price;
                            TimeSpan ts = newEndTime - newStartTime;
                            var totalMin = ts.TotalMinutes;
                            var newPrice = CalculatePrice(spot, (int)totalMin);
                            if (DateTime.Now.AddHours(2) > reservation.StartReservationPeriod)
                            {
                                if (oldPrice - newPrice >= 5 && oldPrice - newPrice < 10)
                                {
                                    newPrice = newPrice + 2;
                                }
                                else if (oldPrice - newPrice >= 10 && oldPrice - newPrice < 20)
                                {
                                    newPrice = newPrice + 4;
                                }
                            }

                            reservation.StartReservationPeriod = newStartTime;
                            reservation.EndReservationPeriod = newEndTime;
                            reservation.SpotId = avaliabeAnotherSpot.Id;
                            reservation.Spot = avaliabeAnotherSpot;
                            reservation.Price = (double)newPrice;

                            return reservation;
                        }
                        return null;
                    }
                }
                return null;
            }
            else
            {
                if (newStartTime >= DateTime.Now.AddHours(1) || avaliableOnSameSpot.IsFree)
                {
                    var oldPrice = reservation.Price;
                    TimeSpan ts = newEndTime - newStartTime;
                    var totalMin = ts.TotalMinutes;
                    var newPrice = CalculatePrice(reservation.Spot, (int)totalMin);
                    if (DateTime.Now.AddHours(2) > reservation.StartReservationPeriod)
                    {
                        if (oldPrice - newPrice >= 5 && oldPrice - newPrice < 10)
                        {
                            newPrice = newPrice + 2;
                        }
                        else if (oldPrice - newPrice >= 10 && oldPrice - newPrice < 20)
                        {
                            newPrice = newPrice + 4;
                        }
                    }
                    reservation.StartReservationPeriod = newStartTime;
                    reservation.EndReservationPeriod = newEndTime;
                    reservation.SpotId = avaliableOnSameSpot.Id;
                    reservation.Spot = avaliableOnSameSpot;
                    reservation.Price = (double)newPrice;

                    return reservation;
                }
                return null;
            }
        }

        public async Task<Reservation> EditReservationPeriod(int id, DateTime newStartTime, DateTime newEndTime, double newPrice, int? newSpotId = null)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Parking).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (newSpotId != null)
            {
                var newSpot = await context.ParkingSpots.Where(x => x.Id == newSpotId).Include(x => x.Reservations).FirstOrDefaultAsync();

                reservation.StartReservationPeriod = newStartTime;
                reservation.EndReservationPeriod = newEndTime;
                reservation.SpotId = newSpot.Id;
                reservation.Spot = newSpot;
                reservation.Price = (double)newPrice;
                newSpot.Reservations.Add(reservation);

                context.Reservations.Attach(reservation);
                context.ParkingSpots.Attach(newSpot);
                context.Entry(reservation).State = EntityState.Modified;
                context.Entry(newSpot).State = EntityState.Modified;
                await Save();

                return reservation;
            }
            else
            {
                reservation.StartReservationPeriod = newStartTime;
                reservation.EndReservationPeriod = newEndTime;
                reservation.Price = (double)newPrice;

                context.Reservations.Attach(reservation);
                context.Entry(reservation).State = EntityState.Modified;
                await Save();

                return reservation;
            }
        }
        public async Task<Reservation> IncreaseReservationEndPeriod(int reservationId, int hoursToIncrease)
        {
            IQueryable<Reservation> reservations1 = context.Reservations
                            .Where(q => q.Id == reservationId).Include(x => x.Parking).Include(x => x.Spot);
            var reservation = await reservations1.FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not resrvation with this Id!");
            }
            IQueryable<ParkingSpot> spot1 = context.ParkingSpots
                            .Where(q => q.Id == reservation.SpotId)
                            .Include(x => x.Parking)
                            .Include(x => x.Reservations);
            var spot = await spot1.FirstOrDefaultAsync();
            var futureSpotReservations = spot.Reservations.Where(x => x.StartReservationPeriod > reservation.EndReservationPeriod).ToList();
            if (futureSpotReservations == null || futureSpotReservations.Count == 0)
            {
                reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(hoursToIncrease);
                context.Reservations.Attach(reservation);
                context.Entry(reservation).State = EntityState.Modified;
                await Save();
                return reservation;
            }
            if (reservation.StartReservationPeriod <= DateTime.Now)
            {
                var v = futureSpotReservations.Where(x => x.StartReservationPeriod <= reservation.EndReservationPeriod.AddHours(hoursToIncrease + 1)).ToList();
                if (v != null && v.Count != 0)
                {
                    var possibly = await CheckIfPossibleToIncreasePeriodOfReservationOnCurrSpot(reservation, hoursToIncrease);
                    if (possibly == true)
                    {
                        for (int i = 0; i < v.Count; i++)
                        {
                            var check = v[i];
                            var res = await CheckIfPeriodIsAvaliableInAParking(v[i].ParkingId, v[i].StartReservationPeriod, v[i].EndReservationPeriod);
                            var ress = res.ToList();
                            if (ress != null && ress.Count > 0)
                            {
                                var e = v[i].Spot;
                                e.Reservations.Remove(v[i]);
                                v[i].Spot = ress[0];
                                v[i].SpotId = ress[0].Id;
                                v[i].Spot.Reservations.Add(v[i]);
                                ress[0].Reservations.Add(v[i]);
                                futureSpotReservations.Remove(v[i]);

                                context.ParkingSpots.Attach(e);
                                context.Reservations.Attach(v[i]);
                                context.ParkingSpots.Attach(v[i].Spot);
                                context.Entry(e).State = EntityState.Modified;
                                context.Entry(v[i]).State = EntityState.Modified;
                                context.Entry(v[i].Spot).State = EntityState.Modified;
                                await Save();
                            }
                        }
                    }

                    if (futureSpotReservations == null || futureSpotReservations.Count == 0)
                    {
                        reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(hoursToIncrease);
                        context.Reservations.Attach(reservation);
                        context.Entry(reservation).State = EntityState.Modified;
                        await Save();
                        return reservation;
                    }
                    for (int i = hoursToIncrease; i > 0; i--)
                    {
                        var k = futureSpotReservations.Find(x => x.StartReservationPeriod <= reservation.EndReservationPeriod.AddHours(i + 1));
                        if (k == null)
                        {
                            throw new ArgumentException("This reservation is active, and it's spot will be avaliable for increase endReservationPeriod with no more than " + i + " hours!");
                        }
                    }
                    throw new ArgumentException("This reservation is active, and it's spot is not avaliable for the next hours!");
                }
            }
            else
            {
                var nextSpotReservationWithinThisPeriod = futureSpotReservations.Find(x => x.StartReservationPeriod <= reservation.EndReservationPeriod.AddHours(hoursToIncrease + 1));
                if (nextSpotReservationWithinThisPeriod != null)
                {
                    IQueryable<Parking> parking1 = context.Parkings
                           .Where(q => q.Id == reservation.ParkingId)
                           .Include(x => x.ParkingSpots)
                           .Include(x => x.Reservations);
                    var a = await parking1.FirstOrDefaultAsync();
                    var b = a.ParkingSpots.Where(x => x.IsPaidTill == null || (DateTime)x.IsPaidTill.Value.AddHours(1) <= reservation.StartReservationPeriod).ToList();
                    for (int i = 0; i < b.Count; i++)
                    {
                        if (b[i].Reservations == null || b[i].Reservations.Count == 0)
                        {
                            reservation.Spot = b[i];
                            reservation.SpotId = b[i].Id;
                            reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(hoursToIncrease);
                            context.Reservations.Attach(reservation);
                            context.ParkingSpots.Attach(b[i]);
                            context.Entry(reservation).State = EntityState.Modified;
                            context.Entry(b[i]).State = EntityState.Modified;
                            await Save();
                            return reservation;
                        }
                        var t = b[i].Reservations.ToList().Find(x => (x.StartReservationPeriod <= reservation.StartReservationPeriod) && (x.EndReservationPeriod.AddHours(1) > reservation.StartReservationPeriod));
                        var h = b[i].Reservations.ToList().Find(x => (x.StartReservationPeriod >= reservation.StartReservationPeriod) && (x.StartReservationPeriod <= reservation.EndReservationPeriod.AddHours(hoursToIncrease + 1)));
                        if (t == null && h == null)
                        {
                            reservation.EndReservationPeriod = reservation.EndReservationPeriod.AddHours(hoursToIncrease);
                            reservation.SpotId = b[i].Id;
                            reservation.Spot = b[i];
                            b[i].Reservations.Add(reservation);
                            context.Reservations.Attach(reservation);
                            context.ParkingSpots.Attach(b[i]);
                            context.Entry(reservation).State = EntityState.Modified;
                            context.Entry(b[i]).State = EntityState.Modified;
                            await Save();
                            return reservation;
                        }
                    }
                }
            }
            throw new ArgumentException("There is not way to increase your reservation with " + hoursToIncrease + " hours !");
        }

        public async Task<bool> CheckIfPossibleToIncreasePeriodOfReservationOnCurrSpot(Reservation reservation, int hoursToIncrease)
        {
            var spot = reservation.Spot;
            var parking = reservation.Parking;
            var spotFutureReservations = spot.Reservations.Where(x => x.StartReservationPeriod > reservation.StartReservationPeriod).ToList();
            if (spotFutureReservations == null && spotFutureReservations.Count == 0)
            {
                return true;
            }
            var spotFutureReservationsWhichBlockIncreaseTime = spotFutureReservations.Where(x => x.StartReservationPeriod <= reservation.EndReservationPeriod.AddHours(hoursToIncrease)).ToList();
            if (spotFutureReservationsWhichBlockIncreaseTime == null && spotFutureReservationsWhichBlockIncreaseTime.Count == 0)
            {
                return true;
            }
            var res = true;
            for (int i = 0; i < spotFutureReservationsWhichBlockIncreaseTime.Count; i++)
            {
                var spots = parking.ParkingSpots.ToList();
                for (int j = 0; j < spots.Count; j++)
                {
                    var check1 = spots[j].Reservations.ToList().Find(x => (x.StartReservationPeriod <= reservation.StartReservationPeriod) && (x.EndReservationPeriod >= reservation.StartReservationPeriod));
                    var check2 = spots[j].Reservations.ToList().Find(x => (x.StartReservationPeriod <= reservation.EndReservationPeriod) && (x.EndReservationPeriod >= reservation.EndReservationPeriod));
                    if (check1 != null || check2 != null)
                    {
                        res = false;
                    }
                }
            }
            return res;
        }


        public async Task<Reservation> AddReservation(Reservation reservation, bool fromNow = false)
        {
            var parkingId = reservation.ParkingId;
            var spotId = reservation.SpotId;
            var parking = await context.Parkings.Where(x => x.Id == parkingId).FirstOrDefaultAsync();
            if (parking == null)
            {
                throw new ArgumentException("There is not parking with this Id!");
            }
            var spot = await context.ParkingSpots.Where(x => x.Id == spotId).FirstOrDefaultAsync();
            if (spot == null)
            {
                throw new ArgumentException("There is not spot with this Id!");
            }
            var user = await context.Users.Where(x => x.Id == reservation.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("There is no user with this Id!");
            }
            var resVehicle = await context.Vehicles.Where(x => x.RegistrationNumber == reservation.CarRegNumber).FirstOrDefaultAsync();
            int? resVehicleId = null;
            if (resVehicle == null)
            {
                var newVehicle = new Vehicle();
                newVehicle.RegistrationNumber = reservation.CarRegNumber;
                context.Vehicles.Add(newVehicle);
                context.Entry(newVehicle).State = EntityState.Added;
                resVehicleId = newVehicle.Id;
                resVehicle = newVehicle;
            }
            else
            {
                resVehicleId = resVehicle.Id;
            }
            if (user.Vehicles == null)
            {
                user.Vehicles = new List<Vehicle>();
                user.Vehicles.Add(resVehicle);
                context.Users.Attach(user);
                context.Entry(user).State = EntityState.Modified;
            }
            else if (!user.Vehicles.Contains(resVehicle))
            {
                user.Vehicles.Add(resVehicle);
                context.Users.Attach(user);
                context.Entry(user).State = EntityState.Modified;
            }
            if (fromNow)
            {
                reservation.StartReservationPeriod = DateTime.Now;
            }
            reservation.EndReservationPeriod = reservation.EndReservationPeriod;
            TimeSpan ts = reservation.EndReservationPeriod - reservation.StartReservationPeriod;
            var totalMin = ts.TotalMinutes;
            var price = CalculatePrice(spot, (int)totalMin);
            reservation.Price = (double)price!;
            reservation.IsPaid = false;
            reservation.CreatedResrvationTime = DateTime.Now;
            reservation.Spot = await GetSpot(reservation.SpotId);
            reservation.User = user;
            context.Reservations.Add(reservation);
            parking.Reservations.Add(reservation);
            spot.Reservations.Add(reservation);
            context.Parkings.Attach(parking);
            context.ParkingSpots.Attach(spot);
            context.Entry(parking).State = EntityState.Modified;
            context.Entry(spot).State = EntityState.Modified;
            await Save();
            return reservation;
        }

        public async Task<Reservation> PayReservation(int id)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.IsPaid == true)
            {
                throw new ArgumentException("This reservation is already paid !");
            }
            reservation.IsPaid = true;
            var user = await context.Users.Where(x => x.Id == reservation.UserId).FirstOrDefaultAsync();
            if (user.ParkCredits != null)
            {
                if (user.ParkCredits <= reservation.Price)
                {
                    user.ParkCredits = 0;
                }
                else
                {
                    user.ParkCredits = user.ParkCredits - reservation.Price;
                }
            }
            context.Reservations.Attach(reservation);
            context.Users.Attach(user);
            context.Entry(reservation.User).State = EntityState.Modified;
            context.Entry(reservation).State = EntityState.Modified;
            await Save();
            return reservation;
        }
        public async Task<Reservation> StartReservation(int id)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Spot).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.IsStarted == true)
            {
                throw new ArgumentException("This reservation is already started !");
            }

            reservation.Spot.IsFree = false;
            reservation.IsStarted = true;

            await Save();
            return reservation;
        }
        public async Task<Reservation> EndReservation(int id)
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Spot).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.IsEnded == true)
            {
                throw new ArgumentException("This reservation is already ended !");
            }
            if (reservation.MinOver == 0)
            {
                AddCredits(reservation.UserId, reservation.Price / 2);
                reservation.isUserCreditsUpdated = true;
                context.Entry(reservation).State = EntityState.Modified;
                await Save();
            }
            else if (reservation.MinOver > 15)
            {
                var fullHours = (reservation.MinOver - 15) / 60;
                var hours = Math.Round(fullHours);
                if (hours < fullHours)
                {
                    hours++;
                }
                RemoveCredits(reservation.UserId, hours * 20);
            }

            reservation.isUserCreditsUpdated = true;
            reservation.IsEnded = true;
            reservation.Spot.IsFree = true;
            context.Reservations.Attach(reservation);
            context.Entry(reservation).State = EntityState.Modified;

            context.ParkingSpots.Attach(reservation.Spot);
            context.Entry(reservation.Spot).State = EntityState.Modified;
            await Save();
            return reservation;
        }

        public async Task<Reservation> CheckAndBlockReservation(int id)///////?????
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.StartReservationPeriod <= DateTime.Now)
            {
                var spot = reservation.Spot;
                var spotReservations = spot.Reservations.OrderBy(x => x.StartReservationPeriod).ToList();
                var reservationIndex = spotReservations.IndexOf(reservation);
                if (reservationIndex > 0)
                {
                    if (spotReservations[reservationIndex - 1].IsEnded == false && reservation.IsBlocked == false)
                    {
                        reservation.IsBlocked = true;
                        context.Reservations.Attach(reservation);
                        context.Entry(reservation).State = EntityState.Modified;
                        await Save();
                        return reservation;
                    }
                }
            }
            return null;
        }


        public async Task<Reservation> ClientLateForEndOfReservation(int id)///////????? not using anywhere
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.StartReservationPeriod <= DateTime.Now)
            {
                var spot = reservation.Spot;
                var spotReservations = spot.Reservations.OrderBy(x => x.StartReservationPeriod).ToList();
                var reservationIndex = spotReservations.IndexOf(reservation);
                if (reservationIndex > 0)
                {
                    if (spotReservations[reservationIndex - 1].IsEnded == false && reservation.IsBlocked == false)
                    {
                        reservation.IsBlocked = true;
                        context.Reservations.Attach(reservation);
                        context.Entry(reservation).State = EntityState.Modified;
                        await Save();
                        return reservation;
                    }
                }
            }
            return null;
        }

        public async Task<Reservation> ReservationGotOverPaidTime(int id)///////?????
        {
            var reservation = await context.Reservations.Where(x => x.Id == id).Include(x => x.Spot).ThenInclude(x => x.Reservations).FirstOrDefaultAsync();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.EndReservationPeriod < DateTime.Now)
            {
                TimeSpan ts = reservation.EndReservationPeriod - DateTime.Now;
                var totalMin = ts.TotalMinutes;
                if (totalMin > 15)
                {
                    reservation.MinOver = totalMin;
                    var spot = reservation.Spot;
                    var spotReservations = spot.Reservations.OrderBy(x => x.StartReservationPeriod).ToList();
                    var currReservationIndex = spotReservations.IndexOf(reservation);
                    if (currReservationIndex >= 0)
                    {
                        var nextReservation = spotReservations[currReservationIndex + 1];
                        TimeSpan ts2 = nextReservation.StartReservationPeriod - DateTime.Now;
                        var totalMin2 = ts.TotalMinutes;
                        if (totalMin2 < 20)
                        {
                            var parkingSpots = await context.ParkingSpots.Where(x => x.ParkingId == reservation.ParkingId).ToListAsync();
                            foreach (var parkingSpot in parkingSpots)
                            {
                                var check = await CheckIfPossibleAddingDefinedReservationToThisSpotsReservations(parkingSpot.Id, nextReservation.StartReservationPeriod, nextReservation.EndReservationPeriod);
                                if (check != null)
                                {
                                    var avNewSpot = check;
                                    //  reservation.IsPaid = false;
                                    nextReservation.Spot = avNewSpot;
                                    nextReservation.IsBlocked = false;
                                    avNewSpot.Reservations.Add(nextReservation);
                                    context.ParkingSpots.Attach(avNewSpot);
                                    context.Entry(nextReservation).State = EntityState.Modified;
                                    context.Entry(avNewSpot).State = EntityState.Modified;
                                    await Save();
                                }
                                else
                                {
                                    nextReservation.IsBlocked = true;
                                    context.Entry(nextReservation).State = EntityState.Modified;
                                    await Save();
                                }
                            }
                        }
                    }
                    reservation.Is15MinOver = true;
                    context.Reservations.Attach(reservation);
                    context.Entry(reservation).State = EntityState.Modified;
                    await Save();
                    return reservation;
                }
            }
            return null;
        }

        public async Task<Reservation> UpdateReservations()///////?????
        {
            var reservations = await context.Reservations.ToListAsync();
            var newFailedReservations = reservations.Where(x => x.StartReservationPeriod.AddMinutes(1) > DateTime.Now && x.IsBlocked == true && x.IsFailed == false);
            var notStartedReservations = reservations.Where(x => x.StartReservationPeriod > DateTime.Now).ToList();
            var endedReservations = reservations.Where(x => x.IsEnded).ToList();
            if (reservations == null)
            {
                throw new ArgumentException("There is no reservations!");
            }
            foreach (var reservation in reservations)
            {
                if (reservation.IsStarted && !reservation.IsEnded)
                {
                    ReservationGotOverPaidTime(reservation.Id);
                }
                if (!reservation.IsStarted && reservation.Spot.IsFree && DateTime.Now.AddMinutes(90) >= reservation.StartReservationPeriod)
                {
                    reservation.CanStartEarly = true;
                    context.Entry(reservation).State = EntityState.Modified;
                    await Save();

                }
            }
            foreach (var reservation in endedReservations)
            {
              
            }
            var blockedReservations = notStartedReservations.Where(x => x.IsBlocked).ToList();
            foreach (var reservation in blockedReservations)
            {

            }
            if (newFailedReservations != null && newFailedReservations.Count() > 0)
            {
                foreach (var reservation in newFailedReservations)
                {
                    reservation.IsFailed = true;
                    AddCredits(reservation.UserId, reservation.Price + 10);
                    context.Entry(reservation).State = EntityState.Modified;
                    await Save();

                }
            }
            return null;
        }


        public async Task<Reservation> UpdateReservationCarNumber(int id, string number)///////?????
        {

            var reservation = await context.Reservations.Where(x => x.Id == id).FirstOrDefaultAsync();
            reservation.CarRegNumber = number;
            context.Entry(reservation).State = EntityState.Modified;
            await Save();
            return reservation;
        }

        public async Task CancelReservation(int reservationId)
        {
            var reservation = context.Reservations.Where(x => x.Id == reservationId).FirstOrDefault();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            if (reservation.StartReservationPeriod >= DateTime.Now.AddMinutes(120))
            {
                AddCredits(reservation.UserId, reservation.Price);
                DeleteReservation(reservationId);
            }
            else if (reservation.StartReservationPeriod < DateTime.Now.AddMinutes(120) && reservation.StartReservationPeriod > DateTime.Now.AddMinutes(30))
            {
                AddCredits(reservation.UserId, (7 * reservation.Price) / 10);
                DeleteReservation(reservationId);
            }
            else
            {
                AddCredits(reservation.UserId, reservation.Price / 2);
                DeleteReservation(reservationId);
            }
        }

        public async Task DeleteReservation(int id)
        {
            var reservation = context.Reservations.Where(x => x.Id == id).FirstOrDefault();
            if (reservation == null)
            {
                throw new ArgumentException("There is not reservation with this Id!");
            }
            context.Reservations.Remove(reservation);
            await Save();
        }
        #endregion

        public async Task<ICollection<Parking>> GetAvaliableParkingsForPeriod(DateTime startDate, DateTime endDate)
        {
            IQueryable<Parking> a = context.Parkings;
            List<Parking> avParkings = new List<Parking>();
            var parkings = await a.ToListAsync();
            foreach (var parking in parkings)
            {
                var res = await CheckIfPeriodIsAvaliableInAParking(parking.Id, startDate, endDate);
                if (res != null && (res.Count > 0))
                {
                    avParkings.Add(parking);
                }
            }
            return avParkings;
        }

        public async Task Save()
        {
            context.SaveChanges();
        }


        #region not-used


        public async Task<ICollection<Reservation>> GetUserReservations(int userId)
        {
            IQueryable<Reservation> reservations = context.Reservations.Where(x => x.UserId == userId).Include(x => x.Parking).OrderBy(x => x.StartReservationPeriod);

            return await reservations.ToListAsync();
        }






        public async Task DeleteSpot(int id)   //not used in app
        {
            var spot = context.ParkingSpots.Where(x => x.Id == id).FirstOrDefault();
            if (spot == null)
            {
                throw new ArgumentException("There is not spot with this Id!");
            }
            context.ParkingSpots.Remove(spot);
            await Save();
        }


        public async Task ClientGoesFromSpot(int spotId)
        {
            var spot = context.ParkingSpots.Where(x => x.Id == spotId).FirstOrDefault();
            if (spot == null)
            {
                throw new ArgumentException("There is not spot with this Id!");
            }
            spot.IsFree = true;
            context.ParkingSpots.Attach(spot);
            context.Entry(spot).State = EntityState.Modified;
            await Save();

        }

        public async Task ClientParkOnSpot(int spotId)
        {
            var spot = context.ParkingSpots.Where(x => x.Id == spotId).FirstOrDefault();
            if (spot == null)
            {
                throw new ArgumentException("There is not spot with this Id!");
            }
            spot.IsFree = false;
            context.ParkingSpots.Attach(spot);
            context.Entry(spot).State = EntityState.Modified;
            await Save();

        }


        public async Task<ParkingSpot> InsertSpot(ParkingSpot spot)  //not used in app
        {
            var parkingId = spot.ParkingId;
            var c = await context.Parkings.Where(x => x.Id == parkingId).FirstOrDefaultAsync();
            if (c == null)
            {
                throw new ArgumentException("There is not parking with this Id!");
            }
            spot.Parking = c;
            context.ParkingSpots.Add(spot);
            c.ParkingSpots.Add(spot);
            context.Parkings.Attach(c);
            context.Entry(c).State = EntityState.Modified;
            await Save();
            return spot;
        }
        ///
        public async Task<Parking> CreateParking(CreateParking parking) //not used in app
        {
            if (parking.spotsCount == 0)
            {
                throw new ArgumentException("ERROR NO SPOTS!");
            }
            Parking newParking = new Parking();
            newParking.Address = parking.Address;
            newParking.Name = parking.Name;
            newParking.Latitude = parking.Latitude;
            newParking.Longitude = parking.Longitude;
            context.Parkings.Add(newParking);
            await Save();
            for (var i = 1; i <= parking.spotsCount; i++)
            {
                var newSpot = new ParkingSpot();
                newSpot.NumberInParking = i;
                newSpot.ParkingId = newParking.Id;

                context.ParkingSpots.Add(newSpot);
                context.Entry(newSpot).State = EntityState.Added;
                await Save();
                newParking.ParkingSpots.Add(newSpot);
                context.Parkings.Attach(newParking);

                context.Entry(newParking).State = EntityState.Modified;
                await Save();
            }
            var _user = new IdentityUser();
            _user.Email = "admin" + newParking.Id + "@test";
            _user.UserName = "admin" + newParking.Id;
            var results = await userManager.CreateAsync(_user, "admin");

            return newParking;
        }
        public async Task<Parking> InsertParking(Parking parking)  //not used in app
        {
            var address = parking.Address;
            var c = await context.Parkings.Where(x => x.Address == address).FirstOrDefaultAsync();
            if (c != null)
            {
                throw new ArgumentException("There is parking registered on this addess!");
            }

            context.Parkings.Add(parking);
            await Save();
            if (parking.ParkingSpots != null && parking.ParkingSpots.Count > 0)
            {
                for (int i = parking.ParkingSpots.Count - 1; i >= 0; i--)
                {
                    var s = new ParkingSpot();
                    s.ParkingId = parking.Id;
                    context.ParkingSpots.Add(s);
                    await Save();
                }
            }
            return parking;
        }


        public async Task UpdateParking(int id, Parking parking)  //not used in app
        {
            var _parking = context.Parkings.Where(x => x.Id == id).FirstOrDefault();
            if (_parking == null)
            {
                throw new ArgumentException("There is not parking with this Id!");
            }
            parking.Rating = _parking.Rating;
            parking.Address = _parking.Address;
            //  parking.IsSecured = _parking.IsSecured;
            parking.IsAvailable = _parking.IsAvailable;
            parking.HasFreeSpot = _parking.HasFreeSpot;

            parking.Name = _parking.Name;

            parking.ParkingSpots = _parking.ParkingSpots;
            foreach (var spot in _parking.ParkingSpots)
            {
                var e = context.ParkingSpots!.Where(x => x.Id == spot.Id).FirstOrDefault();
                if (e == null)
                {
                    throw new ArgumentException("There is not spot with this ID!");
                }
                parking.ParkingSpots!.Add(spot);

            }

            context.Parkings.Attach(parking);
            context.Entry(parking).State = EntityState.Modified;
            await Save();

        }

        public async Task DeleteParking(int id)  //not used in app
        {
            var parking = context.Parkings.Where(x => x.Id == id).FirstOrDefault();
            if (parking == null)
            {
                throw new ArgumentException("There is not parking with this Id!");
            }
            context.Parkings.Remove(parking);
            await Save();
        }

        #endregion
    }
}
