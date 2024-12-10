using Microsoft.Extensions.ObjectPool;

namespace BeSecureTestApi.Dto
{
    public class NewUserDto
    {
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string group { get; set; }
        public string password { get; set; }
    }
}
