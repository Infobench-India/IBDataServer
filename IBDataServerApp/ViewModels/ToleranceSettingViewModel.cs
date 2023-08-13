using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IBDataServerApp.Models;
using IBDataServerApp.Utils;
using IBDataServerApp.Views;
using Newtonsoft.Json.Linq;

namespace IBDataServerApp.ViewModels
{
    public class ToleranceSettingViewModel : ViewModelBase
    {
        #region Parameters

        public DataServerModal dataServerModal { get;}= new DataServerModal();

        private double ah;
        IBTCPServer ibTCPServer;
        public double AHProperty
        {
            get { return ah; }
            set { ah = value; NotifyPropertyChanged(); }
        }

        private double voltage;

        public double VoltageProperty
        {
            get { return voltage; }
            set { voltage = value; NotifyPropertyChanged(); }
        }

        private double current;

        public double CurrentProperty
        {
            get { return current; }
            set { current = value; NotifyPropertyChanged(); }
        }


        private string connectId;

        public string ConnectId
        {
            get { return connectId; }
            set { connectId = value; NotifyPropertyChanged(); }
        }
        
        #endregion

        #region Constructors
        public ToleranceSettingViewModel()
        {
            // DialogService is used to handle dialogs
            SaveCommand = new RelayCommand<object>(Save);
            if (ibTCPServer == null)
            {
                ibTCPServer = new IBTCPServer();
            }

            ibTCPServer.getIPAddress(dataServerModal);
            ibTCPServer.OnStart();
        }

        #endregion

        #region command
        public ICommand SaveCommand { get; set; }
        #endregion

        public void Save(object obj)
        {
            try
            {

                ibTCPServer.getIPAddress(dataServerModal);
                //MainWindow.ahToll = AHProperty;
                //MainWindow.voltageToll = VoltageProperty;
                //MainWindow.currentToll = CurrentProperty;

                //MainWindow.JTolerance.Remove("ahToll");
                //MainWindow.JTolerance.Remove("voltageToll");
                //MainWindow.JTolerance.Remove("currentToll");
                //MainWindow.JTolerance.Add("ahToll", MainWindow.ahToll);
                //MainWindow.JTolerance.Add("voltageToll", MainWindow.voltageToll);
                //MainWindow.JTolerance.Add("currentToll", MainWindow.currentToll);
                //string encoded = EncodeDecode.EncodePasswordToBase64(MainWindow.JTolerance.ToString());
                //System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\tolerance.key", encoded);
                if (ibTCPServer==null)
                {
                    ibTCPServer = new IBTCPServer();
                }
                ibTCPServer.OnStop();
                ibTCPServer.OnStart();
                dataServerModal.ErrorMessage = "Server Started Successfully";
            }
            catch (Exception ex)
            {
                Helper.WriteLogMsg("No any up network is found: "+ ex.ToString());
                dataServerModal.ErrorMessage = "Failed To Start Server";
            }
            
        }

    }
}
