using LoginService;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjInzynierski
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private  AuthenticationService _authenticationService;
        private readonly DelegateCommand<object> _loginCommand;
        private string _username;
        private string _status;
        private User _user;

        public LoginViewModel()
        {
            _authenticationService = new AuthenticationService();
            _loginCommand = new DelegateCommand<object>(Login, CanLogin);
        }

        public User CreatedUser
        {
            get
            {
                return _user;
            }
        }

        #region Properties
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return string.Format("Signed in as {0}. {1}",
                          Thread.CurrentPrincipal.Identity.Name,
                          Thread.CurrentPrincipal.IsInRole("Administrators") ? "You are an administrator!"
                              : "You are NOT a member of the administrators group.");

                return "Not authenticated!";
            }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }
        #endregion

        #region Commands
        public ICommand LoginCommand { get { return _loginCommand; } }

        #endregion

        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            try
            {
                //Validate credentials through the authentication service
                _user = _authenticationService.AuthenticateUser(Username, clearTextPassword);

                //Get the current principal object
                MyPrincipal principal = Thread.CurrentPrincipal as MyPrincipal;
                if (principal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");

                //Authenticate the user
                principal.Identity = new Identity(_user.Username, _user.Role);

                //Update UI
                NotifyPropertyChanged("AuthenticatedUser");
                NotifyPropertyChanged("IsAuthenticated");
                _loginCommand.RaiseCanExecuteChanged();
                //Username = string.Empty; //reset
                //passwordBox.Password = string.Empty; //reset
                //Status = string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Login failed! Please provide some valid credentials.";
            }
            catch (Exception ex)
            {
                Status = string.Format("ERROR: {0}", ex.Message);
            }
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        public bool IsAuthenticated
        {
            get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
