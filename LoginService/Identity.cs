using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace LoginService
{
    class Identity : IIdentity
    {
        public string Name { get; private set; }
        public string[] Roles { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }
        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion

        public Identity(string name, string[] roles)
        {
            Name = name;
            Roles = roles;
        }
    }
}
