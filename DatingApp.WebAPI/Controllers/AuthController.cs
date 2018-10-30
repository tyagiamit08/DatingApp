using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.WebAPI.Data;
using DatingApp.WebAPI.DTOs;
using DatingApp.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserToRegisterDto userToRegisterDto)
        {
            userToRegisterDto.UserName = userToRegisterDto.UserName.ToLower();

            if (await _authRepository.UserExists(userToRegisterDto.UserName))
                return BadRequest("Username already exists");

            var userToRegister = new User
            {
                UserName = userToRegisterDto.UserName
            };

            var createdUser = await _authRepository.Register(userToRegister, userToRegisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _authRepository.Login(userForLoginDto.UserName, userForLoginDto.Password);

            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            //We need signing credentials so that we can assure that token is coming from the server
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
