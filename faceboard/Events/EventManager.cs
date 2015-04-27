using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using faceboardpro;
using Accounts;
using System.Data;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Events
{
    public class EventManager
    {
        public BaseLib.Events eventInviterEvent = null;

        #region Global Variables For Event Inviter

        readonly object lockrThreadControllerEventInviter = new object();

        public bool isStopEventInviter = false;
        int countThreadControllerEventInviter = 0;
        public static int intNoOfFriends = 30;

        public static int minDelayEventInvitor = 10;
        public static int maxDelayEventInvitor = 20;

        public List<Thread> lstThreadsEventInviter = new List<Thread>();

        #endregion

        #region Property For Event Inviter

        public int NoOfThreadsEventInviter
        {
            get;
            set;
        }

        public bool SendToAllFriendsEventInviter
        {
            get;
            set;
        }

        public int NoOfSuggestionPerAccountEventInviter
        {
            get;
            set;
        }

        public int NoOfFriendsSuggestionAtOneTimeEventInviter
        {
            get;
            set;
        }

        public List<string> LstEventURLsEventInviter
        {
            get;
            set;
        }


        #endregion

        public EventManager()
        {
            eventInviterEvent = new BaseLib.Events();
        }

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                eventInviterEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void StartEventInviter()
        {
            try
            {
                countThreadControllerEventInviter = 0;

                int numberOfAccountPatch = 25;

                if (NoOfThreadsEventInviter > 0)
                {
                    numberOfAccountPatch = NoOfThreadsEventInviter;
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
                                lock (lockrThreadControllerEventInviter)
                                {
                                    try
                                    {
                                        if (countThreadControllerEventInviter >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerEventInviter);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsEventInviter);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerEventInviter++;
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

        public void StartMultiThreadsEventInviter(object parameters)
        {
            try
            {
                if (!isStopEventInviter)
                {
                    try
                    {
                        lstThreadsEventInviter.Add(Thread.CurrentThread);
                        lstThreadsEventInviter.Distinct();
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


                                //Login Process

                                Accounts.AccountManager objAccountManager = new AccountManager();


                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {
                                // Call StartActionEventInviter
                                StartActionEventInviter(ref objFacebookUser);
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
                 //   if (!isStopEventInviter)
                    {
                        lock (lockrThreadControllerEventInviter)
                        {
                            countThreadControllerEventInviter--;
                            Monitor.Pulse(lockrThreadControllerEventInviter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionEventInviter(ref FacebookUser fbUser)
        {
            try
            {
                InviteFriendsEventInviter(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void InviteFriendsEventInviter(ref FacebookUser fbUser)
        {
            try
            {
                string fb_dtsg = "";
                int index = 0;
                string __user = "";
                string strEventURLPageSource = "";
                string strplan_id = "";

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                List<string> lstFriend = new List<string>();

                foreach (string lstEventURLsFileitem in LstEventURLsEventInviter)
                {
                    try
                    {
                        int CountInvitation = 1;

                        strEventURLPageSource = HttpHelper.getHtmlfromUrl(new Uri(lstEventURLsFileitem));

                        __user = GlobusHttpHelper.GetParamValue(strEventURLPageSource, "user");
                        if (string.IsNullOrEmpty(__user))
                        {
                            __user = GlobusHttpHelper.ParseJson(strEventURLPageSource, "user");
                        }

                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                        {
                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                            return;
                        }

                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strEventURLPageSource);

                        // Find Total Friends
                        lstFriend = FBUtils.GetAllFriends(ref HttpHelper, __user);
                        lstFriend = lstFriend.Distinct().ToList();

                        GlobusLogHelper.log.Info("Total Friends : " + lstFriend.Count + " for " + fbUser.username);
                        GlobusLogHelper.log.Debug("Total Friends : " + lstFriend.Count + " for " + fbUser.username);

                        List<string> lstids = new List<string>();

                        if (SendToAllFriendsEventInviter)
                        {
                            intNoOfFriends = lstFriend.Count - 1;
                        }

                        foreach (string item in lstFriend)
                        {
                            try
                            {
                                if (item.Contains("&"))
                                {
                                    try
                                    {
                                        string[] IdData = Regex.Split(item, "&");
                                        lstids.Add(IdData[0]);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    lstids.Add(item);

                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        List<string> lstInvitedFriends = new List<string>();
                        foreach (string lstFrienditem in lstids)
                        {
                            try
                            {



                                if (CountInvitation > intNoOfFriends)
                                {

                                    break;
                                }

                                lstInvitedFriends.Add(lstFrienditem);
                                CountInvitation++;
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        #region Split IDs in 100s and Send
                        List<List<string>> split_ListIDs = Utils.Split(lstInvitedFriends, NoOfFriendsSuggestionAtOneTimeEventInviter);


                        foreach (List<string> item in split_ListIDs)
                        {
                            try
                            {
                                index = 0;
                                string checkableitems = "&checkableitems[" + index + "]";
                                string profileChooserItems = "%7B%22";

                                foreach (string lstFrienditem in item)
                                {
                                    try
                                    {
                                        index++;
                                        profileChooserItems = profileChooserItems + lstFrienditem + "%22%3A1%2C%22";
                                        checkableitems = checkableitems + "=" + lstFrienditem + "&checkableitems[" + index + "]";
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }


                                try
                                {
                                    int indexOfLastComma = profileChooserItems.LastIndexOf("%2C%22");
                                    profileChooserItems = profileChooserItems.Remove(indexOfLastComma);
                                    profileChooserItems = profileChooserItems + "%7D";
                                    int indexOfLastcheckableitems = checkableitems.LastIndexOf("&checkableitems[" + index + "]");
                                    checkableitems = checkableitems.Remove(indexOfLastcheckableitems);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }



                                if (lstEventURLsFileitem.Contains("events/"))
                                {
                                    try
                                    {
                                        string eventUrlsTemp = lstEventURLsFileitem + "/";
                                        strplan_id = eventUrlsTemp.Substring(eventUrlsTemp.IndexOf("events/"), (eventUrlsTemp.IndexOf('/', eventUrlsTemp.IndexOf("events/") + 8)) - eventUrlsTemp.IndexOf("events/")).Replace("events/", string.Empty).Trim();
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    #region BySan
                                    if (strEventURLPageSource.Contains("ajax/events/permalink/join.php") || strEventURLPageSource.Contains("Invite Friends"))
                                    {
                                        try
                                        {
                                            string joinPostDataUrl = FBGlobals.Instance.EventInviterPostAjaxJoinPHP;

                                            //eid=160921707405189&ref=0&nctr[_mod]=pagelet_event_header&__user=100004323278246&__a=1&__dyn=798ahxoNpGojEa0&__req=k&fb_dtsg=AQCKCBkm&phstamp=1658167756766107109133

                                            string joinPostData = "eid=" + strplan_id + "&ref=0&nctr[_mod]=pagelet_event_header&__user=" + __user + "&__a=1&__dyn=798aD5z5ynU-wE&__req=9&fb_dtsg=" + fb_dtsg + "&phstamp=165816749496688101132";
                                            string ResponseOfJoinClickPost = HttpHelper.postFormData(new Uri(joinPostDataUrl), joinPostData);

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    #endregion

                                    string strAjaxGetRequest1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxChoosePlan_Id + strplan_id + "&causal_element=js_" + CountInvitation + "&__asyncDialog=1&__user=" + __user + "&__a=1"));
                                    string strAjaxGetRequest2 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxIncludeAllPlan_Id + strplan_id + "&__user=" + __user + "&__a=1"));

                                    //string strPostData = "fb_dtsg=" + fb_dtsg + "&profileChooserItems=" + profileChooserItems + checkableitems + "&__user=" + __user + "&__a=1&phstamp=" + Globals.GenerateTimeStamp() + ""; //fb_dtsg=AQCAp9jD&profileChooserItems=%7B%22100001409031727%22%3A1%7D&checkableitems[0]=100001409031727&__user=100003798185175&__a=1&phstamp=1658167651125710668131"
                                    string strPostData = "fb_dtsg=" + fb_dtsg + "&profileChooserItems=" + profileChooserItems + checkableitems + "&__user=" + __user + "&__a=1&__dyn=798aD5z5ynU&__req=a&phstamp=" + Utils.GenerateTimeStamp() + ""; //fb_dtsg=AQCAp9jD&profileChooserItems=%7B%22100001409031727%22%3A1%7D&checkableitems[0]=100001409031727&__user=100003798185175&__a=1&phstamp=1658167651125710668131"

                                    //string strPostURL = "http://www.facebook.com/ajax/events/permalink/invite.php?plan_id=" + strplan_id + "&profile_chooser=1";

                                    string strPostURL = FBGlobals.Instance.EventInviterPostAjaxInvitePlan_Id + strplan_id + "&source=1";


                                    string lastResponseStatus = string.Empty;
                                    //  string strResponse = HttpHelper.postFormData(new Uri(strPostURL), strPostData);//HttpHelper.postFormData(new Uri(strPostURL), strPostData);

                                    string strResponse = HttpHelper.postFormData(new Uri(strPostURL), strPostData, ref lastResponseStatus, lstEventURLsFileitem);//string strResponse = HttpHelper.postFormData(new Uri(strPostURL), strPostData, ref lastResponseStatus)//HttpHelper.postFormData(new Uri(strPostURL), strPostData);
                                    if (lastResponseStatus.Contains("error: (404) Not Found"))
                                    {
                                        GlobusLogHelper.log.Info("URL : " + lstEventURLsFileitem + " isn't owned by Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("URL : " + lstEventURLsFileitem + " isn't owned by Username : " + fbUser.username);

                                        break;
                                    }


                                    if (!strResponse.Contains("error"))
                                    {
                                        foreach (string id in item)
                                        {
                                            GlobusLogHelper.log.Info("Invited : " + id + " with UserName : " + fbUser.username + " for URL : " + lstEventURLsFileitem);
                                            GlobusLogHelper.log.Debug("Invited : " + id + " with UserName : " + fbUser.username + " for URL : " + lstEventURLsFileitem);
                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayEventInvitor * 1000, maxDelayEventInvitor * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }

                                        GlobusLogHelper.log.Info("Invited Friends : " + item.Count + " with UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Invited Friends : " + item.Count + " with UserName : " + fbUser.username);

                                    }

                                    else
                                    {
                                        GlobusLogHelper.log.Info(" Error With URL : " + lstEventURLsFileitem + " By Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug(" Error With URL : " + lstEventURLsFileitem + " By Username : " + fbUser.username);

                                        break;

                                    }

                                    ///Delay
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayEventInvitor * 1000, maxDelayEventInvitor * 1000);
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

                        #endregion



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

            GlobusLogHelper.log.Info("Process Completed With User Name : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed With User Name : " + fbUser.username);

        }


        // All Event Creator Process

        #region Global Variables For Event Creator

        readonly object lockrThreadControllerEvenCreator = new object();

        public bool isStopEvenCreator = false;
        int countThreadControllerEvenCreator = 0;

        public static int minDelayEventCreator = 10;
        public static int maxDelayEventCreator = 20;


        public List<Thread> lstThreadsEvenCreator = new List<Thread>();

        #endregion

        #region Property For Event Inviter

        public int NoOfThreadsEvenCreator
        {
            get;
            set;
        }

        public List<string> LstEventDetailsEventCreator
        {
            get;
            set;
        }
       
        #endregion

        public void StartEvenCreator()
        {
            try
            {
                countThreadControllerEvenCreator = 0;

                int numberOfAccountPatch = 25;

                if (NoOfThreadsEvenCreator > 0)
                {
                    numberOfAccountPatch = NoOfThreadsEvenCreator;
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
                                lock (lockrThreadControllerEvenCreator)
                                {
                                    try
                                    {
                                        if (countThreadControllerEvenCreator >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerEvenCreator);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsEvenCreator);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerEvenCreator++;
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

        public void StartMultiThreadsEvenCreator(object parameters)
        {
            try
            {
                if (!isStopEvenCreator)
                {
                    try
                    {
                        lstThreadsEvenCreator.Add(Thread.CurrentThread);
                        lstThreadsEvenCreator.Distinct();
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


                                //Login Process

                                Accounts.AccountManager objAccountManager = new AccountManager();


                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }
                        

                            if (objFacebookUser.isloggedin)
                            {
                                // Call StartActionEventInviter
                                StartActionEvenCreator(ref objFacebookUser);
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
                   // if (!isStopEvenCreator)
                    {
                        lock (lockrThreadControllerEvenCreator)
                        {
                            countThreadControllerEvenCreator--;
                            Monitor.Pulse(lockrThreadControllerEvenCreator);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionEvenCreator(ref FacebookUser fbUser)
        {
            try
            {
                CreateEvent(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void CreateEvent(ref FacebookUser fbUser)
        {
            try
            {
                GlobusLogHelper.log.Info("Start Event Creaton With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Event Creaton With Username : " + fbUser.username);


                // Post Data
                //fb_dtsg=AQACnqfz&title=Party&details_text=My%20birhday%20Party&details=My%20birhday%20Party&pre_details=&location_id=&location=Bhiali&isplacetexttag=&pre_location=&pre_location_id=&when_dateIntlDisplay=6%2F18%2F2013&when_date=6%2F18%2F2013&when_time=&when_time_display_time=&audience[0][value]=40&guest_invite=on&pre_guest_invite=&parent_id=&source=10&who=&__user=100004002347820&__a=1&__dyn=7n8apij35zpVpQ9UmAEKU&__req=1f&phstamp=165816567110113102122422

                string fbdtsg = string.Empty;
                string title = string.Empty;
                string detailstext = string.Empty;
                string details = string.Empty;
                string location = string.Empty;
                string whendateIntlDisplay = string.Empty;
                string whendate = string.Empty;
                string whentime = string.Empty;
                string whentimedisplaytime = string.Empty;
                string audiencevalue = string.Empty;
                string userid = string.Empty;
                string locationid = string.Empty;
                string timeid = string.Empty;
                string timezone = string.Empty;

                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                string homePageSource=gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                userid = GlobusHttpHelper.GetParamValue(homePageSource, "user");
                if (string.IsNullOrEmpty(userid))
                {
                    userid = GlobusHttpHelper.ParseJson(homePageSource, "user");
                }

                if (string.IsNullOrEmpty(userid) || userid == "0" || userid.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                fbdtsg = GlobusHttpHelper.Get_fb_dtsg(homePageSource);
                

                foreach (string item in LstEventDetailsEventCreator)
                {
                    try
                    {
                        string[] eventDetailsArr=Regex.Split(item, "<:>");

                        for (int i = 0; i < eventDetailsArr.Length; i++)
                        {
                            try
                            {
                                string EventDetails=eventDetailsArr[i];
                                
                                if (EventDetails.Contains("Name") || EventDetails.Contains("name"))
                                {
                                    try
                                    {
                                        //title = Uri.EscapeDataString(EventDetails.Replace("Name", string.Empty).Replace("name", string.Empty).Trim());
                                        title = Uri.EscapeDataString(EventDetails.Replace("Name", string.Empty).Replace("name", string.Empty).Replace("<",string.Empty).Replace(">",string.Empty).Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (EventDetails.Contains("Details") || EventDetails.Contains("details"))
                                {
                                    try
                                    {
                                        //detailstext = Uri.EscapeDataString(EventDetails.Replace("Details", string.Empty).Replace("details", string.Empty).Trim());
                                        detailstext = Uri.EscapeDataString(EventDetails.Replace("Details", string.Empty).Replace("details", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim());
                                        details = detailstext;

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (EventDetails.Contains("Where") || EventDetails.Contains("where"))
                                {
                                    try
                                    {
                                        //location = Uri.EscapeDataString(EventDetails.Replace("Where", string.Empty).Replace("where", string.Empty).Trim());
                                        location = Uri.EscapeDataString(EventDetails.Replace("Where", string.Empty).Replace("where", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (EventDetails.Contains("When") || EventDetails.Contains("when"))
                                {
                                    try
                                    {
                                        //whendateIntlDisplay = Uri.EscapeDataString(EventDetails.Replace("When", string.Empty).Replace("when", string.Empty).Trim());
                                        whendateIntlDisplay = Uri.EscapeDataString(EventDetails.Replace("When", string.Empty).Replace("when", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim());
                                        whendate = whendateIntlDisplay;

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (EventDetails.Contains("Add a time") || EventDetails.Contains("add a time"))
                                {
                                    try
                                    {
                                        //whentimedisplaytime = Uri.EscapeDataString(EventDetails.Replace("Add a time", string.Empty).Replace("add a time", string.Empty).Trim());
                                        whentime = EventDetails.Replace("Add a time", string.Empty).Replace("add a time", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim();
                                        whentimedisplaytime = Uri.EscapeDataString(EventDetails.Replace("Add a time", string.Empty).Replace("add a time", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim());
                                        //whentime = whentimedisplaytime;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (EventDetails.Contains("Privacy") || EventDetails.Contains("privacy"))
                                {
                                    try
                                    {
                                        //audiencevalue = ("40").Replace("whentimedisplaytime", string.Empty).Replace("whentimedisplaytime", string.Empty).Trim();
                                        audiencevalue = ("40").Replace("whentimedisplaytime", string.Empty).Replace("whentimedisplaytime", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Trim();
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

                        try
                        {
                            string getlocationpage = "https://www.facebook.com/ajax/places/typeahead?value=" + location + "&include_address=2&include_subtext=true&exact_match=false&use_unicorn=true&allow_places=true&allow_cities=true&render_map=true&limit=15&new_js_ranking=0&include_source=plan_edit&city_bias=false&map_height=150&map_width=348&ref=xhp_fb__events__create__location_input%3A%3Arender&sid=771836702690&city_id=1019627&city_set=false&request_id=0.6745269983075559&__user=" + userid +"&__a=1&__dyn=7n8ahyj35zoSt2u6aWizG85oCiq78hyWgSmEVFLFwxBxCbzGxa48jhHw&__req=1q&__rev=1353801%20HTTP/1.1";
                            string pageresponseGetlocationpage = gHttpHelper.getHtmlfromUrl(new Uri(getlocationpage));
                            int startindex = pageresponseGetlocationpage.IndexOf("uid\":");
                            string start = pageresponseGetlocationpage.Substring(startindex).Replace("uid\":",string.Empty);
                            int endindex = start.IndexOf(",");
                            string end = start.Substring(0, endindex).Replace(",",string.Empty);
                            locationid = end.Trim();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        try
                        {
                            if (whentime.StartsWith("0"))
                            {
                                int startindex2 = whentime.IndexOf("0");
                               whentime  = whentime.Substring(startindex2).Replace("0",string.Empty);
                            }
                            string gettimepage = "https://www.facebook.com/ajax/typeahead/time_bootstrap.php?request_id=0.8878094537649304&__user=" + userid + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyKAEWy6zECiq78hACF3qGEVFLFwxBxCbzGxa49UJ6K&__req=25&__rev=1353801%20HTTP/1.1";
                            string pageresponseGetTimePage = gHttpHelper.getHtmlfromUrl(new Uri(gettimepage));
                            int startindex = pageresponseGetTimePage.IndexOf(whentime);
                            string start = pageresponseGetTimePage.Substring(startindex).Replace(whentime, string.Empty);
                            int startindex1 = start.IndexOf("uid\":");
                            string start1 = start.Substring(startindex1).Replace("uid\":",string.Empty);
                            int endindex1 = start1.IndexOf(",");
                            string end = start1.Substring(0, endindex1).Replace(",", string.Empty).Replace("\"",string.Empty);
                            timeid = end.Trim();
                            
                                
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            string timezonedata = "place_id=" + locationid + "&date_str=" + whendateIntlDisplay + "&__user=" + userid + "&ttstamp=" + Utils.GenerateTimeStamp() + "";
                            string url = "https://www.facebook.com/ajax/plans/create/timezone.phpHTTP/1.1";
                            string pgresponse = gHttpHelper.postFormData(new Uri(url), timezonedata);
                            int startindex = pgresponse.IndexOf("tz_identifier\":\"");
                            string start = pgresponse.Substring(startindex).Replace("tz_identifier\":\"", string.Empty);
                            int endindex = start.IndexOf("}");
                            string end = start.Substring(0, endindex).Replace("}", string.Empty).Replace("\"",string.Empty).Replace("\\",string.Empty);
                            timezone = Uri.EscapeDataString(end.Trim());
                        }
                        catch
                        { }

                        string createEventPS = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventCreatorGetCreateEventUrl));

                        string createEventDialogPS = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventCreatorGetAjaxCreateEventDialogUrl + userid + "&__a=1&__dyn=7n8apij35zpVpQ9UmAEKU&__req=g"));

                        string createEventPostSaveUrl = FBGlobals.Instance.EventCreatorPostAjaxCreateEventSaveUrl;

                       //string savePostData = "fb_dtsg=" + fbdtsg + "&title=" + title + "&details_text=" + detailstext + "&details=" + details + "&pre_details=&location_id="+ locationid + "&location=" + location + "&isplacetexttag=&pre_location=&pre_location_id=&when_dateIntlDisplay=" + whendateIntlDisplay + "&when_date=" + whendate + "&when_time="+ timeid +"&when_time_display_time=" + whentimedisplaytime+"&audience[0][value]=" + audiencevalue + "&guest_invite=on&pre_guest_invite=&parent_id=&source=10&who=&__user=" + userid + "&__a=1&__dyn=7n8apij35zpVpQ9UmAEKU&__req=1f&phstamp=" + Utils.GenerateTimeStamp() + "";
                        string savePostData = "fb_dtsg=" + fbdtsg + "&title=" + title + "&details_text=" + detailstext + "&details=" + details + "&pre_details=&location_id=" + locationid + "&location=" + location + "&isplacetexttag=&pre_location=&pre_location_id=&when_dateIntlDisplay=" + whendateIntlDisplay + "&when_date=" + whendate + "&when_time=" + timeid + "&when_time_display_time=" + whentimedisplaytime + "&when_timezone=" + timezone + "&privacyx=1439959856260766&extra_data=&who=&__user=" + userid + "&__a=1&__dyn=7n8apij35zpVpQ9UmAEKU&__req=g&ttstamp=" + Utils.GenerateTimeStamp() + "";
                        string createEventPostSaveRes = gHttpHelper.postFormData(new Uri(createEventPostSaveUrl), savePostData);

                        if (createEventPostSaveRes.Contains("?context=create"))
                        {
                            string eventCreatedURL=string.Empty;

                            try
                            {
                                eventCreatedURL = createEventPostSaveRes.Substring(createEventPostSaveRes.IndexOf("goURI("), createEventPostSaveRes.IndexOf("?context=create", createEventPostSaveRes.IndexOf("goURI(")) - createEventPostSaveRes.IndexOf("goURI(")).Replace("\"", string.Empty).Replace("goURI(", string.Empty).Replace("events", string.Empty).Replace("\\",string.Empty).Replace(@"\\\/",string.Empty).Replace(@"//",string.Empty).Replace(@"/",string.Empty).Trim();
                                eventCreatedURL = FBGlobals.Instance.fbeventsUrl + eventCreatedURL;
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            GlobusLogHelper.log.Info("Event Created URL :" + eventCreatedURL + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Event Created URL :" + eventCreatedURL + " With Username : " + fbUser.username);

                            int delayInSeconds = Utils.GenerateRandom(minDelayEventCreator * 1000, maxDelayEventCreator * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);
                        }
                        else if (createEventPostSaveRes.Contains("errorSummary"))
                        {
                            string errorSummary = FBUtils.GetErrorSummary(createEventPostSaveRes);

                            GlobusLogHelper.log.Info("Event Creation Error  :" + errorSummary + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Event Creation Error  :" + errorSummary + " With Username : " + fbUser.username);


                            int delayInSeconds = Utils.GenerateRandom(minDelayEventCreator * 1000, maxDelayEventCreator * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Error In Event Creation With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Error In Event Creation With Username : " + fbUser.username);

                            int delayInSeconds = Utils.GenerateRandom(minDelayEventCreator * 1000, maxDelayEventCreator * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);
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

            GlobusLogHelper.log.Info("Process Completed Of Event Creaton With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Event Creaton With Username : " + fbUser.username);

        }

    }
}
