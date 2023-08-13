using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IBDataServerApp.Views;
using log4net;

namespace IBDataServerApp.Utils
{
    public class SQLHelper
    {
        protected static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static DataTable HourlyReportLoad(string ReportCategory,DateTime AvailableFrom, DateTime AvailableTo, string connectionString)
        {
            try
            {
                string MonthName = string.Empty;
                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                StrQuery100 = @"SELECT
    [ACED_Area] AS [Area],
    [Alarm_Details] AS [Alarm],
    COUNT(*) AS [Number of Triggers],
    CONVERT(VARCHAR(12), MIN([Time]), 114) AS [Duration of Alarm]
FROM
    [sample_alarm]
WHERE
    CONVERT(DATETIME, [Date]) + CAST([Time] AS DATETIME) BETWEEN '2022-06-13 16:00:00' AND '2024-07-14 11:30:00'
GROUP BY
    [ACED_Area],
    [Alarm_Details]
ORDER BY
    [ACED_Area] ASC,
    [Alarm_Details] ASC;";
                Log.Debug(StrQuery100);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }



        public static DataTable AlarmReportLoad(string reportCategory, string selectedReport, DateTime AvailableFrom, DateTime AvailableTo, string connectionString)
        {
            try
            {
                string tableName = MainWindow.Reports[reportCategory][selectedReport]["tableName"].ToString();
                string MonthName = string.Empty;
                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                StrQuery100 = @"
    SELECT
        [ACED_Area] AS [Area],
        [Alarm_Details] AS [Alarm],
        COUNT(*) AS [Number of Triggers],
        CONVERT(VARCHAR(12), MIN(CONVERT(DATETIME, [Date]) + CAST([Time] AS DATETIME)), 114) AS [Duration of Alarm]
    FROM " + tableName + @"
    WHERE CONVERT(DATETIME, [Date]) + CAST([Time] AS DATETIME) BETWEEN '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + @"'
    GROUP BY
        [ACED_Area],
        [Alarm_Details]
    ORDER BY
        [ACED_Area] ASC,
        [Alarm_Details] ASC;"; ;
                Log.Debug(StrQuery100);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }


        public static string
            [] getBatchModelDailyBatchLoad(DateTime AvailableFrom, DateTime AvailableTo, string connectionString)
        {

            string[] returnResult = new string[2];
            try
            {
                string MonthName = string.Empty;

                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select [Batch_No],[Model] from Daily_Batch WHERE DATALENGTH([Batch_No]) > 0 AND DATALENGTH([Model]) > 0 AND cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                        {
                            returnResult[0] = dt100.Rows[0][0].ToString();
                            returnResult[1] = dt100.Rows[0][1].ToString();
                        }                        
                    }
                }
                return returnResult;
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return returnResult; }
        }

        public static DataTable DailyBatchLoad(DateTime AvailableFrom, DateTime AvailableTo, string connectionString, string batchFilter, string modelFilter)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        if(!string.IsNullOrEmpty(modelFilter)&& !string.IsNullOrEmpty(batchFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Peak_Current]
      ,[Actual_AH]
      ,[Required_AH]
      ,[Voltage]
      ,[Profile_Selected]
      ,[Batch_No] from Daily_Batch WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Model ='" + modelFilter + "' AND Batch_No = '" + batchFilter + "'  ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        else if (!string.IsNullOrEmpty(modelFilter) && string.IsNullOrEmpty(batchFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Peak_Current]
      ,[Actual_AH]
      ,[Required_AH]
      ,[Voltage]
      ,[Profile_Selected]
      ,[Batch_No] from Daily_Batch WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Model ='" + modelFilter + "'  ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        else if (string.IsNullOrEmpty(modelFilter) && !string.IsNullOrEmpty(batchFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Peak_Current]
      ,[Actual_AH]
      ,[Required_AH]
      ,[Voltage]
      ,[Profile_Selected]
      ,[Batch_No] from Daily_Batch WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'  ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        else if (string.IsNullOrEmpty(modelFilter) && string.IsNullOrEmpty(batchFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Peak_Current]
      ,[Actual_AH]
      ,[Required_AH]
      ,[Voltage]
      ,[Profile_Selected]
      ,[Batch_No] from Daily_Batch WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static DataTable SecBatchLoad(DateTime AvailableFrom, DateTime AvailableTo, string connectionString, string batchFilter, string modelFilter)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        if(string.IsNullOrEmpty(batchFilter) && string.IsNullOrEmpty(modelFilter))
                        StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Real_Current]
      ,[Voltage]
      ,[Batch_No] from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";
                        else if (!string.IsNullOrEmpty(batchFilter) && string.IsNullOrEmpty(modelFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Real_Current]
      ,[Voltage]
      ,[Batch_No] from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        else if (string.IsNullOrEmpty(batchFilter) && !string.IsNullOrEmpty(modelFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Real_Current]
      ,[Voltage]
      ,[Batch_No] from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Model = '" + modelFilter + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        else if (!string.IsNullOrEmpty(batchFilter) && !string.IsNullOrEmpty(modelFilter))
                        {
                            StrQuery100 = @"Select [Model]
      ,[DATE_COL1]
      ,[TIME_COL1]
      ,[Real_Current]
      ,[Voltage]
      ,[Batch_No] from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Model = '" + modelFilter + "' AND Batch_No = '" + batchFilter + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        }
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static DataTable DailyReportLoad(string ReportCategory,DateTime AvailableFrom, DateTime AvailableTo, string connectionString)
        {
            DateTime fromDate = Convert.ToDateTime(AvailableFrom);
            //DateTime.TryParse(AvailableFrom,out fromDate);
            string toDate = fromDate.ToString("yyyy/MM/dd");
            try
            {
                string MonthName = string.Empty;
                string StrQuery100 = string.Empty;
                StrQuery100 = @"Select [DATE_COL1]," + MainWindow.Reports[ReportCategory]["dailyReport"]["columns"].ToString() + " from " + MainWindow.Reports[ReportCategory]["dailyReport"]["tableName"].ToString() + " WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY DATE_COL1 ORDER BY DATE_COL1 ASC";
                
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        comm.CommandText = StrQuery100;
                        Log.Debug(StrQuery100);
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;
                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static string[] getBatchModelfromBatch_Sec(DateTime AvailableFrom, DateTime AvailableTo,string batchFilter, string connectionString)
        {

            string[] returnResult = new string[2];
            try
            {
                string MonthName = string.Empty;

                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select [Batch_No],[Model] from Batch_Sec WHERE DATALENGTH([Batch_No]) > 0  AND Batch_No = '" + batchFilter + "' AND DATALENGTH([Model]) > 0 AND cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";

                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                        {
                            returnResult[0] = dt100.Rows[0][0].ToString();
                            returnResult[1] = dt100.Rows[0][1].ToString();
                        }
                    }
                }
                return returnResult;
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return returnResult; }
        }

        public static DataTable DailyBatchGraphLoad(DateTime AvailableFrom, DateTime AvailableTo, string connectionString, string batchFilter, string modelFilter)
        {
            try
            {
                string MonthName = string.Empty;
                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) as 'DateTime' , Real_Current from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "' ORDER BY cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) ASC";
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;
                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static double[] getReqAndActualValues(DateTime AvailableFrom, DateTime AvailableTo, string connectionString, string batchFilter, string modelFilter)
        {

            double[] returnResult = new double[6];
            try
            {
                string MonthName = string.Empty;

                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select MAX(Req_AH) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[0] = (double)comm.ExecuteScalar();

                        StrQuery100 = @"Select MAX(Req_Volatge) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[1] = (double)comm.ExecuteScalar();

                        StrQuery100 = @"Select MAX(Req_Peak_Cuurent) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[2] = (double)comm.ExecuteScalar();

                        StrQuery100 = @"Select MAX(Actual_AH_Sec) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[3] = (double)comm.ExecuteScalar();

                        StrQuery100 = @"Select MAX(Actual_Voltage_Sec) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[4] = (double)comm.ExecuteScalar();

                        StrQuery100 = @"Select MAX(Actual_Peak_Current) from Batch_Sec WHERE cast(DATE_COL1 + ISNULL(' '+TIME_COL1,'') as datetime) between '" + AvailableFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + AvailableTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Batch_No = '" + batchFilter + "'";
                        comm.CommandText = StrQuery100;
                        returnResult[5] = (double)comm.ExecuteScalar();
                        conn.Close();
                        return returnResult;
                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return returnResult; }
        }
        
        public static DataTable GetSchedulerData(string connectionString)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select * from Schedule";
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static DataTable GetTodaysSchedulerData(string connectionString)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select * from TodaysSchedule";
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }


        public static DataTable GetAutoEmailData(string connectionString)
        {
            try
            {
                string MonthName = string.Empty;
                string StrQuery100 = string.Empty;
                DataTable dt100 = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"Select * from AutoEmailsRecipients";
                        comm.CommandText = StrQuery100;
                        SqlDataReader rd = comm.ExecuteReader();
                        dt100.Load(rd);
                        int c = dt100.Rows.Count;

                        conn.Close();
                        if (dt100.Rows.Count > 0)
                            return dt100;
                        else
                            return null;

                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); return null; }
        }

        public static void AddSchedulerData(string connectionString,string ReportCategory, string ReportType, string BatchNumber, string ModelNumber)
        {
            try
            {
                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES 
                       WHERE TABLE_NAME='Schedule') SELECT 1 ELSE SELECT 0";
                        comm.CommandText = StrQuery100;
                        int x = Convert.ToInt32(comm.ExecuteScalar());
                        if (x == 1)
                        {
                            StrQuery100 = "INSERT INTO Schedule (ReportCategory,ReportType,BatchNumber,ModelNumber) VALUES (@ReportType,@BatchNumber,@ModelNumber)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@ReportType", ReportType);
                            comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                            comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                            comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                            comm.ExecuteNonQuery();
                        }
                        else
                        {
                            StrQuery100 = @"CREATE TABLE Schedule (
SrNo int IDENTITY(1,1) PRIMARY KEY,
        ReportType varchar(64) not null,
BatchNumber varchar(64) not null,
ModelNumber varchar(64) not null
    )";
                            comm.CommandText = StrQuery100;
                            comm.ExecuteNonQuery();
                            StrQuery100 = @"INSERT INTO Schedule (ReportCategory,ReportType,BatchNumber,ModelNumber) VALUES (@ReportCategory,@ReportType,@BatchNumber,@ModelNumber)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@ReportType", ReportType);
                            comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                            comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                            comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                            comm.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
        }

        public static void AddTodaysSchedulerData(string connectionString,string ReportCategory, string ReportType, string BatchNumber, string ModelNumber, DateTime AvailableFrom, DateTime AvailableTo, DateTime ScheduleDateTime)
        {
            try
            {
                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES 
                       WHERE TABLE_NAME='TodaysSchedule') SELECT 1 ELSE SELECT 0";
                        comm.CommandText = StrQuery100;
                        int x = Convert.ToInt32(comm.ExecuteScalar());
                        if (x == 1)
                        {
                            StrQuery100 = "INSERT INTO TodaysSchedule (ReportCategory,ReportType,BatchNumber,ModelNumber,AvailableFrom,AvailableTo,ScheduleDateTime) VALUES (@ReportCategory,@ReportType,@BatchNumber,@ModelNumber,@AvailableFrom,@AvailableTo,@ScheduleDateTime)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                            comm.Parameters.AddWithValue("@ReportType", ReportType);
                            comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                            comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                            comm.Parameters.AddWithValue("@AvailableFrom", AvailableFrom);
                            comm.Parameters.AddWithValue("@AvailableTo", AvailableTo);
                            comm.Parameters.AddWithValue("@ScheduleDateTime", ScheduleDateTime);
                        
                            comm.ExecuteNonQuery();
                        }
                        else
                        {
                            StrQuery100 = @"CREATE TABLE TodaysSchedule (
SrNo int IDENTITY(1,1) PRIMARY KEY,
ReportCategory varchar(64) not null,
        ReportType varchar(64) not null,
 BatchNumber varchar(64) not null,
 ModelNumber varchar(64) not null,
AvailableFrom datetime not null,
AvailableTo datetime not null,
ScheduleDateTime datetime not null
    )";
                            comm.CommandText = StrQuery100;
                            comm.ExecuteNonQuery();
                            StrQuery100 = @"INSERT INTO TodaysSchedule (ReportCategory,ReportType,BatchNumber,ModelNumber,AvailableFrom,AvailableTo,ScheduleDateTime) VALUES (@ReportCategory,@ReportType,@BatchNumber,@ModelNumber,@AvailableFrom,@AvailableTo,@ScheduleDateTime)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                            comm.Parameters.AddWithValue("@ReportType", ReportType);
                            comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                            comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                            comm.Parameters.AddWithValue("@AvailableFrom", AvailableFrom);
                            comm.Parameters.AddWithValue("@AvailableTo", AvailableTo);
                            comm.Parameters.AddWithValue("@ScheduleDateTime", ScheduleDateTime);
                            comm.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
            finally
            {
                MainWindow.TodaysSchedules = GetTodaysSchedulerData(MainWindow.connectionString1);
            }
        }

        public static void AddAutoEmailData(string connectionString, string EmailAddress)
        {
            try
            {
                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES 
                       WHERE TABLE_NAME='AutoEmailsRecipients') SELECT 1 ELSE SELECT 0";
                        comm.CommandText = StrQuery100;
                        int x = Convert.ToInt32(comm.ExecuteScalar());
                        if (x == 1)
                        {
                            StrQuery100 = "INSERT INTO AutoEmailsRecipients (Emails) VALUES (@email)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@email", EmailAddress);
                            comm.ExecuteNonQuery();
                        }
                        else
                        {
                            StrQuery100 = @"CREATE TABLE AutoEmailsRecipients (
SrNo int IDENTITY(1,1) PRIMARY KEY,
        Emails varchar(64) not null
    )";
                            comm.CommandText = StrQuery100;
                            comm.ExecuteNonQuery();
                            StrQuery100 = @"INSERT INTO AutoEmailsRecipients (Emails) VALUES (@email)";
                            comm.CommandText = StrQuery100;
                            comm.Parameters.AddWithValue("@email", EmailAddress);
                            comm.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
        }

        public static void UpdateSchedulerData(string connectionString,string ReportCategory, string ReportType, string BatchNumber, string ModelNumber, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"UPDATE Schedule SET ReportCategory = @ReportCategory, ReportType = @ReportType,BatchNumber=@BatchNumber,ModelNumber=@ModelNumber WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.Parameters.AddWithValue("@ReportType", ReportType);
                        comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                        comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                        comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
        }

        public static void UpdateTodaysSchedulerDataFromScheduler(string connectionString,string ReportCategory, string ReportType, string BatchNumber, string ModelNumber, DateTime AvailableFrom, DateTime AvailableTo, DateTime ScheduleDateTime, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"UPDATE TodaysSchedule SET ReportCategory = @ReportCategory, ReportType = @ReportType,BatchNumber= @BatchNumber,ModelNumber=@ModelNumber,AvailableFrom = @AvailableFrom,AvailableTo=@AvailableTo,ScheduleDateTime=@ScheduleDateTime WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                        comm.Parameters.AddWithValue("@ReportType", ReportType);
                        comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                        comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                        comm.Parameters.AddWithValue("@AvailableFrom", AvailableFrom);
                        comm.Parameters.AddWithValue("@AvailableTo", AvailableTo);
                        comm.Parameters.AddWithValue("@ScheduleDateTime", ScheduleDateTime);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
            finally
            {
                // MainWindow.TodaysSchedules = GetTodaysSchedulerData(MainWindow.connectionString1);
            }
        }

        public static void UpdateTodaysSchedulerData(string connectionString,string ReportCategory, string ReportType, string BatchNumber, string ModelNumber, DateTime AvailableFrom, DateTime AvailableTo, DateTime ScheduleDateTime, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"UPDATE TodaysSchedule SET ReportCategory = @ReportCategory, ReportType = @ReportType,BatchNumber= @BatchNumber,ModelNumber=@ModelNumber,AvailableFrom = @AvailableFrom,AvailableTo=@AvailableTo,ScheduleDateTime=@ScheduleDateTime WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.Parameters.AddWithValue("@ReportType", ReportType);
                        comm.Parameters.AddWithValue("@BatchNumber", BatchNumber);
                        comm.Parameters.AddWithValue("@ModelNumber", ModelNumber);
                        comm.Parameters.AddWithValue("@AvailableFrom", AvailableFrom);
                        comm.Parameters.AddWithValue("@AvailableTo", AvailableTo);
                        comm.Parameters.AddWithValue("@ScheduleDateTime", ScheduleDateTime);
                        comm.Parameters.AddWithValue("@ReportCategory", ReportCategory);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
            finally
            {
                MainWindow.TodaysSchedules = GetTodaysSchedulerData(MainWindow.connectionString1);
            }
        }


        public static void UpdateAutoEmailData(string connectionString, string EmailAddress, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"UPDATE AutoEmailsRecipients SET Emails = @email WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.Parameters.AddWithValue("@email", EmailAddress);
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
        }

        public static void DeleteSchedulerData(string connectionString, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"DELETE FROM Schedule WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }

        }

        public static void DeleteTodaysSchedulerData(string connectionString, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"DELETE FROM TodaysSchedule WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }
            finally
            {
                MainWindow.TodaysSchedules = GetTodaysSchedulerData(MainWindow.connectionString1);
            }

        }

        public static void DeleteAutoEmailData(string connectionString, int SrNo)
        {
            try
            {
                string MonthName = string.Empty;


                string StrQuery100 = string.Empty;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = new SqlCommand())
                    {
                        comm.Connection = conn;
                        conn.Open();
                        StrQuery100 = @"DELETE FROM AutoEmailsRecipients WHERE SrNo='" + SrNo + "'";
                        comm.CommandText = StrQuery100;
                        comm.ExecuteNonQuery();
                        conn.Close();
                    }

                }
            }


            catch (Exception ex) { string errormsg = ex.ToString(); }

        }
    }
}
