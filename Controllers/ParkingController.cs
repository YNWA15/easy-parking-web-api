using Microsoft.AspNetCore.Mvc;
using PublicParkingsSofiaWebAPI.Models;
using PublicParkingsSofiaWebAPI.Services;

namespace PublicParkingsSofiaWebAPI.Controllers
{
    [ApiController]
    [Route("api/parkings")]
    public class ParkingController : ControllerBase
    {
        private readonly ParkingServices services;

        public ParkingController(ParkingServices _services)
        {
            services = _services;
        }

        [HttpGet()]
        public async Task<IActionResult> GetParkings()
        {
            var parkings = await services.GetParkings();
            return Ok(parkings);
        }

        [HttpGet("parkingInfo/{parkingId}")]
        public async Task<IActionResult> GetParkingInfo(int parkingId)
        {
            var parking = await services.GetParkingInfo(parkingId);
            return Ok(parking);
        }

        [HttpGet()]
        [Route("sorted/{lat}/{lon}/parkings")]
        public async Task<IActionResult> GetSortedParkings(double lat, double lon)
        {
            var parkings = await services.GetParkings();
            var a = new CoordinatesComparer();
            var b = parkings.OrderByDescending(ob => a.GetDistance(lat, lon, ob.Latitude, ob.Longitude)).ToArray();
           
            return Ok(b);
        }
        [HttpGet()]
        [Route("check/{startPeriodTime}/{endPeriodTime}")]
        public async Task<IActionResult> GetAvaliableParkings(DateTime startPeriodTime, DateTime endPeriodTime)
        {
            ICollection<Parking> parkings = await services.GetAvaliableParkingsForPeriod(startPeriodTime, endPeriodTime);

            return Ok(parkings);
        }

        [HttpGet("{id}", Name = "GetParking")]
        public async Task<IActionResult> GetParking(int id)
        {
            var parking = await services.GetParking(id);

            if (parking == null)
            {
                return NotFound();
            }

            return Ok(parking);
        }
        [HttpPost] //not used
        public async Task<IActionResult> InsertParking([FromBody] Parking parking)
        {
            var a =await services.InsertParking(parking);

            return Ok(a);
        }

        [HttpPost("/create")]  // not used in app
        public async Task<IActionResult> CreateParking([FromBody] CreateParking parking) //not used in app
        {
            var a = await services.CreateParking(parking);

            return Ok(a);
        }

        [HttpPut("{id}")]  // not used
        public async Task<IActionResult> UpdateParking(int id, [FromBody] Parking parking) //not used in app
        {
            
            await services!.UpdateParking(id, parking);

            return Ok();
        }

        [HttpDelete("{id}")]  //not used

        public async Task<IActionResult> DeleteParking(int id) //not used in app
        {
            var parking = await services.GetParking(id);

            if (parking == null)
            {
                return NotFound();
            }

            await services!.DeleteParking(id);

            return NoContent();
        }
    }
    class CoordinatesComparer
    {
        public double GetDistance(double userLongitude, double userLatitude, double parkingLongitude, double parkingLatitude)
        {
            var d1 = userLatitude * (Math.PI / 180.0);
            var num1 = userLongitude * (Math.PI / 180.0);
            var d2 = parkingLatitude * (Math.PI / 180.0);
            var num2 = parkingLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
} 


