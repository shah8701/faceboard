using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace faceboardpro.Domain
{
    class Captcha 
    {

        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string userName;

        public string Username
        {
            get { return userName; }
            set { userName = value; }
        }


        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string captchaService;

        public string CaptchaService
        {
            get { return captchaService; }
            set { captchaService = value; }
        }

        private string status;

        public string Status
        {
            get { return status ; }
            set { status = value; }
        }

    }
}
