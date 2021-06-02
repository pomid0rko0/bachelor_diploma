using System.Collections.Generic;

namespace Database.Domain
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public ICollection<string> Errors { get; set; }
    }
}