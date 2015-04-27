using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using faceboardpro;
using System.Text.RegularExpressions;
using System.Threading;
using Globussoft;
using OpenPOP.POP3;
using BaseLib;
using Emails;
using System.IO;
using System.Data;




namespace Accounts
{
    public class AccountManager
    {

        public Events accountCreationEvent = null;

        #region Global Variables For Account Creator

        public bool isStopAccountCreator = false;

        public List<Thread> lstAccountCreatorThreads = new List<Thread>();
        public static List<string> lstRemoveDuplicate = new List<string>();
        public static string FilePath = string.Empty;

    

        public int countCreatedAccount = 0;

        #endregion

        #region Global Variables Manage Profiles

        readonly object lockr_ThreadControllerManageProfiles = new object();

        public bool isStopManageProfiles = false;

        int count_ThreadControllerManageProfiles = 0;
        int counterCities = 0;
        int counterHomeTown = 0;
        int counterReligion = 0;
        int counterAboutMe = 0;
        int counterCollege = 0;
        int counterHighSchool = 0;
        int counterEmployer = 0;
        int counterActivities = 0;
        int counterInterests = 0;
        int counterMusic = 0;
        int counterMovies = 0;
        int counterBooks = 0;
        int counterQuotationss = 0;
        int counterFavoriteSports = 0;
        int counterFavoriteTeams = 0;
        int counterFamily = 0;
        int counterRole = 0;
        int counterLanguage = 0;
        int counterprofilePic = 0;

        public List<Thread> lstManageProfilesThreads = new List<Thread>();
        public List<string> lstCitiesManageProfiles = new List<string>();
        public List<string> lstBirthdaysManageProfiles = new List<string>();
        public List<string> lstLanguagesManageProfiles = new List<string>();
        public List<string> lstAboutMeManageProfiles = new List<string>();
        public List<string> lstEmployerManageProfiles = new List<string>();
        public List<string> lstCollegeManageProfiles = new List<string>();
        public List<string> lstHighSchoolManageProfiles = new List<string>();
        public List<string> lstReligionManageProfiles = new List<string>();
        public List<string> lstFamilyNamesManageProfiles = new List<string>();
        public List<string> lstHomeTownManageProfiles = new List<string>();
        public List<string> lstActivitiesManageProfiles = new List<string>();
        public List<string> lstInterestsManageProfiles = new List<string>();
        public List<string> lstMoviesManageProfiles = new List<string>();
        public List<string> lstMusicManageProfiles = new List<string>();
        public List<string> lstBooksManageProfiles = new List<string>();
        public List<string> lstRolesManageProfiles = new List<string>();
        public List<string> lstFavoriteSportsManageProfiles = new List<string>();
        public List<string> lstFavoriteTeamsManageProfiles = new List<string>();
        public List<string> lstQuotationsManageProfiles = new List<string>();
        public List<string> lstProfilePicsManageProfiles = new List<string>();
        public List<string> lstCoverPicsManageProfiles = new List<string>();


        #endregion

        #region Get/set Manage Profiles

        public string UpdateOnlyProfile
        {
            get;
            set;
        }

        public static int NoOfThreads
        {
            get;
            set;
        }

        public static string AccountExprotFilePath
        {
            get;
            set;
        }

        #endregion

        public AccountManager()
        {
            accountCreationEvent = new Events();
        }

        public void CreateAccount(object obj)//(FacebookUser fbUser)
        {
            string email = string.Empty;

            try
            {
                if (isStopAccountCreator)
                {
                    return;
                }

                lstAccountCreatorThreads.Add(Thread.CurrentThread);
                lstAccountCreatorThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;

                // RaiseEvent(new string[] { "hi", "hello"});
                    
                string dob = string.Empty;
                string postformid = string.Empty;
                string lsd = string.Empty;
                string reginstance = string.Empty;
                string firstname = string.Empty;
                string lastname = string.Empty;
                string regemail = string.Empty;
                string regemailconfirmation = string.Empty;
                string regpasswd = string.Empty;
                string sex = string.Empty;
                string birthdaymonth = string.Empty;
                string birthdayday = string.Empty;
                string birthdayyear = string.Empty;
                string captchapersistdata = string.Empty;
                string captchasession = string.Empty;
                string extrachallengeparams = string.Empty;
                string recaptchapublickey = string.Empty;
                string authppisgnoncett = null;
                string authp = string.Empty;
                string psig = string.Empty;
                string nonce = string.Empty;
                string tt = string.Empty;
                string time = string.Empty;
                string challenge = string.Empty;
                string captchasummit = string.Empty;
                string post_form_id = string.Empty;

                string Email = string.Empty;
                string Password = string.Empty;

                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUsername = string.Empty;
                string proxyPassword = string.Empty;

                List<string> listFirstName = new List<string>();
                List<string> listLastName = new List<string>();
                List<string> listProxies = new List<string>();

                #region CommentedCode

                //try
                //{
                //    Email = email.Split(':')[0];
                //    Password = email.Split(':')[1];
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //} 
                #endregion

                Array paramsArray = new object[10];
                paramsArray = (Array)obj;

                try
                {
                    email = (string)paramsArray.GetValue(0);
                    listFirstName = (List<string>)paramsArray.GetValue(1);
                    listLastName = (List<string>)paramsArray.GetValue(2);
                    listProxies = (List<string>)paramsArray.GetValue(3);
                    sex = (string)paramsArray.GetValue(4);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                }

                GlobusHttpHelper httpHelper = new GlobusHttpHelper();
                string pageSource = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));//fbhomehtml

                GetCaptchaImageMultiModified(email, ref httpHelper, ref post_form_id, ref lsd,
                  ref reginstance,
                  ref firstname,
                  ref lastname,
                  ref regemail,
                  ref regemailconfirmation,
                  ref regpasswd,
                  ref sex,
                  ref birthdaymonth,
                  ref birthdayday,
                  ref birthdayyear,
                  ref captchapersistdata,
                  ref captchasession,
                  ref extrachallengeparams,
                  ref recaptchapublickey,
                  ref authppisgnoncett,
                  ref authp,
                  ref psig,
                  ref nonce,
                  ref tt,
                  ref time,
                  ref challenge,
                  ref captchasummit, ref dob, listFirstName, listLastName, sex);

                if (email.Split(':').Length > 5)
                {
                    try
                    {
                        proxyAddress = email.Split(':')[2];
                        proxyPort = email.Split(':')[3];
                        proxyUsername = email.Split(':')[4];
                        proxyPassword = email.Split(':')[5];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }

                }
                else if (email.Split(':').Length == 4)
                {
                    try
                    {
                        proxyAddress = email.Split(':')[2];
                        proxyPort = email.Split(':')[3];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }

                if (string.IsNullOrEmpty(proxyPort))
                {
                    proxyPort = "0";
                }

                GlobusLogHelper.log.Info("Start Account Creation Via Email : " + regemail);
                GlobusLogHelper.log.Debug("Start Account Creation Via Email : " + regemail);

                try
                {
                    regemail = Uri.UnescapeDataString(regemail);
                    regemail = Uri.EscapeDataString(regemail);
                    regemailconfirmation = Uri.UnescapeDataString(regemailconfirmation);
                    regemailconfirmation = Uri.EscapeDataString(regemailconfirmation);
                    //reg_email__ = reg_email__.Replace("+", "%2B");
                    //reg_email_confirmation__ = reg_email_confirmation__.Replace("+", "%2B");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (sex == "Male")
                {
                    sex = (2).ToString();
                }
                if (sex == "Female")
                {
                    sex = (1).ToString();
                }
               // Password = reg_passwd__;
                //string url_Registration = "https://www.facebook.com/ajax/register.php?lsd=AVocM3fL&firstname=andrew&lastname=mathews&reg_email__=dustiveloz%2B67%40hotmail.com&reg_email_confirmation__=dustiveloz%2B3%40hotmail.com&reg_passwd__=QwErTyAsDfG12&birthday_month=5&birthday_day=5&birthday_year=1984&sex=1&referrer=&asked_to_login=&terms=on&ab_test_data=&reg_instance="+reg_instance+"&contactpoint_label=email_only&locale=en_US&abtest_registration_group=1&validate_mx_records=1&captcha_persist_data="+captcha_persist_data+"&captcha_session="+captcha_session+"&extra_challenge_params="+extra_challenge_params+"&recaptcha_type=password&captcha_response=&ignore=captcha%7Cpc&__user=0&__a=1&__dyn=7wiU&__req=jsonp_2&__adt=2";
                string urlRegistration = FBGlobals.Instance.fbsignupurl + postformid + "&lsd=" + lsd + "&reg_instance=" + reginstance + "&locale=en_US&terms=on&abtest_registration_group=1&referrer=&md5pass=&validate_mx_records=1&asked_to_login=0&ab_test_data=AAAAAAAAAAAA%2FA%2FAAAAA%2FAAAAAAAAAAAAAAAAAAAA%2FAA%2FfAAfABAAD&firstname=" + firstname + "&lastname=" + lastname + "&reg_email__=" + regemail + "&reg_email_confirmation__=" + regemail + "&reg_passwd__=" + regpasswd + "&sex=" + sex + "&birthday_month=" + birthdaymonth + "&birthday_day=" + birthdayday + "&birthday_year=" + birthdayyear + "&captcha_persist_data=" + captchapersistdata + "&captcha_session=" + captchasession + "&extra_challenge_params=" + extrachallengeparams + "&recaptcha_type=password&captcha_response=" + "" + "&ignore=captcha%7Cpc&__user=0&__a=1&__adt=3";
                string resRegistration = httpHelper.getHtmlfromUrl(new Uri(urlRegistration));

                if (resRegistration.Contains(FBGlobals.Instance.registrationSuccessString))
                {

                 
                    countCreatedAccount++;
                    GlobusLogHelper.log.Info("Account Created Successfully via email " + Uri.UnescapeDataString(regemail));
                    GlobusLogHelper.log.Debug("Account Created Successfully via email " + Uri.UnescapeDataString(regemail));

                    #region Account Insert Into AccountTable

                    // Export text file

                    if (!string.IsNullOrEmpty(AccountExprotFilePath))
                    {
                        try
                        {
                             string Proxy = proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword;
                                
                            string dateOfBirth = birthdaymonth + ":" + birthdayday + ":" + birthdayyear;

                            string CSVHeader = "Email" + "," + "Password" + "," + "FirstName" + ", " + "LastName" + "," + "DateOfBirth" + "," + "sex"+ "," + "proxy";

                            string CSVData =Uri.UnescapeDataString(regemail) + "," + regpasswd + "," + firstname + "," + lastname + "," + dateOfBirth + "," + sex.Replace("1", "female").Replace("2", "Male") + "," + Proxy;

                            Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSVData, AccountExprotFilePath);

                            GlobusLogHelper.log.Info("Data Saved IN Text File");

                            GlobusLogHelper.log.Debug("Data Saved IN Text File");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    //

                    DataSet ds = new DataSet();
                    RaiseEvent(ds, new string[] { Uri.UnescapeDataString(regemail), regpasswd, proxyAddress, proxyPort, proxyUsername, proxyPassword });

                    #endregion

                    if(string.IsNullOrEmpty(Email))
                    {
                        Email = regemail;
                        Password = regpasswd;
                    }

                    FinishRegistration(resRegistration, Email, Password, proxyAddress, proxyPort, proxyUsername, proxyPassword, ref dob, ref httpHelper);

                }
                if (resRegistration.Contains(FBGlobals.Instance.registrationErrorString))
                {
                    GlobusLogHelper.log.Info("Couldn't Create Account  via email " + Uri.UnescapeDataString(regemail));
                    GlobusLogHelper.log.Debug("Couldn't Create Account  via email " + Uri.UnescapeDataString(regemail));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Account Creation Completed Via Email : " + email);
            GlobusLogHelper.log.Debug("Account Creation Completed Via Email : " + email);

        }

        private void FinishRegistration(string responseRegistration, string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, ref string DOB, ref GlobusHttpHelper HttpHelper)
        {
            try
            {
                #region MyRegion
                // Code For ajay+1@gmail.com
                //if (email.Contains("+") || email.Contains("%2B"))
                //{
                //    try
                //    {
                //        string replacePart = email.Substring(email.IndexOf("+"), (email.IndexOf("@", email.IndexOf("+")) - email.IndexOf("+"))).Replace("+", string.Empty);
                //        email = email.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                //    }
                //    catch
                //    {
                //    }
                //}

                //string registration_succeeded = "";
                //string alredy_exist = ""; 
                #endregion

                string registrationstatus = "";
                if (!string.IsNullOrEmpty(responseRegistration) && responseRegistration.Contains("registration_succeeded"))
                {
                    //AddToListBox("Registration Succeeded With User Name : " + email);
                    try
                    {
                        string res = HttpHelper.getHtmlfromUrlProxy(new Uri("http://www.facebook.com/c.php?email=" + email), proxyAddress, Convert.ToInt32(proxyPort), proxyUser, proxyPassword);

                        registrationstatus = "registration_succeeded";

                        if (!string.IsNullOrEmpty(email))
                        {
                            // GlobusFileHelper.AppendStringToTextfileNewLine(email + ":" + password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUser + ":" + proxyPassword + "<>" + DOB, Path.Combine(Globals.FD_DesktopPath, "CreatedAccountsPlusType.txt"));
                        }
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    VerifiyAccount(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref HttpHelper, registrationstatus);
                }
                //else if (!string.IsNullOrEmpty(responseRegistration) && responseRegistration.Contains("It looks like you already have an account on Facebook"))
                else if (responseRegistration.Contains("It looks like you already have an account on Facebook"))
                {
                    //AddToListBox("It looks like you already have an account on Facebook");
                    try
                    {
                        string res = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationPhpEmail + email));       //"http://www.facebook.com/c.php?email="                     
                        registrationstatus = "alredy_exist";
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex.StackTrace);
                    }

                    VerifiyAccount(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref HttpHelper, registrationstatus);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void VerifiyAccount(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
        {
            if (email.Contains("%"))
            {
                email = email.Replace("%40","@");
            }

            try
            {

                if (email.Contains("@gmail"))
                {
                    EmailFactory objEmailFactory = new GmailFactory();
                    objEmailFactory.readEmail(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref  HttpHelper, registrationstatus);

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }

                else if (email.Contains("@yahoo"))
                {
                    EmailFactory objEmailFactory = new YahooFactory();
                    objEmailFactory.readEmail(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref  HttpHelper, registrationstatus);

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }

                else if (email.Contains("@hotmail"))
                {
                    EmailFactory objEmailFactory = new HotmailFactory();
                    objEmailFactory.readEmail(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref  HttpHelper, registrationstatus);

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }

                else if (email.Contains("@live"))
                {
                    EmailFactory objEmailFactory = new LiveFactory();
                    objEmailFactory.readEmail(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref  HttpHelper, registrationstatus);

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }

                else if (email.Contains("@aol"))
                {
                    EmailFactory objEmailFactory = new AolFactory();
                    objEmailFactory.readEmail(email, password, proxyAddress, proxyPort, proxyUser, proxyPassword, DOB, ref  HttpHelper, registrationstatus);

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }

                else
                {
                    GlobusLogHelper.log.Info("Email : " + email + " Not Recognised !");

                    int delayInSeconds = Utils.GenerateRandom(minDelayAccountVerification * 1000, maxDelayAccountVerification * 1000);
                    GlobusLogHelper.log.Info("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    GlobusLogHelper.log.Debug("Delaying for " + delayInSeconds / 1000 + " Seconds With email : " + email);
                    Thread.Sleep(delayInSeconds);
                }


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
            finally
            {
                //Write to text file
                //Also insert in Database
                try
                {
                    if (registrationstatus == "registration_succeeded")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(realEmail + ":" + password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUser + ":" + proxyPassword + "<>" + DOB, Path.Combine(Globals.FD_DesktopPath, "CreatedAccounts.txt"));
                        // DataBaseHandler.InsertQuery("Insert into tb_FBAccount values('" + realEmail + "','" + password + "','" + proxyAddress + "','" + proxyPort + "','" + proxyUser + "','" + proxyPassword + "','" + "" + "','" + "" + "','" + AccountStatus.Status(ProfileStatus.AccountCreated) + "')", "tb_FBAccount");
                    }
                    if (registrationstatus == "alredy_exist")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(realEmail + ":" + password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUser + ":" + proxyPassword + "<>" + DOB, Path.Combine(Globals.FD_DesktopPath, "AlreadyCreatedAccounts.txt"));

                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);

                }
            }
        }


        /// <summary>
        /// Returns Captcha Image from Facebook
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public void GetCaptchaImageMultiModified(string email, ref GlobusHttpHelper HttpHelper, ref string post_form_id, ref string lsd,
           ref string reg_instance,
           ref string firstname,
           ref string lastname,
           ref string reg_email__,
           ref string reg_email_confirmation__,
           ref string reg_passwd__,
           ref string sex,
           ref string birthday_month,
           ref string birthday_day,
           ref string birthday_year,
           ref string captcha_persist_data,
           ref string captcha_session,
           ref string extra_challenge_params,
           ref string recaptcha_public_key,
           ref string authp_pisg_nonce_tt,
           ref string authp,
           ref string psig,
           ref string nonce,
           ref string tt,
           ref string time,
           ref string challenge,
           ref string CaptchaSummit, ref string DOB, List<string> listFirstName, List<string> listLastName, string SexSelect)
        {

            try
            {
                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUsername = string.Empty;
                string proxyPassword = string.Empty;

                string FirstName = string.Empty;
                string LastName = string.Empty;
                string Email = string.Empty;
                string Password = string.Empty;
                //string DOB = string.Empty;

                Email = email.Split(':')[0];
                Password = email.Split(':')[1];

                if (email.Split(':').Length > 5)
                {
                    proxyAddress = email.Split(':')[2];
                    proxyPort = email.Split(':')[3];
                    proxyUsername = email.Split(':')[4];
                    proxyPassword = email.Split(':')[5];
                    //AddToListBox("Setting proxy " + proxyAddress + ":" + proxyPort);
                }
                else if (email.Split(':').Length == 4)
                {
                    //MessageBox.Show("Private proxies not loaded with emails \n Accounts will be created with public proxies");
                    proxyAddress = email.Split(':')[2];
                    proxyPort = email.Split(':')[3];
                }

                FirstName = firstname;
                LastName = lastname;

                {
                    #region Random First & Last Names
                    if (listFirstName.Count > 0)
                    {
                        try
                        {
                            FirstName = listFirstName[Utils.GenerateRandom(0, listFirstName.Count)];
                        }
                        catch (Exception ex)
                        {
                            FirstName = string.Empty;
                        }
                    }
                    if (listLastName.Count > 0)
                    {
                        try
                        {
                            LastName = listLastName[Utils.GenerateRandom(0, listLastName.Count)];
                        }
                        catch (Exception ex)
                        {
                            LastName = string.Empty;
                        }
                    }
                    #endregion
                }

                #region Get Params


                int intProxyPort = 80;
                Regex IdCheck = new Regex("^[0-9]*$");

                if (Utils.IsNumeric(proxyPort))
                {
                    intProxyPort = int.Parse(proxyPort);
                }


                string pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri("http://www.facebook.com/"), proxyAddress, intProxyPort, proxyUsername, proxyPassword);


                #region CSS, JS, & Pixel requests to avoid Socket Detection

                ///JS, CSS, Image Requests
                //RequestsJSCSSIMG.RequestJSCSSIMG(pageSource, ref HttpHelper);


                ///Pixel request
                string reg_instanceValue = FBUtils.GetParamValue(pageSource, "reg_instance");
                //string asyncSignal = Utils.GenerateRandom(3000, 4000).ToString();
                string asyncSignal = string.Empty;
                try
                {
                    asyncSignal = Utils.GenerateRandom(3000, 8000).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                }
                string pixel = HttpHelper.getHtmlfromUrl(new Uri("http://pixel.facebook.com/ajax/register/logging.php?action=form_focus&reg_instance=" + reg_instanceValue + "&asyncSignal=" + asyncSignal + "&__user=0"));
                #endregion

                if (pageSource.Contains("post_form_id"))
                {
                    try
                    {
                        string post_id = pageSource.Substring(pageSource.IndexOf("post_form_id"), 200);
                        string[] Arr1 = post_id.Split('"');
                        post_form_id = Arr1[2];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (pageSource.Contains("lsd"))
                {
                    try
                    {
                        string lsd_val = pageSource.Substring(pageSource.IndexOf("lsd"), 100);
                        string[] Arr_lsd = lsd_val.Split('"');
                        lsd = Arr_lsd[2];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (pageSource.Contains("reg_instance"))
                {
                    try
                    {
                        string reg_instance_val = pageSource.Substring(pageSource.IndexOf("reg_instance"), 200);
                        string[] Arr_reg = reg_instance_val.Split('"');
                        reg_instance = Arr_reg[4];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                firstname = FirstName.Replace(" ", "%20");
                lastname = LastName.Replace(" ", "%20");
                reg_email__ = Email.Replace("@", "%40");
                reg_email_confirmation__ = Email.Replace("@", "%40");
                //reg_passwd__ = Password.Replace("@", "%40");
                reg_passwd__ = Uri.EscapeDataString(Password);//.Replace("@", "%40");
                sex = SexSelect;
                birthday_month = Utils.GenerateRandom(1, 12).ToString();
                birthday_day = Utils.GenerateRandom(1, 28).ToString();
                birthday_year = Utils.GenerateRandom(1980, 1990).ToString();
                DOB = birthday_day + ":" + birthday_month + ":" + birthday_year;

                if (pageSource.Contains("captcha_persist_data"))
                {
                    try
                    {
                        string captcha_persist_data_val = pageSource.Substring(pageSource.IndexOf("captcha_persist_data"), 500);
                        string[] Arr_captcha_persist_data_val = captcha_persist_data_val.Split('"');
                        captcha_persist_data = Arr_captcha_persist_data_val[4];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (pageSource.Contains("captcha_session"))
                {
                    try
                    {
                        string captcha_session_val = pageSource.Substring(pageSource.IndexOf("captcha_session"), 200);
                        string[] Arr_captcha_session_val = captcha_session_val.Split('"');
                        captcha_session = Arr_captcha_session_val[4];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (pageSource.Contains("extra_challenge_params"))
                {
                    try
                    {
                        string extra_challenge_params_val = pageSource.Substring(pageSource.IndexOf("extra_challenge_params"), 500);
                        string[] Arr_extra_challenge_params_val = extra_challenge_params_val.Split('"');
                        authp_pisg_nonce_tt = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = extra_challenge_params.Replace("=", "%3D");
                        extra_challenge_params = extra_challenge_params.Replace("&amp;", "%26");
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
        

        public void updateAccountProfile(FacebookUser fbUser)
        {

        }
        public bool emailVerifyAccount(FacebookUser fbUser)
        {
            return true;

        }
        /// <summary>
        /// Checks the status of the Facebook Account.
        /// </summary>
        /// <param name="fbUser"></param>
        /// <returns></returns>
        public bool checkAccount(FacebookUser fbUser)
        {
            return true;
        }

        /// <summary>
        /// Checks if the email is a valid fb account.
        /// </summary>
        /// <param name="fbUser">Object of FacebookUser</param>
        /// <returns>true if the email is a fb user</returns>

        public bool checkifFBAccount(FacebookUser fbUser)
        {
            return true;
        }

        //public static Events accountCreationEvent = new Events();

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                accountCreationEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

        public void ModifyProfile(ref FacebookUser fbUser)
        {
            try
            {

                GlobusLogHelper.log.Info("Starting Change Fb TimeLine ...!");
                GlobusLogHelper.log.Debug("Starting Change Fb TimeLine ...!");
               

                if (UpdateOnlyProfile == "Profile Details")
                {
                    GlobusLogHelper.log.Info("Starting Update Education ...!");
                    GlobusLogHelper.log.Debug("Starting Update Education ...!");
                    UpdateEducationTimeLine(ref fbUser);

                    GlobusLogHelper.log.Info("Starting Update Living ...!");
                    GlobusLogHelper.log.Debug("Starting Update Living ...!");
                    UpdateLiving(ref fbUser);

                    GlobusLogHelper.log.Info("Starting Update Favorites TimeLine ...!");
                    GlobusLogHelper.log.Debug("Starting Update Favorites TimeLine ...!");
                    UpdateFavoritesTimeLine(ref fbUser);

                    GlobusLogHelper.log.Info("Starting Update About You TimeLine ...!");
                    GlobusLogHelper.log.Debug("Starting Update About You TimeLine ...!");
                    UpdateAboutYouTimeLine(ref fbUser);

                    GlobusLogHelper.log.Info("Starting Update Work And Education ...!");
                    GlobusLogHelper.log.Debug("Starting Update Work And Education ...!");
                    UpdateWorkAndEducation(ref fbUser);

                    GlobusLogHelper.log.Info("Starting Update Basic Info ...!");
                    GlobusLogHelper.log.Debug("Starting Update Basic Info ...!");
                    UpdateBasicInfo(ref fbUser);
                    ChangeFbTimeLine(ref fbUser);
                }

                if (UpdateOnlyProfile == "Profile Pic")
                {
                    GlobusLogHelper.log.Info("Starting Update Profile Pic ...!");
                    GlobusLogHelper.log.Debug("Starting Update Profile Pic ...!");
                    UploadProfileImage(ref fbUser);
                }

                if (UpdateOnlyProfile == "CoverPic")
                {
                    GlobusLogHelper.log.Info("Starting Update AddaCover ...!");
                    SetAddaCover(ref fbUser, lstCoverPicsManageProfiles);
                }


            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void SetAddaCover(ref FacebookUser fbUser, List<string> lstCoverPics)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Starting AddaCover With User Name : " + fbUser.username);
                GlobusLogHelper.log.Debug("Starting AddaCover With User Name : " + fbUser.username);

                List<string> lsttempCoverPics = new List<string>();
                string UsreId = string.Empty;
                string imagePath = string.Empty;

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

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

                lstCoverPics = lstCoverPics.Distinct().ToList();


                //if (AddaCoverUnique == true)
                //{
                //    lock (QueLocker)
                //    {
                //        try
                //        {
                //            imagePath = lstqueueimages.Dequeue();
                //        }
                //        catch { }
                //    }
                //}
                // else
                {
                    //Log("Image should be at least 399 pixels wide");

                    imagePath = lstCoverPics[new Random().Next(0, lstCoverPics.Count)];
                }

                string status = string.Empty;
                bool isAddaCover = HttpHelper.AddaCover(ref HttpHelper, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);

                #region MyRegion
                //while (status == "Please choose an image that's at least 399 pixels wide" && AddaCoverUnique == true)
                //{
                //    try
                //    {
                //        Log("Image : " + imagePath);
                //        Log("Image should be at least 399 pixels wide");
                //        if (AddaCoverUnique == true)
                //        {
                //            lock (QueLocker)
                //            {
                //                try
                //                {
                //                    imagePath = lstqueueimages.Dequeue();
                //                }
                //                catch { }
                //            }
                //        }
                //        //else
                //        //{
                //        //    imagePath = lstCoverPics[new Random().Next(0, lstCoverPics.Count)];
                //        //}
                //        //imagePath = lstCoverPics[0];
                //        //lstCoverPics.RemoveAt(0);
                //        status = string.Empty;
                //        isAddaCover = HttpHelper.AddaCover(ref HttpHelper, UserName, Password, imagePath, proxyAddress, proxyPort, proxyUsername, proxyPassword, ref status);
                //        lock (QueLocker)
                //        {
                //            try
                //            {
                //                if (lstqueueimages.Count < 1)
                //                {
                //                    Log("No Images For Addcover For UserName : " + UserName);
                //                    return;
                //                }
                //            }
                //            catch { }
                //        }
                //    }
                //    catch { }
                //} 
                #endregion

                if (isAddaCover)
                {
                    GlobusLogHelper.log.Info("Image : " + imagePath + " Added Successfully With User Name : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Image : " + imagePath + " Added Successfully With User Name : " + fbUser.username);
                }
                else
                {
                    GlobusLogHelper.log.Info("Image : " + imagePath + " " + status + " With User Name : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Image : " + imagePath + " " + status + " With User Name : " + fbUser.username);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            finally
            {
                GlobusLogHelper.log.Info("Process Completed With : " + fbUser.username);
            }

        }

        public void UploadProfileImage(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Upload Profile Image.. With Username >>> " + fbUser.username);

                string localImagePath = lstProfilePicsManageProfiles[Utils.GenerateRandom(0, lstProfilePicsManageProfiles.Count)].ToString();

                if (httpHelper.MultiPartImageUpload(ref httpHelper, fbUser.username, fbUser.password, localImagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword))
                {
                    GlobusLogHelper.log.Info("Profile Pic Uploaded : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Profile Pic Uploaded : " + fbUser.username);
                    //Globals.CreateFolder(Globals.FD_DesktopPath);
                    //Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(facebooker.Username + ":" + facebooker.Password, Globals.FD_DesktopPath + "\\ProfileImageFile.txt");
                }
                else
                {
                    // Log("Error in Pic Uploading : " + Username); 
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Upload Profile Image. With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Upload Profile Image. With Username >>> " + fbUser.username);

        }

        public void UpdateBasicInfo(ref FacebookUser fbUser) //   BaseLib.ChilkatHttpHelpr HttpHelper
        {
            try
            {

                UpdateBasicInfoFacebookerTimeLine(ref fbUser);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateBasicInfoFacebookerTimeLine(ref FacebookUser fbUser) //BaseLib.ChilkatHttpHelpr HttpHelper
        {

            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update Basic Info.. With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update Basic Info.. With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(PageSrc))
                {
                    try
                    {
                        PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileBasicTimeLine = string.Empty;
                try
                {
                    PageSrcProfileBasicTimeLine = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));                 //"https://www.facebook.com/profile.php?id="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileBasicTimeLine))
                {
                    try
                    {
                        PageSrcProfileBasicTimeLine = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));                //"https://www.facebook.com/profile.php?id="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string PageSrcProfileBasicInfoEdit = string.Empty;
                try
                {
                    PageSrcProfileBasicInfoEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileBasicInfo + UserId));       // "https://www.facebook.com/ajax/timeline/edit_profile/basic_info.php?__a=1&__user="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileBasicInfoEdit))
                {
                    try
                    {
                        PageSrcProfileBasicInfoEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileBasicInfo + UserId));           //"https://www.facebook.com/ajax/timeline/edit_profile/basic_info.php?__a=1&__user="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                //**Get Post Data******************************************

                string fb_dtsg = string.Empty;
                string birthday_month = string.Empty;
                string birthday_day = string.Empty;
                string birthday_year = string.Empty;


                string sex = string.Empty;
                string religion_text = string.Empty;
                string text_languages = string.Empty;
                string School_text = fbUser.college;

                birthday_month = fbUser.birthdaymonth;
                birthday_day = fbUser.birthdayday;
                birthday_year = fbUser.birthdayyear;
                sex = fbUser.sex;
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);
                //strReligion = "i am indian";
                religion_text = fbUser.religion.Replace(" ", "%20");
                text_languages = fbUser.language;
                

                //for Religion 

                string religionPostDada = "fb_dtsg="+fb_dtsg+"&religion=109523995740640&religion_text="+religion_text+"&audience[8787645733][value]=50&religion_desc=&save=1&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDh8St2u6aOQUGyyEC9ACwKyaF299qzCAjFDw&__req=1e&ttstamp=265816811410995115120&__rev=1142402";

                string ResposceReligion = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/save/religion/"), religionPostDada, "");

                Thread.Sleep(1*2*100);



                //b'day

                string profileEditPostData = "field_type=birthday&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=t&fb_dtsg="+fb_dtsg+"&ttstamp=265816811410995115120&__rev=1142402";
                string ResposceprofileEditPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/forms/"), profileEditPostData, "");
                Thread.Sleep(1 * 2 * 100);

                string DateOfBirthPostData = "fb_dtsg="+fb_dtsg+"&audience[8787510733][value]=10&audience[8787805733][value]=10&birthday_month=" + birthday_month + "&birthday_day=" + birthday_day + "&birthday_year=" + birthday_year + "&birthday_confirmation=1&save=1&nctr[_mod]=pagelet_basic&__user=" + UserId + "&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=m&ttstamp=265816811410995115120&__rev=1142402";

                string ResposceDateOfBirthPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/save/birthday/"), DateOfBirthPostData, "");
                Thread.Sleep(1 * 2 * 100);


                //ForGender
                if(string.IsNullOrEmpty(sex))
                {
                     sex = "1";
                }
                string GenderEditPostData = "field_type=gender&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=h&fb_dtsg="+fb_dtsg+"&ttstamp=265816811410995115120&__rev=1142402";
                string responceGenderEditPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/forms/"), GenderEditPostData, "");
                Thread.Sleep(1 * 2 * 100);
                string GenderFinalPostData = "fb_dtsg="+fb_dtsg+"&sex="+sex+"&audience[237760973066217][value]=40&sex_preferred_pronouns=1&sex_visibility=on&save=1&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=i&ttstamp=265816811410995115120&__rev=1142402";
                string responce = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/save/gender/"),GenderFinalPostData,"");

                Thread.Sleep(1 * 2 * 100);

                //for Interested_In

                string InterestedEditPostData = "field_type=interested_in&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=f&fb_dtsg="+fb_dtsg+"&ttstamp=265816811410995115120&__rev=1142402";
                string responceInterestedEditPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/forms/"), InterestedEditPostData,"");
                Thread.Sleep(1 * 2 * 100);
                string InterestedFinalPostData = "fb_dtsg="+fb_dtsg+"&meeting_sex1=on&meeting_sex2=on&audience[8787590733][value]=10&save=1&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=g&ttstamp=265816811410995115120&__rev=1142402";
                string responceInterestedFinalPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/save/interested_in/"), InterestedFinalPostData,"");

                Thread.Sleep(1 * 2 * 100);

                //for languages
           
                string GetEdit="https://www.facebook.com/profile/edit/infotab/forms/?field_type=languages&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=c&__rev=1142402";
                string PageSource = HttpHelper.getHtmlfromUrl(new Uri(GetEdit));


                string languagesFInalPostData = "fb_dtsg="+fb_dtsg+"&audience[8787625733][value]=80&languages[0]=106059522759137&text_languages[0]="+text_languages+"&save=1&nctr[_mod]=pagelet_basic&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=12&ttstamp=265816811410995115120&__rev=1142402";
                string responcelanguagesProfileEditPostData = HttpHelper.postFormData(new Uri("https://www.facebook.com/profile/edit/infotab/save/languages/"), languagesFInalPostData, "");
                Thread.Sleep(1 * 2 * 100);



                string postdataBasicInfo = "fb_dtsg=" + fb_dtsg + "&sex=" + sex + "&sex_visibility=on&birthday_month=&birthday_day=&birthday_year=&birthday_confirmation=1&birthday_visibility=1&audience[8787510733][value]=80&audience[8787590733][value]=80&audience[8787550733][value]=80&audience[8787625733][value]=80&audience[8787645733][custom_value]=50&audience[8787645733][value]=111&audience[8787640733][custom_value]=50&audience[8787640733][value]=111&meeting_sex1=on&meeting_sex2=on&status=1&partner=&anniversary_month=-1&anniversary_day=-1&anniversary_year=-1&languages[0]=106059522759137&languages[1]=113301478683221&text_languages[0]=English&text_languages[1]=American%20English&religion=0&religion_text=&politics=0&politics_text=&save=Save&__user=" + UserId + "&phstamp=";
                // 
                //string postdataBasicInfo = "fb_dtsg=" + fb_dtsg + "&sex=" + sex + "&sex_visibility=on&birthday_month=" + birthday_month + "&birthday_day=" + birthday_day + "&birthday_year=" + birthday_year + "&birthday_visibility=1&birthday_visibility=1&audience[8787510733][value]=40&audience[8787590733][value]=40&audience[8787550733][value]=80&audience[8787625733][value]=80&audience[8787645733][custom_value]=50&audience[8787645733][value]=111&audience[8787640733][custom_value]=50&audience[8787640733][value]=111&meeting_sex1=on&meeting_sex2=on&status=1&partner=&anniversary_month=-1&anniversary_day=-1&anniversary_year=-1&languages[0]=106059522759137&text_languages[0]=English&religion=" + religion_Id + "&religion_text=" + religion_text + "&religion_desc=&politics=0&politics_text=&save=Save&__user=" + UserId + "&phstamp=";              
                Thread.Sleep(1 * 2 * 100);

                string postDataUrl = FBGlobals.Instance.urlPostDataUrlBasicInfoManageProfile;
                string ResposceBasicInfo = string.Empty;
                try
                {
                    ResposceBasicInfo = HttpHelper.postFormData(new Uri(postDataUrl), postdataBasicInfo, "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ResposceBasicInfo))
                {
                    try
                    {

                        ResposceBasicInfo = HttpHelper.postFormData(new Uri(postDataUrl), postdataBasicInfo, "");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                GlobusLogHelper.log.Info("Basic Information Set for " + fbUser.username);
                GlobusLogHelper.log.Debug("Basic Information Set for " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Update Basic Info.. With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Update Basic Info.. With Username >>> " + fbUser.username);

        }

        public void UpdateWorkAndEducation(ref FacebookUser fbUser)
        {
            try
            {
                UpdateWorkTimeLine(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateWorkTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                string School_text = fbUser.college;

                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update Work.. With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update Work.. With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrc))
                {
                    try
                    {
                        PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileWorkEdit = string.Empty;
                try
                {
                    PageSrcProfileWorkEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileEduWork + UserId));            //"https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?__a=1&__user="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileWorkEdit))
                {
                    try
                    {
                        PageSrcProfileWorkEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileEduWork + UserId));       //"https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?__a=1&__user=" 
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                //* Post data for work ***********************************************

                string WorkName = string.Empty;
                string WorkId = string.Empty;

                string fb_dtsg = string.Empty;

                //WorkName = "globussoft Bhilai";
                WorkName = fbUser.employer.Replace(" ", "%20");
                //WorkName = strEmployer;

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);


                string PageSrcProfileBasicInfoEdit1 = string.Empty;
                string get_SearchIDs = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + WorkName + "&category=2200&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2201&page_categories[1]=2202&page_categories[2]=1006&page_categories[3]=1013&context=hub_work_about&services_mask=1&section=2002&sid=856701781890&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=1d";       // "https://www.facebook.com/ajax/typeahead/search.php?value=" 
                try
                {
                    PageSrcProfileBasicInfoEdit1 = HttpHelper.getHtmlfromUrl(new Uri(get_SearchIDs));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileBasicInfoEdit1))
                {
                    try
                    {
                        PageSrcProfileBasicInfoEdit1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/typeahead/search.php?__a=1&value=" + "&category=2200&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2201&page_categories[1]=2202&page_categories[2]=1006&page_categories[3]=1013&context=hub_work&services_mask=1&section=2002&sid=&__user=" + UserId));    //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                                                                                                          //https://www.facebook.com/ajax/typeahead/search.php?__a=1&value=TCS&category=2200&filter[0]=page&viewer=100002506557020&page_categories[0]=2201&page_categories[1]=2202&page_categories[2]=1006&page_categories[3]=1013&context=hub_work&services_mask=1&section=2002&sid=377559032945&__user=100002506557020
                WorkId = GlobusHttpHelper.ParseEncodedJson(PageSrcProfileBasicInfoEdit1, "uid");

                string newWorkName = GlobusHttpHelper.ParseJson(PageSrcProfileBasicInfoEdit1, "\"text");
                if (!string.IsNullOrEmpty(newWorkName))
                {
                    WorkName = newWorkName;
                }


                //if (WorkId== null || WorkId.Length<6)
                {
                    try
                    {
                        string postDataWorkId = "hub_text=" + WorkName + "&type=2002&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
                        string PostUrlWorkId = FBGlobals.Instance.urlPostUrlWorkIdManageProfile;
                        string ResposceWorkId = string.Empty;
                        try
                        {
                            ResposceWorkId = HttpHelper.postFormData(new Uri(PostUrlWorkId), postDataWorkId, "");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (string.IsNullOrEmpty(ResposceWorkId))
                        {
                            try
                            {

                                ResposceWorkId = HttpHelper.postFormData(new Uri(PostUrlWorkId), postDataWorkId, "");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        WorkId = GlobusHttpHelper.GetParamValue(ResposceWorkId, "employer_id");

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                try
                {
                  //  string postDataWork = "fb_dtsg=" + fb_dtsg + "&action_type=add&experience_id=&employer_id=" + WorkId + "&position_id=0&position_text=&location_id=0&location_text=&description=&date[current]=on&date_start[year]=&date_start[month]=&date_start[day]=&date_end[year]=&date_end[month]=&date_end[day]=&save=Add%20Job&nctr[_mod]=pagelet_edit_eduwork&__user=" + UserId + "&phstamp=";
                    string postDataWork = "fb_dtsg="+fb_dtsg+"&ref=about_tab&action_type=add&experience_id=&employer_id="+WorkId+"&position_id=0&position_text=&location_id=0&location_text=&description=&date[current]=on&date_start[year]=&date_start[month]=&date_start[day]=&date_end[year]=&date_end[month]=&date_end[day]=&audience[8787685733][value]=80&save=Add%20Job&nctr[_mod]=pagelet_edit_eduwork&__user="+UserId+"&__a=1&__dyn=7n88Oq9c9FpBudDgDxyIJeaEFoW9J6yUgByVbGAFpaGEVF4YxU&__req=1k&ttstamp=265816811410995115120&__rev=1142402&";
                    string PostUrlWork = FBGlobals.Instance.urlPostUrlWorkManageProfile;                                               
                    string ResposceWork = string.Empty;
                    try
                    {
                       // ResposceWork = HttpHelper.postFormData(new Uri(PostUrlWork), postDataWork, "");

                        ResposceWork = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/profile/edit/infotab/save_work.php "), postDataWork, "");         
                        
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {

                        string PostRequestData = "__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=8&__rev=1142402&__user="+UserId+"&fb_dtsg="+fb_dtsg+"&ph=V3&q=%5B%7B%22user%22%3A%22100001006024349%22%2C%22page_id%22%3A%22lvbptt%22%2C%22posts%22%3A%5B%5B%22time_spent_bit_array%22%2C%7B%22tos_id%22%3A%22lvbptt%22%2C%22start_time%22%3A1393668722%2C%22tos_array%22%3A%5B287%2C0%5D%2C%22tos_len%22%3A9%2C%22tos_seq%22%3A0%2C%22tos_cum%22%3A6%7D%2C1393668730566%2C0%5D%5D%2C%22trigger%22%3A%22time_spent_bit_array%22%7D%5D&ts=1393668730568&ttstamp=265816811410995115120";
                        string Resposce = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/bz"), PostRequestData, "");
 

                        string postDataEducation = "fb_dtsg=" + fb_dtsg + "&ref=about_tab&action_type=add&experience_id=&school_id=123824504308517&school_text=" + School_text + "&experience_type=2004&date_start[year]=&date_start[month]=&date_start[day]=&date_end[year]=&date_end[month]=&date_end[day]=&graduated=on&description=&concentration_ids[0]=0&concentration_ids[1]=0&concentration_ids[2]=0&concentration_text[0]=&concentration_text[1]=&concentration_text[2]=&school_type=college&degree_id=0&degree_text=&audience[210686432304281][value]=80&save=Add%20School&nctr[_mod]=pagelet_edit_eduwork&__user=" + UserId + "&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGeqheCu&__req=e&ttstamp=265816811410995115120&__rev=1142402";
                        ResposceWork = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/profile/edit/infotab/save_edu.php"), postDataEducation, "");
 
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (string.IsNullOrEmpty(ResposceWork))
                    {
                        try
                        {

                            ResposceWork = HttpHelper.postFormData(new Uri(PostUrlWork), postDataWork, "");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    string postDataWorkDone = "fb_dtsg=" + fb_dtsg + "&nctr[_mod]=pagelet_eduwork&__user=" + UserId + "&phstamp=";
                    string PostUrlWorkDone = FBGlobals.Instance.urlPostUrlWorkDoneManageProfile;
                    string ResposceWorkDone = string.Empty;
                    try
                    {
                        ResposceWorkDone = HttpHelper.postFormData(new Uri(PostUrlWorkDone), postDataWorkDone, "");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (string.IsNullOrEmpty(ResposceWorkDone))
                    {
                        try
                        {

                            ResposceWorkDone = HttpHelper.postFormData(new Uri(PostUrlWorkDone), postDataWorkDone, "");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    // new post data by ajay yadav 08-02-2014

                    try
                    { 

                        string PostData="fb_dtsg="+fb_dtsg+"&ref=about_tab&action_type=add&experience_id=&employer_id="+WorkId+"&position_id=0&position_text=&location_id=0&location_text=&description=&date[current]=on&date_start[year]=&date_start[month]=&date_start[day]=&date_end[year]=&date_end[month]=&date_end[day]=&audience[8787685733][value]=80&save=Add%20Job&nctr[_mod]=pagelet_edit_eduwork&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGejheC&__req=w&ttstamp=2658167451111227269&__rev=1114696";
                        string PostUrl = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch;                  //"https://www.facebook.com/ajax/profile/edit/infotab/save_work.php"
                        ResposceWorkDone = HttpHelper.postFormData(new Uri(PostUrl), PostData, "");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    GlobusLogHelper.log.Info("Work Information Set for " + fbUser.username);
                    GlobusLogHelper.log.Debug("Work Information Set for " + fbUser.username);

                    #region College



                    #endregion


                    //Quotations
                    {
                        string quotes = Uri.EscapeDataString(fbUser.quotations);

                        string getURL_Quotations = FBGlobals.Instance.urlPostDataUrlBasicInfoManageEditProfileQuotesUrl + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=2m";         // "https://www.facebook.com/ajax/timeline/edit_profile/quotes.php?__user=" 
                        string res_getURL_Quotations = string.Empty;

                        try
                        {
                            res_getURL_Quotations = HttpHelper.getHtmlfromUrl(new Uri(getURL_Quotations));
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (string.IsNullOrEmpty(res_getURL_Quotations))
                        {
                            try
                            {
                                res_getURL_Quotations = HttpHelper.getHtmlfromUrl(new Uri(getURL_Quotations.Replace("https:", "https:")));
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }

                        string postURL_Quotations = FBGlobals.Instance.urlPostURLQuotationsManageProfile;
                        string postData_Quotations = "fb_dtsg=" + fb_dtsg + "&quotes=" + quotes + "&audience[8787630733][value]=80&save=1&nctr[_mod]=pagelet_quotes&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=2n&phstamp=165816775867048105325";
                        string res_postData_Quotations = string.Empty;
                        try
                        {
                            res_postData_Quotations = HttpHelper.postFormData(new Uri(postURL_Quotations), postData_Quotations, "");
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        if (res_postData_Quotations.Contains("\"jsmods\":{"))
                        {
                            GlobusLogHelper.log.Info("Quotations Set for " + fbUser.username);
                            GlobusLogHelper.log.Debug("Quotations Set for " + fbUser.username);
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

            GlobusLogHelper.log.Info("Process Completed Of Update Work.. With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Update Work.. With Username >>> " + fbUser.username);

        }

        public void UpdateAboutYouTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update About You.. With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update About You.. With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(PageSrc))
                {
                    try
                    {
                        PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }


                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileAboutYou = string.Empty;
                try
                {

                    string s1 = FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info";                                                     // "https://www.facebook.com/profile.php?id=" 

                    PageSrcProfileAboutYou = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));                                                         //"https://www.facebook.com/profile.php?id="
                }
                catch { }
                if (string.IsNullOrEmpty(PageSrcProfileAboutYou))
                {
                    try
                    {
                        PageSrcProfileAboutYou = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));                                                    //"https://www.facebook.com/profile.php?id="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string PageSrcAboutYouEdit = string.Empty;
                try
                {
                    PageSrcAboutYouEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileBioUser + UserId));                        //"https://www.facebook.com/ajax/timeline/edit_profile/bio.php?__a=1&__user="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                                                                                                                                                                                              //https://www.facebook.com/ajax/timeline/edit_profile/bio.php?__a=1&__user=100002506557020

                if (string.IsNullOrEmpty(PageSrcAboutYouEdit))
                {
                    try
                    {
                        PageSrcAboutYouEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileBioUser + UserId));                    //"https://www.facebook.com/ajax/timeline/edit_profile/bio.php?__a=1&__user=
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                //* Post data for About you ***********************************************
                string about_me = string.Empty;
                string fb_dtsg = string.Empty;

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);

                about_me = fbUser.aboutme;

                string postDataAboutYou = "fb_dtsg=" + fb_dtsg + "&about_me=" + about_me + "&audience[8787635733][value]=80&save=Save&__user=" + UserId + "&phstamp=";
                string PostUrlAboutYou = FBGlobals.Instance.urlPostUrlAboutYouManageProfile;
                string ResposceCurrentCityAndHomeTown = string.Empty;
                try
                {
                    ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrlAboutYou), postDataAboutYou, "");

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ResposceCurrentCityAndHomeTown))
                {
                    try
                    {
                        ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrlAboutYou), postDataAboutYou, "");
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

            GlobusLogHelper.log.Info("Process Completed Of Update About You.. With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Update About You.. With Username >>> " + fbUser.username);
        }

        private string UpdateBook_Favorites(string UserId, string fb_dtsg, string BookName, string PageSrcBooksEdit, ref FacebookUser fbUser)
        {
            string BookId = string.Empty;

            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                string get_freshDataBooks = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + BookName + "&filter[0]=page&filter[1]=og-book&viewer=" + UserId + "&context=book_have_read&page_categories[0]=1300&services_mask=1&surface=self_about_collection&surface_collection_id=14&sid=92073710164&bsp=true&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=f";     //"https://www.facebook.com/ajax/typeahead/search.php?value="
                string res_get_freshDataBooks = HttpHelper.getHtmlfromUrl(new Uri(get_freshDataBooks));

                BookId = GlobusHttpHelper.ParseEncodedJson(res_get_freshDataBooks, "uid");

                if (!res_get_freshDataBooks.Contains("\"uid\":"))
                {
                    GlobusLogHelper.log.Info("Couldn't find any Books for : " + BookName);
                    GlobusLogHelper.log.Debug("Couldn't find any Books for : " + BookName);
                }
                else
                {
                    string postURL_fresh_Books = FBGlobals.Instance.urlPostURLfreshBooksManageProfile;                                                                   // "https://www.facebook.com/ajax/typeahead/record_basic_metrics.php"
                    string postData_fresh_Books = "stats[num_queries]=1&stats[filtered_count]=0&stats[selected_id]=" + BookId + "&stats[selected_type]=page&stats[selected_position]=0&stats[selected_with_mouse]=1&stats[selected_query]=" + BookName + "&stats[event_name]=timeline_collection&stats[event_specific_data][context]=book_have_read&stats[event_specific_data][categories][0]=1300&stats[candidate_results]=[%22" + BookId + "%22%2Cnull]&stats[query]=" + BookId + "&stats[sid]=92073710164&stats[avg_query_latency]=970&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=g&fb_dtsg=" + fb_dtsg + "&phstamp=165816510975788050671";
                    string res_postData_fresh_Books = HttpHelper.postFormData(new Uri(postURL_fresh_Books), postData_fresh_Books);


                    //string surface_collection_id = Globussoft.GlobusHttpHelper.ParseEncodedJson(res_get_freshDataBooks, "uid");
                    string surface_collection_id = GlobusHttpHelper.ParseEncodedJson(PageSrcBooksEdit, "surface_collection_id").Replace("}", "");
                    string collection_token = string.Empty;

                    if (PageSrcBooksEdit.Contains("\"collection_token\\\" value=\\"))
                    {
                        collection_token = Uri.EscapeDataString(PageSrcBooksEdit.Substring(PageSrcBooksEdit.IndexOf("\"collection_token\\\" value=\\"), PageSrcBooksEdit.IndexOf(">", PageSrcBooksEdit.IndexOf("\"collection_token\\\" value=\\")) - PageSrcBooksEdit.IndexOf("\"collection_token\\\" value=\\")).Replace("\"collection_token\\\" value=\\", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty).Trim());
                    }


                    string[] array_surface_collection_id = Regex.Split(PageSrcBooksEdit, "\"surface_collection_id\":");
                    if (array_surface_collection_id.Length > 1 && string.IsNullOrEmpty(collection_token))
                    {
                        collection_token = Uri.EscapeDataString(array_surface_collection_id[1].Substring(array_surface_collection_id[1].IndexOf("\"value\":"), array_surface_collection_id[1].IndexOf(",", array_surface_collection_id[1].IndexOf("\"value\":")) - array_surface_collection_id[1].IndexOf("\"value\":")).Replace("\"value\":", string.Empty).Replace("\"", string.Empty).Trim());
                    }

                    string postURL_next_fresh_Books = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineAppCollectionItemStandardCurationSurfaceCollectionId;                                                                                 //"https://www.facebook.com/ajax/timeline/app_collection_item/standard_in_house_og/curation?surface_collection_id=14"
                    string postData_next_fresh_Books = "action=add&mechanism=typeahead&item_id=" + BookId + "&collection_token=" + collection_token + "&surface=self_about_collection&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=h&fb_dtsg=" + fb_dtsg + "&phstamp=165816510975788050215";
                    string res_postData_next_fresh_Books = HttpHelper.postFormData(new Uri(postURL_next_fresh_Books), postData_next_fresh_Books);

                    if (res_postData_next_fresh_Books.Contains("\"jsmods\":{\""))
                    {
                        GlobusLogHelper.log.Info("Books Set : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Books Set : " + fbUser.username);
                    }
                    else if (res_postData_next_fresh_Books.Contains("errorSummary\":\"Already Friends\""))
                    {
                        GlobusLogHelper.log.Info("Already Books Set : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Already Books Set : " + fbUser.username);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Unable to set books : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Unable to set books : " + fbUser.username);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return BookId;
        }

        public void UpdateFavoritesTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update Favorites.. With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update Favorites.. With Username >>> " + fbUser.username);

                string UserId = string.Empty;

                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrc))
                {
                    try
                    {
                        PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileMusic = string.Empty;
                try
                {
                    PageSrcProfileMusic = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=favorites"));          //"https://www.facebook.com/profile.php?id=" 
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileMusic))
                {
                    try
                    {
                        PageSrcProfileMusic = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=favorites"));           //"https://www.facebook.com/profile.php?id="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                string PageSrcProfileMovies = string.Empty;
                try
                {
                    PageSrcProfileMovies = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=movies"));            //"https://www.facebook.com/profile.php?id=" 
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileMusic))
                {
                    try
                    {
                        PageSrcProfileMovies = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=movies"));          //"https://www.facebook.com/profile.php?id="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                                                                                
                string PageSrcMusicEdit = string.Empty;
                try
                {
                    PageSrcMusicEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountAjaxTimelineEditProfileFavorites + UserId)); //"https://www.facebook.com/ajax/timeline/edit_profile/favorites.php?start_edit=1&__a=1&__user=" 
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(PageSrcMusicEdit))
                {
                    try
                    {
                        PageSrcMusicEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountAjaxTimelineEditProfileFavorites + UserId + UserId)); //  "https://www.facebook.com/ajax/timeline/edit_profile/favorites.php?start_edit=1&__a=1&__user="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                string PageSrcBooksEdit = string.Empty;
                try
                {
                    PageSrcBooksEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=books"));              //"https://www.facebook.com/profile.php?id="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcMusicEdit))
                {
                    try
                    {
                        PageSrcBooksEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=books"));                  //"https://www.facebook.com/profile.php?id="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                                                                                         //https://www.facebook.com/ajax/timeline/edit_profile/favorites.php?start_edit=1&__a=1&__user=100002506557020 

                string fb_dtsg = string.Empty;

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);

                //* Post data for Music Id ***********************************************
                #region For Music
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.music))
                    {
                        try
                        {
                            string MusicName = string.Empty;
                            string MusicId = string.Empty;

                            MusicName = fbUser.music.Replace(" ", "%20");

                            string get_freshDataBooks = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + MusicName + "&filter[0]=page&filter[1]=og-book&viewer=" + UserId + "&context=favorite_music&page_categories[0]=1202&services_mask=1&surface=self_about_collection&surface_collection_id=10&sid=1140734203343&bsp=true&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=v";                    //"https://www.facebook.com/ajax/typeahead/search.php?value="
                            string res_get_freshDataBooks = HttpHelper.getHtmlfromUrl(new Uri(get_freshDataBooks));

                            if (!res_get_freshDataBooks.Contains("\"uid\":"))
                            {
                                GlobusLogHelper.log.Info("Couldn't find any music for : " + fbUser.music);
                                GlobusLogHelper.log.Debug("Couldn't find any music for : " + fbUser.music);
                            }
                            else
                            {
                                MusicId = GlobusHttpHelper.ParseEncodedJson(res_get_freshDataBooks, "uid");

                                string postURL_fresh_Books = FBGlobals.Instance.urlPostURLfreshBooksManageProfile;
                                string postData_fresh_Books = "stats[num_queries]=1&stats[filtered_count]=3&stats[selected_id]=" + MusicId + "&stats[selected_type]=page&stats[selected_position]=0&stats[selected_with_mouse]=1&stats[selected_query]=" + MusicName + "&stats[event_name]=timeline_collection&stats[event_specific_data][context]=favorite_music&stats[event_specific_data][categories][0]=1202&stats[candidate_results]=[%22" + MusicId + "%22]&stats[query]=" + MusicName + "&stats[sid]=1140734203343&stats[avg_query_latency]=719.5&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=w&fb_dtsg=" + fb_dtsg + "&phstamp=165816870754984105515";
                                string res_postData_fresh_Books = HttpHelper.postFormData(new Uri(postURL_fresh_Books), postData_fresh_Books);

                                string surface_collection_id = GlobusHttpHelper.ParseEncodedJson(PageSrcProfileMusic, "surface_collection_id").Replace("}", "");
                                string collection_token = string.Empty;

                                if (PageSrcProfileMovies.Contains("\"collection_token\\\" value=\\"))
                                {
                                    collection_token = Uri.EscapeDataString(PageSrcProfileMusic.Substring(PageSrcProfileMusic.IndexOf("\"collection_token\\\" value=\\"), PageSrcProfileMusic.IndexOf(">", PageSrcProfileMusic.IndexOf("\"collection_token\\\" value=\\")) - PageSrcProfileMusic.IndexOf("\"collection_token\\\" value=\\")).Replace("\"collection_token\\\" value=\\", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty).Trim());
                                }


                                string[] array_surface_collection_id = Regex.Split(PageSrcProfileMovies, "\"surface_collection_id\":");
                                if (array_surface_collection_id.Length > 1 && string.IsNullOrEmpty(collection_token))
                                {
                                    collection_token = Uri.EscapeDataString(array_surface_collection_id[1].Substring(array_surface_collection_id[1].IndexOf("\"value\":"), array_surface_collection_id[1].IndexOf(",", array_surface_collection_id[1].IndexOf("\"value\":")) - array_surface_collection_id[1].IndexOf("\"value\":")).Replace("\"value\":", string.Empty).Replace("\"", string.Empty).Trim());
                                }

                                string postURL_next_fresh_Books = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineAppCollectionItemAddFbpageSurfaceCollection + surface_collection_id + "";                                                   // "https://www.facebook.com/ajax/timeline/app_collection_item/add/fbpage/?surface_collection_id="
                                string postData_next_fresh_Books = "action=add&mechanism=typeahead&item_id=" + MusicId + "&collection_token=" + collection_token + "&surface=self_about_collection&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=x&fb_dtsg=" + fb_dtsg + "&phstamp=165816870754984105212";
                                string res_postData_next_fresh_Books = HttpHelper.postFormData(new Uri(postURL_next_fresh_Books), postData_next_fresh_Books);

                                if (res_postData_next_fresh_Books.Contains("\"jsmods\":{\""))
                                {
                                    GlobusLogHelper.log.Info("Music Set : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Music Set : " + fbUser.username);
                                }
                                else if (res_postData_next_fresh_Books.Contains("errorSummary\":\"Already Liked\""))
                                {
                                    GlobusLogHelper.log.Info("Already Music Set : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Already Music Set : " + fbUser.username);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Unable to set Music : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Unable to set Music : " + fbUser.username);
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
                #endregion

                //* Post data for Book ***********************************************
                #region For Book
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.books))
                    {
                        try
                        {
                            string BookName = string.Empty;

                            string BookId = string.Empty;

                            BookName = fbUser.books.Replace(" ", "%20");


                            BookId = UpdateBook_Favorites(UserId, fb_dtsg, BookName, PageSrcBooksEdit, ref fbUser);
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
                #endregion

                //* Post data for Movies ***********************************************
                #region For Movies
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.movies))
                    {
                        try
                        {
                            string MoviesName = string.Empty;
                            string MoviesId = string.Empty;

                            MoviesName = fbUser.movies.Replace(" ", "%20");



                            string get_freshDataBooks = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + MoviesName + "&filter[0]=page&filter[1]=og-book&viewer=" + UserId + "&context=video_movies_have_watched&page_categories[0]=1105&services_mask=1&surface=self_about_collection&surface_collection_id=46&sid=912665993705&bsp=true&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=t";                      // "https://www.facebook.com/ajax/typeahead/search.php?value="
                            string res_get_freshDataBooks = HttpHelper.getHtmlfromUrl(new Uri(get_freshDataBooks));

                            MoviesId = GlobusHttpHelper.ParseEncodedJson(res_get_freshDataBooks, "uid");

                            string postURL_fresh_Books = FBGlobals.Instance.urlPostURLfreshBooksManageProfile;                                                                        // "https://www.facebook.com/ajax/typeahead/record_basic_metrics.php"
                            string postData_fresh_Books = "stats[num_queries]=1&stats[filtered_count]=4&stats[selected_id]=" + MoviesId + "&stats[selected_type]=page&stats[selected_position]=0&stats[selected_with_mouse]=1&stats[selected_query]=" + MoviesName + "&stats[event_name]=timeline_collection&stats[event_specific_data][context]=video_movies_have_watched&stats[event_specific_data][categories][0]=1105&stats[candidate_results]=[%22" + MoviesId + "%22]&stats[query]=the%20dark%20knight&stats[sid]=912665993705&stats[avg_query_latency]=1007&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=u&fb_dtsg=AQAmKNP2&phstamp=165816510975788050601";
                            string res_postData_fresh_Books = HttpHelper.postFormData(new Uri(postURL_fresh_Books), postData_fresh_Books);

                            string surface_collection_id = GlobusHttpHelper.ParseEncodedJson(PageSrcProfileMovies, "surface_collection_id").Replace("}", "");
                            string collection_token = string.Empty;

                            if (PageSrcProfileMovies.Contains("\"collection_token\\\" value=\\"))
                            {
                                collection_token = Uri.EscapeDataString(PageSrcProfileMovies.Substring(PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\"), PageSrcProfileMovies.IndexOf(">", PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\")) - PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\")).Replace("\"collection_token\\\" value=\\", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty).Trim());
                            }


                            string[] array_surface_collection_id = Regex.Split(PageSrcProfileMovies, "\"surface_collection_id\":");
                            if (array_surface_collection_id.Length > 1 && string.IsNullOrEmpty(collection_token))
                            {
                                collection_token = Uri.EscapeDataString(array_surface_collection_id[1].Substring(array_surface_collection_id[1].IndexOf("\"value\":"), array_surface_collection_id[1].IndexOf(",", array_surface_collection_id[1].IndexOf("\"value\":")) - array_surface_collection_id[1].IndexOf("\"value\":")).Replace("\"value\":", string.Empty).Replace("\"", string.Empty).Trim());
                            }

                            string postURL_next_fresh_Books = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineAppCollectionItemStandardCurationSurfaceCollectionId + surface_collection_id + "";                                                  // "https://www.facebook.com/ajax/timeline/app_collection_item/standard_in_house_og/curation?surface_collection_id="
                            string postData_next_fresh_Books = "action=add&mechanism=typeahead&item_id=" + MoviesId + "&collection_token=" + collection_token + "&surface=self_about_collection&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=v&fb_dtsg=" + fb_dtsg + "&phstamp=165816510975788050211";               //"action=add&mechanism=typeahead&item_id="+MoviesId+"&collection_token=100004002347820%3A177822289030932%3A46&surface=self_about_collection&__user="+UserId+"&__a=1&__dyn=7n8aphoCBDxe2K&__req=v&fb_dtsg="+fb_dtsg+"&phstamp=165816510975788050211";
                            string res_postData_next_fresh_Books = HttpHelper.postFormData(new Uri(postURL_next_fresh_Books), postData_next_fresh_Books);

                            if (res_postData_next_fresh_Books.Contains("\"jsmods\":{\""))
                            {
                                GlobusLogHelper.log.Info("Movies Set : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Movies Set : " + fbUser.username);
                            }
                            else if (res_postData_next_fresh_Books.Contains("errorSummary\":\"Already Friends\""))
                            {
                                GlobusLogHelper.log.Info("Already Movie Set : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Already Movie Set : " + fbUser.username);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Unable to set Movies : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Unable to set Movies : " + fbUser.username);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }
                }
                catch (Exception)
                {
                    //Log("Some Problem to set Movies Information " + Username);
                }
                #endregion

                //* Post data for Sports Team ***********************************************
                #region For Sports Team
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.favoriteteams))
                    {
                        try
                        {
                            string SportsTeamName = string.Empty;
                            string SportsTeamId = string.Empty;

                            SportsTeamName = fbUser.favoriteteams.Replace(" ", "%20");



                            string get_freshDataBooks = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + SportsTeamName + "&filter[0]=page&filter[1]=og-book&viewer=" + UserId + "&context=video_movies_have_watched&page_categories[0]=1105&services_mask=1&surface=self_about_collection&surface_collection_id=46&sid=912665993705&bsp=true&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=t";                       // "https://www.facebook.com/ajax/typeahead/search.php?value=" 
                            string res_get_freshDataBooks = HttpHelper.getHtmlfromUrl(new Uri(get_freshDataBooks));

                            if (!res_get_freshDataBooks.Contains("\"uid\":"))
                            {
                                GlobusLogHelper.log.Info("Couldn't find any Favorite Teams for : " + fbUser.favoriteteams);
                                GlobusLogHelper.log.Debug("Couldn't find any Favorite Teams for : " + fbUser.favoriteteams);
                            }

                            SportsTeamId = GlobusHttpHelper.ParseEncodedJson(res_get_freshDataBooks, "uid");



                            string postURL_fresh_Books = FBGlobals.Instance.urlPostURLfreshBooksManageProfile;                                                      // "https://www.facebook.com/ajax/typeahead/record_basic_metrics.php"
                            string postData_fresh_Books = "stats[num_queries]=1&stats[filtered_count]=4&stats[selected_id]=" + SportsTeamId + "&stats[selected_type]=page&stats[selected_position]=0&stats[selected_with_mouse]=1&stats[selected_query]=" + SportsTeamName + "&stats[event_name]=timeline_collection&stats[event_specific_data][context]=video_movies_have_watched&stats[event_specific_data][categories][0]=1105&stats[candidate_results]=[%22" + SportsTeamId + "%22]&stats[query]=the%20dark%20knight&stats[sid]=912665993705&stats[avg_query_latency]=1007&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=u&fb_dtsg=AQAmKNP2&phstamp=165816510975788050601";
                            string res_postData_fresh_Books = HttpHelper.postFormData(new Uri(postURL_fresh_Books), postData_fresh_Books);

                            string surface_collection_id = GlobusHttpHelper.ParseEncodedJson(PageSrcProfileMovies, "surface_collection_id").Replace("}", "");
                            string collection_token = string.Empty;

                            if (PageSrcProfileMovies.Contains("\"collection_token\\\" value=\\"))
                            {
                                collection_token = Uri.EscapeDataString(PageSrcProfileMovies.Substring(PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\"), PageSrcProfileMovies.IndexOf(">", PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\")) - PageSrcProfileMovies.IndexOf("\"collection_token\\\" value=\\")).Replace("\"collection_token\\\" value=\\", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty).Trim());
                            }


                            string[] array_surface_collection_id = Regex.Split(PageSrcProfileMovies, "\"surface_collection_id\":");
                            if (array_surface_collection_id.Length > 1 && string.IsNullOrEmpty(collection_token))
                            {
                                collection_token = Uri.EscapeDataString(array_surface_collection_id[1].Substring(array_surface_collection_id[1].IndexOf("\"value\":"), array_surface_collection_id[1].IndexOf(",", array_surface_collection_id[1].IndexOf("\"value\":")) - array_surface_collection_id[1].IndexOf("\"value\":")).Replace("\"value\":", string.Empty).Replace("\"", string.Empty).Trim());
                            }

                            string postURL_next_fresh_Books = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineAppCollectionItemStandardCurationSurfaceCollectionId + surface_collection_id + "";                                     //"https://www.facebook.com/ajax/timeline/app_collection_item/standard_in_house_og/curation?surface_collection_id="
                            string postData_next_fresh_Books = "action=add&mechanism=typeahead&item_id=" + SportsTeamId + "&collection_token=" + collection_token + "&surface=self_about_collection&__user=" + UserId + "&__a=1&__dyn=7n8aphoCBDxe2K&__req=v&fb_dtsg=" + fb_dtsg + "&phstamp=165816510975788050211";//"action=add&mechanism=typeahead&item_id="+MoviesId+"&collection_token=100004002347820%3A177822289030932%3A46&surface=self_about_collection&__user="+UserId+"&__a=1&__dyn=7n8aphoCBDxe2K&__req=v&fb_dtsg="+fb_dtsg+"&phstamp=165816510975788050211";
                            string res_postData_next_fresh_Books = HttpHelper.postFormData(new Uri(postURL_next_fresh_Books), postData_next_fresh_Books);

                            if (res_postData_next_fresh_Books.Contains("\"jsmods\":{\""))
                            {
                                GlobusLogHelper.log.Info("Sports Set : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Sports Set : " + fbUser.username);
                            }
                            else if (res_postData_next_fresh_Books.Contains("errorSummary\":\"Already Friends\""))
                            {
                                GlobusLogHelper.log.Info("Already Sports Set : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Already Sports Set : " + fbUser.username);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Unable to set Sports : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Unable to set Sports : " + fbUser.username);
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
                #endregion

                //* Post data for Sports  ***********************************************
                #region For Sports
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.favoritesports))
                    {
                        string SportsName = string.Empty;
                        string SportsId = string.Empty;

                        SportsName = fbUser.favoritesports.Replace(" ", "%20");

                        string PageSrcSportsId = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + SportsName + "&category=1501&filter[0]=page&viewer=" + UserId + "&page_categories[0]=1501&context=hub_sport&services_mask=1&section=116472951743471&sid=&__user=" + UserId));                          //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="

                        SportsId = GlobusHttpHelper.ParseEncodedJson(PageSrcSportsId, "uid");

                        if (string.IsNullOrEmpty(SportsId))
                        {
                            try
                            {
                                string postDataSportsId = "hub_id=&hub_text=" + SportsName + "&type=116472951743471&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

                                string PostUrlSportsId = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileEditAddExperience;                                            // "https://www.facebook.com/ajax/profile/edit/add_experience.php?__a=1"

                                string ResposceSportsId = HttpHelper.postFormData(new Uri(PostUrlSportsId), postDataSportsId, "");
                                SportsId = GlobusHttpHelper.GetParamValue(ResposceSportsId, "uid");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        try
                        {
                            string postDataSports = "fb_dtsg=" + fb_dtsg + "&action_type=add&experience_id=0&hub_id=" + SportsId + "&type=116472951743471&description=&save=Add%20Sport&nctr[_mod]=pagelet_all_favorites&__user=" + UserId + "&phstamp=";

                            string PostUrlSports = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileEditSaveExperience;                         // "https://www.facebook.com/ajax/profile/edit/save_experience.php?__a=1"

                            string ResposceSports = HttpHelper.postFormData(new Uri(PostUrlSports), postDataSports, "");

                            GlobusLogHelper.log.Info("Sports Information Set for " + fbUser.username);
                            GlobusLogHelper.log.Debug("Sports Information Set for " + fbUser.username);
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
                #endregion

                //* Post data for Activities  ***********************************************
                #region For Activities
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.activities))
                    {
                        try
                        {
                            string ActivitiesName = string.Empty;
                            string ActivitiesId = string.Empty;

                            ActivitiesName = fbUser.activities.Replace(" ", "%20");

                            string PageSrcSportsId = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + ActivitiesName + "&category=1500&filter[0]=page&viewer=" + UserId + "&page_categories[0]=1500&page_categories[1]=2608&context=hub_activity&services_mask=1&section=1002&sid=1046632555136&existing_ids=&__user=" + UserId));     //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="

                            ActivitiesId = GlobusHttpHelper.ParseEncodedJson(PageSrcSportsId, "uid");

                            string postDataActivities = "fb_dtsg=" + fb_dtsg + "&action_type=add&experience_id=0&hub_id=" + ActivitiesId + "&type=1002&description=&save=Add%20Activity&nctr[_mod]=pagelet_all_favorites&__user=" + UserId + "&phstamp=";

                            string PostUrlActivities = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileEditSaveExperience;                                       // "https://www.facebook.com/ajax/profile/edit/save_experience.php?__a=1"

                            string ResposceActivities = HttpHelper.postFormData(new Uri(PostUrlActivities), postDataActivities, "");

                            GlobusLogHelper.log.Info("Activities Information Set for " + fbUser.username);
                            GlobusLogHelper.log.Debug("Activities Information Set for " + fbUser.username);
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
                #endregion

                //* Post data for Interests  ***********************************************
                #region For Interests
                try
                {
                    if (!string.IsNullOrEmpty(fbUser.interestedin))
                    {
                        try
                        {
                            string InterestsName = string.Empty;
                            string InterestsId = string.Empty;

                            InterestsName = fbUser.interestedin.Replace(" ", "%20");

                            string PageSrcSportsId = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + InterestsName + "&category=1500&filter[0]=page&viewer=" + UserId + "&&context=hub_interest&services_mask=1&section=1003&sid=58814721756&existing_ids=&__user=" + UserId));                        //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="

                            InterestsId = GlobusHttpHelper.ParseEncodedJson(PageSrcSportsId, "uid");


                            string postDataInterests = "fb_dtsg=" + fb_dtsg + "&interests[1003][shown][0]=" + InterestsId + "&text_interests[1003][shown][0]=" + InterestsName + "&audience[113693108715766][value]=40&save=1003&__user=" + UserId + "&phstamp=";

                            string PostUrlInterests = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileEditSaveMediaSection;                           // "https://www.facebook.com/ajax/profile/edit/save_media_section.php?__a=1"

                            string ResposceInterests = HttpHelper.postFormData(new Uri(PostUrlInterests), postDataInterests, "");

                            GlobusLogHelper.log.Info("Interests Information Set for " + fbUser.username);
                            GlobusLogHelper.log.Debug("Interests Information Set for " + fbUser.username);
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
                #endregion

                //* Post data For Done Editing ***********************************************************
                try
                {

                    string postDataFavoritesDone = "fb_dtsg=" + fb_dtsg + "&nctr[_mod]=pagelet_all_favorites&__user=" + UserId + "&phstamp=";

                    string PostUrlFavoritesDone = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineProfileFavoritesUrl;                                              // "https://www.facebook.com/ajax/timeline/edit_profile/favorites.php?done_edit=1&__a=1"

                    string ResposceFavoritesDone = HttpHelper.postFormData(new Uri(PostUrlFavoritesDone), postDataFavoritesDone, "");
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

            GlobusLogHelper.log.Info("Process Completed Update Favorites.. With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Update Favorites.. With Username >>> " + fbUser.username);

        }

        public void UpdateLiving(ref FacebookUser fbUser)
        {
            try
            {
                UpdateCurrentCityAndHomeTownTimeLine(ref fbUser);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void UpdateCurrentCityAndHomeTownTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update CurrentCity And Home Town With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update CurrentCity And Home Town With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(PageSrc))
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                }
                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileCurrentCityAndHomeTown = string.Empty;
                try
                {

                    PageSrcProfileCurrentCityAndHomeTown = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));             //"https://www.facebook.com/profile.php?id=" 

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileCurrentCityAndHomeTown))
                {
                    try
                    {
                        PageSrcProfileCurrentCityAndHomeTown = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));                    //"https://www.facebook.com/profile.php?id=" 
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string PageSrcCurrentCityAndHomeTownEdit = string.Empty;
                try
                {
                    PageSrcCurrentCityAndHomeTownEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineEditProfileHometownUrl + UserId));         //"https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?__a=1&__user="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcCurrentCityAndHomeTownEdit))
                {
                    try
                    {
                        PageSrcCurrentCityAndHomeTownEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineEditProfileHometownUrl + UserId));            //"https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?__a=1&__user="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                //* Post data for work ***********************************************
                string CurrentCityName = string.Empty;
                string CurrentCityId = string.Empty;
                string HomeTownName = string.Empty;
                string HomeTownId = string.Empty;
                string fb_dtsg = string.Empty;

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);


                //** For CurrentCity ***********************************************************
                CurrentCityName = fbUser.currentcity.Replace(" ", "%20").Trim();

                string PageSrcCurrentCityEdit1 = string.Empty;
                string get_SearchIDs = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchPhpValue + CurrentCityName + "&category&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2404&context=hub_current_city&services_mask=1&section=14002&sid=4053562556&existing_ids=&__user=" + UserId + "&__a=1&__dyn=7n8ahyj2qmpm3Ki&__req=l";                       //"https://www.facebook.com/ajax/typeahead/search.php?value="
                try
                {
                    PageSrcCurrentCityEdit1 = HttpHelper.getHtmlfromUrl(new Uri(get_SearchIDs));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcCurrentCityEdit1))
                {
                    get_SearchIDs = get_SearchIDs.Replace("https://", "https://");
                    try
                    {
                        PageSrcCurrentCityEdit1 = HttpHelper.getHtmlfromUrl(new Uri(get_SearchIDs));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                CurrentCityId = GlobusHttpHelper.ParseEncodedJson(PageSrcCurrentCityEdit1, "uid");

                //** For Home Town **************************************************************************
                HomeTownName = fbUser.hometown.Replace(" ", "%20").Trim();
                string PageSrcHomeTownEdit1 = string.Empty;
                try
                {
                    PageSrcHomeTownEdit1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + HomeTownName + "&category&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2404&context=hubs_location&services_mask=1&section=14001&sid=&__user=" + UserId));                         //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="
                }
                catch { }
                if (string.IsNullOrEmpty(PageSrcHomeTownEdit1))
                {
                    try
                    {
                        PageSrcHomeTownEdit1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + HomeTownName + "&category&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2404&context=hubs_location&services_mask=1&section=14001&sid=&__user=" + UserId));                      //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                HomeTownId = GlobusHttpHelper.ParseEncodedJson(PageSrcHomeTownEdit1, "uid");

                //string postDataCurrentCityAndHomeTown = "fb_dtsg="+fb_dtsg+"&current_city="+CurrentCityId+"&audience[8787650733][custom_value]=50&audience[8787650733][value]=111&audience[8787655733][value]=80&hometown="+HomeTownId+"&save=Save&__user="+UserId+"&phstamp=";
                //fb_dtsg=AQBhn9e-&current_city=107567112600135&audience[8787650733][value]=80&audience[8787655733][value]=80&hometown=110484678977638&save=Save&__user=100003486527470&phstamp=16581661041105710145165

                string postDataCurrentCityAndHomeTown = "fb_dtsg=" + fb_dtsg + "&current_city=" + CurrentCityId + "&audience[8787650733][value]=80&audience[8787655733][value]=80&hometown=" + HomeTownId + "&save=Save&__user=" + UserId + "&phstamp=";

                string PostUrlCurrentCityAndHomeTown = FBGlobals.Instance.urlPostCurrentCityAndHomeTownManageProfile;
                string ResposceCurrentCityAndHomeTown = string.Empty;
                try
                {
                    ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrlCurrentCityAndHomeTown), postDataCurrentCityAndHomeTown, "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                #region CommentedCode
                //if (string.IsNullOrEmpty(ResposceCurrentCityAndHomeTown))
                //{
                //    try
                //    {

                //        ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrlCurrentCityAndHomeTown), postDataCurrentCityAndHomeTown, "");
                //    }
                //    catch (Exception ex)
                //    {
                //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                //    }
                //} 
                #endregion

                try
                {
                    string url = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineEditProfileHomeTownAboutTabFieldTypeCurrentCityUrl;                                        //"https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?ref=about_tab&field_type=current_city"

                    string postdataa = "fb_dtsg=" + fb_dtsg + "&current_city=" + CurrentCityId + "&audience[8787650733][value]=50&save=1&nctr[_mod]=pagelet_hometown&__user=" + UserId + "&__a=1&__dyn=7n8apij2qmumdDgDxyIJeaEEkyp9EbEyGgyimEVd4Wo&__req=i&ttstamp=2658167451111227269&__rev=1114696";

                    ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(url), postdataa, "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }


                try
                {
                    string postdataHome = "fb_dtsg="+fb_dtsg+"&hometown="+HomeTownId+"&audience[8787655733][value]=50&save=1&nctr[_mod]=pagelet_hometown&__user="+UserId+"&__a=1&__dyn=7n88SkAMCBDBzpQ9UoHbjyGa58Ciq2W8GA8ABGejheC&__req=l&ttstamp=2658167451111227269&__rev=1114696";
                    string PostUrl = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimeLineEditProfileAboutTabFieldTypeHomeTownUrl;                                  //"https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?ref=about_tab&field_type=hometown"
                    ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrl), postdataHome, "");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }



                GlobusLogHelper.log.Info("Current City And HomeTown Information Set for " + fbUser.username);
                GlobusLogHelper.log.Debug("Current City And HomeTown Information Set for " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Update CurrentCity And Home Town With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Update CurrentCity And Home Town With Username >>> " + fbUser.username);

        }

        public void UpdateEducationTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Update Education.. With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Update Education.. With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                string PageSrc = string.Empty;
                try
                {
                    PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrc))
                {
                    try
                    {
                        PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfileEducationTimeLine = string.Empty;
                try
                {
                    PageSrcProfileEducationTimeLine = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));       //"https://www.facebook.com/profile.php?id=" 
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileEducationTimeLine))
                {
                    try
                    {
                        PageSrcProfileEducationTimeLine = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId + "&sk=info"));      //"https://www.facebook.com/profile.php?id=" 
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                string PageSrcProfileEducationEdit = string.Empty;
                try
                {
                    PageSrcProfileEducationEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileEduWork + UserId));                  //"https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?__a=1&__user="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileEducationEdit))
                {
                    try
                    {
                        PageSrcProfileEducationEdit = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileEduWork + UserId));                   //"https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?__a=1&__user="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                //** Post Data For Education*********************************************
                string EducationName = string.Empty;
                string EducationId = string.Empty;
                string fb_dtsg = string.Empty;

                EducationName = fbUser.college.Replace(" ", "%20");  //("KNIT Sultanpur").Replace(" ","%20");
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);

                string PageSrcProfileEducationEdit1 = string.Empty;
                try
                {
                    PageSrcProfileEducationEdit1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + EducationName + "&category=2602&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2602&page_categories[1]=2250&context=hub_college&services_mask=1&section=2004&sid=&__user=" + UserId));                            //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(PageSrcProfileEducationEdit1))
                {
                    try
                    {
                        PageSrcProfileEducationEdit1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearch + EducationName + "&category=2602&filter[0]=page&viewer=" + UserId + "&page_categories[0]=2602&page_categories[1]=2250&context=hub_college&services_mask=1&section=2004&sid=&__user=" + UserId));   //"https://www.facebook.com/ajax/typeahead/search.php?__a=1&value="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                EducationId = GlobusHttpHelper.ParseEncodedJson(PageSrcProfileEducationEdit1, "uid");

                                                                                                                                   //https://www.facebook.com/ajax/typeahead/search.php?__a=1&value=bit%20bhilai&category=2602&filter[0]=page&viewer=100002506557020&page_categories[0]=2602&page_categories[1]=2250&context=hub_college&services_mask=1&section=2004&sid=1272808678776&existing_ids=116113428402524%2C137810362975954%2C131394250284859&__user=100002506557020


                string postDataEduction = "fb_dtsg=" + fb_dtsg + "&action_type=add&experience_id=&school_id=" + EducationId + "&school_text=" + EducationName + "&date_start[year]=&date_start[month]=&date_start[day]=&date_end[year]=&date_end[month]=&date_end[day]=&graduated=on&description=&concentration_ids[0]=0&concentration_ids[1]=0&concentration_ids[2]=0&concentration_text[0]=&concentration_text[1]=&concentration_text[2]=&school_type=college&degree_id=0&degree_text=&save=Add%20School&nctr[_mod]=pagelet_edit_eduwork&__user=" + UserId + "&phstamp=";
                string PostUrlEduction = FBGlobals.Instance.urlPostEducationManageProfile;
                string ResposceEducation = string.Empty;
                try
                {
                    ResposceEducation = HttpHelper.postFormData(new Uri(PostUrlEduction), postDataEduction, "");

                    if (ResposceEducation.Contains("\"errorSummary\":"))
                    {
                        string errorSummary = FBUtils.GetErrorSummary(ResposceEducation);
                        GlobusLogHelper.log.Info("Error >>> " + errorSummary + "  EducationName >>> " + EducationName + " With Username >>> " + fbUser.username);
                        GlobusLogHelper.log.Debug("Error >>> " + errorSummary + "  EducationName >>> " + EducationName + " With Username >>> " + fbUser.username);
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ResposceEducation))
                {
                    try
                    {
                        PostUrlEduction = FBGlobals.Instance.urlPostEducationManageProfile;

                        ResposceEducation = HttpHelper.postFormData(new Uri(PostUrlEduction), postDataEduction, "");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                GlobusLogHelper.log.Info("Education Information Set for " + fbUser.username);
                GlobusLogHelper.log.Debug("Education Information Set for " + fbUser.username);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Update Education With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Update Education With Username >>> " + fbUser.username);

        }

        public void ChangeFbTimeLine(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Changing TimeLine...With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Changing TimeLine...With Username >>> " + fbUser.username);

                string UserId = string.Empty;
                List<string> facebooklinklist = new List<string>();
                string src = string.Empty;
                string PageSrcProfile1 = string.Empty;
                string PageSrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));

                UserId = GlobusHttpHelper.Get_UserID(PageSrc);

                if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
                {
                    GlobusLogHelper.log.Info("Please Check The Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

                    return;
                }

                string PageSrcProfile = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UserId));           //"https://www.facebook.com/profile.php?id=" 
                if (!PageSrcProfile.Contains("Get Timeline"))
                {
                    PageSrcProfile1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UserId));
                    if (!PageSrcProfile1.Contains("Get Timeline"))
                    {
                        List<string> Hreflist = HttpHelper.GetHrefsFromString(PageSrcProfile);//.GetHrefFromString(PageSrcProfile);
                        foreach (string item in Hreflist)
                        {
                            try
                            {
                                src = "";
                                if (item.Contains(FBGlobals.Instance.fbhomeurl))
                                {
                                    try
                                    {
                                       //string url= item.Replace("  ",string.Empty);
                                       src = HttpHelper.getHtmlfromUrl(new Uri(item));
                                        if (src.Contains("Get Timeline"))
                                        {
                                            break;
                                        }
                                        else
                                        {

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


                if (PageSrcProfile.Contains("Get Timeline") || PageSrcProfile1.Contains("Get Timeline") || src.Contains("Get Timeline"))
                {
                    try
                    {
                        string fb_dtsg = string.Empty;

                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrc);

                        string PostUrlGetTime = FBGlobals.Instance.urlPostTimeLineManageProfile;
                        string PostDataGetTime = "fb_dtsg=" + fb_dtsg + "&nctr[_mod]=pagelet_above_header&__user=" + UserId + "&phstamp=";
                        string ResposceCurrentCityAndHomeTown = HttpHelper.postFormData(new Uri(PostUrlGetTime), PostDataGetTime, "");

                        string PageSrcActivateTimeLine = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlGetActivateTimeLineManageProfile));

                        GlobusLogHelper.log.Info("Facebook Account Change in Time Line : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Facebook Account Change in Time Line : " + fbUser.username);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
                else
                {
                    GlobusLogHelper.log.Info("Facebook Account is already in Time Line : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Facebook Account is already in Time Line : " + fbUser.username);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Process Completed Of Changing TimeLine With Username >>> " + fbUser.username);
            GlobusLogHelper.log.Debug("Process Completed Of Changing TimeLine With Username >>> " + fbUser.username);

        }

        public void LoadProfileData(string folderPath)
        {
            try
            {
                lstCitiesManageProfiles.Clear();
                lstBirthdaysManageProfiles.Clear();
                lstLanguagesManageProfiles.Clear();
                lstAboutMeManageProfiles.Clear();
                lstEmployerManageProfiles.Clear();
                lstCollegeManageProfiles.Clear();
                lstHighSchoolManageProfiles.Clear();
                lstReligionManageProfiles.Clear();
                lstFamilyNamesManageProfiles.Clear();
                lstHomeTownManageProfiles.Clear();
                lstActivitiesManageProfiles.Clear();
                lstInterestsManageProfiles.Clear();
                lstMoviesManageProfiles.Clear();
                lstMusicManageProfiles.Clear();
                lstBooksManageProfiles.Clear();
                lstRolesManageProfiles.Clear();
                lstFavoriteSportsManageProfiles.Clear();
                lstFavoriteTeamsManageProfiles.Clear();

                string[] files = System.IO.Directory.GetFiles(folderPath);


                foreach (string item in files)
                {

                    if (item.Contains("City.txt"))
                    {
                        lstCitiesManageProfiles = GlobusFileHelper.ReadFile(item);

                        GlobusLogHelper.log.Info(lstCitiesManageProfiles.Count + " City Names Loaded");
                        GlobusLogHelper.log.Debug(lstCitiesManageProfiles.Count + " City Names Loaded");
                    }

                    if (item.Contains("Birthday.txt"))
                    {
                        lstBirthdaysManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstBirthdaysManageProfiles.Count + " Birthday Date Loaded");
                        GlobusLogHelper.log.Debug(lstBirthdaysManageProfiles.Count + " Birthday Date Loaded");
                    }

                    if (item.Contains("Language.txt"))
                    {
                        lstLanguagesManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstLanguagesManageProfiles.Count + " Languages  Loaded");
                        GlobusLogHelper.log.Debug(lstLanguagesManageProfiles.Count + " Languages  Loaded");
                    }
                    if (item.Contains("AboutMe.txt"))
                    {
                        lstAboutMeManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstAboutMeManageProfiles.Count + " AboutMe Information  Loaded");
                        GlobusLogHelper.log.Debug(lstAboutMeManageProfiles.Count + " AboutMe Information  Loaded");

                    }
                    if (item.Contains("Employer.txt"))
                    {
                        lstEmployerManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstEmployerManageProfiles.Count + " Employer Information  Loaded");
                        GlobusLogHelper.log.Debug(lstEmployerManageProfiles.Count + " Employer Information  Loaded");

                    }
                    if (item.Contains("College.txt"))
                    {
                        lstCollegeManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstCollegeManageProfiles.Count + " College Information  Loaded");
                        GlobusLogHelper.log.Debug(lstCollegeManageProfiles.Count + " College Information  Loaded");

                    }
                    if (item.Contains("HighSchool.txt"))
                    {
                        lstHighSchoolManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstHighSchoolManageProfiles.Count + " HighSchool Information  Loaded");
                        GlobusLogHelper.log.Debug(lstHighSchoolManageProfiles.Count + " HighSchool Information  Loaded");

                    }
                    if (item.Contains("Religion.txt"))
                    {
                        lstReligionManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstReligionManageProfiles.Count + " Religion Information  Loaded");
                        GlobusLogHelper.log.Debug(lstReligionManageProfiles.Count + " Religion Information  Loaded");

                    }
                    if (item.Contains("FamilyNames.txt"))
                    {
                        lstFamilyNamesManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstFamilyNamesManageProfiles.Count + " FamilyNames Information  Loaded");
                        GlobusLogHelper.log.Debug(lstFamilyNamesManageProfiles.Count + " FamilyNames Information  Loaded");

                    }
                    if (item.Contains("Role.txt"))
                    {
                        lstRolesManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstRolesManageProfiles.Count + " Roles Information  Loaded");
                        GlobusLogHelper.log.Debug(lstRolesManageProfiles.Count + " Roles Information  Loaded");

                    }
                    if (item.Contains("HomeTown.txt"))
                    {
                        lstHomeTownManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstHomeTownManageProfiles.Count + " HomeTown Information  Loaded");
                        GlobusLogHelper.log.Debug(lstHomeTownManageProfiles.Count + " HomeTown Information  Loaded");

                    }
                    if (item.Contains("Interests.txt"))
                    {
                        lstInterestsManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstInterestsManageProfiles.Count + " HomeTown Information  Loaded");
                        GlobusLogHelper.log.Debug(lstInterestsManageProfiles.Count + " HomeTown Information  Loaded");

                    }
                    if (item.Contains("Activities.txt"))
                    {
                        lstActivitiesManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstActivitiesManageProfiles.Count + " Activities Information  Loaded");
                        GlobusLogHelper.log.Debug(lstActivitiesManageProfiles.Count + " Activities Information  Loaded");

                    }
                    if (item.Contains("Movies.txt"))
                    {
                        lstMoviesManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstMoviesManageProfiles.Count + " Movies Information  Loaded");
                        GlobusLogHelper.log.Debug(lstMoviesManageProfiles.Count + " Movies Information  Loaded");

                    }
                    if (item.Contains("Music.txt"))
                    {
                        lstMusicManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstMusicManageProfiles.Count + " Music Information  Loaded");
                        GlobusLogHelper.log.Debug(lstMusicManageProfiles.Count + " Music Information  Loaded");

                    }
                    if (item.Contains("Books.txt"))
                    {
                        lstBooksManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstBooksManageProfiles.Count + " Books Information  Loaded");
                        GlobusLogHelper.log.Debug(lstBooksManageProfiles.Count + " Books Information  Loaded");
                    }
                    if (item.Contains("FavoriteSports.txt"))
                    {
                        lstFavoriteSportsManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstFavoriteSportsManageProfiles.Count + " Favorite Sports Information  Loaded");
                        GlobusLogHelper.log.Debug(lstFavoriteSportsManageProfiles.Count + " Favorite Sports Information  Loaded");

                    }
                    if (item.Contains("FavoriteTeams.txt"))
                    {
                        lstFavoriteTeamsManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstFavoriteTeamsManageProfiles.Count + " Favorite Teams Information  Loaded");
                        GlobusLogHelper.log.Debug(lstFavoriteTeamsManageProfiles.Count + " Favorite Teams Information  Loaded");

                    }

                    if (item.Contains("Quotations.txt"))
                    {
                        lstQuotationsManageProfiles = GlobusFileHelper.ReadFile(item);
                        GlobusLogHelper.log.Info(lstQuotationsManageProfiles.Count + " Quotation Information  Loaded");

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                GlobusLogHelper.log.Debug("Error in Uploading Profile Data !");
            }
        }

        public void RemoveProfile()
        {
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
                                lock (lockr_ThreadControllerManageProfiles)
                                {
                                    try
                                    {
                                        if (count_ThreadControllerManageProfiles >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockr_ThreadControllerManageProfiles);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(RemoveProfileMultiThread);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            count_ThreadControllerManageProfiles++;
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
                else 
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");   
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


        private void RemoveProfileMultiThread(object oo)
        {
            try
            {
                if (!isStopManageProfiles)
                {
                    try
                    {
                        lstManageProfilesThreads.Add(Thread.CurrentThread);
                        lstManageProfilesThreads.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    Array paramArr = new object[2];
                    paramArr = (Array)oo;

                    FacebookUser objFacebookUser = (FacebookUser)paramArr.GetValue(0);

                    if (!objFacebookUser.isloggedin)
                    {
                        GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

                        objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

                        //Login Process

                        LoginUsingGlobusHttp(ref objFacebookUser);
                    }

                    if (objFacebookUser.isloggedin)
                    {
                        // Call RemoveProfileFromAccount
                        RemoveProfileFromAccount(ref objFacebookUser);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                        GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
            }

            finally
            {
               // if (!isStopManageProfiles)
                {
                    count_ThreadControllerManageProfiles--;
                    lock (lockr_ThreadControllerManageProfiles)
                    {
                       // if (!isStopManageProfiles)
                        {
                            Monitor.Pulse(lockr_ThreadControllerManageProfiles);
                        }
                    }
                }

            }
        }


        public void RemoveProfileFromAccount(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                GlobusLogHelper.log.Info("Start Remove Profile With Username >>> " + fbUser.username);
                GlobusLogHelper.log.Debug("Start Remove Profile With Username >>> " + fbUser.username);

                List<string> lsttempCoverPics = new List<string>();
                string UsreId = string.Empty;
                string imagePath = string.Empty;

                string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookUrl));         // "https://www.facebook.com/home.php"
                string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);

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

                string ResponseSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbProfileUrl + UsreId + "&sk=info&edit=1&ref=update_info_button"));  //http://www.facebook.com/profile.php?id=" + UserId + "&sk=info&edit=1&ref=update_info_button 
                string GetForEducation = string.Empty;
                string EducationId = string.Empty;
                string ExperienceType = string.Empty;

                #region Work And Education
                //if (WorkAndEducation)
                {
                    try
                    {
                        //https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?ref=about_tab&__user=100004248131214&__a=1&__req=b
                        string Ajaxurl = "https://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?ref=about_tab&__user=" + UsreId + "&__a=1";
                        //GetForEducation = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/ajax/timeline/edit_profile/eduwork.php?ref=about_tab&__user=" + UsreId + "&__a=1&__req=1"));
                        GetForEducation = HttpHelper.getHtmlfromUrl(new Uri(Ajaxurl));

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (GetForEducation.Contains("edit_work.php"))
                    {
                        try
                        {
                            string[] Edu = System.Text.RegularExpressions.Regex.Split(GetForEducation, "edit_work.php");
                            //?experience_id=
                            foreach (string item in Edu)
                            {
                                try
                                {
                                    if (!item.Contains("for (;;);"))
                                    {
                                        try
                                        {
                                            EducationId = item.Substring(item.IndexOf("experience_id="), (item.IndexOf("&", item.IndexOf("experience_id=")) - item.IndexOf("experience_id="))).Replace("experience_id=", string.Empty).Replace("amp;", string.Empty).Trim();
                                            ExperienceType = item.Substring(item.IndexOf("experience_type"), (item.IndexOf(">", item.IndexOf("experience_type")) - item.IndexOf("experience_type"))).Replace("experience_type", string.Empty).Replace("value=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
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

                    if (!string.IsNullOrEmpty(EducationId))
                    {
                        try
                        {
                            string EducationPostUrl = "http://www.facebook.com/ajax/profile/edit/delete_eduwork.php?ref=about_tab";
                            string EducationPostData = "fb_dtsg=" + fb_dtsg + "&experience_id=" + EducationId + "&entry_id=&experience_type=" + ExperienceType + "&nctr[_mod]=pagelet_edit_eduwork&__user=" + UsreId + "&__a=1&__req=c&phstamp=165816652110849850146";
                            string FirstPostResponse = HttpHelper.postFormData(new Uri(EducationPostUrl), EducationPostData);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            string EducationPostUrl = "http://www.facebook.com/ajax/profile/edit/delete_eduwork.php?ref=about_tab";
                            string Confirmationpostdata = "fb_dtsg=" + fb_dtsg + "&experience_id=" + EducationId + "&entry_id=&experience_type=" + ExperienceType + "&nctr[_mod]=pagelet_edit_eduwork&__user=" + UsreId + "&__a=1&__req=d&phstamp=165816652110849850146&confirmed=1";
                            string SecondPostResponse = HttpHelper.postFormData(new Uri(EducationPostUrl), Confirmationpostdata);
                            if (SecondPostResponse.Contains("remove"))
                            {
                                GlobusLogHelper.log.Info("Removed Work Data With Username >>> " + fbUser.username);
                                GlobusLogHelper.log.Debug("Removed Work Data With Username >>> " + fbUser.username);

                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info("No Work found With Username >>> " + fbUser.username);
                        GlobusLogHelper.log.Debug("No Work found With Username >>> " + fbUser.username);
                    }


                    if (GetForEducation.Contains("edit_school.php"))
                    {
                        try
                        {
                            string[] Edu = System.Text.RegularExpressions.Regex.Split(GetForEducation, "edit_school.php");
                            //?experience_id=
                            foreach (string item in Edu)
                            {
                                try
                                {
                                    if (!item.Contains("for (;;);"))
                                    {
                                        string EducationPostUrl = "http://www.facebook.com/ajax/profile/edit/delete_eduwork.php?ref=about_tab";
                                        try
                                        {
                                            EducationId = item.Substring(item.IndexOf("experience_id="), (item.IndexOf("&", item.IndexOf("experience_id=")) - item.IndexOf("experience_id="))).Replace("experience_id=", string.Empty).Replace("amp;", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            ExperienceType = item.Substring(item.IndexOf("experience_type"), (item.IndexOf(">", item.IndexOf("experience_type")) - item.IndexOf("experience_type"))).Replace("experience_type", string.Empty).Replace("value=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }

                                        if (!string.IsNullOrEmpty(EducationId))
                                        {
                                            try
                                            {
                                                string EducatioPostUrl = "http://www.facebook.com/ajax/profile/edit/delete_eduwork.php?ref=about_tab";
                                                string EducatioPostData = "fb_dtsg=" + fb_dtsg + "&experience_id=" + EducationId + "&entry_id=&experience_type=" + ExperienceType + "&nctr[_mod]=pagelet_edit_eduwork&__user=" + UsreId + "&__a=1&__req=c&phstamp=165816652110849850146";
                                                string FirstPostRespons = HttpHelper.postFormData(new Uri(EducatioPostUrl), EducatioPostData);

                                                string Confirmatiopostdata = "fb_dtsg=" + fb_dtsg + "&experience_id=" + EducationId + "&entry_id=&experience_type=" + ExperienceType + "&nctr[_mod]=pagelet_edit_eduwork&__user=" + UsreId + "&__a=1&__req=1v&phstamp=165816652110849850146&confirmed=1";
                                                string SecondPostRespons = HttpHelper.postFormData(new Uri(EducationPostUrl), Confirmatiopostdata);
                                                if (SecondPostRespons.Contains("remove"))
                                                {
                                                    GlobusLogHelper.log.Info("Removed Education Data With Username >>> " + fbUser.username);
                                                    GlobusLogHelper.log.Debug("Removed Education Data With Username >>> " + fbUser.username);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        else
                                        {
                                            GlobusLogHelper.log.Info("No Education found With Username >>> " + fbUser.username);
                                            GlobusLogHelper.log.Debug("No Education found With Username >>> " + fbUser.username);
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
                #endregion

                #region Living
                //select name=\
                //if (Living)
                {
                    string GetForLiving = string.Empty;
                    try
                    {
                        GetForLiving = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileHometown + UsreId + "&__a=1&__req=2q"));       //"https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?ref=about_tab&__user="
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    if (GetForLiving.Contains("select name=\\"))
                    {
                        try
                        {
                            string AudienceValue1 = string.Empty;
                            string AudienceValue2 = string.Empty;
                            string Value1 = string.Empty;
                            string Value2 = string.Empty;

                            string id_current_city = string.Empty;
                            string id_hometown = string.Empty;

                            try
                            {
                                string[] Livi = System.Text.RegularExpressions.Regex.Split(GetForEducation, "select name=");
                                if (Livi.Count() > 1)
                                {
                                    foreach (string item in Livi)
                                    {
                                        if (!item.Contains("for (;;);"))
                                        {
                                            try
                                            {
                                                try
                                                {
                                                    //current city
                                                    id_current_city = GlobusHttpHelper.GetParamValue(GetForLiving, "current_city");

                                                    //hometown
                                                    id_hometown = GlobusHttpHelper.GetParamValue(GetForLiving, "hometown");
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }

                                                try
                                                {
                                                    AudienceValue1 = item.Substring(item.IndexOf("\\"), (item.IndexOf(">", item.IndexOf("\\")) - item.IndexOf("\\"))).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                                try
                                                {
                                                    Value1 = item.Substring(item.IndexOf("option value="), (item.IndexOf("selected=", item.IndexOf("option value=")) - item.IndexOf("option value="))).Replace("option value=", string.Empty).Replace("option value=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(">u003Coption>u003C", string.Empty).Replace("u003C", string.Empty).Replace("Coption", string.Empty).Trim();
                                                }
                                                catch (Exception ex)
                                                {
                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                }
                                                try
                                                {
                                                    string get1 = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchEduworkInference + UsreId + "&__a=1&__dyn=7n8ahyj2qmu3-iA&__req=6";                    // "https://www.facebook.com/ajax/typeahead/search/eduwork/inference.php?context=hub_current_city&profilesection=14002&__user=" 
                                                    string get2 = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTypeaheadSearchEduworkInferenceHometown + UsreId + "&__a=1&__dyn=7n8ahyj2qmu3-iA&__req=7";            // "https://www.facebook.com/ajax/typeahead/search/eduwork/inference.php?context=hub_hometown&profilesection=14001&__user="

                                                    string postURL_1 = FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileHometown;                                                                     // "https://www.facebook.com/ajax/timeline/edit_profile/hometown.php?ref=about_tab"
                                                    string postData_1 = "fb_dtsg=" + fb_dtsg + "&current_city=&audience[8787650733][value]=80&audience[8787655733][value]=80&hometown=&save=1&nctr[_mod]=pagelet_hometown&__user=" + UsreId + "&__a=1&__dyn=7n8ahyj2qmu3-iA&__req=8&phstamp=165816653908890117196";

                                                    string res_postData_1 = HttpHelper.postFormData(new Uri(postURL_1), postData_1);

                                                    if (res_postData_1.Contains("edit_profile") && res_postData_1.Contains("hometown.php"))
                                                    {
                                                        GlobusLogHelper.log.Info("Removed Living Data With Username >>> " + fbUser.username);
                                                        GlobusLogHelper.log.Debug("Removed Living Data With Username >>> " + fbUser.username);
                                                        break;
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
                #endregion

                #region
                //if (FavouriteQuotation)
                {
                    try
                    {
                        string AudienceValue = string.Empty;
                        string GetQuotation = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageEditProfileQuotesUrl + UsreId + "&__a=1&__req=10"));     //  "https://www.facebook.com/ajax/timeline/edit_profile/quotes.php?__user="
                        string Updateurl = FBGlobals.Instance.urlPostDataUrlBasicInfoManageProfileAjaxUpdateUrl;                                                                         //  "https://www.facebook.com/ajax/presence/update.php";

                        String UpdatePost = "notif_latest=1357379599&notif_latest_read=0&__user=" + UsreId + "&__a=1&__req=11&fb_dtsg=" + fb_dtsg + "&phstamp=16581665211084985098";
                        string ResponseQuotes = HttpHelper.postFormData(new Uri(Updateurl), UpdatePost);
                        try
                        {
                            AudienceValue = GetQuotation.Substring(GetQuotation.IndexOf("select name="), (GetQuotation.IndexOf(">", GetQuotation.IndexOf("select name=")) - GetQuotation.IndexOf("select name="))).Replace("select name=", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Trim();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        try
                        {
                            string quote = "fb_dtsg=" + fb_dtsg + "&quotes=&" + AudienceValue + "=80&save=Save&__user=" + UsreId + "&__a=1&__req=12&phstamp=165816652110849850103";
                            string EditUrl = FBGlobals.Instance.urlPostURLQuotationsManageProfile;                                                                                       // "https://www.facebook.com/ajax/timeline/edit_profile/quotes.php"
                            string ResponseEditQuotes = HttpHelper.postFormData(new Uri(EditUrl), quote);
                            if (ResponseEditQuotes.Contains("quotes_edit_button"))
                            {
                                GlobusLogHelper.log.Info("Removed Quotes With Username >>> " + fbUser.username);
                                GlobusLogHelper.log.Debug("Removed Quotes With Username >>> " + fbUser.username);
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
                #endregion


                #region
                //if (ContactInfo)
                {
                    string ContactInfoUrl = FBGlobals.Instance.urlPostDataUrlBasicInfoManageTimelineEditProfileContact;                                                           // "https://www.facebook.com/ajax/timeline/edit_profile/contact_info.php";
                    string contactinfoPostdata = "nctr[_mod]=pagelet_contact&__user=" + UsreId + "&__a=1&__req=2i&fb_dtsg=" + fb_dtsg + "&phstamp=16581665211084985081";
                    string ContactInfoResponse = HttpHelper.postFormData(new Uri(ContactInfoUrl), contactinfoPostdata);
                    if (ContactInfoResponse.Contains("select name=\\"))
                    {
                        try
                        {
                            string PrivacyValue = string.Empty;
                            string Audience = string.Empty;
                            string[] Livi = System.Text.RegularExpressions.Regex.Split(ContactInfoResponse, "select name=");
                            Dictionary<string, string> DictPostStore = new Dictionary<string, string>();
                            foreach (string item in Livi)
                            {
                                if (!item.Contains("for (;;);"))
                                {
                                    try
                                    {
                                        try
                                        {
                                            Audience = item.Substring(item.IndexOf("\\"), (item.IndexOf(">", item.IndexOf("\\")) - item.IndexOf("\\"))).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            string Valu = item.Substring(item.IndexOf("option value="), (item.IndexOf("selected=", item.IndexOf("option value=")) - item.IndexOf("option value="))).Replace("option value=", string.Empty).Replace("option value=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(">u003Coption>u003C", string.Empty).Replace("u003C", string.Empty).Replace("Coption", string.Empty).Trim();
                                            string[] Vlue = System.Text.RegularExpressions.Regex.Split(Valu, ">");
                                            PrivacyValue = Vlue[0];
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                        try
                                        {
                                            DictPostStore.Add(Audience, PrivacyValue);
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
                            string Postvalue = string.Empty;
                            string PstVlue = string.Empty;
                            foreach (KeyValuePair<string, string> item in DictPostStore)
                            {
                                try
                                {
                                    Postvalue = string.Empty;
                                    try
                                    {
                                        Postvalue = "&" + item.Key + "=" + item.Value;
                                        PstVlue = PstVlue + Postvalue;
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
                            try
                            {
                                string ScontactinfoPostdata = "fb_dtsg=" + fb_dtsg + PstVlue + "=&sn[0]=&sn_serv[0]=1&current_address=&current_geo_id=&current_zip=&current_geo_neighborhood_str=&website=&save=Save&__user=" + UsreId + "&__a=1&__req=2j&phstamp=165816652110849850685";
                                string SContactInfoResponse = HttpHelper.postFormData(new Uri(ContactInfoUrl), ScontactinfoPostdata);

                                GlobusLogHelper.log.Info("Removed contact Info With Username >>> " + fbUser.username);
                                GlobusLogHelper.log.Debug("Removed contact Info With Username >>> " + fbUser.username);

                            }
                            catch(Exception ex) 
                            {
                                GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                #endregion

                #region
                //if (AboutYou)
                {
                    string GetAboutMe = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.urlPostDataUrlBasicInfoManageAjaxTimelineEditProfileBio + UsreId + "&__a=1"));             //"https://www.facebook.com/ajax/timeline/edit_profile/bio.php?__user=" 
                    string AboutmeUrl = FBGlobals.Instance.urlPostUrlAboutYouManageProfile;                                                                      // "https://www.facebook.com/ajax/timeline/edit_profile/bio.php";
                    string[] LiviArr = System.Text.RegularExpressions.Regex.Split(GetAboutMe, "select name=");
                    string Audience1 = string.Empty;
                    string valu1 = string.Empty;
                    foreach (string item in LiviArr)
                    {
                        if (!item.Contains("for (;;);"))
                        {
                            try
                            {
                                try
                                {
                                    Audience1 = item.Substring(item.IndexOf("\\"), (item.IndexOf(">", item.IndexOf("\\")) - item.IndexOf("\\"))).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                try
                                {
                                    valu1 = item.Substring(item.IndexOf("option value="), (item.IndexOf("selected=", item.IndexOf("option value=")) - item.IndexOf("option value="))).Replace("option value=", string.Empty).Replace("option value=", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(">u003Coption>u003C", string.Empty).Replace("u003C", string.Empty).Replace("Coption", string.Empty).Trim();
                                    if (string.IsNullOrEmpty(valu1) || valu1.Contains(">") || valu1.Contains("<"))
                                    {
                                        valu1 = "80";
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
                    try
                    {
                        string AboutPost = "fb_dtsg=" + fb_dtsg + "&about_me=&" + Audience1 + "=" + valu1 + "&save=Save&__user=" + UsreId + "&__a=1&__req=2g&phstamp=165816652110849850105";
                        string AboutResponse = HttpHelper.postFormData(new Uri(AboutmeUrl), AboutPost);
                        if (AboutResponse.Contains("error"))
                        {
                            GlobusLogHelper.log.Info("AboutMe Data Not Removed With Username >>> " + fbUser.username);
                            GlobusLogHelper.log.Debug("AboutMe Data Not Removed With Username >>> " + fbUser.username);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("About Me Data Removed With Username >>> " + fbUser.username);
                            GlobusLogHelper.log.Debug("About Me Data Removed With Username >>> " + fbUser.username);
                            GlobusLogHelper.log.Debug("Process Completed With  Username :  " + fbUser.username);
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

        public void CreateProfile()
        {
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
                                lock (lockr_ThreadControllerManageProfiles)
                                {
                                    try
                                    {
                                        if (count_ThreadControllerManageProfiles >= listAccounts.Count)
                                        {
                                            Monitor.Wait(lockr_ThreadControllerManageProfiles);
                                        }

                                        string acc = account.Remove(account.IndexOf(':'));

                                        //Run a separate thread for each account
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (item != null)
                                        {

                                            Thread profilerThread = new Thread(StartProfileCreationThreads);
                                            profilerThread.Name = "workerThread_Profiler_" + acc;
                                            profilerThread.IsBackground = true;


                                            profilerThread.Start(new object[] { item });

                                            count_ThreadControllerManageProfiles++;
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


        private void StartProfileCreationThreads(object parameters)
        {

            try
            {
                //FacebookUser objFacebookUser = new FacebookUser();

                if (!isStopManageProfiles)
                {
                    try
                    {
                        lstManageProfilesThreads.Add(Thread.CurrentThread);
                        lstManageProfilesThreads.Distinct();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " +ex.StackTrace);
                    }

                    {
                        {
                            try
                            {

                                {
                                    Array paramsArray = new object[1];
                                    paramsArray = (Array)parameters;

                                    FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

                                    string currentCity = "";
                                    string HomeTown = "";
                                    //int Count = RandomNumberGenerator.GenerateRandom(0, lstBirthdays.Count - 1);
                                    int Count = 0;
                                    string Birthday_Month = "";
                                    string BirthDay_Date = "";
                                    string BirthDay_Year = "";
                                    string AboutMe = "";

                                    string Employer = "";
                                    string College = "";
                                    string HighSchool = "";
                                    string Religion = "";
                                    string ProfilePic = "";
                                    string FamilyName = "";
                                    string Role = "";
                                    string Language = "English";
                                    string Activities = "";
                                    string Interests = "";
                                    string Movies = "";
                                    string Music = "";
                                    string Books = "";
                                    string Quotations = "";
                                    string FavoriteSports = "";
                                    string FavoriteTeams = "";

                                    #region Serially Picks

                                    try
                                    {
                                        if (lstCitiesManageProfiles.Count >= 1)
                                        {

                                            if (counterCities >= lstCitiesManageProfiles.Count)
                                            {
                                                counterCities = 0;
                                            }

                                            try
                                            {
                                                currentCity = lstCitiesManageProfiles[counterCities];
                                            }
                                            catch (Exception)
                                            {
                                                currentCity = string.Empty;
                                            }
                                            counterCities++;
                                        }

                                        objFacebookUser.currentcity = currentCity;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterHomeTown >= lstHomeTownManageProfiles.Count)
                                        {
                                            counterHomeTown = 0;
                                        }

                                        if (lstHomeTownManageProfiles.Count >= 1)
                                        {
                                            try
                                            {
                                                HomeTown = lstHomeTownManageProfiles[counterHomeTown];
                                            }
                                            catch (Exception ex)
                                            {
                                                HomeTown = string.Empty;
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }
                                        counterHomeTown++;

                                        objFacebookUser.hometown = HomeTown;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        //if (counter_Cities >= lstCities.Count)
                                        //{
                                        //    counter_Cities = 0;
                                        //}

                                        if (lstBirthdaysManageProfiles.Count >= 1)
                                        {
                                            try
                                            {
                                                Count = Utils.GenerateRandom(0, lstBirthdaysManageProfiles.Count - 1);

                                                Birthday_Month = lstBirthdaysManageProfiles[Count].Split(':')[0];
                                                BirthDay_Date = lstBirthdaysManageProfiles[Count].Split(':')[1];
                                                BirthDay_Year = lstBirthdaysManageProfiles[Count].Split(':')[2];
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        objFacebookUser.birthdaymonth = Birthday_Month;
                                        objFacebookUser.birthdayday = BirthDay_Date;
                                        objFacebookUser.birthdayyear = BirthDay_Year;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }


                                    try
                                    {
                                        if (counterReligion >= lstReligionManageProfiles.Count)
                                        {
                                            counterReligion = 0;
                                        }

                                        if (lstReligionManageProfiles.Count >= 1)
                                        {
                                            Religion = lstReligionManageProfiles[counterReligion];
                                        }
                                        counterReligion++;

                                        objFacebookUser.religion = Religion;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterAboutMe >= lstAboutMeManageProfiles.Count)
                                        {
                                            counterAboutMe = 0;
                                        }

                                        if (lstAboutMeManageProfiles.Count >= 1)
                                        {
                                            AboutMe = lstAboutMeManageProfiles[counterAboutMe];
                                        }
                                        counterAboutMe++;

                                        objFacebookUser.aboutme = AboutMe;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    try
                                    {
                                        if (counterAboutMe >= lstAboutMeManageProfiles.Count)
                                        {
                                            counterAboutMe = 0;
                                        }
                                        if (lstAboutMeManageProfiles.Count >= 1)
                                        {
                                            AboutMe = lstAboutMeManageProfiles[Utils.GenerateRandom(0, lstAboutMeManageProfiles.Count - 1)];
                                        }
                                        counterAboutMe++;

                                        objFacebookUser.aboutme = AboutMe;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }

                                    try
                                    {
                                        if (counterEmployer >= lstEmployerManageProfiles.Count)
                                        {
                                            counterEmployer = 0;
                                        }

                                        if (lstEmployerManageProfiles.Count >= 1)
                                        {
                                            Employer = lstEmployerManageProfiles[counterEmployer];
                                        }
                                        counterEmployer++;

                                        objFacebookUser.employer = Employer;

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterCollege >= lstCollegeManageProfiles.Count)
                                        {
                                            counterCollege = 0;
                                        }

                                        if (lstCollegeManageProfiles.Count >= 1)
                                        {
                                            College = lstCollegeManageProfiles[counterCollege];
                                        }
                                        counterCollege++;

                                        objFacebookUser.college = College;
                                    }
                                    catch (Exception ex)
                                    {
                                        College = string.Empty;
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterHighSchool >= lstHighSchoolManageProfiles.Count)
                                        {
                                            counterHighSchool = 0;
                                        }

                                        if (lstHighSchoolManageProfiles.Count >= 1)
                                        {
                                            HighSchool = lstHighSchoolManageProfiles[counterHighSchool];
                                        }
                                        counterReligion++;

                                        objFacebookUser.highschool = HighSchool;
                                    }
                                    catch (Exception ex)
                                    {
                                        HighSchool = string.Empty;

                                    }
                                    try
                                    {

                                        try
                                        {
                                            if (counterprofilePic >= lstProfilePicsManageProfiles.Count)
                                            {
                                                counterprofilePic = 0;
                                            }
                                            ProfilePic = lstProfilePicsManageProfiles[counterprofilePic];
                                            counterprofilePic++;

                                            objFacebookUser.profilepic = ProfilePic;
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
                                        if (counterFamily >= lstFamilyNamesManageProfiles.Count)
                                        {
                                            counterFamily = 0;
                                        }
                                        if (lstFamilyNamesManageProfiles.Count >= 1)
                                        {
                                            FamilyName = lstFamilyNamesManageProfiles[Utils.GenerateRandom(0, lstFamilyNamesManageProfiles.Count - 1)];
                                        }
                                        counterFamily++;

                                        objFacebookUser.familyname = FamilyName;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterRole >= lstRolesManageProfiles.Count)
                                        {
                                            counterRole = 0;
                                        }
                                        if (lstRolesManageProfiles.Count >= 1)
                                        {
                                            Role = lstRolesManageProfiles[Utils.GenerateRandom(0, lstRolesManageProfiles.Count - 1)];
                                        }
                                        counterRole++;

                                        objFacebookUser.role = Role;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterLanguage >= lstLanguagesManageProfiles.Count)
                                        {
                                            counterLanguage = 0;
                                        }
                                        if (lstLanguagesManageProfiles.Count >= 1)
                                        {
                                            Language = lstLanguagesManageProfiles[Utils.GenerateRandom(0, lstLanguagesManageProfiles.Count - 1)];
                                        }
                                        counterLanguage++;

                                        objFacebookUser.language = Language;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                  
                                    try
                                    {
                                        if (counterActivities >= lstActivitiesManageProfiles.Count)
                                        {
                                            counterActivities = 0;
                                        }
                                        if (lstActivitiesManageProfiles.Count >= 1)
                                        {
                                            Activities = lstActivitiesManageProfiles[counterActivities];
                                        }
                                        counterActivities++;

                                        objFacebookUser.activities = Activities;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterInterests >= lstInterestsManageProfiles.Count)
                                        {
                                            counterInterests = 0;
                                        }
                                        if (lstInterestsManageProfiles.Count >= 1)
                                        {
                                            Interests = lstInterestsManageProfiles[counterInterests];
                                        }
                                        counterInterests++;

                                        objFacebookUser.interestedin = Interests;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterMovies >= lstMoviesManageProfiles.Count)
                                        {
                                            counterMovies = 0;
                                        }
                                        if (lstMoviesManageProfiles.Count >= 1)
                                        {
                                            Movies = lstMoviesManageProfiles[counterMovies];
                                        }
                                        counterMovies++;

                                        objFacebookUser.movies = Movies;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterMusic >= lstMusicManageProfiles.Count)
                                        {
                                            counterMusic = 0;
                                        }
                                        if (lstMusicManageProfiles.Count >= 1)
                                        {
                                            Music = lstMusicManageProfiles[counterMusic];
                                        }
                                        counterMusic++;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterBooks >= lstBooksManageProfiles.Count)
                                        {
                                            counterBooks = 0;
                                        }
                                        if (lstBooksManageProfiles.Count >= 1)
                                        {
                                            Books = lstBooksManageProfiles[counterBooks];
                                        }
                                        counterBooks++;

                                        objFacebookUser.books = Books;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterQuotationss >= lstQuotationsManageProfiles.Count)
                                        {
                                            counterQuotationss = 0;
                                        }
                                        if (lstQuotationsManageProfiles.Count >= 1)
                                        {
                                            Quotations = lstQuotationsManageProfiles[counterQuotationss];
                                        }
                                        counterQuotationss++;

                                        objFacebookUser.quotations = Quotations;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterFavoriteSports >= lstFavoriteSportsManageProfiles.Count)
                                        {
                                            counterFavoriteSports = 0;
                                        }
                                        if (lstFavoriteSportsManageProfiles.Count >= 1)
                                        {
                                            FavoriteSports = lstFavoriteSportsManageProfiles[counterFavoriteSports];
                                        }
                                        counterFavoriteSports++;

                                        objFacebookUser.favoritesports = FavoriteSports;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    try
                                    {
                                        if (counterFavoriteTeams >= lstFavoriteTeamsManageProfiles.Count)
                                        {
                                            counterFavoriteTeams = 0;
                                        }
                                        if (lstFavoriteTeamsManageProfiles.Count >= 1)
                                        {
                                            FavoriteTeams = lstFavoriteTeamsManageProfiles[counterFavoriteTeams];
                                        }
                                        counterFavoriteTeams++;

                                        objFacebookUser.favoriteteams = FavoriteTeams;
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                    #endregion



                                    ///Update Pics
                                    // if (changeonlyprofilepic)
                                    {
                                        try
                                        {
                                            if (counterprofilePic >= lstProfilePicsManageProfiles.Count)
                                            {
                                                counterprofilePic = 0;
                                            }
                                            ProfilePic = lstProfilePicsManageProfiles[counterprofilePic];
                                            counterprofilePic++;

                                            objFacebookUser.profilepic = ProfilePic;
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                        }
                                    }


                                    if (!objFacebookUser.isloggedin)
                                    {
                                        GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

                                        objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


                                        //Login Process

                                        LoginUsingGlobusHttp(ref objFacebookUser);
                                    }

                                    if (objFacebookUser.isloggedin)
                                    {
                                        // Call ModifyProfile()
                                        ModifyProfile(ref objFacebookUser);
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                                        GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
                                    }
                                }
                                // else
                                {

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

            finally
            {
                try
                {
                    lock (lockr_ThreadControllerManageProfiles)
                    {
                      //  if (!isStopManageProfiles)
                        {
                            Monitor.Pulse(lockr_ThreadControllerManageProfiles);
                        }
                    }
                    if (!isStopManageProfiles)
                    {
                        count_ThreadControllerManageProfiles--;
                        lock (lockr_ThreadControllerManageProfiles)
                        {
                            if (!isStopManageProfiles)
                            {
                                Monitor.Pulse(lockr_ThreadControllerManageProfiles);
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

        public void LoginUsingGlobusHttp(ref FacebookUser facebookUser)
        {
            ///Sign In
            try
            {
                GlobusHttpHelper HttpHelper = facebookUser.globusHttpHelper;

                #region Post variable

                string fbpage_id = string.Empty;
                string fb_dtsg = string.Empty;
                string __user = string.Empty;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string xhpc_composerid12 = string.Empty;

                #endregion

                int intProxyPort = 80;

                if (!string.IsNullOrEmpty(facebookUser.proxyport) && Utils.IdCheck.IsMatch(facebookUser.proxyport))
                {
                    intProxyPort = int.Parse(facebookUser.proxyport);
                }

                GlobusLogHelper.log.Info("Logging in with " + facebookUser.username);
                GlobusLogHelper.log.Debug("Logging in with " + facebookUser.username);

                //string valueLSD = "name=" + "\"lsd\"";
                string pageSource=string.Empty;
                try
                {
                    pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);
                }
                catch(Exception  ex)
                {
                    GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
                }

                if (pageSource == null || string.IsNullOrEmpty(pageSource))
                {
                    pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);
                }

                if (pageSource == null)
                {
                    //FanPagePosterLogger("Unable to login with " + Username);
                    return;
                }

                string valueLSD = GlobusHttpHelper.GetParamValue(pageSource, "lsd");

                #region CommentedCOde
                ///JS, CSS, Image Requests
                //RequestsJSCSSIMG.RequestJSCSSIMG(pageSource, ref HttpHelper);

                //int startIndex = pageSource.IndexOf(valueLSD) + 18;
                //string value = pageSource.Substring(startIndex, 5);

                //string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + Username.Split('@')[0] + "%40" + Username.Split('@')[1] + "&pass=" + Password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUserName, proxyPassword); 
                #endregion

                string ResponseLogin = string.Empty;
                try
                {
                    ResponseLogin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationLoginPhpAttempt), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "&locale=en_US&email=" + facebookUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + facebookUser.username.Split('@')[1] + "&pass=" + Uri.EscapeDataString(facebookUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "");           //"https://www.facebook.com/login.php?login_attempt=1"
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : "+ ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ResponseLogin))
                {
                    ResponseLogin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationLoginPhpAttempt), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "&locale=en_US&email=" + facebookUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + facebookUser.username.Split('@')[1] + "&pass=" + Uri.EscapeDataString(facebookUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "");           //"https://www.facebook.com/login.php?login_attempt=1"
                }
                if (ResponseLogin == null)
                {
                    //FanPagePosterLogger("Unable to login with " + Username);
                    return;
                }

                //FBLoginChecker loginChecker = new FBLoginChecker();
                string loginStatus = "";
                if (CheckLogin(ResponseLogin, facebookUser.username, facebookUser.password, facebookUser.proxyip, facebookUser.proxyport, facebookUser.proxyusername, facebookUser.proxypassword, ref loginStatus))
                {
                    GlobusLogHelper.log.Info("Logged in with Username : " + facebookUser.username);
                    GlobusLogHelper.log.Debug("Logged in with Username : " + facebookUser.username);

                    facebookUser.isloggedin = true;

                    #region CommentedCode

                    ///Write to Successful Login Accounts
                    //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SuccessfullyLoggedInAccounts);

                    ///JS, CSS, Image Requests
                    //RequestsJSCSSIMG.RequestJSCSSIMG(pageSource, ref HttpHelper); 
                    #endregion
                }
                else
                {
                    GlobusLogHelper.log.Info("Couldn't login with Username : " + facebookUser.username);
                    facebookUser.isloggedin = false;


                    if (loginStatus == "account has been disabled")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_DisabledAccount);
                    }

                    if (loginStatus == "Please complete a security check")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SecurityCheckAccounts);
                    }


                    if (loginStatus == "Your account is temporarily locked")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_TemporarilyLockedAccount);

                    }
                    if (loginStatus == "have been blocked")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_havebeenblocked);

                    }
                    if (loginStatus == "For security reasons your account is temporarily locked")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SecurityCheckAccountsforsecurityreason);
                    }

                    if (loginStatus == "Account Not Confirmed")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_AccountNotConfirmed);
                    }
                    if (loginStatus == "Temporarily Blocked for 30 Days")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_30daysBlockedAccount);
                    }
                }
            }
            catch (Exception ex)
            {

                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }

        public void LoginUsingGlobusHttpWithProxy(ref FacebookUser facebookUser)
        {
            ///Sign In
            try
            {
                GlobusHttpHelper HttpHelper = facebookUser.globusHttpHelper;

                #region Post variable

                string fbpage_id = string.Empty;
                string fb_dtsg = string.Empty;
                string __user = string.Empty;
                string xhpc_composerid = string.Empty;
                string xhpc_targetid = string.Empty;
                string xhpc_composerid12 = string.Empty;

                #endregion

                int intProxyPort = 80;

                if (!string.IsNullOrEmpty(facebookUser.proxyport) && Utils.IdCheck.IsMatch(facebookUser.proxyport))
                {
                    intProxyPort = int.Parse(facebookUser.proxyport);
                }

                GlobusLogHelper.log.Info("Logging in with " + facebookUser.username);
                GlobusLogHelper.log.Debug("Logging in with " + facebookUser.username);

                //string valueLSD = "name=" + "\"lsd\"";
                string pageSource = string.Empty;
                try
                {
                    pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (pageSource == null || string.IsNullOrEmpty(pageSource))
                {
                    pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);
                }

                if (pageSource == null)
                {
                    //FanPagePosterLogger("Unable to login with " + Username);
                    return;
                }

                string valueLSD = GlobusHttpHelper.GetParamValue(pageSource, "lsd");

                #region CommentedCOde
                ///JS, CSS, Image Requests
                //RequestsJSCSSIMG.RequestJSCSSIMG(pageSource, ref HttpHelper);

                //int startIndex = pageSource.IndexOf(valueLSD) + 18;
                //string value = pageSource.Substring(startIndex, 5);

                //string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + Username.Split('@')[0] + "%40" + Username.Split('@')[1] + "&pass=" + Password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUserName, proxyPassword); 
                #endregion

                string ResponseLogin = string.Empty;
                try
                {
                    ResponseLogin = HttpHelper.postFormDataProxy(new Uri(FBGlobals.Instance.AccountVerificationLoginPhpAttempt), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "&locale=en_US&email=" + facebookUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + facebookUser.username.Split('@')[1] + "&pass=" + Uri.EscapeDataString(facebookUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "", facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);           //"https://www.facebook.com/login.php?login_attempt=1"
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (string.IsNullOrEmpty(ResponseLogin))
                {
                    ResponseLogin = HttpHelper.postFormDataProxy(new Uri(FBGlobals.Instance.AccountVerificationLoginPhpAttempt), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "&locale=en_US&email=" + facebookUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + facebookUser.username.Split('@')[1] + "&pass=" + Uri.EscapeDataString(facebookUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + valueLSD + "", facebookUser.proxyip, intProxyPort, facebookUser.proxyusername, facebookUser.proxypassword);           //"https://www.facebook.com/login.php?login_attempt=1"
                }
                if (ResponseLogin == null)
                {
                    //FanPagePosterLogger("Unable to login with " + Username);
                    return;
                }

                //FBLoginChecker loginChecker = new FBLoginChecker();
                string loginStatus = "";
                if (CheckLogin(ResponseLogin, facebookUser.username, facebookUser.password, facebookUser.proxyip, facebookUser.proxyport, facebookUser.proxyusername, facebookUser.proxypassword, ref loginStatus))
                {
                    GlobusLogHelper.log.Info("Logged in with Username : " + facebookUser.username);
                    GlobusLogHelper.log.Debug("Logged in with Username : " + facebookUser.username);

                    facebookUser.isloggedin = true;

                    #region CommentedCode

                    ///Write to Successful Login Accounts
                    //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SuccessfullyLoggedInAccounts);

                    ///JS, CSS, Image Requests
                    //RequestsJSCSSIMG.RequestJSCSSIMG(pageSource, ref HttpHelper); 
                    #endregion
                }
                else
                {
                    GlobusLogHelper.log.Info("Couldn't login with Username : " + facebookUser.username);
                    facebookUser.isloggedin = false;


                    if (loginStatus == "account has been disabled")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_DisabledAccount);
                    }

                    if (loginStatus == "Please complete a security check")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SecurityCheckAccounts);
                    }


                    if (loginStatus == "Your account is temporarily locked")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_TemporarilyLockedAccount);

                    }
                    if (loginStatus == "have been blocked")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_havebeenblocked);

                    }
                    if (loginStatus == "For security reasons your account is temporarily locked")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_SecurityCheckAccountsforsecurityreason);
                    }

                    if (loginStatus == "Account Not Confirmed")
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_AccountNotConfirmed);
                    }
                    if (loginStatus == "Temporarily Blocked for 30 Days")
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, Globals.path_30daysBlockedAccount);
                    }
                }
            }
            catch (Exception ex)
            {

                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }


        /// <summary>
        /// Checks if account is valid
        /// </summary>
        public bool CheckLogin(string response, string username, string password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, ref string loginStatus)
        {

            try
            {
                if (!string.IsNullOrEmpty(response))
                {


                    if (response.ToLower().Contains("unusual login activity"))
                    {
                        loginStatus = "unusual login activity";
                        Console.WriteLine("Unusual Login Activity: " + username);
                        return false;
                    }
                    if (response.ToLower().Contains("incorrect username"))
                    {
                        loginStatus = "incorrect username";
                        Console.WriteLine("Incorrect username: " + username);
                        return false;
                    }
                    if (response.ToLower().Contains("Choose a verification method".ToLower()))
                    {
                        loginStatus = "Choose a verification method";
                        Console.WriteLine("Choose a verification method: " + username);
                        return false;
                    }
                    if (response.ToLower().Contains("not logged in".ToLower()) && response.ToLower().Contains("It looks like you're not logged in"))
                    {
                        loginStatus = "not logged in";
                        Console.WriteLine("not logged in: " + username);
                        return false;
                    }
                    if (response.Contains("Please log in to continue".ToLower()))
                    {
                        loginStatus = "Please log in to continue";
                        Console.WriteLine("Please log in to continue: " + username);
                        return false;
                    }
                    if (response.Contains("re-enter your password"))
                    {
                        loginStatus = "re-enter your password";
                        Console.WriteLine("Wrong password for: " + username);
                        return false;
                    }
                    if (response.Contains("Incorrect Email"))
                    {
                        loginStatus = "Incorrect Email";
                        Console.WriteLine("Incorrect email: " + username);

                        try
                        {
                            ///Write Incorrect Emails in text file
                            //GlobusFileHelper.AppendStringToTextfileNewLine(username + ":" + password, incorrectEmailFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }


                        return false;
                    }
                    if (response.Contains("have been blocked"))
                    {
                        loginStatus = "have been blocked";
                        Console.WriteLine("you have been blocked: " + username);
                        return false;
                    }
                    if (response.Contains("account has been disabled"))
                    {
                        loginStatus = "account has been disabled";
                        Console.WriteLine("your account has been disabled: " + username);
                        return false;
                    }
                    if (response.Contains("Please complete a security check"))
                    {
                        loginStatus = "Please complete a security check";
                        Console.WriteLine("Please complete a security check: " + username);
                        return false;
                    }
                    if (response.Contains("Please complete a security check"))
                    {
                        loginStatus = "Please complete a security check";
                        Console.WriteLine("You must log in to see this page: " + username);
                        return false;
                    }
                    if (response.Contains("<input value=\"Sign Up\" onclick=\"RegistrationBootloader.bootloadAndValidate();"))
                    {
                        loginStatus = "RegistrationBootloader.bootloadAndValidate()";
                        Console.WriteLine("Not logged in with: " + username);
                        return false;
                    }
                    if (response.Contains("Account Not Confirmed"))
                    {
                        loginStatus = "Account Not Confirmed";
                        Console.WriteLine("Account Not Confirmed " + username);
                        return false;
                    }
                    if (response.Contains("Your account is temporarily locked"))
                    {
                        loginStatus = "Your account is temporarily locked";
                        Console.WriteLine("Your account is temporarily locked: " + username);
                        return false;
                    }
                    if (response.Contains("Your account has been temporarily suspended"))
                    {
                        loginStatus = "Your account has been temporarily suspended";
                        Console.WriteLine("Your account has been temporarily suspended: " + username);
                        return false;
                    }
                    if (response.Contains("You must log in to see this page"))
                    {
                        Console.WriteLine("You must log in to see this page: " + username);
                        return false;
                    }
                    if (response.ToLower().Contains("you must log in to see this page"))
                    {
                        Console.WriteLine("You must log in to see this page: " + username);
                        return false;
                    }
                    if (response.ToLower().Contains("you entered an old password"))
                    {
                        loginStatus = "you entered an old password";
                        Console.WriteLine("You Entered An Old Password: " + username);
                        return false;
                    }
                    if (response.Contains("For security reasons your account is temporarily locked"))
                    {
                        loginStatus = "For security reasons your account is temporarily locked";
                        Console.WriteLine("For security reasons your account is temporarily locked: " + username);
                        return false;
                    }
                    if (response.Contains("Please Verify Your Identity") || response.Contains("please Verify Your Identity"))
                    {
                        loginStatus = "Please Verify Your Identity";
                        Console.WriteLine("Please Verify Your Identity: " + username);
                        return false;
                    }
                    if (response.Contains("Temporarily Blocked for 30 Days"))
                    {
                        loginStatus = "Temporarily Blocked for 30 Days";
                        Console.WriteLine("You're Temporarily Blocked for 30 Days: " + username);
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


            return true;
        }

        // All Activity Of Account Verification

        #region Global Variables For Account Verification

        public bool isStopAccountVerification = false;

        public List<Thread> lstAccountVerificationThreads = new List<Thread>();

        public static int minDelayAccountVerification = 10;
        public static int maxDelayAccountVerification = 20;

        #endregion

        public static string StartAccountVerificationProcessUsing
        {
            get;
            set;
        }

        public static string exportFilePathAccountVerification
        {
            get;
            set;
        }

        //  public static string AccountVerificationProcessUsing { get; set; }


        /// <summary>
        /// Start Account Verification by ajay yadav
        /// </summary>
        /// <param name="obj"></param>
        /// 


        public void StartAccountVerification(object obj)
        {

            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            if (isStopAccountVerification)
            {
                return;
            }
            try
            {
                lstAccountVerificationThreads.Add(Thread.CurrentThread);
                lstAccountVerificationThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }

            FacebookUser objFacebookUser = new FacebookUser();

            string accountUser = string.Empty;
            string accountPass = string.Empty;
            string proxyAddress = string.Empty;
            string proxyPort = string.Empty;
            string proxyUserName = string.Empty;
            string proxyPassword = string.Empty;
            string dateOfBirth = string.Empty;
            string securityAnswer = string.Empty;

            try
            {
                Array paramsArray = new object[10];
                paramsArray = (Array)obj;
                try
                {
                    string email = (string)paramsArray.GetValue(0);
                    try
                    {
                        if (email.Contains(":"))
                        {
                            string[] emailArr = email.Split(':');
                            if (emailArr.Length > 1)
                            {
                                try
                                {

                                    try
                                    {
                                        accountUser = emailArr[0];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        accountPass = emailArr[1];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        proxyAddress = emailArr[2];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        proxyPort = emailArr[3];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        proxyUserName = emailArr[4];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        proxyPassword = emailArr[5];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        dateOfBirth = emailArr[6];
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    try
                                    {
                                        securityAnswer = emailArr[7];
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
                                GlobusLogHelper.log.Info(email + " Is Wrong Format !");
                                GlobusLogHelper.log.Debug(email + " Is Wrong Format !");
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(email + " Is Wrong Format !");
                            GlobusLogHelper.log.Debug(email + " Is Wrong Format !");
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


                objFacebookUser.username = accountUser;
                objFacebookUser.password = accountPass;
                objFacebookUser.proxyip = proxyAddress;
                objFacebookUser.proxyport = proxyPort;
                objFacebookUser.proxyusername = proxyUserName;
                objFacebookUser.proxypassword = proxyPassword;
                objFacebookUser.dateOfBirth = dateOfBirth;
                objFacebookUser.securityAnswer = securityAnswer;

                GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


                if (!objFacebookUser.isloggedin)
                {

                    if (StartAccountVerificationProcessUsing == "Solve Security" || StartAccountVerificationProcessUsing == "Check Facebook Account")
                    {
                        if (StartAccountVerificationProcessUsing == "Solve Security")
                        {
                            AccountVerificationAcction(ref objFacebookUser);
                        }
                        else if (StartAccountVerificationProcessUsing == "Check Facebook Account")
                        {
                            if (StartAccountVerificationProcessUsing == "Check Facebook Account")
                            {
                               //CheckFacebookAccounts(ref fbUser);
                                CheckFacebookAccounts(ref objFacebookUser);
                            }
                        }
                    }
                    else
                    {
                        //Login Process
                        LoginUsingGlobusHttp(ref objFacebookUser);
                    }

                }
                
                if (objFacebookUser.isloggedin)
                {
                    
                    // Call AccountVerification()                   
                    AccountVerificationAcction(ref objFacebookUser);
                }
                else
                {
                   // GlobusLogHelper.log.Info("Couldn't Login With Username : " + objFacebookUser.username);
                   // GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);

                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void AccountVerificationAcction(ref FacebookUser fbUser)
        {
            try
            {
                lstAccountVerificationThreads.Add(Thread.CurrentThread);
                lstAccountVerificationThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            try
            {
                if (StartAccountVerificationProcessUsing == "Account Verification")
                {
                    if (fbUser.isloggedin)
                    {
                        GlobusLogHelper.log.Info("Start Process Of Account Verification With Username : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Start Process Of Account Verification With Username : " + fbUser.username);

                        VerifiyAccount(fbUser.username, fbUser.password, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, "", ref fbUser.globusHttpHelper, "");
                    }
                }

                if (StartAccountVerificationProcessUsing == "Resend confirmation email")
                {
                    //Resend Confirmation..

                    GlobusLogHelper.log.Info("Start Process Of Confirmation email resent with User Name : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Start Process Of Confirmation email resent with User Name : " + fbUser.username);

                    ResendConfirmationEmail(ref fbUser);

                }

                if (StartAccountVerificationProcessUsing == "Solve Security")
                {
                    GlobusLogHelper.log.Info("Start Process Of Solve Security  With Username : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Start Process Of Solve Security  With Username : " + fbUser.username);

                    LoginAccount(ref fbUser);
                }
                if (StartAccountVerificationProcessUsing == "Remove Mobile Number")
                {
                    if (fbUser.isloggedin)
                    {
                        GlobusLogHelper.log.Info("Start Process Remove Mobile Number from Seting : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Start Process Remove Mobile Number from Seting : " + fbUser.username);
                     
                        StartRemoveMobileFromSetting(ref fbUser);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }


            GlobusLogHelper.log.Info("All Process Completed With Username : " + fbUser.username);
            GlobusLogHelper.log.Debug("All Process Completed With Username : " + fbUser.username);
        }

        int conuntAccount = 1;

        public void LoginAccount(ref FacebookUser fbUser)
        {
            try
            {
                GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

                int intProxyPort = 80;

                Regex IdCheck = new Regex("^[0-9]*$");

                if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
                {
                    intProxyPort = int.Parse(fbUser.proxyport);
                }
                string pageSource = string.Empty;
                try
                {
                    pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), fbUser.proxyip, intProxyPort, fbUser.proxyusername, fbUser.proxypassword);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (pageSource == null)
                {
                    try
                    {
                        Thread.Sleep(500);
                        pageSource = HttpHelper.getHtmlfromUrlProxy(new Uri(FBGlobals.Instance.fbLoginPhpUrl), fbUser.proxyip, intProxyPort, fbUser.proxyusername, fbUser.proxypassword);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                if (pageSource == null)
                {
                    return;
                }
                string lsd = string.Empty;
                try
                {
                    lsd = GlobusHttpHelper.GetParamValue(pageSource, "lsd");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
               

                Thread.Sleep(500);
                string ResponseLogin = string.Empty;
                try
                {
                    string postData = "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + lsd + "&locale=en_US&email=" + fbUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + fbUser.username.Split('@')[1] + "&pass=" + (fbUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + lsd + "";
                    ResponseLogin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationLoginPhpAttempt), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + lsd + "&locale=en_US&email=" + fbUser.username.Split('@')[0].Replace("+", "%2B") + "%40" + fbUser.username.Split('@')[1] + "&pass=" + (fbUser.password) + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + lsd + "");     //"https://www.facebook.com/login.php?login_attempt=1"
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (CheckLoginForChecker(ref fbUser, ResponseLogin, fbUser.username, fbUser.password, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, fbUser.dateOfBirth, fbUser.securityAnswer, HttpHelper.gResponse.ResponseUri.ToString(), ref HttpHelper))
                {
                    try
                    {
                        string src_Confirmyouremail = string.Empty;
                        try
                        {
                            src_Confirmyouremail = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbWelcomeUrl));
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        if (!ResponseLogin.Contains("confirmemail.php") || !src_Confirmyouremail.Contains("confirmemail.php"))
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\CorrectAccount.txt");
                            GlobusLogHelper.log.Info("Correct Account : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Correct Account : " + fbUser.username);
                        }


                        if (ResponseLogin.Contains("confirmemail.php") || src_Confirmyouremail.Contains("confirmemail.php"))
                        {
                            try
                            {
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\ResendConfirmAccounts.txt");
                                GlobusLogHelper.log.Info("Not Confirmed Account : " + fbUser.username);
                                GlobusLogHelper.log.Debug("Not Confirmed Account : " + fbUser.username);
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
                                string str_checkpendingAccount = string.Empty;
                                try
                                {
                                    str_checkpendingAccount = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationSetingUrl));
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                                if (str_checkpendingAccount.Contains("(Pending)"))
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\PendingAccounts.txt");
                                    GlobusLogHelper.log.Info("Account verification is pending :" + fbUser.username);
                                    GlobusLogHelper.log.Debug("Account verification is pending :" + fbUser.username);
                                }
                                else
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\RequiresPhoneVerification.txt");
                                    GlobusLogHelper.log.Info("Requires Phone Verification :" + fbUser.username);
                                    GlobusLogHelper.log.Debug("Requires Phone Verification :" + fbUser.username);
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

                conuntAccount++;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            finally { Thread.Sleep(3000); };
        }


        public bool CheckLoginForChecker(ref FacebookUser fbUser, string response, string username, string password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string DateofBirths, string securityAns, string responseURI, ref GlobusHttpHelper HttpHelper)
        {
            try
            {

                int counter_UnlockingProcess = 0;

                if (!string.IsNullOrEmpty(response))
                {

                    if (response.Contains("Confirm Your Email Address") || response.ToLower().Contains("confirm your email address"))
                    {
                        try
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\ConfirmYourEmailAddress_Account.txt");
                            GlobusLogHelper.log.Info("Confirm Your Email Address :" + fbUser.username);
                            GlobusLogHelper.log.Debug("Confirm Your Email Address :" + fbUser.username);


                            return false;
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    responseURI = responseURI.ToLower();
                    response = UnlockMethod(ref fbUser, ref response, ref HttpHelper);

                    response = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));

                    responseURI = HttpHelper.responseURI;

                    counter_UnlockingProcess++;

                    if (responseURI.Contains("checkpoint"))
                    {
                        if (counter_UnlockingProcess <= 2)
                        {
                            response = UnlockMethod(ref fbUser, ref response, ref HttpHelper);
                            counter_UnlockingProcess++;
                        }
                    }

                    RequestFacebookHomePage(ref fbUser, HttpHelper, ref response);

                    if (response.Contains("account disabled"))
                    {
                        Console.WriteLine("account disabled: " + username);
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\DisabledAccounts.txt");
                        GlobusLogHelper.log.Info("Account Disabled : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Account Disabled : " + fbUser.username);

                        return false;
                    }

                    if (response.Contains("unusual login activity"))
                    {
                        Console.WriteLine("Unusual Login Activity: " + username);
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UnusualLoginActivity.txt");
                        GlobusLogHelper.log.Info("Unusual Login Activity : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Unusual Login Activity : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Incorrect Email/Password Combination"))
                    {
                        Console.WriteLine("Incorrect Email/Password Combination: " + username);

                        try
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\IncorrectEmailsPasswordCombinations.txt");
                            GlobusLogHelper.log.Info("Incorrect Emails Password Combination Login Activity : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Incorrect Emails Password Combination Login Activity : " + fbUser.username);

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        return false;
                    }
                    if (response.Contains("Incorrect Email"))
                    {
                        Console.WriteLine("Incorrect email: " + username);

                        try
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\IncorrectEmailsAccount.txt");
                            GlobusLogHelper.log.Info("Incorrect Emails Account : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Incorrect Emails Account : " + fbUser.username);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        return false;
                    }
                    if (response.Contains("incorrect username"))
                    {
                        Console.WriteLine("Incorrect username: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\IncorrectEmailsAccount.txt");
                        GlobusLogHelper.log.Info("Incorrect username : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Incorrect username : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("choose a verification method".ToLower()))
                    {
                        Console.WriteLine("Choose a verification method: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityCheckAccounts.txt");
                        GlobusLogHelper.log.Info("Choose a verification method : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Choose a verification method : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("not logged in".ToLower()))
                    {
                        Console.WriteLine("not logged in: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UndefinedErrorAccounts.txt");
                        GlobusLogHelper.log.Info("not logged in : " + fbUser.username);
                        GlobusLogHelper.log.Debug("not logged in : " + fbUser.username);


                        return false;
                    }
                    if (response.Contains("Please log in to continue".ToLower()))
                    {
                        Console.WriteLine("Please log in to continue: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UndefinedErrorAccounts.txt");
                        GlobusLogHelper.log.Info("Please log in to continue : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Please log in to continue : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("re-enter your password"))
                    {
                        Console.WriteLine("Wrong password for: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\IncorrectPassword.txt");
                        GlobusLogHelper.log.Info("Wrong password for : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Wrong password for : " + fbUser.username);

                        return false;
                    }

                    if (response.Contains("have been blocked"))
                    {
                        Console.WriteLine("you have been blocked: " + username);
                        return false;
                    }
                    if (response.Contains("account has been disabled"))
                    {
                        Console.WriteLine("your account has been disabled: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\DisabledAccounts.txt");
                        GlobusLogHelper.log.Info("your account has been disabled : " + fbUser.username);
                        GlobusLogHelper.log.Debug("your account has been disabled : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Please complete a security check"))
                    {
                        Console.WriteLine("Please complete a security check: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityCheckAccounts.txt");
                        GlobusLogHelper.log.Info("Please complete a security check : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Please complete a security check : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Please complete a security check"))
                    {
                        Console.WriteLine("You must log in to see this page: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityCheckAccounts.txt");
                        GlobusLogHelper.log.Info("Please complete a security check : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Please complete a security check : " + fbUser.username);


                        return false;
                    }

                    if (response.Contains("Your Account's Temporarily Blocked"))
                    {
                        Console.WriteLine("Your Account's Temporarily Blocked: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\emporarilyBlockedAccounts.txt");
                        GlobusLogHelper.log.Info("Your Account's Temporarily Blocked : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Your Account's Temporarily Blocked : " + fbUser.username);


                        return false;


                    }
                    if (response.Contains("Your Account&#039;s Temporarily Blocked"))
                    {
                        Console.WriteLine("Your Account's Temporarily Blocked: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\emporarilyBlockedAccounts.txt");
                        GlobusLogHelper.log.Info("Your Account's Temporarily Blocked : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Your Account's Temporarily Blocked : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Your account has been temporarily suspended"))
                    {
                        Console.WriteLine("Your account has been temporarily suspended: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\TemporarilySuspendedsAccounts.txt");
                        GlobusLogHelper.log.Info("Your account has been temporarily suspended : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Your account has been temporarily suspended : " + fbUser.username);

                        return false;


                    }
                    if (response.Contains("<input value=\"Sign Up\" onclick=\"RegistrationBootloader.bootloadAndValidate();"))
                    {
                        Console.WriteLine("Not logged in with: " + username);
                        return false;
                    }
                    if (response.Contains("Account Not Confirmed"))
                    {
                        Console.WriteLine("Account Not Confirmed " + fbUser.username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\AccountNotConfirmedAccounts.txt");
                        GlobusLogHelper.log.Info("Account Not Confirmed : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Account Not Confirmed : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Your account is temporarily locked"))
                    {
                        Console.WriteLine("Your account is temporarily locked: " + username);
                        bool res = CheckTemporarilyAccount(ref fbUser, ref HttpHelper);
                        if (!res)
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\TemporarilyLockedAccount.txt");
                            GlobusLogHelper.log.Info("Your account is temporarily locked : " + fbUser.username);
                            GlobusLogHelper.log.Debug("Your account is temporarily locked : " + fbUser.username);

                        }
                        return false;
                    }
                    if (response.Contains("You must log in to see this page"))
                    {
                        Console.WriteLine("You must log in to see this page: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UndefinedErrorAccounts.txt");
                        GlobusLogHelper.log.Info("You must log in to see this page : " + fbUser.username);
                        GlobusLogHelper.log.Debug("You must log in to see this page : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("you must log in to see this page"))
                    {
                        Console.WriteLine("You must log in to see this page: " + username);
                        return false;
                    }
                    if (response.Contains("you entered an old password"))
                    {
                        Console.WriteLine("You Entered An Old Password: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\IncorrectPassword.txt");
                        GlobusLogHelper.log.Info("You Entered An Old Password : " + fbUser.username);
                        GlobusLogHelper.log.Debug("You Entered An Old Password : " + fbUser.username);

                        return false;
                    }
                    if (response.Contains("Temporarily Blocked for 30 Days"))
                    {
                        Console.WriteLine("You're Temporarily Blocked for 30 Days: " + username);

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\30daysBlockedAccount.txt");
                        GlobusLogHelper.log.Info("You're Temporarily Blocked for 30 Days : " + fbUser.username);
                        GlobusLogHelper.log.Debug("You're Temporarily Blocked for 30 Days : " + fbUser.username);

                        return false;
                    }

                    if (response.Contains("type=\"submit\"") && response.Contains("value=\"Log in\""))
                    {
                        Console.WriteLine("You're Temporarily Blocked for 30 Days: " + username);

                        //GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\30daysBlockedAccount.txt");
                        GlobusLogHelper.log.Info("Log in again : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Log in again : " + fbUser.username);

                        return false;
                    }
                }
                else
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UndefinedErrorAccounts.txt");
                    GlobusLogHelper.log.Info("Undefined Error Account : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Undefined Error ccount : " + fbUser.username);

                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                return false;
            }
        }

        private bool CheckTemporarilyAccount(ref FacebookUser fbUser, ref GlobusHttpHelper HttpHelper)
        {

            try
            {
                string fb_dtsg = "";
                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));     //"https://www.facebook.com/checkpoint/")
                if (strPageSource.Contains("It looks like someone else may have accessed your account, so we've temporarily locked it to keep it safe. For your privacy"))
                {
                    try
                    {
                        fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                        fb_dtsg = Uri.EscapeDataString(fb_dtsg);
                        string temp_nh = strPageSource.Substring(strPageSource.IndexOf("name=\"nh\" value="), (strPageSource.IndexOf(">", strPageSource.IndexOf("name=\"nh\" value=")) - strPageSource.IndexOf("name=\"nh\" value="))).Replace("name=\"nh\" value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                        string tempPostUrl = FBGlobals.Instance.AccountVerificationCheckpointUrl;                 //"https://www.facebook.com/checkpoint/";

                        string postdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";
                        string tempresponse = HttpHelper.postFormData(new Uri(tempPostUrl), postdata, FBGlobals.Instance.AccountVerificationCheckpointUrl);    //"https://www.facebook.com/checkpoint/"

                        if (tempresponse.Contains("In order to regain access to your account, please log in from a computer you have used before"))
                        {
                            try
                            {
                                string postseconddata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5Bprovide_id%5D%5Bprovide%2Bidentification%2Bto%2Bregain%2Baccess%5D=provide+identification+to+regain+access";
                                string secondResponse = HttpHelper.postFormData(new Uri(tempPostUrl), postseconddata, FBGlobals.Instance.AccountVerificationCheckpointUrl);   //"https://www.facebook.com/checkpoint/"
                                if (secondResponse.Contains("Upload a photo ID"))
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityAccountWithPhotoVerify.txt");
                                    GlobusLogHelper.log.Info("Your Account Need PhotoId : " + fbUser.username);
                                    GlobusLogHelper.log.Debug("Your Account Need PhotoId : " + fbUser.username);

                                    return true;
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
                else
                {
                    return false;

                }
                return false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        string strContinue_value = "value=\"Continue";
        string strContinue_name = "name=\"submit[Continue]";
        string strSkip_value = "value=\"Skip";
        string strSkip_name = "name=\"submit[Skip]";
        string stSubmit_value = "value=\"Submit";
        string stSubmit_name = "name=\"submit[Submit]";
        string strCaptcha_id = "id=\"captcha";
        string strCaptcha_name = "name=\"captcha_persist_data";
        string strCaptcha_persistID = "id=\"captcha_persist_data";
        string strTempLocked = "temporarily blocked";
        string strOkay = "name=\"submit[Okay]";

        string fb_dtsg = "";
        string nh = "";

        private string UnlockMethod(ref FacebookUser fbUser, ref string response, ref GlobusHttpHelper HttpHelper)
        {
            string tempResponse = string.Empty;
            try
            {
                if (response.Contains(strContinue_name) && response.Contains(strContinue_value))
                {
                    GetFbdtsgNh(ref fb_dtsg, ref nh, response);
                    response = Continue(HttpHelper, fb_dtsg, nh);
                }
                if (response.Contains(strCaptcha_name) && response.Contains(strCaptcha_id))
                {

                    bool status_CaptchaSolving = EntercaptchaForSecurityCheck(ref fbUser, ref HttpHelper);
                    response = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl)); //"https://www.facebook.com/checkpoint/"

                }
                if (response.Contains("please enter your birthday") || response.Contains("Provide your birthday"))
                {
                    GlobusLogHelper.log.Info("Start process birthday solving : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Start process birthday solving : " + fbUser.username);

                    if (response.Contains("Provide your birthday"))
                    {
                        ProvideYourBirthday(ref fbUser, ref response, ref HttpHelper);
                    }

                    BirthdaySolving(ref fbUser, ref HttpHelper, ref response);
                }

                response = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));         //"https://www.facebook.com/checkpoint/"

                if (response.Contains(strContinue_name) && response.Contains(strContinue_value))
                {
                    GetFbdtsgNh(ref fb_dtsg, ref nh, response);

                    response = Continue(HttpHelper, fb_dtsg, nh);
                }

                tempResponse = response.ToLower();

                if (response.ToLower().Contains("temporarily blocked") || response.Contains("temporarily locked") || response.Contains("temporarily suspended"))
                {

                    if (response.Contains(strCaptcha_name) && response.Contains(strCaptcha_id))
                    {
                        EntercaptchaForSecurityCheck(ref fbUser, ref HttpHelper);
                    }
                    if (response.Contains(strOkay))
                    {
                        GetFbdtsgNh(ref fb_dtsg, ref nh, response);
                        OkayModified(HttpHelper, fb_dtsg, nh);
                    }

                    if (response.Contains("Your account is temporarily locked"))
                    {
                        bool status = EntercaptchaForTemporarilyLockedAccount(ref fbUser, ref HttpHelper);

                    }
                }

                response = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));                 //"https://www.facebook.com/checkpoint/"
                if (response.Contains(strOkay))
                {
                    GetFbdtsgNh(ref fb_dtsg, ref nh, response);
                    OkayModified(HttpHelper, fb_dtsg, nh);//Okay(HttpHelper, fb_dtsg, nh);
                }
                else if (response.Contains(strContinue_name) && response.Contains(strContinue_value))
                {
                    GetFbdtsgNh(ref fb_dtsg, ref nh, response);

                    response = Continue(HttpHelper, fb_dtsg, nh);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
            return tempResponse;
        }

        public void ProvideYourBirthday(ref FacebookUser fbUser, ref string response, ref GlobusHttpHelper HttpHelper)
        {
            try
            {
                GlobusLogHelper.log.Info("Try to solving provide your birthday : " + fbUser.username);
                GlobusLogHelper.log.Debug("Try to solving provide your birthday : " + fbUser.username);

                GetFbdtsgNh(ref fb_dtsg, ref nh, response);
                string postData = "fb_dtsg=" + fb_dtsg + "&nh=" + nh + "&achal=2&submit%5BSubmit%5D=Submit";
                string resSubmit = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointNextUrl), postData);                  //"https://www.facebook.com/checkpoint/?next"

                if (string.IsNullOrEmpty(resSubmit))
                {
                    Thread.Sleep(2 * 1000);
                    resSubmit = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointNextUrl), postData);
                }

                response = resSubmit;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private bool EntercaptchaForTemporarilyLockedAccount(ref FacebookUser fbUser, ref GlobusHttpHelper HttpHelper)
        {
            string googlecaptcha = string.Empty;
            string fb_dtsg = "";
            string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));                                       //"https://www.facebook.com/checkpoint/"
            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
            fb_dtsg = Uri.EscapeDataString(fb_dtsg);
            string temp_nh = strPageSource.Substring(strPageSource.IndexOf("name=\"nh\" value="), (strPageSource.IndexOf(">", strPageSource.IndexOf("name=\"nh\" value=")) - strPageSource.IndexOf("name=\"nh\" value="))).Replace("name=\"nh\" value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
            string tempPostUrl = FBGlobals.Instance.AccountVerificationCheckpointUrl;                                                                                  //"https://www.facebook.com/checkpoint/";

            string temppostdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";

            string tempresponse = HttpHelper.postFormData(new Uri(tempPostUrl), temppostdata, FBGlobals.Instance.AccountVerificationCheckpointUrl);                     //"https://www.facebook.com/checkpoint/
            string[] captchpersist = Regex.Split(tempresponse, "id=\"captcha_persist_data");
            string captchaimageurl = string.Empty;
            string captcha_persist_data = string.Empty;
            string captcha_session = "";
            string extra_challenge_params = "";
            string challengeurl = "";
            string captchaText = "";
            if (tempresponse.Contains("captcha_persist_data"))
            {
                try
                {

                    string[] facebookcaptcha_persistdata = Regex.Split(tempresponse, "id=\"captcha_persist_data");
                    captcha_persist_data = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            if (tempresponse.Contains("captcha_session"))
            {
                try
                {
                    string captcha_session_val = tempresponse.Substring(tempresponse.IndexOf("captcha_session"), 200);
                    string[] Arr_captcha_session_val = captcha_session_val.Split('"');
                    captcha_session = Arr_captcha_session_val[4];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            if (tempresponse.Contains("extra_challenge_params"))
            {
                try
                {
                    string extra_challenge_params_val = tempresponse.Substring(tempresponse.IndexOf("extra_challenge_params"), 500);
                    string[] Arr_extra_challenge_params_val = extra_challenge_params_val.Split('"');
                    string authp_pisg_nonce_tt = Arr_extra_challenge_params_val[4];
                    extra_challenge_params = Arr_extra_challenge_params_val[4];
                    extra_challenge_params = extra_challenge_params.Replace("=", "%3D");
                    extra_challenge_params = extra_challenge_params.Replace("&amp;", "%26");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            string tempcaptchresponse1 = string.Empty;
            if (tempresponse.Contains("{create_captcha"))
            {
                try
                {
                    string k = tempresponse.Substring(tempresponse.IndexOf("{create_captcha"), (tempresponse.IndexOf("}", tempresponse.IndexOf("{create_captcha")) - tempresponse.IndexOf("{create_captcha"))).Replace("{create_captcha", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string authp = tempresponse.Substring(tempresponse.IndexOf("authp="), (tempresponse.IndexOf(";", tempresponse.IndexOf("authp=")) - tempresponse.IndexOf("authp="))).Replace("authp=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string nonce = tempresponse.Substring(tempresponse.IndexOf("nonce="), (tempresponse.IndexOf(";", tempresponse.IndexOf("nonce=")) - tempresponse.IndexOf("nonce="))).Replace("nonce=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string psig = tempresponse.Substring(tempresponse.IndexOf("psig="), (tempresponse.IndexOf(";", tempresponse.IndexOf("psig=")) - tempresponse.IndexOf("psig="))).Replace("psig=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string tt = tempresponse.Substring(tempresponse.IndexOf("tt="), (tempresponse.IndexOf(";", tempresponse.IndexOf("tt=")) - tempresponse.IndexOf("tt="))).Replace("tt=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string time = tempresponse.Substring(tempresponse.IndexOf("time="), (tempresponse.IndexOf(";", tempresponse.IndexOf("time=")) - tempresponse.IndexOf("time="))).Replace("time=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    string googleimageurl = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationRecaptchaApiChallengeUrl + k + "&ajax=1&xcachestop=0.37163324314553736&authp=" + authp + "&psig=" + psig + "&nonce=" + nonce + "&tt=" + tt + "&time=" + time + "&new_audio_default=1"));  //"https://www.google.com/recaptcha/api/challenge?k="
                    if (googleimageurl.Contains("challenge"))
                    {
                        challengeurl = googleimageurl.Substring(googleimageurl.IndexOf("challenge :"), (googleimageurl.IndexOf(",", googleimageurl.IndexOf("challenge :")) - googleimageurl.IndexOf("challenge :"))).Replace("challenge :", string.Empty).Trim().Replace(")", string.Empty).Replace("'", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                        googlecaptcha = FBGlobals.Instance.AccountVerificationRecaptchaApiImageUrl + challengeurl;                       //"https://www.google.com/recaptcha/api/image?c="

                        System.Net.WebClient webclient = new System.Net.WebClient();
                        byte[] args = webclient.DownloadData(googlecaptcha);

                        string[] arr1 = new string[3] { FBGlobals.dbcUserName, FBGlobals.dbcPassword, "" };
                        captchaText = DecodeDBC(arr1, args);
                        if (!string.IsNullOrEmpty(captchaText))
                        {
                            string postdataforgoogle = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data + "&captcha_session=" + captcha_session + "&extra_challenge_params=" + extra_challenge_params + "%3D1&recaptcha_type=password&recaptcha_challenge_field=" + challengeurl + "&captcha_response=" + captchaText + "&achal=1&submit%5BSubmit%5D=Submit";
                            tempcaptchresponse1 = HttpHelper.postFormData(new Uri(tempPostUrl), postdataforgoogle, FBGlobals.Instance.AccountVerificationCheckpointUrl);   //"https://www.facebook.com/checkpoint/"
                            if (tempcaptchresponse1.Contains("To verify that you are the owner of this account, please identify the people tagged in the following photos. If you aren't sure about a question, please click") || tempcaptchresponse1.Contains("To make sure this is your account, we need you to upload a color photo of your government-issued ID. Your ID should include your name"))
                            {
                                try
                                {
                                    GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Need Photo for Identification");
                                    GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Need Photo for Identification");

                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                        }
                        else
                        {
                            // GlobusLogHelper.log.Info("google Captcha Not Found ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            string tempcaptchresponse = string.Empty;

            #region
            if (tempresponse.Contains("captcha_challenge_code"))
            {
                if (tempresponse.Contains(FBGlobals.Instance.AccountVerificationCaptchaTfimageUrl))  //"https://www.facebook.com/captcha/tfbimage.php"
                {
                    try
                    {
                        challengeurl = tempresponse.Substring(tempresponse.IndexOf("captcha_challenge_code="), (tempresponse.IndexOf("captcha_challenge_hash=", tempresponse.IndexOf("captcha_challenge_code=")) - tempresponse.IndexOf("captcha_challenge_code="))).Replace("captcha_challenge_code=", string.Empty).Trim().Replace(")", string.Empty).Replace("'", string.Empty).Replace("\"", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty).Trim();
                        string facebookurl = FBGlobals.Instance.AccountVerificationCaptchaTfbimageChallengeUrl + challengeurl + "captcha_challenge_hash=" + captcha_persist_data;   //"https://www.facebook.com/captcha/tfbimage.php?captcha_challenge_code="
                        googlecaptcha = facebookurl;


                        System.Net.WebClient webclient = new System.Net.WebClient();
                        byte[] args = webclient.DownloadData(googlecaptcha);
                        string[] arr1 = new string[3] { FBGlobals.dbcUserName, FBGlobals.dbcPassword, "" };

                        captchaText = DecodeDBC(arr1, args);
                        if (!string.IsNullOrEmpty(captchaText))
                        {

                            string captchapostdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data + "&captcha_response=" + captchaText + "&achal=8&submit%5BSubmit%5D=Submit";
                            tempcaptchresponse = HttpHelper.postFormData(new Uri(tempPostUrl), captchapostdata, FBGlobals.Instance.AccountVerificationCheckpointUrl);

                            try
                            {
                                string PostDataAfterenterSecurity = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data + "&captcha_response=" + fbUser.securityAnswer + "&achal=5&submit%5BSubmit%5D=Submit";
                                string ResponseAfterenterSecurity = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterSecurity, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.StackTrace);
                            }

                            if (tempcaptchresponse.Contains("To verify that you are the owner of this account, please identify the people tagged in the following photos. If you aren't sure about a question, please click") || tempcaptchresponse.Contains("To make sure this is your account, we need you to upload a color photo of your government-issued ID. Your ID should include your name"))
                            {
                                return true;
                            }
                            if (tempcaptchresponse.Contains("Use a phone to verify your account"))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Facebook Captcha Not Found ");
                            GlobusLogHelper.log.Debug("Facebook Captcha Not Found ");

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            #endregion
            #region
                if (tempcaptchresponse.Contains("Please confirm your identity") || tempcaptchresponse.Contains("Provide your birthday") || tempcaptchresponse.Contains("Answer your security question"))
                {
                    try
                    {
                        string[] BirthArr = Regex.Split(fbUser.dateOfBirth, "-");
                        string temp_month = string.Empty;
                        string temp_date = string.Empty;
                        string temp_year = string.Empty;

                        if (BirthArr.Count() > 1)
                        {
                            temp_month = BirthArr[0];
                            temp_date = BirthArr[1];
                            temp_year = BirthArr[2];
                        }
                        string achal = string.Empty;
                        try
                        {
                            string[] achalArr = Regex.Split(tempresponse, "name=\"achal\"");
                            achal = achalArr[1].Substring(achalArr[1].IndexOf("value="), (achalArr[1].IndexOf(">", achalArr[1].IndexOf("value=")) - achalArr[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(";", string.Empty).Trim();
                            if (string.IsNullOrEmpty(achal))
                            {
                                achal = "2";
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                        try
                        {
                            if (tempresponse.Contains("To confirm that this is your account, please enter your date of birth"))
                            {
                                string PostDataAfterenterdateofBirth = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&birthday_captcha_month=" + temp_month + "&birthday_captcha_day=" + temp_date + "&birthday_captcha_year=" + temp_year + "&captcha_persist_data=" + captcha_persist_data + "&achal=2&submit%5BSubmit%5D=Submit";
                                string ResponseDataAfterenterdateofBirth = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterdateofBirth, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                if (ResponseDataAfterenterdateofBirth.Contains("Create a New Password") || ResponseDataAfterenterdateofBirth.Contains(" please select a new password"))
                                {
                                    string NewPassword = fbUser.password + "123";

                                    string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                    string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);

                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                        }

                        #region After click of security condition

                        string captcha_persist_data1 = string.Empty;
                        try
                        {
                            if (!string.IsNullOrEmpty(fbUser.securityAnswer))
                            {

                                string PostForSubmitclick = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&achal=5&submit%5BSubmit%5D=Submit";
                                string SubmitResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostForSubmitclick, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                string[] facebookcaptcha_persistdata = Regex.Split(SubmitResponse, "id=\"captcha_persist_data");
                                try
                                {
                                    captcha_persist_data1 = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                }
                                if (string.IsNullOrEmpty(captcha_persist_data1))
                                {
                                    try
                                    {
                                        facebookcaptcha_persistdata = Regex.Split(SubmitResponse, "name=\"captcha_persist_data\"");
                                        //name="captcha_persist_data"
                                        captcha_persist_data1 = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                }
                                try
                                {
                                    string[] achalArr = Regex.Split(SubmitResponse, "name=\"achal\"");
                                    achal = achalArr[1].Substring(achalArr[1].IndexOf("value="), (achalArr[1].IndexOf(">", achalArr[1].IndexOf("value=")) - achalArr[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(";", string.Empty).Trim();
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
                                    string PostForSubmitclick = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&achal=2&submit%5BSubmit%5D=Submit";
                                    string SubmitResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostForSubmitclick, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                    string[] facebookcaptcha_persistdata = Regex.Split(tempresponse, "id=\"captcha_persist_data");
                                    try
                                    {
                                        captcha_persist_data1 = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                                    }
                                    if (string.IsNullOrEmpty(captcha_persist_data1))
                                    {
                                        try
                                        {
                                            facebookcaptcha_persistdata = Regex.Split(SubmitResponse, "name=\"captcha_persist_data\"");

                                            captcha_persist_data1 = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
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
                        #endregion
                        #region Afetr Enter security
                        try
                        {
                            if (!string.IsNullOrEmpty(fbUser.securityAnswer))
                            {
                                try
                                {
                                    string PostDataAfterenterSecurity = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data1 + "&captcha_response=" + fbUser.securityAnswer + "&achal=5&submit%5BSubmit%5D=Submit";
                                    string ResponseAfterenterSecurity = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterSecurity, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                    if (ResponseAfterenterSecurity.Contains("Create a New Password") || ResponseAfterenterSecurity.Contains(" please select a new password"))
                                    {
                                        string NewPassword = fbUser.password + "123";

                                        string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                        string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                        if (ResponseForCreateNewPassword.Contains("www.facebook.com/welcomeback") || ResponseForCreateNewPassword.Contains("sk=welcome") || ResponseForCreateNewPassword.Contains("Redirecting.."))
                                        {
                                        }
                                        else
                                        {
                                            string postDataForCreateNewPassword1 = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BChange+Password%5D=Change+Password";
                                            string ResponseForCreateNewPassword1 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword1, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                            if (ResponseForCreateNewPassword1.Contains("www.facebook.com/welcomeback") || ResponseForCreateNewPassword1.Contains("sk=welcome") || ResponseForCreateNewPassword1.Contains("Redirecting.."))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ResponseAfterenterSecurity.Contains("The text you entered didn") && ResponseAfterenterSecurity.Contains("match the security check"))
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(fbUser.dateOfBirth))
                                                {
                                                    string PostDataAfterenterdateofBirth = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&birthday_captcha_month=" + temp_month + "&birthday_captcha_day=" + temp_date + "&birthday_captcha_year=" + temp_year + "&captcha_persist_data=" + captcha_persist_data1 + "&achal=2&submit%5BSubmit%5D=Submit";
                                                    string ResponseDataAfterenterdateofBirth = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterdateofBirth, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                                    if (ResponseDataAfterenterdateofBirth.Contains("Create a New Password") || ResponseDataAfterenterdateofBirth.Contains(" please select a new password"))
                                                    {
                                                        string NewPassword = fbUser.password + "123";
                                                        string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                                        string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);

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

                            if (true)
                            {
                                try
                                {
                                    string PostDataAfterenterdateofBirth = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&birthday_captcha_month=" + temp_month + "&birthday_captcha_day=" + temp_date + "&birthday_captcha_year=" + temp_year + "&captcha_persist_data=" + captcha_persist_data1 + "&achal=2&submit%5BSubmit%5D=Submit";
                                    string ResponseDataAfterenterdateofBirth = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterdateofBirth, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                    if (ResponseDataAfterenterdateofBirth.Contains("Create a New Password") || ResponseDataAfterenterdateofBirth.Contains(" please select a new password"))
                                    {
                                        string NewPassword = fbUser.password + "123";
                                        string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                        string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);
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
                        #endregion

                        #region Click on Continue
                        try
                        {
                            try
                            {
                                string postforshare = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";
                                string ResponseDatapostforshare = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postforshare, FBGlobals.Instance.AccountVerificationCheckpointUrl);
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
                        #endregion

                        #region Clickon skip

                        try
                        {
                            string postForSkip = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BSkip%5D=Skip";
                            string ResponseDatapostForSkip = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postForSkip, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                        }

                        #endregion

                        Okay(ref fbUser, HttpHelper, fb_dtsg, temp_nh);

                        #region
                        try
                        {
                            string FinalResponse = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbWelcomeUrl));    //"https://www.facebook.com/?sk=welcome"

                            int Pageresponselengh = FinalResponse.Length;
                            if (Pageresponselengh > 8000)
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
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }

                }
                #endregion

            }
            return false;
        }

        private bool RequestFacebookHomePage(ref FacebookUser fbUser, GlobusHttpHelper HttpHelper, ref string response)
        {
            try
            {
                string FinalResponse = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbWelcomeUrl));                              //"https://www.facebook.com/?sk=welcome"

                response = FinalResponse;

                int Pageresponselengh = FinalResponse.Length;
                if (Pageresponselengh > 8000)
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolved.txt");
                    GlobusLogHelper.log.Info("Your Security Problem Solved using UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Your Security Problem Solved using UserName : " + fbUser.username);

                    return true;
                }
                else
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritynotSolved.txt");
                    GlobusLogHelper.log.Info("Your Security Problem Not Solve using UserName : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Your Security Problem Not Solve using UserName : " + fbUser.username);

                    return false;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
            return false;
        }

        private static void GetFbdtsgNh(ref string fb_dtsg, ref string temp_nh, string strPageSource)
        {
            try
            {
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                fb_dtsg = Uri.EscapeDataString(fb_dtsg);
                temp_nh = GlobusHttpHelper.GetParamValue(strPageSource, "nh");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private static string Continue(GlobusHttpHelper HttpHelper, string fb_dtsg, string temp_nh)
        {
            #region Click on Continue
            try
            {
                try
                {
                    string postforshare = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";
                    string ResponseDatapostforshare = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postforshare, FBGlobals.Instance.AccountVerificationCheckpointUrl);             //"https://www.facebook.com/checkpoint/"
                    return ResponseDatapostforshare;
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
            return string.Empty;
            #endregion
        }

        private bool EntercaptchaForSecurityCheck(ref FacebookUser fbUser, ref GlobusHttpHelper HttpHelper)
        {
            try
            {
                string captchaUrl = string.Empty;
                string fb_dtsg = "";
                string temp_nh = "";
                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl));   //"https://www.facebook.com/checkpoint/"
                GetFbdtsgNh(ref fb_dtsg, ref temp_nh, strPageSource);

                string tempPostUrl = FBGlobals.Instance.AccountVerificationCheckpointUrl;                                         // "https://www.facebook.com/checkpoint/";
                string temppostdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";

                string tempresponse = string.Empty;
                string captcha_persist_data = string.Empty;
                string captcha_session = string.Empty;
                string extra_challenge_params = string.Empty;
                string challengeurl = string.Empty;
                string captchaText = string.Empty;

                int counter_loopCaptcha = 0;

            loopCaptcha:

                GetCaptchaPageSrc(HttpHelper, tempPostUrl, temppostdata, ref tempresponse, ref captcha_persist_data, ref captcha_session, ref extra_challenge_params, ref challengeurl, ref captchaText);

                if (string.IsNullOrEmpty(tempresponse))
                {
                    Thread.Sleep(2 * 1000);
                    GetCaptchaPageSrc(HttpHelper, tempPostUrl, temppostdata, ref tempresponse, ref captcha_persist_data, ref captcha_session, ref extra_challenge_params, ref challengeurl, ref captchaText);
                }

                string tempcaptchresponse1 = string.Empty;
                string tempcaptchresponse = string.Empty;

                if (tempresponse.Contains("{create_captcha"))  //Google Captcha
                {
                    SolveGoogleCaptcha(ref fbUser, HttpHelper, ref captchaUrl, fb_dtsg, temp_nh, tempPostUrl, tempresponse, captcha_persist_data, captcha_session, extra_challenge_params, ref challengeurl, ref captchaText, ref tempcaptchresponse1);
                }
                else if (tempresponse.Contains("captcha_challenge_code")) //Facebook Captcha
                {
                    if (tempresponse.Contains(FBGlobals.Instance.AccountVerificationCaptchaTfimageUrl))                                     //"https://www.facebook.com/captcha/tfbimage.php"
                    {
                        bool isSolve = SolveFacebookCaptcha(ref fbUser, HttpHelper, ref captchaUrl, fb_dtsg, temp_nh, tempPostUrl, tempresponse, captcha_persist_data, ref challengeurl, ref captchaText, ref tempcaptchresponse);

                        if (!isSolve)
                        {
                            counter_loopCaptcha++;
                            if (counter_loopCaptcha <= 3)
                            {
                                GlobusLogHelper.log.Debug("Captcha Image not loaded, retrying... " + counter_loopCaptcha);
                                GlobusLogHelper.log.Info("Captcha Image not loaded, retrying... " + counter_loopCaptcha);
                                goto loopCaptcha;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        return false;
                    }
                }
                else
                {
                    //Retry Captcha
                    counter_loopCaptcha++;
                    if (counter_loopCaptcha <= 3)
                    {
                        GlobusLogHelper.log.Info("Captcha Image not loaded, retrying... : " + counter_loopCaptcha);
                        GlobusLogHelper.log.Debug("Captcha Image not loaded, retrying... :" + counter_loopCaptcha);

                        goto loopCaptcha;
                    }
                }
                Okay(ref fbUser, HttpHelper, fb_dtsg, temp_nh);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return false;
        }

        private void Okay(ref FacebookUser fbUser, GlobusHttpHelper HttpHelper, string fb_dtsg, string temp_nh)
        {
            #region Clickon This Is okay
            try
            {
                string postForThisIsOkay = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BThis+is+Okay%5D=This+is+Okay";
                string ResponseDatapostForThisIsOkay = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postForThisIsOkay, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                if (ResponseDatapostForThisIsOkay.Contains("Create a New Password"))
                {
                    string NewPassword = fbUser.password + "123";
                    string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                    string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                    if (ResponseForCreateNewPassword.Contains("www.facebook.com/welcomeback") || ResponseForCreateNewPassword.Contains("sk=welcome") || ResponseForCreateNewPassword.Contains("Redirecting.."))
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolved.txt");
                        GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);
                        GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);

                        // return true;
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            #endregion
        }

        private bool BirthdaySolving(ref FacebookUser fbUser, ref GlobusHttpHelper HttpHelper, ref string birthdayResponse)
        {
            try
            {
                string captchaimageurl = string.Empty;
                string captcha_persist_data = string.Empty;
                string googlecaptcha = string.Empty;
                string captcha_session = "";
                string extra_challenge_params = "";
                string fb_dtsg = "";

                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(birthdayResponse);
                fb_dtsg = Uri.EscapeDataString(fb_dtsg);

                string temp_nh = birthdayResponse.Substring(birthdayResponse.IndexOf("name=\"nh\" value="), (birthdayResponse.IndexOf(">", birthdayResponse.IndexOf("name=\"nh\" value=")) - birthdayResponse.IndexOf("name=\"nh\" value="))).Replace("name=\"nh\" value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                if (birthdayResponse.Contains("captcha_persist_data"))
                {
                    try
                    {
                        string[] facebookcaptcha_persistdata = Regex.Split(birthdayResponse, "id=\"captcha_persist_data");
                        captcha_persist_data = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }

                if (birthdayResponse.Contains(" please enter your birthday"))
                {
                    try
                    {
                        string[] BirthArr = Regex.Split(fbUser.dateOfBirth, "-");
                        string temp_month = string.Empty;
                        string temp_date = string.Empty;
                        string temp_year = string.Empty;
                        if (BirthArr.Count() > 1)
                        {
                            temp_month = BirthArr[0];
                            temp_date = BirthArr[1];
                            temp_year = BirthArr[2];
                        }
                        string achal = string.Empty;
                        try
                        {
                            string[] achalArr = Regex.Split(birthdayResponse, "name=\"achal\"");
                            achal = achalArr[1].Substring(achalArr[1].IndexOf("value="), (achalArr[1].IndexOf(">", achalArr[1].IndexOf("value=")) - achalArr[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(";", string.Empty).Trim();
                            if (string.IsNullOrEmpty(achal))
                            {
                                achal = "2";
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        try
                        {
                            string PostDataAfterenterdateofBirth = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&birthday_captcha_month=" + temp_month + "&birthday_captcha_day=" + temp_date + "&birthday_captcha_year=" + temp_year + "&captcha_persist_data=" + captcha_persist_data + "&achal=2&submit%5BSubmit%5D=Submit";
                            string ResponseDataAfterenterdateofBirth = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterdateofBirth, FBGlobals.Instance.AccountVerificationCheckpointUrl);

                            if (ResponseDataAfterenterdateofBirth.Contains("You took too much time to complete the security challenge"))
                            {

                                GlobusLogHelper.log.Info("You took too much time to complete the security challenge : " + fbUser.username);
                                GlobusLogHelper.log.Debug("You took too much time to complete the security challenge : " + fbUser.username);

                                return false;

                            }

                            if (ResponseDataAfterenterdateofBirth.Contains("Create a New Password") || ResponseDataAfterenterdateofBirth.Contains("please select a new password"))
                            {
                                string NewPassword = fbUser.password + "123";
                                string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                if (ResponseForCreateNewPassword.Contains("www.facebook.com/welcomeback") || ResponseForCreateNewPassword.Contains("sk=welcome") || ResponseForCreateNewPassword.Contains("Redirecting.."))
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolvedChangePassword.txt");
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolved.txt");

                                    GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);
                                    GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);


                                    return true;
                                }
                                else
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritynotSolved.txt");

                                    GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);
                                    GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);

                                    //  return true;
                                }
                            }
                            else
                            {
                                Continue(HttpHelper, fb_dtsg, temp_nh);
                            }
                            #region Clickon skip
                            Skip(HttpHelper, fb_dtsg, temp_nh);
                            #endregion
                            #region Final Check
                            try
                            {
                                string response = string.Empty;
                                return RequestFacebookHomePage(ref fbUser, HttpHelper, ref response);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                            }
                            #endregion

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

                string tempPostUrl = FBGlobals.Instance.AccountVerificationCheckpointUrl;
                string temppostdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BContinue%5D=Continue";
                string tempresponse = HttpHelper.postFormData(new Uri(tempPostUrl), temppostdata);
                string[] captchpersist = Regex.Split(tempresponse, "id=\"captcha_persist_data");

                if (tempresponse.Contains("captcha_persist_data"))
                {
                    try
                    {
                        string[] facebookcaptcha_persistdata = Regex.Split(tempresponse, "id=\"captcha_persist_data");
                        captcha_persist_data = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (tempresponse.Contains("captcha_session"))
                {
                    try
                    {
                        string captcha_session_val = tempresponse.Substring(tempresponse.IndexOf("captcha_session"), 200);
                        string[] Arr_captcha_session_val = captcha_session_val.Split('"');
                        captcha_session = Arr_captcha_session_val[4];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                if (tempresponse.Contains("extra_challenge_params"))
                {
                    try
                    {
                        string extra_challenge_params_val = tempresponse.Substring(tempresponse.IndexOf("extra_challenge_params"), 500);
                        string[] Arr_extra_challenge_params_val = extra_challenge_params_val.Split('"');
                        string authp_pisg_nonce_tt = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = extra_challenge_params.Replace("=", "%3D");
                        extra_challenge_params = extra_challenge_params.Replace("&amp;", "%26");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                if (tempresponse.Contains("please enter your birthday"))
                {
                    try
                    {
                        string[] BirthArr = Regex.Split(fbUser.dateOfBirth, "-");
                        string temp_month = string.Empty;
                        string temp_date = string.Empty;
                        string temp_year = string.Empty;

                        if (BirthArr.Count() > 1)
                        {
                            temp_month = BirthArr[0];
                            temp_date = BirthArr[1];
                            temp_year = BirthArr[2];
                        }
                        string achal = string.Empty;
                        try
                        {
                            string[] achalArr = Regex.Split(tempresponse, "name=\"achal\"");
                            achal = achalArr[1].Substring(achalArr[1].IndexOf("value="), (achalArr[1].IndexOf(">", achalArr[1].IndexOf("value=")) - achalArr[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Replace(";", string.Empty).Trim();
                            if (string.IsNullOrEmpty(achal))
                            {
                                achal = "2";
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                        try
                        {
                            string PostDataAfterenterdateofBirth = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&birthday_captcha_month=" + temp_month + "&birthday_captcha_day=" + temp_date + "&birthday_captcha_year=" + temp_year + "&captcha_persist_data=" + captcha_persist_data + "&achal=2&submit%5BSubmit%5D=Submit";
                            string ResponseDataAfterenterdateofBirth = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), PostDataAfterenterdateofBirth, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                            if (ResponseDataAfterenterdateofBirth.Contains("Create a New Password") || ResponseDataAfterenterdateofBirth.Contains("please select a new password"))
                            {
                                string NewPassword = fbUser.password + "123";
                                string postDataForCreateNewPassword = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&new_pass=" + NewPassword + "&confirm_pass=" + NewPassword + "&submit%5BSubmit%5D=Submit";
                                string ResponseForCreateNewPassword = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postDataForCreateNewPassword, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                                if (ResponseForCreateNewPassword.Contains("www.facebook.com/welcomeback") || ResponseForCreateNewPassword.Contains("sk=welcome") || ResponseForCreateNewPassword.Contains("Redirecting.."))
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolvedChangePassword.txt");
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritySolved.txt");

                                    GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);
                                    GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);


                                    return true;
                                }
                                else
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecuritynotSolved.txt");

                                    GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);
                                    GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Entered New Password : " + NewPassword);

                                    //  return true;
                                }
                            }
                            else
                            {
                                Continue(HttpHelper, fb_dtsg, temp_nh);
                            }
                            #region Clickon skip
                            Skip(HttpHelper, fb_dtsg, temp_nh);
                            #endregion
                            #region Final Check
                            try
                            {
                                string response = string.Empty;
                                return RequestFacebookHomePage(ref fbUser, HttpHelper, ref response);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                            }
                            #endregion

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

            return false;
        }

        private static void Skip(GlobusHttpHelper HttpHelper, string fb_dtsg, string temp_nh)
        {
            try
            {
                string postForSkip = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BSkip%5D=Skip";
                string ResponseDatapostForSkip = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointUrl), postForSkip, FBGlobals.Instance.AccountVerificationCheckpointUrl);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
        }

        private void OkayModified(GlobusHttpHelper HttpHelper, string fb_dtsg, string temp_nh)
        {
            #region Clickon This Is okay
            try
            {
                string postForThisIsOkay = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&submit%5BOkay%5D=Okay";
                string ResponseDatapostForThisIsOkay = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationCheckpointNextUrl), postForThisIsOkay, FBGlobals.Instance.AccountVerificationCheckpointUrl);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            #endregion
        }

        private static void GetCaptchaPageSrc(GlobusHttpHelper HttpHelper, string tempPostUrl, string temppostdata, ref string tempresponse, ref string captcha_persist_data, ref string captcha_session, ref string extra_challenge_params, ref string challengeurl, ref string captchaText)
        {
            try
            {
                tempresponse = HttpHelper.postFormData(new Uri(tempPostUrl), temppostdata, FBGlobals.Instance.AccountVerificationCheckpointUrl);

                string[] captchpersist = Regex.Split(tempresponse, "id=\"captcha_persist_data");
                string captchaimageurl = string.Empty;
                captcha_persist_data = string.Empty;
                captcha_session = "";
                extra_challenge_params = "";
                challengeurl = "";
                captchaText = "";

                if (tempresponse.Contains("captcha_persist_data"))
                {
                    try
                    {
                        string[] facebookcaptcha_persistdata = Regex.Split(tempresponse, "id=\"captcha_persist_data");
                        captcha_persist_data = facebookcaptcha_persistdata[1].Substring(facebookcaptcha_persistdata[1].IndexOf("value="), (facebookcaptcha_persistdata[1].IndexOf(">", facebookcaptcha_persistdata[1].IndexOf("value=")) - facebookcaptcha_persistdata[1].IndexOf("value="))).Replace("value=", string.Empty).Trim().Replace("\\", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                if (tempresponse.Contains("captcha_session"))
                {
                    try
                    {
                        string captcha_session_val = tempresponse.Substring(tempresponse.IndexOf("captcha_session"), 200);
                        string[] Arr_captcha_session_val = captcha_session_val.Split('"');
                        captcha_session = Arr_captcha_session_val[4];
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                    }
                }
                if (tempresponse.Contains("extra_challenge_params"))
                {
                    try
                    {
                        string extra_challenge_params_val = tempresponse.Substring(tempresponse.IndexOf("extra_challenge_params"), 500);
                        string[] Arr_extra_challenge_params_val = extra_challenge_params_val.Split('"');
                        string authp_pisg_nonce_tt = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = Arr_extra_challenge_params_val[4];
                        extra_challenge_params = extra_challenge_params.Replace("=", "%3D");
                        extra_challenge_params = extra_challenge_params.Replace("&amp;", "%26");
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

        private bool SolveGoogleCaptcha(ref FacebookUser fbUser, GlobusHttpHelper HttpHelper, ref string googlecaptcha, string fb_dtsg, string temp_nh, string tempPostUrl, string tempresponse, string captcha_persist_data, string captcha_session, string extra_challenge_params, ref string challengeurl, ref string captchaText, ref string tempcaptchresponse1)
        {
            try
            {
                GlobusLogHelper.log.Info("Try to solving google captcha : " + fbUser.username);
                GlobusLogHelper.log.Debug("Try to solving google captcha : " + fbUser.username);

                string k = tempresponse.Substring(tempresponse.IndexOf("{create_captcha"), (tempresponse.IndexOf("}", tempresponse.IndexOf("{create_captcha")) - tempresponse.IndexOf("{create_captcha"))).Replace("{create_captcha", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string authp = tempresponse.Substring(tempresponse.IndexOf("authp="), (tempresponse.IndexOf(";", tempresponse.IndexOf("authp=")) - tempresponse.IndexOf("authp="))).Replace("authp=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string nonce = tempresponse.Substring(tempresponse.IndexOf("nonce="), (tempresponse.IndexOf(";", tempresponse.IndexOf("nonce=")) - tempresponse.IndexOf("nonce="))).Replace("nonce=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string psig = tempresponse.Substring(tempresponse.IndexOf("psig="), (tempresponse.IndexOf(";", tempresponse.IndexOf("psig=")) - tempresponse.IndexOf("psig="))).Replace("psig=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string tt = tempresponse.Substring(tempresponse.IndexOf("tt="), (tempresponse.IndexOf(";", tempresponse.IndexOf("tt=")) - tempresponse.IndexOf("tt="))).Replace("tt=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string time = tempresponse.Substring(tempresponse.IndexOf("time="), (tempresponse.IndexOf(";", tempresponse.IndexOf("time=")) - tempresponse.IndexOf("time="))).Replace("time=", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                string googleimageurl = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationRecaptchaApiChallengeUrl + k + "&ajax=1&xcachestop=0.37163324314553736&authp=" + authp + "&psig=" + psig + "&nonce=" + nonce + "&tt=" + tt + "&time=" + time + "&new_audio_default=1"));//https://www.google.com/recaptcha/api/challenge?k=

                if (googleimageurl.Contains("challenge"))
                {
                    challengeurl = googleimageurl.Substring(googleimageurl.IndexOf("challenge :"), (googleimageurl.IndexOf(",", googleimageurl.IndexOf("challenge :")) - googleimageurl.IndexOf("challenge :"))).Replace("challenge :", string.Empty).Trim().Replace(")", string.Empty).Replace("'", string.Empty).Replace("&amp", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                    googlecaptcha = FBGlobals.Instance.AccountVerificationRecaptchaApiImageUrl + challengeurl;                              //"https://www.google.com/recaptcha/api/image?c="

                    System.Net.WebClient webclient = new System.Net.WebClient();
                    byte[] args = webclient.DownloadData(googlecaptcha);

                    string[] arr1 = new string[3] { FBGlobals.dbcUserName, FBGlobals.dbcPassword, "" };
                    captchaText = DecodeDBC(arr1, args);
                    if (!string.IsNullOrEmpty(captchaText))
                    {

                        string postdataforgoogle = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data + "&captcha_session=" + captcha_session + "&extra_challenge_params=" + extra_challenge_params + "%3D1&recaptcha_type=password&recaptcha_challenge_field=" + challengeurl + "&captcha_response=" + captchaText + "&achal=1&submit%5BSubmit%5D=Submit";
                        tempcaptchresponse1 = HttpHelper.postFormData(new Uri(tempPostUrl), postdataforgoogle, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                        if (tempcaptchresponse1.Contains("To verify that you are the owner of this account, please identify the people tagged in the following photos. If you aren't sure about a question, please click") || tempcaptchresponse1.Contains("To make sure this is your account, we need you to upload a color photo of your government-issued ID. Your ID should include your name"))
                        {
                            try
                            {
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityAccountWithPhotoVerify.txt");

                                GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Need Photo for Identification ");
                                GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Need Photo for Identification ");

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

                            }
                            return true;
                        }
                        if (tempcaptchresponse1.Contains("Use a phone to verify your account"))
                        {
                            try
                            {
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UsePhoneToVerifyYourAccount.txt");

                                GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Need Phone Verification ");
                                GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Need Phone Verification ");

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                            return true;
                        }
                        return false;
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

            return false;
        }

        private bool SolveFacebookCaptcha(ref FacebookUser fbUser, GlobusHttpHelper HttpHelper, ref string captchaUrl, string fb_dtsg, string temp_nh, string tempPostUrl, string tempresponse, string captcha_persist_data, ref string challengeurl, ref string captchaText, ref string tempcaptchresponse)
        {
            try
            {
                GlobusLogHelper.log.Info("Try to solving facebook captcha : " + fbUser.username);
                GlobusLogHelper.log.Debug("Try to solving facebook captcha : " + fbUser.username);

                challengeurl = tempresponse.Substring(tempresponse.IndexOf("captcha_challenge_code="), (tempresponse.IndexOf("captcha_challenge_hash=", tempresponse.IndexOf("captcha_challenge_code=")) - tempresponse.IndexOf("captcha_challenge_code="))).Replace("captcha_challenge_code=", string.Empty).Trim().Replace(")", string.Empty).Replace("'", string.Empty).Replace("\"", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty).Trim().Replace("&amp;", "");

                string facebookurl = FBGlobals.Instance.AccountVerificationCaptchaTfbimageChallengeUrl + challengeurl + "&captcha_challenge_hash=" + captcha_persist_data;  //"https://www.facebook.com/captcha/tfbimage.php?captcha_challenge_code="
                captchaUrl = facebookurl;

                byte[] captchaPgsrc = HttpHelper.getImgfromUrl(new Uri(captchaUrl));
                System.Net.WebClient webclient = new System.Net.WebClient();
                string[] arr1 = new string[3] { FBGlobals.dbcUserName, FBGlobals.dbcPassword, "" };
                captchaText = DecodeDBC(arr1, captchaPgsrc);

                if (!string.IsNullOrEmpty(captchaText))
                {
                    string captchapostdata = "fb_dtsg=" + fb_dtsg + "&nh=" + temp_nh + "&geo=true&captcha_persist_data=" + captcha_persist_data + "&captcha_response=" + captchaText + "&achal=8&submit%5BSubmit%5D=Submit";
                    tempcaptchresponse = HttpHelper.postFormData(new Uri(tempPostUrl), captchapostdata, FBGlobals.Instance.AccountVerificationCheckpointUrl);
                    if (tempcaptchresponse.Contains("To verify that you are the owner of this account, please identify the people tagged in the following photos. If you aren't sure about a question, please click") || tempcaptchresponse.Contains("To make sure this is your account, we need you to upload a color photo of your government-issued ID. Your ID should include your name"))
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\SecurityAccountWithPhotoVerify.txt");

                        GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Need Photo for Identification ");
                        GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Need Photo for Identification ");

                        return true;
                    }
                    if (tempcaptchresponse.Contains("Use a phone to verify your account"))
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UsePhoneToVerifyYourAccount.txt");

                        GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Use a phone to verify your account ");
                        GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Use a phone to verify your account ");

                        return true;
                    }
                    if (tempcaptchresponse.Contains("Please confirm your identity") && tempcaptchresponse.Contains("Provide your birthday"))
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\Provideyourbirthday.txt");

                        GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " Provide your birthday ");
                        GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " Provide your birthday ");

                        return true;
                    }

                    if (tempcaptchresponse.Contains("The text you entered didn&#039;t match the security check"))
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\UsePhoneToVerifyYourAccount.txt");

                        GlobusLogHelper.log.Info("Your Username : " + fbUser.username + " The text you entered didn't match the security check ");
                        GlobusLogHelper.log.Debug("Your Username : " + fbUser.username + " The text you entered didn't match the security check ");
                    }
                    return false;
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

        public string DecodeDBC(string[] args, byte[] imageBytes)
        {
            try
            {
                DeathByCaptcha.Client client = (DeathByCaptcha.Client)new DeathByCaptcha.SocketClient(args[0], args[1]);
                client.Verbose = true;

                Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

                if (!client.User.LoggedIn)
                {
                    return null;
                }
                if (client.Balance == 0.0)
                {
                    return null;
                }

                for (int i = 2, l = args.Length; i < l; i++)
                {
                    Console.WriteLine("Solving CAPTCHA {0}", args[i]);
                    DeathByCaptcha.Captcha captcha = client.Decode(imageBytes, 2 * DeathByCaptcha.Client.DefaultTimeout);
                    if (null != captcha)
                    {
                        Console.WriteLine("CAPTCHA {0:D} solved: {1}", captcha.Id, captcha.Text);
                        #region CommentedCode
                        //// Report an incorrectly solved CAPTCHA.
                        //// Make sure the CAPTCHA was in fact incorrectly solved, do not
                        //// just report it at random, or you might be banned as abuser.
                        //if (client.Report(captcha))
                        //{
                        //    Console.WriteLine("Reported as incorrectly solved");
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Failed reporting as incorrectly solved");
                        //} 
                        #endregion
                        return captcha.Text;
                    }
                    else
                    {
                        Console.WriteLine("CAPTCHA was not solved");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
            return null;
        }

         //------------Resend Confirmation Email-------------------//

        public void ResendConfirmationEmail(ref FacebookUser fbUser)
        {
            try
            {
                lstAccountVerificationThreads.Add(Thread.CurrentThread);
                lstAccountVerificationThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string __user = fbUser.username;
            try
            {
                string fb_dtsg = "";
                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
                __user = GlobusHttpHelper.GetParamValue(strPageSource, "user");

                if (string.IsNullOrEmpty(__user))
                {
                    __user = GlobusHttpHelper.ParseJson(strPageSource, "user");
                }

                fb_dtsg = Get_fb_dtsg(strPageSource);
                string strAccountPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.ResendConfirmationEmaiSetingAccountSectionUrl));
                if (strAccountPageSource.Contains("Resend confirmation email"))
                {
                    string strPostData = "__user=" + __user + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=1658166795011511610545";
                    string strPostURL = FBGlobals.Instance.ResendConfirmationEmaiSetingAccountResendUrl;
                    string strResponse = HttpHelper.postFormData(new Uri(strPostURL), strPostData);
                    if (strResponse.Contains("Confirmation email resent"))
                    {

                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\ConfirmationEmailResent.txt");
                        GlobusLogHelper.log.Info("Confirmation email resent with User Name : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Confirmation email resent with User Name : " + fbUser.username);
                    }
                    else
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\Couldno'tConfirmationEmailResend.txt");
                        GlobusLogHelper.log.Info("Couldno't Confirmation email resent with User Name : " + fbUser.username);
                        GlobusLogHelper.log.Debug("Couldno't Confirmation email resent with User Name : " + fbUser.username);
                    }
                }
                else
                {
                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\NoOptionForConfirmationEmailResend.txt");
                    GlobusLogHelper.log.Info("Couldno't Confirmation email resent with User Name : " + fbUser.username);
                    GlobusLogHelper.log.Debug("Couldno't Confirmation email resent with User Name : " + fbUser.username);
                }
                GlobusLogHelper.log.Info("Process is completed with User Name : " + fbUser.username);
                GlobusLogHelper.log.Debug("Process is completed with User Name : " + fbUser.username);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


        public static string Get_fb_dtsg(string pgSrc)
        {
            string fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc, "fb_dtsg");
            if (string.IsNullOrEmpty(fb_dtsg))
            {
                fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc, "fb_dtsg");
            }
            return fb_dtsg;
        }

        public void RemoveDuplicate()
        {
            try
            {
                lstAccountVerificationThreads.Add(Thread.CurrentThread);
                lstAccountVerificationThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }
            try
            {
                if (StartAccountVerificationProcessUsing == "Remove Duplicates")
                {
                    GlobusLogHelper.log.Info("Start Process Remove Duplicates ");
                    GlobusLogHelper.log.Debug("Start Process Remove Duplicates ");
                    lstRemoveDuplicate = lstRemoveDuplicate.Distinct().ToList();//lstRemoveDuplicate

                    if (!string.IsNullOrEmpty(FilePath))
                    {
                        GlobusFileHelper.WriteListtoTextfile(lstRemoveDuplicate, FilePath);
                    }
                }
                GlobusLogHelper.log.Info(" Remove All Duplicates ");
                GlobusLogHelper.log.Debug("Remove All Duplicates ");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
        public void ClearTextFiles()
        {
            try
            {
                lstAccountVerificationThreads.Add(Thread.CurrentThread);
                lstAccountVerificationThreads = lstAccountCreatorThreads.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }          
        }

         //----------------Check Facebook Accounts------------------//     

        public void CheckFacebookAccounts(ref FacebookUser fbUser)
        {
            GlobusLogHelper.log.Info("Start Process Check Facebook Account : " + fbUser.username);
            GlobusLogHelper.log.Debug("Start Process Check Facebook Account : " + fbUser.username);
            try
            {

                if (string.IsNullOrEmpty(exportFilePathAccountVerification))
                {
                    string path = Utils.UploadFolderData(GlobusFileHelper.DesktopPath);
                    exportFilePathAccountVerification = path;
                }
                string EmailId = fbUser.username;
                string password = fbUser.password;
                string proxyAddress = fbUser.proxyip;
                string proxyPort = fbUser.proxyport;
                string proxyUserName = fbUser.proxyusername;
                string proxyPassword = fbUser.proxypassword;

                if (string.IsNullOrEmpty(proxyPort) && !Utils.IsNumeric(proxyPort))
                {
                    proxyPort = "80";
                }
                string strPageSource = string.Empty;
                string fb_dtsg = "";
                string str_lsd = "";
                ChilkatHttpHelpr HttpHelpr = new ChilkatHttpHelpr();
                try
                {
                    if (!string.IsNullOrEmpty(proxyAddress))
                    {
                        strPageSource = HttpHelpr.GetHtmlProxy(FBGlobals.Instance.AccountVerificationCheckFacebookAccountsidentifyPhpUrl, proxyAddress, (proxyPort), proxyUserName, proxyPassword);   //"http://www.facebook.com/help/identify.php?ctx=recover"
                    }
                    else
                    {
                        strPageSource = HttpHelpr.GetHtml(FBGlobals.Instance.AccountVerificationCheckFacebookAccountsidentifyPhpUrl);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
                fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                string emailEncode = EmailId.Replace("@", "%40");
                try
                {
                    str_lsd = strPageSource.Substring(strPageSource.IndexOf("name=\"lsd\" value=\""), (strPageSource.IndexOf("autocomplete", strPageSource.IndexOf("name=\"lsd\" value=\"")) - strPageSource.IndexOf("name=\"lsd\" value=\""))).Replace("name=\"lsd\" value=\"", string.Empty).Trim().Replace("\"", string.Empty);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
                string FbstatusPostData = "lsd=" + str_lsd + "&email=" + EmailId + "&did_submit=Search&__user=0&__a=1&__dyn=7w86A&__req=2&fb_dtsg=" + fb_dtsg + "&phstamp=165816872817579102116";
                string FBstatusResponse = HttpHelpr.PostData(FBGlobals.Instance.AccountVerificationCheckFacebookAccountsAjaxLoginHelpUrl, FbstatusPostData, FBGlobals.Instance.AccountVerificationCheckFacebookAccountsLoginIdentifyUrl);        //"https://www.facebook.com/ajax/login/help/identify.php?ctx=recover"//"https://www.facebook.com/login/identify?ctx=recover"
                string strPageSource1 = HttpHelpr.GetHtml(FBGlobals.Instance.AccountVerificationCheckRecoverInitiateUrl);       

                Accounts.AccountManager objAccountManager = new AccountManager();
                objAccountManager.LoginUsingGlobusHttp(ref fbUser);

                if (!fbUser.isloggedin)
                {
                    if (strPageSource1.Contains("fsm fwn fcg") && !strPageSource1.Contains("Find Your Account"))
                    {
                        if (strPageSource1.Contains("Reset Your Password"))
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\ResetYourPassword.txt");
                            GlobusLogHelper.log.Debug("Email : " + EmailId + "Required Security check");
                            GlobusLogHelper.log.Info("Email : " + EmailId + "Required Security check");
                        }
                        else
                        {
                            if (strPageSource1.Contains("Account Disabled") && strPageSource1.Contains("Your account has been disabled"))
                            {
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\AccountDisabledOnFacebook.txt");
                                GlobusLogHelper.log.Debug("Email : " + EmailId + "Your account has been disabled.");
                                GlobusLogHelper.log.Info("Email : " + EmailId + " Your account has been disabled.");
                            }
                            else
                            {
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\HaveAnAccountOnFacebook.txt");
                                GlobusLogHelper.log.Debug("Email : " + EmailId + " Have An Account on Facebook");
                                GlobusLogHelper.log.Info("Email : " + EmailId + " Have An Account on Facebook");
                            }
                        }
                    }
                    else
                    {
                        if (strPageSource1.Contains("A security check is required to proceed"))
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\SecurityCheckIsRequired.txt");
                            GlobusLogHelper.log.Debug("Email : " + EmailId + "Required Security check");
                            GlobusLogHelper.log.Info("Email : " + EmailId + "Required Security check");
                        }
                        else if (strPageSource1.Contains("Incorrect Email"))
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\IncorrectEmail.txt");
                            GlobusLogHelper.log.Debug("Email : " + EmailId + "Required Security check");
                            GlobusLogHelper.log.Info("Email : " + EmailId + "Required Security check");
                        }

                        else
                        {
                            GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\NotHaveAccountOnFacebook.txt");
                            GlobusLogHelper.log.Debug("Email : " + EmailId + "Not Have Account on Facebook");
                            GlobusLogHelper.log.Info("Email : " + EmailId + "Not Have Account on Facebook");
                        }
                    }
                }
                else
                {
                    if (fbUser.isloggedin)
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\WorkingFacebookAccount.txt");
                        GlobusLogHelper.log.Debug("Email : " + EmailId + " Have An Account on Facebook");
                        GlobusLogHelper.log.Info("Email : " + EmailId + " Have An Account on Facebook");
                    }
                    else
                    {
                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.proxypassword, exportFilePathAccountVerification + "\\HaveAnAccountOnFacebook.txt");
                        GlobusLogHelper.log.Debug("Email : " + EmailId + " Have An Account on Facebook");
                        GlobusLogHelper.log.Info("Email : " + EmailId + " Have An Account on Facebook");
                    }


                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }


        //----------------Remove Mobile From Setting------------------//  

        public void StartRemoveMobileFromSetting(ref FacebookUser fbUser)
        {          
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            string strAccountPageSource2 = string.Empty;
            string str_cell = string.Empty;
            string str_linkid = string.Empty;
            string fb_dtsg = "";
            string __user = "";
            string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbWhatIsMyIpUrl));  //"https://www.whatismyip.com"
          
            __user = GlobusHttpHelper.GetParamValue(strPageSource, "user");
            if (string.IsNullOrEmpty(__user))
            {
                __user = GlobusHttpHelper.ParseJson(strPageSource, "user");
            }

            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg (strPageSource);
            string strAccountPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingMobileUrl));  //"https://www.facebook.com/settings?tab=mobile"
            if (!strAccountPageSource.Contains("Activating allows Facebook Mobile to send text messages to your phone. You can receive notifications for friend requests, messages, Wall posts, and status updates from your friends."))
            {
                try
                {
                    if (strAccountPageSource.Contains("mbs fcg") && strAccountPageSource.Contains("SettingsMobileRemoveLink"))
                    {
                        try
                        {
                            try
                            {
                                string PhonenumberandProfile = strAccountPageSource.Substring(strAccountPageSource.IndexOf("phoneNumber\":"), (strAccountPageSource.IndexOf("}", strAccountPageSource.IndexOf("phoneNumber\":")) - strAccountPageSource.IndexOf("phoneNumber\":"))).Replace("phoneNumber\":", string.Empty).Trim().Replace(")", string.Empty).Replace("(", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Replace(";", string.Empty).Trim();
                                string[] arr_mobandprof = Regex.Split(PhonenumberandProfile, ",");
                                string MobileNumber = arr_mobandprof[0];
                                string ProfileId = arr_mobandprof[1].Replace("profileID:", string.Empty);
                                string PostData = "phone_number=" + MobileNumber + "&profile_id=" + ProfileId + "&__user=" + __user + "&__a=1&__req=4&fb_dtsg=" + fb_dtsg + "&phstamp=16581689811210174109106";
                                string Responsepost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingAjaxMobileSetingDeleteUrl), PostData);    //"https://www.facebook.com/ajax/settings/mobile/delete_phone.php"

                                if (Responsepost.Contains("for (;;);{\"__ar\":1,\"payload\":null}"))
                                {
                                    GlobusLogHelper.log.Debug("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + MobileNumber);
                                    GlobusLogHelper.log.Info("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + MobileNumber);
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\RemoveMobileNumberFromAccount.txt");
                    
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password+ ":" + MobileNumber, exportFilePathAccountVerification + "\\RemoveMobileNumberFromAccount.txt");
                                }
                            }
                            catch(Exception ex)
                            {
                                GlobusLogHelper.log.Error(ex.StackTrace);
                            }
                            
                            string[] cellandlinkid = Regex.Split(strAccountPageSource, "{SettingsMobileRemoveLink.init");
                            string[] str_linkidandstr_cell = Regex.Split(cellandlinkid[1], ",");
                            //cellandlinkid[1].Substring(cellandlinkid[1].IndexOf("name=\"lsd\" value=\""), (cellandlinkid[1].IndexOf("autocomplete", cellandlinkid[1].IndexOf("name=\"lsd\" value=\"")) - cellandlinkid[1].IndexOf("name=\"lsd\" value=\""))).Replace("name=\"lsd\" value=\"", string.Empty).Trim().Replace("\"", string.Empty);
                            str_linkid = str_linkidandstr_cell[0].Replace("(", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                            string[] str_cellss = Regex.Split(str_linkidandstr_cell[1], "}");
                            str_cell = str_cellss[0].Replace(")", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                            //string str_cell = cellandlinkid[1].Substring(cellandlinkid[1].IndexOf("name=\"lsd\" value=\""), (cellandlinkid[1].IndexOf("autocomplete", cellandlinkid[1].IndexOf("name=\"lsd\" value=\"")) - cellandlinkid[1].IndexOf("name=\"lsd\" value=\""))).Replace("name=\"lsd\" value=\"", string.Empty).Trim().Replace("\"", string.Empty);
                            string Str_Post_Data = "hasemail=true&cell=" + str_cell + "&linkid=" + str_linkid + "&__user=" + __user + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=16581651078079859093";
                            string posturl = FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingAjaxMobileDeletePhoneUrl;       // "https://www.facebook.com/ajax/settings/mobile/remove_dialog.php";
                            string Str_Res_Data = HttpHelper.postFormData(new Uri(posturl), Str_Post_Data);

                            string psturl = FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingAjaxMobileActivate;         //  "https://www.facebook.com/ajax/mobile/activate.php";
                            string str_pst_dta = "delete=true&profile_id=" + __user + "&cell=" + str_cell + "&linkid=" + str_linkid + "&__user=" + __user + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816510780798590118";
                            string str_response = HttpHelper.postFormData(new Uri(psturl), str_pst_dta);
                            if (str_response.Contains("The phone number has been removed from your account"))
                            {
                                GlobusLogHelper.log.Debug("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                GlobusLogHelper.log.Info("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password + ":" + str_cell, exportFilePathAccountVerification + "\\path_RemoveMobileNumberFromAccount.txt");
                                GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\path_RemoveMobileNumberFromAccount.txt");
                            }
                            else
                            {
                                string strAccountPageSource1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingTabMobileURL));   //https://www.facebook.com/settings?tab=mobile

                                string lastpostdata = "delete=true&profile_id=100004078073153&cell=" + str_cell + "&linkid=" + str_linkid + "&__user=" + __user + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=165816510780798590118&confirmed=1&ajax_password=" + fbUser.password;
                                string lastresponse = HttpHelper.postFormData(new Uri(psturl), lastpostdata);
                                if (lastresponse.Contains("Not Logged In"))
                                {
                                    GlobusLogHelper.log.Debug("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                    GlobusLogHelper.log.Info("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                    GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\RemoveMobileNumberFromAccount.txt");
                                }
                                else
                                {
                                    strAccountPageSource2 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountVerificationRemoveMobileFromSettingTabMobileURL));    //https://www.facebook.com/settings?tab=mobile
                                    if (strAccountPageSource2.Contains("Activating allows Facebook Mobile to send text messages to your phone. You can receive notifications for friend requests, messages, Wall posts, and status updates from your friends."))
                                    {
                                        GlobusLogHelper.log.Debug("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                        GlobusLogHelper.log.Info("Your Account : " + fbUser.username + ":" + fbUser.password + " Remmove Mobile Number : " + str_cell);
                                        GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\RemoveMobileNumberFromAccount.txt");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : "+ex.StackTrace + "In Remove Number");
                       }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace + "In Remove Number");
                }
            }
            else
            {
              GlobusLogHelper.log.Debug("Your Account : " + fbUser.username + ":" + fbUser.password+ " Already Remmove Mobile Number");
              GlobusLogHelper.log.Info("Your Account : " + fbUser.username + ":" + fbUser.password + " Already Remmove Mobile Number");             
              GlobusFileHelper.AppendStringToTextfileNewLine(fbUser.username + ":" + fbUser.password + ":" + fbUser.proxyip + ":" + fbUser.proxyport + ":" + fbUser.proxyusername + ":" + fbUser.password, exportFilePathAccountVerification + "\\AlreadyRemoveMobileNumberFromAccount.txt");
              
            }
        }


        //EditProfileName


        #region Global Variables For EditProfileName

        public bool isStopEditProfileName = false;

        public List<Thread> lstEditProfileNameThread = new List<Thread>();
        int count_ThreadControllerEditProfileName = 0;
        //public static string FilePath = string.Empty;
        public static  List<string> EditProfileName = new List<string>();
        public static int minDelayEditProfileName = 10;
        public static int maxDelayEditProfileName = 20;

        public static string ProcessUsing = string.Empty;

        public int countEditProfileName = 0;

        readonly object lockrThreadControllerEditProfileName = new object();

        #endregion    


        public int NoOfThreadsEditProfileName
        {
            get;
            set;
        }

        public  static string ChangeLanguageEditProfile
        {
            get;
            set;
        }  


        public void StartEditProfileName()
        {
            try
            {
                lstEditProfileNameThread.Add(Thread.CurrentThread);
                lstEditProfileNameThread = lstEditProfileNameThread.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
           
            try
            {
                int numberOfAccountPatch = 25;

                if (NoOfThreadsEditProfileName > 0)
                {
                    NoOfThreadsEditProfileName = NoOfThreadsEditProfileName;
                }

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (FBGlobals.listAccounts.Count >= 1)
                {

                    list_listAccounts = Utils.Split(FBGlobals.listAccounts, NoOfThreadsEditProfileName);

                    foreach (List<string> listAccounts in list_listAccounts)
                    {

                        foreach (string account in listAccounts)
                        {
                            try
                            {
                                  try
                                    {
                                        string acc = account.Remove(account.IndexOf(':'));
                                        FacebookUser item = null;
                                        FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

                                        if (ProcessUsing == "Edit Profile Name")
                                        {

                                         
                                            string Name = EditProfileName[Utils.GenerateRandom(0, EditProfileName.Count - 1)];
                                            //Run a separate thread for each account                                           

                                            if (item != null)
                                            {
                                                Thread profilerThread = new Thread(StartMultiThreadsEditProfileName);
                                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                                profilerThread.IsBackground = true;
                                                profilerThread.Start(new object[] { item, Name });


                                            }
                                        }
                                        else if (ProcessUsing == "Change Language")
                                        {
                                            Thread profilerThread = new Thread(StartMultiThreadsEditProfileName);
                                            profilerThread.IsBackground = true;
                                            profilerThread.Start(new object[] { item, "" });
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }




        public void StartMultiThreadsEditProfileName(object parameters)
        {
            try
            {
                if (!isStopEditProfileName)
                {
                    try
                    {
                        lstEditProfileNameThread.Add(Thread.CurrentThread);
                        lstEditProfileNameThread = lstEditProfileNameThread.Distinct().ToList();
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
                        string Name =(string)paramsArray.GetValue(1);

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

                            if (ProcessUsing == "Edit Profile Name")
                            {
                                try
                                {
                                    EditName_usingGlobus(ref objFacebookUser, Name);
                                    GlobusLogHelper.log.Info("Process completed With Username : " + objFacebookUser.username +" : and Name " +Name );
                                    GlobusLogHelper.log.Debug("Process completed With Username : " + objFacebookUser.username + " : and Name " + Name);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                // EditName(ref objFacebookUser);
                            }
                            else if (ProcessUsing == "Change Language")
                            {
                                //Change Language 
                                try
                                {

                                    ChangeLanguage(ref objFacebookUser, Name);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

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
        }


        public void EditName(ref FacebookUser fbUser)
        {
            string item = string.Empty;
             GlobusHttpHelper chilkatHttpHelper = fbUser.globusHttpHelper;
         
            try
            {             
               

                string fb_dtsg = string.Empty;
                string UsreId = string.Empty;
                string FirstName = string.Empty;
                string LastName = string.Empty;
                string Password=fbUser.password;
             
                GlobusLogHelper.log.Debug("Starting Change Profile Name");
              
                string[] NameArr = Regex.Split(item, ":");
                FirstName = NameArr[0];
                LastName = NameArr[1];               


                string strPageSource = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountEditProfileNameUrl));      //  "https://www.facebook.com/settings?tab=account&section=name&view"

                try
                {
                    fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                    UsreId = GlobusHttpHelper.GetParamValue(strPageSource, "user");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                }
                try
                {
                    string referer = FBGlobals.Instance.AccountEditProfileNameSettingTabUrl;                                 // "https://www.facebook.com/settings?ref=mb#!/settings?tab=account&section=name&view"
                    string posturl = FBGlobals.Instance.AccountEditProfileNameAjaxSettingAccountUrl;                            // "https://www.facebook.com/ajax/settings/account/name.php"

                    string postdata = "fb_dtsg=" + fb_dtsg + "&first_name=" + FirstName + "&middle_name=&last_name=" + LastName + "&display_name=complete&alternate_name=&show_alternate=1&save_password=" + Password + "&__user=" + UsreId + "&__a=1&phstamp=16581681181137110295170";
                    string response = chilkatHttpHelper.postFormData(new Uri(posturl), postdata, "");   
                    int ss = response.Length;
                    string strPageSource1 = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountEditProfileNameSettingTabUrl));                 //"https://www.facebook.com/settings?ref=mb#!/settings?tab=account&section=name&view"
              
                    if (response.Contains("SettingsPanelManager.closeEditor"))
                    {
                        GlobusLogHelper.log.Debug("FirstName Name : " + FirstName + " LastName :" + LastName + "Changed of UserName : " + fbUser.username);
                        return;
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

        public void EditName_usingGlobus(ref FacebookUser fbUser,string Name)
        {
            string item = string.Empty;
            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
            try
            {
                //string Name = EditProfileName[Utils.GenerateRandom(0, EditProfileName.Count - 1)];           
              

                string fb_dtsg = string.Empty;
                string UsreId = string.Empty;


                string FirstName = string.Empty;
                string LastName = string.Empty;
                string Password = fbUser.password;                
               
                GlobusLogHelper.log.Debug("Starting Change Profile Name");
                GlobusLogHelper.log.Info("Starting Change Profile Name");
                
                string[] NameArr = Regex.Split(Name, ":");
                FirstName = NameArr[0];
                try
                {
                    LastName = NameArr[1];

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                };


                string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountEditProfileNameUrl));
                fb_dtsg =GlobusHttpHelper.Get_fb_dtsg(strPageSource);
                UsreId = GlobusHttpHelper.GetParamValue(strPageSource, "user");
                if (string.IsNullOrEmpty(UsreId))
                {
                    try
                    {
                        UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }

                try
                {
                    string referer = FBGlobals.Instance.AccountEditProfileNameSettingTabUrl;
                    string posturl = FBGlobals.Instance.AccountEditProfileNameAjaxSettingAccountUrl;
                    string postdata = "fb_dtsg=" + fb_dtsg + "&first_name=" + FirstName + "&middle_name=&last_name=" + LastName + "&display_name=complete&alternate_name=&show_alternate=1&save_password=" + Password + "&__user=" + UsreId + "&__a=1&phstamp=16581681181137110295170";
                    postdata = "fb_dtsg="+fb_dtsg+"&primary_first_name="+FirstName+"&primary_middle_name=&primary_last_name="+LastName+"&display_format=complete&alternate_name=&show_alternate=1&save_password="+Password+"&__user="+UsreId+"&__a=1&__dyn=7n8anEAMCBynzpQ9UoHaEWy6zECRAyUgByVbGAEGGGeqrWpUpBxCvV8C4-&__req=c&ttstamp=26581711154584105117545168113&__rev=1274745";
                    string response = HttpHelper.postFormData(new Uri(posturl), postdata);
                    int ss = response.Length;
                    string strPageSource1 = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.AccountEditProfileNameSettingTabUrl));

                    if (response.Contains("SettingsPanelManager.closeEditor") || (response.Contains("closeAndAnimateEdited") && response.Contains("SettingsController")))
                    {
                        GlobusLogHelper.log.Debug("FirstName Name : " + FirstName + " LastName :" + LastName + "Changed of UserName : " + fbUser.username);
                        GlobusLogHelper.log.Info("FirstName Name : " + FirstName + " LastName :" + LastName + "Changed of UserName : " + fbUser.username);
                     //   GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword + ":" + FirstName + ":" + LastName, Globals.path_EditNameList);
                        return;


                    }
                    else if (response.Contains("You can't update your name right now because you've already changed it too many times. ") || response.Contains("You can't update your name right now because you've changed it too recently."))
                    {
                        GlobusLogHelper.log.Debug("You can't update your name right now because you've already changed it too many times. . " + "FirstName Name : " + FirstName + " LastName :" + LastName +"ID  : " +fbUser.username);
                        GlobusLogHelper.log.Info("You can't update your name right now because you've already changed it too many times. . " + "FirstName Name : " + FirstName + " LastName :" + LastName + "ID  : " + fbUser.username);
                        return;
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
            GlobusLogHelper.log.Debug("Process completed  With : " + fbUser.username);
        }


        public void StopEditAccountName()
        {
            try
            {
                isStopEditProfileName = true;

                List<Thread> lstTemp = new List<Thread>();
                lstTemp = lstEditProfileNameThread.Distinct().ToList();

                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                        lstEditProfileNameThread.Remove(item);
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


        
        /// <summary>
        ///   Change Account Profile Language
        /// </summary>
        /// <param name="fbUser"></param>
        /// <param name="Name"></param>
        

        public void ChangeLanguage(ref FacebookUser fbUser, string Name)
        {

            //GlobusLogHelper.log.Debug("Please wait language change process start ..");
            GlobusLogHelper.log.Info("Please wait language change process start ..");

            GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

            string fb_dtsg = string.Empty;
            string UsreId = string.Empty;
            string New_language=string.Empty;
            string Language1 = string.Empty;

            string strPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/settings?tab=account&section=language&view"));
            fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);
            UsreId = GlobusHttpHelper.GetParamValue(strPageSource, "user");
            if (string.IsNullOrEmpty(UsreId))
            {
                UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
            }

            #region MyRegion
            //string language = Utils.getBetween(strPageSource, "new_language", "</td></tr></tbody></table></div>");
            //string[] Arr = System.Text.RegularExpressions.Regex.Split(language, "option value=");
            //if (ChangeLanguageEditProfile.Contains("("))
            //{

            //    string SS = ChangeLanguageEditProfile.Replace("(", "&").Replace(")", "&").Trim();
            //    string[] DD = SS.Split('&');
            //    try
            //    {
            //        ChangeLanguageEditProfile = DD[0];
            //        Language1 = DD[1];
            //    }
            //    catch (Exception ex)
            //    {
            //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //    }

            //}

            //foreach (var Arr_item in Arr)
            //{
            //    try
            //    {
            //        if (Arr_item.Contains(ChangeLanguageEditProfile) && Arr_item.Contains(Language1))
            //        {

            //            New_language = Utils.getBetween(Arr_item, "\"", "\">");
            //            if (New_language.Contains("\""))
            //            {
            //                New_language = Utils.getBetween(Arr_item, "\"", "\"");
            //            }
            //            break;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            //    }
            //} 
            #endregion
               ChangeLanguageEditProfile = ChangeLanguageEditProfile.Trim();
        
                string [] Arr= ChangeLanguageEditProfile.Split(':');
                New_language =Arr[1];
                New_language = New_language.Replace(" ",string.Empty);
                try
                {
                    string AccountSettingAjaxUrl = "https://www.facebook.com/ajax/settings/account/language.php?__user=" + UsreId + "&__a=1&__dyn=7n8a9EAMCBCFUSt2u6aOGUGy6zECQqbx2mbAKGiyGGEVF4YxU&__req=f&__rev=1145305&";

                    string PagSourceAjax = HttpHelper.getHtmlfromUrl(new Uri(AccountSettingAjaxUrl));
                    Thread.Sleep(1 * 2 * 1000);
                    string PostData = "fb_dtsg=" + fb_dtsg + "&new_language=" + New_language + "&__user=" + UsreId + "&__a=1&__dyn=7n8a9EAMCBCFUSt2u6aOGUGy6zECQqbx2mbAKGiyGGEVF4YxU&__req=3&ttstamp=265816699808810571&__rev=1145305";
                    string PostUrl = "https://www.facebook.com/ajax/settings/account/language.php";
                    string response = HttpHelper.postFormData(new Uri(PostUrl), PostData);

                    if (response.Contains("settings?tab=account&edited=language\\\", true"))
                    {
                        GlobusLogHelper.log.Info("Success fully change the Language :  " + ChangeLanguageEditProfile.Replace(" ", string.Empty) + " for User : " + fbUser.username);

                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
        }
    }
}
       

    

    
    

