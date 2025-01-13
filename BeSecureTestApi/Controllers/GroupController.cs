using BeSecureTestApi.Dto;
using BeSecureTestApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BeSecureTestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {

        private readonly ILogger<GroupController> _logger;
        GroupService gs;
        public GroupController(ILogger<GroupController> logger)
        {
            gs = new GroupService();
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> Get(string accesstoken)
        {


            (object, HttpStatusCode) t = await gs.GetGroups(accesstoken);
            if (t.Item2 == HttpStatusCode.Forbidden || t.Item2==HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            var jsonArray = JsonSerializer.Deserialize<List<GroupsDto>>(t.Item1.ToString()); // Replace `object` with a specific type if known


            return Ok(jsonArray);
        }
    }
}
