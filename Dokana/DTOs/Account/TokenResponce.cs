namespace Dokana.DTOs.Account
{
    public class TokenResponce
    {
        public bool IsAuthenticated { get; set; }


        public string Token { get; set; }

        public DateTime TokenExpiresOn { get; set; }


        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiresOn { get; set; }



        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }



        public string Messages { get; set; }
    }

}