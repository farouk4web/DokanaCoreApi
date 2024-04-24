namespace Dokana.DTOs.Account
{
    public class NewRefreshTokenDto
    {
        public string Token { get; set; }

        public DateTime ExpireDateTime { get; set; }
    }
}
