using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using AarBatchReportingApp.Utils;
using AarBatchReportingApp.Views;
using Microsoft.Office.Interop.Excel;

namespace AarBatchReportingApp.ViewModels
{
    public class ScheduleReportViewModel : ViewModelBase
    {
        public ScheduleReportViewModel()
        {
            ReportNameList = new List<string>();
            // 1 element
            string[] tankName = new string[2] { "Daily Report", "Hourly Report" };
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

        List<string> reportNameList = new List<string>();

        public List<string> ReportNameList
        {
            get { return reportNameList; }
            set { reportNameList = value; NotifyPropertyChanged(); }
        }
        List<string> reportCategories = new List<string>();
        public List<string> ReportCategories
        {
            get { return reportCategories; }
            set { reportCategories = value; }
        }
        private string _selectedReport = "Daily Report";

        public string SelectedReport
        {
            get { return _selectedReport; }
            set { _selectedReport = value;
                Status=String.Empty;
                BatchFilter = String.Empty;
                ModelFilter = String.Empty;
                if (_selectedReport == "Daily Batch Report" )
                {
                    BatchNumberFilterVisibilty = Visibility.Visible;
                    ModelFilterVisibilty = Visibility.Visible;
                    OtherBatchNumberFilterVisibilty = Visibility.Collapsed;
                    GraphBatchNumberFilterVisibilty = Visibility.Visible;
                }
                else if( _selectedReport == "Graph Format")
                {
                    BatchNumberFilterVisibilty = Visibility.Visible;
                    ModelFilterVisibilty = Visibility.Collapsed;
                    OtherBatchNumberFilterVisibilty = Visibility.Collapsed;
                    GraphBatchNumberFilterVisibilty = Visibility.Visible;
                }
                else if (_selectedReport == "Sec Batch Report")
                {
                    BatchNumberFilterVisibilty = Visibility.Visible;
                    ModelFilterVisibilty = Visibility.Visible;
                    OtherBatchNumberFilterVisibilty = Visibility.Collapsed;
                    GraphBatchNumberFilterVisibilty = Visibility.Visible;
                }
                else
                {
                    BatchNumberFilterVisibilty = Visibility.Collapsed;
                    ModelFilterVisibilty = Visibility.Collapsed;
                    OtherBatchNumberFilterVisibilty = Visibility.Collapsed;
                    GraphBatchNumberFilterVisibilty = Visibility.Collapsed;
                }
                NotifyPropertyChanged(); }
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

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; NotifyPropertyChanged(); }
        }

        private FixedDocumentSequence _document;

        public FixedDocumentSequence Documents
        {
            get { return _document; }
            set { _document = value; NotifyPropertyChanged(); }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; NotifyPropertyChanged(); }
        }

        private double _currentProgress;

        public double CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                _currentProgress = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility batchNumberFilterVisibilty = Visibility.Collapsed;

        public Visibility BatchNumberFilterVisibilty
        {
            get { return batchNumberFilterVisibilty; }
            set { batchNumberFilterVisibilty = value; NotifyPropertyChanged(); }
        }

        private Visibility otherBatchNumberFilterVisibilty;

        public Visibility OtherBatchNumberFilterVisibilty
        {
            get { return otherBatchNumberFilterVisibilty; }
            set { otherBatchNumberFilterVisibilty = value; NotifyPropertyChanged(); }
        }

        private Visibility graphBatchNumberFilterVisibilty;

        public Visibility GraphBatchNumberFilterVisibilty
        {
            get { return graphBatchNumberFilterVisibilty; }
            set { graphBatchNumberFilterVisibilty = value; NotifyPropertyChanged(); }
        }


        private Visibility modelFilterVisibilty = Visibility.Collapsed;

        public Visibility ModelFilterVisibilty
        {
            get { return modelFilterVisibilty; }
            set { modelFilterVisibilty = value; NotifyPropertyChanged(); }
        }

        private object _viewReportPath;

        public object ViewReportPath
        {
            get { return _viewReportPath; }
            set { _viewReportPath = value; NotifyPropertyChanged(); }
        }


        private string batchFilter;

        public string BatchFilter
        {
            get { return batchFilter;  }
            set { batchFilter = value; NotifyPropertyChanged(); }
        }

        private string modelFilter;

        public string ModelFilter
        {
            get { return modelFilter; }
            set { modelFilter = value; NotifyPropertyChanged(); }
        }

        private IDocumentPaginatorSource _viewDocument;

        public IDocumentPaginatorSource ViewDocument
        {
            get { return _viewDocument; }
            set { _viewDocument = value; NotifyPropertyChanged(); }
        }


        #region Commands
        public RelayCommand<object> LoadReportCmd { get { return new RelayCommand<object>(LoadReports); } }
        public RelayCommand<object> ExportReportCmd { get { return new RelayCommand<object>(ExportReports); } }
        public RelayCommand<object> ViewReportCmd { get { return new RelayCommand<object>(ViewReport); } }

        #endregion
        public void ViewReport(object obj)
        {
            if(ViewReportPath!=null)
            ExportReportHelper.DisplayXPSFile(ViewReportPath.ToString().Replace(new FileInfo(ViewReportPath.ToString()).Extension, "") + ".xps");
            //if (ExportReportHelper.fixedDocumentSequence != null)
            //    ViewDocument = ExportReportHelper.fixedDocumentSequence;
        }

        private void displayValue(string propertyName, object value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
            new System.Action(() => {
                switch (propertyName)
                {
                    case "Status":
                        {
                            Status = value.ToString();
                        }
                        break;
                    case "ViewReportPath":
                        {
                            ViewReportPath = value.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }));
        }

        private void LoadReports(object obj)
        {
            IsBusy = true;
            Status = "Generating Report.......";
            CurrentProgress = 0;
            Task.Factory.StartNew(() =>
            {
                if (ExportReportHelper.xpsPackage != null)
                    ExportReportHelper.xpsPackage.Close();
                if (ExportReportHelper.fixedDocumentSequence != null)
                    ExportReportHelper.fixedDocumentSequence = null;
                string[] filePaths = Directory.GetFiles(MainWindow.ViewReportPath);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
                string selectedReport = "";
                switch (SelectedReport)
                {
                    case "Daily Batch Report":
                        selectedReport = "Batch Wise Daily  Report";
                        break;
                    case "Daily Report":
                        selectedReport = "dailyReport";
                        break;
                    case "Hourly Report":
                        selectedReport = "hourlyReport";
                        break;
                    case "Sec Batch Report":
                        selectedReport = "Batch Wise Sec Report";
                        break;
                    case "Graph Format":
                        selectedReport = "GRAPH FORMAT";
                        break;
                    default:
                        break;
                }
                string ReportCategory = "TC";
                object viewReportPath = MainWindow.ViewReportPath + "\\" + MainWindow.Reports[SelectedReportCatagory][selectedReport]["templateName"].ToString();
                object templatePath = MainWindow.ReportTemplatePath + "\\" + MainWindow.Reports[SelectedReportCatagory][selectedReport]["templateName"].ToString();
                string connectionString = MainWindow.Reports[ReportCategory][selectedReport]["connectionString"].ToString();
                if (!File.Exists(templatePath.ToString()))
                    return;
                CurrentProgress = 15;
                switch (SelectedReport)
                {
                    case "Daily Batch Report":
                        CurrentProgress = 25;
                        //string[] BatchAndModelValues = SQLHelper.getBatchModelDailyBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString1);
                        //BatchFilter = string.IsNullOrEmpty(BatchFilter) ? BatchAndModelValues[0] : BatchFilter;
                        //ModelFilter = string.IsNullOrEmpty(ModelFilter) ? BatchAndModelValues[1] : ModelFilter;

                        //CurrentProgress = 35;
                        //if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                        //{
                        //    Status = "No Data Found.";
                        //    displayValue("Status", "No Data Found.");
                        //    return;
                        //}
                        CurrentProgress = 45;
                        System.Data.DataTable dt = SQLHelper.DailyBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString1, BatchFilter, ModelFilter);
                        CurrentProgress = 50;
                        if (dt != null)
                        {
                            ExportReportHelper.writeDailyBatchReportInPortRate(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                            CurrentProgress = 60;
                            ExportReportHelper.ConvertExcel(viewReportPath.ToString(), viewReportPath.ToString().Replace(new FileInfo(viewReportPath.ToString()).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
                            CurrentProgress = 80;
                        }
                        else
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                        break;
                    case "Daily Report":
                        dt = SQLHelper.DailyReportLoad(SelectedReportCatagory, AvailableFrom, AvailableTo, MainWindow.connectionString);
                        CurrentProgress = 50;
                        if (dt != null)
                        {
                            ExportReportHelper.writeDailyReportInLandscap(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
                            CurrentProgress = 60;
                            ExportReportHelper.ConvertExcel(viewReportPath.ToString(), viewReportPath.ToString().Replace(new FileInfo(viewReportPath.ToString()).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
                            CurrentProgress = 80;
                        }
                        else
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }

                        break;
                    case "Hourly Report":
                        dt = SQLHelper.HourlyReportLoad(SelectedReportCatagory, AvailableFrom, AvailableTo, MainWindow.connectionString);
                        CurrentProgress = 50;
                        if (dt != null)
                        {
                            ExportReportHelper.writeHourlyReportInLandscap(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
                            CurrentProgress = 60;
                            ExportReportHelper.ConvertExcel(viewReportPath.ToString(), viewReportPath.ToString().Replace(new FileInfo(viewReportPath.ToString()).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
                            CurrentProgress = 80;
                        }
                        else
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                        break;
                    case "Sec Batch Report":
                        //string[] BatchAndModelGraphValues = SQLHelper.getBatchModelfromBatch_Sec(AvailableFrom, AvailableTo, MainWindow.connectionString2);
                        //BatchFilter = string.IsNullOrEmpty(BatchFilter) ? BatchAndModelGraphValues[0] : BatchFilter;
                        //ModelFilter = string.IsNullOrEmpty(ModelFilter) ? BatchAndModelGraphValues[1] : ModelFilter;

                        //if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                        //{
                        //    Status = "No Data Found.";
                        //    displayValue("Status", "No Data Found.");
                        //    return;
                        //}
                        dt = SQLHelper.SecBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                        CurrentProgress = 50;
                        if (dt != null)
                        {
                            ExportReportHelper.writeDatTableInPortRate(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                            CurrentProgress = 60;
                            ExportReportHelper.ConvertExcel(viewReportPath.ToString(), viewReportPath.ToString().Replace(new FileInfo(viewReportPath.ToString()).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
                            CurrentProgress = 80;
                        }
                        else
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                        break;
                    case "Graph Format":
                        string[] BatchAndModelSecValues = SQLHelper.getBatchModelfromBatch_Sec(AvailableFrom, AvailableTo, BatchFilter, MainWindow.connectionString2);
                        BatchFilter = string.IsNullOrEmpty(BatchFilter) ? BatchAndModelSecValues[0] : BatchFilter;
                        ModelFilter = string.IsNullOrEmpty(ModelFilter) ? BatchAndModelSecValues[1] : ModelFilter;

                        if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                        dt = SQLHelper.DailyBatchGraphLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                        CurrentProgress = 50;
                        if (dt != null)
                        {
                            double[] reqAndActualValues = SQLHelper.getReqAndActualValues(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                            ExportReportHelper.writeGraph(reqAndActualValues, dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                            CurrentProgress = 60;
                            ExportReportHelper.ConvertGraphReport(viewReportPath.ToString(), viewReportPath.ToString().Replace(new FileInfo(viewReportPath.ToString()).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
                            CurrentProgress = 80;
                        }
                        else
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                        break;
                    default:
                        {
                            Status = "No Data Found.";
                            displayValue("Status", "No Data Found.");
                            return;
                        }
                }
                ViewReportPath = viewReportPath;
                displayValue("ViewReportPath", viewReportPath);
            }).ContinueWith((task) =>
            {
                if(CurrentProgress==80)
                Status = "Generated Report. View Now";                
                CurrentProgress = 100;
                IsBusy = false;
                ViewReport(new object());
            }, TaskScheduler.FromCurrentSynchronizationContext());
           
            // Documents = ExportReportHelper.fixedDocumentSequence;

        }

        private void ExportGraphReports(object obj, string ReportCategory)
        {
            Status = "Exporting.....";
            // Documents = ExportReportHelper.fixed
            var availableTo = AvailableTo;
            var availableFrom = AvailableFrom;
            var reportNameList = ReportNameList;
            string selectedReport = "";
            switch (SelectedReport)
            {
                case "Daily Batch Report":
                    selectedReport = "Batch Wise Daily  Report";
                    break;
                case "Daily Report":
                    selectedReport = "dailyReport";
                    break;
                case "Hourly Report":
                    selectedReport = "hourlyReport";
                    break;
                case "Sec Batch Report":
                    selectedReport = "Batch Wise Sec Report";
                    break;
                case "Graph Format":
                    selectedReport = "GRAPH FORMAT";
                    break;
                default:
                    break;
            }
            string templateFileName = MainWindow.Reports[ReportCategory][selectedReport]["templateName"].ToString();
            
            string exportingDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string exportPDFFile = MainWindow.exportPath + "\\" + templateFileName;
            string sourceExcelPath = MainWindow.ViewReportPath + "\\" + templateFileName;
            if (!File.Exists(sourceExcelPath))
                return;
            string printPdfFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".pdf";

            ExportReportHelper.path = Path.Combine(MainWindow.ViewReportPath, templateFileName);
            ExportReportHelper.PrintGraphInPdf(sourceExcelPath, printPdfFileName);
            string printExcelFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".xlsx";
            System.IO.File.Copy(sourceExcelPath, printExcelFileName);
            Status = "Exported. ";
            //ExportReportHelper.PrintExcelInExcel(sourceExcelPath, exportPDFFile);

        }
        private void ExportReports(object obj)
        {
            IsBusy = true;
            CurrentProgress = 0;
            Status = "Exporting.....";
            Task.Factory.StartNew(() =>
            {
                // Documents = ExportReportHelper.fixed
                var availableTo = AvailableTo;
            var availableFrom = AvailableFrom;
            var reportNameList = ReportNameList;
                string selectedReport = "";
                switch (SelectedReport)
                {
                    case "Daily Batch Report":
                        selectedReport = "Batch Wise Daily  Report";
                        break;
                    case "Daily Report":
                        selectedReport = "dailyReport";
                        break;
                    case "Hourly Report":
                        selectedReport = "hourlyReport";
                        break;
                    case "Sec Batch Report":
                        selectedReport = "Batch Wise Sec Report";
                        break;
                    case "Graph Format":
                        selectedReport = "GRAPH FORMAT";
                        break;
                    default:
                        break;
                }
                string templateFileName = MainWindow.Reports[SelectedReportCatagory][selectedReport]["templateName"].ToString();
                string exportingDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string exportPDFFile = MainWindow.exportPath + "\\" + templateFileName;
            string sourceExcelPath = MainWindow.ViewReportPath + "\\" + templateFileName;
            if (!File.Exists(sourceExcelPath))
                return;
                CurrentProgress = 10;
                string printPdfFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".pdf";
                CurrentProgress = 20;
                ExportReportHelper.path = Path.Combine(MainWindow.ViewReportPath, templateFileName);
                CurrentProgress = 30;
                ExportReportHelper.PrintExcelInPdf(sourceExcelPath, printPdfFileName);
                CurrentProgress = 50;
                string printExcelFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".xlsx";
                CurrentProgress = 70;
                System.IO.File.Copy(sourceExcelPath, printExcelFileName);
                CurrentProgress = 90;
            }).ContinueWith((task) =>
            {
                if (CurrentProgress == 90)
                    Status = "Exported. ";
                CurrentProgress = 100;
                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
            //ExportReportHelper.PrintExcelInExcel(sourceExcelPath, exportPDFFile);

        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
