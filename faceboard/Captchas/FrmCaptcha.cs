using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHDocVw;
using mshtml;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Globussoft;
using faceboardpro;
using BaseLib;
using Emails;


namespace Captchas
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3050F669-98B5-11CF-BB82-00AA00BDCE0B")]
    interface IHTMLElementRenderFixed
    {
        void DrawToDC([In]IntPtr hdc);
        void SetDocumentPrinter([In, MarshalAs(UnmanagedType.BStr)]String bstrPrinterName, [In]IntPtr hdc);
    };


    public partial class FrmCaptcha : Form
    {
        public Events pageManagerEvent = null;

        # region Variables

        byte[] imageBytes = new byte[1];
        List<string> lstFname = new List<string>();
        List<string> lstLname = new List<string>();
        string Fname = string.Empty;
        string Lname = string.Empty;
        string Username = string.Empty;
        string Password = string.Empty;

        string CurrentCaptchaSetting=string.Empty;
        string CurrentCaptchaUserName=string.Empty;
        string CurrentCaptchaPassword=string.Empty;

        public static bool CheckDbcUserNamePassword = false;
        public static string EmailAccountDataPath = string.Empty;

        HotmailerStatus hotmailerStatus;

        #endregion

        public FrmCaptcha()
        {
           
            InitializeComponent();
            pageManagerEvent = new Events();
        }



        public FrmCaptcha(List<string> lstFirstName, List<string> lstLastName,string currentCaptchaSetting,string currentCaptchaUserName,string currentCaptchaPassword)
        {
            CurrentCaptchaSetting = currentCaptchaSetting;
            CurrentCaptchaUserName = currentCaptchaUserName;
            CurrentCaptchaPassword = currentCaptchaPassword;

            lstFname = lstFirstName;
            lstLname = lstLastName;
            InitializeComponent();
            pageManagerEvent = new Events();
        }

        private void RaiseEvent(DataSet ds, params string[] parameters)
        {
            try
            {
                EventsArgs eArgs = new EventsArgs(ds, parameters);
                pageManagerEvent.RaiseParamsEvent(eArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

     
        enum HotmailerStatus
        {
            SignUpPage, SubmitButtonClicked, checkforAccountCreation, AccountCreationPreStage, AccountCreationSuccess, LoggedOut, ProxyCatch, SuccessfullyLogout,None,FillCaptcha
        }

        public void loadName()
        {
            try
            {
                int Fnumber = BaseLib.Utils.GenerateRandom(0, lstFname.Count);
                Fname = lstFname[Fnumber];

                int Lnumber = BaseLib.Utils.GenerateRandom(0, lstLname.Count);
                Lname = lstFname[Fnumber];
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);

            }

        }

        public void LoadMainPageAndCreateIE()
        {
            try
            {
                webHotmail.Navigate(FBGlobals.Instance.createHotmailSignUpInboxUrl);

                hotmailerStatus = HotmailerStatus.SignUpPage;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }
       
        public void FindCaptcha()
        {
            try
            {
                CaptchaFactory objCaptchafactory = new DBCService();
                bool processFindcapthastart = true;
                while (processFindcapthastart)
                {


                    try
                    {

                        if (webHotmail.Document != null)
                        {
                            try
                            {
                                imageBytes = null;
                                processFindcapthastart = false;
                                string id = string.Empty;
                                string webdocument = webHotmail.Document.Body.OuterHtml;

                                string[] datalist = System.Text.RegularExpressions.Regex.Split(webdocument, "id=");
                                foreach (var item in datalist)
                                {

                                    if (item.Contains("wlspispHIPBimg") && !item.Contains(".gif"))
                                    {
                                        id = item.Substring(0, item.IndexOf("src=")).Trim();
                                        break;
                                    }
                                }
                                IHTMLImgElement img = null;
                                try
                                {
                                    id = id.Replace("\"", "");
                                    img = (IHTMLImgElement)webHotmail.Document.All[id].DomElement;
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                IHTMLElementRenderFixed irend = (IHTMLElementRenderFixed)img;
                                Bitmap bmp = new Bitmap(img.width, img.height);
                                Graphics g = Graphics.FromImage(bmp);
                                IntPtr hdc = g.GetHdc();
                                irend.DrawToDC(hdc);
                                g.ReleaseHdc(hdc);
                                g.Dispose();
                                //  ImageCaptcha.Image = bmp;
                                bmp.Save(@"C:\Facedominator\logs\HOTMAILdecaptcha" + Fname + ".jpg");



                                string _ImageUrl = Path.Combine(@"C:\Facedominator\logs\HOTMAILdecaptcha" + Fname + ".jpg");
                                System.Net.WebClient webClient = new System.Net.WebClient();
                                imageBytes = webClient.DownloadData(_ImageUrl);

                              

                                if (CheckDbcUserNamePassword)
                                {

                                    Captcha Recaptcha = objCaptchafactory.resolveRecaptcha(imageBytes, CurrentCaptchaUserName, CurrentCaptchaPassword);

                                    CaptchaPic.LoadAsync(_ImageUrl);

                                    FillCaptcha();
                                    
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

        private void webHotmail_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {

                if (webHotmail.ReadyState == WebBrowserReadyState.Complete)
                {

                    switch (hotmailerStatus)
                    {
                        case HotmailerStatus.SignUpPage: FindCaptcha();
                            break;

                        case HotmailerStatus.checkforAccountCreation: CheckStatus();
                            break;

                        case HotmailerStatus.FillCaptcha: FillCaptcha();
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
          }
        /// <summary>
        /// This method is used for check Page status for new Acouunts created or not, after Page Document Completed
        /// </summary>
        public void CheckStatus()
        {

            try
            {
                bool Captchastatus = true;
                Email objemail = new Email();
                if (webHotmail.ReadyState == WebBrowserReadyState.Complete)
                {


                    if (webHotmail.Document.Body.OuterHtml.Contains("Sign out") || webHotmail.Document.Body.OuterHtml.Contains("sign out"))
                    {
                        try
                        {
                            objemail.username = Username;
                            objemail.password = Password;
                            DataSet ds = new DataSet();

                            Captchastatus = false;

                            GlobusLogHelper.log.Info("EmaillAccount :   Username : "+Username+ " Password : "+ Password +" has Created");
                            RaiseEvent(ds, new string[] { "Model : EmailRepository", "Function : InsertCreatedEmailinEmailDatatbase", Username, Password });
                            hotmailerStatus = HotmailerStatus.LoggedOut;

                            //EmailAccountDataPath



                            string CSVHeader = "Username"+","+"Password";
                            string CSV_Content = Username+","+Password;
                            try
                            {
                                Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, EmailAccountDataPath);
                                GlobusLogHelper.log.Debug("Data Saved In CSV ." + CSV_Content);
                                GlobusLogHelper.log.Info("Data Saved In CSV ." + CSV_Content);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }




                            Thread.Sleep(2000);
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (webHotmail.Document.Body.OuterHtml.Contains("<BODY onload=javascript:rd();></BODY>") && webHotmail.Document.Body.OuterHtml.Count() < 200)
                    {

                        try
                        {
                            objemail.username = Username;
                            objemail.password = Password;
                            DataSet ds = new DataSet();
                            Captchastatus = false;
                            GlobusLogHelper.log.Info("EmaillAccount :   Username : " + Username + " Password : " + Password + " has Created");
                            RaiseEvent(ds, new string[] { "Model : EmailRepository", "Function : InsertCreatedEmailinEmailDatatbase", Username, Password });
                            hotmailerStatus = HotmailerStatus.LoggedOut;

                            Thread.Sleep(2000);
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }

                    if (webHotmail.Document.Body.OuterHtml.Contains("<BODY></BODY>") && webHotmail.Document.Body.OuterHtml.Count() < 100)
                    {
                        try
                        {
                            objemail.username = Username;
                            objemail.password = Password;

                            DataSet ds = new DataSet();
                            Captchastatus = false;
                            GlobusLogHelper.log.Info("EmaillAccount :   Username : " + Username + " Password : " + Password + " has Created");
                            RaiseEvent(ds, new string[] { "Model : EmailRepository", "Function : InsertCreatedEmailinEmailDatatbase", Username, Password });
                            hotmailerStatus = HotmailerStatus.LoggedOut;

                            Thread.Sleep(2000);
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (webHotmail.Document.Body.OuterHtml.Contains("Sign Up Error 450"))
                    {

                        try
                        {
                            GlobusLogHelper.log.Error("Error : " + "Error 450 Track IP address");
                            hotmailerStatus = HotmailerStatus.LoggedOut;

                            Thread.Sleep(2000);
                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (webHotmail.Document.Body.OuterHtml.Contains("Sign out and sign in with a different account"))
                    {
                        try
                        {
                            HtmlElementCollection h1 = (HtmlElementCollection)webHotmail.Document.GetElementsByTagName("A");
                            foreach (HtmlElement h2 in h1)
                            {
                                if (h2.OuterHtml.Contains("Sign out and sign in with a different account") || h2.OuterHtml.Contains("Sign out"))
                                {
                                    h2.InvokeMember("click");
                                    string outerhtml = h2.OuterHtml;
                                    string outerhref = outerhtml.Substring(outerhtml.IndexOf("href="), outerhtml.IndexOf("\">") - outerhtml.IndexOf("href=")).Replace("href=", "").Replace("\"", "").Trim();
                                    webHotmail.Navigate(outerhref);

                                    break;
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    if (webHotmail.Document.Body.OuterHtml.Contains("The characters didn't match the picture. Please try again.") && Captchastatus == true)
                    {
                        try
                        {
                            FindCaptcha();
                            hotmailerStatus = HotmailerStatus.None;


                            Thread.Sleep(2000);
                            this.Hide();
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

      
        private void btnCreateCaptcha_Click(object sender, EventArgs e)
        {
            FillCaptcha();
        }

        public void FillCaptcha()
        {
            try
            {
                string captcha = string.Empty;
                if (!string.IsNullOrEmpty(CurrentCaptchaUserName) && !string.IsNullOrEmpty(CurrentCaptchaPassword) && CheckDbcUserNamePassword)
                {
                    captcha = ResolveCaptchaByCaptchaService(imageBytes, CurrentCaptchaSetting, CurrentCaptchaUserName, CurrentCaptchaPassword);

                }
                else
                {
                   captcha= txtCaptcha.Text;
                }
               

                if (string.IsNullOrEmpty(captcha))
                {
                    MessageBox.Show("Please enter the Captcha Value Which you see on Captcha Image");
                    return;
                }


                Emails.EmailFactory hotemail = new Emails.HotmailFactory();

                Emails.Email email = hotemail.createEmail(ref webHotmail, captcha, lstFname, lstLname);

                Username = email.username;
                Password = email.password;
                hotmailerStatus = HotmailerStatus.checkforAccountCreation;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void FrmCaptcha_Load(object sender, EventArgs e)
        {
            try
            {
                loadName();
                LoadMainPageAndCreateIE();

             
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }


        public string ResolveCaptchaByCaptchaService(byte[] imageBytes, string CurrentCaptchaSetting, string CurrentCaptchaUserName,string CurrentCaptchaPassword)
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

        private void FrmCaptcha_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.CheckStopAccountCreation = false;
        }     
    }
}
