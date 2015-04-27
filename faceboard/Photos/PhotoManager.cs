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

namespace Photos
{
    public class PhotoManager
    {
        public BaseLib.Events photoTaggingEvent = null;

        #region Global Variables For Photo Tagging

        readonly object lockrThreadControllerPhotoTagging = new object();
        public bool isStopPhotoTagging = false;
        int countThreadControllerPhotoTagging = 0;
        public static int counterFriend = 0;
        public static int counterFriendName = 0;
        public static int TotalNoOfPhotoTaged_Counnter = 0;

        public static int minDelayPhotoTagging = 10;
        public static int maxDelayPhotoTagging = 20;

        public List<Thread> lstThreadsPhotoTagging = new List<Thread>();

        #endregion

        #region Property For Photo Tagging

        public static string PhotoTagingProcessUsing
        {
            get;
            set;
        }

        public int NoOfThreadsPhotoTagging
        {
            get;
            set;
        }

        public int NoOfTaggingFriendsPhotoTagging
        {
            get;
            set;
        }

        public List<string> LstPhotoTaggingURLsPhotoTagging
        {
            get;
            set;
        }

        #endregion

        #region Global variables for Downloading Photo
        public static string ExportPhotosPath = string.Empty;
        #endregion

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                photoTaggingEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public PhotoManager()
        {
            try
            {
                photoTaggingEvent = new Events();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void StartPhotoTagging()
        {
            try
            {
                countThreadControllerPhotoTagging = 0;
                int numberOfAccountPatch = 25;

                if (NoOfThreadsPhotoTagging > 0)
                {
                    numberOfAccountPatch = NoOfThreadsPhotoTagging;
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
                                lock (lockrThreadControllerPhotoTagging)
                                {
                                    try
                                    {
                                        if (countThreadControllerPhotoTagging >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerPhotoTagging);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsPhotoTagging);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerPhotoTagging++;
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

        public void StartMultiThreadsPhotoTagging(object parameters)
        {
            try
            {
                if (!isStopPhotoTagging)
                {
                    try
                    {
                        lstThreadsPhotoTagging.Add(Thread.CurrentThread);
                        lstThreadsPhotoTagging.Distinct();
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
                                StartActionPhotoTagging(ref objFacebookUser);
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
                   // if (!isStopPhotoTagging)
                    {
                        lock (lockrThreadControllerPhotoTagging)
                        {
                            countThreadControllerPhotoTagging--;
                            Monitor.Pulse(lockrThreadControllerPhotoTagging);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void StartActionPhotoTagging(ref FacebookUser fbUser)
        {
            try
            {
                if (PhotoTagingProcessUsing=="Photo Tag")
                {
                    AddPhotosTag(ref fbUser);
                }
                else if (PhotoTagingProcessUsing == "Post Tag")
                {
                    AddPostTag(ref fbUser);

                }
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddPhotosTag(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string ProFilePost = string.Empty;
                string __user = string.Empty;

                string userId = "";
                List<string> lstphotourl = new List<string>();
                try
                {
                    string UserId = string.Empty;

                    string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                    UserId =GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
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

                    GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                    GlobusLogHelper.log.Debug("Please wait finding the friends ID...");
                    List<string> lstfriendsId = FBUtils.GetAllFriends(ref HttpHelper, UserId);

                    lstfriendsId.Remove("");
                    lstfriendsId = lstfriendsId.Distinct().ToList();


                    string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UserId));
                    string findTheAllFrnList = string.Empty;
                    int countFrnd = 0;
                    try
                    {
                        findTheAllFrnList = FBUtils.getBetween(pageSource, "<span class=\"_gs6\">", "</span>");
                       // findTheAllFrnList = findTheAllFrnList.Replace("\">Friends<span class=\"_gs6\">", string.Empty).Replace(",", string.Empty);
                        try
                        {
                            countFrnd = Convert.ToInt32(findTheAllFrnList);
                        }
                        catch (Exception ex)
                        { }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    GlobusLogHelper.log.Info("Total Friends : " + countFrnd + " With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Total Friends : " + countFrnd+ " With Username : " + fbUser.username);


                    GlobusLogHelper.log.Info("Process Start Of Photo Tagging  With Username >>> " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Start Of Photo Tagging  With Username >>> " + fbUser.username);

                    List<string> lstFriendname = new List<string>();
                    List<string> lstFriend = new List<string>();

                    if (LstPhotoTaggingURLsPhotoTagging.Count > 0)
                    {
                        if (NoOfTaggingFriendsPhotoTagging > lstfriendsId.Count)
                        {
                            NoOfTaggingFriendsPhotoTagging = lstfriendsId.Count;
                        }
                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging+1; i++)
                        {
                            try
                            {
                                string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstfriendsId[i]));
                                if (strFriendInfo.Contains("\"name\":"))
                                {
                                    try
                                    {
                                        //string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                        string strName = Utils.getBetween(strFriendInfo, "\"name\": \"", "\"");
                                        lstFriendname.Add(strName);
                                        lstFriend.Add(lstfriendsId[i]);
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
                        for (int i = 0; i < LstPhotoTaggingURLsPhotoTagging.Count; i++)
                         {
                            try
                            {

                                string photoUrl = LstPhotoTaggingURLsPhotoTagging[i];
                                PhotoTagingToYourFriends(ref fbUser,photoUrl, UserId, ref lstfriendsId,ref lstFriendname,ref lstFriend);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        string profilepagesource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + userId));
                        if (profilepagesource.Contains("pagelet_timeline_photos_nav_top"))
                        {
                            string photourl = profilepagesource.Substring(profilepagesource.IndexOf("pagelet_timeline_photos_nav_top", 200));
                            if (photourl.Contains("href"))
                            {
                                photourl = photourl.Substring(photourl.IndexOf("href="), photourl.IndexOf("\"", photourl.IndexOf("href=") + 10) - photourl.IndexOf("href=")).Replace("href=", string.Empty).Replace("\"", string.Empty);
                                string photopagesource = HttpHelper.getHtmlfromUrl(new Uri(photourl));
                                if (photopagesource.Contains("fbTimelineSection mtm fbTimelinePhotosAndVideosOfMe fbTimelineTabSection"))
                                {
                                    string photohref = photopagesource.Substring(photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent"), photopagesource.IndexOf("</tbody>", photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent")) - photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent"));
                                    if (photohref.Contains("href="))
                                    {
                                        string[] photohrefArr = Regex.Split(photohref, "href=");
                                        for (int i = 1; i < photohrefArr.Length; i++)
                                        {
                                            try
                                            {
                                                string[] photohrefArr1 = Regex.Split(photohrefArr[i], "\"");
                                                string photohref1 = photohrefArr1[1].Replace("amp;", string.Empty);
                                                lstphotourl.Add(photohref1);
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
                        try
                        {
                            string randomphotourl = lstphotourl[new Random().Next(0, lstphotourl.Count - 1)];

                            PhotoTaging(ref fbUser, randomphotourl, userId);
                        }
                        catch(Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " +ex.StackTrace);
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

            GlobusLogHelper.log.Info("Process Completed Of Photo Tagging  With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Photo Tagging  With Username >>> " + fbUser.username);

        }

        private void PhotoTagingToYourFriends(ref FacebookUser fbuser, string photourl, string userId, ref List<string> lstfriendsId, ref List<string> lstFriendname, ref List<string> lstFriend)
        {
            lock(this)
            {
            bool CheckLikePage = true;
            try
            {               
                GlobusHttpHelper HttpHelper = fbuser.globusHttpHelper;
                string cachedAlbum = "";
                string cs_ver = "";
                string pid = "";
                string id = "";
                string subject = "";
                string name = "";
                string action = "";
                string source = "";
                string qn = "";
                string position = "";
                string x = "";
                string y = "";
                string from_facebox = "";
                string fb_dtsg = "";
                string _user = "";
                string phstamp = "";

                string FanpageUrl = Utils.getBetween("@@" + photourl, "@@", "/photos/");
                string pagesourceofrandomphotourl = HttpHelper.getHtmlfromUrl(new Uri(photourl));

                if (CheckLikePage)
                {
                    LikePageWIthPhotoTaging(ref fbuser, photourl);
                    CheckLikePage = false;
                    
                }
                                    


                if (lstFriendname.Count == lstFriend.Count)
                {

                    {
                        int counter = 1;

                        GlobusLogHelper.log.Info("Number of tagging friends : " + NoOfTaggingFriendsPhotoTagging);
                        GlobusLogHelper.log.Debug("Number of tagging friends : " + NoOfTaggingFriendsPhotoTagging);

                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging+1; i++)
                        {
                            try
                            {
                                ///for tagging All friends (Not only Starting Friends)

                                if (lstfriendsId.Count <= counterFriend)
                                {
                                    counterFriendName = 0;
                                    counterFriend = 0;
                                }

                                string randomfriendId = lstfriendsId[counterFriend];
                                string randomfriendName = string.Empty;

                                counterFriendName++;
                                counterFriend++;

                                try
                                {
                                    string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + randomfriendId));
                                    if (strFriendInfo.Contains("\"name\":"))
                                    {
                                        string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                        randomfriendName = strName;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (string.IsNullOrEmpty(randomfriendName))
                                {
                                    continue;
                                }

                                cachedAlbum = "-1";
                                cs_ver = "0";
                                string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                                if (pidArr.Count() > 2)
                                {
                                    try
                                    {
                                        pidArr = Regex.Split(pidArr[2], "\"");
                                        foreach (string item in pidArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 3)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        try
                                                        {
                                                            pid = item.Substring(0, item.IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Trim();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }
                                                        
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        pid = item.Replace("\"", string.Empty).Trim();
                                                        pid = pid.Replace("=","");
                                                        break;
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

                                string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                                if (idArr.Count() > 1)
                                {
                                    try
                                    {
                                        idArr = Regex.Split(idArr[1], "\"");
                                        if (idArr.Count() > 1)
                                        {
                                            foreach (string item in idArr)
                                            {

                                                try
                                                {
                                                    if (item.Length > 6)
                                                    {
                                                        if (item.Contains(","))
                                                        {
                                                                try
                                                                {
                                                                   id = item.Substring(0, item.IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Trim();
                                                                }
                                                                catch(Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " +ex.StackTrace);
                                                                }
                                                                    break;
                                                        }
                                                        else
                                                        {
                                                            id = item.Replace("\"", string.Empty).Trim();
                                                            break;
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
                                subject = randomfriendId;
                                name = randomfriendName;
                                action = "add";
                                source = "permalink";

                                string _rev = Utils.getBetween(pagesourceofrandomphotourl,"_rev",",\"");
                                _rev = _rev.Replace(":", string.Empty).Replace("\"",string.Empty);

                                string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");

                                if (qnArr.Count() > 1)
                                {
                                    try
                                    {
                                        qnArr = Regex.Split(qnArr[1], "\"");
                                        foreach (string item in qnArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 3)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        qn = item.Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty).Trim();
                                                        qn = qn.Replace("[", string.Empty).Replace("]", string.Empty); 
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        qn = item.Replace("\"", string.Empty).Trim();
                                                        break;
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

                                string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");
                                if (positionArr.Count() > 1)
                                {
                                    try
                                    {
                                        positionArr = Regex.Split(positionArr[1], "\"");
                                        foreach (string item in positionArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 6)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        position = item.Replace("\"", string.Empty).Trim();
                                                        break;
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
                                    pid = pid.Replace("=", "");

                                    x = new Random().Next(53, 57).ToString();
                                    x = x + ".18518518518518";
                                    y = new Random().Next(73, 77).ToString();
                                    y = y + ".55555555555556";
                                    from_facebox = "false";

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pagesourceofrandomphotourl);

                                //string fbid = string.Empty;
                               // fbid = Utils.getBetween(pagesourceofrandomphotourl, "fbid=", "&amp");
                                _user = userId;
                               // 
                                if (pagesourceofrandomphotourl.Contains("fbPhotosPhotoActionsTag tagButton uiButton uiButtonOverlay") || pagesourceofrandomphotourl.Contains("Click on the photo to start tagging") || pagesourceofrandomphotourl.Contains("fbPhotosPhotoTagboxBase"))   //  fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay
                                {
                                    string postdata = " cachedAlbum=" + cachedAlbum + "&photo=" + position + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";  //(Almost) cachedAlbum=-1&photo=114983038652468&__user=100004223172781&__a=1&fb_dtsg=AQAuULXo&phstamp=165816511785768811182
                                    string posturl = FBGlobals.Instance.PhotoTaggingPostAjaxTagsAlbumUrl;

                                    //GoRound:
                                    try
                                    {
                                        System.Threading.Thread.Sleep(2 * 1000);
                                        string response = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    //name = Uri.EscapeUriString(name);
                                    string postdata1 = "cs_ver=" + cs_ver + "&pid=" + pid + "&id" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=2658168122979010753"; // (Almost)  cs_ver=0&pid=121971&id=100004223172781&subject=100004155841334&name=Bones%20Hood&action=add&source=permalink&qn=1347604780&position=114983038652468&x=40.55555555555556&y=52.928870292887034&from_facebox=false&tagging_mode=true&__user=100004223172781&__a=1&fb_dtsg=AQAuULXo&phstamp=1658165117857688111271
                                   
                                    string posturl1 = FBGlobals.Instance.PhotoTaggingPostAjaxPhotoTaggingAjaxUrl;

                                    string response1 = string.Empty;

                                    System.Threading.Thread.Sleep(2 * 1000);
                                    response1 = HttpHelper.postFormData(new Uri(posturl1), postdata1);


                                    if (response1.Contains("Sorry, an error occurred.  Please try again.") || response1.Contains("Something Went Wrong"))
                                    {
                                        postdata1 = "cs_ver="+cs_ver+"&pid="+pid+"&fbid=1571706343072884&id=100007006000732&subject=100001169497244&name=Alisha%20Rajpoot&action=add&source=permalink&qn=1421653748&position=1571706343072884&slsource&slset=o.156687351012419&x=80.9670781893005&y=44.372222222222&from_facebox=true&tagging_mode=true&__user=100007006000732&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzElx2ubhHximmey8szoyfwMw&__req=r&fb_dtsg=AQFxWUaIAwGI&ttstamp=265817012087859773651197173&__rev=1565393";
                                    }


                                    if (response1.Contains("Sorry, an error occurred.  Please try again.") || response1.Contains("Something Went Wrong"))
                                    {
                                        string PostUrl = "https://www.facebook.com/ajax/photo_tagging_ajax.php";
                                        if (pid.Contains("&amp;p"))
                                        {
                                            pid = Utils.getBetween("@@"+pid,"@@","&amp");
                                        }
                                        string slset = Utils.getBetween(photourl, "photos/", "/?");
                                        if (string.IsNullOrEmpty(slset))
                                        {
                                            slset = Utils.getBetween(photourl, "set=", "&");
                                        }
                                        string fbid = Utils.getBetween(pagesourceofrandomphotourl, "targetID\":", "},");

                                        string PostData = "cs_ver=" + cs_ver + "&pid=" + pid + "&fbid=" + fbid + "&id=" + id.Replace("&quot;", "") + "&subject=" + subject + "&name=" + Uri.EscapeDataString(name) + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&slsource&slset=" + slset + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&tagging_mode=true&__user=" + _user + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyKAEWCueyp9Esx6iqAdBGeqrWo8pojLDKexm49UJ6K59poW8z8Tzoyfw&__req=p&fb_dtsg=" + fb_dtsg + "&ttstamp=2658171117905365451138410597&__rev=1514665";
                                        PostData = "cs_ver="+cs_ver+"&pid="+pid+"&fbid="+fbid+"&id="+userId+"&subject="+subject+"&name="+Uri.EscapeDataString(name)+"&action="+action+"&source=permalink&qn="+qn+"&position="+position+"&slsource&slset="+slset+"&x="+x+"&y="+y+"&from_facebox=true&tagging_mode=true&__user="+userId+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxCbzElx2ubhHximmey8szoyfwMw&__req=r&fb_dtsg="+fb_dtsg+"&ttstamp=265817012087859773651197173&__rev=1565393";
                                        System.Threading.Thread.Sleep(1000);
                                        response1 = HttpHelper.postFormData(new Uri(PostUrl), PostData);
                                    }


                                    //GoRound1:
                                    if (response1.Contains("\"errorSummary\":\"Something Went Wrong\",\"errorDescription\":\"Sorry, an error occurred.  Please try again"))
                                    {
                                        try
                                        {
                                            string Fbid = Utils.getBetween(pagesourceofrandomphotourl, "fbid=", "&");
                                            string PostD = "cs_ver=" + cs_ver + "&pid=" + pid + "&fbid=" + Fbid + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&slsource&slset=a.447316393454.234124.119317973454&x=" + x + "&y=" + y + "&from_facebox=false&tagging_mode=true&__user=" + _user + "&__a=1&__dyn=7n8apij2qm9udDgDxyG8EipEtCxO4pbGAdGGzQAjFDy8gBw&__req=l&fb_dtsg=" + fb_dtsg + "&ttstamp=265816584106517157&__rev=1175958";
                                            response1 = HttpHelper.postFormData(new Uri(posturl1), PostD);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        } 
                                    }

                                    if (response1.Contains("\"errorSummary\":\"Something Went Wrong\",\"errorDescription\":\"Sorry, an error occurred.  Please try again"))
                                    {
                                        try
                                        {
                                            name = Uri.EscapeUriString(name);
                                            postdata = "cs_ver =" + cs_ver + "&pid=" + pid + "&fbid=" + id + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&slsource&slset=a.1435859579972781.1073741827.100006462599174&x=48.888888888888886&y=64.88888888888889&from_facebox=false&tagging_mode=true&__user=" + _user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=l&fb_dtsg=" + fb_dtsg + "&__rev=" + _rev + "&ttstamp=26581665310810249100";
                                            response1 = HttpHelper.postFormData(new Uri(posturl1), postdata);                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    ///Delay For 1 second                                  

                                    if (response1.Contains("errorDescription"))
                                    {
                                        if (response1.Contains("Already Tagged"))
                                        {
                                            GlobusLogHelper.log.Info("-) PhotoId:" + position + " Already Tagged with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                            GlobusLogHelper.log.Debug("-) PhotoId:" + position + " TAlready Tagged with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                            counterFriendName = counterFriendName - 1;
                                           // counterFriend = counterFriend - 1;
                                        }
                                        else
                                        {
                                            string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                            summaryArr = Regex.Split(summaryArr[1], "\"");
                                            string errorSummery = summaryArr[1];
                                            string errorDiscription = summaryArr[5];

                                            if (response1.Contains("Something went wrong. We're working on getting it fixed as soon as we can.") || response1.Contains("fbPhotosPhotoTagboxes"))
                                            {
                                                name = name.Replace("%20", " ");
                                                GlobusLogHelper.log.Info("-) PhotoId:" + position + " Tag with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                                GlobusLogHelper.log.Debug("-) PhotoId:" + position + " Tag with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);

                                            }
                                            try
                                            {
                                                GlobusLogHelper.log.Info(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);
                                                GlobusLogHelper.log.Debug(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);

                                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
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
                                        try
                                        {
                                            string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                            string CSV_Content = fbuser.username + "," + userId + ", " + name + "," + position;
                                            //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                            try
                                            {
                                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds ");
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds ");
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


                                        TotalNoOfPhotoTaged_Counnter++;

                                        GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                        GlobusLogHelper.log.Debug(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);

                                        counter++;
                                    }
                                }
                                else
                                {

                                    GlobusLogHelper.log.Info(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);
                                    GlobusLogHelper.log.Debug(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);
                                    GlobusLogHelper.log.Info("Please wait .. ");

                                    if (CheckLikePage)
                                    {
                                        LikePageWIthPhotoTaging(ref fbuser, photourl);
                                        CheckLikePage = false;
                                        counterFriendName=counterFriendName-1;
                                        counterFriend = counterFriend-1;
                                    }
                                    

                               
                                  //  counter++;

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
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
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);
                    GlobusLogHelper.log.Debug("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);


                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                    Thread.Sleep(delayInSeconds);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        }


        public void LikePageWIthPhotoTaging(ref FacebookUser fbUser, string lstFanPageUrlsFanPageLiker)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                #region Post variable

                string fbpage_id = string.Empty;
                string fb_dtsg = string.Empty;
                string __user = string.Empty;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string xhpc_composerid12 = string.Empty;
                int NoOfEmailAccount = 20;
                int countPost = 0;

                #endregion

               // List<string> FanpageUrls = lstFanPageUrlsFanPageLiker;

              // foreach (string item in FanpageUrls)
                {
                    try
                    {
                        string FanpageUrl = lstFanPageUrlsFanPageLiker;

                        GlobusLogHelper.log.Info("Started Liking Page " + FanpageUrl + "  with " + fbUser.username);
                        GlobusLogHelper.log.Debug("Started Liking Page " + FanpageUrl + "  with " + fbUser.username);

                        string PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanpageUrl));

                        //PostOnFanPageCommentWithURLAndItsImage(ref HttpHelper);
                        ///JS, CSS, Image Requests
                        //RequestsJSCSSIMG.RequestJSCSSIMG(PageSrcFanPageUrl, ref HttpHelper);

                        #region Extra Requests Before Like
                        string aed = string.Empty;

                        if (PageSrcFanPageUrl.Contains("aed="))
                        {
                            try
                            {
                                string strfb_dtsg = PageSrcFanPageUrl.Substring(PageSrcFanPageUrl.IndexOf("aed="), 1500);
                                string[] Arrfb_dtsg = strfb_dtsg.Split('"');
                                aed = Arrfb_dtsg[0];
                                aed = aed.Replace("\\", "");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }

                        string strUrlAed = FBGlobals.Instance.PageManagerFanPageLikerstrUrlAed + aed;

                        try
                        {
                            string PageSrcFanPageUrlAed = HttpHelper.getHtmlfromUrl(new Uri(strUrlAed));
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        ///JS, CSS, Image Requests
                        //RequestsJSCSSIMG.RequestJSCSSIMG(PageSrcFanPageUrlAed, ref HttpHelper);

                        #endregion

                        GlobusLogHelper.log.Info("Liking : " + FanpageUrl + " With UserName : " + fbUser.username);



                        Thread.Sleep(Utils.GenerateRandom(300, 1200));

                        #region Post Data Params

                        __user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");     //pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
                        if (string.IsNullOrEmpty(__user))
                        {
                            __user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
                        }

                        if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
                        {
                            GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                            return;
                        }

                        try
                        {
                            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcFanPageUrl);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error: " + ex.StackTrace);

                        }

                        #endregion

                        #region Get FB Page ID Modified

                        ///Get FB Page ID
                        fbpage_id = GlobusHttpHelper.GetPageID(PageSrcFanPageUrl, ref FanpageUrl);


                        #endregion


                        //#region Modified 7-6-12

                        string postURL_1st = FBGlobals.Instance.PageManagerFanPageLikerpostURL1st;
                        string postData_1st = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&nctr[_mod]=pagelet_timeline_page_actions&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" + Utils.GenerateTimeStamp() + "";

                        string res_post_1st = string.Empty;

                        try
                        {
                            res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        if (string.IsNullOrEmpty(res_post_1st))
                        {
                            try
                            {

                                //fbpage_id=107539342664071&add=true&reload=false&fan_origin=page_timeline&fan_source=&cat=&nctr[_mod]=pagelet_timeline_page_actions&__user=100001330463773&__a=1&__dyn=798ahxoNoBKfEa0&__req=o&fb_dtsg=AQDXABnH&phstamp=165816888656611072206
                                string postData_2 = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&fan_source=&cat=&nctr[_mod]=pagelet_timeline_page_actions&__user=" + __user + "&__a=1&__dyn=&__req=o&fb_dtsg=" + fb_dtsg + "" + "&phstamp=" + Utils.GenerateTimeStamp() + "";

                                res_post_1st = HttpHelper.postFormData(new Uri(FBGlobals.Instance.PageManagerFanPageLikerFanStatus), postData_2);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Liking Error: " + ex.StackTrace);
                               
                            }
                        }
                        else if (res_post_1st.Contains("\"errorSummary\""))
                        {
                            try
                            {
                                string summary = GlobusHttpHelper.ParseJson(res_post_1st, "errorSummary");
                                string errorDescription = GlobusHttpHelper.ParseJson(res_post_1st, "errorDescription");

                                GlobusLogHelper.log.Info("Liking Error: " + summary + " | Error Description: " + errorDescription);
                                GlobusLogHelper.log.Debug("Liking Error: " + summary + " | Error Description: " + errorDescription);

                                if (errorDescription.Contains("Please log in to continue"))
                                {
                                    string content = fbUser.username + ":" + fbUser.password;
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error: " + ex.StackTrace);

                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error: " + ex.StackTrace);
                    }                    
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error: " + ex.StackTrace);

            }           
        }


        private void PhotoTaging(ref FacebookUser fbUser,string photourl, string userId)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string cachedAlbum = "";
                string cs_ver = "";
                string pid = "";
                string id = "";
                string subject = "";
                string name = "";
                string action = "";
                string source = "";
                string qn = "";
                string position = "";
                string x = "";
                string y = "";
                string from_facebox = "";
                string fb_dtsg = "";
                string _user = "";
                string phstamp = "";

                string pagesourceofrandomphotourl = HttpHelper.getHtmlfromUrl(new Uri(photourl));
                Dictionary<string, string> DfriendIdName = new Dictionary<string, string>();

                if (pagesourceofrandomphotourl.Contains("http://profile.ak.fbcdn.net/hprofile-ak-snc4"))
                {
                    string[] friendIdNameArr = Regex.Split(pagesourceofrandomphotourl, "http://profile.ak.fbcdn.net/hprofile-ak-snc4");
                    for (int i = 2; i < friendIdNameArr.Length; i++)
                    {
                        try
                        {
                            if (friendIdNameArr[i].Contains("</span>"))
                            {
                                if (friendIdNameArr[i].Contains("<span class=\"blueName\">"))
                                {
                                    string[] friendIdArr = Regex.Split(friendIdNameArr[i], "_");
                                    string friendid = friendIdArr[1];
                                    string[] friendNameArr = Regex.Split(friendIdNameArr[i], "<span");
                                    string friendname = friendNameArr[1].Substring(friendNameArr[1].IndexOf(">"), friendNameArr[1].IndexOf("</span>") - friendNameArr[1].IndexOf(">")).Replace(">", string.Empty).Replace("&#039;", string.Empty);
                                    
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(friendid) && !string.IsNullOrEmpty(friendname) && !string.IsNullOrWhiteSpace(friendname))
                                        {
                                            DfriendIdName.Add(friendid, friendname);
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
                if (DfriendIdName.Count > 0)
                {
                    if (NoOfTaggingFriendsPhotoTagging == 1)
                    {

                        KeyValuePair<string, string> randonfriendIdName = DfriendIdName.ElementAt(new Random().Next(0, DfriendIdName.Count - 1));
                        string randomfriendId = randonfriendIdName.Key;
                        string randomfriendName = randonfriendIdName.Value;

                        cachedAlbum = "-1";
                        cs_ver = "0";
                        string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                        pidArr = Regex.Split(pidArr[2], "\"");

                        pid = pidArr[1].Substring(0, pidArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                        idArr = Regex.Split(idArr[1], "\"");

                        id = idArr[1].Substring(0, idArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        subject = randomfriendId;
                        name = randomfriendName;
                        action = "add";
                        source = "permalink";

                        string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");
                        qnArr = Regex.Split(qnArr[1], "\"");
                        qn = qnArr[1].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty);
                        string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");

                        positionArr = Regex.Split(positionArr[1], "\"");
                        position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        x = new Random().Next(53, 57).ToString();

                        x = x + ".18518518518518";
                        y = new Random().Next(73, 77).ToString();
                        y = y + ".55555555555556";
                        from_facebox = "false";

                        string[] fb_dtsgArr = Regex.Split(pagesourceofrandomphotourl, "fb_dtsg");
                        fb_dtsgArr = Regex.Split(fb_dtsgArr[1], "\"");
                        fb_dtsg = fb_dtsgArr[2].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        _user = userId;
                        if (pagesourceofrandomphotourl.Contains("fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay"))
                        {
                            string postdata = " cachedAlbum=" + cachedAlbum + "&photo=" + position + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                            string posturl = FBGlobals.Instance.PhotoTaggingPostAjaxTagsAlbumUrl;
                            string response = HttpHelper.postFormData(new Uri(posturl), postdata);

                            string postdata1 = "cs_ver=" + cs_ver + "&pid=" + pid + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                            string posturl1 = FBGlobals.Instance.PhotoTaggingPostAjaxPhotoTaggingAjaxUrl;
                            string response1 = HttpHelper.postFormData(new Uri(posturl1), postdata1);

                            if (response1.Contains("errorDescription"))
                            {
                                string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                summaryArr = Regex.Split(summaryArr[1], "\"");
                                string errorSummery = summaryArr[1];
                                string errorDiscription = summaryArr[5];

                                GlobusLogHelper.log.Info("PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

                                try
                                {

                                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                    string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                    string CSV_Content = fbUser.username + "," + userId + ", " + name + "," + position;
                                    
                                    //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                
                                TotalNoOfPhotoTaged_Counnter++;

                                GlobusLogHelper.log.Info("PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);
                                GlobusLogHelper.log.Debug("PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);

                                try
                                {

                                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                            GlobusLogHelper.log.Info("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                            GlobusLogHelper.log.Debug("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);

                            try
                            {

                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                        int counter = 1;
                        if (NoOfTaggingFriendsPhotoTagging > DfriendIdName.Count)
                        {
                            NoOfTaggingFriendsPhotoTagging = DfriendIdName.Count;
                        }

                        GlobusLogHelper.log.Info("No of tagging friends : " + NoOfTaggingFriendsPhotoTagging);

                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging; i++)
                        {
                            try
                            {
                                ///for tagging All friends (Not only Starting Friends)

                                if (DfriendIdName.Count <= counterFriend)
                                {
                                    counterFriend = 0;
                                }


                                KeyValuePair<string, string> randonfriendIdName = DfriendIdName.ElementAt(counterFriend); 

                                counterFriend++;

                                string randomfriendId = randonfriendIdName.Key;
                                string randomfriendName = randonfriendIdName.Value;

                                cachedAlbum = "-1";
                                cs_ver = "0";
                                string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                                pidArr = Regex.Split(pidArr[2], "\"");

                                pid = pidArr[1].Substring(0, pidArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                                idArr = Regex.Split(idArr[1], "\"");
                                id = idArr[1].Substring(0, idArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);

                                subject = randomfriendId;
                                name = randomfriendName;
                                action = "add";
                                source = "permalink";

                                string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");
                                qnArr = Regex.Split(qnArr[1], "\"");
                                qn = qnArr[1].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty);
                                string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");
                                positionArr = Regex.Split(positionArr[1], "\"");
                                position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);

                                x = new Random().Next(53, 57).ToString();
                                x = x + ".18518518518518";
                                y = new Random().Next(73, 77).ToString();
                                y = y + ".55555555555556";
                                from_facebox = "false";

                                string[] fb_dtsgArr = Regex.Split(pagesourceofrandomphotourl, "fb_dtsg");
                                fb_dtsgArr = Regex.Split(fb_dtsgArr[1], "\"");
                                fb_dtsg = fb_dtsgArr[2].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                _user = userId;

                                if (pagesourceofrandomphotourl.Contains("fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay"))
                                {
                                    string postdata = " cachedAlbum=" + cachedAlbum + "&photo=" + position + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                                    
                                    string posturl = FBGlobals.Instance.PhotoTaggingPostAjaxTagsAlbumUrl;

                                    string response = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    string postdata1 = "cs_ver=" + cs_ver + "&pid=" + pid + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                                    
                                    string posturl1 = FBGlobals.Instance.PhotoTaggingPostAjaxPhotoTaggingAjaxUrl;

                                    string response1 = HttpHelper.postFormData(new Uri(posturl1), postdata1);
                                    if (response1.Contains("errorDescription"))
                                    {
                                        string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                        summaryArr = Regex.Split(summaryArr[1], "\"");
                                        string errorSummery = summaryArr[1];
                                        string errorDiscription = summaryArr[5];

                                        GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                                        
                                        counter++;

                                        try
                                        {

                                            int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                            string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                            string CSV_Content = fbUser.username + "," + userId + ", " + name + "," + position;
                                            
                                            //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                            try
                                            {

                                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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


                                        TotalNoOfPhotoTaged_Counnter++;

                                        GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);
                                        GlobusLogHelper.log.Debug(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);
                                        
                                        counter++;

                                        try
                                        {

                                            int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                    GlobusLogHelper.log.Info(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                                    GlobusLogHelper.log.Debug(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                                    
                                    counter++;

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                    GlobusLogHelper.log.Debug("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);

                    try
                    {

                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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



        private void PostTagingToYourFriends(ref FacebookUser fbuser, string photourl, string userId, ref List<string> lstfriendsId, ref List<string> lstFriendname, ref List<string> lstFriend)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbuser.globusHttpHelper;

                string cachedAlbum = "";
                string cs_ver = "";
                string pid = "";
                string id = "";
                string subject = "";
                string name = "";
                string action = "";
                string source = "";
                string qn = "";
                string position = "";
                string x = "";
                string y = "";
                string from_facebox = "";
                string fb_dtsg = "";
                string _user = "";
                string phstamp = "";
                string client_id = string.Empty;
                string ftId = string.Empty;
                string pagesourceofrandomphotourl = HttpHelper.getHtmlfromUrl(new Uri(photourl));
                string RefUrl=photourl;
                try
                {
                    client_id = Utils.getBetween(pagesourceofrandomphotourl, "clientid\":", "}]");
                    client_id = client_id.Replace("\"", string.Empty).Replace(":", "%3A");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {

                    ftId = Utils.getBetween(pagesourceofrandomphotourl, "clientid\":", "}]");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                string ft_ent_identifier = Utils.getBetween(pagesourceofrandomphotourl, "ftentidentifier", "instanceid").Replace("\":\"", string.Empty).Replace("\",\"", string.Empty);
                List<string> NameUserId = new List<string>();
                if (lstFriendname.Count == lstFriend.Count)
                {

                    {
                        int counter = 1;

                        GlobusLogHelper.log.Info("Number of tagging friends : " + NoOfTaggingFriendsPhotoTagging);
                        GlobusLogHelper.log.Debug("Number of tagging friends : " + NoOfTaggingFriendsPhotoTagging);

                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging; i++)
                        {
                            try
                            {
                                ///for tagging All friends (Not only Starting Friends)

                                if (lstfriendsId.Count <= counterFriend)
                                {
                                    counterFriendName = 0;
                                    counterFriend = 0;
                                }

                                string randomfriendId = lstfriendsId[counterFriend];
                                string randomfriendName = string.Empty;

                                counterFriendName++;
                                counterFriend++;

                                try
                                {
                                    string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + randomfriendId));
                                    if (strFriendInfo.Contains("\"name\":"))
                                    {
                                        string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                        randomfriendName = strName;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                if (string.IsNullOrEmpty(randomfriendName))
                                {
                                    continue;
                                }

                                cachedAlbum = "-1";
                                cs_ver = "0";
                                string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                                if (pidArr.Count() > 2)
                                {
                                    try
                                    {
                                        pidArr = Regex.Split(pidArr[2], "\"");
                                        foreach (string item in pidArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 3)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        try
                                                        {
                                                            pid = item.Substring(0, item.IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("&quot;",string.Empty).Trim();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                        }

                                                        break;
                                                    }
                                                    else
                                                    {
                                                        pid = item.Replace("\"", string.Empty).Trim();
                                                        pid = pid.Replace("=", "");
                                                        break;
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

                                string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                                if (idArr.Count() > 1)
                                {
                                    try
                                    {
                                        idArr = Regex.Split(idArr[1], "\"");
                                        if (idArr.Count() > 1)
                                        {
                                            foreach (string item in idArr)
                                            {

                                                try
                                                {
                                                    if (item.Length > 6)
                                                    {
                                                        if (item.Contains(","))
                                                        {
                                                            try
                                                            {
                                                                id = item.Substring(0, item.IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Trim();
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            id = item.Replace("\"", string.Empty).Trim();
                                                            break;
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
                                subject = randomfriendId;
                                name = randomfriendName;
                                string sss = name + "@@@" + randomfriendId;
                                NameUserId.Add(sss);
                                action = "add";
                                source = "permalink";
                                photourl = photourl + "$$$$";


                                string parent_fbid = Utils.getBetween(photourl, "posts/", "$$$$");



                                string _rev = Utils.getBetween(pagesourceofrandomphotourl, "_rev", ",\"");
                                _rev = _rev.Replace(":", string.Empty).Replace("\"", string.Empty);

                                string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");

                                if (qnArr.Count() > 1)
                                {
                                    try
                                    {
                                        qnArr = Regex.Split(qnArr[1], "\"");
                                        foreach (string item in qnArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 3)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        qn = item.Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty).Trim();
                                                        qn = qn.Replace("[", string.Empty).Replace("]", string.Empty);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        qn = item.Replace("\"", string.Empty).Trim();
                                                        break;
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

                                string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");
                                if (positionArr.Count() > 1)
                                {
                                    try
                                    {
                                        positionArr = Regex.Split(positionArr[1], "\"");
                                        foreach (string item in positionArr)
                                        {
                                            try
                                            {
                                                if (item.Length > 6)
                                                {
                                                    if (item.Contains(","))
                                                    {
                                                        position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        position = item.Replace("\"", string.Empty).Trim();
                                                        break;
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
                                    pid = pid.Replace("=", "");

                                    x = new Random().Next(53, 57).ToString();
                                    x = x + ".18518518518518";
                                    y = new Random().Next(73, 77).ToString();
                                    y = y + ".55555555555556";
                                    from_facebox = "false";

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pagesourceofrandomphotourl);
                                _user = userId;
                                string comment_text=string.Empty;
                                int countId = 0;
                                if (pagesourceofrandomphotourl.Contains(""))   //  fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay
                                {
                                    int Count = 0;
                                    foreach (var lstFriendname_item in lstFriendname)
                                    {
                                        try
                                        {


                                            string ss="https://www.facebook.com/ajax/typeahead/record_basic_metrics.php";
                                            string p = HttpHelper.getHtmlfromUrl(new Uri(ss));


                                            string Friendid = lstFriend[countId];
                                            string Url = "https://www.facebook.com/ajax/typeahead/search.php?value=" + Uri.EscapeDataString(lstFriendname_item) + "%EF%BB%BF&context=topics_limited_autosuggest&viewer=" + _user + "&filter[0]=page&filter[1]=user&rsp=mentions&post_fbid=" + parent_fbid + "&sid=302121431661&request_id=40865d8c-33cd-419b-d54e-06d9d9a265aa&__user=" + _user + "&__a=1&__dyn=aJj2BW8BgCByizpQ9VUgHFaiV4DBzECQqbx2mbAClaGAiWUNFLOa8xmm4QLGjAKGDh9UGmWhF6nUGQSiZ3oyq4-qq8w&__req=15&__rev=1491267";
                                            string PageSource = HttpHelper.getHtmlfromUrl(new Uri(Url));
                                            string comment = Friendid + " " + lstFriendname_item;
                                            comment_text = comment_text + Uri.EscapeDataString(comment);
                                            countId = countId + 1;

                                          

                                            string selected_score = Utils.getBetween(PageSource, "\"score\":", "}]");
                                            if (selected_score.Contains("},{"))
                                            {
                                                string[] arr = System.Text.RegularExpressions.Regex.Split(selected_score,"}");  
                                                selected_score=arr[0];
                                            }
                                            string stats_request_id = Utils.getBetween(PageSource, "oh=", "=");
                                            string stats_selected_query = Utils.getBetween(PageSource, "oh=", "=");


                                            try
                                            {
                                                string urltest = "stats[num_queries]=1&stats[selected_id]=" + Friendid + "&stats[selected_type]=user&stats[selected_score]=" + selected_score + "&stats[selected_position]=0&stats[selected_with_mouse]=1&stats[selected_query]=&stats[event_name]=mentions&stats[from_location]=comments&stats[candidate_results]=[%22" + Friendid + "]&stats[query]=" + Uri.EscapeDataString(lstFriendname_item) + "&stats[last_not_backspaced_query]=" + Uri.EscapeDataString(lstFriendname_item) + "&stats[queries_history]=[%22" + Uri.EscapeDataString(lstFriendname_item) + "%22]&stats[normalized_backend_query]=" + Uri.EscapeDataString(lstFriendname_item) + "&stats[request_id]=" + stats_request_id + "&stats[request_ids][0]=" + stats_request_id + "&stats[sid]=302121431661&stats[bootstrapped]=1&__user=" + _user + "&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqAdBGeqrWo8pojLyUW5ogDyQqUkBBzEy6Kdy8&__req=m&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170111775449829910110785&__rev=1498922";
                                                string res = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/typeahead/record_basic_metrics.php"), urltest);

                                            }
                                            catch { };

                                            string PostDATA = "ft_ent_identifier="+ft_ent_identifier+"&comment_text=%40[" + comment_text + "]&source=2&client_id=1416288959884%3A1595380136&reply_fbid&parent_comment_id&rootid=u_0_u&clp=%7B%22cl_impid%22%3A%22e603e06c%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22js_l%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A686423211456024%7D&attached_sticker_fbid=0&attached_photo_fbid=0&&ft[tn]=[]&ft[fbfeed_location]=5&av=100001006024349&__user=100001006024349&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqAdBGeqrWo8pojLyUW5ogDyQqUkBBzEy6Kdy8&__req=q&fb_dtsg=AQHl7AoAsabQ&ttstamp=2658172108556511165115979881&__rev=1498265";
                                            string PostURL = "https://www.facebook.com/ajax/ufi/add_comment.php";
                                            string PageResponce = HttpHelper.postFormData(new Uri(PostURL), PostDATA);

                                            string data = "stats[num_queries]=3&stats[selected_id]=" + Friendid + "&stats[selected_type]=user&stats[selected_position]=1&stats[selected_with_mouse]=1&stats[selected_query]=aa&stats[event_name]=mentions&stats[from_location]=comments&stats[candidate_results]=[%22100001505100228%22%2C%22100002196594155%22%2C%22304477473000893%22%2C%22512955058720006%22%2C%22325699947521145%22]&stats[query]=" + lstFriendname_item + "%EF%BB%BF&stats[last_not_backspaced_query]=" + lstFriendname_item + "%EF%BB%BF&stats[queries_history]=[%22a%22%2C%22aa%22%2C%22" + lstFriendname_item + "%EF%BB%BF%22]&stats[request_id]=b2d55def-7af2-48c7-eca3-03b1bd019569&stats[request_ids][0]=b2d55def-7af2-48c7-eca3-03b1bd019569&stats[request_ids][1]=b2d55def-7af2-48c7-eca3-03b1bd019569&stats[request_ids][2]=b2d55def-7af2-48c7-eca3-03b1bd019569&stats[sid]=295260874427&stats[bootstrapped]=1&__user=" + _user + "&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqAdBGeqrWo8pojLyUW5ogDyQqUkBBzEy6Kdy8&__req=p&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711078410811249757179111&__rev=1494788";
                                            string Url0 = "https://www.facebook.com/ajax/typeahead/record_basic_metrics.php";

                                            string response1e = HttpHelper.postFormData(new Uri(Url0), data, "https://www.facebook.com/ever.gusman.3/posts/796243103752550");
                                        }
                                        catch { };
                                    }


                                    //string FinalPostData = "ft_ent_identifier=" + ft_ent_identifier + "&comment_text=%40[100001702530551%3AAnoop%20Jaiswal]%20%40[100007970527788%3AAjay%20Jaat]%20%40[100004182270337%3ASonam%20Tiwari]&source=2&client_id=1415808548544%3A107689017&reply_fbid&parent_comment_id&rootid=u_jsonp_5_c&clp=%7B%22cl_impid%22%3A%22e374686f%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22js_1a%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + parent_fbid + "%7D&attached_sticker_fbid=0&attached_photo_fbid=0&&ft[tn]=[]&ft[fbfeed_location]=5&av=" + _user + "&__user=" + _user + "&__a=1&__dyn=aJj2BW8BgCByizpQ9VUgHFaiV4DBzECQqbx2mbAClaGAiWUNFLOa8xmm4QLGjAKGDh9UGmWhF6nUGQSiZ3oyq4-qq8w&__req=1c&fb_dtsg=" + fb_dtsg + "&ttstamp=2658172785053103108738277115&__rev=1491267";

                                    string FinalPostData = "ft_ent_identifier=" + ft_ent_identifier + "&comment_text=%40[" + comment_text + "]&source=2&client_id=1415808548544%3A107689017&reply_fbid&parent_comment_id&rootid=u_jsonp_5_c&clp=%7B%22cl_impid%22%3A%22e374686f%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22js_1a%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + parent_fbid + "%7D&attached_sticker_fbid=0&attached_photo_fbid=0&&ft[tn]=[]&ft[fbfeed_location]=5&av=" + _user + "&__user=" + _user + "&__a=1&__dyn=aJj2BW8BgCByizpQ9VUgHFaiV4DBzECQqbx2mbAClaGAiWUNFLOa8xmm4QLGjAKGDh9UGmWhF6nUGQSiZ3oyq4-qq8w&__req=1c&fb_dtsg=" + fb_dtsg + "&ttstamp=2658172785053103108738277115&__rev=1491267";
                                    string PostUrlG = "https://www.facebook.com/ajax/ufi/add_comment.php";

                                    string response1 = HttpHelper.postFormData(new Uri(PostUrlG), FinalPostData, "https://www.facebook.com/ever.gusman.3/posts/796243103752550");

      
                                    ///Delay For 1 second                                  

                                    if (response1.Contains("errorDescription"))
                                    {
                                        string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                        summaryArr = Regex.Split(summaryArr[1], "\"");
                                        string errorSummery = summaryArr[1];
                                        string errorDiscription = summaryArr[5];

                                        if (response1.Contains("Something went wrong. We're working on getting it fixed as soon as we can.") || response1.Contains("fbPhotosPhotoTagboxes"))
                                        {
                                            name = name.Replace("%20", " ");
                                            GlobusLogHelper.log.Info("-) PostId:" + position + " Tag with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                            GlobusLogHelper.log.Debug("-) PostId:" + position + " Tag with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);

                                        }
                                        //GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Error Summery : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbuser.username);
                                        //GlobusLogHelper.log.Debug(counter + ") PhotoId:" + position + " Error Summery : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbuser.username);

                                        // counter++;

                                        try
                                        {
                                            GlobusLogHelper.log.Info(counter + ") There is no option for PostTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);
                                            GlobusLogHelper.log.Debug(counter + ") There is no option for PostTag where PhotoUrl is : " + photourl + " UserName: " + fbuser.username);

                                            int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
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
                                            string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                            string CSV_Content = fbuser.username + "," + userId + ", " + name + "," + position;
                                            //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                            try
                                            {

                                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds ");
                                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds ");
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


                                        TotalNoOfPhotoTaged_Counnter++;

                                        GlobusLogHelper.log.Info(counter + ") PostID:" + position + " Tagging Completed with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);
                                        GlobusLogHelper.log.Debug(counter + ") PostID:" + position + " Tagging Completed with UserName : " + fbuser.username + "(" + userId + ")" + " And Friend name: " + name);

                                        counter++;
                                    }
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(counter + ") There is no option for PostID where PostUrl is : " + photourl + " UserName: " + fbuser.username);
                                    GlobusLogHelper.log.Debug(counter + ") There is no option for PostID where PostUrl is : " + photourl + " UserName: " + fbuser.username);

                                    //  counter++;

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                                        GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                                        GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
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
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("There is no option for PostUrl where PostUrl is : " + photourl + " UserName: " + fbuser.username);
                    GlobusLogHelper.log.Debug("There is no option for PostUrl where PostUrl is : " + photourl + " UserName: " + fbuser.username);


                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbuser.username);
                    Thread.Sleep(delayInSeconds);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddPostTag(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string ProFilePost = string.Empty;
                string __user = string.Empty;

                string userId = "";
                List<string> lstphotourl = new List<string>();
                try
                {
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

                    GlobusLogHelper.log.Info("Please wait finding the friends ID...");
                    GlobusLogHelper.log.Debug("Please wait finding the friends ID...");
                    List<string> lstfriendsId = FBUtils.GetAllFriends(ref HttpHelper, UserId);

                    lstfriendsId.Remove("");
                    lstfriendsId = lstfriendsId.Distinct().ToList();


                    string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UserId));

                    string findTheAllFrnList = FBUtils.getBetween(pageSource, "pagelet_timeline_medley_friends", "</span>");
                    findTheAllFrnList = findTheAllFrnList.Replace("\">Friends<span class=\"_gs6\">", string.Empty).Replace(",", string.Empty);
                    int countFrnd = Convert.ToInt32(findTheAllFrnList);

                    GlobusLogHelper.log.Info("Total Friends : " + countFrnd + " With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Total Friends : " + countFrnd + " With Username : " + fbUser.username);


                    GlobusLogHelper.log.Info("Process Start Of Post Tagging  With Username >>> " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Start Of Post Tagging  With Username >>> " + fbUser.username);

                    List<string> lstFriendname = new List<string>();
                    List<string> lstFriend = new List<string>();

                    if (LstPhotoTaggingURLsPhotoTagging.Count > 0)
                    {
                        if (NoOfTaggingFriendsPhotoTagging > lstfriendsId.Count)
                        {
                            NoOfTaggingFriendsPhotoTagging = lstfriendsId.Count;
                        }
                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging; i++)
                        {
                            try
                            {
                                string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstfriendsId[i]));
                                if (strFriendInfo.Contains("\"name\":"))
                                {
                                    try
                                    {
                                        string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                        lstFriendname.Add(strName);
                                        lstFriend.Add(lstfriendsId[i]);
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
                        for (int i = 0; i < LstPhotoTaggingURLsPhotoTagging.Count; i++)
                        {
                            try
                            {

                                string photoUrl = LstPhotoTaggingURLsPhotoTagging[i];
                                PostTagingToYourFriends(ref fbUser, photoUrl, UserId, ref lstfriendsId, ref lstFriendname, ref lstFriend);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        string profilepagesource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + userId));
                        if (profilepagesource.Contains("pagelet_timeline_photos_nav_top"))
                        {
                            string photourl = profilepagesource.Substring(profilepagesource.IndexOf("pagelet_timeline_photos_nav_top", 200));
                            if (photourl.Contains("href"))
                            {
                                photourl = photourl.Substring(photourl.IndexOf("href="), photourl.IndexOf("\"", photourl.IndexOf("href=") + 10) - photourl.IndexOf("href=")).Replace("href=", string.Empty).Replace("\"", string.Empty);
                                string photopagesource = HttpHelper.getHtmlfromUrl(new Uri(photourl));
                                if (photopagesource.Contains("fbTimelineSection mtm fbTimelinePhotosAndVideosOfMe fbTimelineTabSection"))
                                {
                                    string photohref = photopagesource.Substring(photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent"), photopagesource.IndexOf("</tbody>", photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent")) - photopagesource.IndexOf("fbTimelinePhotosAndVideosOfMeContent"));
                                    if (photohref.Contains("href="))
                                    {
                                        string[] photohrefArr = Regex.Split(photohref, "href=");
                                        for (int i = 1; i < photohrefArr.Length; i++)
                                        {
                                            try
                                            {
                                                string[] photohrefArr1 = Regex.Split(photohrefArr[i], "\"");
                                                string photohref1 = photohrefArr1[1].Replace("amp;", string.Empty);
                                                lstphotourl.Add(photohref1);
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
                        try
                        {
                            string randomphotourl = lstphotourl[new Random().Next(0, lstphotourl.Count - 1)];

                            PostTaging(ref fbUser, randomphotourl, userId);
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Post Tagging  With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Post Tagging  With Username >>> " + fbUser.username);

        }
        private void PostTaging(ref FacebookUser fbUser, string photourl, string userId)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string cachedAlbum = "";
                string cs_ver = "";
                string pid = "";
                string id = "";
                string subject = "";
                string name = "";
                string action = "";
                string source = "";
                string qn = "";
                string position = "";
                string x = "";
                string y = "";
                string from_facebox = "";
                string fb_dtsg = "";
                string _user = "";
                string phstamp = "";

                string pagesourceofrandomphotourl = HttpHelper.getHtmlfromUrl(new Uri(photourl));
                Dictionary<string, string> DfriendIdName = new Dictionary<string, string>();

                if (pagesourceofrandomphotourl.Contains("http://profile.ak.fbcdn.net/hprofile-ak-snc4"))
                {
                    string[] friendIdNameArr = Regex.Split(pagesourceofrandomphotourl, "http://profile.ak.fbcdn.net/hprofile-ak-snc4");
                    for (int i = 2; i < friendIdNameArr.Length; i++)
                    {
                        try
                        {
                            if (friendIdNameArr[i].Contains("</span>"))
                            {
                                if (friendIdNameArr[i].Contains("<span class=\"blueName\">"))
                                {
                                    string[] friendIdArr = Regex.Split(friendIdNameArr[i], "_");
                                    string friendid = friendIdArr[1];
                                    string[] friendNameArr = Regex.Split(friendIdNameArr[i], "<span");
                                    string friendname = friendNameArr[1].Substring(friendNameArr[1].IndexOf(">"), friendNameArr[1].IndexOf("</span>") - friendNameArr[1].IndexOf(">")).Replace(">", string.Empty).Replace("&#039;", string.Empty);

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(friendid) && !string.IsNullOrEmpty(friendname) && !string.IsNullOrWhiteSpace(friendname))
                                        {
                                            DfriendIdName.Add(friendid, friendname);
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
                if (DfriendIdName.Count > 0)
                {
                    if (NoOfTaggingFriendsPhotoTagging == 1)
                    {

                        KeyValuePair<string, string> randonfriendIdName = DfriendIdName.ElementAt(new Random().Next(0, DfriendIdName.Count - 1));
                        string randomfriendId = randonfriendIdName.Key;
                        string randomfriendName = randonfriendIdName.Value;

                        cachedAlbum = "-1";
                        cs_ver = "0";
                        string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                        pidArr = Regex.Split(pidArr[2], "\"");

                        pid = pidArr[1].Substring(0, pidArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                        idArr = Regex.Split(idArr[1], "\"");

                        id = idArr[1].Substring(0, idArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        subject = randomfriendId;
                        name = randomfriendName;
                        action = "add";
                        source = "permalink";

                        string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");
                        qnArr = Regex.Split(qnArr[1], "\"");
                        qn = qnArr[1].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty);
                        string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");

                        positionArr = Regex.Split(positionArr[1], "\"");
                        position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        x = new Random().Next(53, 57).ToString();

                        x = x + ".18518518518518";
                        y = new Random().Next(73, 77).ToString();
                        y = y + ".55555555555556";
                        from_facebox = "false";

                        string[] fb_dtsgArr = Regex.Split(pagesourceofrandomphotourl, "fb_dtsg");
                        fb_dtsgArr = Regex.Split(fb_dtsgArr[1], "\"");
                        fb_dtsg = fb_dtsgArr[2].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                        _user = userId;
                        if (pagesourceofrandomphotourl.Contains("fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay"))
                        {
                            string postdata = " cachedAlbum=" + cachedAlbum + "&photo=" + position + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                            string posturl = FBGlobals.Instance.PhotoTaggingPostAjaxTagsAlbumUrl;
                            string response = HttpHelper.postFormData(new Uri(posturl), postdata);

                            string postdata1 = "cs_ver=" + cs_ver + "&pid=" + pid + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";
                            string posturl1 = FBGlobals.Instance.PhotoTaggingPostAjaxPhotoTaggingAjaxUrl;
                            string response1 = HttpHelper.postFormData(new Uri(posturl1), postdata1);

                            if (response1.Contains("errorDescription"))
                            {
                                string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                summaryArr = Regex.Split(summaryArr[1], "\"");
                                string errorSummery = summaryArr[1];
                                string errorDiscription = summaryArr[5];

                                GlobusLogHelper.log.Info("PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

                                try
                                {

                                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                    string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                    string CSV_Content = fbUser.username + "," + userId + ", " + name + "," + position;

                                    //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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

                                TotalNoOfPhotoTaged_Counnter++;

                                GlobusLogHelper.log.Info("PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);
                                GlobusLogHelper.log.Debug("PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);

                                try
                                {

                                    int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                            GlobusLogHelper.log.Info("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                            GlobusLogHelper.log.Debug("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);

                            try
                            {

                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                        int counter = 1;
                        if (NoOfTaggingFriendsPhotoTagging > DfriendIdName.Count)
                        {
                            NoOfTaggingFriendsPhotoTagging = DfriendIdName.Count;
                        }

                        GlobusLogHelper.log.Info("No of tagging friends : " + NoOfTaggingFriendsPhotoTagging);

                        for (int i = 0; i < NoOfTaggingFriendsPhotoTagging; i++)
                        {
                            try
                            {
                                ///for tagging All friends (Not only Starting Friends)

                                if (DfriendIdName.Count <= counterFriend)
                                {
                                    counterFriend = 0;
                                }


                                KeyValuePair<string, string> randonfriendIdName = DfriendIdName.ElementAt(counterFriend);

                                counterFriend++;

                                string randomfriendId = randonfriendIdName.Key;
                                string randomfriendName = randonfriendIdName.Value;

                                cachedAlbum = "-1";
                                cs_ver = "0";
                                string[] pidArr = Regex.Split(pagesourceofrandomphotourl, "pid");
                                pidArr = Regex.Split(pidArr[2], "\"");

                                pid = pidArr[1].Substring(0, pidArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                string[] idArr = Regex.Split(pagesourceofrandomphotourl, "owner");
                                idArr = Regex.Split(idArr[1], "\"");
                                id = idArr[1].Substring(0, idArr[1].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);

                                subject = randomfriendId;
                                name = randomfriendName;
                                action = "add";
                                source = "permalink";

                                string[] qnArr = Regex.Split(pagesourceofrandomphotourl, "serverTime\":");
                                qnArr = Regex.Split(qnArr[1], "\"");
                                qn = qnArr[1].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty).Replace("0", string.Empty);
                                string[] positionArr = Regex.Split(pagesourceofrandomphotourl, "fbid\":");
                                positionArr = Regex.Split(positionArr[1], "\"");
                                position = positionArr[0].Substring(0, positionArr[0].IndexOf(",") - 0).Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);

                                x = new Random().Next(53, 57).ToString();
                                x = x + ".18518518518518";
                                y = new Random().Next(73, 77).ToString();
                                y = y + ".55555555555556";
                                from_facebox = "false";

                                string[] fb_dtsgArr = Regex.Split(pagesourceofrandomphotourl, "fb_dtsg");
                                fb_dtsgArr = Regex.Split(fb_dtsgArr[1], "\"");
                                fb_dtsg = fb_dtsgArr[2].Replace(":", string.Empty).Replace(",", string.Empty).Replace("}", string.Empty);
                                _user = userId;

                                if (pagesourceofrandomphotourl.Contains("fbPhotosPhotoActionsTag tagButton rfloat uiButton uiButtonOverlay"))
                                {
                                    string postdata = " cachedAlbum=" + cachedAlbum + "&photo=" + position + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";

                                    string posturl = FBGlobals.Instance.PhotoTaggingPostAjaxTagsAlbumUrl;

                                    string response = HttpHelper.postFormData(new Uri(posturl), postdata);
                                    string postdata1 = "cs_ver=" + cs_ver + "&pid=" + pid + "&id=" + id + "&subject=" + subject + "&name=" + name + "&action=" + action + "&source=" + source + "&qn=" + qn + "&position=" + position + "&x=" + x + "&y=" + y + "&from_facebox=" + from_facebox + "&fb_dtsg=" + fb_dtsg + "&__user=" + _user + "&phstamp=";

                                    string posturl1 = FBGlobals.Instance.PhotoTaggingPostAjaxPhotoTaggingAjaxUrl;

                                    string response1 = HttpHelper.postFormData(new Uri(posturl1), postdata1);
                                    if (response1.Contains("errorDescription"))
                                    {
                                        string[] summaryArr = Regex.Split(response1, "errorSummary\":");
                                        summaryArr = Regex.Split(summaryArr[1], "\"");
                                        string errorSummery = summaryArr[1];
                                        string errorDiscription = summaryArr[5];

                                        GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

                                        counter++;

                                        try
                                        {

                                            int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                            string CSVHeader = "User_Name" + "," + "userId" + ", " + "FriendName" + "," + " PhotoId";
                                            string CSV_Content = fbUser.username + "," + userId + ", " + name + "," + position;

                                            //FBApplicationData.ExportDataCSVFile(CSVHeader, CSV_Content, FilePath);

                                            try
                                            {

                                                int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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


                                        TotalNoOfPhotoTaged_Counnter++;

                                        GlobusLogHelper.log.Info(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);
                                        GlobusLogHelper.log.Debug(counter + ") PhotoId:" + position + " Tagging Completed with UserName : " + fbUser.username + "(" + userId + ")" + " And Friend name: " + name);

                                        counter++;

                                        try
                                        {

                                            int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                                    GlobusLogHelper.log.Info(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                                    GlobusLogHelper.log.Debug(counter + ") There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);

                                    counter++;

                                    try
                                    {

                                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);
                    GlobusLogHelper.log.Debug("There is no option for PhotoTag where PhotoUrl is : " + photourl + " UserName: " + fbUser.username);

                    try
                    {

                        int delayInSeconds = Utils.GenerateRandom(minDelayPhotoTagging * 1000, maxDelayPhotoTagging * 1000);
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

        ///DownloadPhoto 
        ///
        #region Global Variables For Photo DownloadPhoto

        readonly object lockrThreadControllerDownloadPhoto = new object();
        public bool isStopDwnloadPhoto = false;
        int countThreadControllerDownloadPhoto = 0;

        public static int TotalNoOfDownloadPhoto_Counnter = 0;

        public static int minDelayDownloadPhoto = 10;
        public static int maxDelayDownloadPhoto = 20;

        public List<Thread> lstThreadsDownloadPhoto = new List<Thread>();

        #endregion

        #region Property For PDownloadPhoto

        public int NoOfThreadsDownloadPhoto
        {
            get;
            set;
        }



        public List<string> LstDownloadPhotoURLsDownloadPhoto
        {
            get;
            set;
        }

        #endregion

        public void StartDownloadPhoto()
        {
            try
            {
                countThreadControllerPhotoTagging = 0;
                countThreadControllerDownloadPhoto = 0;
                int numberOfAccountPatch = 25;

                if (NoOfThreadsDownloadPhoto > 0)
                {
                    numberOfAccountPatch = NoOfThreadsDownloadPhoto;
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
                                lock (lockrThreadControllerPhotoTagging)
                                {
                                    try
                                    {
                                        if (countThreadControllerPhotoTagging >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerPhotoTagging);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsDownloadPhoto);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerPhotoTagging++;
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

        public void StartMultiThreadsDownloadPhoto(object parameters)
        {
            try
            {
                if (!isStopDwnloadPhoto)
                {
                    try
                    {
                        lstThreadsDownloadPhoto.Add(Thread.CurrentThread);
                        lstThreadsDownloadPhoto.Distinct();
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
                                StartActionDownloadPhoto(ref objFacebookUser);
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
                    // if (!isStopPhotoTagging)
                    {
                        lock (lockrThreadControllerDownloadPhoto)
                        {
                            countThreadControllerDownloadPhoto--;
                            if (!isStopDwnloadPhoto)
                            {
                                Monitor.Pulse(lockrThreadControllerDownloadPhoto); 
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

        private void StartActionDownloadPhoto(ref FacebookUser fbUser)
        {
            try
            {
                StartDownlodingPhotoFanPage(ref fbUser);

                GlobusLogHelper.log.Info("Process Completed Of Photo Downloding  With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Process Completed Of Photo Downloding  With Username >>> " + fbUser.username);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartDownlodingPhotoFanPage(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string ProFilePost = string.Empty;
                string __user = string.Empty;
                string ExportPath = ExportPhotosPath;
                if (string.IsNullOrEmpty(ExportPath))
                {
                        GlobusLogHelper.log.Info("Please Select Photo Export Folder Path !! ");
                        GlobusLogHelper.log.Debug("Please Select Photo Export Folder Path !! ");
                        return;
                }
                string userId = "";
                List<string> lstphotourl = new List<string>();
                try
                {
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


                    foreach (var LstDownloadPhotoURLsDownloadPhoto_item in LstDownloadPhotoURLsDownloadPhoto)
                    {
                        try
                        {
                            string PageSourceOfTargetedUrl = string.Empty;
                            string URL = string.Empty;
                            string PageTitle = string.Empty;
                            string PageID = string.Empty;                            
                            if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos_stream"))
                            {
                                URL = LstDownloadPhotoURLsDownloadPhoto_item.Replace("?ref=br_rs", "/photos_stream");
                            }
                            else
                            {
                                URL = LstDownloadPhotoURLsDownloadPhoto_item;
                            }

                            if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos_stream"))
                            {
                                URL = LstDownloadPhotoURLsDownloadPhoto_item+ "/photos_stream";
                            }
                            List<string> PhotoURL = new List<string>();
                            try
                            {
                                if (URL.Contains("?ref=br_rs"))
                                {
                                    URL = URL.Replace("?ref=br_rs","/photos_stream");
                                }
                                PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL));

                                if (string.IsNullOrEmpty(PageSourceOfTargetedUrl))
                                {
                                    URL = URL.Replace("/photos_stream", "?sk=photos_stream");
                                    PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL));
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            string AjaxPipeToken=Utils.getBetween(PageSourceOfTargetedUrl,"ajaxpipe_token\":\"","\"");
                            string scrollLoad = Utils.getBetween(PageSourceOfTargetedUrl, "scroll_load\\\":",",");
                            string LastFBID = Utils.getBetween(PageSourceOfTargetedUrl, "last_fbid\\\":", ",");
                            string FetchSize = Utils.getBetween(PageSourceOfTargetedUrl, "fetch_size\\\":", ",");
                            PageTitle = Utils.getBetween(PageSourceOfTargetedUrl, "pageTitle\">", "</title>");
                            PageID = Utils.getBetween(PageSourceOfTargetedUrl,"pageID\":",",");
                            string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "data-non-starred-src");
                            PhotoId = PhotoId.Skip(1).ToArray();
                            int RandomNumber = 0;
                            foreach (string Photoid in PhotoId)
                            {
                                try
                                {
                                    RandomNumber++;
                                    string temp = Utils.getBetween(Photoid, "=\"", "\"");
                                    string CompletePicUrl = string.Empty;
                                    if (temp.Contains("/p") || temp.Contains("/v/"))
                                    {
                                        try
                                        {
                                           // string[] arr = Regex.Split(temp, "/p");
                                           // string Locatstr = arr[1] + "###";
                                           // CompletePicUrl = arr[0] + "/" + Utils.getBetween(Locatstr, "/", "###");
                                            //Modified by MAhesh 07-01-2014
                                            CompletePicUrl = temp;
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                       // CompletePicUrl = CompletePicUrl.Replace("/v", "");
                                        CompletePicUrl = CompletePicUrl.Replace("amp;",string.Empty);
                                        if (string.IsNullOrEmpty(CompletePicUrl))
                                        {
                                            CompletePicUrl = temp.Replace("/v", "");
                                        }
                                    }
                                    string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
                                    try
                                    {
                                        Utils.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
                                        GlobusLogHelper.log.Info(PicName + " downloaded to " + ExportPhotosPath);
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
                            int Count = 0;
                            while (true)
                            {
                                Count = Count + 1;
                                if (Count >= 20)
                                {
                                    break;
                                }
                                string AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt=2"));
                                PhotoId = Regex.Split(AjaxPageSource, "data-non-starred-src");
                                PhotoId = PhotoId.Skip(1).ToArray();
                                foreach (string Photoid in PhotoId)
                                {
                                    try
                                    {
                                        RandomNumber++;
                                        string temp = Utils.getBetween(Photoid, "=\\\"", "\\\"").Replace("\\", "");
                                        string CompletePicUrl = string.Empty;
                                        if (temp.Contains("/p") || temp.Contains("/v/"))
                                        {
                                            //string[] arr = Regex.Split(temp, "/p");
                                            //string Locatstr = arr[1] + "###";
                                           // CompletePicUrl = arr[0] + "/" + Utils.getBetween(Locatstr, "/", "###");
                                           // CompletePicUrl = temp.Replace("/v", "");
                                            //Modified by MAhesh 07-01-2014
                                            CompletePicUrl = temp.Replace("amp;",string.Empty);
                                            if (string.IsNullOrEmpty(CompletePicUrl))
                                            {
                                                CompletePicUrl = temp.Replace("/v", "");
                                            }
                                        }
                                        string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
                                        PicName = PicName.Replace(" ","-"); ;
                                        Utils.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
                                        GlobusLogHelper.log.Info(PicName + " downloaded to " + ExportPhotosPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                LastFBID = Utils.getBetween(AjaxPageSource, "last_fbid\\\":", ","); 
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartDownloadingPhotoProfile(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string ProFilePost = string.Empty;
                string __user = string.Empty;
                string ExportPath = ExportPhotosPath;
                if (string.IsNullOrEmpty(ExportPath))
                {
                    GlobusLogHelper.log.Info("Please Select Photo Export Folder Path !! ");
                    GlobusLogHelper.log.Debug("Please Select Photo Export Folder Path !! ");
                    return;
                }
                string userId = "";
                List<string> lstphotourl = new List<string>();
                try
                {
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


                    foreach (var LstDownloadPhotoURLsDownloadPhoto_item in LstDownloadPhotoURLsDownloadPhoto)
                    {
                        try
                        {
                            string PageSourceOfTargetedUrl = string.Empty;
                            string URL = string.Empty;
                            string PageTitle = string.Empty;
                            string PageID = string.Empty;
                            if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos"))
                            {
                                URL = LstDownloadPhotoURLsDownloadPhoto_item + "/photos_stream";
                            }
                            else
                            {
                                URL = LstDownloadPhotoURLsDownloadPhoto_item;
                            }
                            List<string> PhotoURL = new List<string>();
                            try
                            {
                                PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL));
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            string[] PageValues = Regex.Split(PageSourceOfTargetedUrl, "TimelineAppCollection\", \"enableContentLoader");
                            string CollectionToken = Utils.getBetween(PageValues[1], "pagelet_timeline_app_collection_", "\"");
                            string Cursor = Utils.getBetween(PageSourceOfTargetedUrl, "},\"", "\"]");
                            PageTitle = Utils.getBetween(PageSourceOfTargetedUrl, "pageTitle\">", "</title>");
                            PageID = Utils.getBetween(PageSourceOfTargetedUrl, "pageID\":", ",");
                            string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "data-non-starred-src");
                            PhotoId = PhotoId.Skip(1).ToArray();
                            int RandomNumber = 0;
                            foreach (string Photoid in PhotoId)
                            {
                                try
                                {
                                    RandomNumber++;
                                    string temp = Utils.getBetween(Photoid, "=\"", "\"");
                                    string CompletePicUrl = string.Empty;
                                    if (temp.Contains("/p") || temp.Contains("/v/"))
                                    {
                                        try
                                        {
                                            string[] arr = Regex.Split(temp, "/p");
                                            string Locatstr = arr[1] + "###";
                                            CompletePicUrl = arr[0] + "/" + Utils.getBetween(Locatstr, "/", "###");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(ex.StackTrace);
                                        }
                                        CompletePicUrl = CompletePicUrl.Replace("/v", "");
                                        if (string.IsNullOrEmpty(CompletePicUrl))
                                        {
                                            CompletePicUrl = temp.Replace("/v", "");
                                        }
                                    }
                                    string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();

                                    try
                                    {
                                        Utils.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
                                        GlobusLogHelper.log.Info(PicName + " downloaded to " + ExportPhotosPath);
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
                            int Count = 0;
                            while (true)
                            {
                                Count = Count + 1;
                                if (Count >= 20)
                                {
                                    break;
                                }
                                string AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + CollectionToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A%2C%22last_fbid%22%3A" + Cursor.Replace("\\\"", "") + "%2C%22fetch_size%22%3A%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt=2"));
                                PhotoId = Regex.Split(AjaxPageSource, "data-non-starred-src");
                                PhotoId = PhotoId.Skip(1).ToArray();
                                foreach (string Photoid in PhotoId)
                                {
                                    try
                                    {
                                        RandomNumber++;
                                        string temp = Utils.getBetween(Photoid, "=\\\"", "\\\"").Replace("\\", "");
                                        string CompletePicUrl = string.Empty;
                                        if (temp.Contains("/p") || temp.Contains("/v/"))
                                        {
                                            string[] arr = Regex.Split(temp, "/p");
                                            string Locatstr = arr[1] + "###";
                                            CompletePicUrl = arr[0] + "/" + Utils.getBetween(Locatstr, "/", "###");
                                            CompletePicUrl = temp.Replace("/v", "");
                                            if (string.IsNullOrEmpty(CompletePicUrl))
                                            {
                                                CompletePicUrl = temp.Replace("/v", "");
                                            }
                                        }
                                        string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
                                        PicName = PicName.Replace(" ", "-"); ;
                                        Utils.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
                                        GlobusLogHelper.log.Info(PicName + " downloaded to " + ExportPhotosPath);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                Cursor = Utils.getBetween(AjaxPageSource, "last_fbid\\\":", ",");
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
    }
}
