using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
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
        public async Task<IActionResult> Register(UserRegistrationDto userRegistrationDto)
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
    }
}