using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace One.Models.Users.Security
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public List<Client> Clients { get; set; }
    }

    public class Client
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
