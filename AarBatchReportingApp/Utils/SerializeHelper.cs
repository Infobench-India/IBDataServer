using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AarBatchReportingApp.Utils
{
    internal static class SerializeHelper
    {
        public static string TableToXmlString(DataTable table)
        {

            string xml = string.Empty;
            try
            {
                if (table == null)
                {
                    return null;
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataTable));
                            xmlSerializer.Serialize(streamWriter, table);
                            xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Helper.WriteErrorLog(ex);
            }
            return xml;
        }
        public static DataTable XmlStringToDataTable(string dataTablesString)
        {

            StringReader theReader = new StringReader(dataTablesString);
            DataTable theDataTable = new DataTable();
            try
            {
                theDataTable.ReadXml(theReader);
                foreach (DataRow row in theDataTable.Rows)
                {
                    foreach (DataColumn col in theDataTable.Columns)
                    {
                        //test for null here
                        if (row[col] != null && !string.IsNullOrEmpty(row[col].ToString()) && row[col].ToString() == "-")
                        {
                            row[col] = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteErrorLog(ex);
            }
            return theDataTable;
        }
        public static byte[] StrToByteArray(string str)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static string ByteArrayToStr(byte[] barr)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(barr, 0, barr.Length);
        }
    }

}
