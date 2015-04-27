//Coded ajay yadav in FD 2.0 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using Proxies;
using Microsoft.Win32;
using System.Threading;
using System.Data;
using faceboardpro;
using System.Text.RegularExpressions;

namespace Proxies
{
    public class ProxyManager:IProxyManager
    {
        public BaseLib.Events proxyCheckerEvent = null;

        #region Global Variables For ProxyChecker

        readonly object lockrThreadControllerProxyChecker = new object();

        public bool isStopProxyChecker = false;
        int countThreadControllerProxyChecker = 0;

        public List<Thread> lstThreadsProxyChecker = new List<Thread>();

        #endregion

        #region Property For Event Inviter

        public int NoOfThreadsEventInviter
        {
            get;
            set;
        }
        public static string ExportFilePathProxies
        {
            get;
            set;
        }

        #endregion

        public ProxyManager()
        {
            proxyCheckerEvent = new BaseLib.Events();
        }

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                proxyCheckerEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void StartProxyChecker(object proxyDetails)
        {
            try
            {
                if (isStopProxyChecker)
                {
                    return;
                }

                lstThreadsProxyChecker.Add(Thread.CurrentThread);
                lstThreadsProxyChecker = lstThreadsProxyChecker.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;

                Array arr =new object[1];

                arr = (Array)proxyDetails;


                string proxyDetailsTemp = Convert.ToString(arr.GetValue(0));
                string proxyIPAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUserName = string.Empty;
                string proxyProxyPassword = string.Empty;
                string proxyCheckingURL = string.Empty;

                if (proxyDetailsTemp.Contains(":"))
                {
                    string[] proxyDetailsArr = Regex.Split(proxyDetailsTemp, ":");

                    try
                    {
                        if (proxyDetailsArr.Length > 1)
                        {
                            try
                            {
                                proxyIPAddress = proxyDetailsArr[0];
                                proxyPort = proxyDetailsArr[1];
                                proxyUserName = proxyDetailsArr[2];
                                proxyProxyPassword = proxyDetailsArr[3];
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Proxy Is Not In Correct Format : " + proxyDetailsTemp);
                            GlobusLogHelper.log.Debug("Proxy Is Not In Correct Format : " + proxyDetailsTemp);

                            
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Proxy Is Not In Correct Format : " + proxyDetailsTemp);
                    GlobusLogHelper.log.Debug("Proxy Is Not In Correct Format : " + proxyDetailsTemp);
                }

                proxyCheckingURL = FBGlobals.Instance.fbhomeurl;

                //bool isWorkingProxy = CheckProxy(proxyIPAddress, proxyPort, proxyUserName, proxyProxyPassword, proxyCheckingURL);
                bool isWorkingProxy = CheckProxy(proxyIPAddress, Convert.ToInt32(proxyPort), proxyUserName, proxyProxyPassword, proxyCheckingURL);
                if (isWorkingProxy)
                {
                    GlobusLogHelper.log.Info("Proxy Is Working : " + proxyDetailsTemp);
                    GlobusLogHelper.log.Debug("Proxy Is Working : " + proxyDetailsTemp);

                    //ExportFilePathProxies


                    if (!string.IsNullOrEmpty(ExportFilePathProxies))
                    {
                        try
                        {
                            string commaSeparatedData = proxyIPAddress + "," + proxyPort + "," + proxyUserName + "," + proxyProxyPassword + "," + proxyCheckingURL+","+"Working";

                            string CSVHeader = "proxyIPAddress" + "," + "proxyPort" + "," + "proxyUserName" + ", " + "proxyProxyPassword" + "," + "proxyCheckingURL"+ "," +"ProxyStatus";

                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, commaSeparatedData, ExportFilePathProxies);

                            GlobusLogHelper.log.Info("Proxy Saved IN CSV File");

                            GlobusLogHelper.log.Debug("Proxy Info Saved In CSV");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    try
                    {
                        string commaSeparatedData = proxyIPAddress + "," + proxyPort + "," + proxyUserName + "," + proxyProxyPassword + "," + proxyCheckingURL + "," + "NonWorking";

                        string CSVHeader = "proxyIPAddress" + "," + "proxyPort" + "," + "proxyUserName" + ", " + "proxyProxyPassword" + "," + "proxyCheckingURL" + "," + "ProxyStatus";

                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, commaSeparatedData, ExportFilePathProxies);

                        GlobusLogHelper.log.Info("Proxy Saved IN CSV File");

                        GlobusLogHelper.log.Debug("Proxy Info Saved In CSV");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    
                    GlobusLogHelper.log.Info("Proxy Is Not Working : " + proxyDetailsTemp);
                    GlobusLogHelper.log.Debug("Proxy Is Not Working : " + proxyDetailsTemp);
                }


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }



        /// <summary>
        /// This Method is used for check given proxy has working or not ?
        /// </summary>
        /// <param name="IPaddress">Given IpAddress </param>
        /// <param name="Port">Given Port</param>
        /// <param name="Username">Given Username</param>
        /// <param name="Password">Given Password</param>
        /// <param name="CheckingUrl">Given Url where we check the Proxy</param>
        /// <returns></returns>
        public bool CheckProxy(string IPaddress,string Port ,string Username,string Password,string CheckingUrl)
        {
            ChilkatHttpHelpr objChilkatHelper = new ChilkatHttpHelpr();
            bool status = false;
            try
            {
               
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    string ResponseTime = string.Empty;
                    timer.Start();
                    string Response = objChilkatHelper.GetHtmlProxy(CheckingUrl, IPaddress, Port, Username, Password);
                    timer.Stop();
                    TimeSpan timeTaken = timer.Elapsed;
                    ResponseTime = timeTaken.ToString();
               
                

                if (!string.IsNullOrEmpty(Response) && objChilkatHelper.http.LastStatus != 404 && objChilkatHelper.http.LastStatus != 500 && !Response.Contains("Internet Explorer cannot display the webpage"))
                {
                    status = true;
                }
                else
                {
                    status = false;
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return status;
 
        }

        //New method added by Lijo John 16/10/2014
        public bool CheckProxy(string IPAddress, int Port, string Username, string Password, string URL)
        {
            GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
            bool status = false;
            try
            {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                string ResponseTime = string.Empty;
                timer.Start();
                string Response = objGlobusHttpHelper.getHtmlfromUrlProxy(new Uri(URL),IPAddress,Port,Username,Password);
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                ResponseTime = timeTaken.ToString();

                if (string.IsNullOrEmpty(Response))
                {
                    timeTaken = new TimeSpan();
                    timer.Start();
                    Response = objGlobusHttpHelper.getHtmlfromUrlProxy(new Uri(URL), IPAddress, Port, Username, Password);
                    timer.Stop();
                    timeTaken = timer.Elapsed;
                    ResponseTime = timeTaken.ToString();
                }
                if (!string.IsNullOrEmpty(Response) && !Response.Contains("Internet Explorer cannot display the webpage"))
                {
                    status = true;
                }
                else
                {
                    status = false;
                } 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
            return status;
        }

        /// <summary>
        /// This Method is used for check given proxy has working or not And Return those Proxy set for use in their method
        /// </summary>
        /// <param name="IPaddress">Given IpAddress </param>
        /// <param name="Port">Given Port</param>
        /// <param name="Username">Given Username</param>
        /// <param name="Password">Given Password</param>
        /// <param name="CheckingUrl">Given Url where we check the Proxy</param>
        /// <returns></returns>
        public Proxies.Proxy GetSingleUserProxy(string IPaddress, string Port, string Username, string Password, string CheckingUrl)
        {
            ChilkatHttpHelpr objChilkatHelper = new ChilkatHttpHelpr();
           Proxy objProxy=new Proxy();
            try
            {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                string respnceTime = string.Empty;
                timer.Start();
                string Responce = objChilkatHelper.GetHtmlProxy(CheckingUrl, IPaddress, Port, Username, Password);
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                respnceTime = timeTaken.ToString();

                if (!string.IsNullOrEmpty(Responce) && objChilkatHelper.http.LastStatus != 404 && objChilkatHelper.http.LastStatus != 500 && !Responce.Contains("Internet Explorer cannot display the webpage"))
                {
                    objProxy.IpAddress = IPaddress;
                    objProxy.Port = Port;
                    objProxy.UserName = Username;
                    objProxy.Password = Password;
                }
                else
                {
                   
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return objProxy;

        }

        /// <summary>
        /// This method is used for set Public proxy in registry
        /// </summary>
        /// <param name="IPAddress">Given Public Proxy</param>
        /// <param name="Port">Given Public Proxy</param>
        public bool SetPublicProxy(string IPAddress, string Port)
        {
            bool isSetPublicProxy = false;
            try
            {
                
                string key = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

                string serverName = IPAddress ;      //your proxy server name;

                string port = IPAddress;             //your proxy port;

                string proxy = serverName + ":" + port;

                RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(key, true);

                RegKey.SetValue("ProxyServer", proxy);

                RegKey.SetValue("ProxyEnable", 1);

                object obj = RegKey.GetValue("ProxyServer");

                isSetPublicProxy = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return isSetPublicProxy;
        }
    }
}
interface IProxyManager
{
    bool CheckProxy(string IPaddress, string Port, string Username, string Password, string CheckingUrl);

    Proxies.Proxy GetSingleUserProxy(string IPaddress, string Port, string Username, string Password, string CheckingUrl);

    bool SetPublicProxy(string IPAddress, string Port);
}
