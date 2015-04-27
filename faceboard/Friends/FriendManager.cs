using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BaseLib;
using faceboardpro;
using Accounts;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Friends
{
    public class FriendManager
    {
        #region Global Variables Of Friend Manager

        readonly object requestFriendsThreadControllerlockr = new object();

        public bool isRequestFriendsStop = false;

        int requestFriendsThreadControllerCount = 0;

        public List<Thread> lstRequestFriendsThreads = new List<Thread>();
        public List<string> lstRequestFriendsLocation = new List<string>();
        public List<string> lstRequestFriendsProfileURLs = new List<string>();
        public List<string> lstRequestFriendsKeywords = new List<string>();
        public List<string> lstRequestFriendsFanPageURLs = new List<string>();
        public List<string> lstFriendIds = new List<string>();


        public static int minDelayFriendManager = 10;
        public static int maxDelayFriendManager = 20;

        public static int NoOfFriendRequestFriendManager = 10;
        public static bool Friends_AcceptFriends_Female = false;
        public static bool Friends_AcceptFriends_Male= false;
        public static bool Friends_AcceptSendFrndToSuggestions = false;
        public static string Friends_AcceptSendFrndProcessUsing = string.Empty;
        public static bool Friends_AcceptFriends_CheckTargeted = false;

        public static string FilePathFriendrequestFilePath = string.Empty;
        public static string FilePathFailedFriendRequestFilePath = string.Empty;

        #endregion

        #region Property

        public static bool IsSearchViaKeywords
        {
            get;
            set;
        }

        public static bool IsSearchViaProfileUrls
        {
            get;
            set;
        }

        public static bool IsSearchViaLocation
        {
            get;
            set;
        }

        public static bool IsSearchViaFanPageURLs
        {
            get;
            set;
        }

        public static string SendRequestUsing
        {
            get;
            set;
        }

        public static string Keywords
        {
            get;
            set;
        }

        public static string Location
        {
            get;
            set;
        }

        public static string FanPageURLs
        {
            get;
            set;
        }

        public static int NoOfThreads
        {
            get;
            set;
        }

        public static int NoOfFriendsRequest
        {
            get;
            set;
        }
        public static int NoOfFriendsRequestPerUser
        {
            get;
            set;
        }
        public static int NoOfFriendsRequestParKeyWord
        {
            get;
            set;
        }

        public Queue<string> QueueFriendsProfileUrls = new Queue<string>();

        #endregion

        public FriendManager()
        {
        }


        public void StartSendFriendRequest()
        {
            requestFriendsThreadControllerCount = 0;
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreads > 0)
                {
                    numberOfAccountPatch = NoOfThreads;
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
                                lock (requestFriendsThreadControllerlockr)
                                {
                                    try
                                    {
                                        if (requestFriendsThreadControllerCount >= listAccounts.Count)
                                        {
                                            Monitor.Wait(requestFriendsThreadControllerlockr);
                                        }
                                        try
                                        {
                                            string acc = account.Remove(account.IndexOf(':'));
                                            //Run a separate thread for each account
                                            FacebookUser item = null;
                                            FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                            if (item != null)
                                            {
                                                Thread profilerThread = new Thread(SendFriendRequestMultiThreads);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;
                                                profilerThread.Start(new object[] { item });
                                                requestFriendsThreadControllerCount++;
                                                //tempCounterAccounts++; 
                                            }
                                        }
                                        catch(Exception ex)
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
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void SendFriendRequestMultiThreads(object parameters)
        {
            try
            {
                if (!isRequestFriendsStop)
                {
                    try
                    {
                        lstRequestFriendsThreads.Add(Thread.CurrentThread);
                        lstRequestFriendsThreads.Distinct();
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
                                        // Call SendFriendRequests
                                        SendFriendRequests(ref objFacebookUser);
                                    }
                                    else
                                    {
                                       // GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                        GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                                    }                               

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            //GlobusLogHelper.log.Debug("Process completed !!");
                            //GlobusLogHelper.log.Info("Process completed !!");

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
                   // if (!isRequestFriendsStop)
                    {
                        lock (requestFriendsThreadControllerlockr)
                        {
                            requestFriendsThreadControllerCount--;
                            Monitor.Pulse(requestFriendsThreadControllerlockr);

                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }



        public List<string> GetProfileIdViaKeyWord(ref FacebookUser fbUser, string item_keyword, string UserId)
        {
            List<string> Id_List = new List<string>();
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
                string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + item_keyword + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";

                string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL));
                List<string> list = new List<string>();

                list.Clear();
                List<string> lstLinkData = new List<string>();
                lstLinkData.Clear();
                string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");
                string profileID = string.Empty;
                foreach (string itemurl in Linklist)
                {
                    try
                    {
                        if (!itemurl.Contains("<!DOCTYPE html"))
                        {
                            if (itemurl.Contains("is_friend&quot;:false"))
                            {
                                lstLinkData.Add(itemurl);
                                try
                                {
                                    if (itemurl.Contains("&quot;"))
                                    {
                                        try
                                        {
                                            profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
                                            profileID = profileID.Replace(",", "");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    else
                                    {
                                        profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
                                    }

                                    string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
                                    list.Add(profileURL);
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


                List<string> FriendLink = list.Distinct().ToList();
                Id_List = FriendLink;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Id_List;
        }




        public void SendFriendRequestViaKeywords(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
                int countFriendRequestsSentAllKeyWord = 0;

                if (!IsSearchViaKeywords)
                {
                    return;
                }

                string UserId = string.Empty;

                string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

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

                string keyword = string.Empty;

                if (!string.IsNullOrEmpty(Keywords))
                {
                    keyword = FriendManager.Keywords;
                    lstRequestFriendsKeywords.Add(keyword);
                }
                else if (lstRequestFriendsKeywords.Count > 0)
                {
               
                    try
                    {
                        keyword = lstRequestFriendsKeywords[Utils.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }


                lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();

                foreach (var item_keyword in lstRequestFriendsKeywords)
                {
                    string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + item_keyword + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";

                    string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL));
                    List<string> list = new List<string>();
                    #region for find friend Reqest Link
                    list.Clear();
                    List<string> lstLinkData = new List<string>();
                    lstLinkData.Clear();
                    string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");
                    string profileID = string.Empty;
                    foreach (string itemurl in Linklist)
                    {
                        try
                        {
                            if (!itemurl.Contains("<!DOCTYPE html"))
                            {
                                if (itemurl.Contains("is_friend&quot;:false"))
                                {
                                    lstLinkData.Add(itemurl);
                                    try
                                    {
                                        if (itemurl.Contains("&quot;"))
                                        {
                                            try
                                            {
                                                profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
                                                profileID = profileID.Replace(",", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        else
                                        {
                                            profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
                                        }

                                        string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
                                        list.Add(profileURL);
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
                    #endregion

                    List<string> FriendLink = list.Distinct().ToList();

                    GlobusLogHelper.log.Info(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);
                    GlobusLogHelper.log.Debug(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

                    int countFriendRequestsSent = 0;
                    int counterforblockedFriendrequest = 0;
                    foreach (string FriendRequestLink in FriendLink)
                    {
                        try
                        {
                            if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
                            {

                                return;
                            }
                            if (countFriendRequestsSent >= NoOfFriendsRequestParKeyWord)
                            {
                                break;
                            }

                            GlobusLogHelper.log.Info(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
                            bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);

                            if (requeststatus)
                            {
                                countFriendRequestsSent++;
                                countFriendRequestsSentAllKeyWord++;
                                counterforblockedFriendrequest = 1;
                                GlobusLogHelper.log.Info(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);



                                if (!string.IsNullOrEmpty(FilePathFriendrequestFilePath))
                                {


                                    string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                    string CSV_Content = fbUser.username + "," + FriendRequestLink + ", " + "Request Sent";
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, FilePathFriendrequestFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }



                              //  if (countFriendRequestsSent == NoOfFriendsRequestPerUser && NoOfFriendsRequestPerUser != 0)
                              //  {
                              //      return;
                              //  }

                            }
                            else
                            {

                                if (!string.IsNullOrEmpty(FilePathFailedFriendRequestFilePath))
                                {


                                    string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                    string CSV_Content = fbUser.username + "," + FriendRequestLink + ", " + "Request Sent";
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, FilePathFailedFriendRequestFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                //counterforblockedFriendrequest++;
                                //if (counterforblockedFriendrequest == 3)
                                //{
                                //    break;
                                //}
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

        public void SendFriendRequestViaLocation(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

                if (!IsSearchViaLocation)
                {
                    return;
                }
                string UserId = string.Empty;
                string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
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

                string Location = string.Empty;

                if (!string.IsNullOrEmpty(FriendManager.Location))
                {
                    lstRequestFriendsLocation.Clear();
                    Location = FriendManager.Location;
                    lstRequestFriendsLocation.Add(Location);
                }
                else if (lstRequestFriendsLocation.Count > 0)
                {
                    Location = lstRequestFriendsLocation[Utils.GenerateRandom(0, lstRequestFriendsLocation.Count)];
                }
                int countFriendRequestsSentKeywordAll = 1;

                foreach (var Location_item in lstRequestFriendsLocation)
                {
                    try
                    {
                        #region for find friend Reqest Link
                        //string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + Location_item + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";

                        //string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL));

                        //List<string> list = new List<string>();

                        //
                        //list.Clear();
                        //List<string> lstLinkData = new List<string>();
                        //lstLinkData.Clear();
                        //string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");

                        //string profileID = string.Empty;

                        //foreach (string itemurl in Linklist)
                        //{
                        //    try
                        //    {
                        //        if (!itemurl.Contains("<!DOCTYPE html"))
                        //        {
                        //            if (itemurl.Contains("is_friend&quot;:false"))
                        //            {
                        //                lstLinkData.Add(itemurl);
                        //                try
                        //                {

                        //                    if (itemurl.Contains("&quot;"))
                        //                    {
                        //                        try
                        //                        {
                        //                            profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
                        //                            profileID = profileID.Replace(",", "");
                        //                        }
                        //                        catch (Exception ex)
                        //                        {
                        //                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                 
                        //                        profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
                        //                    }

                        //                    string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
                        //                    list.Add(profileURL);
                        //                }
                        //                catch (Exception ex)
                        //                {
                        //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //                }
                        //            }
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        //    }
                        //}
                        #endregion

                       // GetMemberViaLocation

                        List<string> FriendLink = GetMemberViaLocation(ref fbUser, Location_item);


                        FriendLink = FriendLink.Distinct().ToList();


                        if (FriendLink.Count()==0)
                        {
                            FriendLink = GetProfileIdViaKeyWord(ref fbUser, Location_item,UserId);
                        }

                        GlobusLogHelper.log.Info("Find "+FriendLink.Count +" Number Of Friends From "+ " Keyword : " + Location_item + " Search Friend Requests Url with Email " + fbUser.username);
                        GlobusLogHelper.log.Debug("Find " + FriendLink.Count + " Number Of Friends From " + " Keyword : " + Location_item + " Search Friend Requests Url with Email " + fbUser.username); 

                        int countFriendRequestsSent = 0;
                        int counterforblockedFriendrequest = 0;
                        foreach (string FriendRequestLink in FriendLink)
                        {

                            //if (countFriendRequestsSentKeywordAll > NoOfFriendsRequest)
                            //{
                            //    return;
                            //}
                            if (NoOfFriendsRequestParKeyWord <= countFriendRequestsSent)
                            {
                                break;
                            }


                            GlobusLogHelper.log.Info(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username + "Keyword : " + Location_item);
                            GlobusLogHelper.log.Debug(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username + "Keyword : " + Location_item);
                            bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);

                            if (requeststatus)
                            {
                                countFriendRequestsSent++;
                                counterforblockedFriendrequest = 0;
                                GlobusLogHelper.log.Info(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username + "Keyword : " + Location_item);
                                GlobusLogHelper.log.Debug(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username + "Keyword : " + Location_item);

                                if (!string.IsNullOrEmpty(FilePathFriendrequestFilePath))
                                {


                                    string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                    string CSV_Content = fbUser.username + "," + FriendRequestLink + ", " + "Request Sent";
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, FilePathFriendrequestFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }




                                int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            else
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                                //counterforblockedFriendrequest++;
                                //if (counterforblockedFriendrequest == 3)
                                //{
                                //    break;
                                //}


                                if (!string.IsNullOrEmpty(FilePathFailedFriendRequestFilePath))
                                {


                                    string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                    string CSV_Content = fbUser.username + "," + FriendRequestLink + ", " + "Failed Request Sent";
                                    try
                                    {

                                        Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, FilePathFailedFriendRequestFilePath);
                                        GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                        GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
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



        public List<string> GetMemberViaLocation(ref FacebookUser fbUser,string KeyWord)
        {
            List<string> FriendsList = new List<string>();
            string pageSource_Home = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

            string UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
            }
           // foreach (string KeyWordLstCustomAudiencesscraper_item in KeyWordLstCustomAudiencesscraper)
            {
                try
                {
                    string searchURL = "https://www.facebook.com/typeahead/search/facebar/query/?value=[%22" + Uri.EscapeDataString("Users who live in " + KeyWord) + "%22]&context=facebar&grammar_version=90a525db12a8700dec0db939c6cb250e4f8e8de2&viewer=" + UserId + "&rsp=search&qid=1&max_results=10&sid=0.666867780033499&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizG85oCiq78hyWgSmEVFLFwxBxCbzElx24QqUgKm&__req=a&__rev=1380031";
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
                        CustPageSource = HttpHelper.getHtmlfromUrl(new Uri(query));
                    }


                  FriendsList=  GetGrpMember_Ajax(CustPageSource, ref fbUser, UserId, searchResult);
                  FriendsList = FriendsList.Distinct().ToList();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                return FriendsList;
            }

        }


        private List<string> GetGrpMember_Ajax(string pageSource, ref FacebookUser FBuser, string UserID, string searchResult)
        {
            GlobusHttpHelper HttpHelper = FBuser.globusHttpHelper;
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
                encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "\"");  //\",
                // 
                if (encoded_query.Contains(":\\\"") || encoded_query.Contains("{"))
                {
                    encoded_query = Utils.getBetween(pageSource, "encoded_query\":\"", "vertical");
                    encoded_query = Utils.getBetween(encoded_query, ":\\\"", "\\\"");
                }

                encoded_title = Utils.getBetween(pageSource, "encoded_title\":\"", "\"");
                filter_id = Utils.getBetween(pageSource, "filter_ids\":", "},") + "}";
                pageSource = pageSource.Replace("\\\"", "\"").Replace("\\", "");
                if (pageSource.Contains("user.php?id=") || pageSource.Contains("browse_search"))
                {
                    isContinue = true;

                    string[] arrId = Regex.Split(pageSource, "browse_search");
                    foreach (var arrId_item in arrId)
                    {
                        try
                        {
                            if (!arrId_item.Contains("<html><body><script type") && arrId_item.Contains("data-bt=") && !arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                            {
                                try
                                {
                                    string ProfileLinkUrl = Utils.getBetween(arrId_item, "<a href=\"", "=br_rs&amp;fref=");
                                    ProfileLinkUrl = ProfileLinkUrl;
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
                                    ProfileLinkUrl = ProfileLinkUrl;
                                    ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                   
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

                 


                }
                while (true)
                {
                    List<string> NewListLcal = new List<string>();

                    bool CheckCursor = false;
                    string[] ArrLocal = System.Text.RegularExpressions.Regex.Split(pageSource, "BrowseScrollingPager");
                    string Data = string.Empty;
                    Data = Uri.EscapeDataString("{\"view\":\"list\",\"encoded_query\":\"" + encoded_query + "\",\"encoded_title\":\"" + encoded_title + "\",\"ref\":\"unknown\",\"logger_source\":\"www_main\",\"typeahead_sid\":\"\",\"tl_log\":false,\"impression_id\":\"89ec6c8d\",\"filter_ids\":" + filter_id + ",\"experience_type\":\"grammar\",\"exclude_ids\":null,\"browse_location\":\"\",\"trending_source\":null,\"cursor\":\"" + Cursor + "\"}");
                    string AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=" + Data + "&__user=" + UserID + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKexm48jhHx2Vo&__req=b&__rev=1396250";

                    AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri(AjaxUrl));
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
                    if (pageSource.Contains("user.php?id=") || pageSource.Contains("browse_search"))
                    {
                        isContinue = true;

                        string[] arrId = Regex.Split(AjaxPageSource, "browse_search");
                        foreach (var arrId_item in arrId)
                        {
                            try
                            {
                                if (!arrId_item.Contains("<html><body><script type") && arrId_item.Contains("data-bt=") && !arrId_item.Contains("_7kf _8o _8s lfloat _ohe"))
                                {
                                    try
                                    {
                                        string ProfileLinkUrl = Utils.getBetween(arrId_item, "<a href=\"", "=br_rs&amp;fref=");
                                        ProfileUrlList.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
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
                                        NewListLcal.Add(ProfileLinkUrl.Replace("&amp", "").Replace("ref", "").Replace("?", "").Trim());
                                        ProfileUrlList = ProfileUrlList.Distinct().ToList();
                                        if (ProfileUrlList.Count > 100)
                                        {
                                            break;
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
                        ProfileUrlList = ProfileUrlList.Distinct().ToList();
                        ProfileUrlList.Remove("");
                        NewListLcal = NewListLcal.Distinct().ToList();
                        NewListLcal.Remove("");

                    }
                    else
                    {
                        return ProfileUrlList;
                    }
                    if (ProfileUrlList.Count > 100 || AjaxPageSource.Contains("<div class=\"phm _64f\">End of results</div>\n\nbrowse_end_of_results_footer") || !CheckCursor)
                    {
                        return ProfileUrlList;
                        break;
                    }
                    //return ProfileUrlList;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error >>> " + ex.StackTrace);
            }
            return ProfileUrlList;
        }

        public void SendFriendRequestViaFanPageURLs(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Sending Friend Request using Friend Profile Link with Email : " + fbUser.username);
                GlobusLogHelper.log.Debug("Sending Friend Request using Friend Profile Link with Email : " + fbUser.username);

                string UserId = string.Empty;

                string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

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

                int countFriendRequestsSent = 1;

                foreach (string item in lstFriendIds)
                {

                    try
                    {
                        if (countFriendRequestsSent > NoOfFriendsRequest)
                        {
                            return;
                        }

                        GlobusLogHelper.log.Info(" Friend Requests sending with Id :" + item + " and Email " + fbUser.username);
                        GlobusLogHelper.log.Debug(" Friend Requests sending with Id :" + item + " and Email " + fbUser.username);

                        string FriendRequesttUrl = FBGlobals.Instance.fbProfileUrl + item;

                        bool requeststatus = SendFriendRequestUpdated(FriendRequesttUrl, UserId, ref fbUser);

                        if (requeststatus)
                        {
                            GlobusLogHelper.log.Info(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);
                            GlobusLogHelper.log.Info(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            catch(Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
                            }
                            countFriendRequestsSent++;
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

        public void SendFriendRequestViaProfileURLs(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Sending Friend Request using Friend Profile Link with Email : " + fbUser.username);
              //  GlobusLogHelper.log.Debug("Sending Friend Request using Friend Profile Link with Email : " + fbUser.username);

                string UserId = string.Empty;

                string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                }

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                  //  GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }
                bool Check = false;
                int countFriendRequestsSent = 1;
                
                foreach (string FriendRequesttUrl in lstRequestFriendsProfileURLs)
                {
                    string FriendRequesttUrl1 = string.Empty;
                    if (!FriendRequesttUrl.Contains("facebook"))
                    {
                        Check = true;
                        FriendRequesttUrl1 = "https://www.facebook.com/" + FriendRequesttUrl;
                    }
                    try
                    {
                        if (countFriendRequestsSent > NoOfFriendsRequest)
                        {
                            return;
                        }

                        GlobusLogHelper.log.Info(" Friend Requests sending with Url :" + FriendRequesttUrl + " and Email " + fbUser.username);
                       // GlobusLogHelper.log.Debug(" Friend Requests sending with Url :" + FriendRequesttUrl + " and Email " + fbUser.username);
                        bool requeststatus = false;
                        if (Check == true)
                        {
                            requeststatus = SendFriendRequestUpdated(FriendRequesttUrl1, UserId, ref fbUser);
                        }
                        else
                        {
                            requeststatus = SendFriendRequestUpdated(FriendRequesttUrl, UserId, ref fbUser);
                        }

                        if (requeststatus)
                        {
                            GlobusLogHelper.log.Info(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);
                          //  GlobusLogHelper.log.Debug(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);

                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);

                            if (countFriendRequestsSent == NoOfFriendsRequestPerUser && NoOfFriendsRequestPerUser!=0)
                            {
                                
                                return;
                            }

                            countFriendRequestsSent++;
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

        public void SendFriendRequests(ref FacebookUser fbUser) //ref BaseLib.ChilkatHttpHelpr chilkatHttpHepler
        {
            try
            {
                if (IsSearchViaKeywords)
                {
                    if (!string.IsNullOrEmpty(Keywords))
                    {
                        SendFriendRequestViaKeywords(ref fbUser);
                    }
                    else if (lstRequestFriendsKeywords.Count > 0)
                    {
                        SendFriendRequestViaKeywords(ref fbUser);
                    }

                }
                else if (IsSearchViaLocation)
                {
                    if (!string.IsNullOrEmpty(Location))
                    {
                        SendFriendRequestViaLocation(ref fbUser);
                    }
                    else if (lstRequestFriendsLocation.Count > 0)
                    {
                        SendFriendRequestViaLocation(ref fbUser);
                    }
                }
                else if (IsSearchViaFanPageURLs)
                {
                    if (!string.IsNullOrEmpty(FanPageURLs))
                    {
                        SendFriendRequestViaFanPageURLs(ref fbUser);
                    }
                    else if (lstFriendIds.Count > 0)
                    {
                        SendFriendRequestViaFanPageURLs(ref fbUser);
                    }
                }
                else if (IsSearchViaProfileUrls)
                {
                    if (lstRequestFriendsProfileURLs.Count > 0)
                    {
                        SendFriendRequestViaProfileURLs(ref fbUser);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Please Upload Friends Profile URLs !");
                        GlobusLogHelper.log.Debug("Please Upload Friends Profile URLs !");
                    }
                }
            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Finished sending Requests With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Finished sending Requests With Username : " + fbUser.username);

            Thread.Sleep(4000);
        }

        public bool SendFriendRequestUpdated(string FriendRequestByUrl, string UserId, ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

                string FriRequestUrl = string.Empty;
                string FriendId = string.Empty;

                if (FriendRequestByUrl.Contains("profile"))
                {
                    if (FriendRequestByUrl.Contains("&ref=pymk"))
                    {
                        try
                        {
                            FriRequestUrl = FriendRequestByUrl;
                            string strFriId = FriRequestUrl.Replace("&ref=pymk", "");
                            string[] ArrTemp = strFriId.Split('=');
                            FriendId = ArrTemp[1];
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
                            FriRequestUrl = FriendRequestByUrl + "&ref=pymk";
                            string[] ArrTemp = FriendRequestByUrl.Split('=');
                            FriendId = ArrTemp[1];
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    FriRequestUrl = FriendRequestByUrl;
                }
                if (!FriRequestUrl.Contains("https"))
                {
                    FriRequestUrl = "https://" + FriRequestUrl;
                }
                string pageSrcFriendProfileUrl = httpHelper.getHtmlfromUrl(new Uri(FriRequestUrl));


                if (pageSrcFriendProfileUrl.Contains("profile_id") && string.IsNullOrEmpty(FriendId))
                {
                    string[] Arr = Regex.Split(pageSrcFriendProfileUrl, "profile_id");
                    foreach (string item in Arr)
                    {
                        try
                        {
                            if (!item.Contains("<!DOCTYPE"))
                            {
                                string profileId = item.Substring(0, 40);
                                if (profileId.Contains("&"))
                                {
                                    try
                                    {
                                        string[] TempArr1 = profileId.Split('=');
                                        string[] TempArr = TempArr1[1].Split('&');
                                        FriendId = TempArr[0];
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                if (profileId.Contains(":") && profileId.Contains(","))
                                {
                                    try
                                    {
                                        string[] TempArr = profileId.Split(':');
                                        string[] TempArr1 = TempArr[1].Split(',');
                                        FriendId = TempArr1[0];
                                        break;
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

                

                string fb_dtsg = GlobusHttpHelper.GetParamValue(pageSrcFriendProfileUrl, "fb_dtsg");

                ///** First Post For Friend Request *******************************************///

                string PostUrlFriendRequestFirst = FBGlobals.Instance.urlPostUrlFriendRequestFirstFriendManager;


                string PostDataFriendRequestFirst = "fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

                string ResponseFriendRequestFirst = httpHelper.postFormData(new Uri(PostUrlFriendRequestFirst), PostDataFriendRequestFirst);

                ///** Second Post For Friend Request *******************************************///
              

                string PostUrlFriendRequestSecond = FBGlobals.Instance.urlPostUrlFriendRequestSecondFriendManager;

                string PostDataFriendRequestSecond = "friend=" + FriendId + "&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

                string ResponseFriendRequestSecond = httpHelper.postFormData(new Uri(PostUrlFriendRequestSecond), PostDataFriendRequestSecond);

                ///** Third Post For Friend Request *******************************************///

                //string FriendId = FriendRequestByUrl.Split('=')[1];

                string PostUrlFriendRequestThird = FBGlobals.Instance.urlPostUrlFriendRequestThirdFriendManager;  
                string PostDataFriendRequestThird = string.Empty;
                if (pageSrcFriendProfileUrl.Contains("TimelineCapsule"))
                {
                    try
                    {
                        PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&link_data[gt][profile_owner]=" + FriendId + "&link_data[gt][ref]=timeline%3Atimeline&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
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
                        PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                string ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);

                if (ResponseFriendRequestThird.Contains("errorSummary") && ResponseFriendRequestThird.Contains("Confirmation Required")) // && ResponseFriendRequestThird.Contains("A confirmation is required before you can proceed"))
                {
                    try
                    {
                        PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=" + UserId + "&confirmed=1";
                        ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                if (ResponseFriendRequestThird.Contains("success"))
                {
                    //GlobusLogHelper.log.Info("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);
                    //GlobusLogHelper.log.Debug("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);

                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    Thread.Sleep(delayInSeconds);
                    
                    return true;

                }
                else if (ResponseFriendRequestThird.Contains("Already Sent Request"))
                {
                    GlobusLogHelper.log.Info("Already requested :" + FriRequestUrl + " with Account " + fbUser.username);
                    GlobusLogHelper.log.Debug("Already requested :" + FriRequestUrl + " with Account " + fbUser.username);

                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

                    Thread.Sleep(delayInSeconds);

                    return false;
                }
                else if (ResponseFriendRequestThird.Contains("You've been blocked from using this feature because you may have violated Facebook's Terms."))
                {
                    GlobusLogHelper.log.Info("You've been blocked from using this feature because you may have violated Facebook's Terms.."+"With Account" + fbUser.username);
                    GlobusLogHelper.log.Debug("You've been blocked from using this feature because you may have violated Facebook's Terms.." + "With Account" + fbUser.username);

                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    Thread.Sleep(delayInSeconds);
                }
                else if(ResponseFriendRequestThird.Contains("You&#039;re blocked from sending friend requests for"))
                {
                    GlobusLogHelper.log.Info("You are blocked from sending friend requests "+fbUser.username );
                    GlobusLogHelper.log.Debug("You are blocked from sending friend requests " + fbUser.username);

                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                    Thread.Sleep(delayInSeconds);
                }
                else if (ResponseFriendRequestThird.Contains("\"errorSummary\":") && !ResponseFriendRequestThird.Contains("Already requested"))
                {
                    string errorSummary=FBUtils.GetErrorSummary(ResponseFriendRequestThird);
                    GlobusLogHelper.log.Info("Error Summary : " + errorSummary + " with Url :" + FriRequestUrl + " with Account " + fbUser.username);
                    GlobusLogHelper.log.Debug("Error Summary : " + errorSummary + " with Url :" + FriRequestUrl + " with Account " + fbUser.username);

                }
                else 
                {
                    GlobusLogHelper.log.Info("Some Problem with Url :" + FriRequestUrl + " with Account " + fbUser.username);
                    GlobusLogHelper.log.Debug("Some Problem with Url :" + FriRequestUrl + " with Account " + fbUser.username);
                    return false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        public static string CancelSentFriendRequestExprotFilePath = string.Empty;
        public static string CancelFriendRequestExportFilePath = string.Empty;
        public static string AcceptFriendRequestExportFilePath = string.Empty; 

        public void StartAcceptFriendRequest()
        {
            try
            {
                requestFriendsThreadControllerCount = 0;

                int numberOfAccountPatch = 25;

                if (NoOfThreads > 0)
                {
                    numberOfAccountPatch = NoOfThreads;
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
                                lock (requestFriendsThreadControllerlockr)
                                {
                                    try
                                    {
                                        if (requestFriendsThreadControllerCount >= listAccounts.Count)
                                        {
                                            Monitor.Wait(requestFriendsThreadControllerlockr);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(AcceptFriendRequestMultiThreads);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            requestFriendsThreadControllerCount++;
                                            //tempCounterAccounts++; 
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

        private void AcceptFriendRequestMultiThreads(object parameters)
        {

            try
            {
                if (!isRequestFriendsStop)
                {
                    try
                    {
                        lstRequestFriendsThreads.Add(Thread.CurrentThread);
                        lstRequestFriendsThreads.Distinct();
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
                                        // Call AcceptFriendRequests
                                        if (Friends_AcceptSendFrndProcessUsing == "Accept Friends Request")
                                        {
                                             AcceptFriendRequests(ref objFacebookUser);
                                        }
                                        else if (Friends_AcceptSendFrndProcessUsing == "Cancel Friends Request")
                                        {
                                            CancelFriendRequests(ref objFacebookUser);
                                        }
                                        else if (Friends_AcceptSendFrndProcessUsing == "Cancel Sent Friends Request")
                                        {
                                            CancelSentFriendRequests(ref objFacebookUser);
                                        }
                                        else if (Friends_AcceptSendFrndProcessUsing == "Suggest Friends")
                                        {
                                            SuggestFriends(ref objFacebookUser);
                                        }                                      
                                       
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
                  //  if (!isRequestFriendsStop)
                    {
                        lock (requestFriendsThreadControllerlockr)
                        {
                            requestFriendsThreadControllerCount--;
                            Monitor.Pulse(requestFriendsThreadControllerlockr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void AcceptFriendRequests(ref FacebookUser fbUser)
        {
            int AcceptFrndCount = 0;
            #region FR Requests Acceptins
            try
            {
                lstRequestFriendsThreads.Add(Thread.CurrentThread);
                lstRequestFriendsThreads.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start AcceptFriendRequest With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start AcceptFriendRequest With Username : " + fbUser.username);

                string UserId = string.Empty;

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

                string pageSourceFriendRequests=string.Empty;
                
                List<string> listFriend_Requests=new List<string>();
                List<string> listFriend_Suggestions=new List<string>();


               // Friends_AcceptFriends_CheckTargeted
                 
                string FrinedId = string.Empty;
              
                int tempCount_CheckRequests = 0;
                //code to double check if all FRs have been accepted
                while (CheckFriendCount(ref HttpHelper, ref UserId, ref pageSourceFriendRequests, ref listFriend_Requests, ref listFriend_Suggestions) > 0)
                {
                    try
                    {
                        lstRequestFriendsThreads.Add(Thread.CurrentThread);
                        lstRequestFriendsThreads.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                   
                  
                    if (tempCount_CheckRequests >= NoOfFriendRequestFriendManager)
                    {
                        break;
                    }
                    if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                    {
                        break;
                    }
                    tempCount_CheckRequests++;
                    var itemId = listFriend_Requests.Distinct().ToList();
                    
                    //GlobusLogHelper.log.Info(itemId.Count() + " Friend Request With Username : "+fbUser.username);
                    //GlobusLogHelper.log.Debug(itemId.Count() + " Friend Request With Username : " + fbUser.username);

                    foreach (var item in itemId)
                    {
                        string Gender = string.Empty;
                        bool CheckGenderMale = false;
                        bool CheckGenderFeMale = false;
                        string PageSource = string.Empty;
                        if (item.Contains("profile.php?id"))
                        {
                            string ProfileUrl = item.Replace("?fref=%2Freqs.php", "") + "&sk=about" + "&section=contact-info".Trim();
                            PageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            Gender = Utils.getBetween(PageSource, "Gender", "</div></div>") + "@";
                            Gender = Utils.getBetween(Gender, "<div>", "@");
                            if (string.IsNullOrEmpty(Gender))
                            {
                                try
                                {
                                    string[] GenArr = System.Text.RegularExpressions.Regex.Split(PageSource, "Gender</span>");
                                    Gender = Utils.getBetween(GenArr[1], "<span class=\"_50f4\">", "</span></div></div>");
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            if (Gender.Contains("Male"))
                            {
                                CheckGenderMale = true;
                            }
                            else
                            {
                                CheckGenderFeMale = true;
                            }
                        }
                        else
                        {
                            string ProfileUrl = item.Replace("?fref=%2Freqs.php", "") + "/about" + "?section=contact-info".Trim(); ;
                            PageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            Gender = Utils.getBetween(PageSource, "Gender", "</div></div>")+"@";
                            Gender = Utils.getBetween(Gender, "<div>","@");
                            if (string.IsNullOrEmpty(Gender))
                            {
                                try
                                {
                                    string[] GenArr = System.Text.RegularExpressions.Regex.Split(PageSource, "Gender</span>");
                                    Gender = Utils.getBetween(GenArr[1], "<span class=\"_50f4\">", "</span></div></div>");
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            if (Gender == "Male")
                            {
                                CheckGenderMale = true;
                            }
                            else 
                            {
                                CheckGenderFeMale = true;
                            }
                          
                        }

                        if (CheckGenderMale && !Friends_AcceptFriends_Male)
                        {
                            continue;
                        }
                        else if (CheckGenderFeMale&&!Friends_AcceptFriends_Female)
                        {
                            continue;
                        }

                        try
                        {
                            if (item != UserId)
                            {
                                FrinedId = item;
                                //string pagesource = HttpHelper.getHtmlfromUrl(new Uri(FrinedId));
                                //string fb_dtsg = Globussoft.GlobusHttpHelper.GetParamValue(pagesource, "fb_dtsg");
                                if (FrinedId.Contains("facebook.com/"))
                                {
                                    try
                                    {
                                        FrinedId = HttpHelper.ExtractIDUsingGraphAPI(FrinedId, ref HttpHelper);
                                    }
                                    catch
                                    {
                                        FrinedId = HttpHelper.ExtractIDOfNonTimeLine(FrinedId, ref HttpHelper);
                                    }
                                }

                                string posturl = FBGlobals.Instance.urlPostAddFriendUrlFriendManager;
                                string postdata = "to_friend=" + FrinedId + "&action=confirm&ref_param=/profile.php&__user=" + UserId + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816652574510967111";//"fb_dtsg=" + fb_dtsg + "&confirm=" + FrinedId + "&type=friend_connect&request_id=" + FrinedId + "&list_item_id=" + FrinedId + "_1_req&status_div_id=" + FrinedId + "_1_req_status&sce=1&inline=1&ref=%2Freqs.php&actions[accept]=Confirm&__user=" + UsreId + "&__a=1&phstamp=165816652574510967249";//"post_form_id="+post_form_id+"&fb_dtsg="+fb_dtsg+"&confirm="+FrinedId+"&type=friend_connect&request_id="+FrinedId+"&list_item_id="+FrinedId+"_1_req&status_div_id="+FrinedId+"_1_req_status&sce=&inline=1&ref=jewel&num_visible_requests=1&actions[accept]=Confirm&lsd&post_form_id_source=AsyncRequest&__user="+UsreId

                                string ResponsefriendRequest = HttpHelper.postFormData(new Uri(posturl), postdata);

                                if (ResponsefriendRequest.Contains(":\"Sorry"))
                                {
                                    // post data & URL   https://www.facebook.com/ajax/reqs.php
                                    //fb_dtsg=AQDIbgcJ&confirm=100006524137243&type=friend_connect&request_id=100006524137243&list_item_id=100006524137243_1_req&status_div_id=100006524137243_1_req_status&sce=1&inline=1&ref=%2Freqs.php&ego_log=&actions[accept]=Confirm&__user=100000184892940&__a=1&__dyn=7n8ahyj35zoSt2u5KKDKaEwlyp8y&__req=r&ttstamp=265816873981039974

                                    try
                                    {
                                        posturl = "https://www.facebook.com/ajax/reqs.php";
                                        postdata = "fb_dtsg=" + fb_dtsg + "&confirm=" + FrinedId + "&type=friend_connect&request_id=" + FrinedId + "&list_item_id=" + FrinedId + "_1_req&status_div_id=" + FrinedId + "_1_req_status&sce=1&inline=1&ref=%2Freqs.php&ego_log=&actions[accept]=Confirm&__user=" + 100000184892940 + "&__a=1&__dyn=7n8ahyj35zoSt2u5KKDKaEwlyp8y&__req=r&ttstamp=265816873981039974";

                                        ResponsefriendRequest = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                if (ResponsefriendRequest.Contains(":\"Sorry"))
                                {                                  

                                    GlobusLogHelper.log.Info("Couldn't Accept Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Accept Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {
                                       

                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    continue;
                                }
                                else if (ResponsefriendRequest.Contains(":\"Error"))
                                {
                                    GlobusLogHelper.log.Info("Couldn't Accept Request with friend id : " + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Accept Request with friend id : " + FrinedId + " & Username: " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    continue;
                                }

                                else if (ResponsefriendRequest.Contains("success"))
                                {
                                    AcceptFrndCount = AcceptFrndCount + 1;
                                   
                                    GlobusLogHelper.log.Info("Friend Request accepted with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Friend Request accepted with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                    if (!string.IsNullOrEmpty(AcceptFriendRequestExportFilePath))
                                    {


                                        string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                        string CSV_Content = fbUser.username + "," + FrinedId + ", " + "Friend request Accept";
                                        try
                                        {

                                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, AcceptFriendRequestExportFilePath);
                                            GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                            GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                    //You are friends with this person
                                    if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                                    {
                                        break;
                                    }
                                }
                                else if (ResponsefriendRequest.Contains("You are friends with this person"))
                                {
                                    GlobusLogHelper.log.Info("Friend Request accepted with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Friend Request accepted with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                    //You are friends with this person
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Couldn't Accept Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Accept Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                    
                                    GlobusLogHelper.log.Info("Response : " + ResponsefriendRequest+"  Username : "+fbUser.username);
                                    GlobusLogHelper.log.Debug("Response : " + ResponsefriendRequest + "  Username : " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                }

                            }
                        }
                        catch(Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }               

                #region FR Suggestions Confirms

                if (Friends_AcceptSendFrndToSuggestions)
                {
                    try
                    {
                        foreach (var item in listFriend_Suggestions.Distinct().ToList())
                        {
                            try
                            {

                                if (item != UserId)
                                {
                                    FrinedId = item;
                                    
                                    if (FrinedId.Contains("facebook.com/"))
                                    {
                                        try
                                        {
                                            FrinedId = HttpHelper.ExtractIDUsingGraphAPI(FrinedId, ref HttpHelper);
                                        }
                                        catch
                                        {
                                            FrinedId = HttpHelper.ExtractIDOfNonTimeLine(FrinedId, ref HttpHelper);
                                        }
                                    }

                                    string posturl = FBGlobals.Instance.urlPostAjaxReqsurlFriendManager;
                                    string postdata = "confirm=" + FrinedId + "&type=friend_suggestion&request_id=" + FrinedId + "&list_item_id=" + FrinedId + "_19_req&status_div_id=" + FrinedId + "_19_req_status&sce=&inline=1&ref=jewel&ego_log=&actions[accept]=Add%20Friend&nctr[_mod]=pagelet_bluebar&__user=" + UserId + "&__a=1&__dyn=798aD5z5ynU-wE&__req=1r&fb_dtsg=" + fb_dtsg + "&phstamp=16581661067811" + Utils.GenerateRandom(95570000, 95700000) + "";

                                    string ResponsefriendRequest = HttpHelper.postFormData(new Uri(posturl), postdata);

                                    if (ResponsefriendRequest.Contains(":\"Sorry"))
                                    {
                                        GlobusLogHelper.log.Info("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                        //if (!IsFastFRAccept)
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        continue;
                                    }
                                    else if (ResponsefriendRequest.Contains(":\"Error"))
                                    {
                                        GlobusLogHelper.log.Info("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                        //if (!IsFastFRAccept)
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        continue;
                                    }

                                    else if (ResponsefriendRequest.Contains(".requestStatus\",true"))
                                    {
                                        GlobusLogHelper.log.Info("Suggestion: Suggestions Friend Request Sent with friend id :" + FrinedId + " & Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Suggestion: Suggestions Friend Request Sent with friend id :" + FrinedId + " & Username : " + fbUser.username);
                                    }
                                    else
                                    {

                                        GlobusLogHelper.log.Info("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Info("Suggestion: Couldn't Send Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                        GlobusLogHelper.log.Info("Response : " + ResponsefriendRequest+" & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Response : " + ResponsefriendRequest + " & Username: " + fbUser.username);

                                        //if (!IsFastFRAccept)
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
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
                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            #endregion

            GlobusLogHelper.log.Info("Process Completed Of Accept Friend Request With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Accept Friend Request With Username : " + fbUser.username);
        }

        //cancel Friends request 
        public void CancelFriendRequests(ref FacebookUser fbUser)
        {
            int AcceptFrndCount = 0;
            #region FR Requests Acceptins
            try
            {
                lstRequestFriendsThreads.Add(Thread.CurrentThread);
                lstRequestFriendsThreads.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Starting cancelFriendRequest With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Starting cancelFriendRequest With Username : " + fbUser.username);

                string UserId = string.Empty;

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

                string pageSourceFriendRequests = string.Empty;

                List<string> listFriend_Requests = new List<string>();
                List<string> listFriend_Suggestions = new List<string>();


                string FrinedId = string.Empty;

                int tempCount_CheckRequests = 0;
                //code to double check if all FRs have been accepted
                while (CheckFriendCount(ref HttpHelper, ref UserId, ref pageSourceFriendRequests, ref listFriend_Requests, ref listFriend_Suggestions) > 0)
                {
                    try
                    {
                        lstRequestFriendsThreads.Add(Thread.CurrentThread);
                        lstRequestFriendsThreads.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    tempCount_CheckRequests++;
                    if (tempCount_CheckRequests >= 10)
                    {
                        // break;
                    }
                    if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                    {
                        break;
                    }
                    var itemId = listFriend_Requests.Distinct().ToList();

                    //GlobusLogHelper.log.Info(itemId.Count() + " Friend Request With Username : "+fbUser.username);
                    //GlobusLogHelper.log.Debug(itemId.Count() + " Friend Request With Username : " + fbUser.username);

                    foreach (var item in itemId)
                    {
                        string Gender = string.Empty;
                        bool CheckGenderMale = false;
                        bool CheckGenderFeMale = false;
                        string PageSource = string.Empty;
                        if (item.Contains("profile.php?id"))
                        {
                            string ProfileUrl = item.Replace("?fref=%2Freqs.php", "") + "&sk=about"+"&section=contact-info".Trim();
                            PageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSource);
                            Gender = Utils.getBetween(PageSource, "Gender", "</div></div>") + "@";
                            Gender = Utils.getBetween(Gender, "<div>", "@");
                            if (Gender.Contains("Male"))
                            {
                                CheckGenderMale = true;
                            }
                            else
                            {
                                CheckGenderFeMale = true;
                            }
                        }
                        else
                        {
                            string ProfileUrl = item.Replace("?fref=%2Freqs.php", "") + "/about";
                            PageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileUrl));
                            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSource);
                            Gender = Utils.getBetween(PageSource, "Gender", "</div></div>") + "@";
                            Gender = Utils.getBetween(Gender, "<div>", "@");
                            if (Gender.Contains("Male"))
                            {
                                CheckGenderMale = true;
                            }
                            else
                            {
                                CheckGenderFeMale = true;
                            }

                        }

                        if (CheckGenderMale && !Friends_AcceptFriends_Male)
                        {
                            continue;
                        }
                        else if (CheckGenderFeMale && !Friends_AcceptFriends_Female)
                        {
                            continue;
                        }

                        try
                        {
                            if (item != UserId)
                            {
                                FrinedId = item;
                                //string pagesource = HttpHelper.getHtmlfromUrl(new Uri(FrinedId));
                                //string fb_dtsg = Globussoft.GlobusHttpHelper.GetParamValue(pagesource, "fb_dtsg");
                                if (FrinedId.Contains("facebook.com/"))
                                {
                                    try
                                    {
                                        FrinedId = HttpHelper.ExtractIDUsingGraphAPI(FrinedId, ref HttpHelper);
                                    }
                                    catch
                                    {
                                        FrinedId = HttpHelper.ExtractIDOfNonTimeLine(FrinedId, ref HttpHelper);
                                    }
                                }

                                if (FrinedId.Contains("&amp;"))
                                {
                                    FrinedId = FrinedId.Replace("&amp;fref=%2Freqs.php",string.Empty).Trim();
                                }

                                string posturl = "https://www.facebook.com/ajax/profile/connect/reject.php";
                             //   string postdata = "fb_dtsg=" + fb_dtsg + "&confirm=" + FrinedId + "&type=friend_connect&request_id=" + FrinedId + "&list_item_id=" + FrinedId + "_1_req&status_div_id=" + FrinedId + "_1_req_status&inline=1&ref=jewel&ego_log=AT7GmywcDNmeS_xqAxpj4TPYn7qNEf9X8-kgoeHeumv146pw0_PGpbIiZXqVNNELH3zHFQOREbUBq-bQsh5FLG90NQyCR-AS1ZeZaAQJtQyeuQDKOblffDgoa093y-eIiVhEt1rnXGVWdKdPY3BBDb69D5kMC7xwDXMz-AcmhcSwJDcDae3VhwgY0TL49Q5fHhe64TYBeag0vnqdyavYAaXSQnO5xShI7xUqzAGOBCquWI0w&actions[hide]=1&nctr[_mod]=pagelet_bluebar&__user="+UserId+"&__a=1&__dyn=7n8anEAMCBynzpQ9UoHFaeFDzECQqbx2mbACFaaGGzCC_826m6oDAyoSnx2ubhHAG8Kl1e&__req=9&ttstamp=2658172785657997097118106116&__rev=1398717";
                                string postdata = "profile_id="+FrinedId+"&ref=%2Fprofile.php&floc=profile_box&frefs[0]=none&nctr[_mod]=pagelet_above_header_timeline&__user="+UserId+"&__a=1&__dyn=7n8ahyj2qm9udDgDxyF4EihUtCxO4p9GgyiGGfirWo8pojByUWdDx2ubhHx2Vokw&__req=9&fb_dtsg="+fb_dtsg+"&ttstamp=26581691006953529980894871&__rev=1398717";

                                string ResponsefriendRequest = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/profile/connect/reject.php"), postdata);

                                if (ResponsefriendRequest.Contains("Already Removed") || ResponsefriendRequest.Contains("No request found to remove. You may have already removed this request"))
                                {
                                    // post data & URL   https://www.facebook.com/ajax/reqs.php
                                    //fb_dtsg=AQDIbgcJ&confirm=100006524137243&type=friend_connect&request_id=100006524137243&list_item_id=100006524137243_1_req&status_div_id=100006524137243_1_req_status&sce=1&inline=1&ref=%2Freqs.php&ego_log=&actions[accept]=Confirm&__user=100000184892940&__a=1&__dyn=7n8ahyj35zoSt2u5KKDKaEwlyp8y&__req=r&ttstamp=265816873981039974

                                    try
                                    {
                                        posturl = "https://www.facebook.com/ajax/reqs.php";
                                        postdata = "fb_dtsg=" + fb_dtsg + "&confirm=" + FrinedId + "&type=friend_connect&request_id=" + FrinedId + "&list_item_id=" + FrinedId + "_1_req&status_div_id=" + FrinedId + "_1_req_status&sce=1&inline=1&ref=%2Freqs.php&ego_log=&actions[accept]=Confirm&__user=" + 100000184892940 + "&__a=1&__dyn=7n8ahyj35zoSt2u5KKDKaEwlyp8y&__req=r&ttstamp=265816873981039974";

                                        ResponsefriendRequest = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                if (ResponsefriendRequest.Contains(":\"Sorry"))
                                {


                                    GlobusLogHelper.log.Info("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {


                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    continue;
                                }
                                else if (ResponsefriendRequest.Contains(":\"Error"))
                                {
                                    GlobusLogHelper.log.Info("Couldn'tCancel Request with friend id : " + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id : " + FrinedId + " & Username: " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    continue;
                                }

                                else if (ResponsefriendRequest.Contains("CONNECTION_REQUEST"))
                                {
                                    AcceptFrndCount = AcceptFrndCount + 1;

                                    GlobusLogHelper.log.Info("Cancel Friend request - friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Cancel Friend request - friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    
                                    if (!string.IsNullOrEmpty(CancelFriendRequestExportFilePath))
                                    {


                                        string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                        string CSV_Content = fbUser.username + "," + FrinedId + ", " + "Friend request cancelled";
                                        try
                                        {
                                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, CancelFriendRequestExportFilePath);
                                            GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                            GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                    //You are friends with this person
                                    if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                                    {
                                        break;
                                    }
                                }
                               
                                else
                                {
                                    GlobusLogHelper.log.Info("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);


                                    GlobusLogHelper.log.Info("Response : " + ResponsefriendRequest + "  Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Response : " + ResponsefriendRequest + "  Username : " + fbUser.username);

                                    //if (!IsFastFRAccept)
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
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
            #endregion

            GlobusLogHelper.log.Info("Process Completed Of Cancel Friend Request With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Cancel Friend Request With Username : " + fbUser.username);
        }


        public void CancelSentFriendRequests(ref FacebookUser fbUser)
        {
           
                int AcceptFrndCount = 0;
                #region FR Requests Acceptins
                try
                {
                    lstRequestFriendsThreads.Add(Thread.CurrentThread);
                    lstRequestFriendsThreads.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                try
                {
                    GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                    GlobusLogHelper.log.Info("Start Cancel Sent Friend Request With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Start Cancel Sent Friend Request With Username : " + fbUser.username);

                    string UserId = string.Empty;

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

                    string pageSourceFriendRequests = string.Empty;

                    List<string> listFriend_Requests = new List<string>();
                    List<string> listFriend_Suggestions = new List<string>();


                    string FrinedId = string.Empty;

                    int tempCount_CheckRequests = 0;
                    //code to double check if all FRs have been accepted
                    //while (CheckSentFriendCount(ref HttpHelper, ref UserId, ref pageSourceFriendRequests, ref listFriend_Requests, ref listFriend_Suggestions) > 0)
                    while (CheckSentFriendRequestCountNew(ref HttpHelper, ref UserId, ref pageSourceFriendRequests, ref listFriend_Requests) > 0)
                    {
                        try
                        {
                            lstRequestFriendsThreads.Add(Thread.CurrentThread);
                            lstRequestFriendsThreads.Distinct();
                            Thread.CurrentThread.IsBackground = true;
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }


                        tempCount_CheckRequests++;
                        if (tempCount_CheckRequests >= 10)
                        {
                            // break;
                        }
                        if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                        {
                            break;
                        }
                        var itemId = listFriend_Requests.Distinct().ToList();


                        foreach (var item in itemId)
                        {
                            try
                            {
                                if (item != UserId)
                                {
                                    FrinedId = item;
                                    string posturl = "https://www.facebook.com/ajax/friends/requests/cancel.php";
                                    string postdata = "friend=" + FrinedId + "&cancel_ref=outgoing_requests&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aAix90BCxO4oKAdBGfirWo8pojByUW5ogxd6K4bBxi&__req=q&fb_dtsg=" + fb_dtsg + "&ttstamp=26581705111797677668726673&__rev=1398717&confirmed=1";

                                    string ResponsefriendRequest = string.Empty;
                                    try
                                    {
                                        ResponsefriendRequest = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(ex.Message);
                                        GlobusLogHelper.log.Info("Unable To Cancel Sent Friend Request For UserID"+item);
                                    }

                                    if (ResponsefriendRequest.Contains(":\"Sorry"))
                                    {

                                        GlobusLogHelper.log.Info("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);

                                        //if (!IsFastFRAccept)
                                        {


                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                              GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        continue;
                                    }
                                    else if (ResponsefriendRequest.Contains(":\"Error"))
                                    {
                                        GlobusLogHelper.log.Info("Couldn'tCancel Request with friend id : " + FrinedId + " & Username: " + fbUser.username);
                                         GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id : " + FrinedId + " & Username: " + fbUser.username);
                                        //if (!IsFastFRAccept)
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                           GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                           GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        continue;
                                    }

                                    else if (ResponsefriendRequest.Contains("[\"FriendRequest\\/cancel\""))
                                    {
                                        AcceptFrndCount = AcceptFrndCount + 1;

                                        GlobusLogHelper.log.Info("Friend request cancelled  - friend id :" + FrinedId + " & Username: " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Friend request cancelled  - friend id :" + FrinedId + " & Username: " + fbUser.username);

                                        if (!string.IsNullOrEmpty(CancelSentFriendRequestExprotFilePath))
                                        {


                                            string CSVHeader = "UserName" + "," + "FriendId" + ", " + "Stautus";
                                            string CSV_Content = fbUser.username + "," + FrinedId + ", " + "Friend request cancelled";
                                            try
                                            {

                                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, CancelSentFriendRequestExprotFilePath);
                                                GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                                GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }




                                        int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                        //You are friends with this person
                                        if (AcceptFrndCount >= NoOfFriendRequestFriendManager)
                                        {
                                            break;
                                        }
                                    }

                                    else
                                    {
                                         GlobusLogHelper.log.Info("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);
                                         GlobusLogHelper.log.Debug("Couldn't Cancel Request with friend id :" + FrinedId + " & Username: " + fbUser.username);


                                          GlobusLogHelper.log.Info("Response : " + ResponsefriendRequest + "  Username : " + fbUser.username);
                                           GlobusLogHelper.log.Debug("Response : " + ResponsefriendRequest + "  Username : " + fbUser.username);

                                        //if (!IsFastFRAccept)
                                        {

                                            int delayInSeconds = Utils.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
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
                #endregion

                GlobusLogHelper.log.Info("Process Completed Of Cancel sent Friends Request With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Process Completed Of Cancel sent Friends Request With Username : " + fbUser.username);
            
        }


        public List<string> lstFriendsUrlToSuggestFriends = new List<string>();
        public void SuggestFriends(ref FacebookUser fbUser)
        {
            try
            {
                lstRequestFriendsThreads.Add(Thread.CurrentThread);
                lstRequestFriendsThreads.Distinct();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string FriendName = string.Empty;
                GlobusLogHelper.log.Info("Start Suggest Friends With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Suggest Friends With Username : " + fbUser.username);

                string UserId = string.Empty;
                string FriendID = string.Empty;
                int suggestionCounter = 0;
                string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                string fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "fb_dtsg");
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
                List<string> lstSuggestFriendsId = new List<string>();
                foreach (string ItemFriendUrl in lstFriendsUrlToSuggestFriends)
                { 
                  string FriendsPageSrc=HttpHelper.getHtmlfromUrl(new Uri(ItemFriendUrl));
                  string[] urlData = ItemFriendUrl.Split('/');
                  FriendName = urlData[urlData.Length - 1];
                  string graphResp = HttpHelper.getHtmlfromUrl(new Uri("https://graph.facebook.com/" + FriendName));
                  FriendID = Utils.getBetween(graphResp, "id\": \"", "\"");
                  string postDataToGetSuggestion = "__user="+UserId+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgyimEVFLFwxBxCbzESu49UJ6K59poW8xHzoyfw&__req=11&fb_dtsg="+fb_dtsg+"&ttstamp=2658169975683103119106817079&__rev=1549264";
                  string friendSuggestionResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/chooser/list/friends/suggest/?filter=all&newcomer=" + FriendID),postDataToGetSuggestion);
                  string suggestData = Utils.getBetween(friendSuggestionResp, "[", "]");
                  string[] splitIds = suggestData.Split(',');
                  foreach (string item in splitIds)
                  {
                      string temp = Utils.getBetween(item, "\"", "\"");
                      lstSuggestFriendsId.Add(temp);
                  }
                  lstSuggestFriendsId = lstSuggestFriendsId.Distinct().ToList();

                  foreach (string suggestId in lstSuggestFriendsId)
                  {
                      suggestionCounter++;
                      try
                      {
                          string postSuggextion = "receiver=" + suggestId + "&newcomer=" + FriendID + "&attempt_id=013473d158fa55456cc53f253c8c8431&ref=profile_others_dropdown&__user="+UserId+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgyimEVFLFwxBxCbzESu49UJ6K59poW8xHzoyfw&__req=y&fb_dtsg=" + fb_dtsg + "&ttstamp=2658169545354106991215011056&__rev=1549264";
                          string suggestionResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/friends/suggest"), postSuggextion);

                          if (suggestionResp.Contains("for (;;);{\"__ar\":1,\"payload\":null,\"bootloadable\":{},\"ixData\":{}}"))
                          {
                              GlobusLogHelper.log.Info("Friend Suggestion Id : " + suggestId + " To Friend " + ItemFriendUrl);
                              GlobusLogHelper.log.Debug("Start Suggestion Id : : " + suggestId + "To Friend " + ItemFriendUrl);
                          }
                          if (suggestionCounter == NoOfFriendRequestFriendManager)
                          {
                              break;
                          }
                          int delay = new Random().Next(minDelayFriendManager, maxDelayFriendManager);
                          GlobusLogHelper.log.Info("Delaying For"+delay+"Seconds");
                          GlobusLogHelper.log.Debug("Delaying For" + delay + "Seconds");
                          Thread.Sleep(delay*1000);
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

        private int CheckFriendCount(ref GlobusHttpHelper HttpHelper,ref string UsreId, ref string pageSourceFriendRequests, ref List<string> listFriend_Requests, ref List<string> listFriend_Suggestions)
        {
            listFriend_Requests = new List<string>();
            if (Friends_AcceptFriends_CheckTargeted)
            {
                listFriend_Requests = lstFriendsUrlToSuggestFriends.Distinct().ToList();
            }
            else
            {

            try
            {
                pageSourceFriendRequests = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlGetReqsurlFriendManager));

                 string ProFilePost = string.Empty;

                listFriend_Requests = new List<string>();

                listFriend_Suggestions = new List<string>();

                listFriend_Requests = HttpHelper.ExtractFriendIDs_URLSpecific(ref HttpHelper, ref UsreId, FBGlobals.Instance.urlGetReqsurlFriendManager, ref listFriend_Suggestions);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            }

            if (Friends_AcceptFriends_CheckTargeted)
            {
                NoOfFriendRequestFriendManager = listFriend_Requests.Count();
            }

            return listFriend_Requests.Count;
        }

        private int CheckSentFriendCount(ref GlobusHttpHelper HttpHelper, ref string UsreId, ref string pageSourceFriendRequests, ref List<string> listFriend_Requests, ref List<string> listFriend_Suggestions)
        {
            listFriend_Requests = new List<string>();

            try
            {
                pageSourceFriendRequests = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlGetSentReqsurlFriendManager));

                string ProFilePost = string.Empty;

                listFriend_Requests = new List<string>();

                listFriend_Suggestions = new List<string>();

                listFriend_Requests = HttpHelper.ExtractRequestSendFriendIDs_URLSpecific(ref HttpHelper, ref UsreId, FBGlobals.Instance.urlGetSentReqsurlFriendManager, ref listFriend_Suggestions);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return listFriend_Requests.Count;
        }

        //Developed By Mahesh 26-12-2014
        private int CheckSentFriendRequestCountNew(ref GlobusHttpHelper HttpHelper, ref string UsreId, ref string pageSourceFriendRequests, ref List<string> listFriend_Requests)
        {
            listFriend_Requests = new List<string>();
            try
            {
                string SentRequestPagesrc = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/friends/requests/?fcref=jwl&outgoing=1"));
                HtmlDocument result = new HtmlDocument();
                result.LoadHtml(SentRequestPagesrc);
                List<HtmlNode> SentRequestMainDiv = result.DocumentNode.Descendants().Where
                   (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("mtl _30d _5ewg _5n-u"))).ToList();
                foreach (HtmlNode hlNodeMain in SentRequestMainDiv)
                {
                    string hln = hlNodeMain.OuterHtml.ToString();
                    if (hln.Contains("Friend Requests Sent"))
                    {
                        HtmlDocument result1 = new HtmlDocument();
                        result1.LoadHtml(hln);
                    List<HtmlNode> SentRequestDiv = result1.DocumentNode.Descendants().Where
                       (x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("_6-_"))).ToList();
                    foreach (HtmlNode hlNode in SentRequestDiv)
                    {
                        //string hln = hlNode.OuterHtml.ToString();
                        //if (!hln.Contains("u_jsonp"))
                        {

                            string FriendsUrl = Utils.getBetween(hlNode.InnerHtml.ToString(), "<a href=\"", "\"");

                            if (FriendsUrl.Contains("profile.php"))
                            {
                                string FriendId = Utils.getBetween(FriendsUrl, "id=", "&");
                                listFriend_Requests.Add(FriendId);
                            }
                            else
                            {
                                string[] splitsData = FriendsUrl.Split('/');
                                string UserName = splitsData[splitsData.Length - 1].Split('?')[0];
                                //http://graph.facebook.com/preeti.madav.9
                                string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + UserName));
                                string UserId = Utils.getBetween(graphResp, "id\": \"", "\"");
                                if (!string.IsNullOrEmpty(UserId))
                                {
                                    listFriend_Requests.Add(UserId);  
                                }
                                
                            }

                        }
                    }
                }
                }
                //clearfix mtm uiMorePager stat_elem _646 _52jv  pager_id
                string[] splitsPage = Regex.Split(SentRequestPagesrc, "ajaxify=\"/friends/requests/outgoing/more/?");
                string NextPAgeData = Utils.getBetween(splitsPage[1], "=outgoing_reqs_pager_", "\"");
                bool isVAlidUrl=true;
                int i = 2;
                while(isVAlidUrl)
                {
                    if(!string.IsNullOrEmpty(NextPAgeData))
                    {
                    string NextPageUrl = "https://www.facebook.com/friends/requests/outgoing/more/?page="+i+"&page_size=10&pager_id=outgoing_reqs_pager_" + NextPAgeData + "&__user="+UsreId+"&__a=1&__dyn=7nmajEyl35zoSt2u6aEyx90BCxO4oKAdBGfirWo8popyUW5ogxd6K59poW8xOdy8-&__req=a&__rev=1543964";
                    string NextPageSrc=HttpHelper.getHtmlfromUrl(new Uri(NextPageUrl));
                    NextPAgeData = Utils.getBetween(NextPageSrc, "pager_id=outgoing_reqs_pager_", "\\");
                    i++;

                    string[] UserData = Regex.Split(NextPageSrc, "_8o _8t lfloat _ohe");
                    foreach (string item in UserData)
                    {
                        if (item.Contains("www.facebook.com"))
                        {
                            string ProfileUrl = Utils.getBetween(item, "href=\\\"", "\"");
                            if (ProfileUrl.Contains("profile.php"))
                            {
                                string FriendId = Utils.getBetween(ProfileUrl, "id=", "&");
                                listFriend_Requests.Add(FriendId);
                            }
                            else
                            {
                                string FriendsName = Utils.getBetween(ProfileUrl, "www.facebook.com\\/", "?");
                                string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + FriendsName));
                                string UserId = Utils.getBetween(graphResp, "id\": \"", "\"");
                                listFriend_Requests.Add(UserId);

                            }
                        }

                    }
                    }
                    else
                    {
                     isVAlidUrl=false;
                    }
                }


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            return listFriend_Requests.Count;
        }

        #region Global Variables Of Invite Your Friends

        readonly object InviteYourFriendsThreadControllerlockr = new object();

        public bool isInviteYourFriendsStop = false;

        int InviteYourFriendsThreadControllerCount = 0;

        public List<Thread> lstInviteYourFriendsThreads = new List<Thread>();
        public List<string> lstInviteYourFriendsMobileNumber = new List<string>();

        public List<string> lstInviteYourFriendsEmails = new List<string>();



        public static int minDelayInviteYourFriends = 10;
        public static int maxDelayInviteYourFriends = 20;

        public static int NoOfFriendInviteYourFriends = 10;

        public static string Friends_InviteYourFriendsProcessUsing = string.Empty;

        #endregion


        public static int InviteYourFriendsNoOfThreads
        {
            get;
            set;
        }
        public static string InviteYourFriendsWithTxtMessage
        {
            get;
            set;
        }



        public void StartInviteYourFriends()
        {
            InviteYourFriendsThreadControllerCount = 0;
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreads > 0)
                {
                    numberOfAccountPatch = NoOfThreads;
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
                                lock (InviteYourFriendsThreadControllerlockr)
                                {
                                    try
                                    {
                                        if (InviteYourFriendsThreadControllerCount >= listAccounts.Count)
                                        {
                                            Monitor.Wait(InviteYourFriendsThreadControllerlockr);
                                        }
                                        try
                                        {
                                            string acc = account.Remove(account.IndexOf(':'));
                                            //Run a separate thread for each account
                                            FacebookUser item = null;
                                            FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                            if (item != null)
                                            {
                                                Thread profilerThread = new Thread(SendInviteYourFriendsMultiThreads);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;
                                                profilerThread.Start(new object[] { item });
                                                requestFriendsThreadControllerCount++;
                                                //tempCounterAccounts++; 
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
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void SendInviteYourFriendsMultiThreads(object parameters)
        {
            try
            {
                if (!isInviteYourFriendsStop)
                {
                    try
                    {
                        lstInviteYourFriendsThreads.Add(Thread.CurrentThread);
                        lstInviteYourFriendsThreads.Distinct();
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
                            // Call SendFriendRequests
                            SendInviteYourFriends(ref objFacebookUser);
                        }
                        else
                        {
                            // GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                            GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    //GlobusLogHelper.log.Debug("Process completed !!");
                    //GlobusLogHelper.log.Info("Process completed !!");

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
                    // if (!isRequestFriendsStop)
                    {
                        lock (InviteYourFriendsThreadControllerlockr)
                        {
                            InviteYourFriendsThreadControllerCount--;
                            Monitor.Pulse(InviteYourFriendsThreadControllerlockr);


                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }


        public void SendInviteYourFriends(ref FacebookUser fbUser) //ref BaseLib.ChilkatHttpHelpr chilkatHttpHepler
        {
            try
            {
              

                    SendFriendRequestViaEmals(ref fbUser);
        
            }


            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Finished sending Requests With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Finished sending Requests With Username : " + fbUser.username);

            Thread.Sleep(4000);
        }


        public void SendFriendRequestViaEmals(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
               

                string UserId = string.Empty;

                string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

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

                string fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "fb_dtsg");
              

                lstInviteYourFriendsEmails = lstInviteYourFriendsEmails.Distinct().ToList();
                int countFriendRequestsSent = 1;
                foreach (var item_Emails in lstInviteYourFriendsEmails)
                {
                    string PostUrl = "https://www.facebook.com/invite.php";
                    string PostData = "fb_dtsg=" + fb_dtsg + "&email_list=" + Uri.EscapeDataString(item_Emails) + "&personal=" + InviteYourFriendsWithTxtMessage + "&invite_lang%5B0%5D=en_US&invite_lang%5B1%5D=0&locales%5B0%5D=en_US&locales%5B1%5D=0";
                    string PostResponce=httpHelper.postFormData(new Uri(PostUrl),PostData);
                    if (PostResponce.Contains("Invite More Friends"))
                    {

                        GlobusLogHelper.log.Info(countFriendRequestsSent + " => Invite Sent With Username : " + fbUser.username);
                        GlobusLogHelper.log.Info(countFriendRequestsSent + " => Invite Sent With Username : " + fbUser.username);
                        try
                        {
                            int delayInSeconds = Utils.GenerateRandom(minDelayInviteYourFriends * 1000, maxDelayInviteYourFriends * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        countFriendRequestsSent = countFriendRequestsSent+1;

                    }
                    else
                    {
                        try
                        {
                            int delayInSeconds = Utils.GenerateRandom(minDelayInviteYourFriends * 1000, maxDelayInviteYourFriends * 1000);
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
                   
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }


}
