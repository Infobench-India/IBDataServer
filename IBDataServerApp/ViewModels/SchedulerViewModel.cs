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
    public class SchedulerViewModel : ViewModelBase
    {
        #region Properties
        private string batchNumber = String.Empty;

        public string BatchNumber
        {
            get { return batchNumber; }
            set { batchNumber = value; NotifyPropertyChanged(); }
        }

        private string modelNumber = String.Empty;

        public string ModelNumber
        {
            get { return modelNumber; }
            set { modelNumber = value; NotifyPropertyChanged(); }
        }

        private DataTable dataSource;

        public DataTable DataSource
        {
            get { return dataSource; }
            set { dataSource = value;
                if (dataSource != null)
                {
                    dataSource.Columns["ReportType"].ColumnName = "REPORT TYPE";
                    dataSource.Columns["BatchNumber"].ColumnName = "CHASIS NO";
                    dataSource.Columns["ModelNumber"].ColumnName = "MODEL NO";
                    dataSource.Columns.Remove("CHASIS NO");
                    dataSource.Columns.Remove("MODEL NO");
                }
                NotifyPropertyChanged(); }
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


        private System.Data.DataRowView selectedRow;

        public System.Data.DataRowView SelectedRow
        {
            get { return selectedRow; }
            set
            {
                selectedRow = value;
                if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
                {
                    //BatchNumber = selectedRow.Row.ItemArray[2].ToString();
                    //ModelNumber = selectedRow.Row.ItemArray[3].ToString();
                    BatchNumber = String.Empty;
                    ModelNumber = String.Empty;
                    SelectedReport = selectedRow.Row.ItemArray[1].ToString();
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

        #endregion

        #region Constructor
        public SchedulerViewModel()
        {
            GetData();
            string[] tankName = new string[1]{ "Daily Report"};
            for (int i = 0; i < tankName.Length; i++)
            {
                ReportNameList.Add(tankName[i]);
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
           SQLHelper.AddSchedulerData(MainWindow.connectionString1, SelectedReportCatagory,SelectedReport, BatchNumber,ModelNumber);
            DataSource = SQLHelper.GetSchedulerData(MainWindow.connectionString1);
        }
        private void Update(object obj)
        {
            if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
              SQLHelper.UpdateSchedulerData(MainWindow.connectionString1, SelectedReportCatagory, SelectedReport, BatchNumber, ModelNumber, int.Parse(selectedRow.Row.ItemArray[0].ToString()));
            DataSource = SQLHelper.GetSchedulerData(MainWindow.connectionString1);
        }
        private void Delete(object obj)
        {
            if (selectedRow != null && selectedRow.Row != null && selectedRow.Row.ItemArray != null && selectedRow.Row.ItemArray.Length > 0)
                SQLHelper.DeleteSchedulerData(MainWindow.connectionString1, int.Parse(selectedRow.Row.ItemArray[0].ToString()));
            DataSource = SQLHelper.GetSchedulerData(MainWindow.connectionString1);

        }
        private void GetData()
        {
            DataSource = SQLHelper.GetSchedulerData(MainWindow.connectionString1);

        }

            #endregion
    }
}
