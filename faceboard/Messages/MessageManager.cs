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
using System.Collections.Specialized;

namespace Messages
{
    public class MessageManager
    {
        public BaseLib.Events messageReplyEvent = null;

        #region Global Variables For MessageReply

        readonly object lockrThreadControllerMessageReply = new object();
        public bool isStopMessageReply = false;
        int countThreadControllerMessageReply = 0;
        
        public List<Thread> lstThreadsMessageReply = new List<Thread>();
        public List<string> LstReplyDetailsMessageReply = new List<string>();

        public static int minDelayMessageReply = 10;
        public static int maxDelayMessageReply = 20;

        public static bool CheckSendMessageWithTage = false;
        public static bool useDivideDataOption = false;
        int numberOfMessages = 0;
        int countnumberOfMessagesSent = 1;

        public static int TotalNoofSeneMessage_Counter = 0;

     

        #endregion

        #region Property For MessageReply

        public int NoOfThreadsMessageReply
        {
            get;
            set;
        }

        public List<string> lstImagePathMessage = new List<string>();
        public string SendMessageUsingMessageReply
        {
            get;
            set;
        }
        public static string SendMessageUsingMessage
        {
            get;
            set;
        }

        public string ReplyMessageMessageReply
        {
            get;
            set;
        }
        public static string ReplyMessageMessageSingle
        {
            get;
            set;
        }

        public List<string> LstReplyMessageMessageReply
        {
            get;
            set;
        }


        public static string MessageMessageReplyProcessUsing
        {
            get;
            set;
        }
        public static int MessageMessageSendNoOfFriends
        {
            get;
            set;
        }
        public static List<string> MessageMessageLoadProfileUrl
        {
            get;
            set;
        }
        public static Queue<string> MessageMessageLoadProfileUrlQueue = new Queue<string>();
        #endregion

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                messageReplyEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public MessageManager()
        {
            try
            {
                messageReplyEvent = new Events();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        List<string> lstFBAccounts = new List<string>();
        Queue<string> qImagePathMesssage = new Queue<string>(); 
        public void StartMessageReply()
        {
            try
            {
                foreach (string item in lstImagePathMessage)
                {
                    qImagePathMesssage.Enqueue(item);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            lstFBAccounts = FBGlobals.listAccounts;
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsMessageReply > 0)
                {
                    numberOfAccountPatch = NoOfThreadsMessageReply;
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
                                lock (lockrThreadControllerMessageReply)
                                {
                                    try
                                    {
                                        if (countThreadControllerMessageReply >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerMessageReply);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsMessageReply);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerMessageReply++;
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

        public void StartMultiThreadsMessageReply(object parameters)
        {
           // lock (this)
            {

                try
                {
                    if (!isStopMessageReply)
                    {
                        try
                        {
                            lstThreadsMessageReply.Add(Thread.CurrentThread);
                            lstThreadsMessageReply.Distinct();
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

                                /// Checking Reply User is same as Login User
                                bool isUserExist = false;

                                if (MessageMessageReplyProcessUsing == "SendMessageWithScraper")
                                {
                                    string messageUserName = string.Empty;
                                    foreach (string item in LstReplyDetailsMessageReply)
                                    {
                                        try
                                        {
                                            //if (Islogin())
                                            {

                                                string[] itemArr = Regex.Split(item, ":");

                                                foreach (string item1 in itemArr)
                                                {
                                                    try
                                                    {
                                                        if (item1.Contains("<UserName>"))
                                                        {
                                                            messageUserName = item1.Replace("<UserName>", string.Empty).Trim();
                                                            break;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }

                                                if (messageUserName == objFacebookUser.username)
                                                {
                                                    isUserExist = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    if (!isUserExist)
                                    {
                                        return;
                                    }
                                }

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
                                    StartActionMessageReply(ref objFacebookUser);
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
                  //  if (!isStopMessageReply)
                    {
                        lock (lockrThreadControllerMessageReply)
                        {
                            countThreadControllerMessageReply--;
                            Monitor.Pulse(lockrThreadControllerMessageReply);
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

        private void StartActionMessageReply(ref FacebookUser fbUser)
        {
            try
            {

                if (MessageMessageReplyProcessUsing == "SendMessageWithScraper")
                {
                    ReplyMessage(ref fbUser);

                    GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);
                }
                if (MessageMessageReplyProcessUsing == "SendNormalMessage")
                {
                    SendMessageFacebookerUpdated(ref fbUser);

                    GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);

                }
                if (MessageMessageReplyProcessUsing == "SendTargetedProfile" && !useDivideDataOption)
                {
                    SendMessageTargatedProfile(ref fbUser);

                    GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);
                }
                if (MessageMessageReplyProcessUsing == "SendTargetedProfile" && useDivideDataOption)
                {
                    SendMessageTargetedProfileWithDivideData(ref fbUser);
                    GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);
                }
                if (MessageMessageReplyProcessUsing == "SendMessageWithimage")
                {
                    SendMessageWithImage(ref fbUser);
                    GlobusLogHelper.log.Info("Process Completed With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Process Completed With Username : " + fbUser.username);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void ReplyMessage(ref FacebookUser fbUser)
        {
            try
            {
                try
                {
                   
                    string messageUserName = string.Empty;
                    string editMessage = string.Empty;
                    string friendIdValue = string.Empty;
                    string strGreetingWord = string.Empty;
                    string FriendName = string.Empty;
                    bool IsUseName = false;
                    bool isUserExist = false;
                    string SingaleOriginalMessage = string.Empty;

                    GlobusHttpHelper GlobusHttpHelper = fbUser.globusHttpHelper;

                    if (SendMessageUsingMessageReply == "Single")
                    {
                        editMessage = ReplyMessageMessageSingle;
                    }

                    if (SendMessageUsingMessageReply == "Random")
                    {
                        editMessage = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                    }

                    //if (userName == Username)
                    {
                        //lstloginuser.Add(Username);

                        string UsreId = string.Empty;

                        string pageSource_Home = GlobusHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));


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

                        string userFirstName = string.Empty;
                        string graphValue = GlobusHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UsreId));


                        if (!graphValue.Contains("false") && graphValue.Contains("name\":"))
                        {
                            try
                            {
                                try
                                {
                                    userFirstName = graphValue.Substring(graphValue.IndexOf("name\":"), graphValue.IndexOf(",", graphValue.IndexOf("name\":")) - (graphValue.IndexOf("name\":"))).Replace("name\":", string.Empty).Replace("\"", string.Empty).Trim();
                                    if (userFirstName.Contains(" "))
                                    {
                                        userFirstName = userFirstName.Substring(0, userFirstName.IndexOf(" "));
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

                        SingaleOriginalMessage = editMessage;
                        string MessageFriend = editMessage;
                        int Conunter = 1;
                         
                        int indexname = 0;
                        foreach (string item in LstReplyDetailsMessageReply)
                        {
                            try
                            {
                                if (SendMessageUsingMessageReply == "Single")
                                {
                                    MessageFriend = ReplyMessageMessageSingle;
                                }

                                if (SendMessageUsingMessageReply == "Random")
                                {
                                    MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                                }
                                //if (Islogin())
                                {

                                    string[] itemArr = Regex.Split(item, ":");

                                    foreach (string item1 in itemArr)
                                    {
                                        try
                                        {
                                            if (item1.Contains("<UserName>"))
                                            {
                                                messageUserName = item1.Replace("<UserName>", string.Empty).Trim();
                                            }
                                            if (item1.Contains("<MessageFriendId>"))
                                            {
                                                friendIdValue = item1.Replace("<MessageFriendId>", string.Empty).Trim();
                                            }
                                            if (item1.Contains("<MessageSnippedId>"))
                                            {
                                            }
                                            if (item1.Contains("<MessageSenderName>"))
                                            {
                                            }
                                            if (item1.Contains("<MessagingReadParticipants>"))
                                            {
                                                FriendName = item1.Replace("<MessagingReadParticipants>", string.Empty).Trim();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    if (fbUser.username != messageUserName)
                                    {
                                        continue;
                                    }

                                    string FriendUrl = FBGlobals.Instance.fbProfileUrl + friendIdValue;

                                    GlobusLogHelper.log.Info(Conunter + " Sending Message with " + fbUser.username);
                                    GlobusLogHelper.log.Debug(Conunter + " Sending Message with " + fbUser.username);


                                    string PageSrcMsg = GlobusHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbMessagesUrl));

                                    string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcMsg);

                                    string UrlMsgAjax = string.Empty;
                                    UrlMsgAjax = FBGlobals.Instance.MessageReplyGetAjaxAsyncDialogUrl;

                                    string postUrlMsg = FBGlobals.Instance.MessageReplyPostAjaxMessagingSendUrl;
                                    string PostDataMsg = string.Empty;

                                    if (IsUseName)
                                    {

                                        if (!string.IsNullOrEmpty(MessageFriend) && !string.IsNullOrWhiteSpace(MessageFriend) && !MessageFriend.Contains("www"))
                                        {
                                            if (CheckSendMessageWithTage)
                                            {
                                                try
                                                {
                                                    string PageSource = GlobusHttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + friendIdValue));
                                                    string Name = Utils.getBetween(PageSource, "\"name\": \"", "\",\n");
                                                    string[] Arr = System.Text.RegularExpressions.Regex.Split(MessageFriend, "<>");
                                                    MessageFriend = Arr[0] + Name + Arr[1];
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                            }


                                            if (!string.IsNullOrWhiteSpace(strGreetingWord) || !string.IsNullOrWhiteSpace(strGreetingWord))
                                            {
                                                PostDataMsg = "forward_msgs&body=" + strGreetingWord + " " + FriendName + " ! \n" + Environment.NewLine +Uri.EscapeDataString(MessageFriend) + Environment.NewLine + Environment.NewLine + userFirstName + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";
                                            }
                                            else
                                            {

                                                PostDataMsg = "forward_msgs&body=" + Uri.EscapeDataString(MessageFriend) + Environment.NewLine + Environment.NewLine + userFirstName + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";

                                            }
                                        }
                                    }
                                    else 
                                    {
                                        if (!string.IsNullOrEmpty(MessageFriend) && !string.IsNullOrWhiteSpace(MessageFriend))
                                        {
                                            if (!string.IsNullOrWhiteSpace(strGreetingWord) || !string.IsNullOrWhiteSpace(strGreetingWord))
                                            {
                                                PostDataMsg = "forward_msgs&body=" + strGreetingWord + " " + FriendName + " ! \n" + Environment.NewLine + Uri.EscapeDataString(MessageFriend) + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";
                                            }
                                            else
                                            {
                                                if (CheckSendMessageWithTage)
                                                {
                                                    try
                                                    {
                                                        string Name = string.Empty;
                                                        string PageSource = GlobusHttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + friendIdValue));
                                                        if (PageSource.Contains("\"name\": \""))
                                                        {
                                                            try
                                                            {
                                                                Name = Utils.getBetween(PageSource, "\"name\": \"", "\",\n");
                                                            }
                                                            catch (Exception ex)
                                                            {                                                            
                                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                            }

                                                        }
                                                        if (string.IsNullOrEmpty(Name))
                                                        {
                                                            try
                                                            {
                                                                string[] arr = System.Text.RegularExpressions.Regex.Split(PageSource,"locale\"");
                                                                Name = Utils.getBetween(arr[1], "name", "\"\n}").Replace("\"","").Replace(":","");
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                            }
                                                        }
                                                         
                                                        if (MessageFriend.Contains("<>"))
                                                        {
                                                            string[] Arr = System.Text.RegularExpressions.Regex.Split(MessageFriend, "<>");
                                                            MessageFriend = Arr[0] + " " + Name + " " + Arr[1];
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                string[] Arr = System.Text.RegularExpressions.Regex.Split(SingaleOriginalMessage, "<>");
                                                                MessageFriend = Arr[0] + " " + Name + " " + Arr[1];
                                                            }
                                                            catch { };
                                                        }
                                                        FriendName = "";
                                                       // PostDataMsg = "forward_msgs&body=" + strGreetingWord + " " + FriendName + " ! \n" + Environment.NewLine + MessageFriend + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";
                                                        PostDataMsg = "forward_msgs&body=" + strGreetingWord + " " + FriendName + Environment.NewLine + Uri.EscapeDataString(MessageFriend) + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                }
                                                else
                                                {
                                                    FriendName = "";
                                                    PostDataMsg = "forward_msgs&body=" + strGreetingWord + " " + FriendName + Environment.NewLine + Uri.EscapeDataString(MessageFriend) + "&action=send&recipients[0]=" + friendIdValue + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";
                                                }
                                            }                                            
                                        }
                                    }
                                    indexname++;
                                    string ResponseMsg=string.Empty;
                                    
                                     ResponseMsg = GlobusHttpHelper.postFormData(new Uri(postUrlMsg), PostDataMsg);
                                   
                               
                                    if (ResponseMsg.Contains("errorSummary\":"))
                                    {
                                        string errorSummery = FBUtils.GetErrorSummary(ResponseMsg);//Regex.Split(ResponseMsg, "errorSummary\":");

                                        GlobusLogHelper.log.Info(" Error Summary : " + errorSummery + "  With UserName : " + fbUser.username);
                                        GlobusLogHelper.log.Debug(" Error Summary : " + errorSummery + "  With UserName : " + fbUser.username);

                                        try
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
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

                                        GlobusLogHelper.log.Info(Conunter + " Sent Message with " + fbUser.username);
                                        GlobusLogHelper.log.Debug(Conunter + " Sent Message with " + fbUser.username);
                                        if (Conunter>=MessageMessageSendNoOfFriends)
                                        {
                                            break;
                                        }

                                        //clsMsgManager_DB.UpdateMessageStatusFromtb_MsgManager(Username, SnippedId);

                                        try
                                        {
                                            int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
                                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                            Thread.Sleep(delayInSeconds);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    Conunter++;

                                }

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                finally
                { 
                    //msgSendingThreadCount--;
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void SendMessageFacebookerUpdated(ref FacebookUser fbUser)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                numberOfMessages = MessageMessageSendNoOfFriends;

                string Username = string.Empty;
                string MessageFriend = string.Empty;
                string UsreId = string.Empty;

                Username = fbUser.username;

                GlobusLogHelper.log.Debug("Sending Message with " + Username);

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri((FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookUrl)));    


                string tempUserID = string.Empty;
                List<string> lstFriend = new List<string>();
                UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
                }

               int count_Friends = ExtractFriendCount(ref fbUser, UsreId);

               // int count_Friends = FBUtils.GetAllFriends(ref HttpHelper, UsreId);              

                lstFriend.Clear();

                lstFriend = ExtractFriendIdsFb(ref fbUser, ref UsreId, count_Friends);

             
                GlobusLogHelper.log.Debug("Please wait Getting Friends Id...");
                GlobusLogHelper.log.Info("Please wait Getting Friends Id...");
                List<string> lstfriendss = GetIdFromFacebook(ref fbUser, UsreId);
                GlobusLogHelper.log.Debug(" Find Friend count : " + lstfriendss.Count);
                GlobusLogHelper.log.Info(" Find Friend count : " + lstfriendss.Count);

                lstFriend = lstFriend.Distinct().ToList();
                lstFriend.AddRange(lstfriendss);
                lstFriend = lstFriend.Distinct().ToList();
                //lstFriend = lstFriend.Distinct().ToList();              

               
                int Counter = 1;
                foreach (string FriendId in lstFriend)
                {
                    if (SendMessageUsingMessage == "Single")
                    {
                        MessageFriend = ReplyMessageMessageSingle;
                    }

                    if (SendMessageUsingMessage == "Random")
                    {
                        MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                    }
                    MessageFriend= Uri.EscapeDataString(MessageFriend);
                    try
                    {
                        if (countnumberOfMessagesSent > numberOfMessages)
                        {                            
                            break;
                        }

                        string FriendUrl = FBGlobals.Instance.fbProfileUrl + FriendId;                               
                        GlobusLogHelper.log.Debug(countnumberOfMessagesSent + " Sending Message with " + Username);
                        GlobusLogHelper.log.Info(countnumberOfMessagesSent + " Sending Message with " + Username);

                        string PageSrcMsg = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbMessagesUrl));     

                        string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcMsg);
                        //sendUrlWithThumnail(ref fbUser, "https://www.tumblr.com/", fb_dtsg,UsreId,FriendId);

                        string UrlMsgAjax = string.Empty;
                        UrlMsgAjax = FBGlobals.Instance.MessageReplyGetAjaxAsyncDialogUrl + UsreId;                   
                        string PageSrcMsgAjax = HttpHelper.getHtmlfromUrl(new Uri(UrlMsgAjax));

                        if (CheckSendMessageWithTage)
                        {
                            try
                            {
                                string Name = string.Empty;
                                string PageSource = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + FriendId));
                                if (PageSource.Contains("\"name\": \""))
                                {
                                    try
                                    {
                                        Name = Utils.getBetween(PageSource, "\"name\": \"", "\",\n");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }
                                if (string.IsNullOrEmpty(Name))
                                {
                                    try
                                    {
                                        string[] arr = System.Text.RegularExpressions.Regex.Split(PageSource, "locale\"");
                                        Name = Utils.getBetween(arr[1], "name", "\"\n}").Replace("\"", "").Replace(":", "");
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                                if (MessageFriend.Contains("<>"))
                                {
                                    string[] Arr = System.Text.RegularExpressions.Regex.Split(MessageFriend, "<>");
                                    MessageFriend = Arr[0] + " " + Name + " " + Arr[1];
                                }                               
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        if (!ReplyMessageMessageSingle.Contains("www"))
                        {
                            string postUrlMsg = FBGlobals.Instance.MessageReplyPostAjaxMessagingSendUrl;
                            string PostDataMsg = "forward_msgs&body=" + MessageFriend + "&action=send&recipients[0]=" + FriendId + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";

                            string ResponseMsg = HttpHelper.postFormData(new Uri(postUrlMsg), PostDataMsg, "");

                            string postUrlMsg1 = FBGlobals.Instance.MessageReplyPostAjaxMessagingPhp;
                            string PostDataMsg1 = "fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";

                            string ResponseMsg1 = HttpHelper.postFormData(new Uri(postUrlMsg1), PostDataMsg1, "");
                        }
                        if (ReplyMessageMessageSingle.Contains("www"))
                        {
                            string MessagePage = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/messages/" + UsreId));
                            string postUrl = Uri.EscapeDataString(ReplyMessageMessageSingle);

                            string postThumbnail = "u=" + postUrl + "&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1a&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";
                            string postThumbnailResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/share_scrape.php"), postThumbnail);


                            string Subject = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "subject\\\" value=\\\"", "\\"));
                            string appId = Utils.getBetween(postThumbnailResp, "app_id\\\" value=\\\"", "\\");
                            string favicon = Utils.getBetween(postThumbnailResp, "favicon]\\\" value=\\\"", "\"");
                            favicon = favicon.Replace("/", string.Empty);
                            favicon = Uri.EscapeDataString(favicon);
                            string title = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "title]\\\" value=\\\"", "\\"));
                            string thumbImgSrc = Utils.getBetween(postThumbnailResp, "Thumb img\\\" src=\\\"", "\"");
                            thumbImgSrc = thumbImgSrc.Replace("\\u00253A", ":");
                            thumbImgSrc = thumbImgSrc.Replace("\\u00252F", "/");

                            thumbImgSrc = thumbImgSrc.Replace("\\", string.Empty);
                            thumbImgSrc = thumbImgSrc.Replace("u002", string.Empty);
                            thumbImgSrc = thumbImgSrc.Replace("amp;", string.Empty);
                            thumbImgSrc = Uri.EscapeDataString(thumbImgSrc);
                            string Medium = Utils.getBetween(postThumbnailResp, "[medium]\\\" value=\\\"", "\\");
                            string type = Utils.getBetween(postThumbnailResp, "type]\\\" value=\\\"", "\\");
                            string domain = Utils.getBetween(postThumbnailResp, "domain]\\\" value=\\\"", "\\");
                            string baseDomain = Utils.getBetween(postThumbnailResp, "base_domain]\\\" value=\\\"", "\\");
                            string title_len = Utils.getBetween(postThumbnailResp, "title_len]\\\" value=\\\"", "\\");
                            string summary_len = Utils.getBetween(postThumbnailResp, "summary_len]\\\" value=\\\"", "\\");
                            string min_dimensions0 = Utils.getBetween(postThumbnailResp, "min_dimensions][0]\\\" value=\\\"", "\\");
                            string min_dimensions1 = Utils.getBetween(postThumbnailResp, "min_dimensions][1]\\\" value=\\\"", "\\");
                            string image_dimensions0 = Utils.getBetween(postThumbnailResp, "image_dimensions][0]\\\" value=\\\"", "\\");
                            string image_dimensions1 = Utils.getBetween(postThumbnailResp, "image_dimensions][1]\\\" value=\\\"", "\\");
                            string summary = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "params][summary]\\\" value=\\\"", "\\"));
                            //params][summary]\" value=\"
                            string tt = Utils.GenerateTimeStamp();
                            string TT1 = Uri.EscapeDataString(DateTime.Now.ToString("HH:mm"));

                            string postFinalThumbnail = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "" +
                                                        "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0]" +
                                                        "[timestamp]=" + tt + "" +
                                                        "&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=" + TT1 + "&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + postUrl + "" +
                                                        "&message_batch[0][has_attachment]=true&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "" +
                                                        "&message_batch[0][specific_to_list][1]=fbid%3A" + FriendId + "" +
                                                        "&message_batch[0][content_attachment][subject]=" + Subject + "&message_batch[0][content_attachment][app_id]=" + appId + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][urlInfo][canonical]=" + postUrl + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][urlInfo][final]=" + postUrl + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][urlInfo][user]=" + postUrl + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][favicon]=" + favicon + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][title]=" + title + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][summary]=" + summary + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][images][0]=" + thumbImgSrc + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][medium]=" + Medium + "" +
                                                        "&message_batch[0][content_attachment][attachment][params][url]=" + postUrl + "" +
                                                        "&message_batch[0][content_attachment][attachment][type]=" + type + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][source]=ShareStageExternal&message_batch[0][content_attachment][link_metrics][domain]=" + domain + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][base_domain]=" + baseDomain + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][title_len]=" + title_len + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][summary_len]=0" +
                                                        "&message_batch[0][content_attachment][link_metrics][min_dimensions][0]=" + min_dimensions0 + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][min_dimensions][1]=" + min_dimensions1 + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_with_dimensions]=2" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_pending]=0" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_fetched]=0" +
                                                        "&message_batch[0][content_attachment][link_metrics][image_dimensions][0]=" + image_dimensions0 + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][image_dimensions][1]=" + image_dimensions1 + "" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_selected]=1" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_considered]=2" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_cap]=3" +
                                                        "&message_batch[0][content_attachment][link_metrics][images_type]=ranked" +
                                                        "&message_batch[0][content_attachment][composer_metrics][best_image_w]=100" +
                                                        "&message_batch[0][content_attachment][composer_metrics][best_image_h]=100" +
                                                        "&message_batch[0][content_attachment][composer_metrics][image_selected]=0" +
                                                        "&message_batch[0][content_attachment][composer_metrics][images_provided]=1" +
                                                        "&message_batch[0][content_attachment][composer_metrics][images_loaded]=1" +
                                                        "&message_batch[0][content_attachment][composer_metrics][images_shown]=1" +
                                                        "&message_batch[0][content_attachment][composer_metrics][load_duration]=4136" +
                                                        "&message_batch[0][content_attachment][composer_metrics][timed_out]=0" +
                                                        "&message_batch[0][content_attachment][composer_metrics][sort_order]=" +
                                                        "&message_batch[0][content_attachment][composer_metrics][selector_type]=UIThumbPager_6" +
                                                        "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V2p" +
                                                        "&message_batch[0][status]=0&message_batch[0][message_id]=%3C" + tt + "%3A3925954697-2587754773%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A" + FriendId + "" +
                                                        "&client=web_messenger&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1c" +
                                                        "&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";



                            string ThreadPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to=" + FriendId + "&source=mercury-chat&thread=" + FriendId + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170831091124811651677495&__rev=1694181");
                            string ThreadPost1 = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to=" + FriendId + "&source=mercury-chat&thread=" + FriendId + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170831091124811651677495&__rev=1694181");

                            string postFinalTumbnailresp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), postFinalThumbnail, "https://www.facebook.com/messages/" + FriendId + "");
                            string message_id = Utils.getBetween(postFinalTumbnailresp, "message_id\":\"", "\"");

                            string postFinalTumbnail1 = "message_ids[0]=" + message_id + "&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1e&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";
                            string postFinalResult = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/attachments/fetch_shares.php"), postFinalTumbnail1);


                        }


                        TotalNoofSeneMessage_Counter++;
                        GlobusLogHelper.log.Debug(countnumberOfMessagesSent + " Sent Message with " + Username);
                        GlobusLogHelper.log.Info(countnumberOfMessagesSent + " Sent Message with " + Username);

                        try
                        {
                            int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
                            GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                            Thread.Sleep(delayInSeconds);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        countnumberOfMessagesSent++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                countnumberOfMessagesSent--;
                GlobusLogHelper.log.Debug("Finished Sending " + countnumberOfMessagesSent + " Messages with " + Username);
                GlobusLogHelper.log.Info("Finished Sending " + countnumberOfMessagesSent + " Messages with " + Username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }         
        }

        public void sendUrlWithThumnail(ref FacebookUser fbUser,string SiteUrl,string fbDtsg,string UserId,string FriendsId)
        {
            GlobusHttpHelper objhttp = fbUser.globusHttpHelper;
            try
            {
                string ScrapeUrl = "https://www.facebook.com/ajax/share_scrape.php";
                string PostData = "chat=true&u=" + Uri.EscapeDataString(SiteUrl) + "&__user=" + UserId + "&__a=1&__dyn=7nmajEyl2lm9o-t2u5bGya4Au7pEsx6iqA8Ay9VQC-C26m6oKeDBwIhEoyUnwPUS2O4K5e8Gi4EOGy9KVaK&__req=1e&fb_dtsg=" + fbDtsg + "&ttstamp=2658169757610850101120818283&__rev=1691974";
                string Scraperesp = objhttp.postFormData(new Uri(ScrapeUrl),PostData);
                string currentTime = DateTime.Now.ToString("TT:MM");
                string Subject = string.Empty;
                string AppId = string.Empty;
                string Conical = string.Empty;
                string finalval = string.Empty;
                string userVal = string.Empty;
                string faviconval = string.Empty;
                string Title = string.Empty;
                string Summery = string.Empty;
                string Images = string.Empty;
                string Domain = string.Empty;
                string baseDomain = string.Empty;
                string title_len = string.Empty;
                string summaryLen = string.Empty;
                string mindim1 = string.Empty;
                string mindim2 = string.Empty;
                string imagewithDim = string.Empty;
                string imageDim0 = string.Empty;
                string imageDim1 = string.Empty;
                string msgId = string.Empty;
                Subject = Utils.getBetween(Scraperesp, "subject\\\" value=\\\"", "\\\"");
                AppId = Utils.getBetween(Scraperesp, "app_id\\\" value=\\\"", "\\\"");
                Conical = Utils.getBetween(Scraperesp, "[urlInfo][canonical]\\\" value=\\\"", "\\\"");
                finalval = Utils.getBetween(Scraperesp, "[urlInfo][final]\\\" value=\\\"", "\\\"");
                userVal = Utils.getBetween(Scraperesp, "[urlInfo][user]\\\" value=\\\"", "\\\"");
                faviconval = Utils.getBetween(Scraperesp, "params][favicon]\\\" value=\\\"", "\\\"");
                Title = Utils.getBetween(Scraperesp, "params][title]\\\" value=\\\"", "\\\"");
                Summery = Utils.getBetween(Scraperesp, "[params][summary]\\\" value=\\\"", "\\\"");
                Images = Utils.getBetween(Scraperesp, "params][images][0]\\\" value=\\\"", "\\\"");
                Domain = Utils.getBetween(Scraperesp, "link_metrics][domain]\\\" value=\\\"","\\\"");
                baseDomain = Utils.getBetween(Scraperesp, "link_metrics][base_domain]\\\" value=\\\"", "\\\"");
                title_len = Utils.getBetween(Scraperesp, "link_metrics][title_len]\\\" value=\\\"", "\\\"");
                summaryLen = Utils.getBetween(Scraperesp, "link_metrics][summary_len]\\\" value=\\\"", "\\\"");
                mindim1 = Utils.getBetween(Scraperesp, "link_metrics][min_dimensions][0]\\\" value=\\\"", "\\\"");
                mindim2 = Utils.getBetween(Scraperesp, "link_metrics][min_dimensions][1]\\\" value=\\\"", "\\\"");
                imagewithDim = Utils.getBetween(Scraperesp, "link_metrics][images_with_dimensions]\\\" value=\\\"", "\\\"");
                imageDim0 = Utils.getBetween(Scraperesp, "link_metrics][image_dimensions][0]\\\" value=\\\"", "\\\"");
                imageDim1 = Utils.getBetween(Scraperesp, "link_metrics][image_dimensions][1]\\\" value=\\\"", "\\\"");
                msgId = Utils.getBetween(Scraperesp, "[0][message_id]\\\" value=\\\"", "\\\"");

                string PostThumbnail = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A100001132466717&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0][timestamp]=1429186765657&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=17%3A49&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Achat%3Aweb&message_batch[0][source_tags][0]=source%3Achat&message_batch[0][body]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][has_attachment]=true&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A100007427978343&message_batch[0][specific_to_list][1]=fbid%3A100001132466717&message_batch[0][content_attachment][subject]=Sign%20up%20%7C%20Tumblr&message_batch[0][content_attachment][app_id]=2309869772&message_batch[0][content_attachment][attachment][params][urlInfo][canonical]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][content_attachment][attachment][params][urlInfo][final]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][content_attachment][attachment][params][urlInfo][user]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][content_attachment][attachment][params][urlInfo][log][1416522210]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][content_attachment][attachment][params][urlInfo][log][1416522239]=https%3A%2F%2Fwww.Tumblr.com%2F&message_batch[0][content_attachment][attachment][params][favicon]=https%3A%2F%2Fsecure.assets.tumblr.com%2Fimages%2Ffavicons%2Ffavicon.ico%3F_v%3D2f32b762e629b447737e25d2f97b808a&message_batch[0][content_attachment][attachment][params][title]=Sign%20up%20%7C%20Tumblr&message_batch[0][content_attachment][attachment][params][summary]=Post%20anything%20(from%20anywhere!)%2C%20customize%20everything%2C%20and%20find%20and%20follow%20what%20you%20love.%20Create%20your%20own%20Tumblr%20blog%20today.&message_batch[0][content_attachment][attachment][params][images][0]=https%3A%2F%2Ffbexternal-a.akamaihd.net%2Fsafe_image.php%3Fd%3DAQBpEWlFfsGQ2IHX%26w%3D100%26h%3D100%26url%3Dhttps%253A%252F%252F41.media.tumblr.com%252Fcd7ecaa794dd58fb2e05dee3002a08b4%252Ftumblr_nla7keszEu1u7s19xo1_1280.jpg%26cfs%3D1%26upscale%3D1&message_batch[0][content_attachment][attachment][params][medium]=106&message_batch[0][content_attachment][attachment][params][url]=https%3A%2F%2Fwww.tumblr.com%2F&message_batch[0][content_attachment][attachment][type]=100&message_batch[0][content_attachment][link_metrics][source]=ShareStageExternal&message_batch[0][content_attachment][link_metrics][domain]=www.tumblr.com&message_batch[0][content_attachment][link_metrics][base_domain]=tumblr.com&message_batch[0][content_attachment][link_metrics][title_len]=16&message_batch[0][content_attachment][link_metrics][summary_len]=123&message_batch[0][content_attachment][link_metrics][min_dimensions][0]=70&message_batch[0][content_attachment][link_metrics][min_dimensions][1]=70&message_batch[0][content_attachment][link_metrics][images_with_dimensions]=3&message_batch[0][content_attachment][link_metrics][images_pending]=0&message_batch[0][content_attachment][link_metrics][images_fetched]=0&message_batch[0][content_attachment][link_metrics][image_dimensions][0]=700&message_batch[0][content_attachment][link_metrics][image_dimensions][1]=782&message_batch[0][content_attachment][link_metrics][images_selected]=1&message_batch[0][content_attachment][link_metrics][images_considered]=4&message_batch[0][content_attachment][link_metrics][images_cap]=3&message_batch[0][content_attachment][link_metrics][images_type]=ranked&message_batch[0][content_attachment][composer_metrics][best_image_w]=100&message_batch[0][content_attachment][composer_metrics][best_image_h]=100&message_batch[0][content_attachment][composer_metrics][image_selected]=0&message_batch[0][content_attachment][composer_metrics][images_provided]=1&message_batch[0][content_attachment][composer_metrics][images_loaded]=1&message_batch[0][content_attachment][composer_metrics][images_shown]=1&message_batch[0][content_attachment][composer_metrics][load_duration]=488&message_batch[0][content_attachment][composer_metrics][timed_out]=0&message_batch[0][content_attachment][composer_metrics][sort_order]=&message_batch[0][content_attachment][composer_metrics][selector_type]=UIThumbPager_6&message_batch[0][ui_push_phase]=V3&message_batch[0][status]=0&message_batch[0][message_id]=%3C1429186765657%3A2768739321-851827929%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A100007427978343&client=mercury&__user=100001132466717&__a=1&__dyn=7nmajEyl2lm9o-t2u5bGya4Au7pEsx6iqA8Ay9VQC-C26m6oKeDBwIhEoyUnwPUS2O4K5e8Gi4EOGy9KVaK&__req=1g&fb_dtsg="+fbDtsg+"&ttstamp=2658169757610850101120818283&__rev=1691974";

                string PostThumbResp = objhttp.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"),PostThumbnail);

                string FinalPost = objhttp.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=0&to=100007427978343&source=mercury-chat&thread=100007427978343&__user=100001132466717&__a=1&__dyn=7nmajEyl2lm9o-t2u5bGya4Au7pEsx6iqA8Ay9VQC-C26m6oKeDBwIhEoyUnwPUS2O4K5e8Gi4EOGy9KVaK&__req=1h&fb_dtsg="+fbDtsg+"&ttstamp=2658169757610850101120818283&__rev=1691974");
                string MiDPost = objhttp.postFormData(new Uri("https://www.facebook.com/ajax/mercury/attachments/fetch_shares.php"), "message_ids[0]=mid.1429192999548%3A5ef3439898019b2725&__user=100001132466717&__a=1&__dyn=7nmajEyl2lm9o-t2u5bGya4Au7pEsx6iqA8Ay9VQC-C26m6oKeDBwIhEoyUnwPUS2O4K5e8Gi4EOGy9KVaK&__req=1i&fb_dtsg=" + fbDtsg + "&ttstamp=2658169757610850101120818283&__rev=1691974");

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        public static int ExtractFriendCount(ref FacebookUser fbUser, string UserId)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
           
            int FriendCountNumber = 0;
            try
            {

                string url = FBGlobals.Instance.fbProfileUrl + UserId;                                                
                string PageSrcFriendCountOLd = HttpHelper.getHtmlfromUrl(new Uri(url));
                Regex NumChk = new Regex("^[0-9]*$");
                if (PageSrcFriendCountOLd.Contains("Friends ("))
                {
                  
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
                    else
                    {
                        string Countfrnds = Utils.getBetween(PageSrcFriendCount,"<span class=\"_gs6\">","</span>");
                        FriendCountNumber = int.Parse(Countfrnds);
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
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                List<string> lstFriendTemp = new List<string>();
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
                }
                while (i < FriendCount);

                return lstFriendTemp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<string> GetIdFromFacebook(ref FacebookUser fbUser, string item)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            List<string> lstOwnFriendIds = new List<string>();
            try
            {
                string UserId = string.Empty;

                string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl)); 
                string fb_dtsg = string.Empty;
                UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
                }
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);
                List<string> lstOwnFriendId = GetFirendId(ref fbUser, UserId, item);


                string posturl = FBGlobals.Instance.fbAllFriendsUrl;                                           
                string PostdataForFriends = "uid=" + UserId + "&infinitescroll=1&location=friends_tab_tl&start=14&nctr[_mod]=pagelet_friends&__user=" + UserId + "&__a=1&__req=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816811649895156150";
                string response = HttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");
          
                if (string.IsNullOrEmpty(response))
                {
                    posturl = FBGlobals.Instance.fbAllFriendsUrl;                                               
                    response = HttpHelper.postFormData(new Uri(posturl), PostdataForFriends, "");

                }
                string[] Friends = Regex.Split(response, "user.php");
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
                    List<string> lsttotalajaxid = FriendsTotalId(ref fbUser, UserId, UserId);
                    lstOwnFriendId.AddRange(lstnewfriendid);
                    lstOwnFriendId.AddRange(lsttotalajaxid);
                    lstOwnFriendId = lstOwnFriendId.Distinct().ToList();
                    //LoggerFriendProfileUrl("TotalId : " + lstOwnFriendId.Count);

                    foreach (string item1 in lstOwnFriendId)
                    {
                        try
                        {
                            string Temp = string.Empty;

                            if (item1.Contains("&amp;"))
                            {
                                Temp = item1.Substring(0, item1.IndexOf("&amp;")).Trim();
                            }

                            lstOwnFriendIds.Add(Temp);
                            lstOwnFriendIds.Remove("");

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

            return lstOwnFriendIds;
        }

        private static List<string> GetFirendId(ref FacebookUser fbUser, string UId, string profileurl)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

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
                        //Loger("Get LastName " + last_name + " of User : " + listFriendIditem);
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
                        //FriendlistUrl = FriendlistUrl.Replace("//", "/").Trim();
                    }
                }
                else
                {
                    FriendlistUrl = profileurl + "/friends?ft_ref=mni";
                    if (FriendlistUrl.Contains("//"))
                    {
                        FriendlistUrl = FriendlistUrl.Replace("//", "/").Trim();
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
                        FriendlistUrl = FriendlistUrl.Replace("http", "https");
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

        private static List<string> FriendsTotalId(ref FacebookUser fbUser, string Id, string user)
        {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
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
                        string extractkeywordslimit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbAllFriendsUIdUrl + Id + "&infinitescroll=1&location=friends_tab_tl&start=" + offset + "&__user=" + user + "&__a=1"));   //"https://www.facebook.com/ajax/browser/list/allfriends/?uid=

                      
                        if (extractkeywordslimit.Contains("user.php"))
                        {
                            string[] keyword_idarr = Regex.Split(extractkeywordslimit, "user.php");
                            for (int i = 1; i < keyword_idarr.Length; i++)
                            {
                                string keyword_idarritem = keyword_idarr[i].Substring(keyword_idarr[i].IndexOf("id="), keyword_idarr[i].IndexOf(">") - keyword_idarr[i].IndexOf("id=")).Replace("id=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty);

                                toatalid.Add(keyword_idarritem);
                                toatalid = toatalid.Distinct().ToList();                              
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return toatalid;
        }

        public int NoProfileUrlsPerUser = 0;
        int ProfilesCounter = 0;
        public void SendMessageTargatedProfile(ref FacebookUser fbUser)
        {
           // lock(this)
            {
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            string MessageFriend = string.Empty;
            string Username = string.Empty;
            Username = fbUser.username;
            string FriendId = string.Empty;
            string UsreId = string.Empty;
            numberOfMessages = MessageMessageSendNoOfFriends;
            
            try
            {
                
                /*string MessageMessageLoadProfileUrl_item = string.Empty;
                if (MessageMessageLoadProfileUrlQueue.Count != 0)
                {
                    MessageMessageLoadProfileUrl_item = MessageMessageLoadProfileUrlQueue.Dequeue();
                }*/
                //foreach (var MessageMessageLoadProfileUrl_item in MessageMessageLoadProfileUrl)
                foreach (var MessageMessageLoadProfileUrl_item in MessageMessageLoadProfileUrlQueue)
                {
                    try
                    {
                        string temp=string.Empty;

                        ProfilesCounter++;
                        if (!MessageMessageLoadProfileUrl_item.Contains("https://"))
                        {
                            temp = "https://" + MessageMessageLoadProfileUrl_item;
                        }
                        else
                        {
                            temp =MessageMessageLoadProfileUrl_item;
                        }
                        string PageSource = HttpHelper.getHtmlfromUrl(new Uri(temp));

                        FriendId = GetFriendUserId(ref fbUser,temp);
                        if (string.IsNullOrEmpty(FriendId))
                        { 
                          FriendId=Utils.getBetween(PageSource,"\\/profile.php?id=","&");
                          if (FriendId.Length > 24)
                          {
                              FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "\"");
                          }
                        }
                        UsreId = GlobusHttpHelper.GetParamValue(PageSource, "user");
                        if (string.IsNullOrEmpty(UsreId))
                        {
                            UsreId = GlobusHttpHelper.ParseJson(PageSource, "user");
                        }
                        if (SendMessageUsingMessage == "Single")
                        {
                            MessageFriend = ReplyMessageMessageSingle;
                        }

                        if (SendMessageUsingMessage == "Random")
                        {
                            MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                        }
                        MessageFriend = Uri.EscapeDataString(MessageFriend);
                        try
                        {
                            if (countnumberOfMessagesSent > numberOfMessages)
                            {
                                break;
                            }

                            string FriendUrl = FBGlobals.Instance.fbProfileUrl + FriendId;
                            GlobusLogHelper.log.Debug(countnumberOfMessagesSent + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                            GlobusLogHelper.log.Info(countnumberOfMessagesSent + " Sending Message with " + Username);

                            string PageSrcMsg = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbMessagesUrl));    

                            string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcMsg);
                          
                            string UrlMsgAjax = string.Empty;
                            UrlMsgAjax = FBGlobals.Instance.MessageReplyGetAjaxAsyncDialogUrl + UsreId;                   
                            string PageSrcMsgAjax = HttpHelper.getHtmlfromUrl(new Uri(UrlMsgAjax));


                            if (CheckSendMessageWithTage)
                            {
                                try
                                {
                                    string Name = string.Empty;
                                    string PageSource1 = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + FriendId));
                                    if (PageSource1.Contains("\"name\": \""))
                                    {
                                        try
                                        {
                                            Name = Utils.getBetween(PageSource1, "\"name\": \"", "\",\n");

                                            #region MyRegion

                                            //  Name = System.Web.HttpUtility.(Name);
                                            // Name = Uri.(Name);
                                            //Name = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(Name));
                                            //string asAscii = Encoding.ASCII.GetString(Encoding.UTF8.(Name));
                                            // Name = System.Web.HttpUtility.UrlEncode(Name);
                                            //Name = System.Web.HttpUtility.UrlDecode(Name); 
                                            #endregion
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                    }
                                    if (string.IsNullOrEmpty(Name))
                                    {
                                        try
                                        {
                                            string[] arr = System.Text.RegularExpressions.Regex.Split(PageSource1, "locale\"");
                                            Name = Utils.getBetween(arr[1], "name", "\"\n}").Replace("\"", "").Replace(":", "");
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }

                                    if (MessageFriend.Contains("<>"))
                                    {
                                        string[] Arr = System.Text.RegularExpressions.Regex.Split(MessageFriend, "<>");
                                        MessageFriend = Arr[0] + " " + Name + " " + Arr[1];
                                    }                                  
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            try
                            {
                                if (!ReplyMessageMessageSingle.Contains("www"))
                                {
                                    string postUrlMsg = FBGlobals.Instance.MessageReplyPostAjaxMessagingSendUrl;
                                    string PostDataMsg = "forward_msgs&body=" + MessageFriend + "&action=send&recipients[0]=" + FriendId + "&force_sms=true&fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";

                                    string ResponseMsg = HttpHelper.postFormData(new Uri(postUrlMsg), PostDataMsg, "");

                                    string postUrlMsg1 = FBGlobals.Instance.MessageReplyPostAjaxMessagingPhp;
                                    string PostDataMsg1 = "fb_dtsg=" + fb_dtsg + "&__user=" + UsreId + "&phstamp=";

                                    string ResponseMsg1 = HttpHelper.postFormData(new Uri(postUrlMsg1), PostDataMsg1, "");
                                    string errorSummary = string.Empty;
                                    errorSummary = Utils.getBetween(ResponseMsg1, "errorSummary\":\"", "\"");
                                }


                            //if (!string.IsNullOrEmpty(errorSummary))
                            //    {
                            //         string messageBox = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/messaging/composer.php?ids[0]=" + FriendId + "&ref=timeline&__asyncDialog=1&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgyimEVFLFwxBxCbzElx2ubhHximmey8qUS8zU&__req=d&__rev=1559767"));
                            //         string inputMsg = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/typeahead/first_degree.php?viewer=" + UsreId + "&token=v7&filter[0]=user&options[0]=friends_only&context=messages_bootstrap&request_id=3f97f076-15fb-423e-f92f-02a1e933228e&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=f&__rev=1559767"));
                            //         string messagePostData = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0][timestamp]=1421174982880&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=12%3A19am&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + MessageFriend + "&message_batch[0][has_attachment]=false&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "&message_batch[0][specific_to_list][1]=fbid%3A" + UsreId + "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V3&message_batch[0][status]=0&message_batch[0][message_id]=%3C1421174982880%3A3081462591-2044752731%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A"+FriendId+"&client=mercury&__user="+UsreId+"&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=i&fb_dtsg="+fb_dtsg+"&ttstamp=26581729584113995279115118108&__rev=1559767";

                            //         string messgaePostResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), messagePostData);
                            //     }



                            TotalNoofSeneMessage_Counter++;
                            }
                            catch(Exception ex)
                            {
                              GlobusLogHelper.log.Error(ex.Message);
                            }
                           // ReplyMessageMessageSingle = "https://www.tumblr.com/"; 
                            if (ReplyMessageMessageSingle.Contains("www"))
                            {
                                string MessagePage = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/messages/"+UsreId));
                                string postUrl = Uri.EscapeDataString(ReplyMessageMessageSingle);
                                {
                                string postThumbnail = "u="+postUrl+"&__user="+UsreId+"&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1a&fb_dtsg="+fb_dtsg+"&ttstamp=26581711077653995245897990&__rev=1527549";
                                string postThumbnailResp=HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/share_scrape.php"),postThumbnail);
                                

                                string Subject =Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "subject\\\" value=\\\"", "\\"));
                                string appId = Utils.getBetween(postThumbnailResp, "app_id\\\" value=\\\"", "\\");
                                string favicon = Utils.getBetween(postThumbnailResp, "favicon]\\\" value=\\\"", "\"");
                                favicon=favicon.Replace("/",string.Empty);
                                favicon = Uri.EscapeDataString(favicon);
                                string title = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "title]\\\" value=\\\"", "\\"));
                                string thumbImgSrc = Utils.getBetween(postThumbnailResp, "Thumb img\\\" src=\\\"", "\"");
                                thumbImgSrc = thumbImgSrc.Replace("\\u00253A", ":");
                                thumbImgSrc = thumbImgSrc.Replace("\\u00252F", "/");
                                    
                                thumbImgSrc = thumbImgSrc.Replace("\\", string.Empty);
                                thumbImgSrc = thumbImgSrc.Replace("u002", string.Empty);
                                thumbImgSrc = thumbImgSrc.Replace("amp;", string.Empty);
                                thumbImgSrc = Uri.EscapeDataString(thumbImgSrc);
                                string Medium = Utils.getBetween(postThumbnailResp, "[medium]\\\" value=\\\"", "\\");
                                string type = Utils.getBetween(postThumbnailResp, "type]\\\" value=\\\"", "\\");
                                string domain = Utils.getBetween(postThumbnailResp, "domain]\\\" value=\\\"", "\\");
                                string baseDomain = Utils.getBetween(postThumbnailResp, "base_domain]\\\" value=\\\"", "\\");
                                string title_len = Utils.getBetween(postThumbnailResp, "title_len]\\\" value=\\\"", "\\");
                                string summary_len = Utils.getBetween(postThumbnailResp, "summary_len]\\\" value=\\\"", "\\");
                                string min_dimensions0 = Utils.getBetween(postThumbnailResp, "min_dimensions][0]\\\" value=\\\"", "\\");
                                string min_dimensions1 = Utils.getBetween(postThumbnailResp, "min_dimensions][1]\\\" value=\\\"", "\\");
                                string image_dimensions0 = Utils.getBetween(postThumbnailResp, "image_dimensions][0]\\\" value=\\\"", "\\");
                                string image_dimensions1 = Utils.getBetween(postThumbnailResp, "image_dimensions][1]\\\" value=\\\"", "\\");
                                string summary =Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "params][summary]\\\" value=\\\"", "\\"));
                                //params][summary]\" value=\"
                                string tt = Utils.GenerateTimeStamp();
                                string TT1 =Uri.EscapeDataString(DateTime.Now.ToString("HH:mm"));

                                string postFinalThumbnail = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A"+UsreId+""+
                                                            "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0]"+
                                                            "[timestamp]="+tt+""+
                                                            "&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]="+TT1+"&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]="+postUrl+""+
                                                            "&message_batch[0][has_attachment]=true&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A"+FriendId+""+
                                                            "&message_batch[0][specific_to_list][1]=fbid%3A"+FriendId+""+
                                                            "&message_batch[0][content_attachment][subject]="+Subject+"&message_batch[0][content_attachment][app_id]="+appId+""+
                                                            "&message_batch[0][content_attachment][attachment][params][urlInfo][canonical]="+postUrl+""+
                                                            "&message_batch[0][content_attachment][attachment][params][urlInfo][final]="+postUrl+""+
                                                            "&message_batch[0][content_attachment][attachment][params][urlInfo][user]="+postUrl+""+
                                                            "&message_batch[0][content_attachment][attachment][params][favicon]="+favicon+""+
                                                            "&message_batch[0][content_attachment][attachment][params][title]="+title+""+
                                                            "&message_batch[0][content_attachment][attachment][params][summary]="+summary+""+
                                                            "&message_batch[0][content_attachment][attachment][params][images][0]="+thumbImgSrc+""+
                                                            "&message_batch[0][content_attachment][attachment][params][medium]="+Medium+""+
                                                            "&message_batch[0][content_attachment][attachment][params][url]="+postUrl+""+
                                                            "&message_batch[0][content_attachment][attachment][type]="+type+""+
                                                            "&message_batch[0][content_attachment][link_metrics][source]=ShareStageExternal&message_batch[0][content_attachment][link_metrics][domain]="+domain+""+
                                                            "&message_batch[0][content_attachment][link_metrics][base_domain]="+baseDomain+""+
                                                            "&message_batch[0][content_attachment][link_metrics][title_len]="+title_len+""+
                                                            "&message_batch[0][content_attachment][link_metrics][summary_len]=0"+
                                                            "&message_batch[0][content_attachment][link_metrics][min_dimensions][0]="+min_dimensions0+""+
                                                            "&message_batch[0][content_attachment][link_metrics][min_dimensions][1]="+min_dimensions1+""+
                                                            "&message_batch[0][content_attachment][link_metrics][images_with_dimensions]=2"+
                                                            "&message_batch[0][content_attachment][link_metrics][images_pending]=0"+
                                                            "&message_batch[0][content_attachment][link_metrics][images_fetched]=0"+
                                                            "&message_batch[0][content_attachment][link_metrics][image_dimensions][0]="+image_dimensions0+""+
                                                            "&message_batch[0][content_attachment][link_metrics][image_dimensions][1]="+image_dimensions1+""+
                                                            "&message_batch[0][content_attachment][link_metrics][images_selected]=1"+
                                                            "&message_batch[0][content_attachment][link_metrics][images_considered]=2"+
                                                            "&message_batch[0][content_attachment][link_metrics][images_cap]=3"+
                                                            "&message_batch[0][content_attachment][link_metrics][images_type]=ranked"+
                                                            "&message_batch[0][content_attachment][composer_metrics][best_image_w]=100"+
                                                            "&message_batch[0][content_attachment][composer_metrics][best_image_h]=100"+
                                                            "&message_batch[0][content_attachment][composer_metrics][image_selected]=0"+
                                                            "&message_batch[0][content_attachment][composer_metrics][images_provided]=1"+
                                                            "&message_batch[0][content_attachment][composer_metrics][images_loaded]=1"+
                                                            "&message_batch[0][content_attachment][composer_metrics][images_shown]=1"+
                                                            "&message_batch[0][content_attachment][composer_metrics][load_duration]=4136"+
                                                            "&message_batch[0][content_attachment][composer_metrics][timed_out]=0"+
                                                            "&message_batch[0][content_attachment][composer_metrics][sort_order]="+
                                                            "&message_batch[0][content_attachment][composer_metrics][selector_type]=UIThumbPager_6"+
                                                            "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V2p"+
                                                            "&message_batch[0][status]=0&message_batch[0][message_id]=%3C"+tt+"%3A3925954697-2587754773%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A"+FriendId+""+
                                                            "&client=web_messenger&__user="+UsreId+"&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1c"+
                                                            "&fb_dtsg="+fb_dtsg+"&ttstamp=26581711077653995245897990&__rev=1527549";

                                

                                string ThreadPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to="+FriendId+"&source=mercury-chat&thread="+FriendId+"&__user="+UsreId+"&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg="+fb_dtsg+"&ttstamp=2658170831091124811651677495&__rev=1694181");
                                string ThreadPost1 = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to=" + FriendId + "&source=mercury-chat&thread=" + FriendId + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170831091124811651677495&__rev=1694181");
                               
                                string postFinalTumbnailresp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), postFinalThumbnail, "https://www.facebook.com/messages/" + FriendId + "");
                                string message_id=Utils.getBetween(postFinalTumbnailresp,"message_id\":\"","\"");
                                //string 
                              

                                string postFinalTumbnail1 = "message_ids[0]=" + message_id + "&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1e&fb_dtsg="+fb_dtsg+"&ttstamp=26581711077653995245897990&__rev=1527549";
                                string postFinalResult = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/attachments/fetch_shares.php"), postFinalTumbnail1);

                               
                            
                                }
                            }
                            //GlobusLogHelper.log.Debug(countnumberOfMessagesSent + " Sent Message with " + Username);
                            //GlobusLogHelper.log.Info(countnumberOfMessagesSent + " Sent Message with " + Username);

                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
                                GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                Thread.Sleep(delayInSeconds);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            countnumberOfMessagesSent++;
                            if (useDivideDataOption)
                            {
                                if ((NoProfileUrlsPerUser / ProfilesCounter) == 1)
                                {
                                    MessageMessageLoadProfileUrlQueue.Dequeue();
                                    ProfilesCounter = 0;
                                    break;
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

        public void SendMessageWithImage(ref FacebookUser fbUser)
        {
            // lock(this)
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string MessageFriend = string.Empty;
                string Username = string.Empty;
                Username = fbUser.username;
                string FriendId = string.Empty;
                string UsreId = string.Empty;
                numberOfMessages = MessageMessageSendNoOfFriends;

                try
                {
                    if (MessageMessageLoadProfileUrlQueue.Count == 0)
                    {
                        string PageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com"));
                        UsreId = GlobusHttpHelper.GetParamValue(PageSource, "user");
                        if (string.IsNullOrEmpty(UsreId))
                        {
                            UsreId = GlobusHttpHelper.ParseJson(PageSource, "user");
                        }
                        int count_Friends = ExtractFriendCount(ref fbUser, UsreId);
                        List<string> lstFriend = new List<string>();
                        lstFriend.Clear();

                        lstFriend = ExtractFriendIdsFb(ref fbUser, ref UsreId, count_Friends);
                        for(int i=0;i<MessageMessageSendNoOfFriends;i++)
                        {
                            MessageMessageLoadProfileUrlQueue.Enqueue("https://www.facebook.com/"+lstFriend[i]);
                        }
                    }
                    foreach (var MessageMessageLoadProfileUrl_item in MessageMessageLoadProfileUrlQueue)
                    {
                        try
                        {
                            string temp = string.Empty;

                            ProfilesCounter++;
                            if (!MessageMessageLoadProfileUrl_item.Contains("https://"))
                            {
                                temp = "https://" + MessageMessageLoadProfileUrl_item;
                            }
                            else
                            {
                                temp = MessageMessageLoadProfileUrl_item;
                            }
                            string PageSource = HttpHelper.getHtmlfromUrl(new Uri(temp));

                            FriendId = GetFriendUserId(ref fbUser, temp);
                            if (string.IsNullOrEmpty(FriendId))
                            {
                                FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "&");
                                if (FriendId.Length > 24)
                                {
                                    FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "\"");
                                }
                            }
                            UsreId = GlobusHttpHelper.GetParamValue(PageSource, "user");
                            if (string.IsNullOrEmpty(UsreId))
                            {
                                UsreId = GlobusHttpHelper.ParseJson(PageSource, "user");
                            }
                            if (SendMessageUsingMessage == "Single")
                            {
                                MessageFriend = ReplyMessageMessageSingle;
                            }

                            if (SendMessageUsingMessage == "Random")
                            {
                                MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                            }
                            MessageFriend = Uri.EscapeDataString(MessageFriend);
                            try
                            {
                                if (countnumberOfMessagesSent > numberOfMessages)
                                {
                                    break;
                                }

                                string FriendUrl = FBGlobals.Instance.fbProfileUrl + FriendId;
                                GlobusLogHelper.log.Debug(countnumberOfMessagesSent + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                GlobusLogHelper.log.Info(countnumberOfMessagesSent + " Sending Message with " + Username);

                                string PageSrcMsg = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbMessagesUrl));

                                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcMsg);

                                string UrlMsgAjax = string.Empty;
                                UrlMsgAjax = FBGlobals.Instance.MessageReplyGetAjaxAsyncDialogUrl + UsreId;
                                string PageSrcMsgAjax = HttpHelper.getHtmlfromUrl(new Uri(UrlMsgAjax));


                                if (CheckSendMessageWithTage)
                                {
                                    try
                                    {
                                        string Name = string.Empty;
                                        string PageSource1 = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + FriendId));
                                        if (PageSource1.Contains("\"name\": \""))
                                        {
                                            try
                                            {
                                                Name = Utils.getBetween(PageSource1, "\"name\": \"", "\",\n");
                                               
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }
                                        if (string.IsNullOrEmpty(Name))
                                        {
                                            try
                                            {
                                                string[] arr = System.Text.RegularExpressions.Regex.Split(PageSource1, "locale\"");
                                                Name = Utils.getBetween(arr[1], "name", "\"\n}").Replace("\"", "").Replace(":", "");
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        if (MessageFriend.Contains("<>"))
                                        {
                                            string[] Arr = System.Text.RegularExpressions.Regex.Split(MessageFriend, "<>");
                                            MessageFriend = Arr[0] + " " + Name + " " + Arr[1];
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                try
                                {
                                    string imagepath = string.Empty;
                                    //MessageFriend=Uri.EscapeUriString(MessageFriend);
                                    if (qImagePathMesssage.Count != 0)
                                    {
                                        imagepath = qImagePathMesssage.Dequeue();
                                    }
                                    else
                                    { 
                                      imagepath=lstImagePathMessage[new Random().Next(0,lstImagePathMessage.Count-1)];
                                    }
                                    string AjaxPipeToken = string.Empty;
                                    string QNVal = string.Empty;
                                    AjaxPipeToken = Utils.getBetween(PageSrcMsg, "ajaxpipe_token\":\"", "\"");
                                   
                                    QNVal = Utils.getBetween(PageSrcMsg, "waterfallID\":\"", "\"");
                                    NameValueCollection nvc = new NameValueCollection();
                                    nvc.Add("fb_dtsg", fb_dtsg);
                                    nvc.Add("source", "8");
                                    nvc.Add("profile_id",UsreId);
                                    nvc.Add("grid_id", "u_jsonp_2_4");
                                    nvc.Add("qn",QNVal);
                                    nvc.Add("0", "" + imagepath + "<:><:><:>image/jpeg");
                                    nvc.Add("upload_id","1024");

                                    string ImagePostResp = HttpHelper.UploadImageWaterfallModel("https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?qn=" + QNVal + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9r88DgDxiWEyx97xN6yUgByVbGAF9oyut9LO0xBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=q&fb_dtsg=" + fb_dtsg + "&ttstamp=265817095491141126911211310183&__rev=1696458", "https://www.facebook.com/messages/" + UsreId,nvc,"upload_id","0");
                                    string PhotoFbId = Utils.getBetween(ImagePostResp,"photoFBID\":",",");
                                    string TimeStamp = Utils.GenerateTimeStamp();
                                    string CurrentTime =Uri.EscapeDataString(DateTime.Now.ToString("hh:mm"));
                                    string PostPhotoId = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0][timestamp]=" + TimeStamp + "&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=" + CurrentTime + "&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + MessageFriend + "&message_batch[0][has_attachment]=true&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "&message_batch[0][specific_to_list][1]=fbid%3A" + UsreId + "&message_batch[0][photo_fbids][0]=" + PhotoFbId + "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V3&message_batch[0][status]=0&message_batch[0][message_id]=%3C" + TimeStamp + "%3A269461266-1713744050%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A" + FriendId + "&client=web_messenger&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9r88DgDxiWEyx97xN6yUgByVbGAF9oyut9LO0xBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=10&fb_dtsg=" + fb_dtsg + "&ttstamp=265817095491141126911211310183&__rev=1696458";
                                    string PostPhotoResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), PostPhotoId);
                                 
                                    string errorSummary = string.Empty;
                                    errorSummary = Utils.getBetween(PostPhotoResp, "errorSummary\":\"", "\"");
                                    string messageId = string.Empty;
                                    messageId = Utils.getBetween(PostPhotoResp,"message_id\":\"","\"");

                                    if (string.IsNullOrEmpty(errorSummary) && !string.IsNullOrEmpty(messageId))
                                    {
                                        GlobusLogHelper.log.Info("Message Successfully Sent With UserId " + UsreId + " To FriendId " + FriendId);
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("Failed To Send With UserId " + UsreId + " To FriendId " + FriendId);
                                    }

                                    TotalNoofSeneMessage_Counter++;
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.Message);
                                }
                                // ReplyMessageMessageSingle = "https://www.tumblr.com/";                                 
                                
                            

                                try
                                {
                                    int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
                                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
                                    Thread.Sleep(delayInSeconds);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                countnumberOfMessagesSent++;
                                if (useDivideDataOption)
                                {
                                    if ((NoProfileUrlsPerUser / ProfilesCounter) == 1)
                                    {
                                        MessageMessageLoadProfileUrlQueue.Dequeue();
                                        ProfilesCounter = 0;
                                        break;
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

        public void SendMessageTargetedProfileWithDivideData(ref FacebookUser fbUser)
        {
            //lock (this)
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string MessageFriend = string.Empty;
                string Username = string.Empty;
                Username = fbUser.username;
                string FriendId = string.Empty;
                string UsreId = string.Empty;
                numberOfMessages = MessageMessageSendNoOfFriends;
                lstFBAccounts.Remove(Username);

                try
                {

                    //foreach (var MessageMessageLoadProfileUrl_item in MessageMessageLoadProfileUrlQueue)
                    int count = 0;
                    for (int i = 0; i < NoProfileUrlsPerUser; i++)
                    {
                        count = i;
                        string MessageMessageLoadProfileUrl_item = MessageMessageLoadProfileUrlQueue.Dequeue();
                        try
                        {
                            string temp = string.Empty;

                            ProfilesCounter++;
                            if (!MessageMessageLoadProfileUrl_item.Contains("https://"))
                            {
                                temp = "https://" + MessageMessageLoadProfileUrl_item;
                            }
                            else
                            {
                                temp = MessageMessageLoadProfileUrl_item;
                            }
                            string PageSource = HttpHelper.getHtmlfromUrl(new Uri(temp));

                            FriendId = GetFriendUserId(ref fbUser, temp);
                            if (string.IsNullOrEmpty(FriendId))
                            {
                                FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "&");
                                if (FriendId.Length > 24)
                                {
                                    FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "\"");
                                }
                            }
                            UsreId = GlobusHttpHelper.GetParamValue(PageSource, "user");
                            if (string.IsNullOrEmpty(UsreId))
                            {
                                UsreId = GlobusHttpHelper.ParseJson(PageSource, "user");
                            }
                            if (SendMessageUsingMessage == "Single")
                            {
                                MessageFriend = ReplyMessageMessageSingle;
                            }

                            if (SendMessageUsingMessage == "Random")
                            {
                                MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                            }

                            MessageFriend = Uri.EscapeDataString(MessageFriend);
                            string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSource);
                            try
                            {
                                string messgaePostResp = string.Empty;
                                if (!ReplyMessageMessageSingle.Contains("www"))
                                {
                                    string messageBox = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/messaging/composer.php?ids[0]=" + FriendId + "&ref=timeline&__asyncDialog=1&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgyimEVFLFwxBxCbzElx2ubhHximmey8qUS8zU&__req=d&__rev=1559767"));
                                    string inputMsg = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/typeahead/first_degree.php?viewer=" + UsreId + "&token=v7&filter[0]=user&options[0]=friends_only&context=messages_bootstrap&request_id=3f97f076-15fb-423e-f92f-02a1e933228e&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=f&__rev=1559767"));
                                    string messagePostData = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0][timestamp]=1421174982880&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=12%3A19am&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + MessageFriend + "&message_batch[0][has_attachment]=false&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "&message_batch[0][specific_to_list][1]=fbid%3A" + UsreId + "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V3&message_batch[0][status]=0&message_batch[0][message_id]=%3C1421174982880%3A3081462591-2044752731%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A" + FriendId + "&client=mercury&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=i&fb_dtsg=" + fb_dtsg + "&ttstamp=26581729584113995279115118108&__rev=1559767";

                                    messgaePostResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), messagePostData);
                                }
                                if (ReplyMessageMessageSingle.Contains("www"))
                                {
                                    string MessagePage = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/messages/" + UsreId));
                                    string postUrl = Uri.EscapeDataString(ReplyMessageMessageSingle);

                                    string postThumbnail = "u=" + postUrl + "&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1a&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";
                                    string postThumbnailResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/share_scrape.php"), postThumbnail);


                                    string Subject = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "subject\\\" value=\\\"", "\\"));
                                    string appId = Utils.getBetween(postThumbnailResp, "app_id\\\" value=\\\"", "\\");
                                    string favicon = Utils.getBetween(postThumbnailResp, "favicon]\\\" value=\\\"", "\"");
                                    favicon = favicon.Replace("/", string.Empty);
                                    favicon = Uri.EscapeDataString(favicon);
                                    string title = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "title]\\\" value=\\\"", "\\"));
                                    string thumbImgSrc = Utils.getBetween(postThumbnailResp, "Thumb img\\\" src=\\\"", "\"");
                                    thumbImgSrc = thumbImgSrc.Replace("\\u00253A", ":");
                                    thumbImgSrc = thumbImgSrc.Replace("\\u00252F", "/");

                                    thumbImgSrc = thumbImgSrc.Replace("\\", string.Empty);
                                    thumbImgSrc = thumbImgSrc.Replace("u002", string.Empty);
                                    thumbImgSrc = thumbImgSrc.Replace("amp;", string.Empty);
                                    thumbImgSrc = Uri.EscapeDataString(thumbImgSrc);
                                    string Medium = Utils.getBetween(postThumbnailResp, "[medium]\\\" value=\\\"", "\\");
                                    string type = Utils.getBetween(postThumbnailResp, "type]\\\" value=\\\"", "\\");
                                    string domain = Utils.getBetween(postThumbnailResp, "domain]\\\" value=\\\"", "\\");
                                    string baseDomain = Utils.getBetween(postThumbnailResp, "base_domain]\\\" value=\\\"", "\\");
                                    string title_len = Utils.getBetween(postThumbnailResp, "title_len]\\\" value=\\\"", "\\");
                                    string summary_len = Utils.getBetween(postThumbnailResp, "summary_len]\\\" value=\\\"", "\\");
                                    string min_dimensions0 = Utils.getBetween(postThumbnailResp, "min_dimensions][0]\\\" value=\\\"", "\\");
                                    string min_dimensions1 = Utils.getBetween(postThumbnailResp, "min_dimensions][1]\\\" value=\\\"", "\\");
                                    string image_dimensions0 = Utils.getBetween(postThumbnailResp, "image_dimensions][0]\\\" value=\\\"", "\\");
                                    string image_dimensions1 = Utils.getBetween(postThumbnailResp, "image_dimensions][1]\\\" value=\\\"", "\\");
                                    string summary = Uri.EscapeDataString(Utils.getBetween(postThumbnailResp, "params][summary]\\\" value=\\\"", "\\"));
                                    //params][summary]\" value=\"
                                    string tt = Utils.GenerateTimeStamp();
                                    string TT1 = Uri.EscapeDataString(DateTime.Now.ToString("HH:mm"));

                                    string postFinalThumbnail = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "" +
                                                                "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0]" +
                                                                "[timestamp]=" + tt + "" +
                                                                "&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=" + TT1 + "&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + postUrl + "" +
                                                                "&message_batch[0][has_attachment]=true&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "" +
                                                                "&message_batch[0][specific_to_list][1]=fbid%3A" + FriendId + "" +
                                                                "&message_batch[0][content_attachment][subject]=" + Subject + "&message_batch[0][content_attachment][app_id]=" + appId + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][urlInfo][canonical]=" + postUrl + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][urlInfo][final]=" + postUrl + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][urlInfo][user]=" + postUrl + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][favicon]=" + favicon + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][title]=" + title + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][summary]=" + summary + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][images][0]=" + thumbImgSrc + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][medium]=" + Medium + "" +
                                                                "&message_batch[0][content_attachment][attachment][params][url]=" + postUrl + "" +
                                                                "&message_batch[0][content_attachment][attachment][type]=" + type + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][source]=ShareStageExternal&message_batch[0][content_attachment][link_metrics][domain]=" + domain + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][base_domain]=" + baseDomain + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][title_len]=" + title_len + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][summary_len]=0" +
                                                                "&message_batch[0][content_attachment][link_metrics][min_dimensions][0]=" + min_dimensions0 + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][min_dimensions][1]=" + min_dimensions1 + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_with_dimensions]=2" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_pending]=0" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_fetched]=0" +
                                                                "&message_batch[0][content_attachment][link_metrics][image_dimensions][0]=" + image_dimensions0 + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][image_dimensions][1]=" + image_dimensions1 + "" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_selected]=1" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_considered]=2" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_cap]=3" +
                                                                "&message_batch[0][content_attachment][link_metrics][images_type]=ranked" +
                                                                "&message_batch[0][content_attachment][composer_metrics][best_image_w]=100" +
                                                                "&message_batch[0][content_attachment][composer_metrics][best_image_h]=100" +
                                                                "&message_batch[0][content_attachment][composer_metrics][image_selected]=0" +
                                                                "&message_batch[0][content_attachment][composer_metrics][images_provided]=1" +
                                                                "&message_batch[0][content_attachment][composer_metrics][images_loaded]=1" +
                                                                "&message_batch[0][content_attachment][composer_metrics][images_shown]=1" +
                                                                "&message_batch[0][content_attachment][composer_metrics][load_duration]=4136" +
                                                                "&message_batch[0][content_attachment][composer_metrics][timed_out]=0" +
                                                                "&message_batch[0][content_attachment][composer_metrics][sort_order]=" +
                                                                "&message_batch[0][content_attachment][composer_metrics][selector_type]=UIThumbPager_6" +
                                                                "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V2p" +
                                                                "&message_batch[0][status]=0&message_batch[0][message_id]=%3C" + tt + "%3A3925954697-2587754773%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A" + FriendId + "" +
                                                                "&client=web_messenger&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1c" +
                                                                "&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";



                                    string ThreadPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to=" + FriendId + "&source=mercury-chat&thread=" + FriendId + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170831091124811651677495&__rev=1694181");
                                    string ThreadPost1 = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/messaging/typ.php"), "typ=1&to=" + FriendId + "&source=mercury-chat&thread=" + FriendId + "&__user=" + UsreId + "&__a=1&__dyn=7nmanEyl2lm9o-t2u5bGya4Au74qbx2mbAKGiyEyut9LFwxBxC9V8CdwIhEyfyUnwPUS2O4K5e8GQ8GqcGEyryXw&__req=45&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170831091124811651677495&__rev=1694181");

                                    messgaePostResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), postFinalThumbnail, "https://www.facebook.com/messages/" + FriendId + "");
                                    string message_id = Utils.getBetween(messgaePostResp, "message_id\":\"", "\"");
                                    //string 


                                    string postFinalTumbnail1 = "message_ids[0]=" + message_id + "&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9v88DgDxyIGzGpUW9ACxO4pbGAt4BGeqrWo8pojByUWumu49UJ6K59poW8xHzoyfw&__req=1e&fb_dtsg=" + fb_dtsg + "&ttstamp=26581711077653995245897990&__rev=1527549";
                                    string postFinalResult = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/attachments/fetch_shares.php"), postFinalTumbnail1);



                                }

                                if (messgaePostResp.Contains("errorSummary"))
                                {                                  
                                  GlobusLogHelper.log.Debug((i+1) + " Unable To Send Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                  GlobusLogHelper.log.Info((i+1) + " Unable To Send Message with " + Username+ "To Profiel" + MessageMessageLoadProfileUrl_item);
                                }
                                if (messgaePostResp.Contains("message_id"))
                                {
                                    GlobusLogHelper.log.Debug((i+1) + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                    GlobusLogHelper.log.Info((i+1) + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.Message);
                            }
                            try
                            {
                                int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
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
                            GlobusLogHelper.log.Error(ex.Message);
                        }
                    }
                    if (lstFBAccounts.Count == 0)
                    {
                        while (MessageMessageLoadProfileUrlQueue.Count > 0)
                        {
                            string MessageMessageLoadProfileUrl_item = MessageMessageLoadProfileUrlQueue.Dequeue();
                            try
                            {
                                string temp = string.Empty;

                                ProfilesCounter++;
                                if (!MessageMessageLoadProfileUrl_item.Contains("https://"))
                                {
                                    temp = "https://" + MessageMessageLoadProfileUrl_item;
                                }
                                else
                                {
                                    temp = MessageMessageLoadProfileUrl_item;
                                }
                                string PageSource = HttpHelper.getHtmlfromUrl(new Uri(temp));

                                FriendId = GetFriendUserId(ref fbUser, temp);
                                if (string.IsNullOrEmpty(FriendId))
                                {
                                    FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "&");
                                    if (FriendId.Length > 24)
                                    {
                                        FriendId = Utils.getBetween(PageSource, "\\/profile.php?id=", "\"");
                                    }
                                }
                                UsreId = GlobusHttpHelper.GetParamValue(PageSource, "user");
                                if (string.IsNullOrEmpty(UsreId))
                                {
                                    UsreId = GlobusHttpHelper.ParseJson(PageSource, "user");
                                }
                                if (SendMessageUsingMessage == "Single")
                                {
                                    MessageFriend = ReplyMessageMessageSingle;
                                }

                                if (SendMessageUsingMessage == "Random")
                                {
                                    MessageFriend = LstReplyMessageMessageReply[Utils.GenerateRandom(0, LstReplyMessageMessageReply.Count)];
                                }
                                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSource);
                                try
                                {
                                    string messageBox = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/messaging/composer.php?ids[0]=" + FriendId + "&ref=timeline&__asyncDialog=1&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgyimEVFLFwxBxCbzElx2ubhHximmey8qUS8zU&__req=d&__rev=1559767"));
                                    string inputMsg = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/typeahead/first_degree.php?viewer=" + UsreId + "&token=v7&filter[0]=user&options[0]=friends_only&context=messages_bootstrap&request_id=3f97f076-15fb-423e-f92f-02a1e933228e&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=f&__rev=1559767"));
                                    string messagePostData = "message_batch[0][action_type]=ma-type%3Auser-generated-message&message_batch[0][thread_id]&message_batch[0][author]=fbid%3A" + UsreId + "&message_batch[0][author_email]&message_batch[0][coordinates]&message_batch[0][timestamp]=1421174982880&message_batch[0][timestamp_absolute]=Today&message_batch[0][timestamp_relative]=12%3A19am&message_batch[0][timestamp_time_passed]=0&message_batch[0][is_unread]=false&message_batch[0][is_cleared]=false&message_batch[0][is_forward]=false&message_batch[0][is_filtered_content]=false&message_batch[0][is_spoof_warning]=false&message_batch[0][source]=source%3Atitan%3Aweb&&message_batch[0][body]=" + MessageFriend + "&message_batch[0][has_attachment]=false&message_batch[0][html_body]=false&&message_batch[0][specific_to_list][0]=fbid%3A" + FriendId + "&message_batch[0][specific_to_list][1]=fbid%3A" + UsreId + "&message_batch[0][force_sms]=true&message_batch[0][ui_push_phase]=V3&message_batch[0][status]=0&message_batch[0][message_id]=%3C1421174982880%3A3081462591-2044752731%40mail.projektitan.com%3E&message_batch[0][manual_retry_cnt]=0&&message_batch[0][client_thread_id]=user%3A" + FriendId + "&client=mercury&__user=" + UsreId + "&__a=1&__dyn=7nmajEyl2qm9vyVQ9UoHaEWCueyp9Esx6iWF299qzCC-C26m6oKexm49UJ6K59poW8xHzoyfw&__req=i&fb_dtsg=" + fb_dtsg + "&ttstamp=26581729584113995279115118108&__rev=1559767";

                                    string messgaePostResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/mercury/send_messages.php"), messagePostData);

                                    if (messgaePostResp.Contains("errorSummary"))
                                    {
                                        GlobusLogHelper.log.Debug(count + " Unable To Send Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                        GlobusLogHelper.log.Info(count + " Unable To Send Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                    }
                                    if (messgaePostResp.Contains("message_id"))
                                    {
                                        GlobusLogHelper.log.Debug(count + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                        GlobusLogHelper.log.Info(count + " Sending Message with " + Username + "To Profiel" + MessageMessageLoadProfileUrl_item);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(ex.Message);
                                }
                                try
                                {
                                    int delayInSeconds = Utils.GenerateRandom(minDelayMessageReply * 1000, maxDelayMessageReply * 1000);
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

        public string GetFriendUserId(ref FacebookUser fbUser, string profileUrl)
        {
            string FriendsId = string.Empty;
            try
            {

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
                string pageSource = string.Empty;
                string Profile_Id = profileUrl;
                string ProfileNewUrl = string.Empty;

                profileUrl = profileUrl.Replace("https://www.facebook.com", "https://graph.facebook.com");
                if (profileUrl.Contains("profile.php?"))
                {
                    profileUrl = profileUrl.Replace("profile.php?id=", string.Empty);
                }
                pageSource = HttpHelper.getHtmlfromUrl(new Uri(profileUrl));

                FriendsId = getBetween(pageSource, "\"id\": \"", "\",\n");
                if (string.IsNullOrEmpty(FriendsId) || !Utils.IsNumeric(FriendsId))
                {
                    FriendsId = Utils.getBetween(Profile_Id, "php?id=", "&");
                    ProfileNewUrl = "https://graph.facebook.com/" + FriendsId;
                    pageSource = HttpHelper.getHtmlfromUrl(new Uri(ProfileNewUrl));
                    FriendsId = getBetween(pageSource, "\"id\": \"", "\",\n");

                }
               
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


            return FriendsId;
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

    }
}
