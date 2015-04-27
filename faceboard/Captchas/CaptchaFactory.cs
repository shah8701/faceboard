using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CaptchaClient;
using System.IO;
using BotGuruz.Utils;
using Captchas.Anigate;
using BaseLib;

namespace Captchas
{
   
   public abstract class CaptchaFactory
        {
            public abstract Captcha resolveRecaptcha(byte[] imageBytes, string Username, string Password);
        }

    public class DBCService : CaptchaFactory
    {

        public override Captcha resolveRecaptcha(byte[] imageBytes, string Username, string Password)
        {
            string[] args = new string[] { Username, Password, "" };
            Captcha objCaptcha = new Captcha();
            DeathByCaptcha.Client client = (DeathByCaptcha.Client)new DeathByCaptcha.SocketClient(Username, Password);
            try
            {
               
                client.Verbose = true;

                Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

                for (int i = 2, l = args.Length; i < l; i++)
                {
                    try
                    {
                        Console.WriteLine("Solving CAPTCHA {0}", args[i]);

                        DeathByCaptcha.Captcha captcha = client.Decode(imageBytes, 2 * DeathByCaptcha.Client.DefaultTimeout);
                        if (null != captcha)
                        {
                            Console.WriteLine("CAPTCHA {0:D} solved: {1}", captcha.Id, captcha.Text);

                            objCaptcha.CaptchaImage = captcha.Text;
                            return objCaptcha;
                        }
                        else
                        {

                            Console.WriteLine("CAPTCHA was not solved");
                            return objCaptcha;
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
            return objCaptcha;
        }
        
    }

    public class imagetyperzService : CaptchaFactory
    {
        public override Captcha resolveRecaptcha(byte[] imageBytes, string Username, string Password)
        {
            Captcha objCaptcha = new Captcha();
            try
            {
                
                API.CAPTCHA.CAPTCHA objAPICaptcha = new API.CAPTCHA.CAPTCHA();

                string Captcha = objAPICaptcha.UploadCaptchaAndGetText(Username, Password, imageBytes);
                objCaptcha.CaptchaImage = Captcha;
              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            return objCaptcha;
        }
    }

    public class ByPassCaptchaService : CaptchaFactory
    {
        public override Captcha resolveRecaptcha(byte[] imageBytes, string Username, string Password)
        {
            throw new NotImplementedException(); //Amit to code this part

            #region Code not Completed
            /// code for convert bit[] to Image
              byte[] buffer = imageBytes.ToArray();
              MemoryStream memStream = new MemoryStream();
              memStream.Write(buffer, 0, buffer.Length);
              Image captcha= Image.FromStream(memStream);            
         //    Image captcha = Image.FromFile("Captcha.jpg");
             CaptchaSolver solver = new CaptchaSolver("my-login", "my-access-key");
           //  solver.Initialize();
            // SolveResult result = solver.SolveCaptcha(captcha);
             //string captchaStr = result.Result;
             //captcha.Dispose();
            //solver.Dispose(); 
            #endregion
        }
    
    }

    public class DecaptcherService : CaptchaFactory
    {
        public override Captcha resolveRecaptcha(byte[] imageBytes, string Username, string Password)
        {
             Captcha objCaptcha = new Captcha();
             try
             {
                 string DecaptcherHost = "Decaptcha.com";
                 int DecaptchaPort = 0;


                 /// code for convert bit[] to Image
                 byte[] buffer = imageBytes.ToArray();
                 MemoryStream memStream = new MemoryStream();
                 memStream.Write(buffer, 0, buffer.Length);
                 Image captcha = Image.FromStream(memStream);



                 Decaptcher objDecaptcher = new Decaptcher();
                // string CaptchaImage = objDecaptcher.decaptcha(imageBytes, DecaptcherHost, DecaptchaPort, Username, Password);
             }
             catch (Exception ex)
             {
                 GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
             }
            return objCaptcha;
        }
    }

    public class AnigateService : CaptchaFactory
    {

        public override Captcha resolveRecaptcha(byte[] imageBytes, string AccountKey, string ImagePath)
        {
            Captcha objCaptcha = new Captcha();
            try
            {
                ClsAnigates objAnigate = new ClsAnigates();

                string Captcha = objAnigate.GetCaptcha(AccountKey, ImagePath);
                objCaptcha.CaptchaImage = Captcha;
            }
            catch { };
            return objCaptcha;
        }
    }

}
