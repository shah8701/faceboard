using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Chilkat;
using System.Text.RegularExpressions;
using System.Threading;
using faceboardpro;
using Globussoft;
using Chilkat;
using BaseLib;
using System.Data;

namespace faceboardpro
{
    public class ChilkatIMAP
    {
        public string Username = string.Empty;
        public string Password = string.Empty;

        public string proxyAddress = string.Empty;
        public string proxyPort = string.Empty;
        public string proxyUser = string.Empty;
        public string proxyPass = string.Empty;

        Chilkat.Imap iMap = new Imap();

        public static Events LogEvents = new Events();

        public string Connect(string yahooEmail, string yahooPassword)
        {
            string decodedActivationLink = string.Empty;

            iMap.UnlockComponent("THEBACIMAPMAILQ_OtWKOHoF1R0Q");
            iMap.Connect("imap.n.mail.yahoo.com");
            //iMap.Login("Karlawtt201@yahoo.com", "rga77qViNIV");
            iMap.Login(yahooEmail, yahooPassword);
            iMap.SelectMailbox("Inbox");

            // Get a message set containing all the message IDs
            // in the selected mailbox.
            Chilkat.MessageSet msgSet;
            msgSet = iMap.Search("ALL", true);

            // Fetch all the mail into a bundle object.
            Chilkat.Email email = new Chilkat.Email();
            //bundle = iMap.FetchBundle(msgSet);

            for (int i = msgSet.Count - 1; i > 0; i--)
            {
                try
                {
                    email = iMap.FetchSingle(msgSet.GetId(i), true);
                    if (email.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                    {
                        int startIndex = email.Body.IndexOf("http://www.facebook.com/confirmemail.php?e=");
                        int endIndex = email.Body.IndexOf(">", startIndex) - 1;
                        string activationLink = email.Body.Substring(startIndex, endIndex - startIndex).Replace("\r\n", "");
                        activationLink = activationLink.Replace("3D", "").Replace("hotmai=l", "hotmail").Replace("%40", "@");

                        decodedActivationLink = Uri.UnescapeDataString(activationLink);

                       
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }
            return decodedActivationLink;


        }


        /// <summary>
        /// Gets into Yahoo Email and fetches Activation Link and Sends Http Request to it
        /// Calls LoginVerfy() which sends Http Request
        /// Also sends sends Request to gif URL, and 2 more URLs
        /// </summary>
        /// <param name="yahooEmail"></param>
        /// <param name="yahooPassword"></param>
        public void GetFBMails(string yahooEmail, string yahooPassword)
        {
            try
            {
                string realEmail = yahooEmail;

                if (yahooEmail.Contains("+") || yahooEmail.Contains("%2B"))
                {
                    try
                    {
                        string replacePart = yahooEmail.Substring(yahooEmail.IndexOf("+"), (yahooEmail.IndexOf("@", yahooEmail.IndexOf("+")) - yahooEmail.IndexOf("+"))).Replace("+", string.Empty);
                        yahooEmail = yahooEmail.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                    }
                    catch
                    {
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
                Chilkat.Email email = new Chilkat.Email();
                //bundle = iMap.FetchBundle(msgSet);
                string strEmail = string.Empty;
                List<string> lstData = new List<string>();
                if (msgSet != null)
                {
                    for (int i = msgSet.Count; i > 0; i--)
                    {
                        email = iMap.FetchSingle(msgSet.GetId(i), true);
                        strEmail = email.Subject;
                        string emailHtml = email.GetHtmlBody();
                        lstData.Add(strEmail);
                        if (email.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;

                                    string strBody = email.Body;
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

                                    EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
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
                        else if (email.Subject.Contains("Just one more step to get started on Facebook"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;
                                    string strBody = email.Body;
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
                                    //string href1 = href.Replace("&amp;report=1", "");
                                    //href1 = href.Replace("amp;", "");
                                    if (href.Contains("confirmemail.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreaded(href11, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        //LoginVerfy(href1, staticUrl, email_open_log_picUrl);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                            //return;
                        }

                        //*****************************************************bysanjeev**********************
                        else if (email.Subject.Contains("Facebook Email Verification"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;

                                    string strBody = email.Body;
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
                                    if (href.Contains("confirmcontact.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreaded(verifyhref, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        break;
                                    }//LoginVerfy(href1, staticUrl, email_open_log_picUrl);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
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
        }

        public void GetFBMailsForAccountCreater(string yahooEmail, string yahooPassword)
        {
            try
            {
                string realEmail = yahooEmail;
                // Code For ajay+1@gmail.com
                if (yahooEmail.Contains("+") || yahooEmail.Contains("%2B"))
                {
                    try
                    {
                        string replacePart = yahooEmail.Substring(yahooEmail.IndexOf("+"), (yahooEmail.IndexOf("@", yahooEmail.IndexOf("+")) - yahooEmail.IndexOf("+"))).Replace("+", string.Empty);
                        yahooEmail = yahooEmail.Replace("+", string.Empty).Replace("%2B", string.Empty).Replace(replacePart, string.Empty);
                    }
                    catch
                    {
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
                Chilkat.Email email = new Chilkat.Email();
                //bundle = iMap.FetchBundle(msgSet);
                string strEmail = string.Empty;
                List<string> lstData = new List<string>();
                if (msgSet != null)
                {
                    for (int i = msgSet.Count; i > 0; i--)
                    {
                        email = iMap.FetchSingle(msgSet.GetId(i), true);
                        strEmail = email.Subject;
                        string emailHtml = email.GetHtmlBody();
                        lstData.Add(strEmail);
                        if (email.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;

                                    string strBody = email.Body;
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

                                    EmailVerificationMultithreadedForAccountCreater(href1, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
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
                        else if (email.Subject.Contains("Just one more step to get started on Facebook"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;
                                    string strBody = email.Body;
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
                                    //string href1 = href.Replace("&amp;report=1", "");
                                    //href1 = href.Replace("amp;", "");
                                    if (href.Contains("confirmemail.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreadedForAccountCreater(href11, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        //LoginVerfy(href1, staticUrl, email_open_log_picUrl);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                            //return;
                        }

                        //*****************************************************bysanjeev**********************
                        else if (email.Subject.Contains("Facebook Email Verification"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;

                                    string strBody = email.Body;
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
                                    if (href.Contains("confirmcontact.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreadedForAccountCreater(verifyhref, staticUrl, email_open_log_picUrl, realEmail, yahooPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        break;
                                    }//LoginVerfy(href1, staticUrl, email_open_log_picUrl);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
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
                GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
            }
        }


        public void GetFBMailsForAol(string AolEmail, string AolPassword)
        {
            try
            {
                Username = AolEmail;
                Password = AolPassword;
                //Username = "Karlawtt201@yahoo.com";
                //Password = "rga77qViNIV";
                iMap.UnlockComponent("THEBACIMAPMAILQ_OtWKOHoF1R0Q");

                //iMap.
                //iMap.HttpProxyHostname = "127.0.0.1";
                //iMap.HttpProxyPort = 8888;

                iMap.Connect("imap.aol.com");
                iMap.Login(AolEmail, AolPassword);
                iMap.SelectMailbox("Inbox");

                // Get a message set containing all the message IDs
                // in the selected mailbox.
                Chilkat.MessageSet msgSet;
                //msgSet = iMap.Search("FROM \"facebookmail.com\"", true);
                msgSet = iMap.GetAllUids();

                // Fetch all the mail into a bundle object.
                Chilkat.Email email = new Chilkat.Email();
                //bundle = iMap.FetchBundle(msgSet);
                string strEmail = string.Empty;
                List<string> lstData = new List<string>();
                if (msgSet != null)
                {
                    for (int i = msgSet.Count; i > 0; i--)
                    {
                        email = iMap.FetchSingle(msgSet.GetId(i), true);
                        strEmail = email.Subject;
                        string emailHtml = email.GetHtmlBody();
                        lstData.Add(strEmail);
                        if (email.Subject.Contains("Action Required: Confirm Your Facebook Account"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;

                                    string strBody = email.Body;
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

                                    EmailVerificationMultithreaded(href1, staticUrl, email_open_log_picUrl, AolEmail, AolPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
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
                        else if (email.Subject.Contains("Just one more step to get started on Facebook"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;
                                    string strBody = email.Body;
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
                                    //string href1 = href.Replace("&amp;report=1", "");
                                    //href1 = href.Replace("amp;", "");
                                    if (href.Contains("confirmemail.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreaded(href11, staticUrl, email_open_log_picUrl, AolEmail, AolPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        //LoginVerfy(href1, staticUrl, email_open_log_picUrl);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                            //return;
                        }

                        //*****************************************************bysanjeev**********************
                        else if (email.Subject.Contains("Facebook Email Verification"))
                        {
                            foreach (string href in GetUrlsFromString(email.Body))
                            {
                                try
                                {
                                    string staticUrl = string.Empty;
                                    string email_open_log_picUrl = string.Empty;
                                    string verifyhref = string.Empty;

                                    string strBody = email.Body;
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
                                    if (href.Contains("confirmcontact.php") && email_open_log_picUrl.Contains("email_open_log_pic.php"))
                                    {
                                        EmailVerificationMultithreaded(verifyhref, staticUrl, email_open_log_picUrl, AolEmail, AolPassword, proxyAddress, proxyPort, proxyUser, proxyPass);
                                        break;
                                    }//LoginVerfy(href1, staticUrl, email_open_log_picUrl);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.StackTrace);
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
                GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
            }
        }

        

        /// <summary>
        /// Makes Http Request to Confirmation URL from Mail, also requests other JS, CSS URLs
        /// </summary>
        /// <param name="ConfirmationUrl"></param>
        /// <param name="gif"></param>
        /// <param name="logpic"></param>
        public void EmailVerificationMultithreaded(string ConfirmationUrl, string gif, string logpic, string email, string password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword)
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

                //Log("Logging in with " + Username);

                string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + email.Split('@')[0].Replace("+", "%2B") + "%40" + email.Split('@')[1] + "&pass=" + password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUsername, proxyPassword);

                pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrl(new Uri(ConfirmationUrl));

                try
                {
                    string pgSrc_Gif = HttpHelper.getHtmlfromUrl(new Uri(gif));
                }
                catch { }
                try
                {
                    string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic + "&s=a"));
                }
                catch { }
                try
                {
                    string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic));
                }
                catch { }

                //** User Id ***************//////////////////////////////////
                string UsreId = string.Empty;
                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(ResponseLogin, "user");
                }

                //*** Post Data **************//////////////////////////////////
                string fb_dtsg = GlobusHttpHelper.GetParamValue(ResponseLogin, "fb_dtsg");//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(ResponseLogin, "fb_dtsg");
                }

                string post_form_id = GlobusHttpHelper.GetParamValue(ResponseLogin, "post_form_id");//pageSourceHome.Substring(pageSourceHome.IndexOf("post_form_id"), 200);
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

                //string pageSrc1 = string.Empty;
                string pageSrc2 = string.Empty;
                string pageSrc3 = string.Empty;
                string pageSrc4 = string.Empty;
                string substr1 = string.Empty;

                //if (pgSrc_contact_importer.Contains("Are your friends already on Facebook?") && pgSrc_contact_importer.Contains("Skip this step"))
                if (true)
                {
                    string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                    string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=contact_importer&next_step_name=classmates_coworkers&previous_step_name=friend_requests&skip=Skip%20this%20step&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                    string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                    pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=classmates_coworkers"));

                    Thread.Sleep(1000);

                    //pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted/?step=classmates_coworkers"));
                }
                //if ((pgSrc_contact_importer.Contains("Fill out your Profile Info") || pgSrc_contact_importer.Contains("Fill out your Profile info")) && pgSrc_contact_importer.Contains("Skip"))
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

                    //pageSrc2 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=upload_profile_pic"));

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
                                // Log("Email Verification finished for : " + email);
                                // return;
                            }
                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            Log(ds,"Account is not verified for : " + email);
                        }
                    }
                    if (pageSourceHome.Contains("complete the sign-up process"))
                    {
                        Console.WriteLine("Account is not verified for : " + email);

                        DataSet ds = new DataSet();
                        Log(ds,"Account is not verified for : " + email);
                    }
                    else
                    {
                        // Log("Email Verification finished for : " + email);
                    }


                }
                catch { }

                //AddToListBox("Registration Succeeded for: " + email);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
            }
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

                //Log("Logging in with " + Username);

                string ResponseLogin = HttpHelper.postFormDataProxy(new Uri("https://www.facebook.com/login.php?login_attempt=1"), "charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "&locale=en_US&email=" + email.Split('@')[0].Replace("+", "%2B") + "%40" + email.Split('@')[1] + "&pass=" + password + "&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=" + value + "", proxyAddress, intProxyPort, proxyUsername, proxyPassword);

                pgSrc_ConfirmationUrl = HttpHelper.getHtmlfromUrl(new Uri(ConfirmationUrl));

                try
                {
                    string pgSrc_Gif = HttpHelper.getHtmlfromUrl(new Uri(gif));
                }
                catch { }
                try
                {
                    string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic + "&s=a"));
                }
                catch { }
                try
                {
                    string pgSrc_Logpic = HttpHelper.getHtmlfromUrl(new Uri(logpic));
                }
                catch { }

                //** User Id ***************//////////////////////////////////
                string UsreId = string.Empty;
                if (string.IsNullOrEmpty(UsreId))
                {
                    UsreId = GlobusHttpHelper.ParseJson(ResponseLogin, "user");
                }

                //*** Post Data **************//////////////////////////////////
                string fb_dtsg = GlobusHttpHelper.GetParamValue(ResponseLogin, "fb_dtsg");//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
                if (string.IsNullOrEmpty(fb_dtsg))
                {
                    fb_dtsg = GlobusHttpHelper.ParseJson(ResponseLogin, "fb_dtsg");
                }

                string post_form_id = GlobusHttpHelper.GetParamValue(ResponseLogin, "post_form_id");//pageSourceHome.Substring(pageSourceHome.IndexOf("post_form_id"), 200);
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

                //string pageSrc1 = string.Empty;
                string pageSrc2 = string.Empty;
                string pageSrc3 = string.Empty;
                string pageSrc4 = string.Empty;
                string substr1 = string.Empty;

                //if (pgSrc_contact_importer.Contains("Are your friends already on Facebook?") && pgSrc_contact_importer.Contains("Skip this step"))
                if (true)
                {
                    string phstamp = "16581677684757" + Utils.GenerateRandom(5104244, 9999954);

                    string newPostData = "post_form_id=" + post_form_id + "&fb_dtsg=" + fb_dtsg + "&step_name=contact_importer&next_step_name=classmates_coworkers&previous_step_name=friend_requests&skip=Skip%20this%20step&lsd&post_form_id_source=AsyncRequest&__user=" + UsreId + "&phstamp=" + phstamp + "";
                    string postRes = HttpHelper.postFormData(new Uri("http://www.facebook.com/ajax/growth/nux/wizard/steps.php?__a=1"), newPostData);

                    pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=classmates_coworkers"));

                    Thread.Sleep(1000);

                    //pgSrc_contact_importer = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted/?step=classmates_coworkers"));
                }
                //if ((pgSrc_contact_importer.Contains("Fill out your Profile Info") || pgSrc_contact_importer.Contains("Fill out your Profile info")) && pgSrc_contact_importer.Contains("Skip"))
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

                    //pageSrc2 = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/gettingstarted.php?step=upload_profile_pic"));

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
                                DataSet ds = new DataSet();
                                Log(ds,"Email Verification finished for : " + email);
                                // return;
                            }
                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            Log(ds,"Account is not verified for : " + email);
                        }
                    }
                    if (pageSourceHome.Contains("complete the sign-up process"))
                    {
                        Console.WriteLine("Account is not verified for : " + email);

                        DataSet ds = new DataSet();
                        Log(ds,"Account is not verified for : " + email);
                    }
                    else
                    {
                        // Log("Email Verification finished for : " + email);
                    }

                }
                catch { }

                //AddToListBox("Registration Succeeded for: " + email);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
            }
        }

        public List<string> GetUrlsFromString(string HtmlData)
        {
            
            List<string> lstUrl = new List<string>();

            try
            {
                var regex = new Regex(@"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled);
                var ModifiedString = HtmlData.Replace("\"", " ").Replace("<", " ").Replace(">", " ");
                foreach (Match url in regex.Matches(ModifiedString))
                {
                    lstUrl.Add(url.Value);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
            }
           
            return lstUrl;
        }

        private void Log(DataSet ds,params string[] parameters)
        {
            try
            {
                EventsArgs eventArgs = new EventsArgs(ds,parameters);
                LogEvents.RaiseParamsEvent(eventArgs);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

    }
}


