using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IBDataServerApp.Utils;
using IBDataServerApp.Views;

namespace IBDataServerApp.ViewModels
{
    public class LogInViewModel:ViewModelBase
    {
        #region Parameters
        private bool _isAuthenticated;
        public bool isAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                if (value != _isAuthenticated)
                {
                    _isAuthenticated = value;
                    NotifyPropertyChanged("isAuthenticated");
                }
            }
        }
        private string _username;
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyPropertyChanged("UserName");
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
            }
        }
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage ; }
            set { errorMessage = value; NotifyPropertyChanged(); }
        }

        private string userType;

        public string UserType
        {
            get { return userType; }
            set { userType = value; NotifyPropertyChanged(); }
        }
      
        #endregion
        #region Constructors
        public LogInViewModel()
        {
            // DialogService is used to handle dialogs
            LoginCommand = new RelayCommand<object>(Login);
            
        }

        #endregion

        #region command
        public ICommand LoginCommand { get; set; }
        #endregion        

        public void Login(object credential)
        {
            var passwordBox = credential as PasswordBox;
            var password = passwordBox.Password;
           if(!String.IsNullOrEmpty(UserName) && MainWindow.JCredentials.ContainsKey(UserName))
            {
                if (MainWindow.JCredentials[UserName]["Password"].ToString() == password)
                {
                    isAuthenticated = true;
                    ((MainViewModel)App.app.DataContext).SelectedViewModel = null;
                    ((MainViewModel)App.app.DataContext).LogInViewVisibility = Visibility.Collapsed;
                    ((MainViewModel)App.app.DataContext).MainViewVisibility = Visibility.Visible;
                    ((MainViewModel)App.app.DataContext).SelectedViewModel = ((MainViewModel)App.app.DataContext).ToleranceSettingViewModel;
                    ((MainViewModel)App.app.DataContext).LoggedUserName = UserName;
                    ((MainViewModel)App.app.DataContext).IsLoggedUser = Visibility.Visible;
                    if(UserName=="admin")
                    {
                        ((MainViewModel)App.app.DataContext).IsLoggedAdmin = Visibility.Visible;
                    }
                }
                else
                    ErrorMessage = "WARNING OF PASSWORD OR EMAIL ID NOT CORRECT ";
            }
            else
                ErrorMessage = "WARNING OF PASSWORD OR EMAIL ID NOT CORRECT ";
            //TODO check username and password vs database here.
            //If using membershipprovider then just call Membership.ValidateUser(UserName, Password)
            if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                isAuthenticated = true;
        }

    }
}
