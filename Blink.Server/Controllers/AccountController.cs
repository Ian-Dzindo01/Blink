using Blink.Server.Models.DTO;
using Blink.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blink.Server.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid registration data." });
            }

            var success = await _accountService.RegisterAsync(model);
            if (success)
            {
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(new { message = "Registration failed." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid login data." });
            }

            var success = await _accountService.LoginAsync(model);
            if (success)
            {
                return Ok(new { message = "Login successful." });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }
    }
}
