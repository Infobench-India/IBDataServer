using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using AarBatchReportingApp.Utils;
using AarBatchReportingApp.Views;
using Newtonsoft.Json.Linq;

namespace AarBatchReportingApp.ViewModels
{
    public class UserRegistrationViewModel : ViewModelBase
    {
        public UserRegistrationViewModel()
        {
            SubmitCommand = new RelayCommand<object>(submitUser);
            ResetCommand = new RelayCommand<object>(resetUser);
        }
        #region Properies
        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; NotifyPropertyChanged(); }
        }

        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; NotifyPropertyChanged(); }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; NotifyPropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; NotifyPropertyChanged(); }
        }

        private string confirmPassword;

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value; Error = String.Empty; NotifyPropertyChanged(); }
        }

        private string error;

        public string Error
        {
            get { return error; }
            set { error = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region Commands
        public ICommand SubmitCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion
        private void submitUser(object values)
        {
            FindCommandParameters parameters = new FindCommandParameters();
            foreach (var obj in (object[])values)
            {
                var passwordBox = obj as PasswordBox;
                var password = passwordBox.Password;
                if (passwordBox.Name == "passwordBox1")
                    parameters.Password = password;
                if (passwordBox.Name == "passwordBoxConfirm")
                    parameters.ConfirmPassword = password;
            }

            if (parameters.Password != null && parameters.Password != parameters.ConfirmPassword)
            {

                Error = "Password Mismatch";
                return;
            }
            if (string.IsNullOrEmpty(Email))
            {
                Error = "Invalid Email";
                return;
            }
            if (Email != null && !Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                Error = "Invalid Email";
                return;
            }
            if (MainWindow.JCredentials.ContainsKey(Email))
            {
                Error = "Already Exist";
                return;
            }
            MainWindow.JCredentials.Add(new JProperty(Email,
                new JObject(
                    new JProperty("FirstName", FirstName),
                    new JProperty("LastName", LastName),
                    new JProperty("Email", Email),
                    new JProperty("Password", parameters.Password))));
            string encoded = EncodeDecode.EncodePasswordToBase64(MainWindow.JCredentials.ToString());

            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\credentials.key", encoded);
            Error = "Created Successfully";
            Email = string.Empty;
            foreach (var obj in (object[])values)
            {
                var passwordBox = obj as PasswordBox;
                passwordBox.Password = string.Empty;
            }
        }

        private void resetUser(object values)
        {
            FindCommandParameters parameters = new FindCommandParameters();
            foreach (var obj in (object[])values)
            {
                var passwordBox = obj as PasswordBox;
                var password = passwordBox.Password;
                if (passwordBox.Name == "passwordBox1")
                    parameters.Password = password;
                if (passwordBox.Name == "passwordBoxConfirm")
                    parameters.ConfirmPassword = password;
            }

            if (parameters.Password!= null && parameters.Password != parameters.ConfirmPassword)
            {

                Error = "Password Mismatch";
                return;
            }
            if (Email != null && !Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                Error = "Invalid Email";
                return;
            }
            if (!String.IsNullOrEmpty(Email)&&MainWindow.JCredentials.ContainsKey(Email))
            {
                MainWindow.JCredentials.Remove(Email);
                MainWindow.JCredentials.Add(new JProperty(Email,
                new JObject(
                    new JProperty("FirstName", FirstName),
                    new JProperty("LastName", LastName),
                    new JProperty("Email", Email),
                    new JProperty("Password", parameters.Password))));
                string encoded = EncodeDecode.EncodePasswordToBase64(MainWindow.JCredentials.ToString());

                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\credentials.key", encoded);
                Error = "Reset User Details Successfully";
            }
            else
            {
                Error = "User Not Exists";
            }
            
        }
    }
}
