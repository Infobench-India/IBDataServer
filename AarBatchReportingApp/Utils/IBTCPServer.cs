using AarBatchReportingApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AarBatchReportingApp.Utils
{
    class IBTCPServer
    {
        public long connectId = 0;
        public static string selectedIP ="";
        public static DataServerModal dataServerModalTemp;
        private System.Timers.Timer timer1 = null;
            ServerHelper sw;
            public IBTCPServer()
            {
            }
            public void OnStart()
            {
                try
                {
                    if (!LicenseHelper.IsValid() || !Helper.FileValid())
                        return;
                    timer1 = new System.Timers.Timer();
                    this.timer1.Interval = Convert.ToDouble(Helper.lines[2]) * 60 * 1000;
                    this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);
                    timer1.Enabled = true;
                    if (sw == null)
                    {
                        sw = new ServerHelper();
                        OnServerStart();
                    }
                }
                catch (Exception ex)
                {
                    Helper.WriteErrorLog(ex);
                }

            }
        public void OnStop()
            {
                try
                {
                    if (sw != null && timer1 != null)
                    {
                        OnServerStop();
                        timer1.Enabled = false;
                    }
                    Helper.WriteLogMsg("Infobench Data Server Stopped");
                dataServerModalTemp.ServerStatus = "Infobench Data Server Stopped";
                }
                catch (Exception ex) { string errormsg = ex.ToString(); }
            }
            protected void timer1_Tick(object sender, ElapsedEventArgs e)
            {
                try
                {
                    if (!LicenseHelper.IsValid() || !Helper.FileValid())
                        return;
                    CreatAndSendQuery();
                }
                catch (Exception ex)
                { Helper.WriteErrorLog(ex); }
            }
            ///////////////////////////////////////////////////////////
            private int MaxConnected = 400;
            private int HighLightDelay = 300;

            private Encoding ASCII = Encoding.ASCII;

            private static AutoResetEvent JobDone = new AutoResetEvent(false);

            private TcpListener tcpLsn;
            private Hashtable socketHolder = new Hashtable();
            private Hashtable threadHolder = new Hashtable();
            private Hashtable userHolder = new Hashtable();
            bool keepUser;
            private Thread fThd;
            private bool WaitForClient = true;
            //public void InitializeServer()
            //{

            //    tcpLsn = new TcpListener(65535);
            //    tcpLsn.Start();
            //    Helper.WriteLogMsg("Listen at: " + tcpLsn.LocalEndpoint.ToString());
            //    Thread tcpThd = new Thread(new ThreadStart(WaitingForClient));
            //    threadHolder.Add(connectId, tcpThd);
            //    tcpThd.Start();
            //}
            public void WaitingForClient()
            {
                while (WaitForClient)
                {
                    // Accept will block until someone connects
                    Socket sckt = tcpLsn.AcceptSocket();
                    //if (connectId < 10000)
                    //    Interlocked.Increment(ref connectId);
                    //else
                    connectId = 1;
                    if (socketHolder.Count < MaxConnected)
                    {
                        while (connectId < 10000)
                        {
                            if (!socketHolder.Contains(connectId))
                            {
                                break;
                            }
                            else
                            {
                                Interlocked.Increment(ref connectId);
                                dataServerModalTemp.ClientCount = connectId.ToString();

                            }
                        }
                        // Change to support more HMI Connection
                        if (connectId > 5)
                            return;
                        Thread td = new Thread(new ThreadStart(ReadSocket));
                        lock (this)
                        {
                            // it is used to keep connected Sockets
                            socketHolder.Add(connectId, sckt);
                            // it is used to keep the active thread
                            threadHolder.Add(connectId, td);
                            dataServerModalTemp.ClientCount = connectId.ToString();
                    }
                        td.Start();
                    }
                }
            }
            public void ReadSocket()
            {
                // realId will be not changed for each thread, 
                // but connectId is changed. it can't be used to delete object from Hashtable
                long realId = connectId;
                int ind = -1;
                Socket s = (Socket)socketHolder[realId];
                while (true)
                {
                    if (s.Connected)
                    {
                        Byte[] receive = new Byte[1048576];//37] ;
                        try
                        {
                            // Receive will block until data coming
                            // ret is 0 or Exception happen when Socket connection is broken
                            int ret = s.Receive(receive, receive.Length, 0);
                            if (ret > 0)
                            {
                                string tmp = null;
                                tmp = SerializeHelper.ByteArrayToStr(receive);
                                if (tmp.Length > 0)
                                {
                                    DateTime now1 = DateTime.Now;
                                    String strDate;
                                    strDate = now1.ToShortDateString() + " "
                                                    + now1.ToLongTimeString();
                                    string[] credentitialVales = tmp.Substring(0, tmp.IndexOf('<')).Split('$');
                                    string recievedDataTableXml = tmp.Substring(tmp.IndexOf('<'));
                                    dataServerModalTemp.Recieved = recievedDataTableXml;
                                    if (credentitialVales.Length < 2)
                                        return;
                                    int code = checkUserInfo(credentitialVales[0], credentitialVales[1]);
                                    if (code == 2)
                                    {
                                        userHolder.Add(realId, credentitialVales[0]);
                                        Helper.WriteDebugLogMsg("User " + credentitialVales[0] + " is connected");
                                    }
                                    else if (code == 1)
                                    {
                                        if (credentitialVales.Length > 2 && credentitialVales[2] == "Response" && !string.IsNullOrEmpty(recievedDataTableXml))
                                        {
                                            new Thread(() =>
                                            {
                                                //DataTable recievedDataTable = SerializeHelper.XmlStringToDataTable(recievedDataTableXml);
                                                //recievedDataTable.TableName = credentitialVales[0] + "AuditLog";
                                                SQLServerDB.createTables(SerializeHelper.XmlStringToDataTable(recievedDataTableXml));
                                                Helper.WriteDebugLogMsg(recievedDataTableXml);
                                            }).Start();
                                        }
                                        else
                                        {
                                            string connFail = String.Format(":The user {0} is connected already", credentitialVales[0]);
                                            Byte[] byteData = SerializeHelper.StrToByteArray(connFail);
                                            s.Send(byteData, byteData.Length, 0);
                                            new Thread(() =>
                                            {
                                                userHolder.Remove(realId);
                                                socketHolder.Remove(realId);
                                                threadHolder.Remove(realId);
                                                CloseTheThread(realId);
                                            }).Start();
                                            RemoveAlreadyExistThread(credentitialVales[0]);
                                            break;
                                        }

                                    }
                                    else if (code == 0)
                                    {
                                        string connFail = String.Format(":The user {0} is invalidate", credentitialVales[0]);
                                        Byte[] byteData = SerializeHelper.StrToByteArray(connFail);
                                        s.Send(byteData, byteData.Length, 0);
                                        CloseTheThread(realId);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!s.Connected)
                            {
                                new Thread(() =>
                                {
                                    userHolder.Remove(realId);
                                    socketHolder.Remove(realId);
                                    threadHolder.Remove(realId);
                                    CloseTheThread(realId);
                                }).Start();
                                keepUser = false;
                                break;
                            }
                        }
                    }
                }
                CloseTheThread(realId);
            }
            private int checkUserInfo(string userId, string password)
            {
                //  check the userId and password first
                // ....

                if (password == "BeijerCollector@")// suppose it ok
                {
                    if (userHolder.ContainsValue(userId))
                    {
                        keepUser = true;
                        return 1; // user is login already
                    }
                    else
                        return 2; // user is vailidate

                }
                else
                    return 0; // user not in the database

            }
            private void CloseTheThread(long realId)
            {
                new Thread(() =>
                {
                    Socket skt = (Socket)socketHolder[realId];
                    if (skt != null && skt.Connected)
                        skt.Close();
                    Thread thd = (Thread)threadHolder[realId];
                    if (thd != null && thd.IsAlive)
                        thd.Abort();
                    lock (this)
                    {
                        socketHolder.Remove(realId);
                        threadHolder.Remove(realId);
                        userHolder.Remove(realId);
                    }
                    Interlocked.Decrement(ref connectId);

                });
            }

            private void OnStopServerOrExit()
            {
                if (fThd.IsAlive)
                    fThd.Abort();
                if (tcpLsn != null)
                    tcpLsn.Stop();
                foreach (Socket s in socketHolder.Values)
                {
                    if (s.Connected)
                        s.Close();
                }
                foreach (Thread t in threadHolder.Values)
                {
                    if (t.IsAlive)
                        t.Abort();
                }

            }
            public void OnServerStart()
            {
                if (tcpLsn == null)
                {
                    if (GetLocalIPAddress() != null)
                    {
                        WaitForClient = true;
                        tcpLsn = new TcpListener(GetLocalIPAddress(), GetLocalIPortNo());
                        tcpLsn.Start();
                        Thread tcpThd = new Thread(new ThreadStart(WaitingForClient));
                        threadHolder.Add(connectId, tcpThd);
                        tcpThd.Start();
                        Helper.WriteLogMsg("Listen at: " + tcpLsn.LocalEndpoint.ToString());
                    dataServerModalTemp.ServerStatus = "Listen at: " + tcpLsn.LocalEndpoint.ToString();
                    }
                    else
                    {
                        Helper.WriteLogMsg("No any up network is found: ");
                    }
                }
                else
                {
                    Helper.WriteLogMsg("TCP listener not started");
                }
            }
            public void OnServerStop()
            {
                StopServer();
            }
            private void StopServer()
            {
                try
                {
                    if (tcpLsn == null)
                        return;
                    WaitForClient = false;
                    foreach (Socket s in socketHolder.Values)
                    {
                        if (s.Connected)
                            s.Close();
                    }
                    foreach (Thread t in threadHolder.Values)
                    {
                        if (t.IsAlive)
                            t.Abort();
                    }
                    userHolder.Clear();
                    socketHolder.Clear();
                if (tcpLsn != null)
                    tcpLsn.Stop();
                    threadHolder.Clear();
                    tcpLsn = null;
                    sw = null; 
                    Helper.WriteLogMsg("Infobench Data Server Stopped");
                }
                catch (Exception ex)
                {
                    Helper.WriteErrorLog(ex);
                }
            }

            private void SendQueryToClient(string DataBaseName, string UserNameInput, string DesignedQuery)
            {
                try
                {
                    string buf;
                    string UserName = null;
                    long i = 1;
                    for (i = 1; i <= userHolder.Count; i++)
                    //foreach (string value in userHolder.Values)
                    {
                        if ((string)userHolder[i] == UserNameInput)
                        {
                            UserName = (string)userHolder[i];
                            break;
                        }
                    }
                    string Password = "BeijerCollector@";
                    string MoreValuePresent = "No";
                    string Value = DesignedQuery.Trim();
                    buf = String.Format("{0}${1}${2}${3}${4}${5}", UserName, Password, "Execute", DataBaseName, MoreValuePresent, Value);
                    SendDataToClient(UserName, buf);
                }
                catch (Exception ex)
                {
                    Helper.WriteErrorLog(ex);
                }
            }
            private void SendDataToClient(string clientName, string str)
            {
                long i = 1;
                for (i = 1; i <= userHolder.Count; i++)
                {
                    if ((string)userHolder[i] == clientName)
                    {
                        Socket s = (Socket)socketHolder[i];
                        if (s != null && s.Connected)
                        {
                            Byte[] byteData = SerializeHelper.StrToByteArray(str);
                            s.Send(byteData, byteData.Length, 0);
                        }
                    }
                }

            }

            public void CreatAndSendQuery()
            {
                Helper.readText();
                if (Helper.lines != null && Helper.lines.Length > 4)
                    Int16.TryParse(Helper.lines[4], out SQLServerDB.rowlimit);
                string buf = null;
                string str = null;
                long i = 1;
                for (i = 1; i <= userHolder.Count; i++)
                {
                    if ((string)userHolder[i] != null)
                    {
                        Socket s = (Socket)socketHolder[i];
                        if (s != null && s.Connected)
                        {
                            str = SQLServerDB.GenerateQueryAuditTrail((string)userHolder[i] + "AuditLog");
                            string Password = "BeijerCollector@";
                            string MoreValuePresent = "No";
                            buf = String.Format("{0}${1}${2}${3}${4}${5}", (string)userHolder[i], Password, "Execute", "AuditTrail.db", MoreValuePresent, str);
                            Byte[] byteData = SerializeHelper.StrToByteArray(buf);
                            s.Send(byteData, byteData.Length, 0);
                        }
                    }
                }
                Thread.Sleep(5000);
            }

            public static IPAddress GetLocalIPAddress()
            {
                try
                {
                    string[] ipAndPort = null;
                    if (!string.IsNullOrEmpty(Helper.lines[0]) && Helper.lines[0].Contains(":"))
                    {
                        ipAndPort = Helper.lines[0].Split(':');
                    }
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString() == ipAndPort[0])
                        {
                        selectedIP = ip.ToString();
                        dataServerModalTemp.IpAddress = selectedIP;
                        return ip;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helper.WriteErrorLog(ex);
                }
                return null;
            }

            public static int GetLocalIPortNo()
            {
                int port = 65535;
                try
                {
                    if (!string.IsNullOrEmpty(Helper.lines[0]) && Helper.lines[0].Contains(':'))
                    {
                        string[] ipAndPort = Helper.lines[0].Split(':');
                        int.TryParse(ipAndPort[1], out port);
                    }

                }
                catch (Exception ex)
                {
                    Helper.WriteErrorLog(ex);
                }
                return port;
            }
            private void RemoveAlreadyExistThread(string clientName)
            {
                long i = 1;
                for (i = 1; i <= userHolder.Count; i++)
                {
                    if ((string)userHolder[i] == clientName)
                    {
                        new Thread(() =>
                        {
                            userHolder.Remove(i);
                            socketHolder.Remove(i);
                            threadHolder.Remove(i);
                            CloseTheThread(i);
                        }).Start();
                        break;
                    }
                }
            }
        public void getIPAddress(DataServerModal dataServerModal)
        {
            dataServerModalTemp = dataServerModal;
        }

    }

}

