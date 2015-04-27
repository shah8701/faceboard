using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BaseLib;
using System.Data;
using faceboardpro;
using Accounts;
using System.Text.RegularExpressions;
using System.Web;
using Globussoft;
using System.IO;
using MySql.Data.MySqlClient;
using Dapper;
using System.Configuration;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;




namespace Groups
{
    public class GroupManager
    {
        public BaseLib.Events groupInviterEvent = null;
        public BaseLib.Events groupCamMgrEvent = null;
        public BaseLib.Events groupRequestUnique = null;


        public static string GroupExprotFilePath = string.Empty;
        public static bool ChkbGroup_GrpRequestManager_MultiplePicPerGroup = false;
        public static string GroupRequestCampaignName = string.Empty;
        public static bool GrouRequestUseSavedCampaign=false;
        public static bool ScheduleGroupPosting = false;
        public static string GroupDeletePostFilePath = string.Empty;
        public static string GroupDeletePostCommentFilePath = string.Empty;
        public static bool isPostVideoUrlAsImage = false;
        public static bool isPostMessageWithImageAsEdited = false;

        public void RaiseEventsgroupRequestUnique(params string[] paramValue)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(paramValue);
                groupRequestUnique.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public static string strGroup_DeleteCommentPostProcessUsing
        {
            get;
            set;
        }

        #region Global Variables For GroupInviter

        readonly object lockrThreadControllerGroupInviter = new object();
        public bool isStopGroupInviter = false;
        int countThreadControllerGroupInviter = 0;
        public List<Thread> lstThreadsGroupInviter = new List<Thread>();

        public Queue<string> Queue_Fanpages = new Queue<string>();
        public Queue<string> Queue_Grouppages = new Queue<string>();
        public Queue<string> Queue_Messages = new Queue<string>();
        public Queue<string> Queue_UserIDs = new Queue<string>();
        public Queue<string> Queue_GroupUrls = new Queue<string>();
        public static int minDelayGroupInviter = 10;
        public static int maxDelayGroupInviter = 20;
        public static bool CheckTargetedGroupUrlsUse = false;

        #endregion

        #region Property For GroupInviter

        public int NoOfThreadsGroupInviter
        {
            get;
            set;
        }

        public int AddNoOfFriendsGroupInviter
        {
            get;
            set;
        }
        public List<string> LstGroupUrlsGroupInviter
        {
            get;
            set;
        }

        #endregion

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                groupInviterEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public GroupManager()
        {
            try
            {
                groupInviterEvent = new Events();
                groupCamMgrEvent = new Events();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }



        public void StartGroupInviter()
        {
            try
            {
                
                int numberOfAccountPatch = 25;

                if (NoOfThreadsGroupInviter > 0)
                {
                    numberOfAccountPatch = NoOfThreadsGroupInviter;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerGroupInviter)
                                {
                                    try
                                    {
                                        if (countThreadControllerGroupInviter >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupInviter);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsGroupInviter);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerGroupInviter++;
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartMultiThreadsGroupInviter(object parameters)
        {
            try
            {
                if (!isStopGroupInviter)
                {
                    try
                    {
                        lstThreadsGroupInviter.Add(Thread.CurrentThread);
                        lstThreadsGroupInviter.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {

                        {
                            Array paramsArray = new object[1];
                            paramsArray = (Array)parameters;
                            FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);
                            if (!objFacebookUser.isloggedin)
                            {
                                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

                                objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

                                Accounts.AccountManager objAccountManager = new AccountManager();
                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {
                                // Call StartActionGroupInviter
                                StartActionGroupInviter(ref objFacebookUser);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                            }
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

            finally
            {
                try
                {

                    // if (!isStopGroupInviter)
                    {
                        lock (lockrThreadControllerGroupInviter)
                        {
                            countThreadControllerGroupInviter--;
                            Monitor.Pulse(lockrThreadControllerGroupInviter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionGroupInviter(ref FacebookUser fbUser)
        {
            try
            {
                AddFriendsToGroup(ref fbUser);

         
                GlobusLogHelper.log.Info("Group Inviter Process Completed With : " + fbUser.username);
                GlobusLogHelper.log.Debug("Group Inviter Process Completed With : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddFriendsToGroup(ref FacebookUser fbUser)
        {

            string UserId = string.Empty;
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                }

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

                // Find Total Friends
                GlobusLogHelper.log.Info("Please wait finding the friends user id  : " + " with User Name : " + fbUser.username);
                List<string> lstfriendsId = FBUtils.GetAllFriends(ref HttpHelper, UserId);

                List<string> lstFriendname = new List<string>();
                List<string> lstFriendId = new List<string>();
                GlobusLogHelper.log.Info("Please wait finding the friends name : " + " with User Name : " + fbUser.username);
                int friendAddLimit = AddNoOfFriendsGroupInviter + 20;
                //for (int i = 0; i < lstfriendsId.Count; i++) 
                for (int i = 0; i < friendAddLimit; i++)
                {
                    int rndm =Utils.GenerateRandom(0, lstfriendsId.Count);
                    try
                    {
                        //string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstfriendsId[i]));
                        string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstfriendsId[rndm]));
                        if (strFriendInfo.Contains("\"name\":"))
                        {

                            //   string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                            string strName = Utils.getBetween(strFriendInfo, "first_name\": \"", "\",\n ");
                            lstFriendname.Add(strName);
                            lstFriendId.Add(lstfriendsId[i]);
                        }
                    }
                        
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                for (int i = 0; i < LstGroupUrlsGroupInviter.Count; i++)
                {
                    try
                    {
                        string groupUrl = LstGroupUrlsGroupInviter[i];

                        InviteFriendsToYourGroup(ref fbUser, ref lstFriendname, ref lstFriendId, groupUrl, UserId);
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

        private void InviteFriendsToYourGroup(ref FacebookUser fbUser, ref List<string> lstFriendname, ref List<string> lstFriend, string groupurl, string userId)
        {
            string fb_dtsg = string.Empty;
            string group_id = string.Empty;
            string message_id = string.Empty;
            int counter = 1;
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string strGroupURLPageSource = HttpHelper.getHtmlfromUrl(new Uri(groupurl));

                //
                string PageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + userId));  //"https://www.facebook.com/"
                string frndCount = Utils.getBetween(PageSource, "Friends<span class=\"_gs6\">", "</span>");



                GlobusLogHelper.log.Info("Total No of  friends : " + frndCount + " with User Name : " + fbUser.username);
                GlobusLogHelper.log.Debug("Total No of  friends : " + frndCount + " with User Name : " + fbUser.username);


                  for (int i = 0; i < lstFriend.Count; i++)
                {
                      try
                    {
                        
                        string friendID = lstFriend[i];
                        string friendName = string.Empty;

                        if (counter > AddNoOfFriendsGroupInviter)
                        {
                            break;
                        }


                        try
                        {
                            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strGroupURLPageSource);
                            try
                            {
                                group_id = strGroupURLPageSource.Substring(strGroupURLPageSource.IndexOf("group_id="), strGroupURLPageSource.IndexOf("/", strGroupURLPageSource.IndexOf("group_id=")) - strGroupURLPageSource.IndexOf("group_id=")).Replace("\"", string.Empty).Replace("group_id=", string.Empty).Replace("value=", string.Empty).Trim(); //Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "group_id");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            if (group_id.Contains(" "))
                            {
                                group_id = group_id.Substring(0, group_id.IndexOf(" "));
                            }

                            if (group_id.Contains("&"))
                            {
                                try
                                {
                                    group_id = group_id.Substring(0, group_id.IndexOf("&"));
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            try
                            {
                                message_id = strGroupURLPageSource.Substring(strGroupURLPageSource.IndexOf("message_id="), strGroupURLPageSource.IndexOf("/", strGroupURLPageSource.IndexOf("message_id=")) - strGroupURLPageSource.IndexOf("message_id=")).Replace("\"", string.Empty).Replace("message_id=", string.Empty).Replace("value=", string.Empty).Trim();//Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "message_id");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            if (message_id.Contains(" "))
                            {
                                message_id = message_id.Substring(0, message_id.IndexOf(" "));
                            }

                            if (message_id.Contains("&"))
                            {
                                try
                                {
                                    message_id = message_id.Substring(0, message_id.IndexOf("&"));
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

                        //Get Friends Name
                        try
                        {
                            string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + friendID));
                            if (strFriendInfo.Contains("\"name\":"))
                            {
                                try
                                {
                                    friendName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                if (friendName == string.Empty)
                                {
                                    try
                                    {
                                        friendName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf("}", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        //string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + lstFriend[i] + "&freeform=" + lstFriendname[i] + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";

                        string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + friendID + "&freeform=" + friendName + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";
                        string lastResponseStatus = string.Empty;
                        string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupInviterPostAjaxGroupsMembersAddUrl), strPostData, ref lastResponseStatus);

                        if (strResponse.Contains("You don't have permission"))
                        {
                            GlobusLogHelper.log.Info("You don't have permission to add members to this group by Username : " + fbUser.username + " groupurl : " + groupurl);
                            GlobusLogHelper.log.Debug("You don't have permission to add members to this group by Username : " + fbUser.username + " groupurl : " + groupurl);

                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);

                            break;
                        }
                        else if (strResponse.Contains("errorSummary\":"))
                        {
                            try
                            {
                                try
                                {
                                    string[] summaryArr = Regex.Split(strResponse, "errorSummary\":");
                                    summaryArr = Regex.Split(summaryArr[1], "\"");
                                    string errorSummery = summaryArr[1];
                                    string errorDiscription = summaryArr[5];

                                    GlobusLogHelper.log.Info(counter + ") Friend Name:" + friendName + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug(counter + ") Friend Name:" + friendName + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                                    counter = counter - 1;
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                int delayInSeconds = Utils.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        else if (strResponse.Contains("has been added"))
                        {
                            GlobusLogHelper.log.Info(counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug(counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);


                          

                            if (!string.IsNullOrEmpty(GroupReportExprotFilePath))
                            {
                                try
                                {
                                    string CSVHeader = "friendName" + "," + "userAccountName" + "," + "GroupUrl";
                                    string CSV_Content = friendName + "," + fbUser.username + "," + groupurl;
                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                }
                            }


                            //delay
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(counter + ") Friend Name:" + friendName + " Couldn't Add to Group  with UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug(counter + ") Friend Name:" + friendName + " Couldn't Add to Group  with UserName : " + fbUser.username);

                            //delay
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    counter = counter + 1;
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            //GlobusLogHelper.log.Info("Process completed with User Name : " + fbUser.username + " and URL is : " + groupurl);
            //GlobusLogHelper.log.Debug("Process completed with User Name : " + fbUser.username + " and URL is : " + groupurl);

        }

        #region Globaol veriables of GroupCampaignManager
        readonly object lockrThreadControllerGroupCamapinScheduler = new object();
        public bool isStopGroupCamapinScheduler = false;
        public static bool ChkViewSchedulerTaskUniquePostPerGroup = false;
        public static bool ChkbGroup_GrpRequestManager_UniquePostPerGroup = false;
        int countThreadControllerGroupCamapinScheduler = 0;
        public List<Thread> lstThreadsGroupCamapinScheduler = new List<Thread>();
        public bool chkCountinueProcessGroupCamapinScheduler = false;

        public bool ChkbGroupGrpRequestManagerMultiplePicPerGroup = false;
        //public bool ChkbGroup_GrpRequestManager_MultiplePicPerGroup = false;
        public static string GroupReportInviterExprotFilePath = string.Empty;
        public static List<string> LstGroupUrlsViewSchedulerTaskTargetedUrls = new List<string>();


        // public Queue<string> Queue_GroupUrls = new Queue<string>();
        public string Msgsendingcurrentstatus = string.Empty;
        public string Msgsendingstatus = string.Empty;
        string proxyAddress = string.Empty;
        string proxyPort = string.Empty;
        string proxyUsername = string.Empty;
        string proxyPassword = string.Empty;
        public static bool CheckDataBaseGroupCampaimanager;
        public static string GroupReportExprotFilePath = string.Empty;

        public static bool ChkbGroupViewSchedulerTaskRemoveUrl = false;

        public static int minDelayGroupManager = 10;
        public static int maxDelayGroupManager = 20;


        public readonly object locker_Queue_Messages = new object();

        public readonly object locker_Queue_UserIDs = new object();

        int Message_Counter = 0;
        int TimeCounter = 0;

        #endregion

        #region Property For GroupCampaignManager

        public static int CheckGroupCompaignNoOfGroupsInBatch
        {
            get;
            set;
        }

        public static int CheckGroupCompaign_InterbalInMinuts
        {
            get;
            set;
        }

        public List<string> LstVideoUrlsGroupCampaignManager
        {
            get;
            set;
        }

        public List<string> LstPicUrlsGroupCampaignManager = new List<string>();

        public List<string> LstMessageUrlsGroupCampaignManager
        {
            get;
            set;
        }

        public int NoOfThreadsGroupCamapinScheduler
        {
            get;
            set;
        }

        public List<string> LstGroupUrlsGroupCamapinScheduler
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Event For Grp Campaign Mgr
        /// </summary>

        private void RaiseEventGrpCampaignMgr(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                groupCamMgrEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void StartGroupSchedulerTask()
        {
            countThreadControllerGroupCamapinScheduler = 0;
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsGroupCamapinScheduler > 0)
                {
                    numberOfAccountPatch = NoOfThreadsGroupCamapinScheduler;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);
                    int i = 1;
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerGroupCamapinScheduler)
                                {
                                    try
                                    {
                                        if (countThreadControllerGroupCamapinScheduler >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupCamapinScheduler);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (faceboardpro.FbGroupCampaignManagerGlobals.Account.Count() > 1)
                                        {
                                            if (acc == faceboardpro.FbGroupCampaignManagerGlobals.Account)
                                            {

                                                if (item != null)
                                                {

                                                    i = i + 1;
                                                    Thread profilerThread = new Thread(StartMultiThreadsGroupCamapinScheduler);
                                                    profilerThread.Name = "workerThread_Profiler_" + acc;
                                                    profilerThread.IsBackground = true;


                                                    profilerThread.Start(new object[] { item });

                                                    countThreadControllerGroupCamapinScheduler++;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (item != null)
                                            {                                               
                                                Thread profilerThread = new Thread(StartMultiThreadsGroupCamapinScheduler);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;


                                                profilerThread.Start(new object[] { item });

                                                countThreadControllerGroupCamapinScheduler++;
                                            }
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
                    if (i==1)
                    {

                        DialogResult dialogresult = MessageBox.Show("Please Upload Account : " + faceboardpro.FbGroupCampaignManagerGlobals.Account, "FaceDominator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        GlobusLogHelper.log.Info("Please Upload Account");
                        if (dialogresult == DialogResult.Yes)
                        {

                            return;
                        }
                        else
                        {
                            foreach (List<string> listAccounts in list_listAccounts)
                            {
                                //int tempCounterAccounts = 0; 

                                foreach (string account in listAccounts)
                                {
                                    try
                                    {
                                        lock (lockrThreadControllerGroupCamapinScheduler)
                                        {
                                            try
                                            {
                                                if (countThreadControllerGroupCamapinScheduler >= listAccounts.Count)
                                                {
                                                    Monitor.Wait(lockrThreadControllerGroupCamapinScheduler);
                                                }

                                                string acc = account.Remove(account.IndexOf(':'));

                                                //Run a separate thread for each account
                                                FacebookUser item = null;
                                                FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                                if (faceboardpro.FbGroupCampaignManagerGlobals.Account.Count() > 1)
                                                {
                                                    //if (acc == FaceDominator.FbGroupCampaignManagerGlobals.Account)
                                                    //{

                                                    if (item != null)
                                                    {


                                                        Thread profilerThread = new Thread(StartMultiThreadsGroupCamapinScheduler);
                                                        profilerThread.Name = "workerThread_Profiler_" + acc;
                                                        profilerThread.IsBackground = true;


                                                        profilerThread.Start(new object[] { item });

                                                        countThreadControllerGroupCamapinScheduler++;
                                                    }
                                                    // }

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
                    }


                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        /// <summary>
        /// ----------- Delete Group Post -------------------
        /// </summary>


        #region Global Variables For DeleteGroupPost

        readonly object lockrThreadControllerDeleteGroupPost = new object();
        public bool isStopDeleteGroupPost = false;
        int countThreadControllerDeleteGroupPost = 0;
        public List<Thread> lstThreadsDeleteGroupPost = new List<Thread>();

        public static int minDelayDeleteGroupPost = 10;
        public static int maxDelayDeleteGroupPost = 20;

        public static bool DeleteScheduleGroupPosting = false;
        public static bool CheckContinueProcess = false;


        public static int DeleteNUmberOfPost = 10;
        public static int IntervalTime = 20;


        #endregion

        public void StartDeletePostRequuest()
        {
            countThreadControllerDeleteGroupPost = 0;
            try
            {
                lstThreadsDeleteGroupPost.Add(Thread.CurrentThread);
                lstThreadsDeleteGroupPost.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsGroupCamapinScheduler > 0)
                {
                    numberOfAccountPatch = NoOfThreadsGroupCamapinScheduler;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerDeleteGroupPost)
                                {
                                    try
                                    {
                                        if (countThreadControllerDeleteGroupPost >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupCamapinScheduler);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account

                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread DeletePostThread = new Thread(StartDeletePostRequest);
                                            DeletePostThread.Name = "workerThread_Profiler_" + acc;
                                            DeletePostThread.IsBackground = true;

                                            DeletePostThread.Start(new object[] { item });

                                            countThreadControllerDeleteGroupPost++;
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartMultiThreadsGroupCamapinScheduler(object parameters)
        {
            GlobusLogHelper.log.Debug("Please wait group posting process started ....");
            GlobusLogHelper.log.Info("Please wait group posting process started....");

            try
            {
                if (!isStopGroupCamapinScheduler)
                {
                    try
                    {
                        lstThreadsGroupCamapinScheduler.Add(Thread.CurrentThread);
                        lstThreadsGroupCamapinScheduler.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {

                        Array paramsArray = new object[1];
                        paramsArray = (Array)parameters;

                        FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

                        if (!objFacebookUser.isloggedin)
                        {
                            GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                            objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

                            Accounts.AccountManager objAccountManager = new AccountManager();
                            objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                        }

                        if (objFacebookUser.isloggedin)
                        {
                            StartActionGroupCamapinScheduler(ref objFacebookUser);
                        }
                        else
                        {
                          //  GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                           // GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
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

            finally
            {
                try
                {

                    // if (!isStopGroupCamapinScheduler)
                    {
                        lock (lockrThreadControllerGroupCamapinScheduler)
                        {
                            countThreadControllerGroupCamapinScheduler--;
                            Monitor.Pulse(lockrThreadControllerGroupCamapinScheduler);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartDeletePostRequest(object parameters)
        {
            try
            {
                if (!isStopGroupCamapinScheduler)
                {
                    try
                    {
                        lstThreadsDeleteGroupPost.Add(Thread.CurrentThread);
                        lstThreadsDeleteGroupPost.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {

                        {
                            Array paramsArray = new object[1];
                            paramsArray = (Array)parameters;

                            FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

                            if (!objFacebookUser.isloggedin)
                            {
                                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                                objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

                                Accounts.AccountManager objAccountManager = new AccountManager();
                                objAccountManager.LoginUsingGlobusHttpWithProxy(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {

                                if (strGroup_DeleteCommentPostProcessUsing == "DeleteGroupPost")
                                {
                                   //StartDeleteGrpPostProcess(ref objFacebookUser);
                                    StartDeleteGroupPostProcessNew(ref objFacebookUser);
                                }
                                else if (strGroup_DeleteCommentPostProcessUsing == "PostDeleteComments")
                                {
                                    if (CheckContinueProcess)
                                    {
                                        while (true)
                                        {
                                            if (isStopDeleteGroupPost)
                                            {
                                                break;
                                            }
                                            StartPostDeleteCommentsProcessContinue(ref objFacebookUser);
                                        }
                                        GlobusLogHelper.log.Info("Process Completed with : " + objFacebookUser.username);
                                        GlobusLogHelper.log.Debug("Process Completed with : " + objFacebookUser.username);
                                          
                                    }
                                    else
                                    {
                                        StartPostDeleteCommentsProcess(ref objFacebookUser);
                                    }
                                   
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                            }
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

            finally
            {
                try
                {

                    // if (!isStopGroupCamapinScheduler)
                    {
                        lock (lockrThreadControllerGroupCamapinScheduler)
                        {
                            countThreadControllerGroupCamapinScheduler--;
                            Monitor.Pulse(lockrThreadControllerGroupCamapinScheduler);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionGroupCamapinScheduler(ref FacebookUser fbUser)
        {
            try
            {
                StartGroupComapaignProcess(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void StartDeletePostRequest(ref FacebookUser fbUser)
        {
            try
            {
                StartDeleteGrpPostProcess(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Debug("Deleting Process Completed.");
            GlobusLogHelper.log.Info("Deleting Process Completed.");

        }

        public void StartDeleteGrpPostProcess(ref FacebookUser fbUser)
        {
            int farziCount = 0;
            int SecondFarziCount = 0;

            Dictionary<string, string> CheckStoryID = new Dictionary<string, string>();

            string grpurl = string.Empty;
            string grpurlresponse = string.Empty;
            string story_fbid = string.Empty;
            string __user = "";
            string fb_dtsg = "";

            try
            {
                GlobusLogHelper.log.Info("Please Wait Post Delete process start " + " With UserName : " + fbUser.username);

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string msg = string.Empty;


                string pageSourceFb = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                __user = GlobusHttpHelper.GetParamValue(pageSourceFb, "user");
                if (string.IsNullOrEmpty(__user))
                {
                    __user = GlobusHttpHelper.ParseJson(pageSourceFb, "user").Replace("p?id=", "").Replace("\\", "");
                }

                string FbUrl = FBGlobals.Instance.fbProfileUrl + __user;
                string pgSrFanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(FbUrl));

                string s = Utils.getBetween(pgSrFanPageSearch, "<a class=\"_6-6 _6-7\" href=\"", "\">Timeline");
                string link = s;


                string activityurl = s + "/allactivity";

                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(activityurl));

                fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                }


                string _rev = GlobusHttpHelper.get_Between(pgSrc_FanPageSearch, "revision", ",");
                _rev = _rev.Replace("\":", string.Empty);


                #region MyRegion
                //string GroupActivityUrl = "https://www.facebook.com/ever.gusman.3/allactivity?privacy_source=activity_log&log_filter=groups";



                //string PageSource = HttpHelper.getHtmlfromUrl(new Uri(GroupActivityUrl));


                //fb_dtsg = GlobusHttpHelper.ParseJson(PageSource, "fb_dtsg");


                //string firstStep = "https://www.facebook.com/ajax/notifications/client/get.php";

                //string firstStepPageSource = HttpHelper.postFormData(new Uri(firstStep), "businessID=&cursor&length=5&__user=" + __user + "&__a=1&__dyn=7nmajEyl35zoSt2u6aWizGomyp9Esx6bF299qzCC-C26m4VoKezpUgxd6K59poW8xOdy8-&__req=e&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170899576819772488868&__rev=1509424");

                ////2nd 
                //string SecondStepPostUrl = "https://www.facebook.com/ajax/timeline/delete/confirm";

                //string SecondStepPageSource = HttpHelper.postFormData(new Uri(SecondStepPostUrl), "identifier=S%3A_I" + __user + "%3A586891894749357&location=3&story_dom_id=u_0_1f&nctr[_mod]=pagelet_all_activity_2014_11&__user=" + __user + "&__a=1&__dyn=7nmajEyl35zoSt2u6aWizGomyp9Esx6bF299qzCC-C26m4VoKezpUgxd6K59poW8xOdy8-&__req=f&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170899576819772488868&__rev=1509424");

                //string finalDeletePostUrl = "https://www.facebook.com/ajax/timeline/delete?identifier=S%3A_I" + __user + "%3A586891894749357&location=3&story_dom_id=u_0_1f";
                //string PostDataFinalPost = HttpHelper.postFormData(new Uri(finalDeletePostUrl), "fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&__a=1&__dyn=7nmajEyl35zoSt2u6aWizGomyp9Esx6bF299qzCC-C26m4VoKezpUgxd6K59poW8xOdy8-&__req=g&ttstamp=2658170899576819772488868&__rev=1509424");
                
                #endregion

                string[] ArrStoryIdNew = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "identifier");

                foreach (var ArrStoryIdNew_item in ArrStoryIdNew)
                {
                    if (!ArrStoryIdNew_item.Contains("<!DOCTYPE html>") && !ArrStoryIdNew_item.Contains("label\":\"Unlike\",\"title\":\"\",\"className\""))
                    {
                        string story_dom_id = string.Empty;
                        try
                        {
                            //if (!ArrStoryIdNew_item.Contains("action=unlike"))
                            {

                                string PostHtml = ArrStoryIdNew_item.Replace("\\u0025", "%");
                                string identifier = Utils.getBetween(PostHtml, "=", "&story_dom_id=");
                                if (identifier.Contains("location"))
                                {
                                    string[] Arr = System.Text.RegularExpressions.Regex.Split(identifier, "&location");
                                    identifier = Arr[0];
                                }
                                try
                                {
                                    story_dom_id = Utils.getBetween(PostHtml, "story_dom_id=", "&");
                                    if (story_dom_id.Contains("\""))
                                    {
                                        string[] Arr = System.Text.RegularExpressions.Regex.Split(story_dom_id, "\"");
                                        story_dom_id = Arr[0];
                                    }

                                }
                                catch (Exception ex)
                                {
                                    story_dom_id = Utils.getBetween(PostHtml, "story_dom_id=", "\",\"");

                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                }



                                string PostDataForDeleteGroup = "identifier=" + identifier + "&location=3&story_dom_id=" + story_dom_id + "&nctr[_mod]=pagelet_all_activity_2014_3&__user=" + __user + "&__a=1&__dyn=7n8aqEAMCBynzpQ9UoGya4Cqm5VEsx6iWF29aGEZ94WpUpBw&__req=9&fb_dtsg=" + fb_dtsg + "&ttstamp=265816610982835379&__rev=1177827";
                                string PostUrl = FBGlobals.Instance.GroupAjaxTimelineDeleteConfirm;                                     // "https://www.facebook.com/ajax/timeline/delete/confirm";

                                string NewResponce = HttpHelper.postFormData(new Uri(PostUrl), PostDataForDeleteGroup, " ");
                                NewResponce = NewResponce.Replace("\\u0025", "%");
                                string identifierConfirm = Utils.getBetween(NewResponce, "identifier=", "&amp;");

                                string PostDataConfirmDelete = "fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&__a=1&__dyn=7n8aqEAMCBynzpQ9UoGya4Cqm5VEsx6iWF29aGEZ94WpUpBw&__req=a&ttstamp=265816610982835379&__rev=1177827";
                                string UrlConformDelete = FBGlobals.Instance.GroupAjaxTimelineDeleteIdentifier + identifier + "&location=3&story_dom_id=" + story_dom_id;                      // "https://www.facebook.com/ajax/timeline/delete?identifier="

                                string NewResponceConfirmDelete = HttpHelper.postFormData(new Uri(UrlConformDelete), PostDataConfirmDelete, " ");


                                if (NewResponceConfirmDelete.Contains("bootloadable") && NewResponceConfirmDelete.Contains("remove"))
                                {
                                    //GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Info("Post Deleted With UserName : " + fbUser.username);

                                    if (DeleteScheduleGroupPosting)
                                    {
                                        if (SecondFarziCount == DeleteNUmberOfPost)
                                        {
                                            Thread.Sleep(60 * 1000 * IntervalTime);
                                            SecondFarziCount = 0;
                                        }
                                    }
                                    SecondFarziCount = SecondFarziCount + 1;

                                    // delay
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    Thread.Sleep(10 * 1000);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(ex.StackTrace);
                        }
                    }
                }

                #region OldPostDataFor Delete ActivityLog Change By Ajay yadav Date 26-03-2014
                //string[] ArrStoryId = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "story_dom_id");
                //foreach (var ArrStoryId_item in ArrStoryId)
                //{
                //    if (!ArrStoryId_item.Contains("<!DOCTYPE html>"))
                //    {
                //        try
                //        {
                //            string ProfileiD1 = __user;
                //            string story_fbid1 = Utils.getBetween(ArrStoryId_item, "story_fbid=", "&");
                //            string story_dom_id1 = Utils.getBetween(ArrStoryId_item, "=", "&");
                //            string story_row_time1 = Utils.getBetween(ArrStoryId_item, "story_row_time=", "&");
                //            string visibility_selector_id11 = Utils.getBetween(ArrStoryId_item, "visibility_selector_id=", "\",");

                //            CheckStoryID.Add(story_fbid1, story_fbid1);
                //            try
                //            {
                //                string postdata1 = "pmid=25&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aOGUGy38y9ACwKyaF299qzAQjFw&__req=8&ttstamp=26581657049958373&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&confirmed=true&ban_user=0&fb_dtsg=" + fb_dtsg;

                //                string Response1 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.fbAjaxTimelineTakeAction), postdata1, " ");                     // "https://www.facebook.com/ajax/timeline/take_action_on_story.php"

                //                if (Response1.Contains("This content has been deleted"))
                //                {
                //                    GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);


                //                    // delay

                //                    try
                //                    {
                //                        int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                //                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                //                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                //                        Thread.Sleep(delayInSeconds);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        GlobusLogHelper.log.Error(ex.StackTrace);
                //                    }
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                GlobusLogHelper.log.Error(ex.StackTrace);
                //            }

                //            Thread.Sleep(1000 * 1);
                //        }
                //        catch (Exception ex)
                //        {
                //            GlobusLogHelper.log.Error(ex.StackTrace);
                //        }
                //    }
                //}

                #endregion

                //Ajax request

                string ajaxpipe_token = Utils.getBetween(pgSrc_FanPageSearch, "ajaxpipe_token", "\",").Replace("\":\"", "");

                string data = "";
                data = Utils.getBetween(pgSrc_FanPageSearch, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("\\\", {\\\"year", "{\\\"year") + "pager_load_count\":" + 1 + "}";
                data = data.Replace("\\", "");

                //{
                //    data = AjaxGetPaginationDataFromScrollingPager(pgSrc_FanPageSearch, data);
                //}

                int PageCounter = 2;
                string Response_ajaxGetURL = string.Empty;
                while (true)
                {
                    string NextPageAjaxHtml = string.Empty;
                    string NextPageAjaxUrl = string.Empty;


                    if (PageCounter == 2)
                    {
                        NextPageAjaxHtml = Utils.getBetween(pgSrc_FanPageSearch, "pam uiBoxLightblue uiMorePagerPrimary", "More Activity");
                        NextPageAjaxUrl = FBGlobals.Instance.fbhomeurl + Utils.getBetween(NextPageAjaxHtml, "ajaxify=", "rel=\"async\" role=\"button\">").Replace("amp;", "").Replace("\"", "") + "&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aEyx9z8nCwKyaF299qzQAjFDxCm&__req=6&__rev=1177827";   // "https://www.facebook.com" 
                        Response_ajaxGetURL = HttpHelper.getHtmlfromUrl(new Uri(NextPageAjaxUrl));

                    }
                    else
                    {
                        NextPageAjaxHtml = Utils.getBetween(Response_ajaxGetURL, "pam uiBoxLightblue uiMorePagerPrimary", "More Activity");
                        string ss = Utils.getBetween(NextPageAjaxHtml, "all_activity", "rel=").Replace("amp;", "").Replace("\"", "").Replace("\\\"", "").Replace("\\", "").Replace("\\u0025", "%").Replace("u0025", "%");
                        NextPageAjaxUrl = FBGlobals.Instance.fbajaxTimelineAll_activity + ss + "&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aEyx9z8nCwKyaF299qzQAjFDxCm&__req=6&__rev=1177827";                              //"https://www.facebook.com/ajax/timeline/all_activity" 
                        Response_ajaxGetURL = HttpHelper.getHtmlfromUrl(new Uri(NextPageAjaxUrl));
                    }
                    PageCounter = PageCounter + 1;

                    string[] ArrStoryIdNewPagination = System.Text.RegularExpressions.Regex.Split(Response_ajaxGetURL, "identifier");

                    foreach (var ArrStoryIdNew_item in ArrStoryIdNewPagination)
                    {
                        if (!ArrStoryIdNew_item.Contains("<!DOCTYPE html>"))
                        {
                            string story_dom_id = string.Empty;
                            try
                            {
                                if (!ArrStoryIdNew_item.Contains("label\":\"Unlike\",\"title\":\"\",\"className\""))
                                {

                                    string PostHtml = ArrStoryIdNew_item.Replace("\\u0025", "%");
                                    string identifier = Utils.getBetween(PostHtml, "=", "&story_dom_id=");
                                    if (identifier.Contains("location"))
                                    {
                                        string[] Arr = System.Text.RegularExpressions.Regex.Split(identifier, "&location");
                                        identifier = Arr[0];
                                    }
                                    try
                                    {
                                        story_dom_id = Utils.getBetween(PostHtml, "story_dom_id=", "&");
                                        if (story_dom_id.Contains("\",\""))
                                        {  
                                            story_dom_id = Utils.getBetween(PostHtml, "story_dom_id=", "\",\"");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        story_dom_id = Utils.getBetween(PostHtml, "story_dom_id=", "\",\"");

                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                    }


                                    string PostDataForDeleteGroup = "identifier=" + identifier + "&location=3&story_dom_id=" + story_dom_id + "&nctr[_mod]=pagelet_all_activity_2014_3&__user=" + __user + "&__a=1&__dyn=7n8aqEAMCBynzpQ9UoGya4Cqm5VEsx6iWF29aGEZ94WpUpBw&__req=9&fb_dtsg=" + fb_dtsg + "&ttstamp=265816610982835379&__rev=1177827";
                                    string PostUrl = FBGlobals.Instance.GroupAjaxTimelineDeleteConfirm;                           // "https://www.facebook.com/ajax/timeline/delete/confirm";


                                    string NewResponce = HttpHelper.postFormData(new Uri(PostUrl), PostDataForDeleteGroup, " ");
                                    NewResponce = NewResponce.Replace("\\u0025", "%");
                                    string identifierConfirm = Utils.getBetween(NewResponce, "identifier=", "&amp;");

                                    string PostDataConfirmDelete = "fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&__a=1&__dyn=7n8aqEAMCBynzpQ9UoGya4Cqm5VEsx6iWF29aGEZ94WpUpBw&__req=a&ttstamp=265816610982835379&__rev=1177827";
                                    string UrlConformDelete = FBGlobals.Instance.GroupAjaxTimelineDeleteIdentifier + identifier + "&location=3&story_dom_id=" + story_dom_id;                    // "https://www.facebook.com/ajax/timeline/delete?identifier=" 

                                    string NewResponceConfirmDelete = HttpHelper.postFormData(new Uri(UrlConformDelete), PostDataConfirmDelete, " ");



                                    if (NewResponceConfirmDelete.Contains("bootloadable") && NewResponceConfirmDelete.Contains("remove"))
                                    {
                                        //  GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Info("Post Deleted With UserName : " + fbUser.username);
                                        // delay
                                        try
                                        {
                                            if (DeleteScheduleGroupPosting)
                                            {
                                                if (SecondFarziCount == DeleteNUmberOfPost)
                                                {
                                                    Thread.Sleep(60*1000*IntervalTime);
                                                    SecondFarziCount = 0;
                                                }
                                            }
                                            SecondFarziCount = SecondFarziCount + 1;

                                            int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }
                                    else
                                    {
                                        Thread.Sleep(10 * 1000);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }
                    }

                    #region OldPostDataFor Delete ActivityLog Change By Ajay yadav Date 26-03-2014

                    //string pager_load_count = Utils.getBetween(data,"pager_load_count\":", "}") ;
                    ////GlobusHttpHelper.ParseJson(data, "pager_load_count");
                    ////string ajaxGetURL = FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxgenericPhpTimelineProfileAllActivityPagelet + ajaxpipe_token + "&no_script_path=1&data=" + Uri.EscapeDataString(data) + "&__user=" + __user + "&__a=1&__dyn=7n8a9EAMBlCFUlgDxyG8HzCqm5VEsx6iWF29aGE-QjFDw&__req=jsonp_"+PageCounter+"&__rev=1120684&__adt="+PageCounter+"";          // "https://www.facebook.com/ajax/pagelet/generic.php/TimelineProfileAllActivityPagelet?ajaxpipe=1&ajaxpipe_token="
                    //string ajaxGetURL = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileAllajaxpipeAjaxpipeToken + ajaxpipe_token + "&no_script_path=1&data=" + Uri.EscapeDataString(data) + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=jsonp_" + PageCounter + "&__rev=1120684&__adt=" + PageCounter + "";           // "http://www.facebook.com/ajax/pagelet/generic.php/TimelineProfileAllActivityPagelet?ajaxpipe=1&ajaxpipe_token=" 

                    //Response_ajaxGetURL = HttpHelper.getHtmlfromUrl(new Uri(ajaxGetURL));

                    //if (string.IsNullOrEmpty(Response_ajaxGetURL))
                    //{
                    //    Thread.Sleep(2 * 1000);
                    //    Response_ajaxGetURL = HttpHelper.getHtmlfromUrl(new Uri(ajaxGetURL));
                    //}
                    //#region CodeComment

                    ////Response_ajaxGetURL = HttpHelper.getHtmlfromUrl(new Uri());
                    ////Response_ajaxGetURL = Response_ajaxGetURL.Replace("\\", "").Replace("u003C", "<");

                    ////   ajaxpipe_token = Utils.getBetween(Response_ajaxGetURL, "ajaxpipe_token", "\",").Replace("\":\"", ""); 
                    //#endregion

                    ////data = Utils.getBetween(Response_ajaxGetURL, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    ////data = data.Replace("\\u0025", "%");
                    ////data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";

                    //{
                    //    data = AjaxGetPaginationDataFromScrollingPager(Response_ajaxGetURL, data);
                    //}

                    //if (string.IsNullOrEmpty(data))//(farziCount == 0)
                    //{
                    //    farziCount++;

                    //    data = Utils.getBetween(Response_ajaxGetURL, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    //    data = data.Replace("\\u0025", "%");
                    //    data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";

                    //    string data1 = Utils.getBetween(data, "{", ",\"tab_key");

                    //    string year = GlobusHttpHelper.ParseJson(data1, "year");
                    //    string month = GlobusHttpHelper.ParseJson(data1, "month");
                    //    string modifiedyear = GlobusHttpHelper.ParseJson(data1, "year");
                    //    string modifiedmonth = GlobusHttpHelper.ParseJson(data1, "month");
                    //    if (Convert.ToInt32(month) == 1)
                    //    {
                    //        modifiedmonth = "12";
                    //        modifiedyear = (Convert.ToInt32(year) - 1).ToString();
                    //    }
                    //    else
                    //    {
                    //        modifiedmonth = (Convert.ToInt32(month) - 1).ToString();
                    //    }


                    //    data1 = "{" + data1.Replace("month\":\"" + month + "", "month\":\"" + modifiedmonth + "").Replace("year\":\"" + year + "", "year\":\"" + modifiedyear + "") + "}";

                    //    string getURLAfterFirst = FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxgenericPhpTimelineProfileAllActivityPagelet + ajaxpipe_token + "&no_script_path=1&data=" + Uri.EscapeDataString(data1) + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=jsonp_3&__rev=1122330&__adt=3";   // "https://www.facebook.com/ajax/pagelet/generic.php/TimelineProfileAllActivityPagelet?ajaxpipe=1&ajaxpipe_token=" 

                    //    string res_getURLAfterFirst = HttpHelper.getHtmlfromUrl(new Uri(getURLAfterFirst));

                    //    Response_ajaxGetURL = res_getURLAfterFirst;

                    //    data = Utils.getBetween(res_getURLAfterFirst, "ScrollingPager", "pager_load_count").Replace("?data=", "");
                    //    if (string.IsNullOrEmpty(data))
                    //    {
                    //        data = Utils.getBetween(res_getURLAfterFirst, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    //        data = data.Replace("\\u0025", "%");
                    //        data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";
                    //    }

                    //    #region OldPostDataFor Delete ActivityLog Change By Ajay yadav Date 26-03-2014
                    //    //string[] ArrStoryId1 = System.Text.RegularExpressions.Regex.Split(res_getURLAfterFirst, "story_dom_id");

                    //    //foreach (var ArrStoryId_item in ArrStoryId1)
                    //    //{
                    //    //    if (!ArrStoryId_item.Contains("<html><body><script type=\"text/javascript\">"))
                    //    //    {
                    //    //        try
                    //    //        {

                    //    //            string ProfileiD1 = __user;
                    //    //            string story_fbid1 = Utils.getBetween(ArrStoryId_item, "story_fbid=", "&");
                    //    //            string story_dom_id1 = Utils.getBetween(ArrStoryId_item, "=", "&");
                    //    //            string story_row_time1 = Utils.getBetween(ArrStoryId_item, "story_row_time=", "&");
                    //    //            string visibility_selector_id11 = Utils.getBetween(ArrStoryId_item, "visibility_selector_id=", "\",");

                    //    //            try
                    //    //            {
                    //    //                //https://www.facebook.com/ajax/timeline/take_action_on_story.php
                    //    //                //pmid=34&__user=100007006000732&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=b&ttstamp=265816611880122110122&__rev=1122330&profile_id=100007006000732&activity_log=1&story_dom_id=u_jsonp_3_3h&story_fbid=541237672651067&story_row_time=1390819799&visibility_selector_id=u_jsonp_3_3i&action=remove_content&also_remove_app=0&nctr[_mod]=pagelet_all_activity_2014_1&fb_dtsg=AQBvPznz
                    //    //                string postURL = "https://www.facebook.com/ajax/timeline/take_action_on_story.php";
                    //    //                string postData = "pmid=34&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=b&ttstamp=265816611880122110122&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&fb_dtsg=" + fb_dtsg;//&nctr[_mod]=pagelet_all_activity_2014_1
                    //    //                string Response1 = HttpHelper.postFormData(new Uri(postURL), postData, "https://www.facebook.com/geetha.choubey.1/allactivity");          //https://www.facebook.com/ajax/timeline/take_action_on_story.php
                    //    //            }
                    //    //            catch (Exception)
                    //    //            {

                    //    //                throw;
                    //    //            }
                    //    //            try
                    //    //            {
                    //    //                //string postdata1 = "pmid=25&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aOGUGy38y9ACwKyaF299qzAQjFw&__req=8&ttstamp=26581657049958373&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&confirmed=true&ban_user=0&fb_dtsg=" + fb_dtsg;

                    //    //                //string Response1 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.fbAjaxTimelineTakeAction), postdata1, " ");                     // "https://www.facebook.com/ajax/timeline/take_action_on_story.php"

                    //    //                string postdata1 = "pmid=34&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=c&ttstamp=265816611880122110122&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&confirmed=true&ban_user=0&fb_dtsg=" + fb_dtsg;

                    //    //                string Response1 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.fbAjaxTimelineTakeAction), postdata1, "https://www.facebook.com/geetha.choubey.1/allactivity");          //https://www.facebook.com/ajax/timeline/take_action_on_story.php

                    //    //                if (Response1.Contains("This content has been deleted"))
                    //    //                {
                    //    //                    GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);
                    //    //                }

                    //    //                // delay

                    //    //                try
                    //    //                {
                    //    //                    //int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                    //    //                    //GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    //    //                    //GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    //    //                    //Thread.Sleep(delayInSeconds);
                    //    //                }
                    //    //                catch (Exception ex)
                    //    //                {

                    //    //                    GlobusLogHelper.log.Error(ex.StackTrace);
                    //    //                }

                    //    //            }
                    //    //            catch (Exception ex)
                    //    //            {
                    //    //                GlobusLogHelper.log.Error(ex.StackTrace);
                    //    //            }
                    //    //            //Thread.Sleep(1000 * 1);
                    //    //        }
                    //    //        catch (Exception ex)
                    //    //        {
                    //    //            GlobusLogHelper.log.Error(ex.StackTrace);
                    //    //        }

                    //    //    }

                    //    //}
                    //    #endregion
                    //}
                    //else
                    //{
                    //    //data = Utils.getBetween(Response_ajaxGetURL, "ScrollingPager", "pager_load_count").Replace("?data=", "");
                    //    data = AjaxGetPaginationDataFromScrollingPager(Response_ajaxGetURL, data);
                    //    if (string.IsNullOrEmpty(data))
                    //    {
                    //        data = Utils.getBetween(Response_ajaxGetURL, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    //        data = data.Replace("\\u0025", "%");
                    //        data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";
                    //    }
                    //}


                    //data = data.Replace("\\", "");

                    //int startIndx = data.IndexOf("{");
                    //data = data.Substring(startIndx);

                    //if (!data.Contains("pager_load_count"))
                    //{
                    //    data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";
                    //}
                    //else
                    //{
                    //    data = HttpUtility.UrlDecode(data) + "}";
                    //}


                    //#region MyRegion
                    //// string[] ArrStoryId11 = System.Text.RegularExpressions.Regex.Split(Response_ajaxGetURL, "story_dom_id");

                    ////foreach (var ArrStoryId_item in ArrStoryId11)
                    ////{
                    ////    if (!ArrStoryId_item.Contains("<html><body><script type=\"text/javascript\">"))
                    ////    {
                    ////        try
                    ////        {
                    ////            string ProfileiD1 = __user;
                    ////            string story_fbid1 = Utils.getBetween(ArrStoryId_item, "story_fbid=", "&");
                    ////            string story_dom_id1 = Utils.getBetween(ArrStoryId_item, "=", "&");
                    ////            string story_row_time1 = Utils.getBetween(ArrStoryId_item, "story_row_time=", "&");
                    ////            string visibility_selector_id11 = Utils.getBetween(ArrStoryId_item, "visibility_selector_id=", "\",");

                    ////            CheckStoryID.Add(story_fbid1, story_fbid1);

                    ////            try
                    ////            {

                    ////                string postURL = FBGlobals.Instance.fbAjaxTimelineTakeAction;                                                
                    ////                string postData = "pmid=34&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=b&ttstamp=265816611880122110122&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&fb_dtsg=" + fb_dtsg;//&nctr[_mod]=pagelet_all_activity_2014_1
                    ////                string Response1 = HttpHelper.postFormData(new Uri(postURL), postData, "");                             
                    ////            }
                    ////            catch (Exception ex)
                    ////            {
                    ////                GlobusLogHelper.log.Error(ex.StackTrace);
                    ////            }
                    ////            try
                    ////            {
                    ////                string postdata1 = "pmid=25&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aOGUGy38y9ACwKyaF299qzAQjFw&__req=8&ttstamp=26581657049958373&__rev=" + _rev + "&profile_id=" + ProfileiD1 + "&activity_log=1&story_dom_id=" + story_dom_id1 + "&story_fbid=" + story_fbid1 + "&story_row_time=" + story_row_time1 + "&visibility_selector_id=" + visibility_selector_id11 + "&action=remove_content&also_remove_app=0&confirmed=true&ban_user=0&fb_dtsg=" + fb_dtsg;

                    ////                string Response1 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.fbAjaxTimelineTakeAction), postdata1, " ");                     

                    ////                if (Response1.Contains("This content has been deleted"))
                    ////                {
                    ////                    GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);

                    ////                    // delay

                    ////                    try
                    ////                    {
                    ////                        int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                    ////                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    ////                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    ////                        Thread.Sleep(delayInSeconds);
                    ////                    }
                    ////                    catch (Exception ex)
                    ////                    {

                    ////                        GlobusLogHelper.log.Error(ex.StackTrace);
                    ////                    }
                    ////                }
                    ////            }
                    ////            catch (Exception ex)
                    ////            {
                    ////                GlobusLogHelper.log.Error(ex.StackTrace);
                    ////            }
                    ////            Thread.Sleep(1000 * 1);
                    ////        }
                    ////        catch (Exception ex)
                    ////        {
                    ////            GlobusLogHelper.log.Error(ex.StackTrace);
                    ////        }
                    ////    }
                    ////} 
                    //#endregion

                    //#region CodeComment


                    ////data = Utils.getBetween(Response_ajaxGetURL, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    ////data = data.Replace("\\u0025", "%");
                    ////data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";

                    ////if (farziCount==0)
                    ////{
                    ////    farziCount++;
                    ////    string data1 = Utils.getBetween(data, "{", ",\"tab_key");

                    ////    string year = GlobusHttpHelper.ParseJson(data1, "year");
                    ////    string month = GlobusHttpHelper.ParseJson(data1, "month");
                    ////    string modifiedyear = GlobusHttpHelper.ParseJson(data1, "year");
                    ////    string modifiedmonth = GlobusHttpHelper.ParseJson(data1, "month");
                    ////    if (Convert.ToInt32(month) == 1)
                    ////    {
                    ////        modifiedmonth = "12";
                    ////        modifiedyear = (Convert.ToInt32(year) - 1).ToString();
                    ////    }
                    ////    else
                    ////    {
                    ////        modifiedmonth = (Convert.ToInt32(month) - 1).ToString();
                    ////    }


                    ////    data1 = "{" + data1.Replace("month\":\"" + month + "", "month\":\"" + modifiedmonth + "").Replace("year\":\"" + year + "", "year\":\"" + modifiedyear + "") + "}";

                    ////    string getURLAfterFirst = "https://www.facebook.com/ajax/pagelet/generic.php/TimelineProfileAllActivityPagelet?ajaxpipe=1&ajaxpipe_token=" + ajaxpipe_token + "&no_script_path=1&data=" + Uri.EscapeDataString(data1) + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CCzpQ9UoHaHyG8cy8Ciq2W8GA8ABGejheCu&__req=jsonp_3&__rev=1122330&__adt=3";

                    ////    string res_getURLAfterFirst = HttpHelper.getHtmlfromUrl(new Uri(getURLAfterFirst));

                    ////    data = Utils.getBetween(res_getURLAfterFirst, "ScrollingPager", "pager_load_count").Replace("?data=", "");
                    ////    if (string.IsNullOrEmpty(data))
                    ////    {
                    ////        data = Utils.getBetween(res_getURLAfterFirst, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    ////        data = data.Replace("\\u0025", "%");
                    ////        data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";
                    ////    }
                    ////}
                    ////else
                    ////{
                    ////    data = Utils.getBetween(Response_ajaxGetURL, "ScrollingPager", "pager_load_count").Replace("?data=", "");
                    ////    if (string.IsNullOrEmpty(data))
                    ////    {
                    ////        data = Utils.getBetween(Response_ajaxGetURL, "TimelineProfileAllActivityPagelet", "pager_load_count").Replace("?data=", "");
                    ////        data = data.Replace("\\u0025", "%");
                    ////        data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + PageCounter + "}";
                    ////    }
                    ////}



                    ////data = data.Replace("\\", "");

                    ////int startIndx = data.IndexOf("{");
                    ////data = data.Substring(startIndx);

                    ////data = HttpUtility.UrlDecode(data) + "pager_load_count\":"+PageCounter+"}"; 
                    //#endregion

                    //if (!Response_ajaxGetURL.Contains("clearfix uiMorePager") && PageCounter > 4 || PageCounter >= 20)
                    //{
                    //    break;
                    //}
                    //PageCounter = PageCounter + 1; 
                    #endregion


                    if (!Response_ajaxGetURL.Contains("clearfix uiMorePager") || PageCounter >= 20)
                    {
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void StartDeleteGroupPostProcessNew(ref FacebookUser fbUser)
        {
            try
            {
                GlobusLogHelper.log.Info("Please Wait Post Delete process start " + " With UserName : " + fbUser.username);
                int countNoOfProcessDeleted = 0;
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string msg = string.Empty;
                string pageSourceFb = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));               
                string UserID = GlobusHttpHelper.GetParamValue(pageSourceFb, "user");
                string UserNameResp= HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/"+UserID));
               
                string UserName=Utils.getBetween(UserNameResp,"\"username\": \"","\"");
                string ActivityPageUrl = "https://www.facebook.com/" + UserName + "/allactivity?privacy_source=activity_log&log_filter=groups";
                string GroupActivityPage = HttpHelper.getHtmlfromUrl(new Uri(ActivityPageUrl));
                if (string.IsNullOrEmpty(GroupActivityPage))
                {
                    ActivityPageUrl = "https://www.facebook.com/profile.php?id="+UserID+"&sk=allactivity&privacy_source=activity_log&log_filter=groups";
                    GroupActivityPage = HttpHelper.getHtmlfromUrl(new Uri(ActivityPageUrl));
                }
                string fb_dtsg = GlobusHttpHelper.GetParamValue(GroupActivityPage, "fb_dtsg");
                string[] PostUrls = Regex.Split(GroupActivityPage, "SelectableMenu");
                List<string> postIDs = new List<string>();
                string deletePostData = string.Empty;
                string deleteFinalPostData = string.Empty;
                string deleteResp = string.Empty;
                string deleteFinalResp = string.Empty;
                DateTime now = DateTime.Now;
                string currentMonth = now.Month.ToString();
                string currentYear = now.Year.ToString();
                foreach(string id in PostUrls)
                {
                    if (id.Contains("confirm?identifier"))
                    {
                        try
                        {
                            string Tempid = id;
                            Tempid = Tempid.Replace("\\u0025", "%");
                            //postIDs.Add(Utils.getBetween(Tempid,"=","="));
                            Tempid = Utils.getBetween(Tempid, "=", "\"");
                            postIDs.Add(Tempid);
                            deletePostData = "identifier=" + Tempid + "3&story_dom_id=u_1e_1u&nctr[_mod]=pagelet_all_activity_" + currentYear + "_" + currentMonth + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35zoSt2u6aOGeFxq9ACxO4oKA8ABGeqrWo8pojByUWdDx24QqUkBBzEy78S8zU&__req=1h&fb_dtsg=" + fb_dtsg + "&ttstamp=265817012173104117539510210572&__rev=1542074";
                           deleteResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/timeline/delete/confirm"), deletePostData);
                            deleteFinalPostData = "fb_dtsg=" + fb_dtsg + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35zoSt2u6aOGeFxq9ACxO4oKA8ABGeqrWo8pojByUWdDx24QqUkBBzEy78S8zU&__req=14&ttstamp=265817012173104117539510210572&__rev=1542074";
                            deleteFinalResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/timeline/delete?identifier=" + Tempid), deleteFinalPostData);
                            if (!string.IsNullOrEmpty(GroupDeletePostFilePath))                         
                            {

                                string CSVHeader = "Post Id" + "," + "From" + ", " + "Date";
                                string CSV_Content = Tempid + "," + fbUser.username + ", " + DateTime.Now.ToString();
                                try
                                {

                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupDeletePostFilePath);
                                    GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                    GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            
                            
                            countNoOfProcessDeleted++;
                            int delay = new Random().Next(minDelayDeleteGroupPost, maxDelayDeleteGroupPost);
                            GlobusLogHelper.log.Info("Deleted Post : " + Tempid);
                            GlobusLogHelper.log.Info("Delaying For : " + delay +"Seconds");
                            Thread.Sleep(delay * 1000);
                            if (DeleteScheduleGroupPosting)
                            {
                                if (countNoOfProcessDeleted == DeleteNUmberOfPost)
                                {
                                    GlobusLogHelper.log.Info("Process Paused For " + IntervalTime + " Minute With User" + fbUser.username);
                                    Thread.Sleep(60 * 1000 * IntervalTime);
                                    countNoOfProcessDeleted = 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(ex.Message);
                        }
 
                    }
                }

                int k = 2;
                bool isvalidUrl = true;
                string nextPage = string.Empty;
                string ajaxpipe_token = Utils.getBetween(GroupActivityPage, "ajaxpipe_token", "\",").Replace("\":\"", "");
                int[] year={2014,2013,2012,2011,2010,2009,2008,2007,2006,2005};
                int[] month = {12,11,10,9,8,7,6,5,4,3,2,1};                
                for(int i=0;i<10;i++)
                {
                   for (int s = 0; s < 4;s++ )
                   {
                        for (int j = 0; j < 12; j++)
                        {
                            try
                            {
                                k++;
                                string NextUrl = "https://www.facebook.com/ajax/pagelet/generic.php/TimelineEntStoryActivityLogPagelet?ajaxpipe=1&ajaxpipe_token=" + ajaxpipe_token + "&no_script_path=1&data=%7B%22year%22%3A%22" + year[i] + "%22%2C%22month%22%3A%22" + month[j] + "%22%2C%22log_filter%22%3A%22groups%22%2C%22profile_id%22%3A" + UserID + "%7D&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35zoSt2u6aOGeFxq9ACxO4oKA8ABGeqrWo8pojByUWdDx24QqUkBBzEy78S8zU&__req=jsonp_" + k + "&__rev=1542074&__adt=" + k;
                                string Pagesrc = HttpHelper.getHtmlfromUrl(new Uri(NextUrl));
                                string[] PageSrcSplits = Regex.Split(Pagesrc, "SelectableMenu");
                                foreach (string id in PageSrcSplits)
                                {
                                    if (id.Contains("confirm?identifier"))
                                    {
                                        try
                                        {
                                            string Tempid = id;
                                            Tempid = Tempid.Replace("\\u0025", "%");
                                            //postIDs.Add(Utils.getBetween(Tempid,"=","="));
                                            Tempid = Utils.getBetween(Tempid, "=", "\"");
                                            postIDs.Add(Tempid);
                                            deletePostData = "identifier=" + Tempid + "3&story_dom_id=u_1e_1u&nctr[_mod]=pagelet_all_activity_" +year[i] + "_" +month[j] + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35zoSt2u6aOGeFxq9ACxO4oKA8ABGeqrWo8pojByUWdDx24QqUkBBzEy78S8zU&__req=1h&fb_dtsg=" + fb_dtsg + "&ttstamp=265817012173104117539510210572&__rev=1542074";
                                            deleteResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/timeline/delete/confirm"), deletePostData);
                                            deleteFinalPostData = "fb_dtsg=" + fb_dtsg + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35zoSt2u6aOGeFxq9ACxO4oKA8ABGeqrWo8pojByUWdDx24QqUkBBzEy78S8zU&__req=14&ttstamp=265817012173104117539510210572&__rev=1542074";
                                            deleteFinalResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/timeline/delete?identifier=" + Tempid), deleteFinalPostData);
                                            GlobusLogHelper.log.Info("Deleted Post : " + Tempid);
                                            if (!string.IsNullOrEmpty(GroupDeletePostFilePath))
                                            {

                                                string CSVHeader = "Post Id" + "," + "From" + ", " + "Date";
                                                string CSV_Content = Tempid + "," + fbUser.username + ", " + DateTime.Now.ToString();
                                                try
                                                {

                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupDeletePostFilePath);
                                                    GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                                    GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            } 

                                            countNoOfProcessDeleted++;
                                            int delay = new Random().Next(minDelayDeleteGroupPost, maxDelayDeleteGroupPost);
                                            GlobusLogHelper.log.Info("Delaying For : " + delay + "Seconds");
                                            Thread.Sleep(delay * 1000);
                                            if (DeleteScheduleGroupPosting)
                                            {
                                                if (countNoOfProcessDeleted == DeleteNUmberOfPost)
                                                {
                                                    GlobusLogHelper.log.Info("Process Paused For "+IntervalTime+" Minute With User"+fbUser.username);
                                                    Thread.Sleep(60 * 1000 * IntervalTime);
                                                    countNoOfProcessDeleted = 0;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.Message);
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.Message);
                            }
                        }
                }
                }
                GlobusLogHelper.log.Info("Process Completed With User"+fbUser.username);              
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        private static string AjaxGetPaginationDataFromScrollingPager(string pgSrc_FanPageSearch, string data)
        {
            //}, {\"
            data = Utils.getBetween(pgSrc_FanPageSearch, "ScrollingPager", "}, {").Replace("?data=", "");//Utils.getBetween(pgSrc_FanPageSearch, "ScrollingPager", "pager_load_count").Replace("?data=", "");
            data = data.Replace("\\", "");

            if (data.Contains("{"))
            {
                int startIndx = data.IndexOf("{");
                data = data.Substring(startIndx);
            }


            //data = HttpUtility.UrlDecode(data) + "pager_load_count\":" + 1 + "}";
            return data;
        }

        public void StartGroupComapaignProcess(ref FacebookUser fbUser)
        {
            try
            {

                if (isStopGroupCamapinScheduler)
                {
                    return;
                }

                List<string> lstallgroup_EachAccount = new List<string>();
                Queue<string> Queue_GroupUrls = new Queue<string>();
                int spanhour = 0;
                int spanminut = 0;
                int NumberofHour = 1;

                try
                {
                    int timeforperMessages = (NumberofHour * 60 * 60) / faceboardpro.FbGroupCampaignManagerGlobals.NoOfMessageserHour;

                    int delayMessageSending = timeforperMessages * 1000 + Utils.GenerateRandom(1 * 1000, 5 * 1000);
                }
                catch (Exception ex)
                {
                    //GlobusLogHelper.log.Error(ex.StackTrace);
                }



                GlobusHttpHelper globusHttpHelpr = fbUser.globusHttpHelper;
                ChilkatHttpHelpr chilkatHttpHelpr = new ChilkatHttpHelpr();



                string homePageSource = globusHttpHelpr.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookUrl));
                try
                {
                    lstallgroup_EachAccount = FindOwnGroupUrl(ref fbUser);
                    GlobusLogHelper.log.Info("Username : " + fbUser.username + " Get GroupUrl : " + lstallgroup_EachAccount.Count);

                    {

                        {
                            try
                            {
                                //Code for Insert data in Databse.
                                // string InsertValue = "insert into tb_GroupCampaignMgmt(Account,GroupUrl,GrpCmpName) values('" + facebooker.Username + "','" + lstallgroup_EachAccount[i] + "','" + GroupCampaigName + "')";
                                //  BaseLib.DataBaseHandler.InsertQuery(InsertValue, "tb_GroupCampaignMgmt");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
                bool IsMessagePostingError = false;
                Boolean IssendMessage = false;
                string MessagePostingIssue = string.Empty;
                string username = fbUser.username;
                string message = string.Empty;
                int Grpcounter = 0;
                List<string> lstallgroup_EachAccount1 = new List<string>();
                int Conuter = 0;
                while (true)
                {
                    try
                    {
                        if (fbUser.isloggedin)
                        {
                            string grpurl = string.Empty;
                            lock (locker_Queue_UserIDs)
                            {
                                if (Queue_GroupUrls.Count == 0)
                                {
                                    try
                                    {
                                        foreach (var item in lstallgroup_EachAccount)
                                        {
                                            try
                                            {
                                                lstallgroup_EachAccount1.Add(item);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(ex.StackTrace);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.StackTrace);
                                    }

                                    Conuter = lstallgroup_EachAccount.Count;

                                    foreach (string item in lstallgroup_EachAccount)
                                    {
                                        try
                                        {
                                            Queue_GroupUrls.Enqueue(item);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }
                                }
                                try
                                {
                                    grpurl = Queue_GroupUrls.Dequeue();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                }
                            }

                            if (Conuter == Grpcounter && !chkCountinueProcessGroupCamapinScheduler)
                            {
                                GlobusLogHelper.log.Info("Process completed with  :  " + username);
                                GlobusLogHelper.log.Debug("Process completed with  :  " + username);

                                break;
                            }
                            Grpcounter++;
                            {

                                if (faceboardpro.FbGroupCampaignManagerGlobals.MessageMode == "One Message")
                                {
                                    message = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;
                                }
                                else
                                {
                                    lock (locker_Queue_Messages)
                                    {
                                        if (Queue_Messages.Count == 0)
                                        {
                                            try
                                            {
                                                if (LstMessageUrlsGroupCampaignManager.Count > 0)
                                                {
                                                    foreach (string item in LstMessageUrlsGroupCampaignManager)
                                                    {
                                                        try
                                                        {
                                                            Queue_Messages.Enqueue(item);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(ex.StackTrace);
                                            }
                                        }
                                        try
                                        {
                                            message = Queue_Messages.Dequeue();

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                        }
                                    }
                                }

                                string Picpath = string.Empty;
                                string VideoUrl = string.Empty;

                                try
                                {
                                    // Picpath = LstPicUrlsGroupCampaignManager[new Random().Next(0, LstPicUrlsGroupCampaignManager.Count - 1)];
                                    Picpath = LstPicUrlsGroupCampaignManager[new Random().Next(LstPicUrlsGroupCampaignManager.Count)];

                                    //Picpath = "C:\\Users\\Public\\Pictures\\Sample Pictures";
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                }
                                try
                                {
                                    VideoUrl = LstVideoUrlsGroupCampaignManager[new Random().Next(0, LstVideoUrlsGroupCampaignManager.Count - 1)];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                }


                                if (faceboardpro.FbGroupCampaignManagerGlobals.MessageMode == "One Message")
                                {

                                    
                                    if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Picture with message")
                                    {
                                        try
                                        {
                                            IssendMessage = false;
                                            message = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;

                                            if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                            {
                                                DataSet ds = new DataSet();
                                                RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, message);

                                                if (CheckDataBaseGroupCampaimanager == false)
                                                {
                                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + message + " GroupUrl  >>>" + grpurl);
                                                }
                                                else
                                                {
                                                    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);
                                                    // IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, List<string> Picpath, ref fbUser);
                                                }
                                            }
                                            else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                            {
                                                if (ChkbGroupGrpRequestManagerMultiplePicPerGroup == true)
                                                {
                                                    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, LstPicUrlsGroupCampaignManager, ref fbUser);
                                                }
                                                else
                                                {
                                                    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);
                                                }
                                            }

                                            if (IssendMessage)
                                            {

                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " and Picture Path " + Picpath + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query
                                                }
                                                string GetPostUrl = string.Empty;
                                                string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                int i = 0;
                                                if (IssendMessage)
                                                {
                                                    if (string.IsNullOrEmpty(GetPostUrl))
                                                    {
                                                        try
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");

                                                            foreach (var arr_item in arr)
                                                            {
                                                                try
                                                                {
                                                                    i = i + 1;
                                                                   
                                                                    if (arr_item.Contains("Just now") && !arr_item.Contains("<!DOCTYPE html>"))
                                                                    {
                                                                        GetPostUrl = Utils.getBetween(arr_item, "href=\"", "&amp;");
                                                                        if (GetPostUrl.Contains("<abbr"))
                                                                        {
                                                                            string[] arr11 = System.Text.RegularExpressions.Regex.Split(GetPostUrl,"<abbr");
                                                                            if (arr11[0].Contains("facebook.com"))
                                                                            {
                                                                                GetPostUrl = arr11[0];
                                                                            }
                                                                            else
                                                                            {
                                                                                GetPostUrl="https://www.facebook.com/"+arr11[0];
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                    if (i == 4)
                                                                    {
                                                                        break;
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                }
                                                            }

                                                          if(Pagesource.Contains("posts pending approval"))
                                                            {
                                                                GetPostUrl = "Post pending approval.";
                                                            }
                                                            else if(string.IsNullOrEmpty(GetPostUrl) && !Pagesource.Contains("post pending approval."))
                                                            {
                                                                string[] arr11 = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                GetPostUrl =  Utils.getBetween(arr11[1], "href=\"", "&amp;").Replace("amp;", "").Replace("\"", "").Trim();
                                                                if (!GetPostUrl.Contains("www.facebook.com"))
                                                                {
                                                                    GetPostUrl = "https://www.facebook.com" + GetPostUrl;
                                                                }
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }



                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," +"PostUrl" +"," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }


                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                    ///Added by gargi on 17th August 2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);

                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                                try
                                                {
                                                    //int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                    //GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    //Thread.Sleep(delayInSeconds);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                        }
                                    }
                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Message")
                                    {
                                        try
                                        {
                                            IssendMessage = false;
                                            message = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;

                                            if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                            {
                                                DataSet ds = new DataSet();
                                                RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, message);

                                                if (CheckDataBaseGroupCampaimanager == false)
                                                {
                                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + message + " GroupUrl  >>>" + grpurl);
                                                }
                                                else
                                                {
                                                    IssendMessage = SendingMsgToGroups(grpurl, message, ref fbUser);

                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                            {
                                                IssendMessage = SendingMsgToGroups(grpurl, message, ref fbUser);
                                                //IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);

                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }
                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query 

                                                }
                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                    //Added by ajay yadav on 27th August 2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }

                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Video")
                                    {
                                        try
                                        {
                                            message = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;
                                            IssendMessage = false;

                                            //check database 
                                            if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                            {
                                                DataSet ds = new DataSet();
                                                RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, VideoUrl);

                                                if (CheckDataBaseGroupCampaimanager == false)
                                                {
                                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + VideoUrl + " GroupUrl  >>>" + grpurl);

                                                }
                                                else
                                                {
                                                    if (VideoUrl.Contains("/photos/a"))
                                                    {
                                                        IssendMessage = PostVideoUrlUpdated(grpurl, message, VideoUrl, ref  fbUser);
                                                    }
                                                    else
                                                    {
                                                        IssendMessage = PostVideoUrl(grpurl, message, VideoUrl, ref  fbUser);//, ref facebooker.Password, ref globusHttpHelpr);
                                                    }
                                                    string GetPostUrl = string.Empty;
                                                    string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                    if (IssendMessage)
                                                    {
                                                        if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                            GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                        }
                                                    }

                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                            {
                                                if (VideoUrl.Contains("/photos/a"))
                                                {
                                                    IssendMessage = PostVideoUrlUpdated(grpurl, message, VideoUrl, ref  fbUser);
                                                }
                                                else
                                                {
                                                    IssendMessage = PostVideoUrl(grpurl, message, VideoUrl, ref  fbUser);
                                                }
                                               
                                               
                                                string GetPostUrl = string.Empty;
                                                string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                if (IssendMessage)
                                                {
                                                    if (Pagesource.Contains("post pending approval."))
                                                    {
                                                        GetPostUrl = "pending approval.";
                                                    }
                                                    else if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>")&&!Pagesource.Contains("post pending approval."))
                                                    {
                                                        string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                        GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                    }

                                                    if (string.IsNullOrEmpty(GetPostUrl))
                                                    {
                                                        try
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                            int i = 0;
                                                            foreach (var arr_item in arr)
                                                            {
                                                                try
                                                                {
                                                                    i = i + 1;
                                                                    if (arr_item.Contains("Just now") && !arr_item.Contains("<!DOCTYPE html>"))
                                                                    {
                                                                        GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr_item, "href=\"", "\"><abbr");
                                                                        break;
                                                                    }
                                                                    if (i==4)
                                                                    {
                                                                        break;
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                }
                                                            }
                                                            
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }


                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }

                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + "Video Url : " + VideoUrl + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query 

                                                }

                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);                                                 
                                                    // add by ajay yadav 26 /08/2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }
                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "")
                                    {
                                        try
                                        {
                                            IssendMessage = false;
                                            message = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;
                                            IssendMessage = SendingMsgToGroups(grpurl, message, ref fbUser);
                                            try
                                            {
                                                string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                            }

                                            if (IssendMessage)
                                            {
                                                Message_Counter = Message_Counter + 1;
                                                TimeCounter = TimeCounter + 1;
                                                GlobusLogHelper.log.Info("Sent Message to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Message to GroupUrl : " + grpurl);


                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                            }
                                            if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                            {
                                                DataSet ds = new DataSet();
                                                RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, message);

                                                if (CheckDataBaseGroupCampaimanager == false)
                                                {
                                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + message + " GroupUrl  >>>" + grpurl);
                                                }
                                                else
                                                {
                                                    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);
                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                            {
                                                IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);
                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }

                                            if (IssendMessage)
                                            {
                                                Message_Counter = Message_Counter + 1;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info("Sent Picture Message to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Picture Message to GroupUrl : " + grpurl);


                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                            }
                                            if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                            {
                                                DataSet ds = new DataSet();
                                                RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, VideoUrl);

                                                if (CheckDataBaseGroupCampaimanager)
                                                {
                                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + VideoUrl + " GroupUrl  >>>" + grpurl);
                                                }
                                                else
                                                {
                                                    if (VideoUrl.Contains("/photos/a"))
                                                    {
                                                        IssendMessage = PostVideoUrlUpdated(grpurl, message, VideoUrl, ref  fbUser);
                                                    }
                                                    else
                                                    {
                                                        IssendMessage = PostVideoUrl(grpurl, message, VideoUrl, ref fbUser);
                                                    }
                                                    string GetPostUrl = string.Empty;
                                                    string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                    if (IssendMessage)
                                                    {
                                                        if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                            GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                        }
                                                        else if (Pagesource.Contains("You have 1 post pending approval."))
                                                        {
                                                            GetPostUrl = "You have 1 post pending approval.";
                                                        }
                                                    }

                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                            {
                                                IssendMessage = PostVideoUrl(grpurl, message, VideoUrl, ref fbUser);

                                                string GetPostUrl = string.Empty;
                                                string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                if (IssendMessage)
                                                {
                                                    if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                    {
                                                        string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                        GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                    }
                                                    else if (Pagesource.Contains("You have 1 post pending approval."))
                                                    {
                                                        GetPostUrl = "You have 1 post pending approval.";
                                                    }
                                                }

                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }
                                            if (IssendMessage)
                                            {
                                                Message_Counter = Message_Counter + 1;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info("Sent Video Url to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Video Url to GroupUrl : " + grpurl);

                                            }

                                            if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                            {
                                                if (ScheduleGroupPosting)
                                                {
                                                    TimeCounter = 0;
                                                    GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                    GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                    // TimeCounter = 0;

                                                    Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                    GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                    GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                ///Added by ajay yadav on 26th August 2013
                                                try
                                                {
                                                    int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    Thread.Sleep(delayInSeconds);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                                }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }

                                }
                                else
                                {

                                    if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Picture with message")
                                    {
                                        try
                                        {
                                            try
                                            {
                                                Picpath = LstPicUrlsGroupCampaignManager[new Random().Next(0, LstPicUrlsGroupCampaignManager.Count - 1)];
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(ex.StackTrace);
                                            }

                                            //try
                                            //{
                                            //    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, LstPicUrlsGroupCampaignManager, ref fbUser);
                                            //}
                                            //catch (Exception ex)
                                            //{
                                            //    GlobusLogHelper.log.Error(ex.StackTrace);
                                            //}


                                            if (chkCountinueProcessGroupCamapinScheduler == true)
                                            {

                                                IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, LstPicUrlsGroupCampaignManager, ref fbUser);

                                                string GetPostUrl = string.Empty;
                                                string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                int i = 0;
                                                if (IssendMessage)
                                                {
                                                    if (string.IsNullOrEmpty(GetPostUrl))
                                                    {
                                                        try
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");

                                                            foreach (var arr_item in arr)
                                                            {
                                                                try
                                                                {
                                                                    i = i + 1;

                                                                    if (arr_item.Contains("Just now") && !arr_item.Contains("<!DOCTYPE html>"))
                                                                    {
                                                                        GetPostUrl = Utils.getBetween(arr_item, "href=\"", "&amp;");
                                                                        if (GetPostUrl.Contains("<abbr"))
                                                                        {
                                                                            string[] arr11 = System.Text.RegularExpressions.Regex.Split(GetPostUrl, "<abbr");
                                                                            if (arr11[0].Contains("facebook.com"))
                                                                            {
                                                                                GetPostUrl = arr11[0];
                                                                            }
                                                                            else
                                                                            {
                                                                                GetPostUrl = "https://www.facebook.com/" + arr11[0];
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                    if (i == 4)
                                                                    {
                                                                        break;
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                }
                                                            }

                                                            if (Pagesource.Contains("posts pending approval"))
                                                            {
                                                                GetPostUrl = "Post pending approval.";
                                                            }
                                                            else if (string.IsNullOrEmpty(GetPostUrl) && !Pagesource.Contains("post pending approval."))
                                                            {
                                                                string[] arr11 = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                GetPostUrl = Utils.getBetween(arr11[1], "href=\"", "&amp;").Replace("amp;", "").Replace("\"", "").Trim();
                                                                if (!GetPostUrl.Contains("www.facebook.com"))
                                                                {
                                                                    GetPostUrl = "https://www.facebook.com" + GetPostUrl;
                                                                }
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }


                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PicturePostUrl" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }

                                            else
                                            {
                                                if (ChkbGroupGrpRequestManagerMultiplePicPerGroup == true)
                                                {

                                                    IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, LstPicUrlsGroupCampaignManager, ref fbUser);



                                                    //IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);

                                                    string GetPostUrl = string.Empty;
                                                    string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                    int i = 0;
                                                    if (IssendMessage)
                                                    {
                                                        if (string.IsNullOrEmpty(GetPostUrl))
                                                        {
                                                            try
                                                            {
                                                                string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");

                                                                foreach (var arr_item in arr)
                                                                {
                                                                    try
                                                                    {
                                                                        i = i + 1;

                                                                        if (arr_item.Contains("Just now") && !arr_item.Contains("<!DOCTYPE html>"))
                                                                        {
                                                                            GetPostUrl = Utils.getBetween(arr_item, "href=\"", "&amp;");
                                                                            if (GetPostUrl.Contains("<abbr"))
                                                                            {
                                                                                string[] arr11 = System.Text.RegularExpressions.Regex.Split(GetPostUrl, "<abbr");
                                                                                if (arr11[0].Contains("facebook.com"))
                                                                                {
                                                                                    GetPostUrl = arr11[0].Replace("/\">",string.Empty);
                                                                                }
                                                                                else
                                                                                {
                                                                                    GetPostUrl = "https://www.facebook.com" + arr11[0].Replace("/\">", string.Empty); 
                                                                                }
                                                                            }
                                                                            break;
                                                                        }
                                                                        if (i == 4)
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                    }
                                                                }

                                                                if (Pagesource.Contains("posts pending approval"))
                                                                {
                                                                    GetPostUrl = "Post pending approval.";
                                                                }
                                                                else if (string.IsNullOrEmpty(GetPostUrl) && !Pagesource.Contains("post pending approval."))
                                                                {
                                                                    string[] arr11 = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                    GetPostUrl = Utils.getBetween(arr11[1], "href=\"", "&amp;").Replace("amp;", "").Replace("\"", "").Replace("/\">", string.Empty).Trim();
                                                                    if (!GetPostUrl.Contains("www.facebook.com"))
                                                                    {
                                                                        GetPostUrl = "https://www.facebook.com" + GetPostUrl.Replace("/\">", string.Empty); ;
                                                                    }
                                                                }

                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                            }
                                                        }
                                                    }


                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PicturePostUrl" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                                else
                                                {
                                                    if (isPostMessageWithImageAsEdited)
                                                    {
                                                        IssendMessage = SendingPicMsgToOwnGroupAsEdited(grpurl, message, Picpath, ref fbUser);
                                                    }
                                                    else
                                                    {
                                                        IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Picpath, ref fbUser);
                                                    }
                                                      string GetPostUrl = string.Empty;




                                                    //string GetPostUrl = string.Empty;
                                                    string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                    int i = 0;
                                                    if (IssendMessage)
                                                    {
                                                        if (string.IsNullOrEmpty(GetPostUrl))
                                                        {
                                                            try
                                                            {
                                                                string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");

                                                                foreach (var arr_item in arr)
                                                                {
                                                                    try
                                                                    {
                                                                        i = i + 1;

                                                                        if (arr_item.Contains("Just now") && !arr_item.Contains("<!DOCTYPE html>"))
                                                                        {
                                                                            GetPostUrl = Utils.getBetween(arr_item, "href=\"", "&amp;");
                                                                            if (GetPostUrl.Contains("<abbr"))
                                                                            {
                                                                                string[] arr11 = System.Text.RegularExpressions.Regex.Split(GetPostUrl, "<abbr");
                                                                                if (arr11[0].Contains("facebook.com"))
                                                                                {
                                                                                    GetPostUrl = arr11[0].Replace("/\">", string.Empty);
                                                                                }
                                                                                else
                                                                                {
                                                                                    GetPostUrl = "https://www.facebook.com" + arr11[0].Replace("/\">", string.Empty);
                                                                                }
                                                                            }
                                                                            break;
                                                                        }
                                                                        if (i == 4)
                                                                        {
                                                                            break;
                                                                        }
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                    }
                                                                }

                                                                if (Pagesource.Contains("posts pending approval"))
                                                                {
                                                                    GetPostUrl = "Post pending approval.";
                                                                }
                                                                else if (string.IsNullOrEmpty(GetPostUrl) && !Pagesource.Contains("post pending approval."))
                                                                {
                                                                    string[] arr11 = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                    GetPostUrl = Utils.getBetween(arr11[1], "href=\"", "&amp;").Replace("amp;", "").Replace("\"", "").Replace("/\">", string.Empty).Trim();
                                                                    if (!GetPostUrl.Contains("www.facebook.com"))
                                                                    {
                                                                        GetPostUrl = "https://www.facebook.com" + GetPostUrl.Replace("/\">", string.Empty); ;
                                                                    }
                                                                }

                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                            }
                                                        }
                                                    }


                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PicturePostUrl" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }

                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " and Picture Path " + Picpath + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + " Message : " + message + " and Picture Path " + Picpath + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query 

                                                }


                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                    ///Added by ajay yadav on 26th August 2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With User "+fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With User " + fbUser.username);
                                                // GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                // GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }
                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Message")
                                    {
                                        try
                                        {
                                            try
                                            {
                                                IssendMessage = SendingMsgToGroups(grpurl, message, ref fbUser);
                                               

                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(ex.StackTrace);
                                            }


                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + " Message : " + message + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);

                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query 

                                                }


                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                    ///Added by gargi on 17th August 2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl+" With Username "+fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                    }

                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "Only Video")
                                    {
                                        try
                                        {

                                            string VideoUrlMsg = LstVideoUrlsGroupCampaignManager[new Random().Next(0, LstVideoUrlsGroupCampaignManager.Count)];
                                            IssendMessage = false;
                                            try
                                            {
                                                if (ChkViewSchedulerTaskUniquePostPerGroup == true)
                                                {
                                                    DataSet ds = new DataSet();
                                                    RaiseEventGrpCampaignMgr(ds, "Model : GroupCompaignRepository", "Function :  CheckGroupCompaignReport", grpurl, VideoUrlMsg);

                                                    if (CheckDataBaseGroupCampaimanager == false)
                                                    {
                                                        GlobusLogHelper.log.Debug(" Message All ready send  >>  " + VideoUrlMsg + " GroupUrl  >>>" + grpurl);

                                                    }
                                                    else
                                                    {
                                                        if (VideoUrl.Contains("/photos/a"))
                                                        {
                                                            IssendMessage = PostVideoUrlUpdated(grpurl, message, VideoUrl, ref  fbUser);
                                                        }
                                                        else
                                                        {
                                                            IssendMessage = PostVideoUrl(grpurl, message, VideoUrlMsg, ref fbUser);
                                                        }
                                                        string GetPostUrl = string.Empty;
                                                        string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                        if (IssendMessage)
                                                        {
                                                            if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                            {
                                                                try
                                                                {
                                                                    string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                    GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                }

                                                            }
                                                            else if (Pagesource.Contains("You have 1 post pending approval."))
                                                            {
                                                                GetPostUrl = "You have 1 post pending approval.";
                                                            }
                                                        }

                                                        try
                                                        {
                                                            string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                            string CSV_Content = username + "," + grpurl + "," + message.Replace(",",string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else if (ChkViewSchedulerTaskUniquePostPerGroup == false)
                                                {
                                                    string GetPostUrl = string.Empty;
                                                    if (isPostVideoUrlAsImage)
                                                    {
                                                        PostVideoUrlasImage(grpurl, message, VideoUrlMsg, ref fbUser);
                                                    }
                                                    else
                                                    {
                                                        if (VideoUrl.Contains("/photos/a"))
                                                        {
                                                            IssendMessage = PostVideoUrlUpdated(grpurl, message, VideoUrl, ref  fbUser);
                                                        }
                                                        else
                                                        {
                                                            IssendMessage = PostVideoUrl(grpurl, message, VideoUrlMsg, ref fbUser);
                                                        }
                                                        if (IssendMessage)
                                                        {
                                                            //IssendMessage = PostVideoUrlNew(grpurl, message, VideoUrlMsg, ref fbUser);
                                                        }
                                                      
                                                       
                                                        string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                        if (IssendMessage)
                                                        {
                                                            if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                            {
                                                                try
                                                                {
                                                                    string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                                    GetPostUrl = "https://www.facebook.com" + Utils.getBetween(arr[1], "href=\"", "/\"><abbr title=\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                                }

                                                            }
                                                            else if (Pagesource.Contains("You have 1 post pending approval."))
                                                            {
                                                                GetPostUrl = "You have 1 post pending approval.";
                                                            }
                                                        }
                                                    }
                                                    try
                                                    {
                                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PostUrl" + "," + "DateTime";
                                                        string CSV_Content = username + "," + grpurl + "," + message.Replace(",", string.Empty) + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(ex.StackTrace);
                                            }

                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info(Message_Counter + "Video Url : " + VideoUrl + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + "Video Url : " + VideoUrl + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);



                                                if (!chkCountinueProcessGroupCamapinScheduler)
                                                {
                                                    ///Insert query 
                                                }

                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                    ///Added by gargi on 17th August 2013
                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                                int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                Thread.Sleep(delayInSeconds);
                                            }
                                   
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                            return;
                                        }
                                    }

                                    else if (faceboardpro.FbGroupCampaignManagerGlobals.MessageType == "")
                                    {
                                        try
                                        {
                                            IssendMessage = false;
                                            try
                                            {
                                                GlobusLogHelper.log.Info("UserName :" + username + "Targeturl :" + grpurl + "Message : " + message);
                                                GlobusLogHelper.log.Debug("UserName :" + username + "Targeturl :" + grpurl + "Message : " + message);


                                                IssendMessage = SendingMsgToGroups(grpurl, message, ref fbUser);
                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message + "," + Picpath + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info("Sent Message to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Message to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + " Message : " + message + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);

                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    try
                                                    {
                                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                        Thread.Sleep(delayInSeconds);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                                //GlobusLogHelper.log.Debug("You have been temporarily blocked from performing this action." + grpurl);

                                                try
                                                {
                                                    int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    Thread.Sleep(delayInSeconds);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                                }

                                            }
                                            string PicpathMsg = LstPicUrlsGroupCampaignManager[new Random().Next(0, LstPicUrlsGroupCampaignManager.Count - 1)];
                                            try
                                            {
                                                IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, PicpathMsg, ref fbUser);
                                                string GetPostUrl = string.Empty;
                                                string Pagesource = globusHttpHelpr.getHtmlfromUrl(new Uri(grpurl));
                                                if (IssendMessage)
                                                {
                                                    if (!Pagesource.Contains("<span class=\"_50f8 _50f7\">PINNED POST</span>"))
                                                    {
                                                        try
                                                        {
                                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Pagesource, "<span class=\"fsm fwn fcg\">");
                                                            GetPostUrl = Utils.getBetween(arr[1], "href=\"", "rel=\"theater\"").Replace("amp;", "").Replace("\"", "").Trim();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                        }
                                                    }
                                                }


                                                try
                                                {
                                                    string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "FilePath" + "," + "PicturePostUrl" + "," + "DateTime";
                                                    string CSV_Content = username + "," + grpurl + "," + message + "," + Picpath + "," + GetPostUrl + "," + DateTime.Now;
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            if (IssendMessage)
                                            {

                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info("Sent Picture Message to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Picture Message to GroupUrl : " + grpurl);

                                                GlobusLogHelper.log.Info(Message_Counter + " Message : " + message + " and Picture Path " + Picpath + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + " Message : " + message + " and Picture Path " + Picpath + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);

                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Unable to post on : " + grpurl + " With Username " + fbUser.username);
                                                //  GlobusLogHelper.log.Info("You have been temporarily blocked from performing this action." + grpurl);
                                            }
                                            string VideoUrlMsg = string.Empty;
                                            try
                                            {
                                                VideoUrlMsg = LstVideoUrlsGroupCampaignManager[new Random().Next(0, LstVideoUrlsGroupCampaignManager.Count - 1)];
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                            try
                                            {
                                                IssendMessage = PostVideoUrl(grpurl, message, VideoUrlMsg, ref fbUser);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                            if (IssendMessage)
                                            {
                                                Message_Counter++;
                                                TimeCounter++;
                                                GlobusLogHelper.log.Info("Sent Video Url to GroupUrl : " + grpurl);
                                                GlobusLogHelper.log.Debug("Sent Video Url to GroupUrl : " + grpurl);

                                                GlobusLogHelper.log.Info(Message_Counter + "Video Url : " + VideoUrl + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);
                                                GlobusLogHelper.log.Debug(Message_Counter + "Video Url : " + VideoUrl + " Sent to : " + grpurl + " Using UserName : " + username + "       Date_Time : " + DateTime.Now);

                                                if (TimeCounter >= CheckGroupCompaignNoOfGroupsInBatch)
                                                {
                                                    if (ScheduleGroupPosting)
                                                    {
                                                        TimeCounter = 0;
                                                        GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupCompaign_InterbalInMinuts + "  : Minutes With :" + fbUser.username);

                                                        // TimeCounter = 0;

                                                        Thread.Sleep(1 * 1000 * 60 * CheckGroupCompaign_InterbalInMinuts);
                                                        GlobusLogHelper.log.Debug("Process Continue ..  With :" + fbUser.username);
                                                        GlobusLogHelper.log.Info("Process Continue . With :" + fbUser.username);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                                ///Added by ajay yadav on 26th August 2013
                                                try
                                                {
                                                    int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + username);
                                                    Thread.Sleep(delayInSeconds);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error(ex.StackTrace);
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
                        }
                        else
                        {
                            break;
                        }
                        if (!chkCountinueProcessGroupCamapinScheduler)
                        {
                            if (Grpcounter == lstallgroup_EachAccount.Count)
                            {
                                if (Grpcounter <= 1)
                                {
                                    GlobusLogHelper.log.Info("Message already send please update compaign name!");
                                    GlobusLogHelper.log.Debug("Message already send please update compaign name!");
                                }
                                GlobusLogHelper.log.Info("process Completed !!!");
                                GlobusLogHelper.log.Debug("process Completed !!!");
                                break;
                            }
                        }
                        message = "";
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

        private List<string> FindOwnGroupUrl(ref FacebookUser fbUser)
        {
            //LstGroupUrlsViewSchedulerTaskTargetedUrls

            List<string> list_Group = new List<string>();

            if (!CheckTargetedGroupUrlsUse)
            {

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookBookMarksUrl));

                try
                {

                    string[] allgroup = Regex.Split(pgSrc_FanPageSearch, "href=\"/groups/");
                    foreach (string grpitem in allgroup)
                    {
                        try
                        {

                            if (!grpitem.Contains("<!DOCTYPE html>") && grpitem.Contains("/groups/") && grpitem.Contains("group_user"))
                            {
                                try
                                {
                                    string[] group = Regex.Split(grpitem, "/");

                                    string itemgroups = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + group[0];
                                    list_Group.Add(itemgroups);
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
                    list_Group = list_Group.Distinct().ToList();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (list_Group.Count() == 0)
                {

                    //href="/ajax/bookmark/groups/leave/?group_id=             
                    string[] allgroup = Regex.Split(pgSrc_FanPageSearch, "group_id");
                    foreach (string grpitem in allgroup)
                    {
                        try
                        {

                            if (!grpitem.Contains("<!DOCTYPE html>") && (grpitem.Contains("favoriteOption") || grpitem.Contains("Leave Group")))
                            {
                                try
                                {
                                    string group = Utils.getBetween(grpitem, "=", "\"");
                                    if (group.Contains("&amp;"))
                                    {
                                        string[] arr = System.Text.RegularExpressions.Regex.Split(group, "&amp;");
                                        group = arr[0];
                                    }
                                    string itemgroups = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + group;
                                    list_Group.Add(itemgroups);
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

                list_Group = list_Group.Distinct().ToList();
                return list_Group;
            }
            else
            {
                list_Group = LstGroupUrlsViewSchedulerTaskTargetedUrls.Distinct().ToList();
                return list_Group;
            }

        }

        public bool SendingMsgToOwnGroup(string targeturl, ref FacebookUser fbUser)
        {

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string msg = string.Empty;


            GlobusLogHelper.log.Info("Logged in with " + fbUser.username);
            GlobusLogHelper.log.Debug("Logged in with " + fbUser.username);

            try
            {
                try
                {
                    msg = faceboardpro.FbGroupCampaignManagerGlobals.TextMessage;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                // For DB

                DataSet ds = new DataSet();

                RaiseEvent(ds, "Model : GroupCompaignRepository", "Function : GetAllGroupCompaignReport", targeturl, msg);


                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl));
                List<string> status = HttpHelper.GetTextDataByTagAndAttributeName(pgSrc_FanPageSearch, "span", "uiButtonText");
                for (int i = 0; i < status.Count; i++)
                {
                    Msgsendingstatus = status[i];
                    if (Msgsendingstatus != string.Empty)
                    {
                        Msgsendingcurrentstatus = Msgsendingstatus;

                    }
                }
                if (Msgsendingcurrentstatus.Contains("Notifications"))
                {

                    if (pgSrc_FanPageSearch.Contains("xhpc_composerid") || pgSrc_FanPageSearch.Contains("xhpc_targetid"))//pgSrc_FanPageSearch.Contains("Write Post") &&
                    {
                        try
                        {
                            string __user = "";
                            string fb_dtsg = "";
                            string xhpc_composerid = string.Empty;
                            string xhpc_targetid = string.Empty;
                            int composer_session_id;


                            __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");

                            if (string.IsNullOrEmpty(__user))
                            {
                                __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                            }

                            fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                            if (string.IsNullOrEmpty(fb_dtsg))
                            {
                                fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                            }
                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                            composer_session_id = Convert.ToInt32(BaseLib.Utils.ConvertToUnixTimestamp(DateTime.Now));
                            string composer_sessionid = composer_session_id.ToString();


                            string postdata = "fb_dtsg=" + fb_dtsg + "&postfromfull=true&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_fbx=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_message_text=" + msg + "&xhpc_message=" + msg + "&is_explicit_place=&composertags_place=&composertags_place_name=&composer_session_id=" + composer_session_id + "&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=1658165781016912151427";

                            string posturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxUpdateStatusUrl;
                            string Response = HttpHelper.postFormData(new Uri(posturl), postdata, "");
                            int length = Response.Length;
                            if (Response.Contains(msg) || length > 5000)
                            {
                                if (true)
                                {
                                    try
                                    {
                                        GlobusLogHelper.log.Info(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupManager * 1000, maxDelayGroupManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds");
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds");
                                        Thread.Sleep(delayInSeconds);
                                        return true;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        GlobusLogHelper.log.Info(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Message Sending Fail");
                            }
                            string crtStatus = "Joined";

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return false;
        }

        public bool SendingPicMsgToOwnGroup(string targeturl, string message, string Pic, ref FacebookUser fbUser)
        {
           
            try
            {
                int tempCountMain = 0;
            startAgainMain:


                string username = fbUser.username;
                string password = fbUser.password;

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                bool ReturnPicstatus = false;
                int intProxyPort = 80;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string message_text = string.Empty;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }
                string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookWithouthomeUrl));

                string __user = "";
                string fb_dtsg = "";

                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl));


                if (pgSrc_FanPageSearch.Contains("uiIconText _51z7"))
                {

                    __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                    }

                    fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                    }
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                    try
                    {
                        string Dialogposturl = string.Empty;
                        string DialogPostData = string.Empty;
                        string responseresult = string.Empty;
                        try
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=withtaggericon&loaded_components[5]=placetaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (string.IsNullOrEmpty(responseresult))
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        string getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));
                        if (string.IsNullOrEmpty(getresponse))
                        {
                            try
                            {
                                getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));

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


                    string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
                    _rev = _rev.Replace("\":", string.Empty);



                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=1n&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL1 = "https://www.facebook.com/ajax/composerx/attachment/media/chooser/?composerurihash=1";

                        string intermediatePostResponse1 = HttpHelper.postFormData(new Uri(intermediatePostURL1), intermediatePostData1);
                    }

                    #endregion
                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1o&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL2 = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=2";

                        string intermediatePostResponse2 = HttpHelper.postFormData(new Uri(intermediatePostURL2), intermediatePostData2);
                    }

                    #endregion

                    #region Intermediate Post - Waterfall

                    string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1p&__rev=" + _rev + "&ttstamp=265816710110410481103";
                    string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/video/upload/?composerurihash=3";

                    string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion

                    #region Intermediate Post - Waterfall commemntedCode

                    //string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=u_0_u&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";
                    //string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";

                    //string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    //string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion


                    string UploadPostUrl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostUploadPhotosPostUrl;

                    string imagePath = string.Empty;

                    imagePath = Pic;
                    string status = string.Empty;

                    ReturnPicstatus = HttpHelper.AddaPicture(ref HttpHelper, username, password, imagePath, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain);




                    if (!ReturnPicstatus && tempCountMain <= 1)
                    {
                        goto startAgainMain;
                    }

                    if (ReturnPicstatus)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return false;

        }

        public bool SendingPicMsgToOwnGroupAsEdited(string targeturl, string message, string Pic, ref FacebookUser fbUser)
        {
            
            try
            {
                int tempCountMain = 0;
            startAgainMain:


                string username = fbUser.username;
                string password = fbUser.password;

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                bool ReturnPicstatus = false;
                int intProxyPort = 80;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string message_text = string.Empty;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }
                string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookWithouthomeUrl));

                string __user = "";
                string fb_dtsg = "";

                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl));


                if (pgSrc_FanPageSearch.Contains("uiIconText _51z7"))
                {

                    __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                    }

                    fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                    }
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                    try
                    {
                        string Dialogposturl = string.Empty;
                        string DialogPostData = string.Empty;
                        string responseresult = string.Empty;
                        try
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=withtaggericon&loaded_components[5]=placetaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (string.IsNullOrEmpty(responseresult))
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        string getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));
                        if (string.IsNullOrEmpty(getresponse))
                        {
                            try
                            {
                                getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));

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


                    string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
                    _rev = _rev.Replace("\":", string.Empty);



                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=1n&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL1 = "https://www.facebook.com/ajax/composerx/attachment/media/chooser/?composerurihash=1";

                        string intermediatePostResponse1 = HttpHelper.postFormData(new Uri(intermediatePostURL1), intermediatePostData1);
                    }

                    #endregion
                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1o&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL2 = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=2";

                        string intermediatePostResponse2 = HttpHelper.postFormData(new Uri(intermediatePostURL2), intermediatePostData2);
                    }

                    #endregion

                    #region Intermediate Post - Waterfall

                    string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1p&__rev=" + _rev + "&ttstamp=265816710110410481103";
                    string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/video/upload/?composerurihash=3";

                    string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion

                    #region Intermediate Post - Waterfall commemntedCode

                    //string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=u_0_u&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";
                    //string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";

                    //string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    //string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion


                    string UploadPostUrl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostUploadPhotosPostUrl;

                    string imagePath = string.Empty;

                    imagePath = Pic;
                    string status = string.Empty;
                    int delay = 10;
                    try
                    {
                        delay = new Random().Next(minDelayGroupManager, maxDelayGroupManager);
                    }
                    catch { };
                    ReturnPicstatus = HttpHelper.AddaPictureForEditMessage(ref HttpHelper, username, password, imagePath, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain,delay);




                    if (!ReturnPicstatus && tempCountMain <= 1)
                    {
                        goto startAgainMain;
                    }

                    if (ReturnPicstatus)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return false;

        }

        public bool SendingPicMsgToOwnGroup(string targeturl, string message, List<string> lstpic, ref FacebookUser fbUser)
        {

         
            try
            {
                int tempCountMain = 0;
            startAgainMain:

              
                string username = fbUser.username;
                string password = fbUser.password;

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                bool ReturnPicstatus = false;
                int intProxyPort = 80;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string message_text = string.Empty;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }
                string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookWithouthomeUrl));

                string __user = "";
                string fb_dtsg = "";

                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl));

                if (pgSrc_FanPageSearch.Contains("uiIconText _51z7"))
                {

                    __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                    }

                    fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                    }
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                    try
                    {
                        string Dialogposturl = string.Empty;
                        string DialogPostData = string.Empty;
                        string responseresult = string.Empty;
                        try
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=withtaggericon&loaded_components[5]=placetaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (string.IsNullOrEmpty(responseresult))
                        {
                            Dialogposturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
                            responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        string getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));
                        if (string.IsNullOrEmpty(getresponse))
                        {
                            try
                            {
                                getresponse = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"));

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


                    string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
                    _rev = _rev.Replace("\":", string.Empty);



                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=1n&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL1 = "https://www.facebook.com/ajax/composerx/attachment/media/chooser/?composerurihash=1";

                        string intermediatePostResponse1 = HttpHelper.postFormData(new Uri(intermediatePostURL1), intermediatePostData1);
                    }

                    #endregion
                    #region Intermediate Post - Waterfall
                    {
                        string intermediatePostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1o&__rev=" + _rev + "&ttstamp=265816710110410481103";
                        string intermediatePostURL2 = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=2";

                        string intermediatePostResponse2 = HttpHelper.postFormData(new Uri(intermediatePostURL2), intermediatePostData2);
                    }

                    #endregion

                    #region Intermediate Post - Waterfall

                    string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1p&__rev=" + _rev + "&ttstamp=265816710110410481103";
                    string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/video/upload/?composerurihash=3";

                    string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion

                    #region Intermediate Post - Waterfall commentedCode

                    //string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=u_0_u&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";
                    //string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";

                    //string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

                    //string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

                    #endregion


                    string UploadPostUrl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostUploadPhotosPostUrl;

                    string imagePath = string.Empty;


                    //imagePath = Pic;
                    // imagePath = lstpic;

                    string status = string.Empty;

                    ReturnPicstatus = HttpHelper.AddaPicture2(ref HttpHelper, username, password, lstpic, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain);
                    //ReturnPicstatus = HttpHelper.AddaPicture(ref HttpHelper, username, password, lstpic, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain);

                    if (!ReturnPicstatus && tempCountMain <= 1)
                    {
                        goto startAgainMain;
                    }

                    if (ReturnPicstatus)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {

                }
            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return false;
        }

        public bool SendingMsgToGroups(string targeturl, string message, ref FacebookUser fbUser)
        {
            try
            {
               

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string msg = string.Empty;
                msg = message;
                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl));
                List<string> status = HttpHelper.GetTextDataByTagAndAttributeName(pgSrc_FanPageSearch, "span", "uiButtonText");
                try
                {
                    for (int i = 0; i < status.Count; i++)
                    {
                        try
                        {
                            Msgsendingstatus = status[i];
                            if (Msgsendingstatus != string.Empty)
                            {
                                Msgsendingcurrentstatus = Msgsendingstatus;

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

                // if (Msgsendingcurrentstatus.Contains("Notifications"))
                {

                    if (pgSrc_FanPageSearch.Contains("xhpc_composerid") || pgSrc_FanPageSearch.Contains("xhpc_targetid"))
                    {
                        try
                        {
                            string __user = "";
                            string fb_dtsg = "";
                            string xhpc_composerid = string.Empty;
                            string xhpc_targetid = string.Empty;
                            int composer_session_id;
                            string MESSAGE = string.Empty;

                            try
                            {
                                MESSAGE = Uri.EscapeDataString(msg);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                            if (string.IsNullOrEmpty(__user))
                            {
                                __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                            }

                            fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                            if (string.IsNullOrEmpty(fb_dtsg))
                            {
                                fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                            }

                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");

                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");

                            composer_session_id = Convert.ToInt32(Utils.ConvertToUnixTimestamp(DateTime.Now));


                            string composer_sessionid = composer_session_id.ToString();

                            string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
                            _rev = _rev.Replace("\":", string.Empty);


                   
                            string postdata = "fb_dtsg=" + fb_dtsg + "&postfromfull=true&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_fbx=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_message_text=" + MESSAGE + "&xhpc_message=" + MESSAGE + "&is_explicit_place=&composertags_place=&composertags_place_name=&composer_session_id=" + composer_session_id + "&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=1658165781016912151427";

                            //  postdata = "composer_session_id=62c93be7-17d8-4f1b-b2aa-036f326a8734&fb_dtsg=AQCAABsi&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=u_jsonp_5_c&xhpc_targetid=139180732957326&clp=%7B%22cl_impid%22%3A%220a090d8f%22%2C%22clearcounter%22%3A1%2C%22elementid%22%3A%22u_jsonp_5_r%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A139180732957326%7D&xhpc_message_text=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&xhpc_message=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1388385131&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=100007423262870&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=t&__rev=1062230&ttstamp=2658167656566115105";

                            string posturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;

                            string Response = string.Empty;
                            try
                            {
                                Response = HttpHelper.postFormData(new Uri(posturl), postdata, " ");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            if (string.IsNullOrEmpty(Response))
                            {
                                posturl = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;
                                try
                                {
                                    Response = HttpHelper.postFormData(new Uri(posturl), postdata, targeturl);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            int length = Response.Length;
                            string Status = string.Empty;

                            if (Response.Contains(msg) || length > 5000 || Response.Contains("Your post has been submitted and is pending approval by an admin."))
                            {
                                try
                                {
                                    string PostedUrl = string.Empty;
                                    if (Response.Contains("\"permalink") && Response.Contains("commentcount"))
                                    {
                                   
                                        PostedUrl = Utils.getBetween(Response, "\"permalink", "commentcount").Replace("\\", string.Empty).Replace("\":\"", string.Empty).Replace("\",\"",string.Empty);
                                        PostedUrl = "https://www.facebook.com" + PostedUrl;
                                        Status = "Posted";
                                    }
                                    else if (Response.Contains("post pending approval.") || Response.Contains("Your post has been submitted and is pending approval by an admin."))
                                    {
                                        PostedUrl = "post pending approval.";
                                        Status = "post pending approval.";

                                    }
                                    if (Response.Contains("Your post has been submitted and is pending approval by an admin."))
                                    {
                                        GlobusLogHelper.log.Info("Your post has been submitted and is pending approval by an admin.  " + " Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
                                        Status = "submitted and is pending approval by an admin.";
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
                                    }

                                    try
                                    {
                                        string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "PostedUrl" + ","+"Status"+"," + "DateTime";
                                        string CSV_Content = fbUser.username + "," + targeturl + "," + message + "," + PostedUrl + ","+Status +","+DateTime.Now;
                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error  >>>" + ex.StackTrace);
                                    }

                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Message Sending Fail");
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        public bool SendingMsgToGroups1(string targeturl, string message, ref FacebookUser fbUser)
        {
            try
            {
             


                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string msg = string.Empty;
                string grpurlresponse = string.Empty;
                string grpurl = string.Empty;
                msg = message;
                string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/giovanni.donofrio.750/allactivity"));
                List<string> status = HttpHelper.GetHrefFromString(pgSrc_FanPageSearch);
                List<string> ghgs = HttpHelper.GetHrefsFromString(pgSrc_FanPageSearch);
                string[] groupTag = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "_z6m uiStreamStory uiUnifiedStory timelineLogStory");
                //\":471693342959395,\"
                string story_fbid = faceboardpro.FBUtils.getBetween(pgSrc_FanPageSearch, "context_id", "from_uids").Replace("/", "").Replace(",", "").Replace(":", "").Replace("\\", "").Replace("\"", "");




                string[] gsgs = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "href=");
                foreach (string grlitem in gsgs)
                {
                    if (grlitem.Contains("permalink") & !grlitem.Contains("giovanni.donofrio") & !grlitem.Contains("<!DOCTYPE html>"))
                    {
                        try
                        {
                            string hshs = grlitem.Substring(grlitem.IndexOf("/"), grlitem.IndexOf("\">") - grlitem.IndexOf("/")).Replace("\">", "").Trim();
                            grpurl = "https://www.facebook.com" + hshs;
                            string[] sjdsj = System.Text.RegularExpressions.Regex.Split(hshs, "/");
                            story_fbid = sjdsj[4];
                            // break;
                            //grlitem.Substring(grlitem.StartsWith("href=\"/groups/"), grlitem.IndexOf(">") - grlitem.IndexOf("href=\"/groups/")).Replace("\">", "").Trim();
                        }
                        catch { }

                        {

                            // if (pgSrc_FanPageSearch.Contains("xhpc_composerid") || pgSrc_FanPageSearch.Contains("xhpc_targetid"))
                            {
                                try
                                {
                                    string __user = "";
                                    string fb_dtsg = "";
                                    string xhpc_composerid = string.Empty;
                                    string xhpc_targetid = string.Empty;
                                    int composer_session_id;
                                    string MESSAGE = string.Empty;

                                    try
                                    {
                                        MESSAGE = Uri.EscapeDataString(msg);

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                    if (string.IsNullOrEmpty(__user))
                                    {
                                        __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                    }

                                    fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                    if (string.IsNullOrEmpty(fb_dtsg))
                                    {
                                        fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                    }
                                    xhpc_composerid = GlobusHttpHelper.GetParamValue(grpurlresponse, "composerid");
                                    xhpc_targetid = GlobusHttpHelper.GetParamValue(grpurlresponse, "xhpc_targetid");
                                    composer_session_id = Convert.ToInt32(Utils.ConvertToUnixTimestamp(DateTime.Now));
                                    string composer_sessionid = composer_session_id.ToString();

                                    // string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
                                    string _rev = GlobusHttpHelper.get_Between(pgSrc_FanPageSearch, "revision", ",");
                                    _rev = _rev.Replace("\":", string.Empty);

                                    string ProfileiD = GlobusHttpHelper.ProfileID(pgSrc_FanPageSearch, "CurrentUserInitialData", ",");
                                    string story_row_time = GlobusHttpHelper.GetBetween(pgSrc_FanPageSearch, "astseentime", ",");

                                    string story_dom_id = GlobusHttpHelper.Getdomid(pgSrc_FanPageSearch, "story_dom_id", "&entstory_context=");
                                    string visibility_selector_id = GlobusHttpHelper.selector_id(pgSrc_FanPageSearch, "visibility_selector_id", ",");
                                    string timestamp = GlobusHttpHelper.selector_id(pgSrc_FanPageSearch, "timestamp", "&amp;group_id");


                                    string pgSrc_Fa = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/giovanni.donofrio.750/allactivity"));

                                    string ss = "pmid=55&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aOGUGy38y9ACwKyaF299qzAQjFw&__req=6&fb_dtsg=" + fb_dtsg + "&ttstamp=26581657049958373&__rev=" + _rev;

                                    string Response21 = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/bz"), ss, " ");




                                    string postdata1 = "pmid=25&__user=" + __user + "&__a=1&__dyn=7n8apij35zoSt2u6aOGUGy38y9ACwKyaF299qzAQjFw&__req=8&ttstamp=26581657049958373&__rev=" + _rev + "&profile_id=" + __user + "&activity_log=1&story_dom_id=" + "u_0_2x" + "&story_fbid=" + story_fbid + "&story_row_time=1391754675&visibility_selector_id=" + visibility_selector_id + "&action=remove_content&also_remove_app=0&confirmed=true&ban_user=0&fb_dtsg=" + fb_dtsg;

                                    string Response1 = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/timeline/take_action_on_story.php"), postdata1, " ");


                                    if (Response1.Contains("This content has been deleted"))
                                    {
                                        GlobusLogHelper.log.Info("PostDeletedFromThisGrpUrl" + grpurl + " With UserName : " + fbUser.username);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public bool PostVideoUrl(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
        {

          
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                string composer_session_id = "";
                string fb_dtsg = "";
                string xhpc_composerid = "";
                string xhpc_targetid = "";
                string xhpc_context = "";
                string xhpc_fbx = "";
                string xhpc_timeline = "";
                string xhpc_ismeta = "";
                string xhpc_message_text = "";
                string xhpc_message = "";
                string uithumbpager_width = "128";
                string uithumbpager_height = "128";
                string composertags_place = "";
                string composertags_place_name = "";
                string composer_predicted_city = "";
                //string composer_session_id="";
                string is_explicit_place = "";
                string composertags_city = "";
                string disable_location_sharing = "false";
                string audiencevalue = "80";
                string nctr_mod = "pagelet_timeline_recent";
                string UserId = "";
                string __a = "1";
                string phstamp = "";
                {

                    string strFanPageURL = targeturl;
                    string Parent_FbId=string.Empty;
                    try
                    {
                        Parent_FbId = Utils.getBetween(strFanPageURL + "@@", "/groups/", "@@");
                        if (Utils.IsNumeric(Parent_FbId))
                        {

                        }
                        else
                        {
                            Parent_FbId = string.Empty;
                        }
                    }
                    catch { };
                    

                    string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL));
                    {

                        try
                        {
                            if (string.IsNullOrEmpty(Parent_FbId))
                            {
                                string profile_id = Utils.getBetween(strPageSource, "group_id=", "\"");
                                Parent_FbId = profile_id.Replace("value=", string.Empty).Replace(" ", string.Empty).Trim();
                            }
                            
                        }
                        catch { };

                        UserId = GlobusHttpHelper.Get_UserID(strPageSource);

                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

                        xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                        xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
                        xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                        xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                        xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                        xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                        xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

                        xhpc_message_text = VideoUrl;
                        xhpc_message = message + "      " + xhpc_message_text;
                        string xhpc_message_textPostData=string.Empty;
                        try
                        {
                           
                            xhpc_message_text = Uri.EscapeDataString(xhpc_message);
                            xhpc_message_textPostData=xhpc_message_text;
                            if (VideoUrl.Contains("/photos/a"))
                            {
                                xhpc_message_text = Uri.EscapeDataString(xhpc_message).Replace("%2F", "%252F").Replace("%3F", "%253F");
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                        }
                        string strAjaxRequest1 = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxEmiEndPhpUrl + UserId + "&__a=1"));
                        
                        //NewPostData Create by ajay 10/02/2015

                        //string AjaxGetUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="+Uri.EscapeDataString(VideoUrl)+"&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av="+UserId+"&composerurihash=3";


                        //string PageSourceForAjax = HttpHelper.getHtmlfromUrl(new Uri(AjaxGetUrl));
                        //string FInalPostData = "";


                        string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"));
                       
                        if (string.IsNullOrEmpty(composer_session_idSource))
                        {
                            composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"));

                        }
                        if (string.IsNullOrEmpty(composer_session_idSource))
                        {
                            composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=2"));

                        }
                        #region MyRegion
                        if (VideoUrl.Contains("/photos/a"))
                        {
                            try
                            {


                                // new changes

                                //string sssUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=https%253A%252F%252Fwww.facebook.com%252FFM7777%252Fphotos%252Fa.1516984551923369.1073741828.1513882618900229%252F1520025971619227%252F%253Ftype%253D1%2526theater&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av="+UserId+"&composerurihash=3";
                                //string PostDataTp = "fb_dtsg="+fb_dtsg+"&composerid="+composer_session_id+"&targetid="+xhpc_targetid+"&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=ogtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=ogtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=placetagger&loaded_components[11]=explicitplaceinput&loaded_components[12]=hiddenplaceinput&loaded_components[13]=placenameinput&loaded_components[14]=hiddensessionid&loaded_components[15]=ogtagger&loaded_components[16]=withtagger&loaded_components[17]=cameraicon&loaded_components[18]=citysharericon&nctr[_mod]=pagelet_group_composer&__user=100003606625075&__a=1&__dyn=aJioznEyl2lm9adDgDDx2G8F8yhposhEK49oKiRlaFQKKdirYyy8lBxiLGjAKGy99UGmWhF6nUxJdALhVpqCG4FKi8zVEeE&__req=r&ttstamp=265817055106707610469107103120&__rev=1592818";

                                //string Res11 = HttpHelper.postFormData(new Uri(sssUrl), PostDataTp);

                                // ====================

                                string post = "fb_dtsg=" + fb_dtsg + "&composerid=u_jsonp_8_r&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
                                string Url = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
                                string Res = HttpHelper.postFormData(new Uri(Url), post);
                                string getUrl = "https://www.facebook.com/ajax/typeahead/groups/mentions_bootstrap?group_id=" + xhpc_targetid + "&work_user=false&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&sid=739416692916&request_id=5553ac82-96ba-48c1-a691-8307a113aa28&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=17&__rev=1522031&token=1417666641";

                                string Res2 = HttpHelper.getHtmlfromUrl(new Uri(getUrl));

                                //string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=2309869772&attachment[params][0]=1472864976307114&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
                                //string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av="+UserId;
                                //string Resss = HttpHelper.postFormData(new Uri(UrlPost), post22);
                            }
                            catch { };
                        }

                        #endregion

                        if (composer_session_idSource.Contains("composer_session_id"))
                        {
                            composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

                        }
                     
                        string appid = string.Empty;

                        string Responsed = string.Empty;
                        try
                        {
                            string ss = string.Empty;
                            string VUrl = string.Empty;
                            string jhj = string.Empty;
                            string kkk = string.Empty;
                            string PostData = string.Empty;
                            string FirstResponse=string.Empty;
                            appid = Utils.getBetween(strPageSource, "appid=", "&");
                            try
                            {

                                string Urll = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(VideoUrl) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2";
                                string PostData11="fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
                                FirstResponse = HttpHelper.postFormData(new Uri(Urll), PostData11);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                            string attachment_params = Utils.getBetween(FirstResponse, "attachment[params][0]\\\" value=\\\"","\\\"");
                            string attachment_params_urlInfo_canonical = Utils.getBetween(FirstResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_urlInfo_final = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_urlInfo_user = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_favicon = Utils.getBetween(FirstResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_title = Utils.getBetween(FirstResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#x2018;", "").Replace("&#x2013;", "").Replace("&#x2019;", "");
                            string attachment_params_summary = Utils.getBetween(FirstResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

                            // https://fbexternal-a.akamaihd.net/safe_image.php?d=AQC5tPYMGdfqMSZZ&w=470&h=246&url=http%3A%2F%2Fthechangewithin.net%2Fwp-content%2Fuploads%2F2014%2F11%2Fstupidity-virus-667x375.jpg&cfs=1&upscale=1

                            string[] arr = System.Text.RegularExpressions.Regex.Split(FirstResponse, "scaledImageFitWidth");
                            string attachment_params_images0 = string.Empty;
                            if (arr.Count() > 1)
                            {
                                attachment_params_images0 = Utils.getBetween(arr[1], "src=\\\"", "alt").Replace("\\", "").Replace("u00253A", "%3A").Replace("u00252", "%2").Replace("amp;", "").Replace("\"", "");

                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(attachment_params_images0))
                                {
                                    attachment_params_images0 = Utils.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                }
                            }
                            if (string.IsNullOrEmpty(attachment_params_images0))
                            {
                              string[] arr111=  System.Text.RegularExpressions.Regex.Split(FirstResponse,"attachment[params][images][0]");
                            }
                         
                            string attachment_params_medium = Utils.getBetween(FirstResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_url = Utils.getBetween(FirstResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_type = Utils.getBetween(FirstResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_src = Utils.getBetween(FirstResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_width = Utils.getBetween(FirstResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_height = Utils.getBetween(FirstResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_secure_url = Utils.getBetween(FirstResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_type = Utils.getBetween(FirstResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_source = Utils.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_domain = Utils.getBetween(FirstResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_base_domain = Utils.getBetween(FirstResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_title_len = Utils.getBetween(FirstResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_summary_len = Utils.getBetween(FirstResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_min_dimensions0 = Utils.getBetween(FirstResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_min_dimensions1 = Utils.getBetween(FirstResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_with_dimensions = Utils.getBetween(FirstResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_pending = Utils.getBetween(FirstResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_fetched = Utils.getBetween(FirstResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_image_dimensions0 = Utils.getBetween(FirstResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_image_dimensions1 = Utils.getBetween(FirstResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_considered = Utils.getBetween(FirstResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_selected = Utils.getBetween(FirstResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_cap = Utils.getBetween(FirstResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_type = Utils.getBetween(FirstResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            //string FinalResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=fee06a9d-c617-4071-8ed3-e308f966370a&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DAtbJaKNmbJs&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                                
                            try
                            {
                                if (VideoUrl.Contains("http:"))
                                {

                                }
                                else
                                {

                                }
                                if (VideoUrl.Contains("http:"))
                                {
                                    try
                                    {


                                        string[] FirstArr = Regex.Split(VideoUrl, "http:");

                                        VUrl = "http:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35ynzpQ9UmWWuUQxE &__req=c&phstamp=265816676120578769";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
                                    }
                                    catch { };
                                }
                                else
                                {
                                    try
                                    {


                                        string[] FirstArr = Regex.Split(VideoUrl, "https:");

                                        VUrl = "https:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

                                        //--------------------
                                        string PostUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
                                        string PostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
                                        Responsed = HttpHelper.postFormData(new Uri(PostUrl), PostData2);
                                    }
                                    catch { };
                                }

                                if (string.IsNullOrEmpty(Responsed))
                                {
                                    try
                                    {
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";


                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

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
                                if (VideoUrl.Contains("WWW") || VideoUrl.Contains("www"))
                                {
                                    try
                                    {
                                        VideoUrl = "http://" + VideoUrl;
                                        string[] FirstArr = Regex.Split(VideoUrl, "http:");

                                        VUrl = "http:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";
                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
                                    }
                                    catch { };
                                }

                            }
                            appid = Utils.getBetween(Responsed,"app_id\\\" value=\\\"", "\\\"");
                            Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
                            if (Responsed.Contains("name=") && Responsed.Contains("value="))
                            {
                                try
                                {
                                    string[] strNameValue = Regex.Split(Responsed, "name=");
                                    foreach (var strNameValueitem in strNameValue)
                                    {
                                        try
                                        {
                                            if (strNameValueitem.Contains("value="))
                                            {
                                                string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
                                                if (strSplit.Contains("value="))
                                                {
                                                    try
                                                    {
                                                        string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                        string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());


                                                        dicNameValue.Add(strName, strValue);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                        string strValue = "0";

                                                        dicNameValue.Add(strName, strValue);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
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

                            // for changing summery data
                            dicNameValue = ChangeSummeryValueDic(dicNameValue);

                            string partPostData = string.Empty;
                            foreach (var dicNameValueitem in dicNameValue)
                            {
                                try
                                {

                                    if (dicNameValueitem.Key == "attachment[params][title]")
                                    {
                                        try
                                        {
                                            string value = dicNameValueitem.Value;
                                            string HTmlDEcode = HttpUtility.HtmlDecode(value);
                                            string UrlDEcode = HttpUtility.UrlEncode(HTmlDEcode);

                                            partPostData = partPostData + dicNameValueitem.Key + "=" + UrlDEcode + "&";
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
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

                            //change partpost  data


                            string attachment_params1 = string.Empty;
                                string attachment_params0=string.Empty;

                                try
                                {
                                    attachment_params1 = Utils.getBetween(FirstResponse, "attachment[params][1]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\","").Trim();
                                    attachment_params0 = Utils.getBetween(FirstResponse, "attachment[params][0]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();
                                    if (string.IsNullOrEmpty(attachment_params1))
                                    {
                                         attachment_params0 = Utils.getBetween(FirstResponse,"attachment[params][global_share_id]","/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            partPostData = partPostData.Replace(" ", "+");

                            string resp = string.Empty;
                            string FinalPostData = string.Empty;

                            if (ChkbGroupViewSchedulerTaskRemoveUrl == false)
                            {
                                try
                                {
                                    if (VideoUrl.Contains("/photos/a"))
                                    {
                                        string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
                                        string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
                                        resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
                                            string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
                                            string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
                                            if (string.IsNullOrEmpty(FirstResponse))
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + Uri.EscapeDataString(xhpc_message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
                                            }
                                            else
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&xhpc_message_text=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message + "    :    " + VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                                            }
                                       
                                         }
                                    catch { };
                                    }
                                  
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else
                            {
                                string[] Arr = Regex.Split(xhpc_message_text, "http");
                                try
                                {
                                    xhpc_message_text = Arr[0];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }


                                string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;

                                try
                                {
                                    if (VideoUrl.Contains("/photos/a"))
                                    {
                                        try
                                        {

                                            string post22 = "composer_session_id=86f5e57b-0319-4c5d-a581-fb313b2ccb9d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2231d42652%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1j%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + Parent_FbId + "%7D&xhpc_message_text=&xhpc_message=&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418037348&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4XUKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=265816995851139912186556697&__rev=1522031";
                                            string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
                                            resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
                                        }
                                        catch { };
                                    }
                                    else
                                    {
                                        try
                                        {


                                            messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
                                            string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
                                            string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
                                            if (string.IsNullOrEmpty(FirstResponse))
                                            {
                                                try
                                                {
                                                    resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
                                                }
                                                catch { };
                                            }
                                            else
                                            {

                                                try
                                                {
                                               
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                                                }
                                                catch { };
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                try
                                                {

                                              
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                                                }
                                                catch { };
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                try
                                                {

                                               
                                                string SecondPostUrl = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;

                                                string SecondPostData = "composer_session_id=" + composer_session_id + "&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_composerid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22aeeb80e9%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1l%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A1377865092436016%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1417760932&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=g&ttstamp=265817177997910611967866779&__rev=1520211";
                                                string secondRespoce = HttpHelper.postFormData(new Uri(SecondPostUrl), SecondPostData);
                                                }
                                                catch { };
                                            }
                                        }
                                        catch { };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                            }
                            if (string.IsNullOrEmpty(resp))
                            {
                                try
                                {
                                    resp = HttpHelper.postFormData(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxProfileComposerUrl), FinalPostData);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            if (!string.IsNullOrEmpty(resp))
                            {
                                if (!resp.Contains("Error") || resp.Contains("jsmods"))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        public bool PostVideoUrlUpdated(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
        {

          

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                string composer_session_id = "";
                string fb_dtsg = "";
                string xhpc_composerid = "";
                string xhpc_targetid = "";
                string xhpc_context = "";
                string xhpc_fbx = "";
                string xhpc_timeline = "";
                string xhpc_ismeta = "";
                string xhpc_message_text = "";
                string xhpc_message = "";
             
                string composer_predicted_city = "";
             
              
                string UserId = "";
                {
              

                    string strFanPageURL = targeturl;
                    string Parent_FbId = string.Empty;
                    try
                    {
                        Parent_FbId = Utils.getBetween(strFanPageURL + "@@", "/groups/", "@@");
                        if (Utils.IsNumeric(Parent_FbId))
                        {

                        }
                        else
                        {
                            Parent_FbId = string.Empty;
                        }
                    }
                    catch { };


                    string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL));
                    {

                        try
                        {
                            if (string.IsNullOrEmpty(Parent_FbId))
                            {
                                string profile_id = Utils.getBetween(strPageSource, "group_id=", "\"");
                                Parent_FbId = profile_id.Replace("value=", string.Empty).Replace(" ", string.Empty).Trim();
                            }

                        }
                        catch { };

                        UserId = GlobusHttpHelper.Get_UserID(strPageSource);

                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

                        xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                        xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
                        xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                        xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                        xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                        xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                        xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

                        xhpc_message_text = VideoUrl;
                        xhpc_message = message + "      " + xhpc_message_text;
                        string xhpc_message_textPostData = string.Empty;
                        try
                        {

                            xhpc_message_text = Uri.EscapeDataString(xhpc_message);
                            xhpc_message_textPostData = xhpc_message_text;
                            if (VideoUrl.Contains("/photos/a"))
                            {
                                xhpc_message_text = Uri.EscapeDataString(xhpc_message).Replace("%2F", "%252F").Replace("%3F", "%253F");
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                        }
                       
                        #region MyRegion
                        if (VideoUrl.Contains("/photos/a"))
                        {
                            //try
                            //{                               

                            //    string post = "fb_dtsg=" + fb_dtsg + "&composerid=u_jsonp_8_r&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
                            //    string Url = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
                            //    string Res = HttpHelper.postFormData(new Uri(Url), post);
                            //    string getUrl = "https://www.facebook.com/ajax/typeahead/groups/mentions_bootstrap?group_id=" + xhpc_targetid + "&work_user=false&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&sid=739416692916&request_id=5553ac82-96ba-48c1-a691-8307a113aa28&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=17&__rev=1522031&token=1417666641";

                            //    string Res2 = HttpHelper.getHtmlfromUrl(new Uri(getUrl));

                            //}
                            //catch { };
                        }

                        #endregion

                     

                        string appid = string.Empty;

                        string Responsed = string.Empty;
                        try
                        {
                            string ss = string.Empty;
                            string VUrl = string.Empty;
                            string jhj = string.Empty;
                            string kkk = string.Empty;
                            string PostData = string.Empty;
                            string FirstResponse = string.Empty;
                            appid = Utils.getBetween(strPageSource, "appid=", "&");
                            try
                            {

                                string Urll = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(VideoUrl) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2";
                                string PostData11 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
                                FirstResponse = HttpHelper.postFormData(new Uri(Urll), PostData11);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                            string attachment_params = Utils.getBetween(FirstResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
                            string attachment_params_urlInfo_canonical = Utils.getBetween(FirstResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_urlInfo_final = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_urlInfo_user = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_favicon = Utils.getBetween(FirstResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_title = Utils.getBetween(FirstResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#x2018;", "").Replace("&#x2013;", "").Replace("&#x2019;", "");
                            string attachment_params_summary = Utils.getBetween(FirstResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

                           

                            string[] arr = System.Text.RegularExpressions.Regex.Split(FirstResponse, "scaledImageFitWidth");
                            string attachment_params_images0 = string.Empty;
                            if (arr.Count() > 1)
                            {
                                attachment_params_images0 = Utils.getBetween(arr[1], "src=\\\"", "alt").Replace("\\", "").Replace("u00253A", "%3A").Replace("u00252", "%2").Replace("amp;", "").Replace("\"", "");

                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(attachment_params_images0))
                                {
                                    attachment_params_images0 = Utils.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                }
                            }


                            string attachment_params_medium = Utils.getBetween(FirstResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_url = Utils.getBetween(FirstResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_type = Utils.getBetween(FirstResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_src = Utils.getBetween(FirstResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_width = Utils.getBetween(FirstResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_height = Utils.getBetween(FirstResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_params_video0_secure_url = Utils.getBetween(FirstResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string attachment_type = Utils.getBetween(FirstResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_source = Utils.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_domain = Utils.getBetween(FirstResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_base_domain = Utils.getBetween(FirstResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_title_len = Utils.getBetween(FirstResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_summary_len = Utils.getBetween(FirstResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_min_dimensions0 = Utils.getBetween(FirstResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_min_dimensions1 = Utils.getBetween(FirstResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_with_dimensions = Utils.getBetween(FirstResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_pending = Utils.getBetween(FirstResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_fetched = Utils.getBetween(FirstResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_image_dimensions0 = Utils.getBetween(FirstResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_image_dimensions1 = Utils.getBetween(FirstResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_considered = Utils.getBetween(FirstResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_selected = Utils.getBetween(FirstResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_cap = Utils.getBetween(FirstResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            string link_metrics_images_type = Utils.getBetween(FirstResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                            //string FinalResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=fee06a9d-c617-4071-8ed3-e308f966370a&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DAtbJaKNmbJs&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");

                            try
                            {
                                if (VideoUrl.Contains("http:"))
                                {

                                }
                                else
                                {

                                }
                                if (VideoUrl.Contains("http:"))
                                {
                                    try
                                    {


                                        string[] FirstArr = Regex.Split(VideoUrl, "http:");

                                        VUrl = "http:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35ynzpQ9UmWWuUQxE &__req=c&phstamp=265816676120578769";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
                                    }
                                    catch { };
                                }
                                else
                                {
                                    try
                                    {


                                        string[] FirstArr = Regex.Split(VideoUrl, "https:");

                                        VUrl = "https:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

                                        //--------------------
                                        string PostUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
                                        string PostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
                                        Responsed = HttpHelper.postFormData(new Uri(PostUrl), PostData2);
                                    }
                                    catch { };
                                }

                                if (string.IsNullOrEmpty(Responsed))
                                {
                                    try
                                    {
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";


                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

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
                                if (VideoUrl.Contains("WWW") || VideoUrl.Contains("www"))
                                {
                                    try
                                    {
                                        VideoUrl = "http://" + VideoUrl;
                                        string[] FirstArr = Regex.Split(VideoUrl, "http:");

                                        VUrl = "http:" + FirstArr[1];
                                        jhj = Uri.EscapeUriString(VUrl);
                                        kkk = Uri.EscapeDataString(VUrl);
                                        ss = faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";
                                        PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
                                        Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
                                    }
                                    catch { };
                                }

                            }
                            appid = Utils.getBetween(Responsed, "app_id\\\" value=\\\"", "\\\"");
                            Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
                            if (Responsed.Contains("name=") && Responsed.Contains("value="))
                            {
                                try
                                {
                                    string[] strNameValue = Regex.Split(Responsed, "name=");
                                    foreach (var strNameValueitem in strNameValue)
                                    {
                                        try
                                        {
                                            if (strNameValueitem.Contains("value="))
                                            {
                                                string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
                                                if (strSplit.Contains("value="))
                                                {
                                                    try
                                                    {
                                                        string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                        string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());


                                                        dicNameValue.Add(strName, strValue);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                        string strValue = "0";

                                                        dicNameValue.Add(strName, strValue);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
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

                            // for changing summery data
                            dicNameValue = ChangeSummeryValueDic(dicNameValue);

                            string partPostData = string.Empty;
                            foreach (var dicNameValueitem in dicNameValue)
                            {
                                try
                                {

                                    if (dicNameValueitem.Key == "attachment[params][title]")
                                    {
                                        try
                                        {
                                            string value = dicNameValueitem.Value;
                                            string HTmlDEcode = HttpUtility.HtmlDecode(value);
                                            string UrlDEcode = HttpUtility.UrlEncode(HTmlDEcode);

                                            partPostData = partPostData + dicNameValueitem.Key + "=" + UrlDEcode + "&";
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
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

                            //change partpost  data


                            string attachment_params1 = string.Empty;
                            string attachment_params0 = string.Empty;

                            try
                            {
                                attachment_params1 = Utils.getBetween(FirstResponse, "attachment[params][1]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();
                                attachment_params0 = Utils.getBetween(FirstResponse, "attachment[params][0]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            partPostData = partPostData.Replace(" ", "+");

                            string resp = string.Empty;
                            string FinalPostData = string.Empty;

                            if (ChkbGroupViewSchedulerTaskRemoveUrl == false)
                            {
                                try
                                {
                                    if (VideoUrl.Contains("/photos/a"))
                                    {
                                        string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
                                        string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
                                        resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
                                            string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
                                            string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
                                            if (string.IsNullOrEmpty(FirstResponse))
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + Uri.EscapeDataString(xhpc_message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
                                            }
                                            else
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&xhpc_message_text=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message + "    :    " + VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                                            }

                                        }
                                        catch { };
                                    }

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else
                            {
                                string[] Arr = Regex.Split(xhpc_message_text, "http");
                                try
                                {
                                    xhpc_message_text = Arr[0];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }


                                string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;

                                try
                                {
                                    if (VideoUrl.Contains("/photos/a"))
                                    {
                                        try
                                        {

                                          //  string PostDataFinal = "composer_session_id=ce6350f1-f5ac-4554-ba92-9bc0ccf7254b&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227f13514b%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_6_11%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + Parent_FbId + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=2309869772&attachment[params][0]=1513882618900229&attachment[params][1]=1073741843&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1424339363&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=107970219231827&nctr[_mod]=pagelet_group_composer&__user=100001006024349&__a=1&__dyn=aJioznEyl2lm9adDgDDzbHaF8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdHLhVpqCGuaCV8yfCwFx2iicVGw&__req=18&ttstamp=265817190115801009974104114664997115&__rev=1606715";

                                            string post22 = "composer_session_id=86f5e57b-0319-4c5d-a581-fb313b2ccb9d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2231d42652%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1j%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + Parent_FbId + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418037348&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4XUKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=265816995851139912186556697&__rev=1522031";
                                            string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
                                            resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
                                        }
                                        catch { };
                                    }
                                    else
                                    {
                                        try
                                        {


                                            messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
                                            partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
                                            string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
                                            string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
                                            if (string.IsNullOrEmpty(FirstResponse))
                                            {
                                                try
                                                {
                                                    resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
                                                }
                                                catch { };
                                            }
                                            else
                                            {

                                                try
                                                {

                                                    resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                                                }
                                                catch { };
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                try
                                                {


                                                    resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                                                }
                                                catch { };
                                            }
                                            if (resp.Contains("The message could not be posted to this Wall."))
                                            {
                                                try
                                                {
                                                    string SecondPostUrl = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
                                                    string SecondPostData = "composer_session_id=" + composer_session_id + "&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_composerid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22aeeb80e9%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1l%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A1377865092436016%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1417760932&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=g&ttstamp=265817177997910611967866779&__rev=1520211";
                                                    string secondRespoce = HttpHelper.postFormData(new Uri(SecondPostUrl), SecondPostData);
                                                }
                                                catch { };
                                            }
                                        }
                                        catch { };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                            }
                            if (string.IsNullOrEmpty(resp))
                            {
                                try
                                {
                                    resp = HttpHelper.postFormData(new Uri(faceboardpro.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxProfileComposerUrl), FinalPostData);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            if (!string.IsNullOrEmpty(resp))
                            {
                                if (!resp.Contains("Error") || resp.Contains("jsmods"))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        public bool PostVideoUrlNew(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
         {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string messageId = string.Empty;
            try
            {
                string UserId = string.Empty;
                string fb_dtsg = string.Empty;
                string xhpc_composerid = "";
                string xhpc_targetid = "";
                string xhpc_context = "";
                string xhpc_fbx = "";
                string xhpc_timeline = "";
                string xhpc_ismeta = "";
                string xhpc_message_text = "";
                string xhpc_message = "";
              
                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(targeturl));

                xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
                xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");
                
                UserId = GlobusHttpHelper.Get_UserID(strPageSource);
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                string scrapperUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="+Uri.EscapeDataString(Uri.EscapeDataString(VideoUrl))+"&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av="+UserId+"&composerurihash=2";

                string scrapPostData = "fb_dtsg="+fb_dtsg+"&composerid="+xhpc_composerid+"&targetid="+xhpc_targetid+"&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=mainprivacywidget&loaded_components[6]=ogtaggericon&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=placetagger&loaded_components[11]=explicitplaceinput&loaded_components[12]=hiddenplaceinput&loaded_components[13]=placenameinput&loaded_components[14]=hiddensessionid&loaded_components[15]=ogtagger&loaded_components[16]=withtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user="+UserId+"&__a=1&__dyn=7nmanEyl2lm9udDgDxyIGzGpUW9ACxO4pbGAdBGeqrWo8popyUW5ogDyQqUkBBzEy6Kdy8-&__req=e&ttstamp=265816974739950120779711755&__rev=1559767";

                string scrapResp = HttpHelper.postFormData(new Uri(scrapperUrl), scrapPostData);

                string FinalPostData = Utils.getBetween(scrapResp, "attachment[params][urlInfo][canonical]", "=\\\"attachment[type]\\\" value=\\\"100\\\" \\/>");

                string AppID = string.Empty;
                AppID = Utils.getBetween(scrapResp, "app_id\\\" value=\\\"", "\\\"");
                //If url is of video url
                if (!string.IsNullOrEmpty(FinalPostData))
                {
                    string faviconVal =string.Empty;
                    string summary = string.Empty;
                    string Title = string.Empty;
                    string rankedImage = string.Empty;
                    string thumbNailImg = string.Empty;
                    string share_id = string.Empty;
                    string metaTagMap = string.Empty;
                    string metaTagMap2 = string.Empty;
                    string metaTagMap8 = string.Empty;
                    string metaTagMap11 = string.Empty;
                    string metaTagMap22 = string.Empty;
                    string metaTagMap23 = string.Empty;
                    string metaTagMap24 = string.Empty;
                    string metaTagMap25 = string.Empty;
                    string metaTagMap26 = string.Empty;
                    string metaTagMap27 = string.Empty;
                    string metaTagMap28 = string.Empty;
                    string metaTagMap29 = string.Empty;
                    string metaTagMap30 = string.Empty;
                    string metaTagMap31 = string.Empty;
                    string metaTagMap32 = string.Empty;
                    string metaTagMap33 = string.Empty;
                   
                    string ipaddress=string.Empty;
                    faviconVal= Utils.getBetween(scrapResp, "name=\\\"attachment[params][favicon]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    Title = Utils.getBetween(scrapResp, "attachment[params][title]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    rankedImage = Utils.getBetween(scrapResp, "[ranked_images][images][0]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    thumbNailImg = Utils.getBetween(scrapResp, "attachment[params][images][0]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    summary = Utils.getBetween(scrapResp, "attachment[params][summary]\\\" value=\\\"", "\" \\/>").Replace("\\",string.Empty);
                    share_id = Utils.getBetween(scrapResp, "share_id=", "\\\"").Replace("\\",string.Empty);
                    metaTagMap=Utils.getBetween(scrapResp,"[metaTagMap][1][content]\\\" value=\\\"","\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap2 = Utils.getBetween(scrapResp, "[metaTagMap][2][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap8 = Utils.getBetween(scrapResp, "[metaTagMap][8][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap11 = Utils.getBetween(scrapResp, "[metaTagMap][11][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap22 = Utils.getBetween(scrapResp, "[metaTagMap][22][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    metaTagMap23 = Utils.getBetween(scrapResp, "[metaTagMap][23][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    metaTagMap24 = Utils.getBetween(scrapResp, "[metaTagMap][24][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    metaTagMap25 = Utils.getBetween(scrapResp, "[metaTagMap][25][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap26 = Utils.getBetween(scrapResp, "[metaTagMap][26][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap27 = Utils.getBetween(scrapResp, "[metaTagMap][27][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap28 = Utils.getBetween(scrapResp, "[metaTagMap][28][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap29 = Utils.getBetween(scrapResp, "[metaTagMap][29][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap30 = Utils.getBetween(scrapResp, "[metaTagMap][30][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\",string.Empty);
                    metaTagMap31 = Utils.getBetween(scrapResp, "[metaTagMap][31][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    metaTagMap32 = Utils.getBetween(scrapResp, "[metaTagMap][32][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    metaTagMap33 = Utils.getBetween(scrapResp, "[metaTagMap][33][content]\\\" value=\\\"", "\\\" \\/>").Replace("\\", string.Empty);
                    ipaddress=Utils.getBetween(scrapResp,"attachment[params][domain_ip]\\\" value=\\\"","\\\" \\/>");


                    string veryFinalPostData = "composer_session_id=b257b049-667b-4e67-a2c4-71ad68ffe884&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22d3e4e3f4%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + AppID + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][log][1407766252]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][responseCode]=200&attachment[params][favicon]=" + Uri.EscapeDataString(faviconVal) + "&attachment[params][external_author_id]=26weekplan&attachment[params][title]=" + Uri.EscapeDataString(Title) + "&attachment[params][summary]=" + Uri.EscapeDataString(summary) + "&attachment[params][content_removed]=&attachment[params][images][0]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][ranked_images][images][0]=" + Uri.EscapeDataString(rankedImage) + " &attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][image_info][0][width]=1920&attachment[params][image_info][0][height]=1080&attachment[params][image_info][0][face_centers][0][x]=43.611&attachment[params][image_info][0][face_centers][0][y]=26.667&attachment[params][image_info][0][xray][overlaid_text]=0.0696&attachment[params][image_info][0][xray][synthetic]=0.0145&attachment[params][video_info][duration]=5179&attachment[params][video_info][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][video_info][error_code]=403&attachment[params][video_info][error_message]=Daily%20Limit%20Exceeded&attachment[params][medium]=103&attachment[params][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][time_scraped]=1421150812&attachment[params][cache_hit]=1&attachment[params][global_share_id]=" + share_id + "&attachment[params][was_recent]=&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][name]=title&attachment[params][metaTagMap][1][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][2][name]=description&attachment[params][metaTagMap][2][content]=http%3A%2F%2Fwww.26weekplan.com%2F%20The%204%20Phases%20of%20Digital%20Marketing%20is%20the%20framework%20behind%20the%2026-Week%20Digital%20Marketing%20Plan%20-%20a%20step-by-step%20online%20marketing%20pla...&attachment[params][metaTagMap][3][name]=keywords&attachment[params][metaTagMap][3][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][4][property]=og%3Asite_name&attachment[params][metaTagMap][4][content]=YouTube&attachment[params][metaTagMap][5][property]=og%3Aurl&attachment[params][metaTagMap][5][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][6][property]=og%3Atitle&attachment[params][metaTagMap][6][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][7][property]=og%3Aimage&attachment[params][metaTagMap][7][content]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][metaTagMap][8][property]=og%3Adescription&attachment[params][metaTagMap][8][content]=" + Uri.EscapeDataString(metaTagMap8) + "&attachment[params][metaTagMap][9][property]=al%3Aios%3Aapp_store_id&attachment[params][metaTagMap][9][content]=544007664&attachment[params][metaTagMap][10][property]=al%3Aios%3Aapp_name&attachment[params][metaTagMap][10][content]=YouTube&attachment[params][metaTagMap][11][property]=al%3Aios%3Aurl&attachment[params][metaTagMap][11][content]=" + Uri.EscapeDataString(metaTagMap11) + "&attachment[params][metaTagMap][12][property]=al%3Aandroid%3Aurl&attachment[params][metaTagMap][12][content]=" + Uri.EscapeDataString(VideoUrl) + "%26feature%3Dapplinks&attachment[params][metaTagMap][13][property]=al%3Aandroid%3Aapp_name&attachment[params][metaTagMap][13][content]=YouTube&attachment[params][metaTagMap][14][property]=al%3Aandroid%3Apackage&attachment[params][metaTagMap][14][content]=com.google.android.youtube&attachment[params][metaTagMap][15][property]=al%3Aweb%3Aurl&attachment[params][metaTagMap][15][content]=" + Uri.EscapeDataString(VideoUrl) + "%26feature%3Dapplinks&attachment[params][metaTagMap][16][property]=og%3Atype&attachment[params][metaTagMap][16][content]=video&attachment[params][metaTagMap][17][property]=og%3Avideo&attachment[params][metaTagMap][17][content]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1&attachment[params][metaTagMap][18][property]=og%3Avideo%3Asecure_url&attachment[params][metaTagMap][18][content]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1&attachment[params][metaTagMap][19][property]=og%3Avideo%3Atype&attachment[params][metaTagMap][19][content]=application%2Fx-shockwave-flash&attachment[params][metaTagMap][20][property]=og%3Avideo%3Awidth&attachment[params][metaTagMap][20][content]=1280&attachment[params][metaTagMap][21][property]=og%3Avideo%3Aheight&attachment[params][metaTagMap][21][content]=720&attachment[params][metaTagMap][22][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][22][content]=" + Uri.EscapeDataString(metaTagMap22) + "&attachment[params][metaTagMap][23][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][23][content]=" + Uri.EscapeDataString(metaTagMap23) + "&attachment[params][metaTagMap][24][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][24][content]=" + Uri.EscapeDataString(metaTagMap24) + "&attachment[params][metaTagMap][25][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][25][content]=" + Uri.EscapeDataString(metaTagMap25) + "&attachment[params][metaTagMap][26][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][26][content]=" + Uri.EscapeDataString(metaTagMap26) + "&attachment[params][metaTagMap][27][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][27][content]=" + Uri.EscapeDataString(metaTagMap27) + "&attachment[params][metaTagMap][28][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][28][content]=seminar&attachment[params][metaTagMap][29][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][29][content]=" + Uri.EscapeDataString(metaTagMap29) + "&attachment[params][metaTagMap][30][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][30][content]=" + Uri.EscapeDataString(metaTagMap30) + "&attachment[params][metaTagMap][31][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][31][content]=" + Uri.EscapeDataString(metaTagMap31) + "&attachment[params][metaTagMap][32][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][32][content]=" + Uri.EscapeDataString(metaTagMap32) + "&attachment[params][metaTagMap][33][property]=fb%3Aapp_id&attachment[params][metaTagMap][33][content]=" + Uri.EscapeDataString(metaTagMap33) + "&attachment[params][metaTagMap][34][name]=twitter%3Acard&attachment[params][metaTagMap][34][content]=player&attachment[params][metaTagMap][35][name]=twitter%3Asite&attachment[params][metaTagMap][35][content]=%40youtube&attachment[params][metaTagMap][36][name]=twitter%3Aurl&attachment[params][metaTagMap][36][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][37][name]=twitter%3Atitle&attachment[params][metaTagMap][37][content]=" + Uri.EscapeDataString(metaTagMap2) + "&attachment[params][metaTagMap][38][name]=twitter%3Adescription&attachment[params][metaTagMap][38][content]=" + Uri.EscapeDataString(metaTagMap8) + "&attachment[params][metaTagMap][39][name]=twitter%3Aimage&attachment[params][metaTagMap][39][content]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][metaTagMap][40][name]=twitter%3Aapp%3Aname%3Aiphone&attachment[params][metaTagMap][40][content]=YouTube&attachment[params][metaTagMap][41][name]=twitter%3Aapp%3Aid%3Aiphone&attachment[params][metaTagMap][41][content]=544007664&attachment[params][metaTagMap][42][name]=twitter%3Aapp%3Aname%3Aipad&attachment[params][metaTagMap][42][content]=YouTube&attachment[params][metaTagMap][43][name]=twitter%3Aapp%3Aid%3Aipad&attachment[params][metaTagMap][43][content]=544007664&attachment[params][metaTagMap][44][name]=twitter%3Aapp%3Aurl%3Aiphone&attachment[params][metaTagMap][44][content]=vnd.youtube%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Dj7ChQkZ5mSA%26feature%3Dapplinks&attachment[params][metaTagMap][45][name]=twitter%3Aapp%3Aurl%3Aipad&attachment[params][metaTagMap][45][content]=" + Uri.EscapeDataString(metaTagMap11) + "&attachment[params][metaTagMap][46][name]=twitter%3Aapp%3Aname%3Agoogleplay&attachment[params][metaTagMap][46][content]=YouTube&attachment[params][metaTagMap][47][name]=twitter%3Aapp%3Aid%3Agoogleplay&attachment[params][metaTagMap][47][content]=com.google.android.youtube&attachment[params][metaTagMap][48][name]=twitter%3Aapp%3Aurl%3Agoogleplay&attachment[params][metaTagMap][48][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][49][name]=twitter%3Aplayer&attachment[params][metaTagMap][49][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][50][name]=twitter%3Aplayer%3Awidth&attachment[params][metaTagMap][50][content]=1280&attachment[params][metaTagMap][51][name]=twitter%3Aplayer%3Aheight&attachment[params][metaTagMap][51][content]=720&attachment[params][og_info][properties][0][0]=fb%3Aapp_id&attachment[params][og_info][properties][0][1]=87741124305&attachment[params][redirectPath][0][status]=og%3Aurl&attachment[params][redirectPath][0][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][redirectPath][0][ip]=" + ipaddress + "&attachment[params][video][0][type]=application%2Fx-shockwave-flash&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1%26autoplay%3D1&attachment[params][video][0][width]=1280&attachment[params][video][0][height]=720&attachment[params][video][0][src]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1%26autoplay%3D1&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1421151032&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyIGzGpUW9ACxO4pbGAdBGeqrWo8popyUW5ogDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=2658172668275106571151028949&__rev=1559767";
                    veryFinalPostData = "composer_session_id=b257b049-667b-4e67-a2c4-71ad68ffe884&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22d3e4e3f4%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text="+message+"&xhpc_message="+message+"&aktion=post&app_id=" + AppID + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][urlInfo][log][1407766252]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][responseCode]=200&attachment[params][favicon]=" + Uri.EscapeDataString(faviconVal) + "&attachment[params][external_author_id]=26weekplan&attachment[params][title]=" + Uri.EscapeDataString(Title) + "&attachment[params][summary]=" + Uri.EscapeDataString(summary) + "&attachment[params][content_removed]=&attachment[params][images][0]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][ranked_images][images][0]=" + Uri.EscapeDataString(rankedImage) + " &attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][image_info][0][width]=1920&attachment[params][image_info][0][height]=1080&attachment[params][image_info][0][face_centers][0][x]=43.611&attachment[params][image_info][0][face_centers][0][y]=26.667&attachment[params][image_info][0][xray][overlaid_text]=0.0696&attachment[params][image_info][0][xray][synthetic]=0.0145&attachment[params][video_info][duration]=5179&attachment[params][video_info][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][video_info][error_code]=403&attachment[params][video_info][error_message]=Daily%20Limit%20Exceeded&attachment[params][medium]=103&attachment[params][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][time_scraped]=1421150812&attachment[params][cache_hit]=1&attachment[params][global_share_id]=" + share_id + "&attachment[params][was_recent]=&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][name]=title&attachment[params][metaTagMap][1][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][2][name]=description&attachment[params][metaTagMap][2][content]=http%3A%2F%2Fwww.26weekplan.com%2F%20The%204%20Phases%20of%20Digital%20Marketing%20is%20the%20framework%20behind%20the%2026-Week%20Digital%20Marketing%20Plan%20-%20a%20step-by-step%20online%20marketing%20pla...&attachment[params][metaTagMap][3][name]=keywords&attachment[params][metaTagMap][3][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][4][property]=og%3Asite_name&attachment[params][metaTagMap][4][content]=YouTube&attachment[params][metaTagMap][5][property]=og%3Aurl&attachment[params][metaTagMap][5][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][6][property]=og%3Atitle&attachment[params][metaTagMap][6][content]=" + Uri.EscapeDataString(metaTagMap) + "&attachment[params][metaTagMap][7][property]=og%3Aimage&attachment[params][metaTagMap][7][content]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][metaTagMap][8][property]=og%3Adescription&attachment[params][metaTagMap][8][content]=" + Uri.EscapeDataString(metaTagMap8) + "&attachment[params][metaTagMap][9][property]=al%3Aios%3Aapp_store_id&attachment[params][metaTagMap][9][content]=544007664&attachment[params][metaTagMap][10][property]=al%3Aios%3Aapp_name&attachment[params][metaTagMap][10][content]=YouTube&attachment[params][metaTagMap][11][property]=al%3Aios%3Aurl&attachment[params][metaTagMap][11][content]=" + Uri.EscapeDataString(metaTagMap11) + "&attachment[params][metaTagMap][12][property]=al%3Aandroid%3Aurl&attachment[params][metaTagMap][12][content]=" + Uri.EscapeDataString(VideoUrl) + "%26feature%3Dapplinks&attachment[params][metaTagMap][13][property]=al%3Aandroid%3Aapp_name&attachment[params][metaTagMap][13][content]=YouTube&attachment[params][metaTagMap][14][property]=al%3Aandroid%3Apackage&attachment[params][metaTagMap][14][content]=com.google.android.youtube&attachment[params][metaTagMap][15][property]=al%3Aweb%3Aurl&attachment[params][metaTagMap][15][content]=" + Uri.EscapeDataString(VideoUrl) + "%26feature%3Dapplinks&attachment[params][metaTagMap][16][property]=og%3Atype&attachment[params][metaTagMap][16][content]=video&attachment[params][metaTagMap][17][property]=og%3Avideo&attachment[params][metaTagMap][17][content]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1&attachment[params][metaTagMap][18][property]=og%3Avideo%3Asecure_url&attachment[params][metaTagMap][18][content]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1&attachment[params][metaTagMap][19][property]=og%3Avideo%3Atype&attachment[params][metaTagMap][19][content]=application%2Fx-shockwave-flash&attachment[params][metaTagMap][20][property]=og%3Avideo%3Awidth&attachment[params][metaTagMap][20][content]=1280&attachment[params][metaTagMap][21][property]=og%3Avideo%3Aheight&attachment[params][metaTagMap][21][content]=720&attachment[params][metaTagMap][22][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][22][content]=" + Uri.EscapeDataString(metaTagMap22) + "&attachment[params][metaTagMap][23][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][23][content]=" + Uri.EscapeDataString(metaTagMap23) + "&attachment[params][metaTagMap][24][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][24][content]=" + Uri.EscapeDataString(metaTagMap24) + "&attachment[params][metaTagMap][25][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][25][content]=" + Uri.EscapeDataString(metaTagMap25) + "&attachment[params][metaTagMap][26][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][26][content]=" + Uri.EscapeDataString(metaTagMap26) + "&attachment[params][metaTagMap][27][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][27][content]=" + Uri.EscapeDataString(metaTagMap27) + "&attachment[params][metaTagMap][28][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][28][content]=seminar&attachment[params][metaTagMap][29][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][29][content]=" + Uri.EscapeDataString(metaTagMap29) + "&attachment[params][metaTagMap][30][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][30][content]=" + Uri.EscapeDataString(metaTagMap30) + "&attachment[params][metaTagMap][31][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][31][content]=" + Uri.EscapeDataString(metaTagMap31) + "&attachment[params][metaTagMap][32][property]=og%3Avideo%3Atag&attachment[params][metaTagMap][32][content]=" + Uri.EscapeDataString(metaTagMap32) + "&attachment[params][metaTagMap][33][property]=fb%3Aapp_id&attachment[params][metaTagMap][33][content]=" + Uri.EscapeDataString(metaTagMap33) + "&attachment[params][metaTagMap][34][name]=twitter%3Acard&attachment[params][metaTagMap][34][content]=player&attachment[params][metaTagMap][35][name]=twitter%3Asite&attachment[params][metaTagMap][35][content]=%40youtube&attachment[params][metaTagMap][36][name]=twitter%3Aurl&attachment[params][metaTagMap][36][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][37][name]=twitter%3Atitle&attachment[params][metaTagMap][37][content]=" + Uri.EscapeDataString(metaTagMap2) + "&attachment[params][metaTagMap][38][name]=twitter%3Adescription&attachment[params][metaTagMap][38][content]=" + Uri.EscapeDataString(metaTagMap8) + "&attachment[params][metaTagMap][39][name]=twitter%3Aimage&attachment[params][metaTagMap][39][content]=" + Uri.EscapeDataString(thumbNailImg) + "&attachment[params][metaTagMap][40][name]=twitter%3Aapp%3Aname%3Aiphone&attachment[params][metaTagMap][40][content]=YouTube&attachment[params][metaTagMap][41][name]=twitter%3Aapp%3Aid%3Aiphone&attachment[params][metaTagMap][41][content]=544007664&attachment[params][metaTagMap][42][name]=twitter%3Aapp%3Aname%3Aipad&attachment[params][metaTagMap][42][content]=YouTube&attachment[params][metaTagMap][43][name]=twitter%3Aapp%3Aid%3Aipad&attachment[params][metaTagMap][43][content]=544007664&attachment[params][metaTagMap][44][name]=twitter%3Aapp%3Aurl%3Aiphone&attachment[params][metaTagMap][44][content]=vnd.youtube%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Dj7ChQkZ5mSA%26feature%3Dapplinks&attachment[params][metaTagMap][45][name]=twitter%3Aapp%3Aurl%3Aipad&attachment[params][metaTagMap][45][content]=" + Uri.EscapeDataString(metaTagMap11) + "&attachment[params][metaTagMap][46][name]=twitter%3Aapp%3Aname%3Agoogleplay&attachment[params][metaTagMap][46][content]=YouTube&attachment[params][metaTagMap][47][name]=twitter%3Aapp%3Aid%3Agoogleplay&attachment[params][metaTagMap][47][content]=com.google.android.youtube&attachment[params][metaTagMap][48][name]=twitter%3Aapp%3Aurl%3Agoogleplay&attachment[params][metaTagMap][48][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][49][name]=twitter%3Aplayer&attachment[params][metaTagMap][49][content]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][metaTagMap][50][name]=twitter%3Aplayer%3Awidth&attachment[params][metaTagMap][50][content]=1280&attachment[params][metaTagMap][51][name]=twitter%3Aplayer%3Aheight&attachment[params][metaTagMap][51][content]=720&attachment[params][og_info][properties][0][0]=fb%3Aapp_id&attachment[params][og_info][properties][0][1]=87741124305&attachment[params][redirectPath][0][status]=og%3Aurl&attachment[params][redirectPath][0][url]=" + Uri.EscapeDataString(VideoUrl) + "&attachment[params][redirectPath][0][ip]=" + ipaddress + "&attachment[params][video][0][type]=application%2Fx-shockwave-flash&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1%26autoplay%3D1&attachment[params][video][0][width]=1280&attachment[params][video][0][height]=720&attachment[params][video][0][src]=" + Uri.EscapeDataString(VideoUrl) + "%3Fversion%3D3%26autohide%3D1%26autoplay%3D1&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1421151032&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyIGzGpUW9ACxO4pbGAdBGeqrWo8popyUW5ogDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=2658172668275106571151028949&__rev=1559767";
                    try
                    {
                        string finalResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UserId + ""),veryFinalPostData);
                        messageId = Utils.getBetween(finalResp, "message_id=", "&story_dom_id");
                    }
                    catch (Exception ex)
                    { };

                   
                }

                if (string.IsNullOrEmpty(messageId))
                {
                    string attachmentparams0=Utils.getBetween(scrapResp,"attachment[params][0]\\\" value=\\\"","\\\"" );

                    string PostDataForSimpleUrl = "composer_session_id=a8ddc097-fcf3-4654-87f7-d153ddc360c8&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22a9d7507f%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1r%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A1527082324241636%7D&xhpc_message_text=" + Uri.EscapeDataString(VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + AppID + "&attachment[params][0]="+attachmentparams0+"&attachment[type]=6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1422860846&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=114759761873412&nctr[_mod]=pagelet_group_composer&__user="+UserId+"&__a=1&__dyn=7nmanEyl2lm9udDgDxyG8EihUtCxO4pbGAdBGfirWo8popyUW5ogDyQqUkBBzEy78S8zU46&__req=f&ttstamp=2658172538657771057510767100&__rev=1583304";
                    string finalResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UserId + ""), PostDataForSimpleUrl);
                    messageId = Utils.getBetween(finalResp, "message_id=", "&story_dom_id");
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
          
            if (!string.IsNullOrEmpty(messageId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PostVideoUrlasImage(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
        {

           

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                string composer_session_id = "";
                string fb_dtsg = "";
                string xhpc_composerid = "";
                string xhpc_targetid = "";
                string xhpc_context = "";
                string xhpc_fbx = "";
                string xhpc_timeline = "";
                string xhpc_ismeta = "";
                string xhpc_message_text = "";
                string xhpc_message = "";
                string uithumbpager_width = "128";
                string uithumbpager_height = "128";
                string composertags_place = "";
                string composertags_place_name = "";
                string composer_predicted_city = "";
                //string composer_session_id="";
                string is_explicit_place = "";
                string composertags_city = "";
                string disable_location_sharing = "false";
                string audiencevalue = "80";
                string nctr_mod = "pagelet_timeline_recent";
                string UserId = "";
                string __a = "1";
                string phstamp = "";


                string strFanPageURL = targeturl;

                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL));

                UserId = GlobusHttpHelper.Get_UserID(strPageSource);

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

                xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
                xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");
                string composer_session_idSource = string.Empty;
                xhpc_message_text = VideoUrl;
               // xhpc_message = message + "      " + xhpc_message_text;
                string xhpc_message_textPostData = string.Empty;
                try
                {

                    xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);
                   
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                }
                //string strAjaxRequest1 = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxEmiEndPhpUrl + UserId + "&__a=1"));
                string imageUrl = string.Empty;
                string imagePath = string.Empty;
                try
                {
                    string PostDataToScrap = "fb_dtsg=" + fb_dtsg + "&composerid=u_jsonp_2_o&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=placetagger&loaded_components[11]=explicitplaceinput&loaded_components[12]=hiddenplaceinput&loaded_components[13]=placenameinput&loaded_components[14]=hiddensessionid&loaded_components[15]=ogtagger&loaded_components[16]=withtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5pokHWAVbGEyiuaBKAqhB-8rjpbQummFGx3CV8yfCw&__req=14&ttstamp=2658169541015279566510810872&__rev=1556013";
                    composer_session_idSource = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=2"),PostDataToScrap);
                    string[] imgData = Regex.Split(composer_session_idSource, "url");
                    foreach (string imgItem in imgData)
                    {
                        if (imgItem.Contains(".jpg"))
                        {
                            imageUrl = Utils.getBetween(imgItem, "=", "&amp").Replace("\\u00253A", ":").Replace("\\u00252F","/");
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        WebClient webclient = new WebClient();
                        byte[] args = webclient.DownloadData(imageUrl);
                        string imageName=imageUrl.Split('/')[imageUrl.Split('/').Length-1];
                        File.WriteAllBytes("C:\\Facedominator\\Data\\" + "/" + imageName, args);
                        imagePath="C:\\Facedominator\\Data\\" + "/" + imageName;
                    }
                }
                catch(Exception ex)
                {
                  GlobusLogHelper.log.Error(ex.Message);
                }
                
                string waterfallID = string.Empty;
                string upload_id = "1024";
                string grid_id = string.Empty;
                string album_type = string.Empty;
                string clp = "{\"cl_impid\":\"2b17d4c5\",\"clearcounter\":0,\"elementid\":\""+xhpc_composerid+"\",\"version\":\"x\",\"parent_fbid\":"+xhpc_targetid+"}";
                string tagger_session_id = string.Empty;
                if (!string.IsNullOrEmpty(composer_session_idSource))
                {
                    string composerurihash = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/media/upload/?av=" + UserId + "&composerurihash=3"), "fb_dtsg="+fb_dtsg+"&composerid=u_jsonp_5_o&targetid="+xhpc_targetid+"&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&nctr[_mod]=pagelet_group_composer&__user="+UserId+"&__a=1&__dyn=aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5pokHWAVbGEyiuaBKAqhB-8rjpbQummFGx3CV8yfCw&__req=11&ttstamp=265817265548750815410410071&__rev=1556013");
                    waterfallID = Utils.getBetween(composerurihash, "waterfallID\":\"", "\"");
                    grid_id = Utils.getBetween(composerurihash, "grid_id\":\"", "\"");
                    album_type = Utils.getBetween(composerurihash, "=\\\"album_type\\\" value=\\\"", "\\\"");
                    tagger_session_id = Utils.getBetween(composerurihash, "session_id\":", "}");

                    NameValueCollection nvc = new NameValueCollection();
                    nvc.Add("fb_dtsg", fb_dtsg);
                    nvc.Add("source", "8");
                    nvc.Add("profile_id", UserId);
                    nvc.Add("grid_id", grid_id);
                    nvc.Add("qn", waterfallID);
                    nvc.Add("0", "" + imagePath + "<:><:><:>image/jpeg");
                    nvc.Add("upload_id","1024");
                    string imgUploadResp = HttpHelper.UploadImageWaterfallModel("https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + xhpc_targetid + "&image_height=100&image_width=100&letterbox=0&av=" + UserId + "&qn=" + waterfallID + "&__user=" + UserId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5pokHWAVbGEyiuaBKAqhB-8rjpbQummFGx3CV8yfCw&__req=19&fb_dtsg=" + fb_dtsg + "&ttstamp=265817265548750815410410071&__rev=1556013", targeturl, nvc, "upload_id", "0");
                    string fbid = string.Empty;
                    fbid = Utils.getBetween(imgUploadResp, "fbid\":\"", "\"");       
                    NameValueCollection nvc1 = new NameValueCollection();
                    nvc1.Add("composer_session_id", composer_session_id);
                    nvc1.Add("fb_dtsg", fb_dtsg);
                    nvc1.Add("xhpc_context", xhpc_context);
                    nvc1.Add("grid_id", grid_id);
                    nvc1.Add("xhpc_ismeta", xhpc_ismeta);
                    nvc1.Add("xhpc_timeline", string.Empty);
                    nvc1.Add("xhpc_composerid",xhpc_composerid);
                    nvc1.Add("xhpc_targetid", xhpc_targetid);
                    nvc1.Add("xhpc_publish_type", "1");
                    nvc1.Add("clp", clp);
                    nvc1.Add("xhpc_message_text",Uri.UnescapeDataString(xhpc_message_text));
                    nvc1.Add("xhpc_message", Uri.UnescapeDataString(xhpc_message_text));
                    nvc1.Add("composer_unpublished_photo[]",fbid);
                    nvc1.Add("album_type",album_type);
                    nvc1.Add("is_file_form", "1");
                    nvc1.Add("oid", string.Empty);
                    nvc1.Add("qn", waterfallID);
                    nvc1.Add("application", "composer");
                    nvc1.Add("is_explicit_place",string.Empty);
                    nvc1.Add("composertags_place", string.Empty);
                    nvc1.Add("composertags_place_name", string.Empty);
                    nvc1.Add("tagger_session_id", tagger_session_id);
                    nvc1.Add("action_type_id[]", string.Empty);
                    nvc1.Add("object_str[]", string.Empty);
                    nvc1.Add("object_id[]", string.Empty);
                    nvc1.Add("hide_object_attachment","0");
                    nvc1.Add("og_suggestion_mechanism",string.Empty);
                    nvc1.Add("og_suggestion_logging_data", string.Empty);
                    nvc1.Add("icon_id", string.Empty);
                    nvc1.Add("composertags_city", string.Empty);
                    nvc1.Add("disable_location_sharing", "false");
                    nvc1.Add("composer_predicted_city", string.Empty);
                    string imgUploadResp1 = HttpHelper.UploadImageWaterfallModel("https://upload.facebook.com/media/upload/photos/composer/?av=" + UserId + "&__user=" + UserId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5pokHWAVbGEyiuaBKAqhB-8rjpbQummFGx3CV8yfCw&__req=1l&fb_dtsg=" + fb_dtsg + "&ttstamp=265817265548750815410410071&__rev=1556013", targeturl, nvc1, "composer_predicted_city",string.Empty);
                                                                                
 
                }
                else
                {
                    GlobusLogHelper.log.Error("Sorry Unbale to Fetch Thumbanail of Video Url");
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
           
            return false;
        }

        public Dictionary<string, string> ChangeSummeryValueDic(Dictionary<string, string> dic)
        {
            Dictionary<string, string> _Dic = new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> tempDic = new Dictionary<string, string>();
                tempDic = dic;
                
                foreach (var item in tempDic)
                {
                    try
                    {
                        if (item.Key == "attachment[params][summary]")
                        {
                            if (item.Value.Contains("&"))
                            {
                                _Dic.Add(item.Key, item.Value.Replace("&", ""));

                            }
                        }
                        else
                        {
                            _Dic.Add(item.Key, item.Value);
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

            return _Dic;
        }

        #region Globaol veriables of GroupRequstManager
        readonly object lockrThreadControllerGroupRequstManager = new object();
        public bool isStopGroupGroupRequstManager = false;
        int countThreadControllerGroupGroupRequstManager = 0;
        public List<Thread> lstThreadsGroupGroupRequstManager = new List<Thread>();
        public bool chkCountinueProcessGroupRequstManager = true;
        public bool GroupRequestSendUsingGroupUrlFile = false;
        // public static string GroupExprotFilePath = string.Empty;

        public static int minDelayGroupRequestManager = 10;
        public static int maxDelayGroupRequestManager = 20;

        public static int GroupRequestManagerNoOfGroupRequest = 10;

      


        #endregion

        public static int CheckGroupRequestManagerNoOfGroupsInBatch
        {
            get;
            set;
        }

        public static int CheckGroupRequestManager_InterbalInMinuts
        {
            get;
            set;
        }

        public List<string> LstGroupUrlsGroupRequestManager
        {
            get;
            set;
        }

        public int NoOfThreadsGroupRequestManager
        {
            get;
            set;
        }

        public static string ExportLocationGroupRequest
        {
            get;
            set;
        }

        public void StartGroupRequesManager()
        {
            try
            {
                lstThreadsGroupGroupRequstManager.Add(Thread.CurrentThread);
                lstThreadsGroupGroupRequstManager.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            countThreadControllerGroupGroupRequstManager = 0;

            if (chkCountinueProcessGroupCamapinScheduler == true)
            {
                try
                {
                    if (LstGroupUrlsGroupRequestManager.Count > 0)
                    {
                        foreach (string item in LstGroupUrlsGroupRequestManager)
                        {
                            try
                            {
                                Queue_Messages.Enqueue(item);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }


            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsGroupRequestManager > 0)
                {
                    numberOfAccountPatch = NoOfThreadsGroupRequestManager;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerGroupRequstManager)
                                {
                                    try
                                    {
                                        if (countThreadControllerGroupGroupRequstManager >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupRequstManager);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            try
                                            {

                                                Thread profilerThread = new Thread(StartGroupRequesManagerMultithread);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;

                                                profilerThread.Start(new object[] { item });

                                                countThreadControllerGroupGroupRequstManager++;
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
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }
      
        public void StartGroupRequesManagerMultithread(object parameters)
        {
            GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
            try
            {
                if (!isStopGroupGroupRequstManager)
                {
                    try
                    {
                        lstThreadsGroupGroupRequstManager.Add(Thread.CurrentThread);
                        lstThreadsGroupGroupRequstManager.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)parameters;
                        FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);
                        objFacebookUser.globusHttpHelper = objGlobusHttpHelper;
                        if (!objFacebookUser.isloggedin)
                        {

                            Accounts.AccountManager objAccountManager = new AccountManager();
                            objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                        }

                        if (chkCountinueProcessGroupCamapinScheduler == true && objFacebookUser.isloggedin)
                        {
                            GroupRequestSendersForBrowseGroupunique(ref objGlobusHttpHelper, ref objFacebookUser);

                            GlobusLogHelper.log.Info("Process Completed  " + objFacebookUser.username);
                            GlobusLogHelper.log.Debug("Process Completed  " + objFacebookUser.username);
                        }
                        else
                        {
                            //GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                            //GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                        }

                        if (chkCountinueProcessGroupCamapinScheduler != true)
                        {
                            if (objFacebookUser.isloggedin)
                            {
                                GroupRequestSendersForBrowseGroup(ref objGlobusHttpHelper, ref objFacebookUser);

                                GlobusLogHelper.log.Info("Process Completed  " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Process Completed  " + objFacebookUser.username);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                            }
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

            finally
            {
                try
                {
                    //   if (!isStopGroupInviter)
                    {
                        lock (lockrThreadControllerGroupInviter)
                        {
                            countThreadControllerGroupInviter--;
                            Monitor.Pulse(lockrThreadControllerGroupInviter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void GroupRequestSendersForBrowseGroup(ref GlobusHttpHelper objGlobusHttp, ref FacebookUser fbUser)
        {

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string CheckStatus = string.Empty;
            try
            {
                string UNqgrpurl = string.Empty;
                List<string> lsturll = new List<string>();
                List<string> lsturllKeyword = new List<string>();
                int intProxyPort = 80;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }
                string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                string UserIdGrpMsg = string.Empty;


                UserIdGrpMsg = GlobusHttpHelper.Get_UserID(PageSrcHome);

                int CountRequest = 0;
                int TimeCounter = 0;
                foreach (var Groupurl in LstGroupUrlsGroupRequestManager)
                {
                    if (chkCountinueProcessGroupCamapinScheduler)
                    {
                        DataSet DS = DataBaseHandler.SelectQuery("Select * from GroupRequestUnique where CampaignName='" + GroupRequestCampaignName + "' and URL='" + Groupurl + "' and Account='" + fbUser.username + "'", "GroupRequestUnique");
                        if (DS.Tables[0].Rows.Count > 0)
                        {
                            continue;
                        }
                    }
                    if (CountRequest >= GroupRequestManagerNoOfGroupRequest)
                    {
                        break;
                    }
                    CountRequest = CountRequest + 1;
                    try
                    {
                        if (TimeCounter >= CheckGroupRequestManagerNoOfGroupsInBatch)
                        {

                            GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupRequestManager_InterbalInMinuts + "  : Minutes");
                            GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupRequestManager_InterbalInMinuts + "  : Minutes");

                            TimeCounter = 0;

                            Thread.Sleep(1 * 1000 * 60 * CheckGroupRequestManager_InterbalInMinuts);
                            GlobusLogHelper.log.Debug("Process Continue ..");
                            GlobusLogHelper.log.Info("Process Continue ..");
                        }

                        TimeCounter = TimeCounter + 1;

                        string grpurl = Groupurl;
                        string strGroupUrl = grpurl;
                        string __user = "";
                        string fb_dtsg = "";
                        string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(strGroupUrl));
                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                        if (string.IsNullOrEmpty(__user))
                        {
                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                        if (string.IsNullOrEmpty(fb_dtsg))
                        {
                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                        }
                        try
                        {
                            string currentstatus1 = string.Empty;
                            string aaaa = string.Empty;
                            string groupType = string.Empty;
                            string Userstatus = string.Empty;
                            string currentstatus = string.Empty;
                            string stradminlink = string.Empty;
                            string findstatus = string.Empty;
                            string findstatus1 = string.Empty;

                            string postdataforjoin = string.Empty;
                            string localstr = string.Empty;
                            string Responseofjoin = string.Empty;
                            findstatus = HttpHelper.getHtmlfromUrl(new Uri(grpurl));
                            try
                            {
                                if (grpurl.Contains("http"))
                                {
                                    try
                                    {
                                        string[] grpurlArr = Regex.Split(grpurl, "https://");
                                        string urlforFindingGroupType = grpurlArr[1] + "/members";
                                        string memberurl = urlforFindingGroupType.Replace("//", "/");
                                        memberurl = "https://" + memberurl;
                                        findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl));
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string urlforFindingGroupType = grpurl + "/members";
                                        string memberurl = urlforFindingGroupType.Replace("//", "/");
                                        memberurl = memberurl.Replace("//", "/");
                                        findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl));
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
                            try
                            {
                                int Counter = 0;
                                string[] grpurlArr1 = Regex.Split(grpurl, "/");
                                foreach (var grpurlArr_item in grpurlArr1)
                                {
                                    Counter++;
                                }
                                CheckStatus = grpurlArr1[Counter - 1];
                                if (string.IsNullOrEmpty(CheckStatus))
                                {
                                    CheckStatus = grpurlArr1[Counter - 2];
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            //  List<string> status3 = objGlobusHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "uiButtonText");
                            if (findstatus.Contains("clearfix groupsJumpBarTop") && !findstatus.Contains("Join this group to see the discussion"))
                            {

                                if ((findstatus.Contains("rel=\"async-post\">Join Group</a></li><li>")|| findstatus.Contains("role=\"button\">Join Group</a>")) && !findstatus.Contains(">Pending</span>"))
                                {
                                    currentstatus = "Join Group";
                                }
                                else if (!findstatus.Contains("rel=\"async-post\">Join Group</a></li><li>") && findstatus.Contains(">Pending</span>") &&! findstatus.Contains ("Join this group to see the discussion"))
                                {
                                    GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);

                                    try
                                    {
                                        string CSVHeader = "FbUser" + "," + "GroupURL";
                                        string CSV_Content = fbUser.username + "," + grpurl;

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupRequestManager * 1000, maxDelayGroupRequestManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    continue;
                                }

                                string[] status12 = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "_3-8_ img sp_10pEAcQb1gf sx_431dd2");
                                if (status12.Count() == 1 && !pgSrc_FanPageSearch.Contains("Join this group to see the discussion") && !pgSrc_FanPageSearch.Contains("_42ft _4jy0 _4jy4 _4jy2 selected _51sy"))
                                {
                                    try
                                    {
                                        if (status12[0].Contains("status:pending-") && !status12[0].Contains(">Join group</a></li>") && !status12[0].Contains("Join Group</a></li>"))
                                        {
                                            GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            DataBaseHandler.InsertQuery("Insert into GroupRequestUnique(CampaignName,URL,Account,Status) Values('"+GroupRequestCampaignName+"','" + Groupurl + "','" + fbUser.username + "','Sent')", "GroupRequestUnique");
                                            try
                                            {
                                                string CSVHeader = "FbUser" + "," + "GroupURL";
                                                string CSV_Content = fbUser.username + "," + grpurl;

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayGroupRequestManager * 1000, maxDelayGroupRequestManager * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            continue;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }
                                List<string> status = new List<string>();
                                string[] status1 = null;

                                try
                                {
                                    status1 = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "clearfix groupsJumpBarTop");
                                    status = HttpHelper.GetDataTag(status1[1], "a");
                                }
                                catch { };
                                try
                                {
                                    foreach (var status_item in status)
                                    {
                                        if (status_item == "Join Group" || status_item == "Join group")
                                        {
                                            currentstatus = status_item;
                                            break;
                                        }
                                        if (status_item == "Cancel Request" || status_item == "Cancel Request")
                                        {
                                            currentstatus = status_item;
                                            break;
                                        }
                                    }
                                }
                                catch { };

                                if (string.IsNullOrEmpty(currentstatus))
                                {
                                    if (status[0] == "Join Group" || status[0] == "Join group")
                                    {
                                        currentstatus = status[0];
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (status[2] == "Join Group" || status[2] == "Join group")
                                            {
                                                currentstatus = status[2];
                                            }
                                            else
                                            {
                                                try
                                                {

                                                    if (status[3] == "Join Group" || status[3] == "Join group")
                                                    {
                                                        currentstatus = status[3];
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            if (status[0] == "Cancel Request" || status[0] == "Cancel Request")
                                                            {
                                                                try
                                                                {
                                                                    currentstatus = status[0];
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            currentstatus = Utils.getBetween(status1[1], "async-post\">", "</a>");
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
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                }
                                /// currentstatus = status[2];
                                for (int i = 0; i < status.Count; i++)
                                {
                                    if (currentstatus != string.Empty)
                                    {
                                        currentstatus1 = currentstatus;
                                    }
                                }
                            }


                            try
                            {
                                string crtstatus = "";
                                if (string.IsNullOrEmpty(currentstatus))
                                {
                                    if (findstatus1.Contains("Join Group"))
                                    {
                                        currentstatus = "Join Group";
                                    }
                                }


                                if (currentstatus.Contains("Join Group") || currentstatus1.Contains("Join group") || findstatus.Contains("Join Group") || findstatus.Contains("Join group"))
                                {
                                    try
                                    {
                                        List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
                                        try
                                        {
                                            groupType = GroupType[0];
                                        }
                                        catch { };

                                        if (grpurl.Contains(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl))                                                      //"https://www.facebook.com/groups/"
                                        {
                                            try
                                            {
                                                aaaa = grpurl.Replace(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl, string.Empty).Replace("/", string.Empty);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }
                                        else
                                        {
                                            try
                                            {
                                                aaaa = grpurl.Replace(FBGlobals.Instance.fbhomeurl, string.Empty).Replace("/", string.Empty);                                            //"https://www.facebook.com/"
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        try
                                        {
                                            stradminlink = findstatus.Substring(findstatus.IndexOf("group_id="), (findstatus.IndexOf("\"", findstatus.IndexOf("group_id=")) - findstatus.IndexOf("group_id="))).Replace("group_id=", string.Empty).Replace("\"", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        aaaa = stradminlink;
                                        // string postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=16581657884875010686";

                                        try
                                        {
                                            postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=9&fb_dtsg=" + fb_dtsg + "&__rev=1055839&ttstamp=265816690497512267";
                                            localstr = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl;                                                                         // "https://www.facebook.com/groups/"
                                            Responseofjoin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin, ref localstr);  //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        if (Responseofjoin.Contains("jghdj"))
                                        {
                                            try
                                            {
                                                postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=9&fb_dtsg=" + fb_dtsg + "&__rev=1055839&ttstamp=265816690497512267";
                                                localstr = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl;                                                                         // "https://www.facebook.com/groups/"
                                                Responseofjoin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin, ref localstr);  //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }


                                        crtstatus = "Request Sent";
                                        if (string.IsNullOrEmpty(Responseofjoin))
                                        {
                                            string reff = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + aaaa;                                                                     //  "https://www.facebook.com/groups/"
                                            string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin, ref reff);    //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"

                                        }
                                        if (Responseofjoin.Contains("[\"goURI(") || Responseofjoin.Contains(aaaa) || Responseofjoin.Contains("redirectPageTo") || Responseofjoin.Contains(CheckStatus))
                                        {

                                            GlobusLogHelper.log.Info("Request Sent to URL :" + grpurl + " with :" + fbUser.username);
                                            GlobusLogHelper.log.Debug("Request Sent to URL :" + grpurl + " with :" + fbUser.username);
                                            //string[] param={"Insert","",fbUser.username,Groupurl,"Sent"};
                                            //RaiseEventsgroupRequestUnique(param);
                                            DataBaseHandler.InsertQuery("Insert into GroupRequestUnique(CampaignName,URL,Account,Status) Values('"+GroupRequestCampaignName+"','" + Groupurl + "','" + fbUser.username + "','Sent')", "GroupRequestUnique");

                                            // objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl,Username);           
                                            try
                                            {
                                                string CSVHeader = "FbUser" + "," + "GroupURL";
                                                string CSV_Content = fbUser.username + "," + grpurl;

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
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

                                if (currentstatus1.Contains("Notifications"))
                                {
                                    try
                                    {
                                        List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus1, "span", "fsm fcg");
                                        groupType = GroupType[0];
                                        crtstatus = "Joined";

                                        // objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
                                        //if (!skipalrea dySent)
                                        {
                                            GlobusLogHelper.log.Debug("Request Already Accepted On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Info("Request Already Accepted On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            DataBaseHandler.InsertQuery("Insert into GroupRequestUnique(CampaignName,URL,Account,Status) Values('" + GroupRequestCampaignName + "','" + Groupurl + "','" + fbUser.username + "','Sent')", "GroupRequestUnique");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }
                                if (currentstatus1.Contains("Cancel Request"))
                                {
                                    try
                                    {
                                        List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
                                        groupType = GroupType[0];
                                        crtstatus = "Request Already Sent";
                                        // objclsgrpmngr.InsertGroupmanager(Username, grpurl, groupType, crtstatus);
                                        // objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
                                        // if (!skipalreadySent)
                                        {
                                            GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            DataBaseHandler.InsertQuery("Insert into GroupRequestUnique(CampaignName,URL,Account,Status) Values('" + GroupRequestCampaignName + "','" + Groupurl + "','" + fbUser.username + "','Sent')", "GroupRequestUnique");
                                            try
                                            {
                                                string CSVHeader = "FbUser" + "," + "GroupURL";
                                                string CSV_Content = fbUser.username + "," + grpurl;

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
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
                                if (currentstatus1.Contains("Create Group"))
                                {
                                    GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupRequestManager * 1000, maxDelayGroupRequestManager * 1000);
                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                        Thread.Sleep(delayInSeconds);
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

        public void GroupRequestSendersForBrowseGroupunique(ref GlobusHttpHelper objGlobusHttpHelper, ref FacebookUser fbUser)
        {
            //chkCountinueProcessGroupCamapinScheduler


            

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string CheckStatus = string.Empty;

            int CountRequest=0;
            try
            {
                string UNqgrpurl = string.Empty;
                List<string> lsturll = new List<string>();
                List<string> lsturllKeyword = new List<string>();
                int intProxyPort = 80;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }
                string PageSrcHome = objGlobusHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                string UserIdGrpMsg = string.Empty;

                UserIdGrpMsg = GlobusHttpHelper.Get_UserID(PageSrcHome);

                #region CodeCommented
                //foreach (var Groupurl in LstGroupUrlsGroupRequestManager)
                //try
                //{
                //    if (LstGroupUrlsGroupRequestManager.Count > 0)
                //    {
                //        foreach (string item in LstGroupUrlsGroupRequestManager)
                //        {
                //            try
                //            {
                //                Queue_Messages.Enqueue(item);
                //            }
                //            catch (Exception ex)
                //            {
                //                GlobusLogHelper.log.Error(ex.StackTrace);
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error(ex.StackTrace);
                //} 
                #endregion

                while (Queue_Messages.Count > 0)
                {

                    if (GroupRequestManagerNoOfGroupRequest <= CountRequest)
                    {
                        break;
                    }
                    CountRequest = CountRequest + 1;

                    if (Queue_Messages.Count > 0)
                    {
                        UNqgrpurl = Queue_Messages.Dequeue();
                    }
                    else
                    {
                        return;
                    }

                    try
                    {
                        

                        if (TimeCounter >= CheckGroupRequestManagerNoOfGroupsInBatch)
                        {

                            GlobusLogHelper.log.Debug("Pausing the process for : " + CheckGroupRequestManager_InterbalInMinuts + "  : Minutes");
                            GlobusLogHelper.log.Info("Pausing the process for : " + CheckGroupRequestManager_InterbalInMinuts + "  : Minutes");

                            TimeCounter = 0;

                            Thread.Sleep(1 * 1000 * 60 * CheckGroupRequestManager_InterbalInMinuts);
                            GlobusLogHelper.log.Debug("Process Continue ..");
                            GlobusLogHelper.log.Info("Process Continue ..");
                        }


                        TimeCounter = TimeCounter + 1;

                        string grpurl = UNqgrpurl;
                        string strGroupUrl = grpurl;
                        string __user = "";
                        string fb_dtsg = "";
                        string pgSrc_FanPageSearch = objGlobusHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl));
                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                        if (string.IsNullOrEmpty(__user))
                        {
                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                        if (string.IsNullOrEmpty(fb_dtsg))
                        {
                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                        }
                        try
                        {
                            string currentstatus1 = string.Empty;
                            string aaaa = string.Empty;
                            string groupType = string.Empty;
                            string Userstatus = string.Empty;
                            string currentstatus = string.Empty;
                            string stradminlink = string.Empty;
                            string findstatus = string.Empty;
                            string findstatus1 = string.Empty;

                            findstatus = HttpHelper.getHtmlfromUrl(new Uri(grpurl));
                            try
                            {
                                if (grpurl.Contains("http"))
                                {
                                    try
                                    {
                                        string[] grpurlArr = Regex.Split(grpurl, "https://");
                                        string urlforFindingGroupType = grpurlArr[1] + "/members";
                                        string memberurl = urlforFindingGroupType.Replace("//", "/");
                                        memberurl = "https://" + memberurl;
                                        findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl));
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string urlforFindingGroupType = grpurl + "/members";
                                        string memberurl = urlforFindingGroupType.Replace("//", "/");
                                        memberurl = memberurl.Replace("//", "/");
                                        findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl));
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
                            try
                            {
                                int Counter = 0;
                                string[] grpurlArr1 = Regex.Split(grpurl, "/");
                                foreach (var grpurlArr_item in grpurlArr1)
                                {
                                    Counter++;
                                }
                                CheckStatus = grpurlArr1[Counter - 1];
                                if (string.IsNullOrEmpty(CheckStatus))
                                {
                                    CheckStatus = grpurlArr1[Counter - 2];
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            try
                            {
                                string crtstatus = "";
                                if (findstatus.Contains("clearfix groupsJumpBarTop") && !findstatus.Contains("Join this group to see the discussion") ||findstatus.Contains("_42ft _4jy0 _4jy4 _4jy2 selected _51sy"))
                                {
                                    try
                                    {
                                        List<string> GroupType = objGlobusHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
                                        try
                                        {
                                            groupType = GroupType[0];
                                        }
                                        catch (Exception ex)
                                        {
                                           // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                      

                                        if (grpurl.Contains(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl))                                                      //"https://www.facebook.com/groups/"
                                        {
                                            aaaa = grpurl.Replace(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl, string.Empty).Replace("/", string.Empty);      //"https://www.facebook.com/groups/"
                                        }
                                        else
                                        {
                                            aaaa = grpurl.Replace(FBGlobals.Instance.fbhomeurl, string.Empty).Replace("/", string.Empty);                                            //"https://www.facebook.com/"
                                        }
                                        try
                                        {
                                            stradminlink = findstatus.Substring(findstatus.IndexOf("group_id="), (findstatus.IndexOf("\"", findstatus.IndexOf("group_id=")) - findstatus.IndexOf("group_id="))).Replace("group_id=", string.Empty).Replace("\"", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        aaaa = stradminlink;
                                        // string postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=16581657884875010686";

                                        string postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=9&fb_dtsg=" + fb_dtsg + "&__rev=1055839&ttstamp=265816690497512267";
                                        string localstr = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl;                                                                         // "https://www.facebook.com/groups/"
                                        string Responseofjoin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin, ref localstr);  //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"

                                        crtstatus = "Request Sent";
                                        if (string.IsNullOrEmpty(Responseofjoin))
                                        {
                                            string reff = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + aaaa;                                                                     //  "https://www.facebook.com/groups/"
                                            string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin, ref reff);    //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"

                                        }
                                        if (Responseofjoin.Contains("[\"goURI(") || Responseofjoin.Contains(aaaa) || Responseofjoin.Contains("redirectPageTo") || Responseofjoin.Contains(CheckStatus))
                                        {

                                            GlobusLogHelper.log.Info("Request Sent to URL :" + grpurl + " with :" + fbUser.username);
                                            GlobusLogHelper.log.Debug("Request Sent to URL :" + grpurl + " with :" + fbUser.username);
                                            //   objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl,Username);      
                                            try
                                            {
                                                string CSVHeader = "FbUser" + "," + "GroupURL";
                                                string CSV_Content = fbUser.username + "," + grpurl;

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
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

                                if (currentstatus1.Contains("Notifications"))
                                {
                                    try
                                    {
                                        List<string> GroupType = objGlobusHttpHelper.GetTextDataByTagAndAttributeName(findstatus1, "span", "fsm fcg");
                                        groupType = GroupType[0];
                                        crtstatus = "Joined";

                                        // objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
                                        //if (!skipalreadySent)
                                        {
                                            GlobusLogHelper.log.Debug("Request Already Accepted On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Info("Request Already Accepted On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }
                                if (currentstatus1.Contains("Cancel Request"))
                                {
                                    try
                                    {
                                        List<string> GroupType = objGlobusHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
                                        groupType = GroupType[0];
                                        crtstatus = "Request Already Sent";
                                        // objclsgrpmngr.InsertGroupmanager(Username, grpurl, groupType, crtstatus);
                                        // objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
                                        // if (!skipalreadySent)
                                        {
                                            GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                            
                                            try
                                            {
                                                string CSVHeader = "FbUser" + "," + "GroupURL";
                                                string CSV_Content = fbUser.username + "," + grpurl;

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
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
                                if (currentstatus1.Contains("Create Group"))
                                {
                                    GlobusLogHelper.log.Info("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
                                    }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        int delayInSeconds = Utils.GenerateRandom(minDelayGroupRequestManager * 1000, maxDelayGroupRequestManager * 1000);
                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                        Thread.Sleep(delayInSeconds);
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

        /// <summary>
        /// Star tPostDelete Comments ON Group
        /// </summary>

        public List<string> LstGroup_DeleteCommentPostTargetedPostUrls
        {
            get;
            set;
        }

        public void StartPostDeleteCommentsProcessContinue(ref FacebookUser fbUser)
        {
            try
            {
                lstThreadsDeleteGroupPost.Add(Thread.CurrentThread);
                lstThreadsDeleteGroupPost.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            int countNoOfProcessDeleted = 0;
            try
            {
                foreach (var LstGroup_DeleteCommentPostTargetedPostUrls_item in LstGroup_DeleteCommentPostTargetedPostUrls)
                {
                    try
                    {

                        // PostCommentsOnTargetedUrls


                        bool status = PostCommentsOnTargetedUrls(ref fbUser, LstGroup_DeleteCommentPostTargetedPostUrls_item);


                        if (status)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(GroupDeletePostCommentFilePath))
                                {

                                    string CSVHeader = "Post Url" + "," + "User";
                                    string CSV_Content = LstGroup_DeleteCommentPostTargetedPostUrls_item + "," + fbUser.username;
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupDeletePostCommentFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
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
                    try
                    {
                        countNoOfProcessDeleted++;
                        if (DeleteScheduleGroupPosting)
                        {
                            if (countNoOfProcessDeleted == DeleteNUmberOfPost)
                            {
                                GlobusLogHelper.log.Info("Process Paused For " + IntervalTime + " Minute With User" + fbUser.username);
                                Thread.Sleep(60 * 1000 * IntervalTime);
                                countNoOfProcessDeleted = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }

                }

             
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        public void StartPostDeleteCommentsProcess(ref FacebookUser fbUser)
        {
            try
            {
                lstThreadsDeleteGroupPost.Add(Thread.CurrentThread);
                lstThreadsDeleteGroupPost.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            int countNoOfProcessDeleted = 0;
            try
            {
                foreach (var LstGroup_DeleteCommentPostTargetedPostUrls_item in LstGroup_DeleteCommentPostTargetedPostUrls)
                {
                    try
                    {

                        // PostCommentsOnTargetedUrls


                        bool status = PostCommentsOnTargetedUrls(ref fbUser, LstGroup_DeleteCommentPostTargetedPostUrls_item);

                        
                        if (status)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(GroupDeletePostCommentFilePath))
                                {

                                    string CSVHeader = "Post Url" + "," + "User";
                                    string CSV_Content = LstGroup_DeleteCommentPostTargetedPostUrls_item + "," + fbUser.username;
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupDeletePostCommentFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                                
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayDeleteGroupPost * 1000, maxDelayDeleteGroupPost * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
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
                    try
                    {
                        countNoOfProcessDeleted++;
                        if (DeleteScheduleGroupPosting)
                        {
                            if (countNoOfProcessDeleted == DeleteNUmberOfPost)
                            {
                                GlobusLogHelper.log.Info("Process Paused For " + IntervalTime + " Minute With User" + fbUser.username);
                                Thread.Sleep(60 * 1000 * IntervalTime);
                                countNoOfProcessDeleted = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }

                }

                GlobusLogHelper.log.Info("Process Completed with : " + fbUser.username);
                GlobusLogHelper.log.Debug("Process Completed with : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        public bool PostCommentsOnTargetedUrls(ref FacebookUser fbUser, string TargetedUrl)
        {
            bool status = false;
            GlobusHttpHelper ObjHttpHelper = fbUser.globusHttpHelper;
            string PageSource = string.Empty;
            string __user = string.Empty;
            string fb_dtsg = string.Empty;
            string rootid = string.Empty;
            string ft_ent_identifier = string.Empty;
            string group_id = string.Empty;
            string comment_id = string.Empty;
            string comment_legacyid = string.Empty;
            string client_id = string.Empty;
            string ftId = string.Empty;


            try
            {
                if (!string.IsNullOrEmpty(fbUser.proxyip) && !string.IsNullOrEmpty(fbUser.proxyport))
                {
                    PageSource = ObjHttpHelper.getHtmlfromUrlProxy(new Uri(TargetedUrl), fbUser.proxyip, Convert.ToInt32(fbUser.proxyport), fbUser.proxyusername, fbUser.proxypassword);
                }
                else
                {
                    PageSource = ObjHttpHelper.getHtmlfromUrl(new Uri(TargetedUrl));
                }

                __user = GlobusHttpHelper.GetParamValue(PageSource, "user");
                if (string.IsNullOrEmpty(__user))
                {
                    __user = GlobusHttpHelper.ParseJson(PageSource, "user");
                }

                fb_dtsg = GlobusHttpHelper.GetParamValue(PageSource, "fb_dtsg");
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(PageSource, "fb_dtsg");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                string root = Utils.getBetween(PageSource, "uiUfi UFIContainer _5pc9", "</div>");
                rootid = Utils.getBetween(root, "id=\"", "\">");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                try
                {
                    ft_ent_identifier = Utils.getBetween(PageSource, "ftentidentifier", "\",\"").Replace("\":\"", "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ft_ent_identifier))
                {
                    string[] Arr = System.Text.RegularExpressions.Regex.Split(TargetedUrl, "/permalink/");
                    try
                    {
                        ft_ent_identifier = Arr[1];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (ft_ent_identifier.Contains("?stream"))
                    {
                        try
                        {
                            string[] Arr1 = System.Text.RegularExpressions.Regex.Split(ft_ent_identifier, "/");
                            ft_ent_identifier = Arr1[0];
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }
                    if (string.IsNullOrEmpty(ft_ent_identifier))
                    {
                        ft_ent_identifier = Utils.getBetween(TargetedUrl, "set=gm", "&").Replace(".", "");

                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            string ft_ID=string.Empty;
            try
            {
                group_id = Utils.getBetween(TargetedUrl, "/groups/", "/");

               ft_ID= Utils.getBetween(TargetedUrl+"#****", "permalink/", "#****").Replace("/",string.Empty);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            string PostResponce = string.Empty;
            //PostData Add Comment
            try
            {
                string PostData = "ft_ent_identifier=" + ft_ent_identifier + "&comment_text=.&source=2&client_id=1394434725267%3A2170302505&reply_fbid&parent_comment_id&rootid=" + rootid + "&clp=%7B%22cl_impid%22%3A%22db2bcca2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22js_3%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + ft_ID + "%7D&attached_sticker_fbid=0&attached_photo_fbid=0&giftoccasion&ft[tn]=[]&ft[fbfeed_location]=3&nctr[_mod]=pagelet_group_mall&__user=" + __user + "&__a=1&__dyn=7n8a8gAMCBynzpQ9UoHaHyG8qeyp9Esx6iWF3qGEVF4WpU&__req=r&fb_dtsg=" + fb_dtsg + "&ttstamp=265816711673778372&__rev=1152671";
                string PostUrl = FBGlobals.Instance.GroupPostAndDeleteCommentUrl;
               // PostResponce = ObjHttpHelper.postFormDataProxy(new Uri(PostUrl), PostData,fbUser.proxyip,Convert.ToInt32(fbUser.proxyport),fbUser.proxyusername,fbUser.proxypassword);
                PostResponce = ObjHttpHelper.postFormData(new Uri(PostUrl), PostData);

                if (!PostResponce.Contains("browserArchitecture") || PostResponce.Contains("errorSummary"))
                {
                    try
                    {
                        string PostDataUpdated = "ft_ent_identifier=" + ft_ent_identifier + "&comment_text=.&source=0&client_id=1424687545000%3A259370096&reply_fbid&parent_comment_id&rootid=" + rootid + "&clp=&attached_sticker_fbid=0&attached_photo_fbid=0&&&ft[tn]=[]&ft[fbfeed_location]=2&ft[id]=" + ft_ID + "&ft[author]=" + __user + "&nctr[_mod]=pagelet_group_mall&av=" + __user + "1&__user=" + __user + "&__a=1&__dyn=7nm8RW8BgCBynzpQ9UoGya4Au74qbx2mbAKGiyFqzQC_826m5-9V8CdDx2ubhEoBBzEy78S8zU46iicyaCw&__req=h&fb_dtsg=" + fb_dtsg + "&ttstamp=26581701024871531031139055778511252&__rev=1609713";
                        string PostUrl11 = "https://www.facebook.com/ajax/ufi/add_comment.php";
                        PostResponce = ObjHttpHelper.postFormDataProxy(new Uri(PostUrl11), PostDataUpdated, fbUser.proxyip, Convert.ToInt32(fbUser.proxyport), fbUser.proxyusername, fbUser.proxypassword);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                   
                }              



                if (PostResponce.Contains("CommentAddedActive"))
                {
                    GlobusLogHelper.log.Debug("(.) Send comment sucessfully on : " + TargetedUrl);
                    GlobusLogHelper.log.Info("(.) Send comment sucessfully on : " + TargetedUrl);
                }
                else
                {
                    if (PostResponce.Contains("errorSummary\":"))
                    {
                        string[] summaryArr = Regex.Split(PostResponce, "errorSummary\":");
                        summaryArr = Regex.Split(summaryArr[1], "\"");
                        string errorSummery = summaryArr[1];
                        string errorDiscription = summaryArr[5];

                        GlobusLogHelper.log.Info(" Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                        GlobusLogHelper.log.Debug(" Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

                    }

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                string Comment = Utils.getBetween(PostResponce, "[{\"id", "\",\"fbid\"");

                comment_id = Comment.Replace("\":\"", string.Empty);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            try
            {
                comment_legacyid = Utils.getBetween(PostResponce, "legacyid\":", "\",\"body\"");
                comment_legacyid = comment_legacyid.Replace("\"", string.Empty);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                client_id = Utils.getBetween(PostResponce, "clientid\":", "}]");
                client_id = client_id.Replace("\"", string.Empty).Replace(":", "%3A");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {

                ftId = Utils.getBetween(PostResponce, "clientid\":", "}]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            //PostData Delete Comment

            try
            {
                string PostDeletePostData = "comment_id=" + comment_id + "&comment_legacyid=" + comment_legacyid + "&ft_ent_identifier=" + ft_ent_identifier + "&one_click=false&source=0&client_id=" + client_id + "&ft[tn]=R]&ft[fbfeed_location]=2&ft[id]=" + ft_ent_identifier + "&ft[author]=" + __user + "&nctr[_mod]=pagelet_group_mall&__user=" + __user + "&__a=1&__dyn=7n8a9EAMCBynzpQ9UoHaHyG8qeyp9Esx6iWF3qGEVF4WpU&__req=5&fb_dtsg=" + fb_dtsg + "&ttstamp=265816711673778372&__rev=1152671";
                string PostDeleteUrl = FBGlobals.Instance.GroupPostAndDeleteAjaxufidelete_commentUrl;
             //   string DeletePostResponce = ObjHttpHelper.postFormDataProxy(new Uri(PostDeleteUrl), PostDeletePostData, fbUser.proxyip, Convert.ToInt32(fbUser.proxyport), fbUser.proxyusername, fbUser.proxypassword);
                string DeletePostResponce = ObjHttpHelper.postFormData(new Uri(PostDeleteUrl), PostDeletePostData);





                if (DeletePostResponce.Contains("CommentDeletedActive"))
                {
                    GlobusLogHelper.log.Debug("(.) Comment sucessfully Deleted on : " + TargetedUrl);
                    GlobusLogHelper.log.Info("(.) Comment sucessfully Deleted : " + TargetedUrl);
                    status = true;
                }
                else
                {
                    try
                    {

                        if (PostResponce.Contains("errorSummary\":"))
                        {
                            string[] summaryArr = Regex.Split(PostResponce, "errorSummary\":");
                            summaryArr = Regex.Split(summaryArr[1], "\"");
                            string errorSummery = summaryArr[1];
                            string errorDiscription = summaryArr[5];

                            GlobusLogHelper.log.Info(" Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug(" Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

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

            return status;
        }

        /// <summary>
        /// Star Group Fans Scraper  by ajay yadav Date 22-08-2014
        /// </summary>

        public List<string> LstGroup_GroupFansScraperUrls
        {
            get;
            set;
        }

        public static string strGroup_GroupFansScraperProcessUsing
        {
            get;
            set;
        }

        #region Global Variables For GroupFansScraper

        readonly object lockrThreadControllerGroupFansScraper = new object();
        public bool isStopGroupFansScraper = false;
        int countThreadControllerGroupFansScraper = 0;
        public List<Thread> lstThreadsGroupFansScraper = new List<Thread>();


        public static int minDelayGroupFansScraper = 10;
        public static int maxDelayGroupFansScraper = 20;
       

        #endregion

        public void StartGroupFansScraper()
        {
            countThreadControllerGroupFansScraper = 0;
            try
            {
                lstThreadsGroupFansScraper.Add(Thread.CurrentThread);
                lstThreadsGroupFansScraper.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                int numberOfAccountPatch = 25;

                

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerGroupFansScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerGroupFansScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerGroupFansScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account

                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread DeletePostThread = new Thread(Start_GroupFansScraper);
                                            DeletePostThread.Name = "workerThread_Profiler_" + acc;
                                            DeletePostThread.IsBackground = true;

                                            DeletePostThread.Start(new object[] { item });

                                            countThreadControllerGroupFansScraper++;
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void Start_GroupFansScraper(object parameters)
        {
            try
            {
                if (!isStopGroupFansScraper)
                {
                    try
                    {
                        lstThreadsGroupFansScraper.Add(Thread.CurrentThread);
                        lstThreadsGroupFansScraper.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {

                        {
                            Array paramsArray = new object[1];
                            paramsArray = (Array)parameters;

                            FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

                            if (!objFacebookUser.isloggedin)
                            {
                                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                                objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

                                Accounts.AccountManager objAccountManager = new AccountManager();
                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {

                                if (strGroup_GroupFansScraperProcessUsing == "Group Fans Scraper")
                                {
                                    StartGroupFansScraperProcess(ref objFacebookUser);
                                }
                               
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                            }
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

            finally
            {
                try
                {

                    // if (!isStopGroupCamapinScheduler)
                    {
                        lock (lockrThreadControllerGroupCamapinScheduler)
                        {
                            countThreadControllerGroupCamapinScheduler--;
                            Monitor.Pulse(lockrThreadControllerGroupCamapinScheduler);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartGroupFansScraperProcess(ref FacebookUser fbUser)
        {
            Dictionary<string, string> CheckStoryID = new Dictionary<string, string>();

            string grpurl = string.Empty;
            string grpurlresponse = string.Empty;
            string story_fbid = string.Empty;
            string __user = "";
            string fb_dtsg = "";

            GlobusLogHelper.log.Info("Please Wait process start " + " With UserName : " + fbUser.username);

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;           

            string pageSourceFb = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
            List<string> lstStoreUserInfo = new List<string>();
            List<string> lstOfCommentUserInfo = new List<string>();


            __user = GlobusHttpHelper.GetParamValue(pageSourceFb, "user");
            if (string.IsNullOrEmpty(__user))
            {
                __user = GlobusHttpHelper.ParseJson(pageSourceFb, "user").Replace("p?id=", "").Replace("\\", "");
            }

            #region MyRegion

            foreach (var LstGroup_GroupFansScraperUrls_item in LstGroup_GroupFansScraperUrls)
            {
                try
                {
                   // string _LstGroup_GroupFansScraperUrls_item = "https://www.facebook.com/groups/weareORGI/";
                    pageSourceFb = HttpHelper.getHtmlfromUrl(new Uri(LstGroup_GroupFansScraperUrls_item));

                    string[] Arr = System.Text.RegularExpressions.Regex.Split(pageSourceFb, "class=\"userContentWrapper");
                    try
                    {
                        Arr = Arr.Skip(1).ToArray();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    foreach (string item in Arr)
                    {
                        try
                        {
                            string _item = string.Empty;
                            if (item.Contains("php?id"))
                            {
                                try
                                {
                                    _item = Utils.getBetween(item, "php?id", ">").Replace("=", "").Replace(">", "").Replace("\"", "").Trim();
                                    if (_item.Contains("&amp;") || _item.Contains("&"))
                                    {
                                        _item = _item.Split('&')[0].Trim();
                                    }
                                    if (_item.Contains("}}]"))
                                    {
                                        _item = _item.Split('}')[0].Trim();
                                    }
                                    lstStoreUserInfo.Add(_item);
                                    GlobusLogHelper.log.Info("find the Post User id : " + _item);
                                    GlobusLogHelper.log.Debug("find the Post User id : " + _item);
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


                    string[] _arr = Regex.Split(pageSourceFb, "uiUfi UFIContainer");
                    _arr = _arr.Skip(1).ToArray();
                    foreach (string item in _arr)
                    {
                        try
                        {
                            string[] _arr11 = Regex.Split(item, "UFICommentActorName");
                            foreach (string _item in _arr11)
                            {
                                try
                                {
                                    if (_item.Contains("php?id"))
                                    {
                                        try
                                        {
                                            string __item = Utils.getBetween(_item, "php?id", ">").Replace("=", "").Replace(">", "").Replace("\"", "").Trim();
                                            if (__item.Contains("&amp;") || __item.Contains("&"))
                                            {
                                                __item = __item.Split('&')[0].Trim();
                                            }
                                            if (__item.Contains("}}]"))
                                            {
                                                __item = __item.Split('}')[0].Trim();
                                            }
                                            lstOfCommentUserInfo.Add(__item);
                                            GlobusLogHelper.log.Info("find the Comments User id : " + __item);
                                            GlobusLogHelper.log.Debug("find the COmments User id : " + __item);
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
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                 
                    //pagination 
                    string ajaxpipe_token = Utils.getBetween(pageSourceFb, "ajaxpipe_token", "\",").Replace("\":\"", "");
                    string GroupId = Utils.getBetween(pageSourceFb, "group_id\":", ",\"").Replace("\":\"", "");
                    string end_cursor = Utils.getBetween(pageSourceFb, "end_cursor\\\":\\\"", "=\\\",\\\"").Replace("\":\"", "");
                    string last_view_time = Utils.getBetween(pageSourceFb, "last_view_time", ",\"").Replace("\":\"", "");
                    int Counter = 2;
                    while (true)
                    {

                        string AjaxRequest = "https://www.facebook.com/ajax/pagelet/generic.php/GroupEntstreamPagelet?ajaxpipe=1&ajaxpipe_token=" + ajaxpipe_token + "&no_script_path=1&data=%7B%22last_view_time%22%3A0%2C%22is_file_history%22%3Anull%2C%22end_cursor%22%3A%22" + end_cursor + "%3D%22%2C%22group_id%22%3A" + GroupId + "%2C%22id%22%3A0%2C%22has_cards%22%3Atrue%7D&__user=" + __user + "&__a=1&__dyn=7n8anEAMCBynzpQ9UoHFaeExEW9ACxO4p9GgSGGeqrWo8popyUW4-49UJ6KibKm&__req=jsonp_" + Counter + "&__rev=1383064&__adt=" + Counter + "";

                        string PageSourceAjax = HttpHelper.getHtmlfromUrl(new Uri(AjaxRequest));
                        Counter = Counter + 1;
                        end_cursor = "";
                        string[] Arr1 = System.Text.RegularExpressions.Regex.Split(PageSourceAjax, "userContentWrapper");

                        string[] aaa = System.Text.RegularExpressions.Regex.Split(PageSourceAjax, "GroupEntstreamPagelet");
                        foreach (var aaa_item in aaa)
                        {
                            try
                            {
                                if (aaa_item.Contains("\\\"end_cursor\\\""))
                                {
                                    end_cursor = Utils.getBetween(aaa_item, "\\\"end_cursor\\\"", "=").Replace("end_cursor\":", "").Replace(":\\\"", "");
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }

                        try
                        {
                            Arr1 = Arr1.Skip(1).ToArray();

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        foreach (string item in Arr1)
                        {
                            try
                            {
                                string _item = string.Empty;
                                if (item.Contains("php?id"))
                                {
                                    try
                                    {
                                        _item = Utils.getBetween(item, "php?id", ">").Replace("=", "").Replace(">", "").Replace("\"", "").Replace("\\", "").Trim();
                                        if (_item.Contains("&amp;") || _item.Contains("&"))
                                        {
                                            _item = _item.Split('&')[0].Trim();
                                        }
                                        if (_item.Contains("}}]"))
                                        {
                                            _item = _item.Split('}')[0].Trim();
                                        }
                                        lstStoreUserInfo.Add(_item);
                                        GlobusLogHelper.log.Info("find the Post User id : " + _item);
                                        GlobusLogHelper.log.Debug("find the Post User id : " + _item);
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

                        string[] _arr122 = Regex.Split(PageSourceAjax, "uiUfi UFIContainer");
                        _arr122 = _arr122.Skip(1).ToArray();
                        foreach (string item in _arr122)
                        {
                            try
                            {
                                string[] arr4 = Regex.Split(item, "UFICommentActorName");
                                foreach (string _item in arr4)
                                {
                                    try
                                    {
                                        if (_item.Contains("php?id"))
                                        {
                                            try
                                            {
                                                string __item = Utils.getBetween(_item, "php?id", ">").Replace("=", "").Replace(">", "").Replace("\"", "").Replace("\\", "").Trim();
                                                if (__item.Contains("&amp;") || __item.Contains("&"))
                                                {
                                                    __item = __item.Split('&')[0].Trim();
                                                }
                                                if (__item.Contains("}}]"))
                                                {
                                                    __item = _item.Split('}')[0].Trim();
                                                }
                                                lstOfCommentUserInfo.Add(__item);
                                                GlobusLogHelper.log.Info("find the Comments User id : " + __item);
                                                GlobusLogHelper.log.Debug("find the Comments User id : " + __item);
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
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        // break While Loop

                        if (PageSourceAjax.Contains("<div class=\"phm _64f\">End of results</div>") || string.IsNullOrEmpty(end_cursor))                        
                        {
                            break;
                        }
                    }
                    //-------
                    List<string> tempList = new List<string>();
                   int countNumberOfPost = 0;
                   int countNumberOfComments = 0;
                  string   UserIdmostActiveUser = string.Empty;
                  string UserIdmostActiveUserForComments = string.Empty;
                    foreach (var item in lstStoreUserInfo)
                    {
                        try
                        {
                            if (!tempList.Contains(item))
                            {
                                tempList.Add(item);
                                var result = (from a in lstStoreUserInfo where a == item select a).ToList();
                                
                                  //if (result.Count >= countNumberOfPost)
                                  {
                                    UserIdmostActiveUser =(item);
                                    if (lstOfCommentUserInfo.Contains(UserIdmostActiveUser))
                                    {
                                        try
                                        {
                                            var CountResultsForComments = (from a in lstOfCommentUserInfo where a == item select a).ToList();
                                            countNumberOfComments = CountResultsForComments.Count;
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    countNumberOfPost = result.Count;
                                  }

                                  insertDataInDatabase(GroupId, countNumberOfPost, countNumberOfComments, UserIdmostActiveUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                   // insertDataInDatabase(GroupId, countNumberOfPost, countNumberOfComments, UserIdmostActiveUser);

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
            #endregion
        }

        private static void insertDataInDatabase(string GroupId, int countNumberOfPost, int countNumberOfComments, string UserIdmostActiveUser)
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(getNewLocalconnectionstring()))
                {
                    conn.Open();
                    string query = "Insert into groupdetail.groupmember(UserId,GroupId,PostId,NoOfPosts,NoOfComments) values (" + UserIdmostActiveUser + "," + GroupId + "," + "Null" + "," + countNumberOfPost + "," + countNumberOfComments + ")";
                    int result11 = conn.Execute(query);
                    if (result11 > 0)
                    {
                        //successful insert
                        GlobusLogHelper.log.Debug("Most Active User Id : " + UserIdmostActiveUser + "Group Id : " + GroupId + "Count Number Of Post : " + countNumberOfPost + " Count Number Of Comments : " + countNumberOfComments);
                        GlobusLogHelper.log.Info("Most Active User Id : " + UserIdmostActiveUser + "Group Id : " + GroupId + "Count Number Of Post : " + countNumberOfPost + " Count Number Of Comments : " + countNumberOfComments);
                    }
                    else
                    {
                        GlobusLogHelper.log.Debug("Most Active User Id : " + UserIdmostActiveUser + "Group Id : " + GroupId + "Count Number Of Post : " + countNumberOfPost + " Count Number Of Comments : " + countNumberOfComments);
                        //faliure in insert.
                    }
                }
            }
            catch (Exception ex)
            {
                using (MySqlConnection conn = new MySqlConnection(getNewLocalconnectionstring()))
                {
                    conn.Open();
                    string query = "Insert into groupdetail.groupmember(UserId,GroupId,PostId,NoOfPosts,NoOfComments) values (" + UserIdmostActiveUser + "," + GroupId + "," + "Null" + "," + countNumberOfPost + "," + countNumberOfComments + ")";
                    query = "update groupdetail.groupmember set GroupId=" + GroupId + " and NoOfPosts=" + countNumberOfPost + " and NoOfComments=" + countNumberOfComments + " where UserId=" + UserIdmostActiveUser + " and GroupId=" + GroupId + "";
                    int result11 = conn.Execute(query);

                    GlobusLogHelper.log.Debug("Update data base "+"Most Active User Id : " + UserIdmostActiveUser + "Group Id : " + GroupId + "Count Number Of Post : " + countNumberOfPost + " Count Number Of Comments : " + countNumberOfComments);
                    GlobusLogHelper.log.Info("Update data base " + "Most Active User Id : " + UserIdmostActiveUser + "Group Id : " + GroupId + "Count Number Of Post : " + countNumberOfPost + " Count Number Of Comments : " + countNumberOfComments);
                }


                //GlobusFileHelper.AppendStringToTextfileNewLineWithCarat("Error : urlstore() : urlstore --> " + ex.Message, ErrorLogPath);
            }
        } 

        public static IEnumerable<dynamic> SelectUrlfromCompanyTable()
        {
            IEnumerable<dynamic> result = null;
            

            return result;
        }

        public static string getNewLocalconnectionstring()
        {
            string ConnectionString = "Host=127.0.0.1;User ID=root;Password=AVda_3902;persist security info=False;initial catalog=groupdetail;Pooling=false;";// @"SERVER=localhost;DATABASE=groupdetail;Uid=root;Pwd=password;";
            return ConnectionString;
           // return "Host=127.0.0.1;User ID=root;Password=AVda_3902;persist security info=False;initial catalog=groupdetail;Pooling=false;";
        }

    }
}
