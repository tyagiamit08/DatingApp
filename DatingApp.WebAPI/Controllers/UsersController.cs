using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.WebAPI.Data;
using DatingApp.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
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
    }
}
