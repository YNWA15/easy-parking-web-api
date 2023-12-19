using Microsoft.AspNetCore.Mvc;
using PublicParkingsSofiaWebAPI.Models;
using PublicParkingsSofiaWebAPI.Services;

namespace PublicParkingsSofiaWebAPI.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : Controller
    {
        private readonly ParkingServices services;

        public ReservationController(ParkingServices _services)
        {
            services = _services;
        }

        [HttpGet("/withoutFullInfo", Name = "GetReservationWithoutFullInfo")]
        public async Task<IActionResult> GetReservationsWithoutObjects()
        {
            var reservations = await services.GetReservationsWithoutObjects();
            return Ok(reservations);
        }
        [HttpGet()]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await services.GetReservations();
            return Ok(reservations);
        }

        [HttpGet("/userReservations/{userId}")]
        public async Task<IActionResult> GetUserReservations(int userId) ///notUsed
        {
            var reservations = await services.GetUserReservations(userId);
            return Ok(reservations);
        }

        [HttpGet("/userNotEndedReservations/{userId}")]
        public async Task<IActionResult> GetUserNotEndedReservations(int userId)
        {
            var reservations = await services.GetUserNotEndedReservations(userId);
            return Ok(reservations);
        }

        [HttpGet("{id}", Name = "GetReservation")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await services.GetReservation(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        [HttpGet("futureReservations/{parkingId}")]
        public async Task<IActionResult> GetFutureReservationsForParking(int parkingId)
        {
            var reservations = await services.GetFutureReservationsForParking(parkingId);

            if (reservations == null)
            {
                return NotFound();
            }

            return Ok(reservations);
        }

        [HttpGet()]
        [Route("{parkingId}/{startPeriodTime}/{endPeriodTime}")]
        public async Task<IActionResult> CheckIfPeriodIsAvaliableInAParking(int parkingId, DateTime startPeriodTime, DateTime endPeriodTime)
        {
            var res = await services.CheckIfPeriodIsAvaliableInAParking(parkingId, startPeriodTime, endPeriodTime);

            return Ok(res);
        }

        [HttpGet()]
        [Route("/fromNow/{parkingId}/{endPeriodTime}")]
        public async Task<IActionResult> CheckIfPeriodFromNowIsAvaliableInAParking(int parkingId, DateTime endPeriodTime)
        {
            var res = await services.CheckIfPeriodFromNowIsAvaliableInAParking(parkingId, endPeriodTime);

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> InsertReservation([FromBody] Reservation reservation)
        {
            var a = await services.AddReservation(reservation);

            return Ok(a);
        }

        [HttpPost("fromNow")]
        public async Task<IActionResult> InsertReservationFromNow([FromBody] Reservation reservation)
        {
            var a = await services.AddReservation(reservation, true);

            return Ok(a);
        }

        [HttpPost("pay/{reservationId}")]
        public async Task<IActionResult> PayReservation(int reservationId)
        {
            var a = await services.PayReservation(reservationId);

            return Ok(a);
        }

        [HttpPut()]
        [Route("{reservationId}/{hoursToIncrease}")]
        public async Task<IActionResult> IncreaseReservationEndPeriod(int reservationId, int hoursToIncrease)
        {
            var a = await services.IncreaseReservationEndPeriod(reservationId, hoursToIncrease);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/changeReservationSpot/{reservationId}/{newSpotId}")]
        public async Task<IActionResult> ChangeReservationSpot(int reservationId, int newSpotId)
        {
            var a = await services.ChangeReservationSpot(reservationId, newSpotId);

            return Ok(a);
        }

        [HttpPut()]
        [Route("/canStartEarly/{reservationId}")]
        public async Task<IActionResult> ReservationCanStartEarly(int reservationId)
        {
            var a = await services.ReservationCanStartEarly(reservationId);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/failReservation/{reservationId}")]
        public async Task<IActionResult> FailReservation(int reservationId)
        {
            var a = await services.FailReservation(reservationId);

            return Ok(a);
        }



        [HttpPut()]
        [Route("/startReservation/{reservationId}")]
        public async Task<IActionResult> StartReservation(int reservationId)
        {
            var a = await services.StartReservation(reservationId);

            return Ok(a);
        }

        [HttpPut()]
        [Route("/endReservation/{reservationId}")]
        public async Task<IActionResult> EndReservation(int reservationId)
        {
            var a = await services.EndReservation(reservationId);

            return Ok(a);
        }

        [HttpPut()]
        [Route("/CheckAndBlockReservation/{reservationId}")]
        public async Task<IActionResult> BlockReservation(int reservationId)
        {
            var a = await services.CheckAndBlockReservation(reservationId);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/overTime/{reservationId}")]
        public async Task<IActionResult> ReservationGotOverPaidTime(int reservationId)
        {
            var a = await services.ReservationGotOverPaidTime(reservationId);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/updateReservations")]
        public async Task<IActionResult> UpdateReservations()
        {
            var a = await services.UpdateReservations();

            return Ok(a);
        }
        [HttpPut()]
        [Route("/updateReservationCarNumber/{id}/{number}")]
        public async Task<IActionResult> UpdateReservationCarNumber(int id, string number)
        {
            var a = await services.UpdateReservationCarNumber(id, number);

            return Ok(a);
        }

        [HttpGet()]
        [Route("/tryUpdateReservationPeriod/{id}/{newStartTime}/{newEndTime}")]
        public async Task<IActionResult> TryUpdateReservationPeriod(int id, DateTime newStartTime, DateTime newEndTime)
        {
            var a = await services.TryEditReservationPeriod(id, newStartTime, newEndTime);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/updateReservationPeriodWithoutChangeSpot/{id}/{newStartTime}/{newEndTime}/{newPrice}")]
        public async Task<IActionResult> UpdateReservationPeriod(int id, DateTime newStartTime, DateTime newEndTime, double newPrice)
        {
            var a = await services.EditReservationPeriod(id, newStartTime, newEndTime, newPrice);

            return Ok(a);
        }
        [HttpPut()]
        [Route("/updateReservationPeriodAndChangeSpot/{id}/{newStartTime}/{newEndTime}/{newSpotId}/{newPrice}")]
        public async Task<IActionResult> UpdateReservationPeriod(int id, DateTime newStartTime, DateTime newEndTime, double newPrice, int newSpotId)
        {
            var a = await services.EditReservationPeriod(id, newStartTime, newEndTime, newPrice, newSpotId);

            return Ok(a);
        }

        [HttpGet()]
        [Route("/checkIfPossibleAddOneHourToReservation/{reservationId}")]
        public async Task<IActionResult> CheckIfPossibleAddOneHourToReservation(int reservationId)
        {
            var a = await services.CheckIfPossibleAddOneHourToReservation(reservationId);
            if (a == null)
            {
                return Ok(false);
            }
            return Ok(a);
        }
        [HttpPut()]
        [Route("/addOneHourToReservationAndChangeSpot/{reservationId}/{newSpotId}")]
        public async Task<IActionResult> AddOneHourToReservation(int reservationId, int newSpotId)
        {
            var a = await services.AddOneHourToReservation(reservationId, newSpotId);

            return Ok(a);
        }

        [HttpPut()]
        [Route("/addOneHourToReservationWithoutChangeSpot/{reservationId}")]
        public async Task<IActionResult> AddOneHourToReservationWithoutChangeSpot(int reservationId)
        {
            var a = await services.AddOneHourToReservation(reservationId);

            return Ok(a);
        }

        [HttpDelete("cancelReservation/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            await services.CancelReservation(id);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await services.GetReservation(id);

            if (reservation == null)
            {
                return NotFound();
            }

            await services!.DeleteReservation(id);
            return NoContent();
        }
    }
}
