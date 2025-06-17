using PLX5S.BUSINESS.Dtos.AD;

namespace PLX5S.BUSINESS.Dtos.Auth
{
    public class JWTTokenDto
    {
        public string? MessenDevice { get; set; }
        public string AccessToken { get; set; }

        public DateTime ExpireDate { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpireDateRefreshToken { get; set; }

        public AccountLoginDto AccountInfo { get; set; }
    }
}
