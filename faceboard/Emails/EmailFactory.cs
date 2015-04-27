

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using OpenPOP.POP3;
using System.Text.RegularExpressions;
using System.Threading;
using faceboardpro;
using Globussoft;
using Chilkat;
using Microsoft.Win32;
using System.Net;
using System.Windows.Forms;
using mshtml;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;



namespace Emails
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3050F669-98B5-11CF-BB82-00AA00BDCE0B")]
    interface IHTMLElementRenderFixed
    {
        void DrawToDC([In]IntPtr hdc);
        void SetDocumentPrinter([In, MarshalAs(UnmanagedType.BStr)]String bstrPrinterName, [In]IntPtr hdc);
    };
   public abstract class EmailFactory
    {
      // public abstract Email createEmail(ref WebBrowser wbrwsr, System.Windows.Forms.Timer timer1, System.Windows.Forms.Timer timer2, System.Windows.Forms.Timer timer3, List<string> FirstName, List<string> LastName);//creates an Email Account
       public abstract Email createEmail(ref WebBrowser wbrwsr,string Captcha, List<string> FirstName, List<string> LastName);//creates an Email Account
       public abstract Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus);//Reads an Email to Email Object
       public abstract Email readEmailusingChilkat();

       public List<string> GetUrlsFromString(string HtmlData)
       {
           List<string> lstUrl = new List<string>();

           try
           {
               string strData = HtmlData;
               string[] arr = Regex.Split(strData, "\n");

               foreach (string strhref in arr)
               {
                   if (!strhref.Contains("<!DOCTYPE"))
                   {
                       if (strhref.Contains("http://www.facebook.com/"))
                       {
                           string tempArr = strhref.Replace("\r", "");
                           lstUrl.Add(tempArr);
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           return lstUrl;
       }

       public void EmailVerificationMultithreaded(string ConfirmationUrl, string gif, string logpic, string email, string password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, ref GlobusHttpHelper HttpHelper)
       {
           try
           {
               int intProxyPort = 80;
               Regex IdCheck = new Regex("^[0-9]*$");

               if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
               {
                   intProxyPort = int.Parse(proxyPort);
               }

               string pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrlProxy(new Uri(ConfirmationUrl), proxyAddress, intProxyPort, proxyUsername, proxyPassword);

               string valueLSD = "name=" + "\"lsd\"";
               string pgSrc_Login = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/login.php"));

               int startIndex = pgSrc_Login.IndexOf(valueLSD) + 18;
               string value = pgSrc_Login.Substring(startIndex, 5);

               GlobusLogHelper.log.Info("Logging in with " + email);
               GlobusLogHelper.log.Debug("Logging in with " + email);

               string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + email.Split('@')[0].Replace("+", "%2B") + "%40" + email.Split('@')[1] + "&pass=" + password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUsername, proxyPassword);



               GlobusLogHelper.log.Info("Response Login : " + HttpHelper.gResponse.ResponseUri.ToString());
               GlobusLogHelper.log.Debug("Response Login : " + HttpHelper.gResponse.ResponseUri.ToString());

               pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrlProxy(new Uri(ConfirmationUrl), proxyAddress, intProxyPort, proxyUsername, proxyPassword);

               try
               {
                   string pgSrc_Gif = HttpHelper.getHtmlfromUrl(new Uri(gif));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               try
               {
                   string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic + "&s=a"));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               try
               {
                   string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }

               //** User Id ***************//////////////////////////////////
               string UsreId = string.Empty;
               if (string.IsNullOrEmpty(UsreId))
               {
                   UsreId = GlobusHttpHelper.ParseJson(ResponseLogin, "user");
               }

               //*** Post Data **************//////////////////////////////////
               string fb_dtsg = FBUtils.GetParamValue(ResponseLogin, "fb_dtsg");
               if (string.IsNullOrEmpty(fb_dtsg))
               {
                   fb_dtsg = GlobusHttpHelper.ParseJson(ResponseLogin, "fb_dtsg");
               }

               string post_form_id = FBUtils.GetParamValue(ResponseLogin, "post_form_id");
               if (string.IsNullOrEmpty(post_form_id))
               {
                   post_form_id = GlobusHttpHelper.ParseJson(ResponseLogin, "post_form_id");
               }

               string pgSrc_email_confirmed = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/?email_confirmed=1"));

               string pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=contact_importer"));


               #region Skipping Code

               ///Code for skipping additional optional Page
               try
               {
                   string phstamp = "165816812085115" + Utils.GenerateRandom(10848130, 10999999);

                   string postDataSkipFirstStep = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=friend_requests&next_step_name=contact_importer&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp;

                   string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), postDataSkipFirstStep);
                   Thread.Sleep(1000);
               }
               catch { }

               pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted/?step=contact_importer"));


               //** FB Account Check email varified or not ***********************************************************************************//
               #region  FB Account Check email varified or not


               string pageSrc2 = string.Empty;
               string pageSrc3 = string.Empty;
               string pageSrc4 = string.Empty;
               string substr1 = string.Empty;


               if (true)
               {
                   try
                   {
                       string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                       string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=contact_importer&next_step_name=classmates_coworkers&previous_step_name=friend_requests&skip=Skip%20this%20step&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                       string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                       pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=classmates_coworkers"));

                       Thread.Sleep(1000);
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
                       string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                       string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=info&hs[school][id][0]=&hs[school][text][0]=&hs[start_year][text][0]=-1&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[start_year][text][0]=-1&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                       string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);


                       ///Post Data Parsing
                       Dictionary<string, string> lstfriend_browser_id = new Dictionary<string, string>();

                       string[] initFriendArray = Regex.Split(postRes, "FriendStatus.initFriend");

                       int tempCount = 0;
                       foreach (string item in initFriendArray)
                       {
                           if (tempCount == 0)
                           {
                               tempCount++;
                               continue;
                           }
                           if (tempCount > 0)
                           {
                               int startIndx = item.IndexOf("(\\") + "(\\".Length + 1;
                               int endIndx = item.IndexOf("\\", startIndx);
                               string paramValue = item.Substring(startIndx, endIndx - startIndx);
                               lstfriend_browser_id.Add("friend_browser_id[" + (tempCount - 1) + "]=", paramValue);
                               tempCount++;
                           }
                       }

                       string partPostData = string.Empty;
                       foreach (var item in lstfriend_browser_id)
                       {
                           partPostData = partPostData + item.Key + item.Value + "&";
                       }

                       phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                       string newPostData1 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=pymk&hs[school][id][0]=&hs[school][text][0]=&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&" + partPostData + "phstamp=" + phstamp + "";//"post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=pymk&friend_browser_id[0]=100002869910855&friend_browser_id[1]=100001857152486&friend_browser_id[2]=575678600&friend_browser_id[3]=100003506761599&friend_browser_id[4]=563402235&friend_browser_id[5]=1268675170&friend_browser_id[6]=1701838026&friend_browser_id[7]=623640106&friend_browser_id[8]=648873235&friend_browser_id[9]=100000151781814&friend_browser_id[10]=657007597&friend_browser_id[11]=1483373867&friend_browser_id[12]=778266161&friend_browser_id[13]=1087830021&friend_browser_id[14]=100001333876108&friend_browser_id[15]=100000534308531&friend_browser_id[16]=1213205246&friend_browser_id[17]=45608778&friend_browser_id[18]=100003080150820&friend_browser_id[19]=892195716&friend_browser_id[20]=100001238774509&friend_browser_id[21]=45602360&friend_browser_id[22]=100000054900916&friend_browser_id[23]=100001308090108&friend_browser_id[24]=100000400766182&friend_browser_id[25]=100001159247338&friend_browser_id[26]=1537081666&friend_browser_id[27]=100000743261988&friend_browser_id[28]=1029373920&friend_browser_id[29]=1077680976&friend_browser_id[30]=100000001266475&friend_browser_id[31]=504487658&friend_browser_id[32]=82600225&friend_browser_id[33]=1023509811&friend_browser_id[34]=100000128061486&friend_browser_id[35]=100001853125513&friend_browser_id[36]=576201748&friend_browser_id[37]=22806492&friend_browser_id[38]=100003232772830&friend_browser_id[39]=1447942875&friend_browser_id[40]=100000131241521&friend_browser_id[41]=100002076794734&friend_browser_id[42]=1397169487&friend_browser_id[43]=1457321074&friend_browser_id[44]=1170969536&friend_browser_id[45]=18903839&friend_browser_id[46]=695329369&friend_browser_id[47]=1265734280&friend_browser_id[48]=698096805&friend_browser_id[49]=777678515&friend_browser_id[50]=529685319&hs[school][id][0]=&hs[school][text][0]=&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=100003556207009&phstamp=1658167541109987992266";
                       string postRes1 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData1);

                       pageSrc2 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=upload_profile_pic"));

                       Thread.Sleep(1000);



                       string image_Get = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/images/wizard/nuxwizard_profile_picture.gif"));

                       try
                       {
                           phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                           string newPostData2 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step=upload_profile_pic&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                           string postRes2 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData2);
                       }
                       catch { }
                       try
                       {
                           phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                           string newPostData3 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step=upload_profile_pic&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&submit=Save%20%26%20Continue&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                           string postRes3 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData3);
                       }
                       catch { }
                   }
                   catch { }

               }
               if (pageSrc2.Contains("Set your profile picture") && pageSrc2.Contains("Skip"))
               {
                   try
                   {
                       string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                       string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                       try
                       {
                           string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                           pageSrc3 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=summary"));
                           pageSrc3 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/home.php?ref=wizard"));
                       }
                       catch { }
                   }
                   catch { }

               }
               #endregion
               if (pageSrc3.Contains("complete the sign-up process"))
               {
               }
               if (pgSrc_contact_importer.Contains("complete the sign-up process"))
               {
               }
               #endregion

               ////**Post Message For User***********************/////////////////////////////////////////////////////

               try
               {

                   string pageSourceHome = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/home.php"));

                   string checkpagesource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/settings?ref=mb"));
                   if (!string.IsNullOrEmpty(checkpagesource))
                   {
                       if (checkpagesource.Contains("General Account Settings"))
                       {
                           if (!checkpagesource.Contains("(Pending)") && !checkpagesource.Contains("Please complete a security check") || !checkpagesource.Contains("For security reasons your account is temporarily locked"))
                           {

                           }
                       }
                       else
                       {

                       }
                   }


                   if (pageSourceHome.Contains("complete the sign-up process"))
                   {
                       Console.WriteLine("Account is not verified for : " + email);
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
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }
       }
    }



   public class HotmailFactory : EmailFactory
   {

           WebBrowser webHotmail = new WebBrowser();
          
           bool checkStatus = true;
           #region  Variables

        string _decaptherStaus = string.Empty;
        string _DecpacherPassword = string.Empty;
        string _DBCPassword = string.Empty;
        string _DBCUser = string.Empty;
        string _DecperUser = string.Empty;
        int wrongpicCounter = 0;
        byte[] imageBytes = new byte[1];
        bool depData = false;
        string _Host = "";
        string _Port = "";
        string _Recapctha = "";
        string fName = "";
        string lName = "";
        string mName = "";
        string petname = "";
        string sPassword = "";
       
        string mm = "";
        string dd = "";
        string yy = "";
        Boolean gender = false;
        string temprandom = "";
        string temprandamZip = "";
        int hotmailIdCount = 1;
        int wbpagenotfound = 0;
        public static bool feelcaptcha = false;
        string DecaptchaHost, DecaptchaLogin, DecaptchaPassword;
        int DecaptchaPort;
        int captchaCounter = 0;
        int acceptcounter = 0;
        int reloadcounter = 0;

        List<string> FirstNameList = new List<string>();
        List<string> LastNameList = new List<string>();


        bool NewPage = false;
        int mainCounter = 0;
        int mode = 0;
        public static string Uniusername = string.Empty;

        #region veriables
        byte[] green;
        Bitmap greenBmp;
     //   QueryManager qm = new QueryManager();
        bool CheckProcess = false;
        int proxycount = 0;
        public static int Keycounter = 0;
        public static int ProxyCounter = 0;
        #endregion

           #endregion

           public override Email createEmail(ref WebBrowser wbrwsr,string Captcha ,List<string> FirstName,List<string> LastName)
           {
                Email objemail = new Email();
                try
                {
                    
                    //   throw new NotImplementedException(); //Amit to code this part
                    this.webHotmail = wbrwsr;

                    FirstNameList = FirstName;
                    LastNameList = LastName;
                    _Recapctha = Captcha;
                    FillValuesInWebBrowserPage();
                   
                    Thread.Sleep(4000);
                   
                    objemail.username = mName + "@hotmail.com";
                    objemail.password = sPassword;

                }
                catch { };
               
                   return objemail;
           }
  
           void FillValuesInWebBrowserPage()
           {

               if (webHotmail.Url.ToString().Contains(FBGlobals.Instance.createHotmailSignupUrl))
               {
                   try
                   {
                       Thread.Sleep(2000);
                     
                       Random random = new Random();
                       gender = Convert.ToBoolean(random.Next(2));
                      int Firstnumber=BaseLib.Utils.GenerateRandom(0,FirstNameList.Count());
                      int Lastnumber = BaseLib.Utils.GenerateRandom(0, LastNameList.Count());
                      
                           fName = FirstNameList[Firstnumber];
                           lName = FirstNameList[Lastnumber];
                     
                       temprandom = Convert.ToString(random.Next(100, 10000));
                       temprandamZip = Convert.ToString(random.Next(90000, 91000));
                       mName = fName + "_" + lName + temprandom;
                       string Emailid = mName;
                       string AltEmail = mName + "@yahoo.com";
                      
                       yy = random.Next(1970, 1993).ToString();
                       mm = random.Next(1, 13).ToString();
                       if (mm == "2")
                           dd = random.Next(1, 29).ToString();
                       else
                           dd = random.Next(1, 31).ToString();

                       webHotmail.Document.GetElementById("iFirstName").SetAttribute("value", fName);//FirstName
                       webHotmail.Document.GetElementById("iLastName").SetAttribute("value", lName);//lastName
                       webHotmail.Document.GetElementById("iBirthYear").SetAttribute("value", yy); //Year
                       webHotmail.Document.GetElementById("iBirthMonth").SetAttribute("value", mm);//Month
                       webHotmail.Document.GetElementById("iBirthDay").SetAttribute("value", dd);//Date
                       webHotmail.Document.GetElementById("iZipCode").SetAttribute("value", temprandamZip);//PostalCode
                       webHotmail.Document.GetElementById("imembernamelive").SetAttribute("value", Emailid);//hotmailid                      
                       webHotmail.Document.GetElementById("idomain").SetAttribute("value", "hotmail.com");//Mail Domain
                       webHotmail.Document.GetElementById("iAltEmail").SetAttribute("value", AltEmail); //Alternet Email

                       int Passstring = random.Next(1000, 99999);

                       sPassword = "QwErTy" + Passstring + "!@#$";  //Password

                       webHotmail.Document.GetElementById("iPwd").SetAttribute("value", sPassword);    //password
                       webHotmail.Document.GetElementById("iRetypePwd").SetAttribute("value", sPassword);//retypepassword
                       IHTMLSelectElement IhtmlCountry = (IHTMLSelectElement)webHotmail.Document.GetElementById("iCountry").DomElement;//country
                       IhtmlCountry.selectedIndex = 1;

                       try
                       {
                           IHTMLElement AncherSwitch = (IHTMLElement)webHotmail.Document.GetElementById("iqsaswitch").DomElement;
                           AncherSwitch.click();
                       }
                       catch (Exception ex)
                       {
                           GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                       }
                       HtmlElementCollection htmlMessageselect = (HtmlElementCollection)webHotmail.Document.GetElementsByTagName("select");
                       foreach (HtmlElement Messageselect in htmlMessageselect)
                       {
                           try
                           {
                               if (Messageselect.OuterHtml.Contains("iGender"))
                               {
                                   string tempGender = Messageselect.Id;
                                   HtmlElement ddlGender = webHotmail.Document.GetElementById(tempGender);
                                   ddlGender.Focus();
                                   webHotmail.Document.GetElementById(tempGender).SetAttribute("value", "m");
                               }

                               if (Messageselect.OuterHtml.Contains("iCountry"))
                               {
                                   string tempCountry = Messageselect.Id;
                                   HtmlElement ddlCountry = webHotmail.Document.GetElementById(tempCountry);
                                   ddlCountry.Focus();
                                   webHotmail.Document.GetElementById(tempCountry).SetAttribute("value", "US");
                               }

                               if (Messageselect.OuterHtml.Contains("iSQ"))
                               {
                                   string tempQuestion = Messageselect.Id;
                                   HtmlElement ddlQuestion = webHotmail.Document.GetElementById(tempQuestion);
                                   ddlQuestion.Focus();
                                   webHotmail.Document.GetElementById(tempQuestion).SetAttribute("value", "Name of first pet");
                               }
                           }
                           catch { };
                       }
                       string[] petList = { "Rockey", "jeemi", "cutty", "herry", "Browny", "Dolly", "Sheru", "hunny", "browo", "sonnu", "Mikki", "Vonda", "meesi", "blacky", "peety", "bondy", "merry", "Skeky", "Ready", "jhony" };
                       int petnumber = random.Next(0, 19);
                       petname = petList[petnumber];
                       try
                       {
                           webHotmail.Document.GetElementById("iSA").SetAttribute("value", petname);
                       }
                       catch { };
                       if (gender == true)        
                       {
                           try
                           {
                               IHTMLElement h1 = (IHTMLElement)webHotmail.Document.GetElementById("iGenderMale").DomElement;
                               h1.click();
                           }
                           catch(Exception ex) 
                           {
                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                           };
                       }
                       else
                       {
                           try
                           {
                               IHTMLElement h2 = (IHTMLElement)webHotmail.Document.GetElementById("iGenderFemale").DomElement;
                               h2.click();
                           }
                           catch(Exception ex)
                           {
                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                           };
                       }


                        string webdocument = webHotmail.Document.Body.OuterHtml;
                   string id = string.Empty;
                   string[] datalist = System.Text.RegularExpressions.Regex.Split(webdocument, "id=");
                   foreach (var item in datalist)
                   {
                       try
                       {
                           if (item.Contains("wlspispSolution") && !item.Contains(".gif"))
                           {
                               id = item.Substring(0, item.IndexOf("class")).Trim();
                               break;
                           }
                       }
                       catch { };
                   }
                   try
                   {
                       id = id.Replace("\"", "").Trim();
                       List<string> lst = new List<string>();

                       webHotmail.Document.GetElementById(id).SetAttribute("value", _Recapctha);

                       Thread.Sleep(2000);
                       HtmlElementCollection h1 = (HtmlElementCollection)webHotmail.Document.GetElementsByTagName("input");
                       foreach (HtmlElement h2 in h1)
                       {
                           try
                           {
                               lst.Add(h2.OuterHtml);

                               if (h2.OuterHtml.Contains("submit") && h2.OuterHtml.Contains("I accept"))
                               {
                                   h2.InvokeMember("click");
                                   break;
                               }
                           }
                           catch { };
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
               else if (webHotmail.Url.AbsoluteUri.Contains("http://login.live.com/uilogout.srf?"))
               {
                  // LoadPage();
               }
               else if (webHotmail.Document.Body.OuterHtml.Contains("Internet Explorer cannot display the webpage"))
               {
                   try
                   {
                     // use proxy Setting
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                   }
               }

              


           }

           public override Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
           {
               //throw new NotImplementedException();//Ajay to code this part

               Email objEmail = new Email();

               string realEmail = string.Empty;

               try
               {
                   objEmail.username = email;
                   objEmail.password = password;

                   realEmail = email;
                   POPClient popClient = new POPClient();

                   #region Hotmail
                   if (email.Contains("@hotmail"))
                   {
                       try
                       {
                           // Code For ajay+1@gmail.com
                           if (email.Contains("+") || email.Contains("%2B"))
                           {
                               try
                               {
                                   string replacePart = email.Substring(email.IndexOf("+"), (email.IndexOf("@", email.IndexOf("+")) - email.IndexOf("+"))).Replace("+", string.Empty);
                                   email = email.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                               }
                               catch(Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : "+ex.StackTrace);
                               }
                           }

                           if (popClient.Connected)
                               popClient.Disconnect();
                           popClient.Connect("pop3.live.com", int.Parse("995"), true);  //live
                           popClient.Authenticate(email, password);
                           int Count = popClient.GetMessageCount();

                           for (int i = Count; i >= 1; i--)
                           {
                               try
                               {
                                   OpenPOP.MIME.Message Message = popClient.GetMessage(i);

                                   string subject = Message.Headers.Subject;

                                   if (Message.Headers.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                                   {
                                       foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                       {
                                           try
                                           {
                                               string staticUrl = string.Empty;
                                               string email_open_log_picUrl = string.Empty;

                                               string strBody = Message.MessageBody[0];
                                               string[] arr = Regex.Split(strBody, "src=");
                                               foreach (string item in arr)
                                               {
                                                   if (!item.Contains("<!DOCTYPE"))
                                                   {
                                                       if (item.Contains("static"))
                                                       {
                                                           string[] arrStatic = item.Split('"');
                                                           staticUrl = arrStatic[1];
                                                       }
                                                       if (item.Contains("email_open_log_pic"))
                                                       {
                                                           try
                                                           {
                                                               string[] arrlog_pic = item.Split('"');
                                                               email_open_log_picUrl = arrlog_pic[1];
                                                               email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                           }
                                                           catch(Exception ex)
                                                           {
                                                               GlobusLogHelper.log.Error("Error :"+ex.StackTrace);
                                                           }
                                                           break;
                                                       }
                                                   }
                                               }

                                               string href1 = href.Replace("&amp;report=1", "");
                                               href1 = href.Replace("amp;", "");


                                               objEmail.body = strBody;
                                               objEmail.subject = Message.Headers.Subject;
                                               List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                               foreach (System.Net.Mail.MailAddress item in lstTo)
                                               {
                                                   try
                                                   {
                                                       objEmail.to = objEmail.to + item.ToString();
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                   }
                                               }

                                               objEmail.from = Message.Headers.From.ToString();

                                               objEmail.mailType = MailType.hotmail;

                                               // if (href1.Contains(Uri.EscapeDataString(realEmail)))
                                               {
                                                   EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);
                                               }

                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                       }
                                   }
                                   else if (Message.Headers.Subject.Contains("Just one more step to get started on Facebook"))
                                   {
                                       foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                       {
                                           try
                                           {
                                               string staticUrl = string.Empty;
                                               string email_open_log_picUrl = string.Empty;

                                               string strBody = Message.MessageBody[0];
                                               string[] arr = Regex.Split(strBody, "src=");
                                               foreach (string item in arr)
                                               {
                                                   try
                                                   {
                                                       if (!item.Contains("<!DOCTYPE"))
                                                       {
                                                           if (item.Contains("static"))
                                                           {
                                                               string[] arrStatic = item.Split('"');
                                                               staticUrl = arrStatic[1];
                                                           }
                                                           if (item.Contains("email_open_log_pic"))
                                                           {
                                                               string[] arrlog_pic = item.Split('"');
                                                               email_open_log_picUrl = arrlog_pic[1];
                                                               email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                               break;
                                                           }
                                                       }
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                   }
                                               }

                                               string href1 = href.Replace("&amp;report=1", "");
                                               href1 = href.Replace("amp;", "");


                                               objEmail.body = strBody;
                                               objEmail.subject = Message.Headers.Subject;
                                               List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                               foreach (System.Net.Mail.MailAddress item in lstTo)
                                               {
                                                   try
                                                   {
                                                       objEmail.to = objEmail.to + item.ToString();
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                   }
                                               }

                                               objEmail.from = Message.Headers.From.ToString();

                                               objEmail.mailType = MailType.hotmail;

                                               // if (href1.Contains(Uri.EscapeDataString(realEmail)))
                                               {
                                                   EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);
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
                   #endregion
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }

               return objEmail;
           }

           public override Email readEmailusingChilkat()
       {
           throw new NotImplementedException();
       }
    
   }
   public class YahooFactory : EmailFactory
   {
       public override Email createEmail(ref WebBrowser wbrwsr, string Catcha, List<string> FirstName, List<string> LastName)
       {
           throw new NotImplementedException(); //Ajay to code this part
       }
       public override Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
       {
           //throw new NotImplementedException();//Ajay to code this part

           Email objEmail = new Email();

           string yahooEmail = string.Empty;
           string yahooPassword = string.Empty;
           string Username = string.Empty;
           string Password = string.Empty;
           try
           {
               objEmail.username = email;
               objEmail.password = password;

               string realEmail = email;

               yahooEmail = email; ;
               yahooPassword = password;

               Chilkat.Imap iMap = new Imap();

               // Code For ajay+1@gmail.com
               if (yahooEmail.Contains("+") || yahooEmail.Contains("%2B"))
               {
                   try
                   {
                       string replacePart = yahooEmail.Substring(yahooEmail.IndexOf("+"), (yahooEmail.IndexOf("@", yahooEmail.IndexOf("+")) - yahooEmail.IndexOf("+"))).Replace("+", string.Empty);
                       yahooEmail = yahooEmail.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                   }
               }

               Username = yahooEmail;
               Password = yahooPassword;
               //Username = "Karlawtt201@yahoo.com";
               //Password = "rga77qViNIV";
               iMap.UnlockComponent("THEBACIMAPMAILQ_OtWKOHoF1R0Q");

               //iMap.
               //iMap.HttpProxyHostname = "127.0.0.1";
               //iMap.HttpProxyPort = 8888;

               iMap.Connect("imap.n.mail.yahoo.com");
               iMap.Login(yahooEmail, yahooPassword);
               iMap.SelectMailbox("Inbox");

               // Get a message set containing all the message IDs
               // in the selected mailbox.
               Chilkat.MessageSet msgSet;
               //msgSet = iMap.Search("FROM \"facebookmail.com\"", true);
               msgSet = iMap.GetAllUids();

               // Fetch all the mail into a bundle object.
               Chilkat.Email cEemail = new Chilkat.Email();
               //bundle = iMap.FetchBundle(msgSet);
               string strEmail = string.Empty;
               List<string> lstData = new List<string>();
               if (msgSet != null)
               {
                   for (int i = msgSet.Count; i > 0; i--)
                   {
                       cEemail = iMap.FetchSingle(msgSet.GetId(i), true);
                       strEmail = cEemail.Subject;
                       string emailHtml = cEemail.GetHtmlBody();
                       lstData.Add(strEmail);
                       if (cEemail.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                       {
                           foreach (string href in GetUrlsFromString(cEemail.Body))
                           {
                               try
                               {
                                   string staticUrl = string.Empty;
                                   string email_open_log_picUrl = string.Empty;

                                   string strBody = cEemail.Body;
                                   string[] arr = Regex.Split(strBody, "src=");
                                   // string[] arr = Regex.Split(strBody, "href=");
                                   foreach (string item in arr)
                                   {
                                       if (!item.Contains("<!DOCTYPE"))
                                       {
                                           if (item.Contains("static"))
                                           {
                                               string[] arrStatic = item.Split('"');
                                               staticUrl = arrStatic[1];
                                           }
                                           if (item.Contains("email_open_log_pic"))
                                           {
                                               string[] arrlog_pic = item.Split('"');
                                               email_open_log_picUrl = arrlog_pic[1];
                                               email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                               break;
                                           }
                                       }
                                   }

                                   string href1 = href.Replace("&amp;report=1", "");
                                   href1 = href.Replace("amp;", "");

                                   objEmail.body = strBody;
                                   objEmail.subject = cEemail.Subject;
                                   objEmail.to = cEemail.ReplyTo;



                                   objEmail.from = cEemail.From.ToString();

                                   objEmail.mailType = MailType.yahoomail;

                                   EmailVerificationMultithreadedForAccountCreater(href1, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPassword);
                                   //LoginVerfy(href1, staticUrl, email_open_log_picUrl);
                                   break;
                               }
                               catch (Exception ex)
                               {
                                   Console.WriteLine(ex.StackTrace);
                               }
                           }
                           //return;
                       }
                       else if (cEemail.Subject.Contains("Just one more step to get started on Facebook"))
                       {
                           foreach (string href in GetUrlsFromString(cEemail.Body))
                           {
                               try
                               {
                                   string staticUrl = string.Empty;
                                   string email_open_log_picUrl = string.Empty;
                                   string verifyhref = string.Empty;
                                   string strBody = cEemail.Body;
                                   string[] arr = Regex.Split(strBody, "src=");
                                   string[] arr1 = Regex.Split(strBody, "href=");
                                   foreach (string item in arr)
                                   {
                                       if (!item.Contains("<!DOCTYPE"))
                                       {
                                           if (item.Contains("static"))
                                           {
                                               string[] arrStatic = item.Split('"');
                                               staticUrl = arrStatic[1];
                                           }
                                           if (item.Contains("email_open_log_pic"))
                                           {
                                               string[] arrlog_pic = item.Split('"');
                                               email_open_log_picUrl = arrlog_pic[1];
                                               email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                               break;
                                           }
                                       }
                                   }

                                   foreach (string item1 in arr1)
                                   {
                                       if (item1.Contains("confirmemail.php"))
                                       {
                                           string[] itemurl = Regex.Split(item1, "\"");
                                           verifyhref = itemurl[1].Replace("\"", string.Empty);
                                       }
                                   }

                                   string href1 = verifyhref.Replace("&amp;report=1", "");
                                   string href11 = href1.Replace("amp;", "");

                                   objEmail.body = strBody;
                                   objEmail.subject = cEemail.Subject;
                                   objEmail.to = cEemail.ReplyTo;



                                   objEmail.from = cEemail.From.ToString();

                                   objEmail.mailType = MailType.yahoomail;

                                   //string href1 = href.Replace("&amp;report=1", "");
                                   //href1 = href.Replace("amp;", "");
                                   if (href.Contains("confirmemail.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                   {
                                       EmailVerificationMultithreadedForAccountCreater(href11, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPassword);
                                       //LoginVerfy(href1, staticUrl, email_open_log_picUrl);
                                       break;
                                   }
                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }
                           }
                           //return;
                       }

                       //*****************************************************bysanjeev**********************
                       else if (cEemail.Subject.Contains("Facebook Email Verification"))
                       {
                           foreach (string href in GetUrlsFromString(cEemail.Body))
                           {
                               try
                               {
                                   string staticUrl = string.Empty;
                                   string email_open_log_picUrl = string.Empty;
                                   string verifyhref = string.Empty;

                                   string strBody = cEemail.Body;
                                   string[] arr = Regex.Split(strBody, "src=");
                                   string[] arr1 = Regex.Split(strBody, "href=");
                                   // string[] arr = Regex.Split(strBody, "src=");
                                   foreach (string item in arr)
                                   {
                                       if (!item.Contains("<!DOCTYPE"))
                                       {
                                           if (item.Contains("static"))
                                           {
                                               string[] arrStatic = item.Split('"');
                                               staticUrl = arrStatic[1];
                                           }
                                           if (item.Contains("email_open_log_pic"))
                                           {
                                               string[] arrlog_pic = item.Split('"');
                                               email_open_log_picUrl = arrlog_pic[1];
                                               email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                               break;
                                           }
                                       }
                                   }
                                   foreach (string item1 in arr1)
                                   {
                                       if (item1.Contains("confirmcontact.php"))
                                       {
                                           string[] itemurl = Regex.Split(item1, "\"");
                                           verifyhref = itemurl[1].Replace("\"", string.Empty);
                                       }
                                   }


                                   //string href1 = href.Replace("&amp;report=1", "");
                                   //href1 = href.Replace("&amp", "");

                                   objEmail.body = strBody;
                                   objEmail.subject = cEemail.Subject;
                                   objEmail.to = cEemail.ReplyTo;



                                   objEmail.from = cEemail.From.ToString();

                                   objEmail.mailType = MailType.yahoomail;

                                   if (href.Contains("confirmcontact.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                   {
                                       EmailVerificationMultithreadedForAccountCreater(verifyhref, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPassword);
                                       break;
                                   }//LoginVerfy(href1, staticUrl, email_open_log_picUrl);

                               }
                               catch (Exception ex)
                               {
                                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                               }
                           }
                           //return;
                       }

                       //****************************************************************************************

                   }
               }
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           return objEmail;
       }

       public void EmailVerificationMultithreadedForAccountCreater(string ConfirmationUrl, string gif, string logpic, string email, string password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
       {
           try
           {
               GlobusHttpHelper HttpHelper = new GlobusHttpHelper();

               int intProxyPort = 80;
               Regex IdCheck = new Regex("^[0-9]*$");

               if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
               {
                   intProxyPort = int.Parse(proxyPort);
               }

               string pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrlProxy(new Uri(ConfirmationUrl), proxyAddress, intProxyPort, proxyUsername, proxyPassword);

               string valueLSD = "name=" + "\"lsd\"";
               string pgSrc_Login = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/login.php"));

               int startIndex = pgSrc_Login.IndexOf(valueLSD) + 18;
               string value = pgSrc_Login.Substring(startIndex, 5);

               GlobusLogHelper.log.Info("Logging in with " + email);
               GlobusLogHelper.log.Debug("Logging in with " + email);

               string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + email.Split('@')[0].Replace("+", "%2B") + "%40" + email.Split('@')[1] + "&pass=" + password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUsername, proxyPassword);

               pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrl(new Uri(ConfirmationUrl));

               try
               {
                   string pgSrc_Gif = HttpHelper.getHtmlfromUrl(new Uri(gif));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               try
               {
                   string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic + "&s=a"));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               try
               {
                   string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic));
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }

               //** User Id ***************//////////////////////////////////
               string UsreId = string.Empty;
               if (string.IsNullOrEmpty(UsreId))
               {
                   UsreId = GlobusHttpHelper.ParseJson(ResponseLogin, "user");
               }

               //*** Post Data **************//////////////////////////////////
               string fb_dtsg = GlobusHttpHelper.GetParamValue(ResponseLogin, "fb_dtsg");
               if (string.IsNullOrEmpty(fb_dtsg))
               {
                   fb_dtsg = GlobusHttpHelper.ParseJson(ResponseLogin, "fb_dtsg");
               }

               string post_form_id = GlobusHttpHelper.GetParamValue(ResponseLogin, "post_form_id");
               if (string.IsNullOrEmpty(post_form_id))
               {
                   post_form_id = GlobusHttpHelper.ParseJson(ResponseLogin, "post_form_id");
               }

               string pgSrc_email_confirmed = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/?email_confirmed=1"));

               string pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=contact_importer"));


               #region Skipping Code

               ///Code for skipping additional optional Page
               try
               {
                   string phstamp = "165816812085115" + Utils.GenerateRandom(10848130, 10999999);

                   string postDataSkipFirstStep = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=friend_requests&next_step_name=contact_importer&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp;

                   string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), postDataSkipFirstStep);
                   Thread.Sleep(1000);
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }

               pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted/?step=contact_importer"));


               //** FB Account Check email varified or not ***********************************************************************************//
               #region  FB Account Check email varified or not

               //string pageSrc1 = string.Empty;
               string pageSrc2 = string.Empty;
               string pageSrc3 = string.Empty;
               string pageSrc4 = string.Empty;
               string substr1 = string.Empty;

               
               if (true)
               {
                   string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                   string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=contact_importer&next_step_name=classmates_coworkers&previous_step_name=friend_requests&skip=Skip%20this%20step&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                   string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                   pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=classmates_coworkers"));

                   Thread.Sleep(1000);

                  
               }
               
               if (true)
               {
                   string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                   string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=info&hs[school][id][0]=&hs[school][text][0]=&hs[start_year][text][0]=-1&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[start_year][text][0]=-1&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                   string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                   ///Post Data Parsing
                   Dictionary<string, string> lstfriend_browser_id = new Dictionary<string, string>();

                   string[] initFriendArray = Regex.Split(postRes, "FriendStatus.initFriend");

                   int tempCount = 0;
                   foreach (string item in initFriendArray)
                   {
                       if (tempCount == 0)
                       {
                           tempCount++;
                           continue;
                       }
                       if (tempCount > 0)
                       {
                           int startIndx = item.IndexOf("(\\") + "(\\".Length + 1;
                           int endIndx = item.IndexOf("\\", startIndx);
                           string paramValue = item.Substring(startIndx, endIndx - startIndx);
                           lstfriend_browser_id.Add("friend_browser_id[" + (tempCount - 1) + "]=", paramValue);
                           tempCount++;
                       }
                   }

                   string partPostData = string.Empty;
                   foreach (var item in lstfriend_browser_id)
                   {
                       partPostData = partPostData + item.Key + item.Value + "&";
                   }

                   phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                   string newPostData1 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=pymk&hs[school][id][0]=&hs[school][text][0]=&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&" + partPostData + "phstamp=" + phstamp + "";//"post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=classmates_coworkers&next_step_name=upload_profile_pic&previous_step_name=contact_importer&current_pane=pymk&friend_browser_id[0]=100002869910855&friend_browser_id[1]=100001857152486&friend_browser_id[2]=575678600&friend_browser_id[3]=100003506761599&friend_browser_id[4]=563402235&friend_browser_id[5]=1268675170&friend_browser_id[6]=1701838026&friend_browser_id[7]=623640106&friend_browser_id[8]=648873235&friend_browser_id[9]=100000151781814&friend_browser_id[10]=657007597&friend_browser_id[11]=1483373867&friend_browser_id[12]=778266161&friend_browser_id[13]=1087830021&friend_browser_id[14]=100001333876108&friend_browser_id[15]=100000534308531&friend_browser_id[16]=1213205246&friend_browser_id[17]=45608778&friend_browser_id[18]=100003080150820&friend_browser_id[19]=892195716&friend_browser_id[20]=100001238774509&friend_browser_id[21]=45602360&friend_browser_id[22]=100000054900916&friend_browser_id[23]=100001308090108&friend_browser_id[24]=100000400766182&friend_browser_id[25]=100001159247338&friend_browser_id[26]=1537081666&friend_browser_id[27]=100000743261988&friend_browser_id[28]=1029373920&friend_browser_id[29]=1077680976&friend_browser_id[30]=100000001266475&friend_browser_id[31]=504487658&friend_browser_id[32]=82600225&friend_browser_id[33]=1023509811&friend_browser_id[34]=100000128061486&friend_browser_id[35]=100001853125513&friend_browser_id[36]=576201748&friend_browser_id[37]=22806492&friend_browser_id[38]=100003232772830&friend_browser_id[39]=1447942875&friend_browser_id[40]=100000131241521&friend_browser_id[41]=100002076794734&friend_browser_id[42]=1397169487&friend_browser_id[43]=1457321074&friend_browser_id[44]=1170969536&friend_browser_id[45]=18903839&friend_browser_id[46]=695329369&friend_browser_id[47]=1265734280&friend_browser_id[48]=698096805&friend_browser_id[49]=777678515&friend_browser_id[50]=529685319&hs[school][id][0]=&hs[school][text][0]=&hs[year][text][0]=-1&hs[entry_id][0]=&college[entry_id][0]=&college[school][id][0]=0&college[school][text][0]=&college[year][text][0]=-1&college[type][0]=college&work[employer][id][0]=0&work[employer][text][0]=&work[entry_id][0]=&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=100003556207009&phstamp=1658167541109987992266";
                   string postRes1 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData1);

                   pageSrc2 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=upload_profile_pic"));

                   Thread.Sleep(1000);

                  
                   string image_Get = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/images/wizard/nuxwizard_profile_picture.gif"));

                   try
                   {
                       phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                       string newPostData2 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step=upload_profile_pic&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                       string postRes2 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData2);
                   }
                   catch { }
                   try
                   {
                       phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                       string newPostData3 = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step=upload_profile_pic&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&submit=Save%20%26%20Continue&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                       string postRes3 = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData3);
                   }
                   catch { }

               }
               if (pageSrc2.Contains("Set your profile picture") && pageSrc2.Contains("Skip"))
               {
                   string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);
                   string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=upload_profile_pic&previous_step_name=classmates_coworkers&skip=Skip&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                   try
                   {
                       string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                       pageSrc3 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=summary"));
                       pageSrc3 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/home.php?ref=wizard"));
                   }
                   catch { }

               }
               #endregion
               if (pageSrc3.Contains("complete the sign-up process"))
               {
               }
               if (pgSrc_contact_importer.Contains("complete the sign-up process"))
               {
               }
               #endregion


               ////**Post Message For User***********************/////////////////////////////////////////////////////

               try
               {


                   string pageSourceHome = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/home.php"));
                   string checkpagesource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/settings?ref=mb"));
                   if (!string.IsNullOrEmpty(checkpagesource))
                   {
                       if (checkpagesource.Contains("General Account Settings"))
                       {

                           if (!checkpagesource.Contains("(Pending)") && !checkpagesource.Contains("Please complete a security check"))
                           {
                               GlobusLogHelper.log.Info("Email Verification finished for : " + email);
                               GlobusLogHelper.log.Debug("Email Verification finished for : " + email);
                              
                           }
                       }
                       else
                       {
                           GlobusLogHelper.log.Info("Email Verification finished for : " + email);
                           GlobusLogHelper.log.Debug("Email Verification finished for : " + email);
                       }
                   }
                   if (pageSourceHome.Contains("complete the sign-up process"))
                   {
                       GlobusLogHelper.log.Info("Email Verification finished for : " + email);
                       GlobusLogHelper.log.Debug("Email Verification finished for : " + email);
                   }
                   else
                   {
                       GlobusLogHelper.log.Info("Email Verification finished for : " + email);
                       GlobusLogHelper.log.Debug("Email Verification finished for : " + email);
                   }

               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }

               GlobusLogHelper.log.Info("Email Verification finished for : " + email);
               GlobusLogHelper.log.Debug("Email Verification finished for : " + email);
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }
       }

       public override Email readEmailusingChilkat()
       {
           throw new NotImplementedException();
       }
   }
   public class GmailFactory : EmailFactory
   {
       public override Email createEmail(ref WebBrowser wbrwsr, string Catcha, List<string> FirstName, List<string> LastName)
       {
           throw new NotImplementedException(); //Ajay to code this part
       }
       public override Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
       {
           //throw new NotImplementedException();//Ajay to code this part


            Email objEmail = new Email();

           

           string realEmail = string.Empty;
           try
           {
               objEmail.username = email;
               objEmail.password = password;

               realEmail = email;
               POPClient popClient = new POPClient();


               #region Gmail
               if (email.Contains("@gmail"))
               {
                   // Code For ajay+1@gmail.com
                   if (email.Contains("+") || email.Contains("%2B"))
                   {
                       try
                       {
                           string replacePart = email.Substring(email.IndexOf("+"), (email.IndexOf("@", email.IndexOf("+")) - email.IndexOf("+"))).Replace("+", string.Empty);
                           email = email.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                       }
                       catch (Exception ex)
                       {
                           GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                       }
                   }

                   if (popClient.Connected)
                       popClient.Disconnect();
                   popClient.Connect("pop.gmail.com", int.Parse("995"), true);
                   popClient.Authenticate(email, password);
                   int Count = popClient.GetMessageCount();

                   for (int i = Count; i >= 1; i--)
                   {
                       try
                       {
                           OpenPOP.MIME.Message Message = popClient.GetMessage(i);

                           string subject = Message.Headers.Subject;

                           if (Message.Headers.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                           {
                               foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                               {
                                   try
                                   {
                                       string staticUrl = string.Empty;
                                       string email_open_log_picUrl = string.Empty;

                                       string strBody = Message.MessageBody[0];
                                       string[] arr = Regex.Split(strBody, "src=");
                                       foreach (string item in arr)
                                       {
                                           try
                                           {
                                               if (!item.Contains("<!DOCTYPE"))
                                               {
                                                   if (item.Contains("static"))
                                                   {
                                                       string[] arrStatic = item.Split('"');
                                                       staticUrl = arrStatic[1];
                                                   }
                                                   if (item.Contains("email_open_log_pic"))
                                                   {
                                                       string[] arrlog_pic = item.Split('"');
                                                       email_open_log_picUrl = arrlog_pic[1];
                                                       email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                       break;
                                                   }
                                               }
                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                       }

                                       string href1 = href.Replace("&amp;report=1", "");
                                       href1 = href.Replace("amp;", "");

                                       objEmail.body = strBody;
                                       objEmail.subject = Message.Headers.Subject;
                                       List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                       foreach (System.Net.Mail.MailAddress item in lstTo)
                                       {
                                           try
                                           {
                                               objEmail.to = objEmail.to + item.ToString();
                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                       }

                                       objEmail.from = Message.Headers.From.ToString();

                                       objEmail.mailType = MailType.gmail;

                                       EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

                                   }
                                   catch (Exception ex)
                                   {
                                       Console.WriteLine(ex.StackTrace);
                                   }
                               }

                           }
                           else if (Message.Headers.Subject.Contains("Just one more step to get started on Facebook"))
                           {
                               foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                               {
                                   try
                                   {
                                       string staticUrl = string.Empty;
                                       string email_open_log_picUrl = string.Empty;

                                       string strBody = Message.MessageBody[0];
                                       string[] arr = Regex.Split(strBody, "src=");
                                       foreach (string item in arr)
                                       {
                                           try
                                           {
                                               if (!item.Contains("<!DOCTYPE"))
                                               {
                                                   if (item.Contains("static"))
                                                   {
                                                       string[] arrStatic = item.Split('"');
                                                       staticUrl = arrStatic[1];
                                                   }
                                                   if (item.Contains("email_open_log_pic"))
                                                   {
                                                       string[] arrlog_pic = item.Split('"');
                                                       email_open_log_picUrl = arrlog_pic[1];
                                                       email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                       break;
                                                   }
                                               }
                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                       }

                                       string href1 = href.Replace("&amp;report=1", "");
                                       href1 = href.Replace("amp;", "");

                                       objEmail.body = strBody;
                                       objEmail.subject = Message.Headers.Subject;
                                       List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                       foreach (System.Net.Mail.MailAddress item in lstTo)
                                       {
                                           try
                                           {
                                               objEmail.to = objEmail.to + item.ToString();
                                           }
                                           catch (Exception ex)
                                           {
                                               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                           }
                                       }

                                       objEmail.from = Message.Headers.From.ToString();

                                       objEmail.mailType = MailType.gmail;

                                       EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

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
               #endregion
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           return objEmail;
       }

       public override Email readEmailusingChilkat()
       {
           throw new NotImplementedException();
       }
   }
   public class LiveFactory : EmailFactory
   {
       public override Email createEmail(ref WebBrowser wbrwsr, string Catcha, List<string> FirstName, List<string> LastName)
       {
           throw new NotImplementedException();
       }

       public override Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
       {
           //throw new NotImplementedException();

           Email objEmail = new Email();

           string realEmail = string.Empty;

           try
           {
               objEmail.username = email;
               objEmail.password = password;

               realEmail = email;
               POPClient popClient = new POPClient();

               #region live
                if (email.Contains("@live"))
                {
                    // Code For ajay+1@gmail.com
                    if (email.Contains("+") || email.Contains("%2B"))
                    {
                        try
                        {
                            string replacePart = email.Substring(email.IndexOf("+"), (email.IndexOf("@", email.IndexOf("+")) - email.IndexOf("+"))).Replace("+", string.Empty);
                            email = email.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (popClient.Connected)
                        popClient.Disconnect();
                    popClient.Connect("pop3.live.com", int.Parse("995"), true);
                    popClient.Authenticate(email, password);
                    int Count = popClient.GetMessageCount();

                    for (int i = Count; i >= 1; i--)
                    {
                        try
                        {
                            OpenPOP.MIME.Message Message = popClient.GetMessage(i);

                            string subject = Message.Headers.Subject;

                            if (Message.Headers.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                            {
                                foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                {
                                    try
                                    {
                                        string staticUrl = string.Empty;
                                        string email_open_log_picUrl = string.Empty;

                                        string strBody = Message.MessageBody[0];
                                        string[] arr = Regex.Split(strBody, "src=");
                                        foreach (string item in arr)
                                        {
                                            if (!item.Contains("<!DOCTYPE"))
                                            {
                                                if (item.Contains("static"))
                                                {
                                                    string[] arrStatic = item.Split('"');
                                                    staticUrl = arrStatic[1];
                                                }
                                                if (item.Contains("email_open_log_pic"))
                                                {
                                                    string[] arrlog_pic = item.Split('"');
                                                    email_open_log_picUrl = arrlog_pic[1];
                                                    email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                    break;
                                                }
                                            }
                                        }

                                        string href1 = href.Replace("&amp;report=1", "");
                                        href1 = href.Replace("amp;", "");

                                        objEmail.body = strBody;
                                        objEmail.subject = Message.Headers.Subject;
                                        List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                        foreach (System.Net.Mail.MailAddress item in lstTo)
                                        {
                                            try
                                            {
                                                objEmail.to = objEmail.to + item.ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        objEmail.from = Message.Headers.From.ToString();

                                        objEmail.mailType = MailType.live;

                                        EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                            }
                            else if (Message.Headers.Subject.Contains("Just one more step to get started on Facebook"))
                            {
                                foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                {
                                    try
                                    {
                                        string staticUrl = string.Empty;
                                        string email_open_log_picUrl = string.Empty;

                                        string strBody = Message.MessageBody[0];
                                        string[] arr = Regex.Split(strBody, "src=");
                                        foreach (string item in arr)
                                        {
                                            if (!item.Contains("<!DOCTYPE"))
                                            {
                                                if (item.Contains("static"))
                                                {
                                                    string[] arrStatic = item.Split('"');
                                                    staticUrl = arrStatic[1];
                                                }
                                                if (item.Contains("email_open_log_pic"))
                                                {
                                                    string[] arrlog_pic = item.Split('"');
                                                    email_open_log_picUrl = arrlog_pic[1];
                                                    email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                    break;
                                                }
                                            }
                                        }

                                        string href1 = href.Replace("&amp;report=1", "");
                                        href1 = href.Replace("amp;", "");

                                        objEmail.body = strBody;
                                        objEmail.subject = Message.Headers.Subject;
                                        List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                        foreach (System.Net.Mail.MailAddress item in lstTo)
                                        {
                                            try
                                            {
                                                objEmail.to = objEmail.to + item.ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        objEmail.from = Message.Headers.From.ToString();

                                        objEmail.mailType = MailType.live;

                                        EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

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
                #endregion

           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           return objEmail;
       }

       public override Email readEmailusingChilkat()
       {
           throw new NotImplementedException();
       }
   }
   public class AolFactory : EmailFactory
   {
       public override Email createEmail(ref WebBrowser wbrwsr, string Catcha, List<string> FirstName, List<string> LastName)
       {
           throw new NotImplementedException();
       }

       public override Email readEmail(string email, string password, string proxyAddress, string proxyPort, string proxyUser, string proxyPassword, string DOB, ref GlobusHttpHelper HttpHelper, string registrationstatus)
       {
           //throw new NotImplementedException();

           Email objEmail = new Email();

           string realEmail = string.Empty;

           try
           {
               objEmail.username = email;
               objEmail.password = password;

               realEmail = email;
               POPClient popClient = new POPClient();

               #region Aol
                if (email.Contains("@aol"))
                {
                    // Code For ajay+1@gmail.com
                    if (email.Contains("+") || email.Contains("%2B"))
                    {
                        try
                        {
                            string replacePart = email.Substring(email.IndexOf("+"), (email.IndexOf("@", email.IndexOf("+")) - email.IndexOf("+"))).Replace("+", string.Empty);
                            email = email.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (popClient.Connected)
                        popClient.Disconnect();
                    popClient.Connect("imap.aol.com", int.Parse("995"), true);
                    popClient.Authenticate(email, password);
                    int Count = popClient.GetMessageCount();

                    for (int i = Count; i >= 1; i--)
                    {
                        try
                        {
                            OpenPOP.MIME.Message Message = popClient.GetMessage(i);

                            string subject = Message.Headers.Subject;

                            if (Message.Headers.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                            {
                                foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                {
                                    try
                                    {
                                        string staticUrl = string.Empty;
                                        string email_open_log_picUrl = string.Empty;

                                        string strBody = Message.MessageBody[0];
                                        string[] arr = Regex.Split(strBody, "src=");
                                        foreach (string item in arr)
                                        {
                                            if (!item.Contains("<!DOCTYPE"))
                                            {
                                                if (item.Contains("static"))
                                                {
                                                    string[] arrStatic = item.Split('"');
                                                    staticUrl = arrStatic[1];
                                                }
                                                if (item.Contains("email_open_log_pic"))
                                                {
                                                    string[] arrlog_pic = item.Split('"');
                                                    email_open_log_picUrl = arrlog_pic[1];
                                                    email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                    break;
                                                }
                                            }
                                        }

                                        string href1 = href.Replace("&amp;report=1", "");
                                        href1 = href.Replace("amp;", "");

                                        objEmail.body = strBody;
                                        objEmail.subject = Message.Headers.Subject;
                                        List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                        foreach (System.Net.Mail.MailAddress item in lstTo)
                                        {
                                            try
                                            {
                                                objEmail.to = objEmail.to + item.ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        objEmail.from = Message.Headers.From.ToString();

                                        objEmail.mailType = MailType.aol;

                                        EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }

                            }
                            else if (Message.Headers.Subject.Contains("Just one more step to get started on Facebook"))
                            {
                                foreach (string href in GetUrlsFromString(Message.MessageBody[0]))
                                {
                                    try
                                    {
                                        string staticUrl = string.Empty;
                                        string email_open_log_picUrl = string.Empty;

                                        string strBody = Message.MessageBody[0];
                                        string[] arr = Regex.Split(strBody, "src=");
                                        foreach (string item in arr)
                                        {
                                            if (!item.Contains("<!DOCTYPE"))
                                            {
                                                if (item.Contains("static"))
                                                {
                                                    string[] arrStatic = item.Split('"');
                                                    staticUrl = arrStatic[1];
                                                }
                                                if (item.Contains("email_open_log_pic"))
                                                {
                                                    string[] arrlog_pic = item.Split('"');
                                                    email_open_log_picUrl = arrlog_pic[1];
                                                    email_open_log_picUrl = email_open_log_picUrl.Replace("amp;", "");
                                                    break;
                                                }
                                            }
                                        }

                                        string href1 = href.Replace("&amp;report=1", "");
                                        href1 = href.Replace("amp;", "");

                                        objEmail.body = strBody;
                                        objEmail.subject = Message.Headers.Subject;
                                        List<System.Net.Mail.MailAddress> lstTo = Message.Headers.To;

                                        foreach (System.Net.Mail.MailAddress item in lstTo)
                                        {
                                            try
                                            {
                                                objEmail.to = objEmail.to + item.ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                            }
                                        }

                                        objEmail.from = Message.Headers.From.ToString();

                                        objEmail.mailType = MailType.aol;

                                        EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, password, proxyAddress, proxyPort, proxyUser, proxyPassword, ref HttpHelper);

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
                #endregion
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }

           return objEmail;
       }

       public override Email readEmailusingChilkat()
       {
           throw new NotImplementedException();
       }
   }
}
