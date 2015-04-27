using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using BaseLib;
using faceboardpro;
using Accounts;
using System.IO;
using Globussoft;

namespace Search
{
    public class SearchScraper
    {
        
        public BaseLib.Events SearchScraperEvent = null;

        #region Global Variables For Search Scraper

        readonly object lockrThreadControllerSearchScraper = new object();
        public bool isStopSearchScraper = false;
        int countThreadControllerSearchScraper = 0;
        public List<Thread> lstThreadsSearchScraper = new List<Thread>();

        public static string ScrapersExprotFilePath = string.Empty;

        public string SearchGroup_Membership = string.Empty;
        public string SearchGroup_Privacy = string.Empty;
        public string SearchGroup_About = string.Empty; 
        public string SearchGroup_Name = string.Empty; 


        #endregion

        #region Property For Search Scraper

        public int NoOfThreadsSearchScraper
        {
            get;
            set;
        }
        public List<string> LstEventURLsSearchScraper
        {
            get;
            set;
        }

        public static string exportFilePathAccountVerification
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
                SearchScraperEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
               // GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public SearchScraper()
        {
            SearchScraperEvent = new BaseLib.Events();
        }

        public void StartSearchScraper()
        {
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsSearchScraper > 0)
                {
                    numberOfAccountPatch = NoOfThreadsSearchScraper;
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
                                lock (lockrThreadControllerSearchScraper)
                                {
                                    try
                                    {
                                        if (countThreadControllerSearchScraper >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockrThreadControllerSearchScraper);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartMultiThreadsSearchScraper);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;

                                            profilerThread.Start(new object[] { item });

                                            countThreadControllerSearchScraper++;
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

        public void StartMultiThreadsSearchScraper(object parameters)
        {
            try
            {
                if (!isStopSearchScraper)
                {
                    try
                    {
                        lstThreadsSearchScraper.Add(Thread.CurrentThread);
                        lstThreadsSearchScraper.Distinct();
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
                                //objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
                            }

                            if (objFacebookUser.isloggedin)
                            {
                                // Call StartActionEventInviter
                                StartActionSearchScraper(ref objFacebookUser);
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
                 
                  //  if (!isStopSearchScraper)
                    {
                        lock (lockrThreadControllerSearchScraper)
                        {
                            countThreadControllerSearchScraper--;
                            Monitor.Pulse(lockrThreadControllerSearchScraper);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
        }

        public void StartActionSearchScraper(ref FacebookUser fbUser)
        {
            try
            {
                ExtractGroupsdetailsSearchScraper(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        public static int GlobalExistCounter = 0;
        public static int Pagecounter = 0;

        public void ExtractGroupsdetailsSearchScraper(ref FacebookUser fbUser)
        {
            try
            {

                GlobusHttpHelper objHelper = fbUser.globusHttpHelper;
                string url = string.Empty;

                if (SearchGroup_Membership == "all" && SearchGroup_Privacy == "all" && SearchGroup_Name == "all" && SearchGroup_About == "all")
                {
                    url = "https://www.facebook.com/search/groups/all";

                    string Response = objHelper.getHtmlfromUrl(new Uri(url));
                    string User = string.Empty;


                    DataParser(Response, ref fbUser);

                    string AjaxResponse = AjaxPostForSearch(Response, Response, ref fbUser);


                   // string AjaxResponse = AjaxPostForSearch("", Response, ref FBuser);

                    while (!string.IsNullOrEmpty(AjaxResponse))
                    {
                        //if (Pagecounter < MaxItreation)
                        //{
                            string[] SerachList2 = System.Text.RegularExpressions.Regex.Split(AjaxResponse, "clearfix _zw");
                            if (GlobalExistCounter > 99)
                            {
                                Console.WriteLine("Page response not give the data for more time,so we skip the scraping process");
                                break;
                            }

                            foreach (string item in SerachList2)
                            {
                                try
                                {
                                    if (!item.Contains("<!DOCTYPE html>"))
                                    {
                                        if (GlobalExistCounter < 100)
                                        {
                                            DataParserAjax(AjaxResponse, ref fbUser);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                catch { };
                            }
                      
                            AjaxResponse = AjaxPostForSearch(AjaxResponse, Response, ref fbUser);
                            Pagecounter = Pagecounter + 1;
                    }

                }
                else
                {

                    url = "https://www.facebook.com/search/groups/all";

                    string Response = objHelper.getHtmlfromUrl(new Uri(url));

                    string AjaxUrl = "https://www.facebook.com/ajax/browse/null_state.php?grammar_version=ff07ed4d3774c70937c166803513a3957dc91241&__user=100003654049360&__a=1&__dyn=7n8ahyj35zoSt2u5KKAHyG85oCi8wIw&__req=1";

                    string AjaxResponce = AjaxPostForSearch(Response, Response, ref fbUser);

                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void DataParser(string response, ref FacebookUser fbUser)
        {
            try
            {
                try
                {
                    string Response = response;
                    ChilkatHttpHelpr objChilkat = new ChilkatHttpHelpr();

                    string FirstPgaeName = string.Empty;
                    string SecondPgaeName = string.Empty;
                    string otherPgaeName = string.Empty;
               
                
                  
                    string PageTitle = string.Empty;
                    string PageTitleLink = string.Empty;

                    string SourceTitle = string.Empty;
                    string SourceTitleLink = string.Empty;

                    string memberCount = string.Empty;

                    string FirstPgaeNameLink = string.Empty;
                    string SecondPgaeNameLink = string.Empty;
                    string otherPgaeNameLink = string.Empty;
              
                    string likerData = string.Empty;
                    string SecondlikerData = string.Empty;
                    string ThirdlikerData = string.Empty;

                    string AboutData = string.Empty;
                    string AboutDataLink = string.Empty;

                 
                    bool MemberPage = false;

                    string[] DataList = System.Text.RegularExpressions.Regex.Split(Response, "<div class=\"");
                    foreach (string item2 in DataList)
                    {

                        #region Title

                        if (item2.Contains("&quot;title&quot"))
                        {
                            List<string> hrefLink = objChilkat.GetHrefFromString(item2);

                            try
                            {

                                string titleData = item2.Substring(item2.IndexOf("\"><a") + 5);
                                string DataPage = GetSbstringData(titleData, "\">", "<");

                                PageTitle = DataPage;
                                PageTitleLink = hrefLink[0].ToString();


                            }
                            catch { };


                        }



                        #endregion


                        #region GroupSource

                        if (item2.Contains("quot;sub_headers&quot"))
                        {
                            List<string> hrefLink = objChilkat.GetHrefFromString(item2);

                            try
                            {

                                string titleData = item2.Substring(item2.IndexOf("\"><a") + 5);
                                string DataPage = GetSbstringData(titleData, "\">", "<");

                                SourceTitle = DataPage;
                                if (hrefLink.Count() > 0)
                                {
                                    SourceTitleLink = hrefLink[0].ToString();
                                }


                            }
                            catch { };


                        }



                        #endregion

                        # region Members
                        if (item2.Contains("members") && item2.Count()<150 && MemberPage == false)
                        {
                            MemberPage = true;
                            try
                            {
                                string MemberData = GetSbstringData(item2, "\">", "<").Replace("members","").Replace(",","");
                                memberCount = MemberData;
                            }
                            catch { };


                        }
                        #endregion

                        #region Aboutdata


                        #endregion

                        #region Friendslink
                       #endregion

                    }

                    string Username = string.Empty;
                    if (!string.IsNullOrEmpty(PageTitle))
                    {
                        string data = PageTitleLink.Substring(0, PageTitleLink.IndexOf("?"));

                        Username = data.Split('/')[3];

                        // Username = PageTitle + " : " + PageTitleLink;
                    }
                

                    string Likes = string.Empty;
                    if (!string.IsNullOrEmpty(FirstPgaeName))
                    {
                        //Likes ="Likes "+ FirstPgaeName + " : " + FirstPgaeNameLink;
                        Likes = "Likes " + FirstPgaeNameLink;
                    }
                    if (!string.IsNullOrEmpty(SecondPgaeName))
                    {
                        // Likes = Likes + " and " + SecondPgaeName + " : " + SecondPgaeNameLink;
                        Likes = Likes + " and " + SecondPgaeNameLink;
                    }
                    if (!string.IsNullOrEmpty(otherPgaeName))
                    {
                        // Likes = Likes + " and " + otherPgaeName + " : " + otherPgaeNameLink;
                        Likes = Likes + " and " + otherPgaeNameLink;
                    }

                    if (Username.Count() > 5)
                    {
                        try
                        {
                            string FollowerData = string.Empty;

                            string Userdata = Username.Replace(",", "") + "," + PageTitle.Replace(",", "") + "," + SourceTitle.Replace(",", "") + "," + memberCount.Replace(",", "") + "," + AboutData.Replace(",", "");


                            string FileData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\FBScraperDataDirectory\\SearchUrlUserId.txt";
                            if (!File.Exists(FileData))
                            {
                                try
                                {
                                    string DataHeader = " UserId " + "\t" + " Likes " + "\t" + " Studied In " + " \\t " + " Listens " + " \\t " + "Followers" + "\\t " + "Watches";

                                    GlobusFileHelper.AppendStringToTextfileNewLine(DataHeader, exportFilePathAccountVerification + "\\GroupSearchData");
                                }
                                catch { };
                            }

                            GlobusFileHelper.AppendStringToTextfileNewLine(Userdata, exportFilePathAccountVerification + "\\GroupSearchData");


                            //AppendStringToTextFileNewLine(Data, FileData);
                        }
                        catch { };
                    }


                }
                catch { };


            
        }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void DataParserAjax(string response, ref FacebookUser fbUser)
        {
            try
            {
                try
                {
                    string Response = response;
                    ChilkatHttpHelpr objChilkat = new ChilkatHttpHelpr();

                    string FirstPgaeName = string.Empty;
                    string SecondPgaeName = string.Empty;
                    string otherPgaeName = string.Empty;



                    string PageTitle = string.Empty;
                    string PageTitleLink = string.Empty;

                    string SourceTitle = string.Empty;
                    string SourceTitleLink = string.Empty;

                    string memberCount = string.Empty;

                    string FirstPgaeNameLink = string.Empty;
                    string SecondPgaeNameLink = string.Empty;
                    string otherPgaeNameLink = string.Empty;

                    string likerData = string.Empty;
                    string SecondlikerData = string.Empty;
                    string ThirdlikerData = string.Empty;


                    bool MemberPage = false;
                    Response = Response.Replace("\\", "").Replace("u003C", "<");

                    string[] DataList = System.Text.RegularExpressions.Regex.Split(Response, "<div class=\"");
                    foreach (string item2 in DataList)
                    {

                        #region Title

                        if (item2.Contains("&quot;title&quot"))
                        {
                            List<string> hrefLink = objChilkat.GetHrefFromString(item2);

                            try
                            {

                                string titleData = item2.Substring(item2.IndexOf("\"><a") + 5);
                                string DataPage = GetSbstringData(titleData, "\">", "<");

                                PageTitle = DataPage;
                                PageTitleLink = hrefLink[0].ToString();


                            }
                            catch { };


                        }



                        #endregion


                        #region GroupSource

                        if (item2.Contains("quot;sub_headers&quot"))
                        {
                            List<string> hrefLink = objChilkat.GetHrefFromString(item2);

                            try
                            {

                                string titleData = item2.Substring(item2.IndexOf("\"><a") + 5);
                                string DataPage = GetSbstringData(titleData, "\">", "<");

                                SourceTitle = DataPage;
                                if (hrefLink.Count() > 0)
                                {
                                    SourceTitleLink = hrefLink[0].ToString();
                                }


                            }
                            catch { };


                        }



                        #endregion


                        # region Memebers
                        if (item2.Contains("members") && item2.Count() < 150 && MemberPage == false)
                        {
                            MemberPage = true;
                            try
                            {
                                string MemeberData = GetSbstringData(item2, "\">", "<").Replace("members", "").Replace(",", "");
                                memberCount = MemeberData;
                            }
                            catch { };


                        }
                        #endregion


                    }

                    string Username = string.Empty;
                    if (!string.IsNullOrEmpty(PageTitle))
                    {
                        string data = PageTitleLink.Substring(0, PageTitleLink.IndexOf("?"));

                        Username = data.Split('/')[3];

                        // Username = PageTitle + " : " + PageTitleLink;
                    }


                    string Likes = string.Empty;
                    if (!string.IsNullOrEmpty(FirstPgaeName))
                    {
                        //Likes ="Likes "+ FirstPgaeName + " : " + FirstPgaeNameLink;
                        Likes = "Likes " + FirstPgaeNameLink;
                    }
                    if (!string.IsNullOrEmpty(SecondPgaeName))
                    {
                        // Likes = Likes + " and " + SecondPgaeName + " : " + SecondPgaeNameLink;
                        Likes = Likes + " and " + SecondPgaeNameLink;
                    }
                    if (!string.IsNullOrEmpty(otherPgaeName))
                    {
                        // Likes = Likes + " and " + otherPgaeName + " : " + otherPgaeNameLink;
                        Likes = Likes + " and " + otherPgaeNameLink;
                    }



                    if (Username.Count() > 5)
                    {
                        try
                        {
                            string FollowerData = string.Empty;
                            string FileData = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\FBScraperGroupData\\SearchUrlUserId.txt";
                            if (!File.Exists(FileData))
                            {
                                try
                                {
                                    string DataHeader = " UserId " + "\t" + " Likes " + "\t" + " Studied In " + " \\t " + " Listens " + " \\t " + "Followers" + "\\t " + "Watches";

                                    GlobusFileHelper.AppendStringToTextfileNewLine(DataHeader, exportFilePathAccountVerification + "\\NoOptionForConfirmationEmailResend.txt");
                                }
                                catch { };
                            }

                            //string Data = MainUrl + "\t" + Username.Replace(",", "") + "\t" + Likes.Replace(",", "") + "\t" + studyData.Replace(",", "") + "\t" + Listens.Replace(",", "") + "\t" + FollowerData.Replace(",", "") + " \\t " + Watches.Replace(",", "");



                            //AppendStringToTextFileNewLine(Data, FileData);
                        }
                        catch { };
                    }


                }
                catch { };



            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public static int ReqCounter = 3;
        static char letter = 'a';
        public static string ReqcounterValue = string.Empty;

        public string AjaxPostForSearch(string Responce, string CompleteResponse, ref FacebookUser FBuser)
        {
            string AjaxResponse = string.Empty;
            string ViewData = string.Empty;
            string Encoded_query = string.Empty;
            string Encoded_title = string.Empty;
            string Ref = string.Empty;
            string Logger_source = string.Empty;
            string Typeahead_sid = string.Empty;
            string Tl_log = string.Empty;
            string Impression_id = string.Empty;
            string Filter_ids = string.Empty;
            string Experience_type = string.Empty;
            string Exclude_ids = string.Empty;
            string Cursor = string.Empty;
            string ads_at_end = string.Empty;
            string Userid = string.Empty;

            if (ReqCounter > 10)
            {
                letter = (char)((byte)letter + 3);
                ReqcounterValue = letter.ToString();
                ReqCounter = ReqCounter + 3;
            }
            else
            {
                ReqcounterValue = ReqCounter.ToString();
                ReqCounter = ReqCounter + 3;
            }

            GlobusHttpHelper objGlobusHttpHelper = FBuser.globusHttpHelper;
            
            try
            {
                ViewData = GetFieldForPostData(CompleteResponse, "\"view\"");

                Encoded_query = GetFieldForPostData(CompleteResponse, "\"encoded_query\"");

                Encoded_title = GetFieldForPostData(CompleteResponse, "\"encoded_title\"");

                Ref = GetFieldForPostData(CompleteResponse, "\"ref\"");

                Logger_source = GetFieldForPostData(CompleteResponse, "\"logger_source\"");

                Typeahead_sid = GetFieldForPostData(CompleteResponse, "\"typeahead_sid\"");

                Tl_log = GetFieldForPostData(CompleteResponse, "\"tl_log\"");

                Impression_id = GetFieldForPostData(CompleteResponse, "\"impression_id\"");

                Filter_ids = GetFieldFilterIdsForPostData(CompleteResponse, "\"filter_ids\"");

                Experience_type = GetFieldForPostData(CompleteResponse, "\"experience_type\"");

                Exclude_ids = GetFieldForPostData2(CompleteResponse, "\"exclude_ids\"");

                if (string.IsNullOrEmpty(Responce))
                {
                    Cursor = GetFieldForPostData(CompleteResponse, "\"cursor\"");
                }
                else
                {
                    Cursor = GetFieldForPostData(Responce, "\"cursor\"");
                }
                ads_at_end = GetFieldForPostData2(Responce, "\"ads_at_end\"");

                //rev
               string rev = GetFieldForPostData3(Responce, "rev\"");

                //Userid = GetFieldForPostData(Responce, "\"uid\"");

                try
                {
                    Userid = GlobusHttpHelper.ParseJson(CompleteResponse, "user").Replace("p?id=", "");

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                string PostData = "data={\"view\":\"" + ViewData + "\",\"encoded_query\":\"" + Encoded_query + "\",\"encoded_title\":\"" + Encoded_title + "\",\"ref\":\"" + Ref + "\",\"logger_source\":\"" + Logger_source + "\",\"typeahead_sid\":" + Typeahead_sid + "\"\",\"tl_log\":" + "false" + ",\"impression_id\":\"" + Impression_id + "\",\"filter_ids\":{" + Filter_ids + "},\"experience_type\":\"" + Experience_type + "\",\"exclude_ids\":[],\"cursor\":\"" + Cursor + "\",\"ads_at_end\":true}";

                string AjaxPostUrl = (Uri.EscapeDataString(PostData)).Replace("data%3D", "data=") + "&__user=" + Userid + "&__a=1&__dyn=7n8ahyj35zoSt2u5KKAHyG85oCi8w&__req=" + ReqcounterValue + "&__rev="+rev;

                string AjaxData = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?" + AjaxPostUrl;

                //AjaxData=AjaxData.Replace("%5B%5D%","[]");

                //AjaxData = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=%7B%22view%22%3A%22list%22%2C%22encoded_query%22%3A%22likers(203621737645)%22%2C%22encoded_title%22%3A%22WyJQZW9wbGUrd2hvK2xpa2UrIix7InRleHQiOiJDYXJ0aWVyIiwidWlkIjoyMDM2MjE3Mzc2NDUsInR5cGUiOiJwYWdlIn1d%22%2C%22ref%22%3A%22unknown%22%2C%22logger_source%22%3A%22www_main%22%2C%22typeahead_sid%22%3A%22%22%2C%22tl_log%22%3Afalse%2C%22impression_id%22%3A%22926a2099%22%2C%22filter_ids%22%3A%7B%22100004071856282%22%3A100004071856282%2C%22100000654216535%22%3A100000654216535%2C%22100001078900555%22%3A100001078900555%7D%2C%22experience_type%22%3A%22grammar%22%2C%22exclude_ids%22%3A[]%2C%22cursor%22%3A%22AbpkbBn79vniCzB7MnIezHOaV1ChHhpDJDwGVnNePrHrNAcKAw-KxStC_fN3Zh3oSq0n6r-1QnwO_pU3IQvcr6ZPSggjR9Qr9TKzb37gQIsV22e62XqS6aqE_-REXEgGXWjarkkOClbl4VkBsRQjL3mgt8hkWTfyHTHAK_kS5U66YN-1meqNPXjG__v-gCWGb6Y%22%2C%22ads_at_end%22%3Atrue%7D&__user=100004080771046&__a=1&__dyn=7n8ahyj35CCzpQ9UmWWiKaEwlyp8y&__req=8";


                string REsponse = objGlobusHttpHelper.getHtmlfromUrl(new Uri(AjaxData));

                //string AjaxUrl = "https://www.facebook.com/ajax/pagelet/generic.php/BrowseScrollingSetPagelet?data=%7B%22view%22%3A%22list%22%2C%22encoded_query%22%3A%22likers(203621737645)%22%2C%22encoded_title%22%3A%22WyJQZW9wbGUrd2hvK2xpa2UrIix7InRleHQiOiJDYXJ0aWVyIiwidWlkIjoyMDM2MjE3Mzc2NDUsInR5cGUiOiJwYWdlIn1d%22%2C%22ref%22%3A%22unknown%22%2C%22logger_source%22%3A%22www_main%22%2C%22typeahead_sid%22%3A%22%22%2C%22tl_log%22%3Afalse%2C%22impression_id%22%3A%2275a74068%22%2C%22filter_ids%22%3A%7B%22100004071856282%22%3A100004071856282%2C%22100000654216535%22%3A100000654216535%2C%22100001078900555%22%3A100001078900555%7D%2C%22experience_type%22%3A%22grammar%22%2C%22exclude_ids%22%3A[]%2C%22cursor%22%3A%22Abq0WZENKC_EWt7PPJEGKlJufYHzOHJHK6NV2E5faTQJ3edoRTGrlQR0qy-TYJih1nvVthKwha8fS-ywcOtdK3vXVEkqgIuHioYS8V7tg_Tj1ripYSg4ngHc8kjTPlgSFrC-9uBoVg5e6C5QVBz9X-WA%22%2C%22ads_at_end%22%3Atrue%7D&__user=100004080771046&__a=1&__dyn=7n8ahyj35CCzpQ9UmWWiKaEwlyp8y&__req=12";


                //string data = Uri.UnescapeDataString("filter_ids%22%3A%7B%22100004071856282%22%3A100004071856282%7D%2C%22");
                //string REsponse1 = objGlobusHttpHelper.getHtmlfromUrl(new Uri(AjaxUrl));

                AjaxResponse = REsponse;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return AjaxResponse;
        }
   
        public static string GetFieldForPostData(string Responce, string FilterFiled)
        {
            string Data = string.Empty;
            try
            {
                string[] data = System.Text.RegularExpressions.Regex.Split(Responce, FilterFiled);
                foreach (var item in data)
                {
                    if (item.StartsWith(":"))
                    {
                        string data2 = item.Substring(item.IndexOf(":\""), item.IndexOf("\",") - item.IndexOf(":\"")).Replace(":\"", "").Trim();
                        Data = data2;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Data;
        }

        public static string GetFieldForPostData2(string Responce, string FilterFiled)
        {
            string Data = string.Empty;
            try
            {
                string[] data = System.Text.RegularExpressions.Regex.Split(Responce, FilterFiled);
                foreach (var item in data)
                {
                    if (item.StartsWith(":"))
                    {
                        string data2 = item.Substring(item.IndexOf(":"), item.IndexOf("}") - item.IndexOf(":")).Replace(":", "").Trim();
                        Data = data2;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Data;
        }

        public static string GetFieldForPostData3(string Responce, string FilterFiled)
        {
            string Data = string.Empty;
            try
            {
                string[] data = System.Text.RegularExpressions.Regex.Split(Responce, FilterFiled);
                foreach (var item in data)
                {
                    if (item.StartsWith(":"))
                    {
                        string data2 = item.Substring(item.IndexOf(":"), item.IndexOf(",\"") - item.IndexOf(":")).Replace(":", "").Trim();
                        Data = data2;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Data;
        }

        public static string GetFieldUserForPostData(string Responce, string FilterFiled)
        {
            string Data = string.Empty;
            try
            {
                string[] data = System.Text.RegularExpressions.Regex.Split(Responce, FilterFiled);
                foreach (var item in data)
                {
                    if (item.StartsWith(":"))
                    {
                        string data2 = item.Substring(item.IndexOf(":\""), item.IndexOf("\",") - item.IndexOf(":\"")).Replace(":\"", "").Trim();
                        Data = data2;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Data;
        }

        public static string GetFieldFilterIdsForPostData(string Responce, string FilterFiled)
        {
            string Data = string.Empty;
            try
            {

                ChilkatHttpHelpr objChilkatHttpHelpr = new ChilkatHttpHelpr();


                string[] data = System.Text.RegularExpressions.Regex.Split(Responce, FilterFiled);
                foreach (var item in data)
                {
                    if (item.StartsWith(":"))
                    {
                        string data2 = item.Substring(item.IndexOf(":{"), item.IndexOf("},") - item.IndexOf(":{")).Replace(":{", "").Trim();
                        Data = data2;

                        List<string> hrefLink = objChilkatHttpHelpr.GetHrefFromString(Data);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return Data;
        }

        public static string GetSbstringData(string Response, string StartPoint, string LastPoint)
        {
            string Data = string.Empty;

            try
            {
                Data = Response.Substring(Response.IndexOf(StartPoint), Response.IndexOf(LastPoint) - Response.IndexOf(StartPoint)).Replace(StartPoint, "").Trim();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return Data;
        }

        //for search facebar people Search .

        public static string exportFilePathSearchPeople
        {
            get;
            set;
        }

        public List<string> SearchUserSearchSearchKeyword = new List<string>();
       
    }
}
