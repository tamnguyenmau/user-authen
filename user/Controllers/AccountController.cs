using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using user.Infrastructure.Extensions;
using user.Data.Entities;
using user.Infrastructure.Helpers;
using user.Models;
using user.Services;

namespace user.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly Mapper _mapper;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            Mapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _mapper = mapper;
        }
        [HttpPost("login")]
        [AllowAnonymous]
       
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    var date = DateTime.UtcNow;

                    var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, date.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                        };
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfiguration:SecurityKey"]));

                    var securityToken = new JwtSecurityToken(
                        issuer: _configuration["JwtConfiguration:Issuer"],  
                        audience: _configuration["JwtConfiguration:Audience"],
                        claims: claims,
                        notBefore: date,
                        expires: date.AddMinutes(60),
                        signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                    );


            
                    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
                

                    return Ok(new {token} );
                }
            }

            return BadRequest("Please try another email address or password.");
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
       
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return BadRequest("Please try another user name.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return Ok("Your password has been reset.");
            }

            return BadRequest();
        }
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest("Please try another email address.");
            }
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = UrlBuilder.ResetPasswordCallbackLink(code);
            await _emailSender.SendEmailAsync(request.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok("Please check your email to reset your password.");
        }

        [HttpGet("confirmemail")]
        [AllowAnonymous]
       
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string code)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for email confirm.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirming your email.");
            }

            return BadRequest();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                UserName = request.UserName,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = UrlBuilder.EmailConfirmationLink(user.Id, HttpUtility.UrlEncode(code));
                await _emailSender.SendEmailAsync(request.Email, "Confirm your email",
                    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                return Created($"User/{user.Id}", null);
            }

            return BadRequest(result.GetError());
        }
        

    }
}
