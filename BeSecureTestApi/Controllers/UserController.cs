using BeSecureTestApi.Dto;
using BeSecureTestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        public async Task<IActionResult> Get(string accessToken)
        {


            var (t, status) = await us.GetUsers(accessToken);
            if(status == System.Net.HttpStatusCode.Unauthorized || status == System.Net.HttpStatusCode.Forbidden){
                return Unauthorized();
            }
            var jsonArray = JsonSerializer.Deserialize<List<UsersDto>>(t.ToString()); // Replace `object` with a specific type if known
            // 
            //return Ok(jsonArray.Where(u=>u.pk != 3 && u.pk != 4));
            return Ok(jsonArray);
        }

        [HttpPost]//
        public async Task<IActionResult> AddUser(NewUserDto newUser)
        {

            var r1 = await us.CreateUserAsync(newUser);

            if(r1.Item2 == HttpStatusCode.Forbidden||r1.Item2 == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }

            await us.CreatePassword(newUser.password, newUser.username, newUser.accessToken);
            return Ok();
        }

        [HttpPut("/active")]//
        public async Task<IActionResult> ChanceActive(UserActiveDto User)
        {
            if(User.username== "akadmin" || User.username.Contains("ak-outpost"))
            {
                return BadRequest();
            }

            (UsersDto?,HttpStatusCode) user= await us.getUserByName(User.username, User.accessToken);

            if (user.Item2 == HttpStatusCode.Forbidden || user.Item2 == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }

            if (user.Item1==null)
            {
                return NotFound();
            }

            us.ChangeUserActiveStatus(User);

            return Ok();
        }
    }
}
