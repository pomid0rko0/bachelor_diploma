namespace Database.Domain
{
    public class AuthResult
    {
        public string Token { get; set; }
        public int Expires { get; set; }
    }
}