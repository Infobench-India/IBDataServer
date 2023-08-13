using IBDataServerApp.Utils;
using IBDataServerApp.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace IBDataServerApp.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        public ChangePasswordViewModel()
        {
            ResetCommand = new RelayCommand<object>(resetUser);
        }
        #region Properies
     
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
        public ICommand ResetCommand { get; set; }
        #endregion
        
        private void resetUser(object values)
        {
            ChangeCommandParameters parameters = new ChangeCommandParameters();
            foreach (var obj in (object[])values)
            {
                var passwordBox = obj as PasswordBox;
                var password = passwordBox.Password;
                if (passwordBox.Name == "passwordBox1")
                    parameters.Password = password;
                if (passwordBox.Name == "passwordBoxConfirm")
                    parameters.ConfirmPassword = password;
                if (passwordBox.Name == "oldPassword")
                    parameters.OldPassword = password;
            }

            if (parameters.Password != null && parameters.Password != parameters.ConfirmPassword)
            {

                Error = "Password Mismatch";
                return;
            }
            //if (Email != null && !Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            //{
            //    Error = "Invalid Email";
            //    return;
            //}
            if (!String.IsNullOrEmpty(Email) && MainWindow.JCredentials.ContainsKey(Email))
            {
                JToken userValue = MainWindow.JCredentials.GetValue(Email);
                if (parameters.OldPassword != null && parameters.OldPassword != (userValue as JObject).GetValue("Password").ToString())
                {
                    Error = "Password Mismatch";
                    return;
                }
                MainWindow.JCredentials.Remove(Email);
                MainWindow.JCredentials.Add(new JProperty(Email,
                new JObject(
                    new JProperty("FirstName", (userValue as JObject).GetValue("FirstName").ToString()),
                    new JProperty("LastName", (userValue as JObject).GetValue("LastName").ToString()),
                    new JProperty("Email", Email),
                    new JProperty("Password", parameters.Password))));

                string encoded = EncodeDecode.EncodePasswordToBase64(MainWindow.JCredentials.ToString());

                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\credentials.key", encoded);
                Error = "Change Password Successfully";
                Email = string.Empty;
                foreach (var obj in (object[])values)
                {
                    var passwordBox = obj as PasswordBox;
                    passwordBox.Password = string.Empty;
                }

                }
            else
            {
                Error = "User Not Exists";
            }

        }
    }
}