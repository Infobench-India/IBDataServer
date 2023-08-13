using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBDataServerApp.Utils;
using IBDataServerApp.Views;

namespace IBDataServerApp.ViewModels
{
    public class GenerateReportViewModel : ViewModelBase
    {
        public GenerateReportViewModel()
        {
            ReportNameList = new List<string>();
            // 1 element
            string[] tankName = new string[2]{
                "Daily Report",
                "Hourly Report" };
            for (int i = 0; i < tankName.Length; i++)
            {
                ReportNameList.Add(tankName[i]);
            }
            ReportCategories = new List<string>();
            // 1 element
            string[] reportCategories = new string[2]{
                "TC",
                "PTED" };
            for (int i = 0; i < reportCategories.Length; i++)
            {
                ReportCategories.Add(reportCategories[i]);
            }
        }
        private DateTime _availableFrom = DateTime.Now.AddDays(-1);
        public DateTime AvailableFrom
        {
            get { return _availableFrom; }
            set { _availableFrom = value; NotifyPropertyChanged(); }
        }
        private DateTime _availableTo = DateTime.Now;
        public DateTime AvailableTo
        {
            get { return _availableTo; }
            set
            {
                _availableTo = value;
                NotifyPropertyChanged();
            }
        }
        private string _selectedReport;

        public string SelectedReport
        {
            get { return _selectedReport; }
            set { _selectedReport = value; NotifyPropertyChanged(); }
        }
        List<string> reportNameList = new List<string>();
        public List<string> ReportNameList
        {
            get { return reportNameList; }
            set { reportNameList = value; }
        }
        List<string> reportCategories = new List<string>();
        public List<string> ReportCategories
        {
            get { return reportCategories; }
            set { reportCategories = value; }
        }
        
        private void OnReportLoad()
        {
            DateTime fromDate = Convert.ToDateTime(AvailableFrom);
            //DateTime.TryParse(AvailableFrom,out fromDate);
            string toDate = fromDate.ToString("yyyy/MM/dd");
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection("Server=127.0.0.1;Port=5432;Username=postgres;Password=infobench;Database=BalPaintShop;Pooling=false;"))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        string qr = "hwr_temp,hwr_water_level,hwr_pressure," +
       "hwr_ph,pd_temp,pd_water_level,pd_pressure,pd_ph,d_temp," +
       "d_water_level,d_pressure,d_ph, wr1_water_level,wr1_pressure," +
       "wr1_ph,wr2_water_level,wr2_pressure,wr2_ph,rcdm1_water_level," +
       "rcdm1_pressure,rcdm1_conductivity, tectalis_water_level,tectalis_pressure," +
       "tectalis_conductivity,rcdm2_water_level,rcdm2_pressure,rcdm2_conductivity," +
       "rcdm3_water_level,rcdm3_pressure,rcdm3_conductivity,os_temp," +
       "hwg_pressure,hwg_temp,ov1_temp,ov2_temp";
                        string[] allColumnList = qr.Split(',');
                        string subquery = string.Empty;

                        if (string.IsNullOrEmpty(subquery))
                            return;
                        StrQuery100 = @"Select timestamp," + subquery.TrimEnd(' ').Trim(',') + " from pt_line_parameter_table WHERE timestamp >= '" + AvailableFrom.ToString() + "' AND timestamp <= '" + AvailableTo.ToString() + "' ";

                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();

                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.LocalReport.DataSources.Clear();
                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.LocalReport.DisplayName = "Report1";
                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.LocalReport.ReportPath = "PTLineReport.rdlc";
                        //ReportParameter[] parms = new ReportParameter[2];
                        //parms[0] = new ReportParameter("startDate", AvailableFrom.ToString());
                        //parms[1] = new ReportParameter("endDate", AvailableTo.ToString());

                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.LocalReport.SetParameters(parms);
                        //ReportDataSource rds = new ReportDataSource("DataSet1", dt100);
                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.LocalReport.DataSources.Add(rds);
                        //((MainWindow)System.Windows.Application.Current.MainWindow).reportViewer1.RefreshReport();
                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
        }

        #region Commands
        public RelayCommand<object> SampleCmdWithArgument { get { return new RelayCommand<object>(OnSampleCmdWithArgument); } }
        #endregion

        private void OnSampleCmdWithArgument(object obj)
        {
            
            //string exportingDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string authorsFile = "";
            //switch (SelectedReport)
            //{
            //    case "Daily Batch Report":
            //        authorsFile = "Batch Wise Daily  Report";
            //        break;
            //    case "Daily Report":
            //        authorsFile = "Daily Report";
            //        break;
            //    case "Hourly Report":
            //        authorsFile = "Hourly  Report";
            //        break;
            //    case "Sec Batch Report":
            //        authorsFile = "Batch Wise Sec Report";
            //        break;
            //    case "Graph Format":
            //        authorsFile = "GRAPH FORMAT";
            //        break;
            //    default:
            //        break;
            //}
            //object exportEcelPath = MainWindow.exportPath + "\\" + authorsFile + "" + exportingDateTime + ".xlsx";
            //object templatePath = ExportReportHelper.rootFolder + "\\" + authorsFile + ".xlsx";
            //switch (SelectedReport)
            //{
            //    case "Daily Batch Report":
            //        DataTable dt = SQLHelper.DailyBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString1, BatchFilter, ModelFilter);
            //        if (dt != null)
            //            ExportReportHelper.writeDatTableInPortRate(dt, templatePath.ToString(), exportEcelPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
            //        break;
            //    case "Daily Report":
            //        dt = SQLHelper.DailyReportLoad(AvailableFrom, AvailableTo, MainWindow.connectionString);
            //       if (dt != null)
            //            ExportReportHelper.writeDailyReportInLandscap(dt, templatePath.ToString(), exportEcelPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
            //        break;
            //    case "Hourly Report":
            //        dt = SQLHelper.HourlyReportLoad(AvailableFrom, AvailableTo, MainWindow.connectionString);
            //       if (dt!=null)
            //        ExportReportHelper.writeHourlyReportInLandscap(dt, templatePath.ToString(), exportEcelPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
            //        break;
            //    case "Sec Batch Report":
            //        dt = SQLHelper.SecBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
            //        if (dt != null)
            //            ExportReportHelper.writeDatTableInPortRate(dt, templatePath.ToString(), exportEcelPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
            //        break;
            //    case "Graph Format":
            //        dt = SQLHelper.SecBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2);
            //       if (dt != null)
            //            ExportReportHelper.writeDatTableInPortRate(dt, templatePath.ToString(), exportEcelPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
            //        break;
            //        break;
            //    default:
            //        break;
            //}
        }
    }
    public class ReportList
    {
        public string ReportName
        {
            get;
            set;
        }
        //public bool Check_Status { get; set; }

    }
}
