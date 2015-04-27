using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using Accounts;
using System.Threading;
using faceboardpro;

namespace Campaigns
{
    public class CampaignManager
    {

        public bool isStopCmpFanPageLiker = false;
        List<Thread> lstThreadCmpFanPageLiker = new List<Thread>();

        public void StartCmpFanpageLiker(object parameters)
        {
            try
            {
                string cmpName=string.Empty; 
                string cmpProcess=string.Empty; 
                string account=string.Empty; 
                List<string> lstFanPageURLs=new List<string>();
                List<string> lstFanPageMessages = new List<string>();
                List<string> lstFanPageComments = new List<string>();

                Array paramsArray = new object[6];
                paramsArray = (Array)parameters;

                cmpName = paramsArray.GetValue(0).ToString();
                cmpProcess = paramsArray.GetValue(1).ToString();
                account = paramsArray.GetValue(2).ToString();
                lstFanPageURLs = (List<string>)paramsArray.GetValue(3);
                lstFanPageMessages = (List<string>)paramsArray.GetValue(4);
                lstFanPageComments = (List<string>)paramsArray.GetValue(5);

                cmpFanPageLikerLogin(cmpName, cmpProcess, account, lstFanPageURLs, lstFanPageMessages, lstFanPageComments);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void cmpFanPageLikerLogin(string cmpName, string cmpProcess, string account, List<string> lstFanPageURLs, List<string> lstFanPageMessages, List<string> lstFanPageComments)
        {
            
            try
            {
                if (isStopCmpFanPageLiker)
                {
                    return;
                }
                try
                {
                    lstThreadCmpFanPageLiker.Add(Thread.CurrentThread);
                    lstThreadCmpFanPageLiker.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (account.Contains(":"))
                {
                    string[] AccArr = account.Split(':');
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
                                accountUser = account.Split(':')[0];
                                accountPass = account.Split(':')[1];
                                proxyAddress = account.Split(':')[2];
                                proxyPort = account.Split(':')[3];
                                proxyUserName = account.Split(':')[4];
                                proxyPassword = account.Split(':')[5];
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

                                StartCmpFanPageLikerProcess(ref objFacebookUser,cmpName,cmpProcess,account, lstFanPageURLs, lstFanPageMessages,lstFanPageComments);

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
                        GlobusLogHelper.log.Info("Account : " + account + " Not In Correct Format !");
                        GlobusLogHelper.log.Debug("Account : " + account + " Not In Correct Format !");

                        return ;
                    }
                }

                else
                {
                    GlobusLogHelper.log.Info("Account : " + account + " Not In Correct Format !");
                    GlobusLogHelper.log.Debug("Account : " + account + " Not In Correct Format !");

                    return ;
                }

               
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartCmpFanPageLikerProcess(ref FacebookUser fbUser, string cmpName, string cmpProcess, string account, List<string> lstFanPageURLs, List<string> lstFanPageMessages, List<string> lstFanPageComments)
        {
            try
            {
                Pages.PageManager objPageManager = new Pages.PageManager();

                if (cmpProcess == "Like Page")
                {
                    objPageManager.LikePage(ref fbUser, lstFanPageURLs);
                }

                else if (cmpProcess == "Share Page")
                {
                    objPageManager.SharePage(ref fbUser, lstFanPageURLs);
                }
                else if (cmpProcess == "Like Post")
                {
                    objPageManager.LikePost(ref fbUser, lstFanPageURLs, lstFanPageMessages);
                }
                else if (cmpProcess == "Comment On Post")
                {
                    objPageManager.CommentOnPost(ref fbUser, lstFanPageURLs, lstFanPageComments);
                }
                else
                {
                    GlobusLogHelper.log.Info("Campaign Process Is Not Selected With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Campaign Process Is Not Selected With Username : " + fbUser.username);

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        
    }
}
