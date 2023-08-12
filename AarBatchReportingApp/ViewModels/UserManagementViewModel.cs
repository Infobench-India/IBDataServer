using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AarBatchReportingApp.Utils;

namespace AarBatchReportingApp.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        #region Constructor
        public UserManagementViewModel()
        {
             selectedViewModel = new LogInViewModel();
            LogInViewVisibility = Visibility.Visible;
            //selectedViewModel = new UserRegistrationViewModel();
            //UserRegistrationViewVisibility = Visibility.Visible;
            LoginCommand = new RelayCommand<object>(LogInView);
            ChangePassCommand = new RelayCommand<object>(LoadChangePasswordView);
            RegistrationCommand = new RelayCommand<object>(UserRegistration);
            if (((MainViewModel)App.app.DataContext).LoggedUserName == "admin")
            {
                AdminVisibility = Visibility.Visible;
            }
        }
        #endregion

        #region Properties
        private Visibility logInViewVisibility = Visibility.Visible;
        private Visibility userRegistrationViewVisibility = Visibility.Collapsed;
        private object selectedViewModel;

        public Visibility LogInViewVisibility
        {
            get { return logInViewVisibility; }
            set { logInViewVisibility = value; NotifyPropertyChanged(); }
        }

        public Visibility UserRegistrationViewVisibility
        {
            get { return userRegistrationViewVisibility; }
            set { userRegistrationViewVisibility = value; NotifyPropertyChanged(); }
        }

        public object SelectedViewModel

        {

            get { return selectedViewModel; }

            set { selectedViewModel = value; NotifyPropertyChanged(); }

        }

        private Visibility adminVisibility = Visibility.Collapsed;
        public Visibility AdminVisibility
        {
            get { return adminVisibility; }
            set { adminVisibility = value; NotifyPropertyChanged(); }
        }


        #endregion

        #region command
        public ICommand LoginCommand { get; set; }
        public ICommand ChangePassCommand { get; set; }
        public ICommand RegistrationCommand { get; set; }
        #endregion

        public void UserRegistration(object credential)
        { 
            SelectedViewModel = new UserRegistrationViewModel();
            UserRegistrationViewVisibility = System.Windows.Visibility.Visible;
        }

        public void LogInView(object obj)
        {
            SelectedViewModel = new LogInViewModel();
            UserRegistrationViewVisibility = System.Windows.Visibility.Visible;
        }

        public void LoadChangePasswordView(object obj)
        {
            SelectedViewModel = new ChangePasswordViewModel();
            UserRegistrationViewVisibility = System.Windows.Visibility.Visible;
        }
    }

}
