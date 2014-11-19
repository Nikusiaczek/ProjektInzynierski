using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LoginService
{
    public class MyPrincipal : IPrincipal
    {
        private Identity _identity;
        public Identity Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public bool IsInRole(string role)
        {
            return _identity.Role.Contains(role);
        }
        #endregion
    }

}
