using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using IBDataServerApp.Utils;
using Microsoft.Reporting.WinForms;

namespace IBDataServerApp.Views
{
    /// <summary>
    /// Interaction logic for AlarmReports.xaml
    /// </summary>
    public partial class AlarmReports : UserControl
    {
        public AlarmReports()
        {
            InitializeComponent();
           
        }
        private DateTime _availableFrom = DateTime.Now.AddDays(-1);        
        public DateTime AvailableFrom
        {
            get { return _availableFrom; }
            set { _availableFrom = value; }
        }
        private DateTime _availableTo = DateTime.Now;
        public DateTime AvailableTo
        {
            get { return _availableTo; }
            set
            {
                _availableTo = value;
            }
        }
       
        string selectedReport;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ExportReportHelper.path = @"F:\2019\PlushCon\Reporting Application\Reporting Tool Source Code\IBDataServerApp\IBDataServerApp\bin\Debug\viewReport\Batch Wise Daily  Report.xlsx";
            //// ExportReportHelper.PrintExcelInPdf(ExportReportHelper.path);
            // ExportReportHelper.Convert(ExportReportHelper.path, ExportReportHelper.path.Replace(new FileInfo(ExportReportHelper.path).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
            // DocView.Document = ExportReportHelper.DisplayXPSFile(ExportReportHelper.path.Replace(new FileInfo(ExportReportHelper.path).Extension, "") + ".xps");
            if (ExportReportHelper.fixedDocumentSequence != null)
                DocView.Document = ExportReportHelper.fixedDocumentSequence;
        }
        private void BindCountryDropDown()
        {
            AddReportNameInList();
        }
        private void AddReportNameInList()
        {
            


        }
        private void TanksComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TanksComboBox.ItemsSource = objCountryList.Where(x => x.TankName.StartsWith(TanksComboBox.Text.Trim()));
        }
        private void TanksComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedReport = TanksComboBox.SelectedItem.ToString();
        }
    }
    
}
