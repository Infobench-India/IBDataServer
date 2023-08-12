using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AarBatchReportingApp.Views;
using Microsoft.Office.Interop.Excel;

namespace AarBatchReportingApp.Utils
{
    public class ReportGenerationHelper
    {
        public static string autoExportedReportNames;
        public static void LoadReports(string ReportCategory,string SelectedReport,DateTime AvailableFrom,DateTime AvailableTo,string BatchFilter,string ModelFilter)
        {
            if (ExportReportHelper.xpsPackage != null)
            {
                ExportReportHelper.xpsPackage.Close();
                ExportReportHelper.xpsPackage = null;
            }

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
            string viewReportPath = MainWindow.ViewReportPath + "\\" + MainWindow.Reports[ReportCategory][selectedReport]["templateName"].ToString() ;
            string templatePath = MainWindow.ReportTemplatePath + "\\" + MainWindow.Reports[ReportCategory][selectedReport]["templateName"].ToString();
            string connectionString = MainWindow.Reports[ReportCategory][selectedReport]["connectionString"].ToString();
            if (!File.Exists(templatePath.ToString()))
                return;
            switch (SelectedReport)
            {
                case "Daily Batch Report":
                    //string[] BatchAndModelValues = SQLHelper.getBatchModelDailyBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString1);
                    //BatchFilter = string.IsNullOrEmpty(BatchFilter)?BatchAndModelValues[0]: BatchFilter;
                    //ModelFilter = string.IsNullOrEmpty(ModelFilter)? BatchAndModelValues[1]: ModelFilter;
                    //if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                    //{
                    //    return;
                    //}
                    System.Data.DataTable dt = SQLHelper.DailyBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString1, BatchFilter, ModelFilter);
                    if (dt != null)
                    {
                        ExportReportHelper.writeDailyBatchReportInPortRate(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                        ExportReports(selectedReport, ReportCategory);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "Daily Report":
                    dt = SQLHelper.DailyReportLoad(ReportCategory,AvailableFrom, AvailableTo, MainWindow.connectionString);
                    if (dt != null)
                    {
                        ExportReportHelper.writeDailyReportInLandscap(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
                        ExportReports(selectedReport, ReportCategory);
                    }
                    else
                    {
                        return;
                    }

                    break;
                case "Hourly Report":
                    dt = SQLHelper.HourlyReportLoad(ReportCategory,AvailableFrom, AvailableTo, connectionString);
                    if (dt != null)
                    {
                        ExportReportHelper.writeHourlyReportInLandscap(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString());
                        ExportReports(selectedReport, ReportCategory);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "Sec Batch Report":
                    //string[] BatchAndModelGraphValues = SQLHelper.getBatchModelfromBatch_Sec(AvailableFrom, AvailableTo, MainWindow.connectionString2);
                    //BatchFilter = string.IsNullOrEmpty(BatchFilter) ? BatchAndModelGraphValues[0] : BatchFilter;
                    //ModelFilter = string.IsNullOrEmpty(ModelFilter) ? BatchAndModelGraphValues[1] : ModelFilter;
                    //if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                    //{
                    //    return;
                    //}
                    dt = SQLHelper.SecBatchLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                    if (dt != null)
                    {
                        ExportReportHelper.writeDatTableInPortRate(dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                        ExportReports(selectedReport, ReportCategory);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "Graph Format":
                    string[] BatchAndModelSecValues = SQLHelper.getBatchModelfromBatch_Sec(AvailableFrom, AvailableTo, BatchFilter, MainWindow.connectionString2);
                    BatchFilter = string.IsNullOrEmpty(BatchFilter) ? BatchAndModelSecValues[0] : BatchFilter;
                    ModelFilter = string.IsNullOrEmpty(ModelFilter) ? BatchAndModelSecValues[1] : ModelFilter;
                    if (string.IsNullOrEmpty(BatchFilter) || string.IsNullOrEmpty(ModelFilter))
                    {
                        return;
                    }
                    dt = SQLHelper.DailyBatchGraphLoad(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                    if (dt != null)
                    {
                        double[] reqAndActualValues = SQLHelper.getReqAndActualValues(AvailableFrom, AvailableTo, MainWindow.connectionString2, BatchFilter, ModelFilter);
                        ExportReportHelper.writeGraph(reqAndActualValues, dt, templatePath.ToString(), viewReportPath.ToString(), AvailableFrom.ToString(), AvailableTo.ToString(), BatchFilter, ModelFilter);
                        ExportReports(SelectedReport,ReportCategory);
                    }
                    else
                    {
                        return;
                    }
                    break;
                default:
                    return;
            }
        }

        private static void ExportGraphReports(string selectedReport,string ReportCategory)
        {
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
            autoExportedReportNames = autoExportedReportNames + ";" + printPdfFileName + ";" + printExcelFileName;

        }
        private static void ExportReports(string SelectedReport, string ReportCategory)
        {
            string templateFileName = MainWindow.Reports[ReportCategory][SelectedReport]["templateName"].ToString();
            string exportingDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string exportPDFFile = MainWindow.exportPath + "\\" + templateFileName;
            string sourceExcelPath = MainWindow.ViewReportPath + "\\" + templateFileName;
            if (!File.Exists(sourceExcelPath))
                return;
            string printPdfFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".pdf";

            ExportReportHelper.path = Path.Combine(MainWindow.ViewReportPath, templateFileName);
            ExportReportHelper.PrintExcelInPdf(sourceExcelPath, printPdfFileName);
            string printExcelFileName = exportPDFFile.Replace(new FileInfo(exportPDFFile).Extension, "") + exportingDateTime + ".xlsx";
            System.IO.File.Copy(sourceExcelPath, printExcelFileName);
            autoExportedReportNames = autoExportedReportNames + ";" + printPdfFileName + ";" + printExcelFileName;

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
