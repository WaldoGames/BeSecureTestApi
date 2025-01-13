namespace BeSecureTestApi.Dto
{
    public class UserActiveDto : AccessTokenDto
    {
        public string username { get; set; }
        public bool is_active { get; set; }
    }
}
