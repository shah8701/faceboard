using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Captchas
{
    public class Captcha
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CaptchaImage { get; set; }
        public string CaptchaService { get; set; }
        public string Status { get; set; }
    }

    public enum CaptchaServiceType
    {
        Bypasscaptcha,Beatcaptchas,Decaptcher,DBC,Imagetyperz,Captchasniper,ExpertDecoders,CaptchaInfinity,CaptchaTraders,Antigate,Captchabot,shanibot,Ninekw,CaptchaBreaker,ShaniBPO
    }
}
