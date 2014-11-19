using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace LoginService
{
    public class Identity : IIdentity
    {
        public Identity(string name, string role)
        {
            Name = name;
            Role = role;
        }

        public string Name { get; private set; }
        public string Role { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }
        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion


    }
}
