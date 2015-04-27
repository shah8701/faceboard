using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using System.Threading;
using faceboardpro;
using System.Text.RegularExpressions;
using System.Collections;
using Accounts;
using System.Web;



namespace WallPoster
{
    public class WallPostManager
    {

        #region Global Variables For Wall Poster
        public static bool IsUniqueMessagePosting = false;
        public static bool IsUniquePicPosting = false;
        readonly object lockrThreadControllerWallPoster = new object();
        public bool isStopWallPoster = false;
        int countThreadControllerWallPoster = 0;
        public static int TotalNoOfWallPosterCounter = 0;
        public static int messageCountWallPoster = 0;
        int countWallPoster = 1;
        public static bool statusForGreetingMsgWallPoster = false;

        public List<Thread> lstThreadsWallPoster = new List<Thread>();
        public List<string> lstWallPostURLsWallPoster = new List<string>();
        public List<string> lstMessagesWallPoster = new List<string>();
        public List<string> lstSpinnerWallMessageWallPoster = new List<string>();
        public List<string> lstGreetMsgWallPoster = new List<string>();
        public List<string> lstWallMessageWallPoster = new List<string>();

        public static int minDelayWallPoster = 10;
        public static int maxDelayWallPoster = 20;

        #endregion

        #region Global Variables For Post Pic On Wall

        readonly object lockrThreadControllerPostPicOnWall = new object();
        public bool isStopPostPicOnWall = false;
        public bool chkCountinueProcessGroupCamapinScheduler = false;
        public static bool isPrivacyOnlyMe = false;
        //public bool chkCountinueProcessGroupCamapinScheduler = false;
        //public bool chkCountinueProcessGroupCamapinScheduler = false;
       // public static bool chkCountinueProcessGroupCamapinScheduler = false;
                                               
        int countThreadControllerPostPicOnWall = 0;

        public List<Thread> lstThreadsPostPicOnWall = new List<Thread>();
        public List<string> lstPicturecollectionPostPicOnWall = new List<string>();
        public List<string> lstMessageCollectionPostPicOnWall = new List<string>();
        public List<string> lstWallPostShareLoadTargedUrls = new List<string>();

        public bool chkCountinueProcessContinueShareProcess = false;
        public bool chkWall_PostPicOnWall_ShareVideoOnlyMe = false;

        public static int NumberOfFriendsSendPicOnWall = 5;
        public static int minDelayPostPicOnWal = 10;
        public static int maxDelayPostPicOnWal = 20;

        #endregion

        #region Property For Wall Poster

        public int NoOfThreadsWallPoster
        {
            get;
            set;
        }

        public int NoOfFriendsWallPoster
        {
            get;
            set;
        }

        public bool UseAllUrlWallPoster
        {
            get;
            set;
        }

        public bool IsUseTextMessageWallPoster
        {
            get;
            set;
        }

        public bool IsUseURLsMessageWallPoster
        {
            get;
            set;
        }


        public bool UseOneMsgToAllFriendsWallPoster
        {
            get;
            set;
        }

        public bool UseRandomWallPoster
        {
            get;
            set;
        }

        public bool UseUniqueMsgToAllFriendsWallPoster
        {
            get;
            set;
        }

        public bool ChkSpinnerWallMessaeWallPoster
        {
            get;
            set;
        }

        public string MsgWallPoster
        {
            get;
            set;
        }

        public static string StartProcessUsingWallPoster
        {
            get;
            set;
        }

        #endregion

        #region Property For Post Pic On Wall

        public int NoOfThreadsPostPicOnWall
        {
            get;
            set;
        }

        public bool IsPostAllPicPostPicOnWall
        {
            get;
            set;
        }
        public bool chkWallPostPicOnWallWithMessage
        {
            get;
            set;
        }

        public bool chkWallWallPosterRemoveURLsMessages
        {
            get;
            set;
        }
        public static string StartProcessUsingPostPicOnWall
        {
            get;
            set;
        }
    

        #endregion

        #region Variables
       // public static bool statusForGreetingMsgWallPoster = true;
        public static int TotalNoOfWallPoster_Counter = 0;
       // public static List<string> lstGreetMsgWallPoster = new List<string>();
       //  public static List<string> lstSpinnerWallMessageWallPoster = new List<string>();
        //public static int countWallPoster = 0;
        // public List<string> lstMessageCollectionPostPicOnWall = new List<string>();
         //public List<string> lstPicturecollectionPostPicOnWall = new List<string>();

        #endregion

        private void PostMessageOnWall(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                string FirstName = string.Empty;
                string Name = string.Empty;
                string User = string.Empty;

                //if (HttpHelper.http.FinalRedirectUrl.Contains("https://"))
                {
                    postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

                    string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));

                    try
                    {
                        string GraphPagesource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + friendId));
                        string username = GraphPagesource.Substring(GraphPagesource.IndexOf("first_name\":")).Replace("first_name\":", string.Empty);
                        string[] UsernameArr = Regex.Split(username, "\"");
                        User = UsernameArr[1].Replace("\"", string.Empty).Replace(" ", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (!string.IsNullOrEmpty(User))
                    {
                        try
                        {
                            Name = pageSourceWallPost11.Substring(pageSourceWallPost11.IndexOf("<title>"), pageSourceWallPost11.IndexOf("</title>", pageSourceWallPost11.IndexOf("<title>")) - pageSourceWallPost11.IndexOf("<title>")).Replace("id&quot;:", string.Empty).Replace("<title>", string.Empty).Trim();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        Name = User;
                    }
                    if (Name.Contains(" "))
                    {
                        string[] flName = Regex.Split(Name, " ");
                        FirstName = flName[0];
                    }
                    else
                    {
                        FirstName = Name;

                    }
                    string smessage = string.Empty;
                    if (lstGreetMsgWallPoster.Count < 1)
                    {
                        smessage = "Hello";
                    }
                    else
                    {
                        smessage = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count - 1)];
                    }
                    wallmessage = smessage;

                    if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
                    {
                        GlobusLogHelper.log.Info(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);

                        string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);

                        string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

                        string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");

                        string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
                        string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111, "");
                        int length = ResponseWallPost.Length;

                        string postDataWalllpost1112 = string.Empty;
                        string ResponseWallPost2 = string.Empty;
                        if (!(length > 11000))
                        {
                            postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
                            ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112, "");

                            int length2 = ResponseWallPost2.Length;
                            if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;

                                GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

                                countWallPoster++;
                               
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                               
                                try
                                {
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;
                                GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                countWallPoster++;
                              
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                             
                                try
                                {
                                    #region InsertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                            }
                            if (ResponseWallPost2.Contains("Security Check Required") || ResponseWallPost2.Contains("A security check is required to proceed"))
                            {
                                GlobusLogHelper.log.Info("Security Check Required Account  :" + fbUser.username);
                                GlobusLogHelper.log.Debug("Security Check Required Account  :" + fbUser.username);
                            }
                        }
                        else
                        {
                            TotalNoOfWallPoster_Counter++;
                            GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                            countWallPoster++;
                           
                                try
                                {
                                    int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                           
                            try
                            {
                                #region InsertQuery
                                //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        System.Threading.Thread.Sleep(4000);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);

                        System.Threading.Thread.Sleep(1000);

                    }
                }
            }

        public void StartWallPoster()
        {
            countThreadControllerWallPoster = 0;
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsWallPoster > 0)
                {
                    numberOfAccountPatch = NoOfThreadsWallPoster;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {                    

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                lock (lockrThreadControllerWallPoster)
                                {
                                    try
                                    {
                                        if (countThreadControllerWallPoster >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerWallPoster);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsWallPoster);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerWallPoster++;
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

        public void StartMultiThreadsWallPoster(object parameters)
        {
            try
            {
                if (!isStopWallPoster)
                {
                    try
                    {
                        lstThreadsWallPoster.Add(Thread.CurrentThread);
                        lstThreadsWallPoster.Distinct();
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
                                // Call StartActionMessageReply
                                StartActionWallPoster(ref objFacebookUser);
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
                   // if (!isStopWallPoster)
                    {
                        lock (lockrThreadControllerWallPoster)
                        {
                            countThreadControllerWallPoster--;
                            Monitor.Pulse(lockrThreadControllerWallPoster);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionWallPoster(ref FacebookUser fbUser)
        {
            try
            {
                if (StartProcessUsingWallPoster .Contains("URLs Message"))
                {
                    WallPostingNew(ref fbUser); 
                }
                if (StartProcessUsingWallPoster.Contains("Text Message"))
                {
                    WallPostingWithTestMessage(ref fbUser);
                }
                if (StartProcessUsingWallPoster.Contains("Spinned Message"))
                {
                   // WallPosting(ref fbUser);
                    WallPostingWithTestMessage(ref fbUser);
                }
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void WallPosting(ref FacebookUser fbUser)
        {
            try
            {
                string UsreId = string.Empty;
                string attachmentParamsUrlInfoUser = string.Empty;
                string attachmentParamsUrlInfoCanonical=string.Empty;
                string attachmentParamsUrlInfoFinal = string.Empty;
                string attachmentParamsUrlInfoTitle = string.Empty;
                string attachmentParamsSummary = string.Empty;
                string attachmentParamsMedium = string.Empty;
                string attachmentParamsUrl = string.Empty;
                string attachmentType = string.Empty;
                string linkMetricsSource=string.Empty;
                string linkMetricsDomain = string.Empty;
                string linkMetricsBaseDomain = string.Empty;
                string linkMetricsTitleLen = string.Empty;
                string attachmentParamsfavicon = string.Empty;

                GlobusLogHelper.log.Info("Start Wall Posting With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Wall Posting With Username : " + fbUser.username);

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));             


                string ProFilePost = FBGlobals.Instance.fbProfileUrl;
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

                

                lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();

                GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                GlobusLogHelper.log.Debug("Please wait finding the friends ID...");


                if (NoOfFriendsWallPoster != 0)
                {
                    //GetAllFriends List

                    lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UsreId);
                }

                bool postsuccess = false;
                if (IsUseTextMessageWallPoster)
                {
                    MsgWallPoster = lstWallMessageWallPoster[Utils.GenerateRandom(0, lstWallMessageWallPoster.Count)];
                }
                if (IsUseURLsMessageWallPoster)
                {
                    MsgWallPoster = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                }
                if (ChkSpinnerWallMessaeWallPoster)
                {
                    MsgWallPoster = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count)];
                    lstMessagesWallPoster = lstSpinnerWallMessageWallPoster;
                }

           

                if (lstWallPostURLsWallPoster.Count > 0)
                {
                    lstFriend = lstFriend.Distinct().ToList();
                    if (!UseAllUrlWallPoster)
                    {
                        postsuccess = PostOnWallWithURLAndItsImage(ref fbUser, lstWallPostURLsWallPoster, lstFriend);
                        return;
                    }
                    else
                    {
                        PostOnWallWithAllURLAndItsImage(ref fbUser, lstWallPostURLsWallPoster, lstFriend);
                        //  return;
                    }
                }
                string profileUrl = ProFilePost + UsreId + "&sk=wall";
                string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl));                

                string wallmessage = MsgWallPoster;
                wallmessage = wallmessage.Replace("<friend first name>", string.Empty);

                if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
                {
                    if (lstWallPostURLsWallPoster.Count > 0)
                    {
                        wallmessage = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }

                    wallmessage = wallmessage.Replace("=", "%3D");
                    string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
                    string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                    }

                    string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
                    }
                    string appid = Utils.getBetween(pageSourceWallPostUser, "appid=", "&");
                    string ResponseWallPost = string.Empty;
                    string sessionId = Utils.GenerateTimeStamp();
                    string FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UsreId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
                    string attachment_params_urlInfo_canonical = Utils.getBetween(FirstResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_urlInfo_final = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_urlInfo_user = Utils.getBetween(FirstResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_favicon = Utils.getBetween(FirstResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_title = Utils.getBetween(FirstResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_summary = Utils.getBetween(FirstResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_images0 = Utils.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
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
                    string FinalResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), "composer_session_id=fee06a9d-c617-4071-8ed3-e308f966370a&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + wallmessage + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DAtbJaKNmbJs&xhpc_message=" + Uri.EscapeDataString(wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                             

                    string url = FBGlobals.Instance.WallPosterPostAjaxBzUrl;      //"https://www.facebook.com/ajax/bz";
                    string postdata1 = "__a=1&__dyn=7n8ahyj2JpGu5k9UmAAuUVCxO9w&__req=8&__user=" + UsreId + "&fb_dtsg="+fb_dtsg+"&ph=V3&q=%5B%7B%22user%22%3A%22100004496770422%22%2C%22page_id%22%3A%22d21m66%22%2C%22trigger%22%3A%22censorlogger%22%2C%22time%22%3A1378218459737%2C%22posts%22%3A%5B%5B%22censorlogger%22%2C%7B%22cl_impid%22%3A%2299c32d24%22%2C%22clearcounter%22%3A0%2C%22instrument%22%3A%22composer%22%2C%22elementid%22%3A%22u_0_s%22%2C%22parent_fbid%22%3A100004496770422%2C%22version%22%3A%22x%22%7D%2C28669%5D%5D%7D%5D&ts=1378218489409";
                    string PostRequestThumbNullstr1 = HttpHelper.postFormData(new Uri(url), postdata1, FBGlobals.Instance.fbhomeurl);          //"https://www.facebook.com/"
                    //string FirstResponse = HttpHelper.postFormData(new Uri(""), "");


                   string MessageUrl = Uri.EscapeUriString(wallmessage);
                    string kkk =string.Empty; 
                    string VUrl=string.Empty;
                    string jhj=string.Empty;     
                    string ss=string.Empty;
                    string PostRequestThumbNullstr = string.Empty;

                    if (wallmessage.Contains("http:"))
                    {
                        string[] FirstArr = Regex.Split(wallmessage, "http:");

                        VUrl = "http:" + FirstArr[1];
                        jhj = Uri.EscapeUriString(VUrl);
                        kkk = Uri.EscapeDataString(VUrl);

                        string URlThamb = FBGlobals.Instance.WallPosterGetAjaxComposerScraperUrl + kkk + "&composerurihash=4";         //"https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="
                        string PostUrlTanb = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1&__dyn=7n8ahyj2JpGudDgDxrHFXyG8qeyryo&__req=e&ttstamp=265816554111718986";
                        PostRequestThumbNullstr = HttpHelper.postFormData(new Uri(URlThamb), PostUrlTanb, FBGlobals.Instance.fbhomeurl);
                    }
                    else
                    {
                        try
                        {
                            if (wallmessage.Contains("https:"))
                            {
                                string[] FirstArr = Regex.Split(wallmessage, "https:");
                                if (FirstArr.Count() > 1)
                                {
                                    VUrl = "https:" + FirstArr[1];
                                }
                                else
                                {
                                    VUrl = FirstArr[0];
                                }
                                jhj = Uri.EscapeUriString(VUrl);
                                kkk = Uri.EscapeDataString(VUrl);

                               

                                string URlThamb = FBGlobals.Instance.WallPosterGetAjaxComposerScraperUrl + kkk + "&composerurihash=4";     //"https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="
                                string PostUrlTanb = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1&__dyn=7n8ahyj2JpGudDgDxrHFXyG8qeyryo&__req=e&ttstamp=265816554111718986";
                                PostRequestThumbNullstr = HttpHelper.postFormData(new Uri(URlThamb), PostUrlTanb, FBGlobals.Instance.fbhomeurl);
                            }
                            else
                            {
                               
                                VUrl = wallmessage;                               
                                jhj = Uri.EscapeUriString(VUrl);
                                kkk = Uri.EscapeDataString(VUrl);

                                string URlThamb = FBGlobals.Instance.WallPosterGetAjaxComposerScraperUrl + kkk + "&composerurihash=4";     //"https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="
                                string PostUrlTanb = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1&__dyn=7n8ahyj2JpGudDgDxrHFXyG8qeyryo&__req=e&ttstamp=265816554111718986";
                                PostRequestThumbNullstr = HttpHelper.postFormData(new Uri(URlThamb), PostUrlTanb, FBGlobals.Instance.fbhomeurl);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }
                    #region MyRegion

                    //if (PostRequestThumbNullstr.Contains("attachment[params][urlInfo][user]"))
                    //{
                    //    attachmentParamsUrlInfoUser = GlobusHttpHelper.GetAttachmentParamsUrlInfoUser(PostRequestThumbNullstr, "[params][urlInfo][user]");
                    //}
                    //if (PostRequestThumbNullstr.Contains("attachment[params][urlInfo][canonical]"))
                    //{
                    //    attachmentParamsUrlInfoCanonical = GlobusHttpHelper.GetAttachmentParamsUrlInfoCanonical(PostRequestThumbNullstr, "[params][urlInfo][canonical]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("attachment[params][urlInfo][final]"))
                    //{
                    //    attachmentParamsUrlInfoFinal = GlobusHttpHelper.GetAttachmentParamsUrlInfoFinal(PostRequestThumbNullstr, "[params][urlInfo][final]");
                    //}
                    //if (PostRequestThumbNullstr.Contains("attachment[params][summary]"))
                    //{
                    //    attachmentParamsSummary = GlobusHttpHelper.GetAttachmentParamsSummary(PostRequestThumbNullstr, "attachment[params][summary]");
                    //    //attachmentParamsSummary = attachmentParamsSummary.Replace(" ", "%20");
                    //}

                    //if (PostRequestThumbNullstr.Contains("attachment[params][medium]"))
                    //{
                    //    attachmentParamsMedium = GlobusHttpHelper.GetAttachmentParamsMedium(PostRequestThumbNullstr, "attachment[params][medium]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("attachment[params][url]"))
                    //{
                    //    attachmentParamsUrl = GlobusHttpHelper.GetAttachmentParamsUrl(PostRequestThumbNullstr, "attachment[params][url]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("attachment[type]"))
                    //{
                    //    attachmentType = GlobusHttpHelper.GetAttachmentType(PostRequestThumbNullstr, "attachment[type]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("link_metrics[source]"))
                    //{
                    //    linkMetricsSource = GlobusHttpHelper.GetLinkMetricsSource(PostRequestThumbNullstr, "link_metrics[source]");
                    //}
                    //if (PostRequestThumbNullstr.Contains("link_metrics[domain]"))
                    //{
                    //    linkMetricsDomain = GlobusHttpHelper.GetLinkMetricsDomain(PostRequestThumbNullstr, "link_metrics[domain]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("link_metrics[base_domain]"))
                    //{
                    //    linkMetricsBaseDomain = GlobusHttpHelper.GetLinkMetricsBaseDomain(PostRequestThumbNullstr, "ink_metrics[base_domain]");
                    //}

                    //if (PostRequestThumbNullstr.Contains("link_metrics[title_len]"))
                    //{
                    //    linkMetricsTitleLen = GlobusHttpHelper.GetlinkMetricsTitleLen(PostRequestThumbNullstr, "link_metrics[title_len]");
                    //}
                    //if (PostRequestThumbNullstr.Contains("attachment[params][favicon]"))
                    //{
                    //    attachmentParamsfavicon = GlobusHttpHelper.GetAttachmentParamsfavicon(PostRequestThumbNullstr, "attachment[params][favicon]");
                    //} 
                    #endregion


                   

                   // if (MessageUrl.Contains("www"))
                    {
                       

                        Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
                        if (PostRequestThumbNullstr.Contains("name=") && PostRequestThumbNullstr.Contains("value="))
                        {
                            try
                            {
                                string[] strNameValue = Regex.Split(PostRequestThumbNullstr, "name=");
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
                            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                        }

                        string partPostData = string.Empty;
                        foreach (var dicNameValueitem in dicNameValue)
                        {
                            try
                            {

                                if (dicNameValueitem.Key == "attachment[params][title]")
                                {
                                    string value = dicNameValueitem.Value;

                                    #region CodeCommented
                                    // string HTmlDEcode = Uri.EscapeDataString(value);
                                    // string UrlDEcode = Uri.EscapeDataString(HTmlDEcode);
                                    //partPostData = partPostData + dicNameValueitem.Key + "=" + UrlDEcode + "&"; 
                                    #endregion

                                    partPostData = partPostData + dicNameValueitem.Key + "=" + value + "&";
                                }
                                else
                                {
                                    partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
                                }

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        string resp = string.Empty;
                        string FinalPostData = string.Empty;
                        try
                        {
                

                            string  xhpc_message_text = wallmessage;

                            if (chkWallWallPosterRemoveURLsMessages == true)
                            {
                                xhpc_message_text = "";

                      
                            }
                            else
                            {
                                xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);
                            }

                            string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text +"&xhpc_message=" + xhpc_message_text;
                            partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
                            string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
                            string PostDataa = valuesArr[1].Replace("&aktion=post", messages);

                            FinalPostData = "composer_session_id=470d7ef6-16f4-4a90-a04a-67d464d42b8d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + PostDataa + "&link_metrics[title_len]=6&link_metrics[summary_len]=159&link_metrics[min_dimensions][0]=70&link_metrics[min_dimensions][1]=70&link_metrics[images_with_dimensions]=2&link_metrics[images_pending]=0&link_metrics[images_fetched]=0&link_metrics[image_dimensions][0]=269&link_metrics[image_dimensions][1]=95&link_metrics[images_selected]=2&link_metrics[images_considered]=2&link_metrics[images_cap]=3&link_metrics[images_type]=ranked&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=2&composer_metrics[images_loaded]=2&composer_metrics[images_shown]=2&composer_metrics[load_duration]=113&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1393564470&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8aqEAMCBCFUSt2u6aOGUGy6zECiq78hAKGgyiGGeqheCu&__req=b&ttstamp=265816655451197277&__rev=1140660";
                            #region OldPostData Change by  ajay k yadav 28-02-2014

                            //FinalPostData = "fb_dtsg=" + fb_dtsg + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_fbx=1&xhpc_timeline=&xhpc_composerid=" + PostDataa + "&composer_link_best_image_w=320&composer_link_best_image_h=180&composer_link_image_selected=0&composer_link_images_provided=1&composer_link_images_loaded=1&composer_link_images_shown=1&composer_link_load_duration=10&composer_link_sort_order=0&composer_link_selector_type=UIThumbPager_2&is_explicit_place=&composertags_place=&composertags_place_name=&composer_session_id=1358139593&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__req=16&phstamp=165816853711121159712192"; 
                            #endregion
                            ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl), FinalPostData);  
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        
                    }

                    if (ResponseWallPost.Length >= 300)
                    {
                        TotalNoOfWallPoster_Counter++;

                        GlobusLogHelper.log.Info("Posted message on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Posted message on own wall " + fbUser.username);
                    }

                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't post on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Couldn't post on own wall " + fbUser.username);
                    }
                }

                var itemId = lstFriend.Distinct();

               // GlobusLogHelper.log.Info("Found Id Of Friend Is : " + lstFriend.Count());
               //GlobusLogHelper.log.Debug("Found Id Of Friend Is : " + lstFriend.Count());

                int CountPostWall = 1;

               // messageCountWallPoster = 5;
                messageCountWallPoster = NoOfFriendsWallPoster;

                int friendval = messageCountWallPoster;
                int friendCount = 0;

                if (itemId.Count() > friendval)
                {
                    friendCount = friendval;
                }
                else
                {
                    friendCount = itemId.Count();
                }

                try
                {
                    ///Generate a random no list ranging 0-lstMessages.Count

                    ArrayList randomNoList = Utils.RandomNumbers(lstMessagesWallPoster.Count - 1);
                    randomNoList = Utils.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
                    int msgIndex = 0;

                    foreach (string friendId in itemId)
                    {
                        if (CountPostWall > friendCount)
                        {
                            return;
                        }
                        try
                        {
                            #region SelectQuery
                            // System.Data.DataSet ds = new DataSet();
                            try
                            {
                                //string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
                                // ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            //if (ds.Tables[0].Rows.Count < 1)
                            {
                                // return; 
                            #endregion

                                string message = string.Empty;
                                if (UsreId != friendId)
                                {
                                    #region Select Msg according to Mode
                                    try
                                    {
                                        ///Normal, 1 msg to all friends
                                        if (UseOneMsgToAllFriendsWallPoster)
                                        {
                                            message = MsgWallPoster.Replace(" ", " ");  //%20;
                                        }

                                        ///For Random, might be Unique, might not be
                                        else if (UseRandomWallPoster)
                                        {
                                            if (msgIndex < randomNoList.Count)
                                            {
                                                try
                                                {
                                                    msgIndex = (int)randomNoList[msgIndex];
                                                    message = lstWallPostURLsWallPoster[msgIndex];                                                   
                                                    msgIndex++;
                                                }catch(Exception ex)
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                                   // message = MsgWallPoster;
                                                    GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
                                                }
                                            }
                                            else if (lstWallPostURLsWallPoster.Count > msgIndex)
                                            {
                                                message = lstWallPostURLsWallPoster[msgIndex];
                                                msgIndex++;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    //msgIndex = 0;
                                                    randomNoList = Utils.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
                                                    message = lstMessagesWallPoster[msgIndex];
                                                    message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];
                                                    msgIndex++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                                }
                                            }
                                        }

                                        ///For Unique or Different Msg for each friend                                        

                                        else if (UseUniqueMsgToAllFriendsWallPoster)
                                        {
                                            if (lstMessagesWallPoster.Count > countWallPoster - 1)
                                            {
                                              
                                                message = lstMessagesWallPoster[countWallPoster - 1];
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                                   //message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                                }
                                                catch(Exception ex)
                                                {
                                                    message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
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

                                    try
                                    {
                                        if (!ChkSpinnerWallMessaeWallPoster)
                                        {
                                            if (string.IsNullOrEmpty(message))
                                            {
                                                message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                            }
                                            PostOnFriendsWall(friendId, message, ref fbUser, ref UsreId);
                                        }
                                        else
                                        {
                                            if (lstSpinnerWallMessageWallPoster.Count > 0)
                                            {
                                                PostOnFriendWallUsingSpinMsg(friendId, message, ref fbUser, ref UsreId);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    CountPostWall++;
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

                GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //GlobusLogHelper.log.Info(" Wall Message  not valide message must be contains http or https ");
                //GlobusLogHelper.log.Debug("Wall Message  not valide message must be contains http or https ");
            }
            finally
            {

                GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username);
                // HttpHelper.http.Dispose(); 
            }
        }

        //New Method by Lijo
        public void WallPostingNew(ref FacebookUser fbUser)
        {
            try
            {
                string UserId = string.Empty;
                string attachmentParamsUrlInfoUser = string.Empty;
                string attachmentParamsUrlInfoCanonical = string.Empty;
                string attachmentParamsUrlInfoFinal = string.Empty;
                string attachmentParamsUrlInfoTitle = string.Empty;
                string attachmentParamsSummary = string.Empty;
                string attachmentParamsMedium = string.Empty;
                string attachmentParamsUrl = string.Empty;
                string attachmentType = string.Empty;
                string linkMetricsSource = string.Empty;
                string linkMetricsDomain = string.Empty;
                string linkMetricsBaseDomain = string.Empty;
                string linkMetricsTitleLen = string.Empty;
                string attachmentParamsfavicon = string.Empty;

                GlobusLogHelper.log.Info("Start Wall Posting With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Wall Posting With Username : " + fbUser.username);

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

               

                string ProFilePost = FBGlobals.Instance.fbProfileUrl;
                string tempUserID = string.Empty;
                List<string> lstFriend = new List<string>();

                UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }
                lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();
                bool postsuccess = false;
                if (IsUseTextMessageWallPoster)
                {
                    MsgWallPoster = lstWallMessageWallPoster[Utils.GenerateRandom(0, lstWallMessageWallPoster.Count)];
                }
                if (IsUseURLsMessageWallPoster)
                {
                    MsgWallPoster = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                }
                if (ChkSpinnerWallMessaeWallPoster)
                {
                    MsgWallPoster = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count)];
                    lstMessagesWallPoster = lstSpinnerWallMessageWallPoster;
                }

             
                string profileUrl = ProFilePost + UserId + "&sk=wall";
                string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl));
                string wallmessage = string.Empty;                                
                wallmessage = wallmessage.Replace("<friend first name>", string.Empty);

                if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
                {
                    if (lstWallPostURLsWallPoster.Count > 0)
                    {
                        wallmessage = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }

                    wallmessage = wallmessage.Replace("=", "%3D");
                    string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
                    string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                    }

                    string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
                    }
                    string appid = Utils.getBetween(pageSourceWallPostUser, "appid=", "&");
                    string ResponseWallPost = string.Empty;
                    string sessionId = Utils.GenerateTimeStamp();                    
                    //First Postdata
                    string FirstResponse=string.Empty;
                    string SecondResponse = string.Empty;
                    string xhpc_message_text = string.Empty;

                    if (chkWallWallPosterRemoveURLsMessages)
                    {

                        if (wallmessage.Contains("https:")||(wallmessage.Contains("http:")))
                        {
                            string[] arr = wallmessage.Split(':');
                            if (arr.Count() == 3)
                            {
                                xhpc_message_text = arr[0];
                            }
                            else
                            {
                                xhpc_message_text = string.Empty;
                            }
                        }                       

                    }


                    try
                    {
                        string[] arr = wallmessage.Split(':');
                        if (arr.Count() > 1 && arr.Count()==2)
                        {
                            xhpc_message_text = wallmessage.Split(':')[0] +":"+ wallmessage.Split(':')[1];
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }
                    if (!wallmessage.Contains("https://") && !wallmessage.Contains("http://"))
                    {
                        wallmessage = "https://" + wallmessage;
                    }
                    try
                    {
                        string Message = wallmessage.Split(':')[1] + ":" + wallmessage.Split(':')[2];
                        wallmessage = Message;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }

                    try
                    {
                        string PostDataFirst="fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UserId + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=prompt&loaded_components[8]=withtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=ogtaggericon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMCBynzpQ9UoHFaeFDzECQqbx2mbACFaaGGzCC_826m6oDAyoSnx2ubhHAG8Kl1e&__req=e&ttstamp=265817274821019054566657120&__rev=1400559";
                        FirstResponse = HttpHelper.postFormDataUpdated(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), PostDataFirst);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }
                    if (FirstResponse.Contains("Sorry, we got confused"))
                    {
                        try
                        {
                            FirstResponse = HttpHelper.postFormDataUpdated(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UserId + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(ex.StackTrace);
                        }
                    }
                    try
                    {
                        string PostDataSeccond= "fb_dtsg=" + fb_dtsg + "&composerid=" + UserId + "&targetid=" + UserId + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=prompt&loaded_components[8]=withtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=ogtaggericon&loaded_components[11]=mainprivacywidget&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBgjw&__req=f&ttstamp=265817274821019054566657120&__rev=1400559";
                        SecondResponse = HttpHelper.postFormDataUpdated(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2"), PostDataSeccond);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }
                    string ss = Uri.EscapeDataString("&#x111;");



                    string tagger_session_id = Utils.getBetween(FirstResponse, "tagger_session_id\\\" value=\\\"", "\\\"");
                    string composer_predicted_city = Utils.getBetween(FirstResponse, "composer_predicted_city\\\" value=\\\"","\\\"");
                    string attachment_params = Utils.getBetween(SecondResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
                    string attachment_params_urlInfo_canonical = Utils.getBetween(SecondResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_urlInfo_final = Utils.getBetween(SecondResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_urlInfo_user = Utils.getBetween(SecondResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_favicon = Utils.getBetween(SecondResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_title = Utils.getBetween(SecondResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");

                    attachment_params_title = HttpUtility.HtmlDecode(attachment_params_title);

                    string attachment_params_summary = Utils.getBetween(SecondResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#xea;", "").Replace("&#x1eb1;", "ằ").Replace("&#xe1;", "ằ").Replace("&#xfa;", "ú");
                    attachment_params_summary = HttpUtility.HtmlDecode(attachment_params_summary);

                    string attachment_params_images0 = Utils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#", string.Empty);
                    string attachment_params_medium = Utils.getBetween(SecondResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_url = Utils.getBetween(SecondResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_video0_type = Utils.getBetween(SecondResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_video0_src = Utils.getBetween(SecondResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_video0_width = Utils.getBetween(SecondResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_video0_height = Utils.getBetween(SecondResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_params_video0_secure_url = Utils.getBetween(SecondResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string attachment_type = Utils.getBetween(SecondResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_source = Utils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_domain = Utils.getBetween(SecondResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_base_domain = Utils.getBetween(SecondResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_title_len = Utils.getBetween(SecondResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_summary_len = Utils.getBetween(SecondResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_min_dimensions0 = Utils.getBetween(SecondResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_min_dimensions1 = Utils.getBetween(SecondResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_with_dimensions = Utils.getBetween(SecondResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_pending = Utils.getBetween(SecondResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_fetched = Utils.getBetween(SecondResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_image_dimensions0 = Utils.getBetween(SecondResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_image_dimensions1 = Utils.getBetween(SecondResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_considered = Utils.getBetween(SecondResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_selected = Utils.getBetween(SecondResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_cap = Utils.getBetween(SecondResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
                    string link_metrics_images_type = Utils.getBetween(SecondResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                  //  string xhpc_message_text = wallmessage;
                    if (chkWallWallPosterRemoveURLsMessages == true)
                    {
                     
                    }
                    else
                    {
                        //xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);
                    }
                    //Final PostData
                    if (string.IsNullOrEmpty(FirstResponse))
                    {
                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
                    }
                    else
                    {
                        if (chkWallWallPosterRemoveURLsMessages)
                        {

                            if (xhpc_message_text.Contains("https:"))
                            {
                                string[] arr = xhpc_message_text.Split(':');
                                if (arr.Count() == 3)
                                {
                                    xhpc_message_text = arr[0];
                                }
                                else
                                {
                                    xhpc_message_text = string.Empty;
                                }     
                            }
                            else
                            {

                            }
                                                  
                        }

                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + xhpc_message_text + "&xhpc_message_text=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
                    }
                    if (ResponseWallPost.Contains("The message could not be posted to this Wall.") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action."))
                    {
                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                    }
                    if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
                    {
                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
                    }
                    if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
                    {

                        string WallPostData = "composer_session_id=7a1d3f8c-ec77-4167-8ef6-5df4b1bc33aa&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229d966a62%22%2C%22clearcounter%22%3A1%2C%22elementid%22%3A%22u_0_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + UserId + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073742507&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_composer&__user="+UserId+"&__a=1&__dyn=7n8anEyl2lm9udDgDxyKAEWCueyrhEK49oKiWFaaBGeqrYw8pojLyui9zpUgDyQqUkBBzEy6Kdy8-&__req=29&ttstamp=2658172568911171657910267120&__rev=1503785";
                      ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), WallPostData);
                    }
                    if (ResponseWallPost.Length >= 300)
                    {
                        TotalNoOfWallPoster_Counter++;

                        GlobusLogHelper.log.Info("Posted message on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Posted message on own wall " + fbUser.username);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't post on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Couldn't post on own wall " + fbUser.username);
                    }
                }
                GlobusLogHelper.log.Info("Please wait finding the friend's IDs...");
                GlobusLogHelper.log.Debug("Please wait finding the friend's IDs...");
                if (NoOfFriendsWallPoster != 0)
                {
                    //GetAllFriends List
                    lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UserId);
                }
                var itemId = lstFriend.Distinct();
                int CountPostWall = 0;

                // messageCountWallPoster = 5;
                messageCountWallPoster = NoOfFriendsWallPoster;

                int friendval = messageCountWallPoster;
                int friendCount = 0;

                if (itemId.Count() > friendval)
                {
                    friendCount = friendval;
                }
                else
                {
                    friendCount = itemId.Count();
                }

                try
                {
                    ///Generate a random no list ranging 0-lstMessages.Count

                    ArrayList randomNoList = Utils.RandomNumbers(lstMessagesWallPoster.Count - 1);
                    randomNoList = Utils.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
                    int msgIndex = 0;

                    foreach (string friendId in itemId)
                    {
                        if (CountPostWall >= friendCount)
                        {
                            return;
                        }
                        try
                        {
                            #region SelectQuery
                            // System.Data.DataSet ds = new DataSet();
                            try
                            {
                                //string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
                                // ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            //if (ds.Tables[0].Rows.Count < 1)
                            {
                                // return; 
                            #endregion

                                string message = string.Empty;
                                if (UserId != friendId)
                                {
                                    #region Select Msg according to Mode
                                    try
                                    {
                                        ///Normal, 1 msg to all friends
                                        if (UseOneMsgToAllFriendsWallPoster)
                                        {
                                            message = MsgWallPoster.Replace(" ", " ");  //%20;
                                        }

                                        ///For Random, might be Unique, might not be
                                        else if (UseRandomWallPoster)
                                        {
                                            if (msgIndex < randomNoList.Count)
                                            {
                                                try
                                                {
                                                    msgIndex = (int)randomNoList[msgIndex];
                                                    message = lstWallPostURLsWallPoster[msgIndex];
                                                    msgIndex++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                                    // message = MsgWallPoster;
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }
                                            else if (lstWallPostURLsWallPoster.Count > msgIndex)
                                            {
                                                message = lstWallPostURLsWallPoster[msgIndex];
                                                msgIndex++;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    //msgIndex = 0;
                                                    randomNoList = Utils.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
                                                    message = lstMessagesWallPoster[msgIndex];
                                                    message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];
                                                    msgIndex++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                                }
                                            }
                                        }

                                        ///For Unique or Different Msg for each friend                                        

                                        else if (UseUniqueMsgToAllFriendsWallPoster)
                                        {
                                            if (lstMessagesWallPoster.Count > countWallPoster - 1)
                                            {

                                                message = lstMessagesWallPoster[countWallPoster - 1];
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                                    //message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                                }
                                                catch (Exception ex)
                                                {
                                                    message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
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

                                    try
                                    {
                                        if (!ChkSpinnerWallMessaeWallPoster)
                                        {
                                            if (string.IsNullOrEmpty(message))
                                            {
                                                message = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                                            }
                                            PostOnFriendsWall(friendId, message, ref fbUser, ref UserId);
                                        }
                                        else
                                        {
                                            if (lstSpinnerWallMessageWallPoster.Count > 0)
                                            {
                                                PostOnFriendWallUsingSpinMsg(friendId, message, ref fbUser, ref UserId);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    CountPostWall++;
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

                GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //GlobusLogHelper.log.Info(" Wall Message  not valide message must be contains http or https ");
                //GlobusLogHelper.log.Debug("Wall Message  not valide message must be contains http or https ");
            }
            finally
            {
                if (!isStopWallPoster)
                {

                    GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username); 
                }
                // HttpHelper.http.Dispose(); 
            }
        }      


        private void PostOnFriendsWall(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UserId)
        {           

            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string kkk = string.Empty;
                string VUrl = string.Empty;
                string jhj = string.Empty;
                string ss = string.Empty;

                string friendid = friendId;
                string wallMessage = wallmessage;
                DateTime datetiemvalue = DateTime.Now;
                TimeSpan xcx = DateTime.Now - datetiemvalue;
                string xhpc_message_text = string.Empty;

                if (chkWallWallPosterRemoveURLsMessages == true)
                {


                    if (wallmessage.Contains("https:") || wallmessage.Contains("http:") || wallmessage.Contains("www"))
                    {
                        try
                        {
                            string[] arr = wallmessage.Split(':');
                            if (arr.Count() == 3)
                            {
                                xhpc_message_text = arr[0];
                                wallmessage = arr[1] + ":" + arr[2];
                            }
                            else
                            {
                                xhpc_message_text = string.Empty;
                            }
                        }
                        catch { };
                    }
                    else
                    {

                    }


                }
                else
                {
                    if (wallmessage.Contains("https:") || wallmessage.Contains("http:") || wallmessage.Contains("www"))
                    {
                        try
                        {
                            string[] arr = wallmessage.Split(':');
                            if (arr.Count() == 3)
                            {
                                xhpc_message_text = arr[0];
                                wallmessage = arr[1] + ":" + arr[2];
                            }
                            else
                            {
                                xhpc_message_text = string.Empty;
                            }
                        }
                        catch { };
                    }
                    else
                    {

                    }


                }



                if (!statusForGreetingMsgWallPoster)
                {
                    string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                    postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                    string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));
                    string appid = Utils.getBetween(pageSourceWallPost11, "appid=", "&");
                    string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);
                    string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");
                    string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");                  

                    if (postUrl.Contains("https://"))
                    {
                        postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                        pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));

                        if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
                        {
                            GlobusLogHelper.log.Info(countWallPoster.ToString() + " Posting on wall " + postUrl);
                            GlobusLogHelper.log.Debug(countWallPoster.ToString() + " Posting on wall " + postUrl);
                            string ResponseWallPost1 = string.Empty;
                
                        
                            try
                            {
                                string FirstResponse = string.Empty;
                                string SecondResponse = string.Empty;
                                if (!wallmessage.Contains("https://") && !wallmessage.Contains("http://"))
                                {
                                    wallmessage = "https://" + wallmessage;
                                }
                                try
                                {
                                    string PostDataUrl = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1";
                                    string PostData="fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
                                    FirstResponse = HttpHelper.postFormData(new Uri(PostDataUrl), PostData);

                                    if (FirstResponse.Contains("Who are you with?"))
                                    {
                                        string Post_Url = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av="+UserId+"&composerurihash=1";
                                        string PostData_Url = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid="+xhpc_targetid+"&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=backdateicon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=backdateicon&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&nctr[_mod]=pagelet_timeline_recent&__user="+UserId+"&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqA8ABGeqrWo8pojByUWdDx2ubhHximmey8qUS8zU&__req=e&ttstamp=26581729512056122661171216683&__rev=1503785";
                                        FirstResponse = HttpHelper.postFormData(new Uri(Post_Url), PostData_Url);
                                        if (!FirstResponse.Contains("Who are you with?"))
                                        {
                                            string Post_Url2 = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + UserId + "&composerurihash=1";
                                            string PostData_Url2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&photoswaterfallid=29f9db5dfb9b52c5a4a760ee4510ea07&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=ogtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=withtaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=backdateicon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=aJioznEyl2qm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1e&ttstamp=2658170679789798165112110106695577&__rev=1612042";
                                            FirstResponse = HttpHelper.postFormData(new Uri(Post_Url2), PostData_Url2);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                }
                                if (FirstResponse.Contains("Sorry, we got confused"))
                                {
                                    try
                                    {
                                        FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UserId + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(ex.StackTrace);
                                    }
                                }
                                try
                                {
                                    SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.StackTrace);
                                }
                                string tagger_session_id = Utils.getBetween(FirstResponse, "tagger_session_id\\\" value=\\\"", "\\\"");
                                string composer_predicted_city = Utils.getBetween(FirstResponse, "composer_predicted_city\\\" value=\\\"", "\\\"");
                                string attachment_params = Utils.getBetween(SecondResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
                                string attachment_params_urlInfo_canonical = Utils.getBetween(SecondResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_urlInfo_final = Utils.getBetween(SecondResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_urlInfo_user = Utils.getBetween(SecondResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_favicon = Utils.getBetween(SecondResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                
                                string attachment_params_title = Utils.getBetween(SecondResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                attachment_params_title = HttpUtility.HtmlDecode(attachment_params_title);

                                string attachment_params_summary = Utils.getBetween(SecondResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

                                attachment_params_summary = HttpUtility.HtmlDecode(attachment_params_summary);

                                string attachment_params_images0 = Utils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_medium = Utils.getBetween(SecondResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_url = Utils.getBetween(SecondResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_video0_type = Utils.getBetween(SecondResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_video0_src = Utils.getBetween(SecondResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_video0_width = Utils.getBetween(SecondResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_video0_height = Utils.getBetween(SecondResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_params_video0_secure_url = Utils.getBetween(SecondResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string attachment_type = Utils.getBetween(SecondResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_source = Utils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_domain = Utils.getBetween(SecondResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_base_domain = Utils.getBetween(SecondResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_title_len = Utils.getBetween(SecondResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_summary_len = Utils.getBetween(SecondResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_min_dimensions0 = Utils.getBetween(SecondResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_min_dimensions1 = Utils.getBetween(SecondResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_with_dimensions = Utils.getBetween(SecondResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_pending = Utils.getBetween(SecondResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_fetched = Utils.getBetween(SecondResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_image_dimensions0 = Utils.getBetween(SecondResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_image_dimensions1 = Utils.getBetween(SecondResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_considered = Utils.getBetween(SecondResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_selected = Utils.getBetween(SecondResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_cap = Utils.getBetween(SecondResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
                                string link_metrics_images_type = Utils.getBetween(SecondResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
                               
                                if (chkWallWallPosterRemoveURLsMessages == true)
                                {
                                   

                                        if (xhpc_message_text.Contains("https:"))
                                        {
                                            string[] arr = xhpc_message_text.Split(':');
                                            if (arr.Count() == 3)
                                            {
                                                xhpc_message_text = arr[0];
                                                wallmessage = arr[1] +":"+ arr[2];
                                            }
                                            else
                                            {
                                                xhpc_message_text = string.Empty;
                                            }
                                        }
                                        else
                                        {

                                        }

                                   
                                  //  xhpc_message_text = wallmessage;


                                }
                                else
                                {
                                  //  xhpc_message_text = Uri.EscapeDataString(wallmessage + "    :    " + xhpc_message_text);
                                }
                                //Final PostData
                                xhpc_message_text=Uri.EscapeDataString(xhpc_message_text);
                                string ResponseWallPost = string.Empty;
                                if (string.IsNullOrEmpty(FirstResponse))
                                {
                                    string PostData="composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559";
                                    ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), PostData);
                                }
                                else
                                {
                                    if (chkWallWallPosterRemoveURLsMessages == true)
                                    {
                                        string PostData1 = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" +(xhpc_message_text) + "&xhpc_message_text=" +(xhpc_message_text) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
                                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), PostData1);
                                    }
                                    else
                                    {

                                        string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" +(xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
                                        ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), PostData);
                                    }
                                }
                                if (ResponseWallPost.Contains("The message could not be posted to this Wall.") ||ResponseWallPost.Contains ("Message Failed\",\"errorDescription\"") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action"))
                                {
                                   // chkWallWallPosterRemoveURLsMessages Url

                                    ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
                                }
                                if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
                                {
                                    ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
                                }
                                if (ResponseWallPost.Contains("Sorry, the privacy setting on this post means that you can't share it")&&ResponseWallPost.Contains("Could Not Post to Timeline"))
                                {
                                     ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UserId), "composer_session_id=c9e72d37-ce06-40d8-a3f3-b35c8316bcbd&fb_dtsg="+fb_dtsg+"&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid="+xhpc_composerid+"&xhpc_targetid="+xhpc_targetid+"&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229dbcb61a%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1e%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A"+xhpc_targetid+"%7D&xhpc_message_text="+xhpc_message_text+"&xhpc_message="+xhpc_message_text+"&aktion=post&app_id="+appid+"&attachment[params][0]="+attachment_params+"&attachment[params][1]=1073742507&attachment[type]=2&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id="+tagger_session_id+"&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city="+composer_predicted_city+"&nctr[_mod]=pagelet_timeline_recent&__user="+UserId+"&__a=1&__dyn=7n8ajEyl2lm9udDgDxyKAEWCueyp9Esx6iWF299qzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=26581729512056122661171216683&__rev=1503785");
                                }
                                if (ResponseWallPost.Contains("The message could not be posted to this Wall.") && ResponseWallPost.Contains("The message could not be posted to this Wall"))
                                {
                                    ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UserId), "composer_session_id=2f37c190-d9b1-4d18-aa9d-f4d3d85e687d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2227babdd5%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_4_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=7&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1k&ttstamp=2658170679789798165112110106695577&__rev=1612042"); 
                                }
                                ResponseWallPost1 = ResponseWallPost;
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }

                            #region OldPostData
                            //  string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
                           // string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "composer_session_id=9e791245-11ba-48d6-85fd-114afd7a5061&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&clp=%7B%22cl_impid%22%3A%2276be45eb%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_29%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A100000311523101%7D&xhpc_message_text=" + xhpc_message_text + "%20&xhpc_message=www.google.com%20&aktion=post&app_id=2309869772&attachment[params][urlInfo][canonical]=http%3A%2F%2Fwww.google.com%2F&attachment[params][urlInfo][final]=http%3A%2F%2Fwww.google.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.google.com%2F&attachment[params][favicon]=http%3A%2F%2Fwww.google.com%2Ffavicon.ico&attachment[params][title]=Google&attachment[params][summary]=Search%20the%20world's%20information%2C%20including%20webpages%2C%20images%2C%20videos%20and%20more.%20Google%20has%20many%20special%20features%20to%20help%20you%20find%20exactly%20what%20you're%20looking%20for.&attachment[params][images][0]=https%3A%2F%2Ffbexternal-a.akamaihd.net%2Fsafe_image.php%3Fd%3DAQB9wubxdXiYJSSe%26w%3D100%26h%3D100%26url%3Dhttp%253A%252F%252Fwww.google.com%252Fimages%252Fsrpr%252Flogo9w.png%26cfs%3D1%26upscale&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.google.com%2F&attachment[type]=100&link_metrics[source]=ShareStageExternal&link_metrics[domain]=www.google.com&link_metrics[base_domain]=google.com&link_metrics[title_len]=6&link_metrics[summary_len]=159&link_metrics[min_dimensions][0]=70&link_metrics[min_dimensions][1]=70&link_metrics[images_with_dimensions]=2&link_metrics[images_pending]=0&link_metrics[images_fetched]=0&link_metrics[image_dimensions][0]=269&link_metrics[image_dimensions][1]=95&link_metrics[images_selected]=2&link_metrics[images_considered]=2&link_metrics[images_cap]=3&link_metrics[images_type]=ranked&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=2&composer_metrics[images_loaded]=2&composer_metrics[images_shown]=2&composer_metrics[load_duration]=113&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1393564470&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8aqEAMCBCFUSt2u6aOGUGy6zECiq78hAKGgyiGGeqheCu&__req=b&ttstamp=265816655451197277&__rev=1140660", "");
                            // int length = ResponseWallPost.Length; 

                            #endregion

                            string postDataWalllpost1112 = string.Empty;                            
                            if (!(ResponseWallPost1.Length > 11000))
                            {
                                #region OldPostData
                                // postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
                                // ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112, ""); 
                                #endregion

                                int length2 = ResponseWallPost1.Length;
                                if (length2 > 11000 && ResponseWallPost1.Contains("jsmods") && ResponseWallPost1.Contains("XHPTemplate"))
                                {
                                    TotalNoOfWallPoster_Counter++;

                                    GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

                                    countWallPoster++;                                   
                                   
                                        try
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                
                                    try
                                    {
                                        if (IsUniqueMessagePosting)
                                        {
                                            Utils.InsertIntoDB(fbUser.username, friendId, "Video");
                                        }
                                        #region insertQuery
                                        //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                        //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else if (length2 > 11000 && ResponseWallPost1.Contains("jsmods") && ResponseWallPost1.Contains("XHPTemplate"))
                                {
                                    TotalNoOfWallPoster_Counter++;
                                    GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                    countWallPoster++;

                                      try
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                   
                                    try
                                    {
                                        if (IsUniqueMessagePosting)
                                        {
                                            Utils.InsertIntoDB(fbUser.username, friendId, "Video");
                                        }
                                        #region insertQuery
                                        //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                        //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost1);
                                    GlobusLogHelper.log.Info("Error : " + errorSummary + " not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                }
                            }
                            else
                            {
                                TotalNoOfWallPoster_Counter++;

                                GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

                                countWallPoster++;

                                  try
                                    {                                        
                                        int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                
                                try
                                {
                                    if (IsUniqueMessagePosting)
                                    {
                                        Utils.InsertIntoDB(fbUser.username, friendId, "Video");
                                    }
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
                                    #endregion
                                   
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            System.Threading.Thread.Sleep(4000);
                        }
                        else
                        {
                           // GlobusLogHelper.log.Info("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
                            //GlobusLogHelper.log.Debug("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);

                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    try
                    {
                        PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UserId);
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

        private void PostOnFriendWallUsingSpinMsg(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                string FirstName = string.Empty;
                string Name = string.Empty;
                string User = string.Empty;

                //if (HttpHelper.http.FinalRedirectUrl.Contains("https://"))
                {
                    postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

                    string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));

                    try
                    {
                        string GraphPagesource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + friendId));
                        string username = GraphPagesource.Substring(GraphPagesource.IndexOf("first_name\":")).Replace("first_name\":", string.Empty);
                        string[] UsernameArr = Regex.Split(username, "\"");
                        User = UsernameArr[1].Replace("\"", string.Empty).Replace(" ", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (!string.IsNullOrEmpty(User))
                    {
                        try
                        {
                            Name = pageSourceWallPost11.Substring(pageSourceWallPost11.IndexOf("<title>"), pageSourceWallPost11.IndexOf("</title>", pageSourceWallPost11.IndexOf("<title>")) - pageSourceWallPost11.IndexOf("<title>")).Replace("id&quot;:", string.Empty).Replace("<title>", string.Empty).Trim();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        Name = User;
                    }
                    if (Name.Contains(" "))
                    {
                        string[] flName = Regex.Split(Name, " ");
                        FirstName = flName[0];
                    }
                    else
                    {
                        FirstName = Name;

                    }
                    string smessage = string.Empty;
                    lstGreetMsgWallPoster.Add(wallmessage);
                    if (lstGreetMsgWallPoster.Count < 1)
                    {
                        smessage = "Hello";
                    }
                    else
                    {
                        smessage = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count - 1)];
                    }
                    wallmessage = smessage;

                    if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
                    {
                        GlobusLogHelper.log.Info(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);

                        string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);

                        string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

                        string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");

                        string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
                        string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111, "");
                        int length = ResponseWallPost.Length;

                        string postDataWalllpost1112 = string.Empty;
                        string ResponseWallPost2 = string.Empty;
                        if (!(length > 11000))
                        {
                            postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
                            ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112, "");

                            int length2 = ResponseWallPost2.Length;
                            if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;
                                if (IsUniqueMessagePosting)
                                {
                                    Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                                }
                                GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

                                countWallPoster++;

                              
                                    try
                                    {                                     

                                        int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                
                                try
                                {
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;
                                GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                countWallPoster++;                                
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                               
                                try
                                {
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                            }
                            if (ResponseWallPost2.Contains("Security Check Required") || ResponseWallPost2.Contains("A security check is required to proceed"))
                            {
                                GlobusLogHelper.log.Info("Security Check Required Account  :" + fbUser.username);
                                GlobusLogHelper.log.Debug("Security Check Required Account  :" + fbUser.username);

                            }
                        }
                        else
                        {
                            TotalNoOfWallPoster_Counter++;
                            GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                            countWallPoster++;
                            if (IsUniqueMessagePosting)
                            {
                                Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                            }
                                try
                                {
                                    int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                        
                            try
                            {
                                #region insertQuery
                                //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        System.Threading.Thread.Sleep(4000);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);

                        System.Threading.Thread.Sleep(1000);

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void PostOnFriendWallUsingGreetMsg(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                string FirstName = string.Empty;
                string Name = string.Empty;
                string User = string.Empty;

                lstGreetMsgWallPoster.Add(wallmessage);

               //if (HttpHelper.http.FinalRedirectUrl.Contains("https://"))
                {
                    postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

                    string pageSourceWallPost11 = string.Empty;
                    pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));

                    try
                    {
                        string GraphPagesource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + friendId));
                        string username = GraphPagesource.Substring(GraphPagesource.IndexOf("first_name\":")).Replace("first_name\":", string.Empty);
                        string[] UsernameArr = Regex.Split(username, "\"");
                        User = UsernameArr[1].Replace("\"", string.Empty).Replace(" ", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (string.IsNullOrEmpty(User))
                    {
                        try
                        {
                            Name = pageSourceWallPost11.Substring(pageSourceWallPost11.IndexOf("<title>"), pageSourceWallPost11.IndexOf("</title>", pageSourceWallPost11.IndexOf("<title>")) - pageSourceWallPost11.IndexOf("<title>")).Replace("id&quot;:", string.Empty).Replace("<title>", string.Empty).Trim();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        Name = User;
                    }
                    if (Name.Contains(" "))
                    {
                        string[] flName = Regex.Split(Name, " ");
                        FirstName = flName[0];
                    }
                    else
                    {
                        FirstName = Name;

                    }
                    string smessage = string.Empty;

                    if (lstGreetMsgWallPoster.Count < 1)
                    {
                        smessage = "Hello";
                    }
                    else
                    {
                        smessage = lstGreetMsgWallPoster[Utils.GenerateRandom(0, lstGreetMsgWallPoster.Count - 1)];
                    }

                  


                    string wallsmessage = lstWallMessageWallPoster[Utils.GenerateRandom(0, lstWallMessageWallPoster.Count - 1)];
                    if (wallmessage.Contains("<friend first name>"))
                    {
                        wallmessage = wallsmessage.Replace("<friend first name>", FirstName);
                        wallmessage = smessage + " " + wallmessage;

                     
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Wall Message : " + wallmessage + " Not Contain <friend first name>");
                        GlobusLogHelper.log.Debug("Wall Message : " + wallmessage + " Not Contain <friend first name>");

                    }

                    if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
                    {
                        GlobusLogHelper.log.Info(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug(countWallPoster.ToString() + " Posting on wall " + postUrl + " With Username : " + fbUser.username);

                        string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);

                        string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

                        string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");

                        string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
                        string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111, "");
                        int length = ResponseWallPost.Length;

                        string postDataWalllpost1112 = string.Empty;
                        string ResponseWallPost2 = string.Empty;
                        if (!(length > 11000))
                        {
                            postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
                            ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112, "");

                            int length2 = ResponseWallPost2.Length;

                            if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;
                                GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

                                countWallPoster++;
                              
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);

                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);

                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                
                                try
                                {
                                    if (IsUniqueMessagePosting)
                                    {
                                        Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                                    }
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
                            {
                                TotalNoOfWallPoster_Counter++;
                                GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                countWallPoster++;
                                
                                    try
                                    {
                                        int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);

                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                
                                try
                                {
                                    if (IsUniqueMessagePosting)
                                    {
                                        Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                                    }
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                            }
                            if (ResponseWallPost2.Contains("Security Check Required") || ResponseWallPost2.Contains("A security check is required to proceed"))
                            {
                                GlobusLogHelper.log.Info("Security Check Required Account  :" + fbUser.username);
                                GlobusLogHelper.log.Debug("Security Check Required Account  :" + fbUser.username);

                            }
                        }
                        else
                        {
                            TotalNoOfWallPoster_Counter++;

                            GlobusLogHelper.log.Info("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

                            countWallPoster++;                            
                                try
                                {
                                    int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);

                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With Username : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            
                            try
                            {
                                #region insertQuery
                                //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendId + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                #endregion
                                if (IsUniqueMessagePosting)
                                {
                                    Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        System.Threading.Thread.Sleep(4000);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
                        System.Threading.Thread.Sleep(1000);

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        public bool PostOnWallWithURLAndItsImage(ref FacebookUser fbUser, List<string> lstWallPostURLs, List<string> lstfriends)
        {
            try
            {                
                int CountPostWall = 1;
                int friendval = messageCountWallPoster;
                int friendCount = 0;
                string strWallPostedURL = string.Empty;

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                // if (useAllUrl)
                {
                    ArrayList randomNoList = Utils.RandomNumbers(lstWallPostURLs.Count - 1);

                    int msgIndex = 0;

                    if (lstfriends.Count() > friendval)
                    {
                        friendCount = friendval;
                    }
                    else
                    {
                        friendCount = lstfriends.Count();
                    }
                    ///Normal, 1 msg to all friends
                    if (UseOneMsgToAllFriendsWallPoster)
                    {
                        strWallPostedURL = lstWallPostURLs[(new Random().Next(0, lstWallPostURLs.Count - 1))];  //%20;
                    }
                    foreach (var lstfriendsitem in lstfriends)
                    {
                        if (IsUniqueMessagePosting)
                        {
                            if (Utils.CheckIfMessagePosted(fbUser.username, lstfriendsitem, "PicWithURL"))
                            {
                                continue;
                            } 
                        }
                        try
                        {

                            if (CountPostWall > friendCount)
                            {
                                return true;
                            }

                            #region selectQuery
                            // System.Data.DataSet ds = new DataSet();
                            try
                            {
                                //string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + lstfriendsitem + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
                                //ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            //if (ds.Tables[0].Rows.Count < 1)
                            {
                                ///For Random, might be Unique, might not be 
                            #endregion

                                if (UseRandomWallPoster)
                                {
                                    if (msgIndex < randomNoList.Count)
                                    {
                                        try
                                        {
                                            msgIndex = (int)randomNoList[msgIndex];
                                            strWallPostedURL = lstWallPostURLs[msgIndex];
                                            msgIndex++;
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }
                                    else if (lstWallPostURLs.Count > msgIndex)
                                    {
                                        strWallPostedURL = lstWallPostURLs[msgIndex];
                                        msgIndex++;
                                    }
                                    else
                                    {
                                        msgIndex = 0;
                                        randomNoList = Utils.RandomNumbers(lstWallPostURLs.Count - 1);
                                        strWallPostedURL = lstWallPostURLs[msgIndex];
                                        msgIndex++;
                                    }
                                }                               
                                else if (UseUniqueMsgToAllFriendsWallPoster)
                                {
                                    if (lstWallPostURLs.Count > CountPostWall - 1)
                                    {
                                        strWallPostedURL = lstWallPostURLs[CountPostWall - 1];
                                    }
                                    else
                                    {
                                        strWallPostedURL = lstWallPostURLs[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                    }
                                }
                                string composer_session_id = "";
                                string fb_dtsg = "";
                                string xhpc_composerid = "";
                                string xhpc_targetid = "";
                                string xhpc_context = "";
                                string xhpc_fbx = "";
                                string xhpc_timeline = "";
                                string xhpc_ismeta = "";
                                string xhpc_message_text = string.Empty;
                                string xhpc_message = string.Empty;                             
                                string UsreId = "";                                
                                string friendURL = "";
                                if (CountPostWall == 1)
                                {
                                    friendURL = FBGlobals.Instance.fbProfileUrl + lstfriends[lstfriends.Count - 1] + "&sk=wall";
                                }
                                else
                                {
                                    friendURL = FBGlobals.Instance.fbProfileUrl + lstfriendsitem + "&sk=wall";
                                }
                                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(friendURL));
                                if (strPageSource.Contains("xhpc_composerid") && strPageSource.Contains("xhpc_targetid") && strPageSource.Contains("xhpc_context") && strPageSource.Contains("xhpc_fbx"))
                                {                                    
                                    UsreId = GlobusHttpHelper.Get_UserID(strPageSource);

                                    fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

                                    xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                                    xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                                    xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                                    xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                                    xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                                    xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");
                                    

                                    if (string.IsNullOrEmpty(UsreId))
                                    {
                                        UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                                    }

                                    string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxSatusIsAugmentationUrl + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));
                                    if (string.IsNullOrEmpty(composer_session_idSource))
                                    {
                                        composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxSatusIsAugmentationUrl + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));
                                    }
                                    if (composer_session_idSource.Contains("composer_session_id"))
                                    {
                                        composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

                                    }
                                    string strImageValue = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxScraperUrl + Uri.EscapeDataString(strWallPostedURL) + "&alt_scrape_url=" + Uri.EscapeDataString(strWallPostedURL) + "&targetid=" + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));   //http://www.facebook.com/ajax/metacomposer/attachment/link/scraper.php?scrape_url=http%253A%252F%252Fwww.google.co.in%252F&alt_scrape_url=http%253A%252F%252Fwww.google.co.in%252F&targetid=100003798185175&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=100003798185175&__a=1
                                    if (string.IsNullOrEmpty(strImageValue))
                                    {
                                        strImageValue = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxScraperUrl + Uri.EscapeDataString(strWallPostedURL) + "&alt_scrape_url=" + Uri.EscapeDataString(strWallPostedURL) + "&targetid=" + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));   //http://www.facebook.com/ajax/metacomposer/attachment/link/scraper.php?scrape_url=http%253A%252F%252Fwww.google.co.in%252F&alt_scrape_url=http%253A%252F%252Fwww.google.co.in%252F&targetid=100003798185175&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=100003798185175&__a=1

                                    }
                                    Thread.Sleep(3000);
                                    Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
                                    if (strImageValue.Contains("name=") && strImageValue.Contains("value="))
                                    {
                                        try
                                        {
                                            string[] strNameValue = Regex.Split(strImageValue, "name=");
                                            foreach (var strNameValueitem in strNameValue)
                                            {
                                                try
                                                {
                                                    if (strNameValueitem.Contains("value="))
                                                    {
                                                        string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
                                                        if (strSplit.Contains("value="))
                                                        {
                                                            string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                            string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                            strValue = (strValue);  //Uri.EscapeDataString
                                                            dicNameValue.Add(strName, strValue);
                                                        }
                                                        else
                                                        {
                                                            string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                            string strValue = "0";
                                                            strValue = (strValue);
                                                            dicNameValue.Add(strName, strValue);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }
                                            string strImageUrl = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetSafeImageUrl));
                                            if (string.IsNullOrEmpty(strImageUrl))
                                            {
                                                strImageUrl = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetSafeImageUrl));

                                            }

                                            string partPostData = string.Empty;
                                            foreach (var dicNameValueitem in dicNameValue)
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

                                            string strPostData = ("fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=" + xhpc_context + "&xhpc_fbx=" + xhpc_fbx + "&xhpc_timeline=" + xhpc_timeline + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&" + partPostData + "uithumbpager_width=128&uithumbpager_height=128&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=" + composer_session_id + "&is_explicit_place=&composertags_city=&disable_location_sharing=false&audience[0][value]=80&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1&phstamp=16581671021075776692083");//fb_dtsg=AQCfk9LE&xhpc_composerid=u2v49r_6&xhpc_targetid=100003798185175&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=http%3A%2F%2Fwww.google.co.in%2F&xhpc_message=http%3A%2F%2Fwww.google.co.in%2F&aktion=post&app_id=2309869772&UIThumbPager_Input=0&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][http-equiv]=content-type&attachment[params][metaTagMap][1][content]=text%2Fhtml%3B%20charset%3DISO-8859-1&attachment[params][metaTagMap][2][itemprop]=image&attachment[params][metaTagMap][2][content]=%2Fimages%2Fgoogle_favicon_128.png&attachment[params][medium]=106&attachment[params][urlInfo][canonical]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][urlInfo][final]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][favicon]=http%3A%2F%2Fwww.google.co.in%2Ffavicon.ico&attachment[params][title]=Google&attachment[params][fragment_title]=&attachment[params][external_author]=&attachment[params][summary]=&attachment[params][url]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][error]=1&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Google&attachment[params][og_info][guesses][2][0]=og%3Aimage&attachment[params][og_info][guesses][2][1]=http%3A%2F%2Fwww.google.co.in%2Fimages%2Fgoogle_favicon_128.png&attachment[params][responseCode]=200&attachment[params][images][0]=http%3A%2F%2Fwww.google.co.in%2Fimages%2Fgoogle_favicon_128.png&attachment[params][cache_hit]=1&attachment[type]=100&uithumbpager_width=128&uithumbpager_height=128&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=1341205506&is_explicit_place=&composertags_city=&disable_location_sharing=false&audience[0][value]=80&nctr[_mod]=pagelet_composer&__user=100003798185175&__a=1&phstamp=16581671021075776692083
                                            string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxProfileComposerUrl), strPostData, "");
                                            if (string.IsNullOrEmpty(strResponse))
                                            {
                                                strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxProfileComposerUrl), strPostData, "");

                                            }
                                            if (!strResponse.Contains("error"))
                                            {
                                                try
                                                {
                                                    TotalNoOfWallPoster_Counter++;
                                                    if (IsUniqueMessagePosting)
                                                    {
                                                        Utils.InsertIntoDB(fbUser.username, lstfriendsitem, "PicWithURL"); 
                                                    }
                                                    
                                                    GlobusLogHelper.log.Info(CountPostWall + " Wall Posted With Image URL : " + strWallPostedURL + "on the friend URL : " + friendURL + " With User Name : " + fbUser.username);
                                                    GlobusLogHelper.log.Debug(CountPostWall + " Wall Posted With Image URL : " + strWallPostedURL + "on the friend URL : " + friendURL + " With User Name : " + fbUser.username);

                                                    CountPostWall++;

                                                 
                                                        try
                                                        {
                                                            int delayInSeconds = Utils.GenerateRandom(2 * 1000, 5 * 1000);

                                                            GlobusLogHelper.log.Info("Delaying " + delayInSeconds / 1000 + " In Seconds With Username : " + fbUser.username);
                                                            GlobusLogHelper.log.Debug("Delaying " + delayInSeconds / 1000 + " In Seconds With Username : " + fbUser.username);

                                                            Thread.Sleep(delayInSeconds);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }

                                                    #region commented 
                                                    //if (!string.IsNullOrEmpty(_ExprotFilePath))
                                                    //{
                                                    //    string CSVHeader = "User_Name" + "," + "Friend URL" + ", " + "Message";
                                                    //    string CSV_Content = Username + "," + friendURL + ", " + strWallPostedURL;
                                                    //    FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, _ExprotFilePath);
                                                    //    Log("Export Wall Poster Data!");
                                                    //}
                                                    //try
                                                    //{
                                                    //    string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + lstfriendsitem + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                                    //    BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
                                                    //}
                                                    //catch { } 
                                                    #endregion
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                                // return true;
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
            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        public void PostOnWallWithAllURLAndItsImage(ref FacebookUser fbUser, List<string> lstWallPostURLs, List<string> lstfriends)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                foreach (string wallitem in lstWallPostURLs)
                {
                    try
                    {
                      
                        int CountPostWall = 1;
                        int friendval = messageCountWallPoster;                      
                        string strWallPostedURL = string.Empty;
                        string composer_session_id = "";
                        string fb_dtsg = "";
                        string xhpc_composerid = "";
                        string xhpc_targetid = "";
                        string xhpc_context = "";
                        string xhpc_fbx = "";
                        string xhpc_timeline = "";
                        string xhpc_ismeta = "";
                        string xhpc_message_text = string.Empty;
                        string xhpc_message = string.Empty;                            
                        string UsreId = "";                        
                        string friendURL = "";
                        string Friend = string.Empty;
                        if (CountPostWall == 1)
                        {
                            if (IsUniqueMessagePosting)
                            {
                                while (true)
                                {
                                    Friend = lstfriends[Utils.GenerateRandom(0, lstfriends.Count)];
                                    if (!Utils.CheckIfMessagePosted(fbUser.username, Friend, "PicWithURL"))
                                    {
                                        break;
                                    }
                                } 
                            }
                            friendURL = FBGlobals.Instance.fbProfileUrl + Friend + "&sk=wall";
                        }
                        else
                        {
                            // friendURL = "http://www.facebook.com/profile.php?id=" + lstfriendsitem + "&sk=wall";
                        }
                        string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(friendURL));
                        if (strPageSource.Contains("xhpc_composerid") && strPageSource.Contains("xhpc_targetid") && strPageSource.Contains("xhpc_context") && strPageSource.Contains("xhpc_fbx"))
                        {
                            UsreId = GlobusHttpHelper.Get_UserID(strPageSource);

                            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

                            xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
                            xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
                            xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
                            xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
                            xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
                            xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

                            if (string.IsNullOrEmpty(UsreId))
                            {
                                UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                            }

                            string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxSatusIsAugmentationUrl + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));
                            if (string.IsNullOrEmpty(composer_session_idSource))
                            {
                                composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxSatusIsAugmentationUrl + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));
                            }

                            if (composer_session_idSource.Contains("composer_session_id"))
                            {
                                composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

                            }
                            string strImageValue = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxScraperUrl + Uri.EscapeDataString(wallitem) + "&alt_scrape_url=" + Uri.EscapeDataString(wallitem) + "&targetid=" + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));
                            if (string.IsNullOrEmpty(strImageValue))
                            {

                                strImageValue = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.WallPosterGetAjaxScraperUrl + Uri.EscapeDataString(wallitem) + "&alt_scrape_url=" + Uri.EscapeDataString(wallitem) + "&targetid=" + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"));

                            }
                            Thread.Sleep(3000);
                            Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
                            if (strImageValue.Contains("name=") && strImageValue.Contains("value="))
                            {
                                try
                                {
                                    string[] strNameValue = Regex.Split(strImageValue, "name=");
                                    foreach (var strNameValueitem in strNameValue)
                                    {
                                        try
                                        {
                                            if (strNameValueitem.Contains("value="))
                                            {
                                                string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
                                                if (strSplit.Contains("value="))
                                                {
                                                    string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                    string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                   
                                                    dicNameValue.Add(strName, strValue);
                                                }
                                                else
                                                {
                                                    string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
                                                    string strValue = "0";
                                                   
                                                    dicNameValue.Add(strName, strValue);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    string partPostData = string.Empty;
                                    foreach (var dicNameValueitem in dicNameValue)
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

                                    string strPostData = ("fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=" + xhpc_context + "&xhpc_fbx=" + xhpc_fbx + "&xhpc_timeline=" + xhpc_timeline + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&" + partPostData + "uithumbpager_width=128&uithumbpager_height=128&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=" + composer_session_id + "&is_explicit_place=&composertags_city=&disable_location_sharing=false&audience[0][value]=80&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1&phstamp=16581671021075776692083");//fb_dtsg=AQCfk9LE&xhpc_composerid=u2v49r_6&xhpc_targetid=100003798185175&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=http%3A%2F%2Fwww.google.co.in%2F&xhpc_message=http%3A%2F%2Fwww.google.co.in%2F&aktion=post&app_id=2309869772&UIThumbPager_Input=0&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][http-equiv]=content-type&attachment[params][metaTagMap][1][content]=text%2Fhtml%3B%20charset%3DISO-8859-1&attachment[params][metaTagMap][2][itemprop]=image&attachment[params][metaTagMap][2][content]=%2Fimages%2Fgoogle_favicon_128.png&attachment[params][medium]=106&attachment[params][urlInfo][canonical]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][urlInfo][final]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][favicon]=http%3A%2F%2Fwww.google.co.in%2Ffavicon.ico&attachment[params][title]=Google&attachment[params][fragment_title]=&attachment[params][external_author]=&attachment[params][summary]=&attachment[params][url]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][error]=1&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=http%3A%2F%2Fwww.google.co.in%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Google&attachment[params][og_info][guesses][2][0]=og%3Aimage&attachment[params][og_info][guesses][2][1]=http%3A%2F%2Fwww.google.co.in%2Fimages%2Fgoogle_favicon_128.png&attachment[params][responseCode]=200&attachment[params][images][0]=http%3A%2F%2Fwww.google.co.in%2Fimages%2Fgoogle_favicon_128.png&attachment[params][cache_hit]=1&attachment[type]=100&uithumbpager_width=128&uithumbpager_height=128&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=1341205506&is_explicit_place=&composertags_city=&disable_location_sharing=false&audience[0][value]=80&nctr[_mod]=pagelet_composer&__user=100003798185175&__a=1&phstamp=16581671021075776692083
                                    string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxProfileComposerUrl), strPostData, "");
                                    if (string.IsNullOrEmpty(strResponse))
                                    {
                                        strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxProfileComposerUrl), strPostData, "");

                                    }
                                    if (!strResponse.Contains("error"))
                                    {
                                        try
                                        {
                                            TotalNoOfWallPoster_Counter++;
                                            if (IsUniqueMessagePosting)
                                            {
                                                Utils.InsertIntoDB(fbUser.username, Friend, "PicWithURL"); 
                                            }
                                            GlobusLogHelper.log.Info(CountPostWall + " Wall Posted With Image URL : " + wallitem + "on the Own URL : " + friendURL + " With User Name : " + fbUser.username);
                                            GlobusLogHelper.log.Debug (CountPostWall + " Wall Posted With Image URL : " + wallitem + "on the Own URL : " + friendURL + " With User Name : " + fbUser.username);

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
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

        public void StarPostPicOnWall()
        {           

            try
            {
                countThreadControllerWallPoster = 0;
                int numberOfAccountPatch = 25;

                if (NoOfThreadsWallPoster > 0)
                {
                    numberOfAccountPatch = NoOfThreadsPostPicOnWall;
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
                                lock (lockrThreadControllerPostPicOnWall)
                                {
                                    try
                                    {
                                        if (countThreadControllerWallPoster >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerPostPicOnWall);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsPostPicOnWall);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerPostPicOnWall++;
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

        public void StartMultiThreadsPostPicOnWall(object parameters)
         {
            try
            {
                if (!isStopPostPicOnWall)
                {
                    try
                    {
                        lstThreadsPostPicOnWall.Add(Thread.CurrentThread);
                        lstThreadsPostPicOnWall.Distinct();
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
                                // Call StartActionMessageReply
                                StartActionPostPicOnWall(ref objFacebookUser);
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
                   // if (!isStopPostPicOnWall)
                    {
                        lock (lockrThreadControllerPostPicOnWall)
                        {
                            countThreadControllerPostPicOnWall--;
                            Monitor.Pulse(lockrThreadControllerPostPicOnWall);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionPostPicOnWall(ref FacebookUser fbUser)
        {
            try
            {
                if (StartProcessUsingPostPicOnWall=="Post Pic On Wall")
                {
                    PostPictureOnWall(ref fbUser);
                }
                else if (StartProcessUsingPostPicOnWall=="Share Video ")
                {
                    ShareVideoOnWall(ref fbUser);
                }
                else if (StartProcessUsingPostPicOnWall == "Share Image")
                {
                    ShareImageOnWall(ref fbUser);
                }
                else if (StartProcessUsingPostPicOnWall == "Spam Video")
                {
                    SpamVideoOnWall(ref fbUser);
                }
                else if (StartProcessUsingPostPicOnWall == "Share Fan Page Post ")
                {
                    shareFanpagePostOnWall(ref fbUser);

                    GlobusLogHelper.log.Debug("Process Completed With : " + fbUser.username);
                    GlobusLogHelper.log.Info("Process Completed With : " + fbUser.username);
                }
              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        //add new module share post

        public void shareFanpagePostOnWall(ref FacebookUser fbUser)
        {
            GlobusLogHelper.log.Debug("Sharing post from Fan Page.");
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {

                string Username = fbUser.username;

                foreach (string item in lstWallPostShareLoadTargedUrls)
                {
                    string FanPagePageSource = string.Empty;


                    FanPagePageSource = HttpHelper.getHtmlfromUrl(new Uri(item));


                    ///
                    GetFirstShareLink(FanPagePageSource, ref fbUser, item);
                    
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
             
        }

        private void GetFirstShareLink(string pageSource, ref FacebookUser fbUser,string fanPageUrl)
        {
            try
            {
               
                string strFanPageURL = string.Empty;
                string shareLink = string.Empty;

               GlobusLogHelper.log.Debug("Starting To Find Share Link With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageUrl );
               GlobusLogHelper.log.Info("Starting To Find Share Link With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageUrl);

                if (pageSource.Contains("Share"))
                {
                    try
                    {
                        string timelineUnitContainer = Utils.GetDataWithTagValueByTagAndAttributeName(pageSource, "div", "timelineUnitContainer");

                        string[] arrTimelineUnitContainer = System.Text.RegularExpressions.Regex.Split(pageSource, "timelineUnitContainer");

                        foreach (string item in arrTimelineUnitContainer)
                        {
                            try
                            {

                                if (item.Contains("share_action_link"))
                                {
                                    try
                                    {
                                        string share_action_link = Utils.GetDataWithTagValueByTagAndAttributeName(pageSource, "a", "share_action_link");

                                        string[] arrshare_action_link = System.Text.RegularExpressions.Regex.Split(item, "share_action_link");

                                        foreach (string item1 in arrshare_action_link)
                                        {
                                            try
                                            {

                                                if (item1.Contains("href=\"/ajax/sharer/"))
                                                {
                                                    try
                                                    {
                                                        shareLink = item1.Substring(item1.IndexOf("href="), (item1.IndexOf(" ", item1.IndexOf("href=")) - item1.IndexOf("href="))).Replace("href=", string.Empty).Replace("\"", string.Empty).Trim();

                                                        if (!shareLink.Contains(FBGlobals.Instance.fbhomeurl))
                                                        {
                                                            shareLink = "https://www.facebook.com" + shareLink;
                                                        }


                                                        GlobusLogHelper.log.Debug("Found Share Link :" + shareLink + " With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageUrl);
                                                        GlobusLogHelper.log.Info("Found Share Link :" + shareLink + " With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageUrl);
                                                        try
                                                        {
                                                            SetPostData(ref fbUser, shareLink, fanPageUrl);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }

                                                        break;
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

                                        break;
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
                        if (string.IsNullOrEmpty(shareLink))
                        {
                         
                            GlobusLogHelper.log.Debug("Couldn't Find Share Link With Fan Page URL : " + fanPageUrl + " With User Name : " + fbUser.username);
                            GlobusLogHelper.log.Info("Couldn't Find Share Link With Fan Page URL : " + fanPageUrl + " With User Name : " + fbUser.username);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                     GlobusLogHelper.log.Debug("There Is No Option For Share !");
                     GlobusLogHelper.log.Info("There Is No Option For Share !");
                } 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void SetPostData(ref FacebookUser fbUser, string shareLinkURL, string fanPageURL)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
           
            try
            {
                string strShareLinkURL = string.Empty;
                string fb_dtsg = string.Empty;
                string message_text = string.Empty;
                string attachment0 = string.Empty;
                string attachment1 = string.Empty;
                string attachmentType = string.Empty;
                string appid = string.Empty;
                string __user = string.Empty;


                strShareLinkURL = shareLinkURL;



                string pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com"));

                if (string.IsNullOrEmpty(pageSource))
                {
                    try
                    {
                        Thread.Sleep(1000);
                        pageSource = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com"));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                if (!string.IsNullOrEmpty(pageSource))
                {
                   // 
                    GlobusLogHelper.log.Debug("Starting Post Share With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageURL);
                    GlobusLogHelper.log.Info("Starting Post Share With User Name :  " + fbUser.username + " Fan Page URL : " + fanPageURL);
                    __user = GlobusHttpHelper.GetParamValue(pageSource, "user");//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
                    if (string.IsNullOrEmpty(__user))
                    {
                        __user = GlobusHttpHelper.ParseJson(pageSource, "user");
                    }


                    try
                    {
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        message_text = lstMessageCollectionPostPicOnWall[new Random().Next(0, lstMessageCollectionPostPicOnWall.Count)];
                    }
                    catch { }

                    string[] arrEqual = System.Text.RegularExpressions.Regex.Split(strShareLinkURL, "=");

                    // for (int i = 0; i < arrEqual.Length; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(arrEqual[1]))
                            {
                                try
                                {
                                    string[] arrNumber = System.Text.RegularExpressions.Regex.Split(arrEqual[1], "[^0-9]");

                                    foreach (string item in arrNumber)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item))
                                            {
                                                attachmentType = item;
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

                            if (!string.IsNullOrEmpty(arrEqual[2]))
                            {
                                try
                                {
                                    string[] arrNumber = System.Text.RegularExpressions.Regex.Split(arrEqual[2], "[^0-9]");

                                    foreach (string item in arrNumber)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item))
                                            {
                                                appid = item;
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

                            if (!string.IsNullOrEmpty(arrEqual[3]))
                            {
                                try
                                {
                                    string[] arrNumber = System.Text.RegularExpressions.Regex.Split(arrEqual[3], "[^0-9]");

                                    foreach (string item in arrNumber)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item))
                                            {
                                                attachment0 = item;
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

                            if (!string.IsNullOrEmpty(arrEqual[4]))
                            {
                                try
                                {
                                    string[] arrNumber = System.Text.RegularExpressions.Regex.Split(arrEqual[4], "[^0-9]");

                                    foreach (string item in arrNumber)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item))
                                            {
                                                attachment1 = item;
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
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    string getRequest = HttpHelper.getHtmlfromUrl(new Uri(strShareLinkURL));

                    //Post data
                    //fb_dtsg=AQCGeTrr&ad_params=&friendTarget=&groupTarget=&mode=self&message_text=Heyyyyyyyyyyy&message=Heyyyyyyyyyyy&attachment[params][0]=23314931016&attachment[params][1]=10151146920226017&attachment[type]=22&src=i&appid=25554907596&parent_fbid=&ogid=&audience[0][value]=80&__user=100001530948154&__a=1&phstamp=16581677110184114114301

                    string postData = "fb_dtsg=" + fb_dtsg + "&ad_params=&friendTarget=&groupTarget=&mode=self&message_text=" + message_text + "&message=" + message_text + "&attachment[params][0]=" + attachment0 + "&attachment[params][1]=" + attachment1 + "&attachment[type]=" + attachmentType + "&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&__user=" + __user + "&__a=1&phstamp=16581677110184114114301";
                    string PostUrl = "https://www.facebook.com/ajax/sharer/submit/?__av="+__user;
                //    String response = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/sharer/submit/"), postData);
                    String response = HttpHelper.postFormData(new Uri(PostUrl), postData);

                    if (!response.Contains("error") || response.Contains("DialogHideOnSuccess"))
                    {
                        try
                        {
                            string _ExprotFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\FaceDominatorFbAccount\\PostShareDetails.csv";
                          
                            GlobusLogHelper.log.Debug("Post Shared Successfully With User Name : " + fbUser.username + " Fan Page URL : " + fanPageURL);
                            GlobusLogHelper.log.Info("Post Shared Successfully With User Name : " + fbUser.username + " Fan Page URL : " + fanPageURL);

                            string CSVHeader = "UserName" + ", " + "Fan Page URL";
                            string CSV_Content =fbUser.username + "," + fanPageURL;

                        //    FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, _ExprotFilePath);

                            // Delay 

                         // int delay = new Random().Next(MinDelay * 60, MaxDelay * 60);
                          int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                          GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                          GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                          Thread.Sleep(delayInSeconds);

                       
                          //  GlobusLogHelper.log.Debug("Delay : " + (delay / (60 * 1000)).ToString() + " Minutes With User Name : " + fbUser.username);

                         //   Thread.Sleep(delay);      
                  
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                       GlobusLogHelper.log.Debug("Post Couldn't Share Successfully With User Name : " + fbUser.username + " Fan Page URL : " + fanPageURL);
                       GlobusLogHelper.log.Info("Post Couldn't Share Successfully With User Name : " + fbUser.username + " Fan Page URL : " + fanPageURL);
                    }
                }
                else
                {
                   GlobusLogHelper.log.Debug("Home Page Source Is Null !");
                   GlobusLogHelper.log.Info("Home Page Source Is Null !");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void SpamVideoOnWall(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Spam Video On  Wall With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Spam Video On  Wall With Username : " + fbUser.username);

                    if (!IsPostAllPicPostPicOnWall)
                    {
                        bool ReturnPicstatus = false;
                        int intProxyPort = 80;
                        string xhpc_composerid = string.Empty;
                        string xhpc_targetid = string.Empty;
                        string message_text = string.Empty;
                        string message = string.Empty;
                        string imagePath = string.Empty;
                        Regex IdCheck = new Regex("^[0-9]*$");


                        try
                        {
                            string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                            foreach (var LoadTargedUrls_item in lstWallPostShareLoadTargedUrls)
                            {
                                try
                                {
                                    if (!LoadTargedUrls_item.Contains("photo.php?"))
                                    {
                                        if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                                        {
                                            intProxyPort = int.Parse(fbUser.proxyport);
                                        }


                                        string __user = "";
                                        string fb_dtsg = "";

                                        string pgSrc_FanPageSearch = PageSrcHome;

                                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                        if (string.IsNullOrEmpty(__user))
                                        {
                                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                        }

                                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                                        {
                                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                                            return;
                                        }

                                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                        if (string.IsNullOrEmpty(fb_dtsg))
                                        {
                                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                        }
                                        try
                                        {
                                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        string PageSrcTageted = HttpHelper.getHtmlfromUrl(new Uri(LoadTargedUrls_item));
                                        string hovercardPhpId = string.Empty;
                                        string media_info = string.Empty;
                                        string appid = string.Empty;
                                        string attachment = string.Empty;
                                        string pubcontent_params = string.Empty;
                                        string pageTarget = string.Empty;

                                        string hideable_token = string.Empty;
                                        string story_permalink_token = string.Empty;
                                        string initial_action_name = string.Empty;
                                        string options_button_id = string.Empty;
                                        string ft_tn = string.Empty;
                                        string ft_qid = string.Empty;
                                        string ft_mf_story_key = string.Empty;


                                        hideable_token = Utils.getBetween(PageSrcTageted, "hideable_token=", "&");
                                        story_permalink_token = Utils.getBetween(PageSrcTageted, "story_permalink_token=", "&").Replace("\\u00253A", "%3A");
                                        options_button_id = Utils.getBetween(PageSrcTageted, "options_button_id=", "&");

                                        //open start_dialog
                                        try
                                        {
                                            string PostUrl = "https://www.facebook.com/ajax/afro/start_dialog";
                                            string SharePostData = "rel=dialog-post&story_location=feed&answer=spam&hideable_token=" + hideable_token + "&story_permalink_token=" + story_permalink_token + "&initial_action_name=MARK_SPAM&options_button_id=" + options_button_id + "&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzElw&__req=e&fb_dtsg=" + fb_dtsg + "&ttstamp=2658171112708153105865052118&__rev=1288570&ft[tn]=WWV&ft[fbfeed_location]=5";

                                            string res = string.Empty;
                                            string res1 = string.Empty;
                                            string res2 = string.Empty;
                                            try
                                            {
                                                res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            //Continue 
                                            string time_flow_started = Utils.getBetween(res, "time_flow_started&quot", "&quot").Replace(":", "").Replace(";", "").Replace(",", "").Trim();
                                            string PostUrl1 = "https://www.facebook.com/ajax/afro/continue_dialog";
                                            string SharePostData1 = "fb_dtsg=" + fb_dtsg + "_&answer=spammy&context=%7B%22initial_action_name%22%3A%22MARK_SPAM%22%2C%22time_flow_started%22%3A" + time_flow_started + "%2C%22breadcrumbs%22%3A[%22spam%22]%2C%22story_location%22%3A%22feed%22%2C%22tracking%22%3Anull%2C%22hideable_token%22%3A%22" + hideable_token + "%22%2C%22story_permalink_token%22%3A%22S%3A_I250826161666390%3A659455004136835%22%2C%22story_graphql_token%22%3Anull%7D&__user=" + __user + "&__a=1&__dyn=7n8anEAMCBynzpQ9UoGya4Cq74qbx2mbAKGiyGGEZ9LO7xCm4Vp_AyoSnw&__req=y&ttstamp=265817072811196965981047095&__rev=1288570";
                                            try
                                            {
                                                res1 = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            //third time submitButton post data 


                                            string PostUrl2 = "https://www.facebook.com/ajax/afro/hide_story?rel=dialog-post&story_location=feed&initial_action_name=MARK_SPAM&story_permalink_token=" + story_permalink_token + "&hideable_token=" + hideable_token + "&options_button_id=" + options_button_id;

                                            string SharePostData2 = "__user=" + __user + "&__a=1&__dyn=7n8anEAMCBynzpQ9UoGya4Cq74qbx2mbAKGiyGGEZ9LO7xCm4Vp_AyoSnw&__req=z&fb_dtsg=" + fb_dtsg + "_&ttstamp=265817072811196965981047095&__rev=1288570&ft[tn]=WWV&ft[qid]=6024308654013791799&ft[mf_story_key]=5518012120748252564&ft[fbfeed_location]=1&ft[insertion_position]=5";
                                            try
                                            {
                                                res2 = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }



                                            if (res.Contains("DialogHideOnSuccess"))
                                            {
                                                GlobusLogHelper.log.Info("Spam Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Spam Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Spam Video Not Share WIth UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Spam Video Not Share WIth UserName : " + fbUser.username);
                                            }

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
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
                                    else
                                    {

                                        if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                                        {
                                            intProxyPort = int.Parse(fbUser.proxyport);
                                        }


                                        string __user = "";
                                        string fb_dtsg = "";

                                        string pgSrc_FanPageSearch = PageSrcHome;

                                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                        if (string.IsNullOrEmpty(__user))
                                        {
                                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                        }

                                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                                        {
                                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                                            return;
                                        }

                                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                        if (string.IsNullOrEmpty(fb_dtsg))
                                        {
                                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                        }
                                        try
                                        {
                                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        string PageSrcTageted = HttpHelper.getHtmlfromUrl(new Uri(LoadTargedUrls_item));

                                        string hovercardPhpId = string.Empty;
                                        string media_info = string.Empty;
                                        string appid = string.Empty;
                                        string attachment = string.Empty;
                                        string pubcontent_params = string.Empty;
                                        string pageTarget = string.Empty;
                                        string res=string.Empty;
                                        string cid=string.Empty;
                                        string rid=string.Empty;
                                        string media_pos=string.Empty;
                                        string time_flow_started = string.Empty;
                                        string pp = string.Empty;                                 



                                        //open start_dialog
                                        try
                                        {
                                          
                                            cid=Utils.getBetween(PageSrcTageted,"cid=","&");
                                            rid=Utils.getBetween(PageSrcTageted,"rid=","&");
                                            media_pos=Utils.getBetween(PageSrcTageted,"media_pos=","&");
                                         
                                            string PageSource =HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/report/social.php?content_type=13&cid="+cid+"&rid="+rid+"&media_pos="+media_pos+"&cs_ver=0&__asyncDialog=1&__user="+__user+"&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzESu&__req=c&__rev=1288570"));
                                            pp = Utils.getBetween(PageSource, "pp\\\" value=\\\"", ">").Replace("&quot;", "\"");
                                            pp = Utils.getBetween(pp, "&#123;", "&");
                                           //===
                                            pp = Uri.EscapeDataString(pp);

                                            time_flow_started = Utils.getBetween(PageSource, "time_flow_started&quot", "&quot").Replace(":", "").Replace(";", "").Replace(",", "").Trim();
                                            string PostUrl = "https://www.facebook.com/ajax/report/social.php";
                                            string PostData = "fb_dtsg=" + fb_dtsg + "&report_type=9&pp=%7B%22" + pp + "%7D&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzESu&__req=d&ttstamp=265816981107101102451027311070&__rev=1288570";

                                            res = HttpHelper.postFormData(new Uri(PostUrl), PostData);

                                            string tooltrip = Utils.getBetween(PageSrcTageted, "data-tooltip-uri=\"", "\" class=");
                                            string PostUrl1 = "https://www.facebook.com" + tooltrip;

                                          string FinalPostData="__user="+__user+"&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzESu&__req=e&fb_dtsg="+fb_dtsg+"&ttstamp=265816981107101102451027311070&__rev=1288570";
                                          res = HttpHelper.postFormData(new Uri(PostUrl1), FinalPostData);
                                            if (res.Contains("bootloadable"))
                                            {
                                                GlobusLogHelper.log.Info("Spam Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Spam Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Spam Video Not Share WIth UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Spam Video Not Share WIth UserName : " + fbUser.username);
                                            }

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
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
                    }                  
              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Spam Video With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Spam Video With Username : " + fbUser.username);
        }

        public void ShareVideoOnWall(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Share Video On  Wall With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Share Video On  Wall With Username : " + fbUser.username);

                int CheckContinue = 0;
                if (chkCountinueProcessContinueShareProcess)
                {
                    CheckContinue = 1;
                }

                while (true)
                {                  

                    if (!IsPostAllPicPostPicOnWall)
                    {
                        bool ReturnPicstatus = false;
                        int intProxyPort = 80;
                        string xhpc_composerid = string.Empty;
                        string xhpc_targetid = string.Empty;
                        string message_text = string.Empty;
                        string message = string.Empty;
                        string imagePath = string.Empty;
                        Regex IdCheck = new Regex("^[0-9]*$");

                        try
                        {
                            string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                            foreach (var LoadTargedUrls_item in lstWallPostShareLoadTargedUrls)
                            {                                
                                try
                                {

                                    if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                                    {
                                        intProxyPort = int.Parse(fbUser.proxyport);
                                    }


                                    string __user = "";
                                    string fb_dtsg = "";

                                    string pgSrc_FanPageSearch = PageSrcHome;

                                    __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                    if (string.IsNullOrEmpty(__user))
                                    {
                                        __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                    }

                                    if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                                    {
                                        GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                                        return;
                                    }

                                    fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                    if (string.IsNullOrEmpty(fb_dtsg))
                                    {
                                        fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                    }
                                    try
                                    {
                                        xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    string PageSrcTageted = HttpHelper.getHtmlfromUrl(new Uri(LoadTargedUrls_item));
                                    string hovercardPhpId = string.Empty;
                                    string media_info = string.Empty;
                                    string appid = string.Empty;
                                    string attachment = string.Empty;
                                    string pubcontent_params = string.Empty;
                                    string pageTarget = string.Empty;

                                    try
                                    {
                                        appid = Utils.getBetween(PageSrcTageted, "appid=", "&");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    try
                                    {
                                        hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photo.php?v=", "&set=");

                                        if (string.IsNullOrEmpty(hovercardPhpId))
                                        {
                                            hovercardPhpId = Utils.getBetween(PageSrcTageted, "page.php?id=", "\">");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (string.IsNullOrEmpty(hovercardPhpId))
                                        {
                                            hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photos/a.", "."); ;
                                        }
                                        media_info = Utils.getBetween(PageSrcTageted, "media_info=", ">").Replace("\"", "");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    if (string.IsNullOrEmpty(media_info))
                                    {
                                        media_info = Utils.getBetween(PageSrcTageted, "target_fbid", "target_profile_id").Replace("\"", "").Replace("&quot;", "").Replace("&", "").Replace(":", "").Replace(",", "");
                                    }
                                    string Fid = Utils.getBetween(PageSrcTageted, "php%3Fid", "&").Replace("%3D", "");

                                  string res=string.Empty;

                                  if (chkWall_PostPicOnWall_ShareVideoOnlyMe)
                                  {

                                      //1 request
                                      string getRequestUrl = "https://www.facebook.com/ajax/sharer/?s=11&appid=" + appid + "&p[0]=" + media_info + "&sharer_type=all_modes&__asyncDialog=5&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzElx2&__req=1z&__rev=1296882";
                                      string GetHtml = HttpHelper.getHtmlfromUrl(new Uri(getRequestUrl));


                                      //2nd request 
                                      string getrequestUrl2 = "https://www.facebook.com/ajax/typeahead/first_degree.php?viewer=" + __user + "&filter[0]=user&filter[1]=page&filter[2]=app&filter[3]=group&filter[4]=event&options[0]=friends_only&options[1]=nm&token=v7&context=mentions&rsp=mentions&request_id=0.39445215719752014&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzElx2&__req=20&__rev=1296882";

                                      string GetHtml2 = HttpHelper.getHtmlfromUrl(new Uri(getrequestUrl2));

                                      // 3rd final post data 
                                      string attachment_parms = Utils.getBetween(GetHtml, "attachment[params][0]", "/>").Replace("\\\"", "").Replace("value=", "").Replace("\\", "").Trim();
                                      try
                                      {

                                        //  string PostUrl = "https://www.facebook.com/ajax/sharer/submit/?__av=" + __user;
                                          string PostUrl = "https://www.facebook.com/ajax/sharer/submit/?av=" + __user;
                                          string PostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget=" + pageTarget + "&post_as_page=1&message_text=&message=&attachment[params][0]=" + attachment_parms + "&attachment[type]=11&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=10&UITargetedPrivacyWidget=80&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzElx2&__req=21&ttstamp=2658170113715254847510311480&__rev=1296882";

                                          res = HttpHelper.postFormData(new Uri(PostUrl), PostData);
                                      }
                                      catch { };

                                          if (res.Contains("DialogHideOnSuccess"))
                                          {
                                              GlobusLogHelper.log.Info("Share Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Share Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                          }
                                          else
                                          {
                                              GlobusLogHelper.log.Info("Video Url Not Share WIth UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Video Url Not Share WIth UserName : " + fbUser.username);
                                          }

                                     
                                      try
                                      {
                                          int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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
                                          string ss = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/hovercard/hovercard.php?id=" + hovercardPhpId + "&type=mediatag&media_info=" + media_info + "&endpoint=%2Fajax%2Fhovercard%2Fhovercard.php%3Fid%3D" + Fid + "%26type%3Dmediatag%26media_info%3D" + media_info + "&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=b&__rev=1252742"));
                                          if (string.IsNullOrEmpty(ss))
                                          {
                                              ss = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=11&appid=" + appid + "&p[0]=" + media_info + "&sharer_type=all_modes&__asyncDialog=3&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDxCm4VpWGcw&__req=17&__rev=1255522"));
                                          }

                                          //  string DialogPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=2&appid=" + appid + "&p[0]=354853111241863&p[1]=1073741918&sharer_type=all_modes&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=c&__rev=1252742"));
                                          string DialogPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=2&appid=" + appid + "&p[0]=" + media_info + "&p[1]=1073741918&sharer_type=all_modes&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=c&__rev=1252742"));


                                          try
                                          {
                                              attachment = Utils.getBetween(DialogPageSource, "attachment[params][0]", "/>").Replace(" value=", string.Empty).Replace("\\\"", "").Replace("\\", "");
                                          }
                                          catch (Exception ex)
                                          {
                                              GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                          }

                                          try
                                          {
                                              pubcontent_params = Utils.getBetween(DialogPageSource, "pubcontent_params", ">").Replace("\"", "");
                                          }
                                          catch (Exception ex)
                                          {
                                              GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                          }

                                          string PostUrl = "https://www.facebook.com/ajax/sharer/submit/";
                                          // string[] Arr = System.Text.RegularExpressions.Regex.Split(DialogPageSource,"autocomplete=");
                                          //string ModeValue = Utils.getBetween(DialogPageSource, "name=\\\"mode\\\" value=", "id=").Replace("\\\"",string.Empty);

                                          string SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&message_text=&message=&attachment[params][0]=" + hovercardPhpId + "&attachment[type]=11&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCdz8&__req=k&ttstamp=265817195997498547153112106&__rev=1252742";
                                          try
                                          {
                                              string ModeValue1 = Utils.getBetween(PageSrcHome, "uiSelectorButton uiButton uiButtonSuppressed", "</a>");
                                              string ModeValue = Utils.getBetween(ModeValue1, "uiButtonText", "</span>").Replace("\">", string.Empty);
                                              if (string.IsNullOrEmpty(ModeValue))
                                              {
                                                  ModeValue = Utils.getBetween(PageSrcHome, "u_0_1z", "</span>").Replace("\">", string.Empty);
                                                  if (!ModeValue.Contains("Only Me"))
                                                  {
                                                      ModeValue = Utils.getBetween(PageSrcHome, "_55pe", "</span>").Replace("\">", string.Empty);
                                                  }
                                              }

                                              #region MyRegion
                                              //if (ModeValue == "Only Me" || ModeValue.Contains("Only Me") ||chkWall_PostPicOnWall_ShareVideoOnlyMe)
                                              //{
                                              //    pageTarget = Utils.getBetween(DialogPageSource, "pageTarget", "id").Replace(" value=", string.Empty).Replace("\\\"", "").Replace("\\", "");

                                              //  //  Eduart
                                              //    if (string.IsNullOrEmpty(pageTarget))
                                              //    {
                                              //        pageTarget = Utils.getBetween(PageSrcTageted, "Eduart", "?ref").Replace("/","");
                                              //    }
                                              //   // SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget=" + pageTarget + "&post_as_page=1&message_text=&message=&attachment[params][0]="+hovercardPhpId+"&attachment[type]=11&share_source_type=unknown&src=i&appid="+appid+"&parent_fbid=&ogid=&audience[0][value]=10&UITargetedPrivacyWidget=80&__user="+__user+"&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtV8sx6iWF3qGEZ94WpUpBxemdz8S&__req=f&ttstamp=2658172996953801061194511982&__rev=1260602";
                                              //    SharePostData = "fb_dtsg="+fb_dtsg+"&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget="+pageTarget+"&post_as_page=1&message_text=&message=&attachment[params][0]="+hovercardPhpId+"&attachment[type]=11&share_source_type=unknown&src=i&appid="+appid+"&parent_fbid=&ogid=&audience[0][value]=10&UITargetedPrivacyWidget=80&__user="+__user+"&__a=1&__dyn=7n8ajEAMCBynzpQ9UoGya4Cq7pEsx6iWF3qGEZ9LFDxCm4VpXzESu&__req=w&ttstamp=26581721011021016870114718781&__rev=1293281";
                                              //} 
                                              #endregion
                                          }
                                          catch (Exception ex)
                                          {
                                              GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                          }

                                          try
                                          {
                                              res = HttpHelper.postFormData(new Uri(PostUrl + "?__av=" + __user), SharePostData);
                                          }
                                          catch (Exception ex)
                                          {
                                              GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                          }

                                          if (res.Contains("errorSummary") || res.Contains("Could not post to Wall"))
                                          {

                                              pageTarget = Utils.getBetween(DialogPageSource, "pageTarget", "id").Replace(" value=", string.Empty).Replace("\\\"", "").Replace("\\", ""); ;

                                              string LoadTargedUrls = LoadTargedUrls_item + "@@";
                                              pageTarget = Utils.getBetween(LoadTargedUrls, "php?v=", "@@");
                                              SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget=" + pageTarget + "&post_as_page=1&message_text=&message=&attachment[params][0]=" + pageTarget + "&attachment[type]=11&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&UITargetedPrivacyWidget=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDxCm4VpWGcw&__req=19&ttstamp=2658172120651191138012111676122&__rev=1255522";
                                              res = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/sharer/submit/"), SharePostData);
                                          }

                                          if (string.IsNullOrEmpty(res))
                                          {
                                              try
                                              {
                                                  res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                              }
                                              catch (Exception ex)
                                              {
                                                  GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                              }
                                          }

                                          if (res.Contains("DialogHideOnSuccess"))
                                          {
                                              GlobusLogHelper.log.Info("Share Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Share Video Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                          }
                                          else
                                          {
                                              GlobusLogHelper.log.Info("Video Url Not Share WIth UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Video Url Not Share WIth UserName : " + fbUser.username);
                                          }

                                          try
                                          {
                                              int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                                              GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                              GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                              Thread.Sleep(delayInSeconds);
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
                    }
                    if (CheckContinue == 0)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Share With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Share With Username : " + fbUser.username);
        }

        public void ShareImageOnWall(ref FacebookUser fbUser)
        {
              
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Share Image On  Wall With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Share Image On  Wall With Username : " + fbUser.username);

                int CheckContinue = 0;
                if (chkCountinueProcessContinueShareProcess)
                {
                    CheckContinue = 1;
                }


                while (true)
                {

                    if (!IsPostAllPicPostPicOnWall)
                    {
                        bool ReturnPicstatus = false;
                        int intProxyPort = 80;
                        string xhpc_composerid = string.Empty;
                        string xhpc_targetid = string.Empty;
                        string message_text = string.Empty;
                        string message = string.Empty;
                        string imagePath = string.Empty;
                        Regex IdCheck = new Regex("^[0-9]*$");

                        try
                        {
                            string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                            foreach (var LoadTargedUrls_item in lstWallPostShareLoadTargedUrls)
                            {
                                //--
                                if (LoadTargedUrls_item.Contains("photo.php?fbid"))
                                {
                                    try
                                    {

                                        if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                                        {
                                            intProxyPort = int.Parse(fbUser.proxyport);
                                        }

                                       
                                        string __user = "";

                                        string fb_dtsg = "";

                                        string pgSrc_FanPageSearch = PageSrcHome;

                                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                        if (string.IsNullOrEmpty(__user))
                                        {
                                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                        }

                                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                                        {
                                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                                            return;
                                        }

                                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                        if (string.IsNullOrEmpty(fb_dtsg))
                                        {
                                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                        }
                                        try
                                        {
                                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        string PageSrcTageted = HttpHelper.getHtmlfromUrl(new Uri(LoadTargedUrls_item));
                                        string hovercardPhpId = string.Empty;
                                        string media_info = string.Empty;
                             
                                        string appid = string.Empty;
                                        string attachment = string.Empty;
                                        string pubcontent_params = string.Empty;

                                        try
                                        {
                                            appid = Utils.getBetween(PageSrcTageted, "appid=", "&");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        try
                                        {
                                            hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photo.php?v=", "&set=");
                                            if (string.IsNullOrEmpty(hovercardPhpId))
                                            {
                                                hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photo.php?fbid=", "&set=");
                                            }

                                            if (string.IsNullOrEmpty(hovercardPhpId))
                                            {
                                                hovercardPhpId = Utils.getBetween(PageSrcTageted, "page.php?id=", "\">");
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            if (string.IsNullOrEmpty(hovercardPhpId))
                                            {
                                                hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photos/a.", "."); ;
                                            }
                                            media_info = Utils.getBetween(PageSrcTageted, "media_info=", ">").Replace("\"", "");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }


                                        try
                                        {
                                            string P = Utils.getBetween(PageSrcTageted, "set=a.", "&amp;");
                                            string[] Arr = P.Split('.');
                                            if (Arr.Count() == 3)
                                            {
                                                P = Arr[2];
                                            }
                                            else if (Arr.Count() == 2)
                                            {
                                                P = Arr[1];
                                            }

                                            string p1 = Utils.getBetween(PageSrcTageted, "\"pid\":", ",");
                                            //p1 = Utils.getBetween(p1, "5D=", "&amp;");

                                            //string ss = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=2&appid=" + appid + "&p[0]=" + P + "&p[1]=" + p1 + "&sharer_type=all_modes&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDxCm4VoScw&__req=9&__rev=1257720"));

                                            string DialogPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=2&appid=" + appid + "&p[0]=" + P + "&p[1]=" + p1 + "&sharer_type=all_modes&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=c&__rev=1252742"));
                                            try
                                            {
                                                attachment = Utils.getBetween(DialogPageSource, "attachment[params][0]", "/>").Replace(" value=", string.Empty).Replace("\\\"", "").Replace("\\", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            try
                                            {
                                                pubcontent_params = Utils.getBetween(DialogPageSource, "pubcontent_params", ">").Replace("\"", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }



                                            string PostUrl = "https://www.facebook.com/ajax/sharer/submit/";


                                            // string SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&message_text=&message=&attachment[params][0]=" + hovercardPhpId + "&attachment[type]=11&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCdz8&__req=k&ttstamp=265817195997498547153112106&__rev=1252742";

                                            string SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget=" + __user + "&post_as_page=1&message_text=&message=&attachment[params][0]=" + P + "&attachment[params][1]=" + p1 + "&attachment[type]=2&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&UITargetedPrivacyWidget=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDxCm4VoScw&__req=b&ttstamp=2658170120508211398118898774&__rev=1257720";
                                            string res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            if (res.Contains("errorSummary"))
                                            {
                                                string pageTarget = Utils.getBetween(PageSrcTageted, "user_id", "/>").Replace("value=",string.Empty).Replace("\"","");

                                                SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&pageTarget=" + pageTarget + "&post_as_page=1&message_text=&message=&attachment[params][0]=" + P + "&attachment[params][1]="+p1+"&attachment[type]=2&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&UITargetedPrivacyWidget=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDxCm4VoScw&__req=i&ttstamp=265817111585119117508810812098&__rev=1259482";
                                               
                                                res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                                if (res.Contains("errorSummary"))
                                                {
                                                    SharePostData = "fb_dtsg="+fb_dtsg+"&ad_params=&pubcontent_params=%7B%22sbj_type%22%3A%22fof%22%7D&mode=self&friendTarget=&groupTarget=&pageTarget="+pageTarget+"&post_as_page=1&message_text=&message=&attachment[params][0]="+P+"&attachment[params][1]="+p1+"&attachment[type]=2&share_source_type=unknown&src=i&appid="+appid+"&parent_fbid=&ogid=&audience[0][value]=80&UITargetedPrivacyWidget=80&__user="+__user+"&__a=1&__dyn=7n8anEAMCBynzpQ9UoGya4Cqm5Aqbx2mbAKGiBAGGzQAjO7xCm4VpZaECdw&__req=x&ttstamp=265817211689578857551076788&__rev=1259482";
                                                    res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                                }
                                            }

                                            if (res.Contains("DialogHideOnSuccess"))
                                            {
                                                GlobusLogHelper.log.Info("Share Image Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Share Image Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Image Url Not Share WIth UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Image Url Not Share WIth UserName : " + fbUser.username);
                                            }

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
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
                                }
                                else
                                {
                                    try
                                    {

                                        if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                                        {
                                            intProxyPort = int.Parse(fbUser.proxyport);
                                        }


                                        string __user = "";

                                        string fb_dtsg = "";

                                        string pgSrc_FanPageSearch = PageSrcHome;

                                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                                        if (string.IsNullOrEmpty(__user))
                                        {
                                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                                        }

                                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                                        {
                                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                                            return;
                                        }

                                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                                        if (string.IsNullOrEmpty(fb_dtsg))
                                        {
                                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                                        }
                                        try
                                        {
                                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        string PageSrcTageted = HttpHelper.getHtmlfromUrl(new Uri(LoadTargedUrls_item));
                                        string hovercardPhpId = string.Empty;
                                        string media_info = string.Empty;
                                        string appid = string.Empty;
                                        string attachment = string.Empty;
                                        string pubcontent_params = string.Empty;

                                        try
                                        {
                                            appid = Utils.getBetween(PageSrcTageted, "appid=", "&");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        try
                                        {
                                            hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photo.php?v=", "&set=");

                                            if (string.IsNullOrEmpty(hovercardPhpId))
                                            {
                                                hovercardPhpId = Utils.getBetween(PageSrcTageted, "page.php?id=", "\">");
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            if (string.IsNullOrEmpty(hovercardPhpId))
                                            {
                                                hovercardPhpId = Utils.getBetween(LoadTargedUrls_item, "photos/a.", "."); ;
                                            }
                                            media_info = Utils.getBetween(PageSrcTageted, "media_info=", ">").Replace("\"", "");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }


                                        try
                                        {
                                            string ss = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/hovercard/hovercard.php?id=" + hovercardPhpId + "&type=mediatag&media_info=" + media_info + "&endpoint=%2Fajax%2Fhovercard%2Fhovercard.php%3Fid%3D100002441442201%26type%3Dmediatag%26media_info%3D" + media_info + "&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=b&__rev=1252742"));

                                            string DialogPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/sharer/?s=2&appid=" + appid + "&p[0]=354853111241863&p[1]=1073741918&sharer_type=all_modes&__asyncDialog=1&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=c&__rev=1252742"));
                                            try
                                            {
                                                attachment = Utils.getBetween(DialogPageSource, "attachment[params][0]", "/>").Replace(" value=", string.Empty).Replace("\\\"", "").Replace("\\", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            try
                                            {
                                                pubcontent_params = Utils.getBetween(DialogPageSource, "pubcontent_params", ">").Replace("\"", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }



                                            string PostUrl = "https://www.facebook.com/ajax/sharer/submit/";


                                            // string SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&message_text=&message=&attachment[params][0]=" + hovercardPhpId + "&attachment[type]=11&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCdz8&__req=k&ttstamp=265817195997498547153112106&__rev=1252742";

                                            string SharePostData = "fb_dtsg=" + fb_dtsg + "&ad_params=&pubcontent_params=%7B%22sbj_type%22%3Anull%7D&mode=self&friendTarget=&groupTarget=&message_text=" + LoadTargedUrls_item + "&message=&attachment[params][0]=" + hovercardPhpId + "&attachment[params][1]=1073741918&attachment[type]=2&share_source_type=unknown&src=i&appid=" + appid + "&parent_fbid=&ogid=&audience[0][value]=80&__user=" + __user + "&__a=1&__dyn=7n8ahyj2qm9udDgDxyIGzG8qeyp9Esx6iWF3qGEVF4WpUpBxCuGz8&__req=a&ttstamp=26581727180787555837211865&__rev=1252742";
                                            string res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            if (string.IsNullOrEmpty(res))
                                            {
                                                SharePostData = PostUrl;
                                                res = HttpHelper.postFormData(new Uri(PostUrl), SharePostData);
                                            }

                                            if (res.Contains("DialogHideOnSuccess"))
                                            {
                                                GlobusLogHelper.log.Info("Share Image Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Share Image Url : " + LoadTargedUrls_item + " with UserName : " + fbUser.username);

                                            }
                                            else
                                            {
                                                GlobusLogHelper.log.Info("Image Url Not Share WIth UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Image Url Not Share WIth UserName : " + fbUser.username);
                                            }

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                                Thread.Sleep(delayInSeconds);
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
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (CheckContinue == 0)
                    {
                        break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Share With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Share With Username : " + fbUser.username);
        
        }

        public void PostPictureOnWall(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Post Pic On Wall With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Post Pic On Wall With Username : " + fbUser.username);


                if (!IsPostAllPicPostPicOnWall)
                {
                    try
                    {
                        bool ReturnPicstatus = false;
                        int intProxyPort = 80;
                        string xhpc_composerid = string.Empty;
                        string xhpc_targetid = string.Empty;
                        string message_text = string.Empty;
                        Regex IdCheck = new Regex("^[0-9]*$");
                        if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                        {
                            intProxyPort = int.Parse(fbUser.proxyport);
                        }
                        string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                        string __user = "";
                        string fb_dtsg = "";

                        string pgSrc_FanPageSearch = PageSrcHome;

                        __user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
                        if (string.IsNullOrEmpty(__user))
                        {
                            __user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
                        }

                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                        {
                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                            return;
                        }

                        fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
                        if (string.IsNullOrEmpty(fb_dtsg))
                        {
                            fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
                        }
                        try
                        {
                            xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            string Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
                            string DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";

                            string res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                            if (string.IsNullOrEmpty(res))
                            {
                                Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
                                res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        string imagePath = string.Empty;
                        try
                        {
                            imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        string message = string.Empty;
                        try
                        {
                            if (chkWallPostPicOnWallWithMessage == true)
                            {
                                message = lstMessageCollectionPostPicOnWall[new Random().Next(0, lstMessageCollectionPostPicOnWall.Count)];                              
                            }
                            else
                            {
                                message = "";
                            }

                   
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                       
                        string status = string.Empty;

                        if(chkCountinueProcessGroupCamapinScheduler = false)
                        {
                            ReturnPicstatus = PostPicture(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, message, ref status);
                        }
                        else
                        {
                            ReturnPicstatus = PostPicture1(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);

                        }

                        if (ReturnPicstatus)
                        {
                            GlobusLogHelper.log.Info("Posted Picture On Own Wall !");
                            GlobusLogHelper.log.Debug("Posted Picture On Own Wall !");

                            if (string.IsNullOrEmpty(message))
                            {
                                GlobusLogHelper.log.Info("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);

                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);

                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Picture  Post  On Wall Using UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Picture Not Post To on Wall Using UserName : " + fbUser.username);

                        }


                        //Friends wallposting started

                        if (NumberOfFriendsSendPicOnWall>0)
                        {
                            GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                            GlobusLogHelper.log.Debug("Please wait finding the friends ID...");

                            List<string> lstFriend = new List<string>();
                            lstFriend = FBUtils.GetAllFriends(ref HttpHelper, __user);
                            lstFriend = lstFriend.Distinct().ToList();
                            int CountFriends = 0;
                            bool CheckStatus = false;
                            List<string> TempMessage = lstMessageCollectionPostPicOnWall;
                            foreach (var lstFriend_item in lstFriend)
                            {
                                //Check Database
                                try
                                {

                                    if (IsUniquePicPosting)
                                    {
                                        if (Utils.CheckIfMessagePosted(fbUser.username, lstFriend_item, "PicWithMessage"))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (NumberOfFriendsSendPicOnWall <= CountFriends)
                                {
                                    break;
                                }
                                try
                                {
                                    try
                                    {                                       
                                        message = lstMessageCollectionPostPicOnWall[new Random().Next(0, TempMessage.Count)];
                                       // TempMessage.Remove(message);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }


                                    CheckStatus = PostPictureOnFriendWall(ref fbUser, message, lstFriend_item, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (CheckStatus)
                                {
                                    CountFriends = CountFriends + 1;

                                    //Insert n Database

                                    if (IsUniquePicPosting)
                                    {
                                        Utils.InsertIntoDB(fbUser.username, lstFriend_item, "PicWithMessage");
                                    }


                                    GlobusLogHelper.log.Info("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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
                                    GlobusLogHelper.log.Info("Picture Not Post To on  friends Wall  " + "On Own friends : " + lstFriend_item + "Using UserName :" + fbUser.username);
                                    GlobusLogHelper.log.Info("Picture Not Post To on  friends Wall  " + "On Own friends : " + lstFriend_item + "Using UserName :" + fbUser.username);

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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
                        PostAlluploadedPicOnOwnWall(ref fbUser);
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

            GlobusLogHelper.log.Info("Process Completed Of Post Pic On Wall With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Post Pic On Wall With Username : " + fbUser.username);
        }

        private void PostAlluploadedPicOnOwnWall(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                bool ReturnPicstatus = false;
                int intProxyPort = 80;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string message_text = string.Empty;
                Regex IdCheck = new Regex("^[0-9]*$");
                if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                {
                    intProxyPort = int.Parse(fbUser.proxyport);
                }
                string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                string __user = "";
                string fb_dtsg = "";

                string pgSrc_FanPageSearch = PageSrcHome;
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
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {
                    string Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
                    string DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";

                    string res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                    if (string.IsNullOrEmpty(res))
                    {
                        Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
                        res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
                    }
                  
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                foreach (string imagePath in lstPicturecollectionPostPicOnWall)
                {
                    string message = string.Empty;

                    if (lstMessageCollectionPostPicOnWall.Count > 0 && chkWallPostPicOnWallWithMessage == true)
                    {
                        try
                        {
                            message = lstMessageCollectionPostPicOnWall[new Random().Next(0, lstMessageCollectionPostPicOnWall.Count)];
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        message = "";
                    }

                

                    string status = string.Empty;
                   // if (chkCountinueProcessGroupCamapinScheduler == true)
                    {
                        ReturnPicstatus = PostPicture(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, message, ref status);
                    }
                    if (ReturnPicstatus)
                    {
                        GlobusLogHelper.log.Info("Posted Picture On Own Wall !");
                        GlobusLogHelper.log.Debug("Posted Picture On Own Wall !");

                        if (string.IsNullOrEmpty(message))
                        {
                            GlobusLogHelper.log.Info("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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
                            GlobusLogHelper.log.Info("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);

                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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
                    else
                    {
                        GlobusLogHelper.log.Info("Picture Not Posted To on Wall Using UserName : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Picture Not Posted To on Wall Using UserName : " + fbUser.username);
                        try
                        {
                            int delayInSeconds = Utils.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
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

        public bool PostPicture(ref FacebookUser fbUser, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string message, ref string status)
        {

            bool isSentPicMessage = false;
            string fb_dtsg = string.Empty;
            string photo_id = string.Empty;
            string UsreId = string.Empty;
            string xhpc_composerid = string.Empty;
            string xhpc_targetid = string.Empty;
            string message_text = string.Empty;
            string picfilepath = string.Empty;

            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                picfilepath = localImagePath;

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

                fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                }


                string pageSource_HomeData = pageSource_Home;
                try
                {
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {

                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

                nvc.Add("fb_dtsg", fb_dtsg);
                nvc.Add("xhpc_targetid", xhpc_targetid);
                nvc.Add("xhpc_context", "home");
                nvc.Add("xhpc_ismeta", "1");
                nvc.Add("xhpc_fbx", "1");
                nvc.Add("xhpc_timeline", "");
                nvc.Add("xhpc_composerid", xhpc_composerid);
                nvc.Add("xhpc_message_text", message);
                nvc.Add("xhpc_message", message);



                string response = string.Empty;
                try
                {
                    response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                    
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(response))
                {
                    try
                    {
                        response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string posturl = FBGlobals.Instance.PostPicOnWallPostAjaxCitySharerResetUrl;
                string postdata = "__user=" + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=1658167761111108210145";
                string responsestring = HttpHelper.postFormData(new Uri(posturl), postdata);
                if (!response.Contains("error") || !response.Contains("You will no longer get notifications for this story") && !string.IsNullOrEmpty(response))
                {
                    isSentPicMessage = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return isSentPicMessage;
        }

        public bool PostPicture1(ref FacebookUser fbUser, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword,  ref string status)
        {

            bool isSentPicMessage = false;
            string fb_dtsg = string.Empty;
            string photo_id = string.Empty;
            string UsreId = string.Empty;
            string xhpc_composerid = string.Empty;
            string xhpc_targetid = string.Empty;
            string message_text = string.Empty;
            string picfilepath = string.Empty;

            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                picfilepath = localImagePath;

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

                fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                }


                string pageSource_HomeData = pageSource_Home;
                try
                {
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

                nvc.Add("fb_dtsg", fb_dtsg);
                nvc.Add("xhpc_targetid", xhpc_targetid);
                nvc.Add("xhpc_context", "home");
                nvc.Add("xhpc_ismeta", "1");
                nvc.Add("xhpc_fbx", "1");
                nvc.Add("xhpc_timeline", "");
                nvc.Add("xhpc_composerid", xhpc_composerid);
                //nvc.Add("xhpc_message_text", message);
                //nvc.Add("xhpc_message", message);



                string response = string.Empty;
                try
                {
                    response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(response))
                {
                    try
                    {
                        response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string posturl = FBGlobals.Instance.PostPicOnWallPostAjaxCitySharerResetUrl;
                string postdata = "__user=" + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=1658167761111108210145";
                string responsestring = HttpHelper.postFormData(new Uri(posturl), postdata);
                if (!response.Contains("error") && !string.IsNullOrEmpty(response))
                {
                    isSentPicMessage = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return isSentPicMessage;
        }

        public bool PostPictureOnFriendWall(ref FacebookUser fbUser,string Message,string FriendID, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, ref string status)
        {

            bool isSentPicMessage = false;
            string fb_dtsg = string.Empty;
            string photo_id = string.Empty;
            string UserId = string.Empty;
            string xhpc_composerid = string.Empty;
            string xhpc_targetid = string.Empty;
            string message_text = string.Empty;
            string picfilepath = string.Empty;
            string Response = string.Empty;
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                picfilepath = localImagePath;

               // string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/profile.php?id=" + fbUser.username));

                UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

                fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                }
                string pageSource_HomeData = pageSource_Home;
                try
                {
                    xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                string gridid = string.Empty;
                string qn = string.Empty;
                string source = string.Empty;
                string tagger_session_id = string.Empty;
                string hide_object_attachment = string.Empty;

                string is_file_form =  string.Empty;
                string album_type = string.Empty;
                string composer_unpublished_photo= string.Empty;
                string clp = string.Empty;
                string xhpc_publish_type =  string.Empty;
                string xhpc_context = string.Empty;
                string application =  string.Empty;
                string xhpc_ismeta= string.Empty;
                string xhpc_timeline= string.Empty;
                string disable_location_sharing = string.Empty;





                try
                {
                    Response = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/media/upload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + FriendID + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=mainprivacywidget&loaded_components[10]=ogtaggericon&loaded_components[11]=withtaggericon&loaded_components[12]=placetaggericon&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=w&ttstamp=26581728812272951201044890108&__rev=1391091");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {
                    gridid = Utils.getBetween(Response, "gridID\":\"", "\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    qn = Utils.getBetween(Response, "qn\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    source = Utils.getBetween(Response, "source\":", ",");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    fb_dtsg = Utils.getBetween(Response, "fb_dtsg\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    tagger_session_id = Utils.getBetween(Response, "tagger_session_id\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    hide_object_attachment = Utils.getBetween(Response, "hide_object_attachment\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    is_file_form = Utils.getBetween(Response, "is_file_form\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    album_type = Utils.getBetween(Response, "album_type\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    composer_unpublished_photo = Utils.getBetween(Response, "composer_unpublished_photo[]\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    clp = "{\"cl_impid\":\"62172361\",\"clearcounter\":0,\"elementid\":\"u_0_1h\",\"version\":\"x\",\"parent_fbid\":" + FriendID + "}";
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_publish_type = Utils.getBetween(Response, "xhpc_publish_type\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_context = Utils.getBetween(Response, "xhpc_context\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    application = Utils.getBetween(Response, "application\":\"", "\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_ismeta = Utils.getBetween(Response, "xhpc_ismeta\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    xhpc_timeline = Utils.getBetween(Response, "xhpc_timeline\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    disable_location_sharing = Utils.getBetween(Response, "disable_location_sharing\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

                nvc.Add("fb_dtsg", fb_dtsg);
                nvc.Add("source", source);
                nvc.Add("profile_id", UserId);
                nvc.Add("grid_id", gridid);
                nvc.Add("qn", qn);
                nvc.Add("0", localImagePath);
                nvc.Add("upload_id","1025");



                string response = string.Empty;
                try
                {
                    response = HttpHelper.HttpUploadPictureForWallNew(ref HttpHelper, UserId, "https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + FriendID + "&__av=" + UserId + "&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=13&fb_dtsg=" + fb_dtsg + "&ttstamp=26581728812272951201044890108&__rev=1391091" + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                    composer_unpublished_photo = Utils.getBetween(response, "composer_unpublished_photo[]\\\" value=\\\"", "\\\"");
                    //fb_dtsg = Utils.getBetween(response, "fb_dtsg\\\" value=\\\"", "\\\"");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(response))
                {
                    try
                    {
                        response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UserId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {

                    nvc.Clear();
                    nvc.Add("composer_session_id","");
                    nvc.Add("fb_dtsg", fb_dtsg);
                    nvc.Add("xhpc_context", xhpc_context);
                    nvc.Add("xhpc_ismeta", xhpc_ismeta);
                    nvc.Add("xhpc_timeline", xhpc_timeline);
                    nvc.Add("xhpc_composerid", xhpc_composerid);
                    nvc.Add("xhpc_targetid", FriendID);
                    nvc.Add("xhpc_publish_type", xhpc_publish_type);
                    nvc.Add("clp", "");
                    nvc.Add("xhpc_message_text", Message);
                    nvc.Add("xhpc_message", Message);
                    nvc.Add("composer_unpublished_photo[]", composer_unpublished_photo);
                    nvc.Add("album_type", album_type);
                    nvc.Add("is_file_form", is_file_form);
                    nvc.Add("oid","");
                    nvc.Add("qn", qn);
                    nvc.Add("application", application);
                    nvc.Add("backdated_date[year]","" );
                    nvc.Add("backdated_date[month]", "");
                    nvc.Add("backdated_date[day]", "");
                    nvc.Add("backdated_date[hour]", "");
                    nvc.Add("backdated_date[minute]", "");
                    nvc.Add("is_explicit_place", "");
                    nvc.Add("composertags_place", "");
                    nvc.Add("composertags_place_name", "");
                    nvc.Add("tagger_session_id", tagger_session_id);
                    nvc.Add("action_type_id[]","");
                    nvc.Add("object_str[]", "");
                    nvc.Add("object_id[]", "");
                    nvc.Add("og_location_id[]", "");
                    nvc.Add("hide_object_attachment", hide_object_attachment);
                    nvc.Add("og_suggestion_mechanism", "");
                    nvc.Add("og_suggestion_logging_data", "");
                    nvc.Add("icon_id", "");
                    nvc.Add("composertags_city", "");
                    nvc.Add("disable_location_sharing", disable_location_sharing);
                    nvc.Add("composer_predicted_city","");
                }
                string responsestring = HttpHelper.HttpUploadPictureForWallNewFinal(ref HttpHelper, UserId, "https://upload.facebook.com/media/upload/photos/composer/?__av="+UserId+"&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=o&fb_dtsg="+fb_dtsg+"&ttstamp=26581691188750571181101086979&__rev=1391091", "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
                if (responsestring.Contains("error") && !string.IsNullOrEmpty(responsestring))
                {
                    //string resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/places/city_sharer_reset.php"), "target_id=0&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=r&fb_dtsg=AQEvW29vnlEO&ttstamp=26581691188750571181101086979&__rev=1391091");
                    isSentPicMessage = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return isSentPicMessage;
        }

        public void WallPostingWithTestMessage(ref FacebookUser fbUser)
        {
            try
            {
                string UsreId = string.Empty;

                GlobusLogHelper.log.Info("Start Wall Posting With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Wall Posting With Username : " + fbUser.username);

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                string ProFilePost = FBGlobals.Instance.fbProfileUrl;

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

                lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();
                if (IsUseTextMessageWallPoster)
                {
                    MsgWallPoster = lstWallMessageWallPoster[Utils.GenerateRandom(0, lstWallMessageWallPoster.Count)];
                }
                if (IsUseURLsMessageWallPoster)
                {
                    MsgWallPoster = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
                }
                if (ChkSpinnerWallMessaeWallPoster)
                {
                    MsgWallPoster = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count)];
                }

                if (Globals.CheckLicenseManager =="fdfreetrial")
                { 
                    MsgWallPoster=MsgWallPoster+"\n\n\n\n Sent from FREE version of Facedominator. To remove this message, please buy it.";
                }

                #region CommentedCode
                //if (lstWallPostURLsWallPoster.Count > 0)
                //{
                //    lstFriend = lstFriend.Distinct().ToList();
                //    if (!UseAllUrlWallPoster)
                //    {
                //        postsuccess = PostOnWallWithURLAndItsImage(ref fbUser, lstWallPostURLsWallPoster, lstFriend);
                //        return;
                //    }
                //    else
                //    {
                //        PostOnWallWithAllURLAndItsImage(ref fbUser, lstWallPostURLsWallPoster, lstFriend);
                //        //  return;
                //    }
                //} 
                #endregion

                string profileUrl = ProFilePost + UsreId + "&sk=wall";
                string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl));

                string wallmessage = MsgWallPoster;
                wallmessage = wallmessage.Replace("<friend first name>", string.Empty);

                if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
                {
                    if (lstWallPostURLsWallPoster.Count > 0)
                    {

                        wallmessage = lstWallPostURLsWallPoster[Utils.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Posting message on own wall: " + wallmessage);
                        GlobusLogHelper.log.Debug("Posting message on own wall: " + wallmessage);
                    }

                    wallmessage = wallmessage.Replace("=", "%3D");

                    string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
                    string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");

                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
                    }
                   
                    string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
                    if (string.IsNullOrEmpty(fb_dtsg))
                    {
                        xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
                    }

                    string ResponseWallPost = string.Empty;
                    string sessionId = Utils.GenerateTimeStamp();
                    wallmessage = Uri.EscapeUriString(wallmessage);
                    ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=", "");

                    if (ResponseWallPost.Length < 300)
                    {
                        ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + (xhpc_composerid) + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=", "");
                    }
                    if (ResponseWallPost.Length >= 300)
                    {
                        TotalNoOfWallPoster_Counter++;

                        GlobusLogHelper.log.Info("Posted message on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Posted message on own wall " + fbUser.username);
                    }

                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't post on own wall " + fbUser.username);
                        GlobusLogHelper.log.Debug("Couldn't post on own wall " + fbUser.username);
                    }
                }
                GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                GlobusLogHelper.log.Debug("Please wait finding the friends ID...");
                if (NoOfFriendsWallPoster != 0)
                {
                    lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UsreId);
                    lstFriend = lstFriend.Distinct().ToList();
                }
                var itemId = lstFriend.Distinct();
                try
                {
                     int countFrnd=0;
                    string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UsreId));
                    if (pageSource.Contains("pagelet_timeline_medley_friends"))
                    {
                        string findTheAllFrnList = FBUtils.getBetween(pageSource_Home, "pagelet_timeline_medley_friends", "</span>");
                        if (string.IsNullOrEmpty(findTheAllFrnList))
                        {
                            string[] aa = System.Text.RegularExpressions.Regex.Split(pageSource, "pagelet_timeline_medley_friends");
                            findTheAllFrnList = FBUtils.getBetween(aa[1], "\"_gs6\">", "</span>");
                         
                        }
                   
                        countFrnd = Convert.ToInt32(findTheAllFrnList);
                    }
                    else if (pageSource.Contains("FriendCount"))
                    {
                        string findTheAllFrnList = FBUtils.getBetween(pageSource_Home, "FriendCount", "}");
                    }
                   

                 
                      //FriendCount

                    GlobusLogHelper.log.Info("Found " + countFrnd + " friend's ids");
                    GlobusLogHelper.log.Debug("Found " + countFrnd + " friend's ids");

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                int CountPostWall = 1;

                // messageCountWallPoster = 5;
                messageCountWallPoster = NoOfFriendsWallPoster;

                int friendval = messageCountWallPoster;
                int friendCount = 0;

                if (itemId.Count() > friendval)
                {
                    friendCount = friendval;
                }
                else
                {
                    friendCount = itemId.Count();
                }

                try
                {
                    ///Generate a random no list ranging 0-lstMessages.Count
                    ArrayList randomNoList = Utils.RandomNumbers(lstMessagesWallPoster.Count - 1);

                    int msgIndex = 0;

                    foreach (string friendId in itemId)
                    {
                        if (IsUniqueMessagePosting)
                        {
                            if (Utils.CheckIfMessagePosted(fbUser.username, friendId, "Text"))
                            {
                                continue;
                            }
                        }
                        if (CountPostWall > friendCount)
                        {
                            return;
                        }
                        try
                        {
                            #region SelectQuery
                            // System.Data.DataSet ds = new DataSet();
                            try
                            {
                                //string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
                                // ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            //if (ds.Tables[0].Rows.Count < 1)
                            {
                                // return; 
                            #endregion

                                string message = string.Empty;
                                if (UsreId != friendId)
                                {
                                    #region Select Msg according to Mode
                                    try
                                    {
                                        ///Normal, 1 msg to all friends
                                        if (UseOneMsgToAllFriendsWallPoster)
                                        {
                                            message = MsgWallPoster.Replace(" ", " ");  //%20;
                                        }

                                        ///For Random, might be Unique, might not be
                                        else if (UseRandomWallPoster)
                                        {
                                            if (msgIndex < randomNoList.Count)
                                            {
                                                msgIndex = (int)randomNoList[msgIndex];
                                                message = lstMessagesWallPoster[msgIndex];
                                                msgIndex++;
                                            }
                                            else if (lstMessagesWallPoster.Count > msgIndex)
                                            {
                                                message = lstMessagesWallPoster[msgIndex];
                                                msgIndex++;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    msgIndex = 0;
                                                    randomNoList = Utils.RandomNumbers(lstMessagesWallPoster.Count - 1);
                                                    message = lstMessagesWallPoster[msgIndex];
                                                    msgIndex++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }
                                        }

                                        ///For Unique or Different Msg for each friend                                        

                                        else if (UseUniqueMsgToAllFriendsWallPoster)
                                        {
                                            if (lstSpinnerWallMessageWallPoster.Count > countWallPoster - 1)
                                            {
                                                try
                                                {
                                                    message = lstSpinnerWallMessageWallPoster[countWallPoster - 1];

                                                    if (lstSpinnerWallMessageWallPoster.Contains(message))
                                                    {
                                                        lstSpinnerWallMessageWallPoster.Remove(message);
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
                                                    message = lstSpinnerWallMessageWallPoster[Utils.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count - 1)];
                                                }
                                                catch (Exception ex)
                                                {
                                                    message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
                                                }

                                                    
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    #endregion

                                    try
                                    {
                                        if (!ChkSpinnerWallMessaeWallPoster)
                                        {
                                            if (!string.IsNullOrEmpty(message))
                                            {
                                              

                                                PostOnFriendsWallTestMessage(friendId, message, ref fbUser, ref UsreId); 
                                            }
                                           
                                        }
                                        else
                                        {
                                            if (lstSpinnerWallMessageWallPoster.Count > 0)
                                            {
                                                if (!string.IsNullOrEmpty(message))
                                                {
                                                  
                                                    PostOnFriendWallUsingSpinMsg(friendId, message, ref fbUser, ref UsreId);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    CountPostWall++;
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

                GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            finally
            {
                GlobusLogHelper.log.Info("Wall Posting Completed With Username : " + fbUser.username);
                GlobusLogHelper.log.Debug("Wall Posting Completed With Username : " + fbUser.username);
                // HttpHelper.http.Dispose(); 
            }
        }       

        private void PostOnFriendsWallTestMessage(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string friendid = friendId;
                string wallMessage = wallmessage;
                DateTime datetiemvalue = DateTime.Now;
                TimeSpan xcx = DateTime.Now - datetiemvalue;

                if (!statusForGreetingMsgWallPoster)
                {
                    string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

                    if (postUrl.Contains("https://"))
                    {
                        postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
                        string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl));

                        if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
                        {
                            GlobusLogHelper.log.Info(countWallPoster.ToString() + " Posting on wall " + postUrl);
                            GlobusLogHelper.log.Debug(countWallPoster.ToString() + " Posting on wall " + postUrl);

                            string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);

                            string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

                            string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");
                            wallmessage = Uri.EscapeUriString(wallmessage);
                            string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
                            string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111, "");
                            int length = ResponseWallPost.Length;

                            string postDataWalllpost1112 = string.Empty;
                            string ResponseWallPost2 = string.Empty;
                            if (!(length > 1100))
                            {
                                postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
                                ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112, "");

                                int length2 = ResponseWallPost2.Length;
                                if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
                                {
                                    TotalNoOfWallPoster_Counter++;
                                    if (IsUniqueMessagePosting)
                                    {
                                        Utils.InsertIntoDB(fbUser.username, friendId , "Text"); 
                                    }
                                    GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

                                    countWallPoster++;

                                    try
                                    {                                      

                                        int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    try
                                    {
                                        #region insertQuery
                                        //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                        //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
                                {
                                    TotalNoOfWallPoster_Counter++;
                                    GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                    countWallPoster++;

                                    try
                                    {                                       

                                        int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                        Thread.Sleep(delayInSeconds);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    try
                                    {
                                        #region insertQuery
                                        //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                        //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost2);
                                    GlobusLogHelper.log.Info("Error : " + errorSummary + " not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                }
                            }
                            else
                            {
                                TotalNoOfWallPoster_Counter++;

                                GlobusLogHelper.log.Info("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

                                countWallPoster++;
                                if (IsUniqueMessagePosting)
                                {
                                    Utils.InsertIntoDB(fbUser.username, friendId, "Text");
                                }

                                try
                                {                                 

                                    int delayInSeconds = Utils.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                try
                                {
                                    #region insertQuery
                                    //string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
                                    //BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            System.Threading.Thread.Sleep(4000);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);

                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    try
                    {
                        PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UsreId);
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
