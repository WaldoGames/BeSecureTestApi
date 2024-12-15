using BeSecureTestApi.Dto;
using BeSecureTestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BeSecureTestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        UserServices us;
        public UserController(ILogger<GroupController> logger)
        {
            us = new UserServices();
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {


            var t = await us.GetUsers();
            var jsonArray = JsonSerializer.Deserialize<List<UsersDto>>(t.ToString()); // Replace `object` with a specific type if known
            // 
            return Ok(jsonArray.Where(u=>u.pk != 3 && u.pk != 4));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(NewUserDto newUser)
        {
            await us.CreateUserAsync(newUser);
            await us.CreatePassword(newUser.password, newUser.username);
            return Ok();
        }

        [HttpPut("/active")]
        public async Task<IActionResult> ChanceActive(UserActiveDto User)
        {
            UsersDto? user= await us.getUserByName(User.username);

            if (user==null)
            {
                return NotFound();
            }

            us.ChangeUserActiveStatus(User);

            return Ok();
        }
    }
}
