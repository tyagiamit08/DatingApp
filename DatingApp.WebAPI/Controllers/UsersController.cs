using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.WebAPI.Data;
using DatingApp.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _autoMapper;

        public UsersController(IDatingRepository datingRepository, IMapper autoMapper)
        {
            _datingRepository = datingRepository;
            _autoMapper = autoMapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepository.GetUsers();
            var usersToReturn = _autoMapper.Map<IEnumerable<UsersListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _datingRepository.GetUser(userId);
            var userToReturn = _autoMapper.Map<UserDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, userToUpdateDTO userToUpdate)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepository.GetUser(userId);
            _autoMapper.Map(userToUpdate, userFromRepo);

            if (await _datingRepository.SaveAll())
                return NoContent();

            throw new Exception($"Updating user with Id { userId } failed");
        }
    }
}
