using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Captchas.Anigate
{
    class ClsAnigates
    {

        public string GetCaptcha(string  AccountKey,string IagePath)
        {
            string check = String.Empty;
            do
            {
                AntiCaptcha anticap = new AntiCaptcha();
                check = anticap.GetText(AccountKey, IagePath, 10000);
                if (check == "ERROR_NO_SLOT_AVAILABLE")
                {
                    //add your action here
                }
                lock (this) { //log_program(DateTime.Now + " = got answer: " + check);
                }
                Thread.Sleep(2000);
            }
            while ((check == "ERROR_NO_SLOT_AVAILABLE"));

            return check;
 
        }
    }

    public class PostData5
    {
        private string s_method = String.Empty;

        public string Method { get { return this.s_method; } }
        private string s_action = String.Empty;

        public string Action { get { return this.s_action; } }
        public string Param { get { return this.s_param; } }

        private string s_param = String.Empty;
        public PostData5(string s_PostString)
        {
            if (s_PostString.IndexOf("=") != -1)
            {
                this.s_method = s_PostString.Substring(0, s_PostString.IndexOf("="));
                this.s_action = s_PostString.Substring(s_PostString.IndexOf("=") + 1);
                if (this.s_action.IndexOf("!") != -1)
                {
                    this.s_action = s_action.Substring(0, this.s_action.IndexOf("!")); this.s_param = s_PostString.Substring(s_PostString.IndexOf("!") + 1);
                }

            }

        }
        public static string MultiFormData(string Key, string Value, string Boundary)
        {
            lock (typeof(PostData5))
            {
                string output = "--" + Boundary + "\r\n"; output += "Content-Disposition: form-data; name=\"" + Key + "\"\r\n\r\n";
                output += Value + "\r\n";
                return output;
            }
        }
        public static string MultiFormDataFile(string Key, string Value, string FileName, string FileType, string Boundary)
        {
            lock (typeof(PostData5))
            {
                string output = "--" + Boundary + "\r\n"; output += "Content-Disposition: form-data; name=\"" + Key + "\"; filename=\"" + FileName + "\"\r\n"; output += "Content-Type: " + FileType + " \r\n\r\n";
                output += Value + "\r\n";
                return output;
            }
        }
    }

    public class AntiCaptcha
    {
        public string captcha_id;
        public string response_str;
        public string GetText(string KapchaKey, string patch, int delay)
        {
            try
            {

                //request
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create("http://antigate.com/in.php");
                myReq.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.8.1.15) Gecko/20080623 Firefox/2.0.0.15 WebMoney Advisor";
                myReq.Accept = "*/*";
                myReq.Headers.Add("Accept-Language", "ru");
                myReq.KeepAlive = true;
                myReq.AllowAutoRedirect = false;
                myReq.Method = "POST";

                //POST paramters
                string sBoundary = DateTime.Now.Ticks.ToString("x");
                myReq.ContentType = "multipart/form-data; boundary=" + sBoundary;
                string sPostMultiString = "";
                sPostMultiString += PostData5.MultiFormData("method", "post", sBoundary);
                sPostMultiString += PostData5.MultiFormData("key", KapchaKey, sBoundary);
                //sPostMultiString += PostData5.MultiFormData("numeric", "1", sBoundary);
                //sPostMultiString += PostData5.MultiFormData("regsense", "0", sBoundary);
                string sFileContent = "";

                Stream fStream = null;
                fStream = File.OpenRead(patch);
                int nread;
                try
                {
                    byte[] buffer = new byte[4096];
                    while ((nread = fStream.Read(buffer, 0, 4096)) != 0)
                        sFileContent += Encoding.Default.GetString(buffer);
                    fStream.Close();
                }
                catch (Exception exc)
                {
                  //  MessageBox.Show(exc.Message);
                }



                sPostMultiString += PostData5.MultiFormDataFile("file", sFileContent, Path.GetFileName(patch), "image/pjpeg", sBoundary);
                sPostMultiString += "--" + sBoundary + "--\r\n\r\n";
                byte[] byteArray = Encoding.Default.GetBytes(sPostMultiString);
                myReq.ContentLength = byteArray.Length;
                myReq.GetRequestStream().Write(byteArray, 0, byteArray.Length);
                string pg = "";
                string s = "";
                try
                {
                    HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();

                    StreamReader reader = new StreamReader(myResp.GetResponseStream(), Encoding.GetEncoding(1251));

                    pg = reader.ReadToEnd();
                    string[] pars = pg.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    captcha_id = pars[1];
                    s = pars[1];
                }
                catch (Exception exc)
                {
                    if (pg == "ERROR_NO_SLOT_AVAILABLE")
                        return "ERROR_NO_SLOT_AVAILABLE";
                }

                //add Error handling here =)

                //Waiting for captcha recogniton
                try
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Thread.Sleep(delay);

                        HttpWebRequest myReq2 = (HttpWebRequest)HttpWebRequest.Create("http://antigate.com/res.php?key=" + KapchaKey + "&action=get&id=" + s);

                        myReq2.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.8.1.15) Gecko/20080623 Firefox/2.0.0.15 WebMoney Advisor";
                        myReq2.Accept = "*/*";
                        myReq2.Headers.Add("Accept-Language", "ru");
                        myReq2.KeepAlive = true;
                        myReq2.AllowAutoRedirect = false;
                        myReq2.Method = "GET";

                        HttpWebResponse ret2 = (HttpWebResponse)myReq2.GetResponse();

                        StreamReader reader = new System.IO.StreamReader(ret2.GetResponseStream(), Encoding.GetEncoding(1251));
                        pg = reader.ReadToEnd();
                        response_str = pg;
                        myReq2.Abort();
                        reader.Close();
                        ret2.Close();
                        myReq2 = null;
                        ret2 = null;
                        if (pg != "CAPCHA_NOT_READY")
                        {
                            string[] pars = pg.Split('|');

                            if (pars[0] == "OK")
                            {
                                return pars[1];
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    return exc.Message;
                }
                return "000000";
            }
            catch { return "000000"; }
        }
        public string FalseCaptcha(string KapchaKey)
        {
            string pg = "";
            try
            {
                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create("http://antigate.com/res.php?key=" + KapchaKey + "&action=reportbad&id=" + captcha_id);
                myReq.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.8.1.15) Gecko/20080623 Firefox/2.0.0.15 WebMoney Advisor";
                myReq.Accept = "*/*";
                myReq.Headers.Add("Accept-Language", "ru");
                myReq.KeepAlive = true;
                myReq.AllowAutoRedirect = false;
                myReq.Method = "GET";

                HttpWebResponse ret = (HttpWebResponse)myReq.GetResponse();

                StreamReader reader = new StreamReader(ret.GetResponseStream(), Encoding.GetEncoding(1251));
                pg = reader.ReadToEnd();
                myReq.Abort();
                reader.Close();
                ret.Close();

            }
            catch
            {
                FalseCaptcha(KapchaKey);
            }
            return pg;
        }
    }

}
