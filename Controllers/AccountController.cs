using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klustr_api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public AccountController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userRepo.UserExists(userRegistrationDto.Email!))
            {
                return BadRequest("Email is already taken.");
            }

            var token = await _userRepo.Register(userRegistrationDto);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userRepo.Login(userLoginDto);
            if (token == null)
            {
                return Unauthorized("Invalid Email or Password");
            }

            return Ok(new { token });
        }

        [HttpPost("google-auth")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDto googleAuthDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userRepo.GoogleAuth(googleAuthDto);
            if (token == null)
            {
                return Unauthorized("Invalid Google authentication data");
            }

            return Ok(new { token });
        }
        [HttpGet("findByEmail")]
        public async Task<IActionResult> FindByEmail([FromQuery] string email)
        {
            var user = await _userRepo.FindByEmail(email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
    }
}