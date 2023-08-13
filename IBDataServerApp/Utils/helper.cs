using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBDataServerApp.Utils
{
    class Helper
    {
        private static string HMIStatus = "False";
        public static string[] lines = new string[] { };
        private static string valueQuery = "";
        private static byte[] bytes = new byte[] { };
        private static string[] ModbusAddress = new string[] { };
        private static ushort[] ModbusAddressInts = new ushort[] { };
        private static ushort[] ModbusAddressIntsSort = new ushort[] { };
        private static double[] shorts = new double[] { };
        private static float[] floats = new float[] { };
        public static void WriteErrorLog(Exception ex)
        {
            readText();
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ":" + ex.Source.ToString().Trim() + ":" + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex0)
            {
                string errormsg = ex0.ToString();
            }
        }
        public static void WriteDebugLogMsg(String str)
        {
            readText();
            if (lines.Length > 3 && lines[3] == "Yes")
            {
                readText();
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\DebugErrorLog.txt", true);
                    sw.WriteLine(DateTime.Now.ToString() + ":" + str);
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    string errormsg = ex.ToString();
                }
            }
        }
        public static void WriteLogMsg(String str)
        {

            readText();
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ":" + str);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                string errormsg = ex.ToString();
            }

        }

        public static void readText()
        {
            try
            {
                string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\ConfigFile.txt", Encoding.UTF8);
                lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\ConfigFile.txt", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                string errormsg = ex.ToString();
            }
        }
        public static bool FileValid()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\ConfigFile.txt"))
            {
                readText();
                UInt64 x = 0;
                if (lines.Length >= 2 && !string.IsNullOrEmpty(lines[1]) && !string.IsNullOrEmpty(lines[2]) && UInt64.TryParse(lines[2], out x) && x > 0)
                {
                    SQLServerDB.connString = lines[1];
                    return true;

                }
            }
            return false;
        }


    }

}
