using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using BaseLib;
using faceboardpro;
using Accounts;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Scrapers
{
    public class EventScraper
    {
        public BaseLib.Events eventScraperEvent = null;

        #region Global Variables For Event Scraper

        readonly object lockrThreadControllerEventScraper = new object();
        public bool isStopEventScraper = false;
        int countThreadControllerEventScraper = 0;
        public List<Thread> lstThreadsEventScraper = new List<Thread>();

        public static string ScrapersExprotFilePath = string.Empty;
        public static string ScrapersFansScraperExprotFilePath = string.Empty;

        #endregion


        #region Property For Event Scraper

        public int NoOfThreadsEventScraper
        {
            get;
            set;
        }
        public List<string> LstEventURLsEventScraper
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
                eventScraperEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public EventScraper()
        {
            eventScraperEvent = new BaseLib.Events();
        }

        public void StartEventScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsEventScraper > 0) 
                {
                    numberOfAccountPatch = NoOfThreadsEventScraper;
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
                                lock (lockrThreadControllerEventScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerEventScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerEventScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsEventScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerEventScraper++;
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

        public void StartMultiThreadsEventScraper(object parameters)
        {
            try
            {
                if (!isStopEventScraper)
                {
                    try
                    {
                        lstThreadsEventScraper.Add(Thread.CurrentThread);
                        lstThreadsEventScraper.Distinct();
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

                                StartActionEventScraper(ref objFacebookUser);
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
                  //  if (!isStopEventScraper)
                    {
                        lock (lockrThreadControllerEventScraper)
                        {
                            countThreadControllerEventScraper--;
                            Monitor.Pulse(lockrThreadControllerEventScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionEventScraper(ref FacebookUser fbUser)
        {
            try
            {
                //ExtractEventFriendIdsEventScraper(ref fbUser);
                ExtractEventFriendIdsEventScraperNew(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void ExtractEventFriendIdsEventScraper(ref FacebookUser fbUser)
        {
            string __user = string.Empty;
            string EventDetailsSource = string.Empty;
            string fb_dtsg = string.Empty;
            string inviterstatus = string.Empty;
            Dictionary<string ,string> CheckDuplicats = new Dictionary<string ,string>();

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            foreach (string lstEventURLsFl in LstEventURLsEventScraper)
            {


                if (!lstEventURLsFl.Contains("https://www.facebook.com/events/"))
                {
                        GlobusLogHelper.log.Info("Invalid Url : " + lstEventURLsFl + "Please Upload Valid Url !");
                        GlobusLogHelper.log.Debug("Invalid Url : " + lstEventURLsFl+ "Please Upload Valid Url !");
                }


                try
                {

                    EventDetailsSource = HttpHelper.getHtmlfromUrl(new Uri(lstEventURLsFl));
                    __user = GlobusHttpHelper.GetParamValue(EventDetailsSource, "user");
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(EventDetailsSource, "user");
                    }

                    if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                    {
                        GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                        return;
                    }

                    fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(EventDetailsSource);

                    string[] InvitedData = Regex.Split(EventDetailsSource, "_51m- vTop pas");  //  //prs lfloat uiBoxWhite noborder

                    if (InvitedData.Length < 2)
                    {
                        InvitedData = Regex.Split(EventDetailsSource, "pagelet_event_guests");
                    }

                    foreach (string item in InvitedData)
                    {

                        try
                        {
                            string stradminlink = string.Empty;
                            string EventDetailsSourceForGoing = string.Empty;
                            if (!item.Contains("<!DOCTYPE html>"))
                            {
                                string status = string.Empty;
                                try
                                {
                                    status = item.Substring(item.IndexOf("role=\"button\">"), (item.IndexOf(" ", item.IndexOf("role=\"button\">")) - item.IndexOf("role=\"button\">"))).Replace("role=\"button\">", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Trim();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                try
                                {
                                    status = Utils.getBetween(item, "<span class=\"_3enh\">", "</span>");
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (status.Contains("Going") || status.Contains("going"))
                                {
                                    try
                                    {
                                        inviterstatus = "Going";
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }
                                if (status.Contains("Maybe") || status.Contains("maybe"))
                                {
                                    inviterstatus = "Maybe";
                                }
                                if (status.Contains("Invited") || status.Contains("invited"))
                                {
                                    inviterstatus = "Invited";
                                }
                                try
                                {
                                    stradminlink = item.Substring(item.IndexOf("href="), (item.IndexOf(" ", item.IndexOf("href=")) - item.IndexOf("href="))).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Trim();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                try
                                {
                                    EventDetailsSourceForGoing = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + stradminlink));
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (EventDetailsSourceForGoing.Contains("fbProfileBrowserList fbProfileBrowserListContainer"))
                                {
                                    string[] hnjhj = Regex.Split(EventDetailsSourceForGoing, "fbProfileBrowserListItem uiListItem");
                                    string[] FindFirstPageLoadId = Regex.Split(EventDetailsSourceForGoing, "/ajax/hovercard/user.php");
                                    List<string> lstitem = new List<string>();
                                    foreach (string eachitem in FindFirstPageLoadId)
                                    {
                                        try
                                        {
                                            if (!eachitem.Contains("<!DOCTYPE html>"))
                                            {
                                                string FindsubstringId = eachitem.Substring(eachitem.IndexOf("id="), (eachitem.IndexOf(">", eachitem.IndexOf("id=")) - eachitem.IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Trim();

                                                if (FindsubstringId.Contains("&"))
                                                {
                                                    try
                                                    {
                                                        FindsubstringId = FindsubstringId.Substring(0, FindsubstringId.IndexOf("&")).Trim();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Error >>> " + ex.StackTrace);
                                                    }
                                                }

                                                lstitem.Add(FindsubstringId);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    lstitem = lstitem.Distinct().ToList();

                                 
                                    string strid = stradminlink.Substring(stradminlink.IndexOf("/browse/event_members/?id="), (stradminlink.IndexOf("&", stradminlink.IndexOf("/browse/event_members/?id=")) - stradminlink.IndexOf("/browse/event_members/?id="))).Replace("/browse/event_members/?id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Trim();

                                    string idss = strid;
                                    List<string> allid = Extractsnippet_idsFb(ref HttpHelper, ref __user, ref idss, ref status);
                                    List<string> lstallid = new List<string>();
                                    var lstallidss = lstitem.Concat(allid);
                                    lstallid = lstallidss.ToList();
                                    lstallid = lstallid.Distinct().ToList();

                                    foreach (string singleitem in lstallid)
                                    {
                                        try
                                        {
                                            string singleitem1 = singleitem;

                                            string Name = string.Empty;
                                            string FirstName = string.Empty;
                                            string LastName = string.Empty;
                                            string UserLink = string.Empty;
                                            string UserName = string.Empty;
                                            string Gender = string.Empty;

                                            string name = string.Empty;
                                            string firstname = string.Empty;
                                            string lastname = string.Empty;
                                            string userlink = string.Empty;
                                            string username = string.Empty;
                                            string gender = string.Empty;


                                            if (singleitem.Contains("&"))
                                            {
                                                try
                                                {
                                                    singleitem1 = singleitem.Substring(0, singleitem.IndexOf("&")).Trim();
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            string Graphsource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + singleitem1));

                                            if (Graphsource.Contains("name\":"))
                                            {
                                                try
                                                {
                                                    Name = Graphsource.Substring(Graphsource.IndexOf("name\":")).Replace("name\":", string.Empty);
                                                    string[] NameArr = Regex.Split(Name, "\"");
                                                    name = NameArr[1].Replace("\"", string.Empty);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (Graphsource.Contains("first_name\":"))
                                            {
                                                try
                                                {
                                                    FirstName = Graphsource.Substring(Graphsource.IndexOf("first_name\":")).Replace("first_name\":", string.Empty);

                                                    string[] FirstNameArr = Regex.Split(FirstName, "\"");
                                                    firstname = FirstNameArr[1].Replace("\"", string.Empty);
                                                   
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (Graphsource.Contains("last_name\":"))
                                            {
                                                try
                                                {
                                                    LastName = Graphsource.Substring(Graphsource.IndexOf("last_name\":")).Replace("last_name\":", string.Empty);

                                                    string[] LastNameArr = Regex.Split(LastName, "\"");
                                                    lastname = LastNameArr[1].Replace("\"", string.Empty);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (Graphsource.Contains("link\":"))
                                            {
                                                try
                                                {
                                                    UserLink = Graphsource.Substring(Graphsource.IndexOf("link\":")).Replace("link\":", string.Empty);

                                                    string[] UserLinkArr = Regex.Split(UserLink, "\"");
                                                    userlink = UserLinkArr[1].Replace("\"", string.Empty);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (Graphsource.Contains("username\":"))
                                            {
                                                try
                                                {
                                                    UserName = Graphsource.Substring(Graphsource.IndexOf("username\":")).Replace("username\":", string.Empty);

                                                    string[] UserNameArr = Regex.Split(UserName, "\"");
                                                    username = UserNameArr[1].Replace("\"", string.Empty);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                            }

                                            if (Graphsource.Contains("gender\":"))
                                            {
                                                try
                                                {
                                                    Gender = Graphsource.Substring(Graphsource.IndexOf("gender\":")).Replace("gender\":", string.Empty);

                                                    string[] GenderArr = Regex.Split(Gender, "\"");
                                                    gender = GenderArr[1].Replace("\"", string.Empty);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (string.IsNullOrEmpty(userlink))
                                            {
                                                userlink = FBGlobals.Instance.fbhomeurl + username;    //"https://www.facebook.com/"
                                            }

                                            string FBEmailId = string.Empty;
                                            if (!string.IsNullOrEmpty(username))
                                            {
                                                FBEmailId = username+"@facebook.com";
                                            }
                                            else
                                            {
                                                FBEmailId =  singleitem1 + "@facebook.com"; ;
                                            }
                                          
                                            try
                                            {
                                                CheckDuplicats.Add(userlink, username);

                                                string CSVHeader = "ID" + "," + "Name" + ", " + "firstName" + "," + "LastName" + "," + "userlink" + "," + "userName" + "," + "gender" + "," + "Status" + "," + "EventUrl"+","+"FbEmail";
                                                string CSV_Content = singleitem1 + "," + name + ", " + firstname + "," + lastname + "," + userlink + "," + username + "," + gender + "," + status + "," + lstEventURLsFl + "," + FBEmailId;
                                                try
                                                {
                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath);
                                                    GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                                    GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                                GlobusLogHelper.log.Info("Find Userlink : " + userlink + "and Invite Status : " + status);
                                                GlobusLogHelper.log.Debug("Find Userlink : " + userlink + "and Invite Status : " + status);
                                            }
                                            catch (Exception ex)
                                            {
                                               // GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
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
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }

            GlobusLogHelper.log.Info("Process Completed With User Name : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed With User Name : " + fbUser.username);

        }

        public void ExtractEventFriendIdsEventScraperNew(ref FacebookUser fbUser)
        {
            string __user = string.Empty;
            string EventDetailsSource = string.Empty;
            string fb_dtsg = string.Empty;
            string inviterstatus = string.Empty;
            string EventId = string.Empty;
            string status = string.Empty;

            Dictionary<string, string> CheckDuplicats = new Dictionary<string, string>();
            List<string> lstMemberId = new List<string>(); 
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            foreach (string lstEventURLsFl in LstEventURLsEventScraper)
            {
                if (!lstEventURLsFl.Contains("https://www.facebook.com/events/"))
                {
                    GlobusLogHelper.log.Info("Invalid Url : " + lstEventURLsFl + "Please Upload Valid Url !");
                    GlobusLogHelper.log.Debug("Invalid Url : " + lstEventURLsFl + "Please Upload Valid Url !");
                }
                try
                {
                    string eventPageSrc = HttpHelper.getHtmlfromUrl(new Uri(lstEventURLsFl));
                    EventId = Utils.getBetween(eventPageSrc, "event\",\"uid\":", "}");
                    if (string.IsNullOrEmpty(EventId))
                    {
                        EventId = Utils.getBetween(lstEventURLsFl+"#$$#", "/events/", "#$$#").Replace("/",string.Empty).Trim(); 
                    }
                    __user = GlobusHttpHelper.GetParamValue(eventPageSrc, "user");
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(eventPageSrc, "user");
                    }

                    if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                    {
                        GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                        return;
                    }

                    try
                    {
                        status = "going";
                        #region Scrapped Members from going
                        string goingUrl1 = "https://www.facebook.com/ajax/browser/dialog/event_members/?id=" + EventId + "&edge=temporal_groups%3Amember_of_temporal_group&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=j&__rev=1561259";
                        string goingPage1 = HttpHelper.getHtmlfromUrl(new Uri(goingUrl1));
                        string[] splitsMemberData = Regex.Split(goingPage1, "_8o _8t lfloat _ohe");
                        foreach (string singleMember in splitsMemberData)
                        {
                            string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                            lstMemberId.Add(temp);
                            if (!string.IsNullOrEmpty(temp))
                            {
                                scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                            }
                        }
                        lstMemberId = lstMemberId.Distinct().ToList();
                        int i = 1;
                        int j = 100;
                        while (true)
                        {
                            string inviteUrl2 = "https://www.facebook.com/ajax/browser/list/event_members/?id=" + EventId + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + (j * i) + "&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=m&__rev=1561259";
                            string InvitePageNextSource = HttpHelper.getHtmlfromUrl(new Uri(inviteUrl2));
                            string[] splitsMemberData1 = Regex.Split(InvitePageNextSource, "_8o _8t lfloat _ohe");
                            foreach (string singleMember in splitsMemberData1)
                            {
                                string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                                lstMemberId.Add(temp);
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                                }

                            }
                            lstMemberId = lstMemberId.Distinct().ToList();
                            lstMemberId.Remove(" ");
                            i++;

                            if (!(splitsMemberData1.Length > 1))
                            {
                                break;
                            }
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                    try
                    {
                        #region Scrapped Member from MayBe
                        status = "MayBe";
                        string MayBeUrl1 = "https://www.facebook.com/ajax/browser/dialog/event_members/?id=" + EventId + "&edge=temporal_groups%3Aassociates_of_temporal_group&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=j&__rev=1561259";
                        string MayBePage1 = HttpHelper.getHtmlfromUrl(new Uri(MayBeUrl1));
                        string[] splitsMemberMayBeData = Regex.Split(MayBePage1, "_8o _8t lfloat _ohe");
                        foreach (string singleMember in splitsMemberMayBeData)
                        {
                            string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                            lstMemberId.Add(temp);
                            
                            if (!string.IsNullOrEmpty(temp))
                            {
                                scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                            }
                        }
                        lstMemberId = lstMemberId.Distinct().ToList();
                        int k = 1;
                        int l = 100;
                        while (true)
                        {
                            string MayBeUrl2 = "https://www.facebook.com/ajax/browser/list/event_members/?id=" + EventId + "&edge=temporal_groups%3Aassociates_of_temporal_group&start=" + (l * k) + "&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=m&__rev=1561259";
                            string MayBePageNextSource = HttpHelper.getHtmlfromUrl(new Uri(MayBeUrl2));
                            string[] splitsMemberData1 = Regex.Split(MayBePageNextSource, "_8o _8t lfloat _ohe");
                            foreach (string singleMember in splitsMemberData1)
                            {
                                string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                                lstMemberId.Add(temp);
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                                }
                            }
                            lstMemberId = lstMemberId.Distinct().ToList();
                            lstMemberId.Remove("");
                            k++;

                            if (!(splitsMemberData1.Length > 1))
                            {
                                break;
                            }
                        }


                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                    try
                    {
                        #region Scrapped Member from Invited
                        status = "Invited";
                        string InvitedUrl1 = "https://www.facebook.com/ajax/browser/dialog/event_members/?id=" + EventId + "&edge=temporal_groups%3Ainvitees_of_temporal_group&__asyncDialog=4&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=1d&__rev=1561259";
                        string InvitedPageSrc1 = HttpHelper.getHtmlfromUrl(new Uri(InvitedUrl1));
                        string[] splitsMemberInvitedData = Regex.Split(InvitedPageSrc1, "_8o _8t lfloat _ohe");
                        foreach (string singleMember in splitsMemberInvitedData)
                        {
                            string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                            lstMemberId.Add(temp);
                            if (!string.IsNullOrEmpty(temp))
                            {
                                scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                            }
                        }
                        lstMemberId = lstMemberId.Distinct().ToList();
                        int m = 1;
                        int n = 100;
                        while (true)
                        {
                            string MayBeUrl2 = "https://www.facebook.com/ajax/browser/list/event_members/?id=" + EventId + "&edge=temporal_groups%3Ainvitees_of_temporal_group&start=" + (n * m) + "&__user=" + __user + "&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzGxa49UJ6Kby9poW8xOdy8-&__req=1m&__rev=1561259";
                            string MayBePageNextSource = HttpHelper.getHtmlfromUrl(new Uri(MayBeUrl2));
                            string[] splitsMemberData1 = Regex.Split(MayBePageNextSource, "_8o _8t lfloat _ohe");
                            foreach (string singleMember in splitsMemberData1)
                            {
                                string temp = Utils.getBetween(singleMember, "user.php?id=", "&amp");
                                lstMemberId.Add(temp);
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    scrapMemberUsingGraphApiAndSaved(status, temp, lstEventURLsFl, ref HttpHelper);
                                }
                            }
                            lstMemberId = lstMemberId.Distinct().ToList();
                            lstMemberId.Remove("");
                            m++;

                            if (!(splitsMemberData1.Length > 1))
                            {
                                break;
                            }
                        }

                        GlobusLogHelper.log.Info("Process Completed Of Scraping Members With " + fbUser.username + " Of Event " + lstEventURLsFl);
                        GlobusLogHelper.log.Debug("Process Completed Of Scraping Members With " + fbUser.username + " Of Event " + lstEventURLsFl);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.Message);
                }
            }

        }

        Dictionary<string, string> CheckUnique = new Dictionary<string, string>();
        public void scrapMemberUsingGraphApiAndSaved(string status, string MemberId,string EventUrl,ref GlobusHttpHelper httphelper)
        {
            try
            {                
                Thread.Sleep(800);
                string Name = string.Empty;
                string FirstName = string.Empty;
                string LastName = string.Empty;
                string UserLink = string.Empty;
                string UserName = string.Empty;
                string Gender = string.Empty;
                string locale = string.Empty;
                string FBEmailId = string.Empty;

                string graphresp = httphelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + MemberId));
                if (!string.IsNullOrEmpty(graphresp))
                {
                    JObject jdata = JObject.Parse(graphresp);
                    FirstName=(string)((JValue)jdata["first_name"]);
                    LastName = (string)((JValue)jdata["last_name"]);
                    UserName = (string)((JValue)jdata["name"]);
                    Gender = (string)((JValue)jdata["gender"]);
                    locale = (string)((JValue)jdata["locale"]);    

                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    UserLink = FBGlobals.Instance.fbhomeurl + UserName;
                }
                else
                {
                 UserLink=FBGlobals.Instance.fbhomeurl+MemberId;
                }
                if (!string.IsNullOrEmpty(UserName))
                {
                    FBEmailId = UserName + "@facebook.com";
                }
                else
                {
                    FBEmailId = MemberId + "@facebook.com"; ;
                }
                try
                {
                    CheckUnique.Add(MemberId, UserName);
                string CSVHeader = "ID" + "," + "firstName" + "," + "LastName" + "," + "userlink" + "," + "userName" + "," + "gender" +","+ "locale" + "," + "Status" + "," + "EventUrl" + "," + "FbEmail";
                string CSV_Content = MemberId + ", " + FirstName + "," + LastName + "," + UserLink + "," + UserName + "," + Gender + "," +locale+","+ status + "," + EventUrl + "," + FBEmailId;
                
                    
                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath);
                    GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                    GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);


            }
        }


        public static List<string> Extractsnippet_idsFb(ref GlobusHttpHelper HttpHelper, ref string userid, ref string id, ref string status)
        {
            List<string> lstsnippet_id = new List<string>();
            List<string> lstId = new List<string>();
            try
            {

                int countsnippet_id = 0;
                int limit = 100;
                int countlimit = 0;
                int offset = 40;
                for (int lim = 0; lim < 4; lim++)
                {
                    try
                    {
                        string pagesourceofsnippet_idforLimit = string.Empty;
                        if (status.Equals("going", StringComparison.InvariantCultureIgnoreCase))
                        {
                            pagesourceofsnippet_idforLimit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + limit + "&__user=" + userid + "&__a=1"));
                        }
                        else if (status.Equals("maybe", StringComparison.InvariantCultureIgnoreCase))
                        {
                            pagesourceofsnippet_idforLimit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Aassociates_of_temporal_group&start=" + limit + "&__user=" + userid + "&__a=1"));
                        }
                        else if (status.Equals("invited", StringComparison.InvariantCultureIgnoreCase))
                        {
                            pagesourceofsnippet_idforLimit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Ainvitees_of_temporal_group&start=" + limit + "&__user=" + userid + "&__a=1"));
                        }
                        if (pagesourceofsnippet_idforLimit.Contains("/ajax\\/hovercard\\/user.php?id"))
                        {

                            string[] FindFirstPageLoadIdss = Regex.Split(pagesourceofsnippet_idforLimit, "user.php");
                            List<string> lstitem = new List<string>();

                            foreach (string eachitem in FindFirstPageLoadIdss)
                            {
                                try
                                {
                                    if (!eachitem.Contains("<!DOCTYPE html>"))
                                    {
                                        try
                                        {
                                            string FindsubstringId = eachitem.Substring(eachitem.IndexOf("id="), (eachitem.IndexOf(">", eachitem.IndexOf("id=")) - eachitem.IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Trim();
                                            if (FindsubstringId.Contains("&"))
                                            {
                                                try
                                                {
                                                    FindsubstringId = FindsubstringId.Substring(0, FindsubstringId.IndexOf("&")).Trim();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Error >>> " + ex.StackTrace);
                                                }
                                            }
                                            lstId.Add(FindsubstringId);
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

                        //else
                        //{
                        //    for (int j = 1; j < limit; j++)
                        //    {
                        //        string pagesourceofsnippet_idforoffset1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + j.ToString() + "&__user=" + userid + "&__a=1"));

                        //        if (pagesourceofsnippet_idforoffset1.Contains(""))
                        //        {
                        //            string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset1, "user.php");

                        //            for (int i = 1; i < snippet_idarr.Length; i++)
                        //            {
                        //                try
                        //                {
                        //                    string FindsubstringId = snippet_idarr[i].Substring(snippet_idarr[i].IndexOf("id="), (snippet_idarr[i].IndexOf(">", snippet_idarr[i].IndexOf("id=")) - snippet_idarr[i].IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Trim();
                        //                    lstId.Add(FindsubstringId);
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //                }
                        //            }
                        //        }
                        //        if (!pagesourceofsnippet_idforoffset1.Contains("user.php"))
                        //        {
                        //            break;
                        //        }
                        //        countlimit++;
                        //    }
                        //}
                        limit = limit + 100;
                    }
                    catch { };
                }

                //    string pagesourceofsnippet_idforoffset = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + "200" + "&__user=" + userid + "&__a=1"));
                //    if (pagesourceofsnippet_idforoffset.Contains("user.php"))
                //    {
                //        while (Extractsnippet_idsFbforOffSet(ref HttpHelper, ref userid, offset, ref id))
                //        {
                //            string pagesourceofsnippet_idforoffset3 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + offset.ToString() + "&__user=" + userid + "&__a=1"));
                //            if (pagesourceofsnippet_idforoffset3.Contains("user.php"))
                //            {
                //                string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset3, "user.php");

                //                for (int i = 1; i < snippet_idarr.Length; i++)
                //                {
                //                    try
                //                    {
                //                        string FindsubstringId = snippet_idarr[i].Substring(snippet_idarr[i].IndexOf("id="), (snippet_idarr[i].IndexOf(">", snippet_idarr[i].IndexOf("id=")) - snippet_idarr[i].IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Trim();
                //                        lstId.Add(FindsubstringId);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //                    }
                //                }
                //            }
                //            offset = offset + 20;
                //            Extractsnippet_idsFbforOffSet(ref HttpHelper, ref userid, offset, ref id);
                //        }
                //        countsnippet_id = offset - 20;
                //        for (int j = (offset - 20) + 1; j < offset; j++)
                //        {
                //            string pagesourceofsnippet_idforoffset1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + j.ToString() + "&__user=" + userid + "&__a=1"));
                //            if (pagesourceofsnippet_idforoffset1.Contains("user.php"))
                //            {
                //                string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset1, "user.php");

                //                for (int i = 1; i < snippet_idarr.Length; i++)
                //                {

                //                    try
                //                    {
                //                        string FindsubstringId = snippet_idarr[i].Substring(snippet_idarr[i].IndexOf("id="), (snippet_idarr[i].IndexOf(">", snippet_idarr[i].IndexOf("id=")) - snippet_idarr[i].IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Trim();
                //                        lstId.Add(FindsubstringId);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //                    }
                //                }
                //            }
                //            if (!pagesourceofsnippet_idforoffset1.Contains("user.php"))
                //            {
                //                break;
                //            }
                //            countsnippet_id++;
                //        }

                //        string pagesourceofsnippet_idforoffset2 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + countsnippet_id.ToString() + "&__user=" + userid + "&__a=1"));
                //        if (pagesourceofsnippet_idforoffset2.Contains("user.php"))
                //        {
                //            string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset2, "user.php");

                //            for (int i = 1; i < snippet_idarr.Length; i++)
                //            {
                //                try
                //                {
                //                    string FindsubstringId = snippet_idarr[i].Substring(snippet_idarr[i].IndexOf("id="), (snippet_idarr[i].IndexOf(">", snippet_idarr[i].IndexOf("id=")) - snippet_idarr[i].IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty).Replace("amp;", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Trim();
                //                    lstId.Add(FindsubstringId);
                //                }
                //                catch (Exception ex)
                //                {
                //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //                }
                //            }
                //        }

                //}
                lstId = lstId.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return lstId;

        }

        private static bool Extractsnippet_idsFbforOffSet(ref GlobusHttpHelper HttpHelper, ref string userid, int offset, ref string id)
        {
            try
            {
                string pagesourceofsnippet_idforoffset = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.EventInviterGetAjaxEvent_membersid + id + "&edge=temporal_groups%3Amember_of_temporal_group&start=" + offset.ToString() + "&__user=" + userid + "&__a=1"));
                if (pagesourceofsnippet_idforoffset.Contains("user.php"))
                {
                    return true;
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
            return false;
        }
    }

    public class MessageScraper
    {
        public BaseLib.Events messageScraperEvent = null;

        #region Global Variables For Event Scraper

        readonly object lockrThreadControllerMessageScraper = new object();
        public bool isStopMessageScraper = false;
        int countThreadControllerMessageScraper = 0;
        public List<Thread> lstThreadsMessageScraper = new List<Thread>();

        #endregion


        #region Property For Event Inviter

        public int NoOfThreadsMessageScraper
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
                messageScraperEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public MessageScraper()
        {
            messageScraperEvent = new BaseLib.Events();
        }

        public void StartMessageScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsMessageScraper > 0)
                {
                    numberOfAccountPatch = NoOfThreadsMessageScraper;
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
                                lock (lockrThreadControllerMessageScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerMessageScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerMessageScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsMessageScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerMessageScraper++;
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

        public void StartMultiThreadsMessageScraper(object parameters)
        {
            try
            {
                if (!isStopMessageScraper)
                {
                    try
                    {
                        lstThreadsMessageScraper.Add(Thread.CurrentThread);
                        lstThreadsMessageScraper.Distinct();
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
                                StartActionMessageScraper(ref objFacebookUser);
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
                    //if (!isStopMessageScraper)
                    {
                        lock (lockrThreadControllerMessageScraper)
                        {
                            countThreadControllerMessageScraper--;
                            Monitor.Pulse(lockrThreadControllerMessageScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionMessageScraper(ref FacebookUser fbUser)
        {
            try
            {
                ReadMsgToAllFriends(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void ReadMsgToAllFriends(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper gHttpHelpe = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Please Wait-----" + fbUser.username);
                GlobusLogHelper.log.Debug("Please Wait-----" + fbUser.username);


                string UsreId = string.Empty;

                string pageSource_Home = gHttpHelpe.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                ///CSS JS IMG Requests
                //RequestsJSCSSIMG.RequestJSCSSIMG_Chilkat(pageSource_Home, ref chilkatHttpHelper);

                string tempUserID = string.Empty;

                List<string> lstFriend = new List<string>();

                UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");

                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

                if (string.IsNullOrEmpty(UsreId) || UsreId == "0" || UsreId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);

                lstFriend.Clear();


                List<string> lstMesgnippet_idsFb = Extractsnippet_idsFb(ref fbUser, ref UsreId, pageSource_Home);

                List<string> lstMsgFBIDs = GetMessageFBID(ref fbUser, ref UsreId);

                lstMesgnippet_idsFb.AddRange(lstMsgFBIDs);

                lstMesgnippet_idsFb = lstMesgnippet_idsFb.Distinct().ToList();

                int counter = 0;
        


                foreach (var lstMesgnippet_idsFbitem in lstMesgnippet_idsFb)
                {
                    try
                    {
                        string MsgDate = "";
                        string Msg = "";
                        string MsgSenderName = "";
                        string MsgSnippedId = "";
                        string UserId = "";
                        string MsgFriendId = "";
                        string MessagingReadParticipants = "";
                        string MessagingReadParticipantsId = "";

                        //"https://www.facebook.com/messages/";//

                        string msgUrl = "https://www.facebook.com/messages/conversation-id.";//  FBGlobals.Instance.MessageScraperGetMessagesActionReadUrl;
                        string pagesourceofmsgUrl = gHttpHelpe.getHtmlfromUrl(new Uri(msgUrl + lstMesgnippet_idsFbitem));

                      //  RequestsJSCSSIMG.RequestJSCSSIMG_Chilkat(pagesourceofmsgUrl, ref chilkatHttpHelper);

                        if (pagesourceofmsgUrl.Contains("MessagingReadParticipants"))//pagesourceofmsgUrl.Contains("MessagingReadParticipants")
                        {
                            if (pagesourceofmsgUrl.Contains("MessagingReadParticipants") && pagesourceofmsgUrl.Contains("</a>"))
                            {
                                try
                                {
                                    string MessagingReadParticipantsAndId = pagesourceofmsgUrl.Substring(pagesourceofmsgUrl.IndexOf("MessagingReadParticipants"), pagesourceofmsgUrl.IndexOf("</a>", pagesourceofmsgUrl.IndexOf("MessagingReadParticipants")) - pagesourceofmsgUrl.IndexOf("MessagingReadParticipants")).Replace("MessagingReadParticipants", string.Empty).Trim();
                                    if (MessagingReadParticipantsAndId.Contains("data-hovercard=\"/ajax/hovercard/hovercard.php?id="))
                                    {
                                        try
                                        {
                                            MessagingReadParticipantsId = MessagingReadParticipantsAndId.Substring(MessagingReadParticipantsAndId.IndexOf("id="), (MessagingReadParticipantsAndId.IndexOf("\"", MessagingReadParticipantsAndId.IndexOf("id=")) - MessagingReadParticipantsAndId.IndexOf("id="))).Replace("id=", string.Empty).Trim();
                                            if (MessagingReadParticipantsAndId.Contains(">"))
                                            {
                                                try
                                                {
                                                    string strconcat = (MessagingReadParticipantsAndId + "\"").Remove(0, 2);
                                                    MessagingReadParticipants = (strconcat.Substring(strconcat.IndexOf(">"), strconcat.IndexOf("\"", strconcat.IndexOf(">")) - strconcat.IndexOf(">")).Replace(">", string.Empty).Trim());
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
                                    else
                                    {
                                        try
                                        {
                                            string strMessagingReadParticipants = pagesourceofmsgUrl.Substring(pagesourceofmsgUrl.IndexOf("MessagingReadParticipants"), 500);
                                            if (strMessagingReadParticipants.Contains(">") && strMessagingReadParticipants.Contains("</span>"))
                                            {
                                                MessagingReadParticipants = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(strMessagingReadParticipants.Substring(strMessagingReadParticipants.IndexOf(">"), strMessagingReadParticipants.IndexOf("</", strMessagingReadParticipants.IndexOf(">")) - strMessagingReadParticipants.IndexOf(">")).Replace(">", string.Empty).Trim()));

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

                        if (pagesourceofmsgUrl.Contains("webMessengerRecentMessages"))
                        {
                            try
                            {
                                ScrapMessageForMercury(ref fbUser, pagesourceofmsgUrl, lstMesgnippet_idsFbitem, UsreId, fb_dtsg);

                                continue;
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }


                        string[] msgArr = Regex.Split(pagesourceofmsgUrl, "clearfix");
                        foreach (string msgArritem in msgArr)
                        {
                            try
                            {
                                if (msgArritem.Contains("timestamp") && msgArritem.Contains("content noh") && msgArritem.Contains("<p>"))
                                {
                                    try
                                    {
                                        MsgDate = msgArritem.Substring(msgArritem.IndexOf("timestamp"), msgArritem.IndexOf("</abbr>") - msgArritem.IndexOf("timestamp")).Replace("timestamp", string.Empty).Replace("\">", string.Empty).Replace("<", string.Empty).Replace(",", ";");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (msgArritem.Contains("content noh") && msgArritem.Contains("<p>"))
                                {
                                    try
                                    {
                                        UserId = UsreId;
                                        MsgSnippedId = lstMesgnippet_idsFbitem;
                                        Msg = msgArritem.Substring(msgArritem.IndexOf("<p>"), msgArritem.IndexOf("</p>") - msgArritem.IndexOf("<p>")).Replace("<p>", string.Empty).Replace(",", ";");

                                        Msg = System.Net.WebUtility.HtmlDecode(Msg);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (msgArritem.Contains("<strong>") && msgArritem.Contains("</a>"))
                                {
                                    try
                                    {
                                        MsgSenderName = msgArritem.Substring(msgArritem.IndexOf("<strong>"), msgArritem.IndexOf("</strong>") - msgArritem.IndexOf("<strong>")).Replace("<strong>", string.Empty).Replace(",", ";");
                                        MsgFriendId = MsgSenderName.Substring(MsgSenderName.IndexOf("id="), (MsgSenderName.IndexOf("\"", MsgSenderName.IndexOf("id=")) - MsgSenderName.IndexOf("id="))).Replace("id=", string.Empty).Replace("\"", string.Empty);

                                        MsgSenderName = MsgSenderName.Substring(MsgSenderName.IndexOf(">"), MsgSenderName.IndexOf("</a>") - MsgSenderName.IndexOf(">")).Replace(">", string.Empty).Replace(",", ";");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                counter = InsertUpdateMsgsInDatabase(ref fbUser,counter, MsgDate, Msg, MsgSenderName, MsgSnippedId, UserId, MsgFriendId, MessagingReadParticipants, MessagingReadParticipantsId);
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
              
                //GlobusLogHelper.log.Info("Total Scrapped Messages = " + counter + " With User Name : " + fbUser.username);
                //GlobusLogHelper.log.Debug("Total Scrapped Messages = " + counter + " With User Name : " + fbUser.username);

                GlobusLogHelper.log.Info("Process  Completed With : " + fbUser.username);
                GlobusLogHelper.log.Debug("Process  Completed With : " + fbUser.username);          
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void ScrapMessageForMercury(ref FacebookUser fbUser, string pageSource, string msgURLId, string userId, string fb_dtsg)
        {
            try
            {
                #region MyRegion
                // Post :
                //http://www.facebook.com/ajax/mercury/thread_info.php
                //Data :
                //messages[thread_ids][id.394432847259847][offset]=0&messages[thread_ids][id.394432847259847][limit]=21&messages[thread_ids][id.111614202347306][offset]=0&messages[thread_ids][id.111614202347306][limit]=21&messages[thread_ids][id.305207196266901][offset]=0&messages[thread_ids][id.305207196266901][limit]=21&&client=web_messenger&__user=100004323278246&__a=1&__dyn=798aD5z5zsw&__req=1&fb_dtsg=AQAqGz7M&phstamp=1658165113711225577399 
                #endregion
                
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                string msgUrl = "https://www.facebook.com/messages/conversation-id.";//FBGlobals.Instance.MessageScraperGetMessagesUrl;    //"https://www.facebook.com/messages/?action=read&tid=id.";

                pageSource = gHttpHelper.getHtmlfromUrl(new Uri(msgUrl + msgURLId));

                if (string.IsNullOrEmpty(pageSource))
                {
                    Thread.Sleep(1000);
                    pageSource = gHttpHelper.getHtmlfromUrl(new Uri(msgUrl + msgURLId));
                }

                if (string.IsNullOrEmpty(pageSource))
                {
                    GlobusLogHelper.log.Info("Page Sorce Is Null On The URL : " + msgUrl + msgURLId + " With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Page Sorce Is Null On The URL : " + msgUrl + msgURLId + " With Username : " + fbUser.username);

                    return;
                }

                string[] Arr = System.Text.RegularExpressions.Regex.Split(pageSource,"thread_id");

                string threadId = string.Empty;
                foreach (var Arr_item in Arr)
                {

                    int counter=0;
                    string MsgDate=string.Empty;
                    string Msg=string.Empty;
                    string MsgSenderName=string.Empty;
                    string MsgSnippedId=string.Empty;
                    string MsgFriendId=string.Empty;
                    string MessagingReadParticipants=string.Empty;
                    string MessagingReadParticipantsId = string.Empty;
                    string UserName = string.Empty;
                    string UserId = string.Empty;
                    string tempMessagingReadParticipantsId = string.Empty;
                  
                    try
                    {
                        if (Arr_item.Contains("snippet\":\"") && !Arr_item.Contains("<!DOCTYPE html>"))
                        {
                            string message = Utils.getBetween(Arr_item, "snippet\":\"", "\",\"").Replace("/\\/", "").Replace("\\/", "");

                            Msg = message;
                            string other_user_fbid = string.Empty;

                            other_user_fbid = Utils.getBetween(Arr_item, "\"other_user_fbid\":", ",\"last_action_id\"");
                            MsgSnippedId = other_user_fbid;

                            MsgDate = Utils.getBetween(Arr_item, "timestamp_absolute", "timestamp_datetime").Replace("\"","").Replace(":","").Replace(",","");
                            string DatTme = Utils.getBetween(Arr_item, "timestamp_datetime\":\"", "timestamp_relative").Replace("\"", "").Replace(",", "");

                            MsgDate = MsgDate + " " + DatTme;
                            #region MyRegion
                            //if (MsgDate.Contains("Today"))
                            //{
                            //    MsgDate = DateTime.Now.ToString();
                            //}

                            // 
                            #endregion
                            string jsonData2 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + other_user_fbid + ""));

                            JObject Data2 = JObject.Parse(jsonData2);

                            string name2 = (string)((JValue)Data2["name"]);
                            string first_name2 = (string)((JValue)Data2["first_name"]);
                            string middle_name2 = (string)((JValue)Data2["middle_name"]);
                            string last_name2 = (string)((JValue)Data2["last_name"]);

                            MsgSenderName = name2;

                            //find username

                            string jsonData = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + userId + ""));

                            JObject Data = JObject.Parse(jsonData);

                            string name = (string)((JValue)Data["name"]);
                            string first_name = (string)((JValue)Data["first_name"]);
                            string middle_name = (string)((JValue)Data["middle_name"]);
                            string last_name = (string)((JValue)Data["last_name"]);

                            MessagingReadParticipants = name;

                            UserId = userId;

                            //find Sender Name

                            if (Arr_item.Contains(userId))
                            {

                                string jsonData1 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + userId + ""));

                                JObject Data1 = JObject.Parse(jsonData1);

                                string name1 = (string)((JValue)Data1["name"]);
                                string first_name1 = (string)((JValue)Data1["first_name"]);
                                string middle_name1 = (string)((JValue)Data1["middle_name"]);
                                string last_name1 = (string)((JValue)Data1["last_name"]);

                                MessagingReadParticipants = MessagingReadParticipants;

                                MsgFriendId = MsgSnippedId;
                            }
                            else
                            {
                                if (Arr_item.Contains(tempMessagingReadParticipantsId))
                                {

                                    string jsonData1 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + tempMessagingReadParticipantsId + ""));

                                    JObject Data1 = JObject.Parse(jsonData1);

                                    string name1 = (string)((JValue)Data1["name"]);
                                    string first_name1 = (string)((JValue)Data1["first_name"]);
                                    string middle_name1 = (string)((JValue)Data1["middle_name"]);
                                    string last_name1 = (string)((JValue)Data1["last_name"]);

                                 //   MsgSenderName = name1;

                                    MsgFriendId = tempMessagingReadParticipantsId;
                                }
                            }
                            if (Arr_item.Contains("participants"))
                            {
                                MessagingReadParticipantsId = other_user_fbid;
                            }



                            if (!string.IsNullOrEmpty(MessagingReadParticipantsId) && !string.IsNullOrEmpty(Msg) && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(MsgSnippedId) && !string.IsNullOrEmpty(MsgSenderName) && !string.IsNullOrEmpty(MessagingReadParticipants))
                            {
                                InsertUpdateMsgsInDatabase(ref fbUser, counter, MsgDate, Msg, MsgSenderName, MsgSnippedId, userId, MsgFriendId, MessagingReadParticipants, MessagingReadParticipantsId);
                             
                            }

                        }
                    }
                    catch { };
                    
                }

                #region MyRegion
                //try
                //{
                //    threadId = pageSource.Substring(pageSource.IndexOf("[{\"thread_id\":"), (pageSource.IndexOf(",", pageSource.IndexOf("[{\"thread_id\":")) - pageSource.IndexOf("[{\"thread_id\":"))).Replace("[{\"thread_id\":", string.Empty).Replace("[{\"thread_id\":", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Trim();
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //}


                //string threadIds = string.Empty;
                //try
                //{
                //    threadIds = pageSource.Substring(pageSource.IndexOf("\"thread_ids\":"), (pageSource.IndexOf("]", pageSource.IndexOf("\"thread_ids\":")) - pageSource.IndexOf("\"thread_ids\":"))).Replace("\"thread_ids\":", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("\"", string.Empty).Trim();
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //}

                //if (threadIds.Contains(",") && !string.IsNullOrEmpty(threadIds))
                //{
                //    string id1st = string.Empty;
                //    string id2nd = string.Empty;                  

                //    string[] arrthread_ids = Regex.Split(threadIds, ",");

                //    for (int i = 0; i < arrthread_ids.Length; i++)
                //    {
                //        try
                //        {
                //            if (!arrthread_ids[i].Contains(userId))
                //            {
                //                if (string.IsNullOrEmpty(id1st))
                //                {
                //                    id1st = arrthread_ids[i];
                //                    continue;
                //                }

                //                if (string.IsNullOrEmpty(id2nd))
                //                {
                //                    id2nd = arrthread_ids[i];
                //                    continue;
                //                }
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //        }
                //    }

                //    string[] threadIdsArr = Regex.Split(pageSource, "{\"thread_id\":");

                //    //Post Data 

                //    //messages[thread_ids][id.394432847259847][offset]=0&messages[thread_ids][id.394432847259847][limit]=21&messages[thread_ids][id.111614202347306][offset]=0&messages[thread_ids][id.111614202347306][limit]=21&messages[thread_ids][id.305207196266901][offset]=0&messages[thread_ids][id.305207196266901][limit]=21&messages[thread_ids][id.166655843483085][offset]=0&messages[thread_ids][id.166655843483085][limit]=21&&client=web_messenger&__user=100004323278246&__a=1&__dyn=798aD5z5zsw&__req=1&fb_dtsg=AQCdV62K&phstamp=165816710086545075501

                //    string postData = "messages[thread_ids][" + threadId + "][offset]=0&messages[thread_ids][" + threadId + "][limit]=21&messages[thread_ids][" + id1st + "][offset]=0&messages[thread_ids][" + id1st + "][limit]=21&messages[thread_ids][" + id2nd + "][offset]=0&messages[thread_ids][" + id2nd + "][limit]=21&messages[thread_ids][id." + msgURLId + "][offset]=0&messages[thread_ids][id." + msgURLId + "][limit]=21&&client=web_messenger&__user=" + userId + "&__a=1&__dyn=798aD5z5zsw&__req=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816710086545075501";



                //    string response = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadInfoUrl), postData, "");

                //    if (string.IsNullOrEmpty(response) || !response.Contains("fbid:"))
                //    {
                //        foreach (string item in threadIdsArr)
                //        {
                //            try
                //            {
                //                if (item.Contains("[\"fbid:" + msgURLId)) //["fbid:100002474680870"
                //                {
                //                    threadId = item.Substring(0, item.IndexOf(",")).Replace("\"", string.Empty).Trim();

                //                    threadId = Uri.EscapeDataString(threadId);

                //                    break;
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //            }
                //        }

                //        //messages[thread_ids][id.536559193072238][offset]=1&messages[thread_ids][id.536559193072238][limit]=20&&client=web_messenger&__user=100004192682959&__a=1&__dyn=7n8ahyj35CFIwd98&__req=i&fb_dtsg=AQDBvP7N&phstamp=165816866118805578200
                //         postData = "client=mercury&inbox[offset]=0&inbox[limit]=5&inbox[filter]&__user="+userId+"&__a=1&__dyn=7n8ajEAMNoT88DgDxyG8Eio99Esx68GAummEZ9LFwxBxembzESu48jg&__req=d&fb_dtsg="+fb_dtsg+"&ttstamp=2658171979912271105579911257&__rev=1337404";
                //      //  postData = "messages[thread_ids][" + threadId + "][offset]=1&messages[thread_ids][" + threadId + "][limit]=20&&client=web_messenger&__user=" + userId + "&__a=1&__dyn=7n8ahyj35CFIwd98&__req=i&fb_dtsg=" + fb_dtsg + "&phstamp=165816866118805578200";



                //         response = gHttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/mark_seen.php"), postData, "");
                //      //  response = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadInfoUrl), postData, "");
                //    }

                //    if (response.Contains("fbid:"))
                //    {
                //        try
                //        {
                //            string MsgDate = "";
                //            string Msg = "";
                //            string MsgSenderName = "";
                //            string MsgSnippedId = "";
                //            string UserId = "";
                //            string UserName = string.Empty;
                //            string MsgFriendId = "";
                //            string MessagingReadParticipants = "";
                //            string MessagingReadParticipantsId = "";

                //            string[] arrfbid = Regex.Split(response, "fbid:");

                //            MsgSnippedId = msgURLId;

                //            threadId = Uri.UnescapeDataString(threadId);

                //            foreach (string item in arrfbid)
                //            {
                //                try
                //                {
                //                    string tempMessagingReadParticipantsId = item.Substring(0, item.IndexOf(",")).Replace("\"", string.Empty).Trim();
                //                    if (item.Contains("body\":") && ((item.Contains("\"thread_id\":\"id." + MsgSnippedId + "\"")) || (item.Contains("{\"message_id\":\"mid."))))
                //                    {
                //                        if (!userId.Contains(tempMessagingReadParticipantsId))
                //                        {
                //                            MessagingReadParticipantsId = item.Substring(0, item.IndexOf(",")).Replace("\"", string.Empty).Trim();
                //                            break;
                //                        }
                //                    }
                //                }
                //                catch (Exception ex)
                //                {
                //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //                }
                //            }

                //            foreach (string item in arrfbid)
                //            {

                //                try
                //                {

                //                    if (item.Contains("body\":") && ((item.Contains("\"thread_id\":\"id." + MsgSnippedId + "\"")) || (item.Contains("{\"message_id\":\"mid."))))
                //                    {
                //                        string tempMessagingReadParticipantsId = item.Substring(0, item.IndexOf(",")).Replace("\"", string.Empty).Trim();

                //                        if (!userId.Contains(tempMessagingReadParticipantsId))
                //                        {
                //                            MessagingReadParticipantsId = item.Substring(0, item.IndexOf(",")).Replace("\"", string.Empty).Trim();
                //                        }

                //                        // Message read participents
                //                        string jsonData2 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + MessagingReadParticipantsId + ""));

                //                        JObject Data2 = JObject.Parse(jsonData2);

                //                        string name2 = (string)((JValue)Data2["name"]);
                //                        string first_name2 = (string)((JValue)Data2["first_name"]);
                //                        string middle_name2 = (string)((JValue)Data2["middle_name"]);
                //                        string last_name2 = (string)((JValue)Data2["last_name"]);

                //                        MessagingReadParticipants = name2;

                //                        //find username

                //                        string jsonData = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + userId + ""));

                //                        JObject Data = JObject.Parse(jsonData);

                //                        string name = (string)((JValue)Data["name"]);
                //                        string first_name = (string)((JValue)Data["first_name"]);
                //                        string middle_name = (string)((JValue)Data["middle_name"]);
                //                        string last_name = (string)((JValue)Data["last_name"]);

                //                        UserName = name;

                //                        UserId = userId;

                //                        //find Sender Name

                //                        if (item.Contains(userId))
                //                        {

                //                            string jsonData1 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + userId + ""));

                //                            JObject Data1 = JObject.Parse(jsonData1);

                //                            string name1 = (string)((JValue)Data1["name"]);
                //                            string first_name1 = (string)((JValue)Data1["first_name"]);
                //                            string middle_name1 = (string)((JValue)Data1["middle_name"]);
                //                            string last_name1 = (string)((JValue)Data1["last_name"]);

                //                            MsgSenderName = name1;

                //                            MsgFriendId = userId;
                //                        }
                //                        else
                //                        {
                //                            if (item.Contains(tempMessagingReadParticipantsId))
                //                            {

                //                                string jsonData1 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + tempMessagingReadParticipantsId + ""));

                //                                JObject Data1 = JObject.Parse(jsonData1);

                //                                string name1 = (string)((JValue)Data1["name"]);
                //                                string first_name1 = (string)((JValue)Data1["first_name"]);
                //                                string middle_name1 = (string)((JValue)Data1["middle_name"]);
                //                                string last_name1 = (string)((JValue)Data1["last_name"]);

                //                                MsgSenderName = name1;

                //                                MsgFriendId = tempMessagingReadParticipantsId;
                //                            }
                //                        }

                //                        Msg = item.Substring(item.IndexOf("body\":"), item.IndexOf("\",", item.IndexOf("body\":")) - item.IndexOf("body\":")).Replace("body\":", string.Empty).Replace("\"", string.Empty).Trim();

                //                        MsgDate = item.Substring(item.IndexOf("timestamp_absolute\":"), item.IndexOf(",", item.IndexOf("timestamp_absolute\":")) - item.IndexOf("timestamp_absolute\":")).Replace("timestamp_absolute\":", string.Empty).Replace("\"", string.Empty).Trim();
                //                    }
                //                }
                //                catch (Exception ex)
                //                {
                //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //                }

                //                int counter = 0;

                //                if (!string.IsNullOrEmpty(MessagingReadParticipantsId) && !string.IsNullOrEmpty(Msg) && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(MsgSnippedId) && !string.IsNullOrEmpty(MsgSenderName) && !string.IsNullOrEmpty(MessagingReadParticipants))
                //                {
                //                     InsertUpdateMsgsInDatabase(ref fbUser,counter, MsgDate, Msg, MsgSenderName, MsgSnippedId, userId, MsgFriendId, MessagingReadParticipants, MessagingReadParticipantsId);
                //                     Msg = "";
                //                     MsgSenderName = "";
                //                     MsgSnippedId = "";
                //                     MessagingReadParticipants = "";
                //                     MessagingReadParticipantsId = "";
                //                }
                //  }

                //   }
                //  catch (Exception ex)
                // {
                //   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                // }
                // }
                //}
                // else
                // {
                // Log("Error >>> " + ex.StackTrace);
                // } 
                #endregion

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public static List<string> Extractsnippet_idsFb(ref FacebookUser fbUser, ref string userid, string pageSource_Home)
        {
            List<string> lstsnippet_id = new List<string>();
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                int countsnippet_id = 0;
                int limit = 40;
                int countlimit = 0;
                int offset = 40;
                string pagesourceofsnippet_idforLimit = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.MessageScraperGetAjaxMessagingOffsetLimitUrl + limit + "&__a=1&__user=" + userid));
                if (pagesourceofsnippet_idforLimit.Contains("snippet_id"))
                {
                    string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforLimit, "snippet_id");
                    for (int i = 1; i < snippet_idarr.Length; i++)
                    {
                        try
                        {
                            string snippet_idarritem = snippet_idarr[i].Substring(1, 15);
                            lstsnippet_id.Add(snippet_idarritem);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                }
                else
                {
                    for (int j = 1; j < limit; j++)
                    {
                        try
                        {
                            string pagesourceofsnippet_idforoffset1 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.MessageScraperGetAjaxMessagingOffsetUrl + j.ToString() + "&__a=1&__user=" + userid));
                            if (pagesourceofsnippet_idforoffset1.Contains("snippet_id"))
                            {
                                string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset1, "snippet_id");

                                for (int i = 1; i < snippet_idarr.Length; i++)
                                {
                                    try
                                    {
                                        string snippet_idarritem = snippet_idarr[i].Substring(1, 15);
                                        lstsnippet_id.Add(snippet_idarritem);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            if (!pagesourceofsnippet_idforoffset1.Contains("snippet_id"))
                            {
                                break;
                            }
                            countlimit++;
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }


                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);

                string postData = "client=web_messenger&inbox[offset]=" + offset + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";
                string pagesourceofsnippet_idforoffset = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadListInfoUrl), postData, "");
                if (pagesourceofsnippet_idforoffset.Contains("\"thread_id\":\"id"))//  snippet_id
                {
                    while (Extractsnippet_idsFbforOffSet(ref fbUser, ref userid, offset, fb_dtsg))
                    {
                        try
                        {
                            postData = "client=web_messenger&inbox[offset]=" + offset + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";
                            pagesourceofsnippet_idforoffset = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadListInfoUrl), postData, "");


                            string pagesourceofsnippet_idforoffset3 = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.MessageScraperGetAjaxMessagingOffsetUrl + offset.ToString() + "&__a=1&__user=" + userid));
                            if (pagesourceofsnippet_idforoffset.Contains("\"thread_id\":\"id"))
                            {
                                string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset, "\"thread_id\":\"id");

                                for (int i = 1; i < snippet_idarr.Length; i++)
                                {
                                    try
                                    {
                                        string snippet_idarritem = snippet_idarr[i].Substring(1, 15);
                                        lstsnippet_id.Add(snippet_idarritem);
                                        // return lstsnippet_id;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            offset = offset + 20;
                            Extractsnippet_idsFbforOffSet(ref fbUser, ref userid, offset, fb_dtsg);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    countsnippet_id = offset - 20;

                    //postData = "client=web_messenger&inbox[offset]=" + offset + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";

                    for (int j = (offset - 20) + 1; j < offset; j++)
                    {
                        try
                        {
                            postData = "client=web_messenger&inbox[offset]=" + j + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";
                            string pagesourceofsnippet_idforoffset1 = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadListInfoUrl), postData, "");//HttpHelper.GetHtml("http://www.facebook.com/ajax/messaging/async.php?sk=inbox&offset=" + j.ToString() + "&__a=1&__user=" + userid);
                            if (pagesourceofsnippet_idforoffset1.Contains("\"thread_id\":\"id"))
                            {
                                string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset1, "\"thread_id\":\"id");

                                for (int i = 1; i < snippet_idarr.Length; i++)
                                {
                                    try
                                    {
                                        string snippet_idarritem = snippet_idarr[i].Substring(1, 15);
                                        lstsnippet_id.Add(snippet_idarritem);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            if (!pagesourceofsnippet_idforoffset1.Contains("\"thread_id\":\"id"))
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        countsnippet_id++;
                    }

                    postData = "client=web_messenger&inbox[offset]=" + offset + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";
                    string pagesourceofsnippet_idforoffset2 = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadListInfoUrl), postData, "");//HttpHelper.GetHtml("http://www.facebook.com/ajax/messaging/async.php?sk=inbox&offset=" + countsnippet_id.ToString() + "&__a=1&__user=" + userid);
                    if (pagesourceofsnippet_idforoffset2.Contains("\"thread_id\":\"id"))
                    {
                        string[] snippet_idarr = Regex.Split(pagesourceofsnippet_idforoffset2, "\"thread_id\":\"id");

                        for (int i = 1; i < snippet_idarr.Length; i++)
                        {
                            try
                            {
                                string snippet_idarritem = snippet_idarr[i].Substring(1, 15);
                                lstsnippet_id.Add(snippet_idarritem);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                }

                lstsnippet_id = lstsnippet_id.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return lstsnippet_id;
        }

        private static bool Extractsnippet_idsFbforOffSet(ref FacebookUser fbUser, ref string userid, int offset, string fb_dtsg)
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                string postData = "client=web_messenger&inbox[offset]=" + offset + "&inbox[limit]=40&inbox[filter]&__user=" + userid + "&__a=1&__dyn=7n8aD5z5CFYw&__req=a&fb_dtsg=" + fb_dtsg + "&phstamp=16581656773994897140";
                string pagesourceofsnippet_idforoffset = gHttpHelper.postFormData(new Uri(FBGlobals.Instance.MessageScraperPostAjaxMercuryThreadListInfoUrl), postData, "");
                if (pagesourceofsnippet_idforoffset.Contains("\"thread_id\":\"id"))
                {
                    return true;
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
            return false;
        }

        public static List<string> GetMessageFBID(ref FacebookUser fbUser, ref string userid)
        {
            List<string> lstFBIDs = new List<string>();

            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                string pageSource = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.MessageScraperGetMessagesUrl));

                if (string.IsNullOrEmpty(pageSource))
                {
                    Thread.Sleep(2000);

                    pageSource = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.MessageScraperGetMessagesUrl));

                    if (string.IsNullOrEmpty(pageSource))
                    {
                        return lstFBIDs;
                    }
                }

                if (pageSource.Contains("[\"fbid:"))
                {
                    string[] fbidArr = Regex.Split(pageSource, "\\[\"fbid:");

                    foreach (string item in fbidArr)
                    {
                        try
                        {
                            if (item.Contains("<!DOCTYPE html>"))
                            {
                                continue;
                            }

                            string id = item.Substring(0, item.IndexOf("\"")).Replace("\"", string.Empty).Trim();

                            if (!id.Contains(userid))
                            {
                                lstFBIDs.Add(id);
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

            lstFBIDs = lstFBIDs.Distinct().ToList();
            return lstFBIDs;
        }

        private int InsertUpdateMsgsInDatabase(ref FacebookUser fbUser,int counter, string MsgDate, string Msg, string MsgSenderName, string MsgSnippedId, string UserId, string MsgFriendId, string MessagingReadParticipants, string MessagingReadParticipantsId)
        {
            try
            {
                if (!string.IsNullOrEmpty(Msg))
                {
                    if (!string.IsNullOrEmpty(MessagingReadParticipants))
                    {
                        if (!string.IsNullOrEmpty(MsgFriendId))
                        {
                            DataSet ds=new DataSet();

                            RaiseEvent(ds, "Model : MessageRepository", "Function : GetMessageUsingUserIdNameSnippedIdSenderNameMsg", UserId, fbUser.username, MessagingReadParticipantsId, MessagingReadParticipants, MsgFriendId, MsgSnippedId, MsgSenderName, Msg, MsgDate);                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.StackTrace);
            }
            return counter;
        }
    }

    public class PageScraper
    {
    }

    public class FanPageScraper
    {
        public BaseLib.Events fanPageScraperEvent = null;

        #region Global Variables For FanPageScraper

        readonly object lockrThreadControllerFanPageScraper = new object();
        public bool isStopFanPageScraper = false;
        int countThreadControllerFanPageScraper = 0;
        public List<Thread> lstThreadsFanPageScraper = new List<Thread>();

        #endregion


        #region Property For FanPageScraper

        public int NoOfThreadsFanPageScraper
        {
            get;
            set;
        }
        public List<string> LstEventURLsFanPageScraper
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
                fanPageScraperEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public FanPageScraper()
        {
            fanPageScraperEvent = new BaseLib.Events();
        }

        public void StartFanPageScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsFanPageScraper > 0) 
                {
                    numberOfAccountPatch = NoOfThreadsFanPageScraper;
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
                                lock (lockrThreadControllerFanPageScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerFanPageScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerFanPageScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsFanPageScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerFanPageScraper++;
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

        public void StartMultiThreadsFanPageScraper(object parameters)
        {
            try
            {
                if (!isStopFanPageScraper)
                {
                    try
                    {
                        lstThreadsFanPageScraper.Add(Thread.CurrentThread);
                        lstThreadsFanPageScraper.Distinct();
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
                                StartActionFanPageScraper(ref objFacebookUser);
                                
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
                    //if (!isStopFanPageScraper)
                    {
                        lock (lockrThreadControllerFanPageScraper)
                        {
                            countThreadControllerFanPageScraper--;
                            Monitor.Pulse(lockrThreadControllerFanPageScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }        

        public void StartActionFanPageScraper(ref FacebookUser fbUser)
        {
            try
            {
                //  ExtractEventFriendIdsEventS(ref fbUser);
                //GlobusHttpHelper httpheper = fbUser.globusHttpHelper;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void FindCategoryUsingKeywords(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;
              
                GlobusLogHelper.log.Info("Logged With " + fbUser.username);
                GlobusLogHelper.log.Debug("Logged With " + fbUser.username);

                GlobusLogHelper.log.Info("Please Wait---------!");
                string Keyword = string.Empty;
                string FanpageKeyword = string.Empty;

                List<string> lstUrl = new List<string>();
                List<string> listUrl = new List<string>();
                string pageSource_Home = gHttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/home.php"));
                string tempUserID = string.Empty;
                List<string> lstFriend = new List<string>();
                #region CodeCommented
                //if (Username == selectusername)
                //{
                //    UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                //    if (string.IsNullOrEmpty(UsreId))
                //    {
                //        UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                //    }
                //    if (listkeywords.Count > 0)
                //    {
                //        if (IscrawlFanPage)
                //        {
                //            IscrawlFanPage = false;

                //            foreach (string lk in listkeywords)
                //            {
                //                if (listnegkeywords.Count > 0)
                //                {
                //                    foreach (string lnk in listnegkeywords)
                //                    {
                //                        if (!lk.Contains(lnk))
                //                        {
                //                            CrawlFanpage(ref chilkatHttpHelper, lk);

                //                        }
                //                        else
                //                        {


                //                        }
                //                    }
                //                }
                //                else
                //                {

                //                    CrawlFanpage(ref chilkatHttpHelper, lk);

                //                }
                //            }
                //            //IscrawlFanPage = false;
                //        }
                //    }
                //    else
                //    {


                //        GlobusLogHelper.log.Info("Please Load Your Keyword File !");
                //        GlobusLogHelper.log.Debug("Please Load Your Keyword File !");

                //    }
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }

    public class FriendInfoScraper
    {
        public BaseLib.Events FriendInfoScraperEvent = null;

        #region Global Variables For FriendInfoScraper

        readonly object lockrThreadControllerFriendInfoScraper = new object();
        public bool isStopFriendInfoScraper = false;
        int countThreadControllerFriendInfoScraper = 0;
        public List<Thread> lstThreadsFriendInfoScraper = new List<Thread>();

        #endregion


        #region Property For FriendInfoScraper

        public int NoOfThreadsFriendInfoScraper
        {
            get;
            set;
        }

        public static string ExportFilePathFriendInfoScraper
        {
            get;
            set;
        }

        public static string StartProcessUsingFriendInfoScraper
        {
            get;
            set;
        }

        public List<string> LstProfileURLsFriendInfoScraper
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
                FriendInfoScraperEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public FriendInfoScraper()
        {
            FriendInfoScraperEvent = new BaseLib.Events();
        }

        public void StartFriendInfoScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsFriendInfoScraper > 0)
                {
                    numberOfAccountPatch = NoOfThreadsFriendInfoScraper;
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
                                lock (lockrThreadControllerFriendInfoScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerFriendInfoScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerFriendInfoScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsFriendInfoScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerFriendInfoScraper++;
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

        public void StartMultiThreadsFriendInfoScraper(object parameters)
        {
            try
            {
                if (!isStopFriendInfoScraper)
                {
                    try
                    {
                        lstThreadsFriendInfoScraper.Add(Thread.CurrentThread);
                        lstThreadsFriendInfoScraper.Distinct();
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
                                StartActionFriendInfoScraper(ref objFacebookUser);

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
                  //  if (!isStopFriendInfoScraper)
                    {
                        lock (lockrThreadControllerFriendInfoScraper)
                        {
                            countThreadControllerFriendInfoScraper--;
                            Monitor.Pulse(lockrThreadControllerFriendInfoScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionFriendInfoScraper(ref FacebookUser fbUser)
        {
            try
            {
                if (StartProcessUsingFriendInfoScraper == "Own Friends Info")
                {
                    
                    FriendProfileIdExtractor(ref fbUser);
                }
                if (StartProcessUsingFriendInfoScraper == "Friend Of Friends Info")
                {
                    FriendOfFriendsProfileIdExtractor(ref fbUser); 
                }
                if (StartProcessUsingFriendInfoScraper == "FB Emails Scraper")
                {
                    EmailsIdExtractor(ref fbUser);

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void FriendProfileIdExtractor(ref FacebookUser fbUser)//ref ChilkatHttpHelpr chilkatHttpHelper
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                foreach (string item in LstProfileURLsFriendInfoScraper)//listFriendProfileExtracter
                {
                    try
                    {
                        string UserId = string.Empty;

                        string pageSource_HomePage = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                        string fb_dtsg = string.Empty;
                        UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);
                        int countFriend = 0;
                        countFriend = ExtractFriendCount(ref fbUser,ref gHttpHelper, UserId);

                        GlobusLogHelper.log.Debug("Please  wait ..Getting  the Friends FB Id..");
                        GlobusLogHelper.log.Info("Please  wait ..Getting  the Friends FB Id..");

                        List<string> lstFriends = ExtractFriendIdsFb(ref fbUser, ref gHttpHelper, ref UserId, countFriend);
                        lstFriends = lstFriends.Distinct().ToList();

                         //List<string> lstOwnFriendId = GetFirendId(ref fbUser, ref gHttpHelper, UserId, item);
                        

                         //lstFriends = lstFriends.Concat(lstOwnFriendId).ToList();
                         //lstFriends = lstFriends.Distinct().ToList();


                        ExtractFriendsInformation(ref fbUser, ref gHttpHelper, lstFriends);

                        #region MyRegion
                        //lstOwnFriendId.AddRange(lstFriends);
                        //lstOwnFriendId = lstOwnFriendId.Distinct().ToList();
                        ////FriendRequestSender.OwnFriendsExtractor obj = new OwnFriendsExtractor();
                        //string posturl = FBGlobals.Instance.fbAllFriendsUrl;
                        //string PostdataForFriends = "uid=" + UserId + "&infinitescroll=1&location=friends_tab_tl&start=14&nctr[_mod]=pagelet_friends&__user=" + UserId + "&__a=1&__req=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816811649895156150";
                        //string response = gHttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");
                        //if (string.IsNullOrEmpty(response))
                        //{
                        //    posturl = FBGlobals.Instance.fbAllFriendsUrl;
                        //    response = gHttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");

                        //}
                        //string[] Friends = Regex.Split(response, "user.php");
                        //List<string> lstnewfriendid = new List<string>();
                        //foreach (string iditem in Friends)
                        //{
                        //    try
                        //    {
                        //        if (!iditem.Contains("<!DOCTYPE html>") && !iditem.Contains("for (;;)"))
                        //        {
                        //            try
                        //            {
                        //                string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();

                        //                if (FriendIDS.Contains("&amp;"))
                        //                {
                        //                    FriendIDS = FriendIDS.Substring(0, (iditem.IndexOf("&amp;"))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("&amp;", string.Empty).Replace("&amp", string.Empty).Replace(";", string.Empty).Trim();
                        //                }

                        //                lstnewfriendid.Add(FriendIDS);
                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //            }
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //    }
                        //}
                        //try
                        //{
                        //    List<string> lsttotalajaxid = FriendsTotalId(ref fbUser, ref gHttpHelper, UserId, UserId);
                        //    lstOwnFriendId.AddRange(lstnewfriendid);
                        //    lstOwnFriendId.AddRange(lsttotalajaxid);
                        //    lstOwnFriendId = lstOwnFriendId.Distinct().ToList();
                        //    //LoggerFriendProfileUrl("TotalId : " + lstOwnFriendId.Count);
                        //    ExtractFriendsInformation(ref fbUser, ref gHttpHelper, lstOwnFriendId);
                        //    // break;
                        //}
                        //catch (Exception ex)
                        //{
                        //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //}
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
        }

        public void ExtractFriendsInformation(ref FacebookUser fbUser,ref GlobusHttpHelper gHttpHelper, List<string> lstFriendsids)
        {
            
            GlobusLogHelper.log.Info("Start Scraping Profile Information  With Username : "+fbUser.username);
            GlobusLogHelper.log.Debug("Start Scraping Profile Information  With Username : " + fbUser.username);

            lstFriendsids = lstFriendsids.Distinct().ToList();
            
            foreach (string listFriendIditem in lstFriendsids)
            {
                try
                {
                    //Log("Hello");
                    string Urls = string.Empty;
                    string id = string.Empty;
                    string name = string.Empty;
                    string first_name = string.Empty;
                    string last_name = string.Empty;
                    string link = string.Empty;
                    string gender = string.Empty;
                    string locale = string.Empty;
                    string UserName12 = string.Empty;

                    Urls = FBGlobals.Instance.fbgraphUrl + listFriendIditem + "/";                    // "https://graph.facebook.com/"
                    string pageSrc = gHttpHelper.getHtmlfromUrl(new Uri(Urls));


                    if (pageSrc.Contains("id"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("id"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            id = ArrTemp[2];                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            name = ArrTemp[2];    
                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("first_name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("first_name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            first_name = ArrTemp[2];                           
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("last_name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("last_name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            last_name = ArrTemp[2];                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("link"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("link"), 95);
                            string[] ArrTemp = supsstring.Split('"');
                            link = ArrTemp[2];                            

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    
                    if (pageSrc.Contains("gender"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("gender"));
                            string[] ArrTemp = supsstring.Split('"');
                            gender = ArrTemp[2];
                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("locale"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("locale"));
                            string[] ArrTemp = supsstring.Split('"');
                            locale = ArrTemp[2];
                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    string UserName = string.Empty;
                    if (pageSrc.Contains("username"))
                    {
                        try
                        {
                            string UserName1 = pageSrc.Substring(pageSrc.IndexOf("username"));
                            string[] ArrTemp = UserName1.Split('"');
                            UserName = ArrTemp[2];

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (string.IsNullOrEmpty(link))
                    {
                        link = FBGlobals.Instance.fbhomeurl + UserName;    // "http://www.facebook.com/"

                    }
                    try
                    {

                        string FBEmailId = string.Empty;
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            FBEmailId = UserName + "@facebook.com";
                        }
                        else
                        {
                            FBEmailId = UserName + "@facebook.com"; ;
                        }


                        string ownprofileUrl = FBGlobals.Instance.fbProfileUrl + listFriendIditem + "/about";// + "&sk=info";

                        string profileUrl = FBGlobals.Instance.fbhomeurl + UserName + "/about";


                        string pagesourceofProfileUrl = gHttpHelper.getHtmlfromUrl(new Uri(profileUrl)); 

                        
                        string _ExprotFilePath1 = string.Empty;
                     //   string Currentlocation = "";                  


                        if (pagesourceofProfileUrl.Contains("fbTimelineSummarySectionWrapper") || ((pagesourceofProfileUrl.Contains("Work and Education") || pagesourceofProfileUrl.Contains("Work and education")) || pagesourceofProfileUrl.Contains("Living") || pagesourceofProfileUrl.Contains("Basic Information") || pagesourceofProfileUrl.Contains("Contact Information")))
                        {
                            //foreach (var hrefArr1item in hrefArr1)
                            {
                                try
                                {
                                    //if (hrefArr1item.Contains("/info"))
                                    {                                       
                                     //   string infohref = "http://www.facebook.com/yazhima98/info";

                                        string infopagesource = pagesourceofProfileUrl;
                                      
                                        string birthday = "";                                       
                                        string language = "";                                       
                                        string website = "";
                                        string email = "";
                                        string location = "";
                                        string jobposition = "";
                                        string jobcompany = "";
                                        string Mobile_Phones = "";
                                        string University = "";
                                        string Secondaryschool = "";
                                        string Hometown = "";
                                      
                                        string HighSchools = string.Empty;
                                        string Colleges = string.Empty;
                                        string Employers = string.Empty;
                                        string CurrentCitys = string.Empty;
                                        string Hometowns = string.Empty;
                                        List<string> kkk = gHttpHelper.GetHrefsByTagAndAttributeName(infopagesource, "span", "fwb");

                                        if (infopagesource.Contains(">High School<"))
                                        {
                                            try
                                            {
                                                string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                                                string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                                                List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                foreach (string item in lsttd)
                                                {
                                                    try
                                                    {
                                                        HighSchools = HighSchools + ":" + item;
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
                                        if (infopagesource.Contains(">Employers<"))
                                        {
                                            try
                                            {
                                                string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Employers<"), 1200);
                                                string[] ArrHighschool = Regex.Split(infopagesource1, ">Employers<");
                                                string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                foreach (string item in lsttd)
                                                {
                                                    try
                                                    {
                                                        Employers = Employers + ":" + item;

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                string HighSchool = HS[0];
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        if (pagesourceofProfileUrl.Contains("Lives"))
                                        {
                                            string[] Arr = System.Text.RegularExpressions.Regex.Split(pagesourceofProfileUrl, "Lives in");
                                            CurrentCitys = Utils.getBetween(Arr[1], "\">", "</a>").Replace(",", "");
                                        }
                                        if (pagesourceofProfileUrl.Contains("fsm fwn fcg") && pagesourceofProfileUrl.Contains("From"))
                                        {
                                            string[] Arr = System.Text.RegularExpressions.Regex.Split(pagesourceofProfileUrl, "fsm fwn fcg");
                                            foreach (var Arr_item in Arr)
                                            {
                                                try
                                                {
                                                    if (!Arr_item.Contains("<!DOCTYPE html>"))
                                                    {
                                                        if (Arr_item.Contains("><span>From <a href="))
                                                        {
                                                            string town = Utils.getBetween(Arr_item, "><span>From", "</a></span>").Replace(",", "") + "@@@@";
                                                            Hometown = Utils.getBetween(town, "\">", "@@@@").Replace(",", "");

                                                        }
                                                      
                                                    }
                                                }
                                                catch { };
                                            }
                                            if (pagesourceofProfileUrl.Contains("data-overviewsection=\"edu_work\">") && !pagesourceofProfileUrl.Contains("Ask for Evans's high school"))
                                            {
                                                string[] Arr2 = System.Text.RegularExpressions.Regex.Split(pagesourceofProfileUrl, "_c24 _50f4\">");
                                                foreach (var Arr2_item in Arr2)
                                                {
                                                    if (!Arr2_item.Contains("<!DOCTYPE html>") && !Arr2_item.Contains("Ask for Evans's high school"))
                                                    {
                                                        try
                                                        {
                                                            if (Arr2_item.Contains("Works at <a class="))
                                                            {
                                                                Employers = Utils.getBetween(Arr2_item, "\">", "</a></div>").Replace(",", "");
                                                                if (Employers.Contains("</a> and <a class=\""))
                                                                {
                                                                    string[] arr = System.Text.RegularExpressions.Regex.Split(Employers, "</a> and <a class");
                                                                    Employers = arr[0];
                                                                }
                                                            }
                                                            if (Arr2_item.Contains("Lives in <a"))
                                                            {
                                                                if (string.IsNullOrEmpty(CurrentCitys))
                                                                {
                                                                    CurrentCitys = Utils.getBetween(Arr2_item, "\">", "</a></div>").Replace(",", "");
                                                                }
                                                               
                                                            }
                                                            if (Arr2_item.Contains(" high school") && !Arr2_item.Contains("<div data-pnref=\"rel\">") || (Arr2_item.Contains("Went to <a class=") && Arr2_item.Contains("School") && !Arr2_item.Contains("<div data-pnref=\"rel\">")))
                                                            {
                                                                HighSchools = Utils.getBetween(Arr2_item, "\">", "</a></div>").Replace(",", "");
                                                                if (HighSchools.Contains("</a> and <a class=\""))
                                                                {
                                                                   string [] arr=System.Text.RegularExpressions.Regex.Split(HighSchools,"</a> and <a class"); 
                                                                    HighSchools=arr[0];
                                                                }
                                                            }

                                                           
                                                        }
                                                        catch { };
                                                    }
                                                }
                                            }

                                            //CurrentCitys = Utils.getBetween(Arr[1], "\">", "</a>").Replace(",", "");
                                        }

                                        if (infopagesource.Contains(">College<"))
                                        {
                                            try
                                            {
                                                string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">College<"), 1200);
                                                string[] ArrHighschool = Regex.Split(infopagesource1, ">College<");
                                                string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                foreach (string item in lsttd)
                                                {
                                                    try
                                                    {
                                                        Colleges = Colleges + ":" + item;

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                // string HighSchool = HS[0];

                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }

                                        if (infopagesource.Contains(">Secondary school<"))
                                        {
                                            try
                                            {
                                                string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Secondary school<"), 1200);
                                                string[] ArrHighschool = Regex.Split(infopagesource1, ">Secondary school<");
                                                string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                foreach (string item in lsttd)
                                                {
                                                    try
                                                    {
                                                        Secondaryschool = Secondaryschool + ":" + item;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                //string HighSchool = HS[0];

                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }

                                        if (infopagesource.Contains(">University<"))
                                        {
                                            try
                                            {
                                                string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">University<"), 1200);
                                                string[] ArrHighschool = Regex.Split(infopagesource1, ">University<");
                                                string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                foreach (string item in lsttd)
                                                {
                                                    try
                                                    {
                                                        University = University + ":" + item;
                                                        GlobusLogHelper.log.Info("Find University " + University);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                //string HighSchool = HS[0];

                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }
                                        //Secondary school
                                        if (infopagesource.Contains(">Living<"))   //Current City
                                        {
                                            try
                                            {
                                                // string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Living<"));
                                                string[] ArrHighschool = Regex.Split(infopagesource, ">Living<");
                                                string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");

                                                foreach (string item in ArrHighSchool1)
                                                {
                                                    try
                                                    {
                                                        if (item.Contains("Current City") || item.Contains("Current location"))
                                                        {
                                                            try
                                                            {
                                                                string[] ARRCurrentCity = Regex.Split(item, "Current City");
                                                                List<string> lsttd = gHttpHelper.GetDataTag(ARRCurrentCity[0], "a");
                                                                foreach (string item1 in lsttd)
                                                                {
                                                                    try
                                                                    {
                                                                        CurrentCitys = CurrentCitys + ":" + item1;
                                                                        GlobusLogHelper.log.Info("Find Current City " + CurrentCitys);
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
                                                        if (item.Contains("fsm fwn fcg"))
                                                        {
                                                            try
                                                            {
                                                                string[] ARRHometown = Regex.Split(item, "Hometown");
                                                                List<string> lsttd = gHttpHelper.GetDataTag(ARRHometown[0], "a");
                                                                foreach (string item1 in lsttd)
                                                                {
                                                                    try
                                                                    {
                                                                        Hometowns = Hometowns + ":" + item1;
                                                                       
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
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        if (infopagesource.Contains(">Living<") || infopagesource.Contains("Places Lived"))   //Current City
                                        {                                           

                                            try
                                            {                                                
                                                string [] Home =System.Text.RegularExpressions.Regex.Split(infopagesource, "<div class=\"fsl fwb fcb\">");
                                                Hometown = Home[0];
                                                foreach (var Home_item in Home)
                                                {
                                                    try
                                                    {
                                                        if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Current City"))
                                                        {
                                                            List<string> CCity = gHttpHelper.GetDataTag(Home_item, "a");
                                                            CurrentCitys = CCity[0];
                                                            GlobusLogHelper.log.Info("Find Current City " + CurrentCitys);
                                                        }
                                                        if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Hometown"))
                                                        {
                                                            List<string> Ht= gHttpHelper.GetDataTag(Home_item, "a");
                                                            Hometowns = Ht[0];
                                                            Hometown = Ht[0];
                                                            GlobusLogHelper.log.Info("Found Hometown " + Hometown);
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

                                        try
                                        {
                                            string[] contactinfoArr = Regex.Split(infopagesource, "uiHeader uiHeaderWithImage fbTimelineAboutMeHeader");
                                            if (contactinfoArr.Count() >= 2)
                                            {
                                                List<string> lstcontactinfoArrtd = gHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                                                foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                                {
                                                    try
                                                    {
                                                        if (lstcontactinfoArrtditem.Contains("Employers"))
                                                        {
                                                            Employers = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers", string.Empty).Replace(",", ";").Trim());
                                                            if (Employers.Contains("</a> and <a class=\""))
                                                            {
                                                                string[] arr = System.Text.RegularExpressions.Regex.Split(Employers, "</a> and <a class");
                                                                HighSchools = arr[0];
                                                            }
                                                            GlobusLogHelper.log.Info("Found Colleges " + Colleges);
                                                        }

                                                        if (lstcontactinfoArrtditem.Contains("College"))
                                                        {
                                                            Colleges = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("College", string.Empty).Replace(",", ";").Trim());
                                                            GlobusLogHelper.log.Info("Found Colleges " + Colleges);
                                                        }

                                                        if (lstcontactinfoArrtditem.Contains("High School"))
                                                        {
                                                            HighSchools = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("High School", string.Empty).Replace(",", ";").Trim());
                                                            GlobusLogHelper.log.Info("Found High Schools " + HighSchools);
                                                        }



                                                        if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                                        {
                                                            Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());
                                                            GlobusLogHelper.log.Info("Found Mobile Number " + Mobile_Phones);
                                                        }
                                                        if (lstcontactinfoArrtditem.Contains("Address"))
                                                        {
                                                            location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());
                                                            GlobusLogHelper.log.Info("Found location " + location);
                                                        }
                                                        if (lstcontactinfoArrtditem.Contains("Email"))
                                                        {
                                                            email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                            string[] emailArr1 = Regex.Split(email, " ");
                                                            if (emailArr1.Count() >= 2)
                                                            {
                                                                email = emailArr1[1] + emailArr1[0];
                                                                GlobusLogHelper.log.Info("Found Email " + email);
                                                               
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
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        try
                                        {
                                            if (pagesourceofProfileUrl.Contains("span class=\"_c24 _50f3\">"))
                                            {
                                                string[] attt = System.Text.RegularExpressions.Regex.Split(pagesourceofProfileUrl,"<li class=\"_4tnv _2pif\">");
                                                foreach (var attt_item in attt)
                                                {
                                                    if (attt_item.Contains("<span class=\"_c24 _50f3\"><div>"))
                                                    {
                                                        string getTagHtml = Utils.getBetween(attt_item, "<span class=\"_c24 _50f3\"><div>", "</div></span></div>");
                                                        birthday = getTagHtml.Replace(",", "-");
                                                        if (birthday.Contains("span") || birthday.Contains("Class") || birthday.Contains("\"") || birthday.Contains("li"))
                                                        {
                                                            birthday = "";
                                                        }
                                                        if (attt_item.Contains("gcUeztYmfn9.png")&&attt_item.Contains("<span dir=\"ltr\">"))
                                                        {
                                                            string getTagHtm = Utils.getBetween(attt_item, "<span class=\"_c24 _50f3\"><div>", "</div></span></div>");
                                                            Mobile_Phones = Utils.getBetween(attt_item, "<span dir=\"ltr\">","</span>");
                                                        }
                                                        if (attt_item.Contains("Phones</span>"))
                                                        {
                                                             Mobile_Phones = Utils.getBetween(attt_item, "<span dir=\"ltr\">","</span>");
                                                        }
                                                        if (attt_item.Contains("https://fbstatic-a.akamaihd.net/rsrc.php/v2/yc/r/l8RlKxR65X7.png"))
                                                        {
                                                             string getTagHtml1 = Utils.getBetween(attt_item, "<span class=\"_c24 _50f3\"><div>", "</div></span></div>");
                                                      
                                                      
                                                        }
                                                    }                                                  
                                                }
                                                GlobusLogHelper.log.Info("Found Date Of birth" + birthday);

                                             
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        if (pagesourceofProfileUrl.Contains("contactInfoPhone"))
                                        {
                                            string Phone = Utils.getBetween(pagesourceofProfileUrl,"Contact Information","Address");
                                            string[] arr = System.Text.RegularExpressions.Regex.Split(Phone,"<span dir=");
                                            if (arr.Count()==3)
                                            {
                                                try
                                                {
                                                    string s1 = Utils.getBetween(arr[1], "\"ltr\">", "</span>");
                                                    string s2 = Utils.getBetween(arr[2], "\"ltr\">", "</span>");
                                                    Mobile_Phones = s1 + "-" + s2;
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
                                                    string s1 = Utils.getBetween(arr[1], "\"ltr\">", "</span>");
                                                    Mobile_Phones = s1;
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }
                                          
                                        }
                                        if (id.Contains("www.facebook.com"))
                                        {
                                            
                                        }

                                        if (!string.IsNullOrEmpty(ExportFilePathFriendInfoScraper))
                                        {
                                            try
                                            {
                                                string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;

                                                #region MyRegion                                                
                                              
                                                //string CSVHeader = "Name" + "," + "Birthday" + ", " + "Gender" + "," + "Mobile_Phones" + "," + "Email" + "," + "Location" + "," + "JobPosition" + "," + "JobCompany" + "," + "URL" + "," + "Used Account" + "," + "Hometown" + "," + "CurrentLocation" + "," + "University" + "," + "SecondrySchool";
                                               
                                               //string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + CurrentCitys.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + Colleges.Replace(",", " ") + "," + email.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;

                                               // string CSVHeader = "ExtractUrl" + "," + "ProfileUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";
                                                //string CSV_Content = ownprofileUrl.Replace(",", " ") + "," + profileUrl.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + CurrentCitys.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + Colleges.Replace(",", " ") + "," + email.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ");
                                                #endregion

                                                string CSVHeader = "ProfileUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";
                                                string CSV_Content = profileUrl.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + CurrentCitys.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + Colleges.Replace(",", " ") + "," + FBEmailId.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;
                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFriendInfoScraper);
                                                if (CSV_Content.Contains("</a> and"))
                                                {
                                                    
                                                }
                                                GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                                GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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

                        else
                        {
                            if (!string.IsNullOrEmpty(ExportFilePathFriendInfoScraper))
                            {
                                try
                                {
                                    string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;                                    
                                    string CSVHeader = "ExtractUrl" + "," + "ProfileUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";                                    

                                    string CSV_Content = Urls.Replace(",", " ") + "," + profileUrl.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + ", " + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;

                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFriendInfoScraper);
                                    GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                    GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }

            GlobusLogHelper.log.Info("Process Completed Of Scraping Own Profile Information !");
            GlobusLogHelper.log.Debug("Process Completed Of Scraping Own Profile Information !");
        }

        private List<string> FriendsTotalId(ref FacebookUser fbUser, ref GlobusHttpHelper gHttpHelper, string Id, string user)
        {
            List<string> toatalid = new List<string>();
            try
            {
                int offset = 38;
                int countlimit = 0;
                int countKeyword_id = 0;
                int __req = 1;
                int __adt = 1;


                while (true)
                {
                    if (__adt < 50)
                    {
                        string extractkeywordslimit = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbAllFriendsUIdUrl + Id + "&infinitescroll=1&location=friends_tab_tl&start=" + offset + "&__user=" + user + "&__a=1"));

                        if (!extractkeywordslimit.Contains("user.php"))
                        {
                            extractkeywordslimit = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbAllFriendsUIdUrl + Id + "&infinitescroll=1&location=friends_tab_tl&start=" + offset + "&__user=" + user + "&__a=1"));
                        }

                        if (extractkeywordslimit.Contains("user.php"))
                        {
                            string[] keyword_idarr = Regex.Split(extractkeywordslimit, "user.php");
                            for (int i = 1; i < keyword_idarr.Length; i++)
                            {
                                try
                                {
                                    string keyword_idarritem = keyword_idarr[i].Substring(keyword_idarr[i].IndexOf("id="), keyword_idarr[i].IndexOf(">") - keyword_idarr[i].IndexOf("id=")).Replace("id=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty);

                                    if (keyword_idarritem.Contains("&"))
                                    {
                                        keyword_idarritem = keyword_idarritem.Substring(0, keyword_idarritem.IndexOf("&")).Trim();
                                    }

                                    toatalid.Add(keyword_idarritem);
                                    toatalid = toatalid.Distinct().ToList();                                    
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            offset = offset + 24;
                            __req++;
                            __adt++;

                        }
                        else
                        {
                            break;
                        }
                    }

                    else
                    {
                        break;
                    }
                }
                //  #region

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return toatalid;
        }

        private List<string> GetFirendId(ref FacebookUser fbUser, ref BaseLib.GlobusHttpHelper HttpHelper, string UId, string profileurl)
        {
            List<string> lstfriendid = new List<string>();
            try
            {
                
                string UserId = UId;
                string GraphApiSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + UserId));
                string grpusername = string.Empty;
                if (GraphApiSource.Contains("username"))
                {
                    try
                    {
                        string supsstring = GraphApiSource.Substring(GraphApiSource.IndexOf("username"), 30);
                        string[] ArrTemp = supsstring.Split('"');
                        grpusername = ArrTemp[2];
                      
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string FriendlistUrl = string.Empty;
                if (!string.IsNullOrEmpty(grpusername))
                {
                    FriendlistUrl = FBGlobals.Instance.fbhomeurl + grpusername + "/friends?ft_ref=mni";
                    if (FriendlistUrl.Contains("//"))
                    {
                        // FriendlistUrl = FriendlistUrl.Replace("//", "/").Trim();
                        FriendlistUrl = FriendlistUrl.Replace("//friends?", "/friends?").Trim();
                    }
                }
                else
                {
                    FriendlistUrl = profileurl + "/friends?ft_ref=mni";
                    if (FriendlistUrl.Contains("//"))
                    {
                        FriendlistUrl = FriendlistUrl.Replace("//friends?", "/friends?").Trim();
                    }
                }
                string FriendsResponse = string.Empty;
                try
                {
                    FriendsResponse = HttpHelper.getHtmlfromUrl(new Uri(FriendlistUrl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(FriendsResponse))
                {
                    try
                    {
                        //FriendlistUrl = FriendlistUrl.Replace("http", "https");
                        //FriendlistUrl = FriendlistUrl.Replace("httpss", "https");
                        FriendsResponse = HttpHelper.getHtmlfromUrl(new Uri(FriendlistUrl));
                        
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string[] Friends = Regex.Split(FriendsResponse, "user.php");

                foreach (string iditem in Friends)
                {
                    try
                    {
                        if (!iditem.Contains("<!DOCTYPE html>"))
                        {
                            try
                             {
                                string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();

                                if (FriendIDS.Contains("&"))
                                {
                                    FriendIDS = FriendIDS.Substring(0, FriendIDS.IndexOf("&")).Trim();
                                }

                                lstfriendid.Add(FriendIDS);
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
            return lstfriendid;
        }

        public static List<string> ExtractFriendIdsFb(ref FacebookUser fbUser, ref BaseLib.GlobusHttpHelper HttpHelper, ref string userID, int FriendCount)
        {
            GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;
           
            List<string> lstFriendTemp = new List<string>();
            try
            {
                
                int i = 0;
                do
                {                    

                    string FriendUrl = FBGlobals.Instance.fbAllFriendsUIdUrl + userID + "&infinitescroll=1&location=friends_tab_tl&start=" + i + "&__user=" + userID + "&__a=1";

                    string pageScrFriend = HttpHelper.getHtmlfromUrl(new Uri(FriendUrl));
                    if (string.IsNullOrEmpty(pageScrFriend))
                    {
                        FriendUrl = FBGlobals.Instance.fbAllFriendsUIdUrl + userID + "&infinitescroll=1&location=friends_tab&start=" + i + "&__a=1&__user=" + userID;
                    }
                    pageScrFriend = HttpHelper.getHtmlfromUrl(new Uri(FriendUrl));
                    if (pageScrFriend.Contains("user.php?"))
                    {
                        string[] arr = Regex.Split(pageScrFriend, "user.php");
                        foreach (string strhref in arr)
                        {
                            try
                            {
                                string subStr = strhref.Substring(0, 100);
                                if (subStr.Contains("id"))
                                {

                                    string[] arrTemp = subStr.Split('=');
                                    string Temp = arrTemp[1];

                                    string[] arrTemp1 = Temp.Split('"');
                                    string Temp1 = arrTemp1[0].Replace("\\", "");

                                    if (Temp1.Contains("&amp;"))
                                    {
                                        Temp1 = Temp1.Substring(0, Temp1.IndexOf("&amp;")).Trim();
                                    }

                                    lstFriendTemp.Add(Temp1);
                                    lstFriendTemp.Distinct().ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                    if (i < 39)
                    {

                        i = 39;
                    }
                    else
                    {
                        i = i + 16;
                    }
                } while (i < FriendCount);
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstFriendTemp;
        }

        public static int ExtractFriendCount(ref FacebookUser fbUser, ref BaseLib.GlobusHttpHelper HttpHelper, string UserId)
        {

            int FriendCountNumber = 0;
            try
            {

                string url = FBGlobals.Instance.fbProfileUrl + UserId;
                string PageSrcFriendCountOLd = HttpHelper.getHtmlfromUrl(new Uri(url));
                string[] arr = System.Text.RegularExpressions.Regex.Split(PageSrcFriendCountOLd,"data-medley-id");
                foreach (var arr_item in arr)
                {
                    try
                    {
                        //if (!arr_item.Contains("<!DOCTYPE html>")&&arr_item.Contains("Friends"))
                        {
                            string Count = Utils.getBetween(arr_item,"<span class=\"_gs6\">","</span>");
                            Count = Count.Replace(",", string.Empty);
                            FriendCountNumber = Convert.ToInt32(Count);
                            break;
                        }
                    }
                    catch { };                    
                }

                Regex NumChk = new Regex("^[0-9]*$");
                if (PageSrcFriendCountOLd.Contains("Friends ("))
                {
                    //** Friend Count old way *************************************** 

                    string FriendCount = string.Empty;
                    string[] ArrFrdProfile = Regex.Split(PageSrcFriendCountOLd, "v=friends");
                    string strSub = ArrFrdProfile[1].Substring(0, 20);
                    string[] ArrTe = strSub.Split('(');
                    string[] ArrTe1 = ArrTe[1].Split(')');
                    FriendCount = ArrTe1[0];

                    if (!string.IsNullOrEmpty(FriendCount) && NumChk.IsMatch(FriendCount))
                    {
                        FriendCountNumber = int.Parse(FriendCount);
                    }
                    return FriendCountNumber;
                }
                else
                {

                    //** Friend Count ***************************************

                    string FriendCount = string.Empty;
                    string PageSrcFriendCount = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=friends"));

                    if (PageSrcFriendCount.Contains("fsxl"))
                    {
                        string[] arrFriendCount = Regex.Split(PageSrcFriendCount, "fsxl");
                        string strTempFriCou = arrFriendCount[1];
                        strTempFriCou = strTempFriCou.Substring(0, 20);
                        string[] arrTempFri = strTempFriCou.Split('>');
                        string[] arrTempFri1 = arrTempFri[1].Split('<');
                        FriendCount = arrTempFri1[0];

                        if (!string.IsNullOrEmpty(FriendCount) && NumChk.IsMatch(FriendCount))
                        {
                            FriendCountNumber = int.Parse(FriendCount);
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return FriendCountNumber;
        }

        public void EmailsIdExtractor(ref FacebookUser fbUser)//ref ChilkatHttpHelpr chilkatHttpHelper
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                foreach (string item in LstProfileURLsFriendInfoScraper)
                {
                    try
                    {
                        string UserId = string.Empty;
                        string pageSource_HomePage = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                        string fb_dtsg = string.Empty;
                        UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

                        string ProfileId = FBGlobals.Instance.fbhomeurl + item;

                        string pageSource_ProfileId = gHttpHelper.getHtmlfromUrl(new Uri(ProfileId));

                        string PostData = "field_type=email&nctr[_mod]=pagelet_contact&__user=" + UserId + "&__a=1&__dyn=7n88QW9t2qmvudDgDxyKBgWDBzECiq78hACF299qzCC-C26m6oKeDBDx2ubhHximmey8qUS&__req=k&fb_dtsg=" + fb_dtsg + "&ttstamp=2658171521057178828310510879&__rev=1476421";
                        string PostUrl = "https://www.facebook.com/profile/edit/infotab/forms/";
                        string PostResponce = gHttpHelper.postFormData(new Uri(PostUrl), PostData);

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
        public void FriendOfFriendsProfileIdExtractor(ref FacebookUser fbUser)//ref ChilkatHttpHelpr chilkatHttpHelper
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                foreach (string item in LstProfileURLsFriendInfoScraper)
                {
                    try
                    {
                        string UserId = string.Empty;
                        string pageSource_HomePage = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                        string fb_dtsg = string.Empty;
                        UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

                        GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                        GlobusLogHelper.log.Debug("Please wait finding the friends ID...");

                        List<string> lstOwnFriendId = GetAllFriends(ref fbUser,ref gHttpHelper, UserId);
                        try
                        {
                            ExtractFriendOfFriendsInformation(ref fbUser,ref gHttpHelper, lstOwnFriendId, item, UserId);
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void ExtractFriendOfFriendsInformation(ref FacebookUser fbUser,ref GlobusHttpHelper gHttpHelper, List<string> lstFriendsids, string Prfurl, string User)
        {
            try
            {
                
                GlobusLogHelper.log.Info("Start Scraping Profile Information With Username : "+fbUser.username);
                GlobusLogHelper.log.Debug("Start Scraping Profile Information With Username : " + fbUser.username);
                lstFriendsids = lstFriendsids.Distinct().ToList();
                List<string> lstFriendofFriendUrl = new List<string>();

                List<string> LstFriendofFriendId = new List<string>();
                foreach (string itemurl in lstFriendsids)
                {
                    try
                    {
                        LstFriendofFriendId.Clear();
                        string urls = FBGlobals.Instance.fbProfileUrl + itemurl + "&sk=friends&ft_ref=mni";
                        string ProfilepageSrcaa = gHttpHelper.getHtmlfromUrl(new Uri(urls));
                        string[] Friends = Regex.Split(ProfilepageSrcaa, "user.php");
                        List<string> lstnewfriendid = new List<string>();
                        foreach (string iditem in Friends)
                        {
                            try
                            {
                                if (!iditem.Contains("<!DOCTYPE html>"))
                                {
                                    try
                                    {
                                        string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                        LstFriendofFriendId.Add(FriendIDS);
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


                        List<string> lsttotalajaxid = FriendsTotalId(ref fbUser,ref gHttpHelper, itemurl, User);
                        LstFriendofFriendId.AddRange(lsttotalajaxid);

                        List<string> list_finalFriendsID = new List<string>();

                        LstFriendofFriendId.ForEach(delegate(String friendID)
                        {
                            if (friendID.Contains("&"))
                            {
                                friendID = friendID.Remove(friendID.IndexOf("&"));
                            }
                            list_finalFriendsID.Add(friendID);
                        });

                        list_finalFriendsID = list_finalFriendsID.Distinct().ToList();

                        LstFriendofFriendId.Clear();
                        foreach (string item in list_finalFriendsID)
                        {
                            try
                            {
                                LstFriendofFriendId.Add(item);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        
                        foreach (string listFriendIditem in LstFriendofFriendId)
                        {
                            try
                            {
                                string urlss = FBGlobals.Instance.fbProfileUrl + listFriendIditem + "&sk=friends&ft_ref=mni";
                                string pageSrcaa = gHttpHelper.getHtmlfromUrl(new Uri(urlss));


                                string Urls = string.Empty;
                                string id = string.Empty;
                                string name = string.Empty;
                                string first_name = string.Empty;
                                string last_name = string.Empty;
                                string link = string.Empty;
                                string gender = string.Empty;
                                string locale = string.Empty;

                                Urls = FBGlobals.Instance.fbgraphUrl + listFriendIditem + "/";
                               
                                string pageSrc = gHttpHelper.getHtmlfromUrl(new Uri(Urls));                             

                                if (pageSrc.Contains("id"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("id"), 30);
                                        string[] ArrTemp = supsstring.Split('"');
                                        id = ArrTemp[2];
                                     
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (pageSrc.Contains("name"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("name"), 30);
                                        string[] ArrTemp = supsstring.Split('"');
                                        name = ArrTemp[2];
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (pageSrc.Contains("first_name"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("first_name"), 30);
                                        string[] ArrTemp = supsstring.Split('"');
                                        first_name = ArrTemp[2];
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (pageSrc.Contains("last_name"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("last_name"), 30);
                                        string[] ArrTemp = supsstring.Split('"');
                                        last_name = ArrTemp[2];
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (pageSrc.Contains("link"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("link"), 95);
                                        string[] ArrTemp = supsstring.Split('"');
                                        link = ArrTemp[2];
                                       

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (string.IsNullOrEmpty(link))
                                {
                                    link = FBGlobals.Instance.fbhomeurl + listFriendIditem;
                                    
                                }
                                if (pageSrc.Contains("gender"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("gender"));
                                        string[] ArrTemp = supsstring.Split('"');
                                        gender = ArrTemp[2];
                                       
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (pageSrc.Contains("locale"))
                                {
                                    try
                                    {
                                        string supsstring = pageSrc.Substring(pageSrc.IndexOf("locale"));
                                        string[] ArrTemp = supsstring.Split('"');
                                        locale = ArrTemp[2];
                                       
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                try
                                {
                                    string UserName = string.Empty;
                                    if (pageSrc.Contains("username"))
                                    {
                                        try
                                        {
                                            string UserName1 = pageSrc.Substring(pageSrc.IndexOf("username"));
                                            string[] ArrTemp = UserName1.Split('"');
                                            UserName = ArrTemp[2];

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    string FBEmailId = string.Empty;
                                    if (!string.IsNullOrEmpty(UserName))
                                    {
                                        FBEmailId = UserName + "@facebook.com";
                                    }
                                    else
                                    {
                                        FBEmailId = UserName + "@facebook.com"; ;
                                    }

                                    string ownprofileUrl = FBGlobals.Instance.fbProfileUrl + listFriendIditem + "&sk=info";

                                    string pagesourceofProfileUrl = gHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl));


                                    if (pagesourceofProfileUrl.Contains("fbTimelineSummarySectionWrapper") || (pagesourceofProfileUrl.Contains("Work and Education") || pagesourceofProfileUrl.Contains("Living") || pagesourceofProfileUrl.Contains("Basic Information") || pagesourceofProfileUrl.Contains("Contact Information")))
                                    {
                                       
                                        //foreach (var hrefArr1item in hrefArr1)
                                        {
                                            try
                                            {
                                                //if (hrefArr1item.Contains("/info"))
                                                {
                                                    string infopagesource = pagesourceofProfileUrl;
                                                    
                                                    string birthday = "";                                                    
                                                    string language = "";                                                  
                                                    string website = "";
                                                    string email = "";
                                                    string location = "";
                                                    string jobposition = "";
                                                    string jobcompany = "";
                                                    string Mobile_Phones = "";
                                                    string University = "";
                                                    string Secondaryschool = "";
                                                    string Hometown = "";
                                                    string Currentlocation = "";
                                                    string HighSchools = string.Empty;
                                                    string Colleges = string.Empty;
                                                    string Employers = string.Empty;
                                                    string CurrentCitys = string.Empty;
                                                    string Hometowns = string.Empty;
                                                    string PhoneNumber = string.Empty;
                                                    List<string> kkk = gHttpHelper.GetHrefsByTagAndAttributeName(infopagesource, "span", "fwb");

                                                    if (infopagesource.Contains(">High School<"))
                                                    {
                                                        try
                                                        {
                                                            string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                                                            string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                                                            List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                            foreach (string item in lsttd)
                                                            {
                                                                try
                                                                {
                                                                    HighSchools = HighSchools + ":" + item;
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
                                                    if (infopagesource.Contains(">Employers<"))
                                                    {
                                                        try
                                                        {
                                                            string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Employers<"), 1200);
                                                            string[] ArrHighschool = Regex.Split(infopagesource1, ">Employers<");
                                                            string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                            List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                            foreach (string item in lsttd)
                                                            {
                                                                try
                                                                {
                                                                    Employers = Employers + " : " + item;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                            string HighSchool = HS[0];
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }

                                                    }

                                                    if (infopagesource.Contains(">College<"))
                                                    {
                                                        try
                                                        {
                                                            string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">College<"), 1200);
                                                            string[] ArrHighschool = Regex.Split(infopagesource1, ">College<");
                                                            string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                            List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                            foreach (string item in lsttd)
                                                            {
                                                                try
                                                                {
                                                                    Colleges = Colleges + ":" + item;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                            // string HighSchool = HS[0];

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }

                                                    //if (infopagesource.Contains("Places Lived"))
                                                    //{
                                                    //    string s = Utils.getBetween(infopagesource,"","");

                                                    //    try
                                                    //    {
                                                    //        string GetAbout = getBetween(pagesourceofProfileUrl, "<div class=\"fsl fwb fcb\">", "</div>");
                                                    //        List<string> Arr = gHttpHelper.GetDataTag(GetAbout, "a");
                                                    //        Currentlocation = Arr[0].Replace(",", string.Empty);
                                                    //        GlobusLogHelper.log.Info("Get Current location " + Currentlocation + " of User : " + listFriendIditem);
                                                    //        GlobusLogHelper.log.Debug("Get Current location " + Currentlocation + " of User : " + listFriendIditem);

                                                    //    }
                                                    //    catch (Exception ex)
                                                    //    {
                                                    //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    //    }
                                                    //}

                                                    try
                                                    {
                                                        string[] arr = Regex.Split(pagesourceofProfileUrl, "<div class=\"fsl fwb fcb\">");
                                                        List<string> Arr = gHttpHelper.GetDataTag(arr[1], "a");
                                                        Hometowns = Arr[0].Replace(",", string.Empty);

                                                        GlobusLogHelper.log.Info("Get Hometown " + Hometowns + " of User : " + listFriendIditem);
                                                        GlobusLogHelper.log.Debug("Get Hometown " + Hometowns + " of User : " + listFriendIditem);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }



                                                    if (infopagesource.Contains(">Secondary school<"))
                                                    {
                                                        try
                                                        {
                                                            string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Secondary school<"), 1200);
                                                            string[] ArrHighschool = Regex.Split(infopagesource1, ">Secondary school<");
                                                            string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                            List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                            foreach (string item in lsttd)
                                                            {
                                                                try
                                                                {
                                                                    Secondaryschool = Secondaryschool + ":" + item;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                            //string HighSchool = HS[0];

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }

                                                    if (infopagesource.Contains(">University<"))
                                                    {
                                                        try
                                                        {
                                                            string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">University<"), 1200);
                                                            string[] ArrHighschool = Regex.Split(infopagesource1, ">University<");
                                                            string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                            List<string> lsttd = gHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                            foreach (string item in lsttd)
                                                            {
                                                                try
                                                                {
                                                                    University = University + ":" + item;
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                            //string HighSchool = HS[0];

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    //Secondary school
                                                    if (infopagesource.Contains(">Living<"))
                                                    {
                                                        try
                                                        {
                                                           
                                                            string[] ArrHighschool = Regex.Split(infopagesource, ">Living<");
                                                            string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");

                                                            foreach (string item in ArrHighSchool1)
                                                            {
                                                                try
                                                                {
                                                                    if (item.Contains("Current City") || item.Contains("Current location"))
                                                                    {
                                                                        try
                                                                        {
                                                                            string[] ARRCurrentCity = Regex.Split(item, "Current City");
                                                                            List<string> lsttd = gHttpHelper.GetDataTag(ARRCurrentCity[0], "a");
                                                                            foreach (string item1 in lsttd)
                                                                            {
                                                                                try
                                                                                {
                                                                                    CurrentCitys = CurrentCitys + ":" + item1;
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
                                                                    if (item.Contains("Hometown"))
                                                                    {
                                                                        try
                                                                        {
                                                                            string[] ARRHometown = Regex.Split(item, "Hometown");
                                                                            List<string> lsttd = gHttpHelper.GetDataTag(ARRHometown[0], "a");
                                                                            foreach (string item1 in lsttd)
                                                                            {
                                                                                try
                                                                                {
                                                                                    Hometowns = Hometowns + ":" + item1;
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
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    try
                                                    {
                                                        string[] contactinfoArr = Regex.Split(infopagesource, "uiHeader uiHeaderWithImage fbTimelineAboutMeHeader");
                                                        if (contactinfoArr.Count() >= 2)
                                                        {
                                                            List<string> lstcontactinfoArrtd = gHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                                                            foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                                            {
                                                                try
                                                                {
                                                                    if (lstcontactinfoArrtditem.Contains("Employers"))
                                                                    {
                                                                        Employers = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers", string.Empty).Replace(",", ";").Trim());

                                                                        GlobusLogHelper.log.Info("Get Employers " + Employers + " of User : " + listFriendIditem);
                                                                        GlobusLogHelper.log.Debug("Get Employers " + Employers + " of User : " + listFriendIditem);
                                                                    }

                                                                    if (lstcontactinfoArrtditem.Contains("College"))
                                                                    {
                                                                        Colleges = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("College", string.Empty).Replace(",", ";").Trim());

                                                                        GlobusLogHelper.log.Info("Get Colleges " + Colleges + " of User : " + listFriendIditem);
                                                                        GlobusLogHelper.log.Debug("Get Colleges " + Colleges + " of User : " + listFriendIditem);
                                                                    }

                                                                    if (lstcontactinfoArrtditem.Contains("High School"))
                                                                    {
                                                                        HighSchools = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("High School", string.Empty).Replace(",", ";").Trim());

                                                                        GlobusLogHelper.log.Info("Get HighSchools " + HighSchools + " of User : " + listFriendIditem);
                                                                        GlobusLogHelper.log.Debug("Get HighSchools " + HighSchools + " of User : " + listFriendIditem);
                                                                    }

                                                                    if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                                                    {
                                                                        Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());
                                                                        
                                                                        GlobusLogHelper.log.Info("Get Mobile " + Mobile_Phones + " of User : " + listFriendIditem);
                                                                        GlobusLogHelper.log.Debug("Get Mobile " + Mobile_Phones + " of User : " + listFriendIditem);
                                                                    }
                                                                    if (lstcontactinfoArrtditem.Contains("Address"))
                                                                    {
                                                                        location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());
                                                                      
                                                                        GlobusLogHelper.log.Info("Get location " + location + " of User : " + listFriendIditem);
                                                                        GlobusLogHelper.log.Debug("Get location " + location + " of User : " + listFriendIditem);
                                                                    }
                                                                    if (lstcontactinfoArrtditem.Contains("Email"))
                                                                    {
                                                                        email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                                        string[] emailArr1 = Regex.Split(email, " ");
                                                                        if (emailArr1.Count() >= 2)
                                                                        {
                                                                            email = emailArr1[1] + emailArr1[0];
                                                                          
                                                                            GlobusLogHelper.log.Info("Get Email " + email + " of User : " + listFriendIditem);
                                                                            GlobusLogHelper.log.Debug("Get Email " + email + " of User : " + listFriendIditem);
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
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }


                                                    if (infopagesource.Contains(">Living<") || infopagesource.Contains("Places Lived"))   //Current City
                                                    {

                                                        try
                                                        {
                                                            string[] Home = System.Text.RegularExpressions.Regex.Split(infopagesource, "<div class=\"fsl fwb fcb\">");
                                                            Hometown = Home[0];
                                                            foreach (var Home_item in Home)
                                                            {
                                                                try
                                                                {
                                                                    if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Current City"))
                                                                    {
                                                                        List<string> CCity = gHttpHelper.GetDataTag(Home_item, "a");
                                                                        Currentlocation = CCity[0];
                                                                    }
                                                                    if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Hometown"))
                                                                    {
                                                                        List<string> Ht = gHttpHelper.GetDataTag(Home_item, "a");
                                                                        Hometowns = Ht[0];
                                                                        Hometown = Ht[0];
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



                                                    if (!string.IsNullOrEmpty(ExportFilePathFriendInfoScraper))
                                                    {
                                                        try
                                                        {
                                                            string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;
                                                           
                                                            string CSVHeader = "ExtractUrl" + "," + "FriendInfoLink" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";
                                                            string CSV_Content = Prfurl.Replace(",", " ") + "," + ownprofileUrl.Replace(",", string.Empty) + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + Currentlocation.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + Colleges.Replace(",", " ") + "," + FBEmailId.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;

                                                          

                                                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFriendInfoScraper);
                                                            GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                                            GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(ExportFilePathFriendInfoScraper))
                                        {
                                            try
                                            {
                                                string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;
                                                
                                                string CSVHeader = "ExtractUrl" + "," + "FriendInfoLink" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";
                                                string CSV_Content = Prfurl.Replace(",", " ") + "," + "-".Replace(",", string.Empty) + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + ", " + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;

                                               

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFriendInfoScraper);
                                                GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                                GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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

            GlobusLogHelper.log.Info("Process Completed With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed With Username >>> " + fbUser.username);
        }

        public static List<string> GetAllFriends(ref FacebookUser fbUser, ref GlobusHttpHelper gHttpHelper, string userId)
        {
            List<string> finalList_Friends = new List<string>();

            List<string> list_finalFriendsID = new List<string>();

            try
            {
                string pgSource_Friends = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + userId + "&sk=friends"));

                string[] Friends = Regex.Split(pgSource_Friends, "user.php");

                ParseFriendIDs(Friends, finalList_Friends);

                List<string> lstnewfriendid = new List<string>();

              


                string collection_token = "";
                string cursor = "";

                //check if all friends loaded
                string patternAllFriendsLoaded = "\"TimelineAppCollection\",\"setFullyLoaded\",[],[\"pagelet_timeline_app_collection_";

                int tempCount_AjaxRequests = 0;

                do //
                {
                    try
                    {
                        collection_token = "";
                        cursor = "";

                        string[] arry = Regex.Split(pgSource_Friends, "enableContentLoader");
                        if (arry.Length > 1)
                        {
                            string rawData = arry[1];

                            int startIndx_collection_token = rawData.IndexOf("pagelet_timeline_app_collection_") + "pagelet_timeline_app_collection_".Length;
                            int endIndx_collection_token = rawData.IndexOf("\"", startIndx_collection_token);
                            collection_token = rawData.Substring(startIndx_collection_token, endIndx_collection_token - startIndx_collection_token);

                            int startIndx_cursor = rawData.IndexOf(",\"", endIndx_collection_token) + ",\"".Length;
                            int endIndx_cursor = rawData.IndexOf("\"", startIndx_cursor);
                            cursor = rawData.Substring(startIndx_cursor, endIndx_cursor - startIndx_cursor);

                        }

                        string raw_data = "{\"collection_token\":\"" + collection_token + "\",\"cursor\":\"" + cursor + "\",\"tab_key\":\"friends\",\"profile_id\":" + userId + ",\"overview\":false,\"ftid\":null,\"order\":null,\"sk\":\"friends\"}";
                        string encoded_raw_data = Uri.EscapeDataString(raw_data);

                        string getURL_MoreFriendsAjax = FBGlobals.Instance.FriendInfoScraperGetAjaxAllFriendsUrl + encoded_raw_data + "&__user=" + userId + "&__a=1&__dyn=7n8ahyj2qmudwNAEU&__req=2";
                        string res_getURL_MoreFriendsAjax = gHttpHelper.getHtmlfromUrl(new Uri(getURL_MoreFriendsAjax));

                        pgSource_Friends = res_getURL_MoreFriendsAjax;

                        string[] arry_UserData = Regex.Split(pgSource_Friends, "user.php");

                        ParseFriendIDs(arry_UserData, finalList_Friends);

                        tempCount_AjaxRequests++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                } while ((collection_token != "" && cursor != "") || tempCount_AjaxRequests < 15 || pgSource_Friends.Contains(patternAllFriendsLoaded));



                finalList_Friends.ForEach(delegate(String friendID)
                {
                    if (friendID.Contains("&"))
                    {
                        friendID = friendID.Remove(friendID.IndexOf("&"));
                    }
                    list_finalFriendsID.Add(friendID);
                });

                list_finalFriendsID = list_finalFriendsID.Distinct().ToList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return list_finalFriendsID;
        }

        private static void ParseFriendIDs(string[] Friends, List<string> lstnewfriendid)
        {
            foreach (string iditem in Friends)
            {
                try
                {
                    if (!iditem.Contains("<!DOCTYPE html>"))
                    {
                        try
                        {
                            string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                            lstnewfriendid.Add(FriendIDS);
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

        /// <summary>
        /// FBID EXtractor Module
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        /// 


        public bool isStopFBIDExtracter = false;

        readonly object lockrThreadControllerisStopFBIDExtracter = new object();

        int countThreadControllerisStopFBIDExtracter = 0;

        public List<Thread> lstThreadsisStopFBIDExtracter = new List<Thread>();



        #region Property For FBIDExtracter

        public int NoOfThreadsFBIDExtracter
        {
            get;
            set;
        }

       public bool FBIDExtracterOnlyTargetedID
        {
            get;
            set;
        }
        public List<string> LstFBIDExtractorList
        {
            get;
            set;
        }

        public static string ExportFilePathFBIDExtractor
        {
            get;
            set;
        }      
      

        #endregion
        public List<string> LstFBProfileUrlList
        {
            get;
            set;
        }
        public static string FBIDExtractorProsessUsing
        {
            get;
            set;
        }
        public void StartExtracter()
        {
            
               //ID Scraper
            if (FBIDExtractorProsessUsing == "URL Scraper")
            {
                StartFBIDExtracter();
            }
            else
            {              
               StartFBIDExtracterUsingProfileUrls();              
               
            }
        }
        public void StartFBIDExtracterUsingProfileUrls()
        {
            GlobusHttpHelper ObjGlobHttp = new GlobusHttpHelper();
            string PageSource = string.Empty;
            try
            {
                foreach (var LstFBProfileUrlList_item in LstFBProfileUrlList)
                {
                    string FBID = string.Empty;
                    try
                    {
                        string[] Arr = System.Text.RegularExpressions.Regex.Split(LstFBProfileUrlList_item, ".com/");
                        string GraphUrl = FBGlobals.Instance.fbgraphUrl + Arr[1];
                        PageSource = ObjGlobHttp.getHtmlfromUrl(new Uri(GraphUrl));
                        FBID = Utils.getBetween(PageSource,"\"id\": \"","\"");

                        if (!string.IsNullOrEmpty(ExportFilePathFBIDExtractor) && !string.IsNullOrEmpty(FBID))
                        {
                            try
                            {
                                string ProfileLink = LstFBProfileUrlList_item;
                                string commaSeparatedData = FBID + ProfileLink;

                              
                                if (!string.IsNullOrEmpty(FBID))
                                {
                                    FBID = "'" + FBID;
                                    string CSVHeader = "FBID" + "," + "ProfileLink";
                                    string CSV_Content = FBID + "," + ProfileLink;
                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFBIDExtractor);
                                    GlobusLogHelper.log.Info("FBID  " + FBID + " Saved In CSV");
                                    GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            GlobusLogHelper.log.Debug("Process Completed");
            GlobusLogHelper.log.Info("Process Completed");


        }
        public void StartFBIDExtracter()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsFBIDExtracter > 0)
                {
                    numberOfAccountPatch = NoOfThreadsFriendInfoScraper;
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
                                lock (lockrThreadControllerFriendInfoScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerisStopFBIDExtracter >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerisStopFBIDExtracter);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread FBIDExtractor = new Thread(ExtractFriendsInformation);
                                            FBIDExtractor.Name = "workerThread_Profiler_" + acc;
                                            FBIDExtractor.IsBackground = true;


                                            FBIDExtractor.Start(new object[] { item });

                                            countThreadControllerisStopFBIDExtracter++;
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

        //ExtractFriendsInformation

        public void ExtractFriendsInformation(object parameters)
        {
            try
            {
                if (!isStopFBIDExtracter)
                {
                    try
                    {
                        lstThreadsisStopFBIDExtracter.Add(Thread.CurrentThread);
                        lstThreadsisStopFBIDExtracter.Distinct();
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
                                

                                // Call StartAction                                

                                if (FBIDExtracterOnlyTargetedID == true)
                                {
                                    ExtractFriendsInformationFacebooker(ref objFacebookUser);

                                    GlobusLogHelper.log.Info("Process completed With Username : " + objFacebookUser.username);
                                    GlobusLogHelper.log.Debug("Process completed With Username : " + objFacebookUser.username);
                                }
                                else
                                {
                                    ExtractFriendsInformationFacebooker(ref objFacebookUser);
                                    FriendProfileIdExtractor1(ref objFacebookUser);

                                }
                                return;
                               //FriendOfFriendsProfileIdExtractor1(ref objFacebookUser);                                                        
                               //OwnExtractFriendsInformationFacebooker(ref objFacebookUser);
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
                    //if (!isStopFBIDExtracter)
                    {
                        lock (lockrThreadControllerisStopFBIDExtracter)
                        {
                            countThreadControllerisStopFBIDExtracter--;
                            Monitor.Pulse(lockrThreadControllerisStopFBIDExtracter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }  


        public void ExtractFriendsInformationFacebooker(ref FacebookUser fbUser)
        {
            GlobusHttpHelper chilkatHttpHelper = fbUser.globusHttpHelper;

           GlobusLogHelper.log.Debug("Start Scraping Profile Information !");
           GlobusLogHelper.log.Info("Start Scraping Profile Information !");
        

            foreach (string listFriendIditem in LstFBIDExtractorList)
            {
                try
                {
                    string Urls = string.Empty;
                    string id = string.Empty;
                    string name = string.Empty;
                    string first_name = string.Empty;
                    string last_name = string.Empty;
                    string link = string.Empty;
                    string gender = string.Empty;
                    string locale = string.Empty;
                    string birthday = "";                  
                    string language = "";                 
                    string website = "";
                    string email = "";
                    string location = "";
                    string jobposition = "";
                    string jobcompany = "";
                    string Mobile_Phones = "";
                    string University = "";
                    string Secondaryschool = "";
                    string Hometown = "";
                    string Currentlocation = "";
                    string pagesourceofProfileUrl = string.Empty;
                    string FBEmailId = string.Empty;

                    if (listFriendIditem.Contains("www.facebook.com"))
                    {
                        Urls = listFriendIditem.Replace("https://www.facebook.com/", FBGlobals.Instance.fbgraphUrl) + "/";
                    }
                    else
                    {
                        Urls = FBGlobals.Instance.fbgraphUrl + listFriendIditem + "/";
                    }
                    // "http://graph.facebook.com/"
                   
                    string pageSrc = chilkatHttpHelper.getHtmlfromUrl(new Uri(Urls));              

                    if (pageSrc.Contains("id"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("id"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            id = ArrTemp[2];
                      
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            name = ArrTemp[2];                      

                           
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("first_name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("first_name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            first_name = ArrTemp[2];
                          
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("last_name"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("last_name"), 30);
                            string[] ArrTemp = supsstring.Split('"');
                            last_name = ArrTemp[2];
                        
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("link"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("link"), 95);
                            string[] ArrTemp = supsstring.Split('"');
                            link = ArrTemp[2];
                          

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (string.IsNullOrEmpty(link))
                    {
                        if (!listFriendIditem.Contains("www.facebook.com"))
                        {
                            link = FBGlobals.Instance.fbhomeurl + listFriendIditem;
                        }
                        else
                        {
                           // FBGlobals.Instance.fbhomeurl +
                            link =  listFriendIditem;             // "http://www.facebook.com/" 
                        }
                      
                    }
                    if (pageSrc.Contains("gender"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("gender"));
                            string[] ArrTemp = supsstring.Split('"');
                            gender = ArrTemp[2];
                            
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (pageSrc.Contains("locale"))
                    {
                        try
                        {
                            string supsstring = pageSrc.Substring(pageSrc.IndexOf("locale"));
                            string[] ArrTemp = supsstring.Split('"');
                            locale = ArrTemp[2];                           
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    try
                    {

                        string UserName = string.Empty;
                        if (pageSrc.Contains("username"))
                        {
                            try
                            {
                                string UserName1 = pageSrc.Substring(pageSrc.IndexOf("username"));
                                string[] ArrTemp = UserName1.Split('"');
                                UserName = ArrTemp[2];

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                      
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            FBEmailId = UserName + "@facebook.com";
                        }
                        else
                        {
                            FBEmailId = UserName + "@facebook.com"; ;
                        }




                        string ownprofileUrl = string.Empty;
                        if (!listFriendIditem.Contains("www.facebook.com"))
                        {
                            ownprofileUrl = FBGlobals.Instance.fbhomeurl + listFriendIditem + "/about";
                        }
                        else
                        {
                            ownprofileUrl = link + "/about";
                        }
                           // "http://www.facebook.com/"
                             pagesourceofProfileUrl = chilkatHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl));
                             if (string.IsNullOrEmpty(pagesourceofProfileUrl))
                             {
                                 ownprofileUrl = link + "/about";
                                 pagesourceofProfileUrl = chilkatHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl)); 
                             }

                            try
                            {
                                string GetAbout = getBetween(pagesourceofProfileUrl, "<div class=\"fsl fwb fcb\">", "</div>");
                                List<string> Arr = chilkatHttpHelper.GetDataTag(GetAbout, "a");
                                Currentlocation = Arr[0].Replace(",", string.Empty);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            if (pagesourceofProfileUrl.Contains("fsm fwn fcg") && pagesourceofProfileUrl.Contains("From"))
                            {
                                string[] Arrr = System.Text.RegularExpressions.Regex.Split(pagesourceofProfileUrl, "fsm fwn fcg");
                                foreach (var Arr_item in Arrr)
                                {
                                    try
                                    {
                                        if (!Arr_item.Contains("<!DOCTYPE html>"))
                                        {
                                            if (Arr_item.Contains("><span>From <a href="))
                                            {
                                                string town = Utils.getBetween(Arr_item, "><span>From", "</a></span>").Replace(",", "") + "@@@@";
                                                Hometown = Utils.getBetween(town, "\">", "@@@@").Replace(",", "");

                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(Hometown))
                            {
                                try
                                {
                                    string[] arr = Regex.Split(pagesourceofProfileUrl, "<div class=\"fsl fwb fcb\">");
                                    List<string> Arr = chilkatHttpHelper.GetDataTag(arr[2], "a");
                                    Hometown = Arr[0].Replace(",", string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                
                            }

                            try
                            {
                                string[] arr = Regex.Split(pagesourceofProfileUrl, "fbTimelineSection mtm _bak fbTimelineCompactSection");                                
                                string[] arr1 = Regex.Split(arr[1], "<th class=\"_3sts\">");

                                foreach (var arr1_item in arr1)
                                {
                                    if (arr1_item.Contains("Gender"))
                                    {
                                        try
                                        {
                                            List<string> Arr2 = chilkatHttpHelper.GetDataTag(arr1_item, "div");
                                            gender = Arr2[0];
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        break;
                                    }
                                }

                                foreach (var arr1_item in arr1)
                                {
                                    if (arr1_item.Contains("Birthday"))
                                    {
                                        try
                                        {
                                            List<string> Arr2 = chilkatHttpHelper.GetDataTag(arr1_item, "div");
                                            birthday = Arr2[0].Replace(",","-");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }


                          if (pagesourceofProfileUrl.Contains("clearfix experienceImageBlock lfloat"))
                           {
                            string[] hrefArr = Regex.Split(pagesourceofProfileUrl, "clearfix experienceImageBlock lfloat");
                            string allhref = string.Empty;
                            foreach (var hrefArr_item in hrefArr)
                            {
                                if (!hrefArr_item.Contains("<!DOCTYPE html>"))
                                {
                                    string ss = Utils.getBetween(hrefArr_item, "<div class=\"experienceTitle\">", "</div></div></div></div><");
                                    List<string> Arr = chilkatHttpHelper.GetDataTag(hrefArr_item, "a");
                                    jobcompany=Arr[1].Replace(",",string.Empty);
                                    break;                                   
                                }                                
                            }
                            string Univercity = getBetween(hrefArr[2], "<div class=\"experienceTitle\">", "</div><div");
                            List<string> Uniersity = chilkatHttpHelper.GetDataTag(Univercity, "a");
                            string Uni= Uniersity[0].Replace("&#039;",string.Empty);
                            University = Uni;

                            try
                            {
                                allhref = hrefArr[2].Substring(hrefArr[2].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                                allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 100);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }                           
                            string[] hrefArr1 = Regex.Split(allhref, "href=");
                            foreach (var hrefArr1item in hrefArr1)
                            {
                                try
                                {
                                    if (hrefArr1item.Contains("/info"))
                                    {
                                        string[] infohrefArr = Regex.Split(hrefArr1item, "\"");                                     
                                        string infohref = infohrefArr[1];
                                        string infopagesource = chilkatHttpHelper.getHtmlfromUrl(new Uri(infohref));                                     
                                      
                                        string[] nameArr = Regex.Split(infopagesource, "nameButton uiButton uiButtonOverlay");
                                        List<string> lstspan = chilkatHttpHelper.GetDataTag(nameArr[1], "span");
                                        name = System.Net.WebUtility.HtmlDecode(lstspan[0].Replace(",", ";").Trim());
                                        string[] basicinfoArr = Regex.Split(infopagesource, "uiInfoTable profileInfoTable uiInfoTableFixed");
                                        if (basicinfoArr.Count() >= 2)
                                        {
                                            List<string> lsttd = chilkatHttpHelper.GetDataTag(basicinfoArr[1], "tr");
                                            foreach (string lsttditem in lsttd)
                                            {
                                                if (lsttditem.Contains("Birthday"))
                                                {
                                                    birthday = System.Net.WebUtility.HtmlDecode(lsttditem.Replace("Birthday", string.Empty).Replace(",", ";").Trim());
                                                }
                                                if (lsttditem.Contains("Sex"))
                                                {
                                                    gender = System.Net.WebUtility.HtmlDecode(lsttditem.Replace("Sex", string.Empty).Replace(",", ";").Trim());
                                                }
                                            }
                                        }
                                     
                                        string[] contactinfoArr = Regex.Split(infopagesource, "uiHeader uiHeaderWithImage fbTimelineAboutMeHeader");
                                        if (contactinfoArr.Count() >= 2)
                                        {
                                            List<string> lstcontactinfoArrtd = chilkatHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                                            foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                            {
                                                if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                                {
                                                    Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());
                                                   
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Address"))
                                                {
                                                    location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());
                                                  
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Email"))
                                                {
                                                    email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                    string[] emailArr1 = Regex.Split(email, " ");
                                                    if (emailArr1.Count() >= 2)
                                                    {
                                                        email = emailArr1[1] + emailArr1[0];
                                                      
                                                    }
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Current location") || lstcontactinfoArrtditem.Contains("Current City"))
                                                {
                                                    Currentlocation = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Current location", string.Empty).Replace(",", ";").Replace("Current City", string.Empty).Trim());
                                                    
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Hometown"))
                                                {
                                                    Hometown = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Hometown", string.Empty).Replace(",", ";").Trim());
                                                   
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Employers"))
                                                {
                                                    string[] workinfoArr2 = Regex.Split(infopagesource, "experienceTitle");
                                                    string[] workinfospanArr2 = Regex.Split(workinfoArr2[1], "span");
                                                    if (workinfospanArr2.Count() >= 2)
                                                    {
                                                        jobcompany = System.Net.WebUtility.HtmlDecode(workinfospanArr2[1].Replace("class=\"fwb\">", string.Empty).Replace("</", string.Empty).Replace(",", ";").Trim());
                                                       
                                                    }
                                                    jobposition = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers ·", string.Empty).Replace(jobcompany, string.Empty).Replace(",", ";").Trim());
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Secondary school"))
                                                {
                                                    try
                                                    {
                                                        Secondaryschool = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Secondary school", string.Empty).Replace(",", ";").Trim());
                                                      
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }

                                                if (lstcontactinfoArrtditem.Contains("University"))
                                                {
                                                  
                                                    try
                                                    {
                                                        University = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("University", string.Empty).Replace(",", ";").Trim());

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }

                                            }
                                            try
                                            {
                                                List<string> lstcontactinfoArrtd1 = chilkatHttpHelper.GetDataTag(contactinfoArr[2], "tr");
                                                foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd1)
                                                {
                                                    try
                                                    {
                                                        if (lstcontactinfoArrtditem.Contains("Current location"))
                                                        {
                                                            Currentlocation = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Current location", string.Empty).Replace(",", ";").Trim());
                                                         
                                                        }
                                                        if (lstcontactinfoArrtditem.Contains("Hometown"))
                                                        {
                                                            Hometown = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Hometown", string.Empty).Replace(",", ";").Trim());
                                                          
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
                                        if (contactinfoArr.Count() >= 2)
                                        {
                                            List<string> lstcontactinfoArrtd = chilkatHttpHelper.GetDataTag(contactinfoArr[2], "tr");
                                            foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                            {
                                                if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                                {
                                                    Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());
                                                  
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Address"))
                                                {
                                                    location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());
                                                   
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Email"))
                                                {
                                                    email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                    string[] emailArr1 = Regex.Split(email, " ");
                                                    if (emailArr1.Count() >= 2)
                                                    {
                                                        email = emailArr1[1] + emailArr1[0];
                                                     
                                                    }
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Current location") || lstcontactinfoArrtditem.Contains("Current City"))
                                                {
                                                    Currentlocation = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Current location", string.Empty).Replace(",", ";").Replace("Current City", string.Empty).Trim());
                                                   
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Hometown"))
                                                {
                                                    Hometown = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Hometown", string.Empty).Replace(",", ";").Trim());
                                                  
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Employers"))
                                                {
                                                    string[] workinfoArr2 = Regex.Split(infopagesource, "experienceTitle");
                                                    string[] workinfospanArr2 = Regex.Split(workinfoArr2[1], "span");
                                                    if (workinfospanArr2.Count() >= 2)
                                                    {
                                                        jobcompany = System.Net.WebUtility.HtmlDecode(workinfospanArr2[1].Replace("class=\"fwb\">", string.Empty).Replace("</", string.Empty).Replace(",", ";").Trim());
                                                     
                                                    }
                                                    jobposition = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers ·", string.Empty).Replace(jobcompany, string.Empty).Replace(",", ";").Trim());
                                                }
                                                if (lstcontactinfoArrtditem.Contains("Secondary school"))
                                                {
                                                    try
                                                    {
                                                        Secondaryschool = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Secondary school", string.Empty).Replace(",", ";").Trim());
                                                       
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }

                                                }

                                                if (lstcontactinfoArrtditem.Contains("University"))
                                                {
                                                    University = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("University", string.Empty).Replace(",", ";").Trim());
                                                   
                                                }
                                            }
                                            try
                                            {
                                                List<string> lstcontactinfoArrtd1 = chilkatHttpHelper.GetDataTag(contactinfoArr[2], "tr");
                                                foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd1)
                                                {
                                                    try
                                                    {
                                                        if (lstcontactinfoArrtditem.Contains("Current location"))
                                                        {
                                                            Currentlocation = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Current location", string.Empty).Replace(",", ";").Trim());
                                                           
                                                        }
                                                        if (lstcontactinfoArrtditem.Contains("Hometown"))
                                                        {
                                                            Hometown = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Hometown", string.Empty).Replace(",", ";").Trim());
                                                          
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
                                        string[] workinfoArr1 = Regex.Split(infopagesource, "clearfix experienceImageBlock lfloat");
                                        if (workinfoArr1.Count() >= 2)
                                        {
                                            List<string> workinfospanArr1 = chilkatHttpHelper.GetDataTag(workinfoArr1[1], "tr");
                                            foreach (var workinfospanArr1item in workinfospanArr1)
                                            {
                                                if (workinfospanArr1item.Contains("Employers"))
                                                {
                                                    string[] workinfoArr2 = Regex.Split(infopagesource, "experienceTitle");
                                                    string[] workinfospanArr2 = Regex.Split(workinfoArr2[1], "span");
                                                    if (workinfospanArr2.Count() >= 2)
                                                    {
                                                        jobcompany = System.Net.WebUtility.HtmlDecode(workinfospanArr2[1].Replace("class=\"fwb\">", string.Empty).Replace("</", string.Empty).Replace(",", ";").Trim());
                                                        jobcompany = jobcompany.Replace("&#039;",string.Empty);
                                                    }
                                                    jobposition = System.Net.WebUtility.HtmlDecode(workinfospanArr1item.Replace("Employers ·", string.Empty).Replace(jobcompany, string.Empty).Replace(",", ";").Trim());

                                                }

                                                if (workinfospanArr1item.Contains("Secondary school"))
                                                {
                                                    try
                                                    {
                                                        Secondaryschool = System.Net.WebUtility.HtmlDecode(workinfospanArr1item.Replace("Secondary school", string.Empty).Replace(",", ";").Trim());
                                                        
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }                                                   
                                                }

                                                if (workinfospanArr1item.Contains("University"))
                                                {
                                                    try
                                                    {
                                                        University = System.Net.WebUtility.HtmlDecode(workinfospanArr1item.Replace("University", string.Empty).Replace(",", ";").Trim());
                                                        
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

                    try
                    {
                        if (pagesourceofProfileUrl.Contains("<td class=\"_51m- contactInfoPhone\">"))
                        {
                            string phone = getBetween(pagesourceofProfileUrl, "<td class=\"_51m- contactInfoPhone\">", "</td>");
                            List<string> Phonelst = chilkatHttpHelper.GetDataTag(phone, "span");
                            Mobile_Phones = Phonelst[0].Replace(",",string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    
                  //  Urls

                    if (!string.IsNullOrEmpty(ExportFilePathFBIDExtractor))
                    {
                        try
                        {
                            string ProfileLink = FBGlobals.Instance.fbhomeurl + id;     // "http://facebook.com/"
                            
                            string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;

                            string CSVHeader = "ProfileLink" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "locale"+"FbEmail";
                            string CSV_Content = ProfileLink + "," + id + "," + name + "," + first_name + "," + last_name + "," + locale + "," + FBEmailId;

                           // string CSVHeader = "ProfileLink" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Birthday" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "Email" + "," + "Telephone" + "," + "UserAccount";

                           // string CSV_Content = ProfileLink + "," + id + "," + name + "," + first_name + "," + last_name + "," + birthday + "," + link + "," + gender + "," + locale + "," + Hometown + "," + Currentlocation + ", " + jobcompany + "," + University + "," + Secondaryschool + "," + email + "," + Mobile_Phones + ",," + fbUser.username;// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;
                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFBIDExtractor);
                            GlobusLogHelper.log.Info("Profile Info Saved In CSV");
                            GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
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

        List<string> listFriendId = new List<string>();

        public void ExtractOwnFriendsInformationFacebooker(ref FacebookUser fbUser)
        {

            GlobusHttpHelper chilkatHttpHelper = fbUser.globusHttpHelper;
          
            string UserId = string.Empty;

            string pageSource_HomePage = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));      // "http://www.facebook.com/"

            UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");                                      //pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
            }
            int count_Friends = ExtractFriendCount(ref fbUser, UserId);

            listFriendId.Clear();

            listFriendId = ExtractFriendIdsFb(ref fbUser, ref UserId, count_Friends);


            listFriendId = listFriendId.Distinct().ToList();
            foreach (string listFriendIditem in listFriendId)
            {
                try
                {
                    string ownprofileUrl = FBGlobals.Instance.fbProfileUrl + listFriendIditem;                    // "http://www.facebook.com/profile.php?id=" 
                    string pagesourceofProfileUrl = chilkatHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl));
                    if (pagesourceofProfileUrl.Contains("fbTimelineSummarySectionWrapper"))
                    {
                        string[] hrefArr = Regex.Split(pagesourceofProfileUrl, "pagelet_timeline_summary");
                        string allhref = string.Empty;
                        try
                        {
                            allhref = hrefArr[2].Substring(hrefArr[2].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                            allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 100);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                      
                        string[] hrefArr1 = Regex.Split(allhref, "href=");
                        foreach (var hrefArr1item in hrefArr1)
                        {
                            try
                            {
                                if (hrefArr1item.Contains("/info"))
                                {
                                    string[] infohrefArr = Regex.Split(hrefArr1item, "\"");                               
                                    string infohref = infohrefArr[1];
                                    string infopagesource = chilkatHttpHelper.getHtmlfromUrl(new Uri(infohref));
                                    string name = "";
                                    string birthday = "";
                                    string gender = "";
                                    string language = "";                                    
                                    string website = "";
                                    string email = "";
                                    string location = "";
                                    string jobposition = "";
                                    string jobcompany = "";
                                    string Mobile_Phones = "";
                                    string[] nameArr = Regex.Split(infopagesource, "nameButton uiButton uiButtonOverlay");
                                    List<string> lstspan = chilkatHttpHelper.GetDataTag(nameArr[1], "span");
                                    name = System.Net.WebUtility.HtmlDecode(lstspan[0].Replace(",", ";").Trim());
                                    string[] basicinfoArr = Regex.Split(infopagesource, "uiInfoTable profileInfoTable uiInfoTableFixed");
                                    if (basicinfoArr.Count() >= 2)
                                    {
                                        List<string> lsttd = chilkatHttpHelper.GetDataTag(basicinfoArr[1], "tr");
                                        foreach (string lsttditem in lsttd)
                                        {
                                            if (lsttditem.Contains("Birthday"))
                                            {
                                                birthday = System.Net.WebUtility.HtmlDecode(lsttditem.Replace("Birthday", string.Empty).Replace(",", ";").Trim());
                                            }
                                            if (lsttditem.Contains("Sex"))
                                            {
                                                gender = System.Net.WebUtility.HtmlDecode(lsttditem.Replace("Sex", string.Empty).Replace(",", ";").Trim());
                                            }
                                        }
                                    }
                                
                                    string[] contactinfoArr = Regex.Split(infopagesource, "uiInfoTable mvl mhm profileInfoTable");
                                    if (contactinfoArr.Count() >= 2)
                                    {
                                        List<string> lstcontactinfoArrtd = chilkatHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                                        foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                        {
                                            if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                            {
                                                Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());
                                            }
                                            if (lstcontactinfoArrtditem.Contains("Address"))
                                            {
                                                location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());
                                            }
                                            if (lstcontactinfoArrtditem.Contains("Email"))
                                            {
                                                email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                string[] emailArr1 = Regex.Split(email, " ");
                                                if (emailArr1.Count() >= 2)
                                                {
                                                    email = emailArr1[1] + emailArr1[0];
                                                }
                                            }
                                        }
                                    }

                                    string[] workinfoArr1 = Regex.Split(infopagesource, "uiInfoTable mal profileInfoTable uiInfoTableFixed");
                                    if (workinfoArr1.Count() >= 2)
                                    {
                                        List<string> workinfospanArr1 = chilkatHttpHelper.GetDataTag(workinfoArr1[1], "tr");
                                        foreach (var workinfospanArr1item in workinfospanArr1)
                                        {
                                            if (workinfospanArr1item.Contains("Employers"))
                                            {
                                                string[] workinfoArr2 = Regex.Split(infopagesource, "experienceTitle");
                                                string[] workinfospanArr2 = Regex.Split(workinfoArr2[1], "span");
                                                if (workinfospanArr2.Count() >= 2)
                                                {
                                                    jobcompany = System.Net.WebUtility.HtmlDecode(workinfospanArr2[1].Replace("class=\"fwb\">", string.Empty).Replace("</", string.Empty).Replace(",", ";").Trim());
                                                }
                                                jobposition = System.Net.WebUtility.HtmlDecode(workinfospanArr1item.Replace("Employers ·", string.Empty).Replace(jobcompany, string.Empty).Replace(",", ";").Trim());
                                            }
                                        }

                                    }

                                    if (!string.IsNullOrEmpty(ExportFilePathFBIDExtractor))
                                    {
                                        try
                                        {

                                            string CSVHeader = "Name" + "," + "Birthday" + ", " + "Gender" + "," + "Mobile_Phones" + "," + "Email" + "," + "Location" + "," + "JobPosition" + "," + "JobCompany" + "," + "URL" + "," + "Used Account";
                                            string CSV_Content = name + "," + birthday + ", " + gender + "," + Mobile_Phones + "," + email + "," + location + "," + jobposition + "," + jobcompany + "," + infohref + "," + fbUser.username;


                                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFBIDExtractor);
                                            GlobusLogHelper.log.Info("Own Friend Info Saved In CSV");

                                            GlobusLogHelper.log.Debug("Own Friend Info Saved In CSV");
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

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

            }
        }



        public static int ExtractFriendCount(ref FacebookUser fbUser, string UserId)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            int FriendCountNumber = 0;
            try
            {

                string url = FBGlobals.Instance.fbProfileUrl + UserId;                     // "https://www.facebook.com/profile.php?id=" 
                string PageSrcFriendCountOLd = HttpHelper.getHtmlfromUrl(new Uri(url));
                Regex NumChk = new Regex("^[0-9]*$");
                if (PageSrcFriendCountOLd.Contains("Friends ("))
                {
                    //** Friend Count old way *************************************** 
                    string FriendCount = string.Empty;
                    string[] ArrFrdProfile = Regex.Split(PageSrcFriendCountOLd, "v=friends");
                    string strSub = ArrFrdProfile[1].Substring(0, 20);
                    string[] ArrTe = strSub.Split('(');
                    string[] ArrTe1 = ArrTe[1].Split(')');
                    FriendCount = ArrTe1[0];

                    if (!string.IsNullOrEmpty(FriendCount) && NumChk.IsMatch(FriendCount))
                    {
                        FriendCountNumber = int.Parse(FriendCount);
                    }
                    return FriendCountNumber;
                }
                else
                {

                    //** Friend Count ***************************************

                    string FriendCount = string.Empty;
                    string PageSrcFriendCount = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=friends"));     // "http://www.facebook.com/profile.php?id="

                    if (PageSrcFriendCount.Contains("fsxl"))
                    {
                        string[] arrFriendCount = Regex.Split(PageSrcFriendCount, "fsxl");
                        string strTempFriCou = arrFriendCount[1];
                        strTempFriCou = strTempFriCou.Substring(0, 20);
                        string[] arrTempFri = strTempFriCou.Split('>');
                        string[] arrTempFri1 = arrTempFri[1].Split('<');
                        FriendCount = arrTempFri1[0];

                        if (!string.IsNullOrEmpty(FriendCount) && NumChk.IsMatch(FriendCount))
                        {
                            FriendCountNumber = int.Parse(FriendCount);
                        }
                    }
                    return FriendCountNumber;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static List<string> ExtractFriendIdsFb(ref FacebookUser fbUser, ref string userID, int FriendCount)
        {

            GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;          
            List<string> lstFriendTemp = new List<string>();
            try
            {

                int i = 0;
                do
                {                 

                    string FriendUrl = FBGlobals.Instance.fbAllFriendsUIdUrl + userID + "&infinitescroll=1&location=friends_tab_tl&start=" + i + "&__user=" + userID + "&__a=1";

                    string pageScrFriend = gHttpHelper.getHtmlfromUrl(new Uri(FriendUrl));
                    if (string.IsNullOrEmpty(pageScrFriend))
                    {
                        FriendUrl = FBGlobals.Instance.fbAllFriendsUIdUrl + userID + "&infinitescroll=1&location=friends_tab&start=" + i + "&__a=1&__user=" + userID;
                    }
                    pageScrFriend = gHttpHelper.getHtmlfromUrl(new Uri(FriendUrl));
                    if (pageScrFriend.Contains("user.php?"))
                    {
                        string[] arr = Regex.Split(pageScrFriend, "user.php");
                        foreach (string strhref in arr)
                        {
                            try
                            {
                                string subStr = strhref.Substring(0, 100);
                                if (subStr.Contains("id"))
                                {

                                    string[] arrTemp = subStr.Split('=');
                                    string Temp = arrTemp[1];

                                    string[] arrTemp1 = Temp.Split('"');
                                    string Temp1 = arrTemp1[0].Replace("\\", "");

                                    if (Temp1.Contains("&amp;"))
                                    {
                                        Temp1 = Temp1.Substring(0, Temp1.IndexOf("&amp;")).Trim();
                                    }

                                    lstFriendTemp.Add(Temp1);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                    if (i < 39)
                    {

                        i = 39;
                    }
                    else
                    {
                        i = i + 16;
                    }
                } while (i < FriendCount);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return lstFriendTemp;
        }

        public void OwnExtractFriendsInformationFacebooker(ref FacebookUser fbUser, List<string> lstOwnFriendId)
        {

            GlobusHttpHelper chilkatHttpHelper = fbUser.globusHttpHelper;
            try
            {

                GlobusLogHelper.log.Debug("Logged In With UserName : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Scraping Profile Information !");   

                listFriendId.Clear();

                List<string> lstid = lstOwnFriendId.Distinct().ToList();
             


                foreach (string listFriendIditem in lstid)
                {
                    try
                    {

                        string Urls = string.Empty;
                        string id = string.Empty;
                        string name = string.Empty;
                        string first_name = string.Empty;
                        string last_name = string.Empty;
                        string link = string.Empty;
                        string gender = string.Empty;
                        string locale = string.Empty;



                        Urls = FBGlobals.Instance.fbgraphUrl + listFriendIditem + "/";          // "http://graph.facebook.com/" 
                       
                        string pageSrc = chilkatHttpHelper.getHtmlfromUrl(new Uri(Urls));                    

                        if (pageSrc.Contains("id"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("id"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                id = ArrTemp[2];
                              
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("name"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("name"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                name = ArrTemp[2];
                                
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("first_name"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("first_name"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                first_name = ArrTemp[2];
                               
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("last_name"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("last_name"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                last_name = ArrTemp[2];

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("link"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("link"), 95);
                                string[] ArrTemp = supsstring.Split('"');
                                link = ArrTemp[2];                             

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (string.IsNullOrEmpty(link))
                        {
                            link = FBGlobals.Instance.fbhomeurl + listFriendIditem;                               //"http://www.facebook.com/"                     
                        }
                        if (pageSrc.Contains("gender"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("gender"));
                                string[] ArrTemp = supsstring.Split('"');
                                gender = ArrTemp[2];                             
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("locale"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("locale"));
                                string[] ArrTemp = supsstring.Split('"');
                                locale = ArrTemp[2];                             
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }



                        string UserName = string.Empty;
                        if (pageSrc.Contains("username"))
                        {
                            try
                            {
                                string UserName1 = pageSrc.Substring(pageSrc.IndexOf("username"));
                                string[] ArrTemp = UserName1.Split('"');
                                UserName = ArrTemp[2];

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        string FBEmailId = string.Empty;
                        if (!string.IsNullOrEmpty(UserName))
                        {
                            FBEmailId = UserName + "@facebook.com";
                        }
                        else
                        {
                            FBEmailId = UserName + "@facebook.com"; ;
                        }

                        try
                        {
                            string ownprofileUrl = string.Empty;
                            if (ValidateNumber(listFriendIditem))
                            {
                                ownprofileUrl = FBGlobals.Instance.fbProfileUrl + listFriendIditem + "&sk=about";     // "http://www.facebook.com/profile.php?id="
                            }
                            else
                            {
                                ownprofileUrl = FBGlobals.Instance.fbhomeurl + listFriendIditem + "/info";       // "http://www.facebook.com/" 
                            }
                            string pagesourceofProfileUrl = chilkatHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl));
                            string _ExprotFilePath1 = string.Empty;
                         
                        
                            if (pagesourceofProfileUrl.Contains("fbTimelineSummarySectionWrapper") || (pagesourceofProfileUrl.Contains("Work and education") && pagesourceofProfileUrl.Contains("Living") || pagesourceofProfileUrl.Contains("Basic Information")))// && pagesourceofProfileUrl.Contains("Contact Information")))
                            {
                                #region commentedCode
                                //string[] hrefArr = Regex.Split(pagesourceofProfileUrl, "pagelet_timeline_summary");
                                //string allhref = string.Empty;
                                //try
                                //{
                                //    allhref = hrefArr[2].Substring(hrefArr[2].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                                //    allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 100);
                                //}
                                //catch { }
                                ////string allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                                //string[] hrefArr1 = Regex.Split(allhref, "href=");


                                //foreach (var hrefArr1item in hrefArr1) 
                                #endregion
                                {
                                    try
                                    {
                                        {
                                            string infopagesource = pagesourceofProfileUrl;
                                            string birthday = "";
                                            string language = "";
                                            string website = "";
                                            string email = "";
                                            string location = "";
                                            string jobposition = "";
                                            string jobcompany = "";
                                            string Mobile_Phones = "";
                                            string University = "";
                                            string Secondaryschool = "";
                                            string Hometown = "";
                                            string Currentlocation = "";
                                            string HighSchools = string.Empty;
                                            string Colleges = string.Empty;
                                            string Employers = string.Empty;
                                            string CurrentCitys = string.Empty;
                                            string Hometowns = string.Empty;
                                            List<string> kkk = chilkatHttpHelper.GetHrefsByTagAndAttributeName(infopagesource, "span", "fwb");

                                            try
                                            {

                                                string getTagHtml = Utils.getBetween(pagesourceofProfileUrl, "<th class=\"_3sts\">Birthday</th>", "</div></td>");
                                                string getdiv = Utils.getBetween(getTagHtml, "<div>", "</div>");
                                                birthday = getdiv.Replace(",","-");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }


                                            if (infopagesource.Contains(">High School<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            HighSchools = HighSchools + ":" + item;
                                                            
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
                                            if (infopagesource.Contains(">High School<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            HighSchools = HighSchools + ":" + item;
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
                                            if (infopagesource.Contains(">Employers<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Employers<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">Employers<");
                                                    string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            Employers = Employers + ":" + item;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                    string HighSchool = HS[0];
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (infopagesource.Contains(">College<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">College<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">College<");
                                                    string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            Colleges = Colleges + ":" + item;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                            }

                                            if (infopagesource.Contains(">Secondary school<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Secondary school<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">Secondary school<");
                                                    string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            Secondaryschool = Secondaryschool + ":" + item;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    string[] HS = Regex.Split(ArrHighSchool1[1], "<");                                                  

                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                            }

                                            if (infopagesource.Contains(">University<"))
                                            {
                                                try
                                                {
                                                    string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">University<"), 1200);
                                                    string[] ArrHighschool = Regex.Split(infopagesource1, ">University<");
                                                    string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                                                    foreach (string item in lsttd)
                                                    {
                                                        try
                                                        {
                                                            University = University + ":" + item;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                    }
                                                    string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                            }
                                            //Secondary school
                                            if (infopagesource.Contains(">Living<"))   //Current City
                                            {
                                                try
                                                {
                                                    
                                                    string[] ArrHighschool = Regex.Split(infopagesource, ">Living<");
                                                    string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");

                                                    foreach (string item in ArrHighSchool1)
                                                    {
                                                        try
                                                        {
                                                            if (item.Contains("Current City") || item.Contains("Current location"))
                                                            {
                                                                try
                                                                {
                                                                    string[] ARRCurrentCity = Regex.Split(item, "Current City");
                                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ARRCurrentCity[0], "a");
                                                                    foreach (string item1 in lsttd)
                                                                    {
                                                                        try
                                                                        {
                                                                            CurrentCitys = CurrentCitys + ":" + item1;
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
                                                            if (item.Contains("Hometown"))
                                                            {
                                                                try
                                                                {
                                                                    string[] ARRHometown = Regex.Split(item, "Hometown");
                                                                    List<string> lsttd = chilkatHttpHelper.GetDataTag(ARRHometown[0], "a");
                                                                    foreach (string item1 in lsttd)
                                                                    {
                                                                        try
                                                                        {
                                                                            Hometowns = Hometowns + ":" + item1;

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
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }

                                            if (infopagesource.Contains(">Living<") || infopagesource.Contains("Places Lived"))   //Current City
                                            {

                                                try
                                                {
                                                    string[] Home = System.Text.RegularExpressions.Regex.Split(infopagesource, "<div class=\"fsl fwb fcb\">");
                                                    Hometown = Home[0];
                                                    foreach (var Home_item in Home)
                                                    {
                                                        try
                                                        {
                                                            if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Current City"))
                                                            {
                                                                List<string> CCity = chilkatHttpHelper.GetDataTag(Home_item, "a");
                                                                CurrentCitys = CCity[0];
                                                            }
                                                            if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Hometown"))
                                                            {
                                                                List<string> Ht = chilkatHttpHelper.GetDataTag(Home_item, "a");
                                                                Hometowns = Ht[0];
                                                                Hometown = Ht[0];
                                                                Hometown = Hometown.Replace(",",string.Empty);
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

                                            try
                                            {
                                                string[] contactinfoArr = Regex.Split(infopagesource, "uiHeader uiHeaderWithImage fbTimelineAboutMeHeader");
                                                if (contactinfoArr.Count() >= 2)
                                                {
                                                    List<string> lstcontactinfoArrtd = chilkatHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                                                    foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                                                    {
                                                        try
                                                        {
                                                            if (lstcontactinfoArrtditem.Contains("Employers"))
                                                            {
                                                                Employers = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers", string.Empty).Replace(",", ";").Trim());
                                                            }

                                                            if (lstcontactinfoArrtditem.Contains("College"))
                                                            {
                                                                Colleges = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("College", string.Empty).Replace(",", ";").Trim());
                                                            }

                                                            if (lstcontactinfoArrtditem.Contains("High School"))
                                                            {
                                                                HighSchools = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("High School", string.Empty).Replace(",", ";").Trim());
                                                            }
                                                            if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                                                            {
                                                                Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());

                                                            }
                                                            if (lstcontactinfoArrtditem.Contains("Address"))
                                                            {
                                                                location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());

                                                            }
                                                            if (lstcontactinfoArrtditem.Contains("Email"))
                                                            {
                                                                email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                                                                string[] emailArr1 = Regex.Split(email, " ");
                                                                if (emailArr1.Count() >= 2)
                                                                {
                                                                    email = emailArr1[1] + emailArr1[0];

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
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            // #endregion

                                            try
                                            {
                                                if (pagesourceofProfileUrl.Contains("<td class=\"_51m- contactInfoPhone\">"))
                                                {
                                                    string phone = getBetween(pagesourceofProfileUrl, "<td class=\"_51m- contactInfoPhone\">", "</td>");
                                                    List<string> Phonelst = chilkatHttpHelper.GetDataTag(phone, "span");
                                                    Mobile_Phones = Phonelst[0].Replace(",", string.Empty);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            if (!string.IsNullOrEmpty(ExportFilePathFBIDExtractor))
                                            {
                                                try
                                                {
                                                    
                                                    string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;
                                                    string CSVHeader = "ExtractUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Birthday" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone" + "," + "UserAccont";
                                                    string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + birthday + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + CurrentCitys.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + FBEmailId.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ") + "," + fbUser.username;

                                                    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFBIDExtractor);
                                                    GlobusLogHelper.log.Info("Data Saved IN CSV File");
                                                    GlobusLogHelper.log.Debug("Data Saved IN CSV File");
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
                            else
                            {
                                if (!string.IsNullOrEmpty(ExportFilePathFBIDExtractor))
                                {
                                    try
                                    {
                                        string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;
                                       
                                        string CSVHeader = "ExtractUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";

                                        string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + ", " + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + FBEmailId.Replace(",", " ") + "," + "-".Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathFBIDExtractor);
                                        GlobusLogHelper.log.Info("Data Saved IN CSV File");
                                        GlobusLogHelper.log.Debug("Data Saved IN CSV File");                                       
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
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

               GlobusLogHelper.log.Debug("Process Completed Of Scraping Profile Information With Username >>> " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug("Error >>> ex.Message >>> " + ex.Message + " ex.StackTrace >>> " + ex.StackTrace + " With Username >>> " + fbUser.username);
            }
            GlobusLogHelper.log.Info("Process Completed Of Scraping Profile Information With Username >>> " + fbUser.username);
        }

        public static bool ValidateNumber(string strInputNo)
        {
            Regex IdCheck = new Regex("^[0-9]*$");

            if (!string.IsNullOrEmpty(strInputNo) && IdCheck.IsMatch(strInputNo))
            {
                return true;
            }

            return false;
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public void FriendProfileIdExtractor1(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

               foreach (string item in LstFBIDExtractorList)
                {
                    try
                    {
                        string UserId = string.Empty;

                        string pageSource_HomePage = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                        string fb_dtsg = string.Empty;
                        UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

                        int countFriend = ExtractFriendCount(ref fbUser, ref gHttpHelper, UserId);
                        if (countFriend==0)
                        {
                            string mm = FBGlobals.Instance.fbhomeurl + UserId;             // "https://www.facebook.com/"
                            string pageSource = gHttpHelper.getHtmlfromUrl(new Uri(mm));
                            string ss = Utils.getBetween(pageSource, "Friends<span class=\"_gs6\">", "</span>");
                            ss = ss.Replace(",",string.Empty);
                            countFriend = Convert.ToInt32(ss);
                        }

                        GlobusLogHelper.log.Debug("Please  wait ..Getting  the Friends FB Id..");
                        GlobusLogHelper.log.Info("Please  wait ..Getting  the Friends FB Id..");

                        List<string> lstFriends = ExtractFriendIdsFb(ref fbUser, ref gHttpHelper, ref UserId, countFriend);
                        lstFriends = lstFriends.Distinct().ToList();

                        List<string> lstOwnFriendId = GetFirendId(ref fbUser, ref gHttpHelper, UserId, item);
                        lstOwnFriendId.AddRange(lstFriends);
                        lstOwnFriendId = lstOwnFriendId.Distinct().ToList();
                        //FriendRequestSender.OwnFriendsExtractor obj = new OwnFriendsExtractor();
                        string posturl = FBGlobals.Instance.fbAllFriendsUrl;
                        string PostdataForFriends = "uid=" + UserId + "&infinitescroll=1&location=friends_tab_tl&start=14&nctr[_mod]=pagelet_friends&__user=" + UserId + "&__a=1&__req=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816811649895156150";
                        string response = gHttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");
                        if (string.IsNullOrEmpty(response))
                        {
                            posturl = FBGlobals.Instance.fbAllFriendsUrl;
                            response = gHttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");

                        }
                        string[] Friends = Regex.Split(response, "user.php");
                        List<string> lstnewfriendid = new List<string>();
                        foreach (string iditem in Friends)
                        {
                            try
                            {
                                if (!iditem.Contains("<!DOCTYPE html>") && !iditem.Contains("for (;;)"))
                                {
                                    try
                                    {
                                        string FriendIDS = iditem.Substring(iditem.IndexOf("id="), (iditem.IndexOf(">", iditem.IndexOf("id=")) - iditem.IndexOf("id="))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();

                                        if (FriendIDS.Contains("&amp;"))
                                        {
                                            FriendIDS = FriendIDS.Substring(0, (iditem.IndexOf("&amp;"))).Replace("id=", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("&amp;", string.Empty).Replace("&amp", string.Empty).Replace(";", string.Empty).Trim();
                                        }

                                        lstnewfriendid.Add(FriendIDS);
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
                            List<string> lsttotalajaxid = FriendsTotalId(ref fbUser, ref gHttpHelper, UserId, UserId);
                            lstOwnFriendId.AddRange(lstnewfriendid);
                            lstOwnFriendId.AddRange(lsttotalajaxid);
                            lstOwnFriendId = lstOwnFriendId.Distinct().ToList();
                            //LoggerFriendProfileUrl("TotalId : " + lstOwnFriendId.Count);
                            OwnExtractFriendsInformationFacebooker(ref fbUser, lstOwnFriendId);
                            // break;
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


        public void FriendOfFriendsProfileIdExtractor1(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper gHttpHelper = fbUser.globusHttpHelper;

                foreach (string item in LstProfileURLsFriendInfoScraper)
                {
                    try
                    {
                        string UserId = string.Empty;
                        string pageSource_HomePage = gHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                        string fb_dtsg = string.Empty;
                        UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                        if (string.IsNullOrEmpty(UserId))
                        {
                            UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                        }
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

                        GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                        GlobusLogHelper.log.Debug("Please wait finding the friends ID...");

                        List<string> lstOwnFriendId = GetAllFriends(ref fbUser, ref gHttpHelper, UserId);
                        try
                        {
                            ExtractFriendOfFriendsInformation(ref fbUser, ref gHttpHelper, lstOwnFriendId, item, UserId);
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        /// Group Scraper ///
      
        public List<string> LstOfGroupKeywords
        {
            get;
            set;
        }
        

        ///  GroupMemberScraper  ///
        

        #region Property For FriendInfoScraper

        public int NoOfThreadsGroupMemberScraper
        {
            get;
            set;
        }

        public static bool CheckScrapeCloseGroupUrlsScraper
        {
            get;
            set;
        }
        public static bool CheckScrapeOpenGroupUrlsScraper
        {
            get;
            set;
        }
        public static string ExportFilePathGroupMemberScraper
        {
            get;
            set;
        }
        public static string ExportFilePathGroupMemberScraperGroupUrls
        {
            get;
            set;
        }
        public static string ExportFilePathGroupMemberScraperTxt
        {
            get;
            set;
        }

        public static string StartProcessUsingGroupMemberScraper
        {
            get;
            set;
        }

        public static string GroupMemberScraperUsingAccount
        {
            get;
            set;
        }

        public List<string> LstGroupURLsFriendInfoScraper
        {
            get;
            set;
        }

        public int GroupUrlScraperCheckMembersMin
        {
            get;
            set;
        }
        public int GroupUrlScraperCheckMembersMax
        {
            get;
            set;
        }
        public static string ExportFilePathGroupMemberScraperByKeyWords
        {
            get;
            set;
        }
        

        #endregion

        #region Global Variables For GroupMember Scraper

        readonly object lockrThreadControllerGroupScraper = new object();
        public bool isStopGroupMemberScraper = false;
        int countThreadControllerGroupMemberScraper = 0;
        public List<Thread> lstThreadsGroupMemberScraper = new List<Thread>();

        Dictionary<string, string> DicIds = new Dictionary<string, string>();

        #endregion
     

       public void StartGroupMemberScraper()
       {
           try
           {

               countThreadControllerGroupMemberScraper = 0;
               int numberOfAccountPatch = 25;

               if (NoOfThreadsGroupMemberScraper > 0)
               {
                   numberOfAccountPatch = NoOfThreadsGroupMemberScraper;
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

                           string[] AccountArr = GroupMemberScraperUsingAccount.Split(':');
                           string Account = AccountArr[0];
                           try
                           {
                               lock (lockrThreadControllerGroupScraper)
                               {
                                   try
                                   {
                                       if (countThreadControllerGroupMemberScraper >= listAccounts.Count)
                                       {
                                           Monitor.Wait(lockrThreadControllerGroupScraper);
                                       }

                                       string acc = account.Remove(account.IndexOf(':'));
                                       if (Account==acc)
                                       {
                                           //Run a separate thread for each account

                                           FacebookUser item = null;
                                           FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                           if (item != null)
                                           {

                                               Thread GroupMemberThread = new Thread(StartMultiThreadsGroupMemberInfoScraper);
                                               GroupMemberThread.Name = "workerThread_Profiler_" + acc;
                                               GroupMemberThread.IsBackground = true;


                                               GroupMemberThread.Start(new object[] { item });

                                               countThreadControllerGroupMemberScraper++;
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



       public void StartMultiThreadsGroupMemberInfoScraper(object parameters)
       {
           try
           {
               if (!isStopFriendInfoScraper)
               {
                   try
                   {
                       lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
                       lstThreadsGroupMemberScraper.Distinct();
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


                               
                               if (StartProcessUsingGroupMemberScraper == "Group member Scraper by Urls")
                               {
                                   GetGroupMember(ref objFacebookUser);
                               }
                               else if (StartProcessUsingGroupMemberScraper == "Group url Scraper  by Keywords")
                               {
                                   GetGroupUrls(ref objFacebookUser);
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
                 //  if (!isStopGroupMemberScraper)
                   {
                       lock (lockrThreadControllerGroupScraper)
                       {
                           countThreadControllerGroupMemberScraper--;
                           Monitor.Pulse(lockrThreadControllerGroupScraper);
                       }
                   }
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
           }
       }

       public List<string> sgroupid = new List<string>();
       public static bool opengrouptype = false;
       public static bool closegrouptype = false;

       public void GetGroupUrls(ref FacebookUser fbuse)
       {


           try
           {
               lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
               lstThreadsGroupMemberScraper.Distinct();
               Thread.CurrentThread.IsBackground = true;
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

          GlobusHttpHelper chilkatHttpHelper = fbuse.globusHttpHelper;

          Dictionary<string, string> CheckDuplicates = new Dictionary<string, string>();

            string Username = fbuse.username;
            int GetCountMember =0;
        
       

            try
            {

                //  if (Selectedusername == Username)
                {

                    //GlobusLogHelper.log.Debug("Logged in  With UserName :" + Username);
                    //GlobusLogHelper.log.Info("Logged in  With UserName :" + Username);

                    foreach (string keyword in LstOfGroupKeywords)
                    {
                        try
                        {
                            GetCountMember = FindTheGroupUrls(chilkatHttpHelper, CheckDuplicates, Username, GetCountMember, keyword);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        GlobusLogHelper.log.Debug("Process Completed with : " + Username + " Keyword : " + keyword);
                        GlobusLogHelper.log.Info("Process Completed with : " + Username + " Keyword : " + keyword);

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }          
            
      }

       private int FindTheGroupUrls(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
       {

           try
           {
               lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
               lstThreadsGroupMemberScraper.Distinct();
               Thread.CurrentThread.IsBackground = true;
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           try
           {
               string strKeyword = keyword;
               string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword.Replace(" ","+") + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="

               string __user = "";
               string fb_dtsg = "";

               string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl));



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

               List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


               List<string> distinctPages = pagesList.Distinct().ToList();
               foreach (string distpage in distinctPages)
               {
                   try
                   {
                       string distpage1 = distpage.Replace("d", "groups/");
                       sgroupid.Add(distpage1);
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                   }
               }
               string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
               ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
               ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

               string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL));
               if (string.IsNullOrEmpty(res_ajaxRequest))
               {
                   string AjaxGrpUrl250 = ajaxRequestURL.Replace("pagesize=300", "pagesize=250");
                   res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl250));
                   if (string.IsNullOrEmpty(res_ajaxRequest))
                   {
                       string AjaxGrpUrl200 = ajaxRequestURL.Replace("pagesize=300", "pagesize=200");
                       res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl200));
                       if (string.IsNullOrEmpty(res_ajaxRequest))
                       {
                           string AjaxGrpUrl150 = ajaxRequestURL.Replace("pagesize=300", "pagesize=150");
                           res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl150));
                           if (string.IsNullOrEmpty(res_ajaxRequest))
                           {
                               string AjaxGrpUrl100= ajaxRequestURL.Replace("pagesize=300", "pagesize=100");
                               res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl100));
                               if (string.IsNullOrEmpty(res_ajaxRequest))
                               {
                                   string AjaxGrpUrl50 = ajaxRequestURL.Replace("pagesize=300", "pagesize=50");
                                   res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl50));
                                   if (string.IsNullOrEmpty(res_ajaxRequest))
                                   {
                                       string AjaxGrpUrl30 = ajaxRequestURL.Replace("pagesize=300", "pagesize=30");
                                       res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxGrpUrl30));
                                       if (string.IsNullOrEmpty(res_ajaxRequest))
                                       {
                                           string AjaxUrl20 = ajaxRequestURL.Replace("pagesize=300", "pagesize=20");
                                           res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxUrl20));
                                           if (string.IsNullOrEmpty(res_ajaxRequest))
                                           {
                                               string AjaxUrl = ajaxRequestURL.Replace("pagesize=300", "pagesize=10"); ;
                                               res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(AjaxUrl));
                                           }
                                       }
                                   }
                               }
                           }
                       }
                   }
               }


               string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
               List<string> list = new List<string>();
               List<string> lstLinkData = new List<string>();


               try
               {
                   foreach (string itemurl in Linklist)
                   {
                       try
                       {
                           if (!itemurl.Contains("<!DOCTYPE html"))
                           {
                               if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
                               {
                                   lstLinkData.Add(itemurl);
                                   string strLink = itemurl.Substring(0, 70);

                                   if (strLink.Contains("group") && strLink.Contains("onclick"))
                                   {
                                       try
                                       {
                                           string[] tempArr = strLink.Split('"');
                                           string temp = tempArr[1];
                                           temp = temp.Replace("\\", "");
                                           temp = "https://www.facebook.com" + temp;   // "" 
                                           list.Add(temp);
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
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }


               list = list.Distinct().ToList();

               sgroupid.AddRange(list);
               //}
               foreach (string lsturl in sgroupid)
               {
                   try
                   {
                       string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl));

                       
                       GetCountMember = GetMemberCounts(GetCountMember, findstatus);

                       if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
                       {

                           List<string> GroupType = chilkatHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
                           if (GroupType.Count()==0)
                           {
                               GroupType = chilkatHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "div", "_5mo6"); 
                           }
                           List<string> Groupmember = chilkatHttpHelper.GetDataTagAttribute(findstatus, "div", "fsm fwn fcg");

                           List<string> GroupName = chilkatHttpHelper.GetDataTagAttribute(findstatus, "a", "mrm groupsJumpTitle");

                           if (GroupType[0].Contains("Closed Group"))
                           {
                               try
                               {
                                   string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

                                   string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

                                   if (ownerlink[0].Contains("Admins"))
                                   {
                                       string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
                                       string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
                                   }
                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }
                           }
                           string NoOfGroupMember = string.Empty;
                           foreach (string item in Groupmember)
                           {
                               try
                               {
                                   if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
                                   {
                                       NoOfGroupMember = item;
                                   }
                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }
                              
                           }
                           if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
                           {
                               string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
                               if (Arr.Count()==3)
                               {
                                   try
                                   {
                                       NoOfGroupMember = Utils.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
                                   }
                                   catch (Exception ex)
                                   {
                                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                   }
                               }
                              
                           }
                           if (string.IsNullOrEmpty(NoOfGroupMember))
                           {
                               string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

                               try
                               {
                                   NoOfGroupMember = Utils.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }
                           }

                           string groupType = GroupType[0];

                           if (CheckScrapeOpenGroupUrlsScraper)
                           {
                               if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
                               {
                                   //objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
                                   GlobusLogHelper.log.Debug("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);
                                   GlobusLogHelper.log.Info("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);
                                   try
                                   {

                                       if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
                                       {

                                           string Grppurl = string.Empty;
                                           string Grpkeyword = string.Empty;
                                           string GrpTypes = string.Empty;

                                           Grppurl = lsturl;
                                           Grpkeyword = strKeyword;
                                           GrpTypes = groupType;


                                           try
                                           {
                                               CheckDuplicates.Add(Grppurl, Grppurl);


                                               string CSVHeader = "GroupUrl" + "," + "GroupSearchUrl" + ", " + "GroupTypes" + "," + "NumberOfMember";
                                               string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

                                               string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

                                               Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

                                               Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
                                               GlobusLogHelper.log.Debug("Data Export In csv File !" + CSV_Content);
                                               GlobusLogHelper.log.Info("Data Export In csv File !" + CSV_Content);


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

                               #region Codecommented
                               //try
                               //{

                               //    if (!string.IsNullOrEmpty(_ExprotFilePath))
                               //    {

                               //        string Grppurl = string.Empty;
                               //        string Grpkeyword = string.Empty;
                               //        string GrpTypes = string.Empty;
                               //        //  string GrpStatus = string.Empty;
                               //        Grppurl = lsturl;
                               //        Grpkeyword = strKeyword;
                               //        GrpTypes = groupType;
                               //        //  GrpStatus = currentstatus1;

                               //        string CSVHeader = "Grouppurl" + "," + "Groupkeyword" + ", " + "GroupTypes";// + "," + "Status";
                               //        string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes;// + "," + GrpStatus;

                               //        FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, _ExprotFilePath);

                               //    }
                               //}

                               //catch (Exception ex)
                               //{
                               //    Console.WriteLine(ex.Message);
                               //} 
                               #endregion
                           }

                           if (CheckScrapeCloseGroupUrlsScraper)
                           {
                               if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
                               {
                                   //  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
                                   GlobusLogHelper.log.Debug("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);
                                   GlobusLogHelper.log.Info("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);
                                   try
                                   {

                                       if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
                                       {

                                           string Grppurl = string.Empty;
                                           string Grpkeyword = string.Empty;
                                           string GrpTypes = string.Empty;

                                           Grppurl = lsturl;
                                           Grpkeyword = strKeyword;
                                           GrpTypes = groupType;


                                           try
                                           {
                                               CheckDuplicates.Add(Grppurl, Grppurl);


                                               string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
                                               string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

                                               string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

                                               Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

                                               Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
                                               GlobusLogHelper.log.Debug("Data Export In csv File !" + CSV_Content);
                                               GlobusLogHelper.log.Info("Data Export In csv File !" + CSV_Content);


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
                           try
                           {

                               if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
                               {
                                   string Grppurl = string.Empty;
                                   string Grpkeyword = string.Empty;
                                   string GrpTypes = string.Empty;

                                   Grppurl = lsturl;
                                   Grpkeyword = strKeyword;
                                   GrpTypes = groupType;                              


                                   try
                                   {
                                       CheckDuplicates.Add(Grppurl, Grppurl);

                                       GlobusLogHelper.log.Debug("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);
                                       GlobusLogHelper.log.Info("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

                                       string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
                                       string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

                                       string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

                                       Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

                                       Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
                                       GlobusLogHelper.log.Debug("Data Export In csv File !" + CSV_Content);
                                       GlobusLogHelper.log.Info("Data Export In csv File !" + CSV_Content);
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
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }
          

           return GetCountMember;
       }

       private static int GetMemberCounts(int GetCountMember, string findstatus)
       {
           string GetTagCountMember = Utils.getBetween(findstatus, "<div class=\"groupsAddMemberSideBox\">", "</div>");
           string GetGroupMemberCount = Utils.getBetween(findstatus, "<span id=\"count_text\">", "</span>");

           try
           {

               if (string.IsNullOrEmpty(GetGroupMemberCount))
               {
                   if (findstatus.Contains("uiHeader uiHeaderTopAndBottomBorder uiHeaderSection"))
                   {
                       try
                       {
                           string MemberCount = Utils.getBetween(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection", "</h3>");
                           MemberCount = MemberCount + "</h3>";
                           GetGroupMemberCount = Utils.getBetween(MemberCount, "<h3 class=\"accessible_elem\">", "</h3>");
                       }
                       catch (Exception ex)
                       {
                           GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                       }
                   }
                   else if (findstatus.Contains("<h6 class=\"accessible_elem\">About</h6>"))
                   {
                       try
                       {
                           string MemberCount = Utils.getBetween(findstatus, "<h6 class=\"accessible_elem\">About</h6>", "</div>");
                           MemberCount = Utils.getBetween(MemberCount, "members/\">", "</a>");
                           GetGroupMemberCount = MemberCount;
                       }
                       catch (Exception ex)
                       {
                           GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                       }
                   }
               }
           }
           catch { };
           GetGroupMemberCount = GetGroupMemberCount.Replace("members", "").Replace(",", "").Replace("Members", "").Replace("(", "").Replace(")", "").Replace("Membros","");
           try
           {
               GetCountMember = Convert.ToInt32(GetGroupMemberCount);
           }
           catch { };
           return GetCountMember;
       }

       public static List<string> GetGroups_FBSearch(string pgSrc)
       {
           List<string> lst_Pages = new List<string>();

           string splitPattern = "/hovercard/";

           string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

           foreach (string item in splitPgSrc)
           {
               if (!item.Contains("<!DOCTYPE html>"))
               {
                   try
                   {
                       if (item.Contains("group.php?id"))
                       {
                           int startIndx = item.IndexOf("group.php?id=") + "group.php?id=".Length;
                           int endIndx = item.IndexOf(">", startIndx);
                           string pageURL = FBGlobals.Instance.fbhomeurl + "groups/" + item.Substring(startIndx, endIndx - startIndx).Replace("\"", "").Replace("=", "");

                           lst_Pages.Add(pageURL);
                       }
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                   }

               }
           }

           return lst_Pages;
       }

       public static string GetAjaxURL_MoreResults(string pgSrc)
       {
           string pattern = "search/ajax/more.php?offset";

           if (pgSrc.Contains(pattern))
           {
               string URL = string.Empty;
               try
               {
                   int startIndx = pgSrc.IndexOf(pattern);
                   int endIndx = pgSrc.IndexOf("\"", startIndx);
                   URL = pgSrc.Substring(startIndx, endIndx - startIndx);
                   URL = URL.Replace("&amp;", "&").Replace("pagesize=10", "pagesize=300");//.Replace("pagesize=10", "pagesize=300")
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               return URL;
           }
           return string.Empty;
       }

       public static void myFunction()
       {
           List<string> UserIds = new List<string>();
           string Data = string.Empty;
           GlobusHttpHelper objGlobusHttpHelper=new GlobusHttpHelper();
           string PageSource = objGlobusHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/groups/22320730270/"));
           Data = Utils.getBetween(PageSource, "GroupEntstreamPagelet\\\", ", ", {");
           string[] Users_IDs = Regex.Split(PageSource,"<div class=\"_4-u2 mbm _5jmm _5pat _5v3q _5x16\" data-");
           Users_IDs = Users_IDs.Skip(1).ToArray();
           foreach (string item in Users_IDs)
           {
               UserIds.Add(Utils.getBetween(item,"/ajax/hovercard/user.php?id=","\""));
               Utils.getBetween(item,"","");
           }
       }

       public List<string> GetGroupMember(ref FacebookUser fbuse)
       {
           List<string> lstGrpMember = new List<string>();

           GlobusHttpHelper HttpHelper = fbuse.globusHttpHelper;
           string PageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));


           string __user = GlobusHttpHelper.GetParamValue(PageSource, "user");    //pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
           if (string.IsNullOrEmpty(__user))
           {
               __user = GlobusHttpHelper.ParseJson(PageSource, "user");
           }
           try
           {
               foreach (string item in LstGroupURLsFriendInfoScraper)
               {
                   try
                   {
                     new Thread(() =>
                                       {
                                           try
                                           {
                                               lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
                                               lstThreadsGroupMemberScraper.Distinct();
                                               Thread.CurrentThread.IsBackground = true;
                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                           string link = string.Empty;
                                           try
                                           {
                                               if (item.Contains("groups/"))
                                               {
                                                   GlobusLogHelper.log.Debug("Start Getting Group Members for : " + item);
                                                   GlobusLogHelper.log.Info("Start Getting Group Members for : " + item);

                                                   char lastIndexValue = item[item.Length - 1];

                                                   string realGrpUrl = string.Empty;

                                                   realGrpUrl = item;

                                                   if (lastIndexValue != '/')
                                                   {
                                                       realGrpUrl = item + "/";
                                                   }

                                                   string gid = string.Empty;

                                                   //string ddd = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com"));
                                                   //  string GroupUrl = "https://www.facebook.com/groups/PureLeverageHQ/";

                                                   //   string GetGraphRes = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/groups/248382998621966/members/"));



                                                   string pageSource = HttpHelper.getHtmlfromUrl(new Uri(realGrpUrl + "members/"));   //realGrpUrl + "members/"

                                                   try
                                                   {
                                                       if (pageSource.Contains("gid="))
                                                       {
                                                           gid = pageSource.Substring(pageSource.IndexOf("gid="), pageSource.IndexOf("&", pageSource.IndexOf("gid=")) - pageSource.IndexOf("gid=")).Replace("\"", string.Empty).Replace("gid=", string.Empty).Replace("//", string.Empty);//realGrpUrl.Substring(realGrpUrl.IndexOf("groups/"), (realGrpUrl.Length - (realGrpUrl.IndexOf("groups/")))).Replace("groups/", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(" ", string.Empty).Trim();
                                                       }
                                                       else
                                                       {
                                                           gid = realGrpUrl.Substring(realGrpUrl.IndexOf("groups/"), (realGrpUrl.Length - (realGrpUrl.IndexOf("groups/")))).Replace("groups/", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(" ", string.Empty).Trim();
                                                       }

                                                       if (!Utils.IsNumeric(gid))
                                                       {
                                                           try
                                                           {
                                                               string PageSourceGroupUrl = HttpHelper.getHtmlfromUrl(new Uri(realGrpUrl));
                                                               gid = Utils.getBetween(PageSourceGroupUrl, "group_id=", "\"");
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
                                                   
                                                   if (pageSource.Contains("/ajax/hovercard/user.php?id="))
                                                   {
                                                       string[] arrId = Regex.Split(pageSource, "/ajax/hovercard/user.php");

                                                       foreach (string item1 in arrId)
                                                       {
                                                           try
                                                           {
                                                               if (!item1.Contains("<!DOCTYPE"))
                                                               {
                                                                   if (item1.Contains("</a>"))
                                                                   {
                                                                       string idName = item1.Substring(0, item1.IndexOf("</a>"));

                                                                       if (idName.Contains(">"))
                                                                       {
                                                                           string[] arrIdName = Regex.Split(idName, ">");

                                                                           if (arrIdName.Length > 1)
                                                                           {
                                                                               try
                                                                               {
                                                                                   string id = arrIdName[0].Replace("//", string.Empty).Replace("\"", string.Empty).Replace("?id=", string.Empty).Replace("&amp;extragetparams=%7B%22hc_location%22%3A%22friend_browser%22%7D", string.Empty).Replace("&amp;extragetparams=%7B%22fref%22%3A%22grp_mmbr_list%22%7D", "").Trim();
                                                                                   if (id.Contains("&amp;"))
                                                                                   {
                                                                                       id=id.Split('&')[0];
                                                                                   }
                                                                                   string name = arrIdName[1].Replace("//", string.Empty).Replace("\"", string.Empty).Trim();
                                                                                   if (name.Contains("<img class="))
                                                                                   {
                                                                                       string PageSOurce =  HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + id));
                                                                                       string Fname = getBetween(PageSOurce, " \"first_name\": \"", "\",\n");
                                                                                       string Lname = getBetween(PageSOurce, "\"last_name\": \"", "\",\n");

                                                                                       name = string.Empty;
                                                                                       name = Fname + " " + Lname;
                                                                                   }
                                                                                   if (!id.Contains("for"))
                                                                                   {
                                                                                       DicIds.Add(id, id);

                                                                                       string profileURL=string.Empty;
                                                                                       profileURL= FBGlobals.Instance.fbProfileUrl + id;     // "https://www.facebook.com/profile.php?id=" 
                                                                                       string UserName = string.Empty;
                                                                                       string UserName1 = string.Empty;
                                                                                       Thread.Sleep(2000);
                                                                                       string GraphUrl = FBGlobals.Instance.fbgraphUrl + id;
                                                                                       string getGraphPageSource = HttpHelper.getHtmlfromUrl(new Uri(GraphUrl));
                                                                                       try
                                                                                       {
                                                                                           JObject jdata = JObject.Parse(getGraphPageSource);

                                                                                           name = (string)((JValue)jdata["name"]);
                                                                                           link = (string)((JValue)jdata["link"]);
                                                                                           UserName1 = (string)((JValue)jdata["username"]);

                                                                                           if (string.IsNullOrEmpty(link))
                                                                                           {
                                                                                               UserName = (string)((JValue)jdata["username"]);
                                                                                           }
                                                                                       }
                                                                                       catch (Exception ex)
                                                                                       {
                                                                                           GlobusLogHelper.log.Error(ex.Message);
                                                                                       }
                                                                                     //  string s = Utils.getBetween(getGraphPageSource, "<a class=\"_6-6 _6-7\" href=\"", "\">Timeline");
                                                                                    //   link = s;
                                                                                      // name = Utils.getBetween(getGraphPageSource, "<h2 class=\"_6-f\">", "</a>") + "###";
                                                                                      // name = Utils.getBetween(name, ">", "###");
                                                                                     //  if (name.Contains("<"))
                                                                                   //    {
                                                                                      //     name = "###" + name;
                                                                                      //     name = Utils.getBetween(name, "###", "<").Replace(",", "");

                                                                                      // }

                                                                                       #region CommentedGraphSerchCode
                                                                                       //string getGraphPageSource =HttpHelper.getHtmlfromUrl(new Uri( FBGlobals.Instance.fbgraphUrl + id));
                                                                                       //if (getGraphPageSource.Contains("link"))
                                                                                       //{
                                                                                       //    try
                                                                                       //    {
                                                                                       //        string supsstring = getGraphPageSource.Substring(getGraphPageSource.IndexOf("link"), 95);
                                                                                       //        string[] ArrTemp = supsstring.Split('"');
                                                                                       //        link = ArrTemp[2];


                                                                                       //    }
                                                                                       //    catch (Exception ex)
                                                                                       //    {
                                                                                       //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                                       //    }
                                                                                       //} 
                                                                                       #endregion
                                                                                       string FBEmail = string.Empty;
                                                                                       if (string.IsNullOrEmpty(link))
                                                                                       {
                                                                                           link = FBGlobals.Instance.fbhomeurl + UserName;
                                                                                           
                                                                                       }

                                                                                       if (!string.IsNullOrEmpty(UserName1))
                                                                                       {
                                                                                           FBEmail = UserName1 + "@" + "facebook.com";
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                           FBEmail = id + "@" + "facebook.com";
                                                                                       }

                                                                                      
                                                                                       if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraper))
                                                                                       {
                                                                                           try
                                                                                           {
                                                                                               string CSVHeader = "Profile URL" + "," + "ProfileLink" + "," + "Name" + "," + "facebook Email" + "," + "Group URL";
                                                                                               string CSV_Content = profileURL + "," + link + "," + name + "," + FBEmail + "," + item;
                                                                                               Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraper);

                                                                                               GlobusLogHelper.log.Info("Find ProfileLink : " + link);
                                                                                               GlobusLogHelper.log.Info("Found  Name : " + name);
                                                                                               GlobusLogHelper.log.Info("Found  GroupUrl : " + item);

                                                                                               GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                                                                               GlobusLogHelper.log.Debug("Data Saved IN CSV File");
                                                                                               GlobusLogHelper.log.Info("________________________");
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
                                                                   }
                                                               }
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               Console.WriteLine("Error >>> " + ex.StackTrace);
                                                           }
                                                       }

                                                       // Calling Ajax Fn..

                                                       int start = 96;
                                                       GetGrpMember_Ajax(ref HttpHelper, __user, gid, start, item);
                                                   }
                                                   else
                                                   {
                                                       int start = 96;
                                                       GetGrpMember_Ajax(ref HttpHelper, __user, gid, start, item);
                                                   }
                                               }
                                               else
                                               {
                                                   GlobusLogHelper.log.Debug("Group URL Not In Correct Format ! Please Follow The Format >>> http://www.facebook.com/groups/253568111433276/ ");
                                                   GlobusLogHelper.log.Info("Group URL Not In Correct Format ! Please Follow The Format >>> http://www.facebook.com/groups/253568111433276/ ");
                                               }

                                           }
                                           catch (Exception ex)
                                           {
                                               Console.WriteLine("Error >>> " + ex.StackTrace);
                                           }
                                       }).Start();
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine("Error >>> " + ex.StackTrace);
                   }
               }
           }
           catch (Exception ex)
           {
           }

           return lstGrpMember;
       }

       private void GetGrpMember_Ajax(ref GlobusHttpHelper HttpHelper, string userId, string grpId, int start, string grpUrl)
       {
           try
           {
               //int start = 96;

               bool isContinue = false;

               if (start == 10000)
               {
               }


               string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxBrowserListGroupMemberUrl + grpId + "&gid=" + grpId + "&edge=groups%3Amembers&order=default&view=grid&start=" + start + "&__user=" + userId + "&__a=1"));  // "https://www.facebook.com/ajax/browser/list/group_members/?id="

               // Not find Toatal Members Since Page Source Contains...
               // for (;;);{"__ar":1,"payload":null,"domops":[["appendContent","^div.fbProfileBrowserListContainer",true,{"__html":""}],["remove","^.morePager",true,{"__html":""}]],"jsmods":{"require":[["Arbiter","inform",[],["ProfileBrowser\/LoadMoreContent"]]]},"ixData":[]}

               if (pageSource.Contains("user.php?id="))
               {
                   isContinue = true;

                   string[] arrId = Regex.Split(pageSource, "user.php");

                   foreach (string item1 in arrId)
                   {
                       string link = string.Empty;
                       try
                       {
                           if (!item1.Contains("<!DOCTYPE"))
                           {
                               if (item1.Contains("/a>"))
                               {
                                   string idName = item1.Substring(0, item1.IndexOf("/a>"));

                                   if (idName.Contains(">"))
                                   {
                                       string[] arrIdName = Regex.Split(idName, ">");

                                       if (arrIdName.Length > 1)
                                       {
                                           string id = arrIdName[0].Replace("//", string.Empty).Replace("\"", string.Empty).Replace("?id=", string.Empty).Replace("&amp;extragetparams=%7B%22hc_location%22%3A%22friend_browser%22%7D", string.Empty).Replace("\\", string.Empty).Trim();
                                           if (id.Contains("&amp;"))
                                           {
                                               string [] arr=System.Text.RegularExpressions.Regex.Split(id,"&amp;");
                                               id=arr[0];
                                           }
                                           string name = arrIdName[1].Replace("//", string.Empty).Replace("\"", string.Empty).Replace("\\u003C\\", string.Empty).Trim();


                                           if (!id.Contains("for"))
                                           {
                                               DicIds.Add(id, id);

                                               string profileURL = FBGlobals.Instance.fbProfileUrl + id;            // "https://www.facebook.com/profile.php?id="

                                               string getGraphPageSource = HttpHelper.getHtmlfromUrl(new Uri(profileURL));

                                               string s = Utils.getBetween(getGraphPageSource, "<a class=\"_6-6 _6-7\" href=\"", "\">Timeline").Replace("\" data-tab-key=\"", "").Trim();
                                               link = s;

                                               name = Utils.getBetween(getGraphPageSource, "<h2 class=\"_6-f\">", "</a>") + "###";
                                               if (name.Contains("cover-name\">"))
                                               {
                                                   name = Utils.getBetween(name, "<span id=\"fb-timeline-cover-name\">", "<").Replace(",", "");
                                               }
                                               else
                                               {
                                                   name = Utils.getBetween(name, ">", "###");
                                               }
                                               if (name.Contains("<"))
                                               {
                                                   if (name.Contains("cover-name\">"))
                                                   {
                                                       name = Utils.getBetween(name, "<span id=\"fb-timeline-cover-name\">", "<").Replace(",", "");
                                                   }
                                                   else
                                                   {
                                                       name = "###" + name;
                                                       name = Utils.getBetween(name, "###", "<").Replace(",", "");
                                                   }

                                               }


                                               if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraper))
                                               {
                                                   try
                                                   {
                                                       string CSVHeader = "Profile URL" + "," + "ProfileLink" + "," + "Name" + "," + "Group URL";
                                                       string CSV_Content = profileURL + "," + link + "," + name + "," + grpUrl;
                                                     
                                                       Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraper);

                                                       GlobusLogHelper.log.Info("Found  ProfileLink : " + link);
                                                       GlobusLogHelper.log.Info("Found  Name : " + name);
                                                       GlobusLogHelper.log.Info("Found  GroupUrl : " + grpUrl);
                                                                
                                                       GlobusLogHelper.log.Info("Data Saved IN CSV File");

                                                       GlobusLogHelper.log.Debug("Data Saved IN CSV File");
                                                       GlobusLogHelper.log.Info("________________________");

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
                       }
                       catch (Exception ex)
                       {
                           Console.WriteLine("Error >>> " + ex.StackTrace);
                       }
                   }

                   if (isContinue)
                   {
                       System.Threading.Thread.Sleep(5 * 1000);

                       start = start + 96;

                       GetGrpMember_Ajax(ref HttpHelper, userId, grpId, start, grpUrl);
                   }

                   // Calling Ajax Fn..
               }

               GlobusLogHelper.log.Debug("Process Completed !");
               GlobusLogHelper.log.Info("Process Completed !");
           }
           catch (Exception ex)
           {
               Console.WriteLine("Error >>> " + ex.StackTrace);
           }
       }
    }

    public class CustomAudiencesScraper
    {
        #region Global Variables For CustomAudiencesscraper

        readonly object lockrThreadControllerCustomAudiencesScraper = new object();
        public static string ExportFilePathCustomAudiencesScraper = string.Empty;
        public static string ExportFilePathCustomAudiencesScraperNotepad = string.Empty;
        public static string CustomAudiencesScraperUsingAccount = string.Empty;
      //  public static string StartProcessUsingCustomAudiencesScraper = string.Empty;
        public bool isStopCustomAudiencesScraper = false;
        int countThreadControllerCustomAudiencesScraper = 0;
        public List<Thread> lstThreadsCustomAudiencesscraper = new List<Thread>();

        #endregion


        #region Property For CustomAudiencesscraper
        
        public int NoOfThreadsCustomAudiencesscraper
        {
            get;
            set;
        }
        
        public List<string> KeyWordLstCustomAudiencesscraper
        {
            get;
            set;
        }
        public List<string>UrlsLstCustomAudiencesscraper
        {
            get;
            set;
        }
        public static string StartProcessUsingCustomAudiencesScraper
        {
            get;
            set;
        }


        #endregion

        public void StartCustomAudiencesscraper()
        {
            try
            {
                int numberOfAccountPatch = 25;
                countThreadControllerCustomAudiencesScraper = 0;
                if (NoOfThreadsCustomAudiencesscraper > 0)
                {
                    numberOfAccountPatch = NoOfThreadsCustomAudiencesscraper;
                }
                string[] AccountArr = CustomAudiencesScraperUsingAccount.Split(':');
                string Account = AccountArr[0];
                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach(List<string> listAccounts in list_listAccounts)
                    {
                        //int tempCounterAccounts = 0; 

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerCustomAudiencesScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerCustomAudiencesScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerCustomAudiencesScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));
                                        if (Account == acc)
                                        {
                                            //Run a separate thread for each account
                                            FacebookUser item = null;
                                            FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                            if (item != null)
                                            {

                                                Thread profilerThread = new Thread(StartMultiThreadsCustomAudiencesscraper);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;


                                                profilerThread.Start(new object[] { item });

                                                countThreadControllerCustomAudiencesScraper++;
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


        public void StartMultiThreadsCustomAudiencesscraper(object parameters)
        {
            try
            {
               // if (!isStopFanPageScraper)
                {
                    try
                    {
                        lstThreadsCustomAudiencesscraper.Add(Thread.CurrentThread);
                        lstThreadsCustomAudiencesscraper.Distinct();
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

                                //Login Process
                                Accounts.AccountManager objAccountManager = new AccountManager();
                                objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);                            
                            }
                            if (objFacebookUser.isloggedin)
                            {
                                if (StartProcessUsingCustomAudiencesScraper == "Custom Scraper  by KeyWords")
                                {
                                    StartActionAudiencesScraper(ref objFacebookUser);
                                }
                                else if (StartProcessUsingCustomAudiencesScraper == "Custom Scraper by Urls")
                                {
                                    StartActionAudiencesScraperByUrls(ref objFacebookUser);
                                }
                                else if (StartProcessUsingCustomAudiencesScraper == "Custom Groups Scraper by Urls")
                                {
                                    StartActionAudiencesGroupsScraperByUrls(ref objFacebookUser);
                                }
                                else if (StartProcessUsingCustomAudiencesScraper == "Custom Groups Scraper by Keywords")
                                {
                                    StartActionAudiencesScraperGroupUsingKeywords(ref objFacebookUser);
                                }
                                
                                // Call StartActionEventInviter
                                
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            finally
            {
                try
                {
                    //if (!isStopFanPageScraper)
                    {
                        lock (lockrThreadControllerCustomAudiencesScraper)
                        {
                            countThreadControllerCustomAudiencesScraper--;
                            Monitor.Pulse(lockrThreadControllerCustomAudiencesScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }
        //Group
        public void StartActionAudiencesGroupsScraperByUrls(ref FacebookUser fbUser)
        {
            string pageSource_Home = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

            string UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
            }
            foreach (string UrlsLstCustomAudiencesscraper_item in UrlsLstCustomAudiencesscraper)
            {
                try
                {

                    string searchURL = UrlsLstCustomAudiencesscraper_item;
                    if (!searchURL.Contains("keywords_groups"))
                    {
                        if (searchURL.Contains("keywords"))
                        {
                            try
                            {
                                string temp = searchURL.Split('_')[1];
                                searchURL = searchURL.Replace(temp, "groups");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.Message);
                            }
                        }
                        else
                        {
                            searchURL = searchURL + "keywords_groups";
                        }
                    }
                    string pageSource_Search = HttpHelper.getHtmlfromUrl(new Uri(searchURL));
                    string searchResult = string.Empty;
                    searchResult = searchURL;
                    GetGroupUrls_Ajax(pageSource_Search, ref fbUser, UserId, searchResult);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }

        }

        private void GetGroupDetails(ref FacebookUser FBuser, List<string> ListGroupUrl)
        {
            EventScraper objEventScraper = new EventScraper();
            try
            {
                GlobusHttpHelper HttpHelper = FBuser.globusHttpHelper;
                foreach (var ListGroupUrl_item in ListGroupUrl)
                {
                    try
                    {
                        string GroupName = string.Empty;
                        string GroupUrl = ListGroupUrl_item;
                        string GroupId = string.Empty;
                        int MemberCount = 0; 

                        string PageSource = HttpHelper.getHtmlfromUrl(new Uri(ListGroupUrl_item));
                        try
                        {
                            MemberCount = GetMemberCounts(PageSource);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        GroupId = Utils.getBetween(PageSource, "group_id=", "\"");

                        GroupName = Utils.getBetween(PageSource, "<title id=\"pageTitle\">", "</title>");

                        GlobusLogHelper.log.Debug("Found  Group Name : " + GroupName);
                        GlobusLogHelper.log.Info("Found  Group Name : " + GroupName);

                        GlobusLogHelper.log.Debug("Found  Group ID : " + GroupId);
                        GlobusLogHelper.log.Info("Found  Group ID : " + GroupId);

                        GlobusLogHelper.log.Debug("Found  Group Number of member : " + MemberCount);
                        GlobusLogHelper.log.Info("Found  Group Number of member : " + MemberCount);

                        if (!string.IsNullOrEmpty(ExportFilePathCustomAudiencesScraper))
                        {
                            try
                            {
                                string CSVHeader ="GroupUrl"+","+"ID"+ ","+"Group Name"+","+"GroupMemberCount";

                                string CSV_Content = GroupUrl + "," + GroupId + "," + GroupName + "," + MemberCount;                             

                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathCustomAudiencesScraper);




                                GlobusLogHelper.log.Info("Data Saved IN CSV File");
                                GlobusLogHelper.log.Debug("Data Saved IN CSV File");

                                GlobusLogHelper.log.Info("                                            ");
                                GlobusLogHelper.log.Debug("                                           ");
                                GlobusLogHelper.log.Info(" _________________________  ");
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

        private static int GetMemberCounts( string findstatus)
        {
            int GetCountMember = 0;
            string GetTagCountMember = Utils.getBetween(findstatus, "<div class=\"groupsAddMemberSideBox\">", "</div>");
            string GetGroupMemberCount = Utils.getBetween(findstatus, "<span id=\"count_text\">", "</span>");

            if (string.IsNullOrEmpty(GetGroupMemberCount))
            {
                if (findstatus.Contains("uiHeader uiHeaderTopAndBottomBorder uiHeaderSection"))
                {
                    try
                    {
                        string MemberCount = Utils.getBetween(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection", "</h3>");
                        MemberCount = MemberCount + "</h3>";
                        GetGroupMemberCount = Utils.getBetween(MemberCount, "<h3 class=\"accessible_elem\">", "</h3>");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else if (findstatus.Contains("<h6 class=\"accessible_elem\">About</h6>"))
                {
                    try
                    {
                        string MemberCount = Utils.getBetween(findstatus, "<h6 class=\"accessible_elem\">About</h6>", "</div>");
                        MemberCount = Utils.getBetween(MemberCount, "members/\">", "</a>");
                        GetGroupMemberCount = MemberCount;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            if (string.IsNullOrEmpty(GetGroupMemberCount))
            {
                 return GetCountMember;
            }
           
             GetGroupMemberCount = GetGroupMemberCount.Replace("members", "").Replace(",", "").Replace("Members", "").Replace("(", "").Replace(")", "");     
            try
            {
                GetCountMember = Convert.ToInt32(GetGroupMemberCount);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return GetCountMember;
        }

        private void GetGroupUrls_Ajax(string pageSource, ref FacebookUser FBuser, string UserID, string searchResult)
        {

           List<string>ListGroupUrl=GetGroups_FBSearch(pageSource);

             GlobusHttpHelper HttpHelper = FBuser.globusHttpHelper;
           // group Parse

             GetGroupDetails(ref FBuser, ListGroupUrl);

           
            string AjaxPageSource = string.Empty;
            List<string> ProfileUrlList = new List<string>();
            List<string> ProfileUrlListPagination = new List<string>();
            string encoded_title = string.Empty;
            string encoded_query = string.Empty;
            string filter_id = string.Empty;
            string Cursor = string.Empty;
            try
            {
                bool isContinue = false;
                //encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"{\"bqf\":\"", "\"");
                try
                {
                    encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "\"");  //\",
                    // 
                    if (encoded_query.Contains(":\\\"") || encoded_query.Contains("{"))
                    {
                        encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "vertical");
                        encoded_query = Utils.getBetween(encoded_query, ":\\\"", "\\\"");
                    }
                    string[] encodedQueries = Regex.Split(pageSource, "encoded_query");
                    encoded_query = Utils.getBetween(encodedQueries[1], "{", "}");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.Message);
                }


                if (string.IsNullOrEmpty(encoded_query))
                {
                    encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"{\\\"bqf\\\":\\\"", "\\\"");
                }
                encoded_title = Utils.getBetween(pageSource, "encoded_title\":\"", "\"");
                filter_id = Utils.getBetween(pageSource, "filter_ids\":", "},") + "}";
                pageSource = pageSource.Replace("\\\"", "\"").Replace("\\", "");
                Cursor = Utils.getBetween(pageSource, "cursor\":\"", "\"");


                int i = 2;
                while (true)
                 {
                    List<string> NewListLcal = new List<string>();
                    bool CheckCursor = false;
                    //Edited By Mahesh 29-12-2014
                    encoded_query = "{" + encoded_query + "}";
                    encoded_query = encoded_query.Replace("{{", "{").Replace("}}", "}");                   
                    string[] ArrLocal = System.Text.RegularExpressions.Regex.Split(pageSource, "BrowseScrollingPager");
                    string Data = string.Empty;
                    string SearchData = string.Empty;
                    if (searchResult.Contains("http"))
                    {
                        SearchData = Utils.getBetween(searchResult, "search/", "keywords_groups");
                        SearchData = SearchData + "keywords_groups";
                    }
                   // Data = Uri.EscapeDataString("{\"view\":\"list\",\"encoded_query\":\"" + encoded_query + "\",\"encoded_title\":\"" + encoded_title + "\",\"ref\":\"unknown\",\"logger_source\":\"www_main\",\"typeahead_sid\":\"\",\"tl_log\":false,\"impression_id\":\"89ec6c8d\",\"filter_ids\":" + filter_id + ",\"experience_type\":\"grammar\",\"exclude_ids\":null,\"browse_location\":\"\",\"trending_source\":null,\"cursor\":\"" + Cursor + "\"}");
                    Data = Uri.EscapeDataString("{\"view\":\"list\",\"encoded_query\":\"" + encoded_query + "\",\"encoded_title\":\"" + encoded_title + "\",\"ref\":\"top_filter\",\"logger_source\":\"www_main\",\"typeahead_sid\":\"\",\"tl_log\":false,\"impression_id\":\"dcd588ab\",\"filter_ids\":" + filter_id + ",\"experience_type\":\"grammar\",\"exclude_ids\":null,\"browse_location\":\"\",\"trending_source\":null,\"ref_path\":\"/search/" + SearchData + "\",\"is_trending\":false,\"topic_id\":null,\"story_id\":null,\"cursor\":\"" + Cursor + "\",\"page_number\":" + i + "}");
                  //  string AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + Data + "&__user=" + UserID + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKexm48jhHx2Vo&__req=b&__rev=1396250";
                    string AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + Data + "&__user=" + UserID + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKexm48jhHx2Vo&__req=b&__rev=1396250";
                    AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + Data + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUW5ogDyQqUkBBzEy6Kdy8-&__req=2q&__rev=1543317";

                    AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri(AjaxUrl));
                    i++;
                    List<string> ListGroupUrlNextPage = new List<string>() ;
                    pageSource = AjaxPageSource;
                    if (AjaxPageSource.Contains("cursor\":\""))
                    {
                        Cursor = Utils.getBetween(AjaxPageSource, "cursor\":\"", "\"");
                        CheckCursor = true;
                    }
                    else
                    {
                        CheckCursor = false;
                    }
                   // group Parse


                    string[] GropData = Regex.Split(AjaxPageSource, "8o _8s lfloat _ohe");
                    ArrLocal=ArrLocal.Skip(1).ToArray();
                    string temp = string.Empty;
                    foreach (string GroupURL in GropData)
                    {
                        try
                        {
                            string ProfileLinkUrl = Utils.getBetween(GroupURL, "href=\\\"", "\"");
                            string[] TempData = Regex.Split(ProfileLinkUrl, "\\/");
                            ProfileLinkUrl = TempData[2].Replace("\\", string.Empty);
                            // ProfileLinkUrl = ProfileLinkUrl;
                            ProfileLinkUrl = "https://www.facebook.com/groups/"+ProfileLinkUrl;
                            ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                            ListGroupUrlNextPage.Add(ProfileLinkUrl);
                        }
                        catch
                        { }
                    }
                    ListGroupUrlNextPage = ListGroupUrlNextPage.Distinct().ToList();
                    ListGroupUrlNextPage.Remove("");
                    GetGroupDetails(ref FBuser, ListGroupUrlNextPage);
                    ProfileUrlList = ProfileUrlList.Distinct().ToList();

                    if (ProfileUrlList.Count > 1500 || AjaxPageSource.Contains("<div class=\"phm _64f\">End of results</div>\n\nbrowse_end_of_results_footer") || !CheckCursor)
                    {
                        break;
                    }
                 
                }
               
                GlobusLogHelper.log.Debug("Process Completed !");
                GlobusLogHelper.log.Info("Process Completed !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error >>> " + ex.StackTrace);
            }
        }

        public static List<string> GetGroupsAjax_FBSearch(string pgSrc)
        {
            List<string> lstGroups = new List<string>();

            string splitPattern = "  clearfix _zw";

            string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

            foreach (string item in splitPgSrc)
            {
                if (!item.Contains("<!DOCTYPE html>"))
                {
                    try
                    {
                        if (item.Contains("<a href=\"/groups/"))
                        {
                            int startIndx = item.IndexOf("<a href=\"/groups/") + "/".Length;
                            int endIndx = item.IndexOf(">", startIndx);
                            string pageURL = FBGlobals.Instance.fbhomeurl + "groups/" + item.Substring(startIndx, endIndx - startIndx).Replace("\"", "").Replace("=", "");

                            lstGroups.Add(pageURL);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
            }

            return lstGroups;
        }

        public static List<string> GetGroups_FBSearch(string pgSrc)
        {
            List<string> lstGroups = new List<string>();

            string splitPattern = "<div class=\"_42ef\">";

            string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

            foreach (string item in splitPgSrc)
            {
                if (!item.Contains("<!DOCTYPE html>"))
                {
                    try
                    {
                        if (item.Contains("<a href=\"/groups/"))
                        {
                            string pageURL = FBGlobals.Instance.fbhomeurl + "groups/" + Utils.getBetween(item, "<a href=\"/groups/", "?ref=br_rs");

                            lstGroups.Add(pageURL);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
            }

            return lstGroups;
        }

        public void StartActionAudiencesScraperByUrls(ref FacebookUser fbUser)
        {
            string pageSource_Home = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

            string UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
            }
            foreach (string UrlsLstCustomAudiencesscraper_item in UrlsLstCustomAudiencesscraper)
            {
                try
                {
                    string pageSource_Search = string.Empty;
                    string searchURL = UrlsLstCustomAudiencesscraper_item;
                    if (!string.IsNullOrEmpty(fbUser.proxyip))
                    {
                        pageSource_Search = HttpHelper.getHtmlfromUrlProxy(new Uri(searchURL), fbUser.proxyip, Convert.ToInt32(fbUser.proxyport), fbUser.proxyusername, fbUser.proxypassword);
                    }
                    else
                    {
                        pageSource_Search = HttpHelper.getHtmlfromUrl(new Uri(searchURL));
                    }
                    string searchResult = searchURL + "-";
                    searchResult = Utils.getBetween(searchResult, "str/", "-");
                    GetGrpMember_Ajax(pageSource_Search, ref fbUser, UserId, searchURL, searchResult);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }

        }

        public void StartActionAudiencesScraper(ref FacebookUser fbUser)
        {
            string pageSource_Home = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            
            pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

            string UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
            }
            foreach (string KeyWordLstCustomAudiencesscraper_item in KeyWordLstCustomAudiencesscraper)
            {
                try
                {
                    
                    string searchURL = "https://www.facebook.com/typeahead/search/facebar/query/?value=[%22" + Uri.EscapeDataString(KeyWordLstCustomAudiencesscraper_item) + "%22]&context=facebar&grammar_version=90a525db12a8700dec0db939c6cb250e4f8e8de2&viewer="+UserId+"&rsp=search&qid=1&max_results=10&sid=0.666867780033499&__user="+UserId+"&__a=1&__dyn=7n8ahyj35zoSt2u6aWizG85oCiq78hyWgSmEVFLFwxBxCbzElx24QqUgKm&__req=a&__rev=1380031";
                    string pageSource_Search = HttpHelper.getHtmlfromUrl(new Uri(searchURL));
                    string searchResult=string.Empty;;
                    string CustPageSource = string.Empty;
                    if (pageSource_Search.Contains("semantic"))
                    {

                        searchResult = Utils.getBetween(pageSource_Search, "\"semantic\":\"", "\",\"cost");
                        if (searchResult.Contains("u0025"))
                        {
                              searchResult = searchResult.Replace("u00252B","%20");
                        }


                        string[] URLArr = searchResult.Split('(',',');
                        string query = "https://www.facebook.com/search/str";
                        for (int i = URLArr.Length-1; i >= 0;i-- )
                        {
                            query += "/"+URLArr[i];
                        }
                        query=query.Replace(")","");
                        if (query.Contains("str\\/"))
                        {
                            query = query.Replace("str\\/", string.Empty);
                        }
                        query = query.Replace("\\", string.Empty);
                        query = query.Replace("u0025", "%");
                        query = query.Replace("keywords_top", "keywords_users");
                        CustPageSource = HttpHelper.getHtmlfromUrl(new Uri(query));
                    }
                  

                    GetGrpMember_Ajax(CustPageSource, ref fbUser,UserId,searchResult,"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }         

        }

        public void StartActionAudiencesScraperGroupUsingKeywords(ref FacebookUser fbUser)
        {
            string pageSource_Home = string.Empty;
            string GrammarVersion = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

            string UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
            }            
            GrammarVersion = Utils.getBetween(pageSource_Home, "grammar_version\":\"", "\"");

            foreach (string KeyWordLstCustomAudiencesscraper_item in KeyWordLstCustomAudiencesscraper)
            {
                try
                {
                    string searchURL = "https://www.facebook.com/typeahead/search/facebar/query/?value=[%22" + Uri.EscapeDataString(KeyWordLstCustomAudiencesscraper_item.Replace("\"","")) + "%22]&context=facebar&grammar_version=" + GrammarVersion + "&viewer=" + UserId + "&rsp=search&qid=2&max_results=10&sid=0.15900980797596276&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizG85oCiq78hyWgSmEVFLFwxBxCbzElx24QqUgKm&__req=a&__rev=1380031";
                    string pageSource_Search = HttpHelper.getHtmlfromUrl(new Uri(searchURL));
                    string searchResult = string.Empty; ;
                    string CustPageSource = string.Empty;
                    if (pageSource_Search.Contains("semantic"))
                    {

                        searchResult = Utils.getBetween(pageSource_Search, "\"semantic\":\"", "\",\"cost");
                        string[] URLArr = searchResult.Split('(', ',');
                        string query = "https://www.facebook.com/search/str";
                        for (int i = URLArr.Length - 1; i >= 0; i--)
                        {
                            query += "/" + URLArr[i];
                            
                        }                        
                        query = query.Replace(")", "");
                        if (query.Contains("str\\/"))
                        {
                            query = query.Replace("str\\/", string.Empty);
                        }
                        query = query.Replace("\\", string.Empty);
                        query = query.Replace("u0025", "%");
                        query = query.Replace("/keywords_top", "/keywords_groups");
                        CustPageSource = HttpHelper.getHtmlfromUrl(new Uri(query));
                    }


                    GetGroupUrls_Ajax(CustPageSource, ref fbUser, UserId, searchResult);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }

        }

        private void getProfileUrls(string pageSource, ref FacebookUser FBuser)
        {
            List<string> ProfileUrlList = new List<string>();
            try
            {
                string[] arrId = Regex.Split(pageSource, "_zs fwb");
                if (arrId.Count()==1)
                {
                   arrId = Regex.Split(pageSource, "clearfix _zw"); 
                }
                foreach (var arrId_item in arrId)
                {
                    try
                    {
                        if (!arrId_item.Contains("<!DOCTYPE html>"))
                        {
                            string ProfileLink = Utils.getBetween(arrId_item,"<a href=\"","?ref=br_rs&amp;");
                            if (ProfileLink.Contains("www.facebook.com"))
                            {
                                ProfileUrlList.Add(ProfileLink);   
                            }
                                                    
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                ProfileUrlList = ProfileUrlList.Distinct().ToList();
                if (ProfileUrlList.Count != 0)
                {
                    ExtractUserInformationFacebooker(ref FBuser, ProfileUrlList);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }        
        }

        private void GetGrpMember_Ajax(string pageSource, ref FacebookUser FBuser, string UserID, string searchResult, string refererUrl)
        {
            GlobusHttpHelper HttpHelper = FBuser.globusHttpHelper;
            string AjaxPageSource = string.Empty;
            List<string> ProfileUrlList = new List<string>();
            List<string> ProfileUrlListPagination = new List<string>();
            string encoded_title = string.Empty;
            string encoded_query = string.Empty;
            string filter_id = string.Empty;
            string Cursor=string.Empty;
         //   string searchResult = string.Empty;

            string newdata = Utils.getBetween(pageSource,"{\"view\":\"list","display_params\":[]},");
            newdata = "{\"view\":\"list"+newdata+"display_params\":[]}";
            string newdata1 = Uri.EscapeDataString(newdata);
            newdata1 = newdata1.Replace("%5Cu002520", "%252B");
            string get = Utils.getBetween(newdata1, "ref_path", "keywords_users");
            string newget=get.Replace("%5C","");
            newdata1 = newdata1.Replace(get,newget);
            newdata1 = newdata1.Replace("%5B", "[").Replace("%5D","]");
           string[] newdata2=newdata1.Split(']');
            newdata1=newdata2[0]+"]";
            Cursor = Utils.getBetween(pageSource, "cursor\":\"", "\"");
            int ij= 2;
            string Url = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + newdata1 + "%2C%22cursor%22%3A%22" + Cursor + "%22%2C%22page_number%22%3A" + ij + "%2C%22em%22%3Afalse%2C%22mr%22%3Afalse%2C%22tr%22%3Anull%7D&__user=" + UserID + "&__a=1&__dyn=7nmajEyl35xKt2u6aOGeFxq9ACxO4oKAdy8VFLO0xBxembzES2N6xybxu3fzoaUjUkUgx-Gy9E&__req=k&__rev=1674690";
            try
            {
                bool isContinue = false;
                encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "\"");  //\",
               // 
                if (encoded_query.Contains(":\\\"") || encoded_query.Contains("{"))
                {
                    encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "vertical");
                     encoded_query = Utils.getBetween(encoded_query, ":\\\"", "\\\"");
                }
                string[] encodedQueries = Regex.Split(pageSource, "encoded_query");
                encoded_query = Utils.getBetween(encodedQueries[1], "{", "}");
                encoded_title = Utils.getBetween(pageSource, "encoded_title\":\"", "\"");
                filter_id = Utils.getBetween(pageSource,"filter_ids\":","},")+"}";
                pageSource = pageSource.Replace("\\\"", "\"").Replace("\\", "");


                if (pageSource.Contains("user.php?id=") || pageSource.Contains("browse_search") || pageSource.Contains("type_self_timeline"))
                    {
                        isContinue = true;

                       // string[] arrId = Regex.Split(pageSource, "browse_search");
                        string[] arrId = Regex.Split(pageSource, "_8o _8s lfloat _ohe");  //changed by Mahesh 24-12-2014
                        foreach (var arrId_item in arrId)
                        {
                            try
                            {
                                if (!arrId_item.Contains("<html><body><script type") && arrId_item.Contains("data-bt=") && !arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                                {
                                    try
                                    {
                                        string ProfileLinkUrl = Utils.getBetween(arrId_item, "href=\"", "\"").Replace("?ref=br_rs", string.Empty).Replace("&amp;ref=br_rs",string.Empty);
                                       // ProfileLinkUrl = ProfileLinkUrl;
                                        ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else if (!arrId_item.Contains("<html><body><script type") && arrId_item.Contains("data-bt=") && arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                                {
                                    try
                                    {
                                        string ProfileLinkUrl = Utils.getBetween(arrId_item, "_7kf _8o _8s lfloat _ohe", "?ref").Replace("\"", "").Replace("href=", "");
                                       // ProfileLinkUrl = ProfileLinkUrl;
                                        ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        GlobusLogHelper.log.Info("Found  The Profile Link : " + ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        GlobusLogHelper.log.Debug("Found  The Profile Link : " + ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
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
                        if (pageSource.Contains("cursor\":\""))
                        {
                            Cursor = Utils.getBetween(pageSource, "cursor\":\"", "\"");
                        }
                        ProfileUrlList = ProfileUrlList.Distinct().ToList();
                        ProfileUrlList.Remove("");
                        if (ProfileUrlList.Count != 0)
                        {
                            ExtractUserInformationFacebooker(ref FBuser, ProfileUrlList);
                        }
                       

                       
                    }
                int i = 2;
                while (true)
                {
                    List<string> NewListLcal = new List<string>();
                    encoded_query = "{" + encoded_query + "}";
                    encoded_query = encoded_query.Replace("{{", "{").Replace("}}", "}");
                    bool CheckCursor = false;
                    string[] ArrLocal = System.Text.RegularExpressions.Regex.Split(pageSource, "BrowseScrollingPager");
                    string Data = string.Empty;                  
                    //Eddited By Mahesh 24-12-2014
                    searchResult=searchResult.Replace("\\",string.Empty).Replace("u0025","%").Replace("keywords_users","keywords_top");
                    //Data = Uri.EscapeDataString("{\"view\":\"list\",\"encoded_query\":\""+encoded_query+"\",\"encoded_title\":\""+encoded_title+"\",\"ref\":\"unknown\",\"logger_source\":\"www_main\",\"typeahead_sid\":\"\",\"tl_log\":false,\"impression_id\":\"89ec6c8d\",\"filter_ids\":"+filter_id+",\"experience_type\":\"grammar\",\"exclude_ids\":null,\"browse_location\":\"\",\"trending_source\":null,\"cursor\":\""+Cursor+"\"}");
                    string impression_id = Utils.getBetween(pageSource, "impression_id", "filter_ids").Replace("\\&quot;", "").Replace(":", "").Replace("\\\\","").Replace("\\","").Replace(",","");
                    Data = Uri.EscapeDataString("{\"view\":\"list\",\"encoded_query\":\""+encoded_query+"\",\"encoded_title\":\""+encoded_title+"\",\"ref\":\"top_filter\",\"logger_source\":\"www_main\",\"typeahead_sid\":\"\",\"tl_log\":false,\"impression_id\":\"dcd588ab\",\"filter_ids\":"+filter_id+",\"experience_type\":\"grammar\",\"exclude_ids\":null,\"browse_location\":\"\",\"trending_source\":null,\"ref_path\":\"//search//"+searchResult+"\",\"is_trending\":false,\"topic_id\":null,\"story_id\":null,\"cursor\":\""+Cursor+"\",\"page_number\":"+i+"}");
                   i++;
                    string AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data="+Data+"&__user="+UserID+"&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKexm48jhHx2Vo&__req=b&__rev=1396250";
                    AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + Data + "&__user=" + UserID + "&__a=1&__dyn=7nmajEyl2qm9o-t2u5bHaEWCueyp9Esx6iqAdy9VCC_826m4XUKezo88J6xybxu3fzoaUjUkUKi7WQ&__req=2q&__rev=1543317";
                  //  AjaxUrl = AjaxUrl.Replace("(", "%28").Replace(")", "%29");
                 
                    if (!string.IsNullOrEmpty(FBuser.proxyip))
                    {
                        if (string.IsNullOrEmpty(FBuser.proxyport))
                        {
                            FBuser.proxyport = "0";
                        }
                        AjaxPageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(AjaxUrl), FBuser.proxyip, Convert.ToInt32(FBuser.proxyport), FBuser.proxyusername, FBuser.proxypassword);
                    }
                    else
                    {
                      //  string JustTest = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/chat/hovercard/sidebar.php?ids[0]=100004100903864&ids[1]=100004408057272&ids[2]=100003232738289&ids[3]=100006609976116&ids[4]=100004882864541&ids[5]=100008197008802&ids[6]=100001717051982&ids[7]=100001433269126&__user=100001006024349&__a=1&__dyn=7nmajEyl2lm9o-t2u5bHaEWCueyp9Esx6iqAdy9VCC_826m4XUKezo88J6xybxu3fzoaUjUkUKi4EOGy9E&__req=h&__rev=1673637"));
                      //  AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri(AjaxUrl),refererUrl);
                        AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri(AjaxUrl));
                    }
                   
                    pageSource = AjaxPageSource;
                    if (AjaxPageSource.Contains("cursor\":\""))
                    {
                        Cursor = Utils.getBetween(AjaxPageSource, "cursor\":\"", "\"");
                        CheckCursor = true;
                    }
                    else
                    {

                        CheckCursor = false;
                    }
                    //if (pageSource.Contains("user.php?id=") || pageSource.Contains("browse_search")) //need to work from Here
                    if (AjaxPageSource.Contains("8o _8s lfloat _ohe") || AjaxPageSource.Contains("browse_search") || AjaxPageSource.Contains("user.php?id="))
                    {
                        isContinue = true;
                        AjaxPageSource = AjaxPageSource.Replace("\\", "").Replace("u003C", "<");
                        string[] arrId = Regex.Split(AjaxPageSource, "8o _8s lfloat _ohe");
                        if (arrId.Count()==2)
                        {
                            arrId = Regex.Split(AjaxPageSource, "_8o _8s lfloat _ohe");

                        }

                 

                        foreach (var arrId_item in arrId)
                        {
                            try
                            {
                                if (!arrId_item.Contains("<html><body><script type") && arrId_item.Contains("data-bt=") && !arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                                {
                                    try
                                    {
                                        string ProfileLinkUrl = Utils.getBetween(arrId_item, "<a href=\"", "=br_rs&amp;fref=");
                                        if (string.IsNullOrEmpty(ProfileLinkUrl))
                                        {
                                            ProfileLinkUrl = Utils.getBetween(arrId_item, "href=\\\"", "\"");                                           
                                        }
                                        ProfileLinkUrl=ProfileLinkUrl.Replace("\\/","/");
                                        ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        NewListLcal.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("=br_rs\\","").Replace("?", "").Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else if (!arrId_item.Contains("<html><body><script type") && !arrId_item.Contains("for (;;)") && arrId_item.Contains("data-bt=") && arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                                {
                                    try
                                    {
                                        string ProfileLinkUrl = Utils.getBetween(arrId_item, "_7kf _8o _8s lfloat _ohe", "?ref").Replace("\"", "").Replace("href=", "").Replace("\\", "");
                                        ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        NewListLcal.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Replace("=br_rs\\","").Trim());
                                        GlobusLogHelper.log.Info("Found  The Profile Link : " + ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        GlobusLogHelper.log.Debug("Found  The Profile Link : " + ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
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
                        ProfileUrlList = ProfileUrlList.Distinct().ToList();
                        ProfileUrlList.Remove("");
                        NewListLcal = NewListLcal.Distinct().ToList();
                        NewListLcal.Remove("");
                        if (NewListLcal.Count != 0)
                        {
                            ExtractUserInformationFacebooker(ref FBuser, NewListLcal);
                        }
                        #region MyRegion

                        //foreach (string item1 in arrId)
                        //{
                        //    string link = string.Empty;
                        //    try
                        //    {
                        //        if (!item1.Contains("<!DOCTYPE"))
                        //        {
                        //            if (item1.Contains("/a>"))
                        //            {
                        //                string idName = item1.Substring(0, item1.IndexOf("/a>"));

                        //                if (idName.Contains(">"))
                        //                {
                        //                    string[] arrIdName = Regex.Split(idName, ">");

                        //                    if (arrIdName.Length > 1)
                        //                    {
                        //                        string id = arrIdName[0].Replace("//", string.Empty).Replace("\"", string.Empty).Replace("?id=", string.Empty).Replace("&amp;extragetparams=%7B%22hc_location%22%3A%22friend_browser%22%7D", string.Empty).Replace("\\", string.Empty).Trim();
                        //                        string name = arrIdName[1].Replace("//", string.Empty).Replace("\"", string.Empty).Replace("\\u003C\\", string.Empty).Trim();


                        //                        if (!id.Contains("for"))
                        //                        {
                        //                           // DicIds.Add(id, id);

                        //                            string profileURL = FBGlobals.Instance.fbProfileUrl + id;            // "https://www.facebook.com/profile.php?id="

                        //                           //string getGraphPageSource = HttpHelper.getHtmlfromUrl(new Uri(profileURL));

                        //                           // string s = Utils.getBetween(getGraphPageSource, "<a class=\"_6-6 _6-7\" href=\"", "\">Timeline");
                        //                          //  link = s;

                        //                          //  name = Utils.getBetween(getGraphPageSource, "<h2 class=\"_6-f\">", "</a>") + "###";
                        //                          //  name = Utils.getBetween(name, ">", "###");
                        //                            if (name.Contains("<"))
                        //                            {
                        //                                name = "###" + name;
                        //                                name = Utils.getBetween(name, "###", "<").Replace(",", "");

                        //                            }


                        //                           // if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraper))
                        //                            {
                        //                                //try
                        //                                //{
                        //                                //    string CSVHeader = "Profile URL" + "," + "ProfileLink" + "," + "Name" + "," + "Group URL";
                        //                                //    string CSV_Content = profileURL + "," + link + "," + name + "," + grpUrl;

                        //                                //    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraper);

                        //                                //    GlobusLogHelper.log.Info("Data Saved IN CSV File");

                        //                                //    GlobusLogHelper.log.Debug("Profile Info Saved In CSV");
                        //                                //}
                        //                                //catch (Exception ex)
                        //                                //{
                        //                                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //                                //}
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Console.WriteLine("Error >>> " + ex.StackTrace);
                        //    }
                        //  } 
                        #endregion
                    }
                    else
                    {
                    }
                    if (ProfileUrlList.Count > 1500 || AjaxPageSource.Contains("<div class=\"phm _64f\">End of results</div>\n\nbrowse_end_of_results_footer") || !CheckCursor)
                    {
                        break;
                    }
                }
                GlobusLogHelper.log.Debug("Process Completed !");
                GlobusLogHelper.log.Info("Process Completed !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error >>> " + ex.StackTrace);
            }
        }

        public void ExtractUserInformationFacebooker(ref FacebookUser fbUser, List<string> lstFriendId)
        {
            GlobusHttpHelper chilkatHttpHelper = fbUser.globusHttpHelper;
            try
            {


            
                GlobusLogHelper.log.Debug("Start Scraping Profile Information !");

                List<string> lstid = lstFriendId.Distinct().ToList();

                foreach (string listFriendIditem in lstid)
                {
                    try
                    {
                      
                        string Urls = string.Empty;
                        string id = string.Empty;
                        string name = string.Empty;
                        string first_name = string.Empty;
                        string last_name = string.Empty;
                        string link = string.Empty;
                        string gender = string.Empty;
                        string locale = string.Empty;
                        string ProfileId = string.Empty;
                        string UrlProfile = string.Empty;
                        if (listFriendIditem.Contains("/profile.php"))
                        {
                            ProfileId = Utils.getBetween(listFriendIditem + "@@@@@@@", "=", "@@@@@@@").Replace(";", "").Trim();


                            UrlProfile ="http://graph.facebook.com/"+ ProfileId.Replace("https://www.facebook.com/", FBGlobals.Instance.fbgraphUrl).Replace("?ref/about", "/").Replace("=br_rs", string.Empty);
                        }
                        else
                        {
                            UrlProfile = listFriendIditem.Replace("https://www.facebook.com/", FBGlobals.Instance.fbgraphUrl).Replace("?ref/about", "/").Replace("=br_rs", string.Empty);
                        }
                        Urls = UrlProfile;// FBGlobals.Instance.fbgraphUrl + listFriendIditem + "/";          // "http://graph.facebook.com/" 
                        Urls = Urls.Replace(";=br_rs", string.Empty);      //Add By Mahesh 
                        Urls = Urls.Replace("profile.phpid=", string.Empty);  //Add By MAhesh
                        Urls = Urls.Replace("=br_rs", string.Empty);

                        GlobusLogHelper.log.Info("Scraping Url : " + Urls);
                        GlobusLogHelper.log.Debug("Scraping Url : " + Urls);
                        string pageSrc = string.Empty;
                        if (!string.IsNullOrEmpty(fbUser.proxyip))
                        {
                            pageSrc = chilkatHttpHelper.getHtmlfromUrlProxy(new Uri(Urls), fbUser.proxyip, Convert.ToInt32(fbUser.proxyport), fbUser.proxyusername, fbUser.proxypassword);
                        }
                        else
                        {
                           pageSrc = chilkatHttpHelper.getHtmlfromUrl(new Uri(Urls));
                        }
                        
                        if (pageSrc.Contains("id"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("id"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                id = ArrTemp[2];

                                GlobusLogHelper.log.Debug("Found  the ID : " + id);
                                GlobusLogHelper.log.Info("Found  the ID : " + id);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("name"))
                        {
                            try
                            {
                                string supsstring=string.Empty;
                                try
                                {
                                    supsstring = pageSrc.Substring(pageSrc.IndexOf("\"name\": \""), 30).Replace("\"name\": \"", "").Replace("\",\n", "").Replace("\"\n}",string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.Message);
                                }
                                if (supsstring.Contains('"'))
                                {
                                    string[] ArrTemp = supsstring.Split('"');
                                    try
                                    {
                                        name = ArrTemp[2];
                                    }
                                    catch (Exception ex)
                                    {
                                        name = ArrTemp[0];
                                    }
                                }
                                else
                                {
                                    name = supsstring;
                                }
                                if (string.IsNullOrEmpty(name))
                                {
                                    name = Utils.getBetween(pageSrc, "\"name\": \"", "\"");
                                }
                                GlobusLogHelper.log.Debug("Found  the Name : " + name);
                                GlobusLogHelper.log.Info("Found  the Name : " + name);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("first_name"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("first_name"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                first_name = ArrTemp[2];
                                GlobusLogHelper.log.Debug("Found  the First Name : " + first_name);
                                GlobusLogHelper.log.Info("Found  the First Name : " + first_name);


                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("last_name"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("last_name"), 30);
                                string[] ArrTemp = supsstring.Split('"');
                                last_name = ArrTemp[2];

                                GlobusLogHelper.log.Debug("Found  the Last Name : " + last_name);
                                GlobusLogHelper.log.Info("Found  the Last Name : " + last_name);


                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("link"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("link"), 95);
                                string[] ArrTemp = supsstring.Split('"');
                                link = ArrTemp[2];

                                GlobusLogHelper.log.Debug("Found  the LinkUrl : " + link);
                                GlobusLogHelper.log.Info("Found  the LinkUrl : " + link);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (string.IsNullOrEmpty(link))
                        {
                            link = listFriendIditem.Replace("=br_rs",string.Empty);                              
                        }
                        if (pageSrc.Contains("gender"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("gender"));
                                string[] ArrTemp = supsstring.Split('"');
                                gender = ArrTemp[2];

                                GlobusLogHelper.log.Debug("Found  the Gender : " + gender);
                                GlobusLogHelper.log.Info("Found  the Gender : " + gender);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        if (pageSrc.Contains("locale"))
                        {
                            try
                            {
                                string supsstring = pageSrc.Substring(pageSrc.IndexOf("locale"));
                                string[] ArrTemp = supsstring.Split('"');
                                locale = ArrTemp[2];

                                GlobusLogHelper.log.Debug("Found  the Locale : " + locale);
                                GlobusLogHelper.log.Info("Found  the Locale : " + locale);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        try
                        {


                            string UserName = string.Empty;
                            if (pageSrc.Contains("username"))
                            {
                                try
                                {
                                    string UserName1 = pageSrc.Substring(pageSrc.IndexOf("username"));
                                    string[] ArrTemp = UserName1.Split('"');
                                    UserName = ArrTemp[2];

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            string FBEmailId = string.Empty;
                            if (!string.IsNullOrEmpty(UserName))
                            {
                                FBEmailId = UserName + "@facebook.com";
                            }
                            else
                            {
                                FBEmailId = ProfileId.Replace("=br_rs",string.Empty) + "@facebook.com"; ;
                            }


                            if (string.IsNullOrEmpty(name))
                            {
                                name = first_name + " " + last_name;
                            }
                            string ownprofileUrl = string.Empty;
                            if (ValidateNumber(listFriendIditem))
                            {
                                ownprofileUrl = FBGlobals.Instance.fbProfileUrl + listFriendIditem + "&sk=about";     // "http://www.facebook.com/profile.php?id="
                            }
                            else
                            {
                                ownprofileUrl = listFriendIditem.Replace("=br_rs","") + "/info";      
                            }
                            #region MyRegion
                            //string pagesourceofProfileUrl = chilkatHttpHelper.getHtmlfromUrl(new Uri(ownprofileUrl));
                            //string _ExprotFilePath1 = string.Empty;


                            //if (pagesourceofProfileUrl.Contains("fbTimelineSummarySectionWrapper") || (pagesourceofProfileUrl.Contains("Work and education") && pagesourceofProfileUrl.Contains("Living") || pagesourceofProfileUrl.Contains("Basic Information")))// && pagesourceofProfileUrl.Contains("Contact Information")))
                            //{
                            //    #region commentedCode
                            //    //string[] hrefArr = Regex.Split(pagesourceofProfileUrl, "pagelet_timeline_summary");
                            //    //string allhref = string.Empty;
                            //    //try
                            //    //{
                            //    //    allhref = hrefArr[2].Substring(hrefArr[2].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                            //    //    allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 100);
                            //    //}
                            //    //catch { }
                            //    ////string allhref = hrefArr[3].Substring(hrefArr[3].IndexOf("fbTimelineSummarySectionWrapper"), 2000);
                            //    //string[] hrefArr1 = Regex.Split(allhref, "href=");


                            //    //foreach (var hrefArr1item in hrefArr1) 
                            //    #endregion
                            //    {
                            //        try
                            //        {
                            //            {
                            //                string infopagesource = pagesourceofProfileUrl;
                            //                string birthday = "";
                            //                string language = "";
                            //                string website = "";
                            //                string email = "";
                            //                string location = "";
                            //                string jobposition = "";
                            //                string jobcompany = "";
                            //                string Mobile_Phones = "";
                            //                string University = "";
                            //                string Secondaryschool = "";
                            //                string Hometown = "";
                            //                string Currentlocation = "";
                            //                string HighSchools = string.Empty;
                            //                string Colleges = string.Empty;
                            //                string Employers = string.Empty;
                            //                string CurrentCitys = string.Empty;
                            //                string Hometowns = string.Empty;
                            //                List<string> kkk = chilkatHttpHelper.GetHrefsByTagAndAttributeName(infopagesource, "span", "fwb");

                            //                try
                            //                {

                            //                    string getTagHtml = Utils.getBetween(pagesourceofProfileUrl, "<th class=\"_3sts\">Birthday</th>", "</div></td>");
                            //                    string getdiv = Utils.getBetween(getTagHtml, "<div>", "</div>");
                            //                    birthday = getdiv.Replace(",", "-");
                            //                }
                            //                catch (Exception ex)
                            //                {
                            //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                }


                            //                if (infopagesource.Contains(">High School<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                HighSchools = HighSchools + ":" + item;

                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }

                            //                }
                            //                if (infopagesource.Contains(">High School<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">High School<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">High School<");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                HighSchools = HighSchools + ":" + item;
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }

                            //                }
                            //                if (infopagesource.Contains(">Employers<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Employers<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">Employers<");
                            //                        string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                Employers = Employers + ":" + item;
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                        string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                            //                        string HighSchool = HS[0];
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }
                            //                }

                            //                if (infopagesource.Contains(">College<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">College<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">College<");
                            //                        string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                Colleges = Colleges + ":" + item;
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                        string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }

                            //                }

                            //                if (infopagesource.Contains(">Secondary school<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">Secondary school<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">Secondary school<");
                            //                        string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                Secondaryschool = Secondaryschool + ":" + item;
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                        string[] HS = Regex.Split(ArrHighSchool1[1], "<");

                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }

                            //                }

                            //                if (infopagesource.Contains(">University<"))
                            //                {
                            //                    try
                            //                    {
                            //                        string infopagesource1 = infopagesource.Substring(infopagesource.IndexOf(">University<"), 1200);
                            //                        string[] ArrHighschool = Regex.Split(infopagesource1, ">University<");
                            //                        string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");
                            //                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ArrHighschool[1], "span");
                            //                        foreach (string item in lsttd)
                            //                        {
                            //                            try
                            //                            {
                            //                                University = University + ":" + item;
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                        string[] HS = Regex.Split(ArrHighSchool1[1], "<");
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }

                            //                }
                            //                //Secondary school
                            //                if (infopagesource.Contains(">Living<"))   //Current City
                            //                {
                            //                    try
                            //                    {

                            //                        string[] ArrHighschool = Regex.Split(infopagesource, ">Living<");
                            //                        string[] ArrHighSchool1 = Regex.Split(ArrHighschool[1], "<span class=\"fwb\">");

                            //                        foreach (string item in ArrHighSchool1)
                            //                        {
                            //                            try
                            //                            {
                            //                                if (item.Contains("Current City") || item.Contains("Current location"))
                            //                                {
                            //                                    try
                            //                                    {
                            //                                        string[] ARRCurrentCity = Regex.Split(item, "Current City");
                            //                                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ARRCurrentCity[0], "a");
                            //                                        foreach (string item1 in lsttd)
                            //                                        {
                            //                                            try
                            //                                            {
                            //                                                CurrentCitys = CurrentCitys + ":" + item1;
                            //                                            }
                            //                                            catch (Exception ex)
                            //                                            {
                            //                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                                            }
                            //                                        }
                            //                                    }
                            //                                    catch (Exception ex)
                            //                                    {
                            //                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                                    }
                            //                                }
                            //                                if (item.Contains("Hometown"))
                            //                                {
                            //                                    try
                            //                                    {
                            //                                        string[] ARRHometown = Regex.Split(item, "Hometown");
                            //                                        List<string> lsttd = chilkatHttpHelper.GetDataTag(ARRHometown[0], "a");
                            //                                        foreach (string item1 in lsttd)
                            //                                        {
                            //                                            try
                            //                                            {
                            //                                                Hometowns = Hometowns + ":" + item1;

                            //                                            }
                            //                                            catch (Exception ex)
                            //                                            {
                            //                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                                            }
                            //                                        }
                            //                                    }
                            //                                    catch (Exception ex)
                            //                                    {
                            //                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                                    }
                            //                                }

                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }

                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }
                            //                }

                            //                if (infopagesource.Contains(">Living<") || infopagesource.Contains("Places Lived"))   //Current City
                            //                {

                            //                    try
                            //                    {
                            //                        string[] Home = System.Text.RegularExpressions.Regex.Split(infopagesource, "<div class=\"fsl fwb fcb\">");
                            //                        Hometown = Home[0];
                            //                        foreach (var Home_item in Home)
                            //                        {
                            //                            try
                            //                            {
                            //                                if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Current City"))
                            //                                {
                            //                                    List<string> CCity = chilkatHttpHelper.GetDataTag(Home_item, "a");
                            //                                    CurrentCitys = CCity[0];
                            //                                }
                            //                                if (!Home_item.Contains("<!DOCTYPE html>") && Home_item.Contains("Hometown"))
                            //                                {
                            //                                    List<string> Ht = chilkatHttpHelper.GetDataTag(Home_item, "a");
                            //                                    Hometowns = Ht[0];
                            //                                    Hometown = Ht[0];
                            //                                    Hometown = Hometown.Replace(",", string.Empty);
                            //                                }

                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }
                            //                }

                            //                try
                            //                {
                            //                    string[] contactinfoArr = Regex.Split(infopagesource, "uiHeader uiHeaderWithImage fbTimelineAboutMeHeader");
                            //                    if (contactinfoArr.Count() >= 2)
                            //                    {
                            //                        List<string> lstcontactinfoArrtd = chilkatHttpHelper.GetDataTag(contactinfoArr[1], "tr");
                            //                        foreach (string lstcontactinfoArrtditem in lstcontactinfoArrtd)
                            //                        {
                            //                            try
                            //                            {
                            //                                if (lstcontactinfoArrtditem.Contains("Employers"))
                            //                                {
                            //                                    Employers = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Employers", string.Empty).Replace(",", ";").Trim());
                            //                                }

                            //                                if (lstcontactinfoArrtditem.Contains("College"))
                            //                                {
                            //                                    Colleges = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("College", string.Empty).Replace(",", ";").Trim());
                            //                                }

                            //                                if (lstcontactinfoArrtditem.Contains("High School"))
                            //                                {
                            //                                    HighSchools = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("High School", string.Empty).Replace(",", ";").Trim());
                            //                                }
                            //                                if (lstcontactinfoArrtditem.Contains("Mobile Phones"))
                            //                                {
                            //                                    Mobile_Phones = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Mobile Phones", string.Empty).Replace(",", ";").Trim());

                            //                                }
                            //                                if (lstcontactinfoArrtditem.Contains("Address"))
                            //                                {
                            //                                    location = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Address", string.Empty).Replace(",", ";").Trim());

                            //                                }
                            //                                if (lstcontactinfoArrtditem.Contains("Email"))
                            //                                {
                            //                                    email = System.Net.WebUtility.HtmlDecode(lstcontactinfoArrtditem.Replace("Email", string.Empty).Replace(",", ";").Trim());
                            //                                    string[] emailArr1 = Regex.Split(email, " ");
                            //                                    if (emailArr1.Count() >= 2)
                            //                                    {
                            //                                        email = emailArr1[1] + emailArr1[0];

                            //                                    }
                            //                                }
                            //                            }
                            //                            catch (Exception ex)
                            //                            {
                            //                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //                catch (Exception ex)
                            //                {
                            //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                }

                            //                // #endregion

                            //                try
                            //                {
                            //                    if (pagesourceofProfileUrl.Contains("<td class=\"_51m- contactInfoPhone\">"))
                            //                    {
                            //                        string phone =Utils.getBetween(pagesourceofProfileUrl, "<td class=\"_51m- contactInfoPhone\">", "</td>");
                            //                        List<string> Phonelst = chilkatHttpHelper.GetDataTag(phone, "span");
                            //                        Mobile_Phones = Phonelst[0].Replace(",", string.Empty);
                            //                    }
                            //                }
                            //                catch (Exception ex)
                            //                {
                            //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                }

                            //                if (!string.IsNullOrEmpty(ExportFilePathCustomAudiencesScraper))
                            //                {
                            //                    try
                            //                    {

                            //                        string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;
                            //                        string CSVHeader = "ExtractUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Birthday" + "," + "Link" + "," + "Gender" + "," + "Locale" + "," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone" + "," + "UserAccont";
                            //                        string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + birthday + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + Hometowns.Replace(",", " ") + "," + CurrentCitys.Replace(",", " ") + ", " + Employers.Replace(",", " ") + "," + University.Replace(",", " ") + "," + Secondaryschool.Replace(",", " ") + "," + HighSchools.Replace(",", " ") + "," + email.Replace(",", " ") + "," + Mobile_Phones.Replace(",", " ") + "," + fbUser.username;

                            //                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathCustomAudiencesScraper);
                            //                        GlobusLogHelper.log.Info("Data Saved IN CSV File");
                            //                        GlobusLogHelper.log.Debug("Data Saved IN CSV File");
                            //                    }
                            //                    catch (Exception ex)
                            //                    {
                            //                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            //        }
                            //    }
                            //}
                            //else 
                            #endregion
                            {
                                if (!string.IsNullOrEmpty(ExportFilePathCustomAudiencesScraper))
                                {
                                    try
                                    {
                                        string commaSeparatedData = id + "," + name + "," + first_name + "," + last_name + "," + link + "," + gender + "," + locale;

                                        string CSVHeader = "ExtractUrl" + "," + "Id" + "," + "Name" + ", " + "FirstName" + "," + "LastName" + "," + "Link" + "," + "Gender" + "," + "Locale"+","+"FBEmails";// +"," + "HomeTown" + "," + "CurrentLocation" + "," + "Employer" + "," + "University" + "," + "Secondary School" + "," + "HighSchool " + "," + "College" + "," + "Email" + "," + "Telephone";

                                        //   string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + ", " + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ") + "," + "-".Replace(",", " ");// +"," + jobcompany + "," + infohref + "," + Username + "," + Hometown + "," + Currentlocation + "," + University + "," + Secondaryschool;
                                        string CSV_Content = link.Replace(",", " ") + "," + id.Replace(",", " ") + "," + name.Replace(",", " ") + "," + first_name.Replace(",", " ") + "," + last_name.Replace(",", " ") + "," + link.Replace(",", " ") + "," + gender.Replace(",", " ") + "," + locale.Replace(",", " ")+","+FBEmailId;

                                       
                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathCustomAudiencesScraper);

                                        string CSVHeaderNotPade = "ID";
                                        string CSV_ContentNotPade = id;

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeaderNotPade, CSV_ContentNotPade, ExportFilePathCustomAudiencesScraperNotepad);

                                        

                                        GlobusLogHelper.log.Info("Data Saved IN CSV File");
                                        GlobusLogHelper.log.Debug("Data Saved IN CSV File");

                                        GlobusLogHelper.log.Info("                                            ");
                                        GlobusLogHelper.log.Debug("                                           ");
                                        GlobusLogHelper.log.Info(" _________________________  ");
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
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

              //  GlobusLogHelper.log.Debug("Process Completed Of Scraping Profile Information With Username >>> " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug("Error >>> ex.Message >>> " + ex.Message + " ex.StackTrace >>> " + ex.StackTrace + " With Username >>> " + fbUser.username);
            }
           // GlobusLogHelper.log.Info("Process Completed Of Scraping Profile Information With Username >>> " + fbUser.username);
        }

        public static bool ValidateNumber(string strInputNo)
        {
            Regex IdCheck = new Regex("^[0-9]*$");

            if (!string.IsNullOrEmpty(strInputNo) && IdCheck.IsMatch(strInputNo))
            {
                return true;
            }

            return false;
        }
    }
}
