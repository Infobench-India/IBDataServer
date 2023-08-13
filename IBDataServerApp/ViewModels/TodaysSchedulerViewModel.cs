using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IBDataServerApp.Utils;
using IBDataServerApp.Views;

namespace IBDataServerApp.ViewModels
{
    public class TodaysSchedulerViewModel : ViewModelBase
    {
        #region Properties

        private string batchNumber;

        public string BatchNumber
        {
            get { return batchNumber; }
            set { batchNumber = value; NotifyPropertyChanged(); }
        }

        private string modelNumber;

        public string ModelNumber
        {
            get { return modelNumber; }
            set { modelNumber = value; NotifyPropertyChanged(); }
        }

        private Visibility batchNUmberVisibility = Visibility.Collapsed;

        public Visibility BatchNUmberVisibility
        {
            get { return batchNUmberVisibility; }
            set { batchNUmberVisibility = value; NotifyPropertyChanged(); }
        }

        private Visibility modelNumberVisbility = Visibility.Collapsed;

        public Visibility ModelNumberVisbility
        {
            get { return modelNumberVisbility; }
            set { modelNumberVisbility = value; NotifyPropertyChanged(); }
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

        private DateTime _scheduleTime = DateTime.Now;
        public DateTime ScheduleTime
        {
            get { return _scheduleTime; }
            set
            {
                _scheduleTime = value;
                //int i = DateTime.Compare(_scheduleTime.Date, DateTime.Now.Date);
                //if(i!=0)
                //    _scheduleTime = DateTime.Now;
                NotifyPropertyChanged();
            }
        }

        private DataTable dataSource;

        public DataTable DataSource
        {
            get { return dataSource; }
            set { dataSource = value;
                if(dataSource!=null)
                {
                    dataSource.Columns["ReportType"].ColumnName = "REPORT TYPE";
                    dataSource.Columns["BatchNumber"].ColumnName = "CHASIS NO";
                    dataSource.Columns["ModelNumber"].ColumnName = "MODEL NO";
                    dataSource.Columns["AvailableFrom"].ColumnName = "AVAILABLE FROM";
                    dataSource.Columns["AvailableTo"].ColumnName = "AVAILABLE TO";
                    dataSource.Columns["ScheduleDateTime"].ColumnName = "SCHEDULE DATETIME";
                    dataSource.Columns.Remove("MODEL NO");
                    dataSource.Columns.Remove("CHASIS NO");
                }

                NotifyPropertyChanged(); }
        }

        private System.Data.DataRowView selectedRow;

        public System.Data.DataRowView SelectedRow
        {
            get { return selectedRow; }
            set
            {
                selectedRow = value;
                if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
                {

                    ScheduleTime = Convert.ToDateTime(selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-1]);

                    AvailableTo = Convert.ToDateTime( selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-2]);
                     AvailableFrom = Convert.ToDateTime(selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-3]);
                    //if (selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-3] != null)
                    //{
                    //    ModelNumber = selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-3].ToString();
                    //}
                    //else
                    //{
                        ModelNumber = string.Empty;
                    //}
                    //if (selectedRow.Row.ItemArray[2] != null)
                    //{
                    //    BatchNumber = selectedRow.Row.ItemArray[2].ToString();
                    //}
                    //else
                    //{
                        BatchNumber = string.Empty;
                    //}
                    SelectedReport = selectedRow.Row.ItemArray[selectedRow.Row.Table.Columns.Count-4].ToString();
                }
                NotifyPropertyChanged();
            }
        }
        List<string> reportNameList = new List<string>();

        public List<string> ReportNameList
        {
            get { return reportNameList; }
            set { reportNameList = value; NotifyPropertyChanged(); }
        }

        private string _selectedReport = "Daily Report";

        public string SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                _selectedReport = value;
                switch (_selectedReport)
                {
                    case "Daily Batch Report":
                        ModelNumberVisbility = Visibility.Collapsed;
                        BatchNUmberVisibility = Visibility.Collapsed;
                        break;
                    case "Sec Batch Report":
                        ModelNumberVisbility = Visibility.Collapsed;
                        BatchNUmberVisibility = Visibility.Visible;
                        ModelNumber = String.Empty;
                        break;
                    case "Graph Format":
                        ModelNumberVisbility = Visibility.Collapsed;
                        BatchNUmberVisibility = Visibility.Visible;
                        ModelNumber = String.Empty;
                        break;
                    default:
                        ModelNumberVisbility = Visibility.Collapsed;
                        BatchNUmberVisibility = Visibility.Collapsed;
                        break;
                }
                NotifyPropertyChanged();
            }
        }
        private string _selectedReportCatagory = "TC";

        public string SelectedReportCatagory
        {
            get { return _selectedReportCatagory; }
            set
            {
                _selectedReportCatagory = value;
                NotifyPropertyChanged();
            }
        }
        List<string> reportCategories = new List<string>();
        public List<string> ReportCategories
        {
            get { return reportCategories; }
            set { reportCategories = value; }
        }
        #endregion

        #region Constructor
        public TodaysSchedulerViewModel()
        {
            GetData();
            string[] tankName = new string[2] {
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
        #endregion

        #region Commands
        public RelayCommand<object> AddCommand { get { return new RelayCommand<object>(Add); } }
        public RelayCommand<object> UpdateCommand { get { return new RelayCommand<object>(Update); } }
        public RelayCommand<object> DeleteCommand { get { return new RelayCommand<object>(Delete); } }

        #endregion

        #region PrivateMethod
        private void Add(object obj)
        {
            SQLHelper.AddTodaysSchedulerData(MainWindow.connectionString1, SelectedReportCatagory, SelectedReport,BatchNumber,ModelNumber, AvailableFrom, AvailableTo, ScheduleTime);
            DataSource = SQLHelper.GetTodaysSchedulerData(MainWindow.connectionString1);

            BatchNumber = string.Empty;
            ModelNumber = string.Empty;
        }
        private void Update(object obj)
        {
            if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
                SQLHelper.UpdateTodaysSchedulerData(MainWindow.connectionString1, SelectedReportCatagory, SelectedReport,BatchNumber,ModelNumber, AvailableFrom, AvailableTo, ScheduleTime, int.Parse(selectedRow.Row.ItemArray[0].ToString()));
            DataSource = SQLHelper.GetTodaysSchedulerData(MainWindow.connectionString1);

            BatchNumber = string.Empty;
            ModelNumber = string.Empty;
        }
        private void Delete(object obj)
        {
            if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
                SQLHelper.DeleteTodaysSchedulerData(MainWindow.connectionString1, int.Parse(selectedRow.Row.ItemArray[0].ToString()));
            DataSource = SQLHelper.GetTodaysSchedulerData(MainWindow.connectionString1);

            BatchNumber = string.Empty;
            ModelNumber = string.Empty;

        }
        private void GetData()
        {
            DataSource = SQLHelper.GetTodaysSchedulerData(MainWindow.connectionString1);

        }

        #endregion
    }
}
