using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using IBDataServerApp.Views;
using Microsoft.Office.Interop.Excel;

namespace IBDataServerApp.Utils
{
    public class ExportReportHelper
    {
        public static string path = "";

        public static string rootFolder = @"F:\2020\PlushCon\Report Template";

        public static XpsDocument xpsPackage = null;
        public static FixedDocumentSequence fixedDocumentSequence = null;
        public static void DisplayXPSFile(string xpsFileName)
        {
            if (!File.Exists(xpsFileName))
                return;
            xpsPackage = new XpsDocument(xpsFileName, FileAccess.Read, CompressionOption.SuperFast);
            fixedDocumentSequence = xpsPackage.GetFixedDocumentSequence();
        }

        public static bool ConvertExcel(string sourcePath, string targetPath, XlFixedFormatType targetType)
        {
            if (!File.Exists(sourcePath))
                return false;
            bool result;
            object missing = Type.Missing;
            var application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            try
            {
                object target = targetPath;
                workBook = application.Workbooks.Open(sourcePath);
                workBook.ExportAsFixedFormat(targetType, target);
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;

        }

        public static bool ConvertGraphReport(string sourcePath, string targetPath, XlFixedFormatType targetType)
        {
            if (!File.Exists(sourcePath))
                return false;
            bool result;
            object missing = Type.Missing;
            var application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            try
            {
                object target = targetPath;
                workBook = application.Workbooks.Open(sourcePath);
                //workBook.ExportAsFixedFormat(targetType, target);
                workBook.ExportAsFixedFormat(targetType, target, XlFixedFormatQuality.xlQualityStandard,
        true,
        true,
        1,
        1,
        false);
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;

        }


        public static void PrintExcelInPdf(string sourceFilePath, string destinationFlePath)
        {
            var eapp = new Microsoft.Office.Interop.Excel.Application();
            Type eType = eapp.GetType();
            Microsoft.Office.Interop.Excel.Workbooks Ewb = eapp.Workbooks;
            Type elType = Ewb.GetType();
            string objelName = sourceFilePath;
            Microsoft.Office.Interop.Excel.Workbook ebook = (Microsoft.Office.Interop.Excel.Workbook)elType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, Ewb, new Object[] { objelName, true, true });

            Object oMissing = System.Reflection.Missing.Value;
            ebook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, destinationFlePath);
            eType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, eapp, null);
        }

        public static void PrintGraphInPdf(string sourceFilePath, string destinationFlePath)
        {
            var eapp = new Microsoft.Office.Interop.Excel.Application();
            Type eType = eapp.GetType();
            Microsoft.Office.Interop.Excel.Workbooks Ewb = eapp.Workbooks;
            Type elType = Ewb.GetType();
            string objelName = sourceFilePath;
            Microsoft.Office.Interop.Excel.Workbook ebook = (Microsoft.Office.Interop.Excel.Workbook)elType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, Ewb, new Object[] { objelName, true, true });

            Object oMissing = System.Reflection.Missing.Value;
            // ebook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, destinationFlePath);
            ebook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, destinationFlePath, XlFixedFormatQuality.xlQualityStandard,
        true,
        true,
        1,
        1,
        false);
            eType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, eapp, null);
        }


        public static void PrintExcelInExcel(string path, string exportingDateTime)
        {

            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

            excel.DisplayAlerts = false;
            object missing = Type.Missing;
            var application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBookForExcel = null;
            try
            {
                workBookForExcel = application.Workbooks.Open(path.Replace(new FileInfo(path).Extension, "") + exportingDateTime, missing, missing, missing, missing, missing,
                                  missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBookForExcel.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
            }
            catch
            {
            }
            finally
            {
                if (workBookForExcel != null)
                {
                    workBookForExcel.Close(true, missing, missing);
                    workBookForExcel = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void writeDailyReportInLandscap(System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate)
        {
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            int TableHeader = 5;
            int ReportHeader = 7;
            int ReportHeaderAndTableHeader = TableHeader + ReportHeader;
            int FooterRowCount = 7;
            int headerAndFooterRowCount = ReportHeaderAndTableHeader + FooterRowCount;
            int maxRowPerPage = 37;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "8:12";
            int pageNo = 0;
            int localRowCount = 0;
            int startCellRowCount = ReportHeaderAndTableHeader + 1;
            int endCellRowCount = 0;
            int sumStartColumnNumber = 1;
            try
            {
                xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 2].Value = fromDate;
                xlsht.Cells[10, 2].Value = toDate;
                double[] totalReportSum = new double[dataTablevalue.Columns.Count - 1];
                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 0)
                    {
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {
                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 2;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy(Type.Missing);

                        //Paste format only to the cell C5
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats,
    XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);
                        double[] csum = new double[dataTablevalue.Columns.Count - 1];
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            csum[k - 1] = 0;
                            for (int j = 0; j <= localRowCount; j++)
                                csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                        if (i < dataTablevalue.Rows.Count - 1)
                        {
                            Range from = xlsht.Range[tableHeaderRows];
                            string range = (endCellRowCount + FooterRowCount + ReportHeader).ToString() + ":" + (endCellRowCount + FooterRowCount + ReportHeader + TableHeader).ToString();
                            Range to = xlsht.Range[range];
                            from.Copy(to);
                            from = null;
                            to = null;
                        }
                        startCellRowCount = endCellRowCount + headerAndFooterRowCount;
                        data = null;
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                        localRowCount = 0;
                        continue;
                    }
                    if (i == dataTablevalue.Rows.Count - 1)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 3;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        double[] csum = new double[dataTablevalue.Columns.Count - 1];
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            csum[k - 1] = 0;
                            for (int j = 0; j <= localRowCount; j++)
                                csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        data[localRowCount + 2, 0] = " Report Total";
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
                            data[localRowCount + 2, k] = totalReportSum[k - sumStartColumnNumber];
                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 2];
                        lastRowRange.Font.Bold = true;
                        lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                    }
                    localRowCount++;
                }
            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void writeHourlyReportInLandscap(System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate)
        {
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            int TableHeader = 5;
            int ReportHeader = 7;
            int ReportHeaderAndTableHeader = TableHeader + ReportHeader;
            int FooterRowCount = 7;
            int headerAndFooterRowCount = ReportHeaderAndTableHeader + FooterRowCount;
            int maxRowPerPage = 37;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "8:12";
            int pageNo = 0;
            int localRowCount = 0;
            int startCellRowCount = ReportHeaderAndTableHeader + 1;
            int endCellRowCount = 0;
            int sumStartColumnNumber = 2;
            try
            {
                xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 2].Value = fromDate;
                xlsht.Cells[10, 2].Value = toDate;
                double[] totalReportSum = new double[dataTablevalue.Columns.Count - 1];
                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 0)
                    {
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {

                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 2;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();

                        //Paste format only to the cell C5
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);

                        double[] csum = new double[dataTablevalue.Columns.Count - 1];
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            csum[k - 1] = 0;
                            for (int j = 0; j <= localRowCount; j++)
                                csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[2, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                        if (i < dataTablevalue.Rows.Count - 1)
                        {
                            Range from = xlsht.Range[tableHeaderRows];
                            string range = (endCellRowCount + FooterRowCount + ReportHeader).ToString() + ":" + (endCellRowCount + FooterRowCount + ReportHeader + TableHeader).ToString();
                            Range to = xlsht.Range[range];
                            from.Copy(to);
                            from = null;
                            to = null;
                        }
                        startCellRowCount = endCellRowCount + headerAndFooterRowCount;
                        data = null;
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                        localRowCount = 0;
                        continue;
                    }
                    if (i == dataTablevalue.Rows.Count - 1)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 3;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        double[] csum = new double[dataTablevalue.Columns.Count - 1];
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            csum[k - 1] = 0;
                            for (int j = 0; j <= localRowCount; j++)
                                csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        data[localRowCount + 2, 0] = " Report Total";
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
                            data[localRowCount + 2, k] = totalReportSum[k - sumStartColumnNumber];
                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[2, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 2];
                        lastRowRange.Font.Bold = true;
                        lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                    }
                    localRowCount++;
                }

            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void writeDatTableInPortRate(System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate, string BatchFilter, string ModelFilter)
        {
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            const int TableHeader = 5;
            const int ReportHeader = 7;
            const int FooterRowCount = 9;
            const int maxRowPerPage = 37;
            const int dateCulumnNumber = 2;
            const int timeColumnNumber = 3;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "8:12";
            int localRowCount = 1;
            int startCellRowCount = TableHeader + ReportHeader + 1;
            int endCellRowCount = 0;
            try
            {
                xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 2].Value = fromDate;
                xlsht.Cells[10, 2].Value = toDate;
                xlsht.Cells[8, 5].Value = BatchFilter;
                xlsht.Cells[9, 5].Value = ModelFilter;
                double[] totalReportSum = new double[dataTablevalue.Columns.Count - 1];
                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 1)
                    {
                        data = new object[maxRowPerPage+1, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {

                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount-1, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount;
                        var endCell = (Range)xlsht.Cells[endCellRowCount-1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[TableHeader + ReportHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();

                        //Paste format only to the cell C5
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        writeRange.Value = data;
                        writeRange.Columns[dateCulumnNumber, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[timeColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        startCellRowCount = endCellRowCount + FooterRowCount+ TableHeader + ReportHeader + 1;
                        if (i < dataTablevalue.Rows.Count - 1)
                        {
                            Range from = xlsht.Range[tableHeaderRows];
                            string range = (endCellRowCount + FooterRowCount + ReportHeader+1).ToString() + ":" + (startCellRowCount-1).ToString();
                            Range to = xlsht.Range[range];
                            from.Copy(to);
                            from = null;
                            to = null;
                        }
                        
                        data = null;
                        data = new object[maxRowPerPage+1, dataTablevalue.Columns.Count];
                        localRowCount = 1;
                        continue;
                    }
                    if (i == dataTablevalue.Rows.Count - 1)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount;
                        var endCell = (Range)xlsht.Cells[endCellRowCount-1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[TableHeader + ReportHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        writeRange.Value = data;
                        writeRange.Columns[dateCulumnNumber, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[timeColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                    }
                    localRowCount++;
                }

            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void writeDailyBatchReportInPortRate(System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate, string BatchFilter, string ModelFilter)
        {
            //object misValue = System.Reflection.Missing.Value;
            //Application xlApp = new Application();
            //Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            //Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            //xlApp.Visible = false;
            //int TableHeader = 5;
            //int ReportHeader = 7;
            //int ReportHeaderAndTableHeader = TableHeader + ReportHeader;
            //int FooterRowCount = 7;
            //int headerAndFooterRowCount = ReportHeaderAndTableHeader + FooterRowCount;
            //int maxRowPerPage = 63;
            //var data = new object[maxRowPerPage, 10];
            //string tableHeaderRows = "8:12";
            //int pageNo = 0;
            //int localRowCount = 0;
            //int startCellRowCount = ReportHeaderAndTableHeader + 1;
            //int endCellRowCount = 0;
            //int sumStartColumnNumber = 3;
            //try
            //{
            //    xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
            //    xlsht.Cells[9, 2].Value = fromDate;
            //    xlsht.Cells[10, 2].Value = toDate;
            //    xlsht.Cells[8, 5].Value = BatchFilter;
            //    xlsht.Cells[8, 5].WrapText = true;
            //    xlsht.Cells[9, 5].Value = ModelFilter;
            //    xlsht.Cells[8].Rows.AutoFit();
            //    xlsht.Cells[5].Columns.AutoFit();
            //    double[] totalReportSum = new double[dataTablevalue.Columns.Count - 1];
            //    for (int i = 0; i < dataTablevalue.Rows.Count; i++)
            //    {
            //        if (localRowCount == 0)
            //        {
            //            data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
            //        }
            //        for (int j = 0; j < dataTablevalue.Columns.Count; j++)
            //        {
            //            try
            //            {

            //                object value = dataTablevalue.Rows[i][j];
            //                data[localRowCount, j] = value;

            //            }
            //            catch (Exception ex)
            //            {

            //                string exc = ex.ToString();
            //            }

            //        }
            //        if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
            //        {
            //            var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
            //            endCellRowCount = startCellRowCount + localRowCount + 2;
            //            var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
            //            var writeRange = xlsht.get_Range(startCell, endCell);
            //            Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
            //            cellFormateToCopy.Copy();

            //            //Paste format only to the cell C5
            //            writeRange.PasteSpecial(XlPasteType.xlPasteFormats);                        
            //            double[] csum = new double[dataTablevalue.Columns.Count - 1];
            //            for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
            //            {
            //                csum[k - 1] = 0;
            //                for (int j = 0; j <= localRowCount; j++)
            //                    csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
            //            }
            //            data[localRowCount + 1, 0] = "Page Total";
            //            for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
            //            {
            //                data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
            //                totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
            //            }
            //            writeRange.Value = data;
            //            writeRange.Columns[2, Type.Missing].NumberFormat = "MM/DD/YYYY";
            //            writeRange.Columns[sumStartColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
            //            Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
            //            lastRowRange.Font.Bold = true;
            //            if (i < dataTablevalue.Rows.Count - 1)
            //            {
            //                Range from = xlsht.Range[tableHeaderRows];
            //                string range = (endCellRowCount + FooterRowCount + ReportHeader).ToString() + ":" + (endCellRowCount + FooterRowCount + ReportHeader + TableHeader).ToString();
            //                Range to = xlsht.Range[range];
            //                from.Copy(to);
            //                from = null;
            //                to = null;
            //            }
            //            startCellRowCount = endCellRowCount + headerAndFooterRowCount;
            //            data = null;
            //            data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
            //            localRowCount = 0;
            //            continue;
            //        }
            //        if (i == dataTablevalue.Rows.Count - 1)
            //        {
            //            var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
            //            endCellRowCount = startCellRowCount + localRowCount + 3;
            //            var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
            //            var writeRange = xlsht.get_Range(startCell, endCell);
            //            Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count];
            //            cellFormateToCopy.Copy();
            //            writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
            //            double[] csum = new double[dataTablevalue.Columns.Count - 1];
            //            for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
            //            {
            //                csum[k - 1] = 0;
            //                for (int j = 0; j <= localRowCount; j++)
            //                    csum[k - sumStartColumnNumber] = csum[k - sumStartColumnNumber] + double.Parse(data[j, k].ToString());
            //            }
            //            data[localRowCount + 1, 0] = "Page Total";
            //            data[localRowCount + 2, 0] = " Report Total";
            //            for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
            //            {
            //                data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
            //                totalReportSum[k - sumStartColumnNumber] = totalReportSum[k - sumStartColumnNumber] + csum[k - sumStartColumnNumber];
            //                data[localRowCount + 2, k] = totalReportSum[k - sumStartColumnNumber];
            //            }
            //            writeRange.Value = data;
            //            writeRange.Columns[2, Type.Missing].NumberFormat = "MM/DD/YYYY";
            //            writeRange.Columns[sumStartColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
            //            Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 2];
            //            lastRowRange.Font.Bold = true;
            //            lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
            //            lastRowRange.Font.Bold = true;
            //        }
            //        localRowCount++;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    string st = ex.ToString();
            //}
            //finally
            //{
            //    xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
            //    xlWorkBook.Close(true, misValue, misValue);
            //    xlApp.Quit();
            //    releaseObject(xlsht);
            //    releaseObject(xlWorkBook);
            //    releaseObject(xlApp);
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            const int TableHeader = 5;
            const int ReportHeader = 7;
            const int FooterRowCount = 9;
            const int maxRowPerPage = 37;
            const int dateCulumnNumber = 2;
            const int timeColumnNumber = 3;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "8:12";
            int localRowCount = 1;
            int startCellRowCount = TableHeader + ReportHeader + 1;
            int endCellRowCount = 0;
            try
            {
                xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 2].Value = fromDate;
                xlsht.Cells[10, 2].Value = toDate;
                double[] totalReportSum = new double[dataTablevalue.Columns.Count - 1];
                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 1)
                    {
                        data = new object[maxRowPerPage + 1, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {

                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount - 1, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[TableHeader + ReportHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();

                        //Paste format only to the cell C5
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        writeRange.Value = data;
                        writeRange.Columns[dateCulumnNumber, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[timeColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        startCellRowCount = endCellRowCount + FooterRowCount + TableHeader + ReportHeader + 1;
                        if (i < dataTablevalue.Rows.Count - 1)
                        {
                            Range from = xlsht.Range[tableHeaderRows];
                            string range = (endCellRowCount + FooterRowCount + ReportHeader + 1).ToString() + ":" + (startCellRowCount - 1).ToString();
                            Range to = xlsht.Range[range];
                            from.Copy(to);
                            from = null;
                            to = null;
                        }

                        data = null;
                        data = new object[maxRowPerPage + 1, dataTablevalue.Columns.Count];
                        localRowCount = 1;
                        continue;
                    }
                    if (i == dataTablevalue.Rows.Count - 1)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount;
                        var endCell = (Range)xlsht.Cells[endCellRowCount-1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        Range cellFormateToCopy = (Range)xlsht.Cells[TableHeader + ReportHeader + 1, dataTablevalue.Columns.Count];
                        cellFormateToCopy.Copy();
                        writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        writeRange.Value = data;
                        writeRange.Columns[dateCulumnNumber, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[timeColumnNumber, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                    }
                    localRowCount++;
                }

            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void writeGraph(double[] reqAndActualValues, System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate, string BatchFilter, string ModelFilter)
        {
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            int headerRowCount = 13;
            int headerAndFooterRowCount = 18;
            int maxRowPerPage = 63;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "11:12";
            int pageNo = 0;
            int localRowCount = 0;
            int startCellRowCount = 18;
            int endCellRowCount = 0;
            try
            {


                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 0)
                    {
                        data = new object[dataTablevalue.Rows.Count + 1, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {

                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i != dataTablevalue.Rows.Count - 1)
                        localRowCount++;
                }
                Range chartRange;
                var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                endCellRowCount = startCellRowCount + localRowCount;
                var endCell = (Range)xlsht.Cells[endCellRowCount, dataTablevalue.Columns.Count];
                var writeRange = xlsht.get_Range(startCell, endCell);
                writeRange.Value = data;

                ChartObjects xlCharts = (ChartObjects)xlsht.ChartObjects(Type.Missing);
                ChartObject myChart = (ChartObject)xlCharts.Add(190, 350, 798, 450);
                Chart chartPage = myChart.Chart;
                var seriesRangeYAaxisStart = (Range)xlsht.Cells[startCellRowCount, dataTablevalue.Columns.Count];
                var seriesRangeYAaxisEnd = (Range)xlsht.Cells[endCellRowCount, dataTablevalue.Columns.Count];
                Range cellFormateToCopy = (Range)xlsht.Cells[headerRowCount, 2];
                cellFormateToCopy.Copy();
                xlsht.get_Range(seriesRangeYAaxisStart, seriesRangeYAaxisEnd).PasteSpecial(XlPasteType.xlPasteFormats);

                chartPage.SetSourceData(xlsht.get_Range(seriesRangeYAaxisStart, seriesRangeYAaxisEnd), XlRowCol.xlColumns);
                chartPage.ChartType = XlChartType.xlLine;
                Series ser = (Series)chartPage.SeriesCollection(1);
                var seriesRangeXAaxisStart = (Range)xlsht.Cells[startCellRowCount, dataTablevalue.Columns.Count - 1];
                var seriesRangeXAaxisEnd = (Range)xlsht.Cells[endCellRowCount, dataTablevalue.Columns.Count - 1];
                cellFormateToCopy = (Range)xlsht.Cells[headerRowCount, 1];
                cellFormateToCopy.Copy();
                xlsht.get_Range(seriesRangeXAaxisStart, seriesRangeXAaxisEnd).PasteSpecial(XlPasteType.xlPasteFormats);
                ser.XValues = xlsht.get_Range(seriesRangeXAaxisStart, seriesRangeXAaxisEnd);
                chartPage.HasLegend = false;
                Axis yaxis = (Axis)chartPage.Axes(
        XlAxisType.xlValue,
        XlAxisGroup.xlPrimary);

                yaxis.HasTitle = true;
                yaxis.AxisTitle.Text = "Current";
                Axis xaxis = (Axis)chartPage.Axes(
      XlAxisType.xlCategory,
      XlAxisGroup.xlPrimary);

                xaxis.HasTitle = true;
                xaxis.AxisTitle.Text = "Date Time";
            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlsht.Cells[8, 10].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 10].Value = fromDate;
                xlsht.Cells[10, 10].Value = toDate;
                xlsht.Cells[11, 10].Value = BatchFilter;
                xlsht.Cells[12, 10].Value = ModelFilter;
                xlsht.Cells[15, 9].Value = reqAndActualValues[0].ToString();
                xlsht.Cells[16, 9].Value = reqAndActualValues[1].ToString();
                xlsht.Cells[17, 9].Value = reqAndActualValues[2].ToString();
                xlsht.Cells[15, 10].Value = reqAndActualValues[3].ToString();
                xlsht.Cells[16, 10].Value = reqAndActualValues[4].ToString();
                xlsht.Cells[17, 10].Value = reqAndActualValues[5].ToString();
                if ((reqAndActualValues[3] - reqAndActualValues[0]) <= MainWindow.ahToll && (reqAndActualValues[3] - reqAndActualValues[0]) >= -MainWindow.ahToll)
                {
                    xlsht.Cells[15, 11].Value = "OK";
                }
                else
                {
                    xlsht.Cells[15, 11].Value = "NOTOK";
                }
                if ((reqAndActualValues[4] - reqAndActualValues[1]) <= MainWindow.voltageToll && (reqAndActualValues[4] - reqAndActualValues[1]) >= -MainWindow.voltageToll)
                {
                    xlsht.Cells[16, 11].Value = "OK";
                }
                else
                {
                    xlsht.Cells[16, 11].Value = "NOTOK";
                }
                if ((reqAndActualValues[5] - reqAndActualValues[2]) <= MainWindow.currentToll && (reqAndActualValues[5] - reqAndActualValues[2]) >= -MainWindow.currentToll)
                {
                    xlsht.Cells[17, 11].Value = "OK";
                }
                else
                {
                    xlsht.Cells[17, 11].Value = "NOTOK";
                }
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static void releaseObject(object obj)
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

        public static void CopyFormatFromRowToRange(Application xlsApp, Workbook xlsWorkbook,Worksheet xlsht, int sourceRowNumber, int startTargetRowNumber, int endTargetRowNumber, int startTargetColumnNumber, int endTargetColumnNumber)
        {
            // Define the range from which you want to copy the format (entire row)
            Range sourceRowRange = xlsht.Rows[sourceRowNumber];

            // Calculate the number of columns to copy
            int numColumnsToCopy = endTargetColumnNumber - startTargetColumnNumber + 1;

            // Calculate the number of rows to copy
            int numRowsToCopy = endTargetRowNumber - startTargetRowNumber + 1;

            // Get the target range to copy format (same number of columns as the source row)
            Range targetRowRange = xlsht.Range[xlsht.Cells[startTargetRowNumber, startTargetColumnNumber], xlsht.Cells[startTargetRowNumber + numRowsToCopy - 1, startTargetColumnNumber + numColumnsToCopy - 1]];

            // Copy the format from the source row to the target range
            sourceRowRange.Copy();
            targetRowRange.PasteSpecial(XlPasteType.xlPasteFormats);

            // Clear the clipboard after pasting the format
            xlsApp.CutCopyMode = 0;
        }
        public static void writeAlarmReportInLandscap(System.Data.DataTable dataTablevalue, string Filepath, string SheetName, string fromDate, string toDate)
        {
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Open(Filepath);
            Worksheet xlsht = (Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlApp.Visible = false;
            int TableHeader = 5;
            int ReportHeader = 7;
            int ReportHeaderAndTableHeader = TableHeader + ReportHeader;
            int FooterRowCount = 7;
            int headerAndFooterRowCount = ReportHeaderAndTableHeader + FooterRowCount;
            int maxRowPerPage = 37;
            var data = new object[maxRowPerPage, 10];
            string tableHeaderRows = "8:12";
            int pageNo = 0;
            int localRowCount = 0;
            int startCellRowCount = ReportHeaderAndTableHeader + 1;
            int endCellRowCount = 0;
            int sumStartColumnNumber = 2;
            int maxSumStartColumnNumber = 2;
            try
            {
                xlsht.Cells[8, 2].Value = DateTime.Now.ToString();
                xlsht.Cells[9, 2].Value = fromDate;
                xlsht.Cells[10, 2].Value = toDate;
                object[] totalReportSum = new object[dataTablevalue.Columns.Count - 1];
                for (int i = 0; i < dataTablevalue.Rows.Count; i++)
                {
                    if (localRowCount == 0)
                    {
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                    }
                    for (int j = 0; j < dataTablevalue.Columns.Count; j++)
                    {
                        try
                        {

                            object value = dataTablevalue.Rows[i][j];
                            data[localRowCount, j] = value;

                        }
                        catch (Exception ex)
                        {

                            string exc = ex.ToString();
                        }

                    }
                    if (i > 0 && ((i + 1) % maxRowPerPage) == 0)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 2;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        object[] csum = new object[dataTablevalue.Columns.Count - 1];
                        int sourceRowNumber = ReportHeaderAndTableHeader + 1; // Change this to the desired source row number
                        int startTargetRowNumber = ReportHeaderAndTableHeader + 2; // Change this to the desired start row number for applying the format
                        int endTargetRowNumber = endCellRowCount - 1; // Change this to the desired end row number for applying the format
                        int startTargetColumnNumber = 1; // Change this to the desired start column number for applying the format
                        int endTargetColumnNumber = dataTablevalue.Columns.Count; // Change this to the desired end column number for applying the format

                        CopyFormatFromRowToRange(xlApp, xlWorkBook, xlsht, sourceRowNumber, startTargetRowNumber, endTargetRowNumber, startTargetColumnNumber, endTargetColumnNumber);

                        for (int k = sumStartColumnNumber; k <= maxSumStartColumnNumber + 1; k++)
                        {
                            TimeSpan timeSum = TimeSpan.Zero; // Initialize the sum to zero as TimeSpan
                            double doubleSum = 0.0;
                            for (int j = 0; j <= localRowCount; j++)
                            {
                                string timeStr = data[j, k].ToString(); // Assuming data[j, k] is a string representing time in the format "hh:mm:ss.fff"
                                TimeSpan timeValue;

                                // Convert the time string to TimeSpan
                                if (TimeSpan.TryParseExact(timeStr, @"hh\:mm\:ss\.fff", null, out timeValue))
                                {
                                    timeSum += timeValue; // Add the TimeSpan to the sum

                                    csum[k - sumStartColumnNumber] = timeSum;
                                }
                                if (double.TryParse(timeStr, out double doubleValue))
                                {
                                    doubleSum += doubleValue; // Add the double value to the sum

                                    csum[k - sumStartColumnNumber] = doubleSum;
                                }
                            }

                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        for (int k = sumStartColumnNumber; k < dataTablevalue.Columns.Count - 1; k++)
                        {
                            data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            if (totalReportSum[k - sumStartColumnNumber] != null)
                                totalReportSum[k - sumStartColumnNumber] = (double)totalReportSum[k - sumStartColumnNumber] + (double)csum[k - sumStartColumnNumber];
                            else
                                totalReportSum[k - sumStartColumnNumber] = (double)csum[k - sumStartColumnNumber];
                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[2, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                        if (i < dataTablevalue.Rows.Count - 1)
                        {
                            Range from = xlsht.Range[tableHeaderRows];
                            string range = (endCellRowCount + FooterRowCount + ReportHeader).ToString() + ":" + (endCellRowCount + FooterRowCount + ReportHeader + TableHeader).ToString();
                            Range to = xlsht.Range[range];
                            from.Copy(to);
                            from = null;
                            to = null;
                        }
                        startCellRowCount = endCellRowCount + headerAndFooterRowCount;
                        data = null;
                        data = new object[maxRowPerPage + 2, dataTablevalue.Columns.Count];
                        localRowCount = 0;
                        continue;
                    }
                    if (i == dataTablevalue.Rows.Count - 1)
                    {
                        var startCell = (Range)xlsht.Cells[startCellRowCount, 1];
                        endCellRowCount = startCellRowCount + localRowCount + 3;
                        var endCell = (Range)xlsht.Cells[endCellRowCount - 1, dataTablevalue.Columns.Count];
                        var writeRange = xlsht.get_Range(startCell, endCell);
                        //Range cellFormateToCopy = (Range)xlsht.Cells[ReportHeaderAndTableHeader + 1, dataTablevalue.Columns.Count+1];
                        //cellFormateToCopy.Copy();
                        //writeRange.PasteSpecial(XlPasteType.xlPasteFormats);
                        object[] csum = new object[dataTablevalue.Columns.Count - 1];
                        int sourceRowNumber = ReportHeaderAndTableHeader + 1; // Change this to the desired source row number
                        int startTargetRowNumber = ReportHeaderAndTableHeader + 2; // Change this to the desired start row number for applying the format
                        int endTargetRowNumber = endCellRowCount - 1; // Change this to the desired end row number for applying the format
                        int startTargetColumnNumber = 1; // Change this to the desired start column number for applying the format
                        int endTargetColumnNumber = dataTablevalue.Columns.Count; // Change this to the desired end column number for applying the format

                        CopyFormatFromRowToRange(xlApp, xlWorkBook, xlsht, sourceRowNumber, startTargetRowNumber, endTargetRowNumber, startTargetColumnNumber, endTargetColumnNumber);
                        for (int k = sumStartColumnNumber; k <= maxSumStartColumnNumber+1; k++)
                        {
                            TimeSpan timeSum = TimeSpan.Zero; // Initialize the sum to zero as TimeSpan
                            double doubleSum = 0.0;
                            for (int j = 0; j <= localRowCount; j++)
                            {
                                string timeStr = data[j, k].ToString(); // Assuming data[j, k] is a string representing time in the format "hh:mm:ss.fff"
                                TimeSpan timeValue;

                                // Convert the time string to TimeSpan
                                if (TimeSpan.TryParseExact(timeStr, @"hh\:mm\:ss\.fff", null, out timeValue))
                                {
                                    timeSum += timeValue; // Add the TimeSpan to the sum

                                    csum[k - sumStartColumnNumber] = timeSum;
                                }
                                if (double.TryParse(timeStr, out double doubleValue))
                                {
                                    doubleSum += doubleValue; // Add the double value to the sum

                                    csum[k - sumStartColumnNumber] = doubleSum;
                                }
                            }
                              
                        }
                        data[localRowCount + 1, 0] = "Page Total";
                        data[localRowCount + 2, 0] = " Report Total";
                        for (int k = sumStartColumnNumber; k <= dataTablevalue.Columns.Count - 1; k++)
                        {
                            //data[localRowCount + 1, k] = csum[k - sumStartColumnNumber];
                            if (csum[k - sumStartColumnNumber] is TimeSpan timeSpanValue2)
                                data[localRowCount + 1, k] = timeSpanValue2.ToString();
                            if (csum[k - sumStartColumnNumber] is double doubtValue2)
                                data[localRowCount + 1, k] = doubtValue2.ToString();
                            if (totalReportSum[k - sumStartColumnNumber] != null)
                            {
                                if (csum[k - sumStartColumnNumber] is TimeSpan timeSpanValue)
                                    totalReportSum[k - sumStartColumnNumber] = (TimeSpan)totalReportSum[k - sumStartColumnNumber] + timeSpanValue;
                                if (csum[k - sumStartColumnNumber] is double doubtValue)
                                    totalReportSum[k - sumStartColumnNumber] = (double)totalReportSum[k - sumStartColumnNumber] + doubtValue;
                            }
                            else
                            {
                                if (csum[k - sumStartColumnNumber] is TimeSpan timeSpanValue)
                                        totalReportSum[k - sumStartColumnNumber] = timeSpanValue;
                                if (csum[k - sumStartColumnNumber] is double doubtValue)
                                    totalReportSum[k - sumStartColumnNumber] = doubtValue;
                            }
                            if (csum[k - sumStartColumnNumber] is TimeSpan timeSpanValue1)
                                data[localRowCount + 2, k] = timeSpanValue1.ToString();
                            if (csum[k - sumStartColumnNumber] is double doubtValue1)
                                data[localRowCount + 2, k] = doubtValue1.ToString();

                        }
                        writeRange.Value = data;
                        writeRange.Columns[1, Type.Missing].NumberFormat = "MM/DD/YYYY";
                        writeRange.Columns[2, Type.Missing].NumberFormat = "hh:mm:ss AM/PM";
                        Range lastRowRange = (Range)xlsht.Rows[endCellRowCount - 2];
                        lastRowRange.Font.Bold = true;
                        lastRowRange = (Range)xlsht.Rows[endCellRowCount - 1];
                        lastRowRange.Font.Bold = true;
                    }
                    localRowCount++;
                }

            }
            catch (Exception ex)
            {
                string st = ex.ToString();
            }
            finally
            {
                xlWorkBook.SaveAs(SheetName, XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, XlSaveAsAccessMode.xlExclusive, XlSaveAsAccessMode.xlNoChange, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                releaseObject(xlsht);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
