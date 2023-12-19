using PublicParkingsSofiaWebAPI.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PublicParkingsSofiaWebAPI.Models;
using PublicParkingsSofiaWebAPI.Services;

namespace PublicParkingsSofiaWebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly ParkingServices services;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthManager _authManager;

        public UserController(ParkingServices _services, UserManager<IdentityUser> userManager, IAuthManager authmanager)
        {
            services = _services;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authManager = authmanager ?? throw new ArgumentNullException(nameof(authmanager));
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            var users = await services.GetUsers();
            return Ok(users);
        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await services.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpGet("byemail/{email}", Name = "GetUserByEmail")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await services.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            var a = await services.Registration(user);

            return Ok(a);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {

            await services!.UpdateUser(id, user);

            return Ok();
        }
        [HttpPut("returnCredits/{userId}/{amount}")]
        public async Task<IActionResult> AddCredits(int userId, double amount)
        {

            await services!.AddCredits(userId, amount);

            return Ok();
        }
        [HttpPut("removeCredits/{userId}/{amount}")]
        public async Task<IActionResult> RemoveCredits(int userId, double amount)
        {

            await services!.RemoveCredits(userId, amount);

            return Ok();
        }
        [HttpPut("removeAllCredits/{userId}")]
        public async Task<IActionResult> RemoveAllCredits(int userId)
        {

            await services!.RemoveAllCredits(userId);

            return Ok();
        }
        [HttpPut("payCash/{userId}")]
        public async Task<IActionResult> GiveUserCash(int userId)
        {

            await services!.GiveUserCash(userId);

            return Ok();
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var _user = new IdentityUser();
            _user.Email = user.Email;
            _user.UserName = user.Email;
            var results = await _userManager.CreateAsync(_user, user.Password);
            if (!results.Succeeded)
            {
                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }


            return Accepted();

        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {

            if (!await _authManager.ValidateUser(user))
            {
                return Unauthorized();
            }
            return Accepted(new { Token = await _authManager.CreateToken() });

        }

    }
    public class UserLogin
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
