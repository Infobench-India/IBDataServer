using IBDataServerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBDataServerApp.Models
{
    public class DataServerModal: ViewModelBase
    {
        private string ipAddress;
        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                    NotifyPropertyChanged();
            }
        }

        private string clientCount;
        public string ClientCount
        {
            get { return clientCount; }
            set
            {
                clientCount = value;
                NotifyPropertyChanged();
            }
        }

        private string recieved;
        public string Recieved
        {
            get { return recieved; }
            set
            {
                recieved = value;
                NotifyPropertyChanged();
            }
        }


        private string send;
        public string Send
        {
            get { return send; }
            set
            {
                send = value;
                NotifyPropertyChanged();
            }
        }

        private string serverStatus;
        public string ServerStatus
        {
            get { return serverStatus; }
            set
            {
                serverStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; NotifyPropertyChanged(); }
        }

    }
}
