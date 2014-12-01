using System.Security.Principal;

namespace LoginService
{
    public class Identity : IIdentity
    {
        public string Name { get; private set; }
        public string Role { get; private set; }
        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }

        public Identity(string name, string role)
        {
            Name = name;
            Role = role;
        }
        public string AuthenticationType { get { return "Custom authentication"; } }
    }
}
