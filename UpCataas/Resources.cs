using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace UpCataas
{
    public class Account
    {
        public string UserName { get; set; }
        public SecureString PasswordHash { get; set; }
    }

    public class Login
    {
        public string UserName { get; set; }
        public SecureString Password { get; set; }
    }
}
