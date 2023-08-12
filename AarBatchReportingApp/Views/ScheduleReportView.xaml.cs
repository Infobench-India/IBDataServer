using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Windows.Xps.Packaging;
using AarBatchReportingApp.Utils;
using AarBatchReportingApp.ViewModels;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;

namespace AarBatchReportingApp.Views
{
    /// <summary>
    /// Interaction logic for ScheduleReportView.xaml
    /// </summary>
    public partial class ScheduleReportView : UserControl
    {
        public ScheduleReportView()
        {
            InitializeComponent();
        }
       
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ExportReportHelper.path = @"F:\2019\PlushCon\Reporting Application\Reporting Tool Source Code\AarBatchReportingApp\AarBatchReportingApp\bin\Debug\viewReport\Batch Wise Daily  Report.xlsx";
            //// ExportReportHelper.PrintExcelInPdf(ExportReportHelper.path);
            // ExportReportHelper.Convert(ExportReportHelper.path, ExportReportHelper.path.Replace(new FileInfo(ExportReportHelper.path).Extension, "") + ".xps", XlFixedFormatType.xlTypeXPS);
            // DocView.Document = ExportReportHelper.DisplayXPSFile(ExportReportHelper.path.Replace(new FileInfo(ExportReportHelper.path).Extension, "") + ".xps");
            if(ExportReportHelper.fixedDocumentSequence!=null)
            DocView.Document = ExportReportHelper.fixedDocumentSequence;
         }
    }
}
