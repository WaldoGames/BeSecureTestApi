using BeSecureTestApi.Dto;
using BeSecureTestApi.Services;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get()
        {


            var t = await gs.GetGroups();
            var jsonArray = JsonSerializer.Deserialize<List<GroupsDto>>(t.ToString()); // Replace `object` with a specific type if known


            return Ok(jsonArray);
        }
    }
}
