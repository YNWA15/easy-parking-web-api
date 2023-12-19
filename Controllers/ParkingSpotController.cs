using Microsoft.AspNetCore.Mvc;
using PublicParkingsSofiaWebAPI.Models;
using PublicParkingsSofiaWebAPI.Services;

namespace PublicParkingsSofiaWebAPI.Controllers
{
    [ApiController]
    [Route("api/parkingSpots")]
    public class ParkingSpotController : Controller
    {
        private readonly ParkingServices services;

        public ParkingSpotController(ParkingServices _services)
        {
            services = _services;
        }

        [HttpGet()]
        public async Task<IActionResult> GetSpots()
        {
            var spots = await services.GetSpots();
            return Ok(spots);
        }

        [HttpGet()]
        [Route("busyspots/{parkingId}")]

        public async Task<IActionResult> GetBusySpotsOnParking(int parkingId)
        {
            var spots = await services.GetBusySpotsOnParking(parkingId);
            return Ok(spots);
        }

        [HttpGet("{id}", Name = "GetSpot")]
        public async Task<IActionResult> GetSpot(int id)
        {
            var spot = await services.GetSpot(id);

            if (spot == null)
            {
                return NotFound();
            }

            return Ok(spot);
        }
        [HttpGet("currentSpotReservation/{spotId}")]
        public async Task<IActionResult> GetSpotCurrentReservation(int spotId)
        {
            var res = await services.GetSpotCurrentReservation(spotId);

            return Ok(res);
        }

        [HttpGet()]
        [Route("{parkingSpotId}/{resrvationStartPeriodTime}/{reservationEndPeriodTime}")]
        public async Task<IActionResult> CheckIfPossibleAddingReservationToThisSpotsReservations(int parkingSpotId, DateTime resrvationStartPeriodTime, DateTime reservationEndPeriodTime)
        {
            var res = await services.CheckIfPossibleAddingDefinedReservationToThisSpotsReservations(parkingSpotId, resrvationStartPeriodTime, reservationEndPeriodTime);
            if(res != null)
            {
                return Ok("possible");
            }
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> InsertSpot([FromBody] ParkingSpot spot) //not used in app
        {
            var a = await services.InsertSpot(spot);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpot(int id, [FromBody] ParkingSpot spot) //not used in app
        {

            await services!.UpdateSpot(id, spot);

            return Ok();
        }

        [HttpPut("clientParkOnTheSpot/{spotId}")] //notUsed anywhere
        public async Task<IActionResult> ClientParkOnSpot(int spotId)
        {

            await services!.ClientParkOnSpot(spotId);

            return Ok();
        }

        [HttpPut("clienGoesFromTheSpot/{spotId}")]
        public async Task<IActionResult> ClientGoesFromSpot(int spotId)
        {

            await services!.ClientGoesFromSpot(spotId);

            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteSpot(int id)  // not used 
        {
            var spot = await services.GetSpot(id);

            if (spot == null)
            {
                return NotFound();
            }

            await services!.DeleteSpot(id);
            return NoContent();
        }
    }
}
