using MvvmDialogs;
using log4net;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using IBDataServerApp.Views;
using IBDataServerApp.Utils;
using System.Windows;

namespace IBDataServerApp.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Parameters
        private readonly IDialogService DialogService;
        private Visibility logInViewVisibility = Visibility.Visible;
        private Visibility mainViewVisibility = Visibility.Collapsed;
        private ToleranceSettingViewModel toleranceSettingViewModel = new ToleranceSettingViewModel();
        private object selectedViewModel;
        /// <summary>
        /// Title of the application, as displayed in the top bar of the window
        /// </summary>
        public string Title
        {
            get { return "IBDataServerApp"; }
        }

        public Visibility LogInViewVisibility
        {
            get { return logInViewVisibility; }
            set { logInViewVisibility = value; NotifyPropertyChanged("LogInViewVisibility"); }
        }
        public ToleranceSettingViewModel ToleranceSettingViewModel
        {
            get { return toleranceSettingViewModel; }
            set { toleranceSettingViewModel = value; NotifyPropertyChanged("ToleranceSettingViewModel"); }
        }
        
        public Visibility MainViewVisibility
        {
            get { return mainViewVisibility; }
            set { mainViewVisibility = value; NotifyPropertyChanged("MainViewVisibility"); }
        }

        private Visibility schedulerVisibility;

        public Visibility DailySchedulerVisibility
        {
            get { return schedulerVisibility; }
            set { schedulerVisibility = value; NotifyPropertyChanged(); }
        }

        private Visibility todaysSchedulerVisibility;

        public Visibility TodaysSchedulerVisibility
        {
            get { return todaysSchedulerVisibility; }
            set { todaysSchedulerVisibility = value; NotifyPropertyChanged(); }
        }

        private Visibility autoEmailVisibility;

        public Visibility AutoEmailVisibility
        {
            get { return autoEmailVisibility; }
            set { autoEmailVisibility = value; NotifyPropertyChanged(); }
        }

        private Visibility toleranceSettingVisibility;

        public Visibility ToleranceSettingVisibility
        {
            get { return toleranceSettingVisibility; }
            set { toleranceSettingVisibility = value; NotifyPropertyChanged(); }
        }


        private Visibility alarmReportsVisibility;

        public Visibility AlarmReportsVisibility
        {
            get { return alarmReportsVisibility; }
            set { alarmReportsVisibility = value; NotifyPropertyChanged(); }
        }

        public object SelectedViewModel

        {

            get { return selectedViewModel; }

            set { selectedViewModel = value; NotifyPropertyChanged(); }

        }


        private string loggedUser;

        public string LoggedUserName
        {
            get { return loggedUser; }
            set { loggedUser = value; NotifyPropertyChanged(); }
        }

        private Visibility isLoggedUser = Visibility.Collapsed;

        public Visibility IsLoggedUser
        {
            get { return isLoggedUser; }
            set { isLoggedUser = value; NotifyPropertyChanged(); }
        }

        private Visibility isLoggedAdmin = Visibility.Collapsed;

        public Visibility IsLoggedAdmin
        {
            get { return isLoggedAdmin; }
            set { isLoggedAdmin = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            // DialogService is used to handle dialogs
            this.DialogService = new MvvmDialogs.DialogService();
            SwitchGenerateReportViewCommand = new RelayCommand<object>(OpenGenerateReportView);
            SwitchManageReportViewCommand = new RelayCommand<object>(OpenManageReportView);
            SwitchLogInViewCommand = new RelayCommand<object>(OpenLogInView);
            SwitchScheduleReportCommand = new RelayCommand<object>(OpenScheduleReportView);
            SwitchDailyScheduleCommand = new RelayCommand<object>(SwitchDailyScheduleCommandView);
            SwitchTodayScheduleCommand = new RelayCommand<object>(SwitchTodayScheduleCommandView);
            SwitchAutoEmailCommand = new RelayCommand<object>(SwitchAutoEmailCommandView);
            ToleranceViewLoadCommand = new RelayCommand<object>(ToleranceSettingView);
            AlarmReportsViewCommand = new RelayCommand<object>(AlarmReportsView);
            if (!string.IsNullOrEmpty(MainWindow.loggedUserName))
            IsLoggedUser = Visibility.Visible;
            
        }

        #endregion

        #region Methods
        #endregion

        #region Commands
        public RelayCommand<object> SampleCmdWithArgument { get { return new RelayCommand<object>(OnSampleCmdWithArgument); } }

        public ICommand SaveAsCmd { get { return new RelayCommand(OnSaveAsTest, AlwaysTrue); } }
        public ICommand SaveCmd { get { return new RelayCommand(OnSaveTest, AlwaysTrue); } }
        public ICommand NewCmd { get { return new RelayCommand(OnNewTest, AlwaysTrue); } }
        public ICommand OpenCmd { get { return new RelayCommand(OnOpenTest, AlwaysTrue); } }
        public ICommand ShowAboutDialogCmd { get { return new RelayCommand(OnShowAboutDialog, AlwaysTrue); } }
        public ICommand ExitCmd { get { return new RelayCommand(OnExitApp, AlwaysTrue); } }

        public ICommand LogOutCommand { get { return new RelayCommand(LogOutApp); } }
        public GenerateReportViewModel DataContext { get; set; } = new GenerateReportViewModel();

        private bool AlwaysTrue() { return true; }
        private bool AlwaysFalse() { return false; }

        public ICommand SwitchGenerateReportViewCommand { get; set; }

        public ICommand SwitchManageReportViewCommand { get; set; }
        public ICommand SwitchLogInViewCommand { get; set; }
        public ICommand  SwitchScheduleReportCommand { get; set; }
        public ICommand SwitchDailyScheduleCommand { get; set; }
        public ICommand SwitchTodayScheduleCommand { get; set; }
        public ICommand SwitchAutoEmailCommand { get; set; }
        public ICommand ToleranceViewLoadCommand { get; set; }

        public ICommand AlarmReportsViewCommand { get; set; }
        private void OnSampleCmdWithArgument(object obj)
        {
            // TODO
        }

        private void OnSaveAsTest()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "Save As",
                Filter = "Sample (.xml)|*.xml",
                CheckFileExists = false,
                OverwritePrompt = true
            };

            bool? success = DialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                // Do something
                Log.Info("Saving file: " + settings.FileName);
            }
        }
        private void OnSaveTest()
        {
            // TODO
        }
        private void OnNewTest()
        {
            DataContext =  new GenerateReportViewModel();
        }
        private void OnOpenTest()
        {
            var settings = new OpenFileDialogSettings
            {
                Title = "Open",
                Filter = "Sample (.xml)|*.xml",
                CheckFileExists = false
            };

            bool? success = DialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                // Do something
                Log.Info("Opening file: " + settings.FileName);
            }
        }
        private void OnShowAboutDialog()
        {
            Log.Info("Opening About dialog");
            AboutViewModel dialog = new AboutViewModel();
            var result = DialogService.ShowDialog<About>(this, dialog);
        }
        private void OnExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        private void LogOutApp()
        {
            ((MainViewModel)App.app.DataContext).LoggedUserName = null;
            ((MainViewModel)App.app.DataContext).IsLoggedUser = Visibility.Collapsed;
            ((MainViewModel)App.app.DataContext).SelectedViewModel = null;
        }

        private void OpenScheduleReportView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Visible;
            LogInViewVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = toleranceSettingViewModel;

        }

        private void SwitchDailyScheduleCommandView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Collapsed;
            LogInViewVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Visible;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = new SchedulerViewModel();

        }

        private void SwitchTodayScheduleCommandView(object obj)
        {
            MainViewVisibility = Visibility.Collapsed;
            LogInViewVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            TodaysSchedulerVisibility = Visibility.Visible;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = new TodaysSchedulerViewModel();

        }

        private void SwitchAutoEmailCommandView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Collapsed;
            LogInViewVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Visible;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;

        }

        private void ToleranceSettingView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Visible;
            LogInViewVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Visible;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = toleranceSettingViewModel;

        }


        private void OpenGenerateReportView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Visible;
            LogInViewVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = new GenerateReportViewModel();

        }

        private void AlarmReportsView(object obj)
        {
            TodaysSchedulerVisibility = Visibility.Collapsed;
            MainViewVisibility = Visibility.Collapsed;
            LogInViewVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Visible;
            SelectedViewModel = new AlarmReportsViewModel();

        }


        private void OpenManageReportView(object obj)
        {
            MainViewVisibility = Visibility.Visible;
            LogInViewVisibility = Visibility.Collapsed;
            TodaysSchedulerVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = new ManageReportViewModel();

        }

        private void OpenLogInView(object obj)
        {
            MainViewVisibility = Visibility.Collapsed;
            LogInViewVisibility = Visibility.Visible;
            TodaysSchedulerVisibility = Visibility.Collapsed;
            DailySchedulerVisibility = Visibility.Collapsed;
            AutoEmailVisibility = Visibility.Collapsed;
            ToleranceSettingVisibility = Visibility.Collapsed;
            AlarmReportsVisibility = Visibility.Collapsed;
            SelectedViewModel = new UserManagementViewModel();

        }
        #endregion

        #region Events

        #endregion
    }
}
