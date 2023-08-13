using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using log4net;
using IBDataServerApp.ViewModels;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using FoxLearn.License;
using IBDataServerApp.Utils;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Data;

namespace IBDataServerApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string logPath;
        public static string configPath;
        public static string alarmConfigPath;
        public static string json;
        public static JObject configJsonObj;
        public static JObject alarmsConfigJsonObj;
        public static string mqttBrokerIp;
        public static bool st = true;
        public static string connectionString = string.Empty;
        public static string connectionString1 = string.Empty;
        public static string connectionString2 = string.Empty;
        public static string alarmReportConnectionString = string.Empty;
        public static string Hourly_Data_columns = string.Empty;
        public static string exportPath = string.Empty;
        public static string ViewReportPath = string.Empty;
        public static string ReportTemplatePath = string.Empty;
        static string clientId;
        public static JObject JCredentials;
        public static JObject JTolerance;
        public static string loggedUserName;
        public static string loggedEmail;

        public static DataTable Schedules;
        public static DataTable TodaysSchedules;
        public static DataTable Emails;
        public static JToken Reports;
        public static double ahToll = 5;
        public static double voltageToll = 5;
        public static double currentToll = 5;
        public MainWindow()
        {
            try
            {
                Log.Info("MainWindow");
                if (!LicenseHelper.IsValid())
                {
                    MessageBox.Show("Application not licensed please buy licence");
                    InitializeComponent();
                    System.Windows.Application.Current.MainWindow.Close();
                    return;
                }
                Log.Info("5");
                InitializeComponent();
                Log.Info("6");
                configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
                Log.Info("7");
                ReportTemplatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportTemplate");
                Log.Info("8");
                ViewReportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ViewReport");
                Log.Info("9");
                configJsonObj = JObject.Parse(File.ReadAllText(configPath, Encoding.UTF8));
                Log.Info("10");
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\credentials.key"))
                {

                    string decodedString = EncodeDecode.DecodeFrom64(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\credentials.key"));
                    JCredentials = JObject.Parse(decodedString);
                }
                else
                {
                    JCredentials = new JObject();
                }
                Log.Info("11");
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\tolerance.key"))
                {
                    string decodedString = EncodeDecode.DecodeFrom64(System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\tolerance.key"));
                    JTolerance = JObject.Parse(decodedString);
                    MainWindow.ahToll = Convert.ToDouble(MainWindow.JTolerance["ahToll"].ToString());
                    MainWindow.voltageToll = Convert.ToDouble(MainWindow.JTolerance["voltageToll"].ToString());
                    MainWindow.currentToll = Convert.ToDouble(MainWindow.JTolerance["currentToll"].ToString());
                }
                else
                {
                    Log.Info("12");
                    JTolerance = new JObject();
                }
                foreach (var configKey in configJsonObj.Properties())
                {
                    if (configKey.Name == "Connection")
                    {
                        connectionString = configKey.Value["ConnectionString"].ToString();
                        connectionString1 = JObject.Parse(configKey.Value.ToString())["ConnectionString1"].ToString();
                        connectionString2 = JObject.Parse(configKey.Value.ToString())["ConnectionString2"].ToString();
                        alarmReportConnectionString = JObject.Parse(configKey.Value.ToString())["AlarmReportConnectionString"].ToString();
                        
                    }
                    if (configKey.Name == "ExportFolder")
                    {
                        exportPath = configKey.Value.ToString();
                        if (!System.IO.Directory.Exists(exportPath))
                            System.IO.Directory.CreateDirectory(exportPath);
                    }
                    if (configKey.Name == "Reports")
                    {
                        Reports = configKey.Value;
                    }
                }


                Schedules = SQLHelper.GetSchedulerData(connectionString1);
                TodaysSchedules = SQLHelper.GetTodaysSchedulerData(connectionString1);
                Emails = SQLHelper.GetAutoEmailData(connectionString1);

                ////Scheduler
                string credentialError = string.Empty;
                string refreshTocket = string.Empty;
             Log.Info("Gmail service started");
                //string to = "mansimor2094@gmail.com";
                string to = "mishraashutosh1990@gmail.com;ashutosh.infobench@gmail.com";
                string from = "automailschedule@gmail.com";
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.AddMinutes(3).Minute;

                MyScheduler.IntervalInDays(00, 02, 1,
                () =>
                {

                    ReportGenerationHelper.autoExportedReportNames = String.Empty;

                    if (Emails != null && Emails.Rows.Count > 0)
                    {
                        if (!(Schedules != null && Schedules.Rows.Count > 0))
                            return;
                        foreach (DataRow dtRow in Schedules.Rows)
                        {// On all tables' columns
                        string SelectedReport = string.Empty;
                            DateTime AvailableFrom = DateTime.Now.AddYears(-1);
                            DateTime AvailableTo = DateTime.Now;
                            string BatchFilter = String.Empty;
                            string ModelFilter = string.Empty;
                            string ReportCategory = string.Empty;
                            foreach (DataColumn dc in Schedules.Columns)
                            {
                                switch (dc.ToString())
                                {
                                    case "ReportType":
                                        SelectedReport = dtRow[dc].ToString();
                                        break;
                                    case "ReportCategory":
                                        ReportCategory = dtRow[dc].ToString();
                                        break;
                                    case "BatchNumber":
                                        BatchFilter = dtRow[dc].ToString();
                                        break;
                                    case "ModelNumber":
                                        ModelFilter = dtRow[dc].ToString();
                                        break;
                                    default:
                                        break;
                                }

                            }
                            ReportGenerationHelper.LoadReports(ReportCategory,SelectedReport, AvailableFrom, AvailableTo, BatchFilter, ModelFilter);
                        }
                    }
                });

                bool alreadyWorking = false;
                MyScheduler.IntervalInMinutes(hour, minute, 2,
                () =>
                {
                    if (alreadyWorking == true)
                        return;
                    bool ReportGenerated = false;
                    if (Emails != null && Emails.Rows.Count > 0)
                    {
                        DataTable todaysSchedules = TodaysSchedules;
                        if (!(todaysSchedules != null && todaysSchedules.Rows.Count > 0))
                            return;

                        string SelectedReport = string.Empty;
                        DateTime AvailableFrom = DateTime.Now.AddYears(-1);
                        DateTime AvailableTo = DateTime.Now;
                        DateTime ScheduleDateTime = DateTime.Now;
                        string BatchNumber = String.Empty;
                        string ModelNumber = string.Empty;
                        string ReportCategory = string.Empty;
                        int SrNo = 0;
                        foreach (DataRow dtRow in todaysSchedules.Rows)
                        {// On all tables' columns
                        alreadyWorking = true;
                            foreach (DataColumn dc in todaysSchedules.Columns)
                            {
                                switch (dc.ToString())
                                {
                                    case "ReportType":
                                        SelectedReport = dtRow[dc].ToString();
                                        break;
                                    case "ReportCategory":
                                        ReportCategory = dtRow[dc].ToString();
                                        break;
                                    case "AvailableFrom":
                                        AvailableFrom = Convert.ToDateTime(dtRow[dc].ToString());
                                        break;
                                    case "AvailableTo":
                                        AvailableTo = Convert.ToDateTime(dtRow[dc].ToString());
                                        break;
                                    case "ScheduleDateTime":
                                        ScheduleDateTime = Convert.ToDateTime(dtRow[dc].ToString());
                                        break;
                                    case "SrNo":
                                        SrNo = Convert.ToInt16(dtRow[dc].ToString());
                                        break;
                                    case "ModelNumber":
                                        if (dtRow[dc] != null)
                                            ModelNumber = dtRow[dc].ToString();
                                        break;
                                    case "BatchNumber":
                                        if (dtRow[dc] != null)
                                            BatchNumber = dtRow[dc].ToString();
                                        break;
                                    default:
                                        break;
                                }

                            }
                            int tiime = DateTime.Compare(ScheduleDateTime, DateTime.Now);
                            if (tiime == -1)
                            {
                                ReportGenerationHelper.LoadReports(ReportCategory,SelectedReport, AvailableFrom, AvailableTo, BatchNumber, ModelNumber);
                                AvailableFrom = AvailableFrom.AddDays(1);
                                AvailableTo = AvailableTo.AddDays(1);
                                ScheduleDateTime = ScheduleDateTime.AddDays(1);
                                SQLHelper.UpdateTodaysSchedulerDataFromScheduler(MainWindow.connectionString1, ReportCategory, SelectedReport, BatchNumber, ModelNumber, AvailableFrom, AvailableTo, ScheduleDateTime, SrNo);
                                ReportGenerated = true;
                            }
                        }
                        if (ReportGenerated == true)
                        {

                            ReportGenerationHelper.autoExportedReportNames = String.Empty;

                            TodaysSchedules = SQLHelper.GetTodaysSchedulerData(MainWindow.connectionString1);

                            ReportGenerated = false;
                        }
                        alreadyWorking = false;
                    }
                });

            }
            catch (Exception ex)
            {
                Log.Info("Err Startup", ex);
            }
        }
        private void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*
                if (((MainViewModel)(this.DataContext)).Data.IsModified)
                if (!((MainViewModel)(this.DataContext)).PromptSaveBeforeExit())
                {
                    e.Cancel = true;
                    return;
                }
            */
            Log.Info("Closing App");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new ManageReportViewModel();
        }

        //private void sendMail()
        //{
        //    var fromAddress = new MailAddress("ashutosh.infobench@gmail.com", "From Name");
        //    var toAddress = new MailAddress("mishraashutosh1990@gmail.com", "To Name");
        //    const string fromPassword = "ashutosh@infobench";
        //    const string subject = "Subject";
        //    const string body = "Body";
        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };
        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    { Subject = subject, Body = body })
        //    { smtp.Send(message); }
        //}

    }
}
