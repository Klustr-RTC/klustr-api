using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
using Klustr_api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klustr_api.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class Usercontroller : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public Usercontroller(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    BadRequest(ModelState);
                }

                var userId = User.FindFirst("userId")?.Value!;

                if (await _userRepo.UserExists(updateUserDto.Email))
                {
                    BadRequest("Email already exists");
                }

                var user = await _userRepo.UpdateUserAsync(userId, updateUserDto);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user.ToUserDtoFromUser());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var user = await _userRepo.FindById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDtoFromUser());
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst("userId")?.Value!;
            var isDeleted = await _userRepo.DeleteAccountAsync(userId);
            if (!isDeleted)
            {
                return StatusCode(500, "Account not deleted");
            }

            return Ok(true);
        }

    }
}