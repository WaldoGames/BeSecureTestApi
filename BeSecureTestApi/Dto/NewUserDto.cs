using Microsoft.Extensions.ObjectPool;

namespace BeSecureTestApi.Dto
{
    public class NewUserDto : AccessTokenDto
    {
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string groups { get; set; }
        public string password { get; set; }
    }
}
