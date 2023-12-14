using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using user.Infrastructure.Extensions;
using user.Data.Entities;
using user.Infrastructure.Helpers;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [Authorize]
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly Mapper _mapper;

        public UserController(
            UserManager<User> userManager,
            Mapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var user = await _userManager.GetUserAsync(User);


            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.GetError());
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.Name = request.Name;
            user.Surname = request.Surname;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.GetError());
        }
        [HttpGet("/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.MapToUserDto(user));
        }
        [HttpGet("id")]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
