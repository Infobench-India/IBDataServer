using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using log4net;
using System.Threading.Tasks;
using IBDataServerApp.ViewModels;
using System.Security.Principal;
using FoxLearn.License;

namespace IBDataServerApp.Utils
{
    public class LicenseHelper: ViewModelBase
    {
        private static void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
        public static bool IsValid()
        {
            if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Key.lic"))
            {
                CreateSerialNumberFile();
                Log.Info("Not Licensed, Share Serial.bnt file with Vender");
                return false;
            }
            GrantAccess(AppDomain.CurrentDomain.BaseDirectory);
            KeyManager km = new KeyManager(ComputerInfo.GetComputerId());
            LicenseInfo lic = new LicenseInfo();
            int value = km.LoadSuretyFile(string.Format(@"{0}\Key.lic", AppDomain.CurrentDomain.BaseDirectory), ref lic);
            string productKey = lic.ProductKey;
            if (km.ValidKey(ref productKey))
            {
                KeyValuesClass kv = new KeyValuesClass();
                if (km.DisassembleKey(productKey, ref kv))
                {
                    if (kv.Type == LicenseType.TRIAL  && kv.ProductCode == 10)
                    {

                        int result = DateTime.Compare(DateTime.Now.Date, kv.Expiration );
                        if (!(result < 0))
                        {
                            Log.Info("License Expired, Share Serial.bnt file with Vender");
                            CreateSerialNumberFile();
                            return false;
                        }
                        else
                            return true;
                    }
                    else if (kv.Type == LicenseType.FULL && kv.ProductCode == 10)
                    {
                        return true;
                    }
                    else
                    {
                        Log.Info("License Expired, Share Serial.bnt file with Vender");
                        CreateSerialNumberFile();
                        return false;
                    }
                }
                Log.Info("Not Licensed, Share Serial.bnt file with Vender");
                CreateSerialNumberFile();
                return false;
            }
            else
            {
                Log.Info("Invalid Licensed, Share Serial.bnt file with Vender");
                CreateSerialNumberFile();
                return false;

            }
        }
        private static void CreateSerialNumberFile()
        {
            string fileName = string.Format(@"{0}\Serial.bnt", AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                // Check if file already exists. If yes, delete it. 
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file 
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] serialNumber = new UTF8Encoding(true).GetBytes(FoxLearn.License.ComputerInfo.GetComputerId());
                    fs.Write(serialNumber, 0, serialNumber.Length);

                }
            }
            catch (Exception Ex)
            {
                string st = Ex.ToString();
            }
        }
    }
}
