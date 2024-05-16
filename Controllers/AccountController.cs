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
            if (token.Error != null)
            {
                return BadRequest(token.Error);
            }

            return Ok(new { token.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userRepo.Login(userLoginDto);
            if (token.Error != null)
            {
                return Unauthorized(token.Error);
            }

            return Ok(new { token.Token });
        }

        [HttpPost("google-auth")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDto googleAuthDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var res = await _userRepo.GoogleAuth(googleAuthDto);
            if (res.Error != null)
            {
                return Unauthorized(res.Error);
            }
            return Ok(new { res.Token });
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