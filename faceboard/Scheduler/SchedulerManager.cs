using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BaseLib;
using Accounts;
using faceboardpro;

namespace Scheduler
{
    public class SchedulerManager
    {
        public bool isStopSchFanPageLiker = false;
        List<Thread> lstThreadSchFanPageLiker = new List<Thread>();

        public void StartSchFanpageLiker(object parameters)
        {
            try
            {
                string schName = string.Empty;
                string schProcess = string.Empty;
                string schAccount = string.Empty;
                string schStartDate = string.Empty;
                string schEndDate = string.Empty;
                string schStartTime = string.Empty;
                string schEndTime = string.Empty;
                string schMinDelay = string.Empty;
                string schMaxDelay = string.Empty;
                List<string> lstSchFanPageURLs = new List<string>();
                List<string> lstSchFanPageMessages = new List<string>();
                List<string> lstSchFanPageComments = new List<string>();

                Array paramsArray = new object[15];
                paramsArray = (Array)parameters;

                try
                {
                    schName = paramsArray.GetValue(0).ToString();
                    schProcess = paramsArray.GetValue(1).ToString();
                    schAccount = paramsArray.GetValue(2).ToString();
                    schStartDate = paramsArray.GetValue(3).ToString();
                    schEndDate = paramsArray.GetValue(4).ToString();
                    schStartTime = paramsArray.GetValue(5).ToString();
                    schEndTime = paramsArray.GetValue(6).ToString();
                    schMinDelay = paramsArray.GetValue(7).ToString();
                    schMaxDelay = paramsArray.GetValue(8).ToString();
                    lstSchFanPageURLs = (List<string>)paramsArray.GetValue(9);
                    lstSchFanPageMessages = (List<string>)paramsArray.GetValue(10);
                    lstSchFanPageComments = (List<string>)paramsArray.GetValue(11);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                schFanPageLikerLogin(schName, schProcess, schAccount, schStartDate, schEndDate, schStartTime, schEndTime, schMinDelay, schMaxDelay, lstSchFanPageURLs, lstSchFanPageMessages, lstSchFanPageComments);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void schFanPageLikerLogin(string schName, string schProcess, string schAccount, string schStartDate, string schEndDate, string schStartTime, string schEndTime, string schMinDelay, string schMaxDelay, List<string> lstSchFanPageURLs, List<string> lstSchFanPageMessages, List<string> lstSchFanPageComments)
        {

            try
            {
                if (isStopSchFanPageLiker)
                {
                    return;
                }
                try
                {
                    lstThreadSchFanPageLiker.Add(Thread.CurrentThread);
                    lstThreadSchFanPageLiker.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (schAccount.Contains(":"))
                {
                    string[] AccArr = schAccount.Split(':');
                    if (AccArr.Count() > 1)
                    {
                        try
                        {
                            string accountUser = string.Empty;
                            string accountPass = string.Empty;
                            string proxyAddress = string.Empty;
                            string proxyPort = string.Empty;
                            string proxyUserName = string.Empty;
                            string proxyPassword = string.Empty;
                            string status = string.Empty;

                            try
                            {
                                accountUser = schAccount.Split(':')[0];
                                accountPass = schAccount.Split(':')[1];
                                proxyAddress = schAccount.Split(':')[2];
                                proxyPort = schAccount.Split(':')[3];
                                proxyUserName = schAccount.Split(':')[4];
                                proxyPassword = schAccount.Split(':')[5];
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }


                            FacebookUser objFacebookUser = new FacebookUser();
                            objFacebookUser.username = accountUser;
                            objFacebookUser.password = accountPass;
                            objFacebookUser.proxyip = proxyAddress;
                            objFacebookUser.proxyport = proxyPort;
                            objFacebookUser.proxyusername = proxyUserName;
                            objFacebookUser.proxypassword = proxyPassword;

                            if (!objFacebookUser.isloggedin)
                            {
                                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

                                objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


                                //Login Process

                                Accounts.AccountManager objAccountManager = new AccountManager();


                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {
                                // Call LikePage

                                StartSettingSchFanPageLiker(ref objFacebookUser, schName, schProcess, schAccount, schStartDate, schEndDate, schStartTime, schEndTime, schMinDelay, schMaxDelay, lstSchFanPageURLs, lstSchFanPageMessages, lstSchFanPageComments);

                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Account : " + schAccount + " Not In Correct Format !");
                        GlobusLogHelper.log.Debug("Account : " + schAccount + " Not In Correct Format !");

                        return;
                    }
                }

                else
                {
                    GlobusLogHelper.log.Info("Account : " + schAccount + " Not In Correct Format !");
                    GlobusLogHelper.log.Debug("Account : " + schAccount + " Not In Correct Format !");

                    return;
                }


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartSettingSchFanPageLiker(ref FacebookUser fbUser, string schName, string schProcess, string schAccount, string schStartDate, string schEndDate, string schStartTime, string schEndTime, string schMinDelay, string schMaxDelay, List<string> lstSchFanPageURLs, List<string> lstSchFanPageMessages, List<string> lstSchFanPageComments)
        {
            try
            {
                // to do code for managing scheduler time

                List<string> schlstSchFanPageURLsTemp = lstSchFanPageURLs.Distinct().ToList();

                foreach (string item in schlstSchFanPageURLsTemp)
                {

                    try
                    {
                        List<string> lstFanPageURLsTemp = new List<string>();

                        
                        string schedulerStartDate = Convert.ToDateTime(schStartDate).Date.ToString("dd/MM/yyyy");

                        DateTime schStartDateTemp = (Convert.ToDateTime(schStartDate).Date);
                        int schStartYear = schStartDateTemp.Year;
                        int schStartMonth = schStartDateTemp.Month;
                        int schStartDay = schStartDateTemp.Day;

                        DateTime schEndDateTemp = Convert.ToDateTime(schEndDate).Date;
                        int schEndYear = schEndDateTemp.Year;
                        int schEndMonth = schEndDateTemp.Month;
                        int schEndDay = schEndDateTemp.Day;

                        TimeSpan schStartTimeTemp = Convert.ToDateTime("5 pm").TimeOfDay;//schStartTime
                        TimeSpan schEndTimeTemp = Convert.ToDateTime(schEndTime).TimeOfDay;
                        int schMinDelayTemp = Convert.ToInt32(schMinDelay);
                        int schMaxDelayTemp = Convert.ToInt32(schMaxDelay);

                        DateTime currentDatetime = (DateTime.Now.Date);
                        int currentYear = currentDatetime.Year;
                        int currentMonth = currentDatetime.Month;
                        int currentDay = currentDatetime.Day;

                        string currentDate = Convert.ToDateTime(DateTime.Now.Date).ToString("dd/MM/yyyy");
                        TimeSpan currentTime = DateTime.Now.TimeOfDay;



                        // Process Start On Current Date
                        if (schStartYear > currentYear)
                        {
                            GlobusLogHelper.log.Info("Start Year Is Greater Than Current Year With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Start Year Is Greater Than Current Year With Username : " + fbUser.username);

                            return;
                        }

                        if (schStartMonth > currentMonth)
                        {
                            GlobusLogHelper.log.Info("Start Month Is Greater Than Current Month With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Start Month Is Greater Than Current Month With Username : " + fbUser.username);

                            return;
                        }

                        if (schStartDay > currentDay)
                        {
                            GlobusLogHelper.log.Info("Start Day Is Greater Than Current Day With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Start Day Is Greater Than Current Day With Username : " + fbUser.username);

                            return;
                        }

                        if (schEndYear < currentYear)
                        {
                            GlobusLogHelper.log.Info("End Year Is Less Than Current Year With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("End Year Is Less Than Current Year With Username : " + fbUser.username);

                            return;
                        }

                        if (schEndMonth < currentMonth)
                        {
                            GlobusLogHelper.log.Info("End Month Is Less Than Current Month With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("End Month Is Less Than Current Month With Username : " + fbUser.username);

                            return;
                        }

                        if (schEndDay < currentDay)
                        {
                            GlobusLogHelper.log.Info("End Day Is Less Than Current Day With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("End Day Is Less Than Current Day With Username : " + fbUser.username);

                            return;
                        }

                        if (schEndTimeTemp.Hours < currentTime.Hours && schEndTimeTemp.Minutes < currentTime.Minutes)
                        {
                            GlobusLogHelper.log.Info("End Hours are Less Than Current Hours And End Minutes are Less Than Current Minutes With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("End Hours are Less Than Current Hours And End Minutes are Less Than Current Minutes With Username : " + fbUser.username);

                            return;
                        }

                        lstFanPageURLsTemp.Add(item);

                        StartSchFanPageLikerProcess(ref fbUser, schProcess, lstFanPageURLsTemp, lstSchFanPageMessages, lstSchFanPageComments);

                        ///Dealy

                        int delay = Utils.GenerateRandom(schMinDelayTemp, schMaxDelayTemp);

                        GlobusLogHelper.log.Info("Delay For " + delay + " In Seconds With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Delay For " + delay + " In Seconds With Username : " + fbUser.username);

                        Thread.Sleep(delay);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            finally
            {
                GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);

            }
        }

        public void StartSchFanPageLikerProcess(ref FacebookUser fbUser,string schProcess,List<string> lstSchFanPageURLs, List<string> lstSchFanPageMessages, List<string> lstSchFanPageComments)
        {
            try
            {
                //foreach (string item in schlstSchFanPageURLsTemp)
                {

                    try
                    {
                       
                        Pages.PageManager objPageManager = new Pages.PageManager();

                        if (schProcess == "Like Page")
                        {
                            objPageManager.LikePage(ref fbUser, lstSchFanPageURLs);
                        }

                        else if (schProcess == "Share Page")
                        {
                            objPageManager.SharePage(ref fbUser, lstSchFanPageURLs);
                        }
                        else if (schProcess == "Like Post")
                        {
                            objPageManager.LikePost(ref fbUser, lstSchFanPageURLs, lstSchFanPageMessages);
                        }
                        else if (schProcess == "Comment On Post")
                        {
                            objPageManager.CommentOnPost(ref fbUser, lstSchFanPageURLs, lstSchFanPageComments);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Scheduler Process Is Not Selected With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Scheduler Process Is Not Selected With Username : " + fbUser.username);

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }
}
