using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using faceboardpro;
using System.Reflection;
using log4net;
using log4net.Config;
using Globussoft;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using faceboardpro.Domain;
using BaseLib;
using faceboardpro.Model;
using faceboardpro.Domain;
using Accounts;
using Friends;
using Pages;
using Captchas;
using Events;
using Scrapers;
using Messages;
using Groups;
using Photos;
using Campaigns;
using Scheduler;
using System.Security.Permissions;
using System.IO;
using WallPoster;
using System.Web;
using Search;
using Proxies;

namespace faceboardpro
{
    public partial class FrmDominator : Form
    {

        Utils objUtils = new Utils();
        AccountManager ObjAccountManager = new AccountManager();
        AccountRepository objAccountRepository = new AccountRepository();
        SettingRepository objSettingRepository = new SettingRepository();
        Setting objSetting = new Setting();
        FriendManager objFriendManager = new Friends.FriendManager();
        PageManager objPageManager = new PageManager();
        FanPageStatusRepository objFanPageStatusRepository = new FanPageStatusRepository();
        FanPagePostRepository objFanPagePostRepository = new FanPagePostRepository();
        EmailRepository objEmailRepository = new EmailRepository();
        CaptchaRepository objCaptchaRepositry = new CaptchaRepository();
        FanPageDataRepository objFanPageDataRepository = new FanPageDataRepository();
        GroupCampaignManagerRepository objGroupCampaignManagerRepository = new GroupCampaignManagerRepository();
        GroupCompaignReportRepository objGroupCompaignReportRepository = new GroupCompaignReportRepository();
        GroupRequestUniqueRepository objGroupRequestUniqueRepository = new GroupRequestUniqueRepository();
        SearchScraper ObjSearchScraper = new SearchScraper();

        FrmCaptcha objFrmCaptcha = new FrmCaptcha();
        EventManager objEventManager = new EventManager();
        EventScraper objEventScraper = new EventScraper();

        // SearchScraper  
        GroupRequestUnique objGroupRequestUnique = new GroupRequestUnique();
        MessageScraper objMessageScraper = new MessageScraper();
        faceboardpro.Domain.Message objMessage = new faceboardpro.Domain.Message();
        faceboardpro.Domain.GroupCompaignReport objGroupCompaignReport = new faceboardpro.Domain.GroupCompaignReport();
        MessageRepository objMessageRepository = new MessageRepository();
        MessageManager objMessageManager = new MessageManager();
        GroupManager objGroupManager = new GroupManager();
        SearchScraper objSearchScraper = new SearchScraper();

        PhotoManager objPhotoManager = new PhotoManager();
        Proxies.ProxyManager objProxyManager = new Proxies.ProxyManager();
        WallPostManager objWallPostManager = new WallPostManager();

        FriendInfoScraper objFriendInfoScraper = new FriendInfoScraper();
        CustomAudiencesScraper objCustomAudiencesScraper = new CustomAudiencesScraper();
        


        #region Global Variables Account Creator

        //public bool isStopAccountsAccountCreator = false;
        //public List<Thread> lstThreadStopAccountsAccountCreator = new List<Thread>();

        List<string> lstLoadedEmails = new List<string>();
        List<string> lstLoadedFirstNames = new List<string>();
        List<string> lstLoadedLastNames = new List<string>();

        List<string> lstLoadedEmailsForEmail = new List<string>();
        List<string> lstLoadedFirstNamesForEmail = new List<string>();
        List<string> lstLoadedLastNamesForEmail = new List<string>();
        List<string> lstProxies = new List<string>();

        #endregion


        #region Global Variables EditProfileName

        List<string> lstLoadedNames = new List<string>();
        List<string> lstLoadedEmailsForEditProfileName = new List<string>();

        #endregion


        #region Global Variables Account Varification

        List<string> lstLoadedEmailsAccountVarification = new List<string>();


        #endregion


        #region Global Variables CmpFanPageLiker

        public List<string> lstLoadedAccountsCmpFanPageLiker = new List<string>();
        public List<string> lstLoadedFanPageURLsCmpFanPageLiker = new List<string>();
        public List<string> lstLoadedFanPageMessagesCmpFanPageLiker = new List<string>();


        #endregion

        #region Global Variables ManageProfiles

        public List<string> lstLoadedPicsManageProfiles = new List<string>();
        public List<string> lstCoverPicsManageProfiles = new List<string>();

        #endregion

        #region Global Variables ProxyChecker

        public List<string> lstLoadedProxyProxyChecker = new List<string>();

        #endregion

        # region Global Variables for Captcha Setting

        public static string CurrentCaptchaSetting = string.Empty;

        public static string CurrentCaptchaUserName = string.Empty;
        public static string CurrentCaptchaPassword = string.Empty;


        public static bool isExitDBCSetting = false;
        public static bool isExitDecaptcherSetting = false;
        public static bool isExitAnigateSetting = false;
        public static bool isExitImageTypezSetting = false;

        //public static bool CheckStopAccountCreation = true;

        #endregion

        public FrmDominator()
        {
            InitializeComponent();
       
            XmlConfigurator.Configure();

        }

     
     
        private void FrmDominator_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'faceDominatorDataSet2.FanPagePost' table. You can move, or remove it, as needed.
          
            CopyDataBase();  //Copy database 

           
            Brush background_brush = new SolidBrush(Color.FromArgb(226, 75, 22));
           
            tabMain.TabPages.RemoveAt(11);     //Remove("tabPage7");
           // tabFriends.TabPages.RemoveAt(2);
            tabGroup.TabPages.RemoveAt(5);
            try
            {
                 // remove Account Creator Tab 
                string tabToRemove = "tabPageAccounts";
                for (int i = 0; i < tabMain.TabPages.Count; i++)
                {
                    if (tabMain.TabPages[i].Name.Equals(tabToRemove, StringComparison.OrdinalIgnoreCase))
                    {
                        //tabMain.TabPages.RemoveAt(i);
                        tabAccounts.TabPages.Remove(tabAccounts.TabPages["tabPageAccountCreator"]);
                        tabAccounts.TabPages.Remove(tabAccounts.TabPages["tabPageManageProfile"]);
                       // tabAccounts.TabPages.Remove(tabAccounts.TabPages["tabPageAccountVerification"]);
                        
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
         
           
           
            tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
            tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];

            //Code to print Version in form title

        


            //GlobusLogHelper.log.Info("data");
           
                // GlobusLogHelper.log.Info("Info Is working");

                //Code to print Version in form title            
               // this.Text = "FaceDominator Version : " + GetAssemblyVersion();

                //Accounts.AccountManager.accountCreationEvent.handleParamsEvent += new EventHandler(AccountCreationEvent_handleParamsEvent);
           
           // LoadFormMethod();
                try
                {
                    Thread ObjNew = new Thread(LoadFormMethod);
                    ObjNew.Start();
                  
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
        }

        private void LoadFormMethod()
        {
            objPageManager.pageManagerEvent.handleParamsEvent += new EventHandler(pageManagerEvent_handleParamsEvent);
            objFrmCaptcha.pageManagerEvent.handleParamsEvent += new EventHandler(pageManagerEvent_handleParamsEvent);
            objMessageScraper.messageScraperEvent.handleParamsEvent += new EventHandler(messageScraperEvent_handleParamsEvent);
            objGroupManager.groupCamMgrEvent.handleParamsEvent += new EventHandler(GroupManagerEvent_handleParamsEvent);
            //objGroupManager.groupRequestUnique.handleParamsEvent += new EventHandler(groupRequestUnique_handleParamsEvent);


            //cmoAccounts_AccountCreator_Gender.Invoke(new MethodInvoker(delegate
            //{
            //    cmoAccounts_AccountCreator_Gender.SelectedIndex = 2;

            //}));

           

            BindAccountCreatorFilePath();

            //BindEmailCreatorFilePath();

            BindAccountCreatorFilePath();

            CreateAccountTable();

            CreateEmailTable();

            GetAllEmails();

       //     GetAllCaptchasetting();

            loadCampaignDataInScheduler();
       
        }

        static string thisVersionNumber = string.Empty;

      

        public string GetAssemblyNumber()
        {
            string appName = Assembly.GetAssembly(this.GetType()).Location;
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(appName);
            string versionNumber = assemblyName.Version.ToString();
            return versionNumber;
        }

        public void loadCampaignDataInScheduler()
        {
            try
            {
                CmbGroups_GroupCampaignManager_EditCampaign.Items.Clear();
                GroupCampaign objgroupCamp = new GroupCampaign();
                //ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                objgroupCamp.Module = "Group Posting";
                ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.SelectCampaigns(objgroupCamp);
                foreach (GroupCampaign item in groupCollection)
                {
                    //string groupname = item.GroupCampaignName;
                    CmbGroups_GroupCampaignManager_EditCampaign.Items.Add(item.GroupCampaignName);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        void messageScraperEvent_handleParamsEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eArgs = e as EventsArgs;

                    MessageScraperQuery(eArgs.ds, eArgs.paramsData);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        void GroupManagerEvent_handleParamsEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eArgs = e as EventsArgs;

                    GroupManagerQuery(eArgs.ds, eArgs.paramsData);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void GroupManagerQuery(DataSet ds, params string[] paramValue)
        {
            try
            {
                if (paramValue.Length > 1)
                {
                    if (paramValue[0] == "Model : GroupCompaignRepository")
                    {
                        if (paramValue[1] == "Function : GetAllGroupCompaignReport")
                        {
                            //GetAllGroupCompaignReport
                            try
                            {

                                objGroupCompaignReport.GroupId = paramValue[2];
                                objGroupCompaignReport.MessageText = paramValue[3];

                                ICollection<faceboardpro.Domain.GroupCompaignReport> lstMsg = objGroupCompaignReportRepository.GetAllGroupCompaignReport(objGroupCompaignReport);
                                //objGroupCompaignReportRepository.CheckGroupCompaignReport=objGroupCompaignReportRepository.GetAllGroupCompaignReport(objGroupCompaignReport);
                                if (lstMsg.Count == 0)
                                {
                                    PageManager.CheckDataBase = false;
                                }
                                else
                                {

                                }

                                #region MyRegion
                                //  if (lstMsg.Count > 0)
                                //{
                                //objMessage.MsgDate = paramValue[10];
                                //objMessageRepository.UpdateMsgDate(objMessage);

                                //GlobusLogHelper.log.Info("Message Updated in DataBase ! " + paramValue[3]);
                                //GlobusLogHelper.log.Debug("Message Updated in DataBase ! " + paramValue[3]);
                                //  }
                                //   else
                                //  {
                                //objMessage.MessagingReadParticipantsId = paramValue[4];
                                //objMessage.MessagingReadParticipants = paramValue[5];
                                //objMessage.MsgFriendId = paramValue[6];
                                //objMessage.MsgDate = paramValue[10];
                                //objMessage.MessageStatus = "0";
                                //objMessageRepository.Insert(objMessage);
                                //GlobusLogHelper.log.Info("Message saved in DataBase ! " + paramValue[3]);
                                //GlobusLogHelper.log.Debug("Message saved in DataBase ! " + paramValue[3]);
                                // } 
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }
                        if (paramValue[1] == "Function :  CheckGroupCompaignReport")
                        {
                            //GetAllGroupCompaignReport
                            try
                            {

                                objGroupCompaignReport.GroupId = paramValue[2];
                                objGroupCompaignReport.MessageText = paramValue[3];

                                ICollection<faceboardpro.Domain.GroupCompaignReport> lstMsg = objGroupCompaignReportRepository.CheckGroupCompaignReport(objGroupCompaignReport);
                                if (lstMsg.Count == 0)
                                {
                                    GroupManager.CheckDataBaseGroupCampaimanager = true;
                                }
                                else
                                {
                                    GlobusLogHelper.log.Debug(" Message All ready send  >>  " + objGroupCompaignReport.MessageText + " GroupUrl  >>>" + objGroupCompaignReport.GroupId);
                                    GlobusLogHelper.log.Info(" Message All ready send  >>  " + objGroupCompaignReport.MessageText + " GroupUrl  >>>" + objGroupCompaignReport.GroupId);
                                }


                                #region MyRegion
                                // if (lstMsg.Count > 0)
                                //{
                                //objMessage.MsgDate = paramValue[10];
                                //objMessageRepository.UpdateMsgDate(objMessage);

                                //GlobusLogHelper.log.Info("Message Updated in DataBase ! " + paramValue[3]);
                                //GlobusLogHelper.log.Debug("Message Updated in DataBase ! " + paramValue[3]);
                                //  }
                                //   else
                                //  {
                                //objMessage.MessagingReadParticipantsId = paramValue[4];
                                //objMessage.MessagingReadParticipants = paramValue[5];
                                //objMessage.MsgFriendId = paramValue[6];
                                //objMessage.MsgDate = paramValue[10];
                                //objMessage.MessageStatus = "0";
                                //objMessageRepository.Insert(objMessage);
                                //GlobusLogHelper.log.Info("Message saved in DataBase ! " + paramValue[3]);
                                //GlobusLogHelper.log.Debug("Message saved in DataBase ! " + paramValue[3]);
                                // } 
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }
                        if (GroupManager.ChkViewSchedulerTaskUniquePostPerGroup == true && paramValue[1] == "Function :  CheckGroupCompaignReport")
                        {
                            try
                            {
                                objGroupCompaignReport.GroupId = paramValue[2];
                                objGroupCompaignReport.MessageText = paramValue[3];

                                objGroupCompaignReportRepository.InsertOrUpdate(objGroupCompaignReport);
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

        public void MessageScraperQuery(DataSet ds, params string[] paramValue)
        {
            try
            {
                if (paramValue.Length > 1)
                {
                    if (paramValue[0] == "Model : MessageRepository")
                    {
                        if (paramValue[1] == "Function : GetMessageUsingUserIdNameSnippedIdSenderNameMsg")
                        {

                            try
                            {
                                objMessage.UserId = paramValue[2];
                                objMessage.UserName = paramValue[3];
                                objMessage.MsgSnippedId = paramValue[7];
                                objMessage.MsgSenderName = paramValue[8];
                                objMessage.MessageText = paramValue[9];

                                List<faceboardpro.Domain.Message> lstMsg = objMessageRepository.GetMessageUsingUserIdNameSnippedIdSenderNameMsg(objMessage);

                                if (lstMsg.Count > 0)
                                {
                                    objMessage.MsgDate = paramValue[10];
                                    objMessageRepository.UpdateMsgDate(objMessage);

                                    GlobusLogHelper.log.Info("Message Updated in DataBase ! " + paramValue[3]);
                                    GlobusLogHelper.log.Debug("Message Updated in DataBase ! " + paramValue[3]);
                                }
                                else
                                {
                                    objMessage.MessagingReadParticipantsId = paramValue[4];
                                    objMessage.MessagingReadParticipants = paramValue[5];
                                    objMessage.MsgFriendId = paramValue[6];
                                    objMessage.MsgDate = paramValue[10];
                                    objMessage.MessageStatus = "0";

                                    objMessageRepository.Insert(objMessage);

                                    GlobusLogHelper.log.Info("Message saved in DataBase ! " + paramValue[3]);
                                    GlobusLogHelper.log.Debug("Message saved in DataBase ! " + paramValue[3]);
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

        // This part will do for every module according to Sumit Sir..

        public void BindLodedAccountsInGRVManageProfile()
        {
            try
            {
                DataTable dt = CreateAccountTable();

                List<string> lstTemp = FBGlobals.listAccounts.Distinct().ToList();

                foreach (string item in lstTemp)
                {
                    try
                    {
                        string userName = string.Empty;
                        string password = string.Empty;
                        string proxyIp = string.Empty;
                        string proxyPort = string.Empty;
                        string proxyUserName = string.Empty;
                        string proxyPassword = string.Empty;

                        string[] itemArr = Regex.Split(item, ":");

                        //foreach (string item1 in itemArr)
                        {
                            try
                            {
                                userName = itemArr[0];
                                password = itemArr[1];
                                proxyIp = itemArr[2];
                                proxyPort = itemArr[3];
                                proxyUserName = itemArr[4];
                                proxyPassword = itemArr[5];
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            dt.Rows.Add(userName, password, proxyIp, proxyPort, proxyUserName, proxyPassword);
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

        public void BindEventCreatorFilePath()
        {
            try
            {
                objSetting.Module = "Events_EventCreator";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Events_EventCreator" && item.FileType == "Events_EventCreator_LoadEventDetails")
                        {

                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;

                                lblEvents_EventCreator_EventDetailsPath.Text = item.FilePath;

                                List<string> lstTemp = new List<string>();

                                lstTemp = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                                objEventManager.LstEventDetailsEventCreator = lstTemp.Distinct().ToList();

                                DateTime eTime = DateTime.Now;

                                lblEvents_EventCreator_EventDetailsCount.Text = objEventManager.LstEventDetailsEventCreator.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Event Details Loaded : " + objEventManager.LstEventDetailsEventCreator.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Event Details Loaded : " + objEventManager.LstEventDetailsEventCreator.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Event Details Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Event Details Since File Not Exists : " + item.FilePath);
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

        public void BindEventScraperFilePath()
        {
            try
            {
                objSetting.Module = "Scrapers_EventsScraper";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Scrapers_EventsScraper" && item.FileType == "Scrapers_EventsScraper_LoadEventURLs")
                        {

                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;

                                lblScrapers_EventsScraper_EventURLsPath.Text = item.FilePath;

                                objEventScraper.LstEventURLsEventScraper.Clear();

                                objEventScraper.LstEventURLsEventScraper = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();


                                DateTime eTime = DateTime.Now;

                                lblScrapers_EventsScraper_EventURLsCount.Text = objEventScraper.LstEventURLsEventScraper.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Event URLs Loaded : " + objEventScraper.LstEventURLsEventScraper.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Event URLs Loaded : " + objEventScraper.LstEventURLsEventScraper.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Event URLs Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Event URLs Since File Not Exists : " + item.FilePath);
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

        public void BindEventInviterFilePath()
        {
            try
            {
                objSetting.Module = "Events_EventInviter";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Events_EventInviter" && item.FileType == "Events_EventInviter_LoadEventURLs")
                        {

                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;

                                lblEvents_EventInviter_EventURLsPath.Text = item.FilePath;

                                objEventManager.LstEventURLsEventInviter.Clear();

                                objEventManager.LstEventURLsEventInviter = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                                DateTime eTime = DateTime.Now;

                                lblEvents_EventInviter_EventURLsCount.Text = objEventManager.LstEventURLsEventInviter.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Event URLs Loaded : " + objEventManager.LstEventURLsEventInviter.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Event URLs Loaded : " + objEventManager.LstEventURLsEventInviter.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Event URLs Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Event URLs Since File Not Exists : " + item.FilePath);
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

        private void GetAllEmails()
        {
            try
            {
                dtEmailAccount = new DataTable();
                Email objEmail = new Email();
                ICollection<Email> lstEmail = objEmailRepository.GetAllEmail(objEmail);

                dtEmailAccount = CreateEmailTable();

                foreach (Email item in lstEmail)
                {
                    DataRow newRow = dtEmailAccount.NewRow();
                    try
                    {
                        string Username = item.UserName;
                        string PassWord = item.Password;
                        string ISUsed = item.IsUsed;

                        if (ISUsed == "0")
                        {
                            ISUsed = "Not Used";
                        }
                        else if (ISUsed == "1")
                        {
                            ISUsed = "Used";
                        }
                        else
                        {
                            ISUsed = "Other Issue";
                        }

                        newRow["Username"] = Username;
                        newRow["Password"] = PassWord;
                        newRow["IsUsed"] = ISUsed;

                        dtEmailAccount.Rows.Add(newRow);

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                try
                {
                    //grvEmails_EmailCreator_AccountDetails.DataSource = null;
                   // grvEmails_EmailCreator_AccountDetails.DataSource = dtEmailAccount;

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

        //private void GetAllCaptchasetting()
        //{
        //    try
        //    {

        //        faceboardpro.Domain.Captcha objCaptcha = new faceboardpro.Domain.Captcha();
        //        ICollection<faceboardpro.Domain.Captcha> lstCaptcha = objCaptchaRepositry.GetAllCaptchaSetting(objCaptcha);

        //        foreach (faceboardpro.Domain.Captcha item in lstCaptcha)
        //        {
        //            try
        //            {
        //                string Username = item.Username;
        //                string PassWord = item.Password;
        //                string CaptchaService = item.CaptchaService;
        //                string Status = item.Status;

        //                if (CaptchaService.Contains("DBC"))
        //                {
        //                    txtDbcUserName.Text = Username;
        //                    txtDbcPassword.Text = PassWord;
        //                    CurrentCaptchaUserName = Username;
        //                    CurrentCaptchaPassword = PassWord;
        //                    isExitDBCSetting = true;
        //                    if (Status == "True")
        //                    {
        //                        CurrentCaptchaSetting = "DBC";
        //                        rdbDBC.Checked = true;
        //                    }
        //                }
        //                if (CaptchaService.Contains("Decaptcher"))
        //                {
        //                    txtDecaptchaerUserName.Text = Username;
        //                    txtDecaptcherPassword.Text = PassWord;
        //                    CurrentCaptchaUserName = Username;
        //                    CurrentCaptchaPassword = PassWord;
        //                    isExitDecaptcherSetting = true;
        //                    if (Status == "True")
        //                    {
        //                        CurrentCaptchaSetting = "Decaptcher";
        //                        rdbDecaptcher.Checked = true;
        //                    }
        //                }

        //                if (CaptchaService.Contains("Anigate"))
        //                {
        //                    txtAnigateUserName.Text = Username;
        //                    txtAnigatePassword.Text = PassWord;
        //                    CurrentCaptchaUserName = Username;
        //                    CurrentCaptchaPassword = PassWord;

        //                    isExitAnigateSetting = true;
        //                    if (Status == "True")
        //                    {
        //                        CurrentCaptchaSetting = "Anigate";
        //                        rdbAniGate.Checked = true;
        //                    }
        //                }

        //                if (CaptchaService.Contains("ImageTyperz"))
        //                {
        //                    txtImageTyperzUserName.Text = Username;
        //                    txtImageTypezPassword.Text = PassWord;
        //                    CurrentCaptchaUserName = Username;
        //                    CurrentCaptchaPassword = PassWord;
        //                    isExitImageTypezSetting = true;
        //                    if (Status == "True")
        //                    {
        //                        CurrentCaptchaSetting = "ImageTyperz";
        //                        rdbImageTyperz.Checked = true;
        //                    }
        //                }
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

        public string ResolveCaptchaByCaptchaService(byte[] imageBytes)
        {
            string CaptchaReturnValue = string.Empty;
            try
            {

                Captchas.Captcha objcaptcha = new Captchas.Captcha();
                CaptchaFactory objCaptchaDBC = new DBCService();
                CaptchaFactory objCaptchaDeacaptcher = new DecaptcherService();
                CaptchaFactory objCaptchaAnigat = new AnigateService();
                CaptchaFactory objCaptchaImageTyperz = new imagetyperzService();

                if (CurrentCaptchaSetting.Contains("DBC"))
                {
                    objcaptcha = objCaptchaDBC.resolveRecaptcha(imageBytes, CurrentCaptchaUserName, CurrentCaptchaPassword);
                }
                else if (CurrentCaptchaSetting.Contains("Decaptcher"))
                {
                    objcaptcha = objCaptchaDeacaptcher.resolveRecaptcha(imageBytes, CurrentCaptchaUserName, CurrentCaptchaPassword);
                }
                else if (CurrentCaptchaSetting.Contains("Anigate"))
                {
                    objcaptcha = objCaptchaAnigat.resolveRecaptcha(imageBytes, CurrentCaptchaUserName, CurrentCaptchaPassword);
                }
                else if (CurrentCaptchaSetting.Contains("ImageTyperz"))
                {
                    objcaptcha = objCaptchaImageTyperz.resolveRecaptcha(imageBytes, CurrentCaptchaUserName, CurrentCaptchaPassword);
                }
                CaptchaReturnValue = objcaptcha.CaptchaImage.ToString();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return CaptchaReturnValue;

        }

        public void BindFanPageLikerFilePath()
        {
            try
            {
                objSetting.Module = "Pages_FanPageLiker";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Pages_FanPageLiker" && item.FileType == "Pages_FanPageLiker_LoadFanPageURLs")
                        {

                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;
                                lblPages_FanPageLiker_FanPageURLsPath.Text = item.FilePath;

                                objPageManager.lstFanPageUrlsFanPageLiker.Clear();

                                objPageManager.lstFanPageUrlsFanPageLiker = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();


                                DateTime eTime = DateTime.Now;

                                lblPages_FanPageLiker_FanPageURLsCount.Text = objPageManager.lstFanPageUrlsFanPageLiker.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + objPageManager.lstFanPageUrlsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + objPageManager.lstFanPageUrlsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Fan Page URLs Since File Not Exists : " + item.FilePath);

                            }
                        }
                        if (item.Module == "Pages_FanPageLiker" && item.FileType == "Pages_FanPageLiker_LoadFanPageMessage")
                        {
                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;

                                lblPages_FanPageLiker_FanPageMessagePath.Text = item.FilePath;

                                objPageManager.lstFanPageMessageFanPageLiker.Clear();

                                objPageManager.lstFanPageMessageFanPageLiker = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                                DateTime eTime = DateTime.Now;

                                lblPages_FanPageLiker_FanPageMessageCount.Text = objPageManager.lstFanPageMessageFanPageLiker.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Fan Page Message Loaded : " + objPageManager.lstFanPageMessageFanPageLiker.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Fan Page Message Loaded : " + objPageManager.lstFanPageMessageFanPageLiker.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Fan Page Message Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Fan Page Message Since File Not Exists : " + item.FilePath);
                            }
                        }

                        if (item.Module == "Pages_FanPageLiker" && item.FileType == "Pages_FanPageLiker_LoadFanPageComments")
                        {


                            if (File.Exists(item.FilePath))
                            {
                                DateTime sTime = DateTime.Now;

                                lblPages_FanPageLiker_FanPageCommentsPath.Text = item.FilePath;

                                objPageManager.lstFanPageCommentsFanPageLiker.Clear();

                                objPageManager.lstFanPageCommentsFanPageLiker = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();


                                DateTime eTime = DateTime.Now;

                                lblPages_FanPageLiker_FanPageCommentsCount.Text = objPageManager.lstFanPageCommentsFanPageLiker.Count.ToString();

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Fan Page Comments Loaded : " + objPageManager.lstFanPageCommentsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Fan Page Comments Loaded : " + objPageManager.lstFanPageCommentsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Fan Page Comments Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Fan Page Comments Since File Not Exists : " + item.FilePath);
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

        public void groupRequestUnique_handleParamsEvent(object sender, EventArgs e)
        {
            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eArgs = e as EventsArgs;
                    GroupRequestUniqueQueryManager(eArgs.paramsData);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void GroupRequestUniqueQueryManager(params string[] paramValue)
        {
            try
            {
                if (paramValue[0].Equals("Insert"))
                {
                    objGroupRequestUnique.CampaignName = paramValue[1];
                    objGroupRequestUnique.URL = paramValue[2];
                    objGroupRequestUnique.Account = paramValue[3];
                    objGroupRequestUnique.Status = paramValue[4];
                    objGroupRequestUniqueRepository.Insert(objGroupRequestUnique); 
                }
                else if (paramValue[0].Equals("Select"))
                {
                    //objGroupRequestUniqueRepository.(objGroupRequestUnique);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        void pageManagerEvent_handleParamsEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eArgs = e as EventsArgs;


                    PageManagerQuery(eArgs.ds, eArgs.paramsData);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void PageManagerQuery(DataSet ds, params string[] paramValue)
        {
            try
            {
                if (paramValue.Length > 1)
                {
                    if (paramValue[0] == "Model : FanPageStatusRepository")
                    {
                        try
                        {
                            FanPageStatus objFanPageStatus = new FanPageStatus();

                            if (paramValue[1] == "Function : DeleteUsingMainPageUrl")
                            {
                                objFanPageStatus.MainPageUrl = paramValue[2];

                                objFanPageStatusRepository.DeleteUsingMainPageUrl(objFanPageStatus);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (paramValue[0] == "Model : FanPagePostRepository")
                    {
                        try
                        {
                            FanPagePost objFanPagePost = new FanPagePost();

                            if (paramValue[1] == "Function : Insert")
                            {
                                try
                                {
                                    objFanPagePost.FriendId = paramValue[2];
                                    objFanPagePost.Status = "0";
                                    objFanPagePost.Level = "0";
                                    objFanPagePost.Date = System.DateTime.Now.ToString();
                                    objFanPagePost.MainPageUrl = paramValue[6];

                                    objFanPagePostRepository.Insert(objFanPagePost);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                            if (paramValue[1] == "Function : InsertChasngeLevel")
                            {
                                try
                                {
                                    objFanPagePost.FriendId = paramValue[2];
                                    objFanPagePost.Status = "0";
                                    objFanPagePost.Level = "1";
                                    objFanPagePost.Date = System.DateTime.Now.ToString();
                                    objFanPagePost.MainPageUrl = paramValue[6];

                                    objFanPagePostRepository.InsertChasngeLevel(objFanPagePost);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            if (paramValue[1] == "Function : UpdateStatusUsingFriendId")
                            {
                                try
                                {
                                    objFanPagePost.FriendId = paramValue[2];
                                    objFanPagePost.Status = "1";


                                    objFanPagePostRepository.UpdateStatusUsingFriendId(objFanPagePost);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            if (paramValue[1] == "Function : GetFanPagePostUsingLevelStatusMainPageUrl")
                            {

                                try
                                {
                                    objFanPagePost.MainPageUrl = paramValue[2];
                                    objFanPagePost.Status = "0";
                                    objFanPagePost.Level = "0";

                                    List<FanPagePost> lstFPPost = objFanPagePostRepository.GetFanPagePostUsingLevelStatusMainPageUrl(objFanPagePost);

                                    if (lstFPPost.Count > 0)
                                    {
                                        DataTable dt = new DataTable();
                                        dt.Columns.Add("FriendId", typeof(string));

                                        foreach (FanPagePost item in lstFPPost)
                                        {
                                            try
                                            {
                                                dt.Rows.Add(item.FriendId.ToString());
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        ds.Tables.Add(dt);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                            }

                            if (paramValue[1] == "Function : GetFanPagePostFriendIdMainPageUrlUsingLevelStatusMainPageUrl")
                            {

                                try
                                {
                                    objFanPagePost.MainPageUrl = paramValue[2];
                                    objFanPagePost.Status = "0";
                                    objFanPagePost.Level = "1";

                                    List<FanPagePost> lstFPPost = objFanPagePostRepository.GetFanPagePostUsingLevelStatusMainPageUrl(objFanPagePost);

                                    if (lstFPPost.Count > 0)
                                    {
                                        DataTable dt = new DataTable();
                                        dt.Columns.Add("FriendId", typeof(string));
                                        dt.Columns.Add("MainPageUrl", typeof(string));

                                        foreach (FanPagePost item in lstFPPost)
                                        {
                                            try
                                            {
                                                dt.Rows.Add(item.FriendId.ToString(), item.MainPageUrl);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        ds.Tables.Add(dt);
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

                    if (paramValue[0] == "Model : EmailRepository")
                    {
                        Email objemail = new Email();
                        try
                        {
                            if (paramValue[1] == "Function : InsertCreatedEmailinEmailDatatbase")
                            {
                                objemail.UserName = paramValue[2];
                                objemail.Password = paramValue[3];
                                objemail.IsUsed = "0";
                                objEmailRepository.Insert(objemail);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (paramValue[0] == "Model : CaptchaSetting")
                    {
                        faceboardpro.Domain.Captcha objcaptcha = new faceboardpro.Domain.Captcha();
                        try
                        {
                            if (paramValue[1] == "Function : InsertCaptchaSettinginCaptchaDatatbase")
                            {
                                objcaptcha.Username = paramValue[2];
                                objcaptcha.Password = paramValue[3];
                                objcaptcha.CaptchaService = paramValue[4];
                                objcaptcha.Status = "True";
                                objCaptchaRepositry.Insert(objcaptcha);

                                objCaptchaRepositry.UpdateCaptchaStatusForOtherService(objcaptcha);
                            }
                            if (paramValue[1] == "Function : UpdateCaptchaSettinginCaptchaDatatbase")
                            {
                                objcaptcha.Username = paramValue[2];
                                objcaptcha.Password = paramValue[3];
                                objcaptcha.CaptchaService = paramValue[4];
                                objcaptcha.Status = "True";

                                objCaptchaRepositry.UpdateCaptchaStatusForOtherService(objcaptcha);
                                objCaptchaRepositry.UpdateCaptchaSetting(objcaptcha);
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }


                    if (paramValue[0] == "Model : FanPageDataRepository")
                    {
                        try
                        {
                            FanPageData objFanPageData = new FanPageData();

                            if (paramValue[1] == "Function : Insert")
                            {
                                try
                                {
                                    objFanPageData.Id = (paramValue[2]);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.Name = paramValue[3];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.FirstName = paramValue[4];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.MiddleName = paramValue[5];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.LastName = paramValue[6];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.Link = paramValue[7];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.Gender = paramValue[8];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.Locale = paramValue[9];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.ProfileStatus = paramValue[10];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.Url = paramValue[11];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    objFanPageData.ShowUser = paramValue[12];
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                objFanPageDataRepository.Insert(objFanPageData);
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

        public void BindRequestFriendsFilePath()
        {
            try
            {
                objSetting.Module = "Friends_RequestFriends";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Friends_RequestFriends" && item.FileType == "Friends_RequestFriends_LoadLocation")
                        {
                            lblFriends_RequestFriends_LocationPath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                objFriendManager.lstRequestFriendsLocation.Clear();

                                DateTime sTime = DateTime.Now;

                                objFriendManager.lstRequestFriendsLocation = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                                DateTime eTime = DateTime.Now;

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("Location Loaded : " + objFriendManager.lstRequestFriendsLocation.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("Location Loaded : " + objFriendManager.lstRequestFriendsLocation.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Location Since File Not Exists : " + item.FilePath);

                            }
                        }
                        if (item.Module == "Friends_RequestFriends" && item.FileType == "Friends_RequestFriends_LoadProfileURLs")
                        {

                            lblFriends_RequestFriends_ProfileURLsPath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                objFriendManager.lstRequestFriendsProfileURLs.Clear();

                                DateTime sTime = DateTime.Now;

                                objFriendManager.lstRequestFriendsProfileURLs = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                                DateTime eTime = DateTime.Now;

                                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                                GlobusLogHelper.log.Info("ProfileURLs Loaded : " + objFriendManager.lstRequestFriendsProfileURLs.Count + " In " + timeSpan + " Seconds");
                                GlobusLogHelper.log.Debug("ProfileURLs Loaded : " + objFriendManager.lstRequestFriendsProfileURLs.Count + " In " + timeSpan + " Seconds");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load ProfileURLs Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load ProfileURLs Since File Not Exists : " + item.FilePath);
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

        public void BindManageAccountsFilePath()
        {
            try
            {

                objSetting.Module = "Accounts_ManageAccounts";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Accounts_ManageAccounts" && item.FileType == "Accounts_ManageAccounts_LoadAccounts")
                        {
                            lblAccounts_ManageAccounts_LoadsAccountsPath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                Thread defaultLoadAccountsThread = new Thread(DefaultLoadAccounts);
                                defaultLoadAccountsThread.Start(item.FilePath);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Load Accounts Since File Not Exists : " + item.FilePath);
                                GlobusLogHelper.log.Debug("Load Accounts Since File Not Exists : " + item.FilePath);
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

        public void BindManageProfileFilePath()
        {
            try
            {
                string picsFolderPath = string.Empty;
                string profileDataFolderPath = string.Empty;
                string addaCoverFolderPath = string.Empty;

                objSetting.Module = "Accounts_ManageProfiles";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Accounts_ManageProfiles" && item.FileType == "Accounts_ManageProfiles_LoadPictures")
                        {
                           // lblAccounts_ManageProfiles_PicturesPath.Text = item.FilePath;
                            picsFolderPath = item.FilePath;

                            if (Directory.Exists(item.FilePath))
                            {
                                lstLoadedPicsManageProfiles.Clear();
                                ObjAccountManager.lstProfilePicsManageProfiles.Clear();

                                string[] picsArray = Directory.GetFiles(picsFolderPath);
                                lstLoadedPicsManageProfiles = picsArray.ToList();

                                ObjAccountManager.lstProfilePicsManageProfiles = lstLoadedPicsManageProfiles.Distinct().ToList();

                                GlobusLogHelper.log.Info(lstLoadedPicsManageProfiles.Count + " Profile Images Loaded");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Path Not Exists : " + picsFolderPath);
                            }
                        }

                        if (item.Module == "Accounts_ManageProfiles" && item.FileType == "Accounts_ManageProfiles_LoadCoverPics")
                        {
                            
                            addaCoverFolderPath = item.FilePath;

                            if (Directory.Exists(item.FilePath))
                            {
                                lstCoverPicsManageProfiles.Clear();
                                ObjAccountManager.lstCoverPicsManageProfiles.Clear();

                                string[] picsArray = Directory.GetFiles(addaCoverFolderPath);
                                lstCoverPicsManageProfiles = picsArray.ToList();

                                ObjAccountManager.lstCoverPicsManageProfiles = lstCoverPicsManageProfiles.Distinct().ToList();

                                GlobusLogHelper.log.Info(lstCoverPicsManageProfiles.Count + " Cover Images Loaded");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Path Not Exists : " + addaCoverFolderPath);
                            }

                        }

                        if (item.Module == "Accounts_ManageProfiles" && item.FileType == "Accounts_ManageProfiles_LoadProfileData")
                        {
                         
                            profileDataFolderPath = item.FilePath;

                            if (Directory.Exists(item.FilePath))
                            {
                                ObjAccountManager.LoadProfileData(item.FilePath);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Path Not Exists : " + profileDataFolderPath);
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

        public void BindAccountCreatorFilePath()
        {
            try
            {
                objSetting.Module = "Accounts_AccountCreator";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Accounts_AccountCreator" && item.FileType == "Accounts_AccountCreator_LoadEmails")
                        {
                            //lblaccounts_accountCreator_emailpath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedEmails.Clear();
                                lstLoadedEmails = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();
                                LoadExitingAccounts(item.FilePath);
                             //   lblaccounts_accountCreator_emailCount.Text = lstLoadedEmails.Count.ToString();

                                GlobusLogHelper.log.Info("Emails Loaded : " + lstLoadedEmails.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);
                            }
                        }
                        if (item.Module == "Accounts_AccountCreator" && item.FileType == "Accounts_AccountCreator_LoadFirstNames")
                        {
                          //  lblaccounts_accountCreator_FirstNamepath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedFirstNames.Clear();
                                lstLoadedFirstNames = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                              //  lblaccounts_accountCreator_FirstNameCount.Text = lstLoadedFirstNames.Count.ToString();

                                GlobusLogHelper.log.Info("First Names Loaded : " + lstLoadedFirstNames.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);

                            }
                        }
                        if (item.Module == "Accounts_AccountCreator" && item.FileType == "Accounts_AccountCreator_LoadLastNames")
                        {
                          //  lblaccounts_accountCreator_LastNamepath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedLastNames.Clear();
                                lstLoadedLastNames = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                               // lblaccounts_accountCreator_LastNameCount.Text = lstLoadedLastNames.Count.ToString();

                                GlobusLogHelper.log.Info("Last Names Loaded : " + lstLoadedLastNames.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);

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

        public void BindEmailCreatorFilePath()
        {
            try
            {
                objSetting.Module = "Emails_EmailCreator";

                List<Setting> lstSetting = objSettingRepository.GetFilePath(objSetting);

                foreach (Setting item in lstSetting)
                {
                    try
                    {
                        if (item.Module == "Emails_EmailCreator" && item.FileType == "Emails_EmailCreator_LoadEmails")
                        {
                           // lblEmails_EmailCreator_EmailsPath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedEmailsForEmail.Clear();
                                lstLoadedEmailsForEmail = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                             //   lblEmails_EmailCreator_CountEmails.Text = lstLoadedEmailsForEmail.Count.ToString();

                                GlobusLogHelper.log.Info("Emails Loaded : " + lstLoadedEmailsForEmail.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);

                            }
                        }
                        if (item.Module == "Emails_EmailCreator" && item.FileType == "Emails_EmailCreator_loadFirstName")
                        {
                        //    lblEmails_EmailCreator_FirstNamePath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedFirstNamesForEmail.Clear();
                                lstLoadedFirstNamesForEmail = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                              //  lblEmails_EmailCreator_CountCreatedFirstName.Text = lstLoadedFirstNamesForEmail.Count.ToString();

                                GlobusLogHelper.log.Info("First Names Loaded : " + lstLoadedFirstNamesForEmail.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);

                            }
                        }
                        if (item.Module == "Emails_EmailCreator" && item.FileType == "Emails_EmailCreator_LoadLastNames")
                        {
                           // lblEmails_EmailCreator_LastNamePath.Text = item.FilePath;

                            if (File.Exists(item.FilePath))
                            {
                                lstLoadedLastNamesForEmail.Clear();
                                lstLoadedLastNamesForEmail = GlobusFileHelper.ReadFile(item.FilePath).Distinct().ToList();

                               // lblEmails_EmailCreator_CountLastNames.Text = lstLoadedLastNamesForEmail.Count.ToString();

                                GlobusLogHelper.log.Info("Last Names Loaded : " + lstLoadedLastNamesForEmail.Count);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("File Not Exists : " + item.FilePath);

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

        private void CopyDataBase()
        {
            try
            {

                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fdhomepath);
                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fddatapath);
                string startUpDB = Application.StartupPath + faceboardpro.FBGlobals.Instance.fddbfilename;
                string localAppDataDB = "C:\\faceboardpro\\Data\\faceboardpro.db";
                string startUpDB64 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\faceboardpro.db";
              
                if (!File.Exists(localAppDataDB))
                {
                    if (File.Exists(startUpDB))
                    {
                        try
                        {
                            File.Copy(startUpDB, localAppDataDB);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find a part of the path"))
                            {
                                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fdhomepath);
                                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fddatapath);
                                File.Copy(startUpDB, localAppDataDB);
                            }
                        }
                    }
                    else if (File.Exists(startUpDB64))   //for 64 Bit
                    {
                        try
                        {
                            File.Copy(startUpDB64, localAppDataDB);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Could not find a part of the path"))
                            {
                                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fdhomepath);
                                Directory.CreateDirectory(faceboardpro.FBGlobals.Instance.fddatapath);
                                File.Copy(startUpDB64, localAppDataDB);
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

        void AccountCreationEvent_handleParamsEvent(object sender, EventArgs e)
        {
            try
            {
                if (e is EventsArgs)
                {
                    EventsArgs eArgs = e as EventsArgs;
                    //MessageBox.Show(eArgs.paramsData[0]);

                    SaveData(eArgs.paramsData);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        DataTable dtCreatedAccount = new DataTable();
        DataTable dtEmailAccount = new DataTable();
        DataTable dtCampaignSchedule = new DataTable();

        public DataTable CreateAccountTable()
        {

            try
            {
                DataColumn colUserName = new DataColumn("UserName");
                colUserName.ReadOnly = true;
                DataColumn colPassword = new DataColumn("Password");
                colPassword.ReadOnly = true;
                DataColumn colProxyAddress = new DataColumn("ProxyAddress");
                colProxyAddress.ReadOnly = true;
                DataColumn colProxyPort = new DataColumn("ProxyPort");
                colProxyPort.ReadOnly = true;
                DataColumn colProxyUserName = new DataColumn("ProxyUserName");
                colProxyUserName.ReadOnly = true;
                DataColumn colProxyPassword = new DataColumn("ProxyPassword");
                colProxyPassword.ReadOnly = true;

                //dtCreatedAccount.Columns.Add(colUserName);
                //dtCreatedAccount.Columns.Add(colPassword);
                //dtCreatedAccount.Columns.Add(colProxyAddress);
                //dtCreatedAccount.Columns.Add(colProxyPort);
                //dtCreatedAccount.Columns.Add(colProxyUserName);
                //dtCreatedAccount.Columns.Add(colProxyPassword);

            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("UserName", "UserName");
            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("Password", "Password");
            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("ProxyAddress", "ProxyAddress");
            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("ProxyPort", "ProxyPort");
            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("ProxyUserName", "ProxyUserName");
            //    grvAccounts_AccountCreator_AccountDetails.Columns.Add("ProxyPassword", "ProxyPassword");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return dtCreatedAccount;
        }

        public DataTable CreateEmailTable()
        {
            dtEmailAccount = new DataTable();
            try
            {
                DataColumn colUserName = new DataColumn("UserName");
                colUserName.ReadOnly = true;
                DataColumn colPassword = new DataColumn("Password");
                colPassword.ReadOnly = true;
                DataColumn colIsUsed = new DataColumn("IsUsed");
                colIsUsed.ReadOnly = true;
                dtEmailAccount.Columns.Add(colUserName);
                dtEmailAccount.Columns.Add(colPassword);
                dtEmailAccount.Columns.Add(colIsUsed);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return dtEmailAccount;
        }

        public DataTable CreateGroupScheduleTable(string Module)
        {
            dtCampaignSchedule = new DataTable();
            try
            {
                if (Module.Equals("Group Posting"))
                {
                    DataColumn colCampaignName = new DataColumn("GroupCampaignName");
                    colCampaignName.ReadOnly = true;
                    DataColumn colAccount = new DataColumn("Account");
                    colAccount.ReadOnly = true;
                    DataColumn colPicFilePath = new DataColumn("PicFilePath");
                    colPicFilePath.ReadOnly = true;

                    DataColumn colVideoFilePath = new DataColumn("VideoFilePath");
                    colVideoFilePath.ReadOnly = true;
                    DataColumn colMessageFilePath = new DataColumn("MessageFilePath");
                    colMessageFilePath.ReadOnly = true;
                    DataColumn colScheduleTime = new DataColumn("ScheduleTime");
                    colScheduleTime.ReadOnly = true;

                    DataColumn colCmpStartTime = new DataColumn("CmpStartTime");
                    colCampaignName.ReadOnly = true;
                    DataColumn colAccomplish = new DataColumn("Accomplish");
                    colAccomplish.ReadOnly = true;
                    DataColumn colNoOfMessage = new DataColumn("NoOfMessage");
                    colNoOfMessage.ReadOnly = true;

                    DataColumn colMessageMode = new DataColumn("MessageMode");
                    colMessageMode.ReadOnly = true;
                    DataColumn colMessageType = new DataColumn("MessageType");
                    colMessageType.ReadOnly = true;
                    DataColumn colTextMessage = new DataColumn("TextMessage");
                    colTextMessage.ReadOnly = true;


                    dtCampaignSchedule.Columns.Add(colCampaignName);
                    dtCampaignSchedule.Columns.Add(colAccount);
                    dtCampaignSchedule.Columns.Add(colPicFilePath);

                    dtCampaignSchedule.Columns.Add(colVideoFilePath);
                    dtCampaignSchedule.Columns.Add(colMessageFilePath);
                    dtCampaignSchedule.Columns.Add(colScheduleTime);

                    dtCampaignSchedule.Columns.Add(colCmpStartTime);
                    dtCampaignSchedule.Columns.Add(colAccomplish);
                    dtCampaignSchedule.Columns.Add(colNoOfMessage);
                    dtCampaignSchedule.Columns.Add(colMessageMode);
                    dtCampaignSchedule.Columns.Add(colMessageType);
                    dtCampaignSchedule.Columns.Add(colTextMessage); 
                }
                else if (Module.Equals("Group Request"))
                {
                    DataColumn colCampaignName = new DataColumn("GroupCampaignName");
                    colCampaignName.ReadOnly = true;
                    DataColumn colAccount = new DataColumn("Account");
                    colAccount.ReadOnly = true;
                    DataColumn colVideoFilePath = new DataColumn("Group URLs File Path");
                    colVideoFilePath.ReadOnly = true;
                    DataColumn colScheduleTime = new DataColumn("ScheduleTime");
                    colScheduleTime.ReadOnly = true;

                    DataColumn colCmpStartTime = new DataColumn("CmpStartTime");
                    colCampaignName.ReadOnly = true;
                    DataColumn colAccomplish = new DataColumn("Accomplish");
                    colAccomplish.ReadOnly = true;
                    dtCampaignSchedule.Columns.Add(colCampaignName);
                    dtCampaignSchedule.Columns.Add(colAccount);
                    dtCampaignSchedule.Columns.Add(colVideoFilePath);
                    dtCampaignSchedule.Columns.Add(colScheduleTime);

                    dtCampaignSchedule.Columns.Add(colCmpStartTime);
                    dtCampaignSchedule.Columns.Add(colAccomplish);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return dtCampaignSchedule;
        }

        public void SaveData(params string[] paramValue)
        {
            Account objAccount = new Account();
            try
            {
                objAccount.UserName = paramValue[0];
                objAccount.Password = paramValue[1];
                objAccount.ProxyAddress = paramValue[2];
                objAccount.ProxyPort = paramValue[3];
                objAccount.ProxyUserName = paramValue[4];
                objAccount.ProxyPassword = paramValue[5];


                // For GridView Dispaly
                //DataRow dr = dtCreatedAccount.NewRow();
                //dr["UserName"] = paramValue[0];
                //dr["Password"] = paramValue[1];
                //dr["ProxyAddress"] = paramValue[2];
                //dr["ProxyPort"] = paramValue[3];
                //dr["ProxyUserName"] = paramValue[4];
                //dr["ProxyPassword"] = paramValue[5];

                //if (grvAccounts_AccountCreator_AccountDetails.InvokeRequired)
                //{
                //    grvAccounts_AccountCreator_AccountDetails.Invoke(new MethodInvoker(delegate
                //    {
                //        int rowIndex = grvAccounts_AccountCreator_AccountDetails.Rows.Add(paramValue[0], paramValue[1], paramValue[2], paramValue[3], paramValue[4], paramValue[5]);
                //        //grvAccounts_AccountCreator_AccountDetails.Rows.Insert(rowIndex, paramValue[0], paramValue[1], paramValue[2], paramValue[3], paramValue[4], paramValue[5]);
                //    }));
                //}
                //else
                //{
                //    int rowIndex = grvAccounts_AccountCreator_AccountDetails.Rows.Add(paramValue[0], paramValue[1], paramValue[2], paramValue[3], paramValue[4], paramValue[5]);
                //    //grvAccounts_AccountCreator_AccountDetails.Rows.Insert(rowIndex, paramValue[0], paramValue[1], paramValue[2], paramValue[3], paramValue[4], paramValue[5]);
                //}

                //dtCreatedAccount.Rows.Add(dr);

                //BindCreatedAccountIngrvAccounts_AccountCreator_AccountStatus(dtCreatedAccount);

                //For Database Insertion
                objAccountRepository.Insert(objAccount);

               // grvAccounts_AccountCreator_AccountDetails.ScrollBars = ScrollBars.Both;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

    

        public string GetAssemblyVersion()
        {
            string versionNumber = string.Empty;

            try
            {
                string appName = Assembly.GetAssembly(this.GetType()).Location;
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(appName);
                versionNumber = assemblyName.Version.ToString();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return versionNumber;
        }

   

        #region Insert Setting Into Dadabase

        public void InsertOrUpdateSetting(Setting objSetting)
        {
            try
            {
                objSettingRepository.InsertOrUpdate(objSetting);


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        #endregion

        private void LoadEmails(string emailFile)
        {

            try
            {
                lstLoadedEmails = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedEmails = lstLoadedEmails.Distinct().ToList();
                if (faceboardpro.FBGlobals.Instance.isfreeversion || Globals.CheckLicenseManager == "fdfreetrial")
                {
                    try
                    {
                        lstLoadedEmails.RemoveRange(5, lstLoadedEmails.Count - 5);                        

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

        private void btnAccounts_AccountCreator_LoadFirstName_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {


                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                      
                        lstLoadedFirstNames.Clear();
                        LoadFirstName(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                  
                        //Insert Seeting Into Database
                        objSetting.Module = "Accounts_AccountCreator";
                        objSetting.FileType = "Accounts_AccountCreator_LoadFirstNames";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        //   GlobusLogHelper.log.Debug("First Names Loaded : " + lstLoadedFirstNames.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("First Names Loaded : " + lstLoadedFirstNames.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }

        private void LoadFirstName(string emailFile)
        {

            try
            {
                lstLoadedFirstNames = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedFirstNames = lstLoadedFirstNames.Distinct().ToList();
                if (faceboardpro.FBGlobals.Instance.isfreeversion || Globals.CheckLicenseManager == "fdfreetrial")
                {
                    try
                    {
                        lstLoadedFirstNames.RemoveRange(5, lstLoadedFirstNames.Count - 5);


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

        private void LoadFirstNameForEmail(string emailFile)
        {

            try
            {
                lstLoadedFirstNamesForEmail = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedFirstNamesForEmail = lstLoadedFirstNamesForEmail.Distinct().ToList();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnAccounts_AccountCreator_LoadLastNames_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                       
                        lstLoadedLastNames.Clear();
                        LoadLastName(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                    
                        //Insert Seeting Into Database
                        objSetting.Module = "Accounts_AccountCreator";
                        objSetting.FileType = "Accounts_AccountCreator_LoadLastNames";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        //    GlobusLogHelper.log.Debug("Last Names Loaded : " + lstLoadedLastNames.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Last Names Loaded : " + lstLoadedLastNames.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                // GlobusLogHelper.log.Debug("Error in Uploading Last Names !");

            }
        }

        private void LoadLastName(string emailFile)
        {

            try
            {
                lstLoadedLastNames = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedLastNames = lstLoadedLastNames.Distinct().ToList();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //  GlobusLogHelper.log.Debug("Error in Uploading Last Names !");
            }
        }

        private void LoadLastNameForEmail(string emailFile)
        {

            try
            {
                lstLoadedLastNamesForEmail = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedLastNamesForEmail = lstLoadedLastNamesForEmail.Distinct().ToList();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //  GlobusLogHelper.log.Debug("Error in Uploading Last Names !");
            }
        }

        private void btn_Account_AccountCreator_CreateAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                ObjAccountManager.isStopAccountCreator = false;
                ObjAccountManager.lstAccountCreatorThreads.Clear();

                Regex checkNo = new Regex("^[0-9]*$");

                int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                int threads = 25;

                int maxThread = 25 * processorCount;

          
                ObjAccountManager.accountCreationEvent.handleParamsEvent += new EventHandler(AccountCreationEvent_handleParamsEvent);

                foreach (string item in lstLoadedEmails)
                {
                    try
                    {
                        if (ObjAccountManager.isStopAccountCreator)
                        {
                            break;
                        }

                        string sex = string.Empty;
                        ThreadPool.SetMaxThreads(maxThread, 5);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ObjAccountManager.CreateAccount), new object[] { item, lstLoadedFirstNames, lstLoadedLastNames, lstProxies, sex });

                
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

        private void btnAccounts_ManageProfiles_LoadPictures_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {
                    ofd.SelectedPath = Application.StartupPath + "\\Profile\\Pics";
                    List<string> lstwrongPic = new List<string>();
                    List<string> lstRightPic = new List<string>();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                       // lblAccounts_ManageProfiles_PicturesPath.Text = ofd.SelectedPath;
                        lstLoadedPicsManageProfiles.Clear();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstLoadedPicsManageProfiles = picsArray.ToList();

                        foreach (string item in lstLoadedPicsManageProfiles)
                        {
                            try
                            {
                                string items = item;
                                items = item.ToLower();
                                if (!items.Contains(".jpg") && !items.Contains(".png") && !items.Contains(".jpeg") && !items.Contains(".gif"))
                                {
                                    lstwrongPic.Add(item);

                                }
                                else
                                {
                                    lstRightPic.Add(item);
                                }


                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }

                        lstLoadedPicsManageProfiles.Clear();
                        lstLoadedPicsManageProfiles.AddRange(lstRightPic);

                        if (lstLoadedPicsManageProfiles.Count < 1)
                        {
                            MessageBox.Show("Please Upload Profile Picture !");
                            return;
                        }

                        ObjAccountManager.lstProfilePicsManageProfiles = lstLoadedPicsManageProfiles.Distinct().ToList();
                        GlobusLogHelper.log.Info(lstLoadedPicsManageProfiles.Count + " Profile Images loaded");


                        //Insert Seeting Into Database
                        objSetting.Module = "Accounts_ManageProfiles";
                        objSetting.FileType = "Accounts_ManageProfiles_LoadPictures";
                        objSetting.FilePath = ofd.SelectedPath;

                        InsertOrUpdateSetting(objSetting);

                        //GlobusLogHelper.log.Debug(lstLoadedPicsManageProfiles.Count + " Profile Images loaded");

                        if (lstwrongPic.Count > 1)
                        {
                            GlobusLogHelper.log.Info(lstwrongPic.Count + " Wrong Format Profile Images loaded");
                            // GlobusLogHelper.log.Debug(lstwrongPic.Count + " Wrong Format Profile Images loaded");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //   GlobusLogHelper.log.Debug("Error in Uploading Pictures !");
            }
        }

        private void btnAccounts_ManageProfiles_LoadProfileData_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {

                    ofd.SelectedPath = Application.StartupPath + "\\Profile";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                      //  lblAccounts_ManageProfiles_ProfileDataPath.Text = ofd.SelectedPath;
                        ObjAccountManager.LoadProfileData(ofd.SelectedPath);

                        //Insert Seeting Into Database
                        objSetting.Module = "Accounts_ManageProfiles";
                        objSetting.FileType = "Accounts_ManageProfiles_LoadProfileData";
                        objSetting.FilePath = ofd.SelectedPath;

                        InsertOrUpdateSetting(objSetting);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //  GlobusLogHelper.log.Debug("Error in Uploading Profile Data !");
            }
        }

        private void btnAccounts_ManageProfiles_LoadCoverPics_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lstwrongPic = new List<string>();
                List<string> lstRightPic = new List<string>();
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        //lblAccounts_ManageProfiles_CoverPicsPath.Text = ofd.SelectedPath;
                        lstCoverPicsManageProfiles.Clear();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstCoverPicsManageProfiles = picsArray.ToList();

                        foreach (string item in lstCoverPicsManageProfiles)
                        {
                            try
                            {
                                string items = item;
                                items = item.ToLower();
                                if (!items.Contains(".jpg") && !items.Contains(".png") && !items.Contains(".jpeg") && !items.Contains(".gif"))
                                {
                                    lstwrongPic.Add(item);
                                }
                                else
                                {
                                    lstRightPic.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                            }
                        }
                        lstCoverPicsManageProfiles.Clear();
                        lstCoverPicsManageProfiles.AddRange(lstRightPic);
                        if (lstCoverPicsManageProfiles.Count < 1)
                        {
                            MessageBox.Show("Please Upload Cover Picture !");
                            return;
                        }
                    }

                    GlobusLogHelper.log.Info(lstRightPic.Count + " Cover Images loaded");

                    ObjAccountManager.lstCoverPicsManageProfiles = lstCoverPicsManageProfiles.Distinct().ToList();

                    //Insert Seeting Into Database
                    objSetting.Module = "Accounts_ManageProfiles";
                    objSetting.FileType = "Accounts_ManageProfiles_LoadCoverPics";
                    objSetting.FilePath = ofd.SelectedPath;

                    InsertOrUpdateSetting(objSetting);

                    //  GlobusLogHelper.log.Debug(lstRightPic.Count + " Cover Images loaded");
                    if (lstwrongPic.Count > 1)
                    {
                        GlobusLogHelper.log.Info(lstwrongPic.Count + " Wrong Format Cover Images loaded");
                        GlobusLogHelper.log.Debug(lstwrongPic.Count + " Wrong Format Cover Images loaded");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //    GlobusLogHelper.log.Debug("Error in Uploading Cover Pics !");
            }
        }

        private void btn_Account_AccountCreator_StopAccountsCreation_Click(object sender, EventArgs e)
        {
            try
            {
                Thread threadStopAccountCreation = new Thread(StopAccountCreation);
                threadStopAccountCreation.Start();
                //StopAccountCreation();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void StopAccountCreation()
        {
            try
            {
                ObjAccountManager.isStopAccountCreator = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = ObjAccountManager.lstAccountCreatorThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        ObjAccountManager.lstAccountCreatorThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void LoadUserAccount_Click(object sender, EventArgs e)
        {
            BindAccountCreatorFilePath();
            //try
            //{
            //    Thread uploadAccountsThread = new Thread(LoadFansPageSCraperAccounts);
            //    uploadAccountsThread.Start();
            //}
            //catch (Exception ex)
            //{
            //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //}
        }

        private void btnAccounts_ManageProfiles_CreateProfiles_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    ObjAccountManager.isStopManageProfiles = false;
                    ObjAccountManager.lstManageProfilesThreads.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                  //  int ThreadCount = Convert.ToInt32(txtAccounts_AccountCreator_Threads.Text);
                 //   if (ThreadCount == 0)
                    {
                        GlobusLogHelper.log.Info("Please Enter More than 0 thread ..");
                        GlobusLogHelper.log.Debug("Please Enter More than 0 thread ..");
                        return;
                    }

                  //  if (!string.IsNullOrEmpty(txtAccounts_AccountCreator_Threads.Text) && checkNo.IsMatch(txtAccounts_AccountCreator_Threads.Text))
                    {
                       // threads = Convert.ToInt32(txtAccounts_AccountCreator_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    AccountManager.NoOfThreads = threads;
                    try
                    {
                       // ObjAccountManager.UpdateOnlyProfile = cmbAccounts_ManageProfiles_UpdateOnly.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread createProfileThread = new Thread(ObjAccountManager.CreateProfile);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_RequestFriends_LoadLocation_Click(object sender, EventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaLocation.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblFriends_RequestFriends_LocationPath.Text = ofd.FileName;
                        objFriendManager.lstRequestFriendsLocation.Clear();

                        objFriendManager.lstRequestFriendsLocation = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //lblaccounts_accountCreator_emailCount.Text = lstLoadedEmails.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_RequestFriends";
                        objSetting.FileType = "Friends_RequestFriends_LoadLocation";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Location Loaded : " + objFriendManager.lstRequestFriendsLocation.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Location Loaded : " + objFriendManager.lstRequestFriendsLocation.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_RequestFriends_SendRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objFriendManager.isRequestFriendsStop = false;
                    objFriendManager.lstRequestFriendsThreads.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();  //Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                 //   int ThreadCount = Convert.ToInt32(txtAccounts_AccountCreator_Threads.Text);
                   // if (ThreadCount == 0)
                    {
                        GlobusLogHelper.log.Info("Please Enter More than 0 thread ..");
                        GlobusLogHelper.log.Debug("Please Enter More than 0 thread ..");
                        return;
                    }

                    //if (!string.IsNullOrEmpty(txtAccounts_AccountCreator_Threads.Text) && checkNo.IsMatch(txtAccounts_AccountCreator_Threads.Text))
                    //{
                    //    threads = Convert.ToInt32(txtAccounts_AccountCreator_Threads.Text);
                    //}

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }

                    try
                    {
                        FriendManager.minDelayFriendManager = Convert.ToInt32(txtFriends_RequestFriends_DelayMin.Text);
                        FriendManager.maxDelayFriendManager = Convert.ToInt32(txtFriends_RequestFriends_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    int noOfFriendsRequest = 10;

                    if (!string.IsNullOrEmpty(txtFriends_RequestFriends_NoOfFriendsRequest.Text) && checkNo.IsMatch(txtFriends_RequestFriends_NoOfFriendsRequest.Text))
                    {
                        noOfFriendsRequest = Convert.ToInt32(txtFriends_RequestFriends_NoOfFriendsRequest.Text);
                    }

                    FriendManager.NoOfFriendsRequest = noOfFriendsRequest;
                    FriendManager.NoOfFriendsRequestParKeyWord=Convert.ToInt32(txtFriends_RequestFriends_NoOfFriendsRequestParKeyword.Text);


                    FriendManager.NoOfThreads = threads;

                    if (rdbFriends_RequestFriends_SearchViaLocation.Checked)
                    {
                        FriendManager.IsSearchViaLocation = true;
                        if (!string.IsNullOrEmpty(txtFriends_RequestFriends_Location.Text))
                        {
                            FriendManager.Location = txtFriends_RequestFriends_Location.Text;
                        }
                        else if (objFriendManager.lstRequestFriendsLocation.Count > 0)
                        {
                        }
                        else
                        {
                            MessageBox.Show("Please Enter Location Or Load Location File !");

                            GlobusLogHelper.log.Info("Please Enter Location Or Load Location File !");
                            GlobusLogHelper.log.Debug("Please Enter Location Or Load Location File !");
                            return;
                        }
                    }

                    if (rdbFriends_RequestFriends_SearchViaKeywords.Checked)
                    {
                        FriendManager.IsSearchViaKeywords = true;
                        if (!string.IsNullOrEmpty(txtFriends_RequestFriends_Keywords.Text))
                        {
                            FriendManager.Keywords = txtFriends_RequestFriends_Keywords.Text;
                        }
                        else if (objFriendManager.lstRequestFriendsKeywords.Count > 0)
                        {
                        }
                        else
                        {
                            MessageBox.Show("Please Enter Keywords Or Load Keywords File !");

                            GlobusLogHelper.log.Info("Please Enter Keywords Or Load Keywords File !");
                            GlobusLogHelper.log.Debug("Please Enter Keywords Or Load Keywords File !");
                            return;
                        }
                    }



                    if (rdbFriends_RequestFriends_SearchViaProfileURLs.Checked)
                    {
                        FriendManager.IsSearchViaFanPageURLs = true;
                        //objFriendManager.lstRequestFriendsProfileURLs.count>0
                        FriendManager.IsSearchViaFanPageURLs = false;
                        FriendManager.IsSearchViaLocation = false;
                        //if (!string.IsNullOrEmpty(txtFriends_RequestFriends_Keywords.Text))
                        //{
                        //    FriendManager.Keywords = txtFriends_RequestFriends_Keywords.Text;
                        //}
                        if (objFriendManager.lstRequestFriendsProfileURLs.Count > 0)
                        {
                            FriendManager.IsSearchViaProfileUrls = true;
                        }
                        else
                        {
                            MessageBox.Show("Please Load Profile URLs File !");

                            GlobusLogHelper.log.Info("Please Load Profile URLs File !");
                            GlobusLogHelper.log.Debug("Please Load Profile URLs File !");
                            return;
                        }
                    }

                    if (rdbFriends_RequestFriends_SearchViaFanPageURLs.Checked)
                    {
                        FriendManager.IsSearchViaFanPageURLs = true;
                        FanPageData objFanPageData = new FanPageData();

                        if (rdbFriends_RequestFriends_SearchViaDatabase.Checked)
                        {
                            List<FanPageData> lstFanPageData = objFanPageDataRepository.GetFanPageDataUsingCount(objFanPageData, noOfFriendsRequest);

                            List<string> lstFriendIds = new List<string>();

                            foreach (var item in lstFanPageData)
                            {
                                try
                                {
                                    string friendId = item.Id;
                                    lstFriendIds.Add(friendId);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            objFriendManager.lstFriendIds = lstFriendIds.Distinct().ToList();
                        }
                        else
                        {
                            tabMain.SelectedTab = tabPageScrapers;
                            return;
                        }


                        if (!string.IsNullOrEmpty(txtFriends_RequestFriends_FanPageURLs.Text))
                        {
                            FriendManager.FanPageURLs = txtFriends_RequestFriends_FanPageURLs.Text;
                        }
                    }

                    try
                    {
                        FriendManager.SendRequestUsing = cmbFriends_RequestFriends_SendRequestUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        FriendManager.NoOfFriendsRequestPerUser = int.Parse(txtNoOFRequestPerUser.Text);
                    }
                    catch (Exception ex)
                    {
                        FriendManager.NoOfFriendsRequestPerUser = 0;
                    }
                    Thread sendFriendRequestThread = new Thread(objFriendManager.StartSendFriendRequest);
                    sendFriendRequestThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnAccounts_ManageProfiles_ResetProfiles_Click(object sender, EventArgs e)
        {
            try
            {
                ObjAccountManager.isStopManageProfiles = false;
                ObjAccountManager.lstManageProfilesThreads.Clear();

                Regex checkNo = new Regex("^[0-9]*$");

                int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                int threads = 25;

                int maxThread = 25 * processorCount;

                //if (!string.IsNullOrEmpty(txtAccounts_AccountCreator_Threads.Text) && checkNo.IsMatch(txtAccounts_AccountCreator_Threads.Text))
                //{
                //    threads = Convert.ToInt32(txtAccounts_AccountCreator_Threads.Text);
                //}

                //ObjAccountManager.UpdateOnlyProfile = cmbAccounts_ManageProfiles_UpdateOnly.SelectedItem.ToString();

                Thread createProfileThread = new Thread(ObjAccountManager.RemoveProfile);
                createProfileThread.Start();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_RequestFriends_LoadProfileURLs_Click(object sender, EventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaProfileURLs.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblFriends_RequestFriends_ProfileURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendManager.lstRequestFriendsProfileURLs = lstTemp.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        // lblaccounts_accountCreator_emailCount.Text = lstLoadedEmails.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_RequestFriends";
                        objSetting.FileType = "Friends_RequestFriends_LoadProfileURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("ProfileURLs Loaded : " + objFriendManager.lstRequestFriendsProfileURLs.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("ProfileURLs Loaded : " + objFriendManager.lstRequestFriendsProfileURLs.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_RequestFriends_StopRequest_Click(object sender, EventArgs e)
        {
            try
            {
                objFriendManager.isRequestFriendsStop = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendManager.lstRequestFriendsThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendManager.lstRequestFriendsThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnAccounts_ManageAccounts_LoadAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                Thread uploadAccountThread = new Thread(LoadAccounts);
                uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                uploadAccountThread.IsBackground = true;

                uploadAccountThread.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void DefaultLoadAccounts(object filePath)
        {
            try
            {
                string fileName = (string)filePath;

                FBGlobals.loadedAccountsDictionary.Clear();
                FBGlobals.listAccounts.Clear();

                DataSet ds;

                DataTable dt = new DataTable();

                lblAccounts_ManageAccounts_LoadsAccountsPath.Invoke(new MethodInvoker(delegate
                {
                    lblAccounts_ManageAccounts_LoadsAccountsPath.Text = fileName;
                }));

                dt.Columns.Add("UserName");
                dt.Columns.Add("Password");
                dt.Columns.Add("ProxyAddress");
                dt.Columns.Add("ProxyPort");
                dt.Columns.Add("ProxyUserName");
                dt.Columns.Add("ProxyPassword");


                ds = new DataSet();
                ds.Tables.Add(dt);

                grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                {
                    grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                }));

                DateTime sTime = DateTime.Now;

                List<string> lstAccountsInGroupCampaignManager = new List<string>();
                List<string> lstAccountsInGroupMemberScraper = new List<string>();


                List<string> templist = GlobusFileHelper.ReadFile(fileName);
                int Counter = 0;
                foreach (string item in templist)
                {

                    if (Globals.CheckLicenseManager == "fdfreetrial" && Counter==5)
                    {
                        break; 
                    }
                    Counter = Counter + 1;
                    try
                    {
                        string account = item;
                        string[] AccArr = account.Split(':');
                        if (AccArr.Count() > 1)
                        {
                            string accountUser = account.Split(':')[0];
                            string accountPass = account.Split(':')[1];
                            string proxyAddress = string.Empty;
                            string proxyPort = string.Empty;
                            string proxyUserName = string.Empty;
                            string proxyPassword = string.Empty;
                            string status = string.Empty;

                            int DataCount = account.Split(':').Length;
                            if (DataCount == 2)
                            {
                                //Globals.accountMode = AccountMode.NoProxy;

                            }
                            else if (DataCount == 4)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                            }
                            else if (DataCount > 5 && DataCount < 7)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];

                            }
                            else if (DataCount == 7)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];

                            }

                            dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);

                            try
                            {
                                FacebookUser objFacebookUser = new FacebookUser();
                                objFacebookUser.username = accountUser;
                                objFacebookUser.password = accountPass;
                                objFacebookUser.proxyip = proxyAddress;
                                objFacebookUser.proxyport = proxyPort;
                                objFacebookUser.proxyusername = proxyUserName;
                                objFacebookUser.proxypassword = proxyPassword;

                                FBGlobals.loadedAccountsDictionary.Add(objFacebookUser.username, objFacebookUser);

                                lstAccountsInGroupCampaignManager.Add(objFacebookUser.username + ":" + objFacebookUser.password);
                                lstAccountsInGroupMemberScraper.Add(objFacebookUser.username );


                                FBGlobals.listAccounts.Add(objFacebookUser.username + ":" + objFacebookUser.password + ":" + objFacebookUser.proxyip + ":" + objFacebookUser.proxyport + ":" + objFacebookUser.proxyusername + ":" + objFacebookUser.proxypassword);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            ///Set this to "0" if loading unprofiled accounts
                            string profileStatus = "0";
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Account has some problem : " + item);
                            GlobusLogHelper.log.Debug("Account has some problem : " + item);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                   
                }


                #region lstAccountsInGroupMemberScraper add
                try
                {
                    if (lstAccountsInGroupMemberScraper.Count > 0)
                    {
                        cmbScraper__GroupMemberScraper_Accounts.Items.Clear();
                        foreach (string Account in lstAccountsInGroupMemberScraper)
                        {
                            try
                            {
                                if (cmbScraper__GroupMemberScraper_Accounts.InvokeRequired)
                                {
                                    cmbScraper__GroupMemberScraper_Accounts.Invoke(new MethodInvoker(delegate
                                                {
                                                    cmbScraper__GroupMemberScraper_Accounts.Items.Add(Account);
                                                }));
                                }
                                else
                                {
                                    cmbScraper__GroupMemberScraper_Accounts.Items.Add(Account);
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


                #region lstAccountsInCustomAudiencesScraper add
                try
                {
                    if (lstAccountsInGroupMemberScraper.Count > 0)
                    {
                         cmbScraper__CustomAudiencesScraper_Accounts.Items.Clear();
                        foreach (string Account in lstAccountsInGroupMemberScraper)
                        {
                            try
                            {
                                if ( cmbScraper__CustomAudiencesScraper_Accounts.InvokeRequired)
                                {
                                     cmbScraper__CustomAudiencesScraper_Accounts.Invoke(new MethodInvoker(delegate
                                                {
                                                     cmbScraper__CustomAudiencesScraper_Accounts.Items.Add(Account);
                                                }));
                                }
                                else
                                {
                                     cmbScraper__CustomAudiencesScraper_Accounts.Items.Add(Account);
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
               



                #region For Load Accounts in GroupCampaignManager
                try
                {
                    if (lstAccountsInGroupCampaignManager.Count > 0)
                    {
                        cmbGroups_GroupCampaignManager_Accounts.Items.Clear();
                        foreach (string Account in lstAccountsInGroupCampaignManager)
                        {
                            try
                            {
                                string[] AccontArr = Account.Split(':');
                                if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                {
                                    cmbGroups_GroupCampaignManager_Accounts.Invoke(new MethodInvoker(delegate
                                                {
                                                    cmbGroups_GroupCampaignManager_Accounts.Items.Add(AccontArr[0]);
                                                }));
                                }
                                else
                                {
                                    cmbGroups_GroupCampaignManager_Accounts.Items.Add(Account);
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
                try
                {
                    if (lstAccountsInGroupCampaignManager.Count > 0)
                    {
                        cmbScraper__fanscraper_Accounts.Items.Clear();
                        foreach (string Account in lstAccountsInGroupCampaignManager)
                        {
                            try
                            {
                                string[] AccountPoss = Account.Split(':');
                                string Account1 = AccountPoss[0];

                                if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                {
                                    cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                                    {                                      
                                        cmbScraper__fanscraper_Accounts.Items.Add(Account1);
                                       
                                    }));
                                }
                                else
                                {
                                    cmbScraper__fanscraper_Accounts.Items.Add(Account);
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
                #region For Load Accounts in FanPageSCraper


                #endregion
             

                try
                {
                    grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                    {
                        grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = dt;
                    }));

                    lblaccounts_ManageAccounts_LoadsAccountsCount.Invoke(new MethodInvoker(delegate
                    {
                        if (dt.Rows.Count > 0)
                        {
                            lblaccounts_ManageAccounts_LoadsAccountsCount.Text = dt.Rows.Count.ToString();
                        }
                    }));

                    DateTime eTime = DateTime.Now;

                    string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                

                    //Insert Seeting Into Database
                    objSetting.Module = "Accounts_ManageAccounts";
                    objSetting.FileType = "Accounts_ManageAccounts_LoadAccounts";
                    objSetting.FilePath = fileName;

                    InsertOrUpdateSetting(objSetting);

                    GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

                    GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

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

        private void LoadAccounts()
        {
            //Globals.IsFreeVersion = true;
            if (cmbScraper__fanscraper_Accounts.InvokeRequired)
            {
                cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                {
                    cmbScraper__fanscraper_Accounts.Items.Clear();
                }));
            }

             if (cmbScraper__fanscraper_Accounts.InvokeRequired)
            {
                cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                {
                    cmbGroups_GroupCampaignManager_Accounts.Items.Clear();
                }));
            }
       
             if (cmbScraper__GroupMemberScraper_Accounts.InvokeRequired)
             {
                 cmbScraper__GroupMemberScraper_Accounts.Invoke(new MethodInvoker(delegate
                 {
                     cmbScraper__GroupMemberScraper_Accounts.Items.Clear();
                 }));
             }
             if (cmbScraper__CustomAudiencesScraper_Accounts.InvokeRequired)
             {
                 cmbScraper__CustomAudiencesScraper_Accounts.Invoke(new MethodInvoker(delegate
                 {
                     cmbScraper__CustomAudiencesScraper_Accounts.Items.Clear();
                 }));
             }
                           
            try
            {
                DataSet ds;

                DataTable dt = new DataTable();


                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        if (lblAccounts_ManageAccounts_LoadsAccountsPath.InvokeRequired)
                        {
                            lblAccounts_ManageAccounts_LoadsAccountsPath.Invoke(new MethodInvoker(delegate
                            {
                                lblAccounts_ManageAccounts_LoadsAccountsPath.Text = ofd.FileName;
                            }));
                        }
                        else
                        {
                            lblAccounts_ManageAccounts_LoadsAccountsPath.Text = ofd.FileName;
                        }

                        dt.Columns.Add("UserName");
                        dt.Columns.Add("Password");
                        dt.Columns.Add("ProxyAddress");
                        dt.Columns.Add("ProxyPort");
                        dt.Columns.Add("ProxyUserName");
                        dt.Columns.Add("ProxyPassword");


                        ds = new DataSet();
                        ds.Tables.Add(dt);

                        if (grvAccounts_ManageAccounts_ManageAccountsDetails.InvokeRequired)
                        {
                            grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                            {
                                grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                            }));
                        }
                        else
                        {
                            grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                        }

                        List<string> templist = GlobusFileHelper.ReadFile(ofd.FileName);

                        if (templist.Count > 0)
                        {
                            FBGlobals.loadedAccountsDictionary.Clear();
                            FBGlobals.listAccounts.Clear();
                        }
                        int counter = 0;
                        foreach (string item in templist)
                        {
                            if (Globals.CheckLicenseManager == "fdfreetrial" && counter==5)
                            {
                                break;
                            }
                            counter = counter + 1;
                                try
                                {
                                    string account = item;
                                    string[] AccArr = account.Split(':');
                                    if (AccArr.Count() > 1)
                                    {
                                        string accountUser = account.Split(':')[0];
                                        string accountPass = account.Split(':')[1];
                                        string proxyAddress = string.Empty;
                                        string proxyPort = string.Empty;
                                        string proxyUserName = string.Empty;
                                        string proxyPassword = string.Empty;
                                        string status = string.Empty;

                                        int DataCount = account.Split(':').Length;
                                        if (DataCount == 2)
                                        {
                                            //Globals.accountMode = AccountMode.NoProxy;

                                        }
                                        else if (DataCount == 4)
                                        {

                                            proxyAddress = account.Split(':')[2];
                                            proxyPort = account.Split(':')[3];
                                        }
                                        else if (DataCount > 5 && DataCount < 7)
                                        {

                                            proxyAddress = account.Split(':')[2];
                                            proxyPort = account.Split(':')[3];
                                            proxyUserName = account.Split(':')[4];
                                            proxyPassword = account.Split(':')[5];

                                        }
                                        else if (DataCount == 7)
                                        {

                                            proxyAddress = account.Split(':')[2];
                                            proxyPort = account.Split(':')[3];
                                            proxyUserName = account.Split(':')[4];
                                            proxyPassword = account.Split(':')[5];

                                        }

                                        dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);

                                        try
                                        {
                                            FacebookUser objFacebookUser = new FacebookUser();
                                            objFacebookUser.username = accountUser;
                                            objFacebookUser.password = accountPass;
                                            objFacebookUser.proxyip = proxyAddress;
                                            objFacebookUser.proxyport = proxyPort;
                                            objFacebookUser.proxyusername = proxyUserName;
                                            objFacebookUser.proxypassword = proxyPassword;

                                            FBGlobals.loadedAccountsDictionary.Add(objFacebookUser.username, objFacebookUser);

                                            try
                                            {
                                                if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                                {
                                                    cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                                                    {
                                                        cmbScraper__fanscraper_Accounts.Items.Add(accountUser);
                                                    }));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                                
                                            try
                                            {
                                                if (cmbScraper__CustomAudiencesScraper_Accounts.InvokeRequired)
                                                {
                                                    cmbScraper__CustomAudiencesScraper_Accounts.Invoke(new MethodInvoker(delegate
                                                    {
                                                        cmbScraper__CustomAudiencesScraper_Accounts.Items.Add(accountUser);
                                                    }));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            try
                                            {
                                                if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                                                {
                                                    cmbGroups_GroupCampaignManager_Accounts.Invoke(new MethodInvoker(delegate
                                                    {
                                                        cmbGroups_GroupCampaignManager_Accounts.Items.Add(accountUser);
                                                    }));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            try
                                            {
                                                if (cmbScraper__GroupMemberScraper_Accounts.InvokeRequired)
                                                {
                                                    cmbScraper__GroupMemberScraper_Accounts.Invoke(new MethodInvoker(delegate
                                                    {
                                                        cmbScraper__GroupMemberScraper_Accounts.Items.Add(accountUser);
                                                    }));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                            FBGlobals.listAccounts.Add(objFacebookUser.username + ":" + objFacebookUser.password + ":" + objFacebookUser.proxyip + ":" + objFacebookUser.proxyport + ":" + objFacebookUser.proxyusername + ":" + objFacebookUser.proxypassword);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        ///Set this to "0" if loading unprofiled accounts
                                        ///
                                        string profileStatus = "0";


                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("Account has some problem : " + item);
                                        GlobusLogHelper.log.Debug("Account has some problem : " + item);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            
                        }

                        try
                        {
                            grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                            {
                                grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = dt;
                            }));

                            lblaccounts_ManageAccounts_LoadsAccountsCount.Invoke(new MethodInvoker(delegate
                            {
                                if (dt.Rows.Count > 0)
                                {
                                    lblaccounts_ManageAccounts_LoadsAccountsCount.Text = dt.Rows.Count.ToString();
                                }
                            }));

                            DateTime eTime = DateTime.Now;

                            string timeSpan = (eTime - sTime).TotalSeconds.ToString();


                            //Insert Seeting Into Database
                            objSetting.Module = "Accounts_ManageAccounts";
                            objSetting.FileType = "Accounts_ManageAccounts_LoadAccounts";
                            objSetting.FilePath = ofd.FileName;

                            InsertOrUpdateSetting(objSetting);

                            GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

                            GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");
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

        private void LoadExitingAccounts(string FilePath)
        {
            //Globals.IsFreeVersion = true;
            try
            {
                DataSet ds;

                DataTable dt = new DataTable();

                DateTime sTime = DateTime.Now;

                lblAccounts_ManageAccounts_LoadsAccountsPath.Invoke(new MethodInvoker(delegate
                {
                    lblAccounts_ManageAccounts_LoadsAccountsPath.Text = FilePath;
                }));

                dt.Columns.Add("UserName");
                dt.Columns.Add("Password");
                dt.Columns.Add("ProxyAddress");
                dt.Columns.Add("ProxyPort");
                dt.Columns.Add("ProxyUserName");
                dt.Columns.Add("ProxyPassword");


                ds = new DataSet();
                ds.Tables.Add(dt);

                grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                {
                    grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                }));

                List<string> templist = GlobusFileHelper.ReadFile(FilePath);

                if (templist.Count > 0)
                {
                    FBGlobals.loadedAccountsDictionary.Clear();
                    FBGlobals.listAccounts.Clear();
                }
                List<string> lstAccountsInGroupCampaignManager = new List<string>();
                List<string> lstAccountsInGroupMemberScraper = new List<string>();

                foreach (string item in templist)
                {
                    try
                    {
                        string account = item;
                        string[] AccArr = account.Split(':');
                        if (AccArr.Count() > 1)
                        {
                            string accountUser = account.Split(':')[0];
                            string accountPass = account.Split(':')[1];
                            string proxyAddress = string.Empty;
                            string proxyPort = string.Empty;
                            string proxyUserName = string.Empty;
                            string proxyPassword = string.Empty;
                            string status = string.Empty;

                            int DataCount = account.Split(':').Length;
                            if (DataCount == 2)
                            {
                                //Globals.accountMode = AccountMode.NoProxy;

                            }
                            else if (DataCount == 4)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                            }
                            else if (DataCount > 5 && DataCount < 7)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];

                            }
                            else if (DataCount == 7)
                            {

                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];

                            }

                            dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);

                            try
                            {
                                FacebookUser objFacebookUser = new FacebookUser();
                                objFacebookUser.username = accountUser;
                                objFacebookUser.password = accountPass;
                                objFacebookUser.proxyip = proxyAddress;
                                objFacebookUser.proxyport = proxyPort;
                                objFacebookUser.proxyusername = proxyUserName;
                                objFacebookUser.proxypassword = proxyPassword;

                                FBGlobals.loadedAccountsDictionary.Add(objFacebookUser.username, objFacebookUser);

                                lstAccountsInGroupMemberScraper.Add(objFacebookUser.username );

                                lstAccountsInGroupCampaignManager.Add(objFacebookUser.username + ":" + objFacebookUser.password);
                                FBGlobals.listAccounts.Add(objFacebookUser.username + ":" + objFacebookUser.password + ":" + objFacebookUser.proxyip + ":" + objFacebookUser.proxyport + ":" + objFacebookUser.proxyusername + ":" + objFacebookUser.proxypassword);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            ///Set this to "0" if loading unprofiled accounts
                            string profileStatus = "0";
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Account has some problem : " + item);
                            GlobusLogHelper.log.Debug("Account has some problem : " + item);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                #region For Load Accounts in GroupCampaignManager
                try
                {
                    if (lstAccountsInGroupCampaignManager.Count > 0)
                    {
                        foreach (string Account in lstAccountsInGroupCampaignManager)
                        {
                            try
                            {
                                cmbGroups_GroupCampaignManager_Accounts.Items.Add(Account);
                                

                            }
                            catch { };

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
                    if (lstAccountsInGroupMemberScraper.Count > 0)
                    {
                        foreach (string Account in lstAccountsInGroupMemberScraper)
                        {
                            try
                            {
                                cmbScraper__GroupMemberScraper_Accounts.Items.Add(Account);
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
                    grvAccounts_ManageAccounts_ManageAccountsDetails.Invoke(new MethodInvoker(delegate
                    {
                        grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = dt;
                    }));

                    lblaccounts_ManageAccounts_LoadsAccountsCount.Invoke(new MethodInvoker(delegate
                    {
                        if (dt.Rows.Count > 0)
                        {
                            lblaccounts_ManageAccounts_LoadsAccountsCount.Text = dt.Rows.Count.ToString();
                        }
                    }));

                    DateTime eTime = DateTime.Now;

                    string timeSpan = (eTime - sTime).TotalSeconds.ToString();


                    //Insert Seeting Into Database
                    objSetting.Module = "Accounts_ManageAccounts";
                    objSetting.FileType = "Accounts_ManageAccounts_LoadAccounts";
                    objSetting.FilePath = FilePath;

                    InsertOrUpdateSetting(objSetting);

                    GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

                    GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

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

        private void btnFriends_RequestFriends_AcceptRequest_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objFriendManager.isRequestFriendsStop = false;
                    objFriendManager.lstRequestFriendsThreads.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        FriendManager.minDelayFriendManager = Convert.ToInt32(txtFriends_FriendsAccept_DelayMin.Text);
                        FriendManager.maxDelayFriendManager = Convert.ToInt32(txtFriends_FriendsAccept_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtFriends_AcceptFriends_Threads.Text) && checkNo.IsMatch(txtFriends_AcceptFriends_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtFriends_AcceptFriends_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    try
                    {
                        FriendManager.Friends_AcceptFriends_Female = Friends_AcceptFriends_Female.Checked;
                        FriendManager.Friends_AcceptFriends_Male = Friends_AcceptFriends_Male.Checked;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        FriendManager.NoOfFriendRequestFriendManager = Convert.ToInt32(txtFriends_AcceptFriends_NoOfFriends.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        FriendManager.Friends_AcceptSendFrndToSuggestions = Friends_AcceptFriends_SendFrndToSuggestions.Checked;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        FriendManager.Friends_AcceptSendFrndProcessUsing = cmbFriends_AcceptFriendsRequest_StartProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    //cmbFriends_AcceptFriendsRequest_StartProcessUsing

                    FriendManager.NoOfThreads = threads;       
                    if (chk_UseTargetedProfileUrls.Checked)
                    {
                           FriendManager.Friends_AcceptFriends_CheckTargeted = true;
                    }
                    if((chk_UseTargetedProfileUrls.Checked) && (objFriendManager.lstFriendsUrlToSuggestFriends.Count==0))
                    {
                        GlobusLogHelper.log.Info("Please Load Targeted Profile Urls!");
                        GlobusLogHelper.log.Debug("Please Load Targeted Profile Urls!");
                        return;
                    }

                    Thread acceptFriendRequestThread = new Thread(objFriendManager.StartAcceptFriendRequest);
                    acceptFriendRequestThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_AcceptFriends_StopRequest_Click(object sender, EventArgs e)
        {
            try
            {
                objFriendManager.isRequestFriendsStop = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendManager.lstRequestFriendsThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendManager.lstRequestFriendsThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnFriends_RequestFriends_LoadKeywords_Click(object sender, EventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaKeywords.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        objFriendManager.lstRequestFriendsKeywords.Clear();

                        objFriendManager.lstRequestFriendsKeywords = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_RequestFriends";
                        objSetting.FileType = "Friends_RequestFriends_LoadKeywords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Keywords Loaded : " + objFriendManager.lstRequestFriendsKeywords.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Keywords Loaded : " + objFriendManager.lstRequestFriendsKeywords.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_RequestFriends_LoadFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        objFriendManager.lstRequestFriendsFanPageURLs.Clear();

                        objFriendManager.lstRequestFriendsFanPageURLs = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_RequestFriends";
                        objSetting.FileType = "Friends_RequestFriends_LoadFanPageURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + objFriendManager.lstRequestFriendsFanPageURLs.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + objFriendManager.lstRequestFriendsFanPageURLs.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnAccounts_ManageProfiles_StopCreateProfiles_Click(object sender, EventArgs e)
        {
            try
            {
                ObjAccountManager.isStopManageProfiles = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = ObjAccountManager.lstManageProfilesThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        ObjAccountManager.lstManageProfilesThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnScrapers_FanPageScraper_LoadFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_FanPageScraper_LoadFanPageURLsPath.Text = ofd.FileName;
                        objPageManager.lstFanPageURLs.Clear();
                        objPageManager.lstFanPageURLs = GlobusFileHelper.ReadFile(ofd.FileName);
                        objPageManager.lstFanPageURLs = objPageManager.lstFanPageURLs.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScrapers_FanPageScraper_LoadFanPageURLsCount.Text = objPageManager.lstFanPageURLs.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_FanPageScraper";
                        objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + objPageManager.lstFanPageURLs.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + objPageManager.lstFanPageURLs.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FanPageScraper_ExtractIds_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objPageManager.isStopFanPageScraper = false;
                    objPageManager.lstFanPageScraperThreads.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_FanPageScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_FanPageScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_FanPageScraper_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    PageManager.NoOfThreads = threads;


                    if (string.IsNullOrEmpty(EventScraper.ScrapersFansScraperExprotFilePath))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    try
                    {
                        PageManager.StartProcessUsingFanPageScraper = cmbScraper_FanPageScraper_ProcessUsing.SelectedItem.ToString();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }
                   


                    try
                    {
                        if (PageManager.StartProcessUsingFanPageScraper == "Fans Scraper by Urls")
                        {
                            if (objPageManager.lstFanPageURLs.Count()==0)
                            {
                                 GlobusLogHelper.log.Info("Please Load Fans Urls !");
                                 GlobusLogHelper.log.Debug("Please Load Fans Urls !");
                                 return;
                            }
                        }
                        else if (PageManager.StartProcessUsingFanPageScraper == "Fan Page Scraper  ")
                        {
                            if (objPageManager.lstFanPageKeywords.Count()==0 )
                            {
                                  GlobusLogHelper.log.Info("Please Load Keywords !");
                                 GlobusLogHelper.log.Debug("Please Load Keywords !");
                                 return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                   
                    try
                    {
                        if (PageManager.StartProcessUsingFanPageScraper == "Fan Page Scraper  ")
                        {
                            PageManager.FanPageScraperUsingAccount = cmbScraper__fanscraper_Accounts.SelectedItem.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                   
                    //Thread extractFanPageIdsThread = new Thread(objPageManager.StartGetExtractIds);
                    Thread extractFanPageIdsThread = new Thread(objPageManager.StartGetExtractIdsNew);
                    extractFanPageIdsThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnCreateEmials_Click(object sender, EventArgs e)
        {
            Captchas.FrmCaptcha objCaptcha = new Captchas.FrmCaptcha();
            objCaptcha.ShowDialog();
            //Captchas.FrmCaptcha objCaptcha = new Captchas.FrmCaptcha();
            //objCaptcha.ShowDialog();
        }

        private void btnEmails_EmailCreator_loadFirstName_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        //lblEmails_EmailCreator_FirstNamePath.Text = ofd.FileName;

                        try
                        {
                            lstLoadedFirstNamesForEmail.Clear();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        LoadFirstNameForEmail(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                     //   lblEmails_EmailCreator_CountCreatedFirstName.Text = lstLoadedFirstNamesForEmail.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Emails_EmailCreator";
                        objSetting.FileType = "Emails_EmailCreator_loadFirstName";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("First Names Loaded : " + lstLoadedFirstNames.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Info("First Names Loaded : " + lstLoadedFirstNamesForEmail.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnEmails_EmailCreator_loadLastNames_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                      //  lblEmails_EmailCreator_LastNamePath.Text = ofd.FileName;
                        lstLoadedLastNamesForEmail.Clear();
                        LoadLastNameForEmail(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                       // lblEmails_EmailCreator_CountLastNames.Text = lstLoadedLastNamesForEmail.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Emails_EmailCreator";
                        objSetting.FileType = "Emails_EmailCreator_LoadLastNames";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("Last Names Loaded : " + lstLoadedLastNames.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Info("Last Names Loaded : " + lstLoadedLastNamesForEmail.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }

        private void btnEmails_EmailCreator_StartEmail_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                  //  FrmCaptcha.CheckDbcUserNamePassword = chkEmails_EmailCreator_DBC.Checked;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (lstLoadedFirstNamesForEmail.Count() < 1)
                {
                    MessageBox.Show("List of First name is Empty, Please upload First Name", "First Name Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (lstLoadedLastNamesForEmail.Count() < 1)
                {
                    MessageBox.Show("List of Last name is Empty, Please upload Last Name", "Last Name Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
              

              //  if (chkEmails_EmailCreator_DBC.Checked)
                {
                    int count = 1;
                    try
                    {
                        while (Globals.CheckStopAccountCreation)
                        {
                            Captchas.FrmCaptcha objCaptcha = new Captchas.FrmCaptcha(lstLoadedFirstNamesForEmail, lstLoadedLastNamesForEmail, CurrentCaptchaSetting, CurrentCaptchaUserName, CurrentCaptchaPassword);
                            objCaptcha.pageManagerEvent.handleParamsEvent += new EventHandler(pageManagerEvent_handleParamsEvent);
                            objCaptcha.ShowDialog();
                            GetAllEmails();
                            count = count + 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
               // }
                //else
                //{
                //    try
                //    {
                //        Captchas.FrmCaptcha objCaptcha = new Captchas.FrmCaptcha(lstLoadedFirstNamesForEmail, lstLoadedLastNamesForEmail, CurrentCaptchaSetting, CurrentCaptchaUserName, CurrentCaptchaPassword);
                //        objCaptcha.pageManagerEvent.handleParamsEvent += new EventHandler(pageManagerEvent_handleParamsEvent);
                //        objCaptcha.ShowDialog();
                //        GetAllEmails();
                //    }
                //    catch (Exception ex)
                //    {
                //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                //    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }

        }

        bool isChecked = false;
        private void rdbFriends_RequestFriends_SearchViaDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdbFriends_RequestFriends_SearchViaDatabase.Checked && !isChecked)
                    rdbFriends_RequestFriends_SearchViaDatabase.Checked = false;
                else
                {
                    rdbFriends_RequestFriends_SearchViaDatabase.Checked = true;
                    isChecked = false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }

        private void rdbFriends_RequestFriends_SearchViaDatabase_CheckedChanged(object sender, EventArgs e)
        {
            isChecked = rdbFriends_RequestFriends_SearchViaDatabase.Checked;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            Captchas.FrmCaptcha objCaptcha = new Captchas.FrmCaptcha(lstLoadedFirstNamesForEmail, lstLoadedLastNamesForEmail, CurrentCaptchaSetting, CurrentCaptchaUserName, CurrentCaptchaPassword);
            objCaptcha.ShowDialog();
        }

        private void btnScrapers_FanPageScraper_ExtractIds_MouseHover(object sender, EventArgs e)
        {
            try
            {
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(this.btnScrapers_FanPageScraper_ExtractIds, "Extract Like User Ids Who Have Liked The Fan Pages !");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageLiker_LoadFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objPageManager.lstFanPageUrlsFanPageLiker = lstTemp.Distinct().ToList();

                        
                        DateTime eTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageURLsCount.Text = objPageManager.lstFanPageUrlsFanPageLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPageLiker";
                        objSetting.FileType = "Pages_FanPageLiker_LoadFanPageURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + objPageManager.lstFanPageUrlsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + objPageManager.lstFanPageUrlsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageLiker_LoadFanPageMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageMessagePath.Text = ofd.FileName;

                        objPageManager.lstFanPageMessageFanPageLiker.Clear();

                        objPageManager.lstFanPageMessageFanPageLiker = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageMessageCount.Text = objPageManager.lstFanPageMessageFanPageLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPageLiker";
                        objSetting.FileType = "Pages_FanPageLiker_LoadFanPageMessage";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Message Loaded : " + objPageManager.lstFanPageMessageFanPageLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Message Loaded : " + objPageManager.lstFanPageMessageFanPageLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageLiker_LoadFanPageComments_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageCommentsPath.Text = ofd.FileName;

                        objPageManager.lstFanPageCommentsFanPageLiker.Clear();

                        objPageManager.lstFanPageCommentsFanPageLiker = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageCommentsCount.Text = objPageManager.lstFanPageCommentsFanPageLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPageLiker";
                        objSetting.FileType = "Pages_FanPageLiker_LoadFanPageComments";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Comments Loaded : " + objPageManager.lstFanPageCommentsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Comments Loaded : " + objPageManager.lstFanPageCommentsFanPageLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageLiker_StartFanPageLiker_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objPageManager.isStopFanPageLiker = false;
                    objPageManager.lstThreadsFanPageLiker.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtPages_FanPageLiker_Threads.Text) && checkNo.IsMatch(txtPages_FanPageLiker_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtPages_FanPageLiker_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    PageManager.NoOfThreads = threads;

                    foreach (string item in objPageManager.lstFanPageMessageFanPageLiker)
                    {
                        PageManager.queFanPageMessagesFanPageLiker.Enqueue(item);
                    }
                    foreach (string item in objPageManager.lstFanPageUrlsFanPageLiker)
                    {
                        PageManager.queFanPageURLsFanPageLiker.Enqueue(item);
                    }
                    foreach (string item in objPageManager.lstFreindsPagePostsLiker)
                    {
                        PageManager.queueFriendsUrlFriendsPostLiker.Enqueue(item);                
                    }
                    try
                    {
                        PageManager.StartProcessUsingFanPageLiker = cmbPages_FanPageLiker_StartFanPageLikerProcessUsing.SelectedItem.ToString();
                        if (chkLikePostsWithUploadedUrl.Checked)
                        {
                            objPageManager.isLikePostThroughFreindsUrls = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using Drop down list.");
                        GlobusLogHelper.log.Info("Please Select Start Process Using Drop Down List.");
                        return;

                    }

                    try
                    {
                        PageManager.NoOfPostFanPageLikercount = Convert.ToInt32(txtPages_FanPageLiker_NoofPostPerAccount.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    try
                    {
                        PageManager.minDelayFanPageLiker = Convert.ToInt32(txtPage_FanPageLiker_DelayMin.Text);
                        PageManager.maxDelayFanPageLiker = Convert.ToInt32(txtPage_FanPageLiker_DelayMax.Text);
                       
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread createProfileThread = new Thread(objPageManager.StartLikePage);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnEvents_EventInviter_LoadEventURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblEvents_EventInviter_EventURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objEventManager.LstEventURLsEventInviter = lstTemp.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        lblEvents_EventInviter_EventURLsCount.Text = objEventManager.LstEventURLsEventInviter.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Events_EventInviter";
                        objSetting.FileType = "Events_EventInviter_LoadEventURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Event URLs Loaded : " + objEventManager.LstEventURLsEventInviter.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Event URLs Loaded : " + objEventManager.LstEventURLsEventInviter.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnEvents_EventInviter_InviteFriends_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objEventManager.isStopEventInviter = false;
                    objEventManager.lstThreadsEventInviter.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;


                    try
                    {
                        EventManager.minDelayEventInvitor = Convert.ToInt32(txtEvent_EventInviter_DelayMin.Text);
                        EventManager.maxDelayEventInvitor = Convert.ToInt32(txtEvent_EventInviter_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (!string.IsNullOrEmpty(txtEvents_EventInviter_Threads.Text) && checkNo.IsMatch(txtEvents_EventInviter_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtEvents_EventInviter_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    try
                    {
                        objEventManager.NoOfThreadsEventInviter = threads;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (objEventManager.LstEventURLsEventInviter==null)//&& objEventManager.LstEventURLsEventInviter.Count < 1
                    {
                        MessageBox.Show("Please Load Event URLs !");
                        return;
                    }
                    //intNoOfFriends
                    try
                    {
                        objEventManager.NoOfFriendsSuggestionAtOneTimeEventInviter = Convert.ToInt32(cmbEvents_EventInviter_NoOfFriendsToSuggestAtOneTime.SelectedItem);
                        EventManager.intNoOfFriends = Convert.ToInt32(cmbEvents_EventInviter_NoOfFriendsToSuggestAtOneTime.SelectedItem);
                    }
                    catch (Exception)
                    {
                        cmbEvents_EventInviter_NoOfFriendsToSuggestAtOneTime.SelectedItem = 10;
                        objEventManager.NoOfFriendsSuggestionAtOneTimeEventInviter = 10;
                    }

                    try
                    {
                        objEventManager.NoOfSuggestionPerAccountEventInviter = Convert.ToInt32(txtEvents_EventInviter_NoOfSuggestionsPerAccount.Text);

                    }
                    catch (Exception)
                    {
                        txtEvents_EventInviter_NoOfSuggestionsPerAccount.Text = Convert.ToString(30);
                        objEventManager.NoOfSuggestionPerAccountEventInviter = 30;
                    }

                    if (chkEvents_EventInviter_SendToAll.Checked)
                    {
                        objEventManager.SendToAllFriendsEventInviter = true;
                    }
                    else
                    {
                        objEventManager.SendToAllFriendsEventInviter = false;
                    }


                    Thread createProfileThread = new Thread(objEventManager.StartEventInviter);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_EventsScraper_LoadEventURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_EventsScraper_EventURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        List<string> lstTempt = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        foreach (var item in lstTemp)
                        {
                            if (item.Contains("https://www.facebook.com/events/") || item.Contains("http://www.facebook.com/events/"))
                            {
                                lstTempt.Add(item);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Invalid Event Url : " + item);
                                GlobusLogHelper.log.Debug("Invalid Event Url : " + item);
                               
                            }
                        }

                        objEventScraper.LstEventURLsEventScraper = lstTempt.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_EventsScraper_EventURLsCount.Text = objEventScraper.LstEventURLsEventScraper.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_EventsScraper";
                        objSetting.FileType = "Scrapers_EventsScraper_LoadEventURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Event URLs Loaded : " + objEventScraper.LstEventURLsEventScraper.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Event URLs Loaded : " + objEventScraper.LstEventURLsEventScraper.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_EventsScraper_ExtractEventMember_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objEventScraper.isStopEventScraper = false;
                    objEventScraper.lstThreadsEventScraper.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_EventsScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_EventsScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_EventsScraper_Threads.Text);
                    }

                    if (string.IsNullOrEmpty(EventScraper.ScrapersExprotFilePath))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }



                    try
                    {
                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objEventScraper.NoOfThreadsEventScraper = threads;

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                     if (objEventScraper.LstEventURLsEventScraper.Count < 1)
                    {
                        MessageBox.Show("Please Load Event URLs !");
                        return;
                    }


                    Thread createProfileThread = new Thread(objEventScraper.StartEventScraper);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnEvents_EventCreator_LoadEventDetails_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblEvents_EventCreator_EventDetailsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objEventManager.LstEventDetailsEventCreator = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblEvents_EventCreator_EventDetailsCount.Text = objEventManager.LstEventDetailsEventCreator.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Events_EventCreator";
                        objSetting.FileType = "Events_EventCreator_LoadEventDetails";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Event Details Loaded : " + objEventManager.LstEventDetailsEventCreator.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Event Details Loaded : " + objEventManager.LstEventDetailsEventCreator.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnEvents_EventCreator_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objEventManager.isStopEvenCreator = false;
                    objEventManager.lstThreadsEvenCreator.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        EventManager.minDelayEventInvitor = Convert.ToInt32(txtEvent_EventInviter_DelayMin.Text);
                        EventManager.maxDelayEventInvitor = Convert.ToInt32(txtEvent_EventInviter_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtEvents_EventCreator_Threads.Text) && checkNo.IsMatch(txtEvents_EventCreator_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtEvents_EventCreator_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objEventManager.NoOfThreadsEvenCreator = threads;

                    if (objEventManager.LstEventDetailsEventCreator.Count < 1)
                    {
                        MessageBox.Show("Please Load Event Details !");
                        return;
                    }
                    else
                    {

                        if ((objEventManager.LstEventDetailsEventCreator.Contains("Name") && objEventManager.LstEventDetailsEventCreator.Contains("name")) && (objEventManager.LstEventDetailsEventCreator.Contains("Details") && objEventManager.LstEventDetailsEventCreator.Contains("details")) && (objEventManager.LstEventDetailsEventCreator.Contains("Where") && objEventManager.LstEventDetailsEventCreator.Contains("where")) && (objEventManager.LstEventDetailsEventCreator.Contains("When") && objEventManager.LstEventDetailsEventCreator.Contains("when")))
                        {
                            MessageBox.Show("Please Load Event Details In This Format --> Name <Party> <:> Details <Birthday Party> <:> Where <India> <:> When <06/25/13> <:> Add a time <12:45 pm> <:> Privacy <Public>");
                            return;
                        }
                    }



                    Thread createProfileThread = new Thread(objEventManager.StartEvenCreator);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];     
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabMain.SelectedTab == tabMain.TabPages["tabPageAccounts"])
                {
                    BindAccountCreatorFilePath();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageFriends"])
                {
                    BindRequestFriendsFilePath();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPagePages"])
                {
                    BindFanPageLikerFilePath();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageEvents"])
                {
                    BindEventCreatorFilePath();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageScrapers"])
                {
                    //BindFanPageScraperFilePath();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageMessages"])
                {
                    //BindFanPageScraperFilePath();
                    BindMessagesInGRV();
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageScheduler"])
                {
                    List<string> lstSchAccounts = GetAllSchedulerAccount();

                    BindAccountInSchedulerGrid(lstSchAccounts);
                }

                if (tabMain.SelectedTab == tabMain.TabPages["tabPageCampaigns"])
                {
                    List<string> lstCmpAccounts = GetAllCampaignAccount();

                    BindAccountInCmpGrid(lstCmpAccounts);
                }



            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void BindMessagesInGRV()
        {
            try
            {
                if (grvMessages_MessageReply_MessageDetails.Rows.Count > 0)
                {
                    return;
                }

                ICollection<faceboardpro.Domain.Message> lstMesages = objMessageRepository.GetAllMessage();

                if (lstMesages.Count > 0)
                {
                    if (grvMessages_MessageReply_MessageDetails.RowCount > 0)
                    {
                        DataGridViewCell CheckBox = grvMessages_MessageReply_MessageDetails.Rows[0].Cells[0];
                        if (CheckBox == null)
                        {
                            AddCheckBoxInGridView(ref grvMessages_MessageReply_MessageDetails);
                        }
                    }
                    else
                    {
                        AddCheckBoxInGridView(ref grvMessages_MessageReply_MessageDetails);
                    }

                    DataTable dtMessagedetails = new DataTable();
                    try
                    {

                        DataColumn col_UserName = new DataColumn("UserName");
                        col_UserName.ReadOnly = true;
                        DataColumn col_MessageFriendId = new DataColumn("MessageFriendId");
                        col_MessageFriendId.ReadOnly = true;
                        DataColumn col_MessageSnippedId = new DataColumn("MessageSnippedId");
                        col_MessageSnippedId.ReadOnly = true;
                        DataColumn col_MessageSenderName = new DataColumn("MessageSenderName");
                        col_MessageSenderName.ReadOnly = true;
                        DataColumn col_MessagingReadParticipants = new DataColumn("MessagingReadParticipants");
                        col_MessagingReadParticipants.ReadOnly = true;
                        DataColumn col_MessageText = new DataColumn("MessageText");
                        col_MessageText.ReadOnly = true;
                        DataColumn col_SetColor = new DataColumn("SetColor");
                        col_SetColor.ReadOnly = true;

                        dtMessagedetails.Columns.Add(col_UserName);
                        dtMessagedetails.Columns.Add(col_MessageFriendId);
                        dtMessagedetails.Columns.Add(col_MessageSnippedId);
                        dtMessagedetails.Columns.Add(col_MessageSenderName);
                        dtMessagedetails.Columns.Add(col_MessagingReadParticipants);
                        dtMessagedetails.Columns.Add(col_MessageText);
                        dtMessagedetails.Columns.Add(col_SetColor);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    foreach (var item in lstMesages)
                    {
                        try
                        {
                            //if (item["MsgFriendId"].ToString() != item["UserId"].ToString())
                            if (item.MsgFriendId != item.UserId)
                            {
                                DataRow dr = dtMessagedetails.NewRow();

                                dr["UserName"] = (item.UserName.ToString());
                                dr["MessageFriendId"] = (item.MsgFriendId.ToString());
                                dr["MessageSnippedId"] = (item.MsgSnippedId.ToString());
                                dr["MessageSenderName"] = (item.MsgSenderName.ToString());
                                dr["MessagingReadParticipants"] = (item.MessagingReadParticipants.ToString());
                                dr["MessageText"] = item.MessageText;
                                dr["SetColor"] = "false";

                                dtMessagedetails.Rows.Add(dr);
                            }
                            else
                            {
                                DataRow dr = dtMessagedetails.NewRow();

                                dr["UserName"] = (item.UserName.ToString());
                                dr["MessageFriendId"] = (item.MsgFriendId.ToString());
                                dr["MessageSnippedId"] = (item.MsgSnippedId.ToString());
                                dr["MessageSenderName"] = (item.MsgSenderName.ToString());
                                dr["MessagingReadParticipants"] = (item.MessagingReadParticipants.ToString());
                                dr["MessageText"] = item.MessageText;
                                dr["SetColor"] = "True";

                                dtMessagedetails.Rows.Add(dr);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    grvMessages_MessageReply_MessageDetails.DataSource = dtMessagedetails;

                    if (grvMessages_MessageReply_MessageDetails.Rows.Count > 0)
                    {
                        for (int i = 0; i < grvMessages_MessageReply_MessageDetails.Rows.Count; i++)
                        {
                            try
                            {
                                string setColor = grvMessages_MessageReply_MessageDetails.Rows[i].Cells["SetColor"].Value.ToString();
                                if (setColor == "True")
                                {
                                    grvMessages_MessageReply_MessageDetails.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                                    grvMessages_MessageReply_MessageDetails.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }

                    grvMessages_MessageReply_MessageDetails.Columns[2].Visible = false;
                    grvMessages_MessageReply_MessageDetails.Columns[3].Visible = false;
                    grvMessages_MessageReply_MessageDetails.Columns[7].Visible = false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddCheckBoxInGridView(ref DataGridView dgvName)
        {
            try
            {
                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                checkBoxColumn.Width = 30;
                checkBoxColumn.Name = " Select";
                checkBoxColumn.ReadOnly = false;
                dgvName.Columns.Add(checkBoxColumn);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddButtonInGridView(ref DataGridView dgvName, string btnName, int dispalyIndex, Color foreColor, Color backColor)
        {
            try
            {
                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.DefaultCellStyle.BackColor = backColor;
                buttonColumn.Width = 20;
                buttonColumn.Name = btnName;
                buttonColumn.UseColumnTextForButtonValue = true;
                buttonColumn.Text = btnName;
                buttonColumn.ReadOnly = true;
                buttonColumn.DisplayIndex = dispalyIndex;

                dgvName.Columns.Insert(dispalyIndex, buttonColumn);

                //dgvName.Columns[btnName].DefaultCellStyle.ForeColor = foreColor;

                //dgvName.Columns[btnName].DefaultCellStyle.BackColor = backColor;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddComboBoxInGridView(ref DataGridView dgvName, string cmbName, int dispalyIndex, Color foreColor, Color backColor, List<string> lstitem)
        {
            try
            {
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
                comboBoxColumn.Name = cmbName;
                comboBoxColumn.HeaderText = cmbName;


                //comboBoxColumn.Items.AddRange(lstitem);

                foreach (string item in lstitem)
                {
                    try
                    {
                        comboBoxColumn.Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                dgvName.Columns.Insert(dispalyIndex, comboBoxColumn);

                for (int i = 0; i < dgvName.Rows.Count; i++)
                {
                    try
                    {
                        dgvName.Rows[i].Cells[cmbName].Value = (comboBoxColumn.Items[0].ToString());
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                //dgvName.Columns[btnName].DefaultCellStyle.ForeColor = foreColor;

                //dgvName.Columns[btnName].DefaultCellStyle.BackColor = backColor;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddLinkInGridView(ref DataGridView dgvName, string btnName, int dispalyIndex, Color foreColor, Color backColor)
        {
            try
            {
                DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
                linkColumn.Width = 20;
                linkColumn.Name = btnName;
                linkColumn.UseColumnTextForLinkValue = true;
                linkColumn.Text = btnName;
                linkColumn.ReadOnly = true;
                linkColumn.DisplayIndex = dispalyIndex;
                //dgvName.Columns[btnName].DefaultCellStyle.ForeColor = foreColor;
                //dgvName.Columns[btnName].DefaultCellStyle.BackColor = backColor;
                dgvName.Columns.Insert(dispalyIndex, linkColumn);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddTextBoxInGridView(ref DataGridView dgvName, string btnName, int dispalyIndex, Color foreColor, Color backColor)
        {
            try
            {
                DataGridViewTextBoxColumn textBoxColumn = new DataGridViewTextBoxColumn();
                textBoxColumn.Name = btnName;
                textBoxColumn.HeaderText = btnName;
                textBoxColumn.ReadOnly = false;
                dgvName.Columns.Insert(dispalyIndex, textBoxColumn);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AddCalendarColumnInGridView(ref DataGridView dgvName, string Name, int dispalyIndex, Color foreColor, Color backColor)
        {
            try
            {
                BaseLib.CalendarColumn calendarColumn = new BaseLib.CalendarColumn();
                calendarColumn.Name = Name;
                calendarColumn.HeaderText = Name;
                calendarColumn.ReadOnly = false;
                dgvName.Columns.Insert(dispalyIndex, calendarColumn);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabControlProfileManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabAccounts.SelectedTab == tabAccounts.TabPages["tabPageManageProfile"])
                {
                    BindManageProfileFilePath();
                }

                if (tabAccounts.SelectedTab == tabAccounts.TabPages["tabPageManageAccounts"])
                {
                    BindManageAccountsFilePath();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabFriends.SelectedTab == tabFriends.TabPages["tabPageFriendRequest"])
                {
                    BindRequestFriendsFilePath();
                }

                if (tabFriends.SelectedTab == tabFriends.TabPages["tabPageAcceptFriends"])
                {
                    //BindAcceptFriendsFilePath();
                }

                if (tabFriends.SelectedTab == tabFriends.TabPages["tabPageMassFriendsAdder"])
                {
                    //BindMassFriendsAdderFilePath();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabPagesPages.SelectedTab == tabPagesPages.TabPages["tabPageFanPageLiker"])
                {
                    BindFanPageLikerFilePath();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void tabEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabEvents.SelectedTab == tabEvents.TabPages["tabPageEventCreator"])
                {
                    BindEventCreatorFilePath();
                }

                if (tabEvents.SelectedTab == tabEvents.TabPages["tabPageEventInviter"])
                {
                    BindEventInviterFilePath();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabScrapers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabScrapers.SelectedTab == tabScrapers.TabPages["tabPageFanPageScraper"])
                {
                    //BindFanPageScraperFilePath();
                }

                if (tabScrapers.SelectedTab == tabScrapers.TabPages["tabPageEventsScraper"])
                {
                    BindEventScraperFilePath();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_MessageScraper_ExtractMessages_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objMessageScraper.isStopMessageScraper = false;
                    objMessageScraper.lstThreadsMessageScraper.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_MessageScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_MessageScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_MessageScraper_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objMessageScraper.NoOfThreadsMessageScraper = threads;


                    Thread createProfileThread = new Thread(objMessageScraper.StartMessageScraper);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnMessages_MessageReply_LoadReplyMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblMessages_MessageReply_ReplyMessagePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objMessageManager.LstReplyMessageMessageReply = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblMessages_MessageReply_ReplyMessageCount.Text = lstTemp.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Messages_ReplyMessage";
                        objSetting.FileType = "Messages_ReplyMessage_LoadReplyMessage";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Reply Message Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Reply Message Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnMessages_MessageReply_Start_Click(object sender, EventArgs e)
        {
            
            try
            {

                if (FBGlobals.listAccounts.Count > 0)
                {
                    objMessageManager.isStopMessageReply = false;
                    objMessageManager.lstThreadsMessageReply.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {

                        MessageManager.minDelayMessageReply = Convert.ToInt32(txtMessage_MessageReply_DelayMin.Text);
                        MessageManager.maxDelayMessageReply = Convert.ToInt32(txtMessage_MessageReply_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                    try
                    {
                        MessageManager.MessageMessageSendNoOfFriends = Convert.ToInt32(txtMessage_MessageReply_NoOfFriends.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        MessageManager.CheckSendMessageWithTage = chkMessage_MessageReply_SendMessageWithTag.Checked;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        MessageManager.ReplyMessageMessageSingle = txtMessages_MessageReply_SingleMessage.Text;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        MessageManager.MessageMessageReplyProcessUsing = CmbMessage_MessageReply_StartProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }
                    try
                    {
                        MessageManager.SendMessageUsingMessage = cmbMessages_MessageReply_SendMessageUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtMessages_MessageReply_Threads.Text) && checkNo.IsMatch(txtMessages_MessageReply_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtMessages_MessageReply_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objMessageManager.NoOfThreadsMessageReply = threads;

                    //Collect Selected Info For Reply in list => objMessageManager.LstReplyDetailsMessageReply

                    if (MessageManager.MessageMessageReplyProcessUsing == "SendMessageWithScraper")
                    {
                        objMessageManager.LstReplyDetailsMessageReply.Clear();
                        foreach (DataGridViewRow row in grvMessages_MessageReply_MessageDetails.Rows)
                        {
                            try
                            {

                                if (row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
                                {
                                    objMessageManager.LstReplyDetailsMessageReply.Add("<UserName>" + row.Cells[1].Value.ToString() + ":" + "<MessageFriendId>" + row.Cells[2].Value.ToString() + ":" + "<MessageSnippedId>" + row.Cells[3].Value.ToString() + ":" + "<MessageSenderName>" + row.Cells[4].Value.ToString() + ":" + "<MessagingReadParticipants>" + row.Cells[5].Value.ToString());
                                    txtMessages_Message_MessageBox.Text = row.Cells[6].Value.ToString();

                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                    }

                    if (objMessageManager.LstReplyDetailsMessageReply.Count < 1 && MessageManager.MessageMessageReplyProcessUsing == "SendMessageWithScraper")
                    {
                        GlobusLogHelper.log.Info("Please Select Friend OR Scrap Messages From Scraper Module !");
                        GlobusLogHelper.log.Debug("Please Select Friend OR Scrap Messages From Scraper Module !");

                        MessageBox.Show("Please Select Friend OR Scrap Messages From Scraper Module !");

                        tabMain.SelectedTab = tabMain.TabPages["tabPageScrapers"];
                        tabScrapers.SelectedTab = tabScrapers.TabPages["tabPageMessageScraper"]; 

                        //tabAccounts.SelectedTab = tabMain.TabPages["tabPageMessages"];                      
                        return;
                    }

                    if (cmbMessages_MessageReply_SendMessageUsing.SelectedItem.ToString() == "Random")
                    {
                        try
                        {
                            if (objMessageManager.LstReplyMessageMessageReply.Count < 1)
                            {
                                MessageBox.Show("Please Load Reply Message !");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }


                        objMessageManager.SendMessageUsingMessageReply = "Random";
                    }

                    if (cmbMessages_MessageReply_SendMessageUsing.SelectedItem.ToString() == "Single" && MessageManager.MessageMessageReplyProcessUsing == "SendMessageWithScraper")
                    {
                        if (string.IsNullOrEmpty(txtMessages_MessageReply_SingleMessage.Text))
                        {
                            MessageBox.Show("Please Write The Message In Single Message Box !");
                            return;
                        }

                        objMessageManager.ReplyMessageMessageReply = txtMessages_MessageReply_SingleMessage.Text;
                        objMessageManager.SendMessageUsingMessageReply = "Single";

                    }

                    try
                    {
                        int cntNoOfProfileUrls = MessageManager.MessageMessageLoadProfileUrl.Count;
                        int noOfUsers = FBGlobals.listAccounts.Count;
                        if (chkUseDivideData.Checked)
                        {
                            MessageManager.useDivideDataOption = true;
                        }
                        if (rdbDivideEqually.Checked)
                        {
                            if (noOfUsers < cntNoOfProfileUrls)
                            {
                                objMessageManager.NoProfileUrlsPerUser = (cntNoOfProfileUrls / noOfUsers);
                            }
                            else
                            {
                                objMessageManager.NoProfileUrlsPerUser = noOfUsers;
                            }
                        }
                        if(rdbDivideGivenByUser.Checked)
                        {
                            try
                            {
                                objMessageManager.NoProfileUrlsPerUser = int.Parse(txtNoOFMessagesPerAccount.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.Message);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                    Thread messageReplyThread = new Thread(objMessageManager.StartMessageReply);
                    messageReplyThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnMessages_MessageReply_LoadKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblMessages_MessageReply_KeywordsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        //objEventManager.LstEventDetailsEventCreator = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblMessages_MessageReply_KeywordsCount.Text = lstTemp.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Messages_ReplyMessage";
                        objSetting.FileType = "Messages_ReplyMessage_LoadKeywords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Keywords Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Keywords Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabMessages.SelectedTab == tabMessages.TabPages["tabPageMessageReply"])
                {
                    BindMessagesInGRV();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupInviter_LoadGroupURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupInviter_GroupURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objGroupManager.LstGroupUrlsGroupInviter = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblGroups_GroupInviter_GroupURLsCount.Text = objGroupManager.LstGroupUrlsGroupInviter.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();


                        foreach (var item in lstTemp)
                        {
                            if (item.Contains("group") && item.Contains("http"))
                            {

                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Invalid Group Url : " + item);
                                GlobusLogHelper.log.Debug("Invalid Group Url : "+ item);
                                objGroupManager.LstGroupUrlsGroupInviter.Remove(item);
                            }
                        }

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupInviter";
                        objSetting.FileType = "Groups_GroupInviter_LoadGroupURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group URLs Loaded : " + objGroupManager.LstGroupUrlsGroupInviter.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group URLs Loaded : " + objGroupManager.LstGroupUrlsGroupInviter.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupInviter_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objGroupManager.isStopGroupInviter = false;
                    objGroupManager.lstThreadsGroupInviter.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtEvents_EventCreator_Threads.Text) && checkNo.IsMatch(txtEvents_EventCreator_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtEvents_EventCreator_Threads.Text);
                    }

                    if (string.IsNullOrEmpty(GroupManager.GroupReportExprotFilePath))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    try
                    {
                        //Group_GroupInviterNoOfFriends
                        int GroupInviterNoOfFriends = Convert.ToInt32(Group_GroupInviter_NoOfFriends.Text);
                        objGroupManager.AddNoOfFriendsGroupInviter = GroupInviterNoOfFriends;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objGroupManager.NoOfThreadsGroupInviter = threads;

                    if (objGroupManager.LstGroupUrlsGroupInviter.Count < 1)
                    {
                        MessageBox.Show("Please Load Group URLs !");
                        return;
                    }
                    try
                    {

                        GroupManager.minDelayGroupInviter = Convert.ToInt32(txtGroupManager_GroupInviter_DelayMin.Text);
                        GroupManager.maxDelayGroupInviter = Convert.ToInt32(txtGroupManager_GroupInviter_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread groupInviterThread = new Thread(objGroupManager.StartGroupInviter);
                    groupInviterThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPhotos_TagPhoto_LoadPhotoTagURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPhotos_TagPhoto_PhotoTagURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPhotoManager.LstPhotoTaggingURLsPhotoTagging = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblGroups_GroupInviter_GroupURLsCount.Text = objPhotoManager.LstPhotoTaggingURLsPhotoTagging.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Photos_TagPhoto";
                        objSetting.FileType = "Photos_TagPhoto_LoadPhotoTagURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Photo Tag URLs Loaded : " + objPhotoManager.LstPhotoTaggingURLsPhotoTagging.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Photo Tag URLs Loaded : " + objPhotoManager.LstPhotoTaggingURLsPhotoTagging.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPhotos_TagPhoto_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objPhotoManager.isStopPhotoTagging = false;
                    objPhotoManager.lstThreadsPhotoTagging.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtPhotos_TagPhoto_Threads.Text) && checkNo.IsMatch(txtPhotos_TagPhoto_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtPhotos_TagPhoto_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objPhotoManager.NoOfThreadsPhotoTagging = threads;

                    int tagNoOfFriends = 5;

                    if (!string.IsNullOrEmpty(txtPhotos_TagPhoto_TagNoOfFriends.Text) && checkNo.IsMatch(txtPhotos_TagPhoto_TagNoOfFriends.Text))
                    {
                        tagNoOfFriends = Convert.ToInt32(txtPhotos_TagPhoto_TagNoOfFriends.Text);
                    }


                    objPhotoManager.NoOfTaggingFriendsPhotoTagging = tagNoOfFriends;

                    if (objPhotoManager.LstPhotoTaggingURLsPhotoTagging.Count < 1)
                    {
                        MessageBox.Show("Please Load Photo Tag URLs !");
                        return;
                    }

                    try
                    {
                        PhotoManager.PhotoTagingProcessUsing = CmbPhoto_TagPhoto_StartProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    try
                    {
                        PhotoManager.minDelayPhotoTagging = Convert.ToInt32(txtPhoto_TagPhoto_DelayMin.Text);
                        PhotoManager.maxDelayPhotoTagging = Convert.ToInt32(txtPhoto_TagPhoto_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    Thread photoTaggingThread = new Thread(objPhotoManager.StartPhotoTagging);
                    photoTaggingThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        #region CodeCommented
        //private void btnPages_WallPoster_LoadTextMessages_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblPages_WallPoster_TextMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();


        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


        //                objPageManager.lstWallMessageWallPoster = lstTemp.Distinct().ToList();


        //                DateTime eTime = DateTime.Now;

        //                lblPages_WallPoster_TextMessagesCount.Text = objPageManager.lstWallMessageWallPoster.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_WallPoster";
        //                objSetting.FileType = "Pages_WallPoster_LoadTextMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("Text Messages Loaded : " + objPageManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("Text Messages Loaded : " + objPageManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnPages_WallPoster_LoadURLsMessages_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblPages_WallPoster_URLsMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();


        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


        //                objPageManager.lstWallPostURLsWallPoster= lstTemp.Distinct().ToList();


        //                DateTime eTime = DateTime.Now;

        //                lblPages_WallPoster_URLsMessagesCount.Text = objPageManager.lstWallPostURLsWallPoster.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_WallPoster";
        //                objSetting.FileType = "Pages_WallPoster_LoadURLsMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("URLs Messages Loaded : " + objPageManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("URLs Messages Loaded : " + objPageManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnPages_WallPoster_LoadSpinnedMessages_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblPages_WallPoster_SpinnedMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();


        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


        //                objPageManager.lstSpinnerWallMessageWallPoster = lstTemp.Distinct().ToList();


        //                DateTime eTime = DateTime.Now;

        //                lblPages_WallPoster_SpinnedMessagesCount.Text = objPageManager.lstSpinnerWallMessageWallPoster.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_WallPoster";
        //                objSetting.FileType = "Pages_WallPoster_LoadSpinnedMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("Spinned Messages Loaded : " + objPageManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("Spinned Messages Loaded : " + objPageManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnPages_WallPoster_Start_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (FBGlobals.listAccounts.Count > 0)
        //        {
        //            objPageManager.isStopWallPoster = false;
        //            objPageManager.lstThreadsWallPoster.Clear();

        //            Regex checkNo = new Regex("^[0-9]*$");

        //            int processorCount = objUtils.GetProcessor();

        //            int threads = 25;

        //            int maxThread = 25 * processorCount;

        //            if (!string.IsNullOrEmpty(txtPages_WallPoster_Threads.Text) && checkNo.IsMatch(txtPages_WallPoster_Threads.Text))
        //            {
        //                threads = Convert.ToInt32(txtPages_WallPoster_Threads.Text);
        //            }

        //            if (threads > maxThread)
        //            {
        //                threads = 25;
        //            }
        //            objPageManager.NoOfThreadsWallPoster = threads;

        //            int noOfFriends = 5;

        //            if (!string.IsNullOrEmpty(txtPages_WallPoster_NoOfFriends.Text) && checkNo.IsMatch(txtPages_WallPoster_NoOfFriends.Text))
        //            {
        //                noOfFriends = Convert.ToInt32(txtPages_WallPoster_NoOfFriends.Text);
        //                objPageManager.NoOfFriendsWallPoster = noOfFriends;
        //            }
        //            else
        //            {
        //                txtPages_WallPoster_NoOfFriends.Text = (5).ToString();
        //                objPageManager.NoOfFriendsWallPoster = noOfFriends;
        //            }

        //            if (rdbPages_WallPoster_UseTextMessages.Checked)
        //            {
        //                if (objPageManager.lstWallMessageWallPoster.Count < 1)
        //                {
        //                    GlobusLogHelper.log.Info("Please Load Text Messages !");
        //                    GlobusLogHelper.log.Debug("Please Load Text Messages !");

        //                    MessageBox.Show("Please Load Text Messages !");
        //                    return;
        //                }
        //                objPageManager.IsUseTextMessageWallPoster = true;
        //            }
        //            else if (rdbPages_WallPoster_UseURLsMessages.Checked)
        //            {
        //                if (objPageManager.lstWallPostURLsWallPoster.Count < 1)
        //                {
        //                    GlobusLogHelper.log.Info("Please Load URLs Messages !");
        //                    GlobusLogHelper.log.Debug("Please Load URLs Messages !");

        //                    MessageBox.Show("Please Load URLs Messages !");
        //                    return;
        //                }

        //                objPageManager.IsUseURLsMessageWallPoster = true;
        //                objPageManager.UseAllUrlWallPoster = true;
        //            }
        //            else if (rdbPages_WallPoster_UseSpinnedMessages.Checked)
        //            {
        //                if (objPageManager.lstSpinnerWallMessageWallPoster.Count < 1)
        //                {
        //                    GlobusLogHelper.log.Info("Please Load Spinned Messages !");
        //                    GlobusLogHelper.log.Debug("Please Load Spinned Messages !");

        //                    MessageBox.Show("Please Load Spinned Messages !");
        //                    return;
        //                }

        //                objPageManager.ChkSpinnerWallMessaeWallPoster = true;
        //            }
        //            else
        //            {
        //            }

        //            string messagePostingMode = string.Empty;

        //            try
        //            {
        //                messagePostingMode = cmbPages_WallPoster_MessagePostingMode.SelectedItem.ToString();
        //            }
        //            catch (Exception ex)
        //            {
        //                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //            }

        //            if (messagePostingMode == "Same Message Posting")
        //            {
        //                objPageManager.UseOneMsgToAllFriendsWallPoster = true;
        //            }
        //            else if (messagePostingMode == "Unique Message Posting")
        //            {
        //                objPageManager.UseUniqueMsgToAllFriendsWallPoster = true;
        //            }
        //            else if (messagePostingMode == "Random Message Posting")
        //            {
        //                objPageManager.UseRandomWallPoster = true;
        //            }
        //            else
        //            {
        //                objPageManager.UseRandomWallPoster = true;
        //                cmbPages_WallPoster_MessagePostingMode.SelectedIndex=2;
        //                cmbPages_WallPoster_MessagePostingMode.SelectedItem = cmbPages_WallPoster_MessagePostingMode.SelectedIndex;
        //            }

        //            Thread wallPosterThread = new Thread(objPageManager.StartWallPoster);
        //            wallPosterThread.Start();
        //        }
        //        else
        //        {
        //            GlobusLogHelper.log.Info("Please Load Accounts !");
        //            GlobusLogHelper.log.Debug("Please Load Accounts !");

        //            tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
        //            tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //}

        //private void btnPages_PostPicOnWall_LoadGreetingMessages_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblPages_PostPicOnWall_LoadGreetingMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();


        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


        //                objPageManager.lstMessageCollectionPostPicOnWall = lstTemp.Distinct().ToList();


        //                DateTime eTime = DateTime.Now;

        //                lblPages_PostPicOnWall_LoadGreetingMessagesCount.Text = objPageManager.lstMessageCollectionPostPicOnWall.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_PostPicOnWall";
        //                objSetting.FileType = "Pages_PostPicOnWall_LoadGreetingMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("Greeting Messages Loaded : " + objPageManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("Greeting Messages Loaded : " + objPageManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnPages_PostPicOnWall_LoadPicsFolder_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        List<string> lstWallPics = new List<string>();
        //        List<string> lstCorrectWallPics = new List<string>();

        //        using (FolderBrowserDialog ofd = new FolderBrowserDialog())
        //        {
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                lblPages_PostPicOnWall_LoadPicsFolderImagePath.Text = ofd.SelectedPath;
        //                lstWallPics.Clear();
        //                lstCorrectWallPics.Clear();
        //                string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
        //                lstWallPics = picsArray.Distinct().ToList();
        //                string PicFilepath = ofd.SelectedPath;
        //                foreach (string item in lstWallPics)
        //                {
        //                    try
        //                    {
        //                        string items = item.ToLower();
        //                        if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
        //                        {
        //                            lstCorrectWallPics.Add(item);
        //                        }
        //                        else
        //                        {
        //                            GlobusLogHelper.log.Info("Wrong File Is :" + item);
        //                            GlobusLogHelper.log.Debug("Wrong File Is :" + item);

        //                        }

        //                    }
        //                    catch (Exception ex) { Console.WriteLine(ex.StackTrace); }

        //                }

        //                lstCorrectWallPics = lstCorrectWallPics.Distinct().ToList();
        //                objPageManager.lstPicturecollectionPostPicOnWall = lstCorrectWallPics;

        //                GlobusLogHelper.log.Info(lstCorrectWallPics.Count + "  Pics loaded");
        //                GlobusLogHelper.log.Info(lstCorrectWallPics.Count + "  Pics loaded");

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void btnPages_PostPicOnWall_Start_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (FBGlobals.listAccounts.Count > 0)
        //        {
        //            objPageManager.isStopPostPicOnWall = false;
        //            objPageManager.lstThreadsPostPicOnWall.Clear();

        //            Regex checkNo = new Regex("^[0-9]*$");

        //            int processorCount = objUtils.GetProcessor();

        //            int threads = 25;

        //            int maxThread = 25 * processorCount;

        //            if (!string.IsNullOrEmpty(txtPages_PostPicOnWall_Threads.Text) && checkNo.IsMatch(txtPages_PostPicOnWall_Threads.Text))
        //            {
        //                threads = Convert.ToInt32(txtPages_PostPicOnWall_Threads.Text);
        //            }

        //            if (threads > maxThread)
        //            {
        //                threads = 25;
        //            }
        //            objPageManager.NoOfThreadsPostPicOnWall = threads;

        //            if (objPageManager.lstPicturecollectionPostPicOnWall.Count < 1)
        //            {
        //                GlobusLogHelper.log.Info("Please Load Pics Folder !");
        //                GlobusLogHelper.log.Debug("Please Load Pics Folder !");
        //                return;
        //            }
        //            if (chkPages_PostPicOnWall_UseAllPics.Checked)
        //            {
        //                objPageManager.IsPostAllPicPostPicOnWall = true;
        //            }
        //            else
        //            {
        //                objPageManager.IsPostAllPicPostPicOnWall = false;
        //            }


        //            Thread postPicOnWallThread = new Thread(objPageManager.StarPostPicOnWall);
        //            postPicOnWallThread.Start();
        //        }
        //        else
        //        {
        //            GlobusLogHelper.log.Info("Please Load Accounts !");
        //            GlobusLogHelper.log.Debug("Please Load Accounts !");

        //            tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
        //            tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //} 
        #endregion

        private void grvCampaigns_CampaignProcess_CampaignProcessDetails_Paint(object sender, PaintEventArgs e)
        {

            //AddButtonInGridView(ref grvCampaigns_CampaignProcess_CampaignProcessDetails, "ON", 2, Color.Green, Color.Green);

        }

        private void grvAccounts_AccountCreator_AccountDetails_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void grvAccounts_AccountCreator_AccountDetails_BackgroundColorChanged(object sender, EventArgs e)
        {

        }

        private void btnCampaigns_cmpFanPageLiker_LoadAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedAccountsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblCampaigns_cmpFanPageLiker_LoadAccountsCount.Text = lstTemp.Count.ToString();

                        #region COmmentedCOde
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Accounts Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Accounts Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnCampaigns_cmpFanPageLiker_LoadFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblCampaigns_cmpFanPageLiker_LoadFanPageURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedFanPageURLsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblCampaigns_cmpFanPageLiker_LoadFanPageURLsCount.Text = lstTemp.Count.ToString();

                        #region ComentedCode
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;
                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Campaign Fan Page URLs Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Campaign Fan Page URLs Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnCampaigns_cmpFanPageLiker_LoadFanPageMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedFanPageMessagesCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesCount.Text = lstTemp.Count.ToString();

                        #region COmmentedCode
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting);

                        #endregion

                        GlobusLogHelper.log.Debug("Campaign Fan Page Messages Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Campaign Fan Page Messages Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnCampaigns_cmpFanPageLiker_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstLoadedAccountsCmpFanPageLiker.Count < 1 && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Campaign Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Campaign Accounts !");

                    MessageBox.Show("Please Load Campaign Accounts !");
                    return;
                }
                if (lstLoadedFanPageURLsCmpFanPageLiker.Count < 1 && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Campaign Fan Page URLs !");
                    GlobusLogHelper.log.Debug("Please Load Campaign Fan Page URLs !");

                    MessageBox.Show("Please Load Campaign Accounts !");
                    return;
                }

                if (lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsCount.Text == "0" && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsCount.Text) && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Campaign Fan Page Comments !");
                    GlobusLogHelper.log.Debug("Please Load Campaign Fan Page Comments !");

                    MessageBox.Show("Please Load Campaign Fan Page Comments !");
                    return;
                }

                if (lstLoadedFanPageMessagesCmpFanPageLiker.Count < 1 && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Campaign Fan Page Messages !");
                    GlobusLogHelper.log.Debug("Please Load Campaign Fan Page Messages !");

                    MessageBox.Show("Please Load Campaign Fan Page Messages !");
                    return;
                }
                if (string.IsNullOrEmpty(txtCampaigns_cmpFanPageLiker_CampaignName.Text))
                {
                    GlobusLogHelper.log.Info("Please Enter Unique Campaign Name !");
                    GlobusLogHelper.log.Debug("Please Enter Unique Campaign Name !");

                    MessageBox.Show("Please Enter Unique Campaign Name !");
                    return;
                }
                if (string.IsNullOrEmpty(cmbCampaigns_cmpFanPageLiker_CampaignProcess.SelectedItem.ToString()))
                {
                    GlobusLogHelper.log.Info("Please Select Campaign Process !");
                    GlobusLogHelper.log.Debug("Please Select Campaign Process !");

                    MessageBox.Show("Please Select Campaign Process !");
                    return;
                }

                CmpFanPageLiker objCmpFanPageLiker = new CmpFanPageLiker();
                CmpFanPageLikerRepository objCmpFanPageLikerRepository = new CmpFanPageLikerRepository();

                objCmpFanPageLiker.AccountsFile = lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text;
                objCmpFanPageLiker.FanPageURLsFile = lblCampaigns_cmpFanPageLiker_LoadFanPageURLsPath.Text;
                objCmpFanPageLiker.FanPageMessageFile = lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath.Text;
                objCmpFanPageLiker.FanPageCommentFile = lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsPath.Text;

                objCmpFanPageLiker.CampaignName = txtCampaigns_cmpFanPageLiker_CampaignName.Text;
                objCmpFanPageLiker.CampaignProcess = cmbCampaigns_cmpFanPageLiker_CampaignProcess.SelectedItem.ToString();

                bool CheckDatainsert = objCmpFanPageLikerRepository.InsertCompaign(objCmpFanPageLiker);
               if (CheckDatainsert)
               {
                   GlobusLogHelper.log.Info("Campaign Saved !");
                   GlobusLogHelper.log.Debug("Campaign Saved !");
               }
               else
               {
                   GlobusLogHelper.log.Info("Check Campaign Name !");
                   GlobusLogHelper.log.Debug("Check Campaign Name !");
               }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


        }

        public List<string> GetAllCampaignAccount()
        {
            List<string> lstAllCmpAccount = new List<string>();
            try
            {
                CmpFanPageLikerRepository objCmpFanPageLikerRepository = new CmpFanPageLikerRepository();
                ICollection<CmpFanPageLiker> cmpData = objCmpFanPageLikerRepository.GetAllCampaign();

                foreach (CmpFanPageLiker item in cmpData)
                {
                    try
                    {
                        string AccountsFile = item.AccountsFile;
                        if (File.Exists(AccountsFile))
                        {
                            List<string> lstAccounts = GlobusFileHelper.ReadFile(AccountsFile);

                            foreach (string account in lstAccounts)
                            {
                                try
                                {
                                    lstAllCmpAccount.Add("CampaignName " + item.CampaignName + " <:> Account " + account + " <:> CampaignProcess " + item.CampaignProcess);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Accounts File Not Exists : " + AccountsFile + " With Campaign Name : " + item.CampaignName);
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

            return lstAllCmpAccount;
        }

        public List<string> GetAllSchedulerAccount()
        {
            List<string> lstAllSchAccount = new List<string>();
            try
            {
                SchFanPageLikerRepository objSchFanPageLikerRepository = new SchFanPageLikerRepository();
                ICollection<SchFanPageLiker> schDataCollection = objSchFanPageLikerRepository.GetAllScheduler();

                foreach (SchFanPageLiker item in schDataCollection)
                {
                    try
                    {
                        string AccountsFile = item.AccountsFile;
                        if (File.Exists(AccountsFile))
                        {
                            List<string> lstAccounts = GlobusFileHelper.ReadFile(AccountsFile);

                            foreach (string account in lstAccounts)
                            {
                                try
                                {
                                    lstAllSchAccount.Add("CampaignName " + item.SchedulerName + " <:> Account " + account + " <:> CampaignProcess " + item.SchedulerProcess + " <:> StartDate " + item.StartDate + " <:> EndDate " + item.EndDate + " <:> StartTime " + item.StartTime + " <:> EndTime " + item.EndTime);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Accounts File Not Exists : " + AccountsFile + " With Campaign Name : " + item.SchedulerName);
                            GlobusLogHelper.log.Debug("Accounts File Not Exists : " + AccountsFile + " With Campaign Name : " + item.SchedulerName);

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

            return lstAllSchAccount;
        }

        public void BindAccountInCmpGrid(List<string> accounts)
        {
            try
            {
                if (grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows.Count >= 1)
                {
                    return;
                }

                if (accounts.Count < 1)
                {
                    GlobusLogHelper.log.Info("Please Save Campaign !");
                    GlobusLogHelper.log.Debug("Please Save Campaign !");

                    return;
                }

                DataTable dt = new DataTable();

                DataColumn CampaignName = new DataColumn();
                CampaignName.ColumnName = "Campaign Name";
                CampaignName.ReadOnly = true;
                dt.Columns.Add(CampaignName);

                DataColumn Account = new DataColumn();
                Account.ColumnName = "Account";
                Account.ReadOnly = true;
                dt.Columns.Add(Account);



                foreach (string item in accounts)
                {
                    try
                    {
                        string cmpName = string.Empty;
                        string cmpAccount = string.Empty;
                        string cmpProcess = string.Empty;

                        if (item.Contains("<:>"))
                        {
                            string[] itemArr = Regex.Split(item, "<:>");

                            foreach (string item1 in itemArr)
                            {
                                try
                                {
                                    if (item1.Contains("CampaignName"))
                                    {
                                        cmpName = item1.Replace("CampaignName", string.Empty).Trim();
                                    }
                                    if (item1.Contains("Account"))
                                    {
                                        cmpAccount = item1.Replace("Account", string.Empty).Trim();
                                    }
                                    if (item1.Contains("CampaignProcess"))
                                    {
                                        cmpProcess = item1.Replace("CampaignProcess", string.Empty).Trim();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            dt.Rows.Add(cmpName, cmpAccount);
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }


                grvCampaigns_CampaignProcess_CampaignProcessDetails.DataSource = dt;

                //Campaign Process
                List<string> lstCmpProcess = new List<string>();
                lstCmpProcess.Add("Like Page");
                lstCmpProcess.Add("Share Page");
                lstCmpProcess.Add("Like Post");
                lstCmpProcess.Add("Comment On Post");

                AddComboBoxInGridView(ref grvCampaigns_CampaignProcess_CampaignProcessDetails, "Select Campaign Process", 2, Color.Red, Color.Red, lstCmpProcess);

                AddButtonInGridView(ref grvCampaigns_CampaignProcess_CampaignProcessDetails, "ON", 3, Color.Green, Color.Green);
                AddButtonInGridView(ref grvCampaigns_CampaignProcess_CampaignProcessDetails, "OFF", 4, Color.Red, Color.Red);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void BindAccountInSchedulerGrid(List<string> accounts)
        {
            try
            {
                if (grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows.Count >= 1)
                {
                    return;
                }

                grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows.Clear();

                if (accounts.Count < 1)
                {
                    GlobusLogHelper.log.Info("Please Save Campaign !");
                    GlobusLogHelper.log.Debug("Please Save Campaign !");

                    return;
                }

                DataTable dt = new DataTable();

                DataColumn CampaignName = new DataColumn();
                CampaignName.ColumnName = "Scheduler Name";
                CampaignName.ReadOnly = true;
                dt.Columns.Add(CampaignName);

                DataColumn Account = new DataColumn();
                Account.ColumnName = "Account";
                Account.ReadOnly = true;
                dt.Columns.Add(Account);





                foreach (string item in accounts)
                {
                    try
                    {
                        string schName = string.Empty;
                        string schAccount = string.Empty;
                        string schProcess = string.Empty;
                        string schStartDate = string.Empty;
                        string schEndDate = string.Empty;
                        string schStartTime = string.Empty;
                        string schEndTime = string.Empty;

                        if (item.Contains("<:>"))
                        {
                            string[] itemArr = Regex.Split(item, "<:>");

                            foreach (string item1 in itemArr)
                            {
                                try
                                {
                                    if (item1.Contains("CampaignName"))
                                    {
                                        schName = item1.Replace("CampaignName", string.Empty).Trim();
                                    }
                                    if (item1.Contains("Account"))
                                    {
                                        schAccount = item1.Replace("Account", string.Empty).Trim();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            dt.Rows.Add(schName, schAccount);
                        }


                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }


                grvScheduler_SchedulerProcess_SchedulerProcessDetails.DataSource = dt;

                //Campaign Process
                List<string> lstCmpProcess = new List<string>();
                lstCmpProcess.Add("Like Page");
                lstCmpProcess.Add("Share Page");
                lstCmpProcess.Add("Like Post");
                lstCmpProcess.Add("Comment On Post");

                AddComboBoxInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "Select Scheduler Process", 2, Color.Red, Color.Red, lstCmpProcess);

                AddCalendarColumnInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "Start Date", 3, Color.Red, Color.Red);
                AddCalendarColumnInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "End Date", 4, Color.Red, Color.Red);

                AddTextBoxInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "Start Time", 5, Color.Green, Color.Green);
                AddTextBoxInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "End Time", 6, Color.Green, Color.Green);

                AddTextBoxInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "Min Delay", 7, Color.Green, Color.Green);
                AddTextBoxInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "Max Delay", 8, Color.Green, Color.Green);


                AddButtonInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "ON", 9, Color.Green, Color.Green);
                AddButtonInGridView(ref grvScheduler_SchedulerProcess_SchedulerProcessDetails, "OFF", 10, Color.Red, Color.Red);


                int counter = 0;
                foreach (DataGridViewRow item in grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows)
                {
                    try
                    {
                        string schStartDate = string.Empty;
                        string schEndDate = string.Empty;
                        string schStartTime = string.Empty;
                        string schEndTime = string.Empty;

                        string accountDeatils = accounts[counter];
                        string[] accountDeatailsArr = Regex.Split(accountDeatils, "<:>");

                        foreach (string item1 in accountDeatailsArr)
                        {
                            try
                            {
                                if (item1.Contains("StartDate"))
                                {
                                    schStartDate = item1.Replace("StartDate", string.Empty).Trim();

                                    item.Cells[3].Value = schStartDate;
                                }
                                if (item1.Contains("EndDate"))
                                {
                                    schEndDate = item1.Replace("EndDate", string.Empty).Trim();

                                    item.Cells[4].Value = schEndDate;
                                }
                                if (item1.Contains("StartTime"))
                                {
                                    schStartTime = item1.Replace("StartTime", string.Empty).Trim();

                                    item.Cells[5].Value = schStartTime;
                                }
                                if (item1.Contains("EndTime"))
                                {
                                    schEndTime = item1.Replace("EndTime", string.Empty).Trim();

                                    item.Cells[6].Value = schEndTime;
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        counter++;

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

        private void grvAccounts_AccountCreator_AccountDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        Dictionary<int, Thread> dicCmpRowIndexThread = new Dictionary<int, Thread>();

        private void grvCampaigns_CampaignProcess_CampaignProcessDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "ON")
                {
                    string camName = string.Empty;
                    string camProcess = string.Empty;
                    string camaccount = string.Empty;

                    camName = grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows[e.RowIndex].Cells["Campaign Name"].Value.ToString();
                    camaccount = grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows[e.RowIndex].Cells["Account"].Value.ToString();
                    camProcess = grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows[e.RowIndex].Cells["Select Campaign Process"].Value.ToString();

                    if (camProcess == "Like Page" || camProcess == "Share Page" || camProcess == "Like Post" || camProcess == "Comment On Post")
                    {
                        List<string> lstFanPageURLs = new List<string>();
                        List<string> lstFanPageMessages = new List<string>();
                        List<string> lstFanPageComments = new List<string>();

                        CmpFanPageLiker objCmpFanPageLiker = new CmpFanPageLiker();
                        CmpFanPageLikerRepository objCmpFanPageLikerRepository = new CmpFanPageLikerRepository();

                        objCmpFanPageLiker.CampaignName = camName;

                        List<CmpFanPageLiker> lstCmpFanPageLiker = objCmpFanPageLikerRepository.GetCmpFanPageLikerDataUsingCmpName(objCmpFanPageLiker);

                        foreach (CmpFanPageLiker item in lstCmpFanPageLiker)
                        {
                            try
                            {
                                if (File.Exists(item.FanPageURLsFile))
                                {
                                    lstFanPageURLs = GlobusFileHelper.ReadFile(item.FanPageURLsFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageURLsFile + " With Campaign Name : " + camName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageURLsFile + " With Campaign Name : " + camName);
                                }

                                if (File.Exists(item.FanPageMessageFile))
                                {
                                    lstFanPageMessages = GlobusFileHelper.ReadFile(item.FanPageMessageFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageMessageFile + " With Campaign Name : " + camName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageMessageFile + " With Campaign Name : " + camName);
                                }
                                if (File.Exists(item.FanPageCommentFile))
                                {
                                    lstFanPageComments = GlobusFileHelper.ReadFile(item.FanPageCommentFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageCommentFile + " With Campaign Name : " + camName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageCommentFile + " With Campaign Name : " + camName);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        if (string.IsNullOrEmpty(camName) && string.IsNullOrEmpty(camProcess) && string.IsNullOrEmpty(camaccount) && lstFanPageURLs.Count < 1 && lstFanPageMessages.Count < 1 && lstFanPageComments.Count < 1)
                        {
                            GlobusLogHelper.log.Info("There is No Record in Campaign Name Or Campaign Process Or Campaign Account Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");
                            GlobusLogHelper.log.Debug("There is No Record in Campaign Name Or Campaign Process Or Campaign Account Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");

                            MessageBox.Show("There is No Record in Campaign Name Or Campaign Process Or Campaign Account Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");
                            return;

                        }
                        CampaignManager objCampaignManager = new CampaignManager();

                        Thread startCmpFanpageLikerThread = new Thread(objCampaignManager.StartCmpFanpageLiker);

                        //Add Current Thread In  dicCmpRowIndexThread For Stop Particular Thraed
                        Thread.CurrentThread.IsBackground = true;

                        dicCmpRowIndexThread.Add(e.RowIndex, startCmpFanpageLikerThread);

                        startCmpFanpageLikerThread.Start(new object[] { camName, camProcess, camaccount, lstFanPageURLs, lstFanPageMessages, lstFanPageComments });



                    }

                }
                if (grvCampaigns_CampaignProcess_CampaignProcessDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "OFF")
                {
                    Thread stopThread = new Thread(CmpStopRowIndexThread);
                    stopThread.Start(e.RowIndex);
                    //CmpStopRowIndexThread(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void CmpStopRowIndexThread(object ocmpGrvRowIndex)
        {
            int cmpGrvRowIndex = (int)ocmpGrvRowIndex;
            try
            {
                Dictionary<int, Thread> dicTemp = dicCmpRowIndexThread;

                foreach (KeyValuePair<int, Thread> item in dicTemp)
                {
                    try
                    {
                        if (item.Key == cmpGrvRowIndex)
                        {
                            if (item.Value.IsAlive)
                            {
                                item.Value.Abort();
                            }
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped Of The Row Index : " + cmpGrvRowIndex);
            GlobusLogHelper.log.Debug("Process Stopped Of The Row Index : " + cmpGrvRowIndex);
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void SchStopRowIndexThread(object ocmpGrvRowIndex)
        {
            int cmpGrvRowIndex = (int)ocmpGrvRowIndex;
            try
            {
                Dictionary<int, Thread> dicTemp = dicSchRowIndexThread;

                foreach (KeyValuePair<int, Thread> item in dicTemp)
                {
                    try
                    {
                        if (item.Key == cmpGrvRowIndex)
                        {
                            if (item.Value.IsAlive)
                            {
                                item.Value.Abort();
                            }
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped Of The Row Index : " + cmpGrvRowIndex);
            GlobusLogHelper.log.Debug("Process Stopped Of The Row Index : " + cmpGrvRowIndex);
        }

        private void btnCampaigns_cmpFanPageLiker_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                BindAllCmpInCumboBox();

                cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Visible = true;
                btnCampaigns_cmpFanPageLiker_Update.Visible = true;
                lblCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Visible = true;

                txtCampaigns_cmpFanPageLiker_CampaignName.Visible = false;
                lblCampaigns_cmpFanPageLiker_CampaignName.Visible = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void BindAllCmpInCumboBox()
        {
            try
            {
                cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Items.Clear();

                CmpFanPageLikerRepository objCmpFanPageLikerRepository = new CmpFanPageLikerRepository();
                ICollection<CmpFanPageLiker> cmpDataCollection = objCmpFanPageLikerRepository.GetAllCampaign();

                foreach (CmpFanPageLiker item in cmpDataCollection)
                {
                    try
                    {
                        cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Items.Add(item.CampaignName);
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

        public void BindAllSchedulerInCumboBox()
        {
            try
            {
                cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate.Items.Clear();

                SchFanPageLikerRepository objSchFanPageLikerRepository = new SchFanPageLikerRepository();
                ICollection<SchFanPageLiker> schDataCollection = objSchFanPageLikerRepository.GetAllScheduler();

                foreach (SchFanPageLiker item in schDataCollection)
                {
                    try
                    {
                        cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate.Items.Add(item.SchedulerName);
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

        private void btnCampaigns_cmpFanPageLiker_Update_Click(object sender, EventArgs e)
        {
            try
            {
                CmpFanPageLikerRepository objCmpFanPageLikerRepository = new CmpFanPageLikerRepository();
                CmpFanPageLiker objCmpFanPageLiker = new CmpFanPageLiker();

                if (string.IsNullOrEmpty(cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.SelectedItem.ToString()))
                {
                    GlobusLogHelper.log.Info("Please Select Campaign For Update !");
                    GlobusLogHelper.log.Debug("Please Select Campaign For Update !");

                    MessageBox.Show("Please Select Campaign For Update !");
                    return;
                }

                objCmpFanPageLiker.CampaignName = cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.SelectedItem.ToString();

                try
                {
                    objCmpFanPageLiker.CampaignProcess = cmbCampaigns_cmpFanPageLiker_CampaignProcess.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                if (string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text) && lblCampaigns_cmpFanPageLiker_LoadAccountsCount.Text == "0" && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadFanPageURLsPath.Text) && lblCampaigns_cmpFanPageLiker_LoadFanPageURLsCount.Text == "0" && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath.Text) && lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath.Text == "0" && string.IsNullOrEmpty(lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsPath.Text) && lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsCount.Text == "0")
                {
                    GlobusLogHelper.log.Info("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");
                    GlobusLogHelper.log.Debug("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");

                    MessageBox.Show("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");
                    return;
                }

                objCmpFanPageLiker.AccountsFile = lblCampaigns_cmpFanPageLiker_LoadAccountsPath.Text;
                objCmpFanPageLiker.FanPageURLsFile = lblCampaigns_cmpFanPageLiker_LoadFanPageURLsPath.Text;
                objCmpFanPageLiker.FanPageMessageFile = lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath.Text;
                objCmpFanPageLiker.FanPageCommentFile = lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsPath.Text;


                int respose = objCmpFanPageLikerRepository.UpdateUsingCmpName(objCmpFanPageLiker);

                GlobusLogHelper.log.Info("Affected Rows : " + respose);
                GlobusLogHelper.log.Debug("Affected Rows : " + respose);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabCampaigns_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabCampaigns.SelectedTab == tabCampaigns.TabPages[2])
                {

                    cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Visible = false;
                    btnCampaigns_cmpFanPageLiker_Update.Visible = false;
                    lblCampaigns_cmpFanPageLiker_SelectCampaignForUpdate.Visible = false;

                    txtCampaigns_cmpFanPageLiker_CampaignName.Visible = true;
                    lblCampaigns_cmpFanPageLiker_CampaignName.Visible = true;


                    List<string> lstCmpAccounts = GetAllCampaignAccount();


                    BindAccountInCmpGrid(lstCmpAccounts);

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnCampaigns_cmpFanPageLiker_LoadFanPageComments_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedFanPageMessagesCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsCount.Text = lstTemp.Count.ToString();

                        #region CodeCommented
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Campaign Fan Page Comments Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Campaign Fan Page Comments Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        Dictionary<int, Thread> dicSchRowIndexThread = new Dictionary<int, Thread>();

        private void grvScheduler_SchedulerProcess_SchedulerProcessDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "ON")
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


                    schName = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Scheduler Name"].Value.ToString();
                    schAccount = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Account"].Value.ToString();
                    schProcess = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Select Scheduler Process"].Value.ToString();
                    schStartDate = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Start Date"].Value.ToString();
                    schEndDate = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["End Date"].Value.ToString();
                    schStartTime = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Start Time"].Value.ToString();
                    schEndTime = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["End Time"].Value.ToString();
                    schMinDelay = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Min Delay"].Value.ToString();
                    schMaxDelay = grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells["Max Delay"].Value.ToString();


                    if (schProcess == "Like Page" || schProcess == "Share Page" || schProcess == "Like Post" || schProcess == "Comment On Post")
                    {
                        List<string> lstFanPageURLs = new List<string>();
                        List<string> lstFanPageMessages = new List<string>();
                        List<string> lstFanPageComments = new List<string>();

                        SchFanPageLiker objSchFanPageLiker = new SchFanPageLiker();
                        SchFanPageLikerRepository objSchFanPageLikerRepository = new SchFanPageLikerRepository();

                        objSchFanPageLiker.SchedulerName = schName;

                        List<SchFanPageLiker> lstSchFanPageLiker = objSchFanPageLikerRepository.GetSchFanPageLikerDataUsingSchName(objSchFanPageLiker);

                        foreach (SchFanPageLiker item in lstSchFanPageLiker)
                        {
                            try
                            {
                                if (File.Exists(item.FanPageURLsFile))
                                {
                                    lstFanPageURLs = GlobusFileHelper.ReadFile(item.FanPageURLsFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageURLsFile + " With Scheduler Name : " + schName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageURLsFile + " With Scheduler Name : " + schName);
                                }

                                if (File.Exists(item.FanPageMessageFile))
                                {
                                    lstFanPageMessages = GlobusFileHelper.ReadFile(item.FanPageMessageFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageMessageFile + " With Scheduler Name : " + schName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageMessageFile + " With Scheduler Name : " + schName);
                                }
                                if (File.Exists(item.FanPageCommentsFile))
                                {
                                    lstFanPageComments = GlobusFileHelper.ReadFile(item.FanPageCommentsFile);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("File Not Exists : " + item.FanPageCommentsFile + " With Scheduler Name : " + schName);
                                    GlobusLogHelper.log.Debug("File Not Exists : " + item.FanPageCommentsFile + " With Scheduler Name : " + schName);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        if (string.IsNullOrEmpty(schName) && string.IsNullOrEmpty(schProcess) && string.IsNullOrEmpty(schAccount) && string.IsNullOrEmpty(schStartDate) && string.IsNullOrEmpty(schEndDate) && string.IsNullOrEmpty(schStartTime) && string.IsNullOrEmpty(schEndTime) && lstFanPageURLs.Count < 1 && lstFanPageMessages.Count < 1 && lstFanPageComments.Count < 1)
                        {
                            GlobusLogHelper.log.Info("There is No Record in Scheduler Name Or Scheduler Process Or Scheduler Account Or Start Date Or End Date Or Start Time Or End Time Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");
                            GlobusLogHelper.log.Debug("There is No Record in Scheduler Name Or Scheduler Process Or Scheduler Account Or Start Date Or End Date Or Start Time Or End Time Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");

                            MessageBox.Show("There is No Record in Scheduler Name Or Scheduler Process Or Scheduler Account Or Start Date Or End Date Or Start Time Or End Time Or Fan Page URLs Or Fan Page Messages Or Fan Page Comments !");
                            return;

                        }

                        if (string.IsNullOrEmpty(schMinDelay))
                        {
                            schMinDelay = Convert.ToString(5);
                        }

                        if (string.IsNullOrEmpty(schMaxDelay))
                        {
                            schMaxDelay = Convert.ToString(5);
                        }

                        SchedulerManager objSchedulerManager = new SchedulerManager();



                        Thread startSchFanpageLikerThread = new Thread(objSchedulerManager.StartSchFanpageLiker);

                        //Add Current Thread In  dicCmpRowIndexThread For Stop Particular Thraed
                        Thread.CurrentThread.IsBackground = true;

                        dicSchRowIndexThread.Add(e.RowIndex, startSchFanpageLikerThread);

                        startSchFanpageLikerThread.Start(new object[] { schName, schProcess, schAccount, schStartDate, schEndDate, schStartTime, schEndTime, schMinDelay, schMaxDelay, lstFanPageURLs, lstFanPageMessages, lstFanPageComments });

                    }

                }
                if (grvScheduler_SchedulerProcess_SchedulerProcessDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "OFF")
                {
                    Thread stopThread = new Thread(SchStopRowIndexThread);
                    stopThread.Start(e.RowIndex);
                    //CmpStopRowIndexThread(e.RowIndex);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void tabSchduler_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabSchduler.SelectedTab == tabSchduler.TabPages[0])
                {

                    List<string> lstSchAccounts = GetAllSchedulerAccount();

                    BindAccountInSchedulerGrid(lstSchAccounts);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if ((lblScheduler_SchFanPageLiker_AccountsCount.Text == "0" || string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_AccountsCount.Text)) && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_AccountsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Scheduler Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Scheduler Accounts !");

                    MessageBox.Show("Please Load Scheduler Accounts !");
                    return;
                }
                if ((lblScheduler_SchFanPageLiker_FanPageURLsCount.Text == "0" || string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageURLsCount.Text)) && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageURLsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Scheduler Fan Page URLs !");
                    GlobusLogHelper.log.Debug("Please Load Scheduler Fan Page URLs !");

                    MessageBox.Show("Please Load Scheduler Accounts !");
                    return;
                }

                if ((lblScheduler_SchFanPageLiker_FanPageCommentsCount.Text == "0" || string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageCommentsCount.Text)) && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageCommentsPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Scheduler Fan Page Comments !");
                    GlobusLogHelper.log.Debug("Please Load Scheduler Fan Page Comments !");

                    MessageBox.Show("Please Load Scheduler Fan Page Comments !");
                    return;
                }

                if ((lblScheduler_SchFanPageLiker_FanPageMessagesCount.Text == "0" || string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageMessagesCount.Text)) && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageMessagesPath.Text))
                {
                    GlobusLogHelper.log.Info("Please Load Scheduler Fan Page Messages !");
                    GlobusLogHelper.log.Debug("Please Load Scheduler Fan Page Messages !");

                    MessageBox.Show("Please Load Scheduler Fan Page Messages !");
                    return;
                }
                if (string.IsNullOrEmpty(txtScheduler_SchFanPageLiker_SchedulerName.Text))
                {
                    GlobusLogHelper.log.Info("Please Enter Unique Scheduler Name !");
                    GlobusLogHelper.log.Debug("Please Enter Unique Scheduler Name !");

                    MessageBox.Show("Please Enter Unique Scheduler Name !");
                    return;
                }
                if (string.IsNullOrEmpty(cmbScheduler_SchFanPageLiker_SchedulerProcess.SelectedItem.ToString()))
                {
                    GlobusLogHelper.log.Info("Please Select Scheduler Process !");
                    GlobusLogHelper.log.Debug("Please Select Scheduler Process !");

                    MessageBox.Show("Please Select Scheduler Process !");
                    return;
                }

                if (string.IsNullOrEmpty(dtpScheduler_SchFanPageLiker_StartDate.Text))
                {
                    GlobusLogHelper.log.Info("Please Select Scheduler Start Date !");
                    GlobusLogHelper.log.Debug("Please Select Scheduler Start Date !");

                    MessageBox.Show("Please Select Scheduler Start Date !");
                    return;
                }

                if (string.IsNullOrEmpty(dtpScheduler_SchFanPageLiker_EndDate.Text))
                {
                    GlobusLogHelper.log.Info("Please Select Scheduler End Date !");
                    GlobusLogHelper.log.Debug("Please Select Scheduler End Date !");

                    MessageBox.Show("Please Select Scheduler End Date !");
                    return;
                }

                if (string.IsNullOrEmpty(txtScheduler_SchFanPageLiker_StartTime.Text))
                {
                    GlobusLogHelper.log.Info("Please Select Scheduler Start Time !");
                    GlobusLogHelper.log.Debug("Please Select Scheduler Start Time !");

                    MessageBox.Show("Please Select Scheduler Start Time !");
                    return;
                }

                if (string.IsNullOrEmpty(txtScheduler_SchFanPageLiker_EndTime.Text))
                {
                    GlobusLogHelper.log.Info("Please Select Scheduler End Time !");
                    GlobusLogHelper.log.Debug("Please Select Scheduler End Time !");

                    MessageBox.Show("Please Select Scheduler End Time !");
                    return;
                }

                SchFanPageLiker objSchFanPageLiker = new SchFanPageLiker();
                SchFanPageLikerRepository objSchFanPageLikerRepository = new SchFanPageLikerRepository();

                objSchFanPageLiker.AccountsFile = lblScheduler_SchFanPageLiker_AccountsPath.Text;
                objSchFanPageLiker.FanPageURLsFile = lblScheduler_SchFanPageLiker_FanPageURLsPath.Text;
                objSchFanPageLiker.FanPageMessageFile = lblScheduler_SchFanPageLiker_FanPageMessagesPath.Text;
                objSchFanPageLiker.FanPageCommentsFile = lblScheduler_SchFanPageLiker_FanPageCommentsPath.Text;

                objSchFanPageLiker.SchedulerName = txtScheduler_SchFanPageLiker_SchedulerName.Text;
                objSchFanPageLiker.SchedulerProcess = cmbScheduler_SchFanPageLiker_SchedulerProcess.SelectedItem.ToString();
                objSchFanPageLiker.StartDate = Convert.ToDateTime(dtpScheduler_SchFanPageLiker_StartDate.Text);
                objSchFanPageLiker.EndDate = Convert.ToDateTime(dtpScheduler_SchFanPageLiker_EndDate.Text);
                objSchFanPageLiker.StartTime = txtScheduler_SchFanPageLiker_StartTime.Text;
                objSchFanPageLiker.EndTime = txtScheduler_SchFanPageLiker_EndTime.Text;
                objSchFanPageLiker.Status = "0";

                objSchFanPageLikerRepository.Insert(objSchFanPageLiker);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_LoadAccounts_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScheduler_SchFanPageLiker_AccountsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedAccountsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScheduler_SchFanPageLiker_AccountsCount.Text = lstTemp.Count.ToString();

                        #region ComentedCode
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Accounts Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Accounts Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_LoadFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScheduler_SchFanPageLiker_FanPageURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedAccountsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScheduler_SchFanPageLiker_FanPageURLsCount.Text = lstTemp.Count.ToString();

                        #region CommentedCode
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_LoadFanPageMessages_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScheduler_SchFanPageLiker_FanPageMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedAccountsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScheduler_SchFanPageLiker_FanPageMessagesCount.Text = lstTemp.Count.ToString();

                        #region commentedCode

                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Fan Page Messages Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Fan Page Messages Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_LoadFanPageComments_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScheduler_SchFanPageLiker_FanPageCommentsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName);

                        lstTemp = lstTemp.Distinct().ToList();

                        lstLoadedAccountsCmpFanPageLiker = lstTemp;

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScheduler_SchFanPageLiker_FanPageCommentsCount.Text = lstTemp.Count.ToString();

                        #region CommentedCode
                        ////Insert Seeting Into Database
                        //objSetting.Module = "Scrapers_FanPageScraper";
                        //objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageURLs";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion

                        GlobusLogHelper.log.Debug("Fan Page Comments Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Fan Page Comments Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                BindAllSchedulerInCumboBox();

                cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate.Visible = true;
                lblScheduler_SchFanPageLiker_SelectSchedulerForUpdate.Visible = true;
                btnScheduler_SchFanPageLiker_Update.Visible = true;


                txtScheduler_SchFanPageLiker_SchedulerName.Visible = false;
                lblScheduler_SchFanPageLiker_SchedulerName.Visible = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScheduler_SchFanPageLiker_Update_Click(object sender, EventArgs e)
        {
            try
            {
                SchFanPageLikerRepository objSchFanPageLikerRepository = new SchFanPageLikerRepository();
                SchFanPageLiker objSchFanPageLiker = new SchFanPageLiker();

                if (string.IsNullOrEmpty(cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate.SelectedItem.ToString()))
                {
                    GlobusLogHelper.log.Info("Please Select Campaign For Update !");
                    GlobusLogHelper.log.Debug("Please Select Campaign For Update !");

                    MessageBox.Show("Please Select Campaign For Update !");
                    return;
                }

                objSchFanPageLiker.SchedulerName = cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate.SelectedItem.ToString();

                try
                {
                    objSchFanPageLiker.SchedulerProcess = cmbScheduler_SchFanPageLiker_SchedulerProcess.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                if (string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_AccountsPath.Text) && lblScheduler_SchFanPageLiker_AccountsCount.Text == "0" && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageURLsPath.Text) && lblScheduler_SchFanPageLiker_FanPageURLsCount.Text == "0" && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageMessagesPath.Text) && lblScheduler_SchFanPageLiker_FanPageMessagesCount.Text == "0" && string.IsNullOrEmpty(lblScheduler_SchFanPageLiker_FanPageCommentsPath.Text) && lblScheduler_SchFanPageLiker_FanPageCommentsCount.Text == "0")
                {
                    GlobusLogHelper.log.Info("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");
                    GlobusLogHelper.log.Debug("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");

                    MessageBox.Show("Please Load Accounts, Fan Page URLs, Fan Page Comments And Fan Page Messages !");
                    return;
                }

                objSchFanPageLiker.AccountsFile = lblScheduler_SchFanPageLiker_AccountsPath.Text;
                objSchFanPageLiker.FanPageURLsFile = lblScheduler_SchFanPageLiker_FanPageURLsPath.Text;
                objSchFanPageLiker.FanPageMessageFile = lblScheduler_SchFanPageLiker_FanPageMessagesPath.Text;
                objSchFanPageLiker.FanPageCommentsFile = lblScheduler_SchFanPageLiker_FanPageCommentsPath.Text;
                objSchFanPageLiker.StartDate = Convert.ToDateTime(dtpScheduler_SchFanPageLiker_StartDate.Text);
                objSchFanPageLiker.EndDate = Convert.ToDateTime(dtpScheduler_SchFanPageLiker_EndDate.Text);
                objSchFanPageLiker.StartTime = txtScheduler_SchFanPageLiker_StartTime.Text;
                objSchFanPageLiker.EndTime = txtScheduler_SchFanPageLiker_EndTime.Text;

                int respose = objSchFanPageLikerRepository.UpdateUsingSchName(objSchFanPageLiker);

                GlobusLogHelper.log.Info("Affected Rows : " + respose);
                GlobusLogHelper.log.Debug("Affected Rows : " + respose);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

      

        private void btnProxies_ProxyChecker_LoadProxies_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblProxies_ProxyChecker_ProxiesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        lblProxies_ProxyChecker_ProxiesCount.Text = lstTemp.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lstLoadedProxyProxyChecker = lstTemp.Distinct().ToList();

                        #region CommentedCode

                        ////Insert Seeting Into Database
                        //objSetting.Module = "Messages_ReplyMessage";
                        //objSetting.FileType = "Messages_ReplyMessage_LoadReplyMessage";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting);                        
                        #endregion

                        GlobusLogHelper.log.Info("Proxies Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Proxies Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnProxies_ProxyChecker_CheckProxy_Click(object sender, EventArgs e)
        {
            try
            {

                objProxyManager.isStopProxyChecker = false;
                objProxyManager.lstThreadsProxyChecker.Clear();

                Regex checkNo = new Regex("^[0-9]*$");

                int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                int threads = 25;

                int maxThread = 25 * processorCount;

                int ThreadCount=Convert.ToInt32(txtProxies_ProxyChecker_Threads.Text);
                if (ThreadCount==0)
                {
                    GlobusLogHelper.log.Info("Please Enter More than 0 thread ..");
                    GlobusLogHelper.log.Debug("Please Enter More than 0 thread ..");
                    return;
                }

                if (!string.IsNullOrEmpty(txtProxies_ProxyChecker_Threads.Text) && checkNo.IsMatch(txtProxies_ProxyChecker_Threads.Text))
                {
                    threads = Convert.ToInt32(txtProxies_ProxyChecker_Threads.Text);
                }

                foreach (string item in lstLoadedProxyProxyChecker)
                {
                    try
                    {
                        if (objProxyManager.isStopProxyChecker)
                        {
                            break;
                        }


                        ThreadPool.SetMaxThreads(maxThread, 50);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(objProxyManager.StartProxyChecker), new object[] { item });

                        Thread.CurrentThread.IsBackground = true;

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

        private void btnPages_FanPageLiker_StopFanPageLiker_Click(object sender, EventArgs e)
        {
            try
            {

                objPageManager.isStopFanPageLiker = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsFanPageLiker.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsFanPageLiker.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnPages_WallPoster_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPageManager.isStopWallPoster = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsWallPoster.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsWallPoster.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnPages_PostPicOnWall_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPageManager.isStopPostPicOnWall = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsPostPicOnWall.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsPostPicOnWall.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnEvents_EventCreator_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objEventManager.isStopEvenCreator = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objEventManager.lstThreadsEvenCreator.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objEventManager.lstThreadsEvenCreator.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnEvents_EventInviter_StopInviteFriends_Click(object sender, EventArgs e)
        {
            try
            {
                objEventManager.isStopEventInviter = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objEventManager.lstThreadsEventInviter.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objEventManager.lstThreadsEventInviter.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnMessages_MessageReply_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objMessageManager.isStopMessageReply = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objMessageManager.lstThreadsMessageReply.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objMessageManager.lstThreadsMessageReply.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnGroups_GroupInviter_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objGroupManager.isStopGroupInviter = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objGroupManager.lstThreadsGroupInviter.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objGroupManager.lstThreadsGroupInviter.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnPhotos_TagPhoto_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPhotoManager.isStopPhotoTagging = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPhotoManager.lstThreadsPhotoTagging.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPhotoManager.lstThreadsPhotoTagging.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnScrapers_FanPageScraper_StopFanPageURLs_Click(object sender, EventArgs e)
        {
            try
            {
                objPageManager.isStopFanPageScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstFanPageScraperThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstFanPageScraperThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnScrapers_EventsScraper_StopEventsScraper_Click(object sender, EventArgs e)
        {
            try
            {
                objEventScraper.isStopEventScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objEventScraper.lstThreadsEventScraper.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objEventScraper.lstThreadsEventScraper.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnScrapers_MessageScraper_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objMessageScraper.isStopMessageScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objMessageScraper.lstThreadsMessageScraper.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objMessageScraper.lstThreadsMessageScraper.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnEmails_EmailCreator_StopEmailCreation_Click(object sender, EventArgs e)
        {
            Globals.CheckStopAccountCreation = false;
        }

        private void btnProxies_ProxyChecker_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objProxyManager.isStopProxyChecker = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objProxyManager.lstThreadsProxyChecker.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objProxyManager.lstThreadsProxyChecker.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        //private void Btn_Captcha_SaveSetting_Click(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        string currentOprationTask = string.Empty;
        //        faceboardpro.Domain.Captcha objcaptcha = new faceboardpro.Domain.Captcha();

        //        string Username = string.Empty;
        //        string Password = string.Empty;
        //        string CaptchaService = string.Empty;
        //        if (rdbDBC.Checked)
        //        {
        //            Username = txtDbcUserName.Text;
        //            Password = txtDbcPassword.Text;

        //            FBGlobals.dbcUserName = Username;
        //            FBGlobals.dbcPassword = Password;

        //            CaptchaService = "DBC";
        //            if (isExitDBCSetting)
        //            {
        //                currentOprationTask = "Update";
        //            }
        //            else
        //            {
        //                currentOprationTask = "Insert";
        //            }


        //        }
        //        else if (rdbDecaptcher.Checked)
        //        {
        //            Username = txtDecaptchaerUserName.Text;
        //            Password = txtDecaptcherPassword.Text;
        //            CaptchaService = "Decaptcher";

        //            if (isExitDecaptcherSetting)
        //            {
        //                currentOprationTask = "Update";
        //            }
        //            else
        //            {
        //                currentOprationTask = "Insert";
        //            }
        //        }
        //        else if (rdbAniGate.Checked)
        //        {
        //            Username = txtAnigateUserName.Text;
        //            Password = txtAnigatePassword.Text;
        //            CaptchaService = "Anigate";

        //            if (isExitAnigateSetting)
        //            {
        //                currentOprationTask = "Update";
        //            }
        //            else
        //            {
        //                currentOprationTask = "Insert";
        //            }
        //        }
        //        else if (rdbImageTyperz.Checked)
        //        {
        //            Username = txtImageTyperzUserName.Text;
        //            Password =txtImageTypezPassword.Text;
        //            CaptchaService = "ImageTyperz";
        //            if (isExitImageTypezSetting)
        //            {
        //                currentOprationTask = "Update";
        //            }
        //            else
        //            {
        //                currentOprationTask = "Insert";
        //            }
        //        }

        //        if (string.IsNullOrEmpty(Username))
        //        {
        //            GlobusLogHelper.log.Info("Please enter the Username");
        //            GlobusLogHelper.log.Debug("Please enter the Username");
        //            return;
        //        }

        //        if (string.IsNullOrEmpty(Password))
        //        {
        //            GlobusLogHelper.log.Info("Please enter the Password");
        //            GlobusLogHelper.log.Debug("Please enter the Password");
        //            return;
        //        }



        //        DataSet ds = new DataSet();




        //        if (currentOprationTask == "Insert")
        //        {
        //            GlobusLogHelper.log.Info("CaptchaAccount :   Username : " + Username + " Password : " + Password + " CaptchaService : " + CaptchaService + "  has Saved");
        //            PageManagerQuery(ds, new string[] { "Model : CaptchaSetting", "Function : InsertCaptchaSettinginCaptchaDatatbase", Username, Password, CaptchaService, "True" });
        //        }
        //        else if (currentOprationTask == "Update")
        //        {
        //            GlobusLogHelper.log.Info("CaptchaAccount :   Username : " + Username + " Password : " + Password + " CaptchaService : " + CaptchaService + "  has Saved");
        //            PageManagerQuery(ds, new string[] { "Model : CaptchaSetting", "Function : UpdateCaptchaSettinginCaptchaDatatbase", Username, Password, CaptchaService, "True" });

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        private void rdbDBC_CheckedChanged(object sender, EventArgs e)
        {
        //    if (rdbDBC.Checked)
        //    {
        //        setCaptchatextforDbc(true);
        //    }
        }

        public void setCaptchatextforDbc(bool status)
        {
            //txtDbcUserName.Enabled = status;
            //txtDbcPassword.Enabled = status;

            //txtDecaptchaerUserName.Enabled = !status;
            //txtDecaptcherPassword.Enabled = !status;

            //txtImageTyperzUserName.Enabled = !status;
            //txtImageTypezPassword.Enabled = !status;

            //txtAnigateUserName.Enabled = !status;
            //txtAnigatePassword.Enabled = !status;

        }

        public void setCaptchatextforDeacaptcher(bool status)
        {
            //txtDbcUserName.Enabled = !status;
            //txtDbcPassword.Enabled = !status;

            //txtDecaptchaerUserName.Enabled = status;
            //txtDecaptcherPassword.Enabled = status;

            //txtImageTyperzUserName.Enabled = !status;
            //txtImageTypezPassword.Enabled = !status;

            //txtAnigateUserName.Enabled = !status;
            //txtAnigatePassword.Enabled = !status;

        }

        public void setCaptchatextforImageTypez(bool status)
        {
            //txtDbcUserName.Enabled = !status;
            //txtDbcPassword.Enabled = !status;

            //txtDecaptchaerUserName.Enabled = !status;
            //txtDecaptcherPassword.Enabled = !status;

            //txtImageTyperzUserName.Enabled = status;
            //txtImageTypezPassword.Enabled = status;

            //txtAnigateUserName.Enabled = !status;
            //txtAnigatePassword.Enabled = !status;

        }

        public void setCaptchatextforAnimet(bool status)
        {
            //txtDbcUserName.Enabled = !status;
            //txtDbcPassword.Enabled = !status;

            //txtDecaptchaerUserName.Enabled = !status;
            //txtDecaptcherPassword.Enabled = !status;

            //txtImageTyperzUserName.Enabled = !status;
            //txtImageTypezPassword.Enabled = !status;

            //txtAnigateUserName.Enabled = status;
            //txtAnigatePassword.Enabled = status;

        }

        private void rdbDecaptcher_CheckedChanged(object sender, EventArgs e)
        {
          //  if (rdbDecaptcher.Checked)
            //{
            //    setCaptchatextforDeacaptcher(true);
            //}
        }

        private void rdbAniGate_CheckedChanged(object sender, EventArgs e)
        {

            //if (rdbAniGate.Checked)
            //{
            //    setCaptchatextforAnimet(true);
            //}
        }

        private void rdbImageTyperz_CheckedChanged(object sender, EventArgs e)
        {
            //if (rdbImageTyperz.Checked)
            //{
            //    setCaptchatextforImageTypez(true);
            //}
        }

        private void btnPage_CommentLiker_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objPageManager.isStopCommentLiker = false;
                    objPageManager.lstThreadsCommentLiker.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtPages_CommentPoster_Threads.Text) && checkNo.IsMatch(txtPages_CommentPoster_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtPages_CommentPoster_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objPageManager.NoOfThreadsCommentLiker = threads;
                    try
                    {
                        objPageManager.LimitOFPostPerAccountCommentLiker =int.Parse(txtPages_CommentPoster_LimitPerAcc.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                    try
                    {
                        PageManager.minDelayFanPagePoster = Convert.ToInt32(txtPage_CommentLiker_DelayMin.Text);
                        PageManager.maxDelayFanPagePoster = Convert.ToInt32(txtPage_CommentLiker_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }


                    try
                    {
                        PageManager.PageCommentLikerTargetedProcessUsing = cmb_CommentLiker_StartFanPageLikerProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }

                    if (PageManager.PageCommentLikerTargetedProcessUsing == "TargetedLike Comment")                    
                    {
                        if (objPageManager.lstFanPageUrlTargedCollectionCommentLiker.Count == 0)
                        {
                            GlobusLogHelper.log.Debug("Please Load Targeted Urls.!");
                            GlobusLogHelper.log.Info("Please Load Targeted Urls.!");
                            return;
                        }
                    }
                    if (PageManager.PageCommentLikerTargetedProcessUsing == "Like and Post Comment")
                    {
                        if (objPageManager.lstFanPageUrlCollectionCommentLiker.Count == 0)
                        {
                            GlobusLogHelper.log.Debug("Please Load Targeted Urls.!");
                            GlobusLogHelper.log.Info("Please Load Targeted Urls.!");
                            return;
                        }
                    }
                    if (PageManager.PageCommentLikerTargetedProcessUsing == "PostLiker")
                    {
                        if (objPageManager.lstFanPageUrlCollectionCommentLiker.Count == 0)
                        {
                            GlobusLogHelper.log.Debug("Please Load PostTargeted Urls.!");
                            GlobusLogHelper.log.Info("Please Load PostTargeted Urls.!");
                            return;
                        }
                    }

                    if (PageManager.PageCommentLikerTargetedProcessUsing == "PhotoLiker")
                    {
                        if (objPageManager.lstFanPageUrlCollectionCommentLiker.Count == 0)
                        {
                            GlobusLogHelper.log.Debug("Please Load PhotoTargeted Urls.!");
                            GlobusLogHelper.log.Info("Please Load PhotoTargeted Urls.!");
                            return;
                        }
                    }

                    Thread postCommentLikerThread = new Thread(objPageManager.StarCommentLiker);
                    postCommentLikerThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_CommentLiker_LoadUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_CommentLiker_LoadFanPageUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstFanPageUrlCollectionCommentLiker = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPages_CommentLiker_LoadFanPageUrlsCount.Text = objPageManager.lstFanPageUrlCollectionCommentLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_CommentLiker";
                        objSetting.FileType = "Pages_CommentLiker_LoadFanPageUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Comment Liker Loaded : " + objPageManager.lstFanPageUrlCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Greeting Messages Loaded : " + objPageManager.lstFanPageUrlCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnAccounts_AccountVarification_LoadEmails_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblaccounts_accountVarification_emailpath.Text = ofd.FileName;
                        lstLoadedEmailsAccountVarification.Clear();
                        LoadEmailsAccountsVarification(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblaccounts_accountVarification_emailCount.Text = lstLoadedEmailsAccountVarification.Distinct().ToList().Count.ToString();
                        #region commentedCode
                        //Insert Seeting Into Database
                        //objSetting.Module = "Accounts_AccountCreator";
                        //objSetting.FileType = "Accounts_AccountCreator_LoadEmails";
                        //objSetting.FilePath = ofd.FileName;

                        //InsertOrUpdateSetting(objSetting); 
                        #endregion
                        GlobusLogHelper.log.Debug("Emails Loaded : " + lstLoadedEmailsAccountVarification.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Info("Emails Loaded : " + lstLoadedEmailsAccountVarification.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void LoadEmailsAccountsVarification(string emailFile)
        {
            try
            {
                lstLoadedEmailsAccountVarification = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedEmailsAccountVarification = lstLoadedEmailsAccountVarification.Distinct().ToList();

                if (faceboardpro.FBGlobals.Instance.isfreeversion)
                {
                    try
                    {
                        lstLoadedEmailsAccountVarification.RemoveRange(5, lstLoadedEmails.Count - 5);
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

        private void btn_Account_AccountVarification_Start_Click(object sender, EventArgs e)
        {
            string AccountVerificationProcessUsing = string.Empty;

            try
            {
                ObjAccountManager.isStopAccountVerification = false;
                ObjAccountManager.lstAccountVerificationThreads.Clear();

                Regex checkNo = new Regex("^[0-9]*$");

                int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                int threads = 25;

                int maxThread = 25 * processorCount;

                if (chkAccounts_AccountVerification_ExportFile.Checked)
                {
                    AccountManager.exportFilePathAccountVerification = GlobusFileHelper.DesktopPathAccountChecker;
                }
                try
                {
                    AccountManager.minDelayAccountVerification = Convert.ToInt32(txtAccount_AcoountVerification_DelayMin.Text);
                    AccountManager.maxDelayAccountVerification = Convert.ToInt32(txtAccount_AcoountVerification_DelayMax.Text);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {
                    AccountVerificationProcessUsing = cmoAccounts_AccountVerificationProcessUsing.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                    GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                    return;
                }
                int ThreadCount = Convert.ToInt32(txtAccounts_AccountVerification_Threads.Text);
                if (ThreadCount == 0)
                {
                    GlobusLogHelper.log.Info("Please Enter More than 0 thread ..");
                    GlobusLogHelper.log.Debug("Please Enter More than 0 thread ..");
                    return;
                }

                if (!string.IsNullOrEmpty(txtAccounts_AccountVerification_Threads.Text) && checkNo.IsMatch(txtAccounts_AccountVerification_Threads.Text))
                {
                    maxThread = Convert.ToInt32(txtAccounts_AccountVerification_Threads.Text);
                }
                if (lstLoadedEmailsAccountVarification.Count() > 0)
                {
                    AccountManager.StartAccountVerificationProcessUsing = AccountVerificationProcessUsing;
                    foreach (string item in lstLoadedEmailsAccountVarification)
                    {
                        try
                        {
                            if (ObjAccountManager.isStopAccountVerification)
                            {
                                break;
                            }
                            #region CommentedCode threadPool
                            //Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                            //ThreadPool.SetMaxThreads(maxThread, 5);
                            //ThreadPool.QueueUserWorkItem(new WaitCallback(StartAccountVerification), new object[] { item });
                            //ObjAccountManager.StartAccountVerification(new object[] { item }); 
                            #endregion

                            Thread threadStartAccountVerification = new Thread(StartAccountVerification);
                            threadStartAccountVerification.SetApartmentState(ApartmentState.STA);
                            threadStartAccountVerification.Start(new object[] { item });
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    GlobusLogHelper.log.Debug("Please Load Emails Account.");
                    GlobusLogHelper.log.Info("Please Load Emails Account.");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void StartAccountVerification(object obj)
        {
            try
            {
                ObjAccountManager.lstAccountVerificationThreads.Add(Thread.CurrentThread);
                ObjAccountManager.lstAccountVerificationThreads = ObjAccountManager.lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            try
            {
                ObjAccountManager.StartAccountVerification(obj);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        #region MyRegion
        private void toolStripMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
          
            tabMain.SelectedTab = tabMain.TabPages["tabPagePages"];
          
        }

        private void toolStripButtonAccounts_Click(object sender, EventArgs e)
        {
              tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];              
           
        }
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabWall"];  
        }
        private void toolStripButtonFriends_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPageFriends"];
        }

        private void toolStripButtonPages_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButtonEvents_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPageEvents"];
        }

        private void toolStripButtonMessages_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPageMessages"];
        }

        private void toolStripButtontabGroups_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabGroups"];
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPage3"];
        }

        private void toolStripButtontabPageScrapers_Click(object sender, EventArgs e)
        {
           tabMain.SelectedTab = tabMain.TabPages["tabPageScrapers"];
        }

        private void toolStripButtontabPageCampaigns_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPageCampaigns"];
        }

        private void toolStripbtnScheduler_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPageScheduler"];
        }

        private void toolStripButtontabPage7_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPage7"];
        }

        private void toolStripButtontabPage8_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPage8"];
        }

        private void toolStripButtontabPage9_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPage9"];
        }

        private void toolStriptabPage10_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabPage5"];
        }
        private void toolStripButtonSearch10_Click(object sender, EventArgs e)
        {
            tabMain.SelectedTab = tabMain.TabPages["tabSearch"];
        }  
        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {

        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void tabPageAccounts_Click(object sender, EventArgs e)
        {

        }

        private void grvAccounts_AccountCreator_AccountDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageAccountCreator_Click(object sender, EventArgs e)
        {

        }

        private void splitContainerAccountCreator_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblaccounts_accountCreator_CountCreatedAccount_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_GenerateNameCount_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_LastNameCount_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_FirstNameCount_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_emailCount_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_GenerateNamepath_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_LastNamepath_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_FirstNamepath_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_accountCreator_emailpath_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void lblAccounts_AccountCreator_Threads_Click(object sender, EventArgs e)
        {

        }

        private void txtAccounts_AccountCreator_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmoAccounts_AccountCreator_Gender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkAccounts_AccountCreator_UseDelay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbAccounts_AccountCreator_UseBrowser_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbAccounts_AccountCreator_UseSockets_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_Accounts_CreateAccounts_GenerateNames_Click(object sender, EventArgs e)
        {

        }

        private void menuStripMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabPageManageProfile_Click(object sender, EventArgs e)
        {

        }

        private void splitContainerManageProfiles_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_Threads_Click(object sender, EventArgs e)
        {

        }

        private void txtAccounts_ManageProfiles_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkAccounts_ManageProfiles_UpdateOnly_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmbAccounts_ManageProfiles_UpdateOnly_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_ProfileDataCount_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_PicturesCount_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_CoverPicsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_ProfileDataPath_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageProfiles_PicturesPath_Click(object sender, EventArgs e)
        {

        }

        private void grvAccounts_ManageProfiles_AccountDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageManageAccounts_Click(object sender, EventArgs e)
        {

        }

        private void splitContainerManageAccounts_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void lblaccounts_ManageAccounts_LoadsAccountsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblAccounts_ManageAccounts_LoadsAccountsPath_Click(object sender, EventArgs e)
        {

        }

        private void grvAccounts_ManageAccounts_ManageAccountsDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageAccountVerification_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer20_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        #endregion

        private void btn_Account_AccountVarification_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                Thread threadStopAccountVerification = new Thread(StopAccountVerification);
                threadStopAccountVerification.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StopAccountVerification()
        {
            try
            {
                ObjAccountManager.isStopAccountVerification = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = ObjAccountManager.lstAccountVerificationThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        ObjAccountManager.lstAccountVerificationThreads.Remove(item);
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

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        #region MyRegion


        private void rdbFriends_RequestFriends_SearchViaFanPageURLs_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbFriends_RequestFriends_SearchViaFanPageURLs.Checked)
            {
                tabMain.SelectedTab = tabMain.TabPages["tabPageScrapers"];
                tabScrapers.SelectedTab = tabScrapers.TabPages["tabPageFanPageScraper"];
            }
        }




        private void btnPages_CommentLiker_Messages_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_CommentLiker_LoadMessagePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstMessageCollectionCommentLiker = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPages_CommentLiker_LoadMessageCount.Text = objPageManager.lstMessageCollectionCommentLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_CommentLiker";
                        objSetting.FileType = "Pages_CommentLiker_Load";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info(" Messages Loaded : " + objPageManager.lstMessageCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug(" Messages Loaded : " + objPageManager.lstMessageCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        #region UseLessCode
        private void dataGridView16_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageEvents_Click(object sender, EventArgs e)
        {

        }

        private void tabPageEventCreator_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblEvents_EventCreator_EventDetailsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblEvents_EventCreator_EventDetailsCount_Click(object sender, EventArgs e)
        {

        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void txtEvents_EventCreator_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageEventInviter_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblEvents_EventInviter_EventURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblEvents_EventInviter_EventURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }

        private void grpSelectionMode_Enter(object sender, EventArgs e)
        {

        }

        private void lblSelectNoOfFriendsToSuggestAtOneTime_Click(object sender, EventArgs e)
        {

        }

        private void cmbEvents_EventInviter_NoOfFriendsToSuggestAtOneTime_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblNoOfSuggestions_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void chkEvents_EventInviter_SendToAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtEvents_EventInviter_NoOfSuggestionsPerAccount_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void txtEvents_EventInviter_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageMessages_Click(object sender, EventArgs e)
        {

        }

        private void tabPageMessageReply_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer9_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblMessages_MessageReply_KeywordsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblMessages_MessageReply_KeywordsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblMessages_MessageReply_ReplyMessagePath_Click(object sender, EventArgs e)
        {

        }

        private void lblMessages_MessageReply_ReplyMessageCount_Click(object sender, EventArgs e)
        {

        }

        private void groupBox17_Enter(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void txtMessages_MessageReply_SingleMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void cmbMessages_MessageReply_SendMessageUsing_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void txtMessages_MessageReply_Threads_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private void grvMessages_MessageReply_MessageDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            #region MyRegion
            //foreach (DataGridViewRow row in grvMessages_MessageReply_MessageDetails.Rows)
            //{
            //    try
            //    {

            //        if (row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
            //        {
            //          // objMessageManager.LstReplyDetailsMessageReply.Add("<UserName>" + row.Cells[1].Value.ToString() + ":" + "<MassageFriendId>" + row.Cells[2].Value.ToString() + ":" + "<MassageSnippedId>" + row.Cells[3].Value.ToString() + ":" + "<MassageSenderName>" + row.Cells[4].Value.ToString() + ":" + "<MessagingReadParticipants>" + row.Cells[5].Value.ToString());
            //            string TextMessage = row.Cells[6].Value.ToString();
            //            txtMessages_Message_MessageBox.Text = TextMessage;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //    }
            //} 
            #endregion

        }

        #region MyRegion
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }



        private void tabPageGroupInviter_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer10_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblGroups_GroupInviter_GroupURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblGroups_GroupInviter_GroupURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void groupBox18_Enter(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void txtGroups_GroupInviter_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void ddataGridView10_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageTagPhoto_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer11_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void lblPhotos_TagPhoto_PhotoTagURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblPhotos_TagPhoto_PhotoTagURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void groupBox19_Enter(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void txtPhotos_TagPhoto_TagNoOfFriends_TextChanged(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void txtPhotos_TagPhoto_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView9_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageScrapers_Click(object sender, EventArgs e)
        {

        }

        private void tabPageFanPageScraper_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer5_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void lblScrapers_FanPageScraper_LoadFanPageURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScrapers_FanPageScraper_LoadFanPageURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void groupBox13_Enter(object sender, EventArgs e)
        {

        }

        private void label47_Click(object sender, EventArgs e)
        {

        }

        private void txtScrapers_FanPageScraper_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageEventsScraper_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void lblScrapers_EventsScraper_EventURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScrapers_EventsScraper_EventURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void txtScrapers_EventsScraper_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageMessageScraper_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer8_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void groupBox16_Enter(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void txtScrapers_MessageScraper_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView7_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageCampaigns_Click(object sender, EventArgs e)
        {

        }

        private void tabPageCampaignProcess_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer14_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox23_Enter(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer15_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox25_Enter(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageCommentsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_SelectCampaignForUpdate_Click(object sender, EventArgs e)
        {

        }

        private void cmbCampaigns_cmpFanPageLiker_SelectCampaignForUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_CampaignName_Click(object sender, EventArgs e)
        {

        }

        private void txtCampaigns_cmpFanPageLiker_CampaignName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label61_Click(object sender, EventArgs e)
        {

        }

        private void cmbCampaigns_cmpFanPageLiker_CampaignProcess_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesCount_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageMessagesPath_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadFanPageURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadAccountsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblCampaigns_cmpFanPageLiker_LoadAccountsPath_Click(object sender, EventArgs e)
        {

        }

        private void label68_Click(object sender, EventArgs e)
        {

        }

        private void groupBox26_Enter(object sender, EventArgs e)
        {

        }

        private void label69_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView13_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageScheduler_Click(object sender, EventArgs e)
        {

        }

        private void tabPageSchedulerProcess_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer16_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox31_Enter(object sender, EventArgs e)
        {

        }

        private void label60_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void grvScheduler_SchedulerProcess_SchedulerProcessDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPageSchFanPageLiker_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer17_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox32_Enter(object sender, EventArgs e)
        {

        }

        private void label80_Click(object sender, EventArgs e)
        {

        }

        private void txtScheduler_SchFanPageLiker_EndTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void label79_Click(object sender, EventArgs e)
        {

        }

        private void txtScheduler_SchFanPageLiker_StartTime_TextChanged(object sender, EventArgs e)
        {

        }

        private void label78_Click(object sender, EventArgs e)
        {

        }

        private void dtpScheduler_SchFanPageLiker_EndDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label77_Click(object sender, EventArgs e)
        {

        }

        private void dtpScheduler_SchFanPageLiker_StartDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageCommentsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageCommentsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_SelectSchedulerForUpdate_Click(object sender, EventArgs e)
        {

        }

        private void cmbScheduler_SchFanPageLiker_SelectSchedulerForUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_SchedulerName_Click(object sender, EventArgs e)
        {

        }

        private void txtScheduler_SchFanPageLiker_SchedulerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label66_Click(object sender, EventArgs e)
        {

        }

        private void cmbScheduler_SchFanPageLiker_SchedulerProcess_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageMessagesCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageMessagesPath_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageURLsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_FanPageURLsPath_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_AccountsCount_Click(object sender, EventArgs e)
        {

        }

        private void lblScheduler_SchFanPageLiker_AccountsPath_Click(object sender, EventArgs e)
        {

        }

        private void label75_Click(object sender, EventArgs e)
        {

        }

        private void groupBox33_Enter(object sender, EventArgs e)
        {

        }

        private void label76_Click(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView14_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageEmailCreator_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer6_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_CountGenratedNames_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_CountLastNames_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_CountCreatedFirstName_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_CountEmails_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_GenratedNamePath_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_LastNamePath_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_FirstNamePath_Click(object sender, EventArgs e)
        {

        }

        private void lblEmails_EmailCreator_EmailsPath_Click(object sender, EventArgs e)
        {

        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {

        }

        private void label49_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEmails_EmailCreator_GenerateNames_Click(object sender, EventArgs e)
        {

        }

        private void grvEmails_EmailCreator_AccountDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage8_Click(object sender, EventArgs e)
        {

        }

        private void tabControl5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer18_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void groupBox35_Enter(object sender, EventArgs e)
        {

        }

        private void lblProxies_ProxyChecker_ProxiesCount_Click(object sender, EventArgs e)
        {

        }

        private void lblProxies_ProxyChecker_ProxiesPath_Click(object sender, EventArgs e)
        {

        }

        private void rdbProxies_ProxyChecker_Private_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbProxies_ProxyChecker_Public_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label91_Click(object sender, EventArgs e)
        {

        }

        private void groupBox34_Enter(object sender, EventArgs e)
        {

        }

        private void label94_Click(object sender, EventArgs e)
        {

        }

        private void txtProxies_ProxyChecker_Threads_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView15_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage11_Click(object sender, EventArgs e)
        {

        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void groupBox30_Enter(object sender, EventArgs e)
        {

        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void groupBox29_Enter(object sender, EventArgs e)
        {

        }

        private void txtImageTypezPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label58_Click(object sender, EventArgs e)
        {

        }

        private void txtImageTyperzUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void groupBox28_Enter(object sender, EventArgs e)
        {

        }

        private void txtAnigatePassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void txtAnigateUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label48_Click(object sender, EventArgs e)
        {

        }

        private void groupBox27_Enter(object sender, EventArgs e)
        {

        }

        private void txtDecaptcherPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void txtDecaptchaerUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void groupBox24_Enter(object sender, EventArgs e)
        {

        }

        private void txtDbcPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void txtDbcUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void tabPage10_Click(object sender, EventArgs e)
        {

        }

        private void tabControl4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

    

        private void label93_Click(object sender, EventArgs e)
        {

        }

        private void label89_Click(object sender, EventArgs e)
        {

        }

        private void label90_Click(object sender, EventArgs e)
        {

        }

        private void label87_Click(object sender, EventArgs e)
        {

        }

        private void label88_Click(object sender, EventArgs e)
        {

        }

        private void label85_Click(object sender, EventArgs e)
        {

        }

        private void label86_Click(object sender, EventArgs e)
        {

        }

        private void label83_Click(object sender, EventArgs e)
        {

        }

        private void label84_Click(object sender, EventArgs e)
        {

        }

        private void label74_Click(object sender, EventArgs e)
        {

        }

        private void label81_Click(object sender, EventArgs e)
        {

        }

        private void label82_Click(object sender, EventArgs e)
        {

        }

        private void label73_Click(object sender, EventArgs e)
        {

        }

        private void label71_Click(object sender, EventArgs e)
        {

        }

        private void label72_Click(object sender, EventArgs e)
        {

        }

        private void label67_Click(object sender, EventArgs e)
        {

        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void label64_Click(object sender, EventArgs e)
        {

        }

        private void label65_Click(object sender, EventArgs e)
        {

        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void label62_Click(object sender, EventArgs e)
        {

        }

        private void listBoxLogs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion

        string AccountsAccountVerificationExportFilePath = string.Empty;

        private void chkAccounts_AccountVerification_ExportFile_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAccounts_AccountVerification_ExportFile.Checked)
                {
                    string FilePath = string.Empty;
                    try
                    {
                        using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                        {
                            ofd.SelectedPath = Application.StartupPath;

                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                FilePath = ofd.SelectedPath;
                                GlobusFileHelper.DesktopPathAccountChecker = FilePath;
                                AccountManager.exportFilePathAccountVerification = ofd.SelectedPath;
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

        private void btnWall_WallPoster_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objWallPostManager.isStopWallPoster = false;
                    objWallPostManager.lstThreadsWallPoster.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        WallPostManager.minDelayWallPoster = Convert.ToInt32(txtWall_WallPoster_DelayMin.Text);
                        WallPostManager.maxDelayWallPoster = Convert.ToInt32(txtWall_WallPoster_DelayMax.Text);

                        WallPostManager.messageCountWallPoster = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtWall_PostPicOnWall_Threads.Text) && checkNo.IsMatch(txtWall_PostPicOnWall_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtWall_PostPicOnWall_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objWallPostManager.NoOfThreadsWallPoster = threads;

                    int noOfFriends = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);

                    if (!string.IsNullOrEmpty(txtWall_WallPoster_NoOfFriends.Text) && checkNo.IsMatch(txtWall_WallPoster_NoOfFriends.Text))
                    {
                        noOfFriends = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);
                        objWallPostManager.NoOfFriendsWallPoster = noOfFriends;
                    }
                    else
                    {
                        txtWall_WallPoster_NoOfFriends.Text = (5).ToString();
                        objWallPostManager.NoOfFriendsWallPoster = noOfFriends;
                    }

                    if (rdbWall_WallPoster_UseTextMessages.Checked)
                    {
                        if (objWallPostManager.lstWallMessageWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load Text Messages !");
                            GlobusLogHelper.log.Debug("Please Load Text Messages !");

                            MessageBox.Show("Please Load Text Messages !");
                            return;
                        }
                        objWallPostManager.IsUseTextMessageWallPoster = true;
                    }
                    else if (rdbWall_WallPoster_UseURLsMessages.Checked)
                    {
                        if (objWallPostManager.lstWallPostURLsWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load URLs Messages !");
                            GlobusLogHelper.log.Debug("Please Load URLs Messages !");

                            MessageBox.Show("Please Load URLs Messages !");
                            return;
                        }

                        objWallPostManager.IsUseURLsMessageWallPoster = true;
                        objWallPostManager.UseAllUrlWallPoster = true;
                    }
                    else if (rdbWall_WallPoster_UseSpinnedMessages.Checked)
                    {
                        if (objWallPostManager.lstSpinnerWallMessageWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load Spinned Messages !");
                            GlobusLogHelper.log.Debug("Please Load Spinned Messages !");

                            MessageBox.Show("Please Load Spinned Messages !");
                            return;
                        }

                        objWallPostManager.ChkSpinnerWallMessaeWallPoster = true;
                    }
                    else
                    {
                    }

                    string messagePostingMode = string.Empty;

                    try
                    {

                        messagePostingMode = cmbWall_WallPoster_MessagePostingMode.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Message Posting Mode drop down list.");
                        GlobusLogHelper.log.Info("Please select Message Posting Mode drop down list.");
                        return;
                    }

                    if (messagePostingMode == "Same Message Posting")
                    {
                        objWallPostManager.UseOneMsgToAllFriendsWallPoster = true;
                    }
                    else if (messagePostingMode == "Unique Message Posting")
                    {
                        objWallPostManager.UseUniqueMsgToAllFriendsWallPoster = true;
                    }
                    else if (messagePostingMode == "Random Message Posting")
                    {
                        objWallPostManager.UseRandomWallPoster = true;
                    }
                    else
                    {
                        objWallPostManager.UseRandomWallPoster = true;
                        //cmbPages_WallPoster_MessagePostingMode.SelectedIndex = 2;
                        //cmbPages_WallPoster_MessagePostingMode.SelectedItem = cmbPages_WallPoster_MessagePostingMode.SelectedIndex;
                    }

                    Thread wallPosterThread = new Thread(objWallPostManager.StartWallPoster);
                    wallPosterThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnWall_WallPoster_LoadTextMessages_Click(object sender, EventArgs e)
        {
            rdbWall_WallPoster_UseTextMessages.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_WallPoster_TextMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objWallPostManager.lstWallMessageWallPoster = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblWall_WallPoster_TextMessagesCount.Text = objWallPostManager.lstWallMessageWallPoster.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_WallPoster";
                        objSetting.FileType = "Pages_WallPoster_LoadTextMessages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Text Messages Loaded : " + objWallPostManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Text Messages Loaded : " + objWallPostManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        #region CommentedCode
        //private void btnWall_WallPoster_LoadURLsMessages_Click(object sender, EventArgs e)
        //{
        //    rdbWall_WallPoster_UseURLsMessages.Checked = true;
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblWall_WallPoster_URLsMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();

        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

        //                objWallPostManager.lstWallPostURLsWallPoster= lstTemp.Distinct().ToList();
        //                //lstWallPostURLsWallPoster
        //                objWallPostManager.lstMessagesWallPoster = lstTemp.Distinct().ToList();
        //                DateTime eTime = DateTime.Now;

        //                lblWall_WallPoster_URLsMessagesCount.Text = objWallPostManager.lstMessagesWallPoster.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_WallPoster";
        //                objSetting.FileType = "Pages_WallPoster_LoadURLsMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("URLs Messages Loaded : " + objPageManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("URLs Messages Loaded : " + objPageManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnWall_WallPoster_LoadSpinnedMessages_Click(object sender, EventArgs e)
        //{
        //    rdbWall_WallPoster_UseSpinnedMessages.Checked = true;
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblWall_WallPoster_SpinnedMessagesPath.Text = ofd.FileName;
        //                List<string> lstTemp = new List<string>();
        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
        //                objWallPostManager.lstSpinnerWallMessageWallPoster = lstTemp.Distinct().ToList();

        //                DateTime eTime = DateTime.Now;

        //                lblWall_WallPoster_SpinnedMessagesCount.Text = objWallPostManager.lstSpinnerWallMessageWallPoster.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Pages_WallPoster";
        //                objSetting.FileType = "Pages_WallPoster_LoadSpinnedMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("Spinned Messages Loaded : " + objWallPostManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("Spinned Messages Loaded : " + objWallPostManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //private void btnWall_PostPicOnWall_LoadGreetingMessages_Click(object sender, EventArgs e)
        //{
        //    chkWall_PostPicOnWall_WithMessage.Checked = true;
        //    try
        //    {
        //        using (OpenFileDialog ofd = new OpenFileDialog())
        //        {

        //            ofd.Filter = "Text Files (*.txt)|*.txt";
        //            ofd.InitialDirectory = Application.StartupPath;
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                DateTime sTime = DateTime.Now;

        //                lblWall_PostPicOnWall_LoadGreetingMessagesPath.Text = ofd.FileName;

        //                List<string> lstTemp = new List<string>();


        //                lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


        //                objWallPostManager.lstMessageCollectionPostPicOnWall = lstTemp.Distinct().ToList();


        //                DateTime eTime = DateTime.Now;

        //                lblWall_PostPicOnWall_LoadGreetingMessagesCount.Text = objWallPostManager.lstMessageCollectionPostPicOnWall.Count.ToString();

        //                string timeSpan = (eTime - sTime).TotalSeconds.ToString();

        //                //Insert Seeting Into Database
        //                objSetting.Module = "Wall_PostPicOnWall";
        //                objSetting.FileType = "Wall_PostPicOnWall_LoadGreetingMessages";
        //                objSetting.FilePath = ofd.FileName;

        //                InsertOrUpdateSetting(objSetting);

        //                GlobusLogHelper.log.Info("Greeting Messages Loaded : " + objWallPostManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
        //                GlobusLogHelper.log.Debug("Greeting Messages Loaded : " + objWallPostManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //}

        //private void btnWall_PostPicOnWall_LoadPicsFolder_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        List<string> lstWallPics = new List<string>();
        //        List<string> lstCorrectWallPics = new List<string>();

        //        using (FolderBrowserDialog ofd = new FolderBrowserDialog())
        //        {
        //            if (ofd.ShowDialog() == DialogResult.OK)
        //            {
        //                lblWall_PostPicOnWall_LoadPicsFolderImagePath.Text = ofd.SelectedPath;
        //                lstWallPics.Clear();
        //                lstCorrectWallPics.Clear();
        //                string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
        //                lstWallPics = picsArray.Distinct().ToList();
        //                string PicFilepath = ofd.SelectedPath;                     
        //                foreach (string item in lstWallPics)
        //                {
        //                    try
        //                    {
        //                        string items = item.ToLower();
        //                        if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
        //                        {
        //                            lstCorrectWallPics.Add(item);
        //                        }
        //                        else
        //                        {
        //                            GlobusLogHelper.log.Info("Wrong File Is :" + item);
        //                            GlobusLogHelper.log.Debug("Wrong File Is :" + item);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    { 
        //                        GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
        //                    }
        //                }

        //                lstCorrectWallPics = lstCorrectWallPics.Distinct().ToList();
        //                objWallPostManager.lstPicturecollectionPostPicOnWall = lstCorrectWallPics;
        //                lblWall_PostPicOnWall_LoadPicsFolderImageCount.Text = objWallPostManager.lstPicturecollectionPostPicOnWall.Count.ToString();

        //                GlobusLogHelper.log.Info(lstCorrectWallPics.Count + "  Pics loaded");
        //                GlobusLogHelper.log.Debug(lstCorrectWallPics.Count + "  Pics loaded");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }        
        //}

        //private void btnWall_PostPicOnWall_Start_Click(object sender, EventArgs e)
        //{           
        //    try
        //    {
        //        if (FBGlobals.listAccounts.Count > 0)
        //        {
        //            objWallPostManager.isStopPostPicOnWall = false;
        //            objWallPostManager.lstThreadsPostPicOnWall.Clear();

        //            Regex checkNo = new Regex("^[0-9]*$");

        //            int processorCount = objUtils.GetProcessor();

        //            int threads = 25;

        //            int maxThread = 25 * processorCount;

        //            if (!string.IsNullOrEmpty(txtPages_PostPicOnWall_Threads.Text) && checkNo.IsMatch(txtPages_PostPicOnWall_Threads.Text))
        //            {
        //                threads = Convert.ToInt32(txtPages_PostPicOnWall_Threads.Text);
        //            }
        //            try
        //            {
        //                WallPostManager.minDelayPostPicOnWal = Convert.ToInt32(txtPostPic_PostPicOnWal_DelayMin.Text);
        //                WallPostManager.maxDelayPostPicOnWal = Convert.ToInt32(txtPostPic_PostPicOnWal_DelayMax.Text);
        //            }
        //            catch(Exception ex)
        //            {
        //                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //            }

        //            if (threads > maxThread)
        //            {
        //                threads = 25;
        //            }
        //            objWallPostManager.NoOfThreadsPostPicOnWall = threads;

        //            if (objWallPostManager.lstPicturecollectionPostPicOnWall.Count < 1)
        //            {
        //                GlobusLogHelper.log.Info("Please Load Pics Folder !");
        //                GlobusLogHelper.log.Debug("Please Load Pics Folder !");
        //                return;
        //            }
        //            if (chkWall_PostPicOnWall_UseAllPics.Checked)
        //            {
        //                objWallPostManager.IsPostAllPicPostPicOnWall = true;
        //            }
        //            else
        //            {
        //                objWallPostManager.IsPostAllPicPostPicOnWall = false;
        //            }


        //            Thread postPicOnWallThread = new Thread(objWallPostManager.StarPostPicOnWall);
        //            postPicOnWallThread.Start();
        //        }
        //        else
        //        {
        //            GlobusLogHelper.log.Info("Please Load Accounts !");
        //            GlobusLogHelper.log.Debug("Please Load Accounts !");

        //            tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
        //            tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //} 
        #endregion

        private void btnGroups_GroupCampaignManager_LoadPicUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {


                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        lblGroups_GroupCampaignManager_PicURLsPath.Text = ofd.SelectedPath;
                        List<string> lstpicdata = new List<string>();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstpicdata = picsArray.ToList();



                        List<string> lstDistinctlstpicdata = new List<string>();
                        List<string> lstWronglstpicdata = new List<string>();
                        foreach (string item in lstpicdata)
                        {
                            try
                            {
                                string items = item.ToLower();

                                if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
                                {
                                    lstDistinctlstpicdata.Add(item);


                                }
                                else
                                {
                                    lstWronglstpicdata.Add(item);

                                }


                            }
                            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }



                            objGroupManager.LstPicUrlsGroupCampaignManager = lstDistinctlstpicdata;
                        }

                        lblGroups_GroupCampaignManager_PicURLsCount.Text = objGroupManager.LstPicUrlsGroupCampaignManager.Count().ToString();
                        Console.WriteLine(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Info(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Debug(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Info(lstWronglstpicdata.Count + " Incorrect Picture loaded");
                        GlobusLogHelper.log.Debug(lstWronglstpicdata.Count + " Incorrect Picture loaded");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
            try
            {

            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }

        private void btnGroups_GroupCampaignManager_LoadVideoUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupCampaignManager_VideoURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objGroupManager.LstVideoUrlsGroupCampaignManager = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblGroups_GroupCampaignManager_VideoURLsCount.Text = objGroupManager.LstVideoUrlsGroupCampaignManager.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupCampaignManager";
                        objSetting.FileType = "Groups_GroupCampaignManager_LoadURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("URLs Loaded : " + objGroupManager.LstVideoUrlsGroupCampaignManager.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("URLs Loaded : " + objGroupManager.LstVideoUrlsGroupCampaignManager.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupCampaignManager_LoadMessages_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupCampaignManager_MessagePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objGroupManager.LstMessageUrlsGroupCampaignManager = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblGroups_GroupCampaignManager_MessageCount.Text = objGroupManager.LstMessageUrlsGroupCampaignManager.Count.ToString();
                        FbGroupCampaignManagerGlobals.NoOfMessages = objGroupManager.LstMessageUrlsGroupCampaignManager.Count();
                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupCampaignManager";
                        objSetting.FileType = "Groups_GroupInviter_LoadMessaages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Message Loaded : " + objGroupManager.LstMessageUrlsGroupCampaignManager.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Message Loaded : " + objGroupManager.LstMessageUrlsGroupCampaignManager.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnWall_WallPoster_Stop_Click(object sender, EventArgs e)        
        {
            try
            {
                objWallPostManager.isStopWallPoster = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objWallPostManager.lstThreadsWallPoster.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objWallPostManager.lstThreadsWallPoster.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnGroups_GroupCampaignManager_Start_Click(object sender, EventArgs e)
        {
            #region Variables
              string CampaignName = string.Empty;
            string MessageMode = string.Empty;
            string MessageType = string.Empty;
            string TextMessage = string.Empty;
            string PicMessage = string.Empty;
            string VideoMessage = string.Empty;
            string Account = string.Empty;

            #endregion Variables

            # region Classes
            GroupCampaign objGroupCamapign = new GroupCampaign();
            #endregion
            string Process = string.Empty;
            try
            {
                Process = CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                return;

            }
            //  GroupManager.minDelayGroupManager = Convert.ToInt32();
            if (Process != null)
            {

                if (CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString().Contains("View Scheduler Task"))
                {
                    tabMain.SelectedTab = tabMain.TabPages["tabGroups"];
                    tabGroup.SelectedTab = tabGroup.TabPages["tabGroupViewSchedulerTask"];
                    return;
                }
                else if (CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString().Contains("Setting"))
                {
                    //    FrmGroupCampaignManagerSetting objFrmGroupCampaignManagerSetting = new FrmGroupCampaignManagerSetting();
                    //     objFrmGroupCampaignManagerSetting.ShowDialog();
                    return;
                }
            }
            try
            {
                try
                {

                    CampaignName = TxtGroups_GroupCampaignManager_NewCampaign.Text;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                int count = 0;
                if (Process == "Save Campaign")
                {
                    GroupCampaign objgroupCamp = new GroupCampaign();

                    string strCampaignName = TxtGroups_GroupCampaignManager_NewCampaign.Text;
                    ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                    foreach (GroupCampaign item in groupCollection)
                        {
                            if (CampaignName == item.GroupCampaignName)
                            {
                                count++;
                                string groupname = item.GroupCampaignName;
                            }
                        }
                    

                    if (count > 0)
                    {
                        GlobusLogHelper.log.Error("Campaign name already exists in datatable.!");
                        MessageBox.Show("Campaign name already exists in datatable.!");
                        return;
                    }
                }


                if (rdbGroup_GroupCampaignManager_RandomMessage.Checked == true)
                {
                    MessageMode = "Random Message";

                    #region Random Message
                    if (rdbGroup_GroupCampaignManager_RandomMessage.Checked)
                    {
                        //TxtGroups_GroupCampaignManager_SingleMessage
                        if (!string.IsNullOrEmpty(TxtGroups_GroupCampaignManager_SingleMessage.Text))
                        {
                            if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked)
                            {
                                // TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                                TextMessage = TxtGroups_GroupCampaignManager_SingleMessage.Text;
                                //  LstMessageUrlsGroupCampaignManager
                            }


                        }
                        //else
                        //{
                        //    if (!rdbGroup_GroupCampaignManager_Video.Checked)
                        //    {
                        //        GlobusLogHelper.log.Info("Please upload message file into Message File Box !");
                        //        GlobusLogHelper.log.Debug("Please upload message file into Message File Box !");
                        //        return;
                        //    }
                        //}
                    }
                    if (rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Pictures in load Pictures section !");
                            GlobusLogHelper.log.Debug("Please upload Pictures in load Pictures section !");

                        }
                    }
                    if (rdbGroup_GroupCampaignManager_Video.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload URL messages in load url section !");
                            GlobusLogHelper.log.Debug("Please upload URL messages in load url section !");

                        }
                    }
                    if (!rdbGroup_GroupCampaignManager_Onlymessage.Checked && !rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked && !rdbGroup_GroupCampaignManager_Video.Checked)
                    {

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload message file into Message section !");
                            GlobusLogHelper.log.Debug("Please upload message file into Message section !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Pictures in load Pictures section!");
                            GlobusLogHelper.log.Debug("Please upload Pictures in load Pictures section !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload URL messages in load url section!");
                            GlobusLogHelper.log.Debug("Please upload URL messages in load url section !");
                            return;
                        }

                    }
                    #endregion
                }
                if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked == true)
                {
                    MessageMode = "One Message";
                    #region one message
                    if (rdbGroup_GroupCampaignManager_Onlymessage.Checked || rdbGroup_GroupCampaignManager_Video.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            //  TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                            TextMessage = TxtGroups_GroupCampaignManager_SingleMessage.Text;

                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please insert message into Message Box !");
                            GlobusLogHelper.log.Debug("Please insert message into Message Box !");
                            return;
                        }
                    }

                    if (rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text) && !string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked)
                            {
                                TextMessage = TxtGroups_GroupCampaignManager_SingleMessage.Text;
                            }
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                            if (!rdbGroup_GroupCampaignManager_SelectOneMessage.Checked)
                            {
                                TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                                if (TextMessage == " ")
                                {
                                    TextMessage = TxtGroups_GroupCampaignManager_SingleMessage.Text;
                                }
                            }

                        }

                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File section !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File section !");
                            return;
                        }
                    }
                    if (rdbGroup_GroupCampaignManager_Video.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File section !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File section !");
                            return;
                        }
                    }
                    if (!rdbGroup_GroupCampaignManager_Onlymessage.Checked && !rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked && !rdbGroup_GroupCampaignManager_Video.Checked)
                    {

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {

                            GlobusLogHelper.log.Info("Please insert message into Message section !");
                            GlobusLogHelper.log.Debug("Please insert message into Message section !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File section  !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File section  !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File section !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File section !");
                            return;
                        }

                    }
                    #endregion

                }

                try
                {
                    if (rdbGroup_GroupCampaignManager_Onlymessage.Checked)
                    {

                        MessageType = "Only Message";
                        if (TextMessage == "" && objGroupManager.LstMessageUrlsGroupCampaignManager.Count() == null)
                        {
                            GlobusLogHelper.log.Debug("Please Load Text  Message !");
                            GlobusLogHelper.log.Info("Please Load Text Message !");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked)
                {
                    MessageType = "Only Picture with message";
                    if (PicMessage ==" ")
                    {
                        GlobusLogHelper.log.Debug("Please Load Picture folder !");
                        GlobusLogHelper.log.Info("Please Load Picture folder !");
                        
                    }
                }
               

                if (rdbGroup_GroupCampaignManager_Video.Checked)
                {
                    MessageType = "Only Video";

                    if (VideoMessage == " ")
                    {
                        GlobusLogHelper.log.Debug("Please Load  URLs file !");
                        GlobusLogHelper.log.Info("Please Load URLs  file !");
                        return;

                    }
                }


                Boolean possiblePath_fanepageurl = false;
                Boolean possiblePath_messagefile = false;
                Boolean possiblePath_Picfile = false;
                Boolean possiblePath_Videofile = false;
                int countmsg = 0;

                try
                {

                    possiblePath_messagefile = (lblGroups_GroupCampaignManager_MessagePath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                    possiblePath_Picfile = (lblGroups_GroupCampaignManager_PicURLsPath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                    possiblePath_Videofile = (lblGroups_GroupCampaignManager_VideoURLsPath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {
                    string ddlusername = string.Empty;
                    try
                    {
                         ddlusername = cmbGroups_GroupCampaignManager_Accounts.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    string[] Arr = ddlusername.Split(':');
                    try
                    {
                        ddlusername =Arr[0];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(TxtGroups_GroupCampaignManager_NewCampaign.Text))
                    {
                        objGroupCamapign.GroupCampaignName = CampaignName;
                        objGroupCamapign.Account = ddlusername;
                        objGroupCamapign.PicFilePath = PicMessage;
                        objGroupCamapign.TextMessage = TextMessage;
                        objGroupCamapign.VideoFilePath = VideoMessage;
                        objGroupCamapign.MessageFilePath = lblGroups_GroupCampaignManager_MessagePath.Text.ToString();
                        objGroupCamapign.ScheduleTime = FbGroupCampaignManagerGlobals.Scheduletime.ToString();
                        objGroupCamapign.NoOfMessage = FbGroupCampaignManagerGlobals.NoOfMessages.ToString();
                        objGroupCamapign.CmpStartTime = "";
                        objGroupCamapign.Accomplish = "0";
                        objGroupCamapign.MessageMode = MessageMode;
                        objGroupCamapign.MessageType = MessageType;
                        if (rdbGroupPosting.Checked)
                        {
                            objGroupCamapign.Module = "Group Posting";
                        }
                        else
                        {
                            objGroupCamapign.Module = "Group Request";
                        }
                        if (CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString().Contains("Save Campaign"))
                        {
                            objGroupCampaignManagerRepository.Insert(objGroupCamapign);
                            GlobusLogHelper.log.Info("Saved Campaign in database.");
                            GlobusLogHelper.log.Debug("Saved Campaign in database.");
                            try
                            {
                                CmbGroups_GroupCampaignManager_EditCampaign.Items.Clear();
                                GroupCampaign objgroupCamp = new GroupCampaign();
                                //ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                                objgroupCamp.Module = "Group Posting";
                                ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.SelectCampaigns(objgroupCamp);
                                foreach (GroupCampaign item in groupCollection)
                                {
                                    //string groupname = item.GroupCampaignName;
                                    CmbGroups_GroupCampaignManager_EditCampaign.Items.Add(item.GroupCampaignName);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                            if (rdbGroupRequest.Checked)
                            {
                                tabMain.SelectedTab = tabMain.TabPages["tabGroups"];
                                tabGroup.SelectedTab = tabGroup.TabPages["tabGroupRequestManager"];
                            }
                            else
                            {
                                tabMain.SelectedTab = tabMain.TabPages["tabGroups"];
                                tabGroup.SelectedTab = tabGroup.TabPages["tabGroupViewSchedulerTask"];
                            }
                           
                        }

                        else if (CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString().Contains("Update Campaign"))
                        {
                               int Res=0;
                               try
                               {

                                   Res = objGroupCampaignManagerRepository.UpdateQuery(objGroupCamapign);
                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }

                               if (Res == 1)
                               {
                                   try
                                   {
                                       CmbGroups_GroupCampaignManager_EditCampaign.Items.Clear();
                                       GroupCampaign objgroupCamp = new GroupCampaign();
                                       //ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                                       objgroupCamp.Module = "Group Posting";
                                       ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.SelectCampaigns(objgroupCamp);
                                       foreach (GroupCampaign item in groupCollection)
                                       {
                                           //string groupname = item.GroupCampaignName;
                                           CmbGroups_GroupCampaignManager_EditCampaign.Items.Add(item.GroupCampaignName);
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                   }
                                   GlobusLogHelper.log.Info("Campaign Updated Successfully.");
                                   GlobusLogHelper.log.Debug("Campaign Updated Successfully.");
                               }
                               else
                               {
                                   GlobusLogHelper.log.Info("Campaign not Updated ,Campaign name not Editable.");
                                   GlobusLogHelper.log.Debug("Campaign not Updated ,Campaign name not Editable.");

                               }

                        }
                        // Insert data in Database    
                      

                    }
                    else
                    {

                        GlobusLogHelper.log.Info("Please Enter Name Of Campaign..!");
                        GlobusLogHelper.log.Debug("Please Enter Name Of Campaign..!");
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

        public void startSaveGroupCampainManagerdata()
        {
            #region Variables
            string CampaignName = string.Empty;
            string MessageMode = string.Empty;
            string MessageType = string.Empty;
            string TextMessage = string.Empty;
            string PicMessage = string.Empty;
            string VideoMessage = string.Empty;

            #endregion Variables

            try
            {

                string Process = CmbGroup_GroupCampaignManager_StartProcessUsing.SelectedItem.ToString();
                CampaignName = TxtGroups_GroupCampaignManager_NewCampaign.Text;


                if (rdbGroup_GroupCampaignManager_RandomMessage.Checked == true)
                {
                    MessageMode = "Random Message";
                    if (rdbGroup_GroupCampaignManager_RandomMessage.Checked)
                    {

                        if (!string.IsNullOrEmpty(TxtGroups_GroupCampaignManager_SingleMessage.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload message file into Message File Box !");
                            GlobusLogHelper.log.Debug("Please upload message file into Message File Box !");
                            return;
                        }
                    }
                    if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File Box !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File Box !");

                        }
                    }
                    if (rdbGroup_GroupCampaignManager_Video.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File Box !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File Box !");

                        }
                    }
                    if (!rdbGroup_GroupCampaignManager_Onlymessage.Checked && !rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked && !rdbGroup_GroupCampaignManager_Video.Checked)
                    {

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload message file into Message File Box !");
                            GlobusLogHelper.log.Debug("Please upload message file into Message File Box !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File Box !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File Box !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File Box !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File Box !");
                            return;
                        }

                    }
                }
                if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked == true)
                {
                    MessageMode = "One Message";
                    if (rdbGroup_GroupCampaignManager_Onlymessage.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;

                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please insert message into Message Box !");
                            GlobusLogHelper.log.Debug("Please insert message into Message Box !");
                            return;
                        }
                    }

                    if (rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text) && !string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File Box !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File Box !");
                            return;
                        }
                    }
                    if (rdbGroup_GroupCampaignManager_Video.Checked)
                    {
                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File Box !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File Box !");
                            return;
                        }
                    }
                    if (!rdbGroup_GroupCampaignManager_Onlymessage.Checked && !rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked && !rdbGroup_GroupCampaignManager_Video.Checked)
                    {

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_MessagePath.Text))
                        {
                            TextMessage = lblGroups_GroupCampaignManager_MessagePath.Text;
                        }
                        else
                        {

                            GlobusLogHelper.log.Info("Please insert message into Message Box !");
                            GlobusLogHelper.log.Debug("Please insert message into Message Box !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_PicURLsPath.Text))
                        {
                            PicMessage = lblGroups_GroupCampaignManager_PicURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Picture file into Picture File Box !");
                            GlobusLogHelper.log.Debug("Please upload Picture file into Picture File Box !");
                            return;
                        }

                        if (!string.IsNullOrEmpty(lblGroups_GroupCampaignManager_VideoURLsPath.Text))
                        {
                            VideoMessage = lblGroups_GroupCampaignManager_VideoURLsPath.Text;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please upload Video file into Video File Box !");
                            GlobusLogHelper.log.Debug("Please upload Video file into Video File Box !");
                            return;
                        }
                    }
                }
                if (rdbGroup_GroupCampaignManager_SelectOneMessage.Checked)
                {
                    MessageType = "Only Message";
                }
                if (rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked)
                {
                    MessageType = "Only Picture with message";
                }
                if (rdbGroup_GroupCampaignManager_Video.Checked)
                {
                    MessageType = "Only Video";
                }


                Boolean possiblePath_fanepageurl = false;
                Boolean possiblePath_messagefile = false;
                Boolean possiblePath_Picfile = false;
                Boolean possiblePath_Videofile = false;
                int countmsg = 0;

                try
                {

                    possiblePath_messagefile = (lblGroups_GroupCampaignManager_MessagePath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                    possiblePath_Picfile = (lblGroups_GroupCampaignManager_PicURLsPath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                    possiblePath_Videofile = (lblGroups_GroupCampaignManager_VideoURLsPath.Text).IndexOfAny(Path.GetInvalidPathChars()) == -1;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                try
                {

                    string ddlusername = "";
                    try
                    {
                        // ddlusername = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(TxtGroups_GroupCampaignManager_NewCampaign.Text))
                    {

                        // Insert data in Database
                    }
                    else
                    {

                        GlobusLogHelper.log.Info("Please Enter Name Of Campaign..!");
                        GlobusLogHelper.log.Debug("Please Enter Name Of Campaign..!");
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

        private void btnWall_PostPicOnWall_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objWallPostManager.isStopPostPicOnWall = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objWallPostManager.lstThreadsPostPicOnWall.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objWallPostManager.lstThreadsPostPicOnWall.Remove(item);
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

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void tabGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabGroup.SelectedTab == tabGroup.TabPages["tabGroupViewSchedulerTask"] )
                {
                    LoadCampaignData("Group Posting");
                }
                else if (tabGroup.SelectedTab == tabGroup.TabPages["tabGroupRequestManager"])
                {
                    LoadCampaignData("Group Request");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void LoadCampaignData(string Module)        
        {
            try
            {
                dtCampaignSchedule = new DataTable();
                GroupCampaign objGroup = new GroupCampaign();
                objGroup.Module=Module;
                ICollection<GroupCampaign> lstGroupScheduleData = objGroupCampaignManagerRepository.SelectCampaigns(objGroup);

                dtEmailAccount = CreateGroupScheduleTable(Module);

                foreach (GroupCampaign item in lstGroupScheduleData)
                {
                    DataRow newRow = dtCampaignSchedule.NewRow();
                    try
                    {
                        if (Module.Equals("Group Posting"))
                        {
                            string GroupCampiagnName = item.GroupCampaignName;
                            string Account = item.Account;
                            string PicFilePath = item.PicFilePath;


                            string VideoFilePath = item.VideoFilePath;


                            string MessageFilePath = item.MessageFilePath;


                            string ScheduleTime = item.ScheduleTime;

                            string cmpStartTime = item.CmpStartTime;
                            string Accomplish = item.Accomplish;
                            string NoOfMessage = item.NoOfMessage;

                            string MessageMode = item.MessageMode;
                            string MessageType = item.MessageType;
                            string TextMessage = item.TextMessage;


                            newRow["GroupCampaignName"] = GroupCampiagnName;
                            newRow["Account"] = Account;
                            newRow["PicFilePath"] = PicFilePath;

                            newRow["VideoFilePath"] = VideoFilePath;
                            newRow["MessageFilePath"] = MessageFilePath;
                            newRow["ScheduleTime"] = ScheduleTime;

                            newRow["cmpStartTime"] = cmpStartTime;
                            newRow["Accomplish"] = Accomplish;
                            newRow["NoOfMessage"] = NoOfMessage;

                            newRow["MessageMode"] = MessageMode;
                            newRow["MessageType"] = MessageType;
                            newRow["TextMessage"] = TextMessage;

                            dtCampaignSchedule.Rows.Add(newRow);
                            try
                            {
                                grvGroup_ViewSchedulerTask_CampaignScheduler.DataSource = null;
                                grvGroup_ViewSchedulerTask_CampaignScheduler.DataSource = dtCampaignSchedule;

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        else if (Module.Equals("Group Request"))
                        {
                            string GroupCampiagnName = item.GroupCampaignName;
                            string Account = item.Account;
                            string GroupURL = item.VideoFilePath;
                            string ScheduleTime = item.ScheduleTime;
                            string cmpStartTime = item.CmpStartTime;
                            string Accomplish = item.Accomplish;
                            newRow["GroupCampaignName"] = GroupCampiagnName;
                            newRow["Account"] = Account;
                            newRow["Group URLs File Path"] = GroupURL;
                            newRow["ScheduleTime"] = ScheduleTime;
                            newRow["cmpStartTime"] = cmpStartTime;
                            newRow["Accomplish"] = Accomplish;
                            dtCampaignSchedule.Rows.Add(newRow);
                            try
                            {
                                dgvGroupRequestManager.DataSource = null;
                                dgvGroupRequestManager.DataSource = dtCampaignSchedule;

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
                if (Module.Equals("Group Request"))
                {
                try
                {
                    dgvGroupRequestManager.DataSource = null;
                    dgvGroupRequestManager.DataSource = dtCampaignSchedule;

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                }
                if(Module.Equals("Group Posting"))
                {
                try
                {
                    grvGroup_ViewSchedulerTask_CampaignScheduler.DataSource = null;
                    grvGroup_ViewSchedulerTask_CampaignScheduler.DataSource = dtCampaignSchedule;

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

        private void btnGroups_ViewScheduleTask_Start_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (ChkbGroup_ViewSchedulerTask_UniquePostPerGroup.Checked==true&&ChkbGroup_ViewSchedulerTask_ContinueProcess.Checked==true)
                    {
                          GlobusLogHelper.log.Debug("Please select any one :-1 Unique Post PerGroup OR 2- ContinueProcess");
                          GlobusLogHelper.log.Info("Please select any one :-1 Unique Post PerGroup OR 2- ContinueProcess");
                          return;
                    }


                    if (CmbGroup_ViewScheduleTask_StartProcessUsing.Text == "Delete All Camapigns")
                    {
                        try
                        {
                            objGroupCampaignManagerRepository.DeleteAll();
                            LoadCampaignData("Group Posting");
                            LoadCampaignData("Group Posting");
                            GlobusLogHelper.log.Debug("Deleted All Camapigns.");
                            GlobusLogHelper.log.Info("Deleted All Camapigns.");
                            dgvGroupRequestManager.Refresh();
                            grvGroup_ViewSchedulerTask_CampaignScheduler.Refresh();
                           
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        return;
                    }
                    if (CmbGroup_ViewScheduleTask_StartProcessUsing.Text == "Refresh")
                    {
                        try
                        {
                            LoadCampaignData("Group Posting");
                            dgvGroupRequestManager.Refresh();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        return;
                    }
                    if (CmbGroup_ViewScheduleTask_StartProcessUsing.Text == "Remove Selected Task")
                    {
                        try
                        {
                            if (grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows.Count > 0)
                            {                      
                                DataTable ds = (DataTable)grvGroup_ViewSchedulerTask_CampaignScheduler.DataSource;
                                string CampaignName = string.Empty;

                                foreach (DataGridViewRow row in grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows)
                                {
                                    try
                                    {
                                        if (row.Cells[0].Value != null)
                                        {
                                            CampaignName = row.Cells[0].Value.ToString();

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                              
                                DataRow dr = ds.Rows[0];
                              //  string CampaignName = dr.ItemArray[0].ToString();
                                string query = "delete from GroupCampaign where GroupCampaignName = '" + CampaignName + "'";
                                GroupCampaign ObjGroupCamp = new GroupCampaign();
                                ObjGroupCamp.GroupCampaignName = CampaignName;
                                ObjGroupCamp.Module = "Group Posting";
                                objGroupCampaignManagerRepository.DeleteSelectRows(ObjGroupCamp);
                                LoadCampaignData("Group Posting");
                                dgvGroupRequestManager.Refresh();
                                GlobusLogHelper.log.Debug("Deleted Selected Camapign.");
                                GlobusLogHelper.log.Info("Deleted Selected Camapign..");
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        return;
                    }

                    try
                    {
                        GroupManager.CheckTargetedGroupUrlsUse = Check_Group_ViewSchodulerTask_UseTargetedUrls.Checked;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (FBGlobals.listAccounts.Count > 0)
                    {
                        try
                        {
                            GroupManager.minDelayGroupManager = Convert.ToInt32(txtGroup_GroupManager_DelayMin.Text);
                            GroupManager.maxDelayGroupManager = Convert.ToInt32(txtGroup_GroupManager_DelayMax.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (ChkbGroup_ViewSchedulerTask_ContinueProcess.Checked)
                        {
                            objGroupManager.chkCountinueProcessGroupCamapinScheduler = true;
                        }

                        try
                        {
                            GroupManager.CheckGroupCompaignNoOfGroupsInBatch = Convert.ToInt32(txtGroupManager_GroupCompaign_NoOfGroupsInBatch.Text);
                            GroupManager.CheckGroupCompaign_InterbalInMinuts = Convert.ToInt32(txtGroupManager_GroupCompaign_IntervalMinute.Text);

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            GroupManager.ChkViewSchedulerTaskUniquePostPerGroup = ChkbGroup_ViewSchedulerTask_UniquePostPerGroup.Checked;

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        if (ChkbGroup_GrpRequestManager_MultiplePicPerGroup.Checked)
                        {
                            objGroupManager.ChkbGroupGrpRequestManagerMultiplePicPerGroup = true;
                        }


                        if (grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows.Count > 0)
                        {
                            try
                            {

                                faceboardpro.FbGroupCampaignManagerGlobals.Account = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["Account"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.PicFilePath = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["PicFilePath"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.VideoFilePath = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["VideoFilePath"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.MessageFilePath = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["MessageFilePath"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.ScheduleTime = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["ScheduleTime"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.cmpStartTime = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["cmpStartTime"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.Accomplish = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["Accomplish"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.GroupCampiagnName = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["GroupCampaignName"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.NoOfMessage = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["NoOfMessage"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.MessageMode = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["MessageMode"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.MessageType = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["MessageType"].Value.ToString();
                                faceboardpro.FbGroupCampaignManagerGlobals.TextMessage = grvGroup_ViewSchedulerTask_CampaignScheduler.SelectedRows[0].Cells["TextMessage"].Value.ToString();


                                if (!string.IsNullOrEmpty(faceboardpro.FbGroupCampaignManagerGlobals.PicFilePath))
                                {
                                    List<string> lstpicdata = new List<string>();
                                    string[] picsArray = Directory.GetFiles(faceboardpro.FbGroupCampaignManagerGlobals.PicFilePath);
                                    lstpicdata = picsArray.ToList();



                                    List<string> lstDistinctlstpicdata = new List<string>();

                                    foreach (string item in lstpicdata)
                                    {
                                        try
                                        {
                                            string items = item.ToLower();

                                            if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
                                            {
                                                lstDistinctlstpicdata.Add(item);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                    }
                                    objGroupManager.LstPicUrlsGroupCampaignManager = lstDistinctlstpicdata;
                                }

                                if (!string.IsNullOrEmpty(faceboardpro.FbGroupCampaignManagerGlobals.VideoFilePath))
                                {
                                    try
                                    {
                                        objGroupManager.LstVideoUrlsGroupCampaignManager = GlobusFileHelper.ReadFiletoStringList(faceboardpro.FbGroupCampaignManagerGlobals.VideoFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                }

                                if (!string.IsNullOrEmpty(faceboardpro.FbGroupCampaignManagerGlobals.MessageFilePath))
                                {
                                    if (chkSGroup_GroupManager_CreateSpinMessage.Checked)
                                    {                                      
                                        List<string> lstTemp = new List<string>();
                                        lstTemp = GlobusFileHelper.ReadFile(faceboardpro.FbGroupCampaignManagerGlobals.MessageFilePath).Distinct().ToList();
                                        objGroupManager.LstMessageUrlsGroupCampaignManager = new List<string>();
                                        foreach (string item in lstTemp)        
                                        {
                                            try
                                            {
                                                GlobusLogHelper.log.Info("Genrating Spinned Messages..");
                                                GlobusLogHelper.log.Debug("Genrating Spinned Messages..");
                                                LoadSpinMessagesPostScheduler(item);
                                                GlobusLogHelper.log.Info("Spinned Messages Loaded : " + objGroupManager.LstMessageUrlsGroupCampaignManager.Count);
                                                GlobusLogHelper.log.Debug("Spinned Messages Loaded : " + objGroupManager.LstMessageUrlsGroupCampaignManager.Count);

                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        
                                        //objGroupManager.LstMessageUrlsGroupCampaignManager.RemoveAt(0);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            objGroupManager.LstMessageUrlsGroupCampaignManager = GlobusFileHelper.ReadFiletoStringList(faceboardpro.FbGroupCampaignManagerGlobals.MessageFilePath);
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
                        else
                        {
                            MessageBox.Show("Please Select the one row in GridView !");
                            return;

                        }

                        try
                        {
                            GroupManager.ChkbGroupViewSchedulerTaskRemoveUrl = ChkbGroup_ViewSchedulerTask_RemoveUrl.Checked;
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }


                        objGroupManager.isStopGroupCamapinScheduler = false;
                        objGroupManager.lstThreadsGroupCamapinScheduler.Clear();

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;

                        if (!string.IsNullOrEmpty(txtGroups_ViewSchedulerTask_Threads.Text) && checkNo.IsMatch(txtGroups_ViewSchedulerTask_Threads.Text))
                        {
                            threads = Convert.ToInt32(txtGroups_ViewSchedulerTask_Threads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objGroupManager.NoOfThreadsGroupCamapinScheduler = threads;

                        Thread groupSchedulerTaskThread = new Thread(objGroupManager.StartGroupSchedulerTask);
                        groupSchedulerTaskThread.Start();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Please Load Accounts !");
                        GlobusLogHelper.log.Debug("Please Load Accounts !");

                        tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                        tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
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

        private void CmbGroups_GroupCampaignManager_EditCampaign_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                GroupCampaign objgroupCamp = new GroupCampaign();
                ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);


                string strCampaignName = string.Empty;
                try
                {
                    strCampaignName = CmbGroups_GroupCampaignManager_EditCampaign.Text;
                    TxtGroups_GroupCampaignManager_NewCampaign.Text = strCampaignName;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (!string.IsNullOrWhiteSpace(strCampaignName))
                {

                    if (CmbGroups_GroupCampaignManager_EditCampaign.Items.Count > 0)
                    {
                        foreach (GroupCampaign item in groupCollection)
                        {
                            if (item.GroupCampaignName == CmbGroups_GroupCampaignManager_EditCampaign.SelectedItem.ToString())
                            {

                                try
                                {
                                    lblGroups_GroupCampaignManager_PicURLsPath.Text = (item.PicFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    lblGroups_GroupCampaignManager_VideoURLsPath.Text = (item.VideoFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    lblGroups_GroupCampaignManager_MessagePath.Text = (item.MessageFilePath);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                // comboBox1.SelectedItem= (StringEncoderDecoder.Decode(item[3].ToString()));
                                string rdbutton = item.MessageType;
                                if (rdbutton == "Only Message")
                                {
                                    rdbGroup_GroupCampaignManager_Onlymessage.Checked = true;
                                }
                                else
                                {
                                    if (rdbutton == "Only Picture with message")
                                    {
                                        rdbGroup_GroupCampaignManager_OnlyPicWithMessage.Checked = true;
                                    }
                                    else
                                    {
                                        rdbGroup_GroupCampaignManager_Video.Checked = true;
                                    }
                                }
                                string str = item.MessageMode;
                                if (str == "Random Message")
                                {
                                    rdbGroup_GroupCampaignManager_SelectOneMessage.Checked = true;
                                }
                                else
                                {
                                    rdbGroup_GroupCampaignManager_RandomMessage.Checked = true;
                                }
                                TxtGroups_GroupCampaignManager_SingleMessage.Text = item.TextMessage;
                                // txtNameFile.Text = (StringEncoderDecoder.Decode(item[4].ToString()));
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Select Campaign Name From Edit Selected Compaign !");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_ViewScheduleTask_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objGroupManager.isStopGroupCamapinScheduler = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objGroupManager.lstThreadsGroupCamapinScheduler.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objGroupManager.lstThreadsGroupCamapinScheduler.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void chkScrapers_FansScraper_ExportFanpageUrl_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                string FilePath = string.Empty;
                string FilePath1 = string.Empty;


                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                string FilePathKeyword = FilePath;
                FilePath = FilePath + "\\FansScraperUrl.csv";

                string FilePath2 = FilePathKeyword;

                FilePath1 = FilePath2 + "\\FanPageScraperKeyword.csv";


                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                EventScraper.ScrapersFansScraperExprotFilePath = FilePath;
                PageManager.ScrapersExprotFilePath = FilePath1;
                PageManager.ScrapersFansScraperExprotFilePath = FilePath;
                GlobusFileHelper.DesktopPath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                FilePath = FilePath + "\\CreatedAccount.csv";
                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                GlobusFileHelper.DesktopPath = FilePath;
                AccountManager.AccountExprotFilePath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupRequestManager_LoadGuoup_Urls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupRequestManager_GroupURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        List<string> CheckValidList = new List<string>();
                        foreach (var lstTemp_item in lstTemp)
                        {
                            try
                            {
                                if (lstTemp_item.Contains("https://www.facebook.com/groups/")||lstTemp_item.Contains("http://www.facebook.com/groups/"))
                                {
                                    CheckValidList.Add(lstTemp_item);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Invalid Group Url : " + lstTemp_item);
                                    GlobusLogHelper.log.Debug("Invalid Group Url : " + lstTemp_item);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }


                        objGroupManager.LstGroupUrlsGroupRequestManager = CheckValidList.Distinct().ToList();
                        DateTime eTime = DateTime.Now;
                        lblGroups_GroupRequestManager_GroupURLsCount.Text = objGroupManager.LstGroupUrlsGroupRequestManager.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupRequestManager";
                        objSetting.FileType = "Groups_GroupRequestManager_LoadGroupURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group URLs Loaded : " + objGroupManager.LstGroupUrlsGroupRequestManager.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group URLs Loaded : " + objGroupManager.LstGroupUrlsGroupRequestManager + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupRequestManager_Start_Click(object sender, EventArgs e)
        {
            string GroupRequestManagerProcessUsing = string.Empty;
            try
            {
                if (CmbGroups_GroupRequestManager_StartProcessUsing.Text == "Delete All Camapigns")
                {
                    try
                    {
                        objGroupCampaignManagerRepository.DeleteAll();
                        LoadCampaignData("Group Request");
                        LoadCampaignData("Group Request");
                        GlobusLogHelper.log.Debug("Deleted All Camapigns.");
                        GlobusLogHelper.log.Info("Deleted All Camapigns.");
                        dgvGroupRequestManager.Refresh();
                        grvGroup_ViewSchedulerTask_CampaignScheduler.Refresh();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    return;
                }
                if (CmbGroups_GroupRequestManager_StartProcessUsing.Text == "Refresh")
                {
                    try
                    {
                        LoadCampaignData("Group Request");
                        dgvGroupRequestManager.Refresh();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    return;
                }
                if (CmbGroups_GroupRequestManager_StartProcessUsing.Text == "Remove Selected Task")
                {
                    try
                    {
                        if (dgvGroupRequestManager.SelectedRows.Count > 0)
                        {
                            DataTable ds = (DataTable)dgvGroupRequestManager.DataSource;
                            string CampaignName = string.Empty;

                            foreach (DataGridViewRow row in dgvGroupRequestManager.SelectedRows)
                            {
                                try
                                {
                                    if (row.Cells[0].Value != null)
                                    {
                                        CampaignName = row.Cells[0].Value.ToString();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }

                            DataRow dr = ds.Rows[0];
                            //  string CampaignName = dr.ItemArray[0].ToString();
                            string query = "delete from GroupCampaign where GroupCampaignName = '" + CampaignName + "'";
                            GroupCampaign ObjGroupCamp = new GroupCampaign();
                            ObjGroupCamp.GroupCampaignName = CampaignName;
                            ObjGroupCamp.Module = "Group Request";
                            objGroupCampaignManagerRepository.DeleteSelectRows(ObjGroupCamp);
                            LoadCampaignData("Group Request");
                            dgvGroupRequestManager.Refresh();
                            GlobusLogHelper.log.Debug("Deleted Selected Camapign.");
                            GlobusLogHelper.log.Info("Deleted Selected Camapign..");
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    return;
                }



                GroupManager.GroupRequestManagerNoOfGroupRequest = Convert.ToInt32(txtGroupRequestManagerNoOfGroupRequest.Text);

                



                if (FBGlobals.listAccounts.Count > 0)
                {
                    objGroupManager.isStopGroupGroupRequstManager = false;
                    objGroupManager.lstThreadsGroupGroupRequstManager.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;
                    if (string.IsNullOrEmpty(GlobusFileHelper.DesktopPath))
                    {

                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    int maxThread = 25 * processorCount;
                    try
                    {
                        GroupManager.minDelayGroupRequestManager = Convert.ToInt32(txtGroupManager_GroupRequestManager_DelayMin.Text);
                        GroupManager.maxDelayGroupRequestManager = Convert.ToInt32(txtGroupManager_GroupRequestManager_DelayMax.Text);

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtGroups_GroupRequestManager_Threads.Text) && checkNo.IsMatch(txtGroups_GroupRequestManager_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtGroups_GroupRequestManager_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    try
                    {
                        if (rdbGroupRequestSavedCampaigns.Checked)
                        {
                            objGroupManager.LstGroupUrlsGroupRequestManager=null;
                            GroupManager.GroupRequestCampaignName = dgvGroupRequestManager.SelectedRows[0].Cells["GroupCampaignName"].Value.ToString();
                            objGroupManager.LstGroupUrlsGroupRequestManager = GlobusFileHelper.ReadFile(dgvGroupRequestManager.SelectedRows[0].Cells["Group URLs File Path"].Value.ToString()).Distinct().ToList();
                            if (objGroupManager.LstGroupUrlsGroupRequestManager.Count < 1)
                            {
                                GlobusLogHelper.log.Info("Please upload Group URLs in Text file: " + dgvGroupRequestManager.SelectedRows[0].Cells["Group URLs File Path"].Value.ToString());
                                GlobusLogHelper.log.Info("Please upload Group URLs in Text file: " + dgvGroupRequestManager.SelectedRows[0].Cells["Group URLs File Path"].Value.ToString());
                                return;
                            }
                        }
                        if (objGroupManager.LstGroupUrlsGroupRequestManager == null || objGroupManager.LstGroupUrlsGroupRequestManager.Count<1)
                        {
                            GlobusLogHelper.log.Info("Please Load Group URLs  !");
                            GlobusLogHelper.log.Debug("Please Load Group URLs !");

                            return;
                        }
                        
                    }
                     catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    objGroupManager.NoOfThreadsGroupRequestManager = threads;

                    try
                    {
                        GroupManager.CheckGroupRequestManagerNoOfGroupsInBatch =Convert.ToInt32(txtGroupManager_GroupRequestManager_NoOfGroupsInBatch.Text);
                        GroupManager.CheckGroupRequestManager_InterbalInMinuts = Convert.ToInt32(txtGroupManager_GroupRequestManager_InterbalInminuts.Text);

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                    return;
                }
                if (ChkbGroup_GrpRequestManager_UniquePostPerGroup.Checked)
                {
                    objGroupManager.chkCountinueProcessGroupCamapinScheduler = true;
                }
                try
                {

                    GroupRequestManagerProcessUsing = CmbGroups_GroupRequestManager_StartProcessUsing.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                    GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                    MessageBox.Show("Please select Start Process Using drop down list.");
                    return;
                }
                
                if (GroupRequestManagerProcessUsing == "RequestSend Using GroupUrl File")
                {
                    objGroupManager.GroupRequestSendUsingGroupUrlFile = true;
                }

                Thread GroupRequestSendUsingGroupUrl = new Thread(objGroupManager.StartGroupRequesManager);
                GroupRequestSendUsingGroupUrl.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkGroup_GroupRequestManager_GroupUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkGroup_GroupRequestManager_GroupUrl.Checked)
            {
                return;
            }
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                if (!string.IsNullOrEmpty(FilePath))
                {
                    FilePath = FilePath + "\\GroupUrlsReport.csv";
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                    GroupManager.ExportLocationGroupRequest = FilePath;
                    GlobusFileHelper.DesktopPath = FilePath;
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Select Export Data File Path .!");
                    GlobusLogHelper.log.Debug("Please Select Export Data File Path .!");
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FriendsScraper_LoadProfileUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_FriendsScraper_LoadProfileURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendInfoScraper.LstProfileURLsFriendInfoScraper = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_FriendsScraper_LoadProfileURLsCount.Text = objFriendInfoScraper.LstProfileURLsFriendInfoScraper.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_FriendsScraper";
                        objSetting.FileType = "Scrapers_FriendsScraper_LoadProfileURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Profile URLs Loaded : " + objFriendInfoScraper.LstProfileURLsFriendInfoScraper.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Profile URLs Loaded : " + objFriendInfoScraper.LstProfileURLsFriendInfoScraper.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FriendsScraper_Extract_Click(object sender, EventArgs e)
        {

            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    if (objFriendInfoScraper.LstProfileURLsFriendInfoScraper==null)
                    {
                        GlobusLogHelper.log.Info("Please Load Profile URLs !");
                        GlobusLogHelper.log.Debug("Please Load Profile URLs !");

                        return;
                    }                  

                    objFriendInfoScraper.isStopFriendInfoScraper = false;
                    objFriendInfoScraper.lstThreadsFriendInfoScraper.Clear();

                    string startProcessUsing = string.Empty;

                    if (cmbScrapers_FriendsScraper_StartProcessUsing.SelectedIndex <= -1)
                    {
                        startProcessUsing = cmbScrapers_FriendsScraper_StartProcessUsing.Items[0].ToString();
                    }
                    else
                    {
                        startProcessUsing = cmbScrapers_FriendsScraper_StartProcessUsing.SelectedItem.ToString();

                    }
                    if (string.IsNullOrEmpty(FriendInfoScraper.ExportFilePathFriendInfoScraper))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }

                    FriendInfoScraper.StartProcessUsingFriendInfoScraper = startProcessUsing;

                    if (startProcessUsing == "Own Friends Info" && !string.IsNullOrEmpty(ownFriendsInfoFilePath))
                    {
                        FriendInfoScraper.ExportFilePathFriendInfoScraper = ownFriendsInfoFilePath;
                    }
                    if (startProcessUsing == "Friend Of Friends Info" && !string.IsNullOrEmpty(friendOfFriendsInfoFilePath))
                    {
                        FriendInfoScraper.ExportFilePathFriendInfoScraper = friendOfFriendsInfoFilePath;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_FriendsScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_FriendsScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_FriendsScraper_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objFriendInfoScraper.NoOfThreadsFriendInfoScraper = threads;
                    try
                    {
                        if (cmbScrapers_FriendsScraper_StartProcessUsing.SelectedIndex == -1)
                        {
                            GlobusLogHelper.log.Debug(" Please Select Start Process Using .");
                            GlobusLogHelper.log.Info(" Please Select Start Process Using .");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                    Thread extractFanPageIdsThread = new Thread(objFriendInfoScraper.StartFriendInfoScraper);
                    extractFanPageIdsThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FriendsScraper_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objFriendInfoScraper.isStopFriendInfoScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendInfoScraper.lstThreadsFriendInfoScraper.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendInfoScraper.lstThreadsFriendInfoScraper.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        string ownFriendsInfoFilePath = string.Empty;
        string friendOfFriendsInfoFilePath = string.Empty;

        private void chkScrapers_FriendsScraper_Export_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkScrapers_FriendsScraper_Export.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    ownFriendsInfoFilePath = FilePath + "\\OwnFriendsInfoScraper.csv";
                    friendOfFriendsInfoFilePath = FilePath + "\\FriendOfFriendsInfoScraper.csv";

                    FriendInfoScraper.ExportFilePathFriendInfoScraper = ownFriendsInfoFilePath;

                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                }
                else
                {
                    FriendInfoScraper.ExportFilePathFriendInfoScraper = string.Empty;
 
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFanPagesPoster__LoadUrls_Click(object sender, EventArgs e)
        {

            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;


                        lblPages_FanPagePoster_LoadFanPageUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstFanPageUrlCollectionFanPagePoster = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPage_FanPagePoster_LoadFanPageUrlsCount.Text = objPageManager.lstFanPageUrlCollectionFanPagePoster.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPagePoster";
                        objSetting.FileType = "Pages_FanPagePoster_LoadFanPageUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPagePoster.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPagePoster.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFanPagesPoster__LoadFanPagePostUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;


                        lblPages_FanPagePoster_LoadFanPagePostUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstFanPageUrlCollectionFanPagePostUrl = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPages_FanPagePoster_LoadFanPagePostUrlsCount.Text = objPageManager.lstFanPageUrlCollectionFanPagePostUrl.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPagePoster";
                        objSetting.FileType = "Pages_FanPagePoster_LoadFanPagePostUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Post Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPagePostUrl.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Post Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPagePostUrl.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnPage_FanPagePoster_start_Click(object sender, EventArgs e)
        {

            string FanPagePosterProcessUsing = string.Empty;
            try
            {
                PageManager.minDelayFanPagePoster = Convert.ToInt32(txtPage_FanPagePoster_DelayMin.Text);
                PageManager.maxDelayFanPagePoster = Convert.ToInt32(txtPage_FanPagePoster_DelayMax.Text);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                PageManager.StartProcessUsingFanPagePoster = cmbPages_FanPagePoster_StartFanPagePosterProcessUsing.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                return;
            }
           
             PageManager.noOfPicsPerURL = Convert.ToInt32(txtNoImagesPerURL.Text);
         
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        int NumberOfPost=Convert.ToInt32(txtNoImagesPerURL.Text);
                        if (NumberOfPost==0)
                        {
                            MessageBox.Show("Zero Number Of Post per Url Please fill number of post .");
                            GlobusLogHelper.log.Debug("Zero Number Of Post per Url Please fill number of post .");
                            GlobusLogHelper.log.Info("Zero Number Of Post per Url Please fill number of post .");
                            return;
                        }
                    


                        objPageManager.isStopFanPageUrlPoster = false;
                        objPageManager.lstThreadsFanPagePoster.Clear();

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;

                        if (!string.IsNullOrEmpty(txtPages_FanPagePoster_Threads.Text) && checkNo.IsMatch(txtPages_FanPagePoster_Threads.Text))
                        {
                            threads = Convert.ToInt32(txtPages_FanPagePoster_Threads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objPageManager.NoOfThreadsFanPagePoster = threads;

                        Thread FanPagePosterThread = new Thread(objPageManager.StartFanPagePostr);
                        FanPagePosterThread.Start();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void chkPage_FanPageUrlPoster_ExportAccount_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                FilePath = FilePath + "\\FanPageUrlPoster.csv";
                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                GlobusFileHelper.DesktopFanFilePath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void SortingTest()        
        {

        

        }


        private void btnWall_WallPoster_Start_Click_1(object sender, EventArgs e)
        {

           // SortingTest();
            try
            {


                if (FBGlobals.listAccounts.Count > 0)
                {
                    objWallPostManager.isStopWallPoster = false;
                    objWallPostManager.lstThreadsWallPoster.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        WallPostManager.minDelayWallPoster = Convert.ToInt32(txtWall_WallPoster_DelayMin.Text);
                        WallPostManager.maxDelayWallPoster = Convert.ToInt32(txtWall_WallPoster_DelayMax.Text);

                        WallPostManager.messageCountWallPoster = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (!string.IsNullOrEmpty(txtWall_PostPicOnWall_Threads.Text) && checkNo.IsMatch(txtWall_PostPicOnWall_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtWall_PostPicOnWall_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objWallPostManager.NoOfThreadsWallPoster = threads;

                    int noOfFriends = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);

                    try
                    {
                        WallPostManager.StartProcessUsingWallPoster = cmbWall_WallPoster_StartProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }
                    if (WallPostManager.StartProcessUsingWallPoster=="Spinned Message")
                    {
                        if (objWallPostManager.lstSpinnerWallMessageWallPoster.Count()<1)
                        {
                            GlobusLogHelper.log.Debug("Please Load Spinned Message.");
                            GlobusLogHelper.log.Info("Please Load Spinned Message.");
                            return;
                        }
                    }

                    if (WallPostManager.StartProcessUsingWallPoster == "Text Message")
                    {
                        if (objWallPostManager.lstWallMessageWallPoster.Count() < 1)
                        {
                            GlobusLogHelper.log.Debug("Please Load Text Message.");
                            GlobusLogHelper.log.Info("Please Load Text Message.");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txtWall_WallPoster_NoOfFriends.Text) && checkNo.IsMatch(txtWall_WallPoster_NoOfFriends.Text))
                    {
                        noOfFriends = Convert.ToInt32(txtWall_WallPoster_NoOfFriends.Text);
                        objWallPostManager.NoOfFriendsWallPoster = noOfFriends;
                    }
                    else
                    {
                        txtWall_WallPoster_NoOfFriends.Text = (5).ToString();
                        objWallPostManager.NoOfFriendsWallPoster = noOfFriends;
                    }

                    if (rdbWall_WallPoster_UseTextMessages.Checked)
                    {
                        if (objWallPostManager.lstWallMessageWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load Text Messages !");
                            GlobusLogHelper.log.Debug("Please Load Text Messages !");

                            MessageBox.Show("Please Load Text Messages !");
                            return;
                        }
                        objWallPostManager.IsUseTextMessageWallPoster = true;
                    }
                    else if (rdbWall_WallPoster_UseURLsMessages.Checked)
                    {
                        if (objWallPostManager.lstWallPostURLsWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load URLs Messages !");
                            GlobusLogHelper.log.Debug("Please Load URLs Messages !");

                            MessageBox.Show("Please Load URLs Messages !");
                            return;
                        }

                        objWallPostManager.IsUseURLsMessageWallPoster = true;
                        objWallPostManager.UseAllUrlWallPoster = true;
                    }
                    else if (rdbWall_WallPoster_UseSpinnedMessages.Checked)
                    {
                        if (objWallPostManager.lstSpinnerWallMessageWallPoster.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please Load Spinned Messages !");
                            GlobusLogHelper.log.Debug("Please Load Spinned Messages !");

                            MessageBox.Show("Please Load Spinned Messages !");
                            return;
                        }

                        objWallPostManager.ChkSpinnerWallMessaeWallPoster = true;
                    }
                    else
                    {
                    }

                    if (chk_Wall_WallPoster_RemoveURLsMessages.Checked)
                    {
                        objWallPostManager.chkWallWallPosterRemoveURLsMessages = true;
                    }
                    else
                    {
                        objWallPostManager.chkWallWallPosterRemoveURLsMessages = false;
                    }


                    string messagePostingMode = string.Empty;

                    try
                    {
                        messagePostingMode = cmbWall_WallPoster_MessagePostingMode.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Message Posting Mode drop down list.");
                        GlobusLogHelper.log.Info("Please select Message Posting Mode drop down list.");
                        return;
                    }

                    if (messagePostingMode == "Same Message Posting")
                    {
                        objWallPostManager.UseOneMsgToAllFriendsWallPoster = true;
                    }
                    else if (messagePostingMode == "Unique Message Posting")
                    {
                        objWallPostManager.UseUniqueMsgToAllFriendsWallPoster = true;
                    }
                    else if (messagePostingMode == "Random Message Posting")
                    {
                        objWallPostManager.UseRandomWallPoster = true;
                    }
                    else
                    {
                        objWallPostManager.UseRandomWallPoster = true;
                        //cmbPages_WallPoster_MessagePostingMode.SelectedIndex = 2;
                        //cmbPages_WallPoster_MessagePostingMode.SelectedItem = cmbPages_WallPoster_MessagePostingMode.SelectedIndex;
                    }

                    try
                    {
                        if (chkUniqueMessagePosting.Checked)
                        {
                            WallPostManager.IsUniqueMessagePosting = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread wallPosterThread = new Thread(objWallPostManager.StartWallPoster);
                    wallPosterThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnWall_WallPoster_LoadTextMessages_Click_1(object sender, EventArgs e)
        {
            rdbWall_WallPoster_UseTextMessages.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_WallPoster_TextMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objWallPostManager.lstWallMessageWallPoster = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblWall_WallPoster_TextMessagesCount.Text = objWallPostManager.lstWallMessageWallPoster.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Wall_WallPoster";
                        objSetting.FileType = "Wall_WallPoster_LoadTextMessages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Text Messages Loaded : " + objWallPostManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Text Messages Loaded : " + objWallPostManager.lstWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnWall_WallPoster_LoadURLsMessages_Click(object sender, EventArgs e)
        {

            rdbWall_WallPoster_UseURLsMessages.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_WallPoster_URLsMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objWallPostManager.lstWallPostURLsWallPoster = lstTemp.Distinct().ToList();
                        //lstWallPostURLsWallPoster

                        DateTime eTime = DateTime.Now;

                        lblWall_WallPoster_URLsMessagesCount.Text = objWallPostManager.lstWallPostURLsWallPoster.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Wall_WallPoster";
                        objSetting.FileType = "Wall_WallPoster_LoadURLsMessages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("URLs Messages Loaded : " + objWallPostManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("URLs Messages Loaded : " + objWallPostManager.lstWallPostURLsWallPoster.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnWall_WallPoster_LoadSpinnedMessages_Click(object sender, EventArgs e)
        {
            Thread ObjNew = new Thread(LoadSpinMessages);
            ObjNew.SetApartmentState(ApartmentState.STA);
            ObjNew.Start();
          //  LoadSpinMessages();

        }

        private void LoadSpinMessages()
        {

            rdbWall_WallPoster_UseSpinnedMessages.Invoke(new MethodInvoker(delegate
            {
                objWallPostManager.lstSpinnerWallMessageWallPoster.Clear();
                rdbWall_WallPoster_UseSpinnedMessages.Checked = true;
            }));



          
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_WallPoster_SpinnedMessagesPath.Invoke(new MethodInvoker(delegate
                        {
                            lblWall_WallPoster_SpinnedMessagesPath.Text = ofd.FileName;
                        }));


                        List<string> lstTemp = new List<string>();
                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        //  objWallPostManager.lstSpinnerWallMessageWallPoster = lstTemp.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        //   lblWall_WallPoster_SpinnedMessagesCount.Text = objWallPostManager.lstSpinnerWallMessageWallPoster.Count.ToString();

                        foreach (string item in lstTemp)
                        {
                            //List<string> lsttspin = Utils.GetSpinnedComments(item, '|');
                            // object obj=new object();
                            //obj=(object)item;
                            //Utils.GetStartSpinnedListItem(item);

                            //foreach (string item1 in Utils.list_SpinnedCreatorMessages)
                            //{
                            //    objWallPostManager.lstSpinnerWallMessageWallPoster.Add(item1);
                            //}
                            //  ThreadPool.QueueUserWorkItem(new WaitCallback(GenrateSpinMesaages), new object[] { item });
                            //  Thread.Sleep(500);
                            GenrateSpinMesaages(item);

                        }
                        lblWall_WallPoster_SpinnedMessagesCount.Invoke(new MethodInvoker(delegate
                        {
                            lblWall_WallPoster_SpinnedMessagesCount.Text = objWallPostManager.lstSpinnerWallMessageWallPoster.Count.ToString();
                        }));
                        

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Wall_WallPoster";
                        objSetting.FileType = "Wall_WallPoster_LoadSpinnedMessages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Spinned Messages Loaded : " + objWallPostManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Spinned Messages Loaded : " + objWallPostManager.lstSpinnerWallMessageWallPoster.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


      //  public void GenrateSpinMesaages(object Item)
        public void GenrateSpinMesaages(string Item)
        {
            // Array paramsArray = new object[2];
            // paramsArray = (Array)Item;
            //  string item = paramsArray.GetValue(0).ToString();
            string item = Item;
            Utils.GetStartSpinnedListItem(item);

            foreach (string item1 in Utils.list_SpinnedCreatorMessages)
            {
                objWallPostManager.lstSpinnerWallMessageWallPoster.Add(item1);
              
            }

        }
        private void btnWall_PostPicOnWall_Start_Click(object sender, EventArgs e)
        {

            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objWallPostManager.isStopPostPicOnWall = false;
                    objWallPostManager.lstThreadsPostPicOnWall.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtWall_PostPicOnWall_Threads.Text) && checkNo.IsMatch(txtWall_PostPicOnWall_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtWall_PostPicOnWall_Threads.Text);
                    }
                    try
                    {
                        WallPostManager.minDelayPostPicOnWal = Convert.ToInt32(txtPostPic_PostPicOnWal_DelayMin.Text);
                        WallPostManager.maxDelayPostPicOnWal = Convert.ToInt32(txtPostPic_PostPicOnWal_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        WallPostManager.NumberOfFriendsSendPicOnWall = Convert.ToInt32(txtWall_WallPosterPic_NoOfFriends.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        WallPostManager.StartProcessUsingPostPicOnWall = cmbWall_PostPicOnWall_Mode.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }

                    try
                    {
                        if (chkUniquePicsPosting.Checked)
                        {
                            WallPostManager.IsUniquePicPosting = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objWallPostManager.NoOfThreadsPostPicOnWall = threads;


                    if (objWallPostManager.lstPicturecollectionPostPicOnWall.Count < 1 && WallPostManager.StartProcessUsingPostPicOnWall != "Share Video " && WallPostManager.StartProcessUsingPostPicOnWall != "Share Image" && WallPostManager.StartProcessUsingPostPicOnWall != "Spam Video" && WallPostManager.StartProcessUsingPostPicOnWall == "Post Pic On Wall")
                    {
                        GlobusLogHelper.log.Info("Please Load Pics Folder !");
                        GlobusLogHelper.log.Debug("Please Load Pics Folder !");
                        return;
                    }
                    if ((WallPostManager.StartProcessUsingPostPicOnWall == "Share Video "))
                    {
                        if (objWallPostManager.lstWallPostShareLoadTargedUrls.Count==0)
                        {
                            GlobusLogHelper.log.Info("Please Load Targeted Urls !");
                            GlobusLogHelper.log.Debug("Please Load Targeted Urls !");
                             return; 
                        }
                    }
                    if (ChkbWall_PostPicOnWall_ContinueShareProcess.Checked)
                    {
                        objWallPostManager.chkCountinueProcessContinueShareProcess = true;
                    }
                    if (chkWall_PostPicOnWall_ShareVideoOnlyMe.Checked)
                    {
                        objWallPostManager.chkWall_PostPicOnWall_ShareVideoOnlyMe = true;
                    }
                    

                    if ((WallPostManager.StartProcessUsingPostPicOnWall == "Share Image"))
                    {
                        if (objWallPostManager.lstWallPostShareLoadTargedUrls.Count == 0)
                        {
                            GlobusLogHelper.log.Info("Please Load Targeted Urls !");
                            GlobusLogHelper.log.Debug("Please Load Targeted Urls !");
                            return;
                        }
                    }
                    if (chkWall_PostPicOnWall_UseAllPics.Checked)
                    {
                        objWallPostManager.IsPostAllPicPostPicOnWall = true;
                    }
                    else
                    {
                        objWallPostManager.IsPostAllPicPostPicOnWall = false;
                    }

                    if (chkWall_PostPicOnWall_WithMessage.Checked)
                    {
                        objWallPostManager.chkWallPostPicOnWallWithMessage = true;
                    }


                    Thread postPicOnWallThread = new Thread(objWallPostManager.StarPostPicOnWall);
                    postPicOnWallThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btnWall_PostPicOnWall_LoadGreetingMessages_Click(object sender, EventArgs e)
        {

            chkWall_PostPicOnWall_WithMessage.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_PostPicOnWall_LoadGreetingMessagesPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        string temp = GlobusFileHelper.ReadStringFromTextfile(ofd.FileName);

                        objWallPostManager.lstMessageCollectionPostPicOnWall = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblWall_PostPicOnWall_LoadGreetingMessagesCount.Text = objWallPostManager.lstMessageCollectionPostPicOnWall.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Wall_PostPicOnWall";
                        objSetting.FileType = "Wall_PostPicOnWall_LoadGreetingMessages";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Greeting Messages Loaded : " + objWallPostManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Greeting Messages Loaded : " + objWallPostManager.lstMessageCollectionPostPicOnWall.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnWall_PostPicOnWall_LoadPicsFolder_Click(object sender, EventArgs e)
        {

            try
            {
                List<string> lstWallPics = new List<string>();
                List<string> lstCorrectWallPics = new List<string>();

                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        lblWall_PostPicOnWall_LoadPicsFolderImagePath.Text = ofd.SelectedPath;
                        lstWallPics.Clear();
                        lstCorrectWallPics.Clear();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstWallPics = picsArray.Distinct().ToList();
                        string PicFilepath = ofd.SelectedPath;
                        foreach (string item in lstWallPics)
                        {
                            try
                            {
                                string items = item.ToLower();
                                if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
                                {
                                    lstCorrectWallPics.Add(item);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Wrong File Is :" + item);
                                    GlobusLogHelper.log.Debug("Wrong File Is :" + item);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        lstCorrectWallPics = lstCorrectWallPics.Distinct().ToList();
                        objWallPostManager.lstPicturecollectionPostPicOnWall = lstCorrectWallPics;
                        lblWall_PostPicOnWall_LoadPicsFolderImageCount.Text = objWallPostManager.lstPicturecollectionPostPicOnWall.Count.ToString();

                        GlobusLogHelper.log.Info(lstCorrectWallPics.Count + "  Pics loaded");
                        GlobusLogHelper.log.Debug(lstCorrectWallPics.Count + "  Pics loaded");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


        }

        private void btnFanPagePosterStop_start_Click(object sender, EventArgs e)
        {
            try
            {

                objPageManager.isStopFanPageUrlPoster = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsFanPagePoster.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsFanPagePoster.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                objPageManager.isStopFanPageCommentLiker = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsCommentLiker.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsCommentLiker.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnFanPagesPoster__LoadPicsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> lstFanPagePics = new List<string>();
                List<string> lstCorrectFanPagePics = new List<string>();

                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        lblPages_FanPagePoster_LoadFanPagePicsPath.Text = ofd.SelectedPath;
                        lstFanPagePics.Clear();
                        lstCorrectFanPagePics.Clear();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstFanPagePics = picsArray.Distinct().ToList();
                        string PicFilepath = ofd.SelectedPath;
                        foreach (string item in lstFanPagePics)
                        {
                            try
                            {
                                string items = item.ToLower();
                                if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
                                {
                                    lstCorrectFanPagePics.Add(item);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Wrong File Is :" + item);
                                    GlobusLogHelper.log.Debug("Wrong File Is :" + item);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        lstCorrectFanPagePics = lstCorrectFanPagePics.Distinct().ToList();
                        objPageManager.lstPicturecollectionPostPicOnFanPageWall = lstCorrectFanPagePics;
                        lblPages_FanPagePoster_LoadFanPagePicsCount.Text = objPageManager.lstPicturecollectionPostPicOnFanPageWall.Count.ToString();

                        GlobusLogHelper.log.Info(lstCorrectFanPagePics.Count + "  Pics loaded");
                        GlobusLogHelper.log.Debug(lstCorrectFanPagePics.Count + "  Pics loaded");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFanPagesPoster__LoadTextMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FanPagePoster_LoadFanPagePostMessagePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objPageManager.lstFanPageCollectionFanPagePosterMessage = lstTemp.Distinct().ToList();
                        objPageManager.lstFanPageUrlCollectionFanPagePostUrl = lstTemp.Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        lblPages_FanPagePoster_LoadFanPageMessageCount.Text = objPageManager.lstFanPageCollectionFanPagePosterMessage.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPagePoster";
                        objSetting.FileType = "Pages_FanPagePoster_LoadFanPageMessage";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Urls Loaded : " + objPageManager.lstFanPageCollectionFanPagePosterMessage.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Urls Loaded : " + objPageManager.lstFanPageCollectionFanPagePosterMessage.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btn_Groups_GroupSearch_StartScraping_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objEventScraper.isStopEventScraper = false;
                    objEventScraper.lstThreadsEventScraper.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtSearch_GroupSearch_Threads.Text) && checkNo.IsMatch(txtSearch_GroupSearch_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtSearch_GroupSearch_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objSearchScraper.NoOfThreadsSearchScraper = threads;

                    if (!string.IsNullOrEmpty(txtSearch_GorupSearch_Name.Text))
                    {
                        objSearchScraper.SearchGroup_Name = txtSearch_GorupSearch_Name.Text;
                    }
                    else
                    {
                        objSearchScraper.SearchGroup_Name = "all";
                    }

                    if (cmbSearch_GorupSearch_MemberShip.SelectedItem != null)
                    {
                        objSearchScraper.SearchGroup_Membership = cmbSearch_GorupSearch_MemberShip.SelectedItem.ToString();
                    }
                    else
                    {
                        objSearchScraper.SearchGroup_Membership = "all";
                    }

                    if (cmbSearch_GorupSearch_Privacy.SelectedItem != null)
                    {
                        objSearchScraper.SearchGroup_Privacy = cmbSearch_GorupSearch_Privacy.SelectedItem.ToString();
                    }
                    else
                    {
                        objSearchScraper.SearchGroup_Privacy = "all";
                    }

                    if (cmbSearch_GorupSearch_About.SelectedItem != null)
                    {
                        objSearchScraper.SearchGroup_About = cmbSearch_GorupSearch_About.SelectedItem.ToString();
                    }
                    else
                    {
                        objSearchScraper.SearchGroup_About = "all";
                    }

                    Thread createProfileThread = new Thread(objSearchScraper.StartSearchScraper);
                    createProfileThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void grvMessages_MessageReply_MessageDetails_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            foreach (DataGridViewRow row in grvMessages_MessageReply_MessageDetails.Rows)
            {
                try
                {
                    if (row.Cells[0].Value != null && Convert.ToBoolean(row.Cells[0].Value) == true)
                    {
                        objMessageManager.LstReplyDetailsMessageReply.Add("<UserName>" + row.Cells[1].Value.ToString() + ":" + "<MessageFriendId>" + row.Cells[2].Value.ToString() + ":" + "<MessageSnippedId>" + row.Cells[3].Value.ToString() + ":" + "<MessageSenderName>" + row.Cells[4].Value.ToString() + ":" + "<MessagingReadParticipants>" + row.Cells[5].Value.ToString());
                        txtMessages_Message_MessageBox.Text = row.Cells[6].Value.ToString();

                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void btnMessages_MessageReply_LoadProfileUrl_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblMessages_MessageReply_ProfileUrlPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        MessageManager.MessageMessageLoadProfileUrl = lstTemp.Distinct().ToList();
                        foreach (string item in MessageManager.MessageMessageLoadProfileUrl)
                        {
                            MessageManager.MessageMessageLoadProfileUrlQueue.Enqueue(item);
                        }      
       

                        DateTime eTime = DateTime.Now;

                        lblMessages_MessageTargated_ProfileIDCount.Text = lstTemp.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Messages_ReplyURL";
                        objSetting.FileType = "Messages_ReplyURL_LoadProfileUrl";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Reply URL Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Reply URL Loaded : " + lstTemp.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public class StringEncoderDecoder
        {
            public static string Encode(string data)
            {
                try
                {
                    string encoded = data.Replace("'", "^");
                    return encoded;
                }
                catch { return ""; }
            }

            public static string Decode(string data)
            {
                try
                {
                    string decoded = data.Replace("^", "'");
                    return decoded;
                }
                catch { return ""; }
            }
        }

        private void btnGroups_GroupRequestManager_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objGroupManager.isStopGroupGroupRequstManager = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objGroupManager.lstThreadsGroupGroupRequstManager.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objGroupManager.lstThreadsGroupGroupRequstManager.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnGroups_GroupRequestManager_LoadSearchGroupKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupRequestManager_LoadSearchGroupKeywordPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        objGroupManager.LstGroupUrlsGroupRequestManager = lstTemp.Distinct().ToList();
                        DateTime eTime = DateTime.Now;
                        lblGroups_GroupRequestManager_LoadSearchGroupKeywordCount.Text = objGroupManager.LstGroupUrlsGroupRequestManager.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupRequestManager";
                        objSetting.FileType = "Groups_GroupRequestManager_LoadSearchGroupKeywords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Event Keywords Loaded : " + objGroupManager.LstGroupUrlsGroupRequestManager.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Event Keywords Loaded : " + objGroupManager.LstGroupUrlsGroupRequestManager + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Check GroupCompaignReportStatus
        /// </summary>
        /// <param name="GroupUrl"></param>
        /// <param name="message"></param>
        /// <returns></returns>


        public bool GroupCompaignReport(string GroupUrl, string message)
        {
            bool checkStatus = false;

            try
            {

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return checkStatus;
        }

        private void btnPages_CommentLikerTargeted_LoadUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_CommentLikerTargeted_LoadFanPageUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstFanPageUrlTargedCollectionCommentLiker = lstTemp.Distinct().ToList();
                   

                        DateTime eTime = DateTime.Now;

                        lblPages_CommentLikerTargeted_LoadMessageCount.Text = objPageManager.lstFanPageUrlTargedCollectionCommentLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_CommentLikerTargeted";
                        objSetting.FileType = "Pages_CommentLikerTargeted_LoadFanPageUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Targeted Comment Liker Loaded : " + objPageManager.lstFanPageUrlTargedCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Targeted Comment Liker Loaded : " + objPageManager.lstFanPageUrlTargedCollectionCommentLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkSGroup_GroupManager_ExportGroupUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkSGroup_GroupManager_ExportGroupUrl.Checked)
            {
                return;
            }
            try
            {
                if (chkSGroup_GroupManager_ExportGroupUrl.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    if (!string.IsNullOrEmpty(FilePath))
                    {
                        FilePath = FilePath + "\\GroupReport.csv";
                        GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                        GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                        GroupManager.GroupReportExprotFilePath = FilePath;
                        GlobusFileHelper.DesktopPath = FilePath;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Please Select Export Data File Path .!");
                        GlobusLogHelper.log.Debug("Please Select Export Data File Path .!");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
      
        string GroupSearchFilePath = string.Empty;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkSearch_GroupSearch_Export.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    GroupSearchFilePath = FilePath + "\\GroupSearch.csv";


                    SearchScraper.exportFilePathAccountVerification = FilePath;

                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void FrmDominator_FormClosed(object sender, FormClosedEventArgs e)
        {

            try
            {
                var prc = System.Diagnostics.Process.GetProcesses();
                foreach (var item in prc)
                {
                    try
                    {
                        if (item.ProcessName.Contains("FD_LicensingManager"))
                        {
                            item.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                Application.ExitThread();
                Application.Exit();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void DeleteAccounts()
        {
            objSetting.Module = "Accounts_ManageAccounts";
            objSetting.FileType = "Accounts_ManageAccounts_LoadAccounts";
            objSettingRepository.DeleteAccounts(objSetting);

            try
            {
                lblaccounts_ManageAccounts_LoadsAccountsCount.Invoke(new MethodInvoker(delegate
                {
                    lblaccounts_ManageAccounts_LoadsAccountsCount.Text = null;
                    lblAccounts_ManageAccounts_LoadsAccountsPath.Text = null;
                }));

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                if (cmbScraper__CustomAudiencesScraper_Accounts.InvokeRequired)
                {
                    cmbScraper__CustomAudiencesScraper_Accounts.Invoke(new MethodInvoker(delegate
                    {
                        cmbScraper__CustomAudiencesScraper_Accounts.Items.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            

            try
            {
                if (cmbScraper__fanscraper_Accounts.InvokeRequired)
                {
                    cmbScraper__fanscraper_Accounts.Invoke(new MethodInvoker(delegate
                    {
                        cmbScraper__fanscraper_Accounts.Items.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                if (cmbScraper__GroupMemberScraper_Accounts.InvokeRequired)
                {
                    cmbScraper__GroupMemberScraper_Accounts.Invoke(new MethodInvoker(delegate
                    {
                        cmbScraper__GroupMemberScraper_Accounts.Items.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            try
            {
                if (cmbGroups_GroupCampaignManager_Accounts.InvokeRequired)
                {
                    cmbGroups_GroupCampaignManager_Accounts.Invoke(new MethodInvoker(delegate
                    {
                        cmbGroups_GroupCampaignManager_Accounts.Items.Clear();
                    }));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
           

            GlobusLogHelper.log.Info("All Accounts Deleted ");
            GlobusLogHelper.log.Debug("All Accounts Deleted ");
            
        }

        private void btnAccounts_ManageAccounts_DeleteAccounts_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("Do you really want to Delete accounts from database ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Thread DeleteAccount = new Thread(DeleteAccounts);
                    DeleteAccount.Start();
                    grvAccounts_ManageAccounts_ManageAccountsDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            //Delete Main Accounts
          

        }

        private void btnScrapers_FBIDextractor_ExtractFBID_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objFriendInfoScraper.LstFBIDExtractorList.Count < 1)
                        {
                            GlobusLogHelper.log.Info("Please FBID File !");
                            GlobusLogHelper.log.Debug("Please FBID File !");

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    objFriendInfoScraper.isStopFBIDExtracter = false;
                    objFriendInfoScraper.lstThreadsisStopFBIDExtracter.Clear();

                 

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();
                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_FBIDScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_FBIDScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_FBIDScraper_Threads.Text);
                    }
                    if (string.IsNullOrEmpty(FriendInfoScraper.ExportFilePathFBIDExtractor))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }

                    try
                    {
                        objFriendInfoScraper.FBIDExtracterOnlyTargetedID = chkFBIDExtracter_FBIDExtracter_OnlyTargetedID.Checked;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        FriendInfoScraper.FBIDExtractorProsessUsing = cmbScrapers_FBIDScraper_StartProcessUsing.SelectedItem.ToString();
                    }
                     catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (FriendInfoScraper.FBIDExtractorProsessUsing == "ID Scraper")
                    {
                        if (objFriendInfoScraper.LstFBProfileUrlList == null)
                        {
                            GlobusLogHelper.log.Debug("Please Upload Profile Urls");
                            GlobusLogHelper.log.Info("Please Upload Profile Urls");
                            return;
                        }
                    }


                    objFriendInfoScraper.NoOfThreadsFriendInfoScraper = threads;

                    Thread extractFanPageIdsThread = new Thread(objFriendInfoScraper.StartExtracter);
                    extractFanPageIdsThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FBIDExtracter_IDFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_FBIDExtractor_LoadFBIDPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendInfoScraper.LstFBIDExtractorList = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_FBIDExtracter_LoadFBIDCount.Text = objFriendInfoScraper.LstFBIDExtractorList.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database

                        objSetting.Module = "Scrapers_FBIDExtracter";
                        objSetting.FileType = "Scrapers_FBIDExtracter_FBIDfile";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("FBID Loaded : " + objFriendInfoScraper.LstFBIDExtractorList.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("FBID Loaded : " + objFriendInfoScraper.LstFBIDExtractorList.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        string FBIDExtracterFilePath = string.Empty;
        string FBIDExtracterInfoFilePath = string.Empty;

        private void chkFBIDExtracter_FBIDExtracter_Export_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkFBIDExtracter_FBIDExtracter_Export.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    FBIDExtracterFilePath = FilePath + "\\FBIDExtracter.csv";
                    FBIDExtracterInfoFilePath = FilePath + "\\FBIDExtracterInfo.csv";

                    FriendInfoScraper.ExportFilePathFBIDExtractor = FBIDExtracterInfoFilePath;

                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FBIDScraper_Stop_Click(object sender, EventArgs e)
        {

            try
            {
                objFriendInfoScraper.isStopFBIDExtracter = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendInfoScraper.lstThreadsisStopFBIDExtracter.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendInfoScraper.lstThreadsisStopFBIDExtracter.Remove(item);
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

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }
        
        private void btnScrapers_GroupMemberScraper_LoadGroupUrls_Click(object sender, EventArgs e)
        {

            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_GroupMemberScraper_LoadGroupURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendInfoScraper.LstGroupURLsFriendInfoScraper = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_GroupMemberScraper_LoadGroupURLsCount.Text = objFriendInfoScraper.LstGroupURLsFriendInfoScraper.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_GroupMemberScraper";
                        objSetting.FileType = "Scrapers_GroupMemberScraper_LoadGroupURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group URLs Loaded : " + objFriendInfoScraper.LstGroupURLsFriendInfoScraper.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group URLs Loaded : " + objFriendInfoScraper.LstGroupURLsFriendInfoScraper.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_GroupMemberScraper_Extract_Click(object sender, EventArgs e)
        {

            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {

                        //objFriendInfoScraper.LstGroupURLsFriendInfoScraper.Count < 1 ||
                        if (objFriendInfoScraper.LstGroupURLsFriendInfoScraper == null && cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.SelectedItem.ToString() == "Group member Scraper by Urls")
                        {
                            GlobusLogHelper.log.Info("Please Load Group URLs  !");
                            GlobusLogHelper.log.Debug("Please Load Group URLs !");

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (string.IsNullOrEmpty(FriendInfoScraper.ExportFilePathGroupMemberScraper))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }


                    try
                    {
                        FriendInfoScraper.GroupMemberScraperUsingAccount = cmbScraper__GroupMemberScraper_Accounts.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                   

                    try
                    {
                        FriendInfoScraper.CheckScrapeCloseGroupUrlsScraper=chkScrapers_GroupMemberScraper_CloseGrp.Checked;
                        FriendInfoScraper.CheckScrapeOpenGroupUrlsScraper = chkScrapers_GroupMemberScraper_OpenGrp.Checked;
                    }
                   catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {

                        //objFriendInfoScraper.LstGroupURLsFriendInfoScraper.Count < 1 ||
                        if (objFriendInfoScraper.LstOfGroupKeywords == null && cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.SelectedItem.ToString() == "Group url Scraper  by Keywords")
                        {
                            GlobusLogHelper.log.Info("Please Load Group Keywords  !");
                            GlobusLogHelper.log.Debug("Please Load Group Keywords !");

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                 
                    objFriendInfoScraper.isStopGroupMemberScraper = false;
                    objFriendInfoScraper.lstThreadsGroupMemberScraper.Clear();

                    try
                    {
                        objFriendInfoScraper.GroupUrlScraperCheckMembersMin = Convert.ToInt32(txtScrapers_GroupMemberScraper_CheckMembersMin.Text);
                        objFriendInfoScraper.GroupUrlScraperCheckMembersMax = Convert.ToInt32(txtScrapers_GroupMemberScraper_CheckMembersMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    string startProcessUsing = string.Empty;

                    #region Commemt ProcessUsingCode

                    try
                    {

                        FriendInfoScraper.StartProcessUsingGroupMemberScraper = cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.SelectedItem.ToString();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }

                    //if (cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.SelectedIndex <= -1)
                    //{
                    //    startProcessUsing = cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.Items[0].ToString();
                    //}
                    //else
                    //{
                    //    startProcessUsing = cmbScrapers_GroupMemberScraperScraper_StartProcessUsing.SelectedItem.ToString();

                    //}

                    #endregion
                    
                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_GroupMemberScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_GroupMemberScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_GroupMemberScraper_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objFriendInfoScraper.NoOfThreadsGroupMemberScraper = threads;

                    Thread extractGroupMemberThread = new Thread(objFriendInfoScraper.StartGroupMemberScraper);
                    extractGroupMemberThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        string ownGroupMemberInfoFilePath = string.Empty;
        string ownGroupMemberInfoFilePathTxt = string.Empty;

        private void chkScrapers_GroupMemberScraper_Export_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkScrapers_GroupMemberScraper_Export.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    string FilePath1 = string.Empty;
                    string ByKeywords = FilePath;

                    ownGroupMemberInfoFilePath = FilePath + "\\GroupInfoScraperByUrl.csv";
                    ownGroupMemberInfoFilePathTxt = FilePath + "\\GroupInfoScraper.txt";
                    ByKeywords = ByKeywords + "\\GroupInfoScraperByKeyWords.csv";
                    #region MyRegion
                    //ownGroupUrlFilePath =FilePath1 + "\\GroupUrlScraper.txt";
                    //try
                    //{
                    //    string Txt_Content = "GroupUrl\t\t\t" + "," + "Groupkeyword\t\t\t" + ", " + "GroupTypes\t\t\t" + "," + "NumberOfMember";
                    //    Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ownGroupMemberInfoFilePathTxt);
                    //}
                    //catch (Exception ex)
                    //{
                    //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    //} 
                    #endregion
                    FriendInfoScraper.ExportFilePathGroupMemberScraper = ownGroupMemberInfoFilePath;
                    FriendInfoScraper.ExportFilePathGroupMemberScraperByKeyWords = ByKeywords;

                    //FriendInfoScraper.ExportFilePathGroupMemberScraperTxt = ownGroupMemberInfoFilePathTxt;

                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath); 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_GroupMemberScraper_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objFriendInfoScraper.isStopGroupMemberScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendInfoScraper.lstThreadsGroupMemberScraper.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendInfoScraper.lstThreadsGroupMemberScraper.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnGroups_DeleteGroupPost_Start_Click(object sender, EventArgs e)
        {
              
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objGroupManager.isStopDeleteGroupPost = false;
                    objGroupManager.lstThreadsDeleteGroupPost.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtGroups_DeleteGroupPost_Threads.Text) && checkNo.IsMatch(txtGroups_DeleteGroupPost_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtGroups_DeleteGroupPost_Threads.Text);
                    }

                    try
                    {

                        GroupManager.strGroup_DeleteCommentPostProcessUsing = CmbGroups_GroupDeletePost_StartProcessUsing.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select  Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select  Process Using drop down list.");
                        return;
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                  //  objGroupManager.NoOfThreadsGroupInviter = threads;

                    if (GroupManager.DeleteScheduleGroupPosting)
                    {
                        try
                        {
                            GroupManager.DeleteNUmberOfPost = Convert.ToInt32(txtGroup_DeleteGroupPost_NoOfGroupPost.Text);
                            GroupManager.IntervalTime = Convert.ToInt32(txtGroup_DeleteGroupPost_IntervalTime.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                       
                    }

                    try
                    {
                        GroupManager.CheckContinueProcess = chk_ContinueProcessGrpCommentPost.Checked;
                    }
                       catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {

                        GroupManager.minDelayDeleteGroupPost = Convert.ToInt32(txtGroup_DeleteGroupPost_DelayMin.Text);
                        GroupManager.maxDelayDeleteGroupPost = Convert.ToInt32(txtGroup_DeleteGroupPost_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread groupDeletePostThread = new Thread(objGroupManager.StartDeletePostRequuest);
                    groupDeletePostThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        
        }

        private void btnGroups_DeleteGroupPost_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objGroupManager.isStopDeleteGroupPost = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objGroupManager.lstThreadsDeleteGroupPost.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objGroupManager.lstThreadsDeleteGroupPost.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnAccounts_EditProfileName_LoadProfileName_Click(object sender, EventArgs e)
        {
            
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {


                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblaccounts_EditProfileName_Namepath.Text = ofd.FileName;
                        lstLoadedNames.Clear();
                        LoadEditName(ofd.FileName);

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblaccounts_EditAccountName_EditProfileName.Text = lstLoadedNames.Count.ToString();
                        AccountManager.EditProfileName = lstLoadedNames;

                        //Insert Seeting Into Database

                        objSetting.Module = "Accounts_EditProfileName";
                        objSetting.FileType = "Accounts_EditProfileName_LoadNames";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("Names Loaded : " + lstLoadedNames.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Names Loaded : " + lstLoadedNames.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);            

            }      
        }

        private void LoadEditName(string emailFile)
        {

            try
            {
                lstLoadedNames = GlobusFileHelper.ReadFile(emailFile);
                lstLoadedNames = lstLoadedNames.Distinct().ToList();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);             
            }
        }

        private void btn_Account_EditProfileName_Start_Click(object sender, EventArgs e)
        {
            string AccountEditProfileNameProcessUsing = string.Empty;
            try
            {
                ObjAccountManager.isStopEditProfileName = false;
                ObjAccountManager.lstEditProfileNameThread.Clear();

                Regex checkNo = new Regex("^[0-9]*$");

                int processorCount = objUtils.GetProcessor();    //Environment.ProcessorCount;

                int threads = 25;

                int maxThread = 25 * processorCount;

                try
                {
                    AccountEditProfileNameProcessUsing = DdAccountEditProfileNameProcessUsing.SelectedItem.ToString();
                    AccountManager.ProcessUsing = DdAccountEditProfileNameProcessUsing.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                    GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                    return;
                }

                int ThreadCount = Convert.ToInt32(txtAccount_EditProfileName_Threads.Text);
                if (ThreadCount == 0)
                {
                    GlobusLogHelper.log.Info("Please Enter More than 0 thread ..");
                    GlobusLogHelper.log.Debug("Please Enter More than 0 thread ..");
                    return;
                }

                //if (!string.IsNullOrEmpty(txtAccount_EditProfileName_Threads.Text) && checkNo.IsMatch(txtAccounts_AccountCreator_Threads.Text))
                //{
                //    threads = Convert.ToInt32(txtAccount_EditProfileName_Threads.Text);
                //}

                try
                {
                    AccountManager.ChangeLanguageEditProfile = AccountEditProfileNameChooseLanguage.SelectedItem.ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


           //     if (lstLoadedNames.Count() > 0)
                {                  
                        try
                        {
                            Thread threadStartEditProfileName = new Thread(ObjAccountManager.StartEditProfileName);
                            threadStartEditProfileName.SetApartmentState(ApartmentState.STA);
                            threadStartEditProfileName.Start();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                   
                }
              //  else
                //{
                //    GlobusLogHelper.log.Debug("Please Load Profile Name .");
                //    GlobusLogHelper.log.Info("Please Load Profile Name.");
                //}
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btn_Account_EditProfileName_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                Thread threadStopAccountVerification = new Thread(ObjAccountManager.StopEditAccountName);
                threadStopAccountVerification.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FanScraper_LoadFanPageKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScraper_FanPageKeyword_FanPageKeywordPath.Text = ofd.FileName;
                        objPageManager.lstFanPageKeywords.Clear();
                        objPageManager.lstFanPageKeywords = GlobusFileHelper.ReadFile(ofd.FileName);
                        objPageManager.lstFanPageKeywords = objPageManager.lstFanPageKeywords.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        lblScraper_FanPageKeyword_FanPageKeywordCount.Text = objPageManager.lstFanPageKeywords.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_FanPageScraper";
                        objSetting.FileType = "Scrapers_FanPageScraper_LoadFanPageKeywords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("Fan Page Keywords Loaded : " + objPageManager.lstFanPageKeywords.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Fan Page Keywords Loaded : " + objPageManager.lstFanPageKeywords.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        string ProxiesInfoFilePath = string.Empty;

        private void chkProxies_ProxyChecker_ExportProxies_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkProxies_ProxyChecker_ExportProxies.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    ProxiesInfoFilePath = FilePath + "\\Proxies.csv";
                    ProxyManager.ExportFilePathProxies = ProxiesInfoFilePath;
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_DeleteGroupPost_LoadTargeted_Urls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_DeleteGroupPost_LoadTargetedUrls_Path.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        objGroupManager.LstGroup_DeleteCommentPostTargetedPostUrls = lstTemp.Distinct().ToList();
                        DateTime eTime = DateTime.Now;
                        lblGroups_DeleteGroupPost_LoadTargetedUrls_Count.Text = objGroupManager.LstGroup_DeleteCommentPostTargetedPostUrls.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_DeleteGroupPost";
                        objSetting.FileType = "Groups_DeleteGroupPost_LoadTargetedURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Targeted URLs Loaded : " + objGroupManager.LstGroup_DeleteCommentPostTargetedPostUrls.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Targeted URLs Loaded : " + objGroupManager.LstGroup_DeleteCommentPostTargetedPostUrls + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkScrapers_EventScraper_ExportEventScraper_CheckedChanged(object sender, EventArgs e)
        {


            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                FilePath = FilePath + "\\EventScraper.csv";
                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                EventScraper.ScrapersExprotFilePath = FilePath;
                GlobusFileHelper.DesktopPath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkEmail_EmailExport_Export_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                FilePath = FilePath + "\\CreatedEmailAccount.csv";
                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                GlobusFileHelper.DesktopPath = FilePath;
                FrmCaptcha.EmailAccountDataPath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_GroupMemberScraper_LoadGroupKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_GroupMemberScraper_LoadGroupUKeyWordsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendInfoScraper.LstOfGroupKeywords = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_GroupMemberScraper_LoadGroupKeyWordsCount.Text = objFriendInfoScraper.LstOfGroupKeywords.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_GroupMemberScraper";
                        objSetting.FileType = "Scrapers_GroupMemberScraper_LoadGroupKeyWords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group Keywords Loaded : " + objFriendInfoScraper.LstOfGroupKeywords.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group Keywords Loaded : " + objFriendInfoScraper.LstOfGroupKeywords.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkSGroup_GroupInviter_ExportGroupUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkSGroup_GroupInviter_ExportGroupUrl.Checked)
            {
                return;
            }
                
          
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                if (!string.IsNullOrEmpty(FilePath))
                {
                    FilePath = FilePath + "\\GroupInviterReport.csv";
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                    GroupManager.GroupReportExprotFilePath = FilePath;
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Select Export Data File Path .!");
                    GlobusLogHelper.log.Debug("Please Select Export Data File Path .!");
                }
              
               // GlobusFileHelper.DesktopPath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void txtFriends_RequestFriends_Keywords_MouseClick(object sender, MouseEventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaKeywords.Checked = true;
            txtFriends_RequestFriends_Location.Enabled = false;
            txtFriends_RequestFriends_Keywords.Enabled = true;
        }
     
        private void txtFriends_RequestFriends_Location_MouseClick(object sender, MouseEventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaLocation.Checked = true;
            txtFriends_RequestFriends_Keywords.Enabled = false;
            txtFriends_RequestFriends_Location.Enabled = true;
        }

        private void rdbFriends_RequestFriends_SearchViaDatabase_MouseClick(object sender, MouseEventArgs e)
        {
            rdbFriends_RequestFriends_SearchViaFanPageURLs.Checked = true;
        }

        private void rdbFriends_RequestFriends_SearchViaKeywords_MouseClick(object sender, MouseEventArgs e)
        {
            txtFriends_RequestFriends_Keywords.Enabled = true;
            txtFriends_RequestFriends_Location.Enabled = false;
        }

        private void rdbFriends_RequestFriends_SearchViaLocation_MouseClick(object sender, MouseEventArgs e)
        {
            txtFriends_RequestFriends_Location.Enabled = true;
            txtFriends_RequestFriends_Keywords.Enabled = false;
        }

        private void btnWall_PostPicOnWall_LoadTargetedUrls_Click(object sender, EventArgs e)
        {
           // chkWall_PostPicOnWall_WithMessage.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblWall_PostPicOnWall_LoadTargetedUrlsFilePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objWallPostManager.lstWallPostShareLoadTargedUrls = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblWall_PostPicOnWall_LoadTargetedUrlsCount.Text = objWallPostManager.lstWallPostShareLoadTargedUrls.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Wall_PostPicOnWall";
                        objSetting.FileType = "Wall_PostPicOnWall_LoadTargetedUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Targeted Urls Loaded : " + objWallPostManager.lstWallPostShareLoadTargedUrls.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Targeted Urls Loaded : " + objWallPostManager.lstWallPostShareLoadTargedUrls.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_FBIDExtracter_ProfileUrlFile_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_FBIDExtractor_LoadFBProfileUrlPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendInfoScraper.LstFBProfileUrlList = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_FBIDExtracter_LoadFBProfileUrlCount.Text = objFriendInfoScraper.LstFBProfileUrlList.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database

                        objSetting.Module = "Scrapers_FBProfileUrlList";
                        objSetting.FileType = "Scrapers_FBIDExtracter_FBProfileUrl";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("FB Profile Urls Loaded : " + objFriendInfoScraper.LstFBProfileUrlList.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("FB Profile Urls Loaded : " + objFriendInfoScraper.LstFBProfileUrlList.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void FrmDominator_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogresult = MessageBox.Show("Sure you want to exit faceboardpro App", "faceboardpro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogresult == DialogResult.Yes)
            {
                //logic of close program
            }
            else
            {
                e.Cancel = true;
                this.Activate();

            }
        }

        private void btnGroups_ViewSchedulerTask_LoadTargetedGroupURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_ViewSchedulerTask_TargetedGroupURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        GroupManager.LstGroupUrlsViewSchedulerTaskTargetedUrls = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblGroups_ViewSchedulerTask_TargetedGroupURLsCount.Text = GroupManager.LstGroupUrlsViewSchedulerTaskTargetedUrls.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_ViewSchedulerTask";
                        objSetting.FileType = "Groups_ViewSchedulerTask_LoadTargetedGroup";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group URLs Loaded : " + GroupManager.LstGroupUrlsViewSchedulerTaskTargetedUrls.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group URLs Loaded : " + GroupManager.LstGroupUrlsViewSchedulerTaskTargetedUrls.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        List<string> lstLoadedSearchKeywords = new List<string>();

        private void btnSearchUserSearch_LoadSearchKeywords_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                      //  lblSearch_SearchKeywords_SearchKeywordsPath.Text = ofd.FileName;
                        objSearchScraper.SearchUserSearchSearchKeyword.Clear();
                        objSearchScraper.SearchUserSearchSearchKeyword = GlobusFileHelper.ReadFile(ofd.FileName);
                        objSearchScraper.SearchUserSearchSearchKeyword = objSearchScraper.SearchUserSearchSearchKeyword.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                       // lblSearch_SearchKeywords_SearchKeywordsCount.Text = objSearchScraper.SearchUserSearchSearchKeyword.Count.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Search_UserSearch";
                        objSetting.FileType = "Search_UserSearch_LoadSearchKeyword";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Debug("Search Keyword : " + lstLoadedEmails.Count + " In " + timeSpan + " Seconds");

                        GlobusLogHelper.log.Info("Search Keyword : " + lstLoadedEmails.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_CustomAudienceScraper_LoadGroupUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_CustomAudienceScraper_LoadKeyWordsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objCustomAudiencesScraper.KeyWordLstCustomAudiencesscraper = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_CustomAudienceScraper_LoadKeyWordsCount.Text = objCustomAudiencesScraper.KeyWordLstCustomAudiencesscraper.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_CustomAudienceScraper";
                        objSetting.FileType = "Scrapers_CustomAudienceScraper_LoadAudienceKeyWords";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Search Audiences Keywords Loaded : " + objCustomAudiencesScraper.KeyWordLstCustomAudiencesscraper.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Search Audiences Keywords Loaded : " + objCustomAudiencesScraper.KeyWordLstCustomAudiencesscraper.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_CustomAudienceScraper_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objCustomAudiencesScraper.KeyWordLstCustomAudiencesscraper == null && objCustomAudiencesScraper.UrlsLstCustomAudiencesscraper == null)
                        {
                            GlobusLogHelper.log.Info("Please Load Search Audiences Keywords  !");
                            GlobusLogHelper.log.Debug("Please Load Search Audiences Keywords  !");

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    if (string.IsNullOrEmpty(CustomAudiencesScraper.ExportFilePathCustomAudiencesScraper))
                    {
                        try
                        {

                            MessageBox.Show(" Please check Export check-box .");
                            GlobusLogHelper.log.Debug(" Please check Export check-box .");
                            GlobusLogHelper.log.Info(" Please check Export check-box .");
                            return;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }


                    try
                    {
                        CustomAudiencesScraper.CustomAudiencesScraperUsingAccount = cmbScraper__CustomAudiencesScraper_Accounts.SelectedItem.ToString();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                       
                    }

                    objCustomAudiencesScraper.isStopCustomAudiencesScraper = false;
                    objCustomAudiencesScraper.lstThreadsCustomAudiencesscraper.Clear();
                 

                    string startProcessUsing = string.Empty;

                    #region Commemt ProcessUsingCode

                    try
                    {

                        CustomAudiencesScraper.StartProcessUsingCustomAudiencesScraper = cmbScrapers_CustomScraperScraper_StartProcessUsing.SelectedItem.ToString();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
                        return;
                    }                

                    #endregion

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();//Environment.ProcessorCount;

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtScrapers_CustomAudiencesScraper_Threads.Text) && checkNo.IsMatch(txtScrapers_CustomAudiencesScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtScrapers_CustomAudiencesScraper_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objCustomAudiencesScraper.NoOfThreadsCustomAudiencesscraper = threads;

                    Thread extractGroupMemberThread = new Thread(objCustomAudiencesScraper.StartCustomAudiencesscraper);
                    extractGroupMemberThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        string CustomAudiencesScraperFilePath = string.Empty;
        string CustomAudiencesScraperFilePathTxt = string.Empty;

        private void chkScrapers_CustomAudiencesScraper_Export_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkScrapers_CustomAudiencesScraper_Export.Checked)
                {
                    string FilePath = string.Empty;
                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    string FilePath1 = string.Empty;
                    string ByKeywords = FilePath;
                    CustomAudiencesScraper.ExportFilePathCustomAudiencesScraperNotepad = FilePath + "\\CustomAudiencesScraper.txt";
                    CustomAudiencesScraperFilePath = FilePath + "\\" + cmbScrapers_CustomScraperScraper_StartProcessUsing.SelectedItem.ToString().Replace(" ", "") + ".csv";
                    CustomAudiencesScraper.ExportFilePathCustomAudiencesScraper = CustomAudiencesScraperFilePath;
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath); 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkWall_PostPicOnWall_ShareVideoOnlyMe_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkWall_PostPicOnWall_ShareVideoOnlyMe.Checked)
                {
                    WallPostManager.isPrivacyOnlyMe = true;
                }
                else
                {
                    WallPostManager.isPrivacyOnlyMe = false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageInviter_LoadUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        blPage_FanPageInviter_LoadFanPageUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstFanPageUrlCollectionFanPageInviter = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        blPage_FanPageInviter_LoadFanPageUrlsCount.Text = objPageManager.lstFanPageUrlCollectionFanPageInviter.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FanPageInviter";
                        objSetting.FileType = "Pages_FanPageInviter_LoadFanPageUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPageInviter.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Urls Loaded : " + objPageManager.lstFanPageUrlCollectionFanPageInviter.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageInviter_Start_Click(object sender, EventArgs e)
        {
            string FanPageInviterProcessUsing = string.Empty;
            try
            {
                PageManager.minDelayFanPageInviter = Convert.ToInt32(txtPage_FanPageInviter_DelayMin.Text);
                PageManager.maxDelayFanPageInviter = Convert.ToInt32(txtPage_FanPageInviter_DelayMax.Text);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            #region MyRegion
            //try
            //{
            //    PageManager.FanPageInviterProcessUsing = cmbPages_FanPageInviter_StartFanPagePosterProcessUsing.SelectedItem.ToString();
            //}
            //catch (Exception ex)
            //{
            //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //    GlobusLogHelper.log.Debug("Please select Start Process Using drop down list.");
            //    GlobusLogHelper.log.Info("Please select Start Process Using drop down list.");
            //    return;
            //} 
            #endregion

            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {

                        PageManager.FanPageInviterNoOfFriendSendInviter = Convert.ToInt32(txtPages_FanPageInviter_NoOfFriendsToInvite.Text);

                        objPageManager.isStopFanPageInviter = false;
                        objPageManager.lstThreadsFanPageInviter.Clear();

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;

                        if (!string.IsNullOrEmpty(txtPages_FanPageInviter_Threads.Text) && checkNo.IsMatch(txtPages_FanPageInviter_Threads.Text))
                        {
                            threads = Convert.ToInt32(txtPages_FanPageInviter_Threads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objPageManager.NoOfThreadsFanPageInviter = threads;

                        Thread FanPagePosterThread = new Thread(objPageManager.StartFanPageInviter);
                        FanPagePosterThread.Start();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_FanPageInviter_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPageManager.isStopFanPageInviter = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsFanPageInviter.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsFanPageInviter.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void grvMessages_MessageReply_MessageDetails_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                MessageRepository objMessageRepository = new MessageRepository();
                faceboardpro.Domain.Message objMessag = new faceboardpro.Domain.Message();
                DataSet dsDataTable = new DataSet();
                //dsDataTable = objMessageRepository.SelectLoginFields();
                string userName = grvMessages_MessageReply_MessageDetails.SelectedRows[0].Cells["UserName"].Value.ToString();
                string MessageSenderName = grvMessages_MessageReply_MessageDetails.SelectedRows[0].Cells["MessageSenderName"].Value.ToString();
                string MessagingReadParticipate = grvMessages_MessageReply_MessageDetails.SelectedRows[0].Cells["MessagingReadParticipants"].Value.ToString();


                objMessag.UserName = userName;
                objMessag.MsgSenderName = MessageSenderName;
                objMessag.MessagingReadParticipants = MessagingReadParticipate;
                DialogResult dialogResult = MessageBox.Show("Do you want to delete row  from Data Table and Grid.", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //delete row and data from Table and gridview row ..
                    objMessageRepository.DeleteSelectedRowData(objMessag);

                    try
                    {
                        //DataRow[] drModelDetails = dsDataTable.Tables[0].Select("Id = '" + userId + "'");

                        //if (drModelDetails.Count() != 0)
                        //    dsDataTable.Tables[0].Rows.Remove(drModelDetails[0]);

                        //GlobusLogHelper.log.Info("[ " + userId + " is Deleted. ]");

                    }
                    catch (Exception)
                    {
                    }

                }
                else
                {

                    e.Cancel = true;
                }
                //LoadCampaign();

            }
            catch { }
        }

        private void btnGroups_GroupFansScraper_LoadGroup_Urls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblGroups_GroupFansScraper_LoadGroupUrls_Path.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        objGroupManager.LstGroup_GroupFansScraperUrls = lstTemp.Distinct().ToList();
                        DateTime eTime = DateTime.Now;
                        lblGroups_GroupFansScraper_LoadGroupUrls_Count.Text = objGroupManager.LstGroup_GroupFansScraperUrls.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Groups_GroupFansScraper";
                        objSetting.FileType = "Groups_GroupFansScraper_LoadGroupURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Group URLs Loaded : " + objGroupManager.LstGroup_GroupFansScraperUrls.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Group URLs Loaded : " + objGroupManager.LstGroup_GroupFansScraperUrls.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnGroups_GroupFansScraper_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objGroupManager.isStopGroupFansScraper = false;
                    objGroupManager.lstThreadsGroupFansScraper.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtGroups_GroupFansScraper_Threads.Text) && checkNo.IsMatch(txtGroups_GroupFansScraper_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtGroups_GroupFansScraper_Threads.Text);
                    }

                    try
                    {

                        GroupManager.strGroup_GroupFansScraperProcessUsing = CmbGroups_GroupFansScraper_StartProcessUsing.SelectedItem.ToString();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        GlobusLogHelper.log.Debug("Please select  Process Using drop down list.");
                        GlobusLogHelper.log.Info("Please select  Process Using drop down list.");
                        return;
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    //  objGroupManager.NoOfThreadsGroupInviter = threads;

                    try
                    {

                        GroupManager.minDelayGroupFansScraper = Convert.ToInt32(txtGroup_GroupFansScraper_DelayMin.Text);
                        GroupManager.maxDelayGroupFansScraper = Convert.ToInt32(txtGroup_GroupFansScraper_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    Thread groupDeletePostThread = new Thread(objGroupManager.StartGroupFansScraper);
                    groupDeletePostThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_CustomAudienceScraper_LoadQueryUrls_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblScrapers_CustomAudienceScraper_LoadUrlsWordsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();

                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objCustomAudiencesScraper.UrlsLstCustomAudiencesscraper = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblScrapers_CustomAudienceScraper_LoadUrlsWordsCount.Text = objCustomAudiencesScraper.UrlsLstCustomAudiencesscraper.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Scrapers_CustomAudienceScraper";
                        objSetting.FileType = "Scrapers_CustomAudienceScraper_LoadAudienceUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Search Audiences Urls Loaded : " + objCustomAudiencesScraper.UrlsLstCustomAudiencesscraper.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Search Audiences Urls Loaded : " + objCustomAudiencesScraper.UrlsLstCustomAudiencesscraper.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnScrapers_CustomAudienceScraper_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objCustomAudiencesScraper.isStopCustomAudiencesScraper = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objCustomAudiencesScraper.lstThreadsCustomAudiencesscraper.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objCustomAudiencesScraper.lstThreadsCustomAudiencesscraper.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }
      
        private void btnPhotos_DownloadPhoto_FanpageUrlURLs_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPhotos_DownloadPhoto_DownloadPhotoURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPhotoManager.LstDownloadPhotoURLsDownloadPhoto = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblPhotos_DownloadPhoto_DownloadPhotoURLsCount.Text = objPhotoManager.LstDownloadPhotoURLsDownloadPhoto.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Photos_DownloadPhoto";
                        objSetting.FileType = "Photos_DownloadPhotoLoadPhotoTagURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fanpage URLs Loaded : " + objPhotoManager.LstDownloadPhotoURLsDownloadPhoto.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fanpage Tag URLs Loaded : " + objPhotoManager.LstDownloadPhotoURLsDownloadPhoto.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPhotos_DownloadPhoto_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkPhoto_PhotoDownload_ExportDownloadPicture.Checked)
                {
                    MessageBox.Show("Please Check the \" Export Data \" CheckBox..!");
                    return;
                }
                else if (FBGlobals.listAccounts.Count > 0)
                {
                    objPhotoManager.isStopDwnloadPhoto = false;
                    objPhotoManager.lstThreadsDownloadPhoto.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtPhotos_DownloadPhoto_Threads.Text) && checkNo.IsMatch(txtPhotos_DownloadPhoto_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtPhotos_DownloadPhoto_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    objPhotoManager.NoOfThreadsDownloadPhoto = threads;

                    int tagNoOfFriends = 5;


                    try
                    {
                        if (objPhotoManager.LstDownloadPhotoURLsDownloadPhoto.Count < 1)
                        {
                            MessageBox.Show("Please Load Fanpage URLs !");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        PhotoManager.minDelayDownloadPhoto = Convert.ToInt32(txtPhoto_DownloadPhoto_DelayMin.Text);
                        PhotoManager.maxDelayDownloadPhoto = Convert.ToInt32(txtPhoto_DownloadPhoto_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    Thread photoTaggingThread = new Thread(objPhotoManager.StartDownloadPhoto);
                    photoTaggingThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPhotos_DownloadPhoto_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPhotoManager.isStopDwnloadPhoto = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPhotoManager.lstThreadsDownloadPhoto.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPhotoManager.lstThreadsDownloadPhoto.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                string FilePathKeyword = FilePath;
                if (string.IsNullOrEmpty(FilePathKeyword))
                {
                    GlobusLogHelper.log.Info("Please Select a Photos Export File Path !!");
                    chkPhoto_PhotoDownload_ExportDownloadPicture.Checked = false;
                }
                else
                {
                    GlobusLogHelper.log.Info("Export Photos File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Photos File Path :" + FilePath);
                    PhotoManager.ExportPhotosPath = FilePath;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void rdbGroupRequest_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbGroupRequest.Checked)
            {
                groupBox44.Enabled=false;
                groupBox45.Enabled = false;
                btnGroups_GroupCampaignManager_LoadVideoUrls.Text = "Load Group URLs";
                btnGroups_GroupCampaignManager_LoadMessages.Enabled = false;
                btnGroups_GroupCampaignManager_LoadPicUrls.Enabled = false;
                TxtGroups_GroupCampaignManager_SingleMessage.Enabled = false;
                rdbGroup_GroupCampaignManager_Video.Checked = true;
                label107.Enabled = false;
                try
                {
                    CmbGroups_GroupCampaignManager_EditCampaign.Items.Clear();
                    GroupCampaign objgroupCamp = new GroupCampaign();
                    //ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                    objgroupCamp.Module = "Group Request";
                    ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.SelectCampaigns(objgroupCamp);
                    foreach (GroupCampaign item in groupCollection)
                    {
                        //string groupname = item.GroupCampaignName;
                        CmbGroups_GroupCampaignManager_EditCampaign.Items.Add(item.GroupCampaignName);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void rdbGroupPosting_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbGroupPosting.Checked)
            {
                groupBox44.Enabled = true;
                groupBox45.Enabled = true;
                btnGroups_GroupCampaignManager_LoadVideoUrls.Text = "Load URLs";
                btnGroups_GroupCampaignManager_LoadMessages.Enabled =true;
                btnGroups_GroupCampaignManager_LoadPicUrls.Enabled = true;
                TxtGroups_GroupCampaignManager_SingleMessage.Enabled = true;
                label107.Enabled = true;
                try
                {
                    CmbGroups_GroupCampaignManager_EditCampaign.Items.Clear();
                    GroupCampaign objgroupCamp = new GroupCampaign();
                    //ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.GetAllAccount(objgroupCamp);
                    objgroupCamp.Module = "Group Posting";
                    ICollection<GroupCampaign> groupCollection = objGroupCampaignManagerRepository.SelectCampaigns(objgroupCamp);
                    foreach (GroupCampaign item in groupCollection)
                    {
                        //string groupname = item.GroupCampaignName;
                        CmbGroups_GroupCampaignManager_EditCampaign.Items.Add(item.GroupCampaignName);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void rdbGroupRequestSavedCampaigns_CheckedChanged(object sender, EventArgs e)
        {
            dgvGroupRequestManager.Enabled = true;
            btnGroups_GroupRequestManager_LoadGuoup_Urls.Enabled = false;
            btnGroups_GroupRequestManager_LoadSearchGroupKeywords.Enabled = false;
        }

        private void rdbLoadURLsKeyword_CheckedChanged(object sender, EventArgs e)
        {
            dgvGroupRequestManager.Enabled = false;
            btnGroups_GroupRequestManager_LoadGuoup_Urls.Enabled = true;
            btnGroups_GroupRequestManager_LoadSearchGroupKeywords.Enabled = true;
        }

        private void cmbScrapers_CustomScraperScraper_StartProcessUsing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbScrapers_CustomScraperScraper_StartProcessUsing.SelectedIndex > 0)
            {
                chkScrapers_CustomAudiencesScraper_Export.Enabled = true;
            }
            else
            {
                chkScrapers_CustomAudiencesScraper_Export.Enabled = false;
            }
        }

        private void chkGroupTaskSchedulerSchedulePosts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGroupTaskSchedulerSchedulePosts.Checked)
            {
                GroupManager.ScheduleGroupPosting = true;
                txtGroupManager_GroupCompaign_IntervalMinute.Enabled = true;
                txtGroupManager_GroupCompaign_NoOfGroupsInBatch.Enabled = true;
            }
            else
            {
                GroupManager.ScheduleGroupPosting = false;
                txtGroupManager_GroupCompaign_IntervalMinute.Enabled = false;
                txtGroupManager_GroupCompaign_NoOfGroupsInBatch.Enabled = false;
            }
        }

        private void chkGroup_DeleteGroupPostSchedule_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGroup_DeleteGroupPostSchedule.Checked)
            {
                GroupManager.DeleteScheduleGroupPosting = true;
                txtGroup_DeleteGroupPost_NoOfGroupPost.Enabled = true;
                txtGroup_DeleteGroupPost_IntervalTime.Enabled = true;
            }
            else
            {
                GroupManager.DeleteScheduleGroupPosting = false;
                  txtGroup_DeleteGroupPost_NoOfGroupPost.Enabled = false;
                txtGroup_DeleteGroupPost_IntervalTime.Enabled = false;
            }
        }

        private void btnFriends_MassFriendsAdder_AddFriends_Click(object sender, EventArgs e)
        {
            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    objFriendManager.isInviteYourFriendsStop = false;
                    objFriendManager.lstInviteYourFriendsThreads.Clear();

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    if (!string.IsNullOrEmpty(txtFriends_InviteYourFriends_Threads.Text) && checkNo.IsMatch(txtFriends_InviteYourFriends_Threads.Text))
                    {
                        threads = Convert.ToInt32(txtFriends_InviteYourFriends_Threads.Text);
                    }

                    if (threads > maxThread)
                    {
                        threads = 25;
                    }
                    FriendManager.InviteYourFriendsNoOfThreads = threads;
                    FriendManager.InviteYourFriendsWithTxtMessage = TxtFriendsMassFriendsAdderPersonalMassege.Text;
                    int tagNoOfFriends = 5;

                   
                    try
                    {
                        if (objFriendManager.lstInviteYourFriendsMobileNumber.Count < 1 && objFriendManager.lstInviteYourFriendsEmails.Count < 1)
                        {
                            MessageBox.Show("Load Phone number Or Emails !");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                       FriendManager.minDelayInviteYourFriends = Convert.ToInt32(txtFriends_InviteYourFriends_DelayMin.Text);
                       FriendManager.maxDelayInviteYourFriends = Convert.ToInt32(txtFriends_InviteYourFriends_DelayMax.Text);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    Thread ObjThread = new Thread(objFriendManager.StartInviteYourFriends);
                    ObjThread.Start();
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void btn_Friends_MassFriendsAdder_LoadMobileNumber_Click(object sender, EventArgs e)
        {
            rdbFriends_MassFriendsAdder_UseMobileNumber.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        objFriendManager.lstInviteYourFriendsEmails.Clear();

                        objFriendManager.lstInviteYourFriendsEmails = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_MobileNumber";
                        objSetting.FileType = "Friends_InviteYourFriends_LoadMobileNumber";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Mobile Number Loaded : " + objFriendManager.lstInviteYourFriendsEmails.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Mobile Number Loaded : " + objFriendManager.lstInviteYourFriendsEmails.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btn_Friends_MassFriendsAdder_LoadEmails_Click(object sender, EventArgs e)
        {
            rdbFriends_MassFriendsAdder_UseEmails.Checked = true;
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        objFriendManager.lstInviteYourFriendsEmails.Clear();

                        objFriendManager.lstInviteYourFriendsEmails = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_Emails";
                        objSetting.FileType = "Friends_Emails_LoadEmails";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Emails Loaded : " + objFriendManager.lstInviteYourFriendsEmails.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Emails Loaded : " + objFriendManager.lstInviteYourFriendsEmails.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnFriends_MassFriendsAdder_StopAddFriends_Click(object sender, EventArgs e)
        {
            try
            {
                objFriendManager.isRequestFriendsStop = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objFriendManager.lstInviteYourFriendsThreads.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objFriendManager.lstInviteYourFriendsThreads.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void btnPages_WebsiteSiteLiker_Start_Click(object sender, EventArgs e)
        {
          
            try
            {
                PageManager.minDelayWebSiteLiker = Convert.ToInt32(txtPage_WebSiteLiker_DelayMin.Text);
                PageManager.maxDelayWebSiteLiker = Convert.ToInt32(txtPage_WebSiteLiker_DelayMax.Text);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

         

            try
            {
                if (FBGlobals.listAccounts.Count > 0)
                {
                    try
                    {


                        objPageManager.isStopWebSiteLiker = false;
                        objPageManager.lstThreadsWebSiteLiker.Clear();

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;

                        if (!string.IsNullOrEmpty(txtPages_FanPageInviter_Threads.Text) && checkNo.IsMatch(txtPages_FanPageInviter_Threads.Text))
                        {
                            threads = Convert.ToInt32(txtPages_FanPageInviter_Threads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objPageManager.NoOfThreadsWebSiteLiker = threads;
                        PageManager.noOfLikesPerAccount = Convert.ToInt32(txtPages_WebsiteLiker_NoOfLikeParAccounts.Text);

                        Thread FanPagePosterThread = new Thread(objPageManager.StartWebSiteLiker);
                        FanPagePosterThread.Start();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                    tabMain.SelectedTab = tabMain.TabPages["tabPageAccounts"];
                    tabAccounts.SelectedTab = tabAccounts.TabPages["tabPageManageAccounts"];
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_WebsiteLiker_LoadWebsiteLiker_Click(object sender, EventArgs e)
        {

            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        blPage_WebSiteLiker_LoadWebsiteUrlsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstWebSiteLikerCollectionWebSiteLiker = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        blPage_WebSiteLiker_LoadWebsiteUrlsCount.Text = objPageManager.lstWebSiteLikerCollectionWebSiteLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_WebSiteLiker";
                        objSetting.FileType = "Pages_WebSiteLiker_LoadWebsiteUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Urls Loaded : " + objPageManager.lstWebSiteLikerCollectionWebSiteLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Urls Loaded : " + objPageManager.lstWebSiteLikerCollectionWebSiteLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnPages_WebsiteLiker_LoadMessages_Click(object sender, EventArgs e)
        {        
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        blPage_WebSiteLiker_LoadMessagePath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();


                        objPageManager.lstWebSiteLikerCollectionMessages = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        blPage_WebSiteLiker_LoadMessageCount.Text = objPageManager.lstWebSiteLikerCollectionMessages.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_WebSiteLiker";
                        objSetting.FileType = "Pages_WebSiteLiker_LoadWebsiteUrls";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page Urls Loaded : " + objPageManager.lstWebSiteLikerCollectionMessages.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page Urls Loaded : " + objPageManager.lstWebSiteLikerCollectionMessages.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        
        }


        private void btnPages_FreindsPagePostLiker_LoadFriendPageURLs_Click(object sender, EventArgs e)
        {

            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FreindsPostLiker_FriendsURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objPageManager.lstFreindsPagePostsLiker = lstTemp.Distinct().ToList();

                        DateTime eTime = DateTime.Now;

                        //lblPages_FanPageLiker_FanPageURLsCount.Text = objPageManager.lstFanPageUrlsFanPageLiker.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Pages_FriendsPostLiker";
                        objSetting.FileType = "Pages_FriendsPostLiker_LoadFriendsURLs";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Friends URLs Loaded : " + objPageManager.lstFreindsPagePostsLiker.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Friends URLs Loaded : " + objPageManager.lstFreindsPagePostsLiker.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

        private void chkLikePostsWithUploadedUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLikePostsWithUploadedUrl.Checked)
            {
                btnPages_FreindsPagePostLiker_LoadFriendPageURLs.Enabled = true;
            }
            else
            {
                btnPages_FreindsPagePostLiker_LoadFriendPageURLs.Enabled = false;
            }
        }

       


        private void btnPages_WebsiteSiteLiker_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                objPageManager.isStopWebSiteLiker = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = objPageManager.lstThreadsWebSiteLiker.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        objPageManager.lstThreadsWebSiteLiker.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        //Thread.ResetAbort();
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Stopped !");
            GlobusLogHelper.log.Debug("Process Stopped !");
        }

        private void chExportCancelFriendRequest_CheckedChanged(object sender, EventArgs e)
        {
            if (chExportCancelFriendRequest.Checked)
            {
                try
                {

                    string FilePath = string.Empty;
                    string FilePathCancelFriendrequest = string.Empty;
                    string FilePathAcceptFriendRequest = string.Empty;

                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    FilePathCancelFriendrequest = FilePath;
                    FilePathAcceptFriendRequest = FilePath;
                    string FilePathKeyword = FilePath;
                    FilePath = FilePath + "\\CancelSentFreindRequest.csv";
                    FilePathCancelFriendrequest = FilePathCancelFriendrequest + "\\CancelFriendrequest.csv";
                    FilePathAcceptFriendRequest = FilePathAcceptFriendRequest + "\\AcceptFriendRequest.csv";
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                    FriendManager.CancelSentFriendRequestExprotFilePath = FilePath;
                    FriendManager.AcceptFriendRequestExportFilePath = FilePathAcceptFriendRequest;
                    FriendManager.CancelFriendRequestExportFilePath = FilePathCancelFriendrequest;

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void chkSGroup_DeletePost_ExportPostUrlWithUser_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                string FilePath1 = string.Empty;

                FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                FilePath1 = FilePath;
                string FilePathKeyword = FilePath;
                FilePath = FilePath + "\\DeletePostsFromGroup.csv";
                FilePath1 = FilePath1 + "\\DeleteCommentFromGroupUrl.csv";

                string processType = CmbGroups_GroupDeletePost_StartProcessUsing.SelectedItem.ToString();
                if (processType.Contains("DeleteGroupPost"))
                {
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                }
                if (processType.Contains("PostDeleteComments"))
                {
                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath1);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath1);
                }

                GroupManager.GroupDeletePostFilePath = FilePath;
                GroupManager.GroupDeletePostCommentFilePath = FilePath1;
                GlobusFileHelper.DesktopPath = FilePath;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkUseDivideData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseDivideData.Checked)
            {
                GrpDivideDataOption.Enabled = true;
               
            }
            else
            {
                GrpDivideDataOption.Enabled = false;
            }
        }

        private void btnFriendsLoadFriendsUrl_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {

                    ofd.Filter = "Text Files (*.txt)|*.txt";
                    ofd.InitialDirectory = Application.StartupPath;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DateTime sTime = DateTime.Now;

                        lblPages_FanPageLiker_FanPageURLsPath.Text = ofd.FileName;

                        List<string> lstTemp = new List<string>();


                        lstTemp = GlobusFileHelper.ReadFile(ofd.FileName).Distinct().ToList();

                        objFriendManager.lstFriendsUrlToSuggestFriends = lstTemp.Distinct().ToList();


                        DateTime eTime = DateTime.Now;

                        lblSuggestFriendsUrlCount.Text = objFriendManager.lstFriendsUrlToSuggestFriends.Count.ToString();

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        //Insert Seeting Into Database
                        objSetting.Module = "Friends_Accept/CancelFriends";
                        objSetting.FileType = "Friends_Accept/CancelFriends_LoadFriendsUrl";
                        objSetting.FilePath = ofd.FileName;

                        InsertOrUpdateSetting(objSetting);

                        GlobusLogHelper.log.Info("Fan Page URLs Loaded : " + objFriendManager.lstFriendsUrlToSuggestFriends.Count + " In " + timeSpan + " Seconds");
                        GlobusLogHelper.log.Debug("Fan Page URLs Loaded : " + objFriendManager.lstFriendsUrlToSuggestFriends.Count + " In " + timeSpan + " Seconds");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

        }

       

        private void chkMessageWithUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMessageWithUrl.Checked)
            {
                objPageManager.isPostMessageWithUrl = true;
            }
            else
            {
                objPageManager.isPostMessageWithUrl = false;
            }
        }

        private void ChkbGroup_ViewSchedulerTask_PostVideoUrlAsImage_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkbGroup_ViewSchedulerTask_PostVideoUrlAsImage.Checked)
            {
                GroupManager.isPostVideoUrlAsImage = true;
            }
            else
            {
                GroupManager.isPostVideoUrlAsImage = false;
            }
        }

        private void ChkbGroup_ViewSchedulerTask_PostMessageWithImageAsEdited_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkbGroup_ViewSchedulerTask_PostMessageWithImageAsEdited.Checked)
            {
                GroupManager.isPostMessageWithImageAsEdited = true;
            }
            else
            {
                GroupManager.isPostMessageWithImageAsEdited = true;
            }
        }

        private void chkSGroup_GroupManager_CreateSpinMessage_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkSGroup_GroupManager_CreateSpinMessage.Checked)
            //{
            //    Thread ObjNew = new Thread(LoadSpinMessagesPostScheduler);
            //    ObjNew.SetApartmentState(ApartmentState.STA);
            //    ObjNew.Start();

            //}
        }

        private void LoadSpinMessagesPostScheduler(string Temp)
        {

           
                objGroupManager.LstMessageUrlsGroupCampaignManager.Clear();
                //chkSGroup_GroupManager_CreateSpinMessage.Checked = true;
                try
                {

                    GenrateSpinMesaagesPostSchedule(Temp);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
        }

        public void GenrateSpinMesaagesPostSchedule(string Item)
        {
            // Array paramsArray = new object[2];
            // paramsArray = (Array)Item;
            //  string item = paramsArray.GetValue(0).ToString();
            string item = Item;
            Utils.GetStartSpinnedListItem(item);

            foreach (string item1 in Utils.list_SpinnedCreatorMessages)
            {
                objGroupManager.LstMessageUrlsGroupCampaignManager.Add(item1);

            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://pvadomination.com/");
        }

        private void chk_ExportDataFriendsModule_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_ExportDataFriendsModule.Checked)
            {
                try
                {

                    string FilePath = string.Empty;
                    string FilePathFriendrequest = string.Empty;
                    string FilePathFailedFriendRequest = string.Empty;

                    FilePath = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    FilePathFriendrequest = FilePath;
                    FilePathFailedFriendRequest = FilePath;
                    string FilePathKeyword = FilePath;
                
                    FilePathFriendrequest = FilePathFriendrequest + "\\Friendrequest.csv";
                    FilePathFailedFriendRequest = FilePathFailedFriendRequest + "\\FailedFriendRequest.csv";

                    GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                    GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);


                    FriendManager.FilePathFailedFriendRequestFilePath = FilePathFailedFriendRequest;
                    FriendManager.FilePathFriendrequestFilePath = FilePathFriendrequest;

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        private void btnLoadImagesToMessage_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog ofd = new FolderBrowserDialog())
                {


                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        lblGroups_GroupCampaignManager_PicURLsPath.Text = ofd.SelectedPath;
                        List<string> lstpicdata = new List<string>();
                        string[] picsArray = Directory.GetFiles(ofd.SelectedPath);
                        lstpicdata = picsArray.ToList();

                        List<string> lstDistinctlstpicdata = new List<string>();
                        List<string> lstWronglstpicdata = new List<string>();
                        foreach (string item in lstpicdata)
                        {
                            try
                            {
                                string items = item.ToLower();

                                if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
                                {
                                    lstDistinctlstpicdata.Add(item);


                                }
                                else
                                {
                                    lstWronglstpicdata.Add(item);

                                }


                            }
                            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }



                            objMessageManager.lstImagePathMessage = lstDistinctlstpicdata;
                        }

                        lblGroups_GroupCampaignManager_PicURLsCount.Text = objGroupManager.LstPicUrlsGroupCampaignManager.Count().ToString();
                        Console.WriteLine(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Info(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Debug(lstDistinctlstpicdata.Count + " Picture loaded");
                        GlobusLogHelper.log.Info(lstWronglstpicdata.Count + " Incorrect Picture loaded");
                        GlobusLogHelper.log.Debug(lstWronglstpicdata.Count + " Incorrect Picture loaded");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }

        private void tabMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Brush _TextBrush;

                // Get the item from the collection.
                TabPage _TabPage = tabMain.TabPages[e.Index];

                // Get the real bounds for the tab rectangle.
                Rectangle _TabBounds = tabMain.GetTabRect(e.Index);

                if (e.State == DrawItemState.Selected)
                {
                    // Draw a different background color, and don't paint a focus rectangle.
                    Brush background_brush1 = new SolidBrush(Color.FromArgb(164, 49, 21));//(95, 181, 232));
                    _TextBrush = new SolidBrush(Color.White);
                    g.FillRectangle(background_brush1, e.Bounds);
                }
                else
                {
                    _TextBrush = new System.Drawing.SolidBrush(Color.Brown);
                    g.FillRectangle(Brushes.Brown, e.Bounds);
                    e.DrawBackground();
                }

                // Use our own font. Because we CAN.
                Font _TabFont = new Font("Verdana", 12, FontStyle.Bold, GraphicsUnit.Pixel);


                //Set On tab For hiding Side Space of Tab button....

                Brush background_brush = new SolidBrush(Color.FromArgb(86, 137, 194));

                Rectangle LastTabRect = tabMain.GetTabRect(tabMain.TabPages.Count - 1);

                Rectangle rect = new Rectangle();

                rect.Location = new Point(0, LastTabRect.Bottom + 3);

                rect.Size = new Size(_TabPage.Right - (_TabPage.Width + 3), _TabPage.Height);

                //e.Graphics.FillRectangle(background_brush, rect);
                //e.Graphics.DrawImage(recSideImage, rect);


                // Draw string. Center the text.
                StringFormat _StringFlags = new StringFormat();
                _StringFlags.Alignment = StringAlignment.Center;
                _StringFlags.LineAlignment = StringAlignment.Center;
                //g.DrawString(_TabPage.Text, TabFont, TextBrush,
                //_TabBounds, new StringFormat(_StringFlags));
            }
            catch { } 
        }
        
    }

    #region Support Functions for various UI



    #endregion

    /// <summary>
    /// 
    /// </summary>

    public class GlobusLogAppender : log4net.Appender.AppenderSkeleton
    {

        private static readonly object lockerLog4Append = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                string loggerName = loggingEvent.Level.Name;

                FrmDominator frmDominator = (FrmDominator)Application.OpenForms["FrmDominator"];

                lock (lockerLog4Append)
                {
                    switch (loggingEvent.Level.Name)
                    {
                        case "DEBUG":
                            try
                            {
                                //if (frmDominator.listBoxLogs.InvokeRequired)
                                {
                                    if (frmDominator.listBoxLogs.InvokeRequired)  //if (frmDominator.InvokeRequired)
                                    {
                                        frmDominator.listBoxLogs.Invoke(new MethodInvoker(delegate
                                        {
                                            try
                                            {
                                                if (frmDominator.listBoxLogs.Items.Count > 1000)
                                                {
                                                    frmDominator.listBoxLogs.Items.RemoveAt(frmDominator.listBoxLogs.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                                }

                                                frmDominator.listBoxLogs.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }

                                        }));

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (frmDominator.listBoxLogs.Items.Count > 1000)
                                            {
                                                frmDominator.listBoxLogs.Items.RemoveAt(frmDominator.listBoxLogs.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                            }

                                            frmDominator.listBoxLogs.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
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
                                Console.WriteLine("Error Case Debug : " + ex.StackTrace);
                                Console.WriteLine("Error Case Debug : " + ex.Message);
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            break;
                        case "INFO":
                            try
                            {
                                //if (frmDominator.toolStripStatusLabel.InvokeRequired)
                                if (false)
                                {
                                    frmDominator.toolStripMain.Invoke(new MethodInvoker(delegate
                                    {
                                        try
                                        {
                                            string LoggerRenderedMessage =string.Empty;
                                            //if (loggingEvent.RenderedMessage.Contains("{") && loggingEvent.RenderedMessage.Contains("{"))
                                            //{
                                            //     LoggerRenderedMessage = loggingEvent.RenderedMessage;
                                            //}
                                            LoggerRenderedMessage = loggingEvent.RenderedMessage;
                                            //LoggerRenderedMessage = string.Join("\r\n", LoggerRenderedMessage);
                                            LoggerRenderedMessage = LoggerRenderedMessage.Replace(Environment.NewLine, " ");
                                            frmDominator.toolStripStatusLabel.Text = LoggerRenderedMessage;
                                            //frmDominator.toolStripStatusLabel.Text = loggingEvent.RenderedMessage;
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }));
                                }
                                else
                                {
                                    try
                                    {
                                        string LoggerRenderedMessage = string.Empty;
                                        LoggerRenderedMessage = loggingEvent.RenderedMessage;
                                       // LoggerRenderedMessage = string.Join("\r\n", LoggerRenderedMessage);
                                        LoggerRenderedMessage = LoggerRenderedMessage.Replace(Environment.NewLine, " ");
                                        frmDominator.toolStripStatusLabel.Text = LoggerRenderedMessage;
                                        //frmDominator.toolStripStatusLabel.Text = loggingEvent.RenderedMessage;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }


                                if (frmDominator.listBoxLogs.InvokeRequired)//if (frmDominator.InvokeRequired)
                                {
                                    frmDominator.listBoxLogs.Invoke(new MethodInvoker(delegate
                                    {
                                        try
                                        {
                                            if (frmDominator.listBoxLogs.Items.Count > 1000)
                                            {
                                                frmDominator.listBoxLogs.Items.RemoveAt(frmDominator.listBoxLogs.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                            }

                                            frmDominator.listBoxLogs.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                    }));

                                }
                                else
                                {
                                    try
                                    {
                                        if (frmDominator.listBoxLogs.Items.Count > 1000)
                                        {
                                            frmDominator.listBoxLogs.Items.RemoveAt(frmDominator.listBoxLogs.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                        }

                                        frmDominator.listBoxLogs.Items.Insert(0, loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case INFO : " + ex.StackTrace);
                                Console.WriteLine("Error Case INFO : " + ex.Message);
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            break;
                          default:
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
