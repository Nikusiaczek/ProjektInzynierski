using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace LoginService
{

    public interface IAuthenticationService
    {
        User AuthenticateUser(string username, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        protected class InternalUserData
        {
            public InternalUserData(string username, string hashedPassword, string role)
            {
                Username = username;
                HashedPassword = hashedPassword;
                Role = role;
            }
            public string Username { get; private set;}

            public string HashedPassword { get; private set;}

            public string Role { get; private set;}

        }

        private List<InternalUserData> _users = new List<InternalUserData>();

        public User AuthenticateUser(string username, string clearTextPassword)
        {
            RetrieveUsers();
            InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
                && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));
            if (userData == null)
                throw new UnauthorizedAccessException("Odmowa dostępu. Proszę podać właściwe dane.");

            return new User(userData.Username, userData.Role);
        }

        private string CalculateHash(string clearTextPassword, string salt)
        {
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            return Convert.ToBase64String(hash);
        }

        public void RetrieveUsers()
        {
            string connectionString = "Data Source=AGALAP;Initial Catalog=\"D:\\PROJEKTY VISUAL\\BD\\USERSDATA.MDF\";Integrated Security=True";
            //string connectionString = @"Server=(localdb)\V11;Database=UsersData;Trusted_Connection=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT UserName, Password, Role FROM [UserData]", connection))
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string UserPassword = Convert.ToString(reader["Password"]);
                    string UserName = Convert.ToString(reader["UserName"]);
                    string UserRole = Convert.ToString(reader["Role"]);
                    InternalUserData user = new InternalUserData(UserName, UserPassword, UserRole);
                    _users.Add(user);
                }
                connection.Close();
            }
        }
    }

    public class User
    {
        public User(string username, string role)
        {
            Username = username;
            Role = role;
        }
        public string Username { get; set;}

        public string Role { get; set;}

    }
}
